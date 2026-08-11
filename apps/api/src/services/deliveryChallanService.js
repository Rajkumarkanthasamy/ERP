import { getDb } from '../db/connection.js';
import { logActivity } from './helpers.js';
import { genNumber } from './entityHelpers.js';

function mapHeader(row) {
  if (!row) return null;
  return {
    id: row.id,
    dcNumber: row.dc_number,
    customerCode: row.customer_code,
    customerName: row.customer_name,
    projectCode: row.project_code,
    dcDate: row.dc_date,
    status: row.status,
    vehicleNo: row.vehicle_no,
    transporter: row.transporter,
    ginNumber: row.gin_number,
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
    dcId: row.dc_id,
    dcNumber: row.dc_number,
    itemCode: row.item_code,
    itemDescription: row.item_description,
    quantity: row.quantity,
    uom: row.uom,
    remarks: row.remarks,
  };
}

export function listDeliveryChallans({ status, q } = {}) {
  const db = getDb();
  const where = ['1=1'];
  const params = [];
  if (status && status !== 'All') {
    where.push('status = ?');
    params.push(status);
  }
  if (q) {
    where.push('(dc_number LIKE ? OR IFNULL(customer_name,"") LIKE ? OR IFNULL(project_code,"") LIKE ?)');
    params.push(`%${q}%`, `%${q}%`, `%${q}%`);
  }
  return db
    .prepare(`SELECT * FROM delivery_challans WHERE ${where.join(' AND ')} ORDER BY id DESC`)
    .all(...params)
    .map(mapHeader);
}

export function getDeliveryChallan(dcNumber) {
  const db = getDb();
  const header = mapHeader(db.prepare('SELECT * FROM delivery_challans WHERE dc_number = ?').get(dcNumber));
  if (!header) return null;
  const lines = db
    .prepare('SELECT * FROM delivery_challan_details WHERE dc_number = ? ORDER BY id')
    .all(dcNumber)
    .map(mapLine);
  return { ...header, lines };
}

export function createDeliveryChallan(payload, user) {
  if (!payload.lines?.length) throw new Error('At least one line is required');
  const db = getDb();
  const dcNumber = payload.dcNumber || genNumber('DC-', 'delivery_challans', 'dc_number');

  const tx = db.transaction(() => {
    const info = db
      .prepare(
        `INSERT INTO delivery_challans (
          dc_number, customer_code, customer_name, project_code, status, vehicle_no,
          transporter, gin_number, remarks, created_by
        ) VALUES (?, ?, ?, ?, 'Draft', ?, ?, ?, ?, ?)`
      )
      .run(
        dcNumber,
        payload.customerCode || null,
        payload.customerName || null,
        payload.projectCode || null,
        payload.vehicleNo || null,
        payload.transporter || null,
        payload.ginNumber || null,
        payload.remarks || null,
        user.username
      );

    const insert = db.prepare(
      `INSERT INTO delivery_challan_details (dc_id, dc_number, item_code, item_description, quantity, uom, remarks)
       VALUES (?, ?, ?, ?, ?, ?, ?)`
    );
    for (const line of payload.lines) {
      insert.run(
        info.lastInsertRowid,
        dcNumber,
        line.itemCode,
        line.itemDescription || null,
        line.quantity || 0,
        line.uom || 'NOS',
        line.remarks || null
      );
    }
  });
  tx();

  logActivity({
    entityType: 'DC',
    entityRef: dcNumber,
    action: 'Created',
    details: payload.customerName || payload.projectCode || '',
    byUser: user.username,
  });
  return getDeliveryChallan(dcNumber);
}

export function updateDeliveryChallan(dcNumber, payload, user) {
  const db = getDb();
  const existing = db.prepare('SELECT * FROM delivery_challans WHERE dc_number = ?').get(dcNumber);
  if (!existing) throw new Error('Delivery challan not found');
  if (!['Draft', 'Open'].includes(existing.status)) throw new Error('Only Draft DCs can be updated');

  db.prepare(
    `UPDATE delivery_challans SET customer_code = ?, customer_name = ?, project_code = ?,
      vehicle_no = ?, transporter = ?, gin_number = ?, remarks = ?,
      modified_by = ?, modified_at = datetime('now')
     WHERE dc_number = ?`
  ).run(
    payload.customerCode ?? existing.customer_code,
    payload.customerName ?? existing.customer_name,
    payload.projectCode ?? existing.project_code,
    payload.vehicleNo ?? existing.vehicle_no,
    payload.transporter ?? existing.transporter,
    payload.ginNumber ?? existing.gin_number,
    payload.remarks ?? existing.remarks,
    user.username,
    dcNumber
  );

  if (payload.lines) {
    db.prepare('DELETE FROM delivery_challan_details WHERE dc_number = ?').run(dcNumber);
    const insert = db.prepare(
      `INSERT INTO delivery_challan_details (dc_id, dc_number, item_code, item_description, quantity, uom, remarks)
       VALUES (?, ?, ?, ?, ?, ?, ?)`
    );
    for (const line of payload.lines) {
      insert.run(
        existing.id,
        dcNumber,
        line.itemCode,
        line.itemDescription || null,
        line.quantity || 0,
        line.uom || 'NOS',
        line.remarks || null
      );
    }
  }
  return getDeliveryChallan(dcNumber);
}

export function updateDeliveryChallanStatus(dcNumber, { action, reason, user }) {
  const db = getDb();
  const row = db.prepare('SELECT * FROM delivery_challans WHERE dc_number = ?').get(dcNumber);
  if (!row) throw new Error('Delivery challan not found');

  let status = row.status;
  switch (action) {
    case 'issue':
      status = 'Issued';
      break;
    case 'dispatch':
      status = 'Dispatched';
      break;
    case 'deliver':
      status = 'Delivered';
      break;
    case 'cancel':
      status = 'Cancelled';
      break;
    default:
      throw new Error('Unknown action');
  }

  db.prepare(
    `UPDATE delivery_challans SET status = ?, modified_by = ?, modified_at = datetime('now') WHERE dc_number = ?`
  ).run(status, user.username, dcNumber);

  logActivity({
    entityType: 'DC',
    entityRef: dcNumber,
    action,
    details: reason || status,
    byUser: user.username,
  });
  return getDeliveryChallan(dcNumber);
}
