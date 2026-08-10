import { getDb } from '../db/connection.js';
import { logActivity } from './helpers.js';
import { genNumber } from './entityHelpers.js';

function mapComplaint(row) {
  if (!row) return null;
  return {
    id: row.id,
    complaintNumber: row.complaint_number,
    customerCode: row.customer_code,
    customerName: row.customer_name,
    subject: row.subject,
    description: row.description,
    category: row.category,
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

export function listComplaints({ status, q } = {}) {
  const db = getDb();
  const where = ['1=1'];
  const params = [];
  if (status && status !== 'All') {
    where.push('status = ?');
    params.push(status);
  }
  if (q) {
    where.push('(complaint_number LIKE ? OR IFNULL(subject,"") LIKE ? OR IFNULL(customer_name,"") LIKE ?)');
    params.push(`%${q}%`, `%${q}%`, `%${q}%`);
  }
  return db
    .prepare(`SELECT * FROM complaints WHERE ${where.join(' AND ')} ORDER BY id DESC`)
    .all(...params)
    .map(mapComplaint);
}

export function getComplaint(complaintNumber) {
  return mapComplaint(
    getDb().prepare('SELECT * FROM complaints WHERE complaint_number = ?').get(complaintNumber)
  );
}

export function createComplaint(payload, user) {
  if (!payload.subject) throw new Error('subject is required');
  const db = getDb();
  const complaintNumber = payload.complaintNumber || genNumber('CMP-', 'complaints', 'complaint_number');
  db.prepare(
    `INSERT INTO complaints (
      complaint_number, customer_code, customer_name, subject, description, category, priority,
      status, assigned_to, remarks, created_by
    ) VALUES (?, ?, ?, ?, ?, ?, ?, 'Open', ?, ?, ?)`
  ).run(
    complaintNumber,
    payload.customerCode || null,
    payload.customerName || null,
    payload.subject,
    payload.description || null,
    payload.category || 'Support',
    payload.priority || 'Normal',
    payload.assignedTo || null,
    payload.remarks || null,
    user.username
  );
  logActivity({
    entityType: 'COMPLAINT',
    entityRef: complaintNumber,
    action: 'Created',
    details: payload.subject,
    byUser: user.username,
  });
  return getComplaint(complaintNumber);
}

export function updateComplaint(complaintNumber, payload, user) {
  const db = getDb();
  const existing = db.prepare('SELECT * FROM complaints WHERE complaint_number = ?').get(complaintNumber);
  if (!existing) throw new Error('Complaint not found');
  db.prepare(
    `UPDATE complaints SET customer_code = ?, customer_name = ?, subject = ?, description = ?,
      category = ?, priority = ?, assigned_to = ?, resolution = ?, remarks = ?,
      modified_by = ?, modified_at = datetime('now')
     WHERE complaint_number = ?`
  ).run(
    payload.customerCode ?? existing.customer_code,
    payload.customerName ?? existing.customer_name,
    payload.subject ?? existing.subject,
    payload.description ?? existing.description,
    payload.category ?? existing.category,
    payload.priority ?? existing.priority,
    payload.assignedTo ?? existing.assigned_to,
    payload.resolution ?? existing.resolution,
    payload.remarks ?? existing.remarks,
    user.username,
    complaintNumber
  );
  return getComplaint(complaintNumber);
}

export function updateComplaintStatus(complaintNumber, { action, reason, resolution, user }) {
  const db = getDb();
  const row = db.prepare('SELECT * FROM complaints WHERE complaint_number = ?').get(complaintNumber);
  if (!row) throw new Error('Complaint not found');

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
    `UPDATE complaints SET status = ?, resolved_date = ?, resolution = ?,
      modified_by = ?, modified_at = datetime('now') WHERE complaint_number = ?`
  ).run(status, resolvedDate, resText, user.username, complaintNumber);

  logActivity({
    entityType: 'COMPLAINT',
    entityRef: complaintNumber,
    action,
    details: reason || status,
    byUser: user.username,
  });
  return getComplaint(complaintNumber);
}
