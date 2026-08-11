import { getDb } from '../db/connection.js';
import { logActivity } from './helpers.js';
import { genNumber } from './entityHelpers.js';

const TXN_TYPES = ['GIN', 'Issue', 'Return', 'Adjust', 'Production', 'Transfer', 'ReverseGIN'];

function mapTxn(row) {
  if (!row) return null;
  return {
    id: row.id,
    txnNumber: row.txn_number,
    txnType: row.txn_type,
    itemCode: row.item_code,
    itemDescription: row.item_description,
    quantity: row.quantity,
    uom: row.uom,
    projectCode: row.project_code,
    referenceNo: row.reference_no,
    fromLocation: row.from_location,
    toLocation: row.to_location,
    remarks: row.remarks,
    status: row.status,
    createdBy: row.created_by,
    createdAt: row.created_at,
  };
}

function mapLedger(row) {
  if (!row) return null;
  return {
    id: row.id,
    itemCode: row.item_code,
    itemDescription: row.item_description,
    uom: row.uom,
    quantityOnHand: row.quantity_on_hand,
    reservedQty: row.reserved_qty,
    location: row.location,
    lastTxnDate: row.last_txn_date,
    updatedAt: row.updated_at,
  };
}

export function listLedger(q) {
  const db = getDb();
  const rows = q
    ? db
        .prepare(
          `SELECT * FROM stock_ledger
           WHERE item_code LIKE ? OR IFNULL(item_description,'') LIKE ?
           ORDER BY item_code`
        )
        .all(`%${q}%`, `%${q}%`)
    : db.prepare('SELECT * FROM stock_ledger ORDER BY item_code').all();
  return rows.map(mapLedger);
}

export function listTransactions({ type, q } = {}) {
  const db = getDb();
  const where = ['1=1'];
  const params = [];
  if (type && type !== 'All') {
    where.push('txn_type = ?');
    params.push(type);
  }
  if (q) {
    where.push('(txn_number LIKE ? OR item_code LIKE ? OR IFNULL(reference_no,"") LIKE ?)');
    params.push(`%${q}%`, `%${q}%`, `%${q}%`);
  }
  return db
    .prepare(`SELECT * FROM stock_transactions WHERE ${where.join(' AND ')} ORDER BY id DESC`)
    .all(...params)
    .map(mapTxn);
}

export function getTransaction(txnNumber) {
  return mapTxn(getDb().prepare('SELECT * FROM stock_transactions WHERE txn_number = ?').get(txnNumber));
}

function applyLedgerDelta(db, { itemCode, itemDescription, uom, qtyDelta, location }) {
  const existing = db.prepare('SELECT * FROM stock_ledger WHERE item_code = ?').get(itemCode);
  if (existing) {
    db.prepare(
      `UPDATE stock_ledger SET quantity_on_hand = quantity_on_hand + ?, item_description = COALESCE(?, item_description),
        uom = COALESCE(?, uom), location = COALESCE(?, location), last_txn_date = datetime('now'),
        updated_at = datetime('now')
       WHERE item_code = ?`
    ).run(qtyDelta, itemDescription || null, uom || null, location || null, itemCode);
  } else {
    db.prepare(
      `INSERT INTO stock_ledger (item_code, item_description, uom, quantity_on_hand, location, last_txn_date)
       VALUES (?, ?, ?, ?, ?, datetime('now'))`
    ).run(itemCode, itemDescription || null, uom || 'NOS', qtyDelta, location || 'MAIN');
  }
}

export function createTransaction(payload, user) {
  if (!payload.txnType || !TXN_TYPES.includes(payload.txnType)) {
    throw new Error(`txnType must be one of: ${TXN_TYPES.join(', ')}`);
  }
  if (!payload.itemCode) throw new Error('itemCode is required');
  if (!payload.quantity && payload.quantity !== 0) throw new Error('quantity is required');

  const db = getDb();
  const inbound = ['GIN', 'Return', 'Production', 'Adjust'].includes(payload.txnType);
  const outbound = ['Issue', 'Transfer', 'ReverseGIN'].includes(payload.txnType);
  // Adjust can be signed via quantity; Production/GIN/Return add; Issue/Transfer/ReverseGIN subtract
  let delta = Number(payload.quantity);
  if (outbound && payload.txnType !== 'Adjust') delta = -Math.abs(delta);
  else if (inbound && payload.txnType !== 'Adjust') delta = Math.abs(delta);

  const prefixMap = {
    GIN: 'GIN-',
    Issue: 'ISS-',
    Return: 'RET-',
    Adjust: 'ADJ-',
    Production: 'PRD-',
    Transfer: 'TRF-',
    ReverseGIN: 'RGIN-',
  };
  const txnNumber = payload.txnNumber || genNumber(prefixMap[payload.txnType], 'stock_transactions', 'txn_number');

  const tx = db.transaction(() => {
    db.prepare(
      `INSERT INTO stock_transactions (
        txn_number, txn_type, item_code, item_description, quantity, uom, project_code,
        reference_no, from_location, to_location, remarks, status, created_by
      ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, 'Posted', ?)`
    ).run(
      txnNumber,
      payload.txnType,
      payload.itemCode,
      payload.itemDescription || null,
      payload.quantity,
      payload.uom || 'NOS',
      payload.projectCode || null,
      payload.referenceNo || null,
      payload.fromLocation || null,
      payload.toLocation || null,
      payload.remarks || null,
      user.username
    );

    applyLedgerDelta(db, {
      itemCode: payload.itemCode,
      itemDescription: payload.itemDescription,
      uom: payload.uom,
      qtyDelta: delta,
      location: payload.toLocation || payload.fromLocation || 'MAIN',
    });
  });
  tx();

  logActivity({
    entityType: 'STOCK',
    entityRef: txnNumber,
    action: payload.txnType,
    details: `${payload.itemCode} qty ${payload.quantity}`,
    byUser: user.username,
  });

  return getTransaction(txnNumber);
}

export function getTxnTypes() {
  return TXN_TYPES;
}
