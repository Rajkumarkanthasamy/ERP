import { getDb } from '../db/connection.js';
import { logActivity } from './helpers.js';
import { genNumber } from './entityHelpers.js';

function mapCall(row) {
  if (!row) return null;
  return {
    id: row.id,
    callNumber: row.call_number,
    customerCode: row.customer_code,
    customerName: row.customer_name,
    projectCode: row.project_code,
    subject: row.subject,
    priority: row.priority,
    status: row.status,
    reportedDate: row.reported_date,
    assignedTo: row.assigned_to,
    resolvedDate: row.resolved_date,
    resolution: row.resolution,
    remarks: row.remarks,
    createdBy: row.created_by,
    createdAt: row.created_at,
    modifiedBy: row.modified_by,
    modifiedAt: row.modified_at,
  };
}

export function listServiceCalls({ status, q } = {}) {
  const db = getDb();
  const where = ['1=1'];
  const params = [];
  if (status && status !== 'All') {
    where.push('status = ?');
    params.push(status);
  }
  if (q) {
    where.push('(call_number LIKE ? OR IFNULL(customer_name,"") LIKE ? OR IFNULL(subject,"") LIKE ?)');
    params.push(`%${q}%`, `%${q}%`, `%${q}%`);
  }
  return db
    .prepare(`SELECT * FROM service_calls WHERE ${where.join(' AND ')} ORDER BY id DESC`)
    .all(...params)
    .map(mapCall);
}

export function getServiceCall(callNumber) {
  return mapCall(getDb().prepare('SELECT * FROM service_calls WHERE call_number = ?').get(callNumber));
}

export function createServiceCall(payload, user) {
  const db = getDb();
  const callNumber = payload.callNumber || genNumber('SC-', 'service_calls', 'call_number');
  db.prepare(
    `INSERT INTO service_calls (
      call_number, customer_code, customer_name, project_code, subject, priority, status,
      assigned_to, remarks, created_by
    ) VALUES (?, ?, ?, ?, ?, ?, 'Open', ?, ?, ?)`
  ).run(
    callNumber,
    payload.customerCode || null,
    payload.customerName || null,
    payload.projectCode || null,
    payload.subject || null,
    payload.priority || 'Normal',
    payload.assignedTo || null,
    payload.remarks || null,
    user.username
  );
  logActivity({
    entityType: 'SERVICE',
    entityRef: callNumber,
    action: 'Created',
    details: payload.subject || '',
    byUser: user.username,
  });
  return getServiceCall(callNumber);
}

export function updateServiceCall(callNumber, payload, user) {
  const db = getDb();
  const existing = db.prepare('SELECT * FROM service_calls WHERE call_number = ?').get(callNumber);
  if (!existing) throw new Error('Service call not found');
  db.prepare(
    `UPDATE service_calls SET customer_code = ?, customer_name = ?, project_code = ?, subject = ?,
      priority = ?, status = ?, assigned_to = ?, resolution = ?, remarks = ?,
      modified_by = ?, modified_at = datetime('now')
     WHERE call_number = ?`
  ).run(
    payload.customerCode ?? existing.customer_code,
    payload.customerName ?? existing.customer_name,
    payload.projectCode ?? existing.project_code,
    payload.subject ?? existing.subject,
    payload.priority ?? existing.priority,
    payload.status ?? existing.status,
    payload.assignedTo ?? existing.assigned_to,
    payload.resolution ?? existing.resolution,
    payload.remarks ?? existing.remarks,
    user.username,
    callNumber
  );
  return getServiceCall(callNumber);
}

export function updateServiceCallStatus(callNumber, { action, reason, resolution, user }) {
  const db = getDb();
  const row = db.prepare('SELECT * FROM service_calls WHERE call_number = ?').get(callNumber);
  if (!row) throw new Error('Service call not found');

  let status = row.status;
  let resolvedDate = row.resolved_date;
  let resText = row.resolution;

  switch (action) {
    case 'assign':
      status = 'Assigned';
      break;
    case 'start':
      status = 'In Progress';
      break;
    case 'resolve':
      status = 'Resolved';
      resolvedDate = new Date().toISOString();
      resText = resolution || reason || 'Resolved';
      break;
    case 'close':
      status = 'Closed';
      break;
    case 'reopen':
      status = 'Open';
      resolvedDate = null;
      break;
    default:
      throw new Error('Unknown action');
  }

  db.prepare(
    `UPDATE service_calls SET status = ?, resolved_date = ?, resolution = ?,
      modified_by = ?, modified_at = datetime('now') WHERE call_number = ?`
  ).run(status, resolvedDate, resText, user.username, callNumber);

  logActivity({
    entityType: 'SERVICE',
    entityRef: callNumber,
    action,
    details: reason || status,
    byUser: user.username,
  });
  return getServiceCall(callNumber);
}
