import { getDb } from '../db/connection.js';
import {
  PR_LINE_STATUS,
  PR_STATUS,
  computePoAmounts,
  computePoStatus,
  calculatePriceVariance,
  getNextPoApprovalStep,
  PO_STATUS_OPTIONS,
} from '../constants.js';
import { applyGst, logActivity } from './helpers.js';

function groupByPoRef(rows) {
  const map = new Map();
  for (const row of rows) {
    const key = row.po_ref;
    if (!map.has(key)) map.set(key, []);
    map.get(key).push(row);
  }
  return [...map.entries()].map(([poRef, lines]) => {
    const amounts = computePoAmounts(lines);
    const status = computePoStatus({ lines });
    const approval = {
      pmApproved: !!lines[0].pm_approved,
      mhApproved: !!lines[0].mh_approved,
      pcApproved: !!lines[0].pc_approved,
      omApproved: !!lines[0].om_approved,
      gmApproved: !!lines[0].gm_approved,
    };
    return {
      poRef,
      status,
      ...amounts,
      vendorCode: lines[0].vendor_code,
      vendorName: lines[0].vendor_name,
      projectCode: lines[0].project_code,
      prNumber: lines[0].pr_number,
      preparedBy: lines[0].prepared_by,
      preparedDate: lines[0].prepared_date,
      poApproved: !!lines[0].po_approved,
      poGeneratedBy: lines[0].po_generated_by,
      poGeneratedDate: lines[0].po_generated_date,
      ...approval,
      nextStep: getNextPoApprovalStep({
        ...approval,
        approvalTier: amounts.approvalTier,
      }),
      lineCount: lines.length,
      lines: lines.map(mapLine),
    };
  });
}

function mapLine(row) {
  return {
    id: row.id,
    poRef: row.po_ref,
    prId: row.pr_id,
    prNumber: row.pr_number,
    detailId: row.detail_id,
    projectCode: row.project_code,
    vendorCode: row.vendor_code,
    vendorName: row.vendor_name,
    itemCode: row.item_code,
    itemDescription: row.item_description,
    specification: row.specification,
    make: row.make,
    mfgPartNo: row.mfg_part_no,
    requiredQty: row.required_qty,
    remainingQty: row.remaining_qty,
    uom: row.uom,
    unitPrice: row.unit_price,
    amount: row.amount,
    igstRate: row.igst_rate,
    igstAmount: row.igst_amount,
    sgstRate: row.sgst_rate,
    sgstAmount: row.sgst_amount,
    cgstRate: row.cgst_rate,
    cgstAmount: row.cgst_amount,
    preparedBy: row.prepared_by,
    preparedDate: row.prepared_date,
    poApproved: !!row.po_approved,
    poGeneratedBy: row.po_generated_by,
    poGeneratedDate: row.po_generated_date,
    poSentToVendorBy: row.po_sent_to_vendor_by,
    poSentVendorDate: row.po_sent_vendor_date,
    pmName: row.pm_name,
    pmApproved: !!row.pm_approved,
    pmApprovedDate: row.pm_approved_date,
    mhName: row.mh_name,
    mhApproved: !!row.mh_approved,
    mhApprovedDate: row.mh_approved_date,
    pcName: row.pc_name,
    pcApproved: !!row.pc_approved,
    pcApprovedDate: row.pc_approved_date,
    pcRemarks: row.pc_remarks,
    omName: row.om_name,
    omApproved: !!row.om_approved,
    omApprovedDate: row.om_approved_date,
    omRemarks: row.om_remarks,
    gmName: row.gm_name,
    gmApproved: !!row.gm_approved,
    gmApprovedDate: row.gm_approved_date,
    finalizedBy: row.finalized_by,
    finalizedDate: row.finalized_date,
    finalStatus: row.final_status,
    rejectReason: row.reject_reason,
    cancelledBy: row.cancelled_by,
    closedBy: row.closed_by,
    remarks: row.remarks,
  };
}

function nextPoRef() {
  const db = getDb();
  const now = new Date();
  const prefix = `PO-${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-`;
  const row = db
    .prepare(`SELECT po_ref FROM purchase_orders WHERE po_ref LIKE ? ORDER BY po_ref DESC LIMIT 1`)
    .get(`${prefix}%`);
  let n = 1;
  if (row?.po_ref) n = (parseInt(row.po_ref.split('-').pop(), 10) || 0) + 1;
  return `${prefix}${String(n).padStart(5, '0')}`;
}

export function listApprovedPRsForPO() {
  const db = getDb();
  return db
    .prepare(
      `SELECT pr.*,
        (SELECT COUNT(*) FROM purchase_request_details d
          WHERE d.pr_number = pr.pr_number AND IFNULL(d.line_status,'Pending') = 'Pending') AS pending_lines
       FROM purchase_requests pr
       WHERE pr.status IN (?, ?)
         AND IFNULL(pr.is_clubbed, 0) = 0
         AND EXISTS (
           SELECT 1 FROM purchase_request_details d
           WHERE d.pr_number = pr.pr_number AND IFNULL(d.line_status,'Pending') = 'Pending'
         )
       ORDER BY pr.request_date ASC`
    )
    .all(PR_STATUS.APPROVED, PR_STATUS.PARTIALLY_CONVERTED)
    .map((r) => ({
      id: r.id,
      prNumber: r.pr_number,
      projectCode: r.project_code,
      vendorCode: r.vendor_code,
      vendorName: r.vendor_name,
      totalAmount: r.total_amount,
      status: r.status,
      pendingLines: r.pending_lines,
      requestDate: r.request_date,
    }));
}

export function convertPRsToPO({ prNumbers, lineIds, gstMode = 'cgst_sgst', user }) {
  if (!prNumbers?.length) throw new Error('Select at least one PR');
  const db = getDb();
  const createdRefs = [];

  const tx = db.transaction(() => {
    // Collect pending lines
    const placeholders = prNumbers.map(() => '?').join(',');
    let lines = db
      .prepare(
        `SELECT d.*, pr.id AS header_id, pr.vendor_code AS hdr_vendor, pr.vendor_name AS hdr_vendor_name,
                pr.project_code AS hdr_project
         FROM purchase_request_details d
         JOIN purchase_requests pr ON pr.pr_number = d.pr_number
         WHERE d.pr_number IN (${placeholders})
           AND IFNULL(d.line_status,'Pending') = 'Pending'`
      )
      .all(...prNumbers);

    if (lineIds?.length) {
      const set = new Set(lineIds);
      lines = lines.filter((l) => set.has(l.id));
    }
    if (!lines.length) throw new Error('No pending lines to convert');

    // Group by vendor
    const byVendor = new Map();
    for (const line of lines) {
      const key = line.vendor_code || line.hdr_vendor;
      if (!byVendor.has(key)) byVendor.set(key, []);
      byVendor.get(key).push(line);
    }

    const insert = db.prepare(`
      INSERT INTO purchase_orders (
        po_ref, pr_id, pr_number, detail_id, project_code, vendor_code, vendor_name,
        item_code, item_description, specification, make, mfg_part_no,
        required_qty, remaining_qty, uom, unit_price, amount,
        igst_rate, igst_amount, sgst_rate, sgst_amount, cgst_rate, cgst_amount,
        prepared_by, prepared_date, remarks
      ) VALUES (
        ?, ?, ?, ?, ?, ?, ?,
        ?, ?, ?, ?, ?,
        ?, ?, ?, ?, ?,
        ?, ?, ?, ?, ?, ?,
        ?, datetime('now'), ?
      )
    `);

    for (const [, vendorLines] of byVendor) {
      const poRef = nextPoRef();
      createdRefs.push(poRef);
      for (const line of vendorLines) {
        const amount = Number(line.total_cost) || Number(line.quantity) * Number(line.unit_cost);
        const gst =
          gstMode === 'igst'
            ? applyGst(amount, { igst: 18 })
            : applyGst(amount, { sgst: 9, cgst: 9 });
        insert.run(
          poRef,
          line.header_id,
          line.pr_number,
          line.id,
          line.project_code || line.hdr_project,
          line.vendor_code || line.hdr_vendor,
          line.vendor_name || line.hdr_vendor_name,
          line.item_code,
          line.item_description,
          line.specification,
          line.make,
          line.mfg_part_no,
          line.quantity,
          line.quantity,
          line.uom,
          line.unit_cost,
          amount,
          gst.igst_rate,
          gst.igst_amount,
          gst.sgst_rate,
          gst.sgst_amount,
          gst.cgst_rate,
          gst.cgst_amount,
          user.displayName || user.username,
          line.remarks
        );
        db.prepare(
          `UPDATE purchase_request_details SET line_status = ? WHERE id = ?`
        ).run(PR_LINE_STATUS.CONVERTED_TO_PO, line.id);
      }

      // Update PR conversion status
      for (const prNumber of new Set(vendorLines.map((l) => l.pr_number))) {
        const pending = db
          .prepare(
            `SELECT COUNT(*) AS c FROM purchase_request_details
             WHERE pr_number = ? AND IFNULL(line_status,'Pending') = 'Pending'`
          )
          .get(prNumber).c;
        const status = pending > 0 ? PR_STATUS.PARTIALLY_CONVERTED : PR_STATUS.FULLY_CONVERTED;
        db.prepare(
          `UPDATE purchase_requests SET status = ?, modified_by = ?, modified_at = datetime('now') WHERE pr_number = ?`
        ).run(status, user.username, prNumber);
      }

      logActivity({
        entityType: 'PO',
        entityRef: poRef,
        action: 'Created',
        details: `From PRs: ${[...new Set(vendorLines.map((l) => l.pr_number))].join(', ')}`,
        byUser: user.username,
      });
    }
  });

  tx();
  return { poRefs: createdRefs };
}

export function listPOsForApproval() {
  const db = getDb();
  const rows = db
    .prepare(
      `SELECT * FROM purchase_orders
       WHERE IFNULL(po_approved, 0) = 0
         AND IFNULL(final_status, '') <> 'Rejected'
         AND cancelled_by IS NULL
         AND closed_by IS NULL
       ORDER BY po_ref, id`
    )
    .all();
  return groupByPoRef(rows);
}

export function approvePO(poRef, { step, action, remarks, user }) {
  const db = getDb();
  const lines = db.prepare('SELECT * FROM purchase_orders WHERE po_ref = ?').all(poRef);
  if (!lines.length) throw new Error('PO not found');

  const amounts = computePoAmounts(lines);
  const first = lines[0];
  const approvalStep =
    step ||
    (action === 'reject'
      ? 'reject'
      : action === 'approve'
        ? getNextPoApprovalStep({
            pmApproved: !!first.pm_approved,
            mhApproved: !!first.mh_approved,
            pcApproved: !!first.pc_approved,
            omApproved: !!first.om_approved,
            gmApproved: !!first.gm_approved,
            approvalTier: amounts.approvalTier,
          })
        : null);
  if (!approvalStep) throw new Error('PO has no pending approval step');
  if (first.po_approved && !['generate', 'send'].includes(approvalStep)) {
    throw new Error('PO already fully approved');
  }

  const tx = db.transaction(() => {
    if (approvalStep === 'reject') {
      db.prepare(
        `UPDATE purchase_orders SET reject_reason = ?, final_status = 'Rejected', modified_at = datetime('now')
         WHERE po_ref = ?`
      ).run(remarks || 'Rejected', poRef);
      logActivity({ entityType: 'PO', entityRef: poRef, action: 'Rejected', details: remarks, byUser: user.username });
      return;
    }

    if (approvalStep === 'pm') {
      if (first.pm_approved) throw new Error('PM already approved');
      db.prepare(
        `UPDATE purchase_orders SET pm_name = ?, pm_approved = 1, pm_approved_date = datetime('now'), modified_at = datetime('now')
         WHERE po_ref = ?`
      ).run(user.displayName || user.username, poRef);
    } else if (approvalStep === 'mh') {
      if (!first.pm_approved) throw new Error('PM approval required first');
      if (first.mh_approved) throw new Error('MH already approved');
      db.prepare(
        `UPDATE purchase_orders SET mh_name = ?, mh_approved = 1, mh_approved_date = datetime('now'), modified_at = datetime('now')
         WHERE po_ref = ?`
      ).run(user.displayName || user.username, poRef);
    } else if (approvalStep === 'pc') {
      if (!first.pm_approved || !first.mh_approved) throw new Error('PM and MH approval required first');
      if (first.pc_approved) throw new Error('PC already approved');
      const finalize = amounts.approvalTier === 'PC';
      db.prepare(
        `UPDATE purchase_orders SET
          pc_name = ?, pc_approved = 1, pc_approved_date = datetime('now'), pc_remarks = ?,
          po_approved = CASE WHEN ? THEN 1 ELSE po_approved END,
          finalized_by = CASE WHEN ? THEN ? ELSE finalized_by END,
          finalized_date = CASE WHEN ? THEN datetime('now') ELSE finalized_date END,
          final_status = CASE WHEN ? THEN 'Approved' ELSE final_status END,
          modified_at = datetime('now')
         WHERE po_ref = ?`
      ).run(
        user.displayName || user.username,
        remarks || null,
        finalize ? 1 : 0,
        finalize ? 1 : 0,
        user.displayName || user.username,
        finalize ? 1 : 0,
        finalize ? 1 : 0,
        poRef
      );
    } else if (approvalStep === 'om') {
      if (!first.pc_approved) throw new Error('PC approval required first');
      if (amounts.approvalTier === 'PC') throw new Error('OM not required for this PO amount');
      if (first.om_approved) throw new Error('OM already approved');
      const finalize = amounts.approvalTier === 'OM';
      db.prepare(
        `UPDATE purchase_orders SET
          om_name = ?, om_approved = 1, om_approved_date = datetime('now'), om_remarks = ?,
          po_approved = CASE WHEN ? THEN 1 ELSE po_approved END,
          finalized_by = CASE WHEN ? THEN ? ELSE finalized_by END,
          finalized_date = CASE WHEN ? THEN datetime('now') ELSE finalized_date END,
          final_status = CASE WHEN ? THEN 'Approved' ELSE final_status END,
          modified_at = datetime('now')
         WHERE po_ref = ?`
      ).run(
        user.displayName || user.username,
        remarks || null,
        finalize ? 1 : 0,
        finalize ? 1 : 0,
        user.displayName || user.username,
        finalize ? 1 : 0,
        finalize ? 1 : 0,
        poRef
      );
    } else if (approvalStep === 'gm') {
      if (amounts.approvalTier !== 'GM') throw new Error('GM not required for this PO amount');
      if (!first.om_approved) throw new Error('OM approval required first');
      db.prepare(
        `UPDATE purchase_orders SET
          gm_name = ?, gm_approved = 1, gm_approved_date = datetime('now'),
          po_approved = 1, finalized_by = ?, finalized_date = datetime('now'),
          final_status = 'Approved', modified_at = datetime('now')
         WHERE po_ref = ?`
      ).run(user.displayName || user.username, user.displayName || user.username, poRef);
    } else if (approvalStep === 'generate') {
      if (!first.po_approved) throw new Error('PO must be fully approved before generate');
      if (first.po_generated_by) throw new Error('PO is already generated');
      db.prepare(
        `UPDATE purchase_orders SET po_generated_by = ?, po_generated_date = datetime('now'), modified_at = datetime('now')
         WHERE po_ref = ?`
      ).run(user.displayName || user.username, poRef);
    } else if (approvalStep === 'send') {
      if (!first.po_generated_by) throw new Error('Generate PO first');
      if (first.po_sent_to_vendor_by) throw new Error('PO is already sent to the vendor');
      db.prepare(
        `UPDATE purchase_orders SET po_sent_to_vendor_by = ?, po_sent_vendor_date = datetime('now'), modified_at = datetime('now')
         WHERE po_ref = ?`
      ).run(user.displayName || user.username, poRef);
    } else {
      throw new Error('Unknown approval step');
    }

    logActivity({
      entityType: 'PO',
      entityRef: poRef,
      action: approvalStep,
      details: remarks || amounts.approvalTier,
      byUser: user.username,
    });
  });

  tx();
  return getPO(poRef);
}

export function getPO(poRef) {
  const db = getDb();
  const rows = db.prepare('SELECT * FROM purchase_orders WHERE po_ref = ? ORDER BY id').all(poRef);
  if (!rows.length) return null;
  return groupByPoRef(rows)[0];
}

export function listPOStatus({ status, poNumber, vendor, project, q, from, to } = {}) {
  const db = getDb();
  const where = ['1=1'];
  const params = [];
  if (poNumber) {
    where.push('po_ref LIKE ?');
    params.push(`%${poNumber}%`);
  }
  if (vendor) {
    where.push('(vendor_code LIKE ? OR vendor_name LIKE ?)');
    params.push(`%${vendor}%`, `%${vendor}%`);
  }
  if (project) {
    where.push('project_code LIKE ?');
    params.push(`%${project}%`);
  }
  if (q) {
    where.push(
      `(po_ref LIKE ? OR vendor_code LIKE ? OR IFNULL(vendor_name, '') LIKE ? OR project_code LIKE ?)`
    );
    params.push(`%${q}%`, `%${q}%`, `%${q}%`, `%${q}%`);
  }
  if (from) {
    where.push("date(prepared_date) >= date(?)");
    params.push(from);
  }
  if (to) {
    where.push("date(prepared_date) <= date(?)");
    params.push(to);
  }

  const rows = db
    .prepare(`SELECT * FROM purchase_orders WHERE ${where.join(' AND ')} ORDER BY po_ref DESC, id`)
    .all(...params);
  let groups = groupByPoRef(rows);
  if (status && status !== 'All') {
    groups = groups.filter((g) => g.status === status);
  }
  return { statusOptions: PO_STATUS_OPTIONS, items: groups };
}

export function priceVariance(prNumber) {
  const db = getDb();
  const lines = db
    .prepare('SELECT * FROM purchase_request_details WHERE pr_number = ?')
    .all(prNumber);
  if (!lines.length) throw new Error('PR not found or has no lines');

  return lines.map((line) => {
    const item = db.prepare('SELECT * FROM items WHERE item_code = ?').get(line.item_code);
    const lastPo = db
      .prepare(
        `SELECT unit_price, po_ref, prepared_date FROM purchase_orders
         WHERE item_code = ? ORDER BY id DESC LIMIT 1`
      )
      .get(line.item_code);

    const baseline = Number(lastPo?.unit_price ?? item?.latest_purchase_price ?? item?.standard_cost ?? 0);
    const current = Number(line.unit_cost || 0);
    const variance = calculatePriceVariance(current, baseline);

    return {
      itemCode: line.item_code,
      itemDescription: line.item_description,
      prUnitCost: current,
      baselinePrice: baseline,
      lastPoRef: lastPo?.po_ref || null,
      lastPoDate: lastPo?.prepared_date || null,
      variancePct: variance.variancePct,
      varianceAmount: variance.varianceAmount,
      flag: variance.flag,
      standardCost: item?.standard_cost ?? null,
    };
  });
}
