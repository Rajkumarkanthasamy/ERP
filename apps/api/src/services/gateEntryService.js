import { getDb } from '../db/connection.js';
import { logActivity } from './helpers.js';
import { genNumber } from './entityHelpers.js';

function mapEntry(row) {
  if (!row) return null;
  return {
    id: row.id,
    entryNumber: row.entry_number,
    entryType: row.entry_type,
    vehicleNo: row.vehicle_no,
    transporter: row.transporter,
    vendorCode: row.vendor_code,
    vendorName: row.vendor_name,
    customerCode: row.customer_code,
    customerName: row.customer_name,
    purpose: row.purpose,
    invoiceNo: row.invoice_no,
    status: row.status,
    inTime: row.in_time,
    outTime: row.out_time,
    remarks: row.remarks,
    createdBy: row.created_by,
    createdAt: row.created_at,
    modifiedBy: row.modified_by,
    modifiedAt: row.modified_at,
  };
}

export function listGateEntries({ status, type, entryType, q } = {}) {
  const db = getDb();
  const where = ['1=1'];
  const params = [];
  if (status && status !== 'All') {
    where.push('status = ?');
    params.push(status);
  }
  const requestedType = entryType || type;
  if (requestedType && requestedType !== 'All') {
    if (requestedType === 'Inward') {
      where.push("entry_type IN ('Inward', 'Inbound')");
    } else {
      where.push('entry_type = ?');
      params.push(requestedType);
    }
  }
  if (q) {
    where.push(
      '(entry_number LIKE ? OR IFNULL(vehicle_no,"") LIKE ? OR IFNULL(vendor_name,"") LIKE ? OR IFNULL(invoice_no,"") LIKE ?)'
    );
    params.push(`%${q}%`, `%${q}%`, `%${q}%`, `%${q}%`);
  }
  return db
    .prepare(`SELECT * FROM gate_entries WHERE ${where.join(' AND ')} ORDER BY id DESC`)
    .all(...params)
    .map(mapEntry);
}

export function getGateEntry(entryNumber) {
  return mapEntry(getDb().prepare('SELECT * FROM gate_entries WHERE entry_number = ?').get(entryNumber));
}

export function createGateEntry(payload, user) {
  const db = getDb();
  const entryNumber = payload.entryNumber || genNumber('GE-', 'gate_entries', 'entry_number');
  db.prepare(
    `INSERT INTO gate_entries (
      entry_number, entry_type, vehicle_no, transporter, vendor_code, vendor_name,
      customer_code, customer_name, purpose, invoice_no, status, remarks, created_by
    ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, 'Open', ?, ?)`
  ).run(
    entryNumber,
    payload.entryType || 'Inward',
    payload.vehicleNo || null,
    payload.transporter || null,
    payload.vendorCode || null,
    payload.vendorName || null,
    payload.customerCode || null,
    payload.customerName || null,
    payload.purpose || null,
    payload.invoiceNo || null,
    payload.remarks || null,
    user.username
  );
  logActivity({
    entityType: 'GATE',
    entityRef: entryNumber,
    action: 'Created',
    details: payload.vehicleNo || payload.purpose || '',
    byUser: user.username,
  });
  return getGateEntry(entryNumber);
}

export function updateGateEntry(entryNumber, payload, user) {
  const db = getDb();
  const existing = db.prepare('SELECT * FROM gate_entries WHERE entry_number = ?').get(entryNumber);
  if (!existing) throw new Error('Gate entry not found');
  db.prepare(
    `UPDATE gate_entries SET entry_type = ?, vehicle_no = ?, transporter = ?, vendor_code = ?,
      vendor_name = ?, customer_code = ?, customer_name = ?, purpose = ?, invoice_no = ?,
      remarks = ?, modified_by = ?, modified_at = datetime('now')
     WHERE entry_number = ?`
  ).run(
    payload.entryType ?? existing.entry_type,
    payload.vehicleNo ?? existing.vehicle_no,
    payload.transporter ?? existing.transporter,
    payload.vendorCode ?? existing.vendor_code,
    payload.vendorName ?? existing.vendor_name,
    payload.customerCode ?? existing.customer_code,
    payload.customerName ?? existing.customer_name,
    payload.purpose ?? existing.purpose,
    payload.invoiceNo ?? existing.invoice_no,
    payload.remarks ?? existing.remarks,
    user.username,
    entryNumber
  );
  return getGateEntry(entryNumber);
}

export function updateGateEntryStatus(entryNumber, { action, reason, user }) {
  const db = getDb();
  const row = db.prepare('SELECT * FROM gate_entries WHERE entry_number = ?').get(entryNumber);
  if (!row) throw new Error('Gate entry not found');

  let status = row.status;
  let outTime = row.out_time;

  switch (action) {
    case 'checkout':
      status = 'Completed';
      outTime = new Date().toISOString();
      break;
    case 'cancel':
      status = 'Cancelled';
      break;
    case 'hold':
      status = 'On Hold';
      break;
    case 'release':
      status = 'Open';
      break;
    default:
      throw new Error('Unknown action');
  }

  db.prepare(
    `UPDATE gate_entries SET status = ?, out_time = ?, modified_by = ?, modified_at = datetime('now')
     WHERE entry_number = ?`
  ).run(status, outTime, user.username, entryNumber);

  logActivity({
    entityType: 'GATE',
    entityRef: entryNumber,
    action,
    details: reason || status,
    byUser: user.username,
  });
  return getGateEntry(entryNumber);
}
