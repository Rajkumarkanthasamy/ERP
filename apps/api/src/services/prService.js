import { getDb } from '../db/connection.js';
import { PR_LIMIT, PR_LINE_STATUS, PR_STATUS } from '../constants.js';
import { logActivity } from './helpers.js';

function mapPr(row) {
  if (!row) return null;
  return {
    id: row.id,
    prNumber: row.pr_number,
    projectCode: row.project_code,
    productNo: row.product_no,
    vendorCode: row.vendor_code,
    vendorName: row.vendor_name,
    totalAmount: row.total_amount,
    status: row.status,
    requestedBy: row.requested_by,
    requestDate: row.request_date,
    approvedBy: row.approved_by,
    approvedDate: row.approved_date,
    rejectionReason: row.rejection_reason,
    holdReason: row.hold_reason,
    remarks: row.remarks,
    isClubbed: !!row.is_clubbed,
    clubbedFromPrIds: row.clubbed_from_pr_ids,
    createdBy: row.created_by,
    modifiedBy: row.modified_by,
    modifiedAt: row.modified_at,
    ageDays: row.age_days,
    itemCount: row.item_count,
  };
}

function mapLine(row) {
  return {
    id: row.id,
    prId: row.pr_id,
    prNumber: row.pr_number,
    projectCode: row.project_code,
    productNo: row.product_no,
    itemCode: row.item_code,
    itemDescription: row.item_description,
    specification: row.specification,
    make: row.make,
    mfgPartNo: row.mfg_part_no,
    quantity: row.quantity,
    uom: row.uom,
    unitCost: row.unit_cost,
    totalCost: row.total_cost,
    vendorCode: row.vendor_code,
    vendorName: row.vendor_name,
    bomCode: row.bom_code,
    hsnCode: row.hsn_code,
    lineStatus: row.line_status,
    remarks: row.remarks,
  };
}

export function listPRs({ status, vendor, project, q } = {}) {
  const db = getDb();
  const where = ['1=1'];
  const params = [];
  if (status && status !== 'All') {
    where.push('pr.status = ?');
    params.push(status);
  }
  if (vendor) {
    where.push('(pr.vendor_code LIKE ? OR pr.vendor_name LIKE ?)');
    params.push(`%${vendor}%`, `%${vendor}%`);
  }
  if (project) {
    where.push('pr.project_code LIKE ?');
    params.push(`%${project}%`);
  }
  if (q) {
    where.push('(pr.pr_number LIKE ? OR pr.remarks LIKE ?)');
    params.push(`%${q}%`, `%${q}%`);
  }
  const rows = db
    .prepare(
      `SELECT pr.*,
        CAST((julianday('now') - julianday(pr.request_date)) AS INTEGER) AS age_days,
        (SELECT COUNT(*) FROM purchase_request_details d WHERE d.pr_number = pr.pr_number) AS item_count
       FROM purchase_requests pr
       WHERE ${where.join(' AND ')}
       ORDER BY pr.request_date DESC, pr.id DESC`
    )
    .all(...params);
  return rows.map(mapPr);
}

export function getPR(prNumber) {
  const db = getDb();
  const header = db
    .prepare(
      `SELECT pr.*,
        CAST((julianday('now') - julianday(pr.request_date)) AS INTEGER) AS age_days,
        (SELECT COUNT(*) FROM purchase_request_details d WHERE d.pr_number = pr.pr_number) AS item_count
       FROM purchase_requests pr WHERE pr.pr_number = ?`
    )
    .get(prNumber);
  if (!header) return null;
  const lines = db
    .prepare('SELECT * FROM purchase_request_details WHERE pr_number = ? ORDER BY id')
    .all(prNumber)
    .map(mapLine);
  return { ...mapPr(header), lines };
}

function packByVendorLimit(lines) {
  // Group by vendor, then pack into buckets of ≤ PR_LIMIT
  const byVendor = new Map();
  for (const line of lines) {
    const key = line.vendorCode;
    if (!byVendor.has(key)) byVendor.set(key, []);
    byVendor.get(key).push(line);
  }

  const packs = [];
  for (const [vendorCode, vendorLines] of byVendor) {
    let current = [];
    let currentTotal = 0;
    for (const line of vendorLines) {
      const cost = Number(line.quantity) * Number(line.unitCost);
      if (current.length && currentTotal + cost > PR_LIMIT) {
        packs.push({ vendorCode, lines: current, total: currentTotal, overLimit: false });
        current = [];
        currentTotal = 0;
      }
      current.push(line);
      currentTotal += cost;
    }
    if (current.length) {
      packs.push({
        vendorCode,
        lines: current,
        total: currentTotal,
        overLimit: currentTotal > PR_LIMIT,
      });
    }
  }
  return packs;
}

export function createPRs({
  projectCode,
  productNo,
  vendorCode,
  vendorName,
  lines,
  remarks,
  user,
}) {
  if (!projectCode) throw new Error('Project code is required');
  if (!lines?.length) throw new Error('At least one line item is required');

  const db = getDb();
  const normalizedLines = lines.map((line) => ({
    ...line,
    vendorCode: line.vendorCode || vendorCode,
    vendorName: line.vendorName || vendorName,
  }));
  if (normalizedLines.some((line) => !line.vendorCode)) {
    throw new Error('Vendor code is required for every PR line');
  }
  if (
    normalizedLines.some(
      (line) =>
        !line.itemCode || Number(line.quantity) <= 0 || Number(line.unitCost) < 0
    )
  ) {
    throw new Error('Every PR line requires an item, positive quantity, and valid unit cost');
  }

  const packs = packByVendorLimit(normalizedLines);
  const created = [];

  const tx = db.transaction(() => {
    for (const pack of packs) {
      const vendor = db.prepare('SELECT * FROM vendors WHERE vendor_code = ?').get(pack.vendorCode);
      const now = new Date();
      const prefix = `PR-${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-`;
      const row = db
        .prepare(`SELECT pr_number FROM purchase_requests WHERE pr_number LIKE ? ORDER BY pr_number DESC LIMIT 1`)
        .get(`${prefix}%`);
      let n = 1;
      if (row?.pr_number) {
        const last = row.pr_number.split('-').pop();
        n = (parseInt(last, 10) || 0) + 1;
      }
      const actualPr = `${prefix}${String(n).padStart(7, '0')}`;

      const info = db
        .prepare(
          `INSERT INTO purchase_requests (
            pr_number, project_code, product_no, vendor_code, vendor_name, total_amount,
            status, requested_by, created_by, remarks
          ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)`
        )
        .run(
          actualPr,
          projectCode,
          productNo || null,
          pack.vendorCode,
          vendor?.vendor_name || pack.lines[0]?.vendorName || pack.vendorCode,
          pack.total,
          PR_STATUS.PENDING,
          user.displayName || user.username,
          user.username,
          remarks || (pack.overLimit ? 'Warning: single pack exceeds ₹12L limit' : null)
        );

      const insertLine = db.prepare(`
        INSERT INTO purchase_request_details (
          pr_id, pr_number, project_code, product_no, item_code, item_description, specification,
          make, mfg_part_no, quantity, uom, unit_cost, total_cost, vendor_code, vendor_name,
          bom_code, hsn_code, line_status, remarks, created_by
        ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
      `);

      for (const line of pack.lines) {
        const totalCost = Number(line.quantity) * Number(line.unitCost);
        insertLine.run(
          info.lastInsertRowid,
          actualPr,
          projectCode,
          productNo || null,
          line.itemCode,
          line.itemDescription || null,
          line.specification || null,
          line.make || null,
          line.mfgPartNo || null,
          line.quantity,
          line.uom || 'NOS',
          line.unitCost,
          totalCost,
          pack.vendorCode,
          vendor?.vendor_name || line.vendorName || null,
          line.bomCode || null,
          line.hsnCode || null,
          PR_LINE_STATUS.PENDING,
          line.remarks || null,
          user.username
        );
      }

      logActivity({
        entityType: 'PR',
        entityRef: actualPr,
        action: 'Created',
        details: `${pack.lines.length} lines, ₹${pack.total.toFixed(2)}`,
        byUser: user.username,
      });

      created.push({
        prNumber: actualPr,
        vendorCode: pack.vendorCode,
        totalAmount: pack.total,
        lineCount: pack.lines.length,
        overLimit: pack.overLimit,
      });
    }
  });

  tx();
  return { created, autoSplit: created.length > 1 };
}

export function updatePRStatus(prNumber, { action, reason, user }) {
  const db = getDb();
  const pr = db.prepare('SELECT * FROM purchase_requests WHERE pr_number = ?').get(prNumber);
  if (!pr) throw new Error('PR not found');

  let status = pr.status;
  const patch = {
    approved_by: pr.approved_by,
    approved_date: pr.approved_date,
    rejection_reason: pr.rejection_reason,
    hold_reason: pr.hold_reason,
  };

  switch (action) {
    case 'approve':
      if (![PR_STATUS.PENDING, PR_STATUS.ON_HOLD].includes(pr.status)) {
        throw new Error(`Cannot approve PR in status ${pr.status}`);
      }
      status = PR_STATUS.APPROVED;
      patch.approved_by = user.displayName || user.username;
      patch.approved_date = new Date().toISOString();
      break;
    case 'reject':
      status = PR_STATUS.REJECTED;
      patch.rejection_reason = reason || 'Rejected';
      break;
    case 'hold':
      status = PR_STATUS.ON_HOLD;
      patch.hold_reason = reason || 'On hold';
      break;
    case 'release':
      if (pr.status !== PR_STATUS.ON_HOLD) throw new Error('Only On Hold PRs can be released');
      status = PR_STATUS.PENDING;
      patch.hold_reason = null;
      break;
    default:
      throw new Error('Unknown action');
  }

  db.prepare(
    `UPDATE purchase_requests SET
      status = ?, approved_by = ?, approved_date = ?, rejection_reason = ?, hold_reason = ?,
      modified_by = ?, modified_at = datetime('now')
     WHERE pr_number = ?`
  ).run(
    status,
    patch.approved_by,
    patch.approved_date,
    patch.rejection_reason,
    patch.hold_reason,
    user.username,
    prNumber
  );

  logActivity({
    entityType: 'PR',
    entityRef: prNumber,
    action: action,
    details: reason || status,
    byUser: user.username,
  });

  return getPR(prNumber);
}

export function clubPRs({ prNumbers, user }) {
  if (!prNumbers || prNumbers.length < 2) throw new Error('Select at least two PRs to club');
  const db = getDb();

  const prs = prNumbers.map((n) => db.prepare('SELECT * FROM purchase_requests WHERE pr_number = ?').get(n));
  if (prs.some((p) => !p)) throw new Error('One or more PRs not found');
  if (prs.some((p) => p.status !== PR_STATUS.APPROVED)) {
    throw new Error('Only Approved PRs can be clubbed');
  }
  const vendorCode = prs[0].vendor_code;
  if (prs.some((p) => p.vendor_code !== vendorCode)) {
    throw new Error('All PRs must belong to the same vendor');
  }
  const total = prs.reduce((s, p) => s + Number(p.total_amount || 0), 0);
  if (total > PR_LIMIT) throw new Error(`Clubbed total ₹${total} exceeds ₹12,00,000 limit`);

  let newPrNumber;
  const tx = db.transaction(() => {
    const now = new Date();
    const prefix = `PR-${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-`;
    const row = db
      .prepare(`SELECT pr_number FROM purchase_requests WHERE pr_number LIKE ? ORDER BY pr_number DESC LIMIT 1`)
      .get(`${prefix}%`);
    let n = 1;
    if (row?.pr_number) n = (parseInt(row.pr_number.split('-').pop(), 10) || 0) + 1;
    newPrNumber = `${prefix}${String(n).padStart(7, '0')}`;

    const info = db
      .prepare(
        `INSERT INTO purchase_requests (
          pr_number, project_code, product_no, vendor_code, vendor_name, total_amount,
          status, requested_by, approved_by, approved_date, created_by, remarks, is_clubbed, clubbed_from_pr_ids
        ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, datetime('now'), ?, ?, 1, ?)`
      )
      .run(
        newPrNumber,
        prs.map((p) => p.project_code).filter((v, i, a) => a.indexOf(v) === i).join(','),
        prs[0].product_no,
        vendorCode,
        prs[0].vendor_name,
        total,
        PR_STATUS.APPROVED,
        user.displayName || user.username,
        user.displayName || user.username,
        user.username,
        `Clubbed from ${prNumbers.join(', ')}`,
        prNumbers.join(',')
      );

    const insertLine = db.prepare(`
      INSERT INTO purchase_request_details (
        pr_id, pr_number, project_code, product_no, item_code, item_description, specification,
        make, mfg_part_no, quantity, uom, unit_cost, total_cost, vendor_code, vendor_name,
        bom_code, hsn_code, line_status, remarks, created_by
      ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    `);

    for (const pr of prs) {
      const lines = db.prepare('SELECT * FROM purchase_request_details WHERE pr_number = ?').all(pr.pr_number);
      for (const line of lines) {
        insertLine.run(
          info.lastInsertRowid,
          newPrNumber,
          line.project_code,
          line.product_no,
          line.item_code,
          line.item_description,
          line.specification,
          line.make,
          line.mfg_part_no,
          line.quantity,
          line.uom,
          line.unit_cost,
          line.total_cost,
          line.vendor_code,
          line.vendor_name,
          line.bom_code,
          line.hsn_code,
          PR_LINE_STATUS.PENDING,
          line.remarks,
          user.username
        );
      }
      db.prepare(
        `UPDATE purchase_requests SET status = ?, modified_by = ?, modified_at = datetime('now') WHERE pr_number = ?`
      ).run(PR_STATUS.CLUBBED, user.username, pr.pr_number);
    }

    logActivity({
      entityType: 'PR',
      entityRef: newPrNumber,
      action: 'Clubbed',
      details: `From ${prNumbers.join(', ')}`,
      byUser: user.username,
    });
  });

  tx();
  return getPR(newPrNumber);
}

export function kanbanColumns() {
  const db = getDb();
  const rows = db
    .prepare(
      `SELECT pr.*,
        CAST((julianday('now') - julianday(pr.request_date)) AS INTEGER) AS age_days,
        (SELECT COUNT(*) FROM purchase_request_details d WHERE d.pr_number = pr.pr_number) AS item_count
       FROM purchase_requests pr
       WHERE pr.status NOT IN (?, ?)
       ORDER BY pr.request_date ASC`
    )
    .all(PR_STATUS.CLUBBED, PR_STATUS.REJECTED)
    .map(mapPr);

  const columns = {
    [PR_STATUS.PENDING]: [],
    [PR_STATUS.ON_HOLD]: [],
    [PR_STATUS.APPROVED]: [],
    [PR_STATUS.PARTIALLY_CONVERTED]: [],
    [PR_STATUS.FULLY_CONVERTED]: [],
    'PO Pending': [],
    'PO Approved': [],
  };

  for (const pr of rows) {
    if (columns[pr.status]) columns[pr.status].push(pr);
    else columns[PR_STATUS.PENDING].push(pr);
  }

  // Enrich with PO stage buckets
  const poPending = db
    .prepare(
      `SELECT DISTINCT pr_number FROM purchase_orders WHERE po_approved = 0 AND pr_number IS NOT NULL`
    )
    .all()
    .map((r) => r.pr_number);
  const poApproved = db
    .prepare(
      `SELECT DISTINCT pr_number FROM purchase_orders WHERE po_approved = 1 AND pr_number IS NOT NULL`
    )
    .all()
    .map((r) => r.pr_number);

  for (const pr of rows) {
    if (poApproved.includes(pr.prNumber)) columns['PO Approved'].push(pr);
    else if (poPending.includes(pr.prNumber)) columns['PO Pending'].push(pr);
  }

  return columns;
}

export function moveKanbanCard(prNumber, targetStatus, user) {
  const allowed = [PR_STATUS.PENDING, PR_STATUS.ON_HOLD, PR_STATUS.APPROVED, PR_STATUS.REJECTED];
  if (!allowed.includes(targetStatus)) {
    throw new Error('Kanban move only allowed to Pending / On Hold / Approved / Rejected');
  }
  const actionMap = {
    [PR_STATUS.PENDING]: 'release',
    [PR_STATUS.ON_HOLD]: 'hold',
    [PR_STATUS.APPROVED]: 'approve',
    [PR_STATUS.REJECTED]: 'reject',
  };
  return updatePRStatus(prNumber, {
    action: actionMap[targetStatus],
    reason: `Kanban move to ${targetStatus}`,
    user,
  });
}
