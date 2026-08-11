import { getDb } from '../db/connection.js';
import { logActivity } from './helpers.js';
import { genNumber } from './entityHelpers.js';

function mapEntry(row, lines = []) {
  if (!row) return null;
  const firstLine = lines[0] || {};
  return {
    id: row.id,
    entryNumber: row.entry_number,
    entryType: row.entry_type === 'Inbound' ? 'Inward' : row.entry_type,
    vehicleNo: row.vehicle_no,
    transporter: row.transporter,
    vendorCode: row.vendor_code,
    vendorName: row.vendor_name,
    customerCode: row.customer_code,
    customerName: row.customer_name,
    purpose: row.purpose || row.document_type,
    documentType: row.document_type || row.purpose,
    documentNo: row.document_no || row.invoice_no,
    invoiceNo: row.invoice_no,
    invoiceDate: row.invoice_date,
    ewayBillNo: row.eway_bill_no,
    ewayBillDate: row.eway_bill_date,
    lrPodNo: row.lr_pod_no,
    lrPodDate: row.lr_pod_date,
    status: row.status,
    inTime: row.in_time,
    outTime: row.out_time,
    remarks: row.remarks,
    createdBy: row.created_by,
    createdAt: row.created_at,
    modifiedBy: row.modified_by,
    modifiedAt: row.modified_at,
    lines,
    lineCount: lines.length,
    totalQuantity: lines.reduce((sum, line) => sum + Number(line.quantity || 0), 0),
    itemCode: firstLine.itemCode || '',
    itemDescription: firstLine.itemDescription || '',
    quantity: firstLine.quantity || 0,
  };
}

function listLines(db, entryNumber) {
  return db
    .prepare(
      `SELECT id, item_code AS itemCode, item_description AS itemDescription, quantity
       FROM gate_entry_details WHERE entry_number = ? ORDER BY id`
    )
    .all(entryNumber);
}

function normalizeLines(payload) {
  return (Array.isArray(payload.lines) ? payload.lines : [])
    .map((line) => ({
      itemCode: String(line.itemCode || '').trim(),
      itemDescription: String(line.itemDescription || '').trim(),
      quantity: Number(line.quantity || 0),
    }))
    .filter((line) => line.itemCode || line.itemDescription || line.quantity);
}

function validatePayload(payload) {
  const entryType = payload.entryType || 'Inward';
  if (!['Inward', 'Outward', 'Manual'].includes(entryType)) {
    throw new Error('Entry type must be Inward, Outward, or Manual');
  }
  if (!payload.documentType?.trim()) throw new Error('Document type is required');
  if (!payload.documentNo?.trim()) throw new Error('Document number is required');
  if (!payload.vendorName?.trim()) throw new Error('Party / vendor name is required');
  if (entryType === 'Inward' && !payload.invoiceNo?.trim()) {
    throw new Error('Invoice number is required for gate inward');
  }
  if (entryType === 'Outward') {
    if (!payload.vehicleNo?.trim()) throw new Error('Vehicle number is required');
    if (!payload.lrPodNo?.trim()) throw new Error('LR / POD number is required');
    if (!payload.lrPodDate) throw new Error('LR / POD date is required');
  }
  const lines = normalizeLines(payload);
  if (!lines.length) throw new Error('At least one item line is required');
  if (
    lines.some(
      (line) =>
        !line.itemDescription ||
        !Number.isFinite(line.quantity) ||
        line.quantity <= 0
    )
  ) {
    throw new Error('Every gate item requires a description and positive quantity');
  }
  return { entryType, lines };
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
      `(entry_number LIKE ? OR IFNULL(vehicle_no, '') LIKE ? OR IFNULL(vendor_name, '') LIKE ? OR IFNULL(invoice_no, '') LIKE ? OR IFNULL(document_no, '') LIKE ?)`
    );
    params.push(`%${q}%`, `%${q}%`, `%${q}%`, `%${q}%`, `%${q}%`);
  }
  const rows = db
    .prepare(`SELECT * FROM gate_entries WHERE ${where.join(' AND ')} ORDER BY id DESC`)
    .all(...params);
  return rows.map((row) => mapEntry(row, listLines(db, row.entry_number)));
}

export function getGateEntry(entryNumber) {
  const db = getDb();
  return mapEntry(
    db.prepare('SELECT * FROM gate_entries WHERE entry_number = ?').get(entryNumber),
    listLines(db, entryNumber)
  );
}

export function createGateEntry(payload, user) {
  const { entryType, lines } = validatePayload(payload);
  const db = getDb();
  const entryNumber = payload.entryNumber || genNumber('GE-', 'gate_entries', 'entry_number');
  const transaction = db.transaction(() => {
    const info = db.prepare(
      `INSERT INTO gate_entries (
        entry_number, entry_type, vehicle_no, transporter, vendor_code, vendor_name,
        customer_code, customer_name, purpose, document_type, document_no, invoice_no,
        invoice_date, eway_bill_no, eway_bill_date, lr_pod_no, lr_pod_date,
        status, remarks, created_by
      ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, 'Recorded', ?, ?)`
    ).run(
      entryNumber,
      entryType,
      payload.vehicleNo || null,
      payload.transporter || null,
      payload.vendorCode || null,
      payload.vendorName.trim(),
      payload.customerCode || null,
      payload.customerName || null,
      payload.documentType,
      entryType === 'Manual' ? 'Manual' : payload.documentType,
      payload.documentNo,
      payload.invoiceNo || null,
      payload.invoiceDate || null,
      payload.ewayBillNo || null,
      payload.ewayBillDate || null,
      payload.lrPodNo || null,
      payload.lrPodDate || null,
      payload.remarks || null,
      user.username
    );
    const insertLine = db.prepare(
      `INSERT INTO gate_entry_details
        (gate_entry_id, entry_number, item_code, item_description, quantity)
       VALUES (?, ?, ?, ?, ?)`
    );
    lines.forEach((line) => {
      insertLine.run(
        info.lastInsertRowid,
        entryNumber,
        line.itemCode || null,
        line.itemDescription,
        line.quantity
      );
    });
    logActivity({
      entityType: 'GATE',
      entityRef: entryNumber,
      action: 'Created',
      details: `${entryType}: ${payload.documentNo}`,
      byUser: user.username,
    });
  });
  transaction();
  return getGateEntry(entryNumber);
}

export function updateGateEntry(entryNumber, payload, user) {
  const db = getDb();
  const existing = db.prepare('SELECT * FROM gate_entries WHERE entry_number = ?').get(entryNumber);
  if (!existing) throw new Error('Gate entry not found');
  const current = getGateEntry(entryNumber);
  const merged = {
    ...current,
    ...payload,
    entryType: payload.entryType ?? existing.entry_type,
    documentType:
      payload.documentType ?? existing.document_type ?? existing.purpose,
    documentNo: payload.documentNo ?? existing.document_no ?? existing.invoice_no,
    vendorName: payload.vendorName ?? existing.vendor_name,
    lines: payload.lines ?? current.lines,
  };
  const { entryType, lines } = validatePayload(merged);
  const transaction = db.transaction(() => {
    db.prepare(
      `UPDATE gate_entries SET entry_type = ?, vehicle_no = ?, transporter = ?, vendor_code = ?,
        vendor_name = ?, customer_code = ?, customer_name = ?, purpose = ?, document_type = ?,
        document_no = ?, invoice_no = ?, invoice_date = ?, eway_bill_no = ?,
        eway_bill_date = ?, lr_pod_no = ?, lr_pod_date = ?, remarks = ?,
        modified_by = ?, modified_at = datetime('now')
       WHERE entry_number = ?`
    ).run(
      entryType,
      merged.vehicleNo || null,
      merged.transporter || null,
      merged.vendorCode || null,
      merged.vendorName,
      merged.customerCode || null,
      merged.customerName || null,
      merged.documentType,
      entryType === 'Manual' ? 'Manual' : merged.documentType,
      merged.documentNo,
      merged.invoiceNo || null,
      merged.invoiceDate || null,
      merged.ewayBillNo || null,
      merged.ewayBillDate || null,
      merged.lrPodNo || null,
      merged.lrPodDate || null,
      merged.remarks || null,
      user.username,
      entryNumber
    );
    db.prepare('DELETE FROM gate_entry_details WHERE entry_number = ?').run(entryNumber);
    const insertLine = db.prepare(
      `INSERT INTO gate_entry_details
        (gate_entry_id, entry_number, item_code, item_description, quantity)
       VALUES (?, ?, ?, ?, ?)`
    );
    lines.forEach((line) =>
      insertLine.run(existing.id, entryNumber, line.itemCode || null, line.itemDescription, line.quantity)
    );
  });
  transaction();
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
