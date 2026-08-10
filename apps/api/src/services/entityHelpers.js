import { getDb } from '../db/connection.js';

/** Generate PREFIX-YYYY-##### using calendar year */
export function genNumber(prefix, table, column, width = 5) {
  const db = getDb();
  const year = new Date().getFullYear();
  const fullPrefix = `${prefix}${year}-`;
  const row = db
    .prepare(`SELECT ${column} AS num FROM ${table} WHERE ${column} LIKE ? ORDER BY ${column} DESC LIMIT 1`)
    .get(`${fullPrefix}%`);
  let n = 1;
  if (row?.num) {
    const last = String(row.num).split('-').pop();
    n = (parseInt(last, 10) || 0) + 1;
  }
  return `${fullPrefix}${String(n).padStart(width, '0')}`;
}
