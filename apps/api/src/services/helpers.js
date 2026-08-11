import { getDb } from '../db/connection.js';

export function nextNumber(prefixPattern, table, column) {
  const db = getDb();
  const now = new Date();
  const yyyy = now.getFullYear();
  const mm = String(now.getMonth() + 1).padStart(2, '0');
  const prefix = prefixPattern
    .replace('YYYY', String(yyyy))
    .replace('MM', mm);
  const row = db
    .prepare(`SELECT ${column} AS num FROM ${table} WHERE ${column} LIKE ? ORDER BY ${column} DESC LIMIT 1`)
    .get(`${prefix}%`);
  let next = 1;
  if (row?.num) {
    const parts = String(row.num).split('-');
    const last = parts[parts.length - 1];
    const n = parseInt(last, 10);
    if (!Number.isNaN(n)) next = n + 1;
  }
  const width = prefixPattern.includes('#######') ? 7 : 5;
  return `${prefix}${String(next).padStart(width, '0')}`;
}

export function logActivity({ entityType, entityRef, action, details, byUser }) {
  const db = getDb();
  db.prepare(
    `INSERT INTO activity_log (entity_type, entity_ref, action, details, by_user)
     VALUES (?, ?, ?, ?, ?)`
  ).run(entityType, entityRef, action, details || '', byUser || '');
}

export function applyGst(amount, { igst = 0, sgst = 9, cgst = 9 } = {}) {
  const base = Number(amount) || 0;
  if (igst > 0) {
    return {
      igst_rate: igst,
      igst_amount: +(base * igst / 100).toFixed(2),
      sgst_rate: 0,
      sgst_amount: 0,
      cgst_rate: 0,
      cgst_amount: 0,
    };
  }
  return {
    igst_rate: 0,
    igst_amount: 0,
    sgst_rate: sgst,
    sgst_amount: +(base * sgst / 100).toFixed(2),
    cgst_rate: cgst,
    cgst_amount: +(base * cgst / 100).toFixed(2),
  };
}
