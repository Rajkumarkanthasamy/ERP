import { getDb } from '../db/connection.js';
import { logActivity } from './helpers.js';
import { genNumber } from './entityHelpers.js';

function mapHeader(row) {
  if (!row) return null;
  return {
    id: row.id,
    indentNumber: row.indent_number,
    projectCode: row.project_code,
    requestedBy: row.requested_by,
    requestDate: row.request_date,
    requiredDate: row.required_date,
    status: row.status,
    approvedBy: row.approved_by,
    approvedDate: row.approved_date,
    rejectionReason: row.rejection_reason,
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
    indentId: row.indent_id,
    indentNumber: row.indent_number,
    itemCode: row.item_code,
    itemDescription: row.item_description,
    quantity: row.quantity,
    uom: row.uom,
    remarks: row.remarks,
  };
}

export function listIndents({ status, q } = {}) {
  const db = getDb();
  const where = ['1=1'];
  const params = [];
  if (status && status !== 'All') {
    where.push('status = ?');
    params.push(status);
  }
  if (q) {
    where.push('(indent_number LIKE ? OR IFNULL(project_code,"") LIKE ?)');
    params.push(`%${q}%`, `%${q}%`);
  }
  return db
    .prepare(`SELECT * FROM indents WHERE ${where.join(' AND ')} ORDER BY id DESC`)
    .all(...params)
    .map(mapHeader);
}

export function getIndent(indentNumber) {
  const db = getDb();
  const header = mapHeader(db.prepare('SELECT * FROM indents WHERE indent_number = ?').get(indentNumber));
  if (!header) return null;
  const lines = db
    .prepare('SELECT * FROM indent_details WHERE indent_number = ? ORDER BY id')
    .all(indentNumber)
    .map(mapLine);
  return { ...header, lines };
}

export function createIndent(payload, user) {
  if (!payload.lines?.length) throw new Error('At least one line is required');
  const db = getDb();
  const indentNumber = payload.indentNumber || genNumber('IND-', 'indents', 'indent_number');

  const tx = db.transaction(() => {
    const info = db
      .prepare(
        `INSERT INTO indents (
          indent_number, project_code, requested_by, required_date, status, remarks, created_by
        ) VALUES (?, ?, ?, ?, 'Open', ?, ?)`
      )
      .run(
        indentNumber,
        payload.projectCode || null,
        user.displayName || user.username,
        payload.requiredDate || null,
        payload.remarks || null,
        user.username
      );

    const insert = db.prepare(
      `INSERT INTO indent_details (indent_id, indent_number, item_code, item_description, quantity, uom, remarks)
       VALUES (?, ?, ?, ?, ?, ?, ?)`
    );
    for (const line of payload.lines) {
      insert.run(
        info.lastInsertRowid,
        indentNumber,
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
    entityType: 'INDENT',
    entityRef: indentNumber,
    action: 'Created',
    details: payload.projectCode || '',
    byUser: user.username,
  });
  return getIndent(indentNumber);
}

export function updateIndent(indentNumber, payload, user) {
  const db = getDb();
  const existing = db.prepare('SELECT * FROM indents WHERE indent_number = ?').get(indentNumber);
  if (!existing) throw new Error('Indent not found');
  if (!['Open', 'Draft'].includes(existing.status)) throw new Error('Only Open indents can be updated');

  db.prepare(
    `UPDATE indents SET project_code = ?, required_date = ?, remarks = ?,
      modified_by = ?, modified_at = datetime('now') WHERE indent_number = ?`
  ).run(
    payload.projectCode ?? existing.project_code,
    payload.requiredDate ?? existing.required_date,
    payload.remarks ?? existing.remarks,
    user.username,
    indentNumber
  );

  if (payload.lines) {
    db.prepare('DELETE FROM indent_details WHERE indent_number = ?').run(indentNumber);
    const insert = db.prepare(
      `INSERT INTO indent_details (indent_id, indent_number, item_code, item_description, quantity, uom, remarks)
       VALUES (?, ?, ?, ?, ?, ?, ?)`
    );
    for (const line of payload.lines) {
      insert.run(
        existing.id,
        indentNumber,
        line.itemCode,
        line.itemDescription || null,
        line.quantity || 0,
        line.uom || 'NOS',
        line.remarks || null
      );
    }
  }
  return getIndent(indentNumber);
}

export function updateIndentStatus(indentNumber, { action, reason, user }) {
  const db = getDb();
  const row = db.prepare('SELECT * FROM indents WHERE indent_number = ?').get(indentNumber);
  if (!row) throw new Error('Indent not found');

  let status = row.status;
  let approvedBy = row.approved_by;
  let approvedDate = row.approved_date;
  let rejectionReason = row.rejection_reason;

  switch (action) {
    case 'approve':
      if (!['Open', 'Pending'].includes(row.status)) throw new Error(`Cannot approve in status ${row.status}`);
      status = 'Approved';
      approvedBy = user.displayName || user.username;
      approvedDate = new Date().toISOString();
      break;
    case 'reject':
      status = 'Rejected';
      rejectionReason = reason || 'Rejected';
      break;
    case 'close':
      status = 'Closed';
      break;
    default:
      throw new Error('Unknown action');
  }

  db.prepare(
    `UPDATE indents SET status = ?, approved_by = ?, approved_date = ?, rejection_reason = ?,
      modified_by = ?, modified_at = datetime('now') WHERE indent_number = ?`
  ).run(status, approvedBy, approvedDate, rejectionReason, user.username, indentNumber);

  logActivity({
    entityType: 'INDENT',
    entityRef: indentNumber,
    action,
    details: reason || status,
    byUser: user.username,
  });
  return getIndent(indentNumber);
}
