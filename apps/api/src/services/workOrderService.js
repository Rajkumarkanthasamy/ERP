import { getDb } from '../db/connection.js';
import { logActivity } from './helpers.js';
import { genNumber } from './entityHelpers.js';

function mapHeader(row) {
  if (!row) return null;
  return {
    id: row.id,
    woNumber: row.wo_number,
    projectCode: row.project_code,
    productNo: row.product_no,
    vendorCode: row.vendor_code,
    vendorName: row.vendor_name,
    status: row.status,
    startDate: row.start_date,
    dueDate: row.due_date,
    approvedBy: row.approved_by,
    approvedDate: row.approved_date,
    remarks: row.remarks,
    createdBy: row.created_by,
    createdAt: row.created_at,
    modifiedBy: row.modified_by,
    modifiedAt: row.modified_at,
  };
}

function mapLine(row) {
  return {
    id: row.id,
    woId: row.wo_id,
    woNumber: row.wo_number,
    itemCode: row.item_code,
    itemDescription: row.item_description,
    quantity: row.quantity,
    uom: row.uom,
    unitCost: row.unit_cost,
    totalCost: row.total_cost,
    remarks: row.remarks,
  };
}

export function listWorkOrders({ status, q } = {}) {
  const db = getDb();
  const where = ['1=1'];
  const params = [];
  if (status && status !== 'All') {
    where.push('status = ?');
    params.push(status);
  }
  if (q) {
    where.push('(wo_number LIKE ? OR IFNULL(project_code,"") LIKE ? OR IFNULL(vendor_code,"") LIKE ?)');
    params.push(`%${q}%`, `%${q}%`, `%${q}%`);
  }
  return db
    .prepare(`SELECT * FROM work_orders WHERE ${where.join(' AND ')} ORDER BY id DESC`)
    .all(...params)
    .map(mapHeader);
}

export function getWorkOrder(woNumber) {
  const db = getDb();
  const header = mapHeader(db.prepare('SELECT * FROM work_orders WHERE wo_number = ?').get(woNumber));
  if (!header) return null;
  const lines = db
    .prepare('SELECT * FROM work_order_details WHERE wo_number = ? ORDER BY id')
    .all(woNumber)
    .map(mapLine);
  return { ...header, lines };
}

export function createWorkOrder(payload, user) {
  if (!payload.lines?.length) throw new Error('At least one line is required');
  const db = getDb();
  const woNumber = payload.woNumber || genNumber('WO-', 'work_orders', 'wo_number');

  const tx = db.transaction(() => {
    const info = db
      .prepare(
        `INSERT INTO work_orders (
          wo_number, project_code, product_no, vendor_code, vendor_name, status,
          start_date, due_date, remarks, created_by
        ) VALUES (?, ?, ?, ?, ?, 'Open', ?, ?, ?, ?)`
      )
      .run(
        woNumber,
        payload.projectCode || null,
        payload.productNo || null,
        payload.vendorCode || null,
        payload.vendorName || null,
        payload.startDate || null,
        payload.dueDate || null,
        payload.remarks || null,
        user.username
      );

    const insert = db.prepare(
      `INSERT INTO work_order_details (
        wo_id, wo_number, item_code, item_description, quantity, uom, unit_cost, total_cost, remarks
      ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)`
    );
    for (const line of payload.lines) {
      const qty = Number(line.quantity || 0);
      const unit = Number(line.unitCost || 0);
      insert.run(
        info.lastInsertRowid,
        woNumber,
        line.itemCode,
        line.itemDescription || null,
        qty,
        line.uom || 'NOS',
        unit,
        qty * unit,
        line.remarks || null
      );
    }
  });
  tx();

  logActivity({
    entityType: 'WO',
    entityRef: woNumber,
    action: 'Created',
    details: payload.projectCode || '',
    byUser: user.username,
  });
  return getWorkOrder(woNumber);
}

export function updateWorkOrder(woNumber, payload, user) {
  const db = getDb();
  const existing = db.prepare('SELECT * FROM work_orders WHERE wo_number = ?').get(woNumber);
  if (!existing) throw new Error('Work order not found');
  if (!['Open', 'Draft'].includes(existing.status)) throw new Error('Only Open work orders can be updated');

  db.prepare(
    `UPDATE work_orders SET project_code = ?, product_no = ?, vendor_code = ?, vendor_name = ?,
      start_date = ?, due_date = ?, remarks = ?, modified_by = ?, modified_at = datetime('now')
     WHERE wo_number = ?`
  ).run(
    payload.projectCode ?? existing.project_code,
    payload.productNo ?? existing.product_no,
    payload.vendorCode ?? existing.vendor_code,
    payload.vendorName ?? existing.vendor_name,
    payload.startDate ?? existing.start_date,
    payload.dueDate ?? existing.due_date,
    payload.remarks ?? existing.remarks,
    user.username,
    woNumber
  );

  if (payload.lines) {
    db.prepare('DELETE FROM work_order_details WHERE wo_number = ?').run(woNumber);
    const insert = db.prepare(
      `INSERT INTO work_order_details (
        wo_id, wo_number, item_code, item_description, quantity, uom, unit_cost, total_cost, remarks
      ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)`
    );
    for (const line of payload.lines) {
      const qty = Number(line.quantity || 0);
      const unit = Number(line.unitCost || 0);
      insert.run(
        existing.id,
        woNumber,
        line.itemCode,
        line.itemDescription || null,
        qty,
        line.uom || 'NOS',
        unit,
        qty * unit,
        line.remarks || null
      );
    }
  }
  return getWorkOrder(woNumber);
}

export function updateWorkOrderStatus(woNumber, { action, reason, user }) {
  const db = getDb();
  const row = db.prepare('SELECT * FROM work_orders WHERE wo_number = ?').get(woNumber);
  if (!row) throw new Error('Work order not found');

  let status = row.status;
  let approvedBy = row.approved_by;
  let approvedDate = row.approved_date;

  switch (action) {
    case 'approve':
      status = 'Approved';
      approvedBy = user.displayName || user.username;
      approvedDate = new Date().toISOString();
      break;
    case 'start':
      status = 'In Progress';
      break;
    case 'complete':
      status = 'Completed';
      break;
    case 'close':
      status = 'Closed';
      break;
    case 'cancel':
      status = 'Cancelled';
      break;
    default:
      throw new Error('Unknown action');
  }

  db.prepare(
    `UPDATE work_orders SET status = ?, approved_by = ?, approved_date = ?,
      modified_by = ?, modified_at = datetime('now') WHERE wo_number = ?`
  ).run(status, approvedBy, approvedDate, user.username, woNumber);

  logActivity({
    entityType: 'WO',
    entityRef: woNumber,
    action,
    details: reason || status,
    byUser: user.username,
  });
  return getWorkOrder(woNumber);
}
