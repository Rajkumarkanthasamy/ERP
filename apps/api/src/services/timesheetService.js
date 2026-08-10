import { getDb } from '../db/connection.js';
import { logActivity } from './helpers.js';
import { genNumber } from './entityHelpers.js';

function mapEntry(row) {
  if (!row) return null;
  return {
    id: row.id,
    entryNumber: row.entry_number,
    userName: row.user_name,
    projectCode: row.project_code,
    workDate: row.work_date,
    hours: row.hours,
    activity: row.activity,
    status: row.status,
    approvedBy: row.approved_by,
    approvedDate: row.approved_date,
    remarks: row.remarks,
    createdBy: row.created_by,
    createdAt: row.created_at,
    modifiedBy: row.modified_by,
    modifiedAt: row.modified_at,
  };
}

export function listTimesheets({ status, q, userName } = {}) {
  const db = getDb();
  const where = ['1=1'];
  const params = [];
  if (status && status !== 'All') {
    where.push('status = ?');
    params.push(status);
  }
  if (userName) {
    where.push('user_name LIKE ?');
    params.push(`%${userName}%`);
  }
  if (q) {
    where.push('(entry_number LIKE ? OR IFNULL(project_code,"") LIKE ? OR IFNULL(activity,"") LIKE ?)');
    params.push(`%${q}%`, `%${q}%`, `%${q}%`);
  }
  return db
    .prepare(`SELECT * FROM timesheets WHERE ${where.join(' AND ')} ORDER BY work_date DESC, id DESC`)
    .all(...params)
    .map(mapEntry);
}

export function getTimesheet(entryNumber) {
  return mapEntry(getDb().prepare('SELECT * FROM timesheets WHERE entry_number = ?').get(entryNumber));
}

export function createTimesheet(payload, user) {
  if (!payload.workDate) throw new Error('workDate is required');
  if (!payload.hours && payload.hours !== 0) throw new Error('hours is required');
  const db = getDb();
  const entryNumber = payload.entryNumber || genNumber('TS-', 'timesheets', 'entry_number');
  db.prepare(
    `INSERT INTO timesheets (
      entry_number, user_name, project_code, work_date, hours, activity, status, remarks, created_by
    ) VALUES (?, ?, ?, ?, ?, ?, 'Draft', ?, ?)`
  ).run(
    entryNumber,
    payload.userName || user.displayName || user.username,
    payload.projectCode || null,
    payload.workDate,
    payload.hours,
    payload.activity || null,
    payload.remarks || null,
    user.username
  );
  logActivity({
    entityType: 'TIMESHEET',
    entityRef: entryNumber,
    action: 'Created',
    details: `${payload.hours}h`,
    byUser: user.username,
  });
  return getTimesheet(entryNumber);
}

export function updateTimesheet(entryNumber, payload, user) {
  const db = getDb();
  const existing = db.prepare('SELECT * FROM timesheets WHERE entry_number = ?').get(entryNumber);
  if (!existing) throw new Error('Timesheet not found');
  if (!['Draft', 'Rejected'].includes(existing.status)) throw new Error('Only Draft timesheets can be updated');

  db.prepare(
    `UPDATE timesheets SET user_name = ?, project_code = ?, work_date = ?, hours = ?, activity = ?,
      remarks = ?, status = 'Draft', modified_by = ?, modified_at = datetime('now')
     WHERE entry_number = ?`
  ).run(
    payload.userName ?? existing.user_name,
    payload.projectCode ?? existing.project_code,
    payload.workDate ?? existing.work_date,
    payload.hours ?? existing.hours,
    payload.activity ?? existing.activity,
    payload.remarks ?? existing.remarks,
    user.username,
    entryNumber
  );
  return getTimesheet(entryNumber);
}

export function updateTimesheetStatus(entryNumber, { action, reason, user }) {
  const db = getDb();
  const row = db.prepare('SELECT * FROM timesheets WHERE entry_number = ?').get(entryNumber);
  if (!row) throw new Error('Timesheet not found');

  let status = row.status;
  let approvedBy = row.approved_by;
  let approvedDate = row.approved_date;

  switch (action) {
    case 'submit':
      status = 'Submitted';
      break;
    case 'approve':
      status = 'Approved';
      approvedBy = user.displayName || user.username;
      approvedDate = new Date().toISOString();
      break;
    case 'reject':
      status = 'Rejected';
      break;
    default:
      throw new Error('Unknown action');
  }

  db.prepare(
    `UPDATE timesheets SET status = ?, approved_by = ?, approved_date = ?,
      modified_by = ?, modified_at = datetime('now') WHERE entry_number = ?`
  ).run(status, approvedBy, approvedDate, user.username, entryNumber);

  logActivity({
    entityType: 'TIMESHEET',
    entityRef: entryNumber,
    action,
    details: reason || status,
    byUser: user.username,
  });
  return getTimesheet(entryNumber);
}
