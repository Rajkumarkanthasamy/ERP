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

export function createGRN({ poRef, vendorCode, projectCode, remarks, lines, user }) {
  if (!lines?.length) throw new Error('No lines to receive');
  const db = getDb();
  const grnNumber = nextGrnNumber();

  const tx = db.transaction(() => {
    const info = db
      .prepare(
        `INSERT INTO procurement_grn
          (grn_number, po_ref, vendor_code, project_code, received_by, remarks, created_by, status)
         VALUES (?, ?, ?, ?, ?, ?, ?, 'Received')`
      )
      .run(
        grnNumber,
        poRef || null,
        vendorCode || null,
        projectCode || null,
        user.displayName || user.username,
        remarks || null,
        user.username
      );

    const insertDetail = db.prepare(`
      INSERT INTO procurement_grn_details
        (grn_id, grn_number, po_id, item_code, ordered_qty, received_qty, unit_price, amount, uom, remarks)
      VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    `);

    for (const line of lines) {
      const recvQty = Number(line.receivedQty || 0);
      if (recvQty <= 0) continue;
      const po = db.prepare('SELECT * FROM purchase_orders WHERE id = ?').get(line.poId);
      if (!po) throw new Error(`PO line ${line.poId} not found`);
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
      details: `PO ${poRef || ''}`,
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
              status, remarks
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
              status, remarks
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
