import { getDb } from '../db/connection.js';
import { logActivity } from './helpers.js';
import { genNumber } from './entityHelpers.js';

function mapNc(row) {
  if (!row) return null;
  return {
    id: row.id,
    ncNumber: row.nc_number,
    projectCode: row.project_code,
    itemCode: row.item_code,
    source: row.source,
    description: row.description,
    severity: row.severity,
    status: row.status,
    raisedBy: row.raised_by,
    raisedDate: row.raised_date,
    assignedTo: row.assigned_to,
    closedBy: row.closed_by,
    closedDate: row.closed_date,
    correctiveAction: row.corrective_action,
    remarks: row.remarks,
    createdBy: row.created_by,
    createdAt: row.created_at,
    modifiedBy: row.modified_by,
    modifiedAt: row.modified_at,
  };
}

function mapEscalation(row) {
  if (!row) return null;
  return {
    id: row.id,
    escalationNumber: row.escalation_number,
    relatedType: row.related_type,
    relatedRef: row.related_ref,
    projectCode: row.project_code,
    title: row.title,
    description: row.description,
    priority: row.priority,
    status: row.status,
    raisedBy: row.raised_by,
    raisedDate: row.raised_date,
    assignedTo: row.assigned_to,
    resolvedBy: row.resolved_by,
    resolvedDate: row.resolved_date,
    remarks: row.remarks,
    createdBy: row.created_by,
    createdAt: row.created_at,
    modifiedBy: row.modified_by,
    modifiedAt: row.modified_at,
  };
}

export function listNcs({ status, q } = {}) {
  const db = getDb();
  const where = ['1=1'];
  const params = [];
  if (status && status !== 'All') {
    where.push('status = ?');
    params.push(status);
  }
  if (q) {
    where.push('(nc_number LIKE ? OR IFNULL(description,"") LIKE ? OR IFNULL(project_code,"") LIKE ?)');
    params.push(`%${q}%`, `%${q}%`, `%${q}%`);
  }
  return db
    .prepare(`SELECT * FROM quality_ncs WHERE ${where.join(' AND ')} ORDER BY id DESC`)
    .all(...params)
    .map(mapNc);
}

export function getNc(ncNumber) {
  return mapNc(getDb().prepare('SELECT * FROM quality_ncs WHERE nc_number = ?').get(ncNumber));
}

export function createNc(payload, user) {
  if (!payload.description) throw new Error('description is required');
  const db = getDb();
  const ncNumber = payload.ncNumber || genNumber('NC-', 'quality_ncs', 'nc_number');
  db.prepare(
    `INSERT INTO quality_ncs (
      nc_number, project_code, item_code, source, description, severity, status,
      raised_by, assigned_to, remarks, created_by
    ) VALUES (?, ?, ?, ?, ?, ?, 'Open', ?, ?, ?, ?)`
  ).run(
    ncNumber,
    payload.projectCode || null,
    payload.itemCode || null,
    payload.source || null,
    payload.description,
    payload.severity || 'Minor',
    user.displayName || user.username,
    payload.assignedTo || null,
    payload.remarks || null,
    user.username
  );
  logActivity({
    entityType: 'NC',
    entityRef: ncNumber,
    action: 'Created',
    details: payload.description,
    byUser: user.username,
  });
  return getNc(ncNumber);
}

export function updateNc(ncNumber, payload, user) {
  const db = getDb();
  const existing = db.prepare('SELECT * FROM quality_ncs WHERE nc_number = ?').get(ncNumber);
  if (!existing) throw new Error('NC not found');
  db.prepare(
    `UPDATE quality_ncs SET project_code = ?, item_code = ?, source = ?, description = ?, severity = ?,
      assigned_to = ?, corrective_action = ?, remarks = ?, modified_by = ?, modified_at = datetime('now')
     WHERE nc_number = ?`
  ).run(
    payload.projectCode ?? existing.project_code,
    payload.itemCode ?? existing.item_code,
    payload.source ?? existing.source,
    payload.description ?? existing.description,
    payload.severity ?? existing.severity,
    payload.assignedTo ?? existing.assigned_to,
    payload.correctiveAction ?? existing.corrective_action,
    payload.remarks ?? existing.remarks,
    user.username,
    ncNumber
  );
  return getNc(ncNumber);
}

export function updateNcStatus(ncNumber, { action, reason, correctiveAction, user }) {
  const db = getDb();
  const row = db.prepare('SELECT * FROM quality_ncs WHERE nc_number = ?').get(ncNumber);
  if (!row) throw new Error('NC not found');

  let status = row.status;
  let closedBy = row.closed_by;
  let closedDate = row.closed_date;
  let ca = row.corrective_action;

  switch (action) {
    case 'assign':
      status = 'In Progress';
      break;
    case 'close':
      status = 'Closed';
      closedBy = user.displayName || user.username;
      closedDate = new Date().toISOString();
      ca = correctiveAction || reason || ca;
      break;
    case 'escalate':
      status = 'Escalated';
      break;
    case 'reopen':
      status = 'Open';
      closedBy = null;
      closedDate = null;
      break;
    default:
      throw new Error('Unknown action');
  }

  db.prepare(
    `UPDATE quality_ncs SET status = ?, closed_by = ?, closed_date = ?, corrective_action = ?,
      modified_by = ?, modified_at = datetime('now') WHERE nc_number = ?`
  ).run(status, closedBy, closedDate, ca, user.username, ncNumber);

  logActivity({
    entityType: 'NC',
    entityRef: ncNumber,
    action,
    details: reason || status,
    byUser: user.username,
  });
  return getNc(ncNumber);
}

export function listEscalations({ status, q } = {}) {
  const db = getDb();
  const where = ['1=1'];
  const params = [];
  if (status && status !== 'All') {
    where.push('status = ?');
    params.push(status);
  }
  if (q) {
    where.push('(escalation_number LIKE ? OR IFNULL(title,"") LIKE ? OR IFNULL(project_code,"") LIKE ?)');
    params.push(`%${q}%`, `%${q}%`, `%${q}%`);
  }
  return db
    .prepare(`SELECT * FROM escalations WHERE ${where.join(' AND ')} ORDER BY id DESC`)
    .all(...params)
    .map(mapEscalation);
}

export function getEscalation(escalationNumber) {
  return mapEscalation(
    getDb().prepare('SELECT * FROM escalations WHERE escalation_number = ?').get(escalationNumber)
  );
}

export function createEscalation(payload, user) {
  if (!payload.title) throw new Error('title is required');
  const db = getDb();
  const escalationNumber =
    payload.escalationNumber || genNumber('ESC-', 'escalations', 'escalation_number');
  db.prepare(
    `INSERT INTO escalations (
      escalation_number, related_type, related_ref, project_code, title, description,
      priority, status, raised_by, assigned_to, remarks, created_by
    ) VALUES (?, ?, ?, ?, ?, ?, ?, 'Open', ?, ?, ?, ?)`
  ).run(
    escalationNumber,
    payload.relatedType || null,
    payload.relatedRef || null,
    payload.projectCode || null,
    payload.title,
    payload.description || null,
    payload.priority || 'High',
    user.displayName || user.username,
    payload.assignedTo || null,
    payload.remarks || null,
    user.username
  );
  logActivity({
    entityType: 'ESC',
    entityRef: escalationNumber,
    action: 'Created',
    details: payload.title,
    byUser: user.username,
  });
  return getEscalation(escalationNumber);
}

export function updateEscalation(escalationNumber, payload, user) {
  const db = getDb();
  const existing = db.prepare('SELECT * FROM escalations WHERE escalation_number = ?').get(escalationNumber);
  if (!existing) throw new Error('Escalation not found');
  db.prepare(
    `UPDATE escalations SET related_type = ?, related_ref = ?, project_code = ?, title = ?,
      description = ?, priority = ?, assigned_to = ?, remarks = ?,
      modified_by = ?, modified_at = datetime('now')
     WHERE escalation_number = ?`
  ).run(
    payload.relatedType ?? existing.related_type,
    payload.relatedRef ?? existing.related_ref,
    payload.projectCode ?? existing.project_code,
    payload.title ?? existing.title,
    payload.description ?? existing.description,
    payload.priority ?? existing.priority,
    payload.assignedTo ?? existing.assigned_to,
    payload.remarks ?? existing.remarks,
    user.username,
    escalationNumber
  );
  return getEscalation(escalationNumber);
}

export function updateEscalationStatus(escalationNumber, { action, reason, user }) {
  const db = getDb();
  const row = db.prepare('SELECT * FROM escalations WHERE escalation_number = ?').get(escalationNumber);
  if (!row) throw new Error('Escalation not found');

  let status = row.status;
  let resolvedBy = row.resolved_by;
  let resolvedDate = row.resolved_date;

  switch (action) {
    case 'acknowledge':
      status = 'In Progress';
      break;
    case 'resolve':
      status = 'Resolved';
      resolvedBy = user.displayName || user.username;
      resolvedDate = new Date().toISOString();
      break;
    case 'close':
      status = 'Closed';
      break;
    case 'reopen':
      status = 'Open';
      resolvedBy = null;
      resolvedDate = null;
      break;
    default:
      throw new Error('Unknown action');
  }

  db.prepare(
    `UPDATE escalations SET status = ?, resolved_by = ?, resolved_date = ?,
      modified_by = ?, modified_at = datetime('now') WHERE escalation_number = ?`
  ).run(status, resolvedBy, resolvedDate, user.username, escalationNumber);

  logActivity({
    entityType: 'ESC',
    entityRef: escalationNumber,
    action,
    details: reason || status,
    byUser: user.username,
  });
  return getEscalation(escalationNumber);
}
