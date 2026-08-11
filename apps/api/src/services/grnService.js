import { getDb } from '../db/connection.js';
import { logActivity } from './helpers.js';

export function listOpenPOLines() {
  const db = getDb();
  return db
    .prepare(
      `SELECT id, po_ref AS poRef, project_code AS projectCode, vendor_code AS vendorCode,
              vendor_name AS vendorName, item_code AS itemCode, item_description AS itemDescription,
              uom, required_qty AS orderedQty, IFNULL(remaining_qty, required_qty) AS remainingQty,
              unit_price AS unitPrice, amount, po_generated_by AS poGeneratedBy, po_generated_date AS poGeneratedDate
       FROM purchase_orders
       WHERE po_approved = 1
         AND IFNULL(remaining_qty, required_qty) > 0
       ORDER BY id DESC`
    )
    .all();
}

function nextGrnNumber() {
  const db = getDb();
  const now = new Date();
  const prefix = `GRN-${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-`;
  const row = db
    .prepare(`SELECT grn_number FROM procurement_grn WHERE grn_number LIKE ? ORDER BY grn_number DESC LIMIT 1`)
    .get(`${prefix}%`);
  let n = 1;
  if (row?.grn_number) n = (parseInt(row.grn_number.split('-').pop(), 10) || 0) + 1;
  return `${prefix}${String(n).padStart(5, '0')}`;
}

export function createGRN({
  poRef,
  vendorCode,
  projectCode,
  invoiceNo,
  remarks,
  lines,
  user,
}) {
  if (!lines?.length) throw new Error('No lines to receive');
  const db = getDb();
  const normalizedLines = lines
    .map((line) => ({
      ...line,
      poId: line.poId ?? line.detailId ?? line.id,
      receivedQty: Number(line.receivedQty ?? line.quantity ?? 0),
    }))
    .filter((line) => line.receivedQty > 0);
  if (!normalizedLines.length) throw new Error('At least one positive receipt quantity is required');
  if (normalizedLines.some((line) => !line.poId)) {
    throw new Error('Every GRN line requires a PO line identifier');
  }
  const firstPo = db
    .prepare('SELECT * FROM purchase_orders WHERE id = ?')
    .get(normalizedLines[0].poId);
  if (!firstPo) throw new Error(`PO line ${normalizedLines[0].poId} not found`);
  const resolvedPoRef = poRef || firstPo.po_ref;
  const resolvedVendorCode = vendorCode || firstPo.vendor_code;
  const resolvedProjectCode = projectCode || firstPo.project_code;
  const grnNumber = nextGrnNumber();

  const tx = db.transaction(() => {
    const info = db
      .prepare(
        `INSERT INTO procurement_grn
          (grn_number, po_ref, vendor_code, project_code, invoice_no, received_by, remarks, created_by, status)
         VALUES (?, ?, ?, ?, ?, ?, ?, ?, 'Received')`
      )
      .run(
        grnNumber,
        resolvedPoRef,
        resolvedVendorCode || null,
        resolvedProjectCode || null,
        invoiceNo || null,
        user.displayName || user.username,
        remarks || null,
        user.username
      );

    const insertDetail = db.prepare(`
      INSERT INTO procurement_grn_details
        (grn_id, grn_number, po_id, item_code, ordered_qty, received_qty, unit_price, amount, uom, remarks)
      VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    `);

    for (const line of normalizedLines) {
      const recvQty = line.receivedQty;
      const po = db.prepare('SELECT * FROM purchase_orders WHERE id = ?').get(line.poId);
      if (!po) throw new Error(`PO line ${line.poId} not found`);
      if (po.po_ref !== resolvedPoRef) {
        throw new Error('A GRN can only receive lines from one purchase order');
      }
      const remaining = Number(po.remaining_qty ?? po.required_qty);
      if (recvQty > remaining) throw new Error(`Received qty exceeds remaining for ${po.item_code}`);

      insertDetail.run(
        info.lastInsertRowid,
        grnNumber,
        po.id,
        po.item_code,
        po.required_qty,
        recvQty,
        po.unit_price,
        +(recvQty * po.unit_price).toFixed(2),
        po.uom,
        line.remarks || null
      );

      db.prepare(
        `UPDATE purchase_orders
         SET remaining_qty = CASE WHEN IFNULL(remaining_qty, required_qty) - ? < 0 THEN 0
                                  ELSE IFNULL(remaining_qty, required_qty) - ? END,
             modified_at = datetime('now')
         WHERE id = ?`
      ).run(recvQty, recvQty, po.id);

      // Update item latest purchase price
      db.prepare('UPDATE items SET latest_purchase_price = ? WHERE item_code = ?').run(
        po.unit_price,
        po.item_code
      );
    }

    logActivity({
      entityType: 'GRN',
      entityRef: grnNumber,
      action: 'Received',
      details: `PO ${resolvedPoRef}`,
      byUser: user.username,
    });
  });

  tx();
  return getGRN(grnNumber);
}

export function listGRNs() {
  const db = getDb();
  return db
    .prepare(
      `SELECT id, grn_number AS grnNumber, po_ref AS poRef, vendor_code AS vendorCode,
              project_code AS projectCode, received_by AS receivedBy, received_date AS receivedDate,
              invoice_no AS invoiceNo, status, remarks,
              (SELECT IFNULL(SUM(d.amount), 0) FROM procurement_grn_details d
               WHERE d.grn_number = procurement_grn.grn_number) AS totalAmount
       FROM procurement_grn ORDER BY id DESC`
    )
    .all();
}

export function getGRN(grnNumber) {
  const db = getDb();
  const header = db
    .prepare(
      `SELECT id, grn_number AS grnNumber, po_ref AS poRef, vendor_code AS vendorCode,
              project_code AS projectCode, received_by AS receivedBy, received_date AS receivedDate,
              invoice_no AS invoiceNo, status, remarks
       FROM procurement_grn WHERE grn_number = ?`
    )
    .get(grnNumber);
  if (!header) return null;
  const details = db
    .prepare(
      `SELECT id, po_id AS poId, item_code AS itemCode, ordered_qty AS orderedQty,
              received_qty AS receivedQty, unit_price AS unitPrice, amount, uom, remarks
       FROM procurement_grn_details WHERE grn_number = ?`
    )
    .all(grnNumber);
  return { ...header, lines: details };
}
