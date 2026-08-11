import { getMssqlPool, mssqlQuery, sql } from '../db/mssql.js';
import { formatLegacyDate } from './mssqlGinBridgeService.js';

export function issueDelta(txnType, quantity) {
  const qty = Math.abs(Number(quantity || 0));
  if (!Number.isFinite(qty) || qty <= 0) throw new Error('quantity must be positive');
  if (txnType === 'Issue') return -qty;
  if (txnType === 'Return') return qty;
  throw new Error('Only Issue and Return are mapped for live stock posting');
}

async function nextTxnNumber(tx, prefix) {
  const req = new sql.Request(tx);
  const result = await req.query(`
    SELECT ISNULL(MAX(TRY_CAST(REPLACE(TransactionNumber, '${prefix}', '') AS INT)), 0) + 1 AS nextId
    FROM ERPTransactionLog
    WHERE TransactionNumber LIKE '${prefix}%'
  `);
  const id = Number(result.recordset[0]?.nextId || 1);
  return `${prefix}${id}`;
}

async function writeAudit(tx, { txnNumber, userName, action, dateText }) {
  const audit = new sql.Request(tx);
  audit.input('TransactionNumber', sql.NVarChar, txnNumber);
  audit.input('TransactionBy', sql.NVarChar, String(userName || 'web').slice(0, 256));
  audit.input('TransactionDate', sql.NVarChar, dateText);
  audit.input('Action', sql.NVarChar, action);
  audit.input('TableName', sql.NVarChar, 'ERPInventoryLogs');
  await audit.query(`
    INSERT INTO ERPTransactionLog
      (TransactionNumber, TransactionBy, TransactionDate, Action, TableName, TransactionDateTime)
    VALUES
      (@TransactionNumber, @TransactionBy, @TransactionDate, @Action, @TableName, GETDATE())
  `);
}

/**
 * FIFO issue: consume RemainingQty from ERPInventoryLogs oldest-first.
 * Return: add RemainingQty back onto the newest matching project/item batch when possible.
 */
async function applyFifoIssue(tx, { itemCode, quantity, projectCode }) {
  const req = new sql.Request(tx);
  req.input('ItemCode', sql.NVarChar, itemCode);
  const lots = await req.query(`
    SELECT Id, ISNULL(RemainingQty, 0) AS RemainingQty, ISNULL(IssuedQty, 0) AS IssuedQty,
           ISNULL(ProjectCode, '') AS ProjectCode, GINNumber
    FROM ERPInventoryLogs
    WHERE ItemCode = @ItemCode AND ISNULL(RemainingQty, 0) > 0
    ORDER BY Id ASC
  `);
  let remaining = quantity;
  const consumed = [];
  for (const lot of lots.recordset) {
    if (remaining <= 0) break;
    if (projectCode && lot.ProjectCode && lot.ProjectCode !== projectCode && lot.ProjectCode !== 'NA') {
      continue;
    }
    const take = Math.min(Number(lot.RemainingQty), remaining);
    const upd = new sql.Request(tx);
    upd.input('Id', sql.Int, lot.Id);
    upd.input('RemainingQty', sql.Float, Number(lot.RemainingQty) - take);
    upd.input('IssuedQty', sql.Float, Number(lot.IssuedQty) + take);
    await upd.query(`
      UPDATE ERPInventoryLogs
      SET RemainingQty = @RemainingQty, IssuedQty = @IssuedQty
      WHERE Id = @Id
    `);
    consumed.push({ id: lot.Id, ginNumber: lot.GINNumber, quantity: take });
    remaining -= take;
  }
  if (remaining > 0.0001) {
    throw new Error(`Insufficient FIFO stock for ${itemCode}; short by ${remaining}`);
  }
  return consumed;
}

async function applyReturnToLots(tx, { itemCode, quantity, projectCode, referenceNo }) {
  const req = new sql.Request(tx);
  req.input('ItemCode', sql.NVarChar, itemCode);
  if (projectCode) req.input('ProjectCode', sql.NVarChar, projectCode);

  if (referenceNo) {
    req.input('GINNumber', sql.NVarChar, String(referenceNo).slice(0, 256));
    const exact = await req.query(`
      SELECT TOP 1 Id, ISNULL(RemainingQty, 0) AS RemainingQty, ISNULL(IssuedQty, 0) AS IssuedQty
      FROM ERPInventoryLogs
      WHERE ItemCode = @ItemCode AND GINNumber = @GINNumber
      ORDER BY Id DESC
    `);
    if (exact.recordset[0]) {
      const lot = exact.recordset[0];
      const upd = new sql.Request(tx);
      upd.input('Id', sql.Int, lot.Id);
      upd.input('RemainingQty', sql.Float, Number(lot.RemainingQty) + quantity);
      upd.input('IssuedQty', sql.Float, Math.max(0, Number(lot.IssuedQty) - quantity));
      await upd.query(`
        UPDATE ERPInventoryLogs
        SET RemainingQty = @RemainingQty, IssuedQty = @IssuedQty
        WHERE Id = @Id
      `);
      return [{ id: lot.Id, quantity }];
    }
  }

  const lots = await req.query(`
    SELECT TOP 1 Id, ISNULL(RemainingQty, 0) AS RemainingQty, ISNULL(IssuedQty, 0) AS IssuedQty
    FROM ERPInventoryLogs
    WHERE ItemCode = @ItemCode
      ${projectCode ? "AND (ProjectCode = @ProjectCode OR ProjectCode = 'NA' OR ProjectCode IS NULL)" : ''}
    ORDER BY Id DESC
  `);

  if (!lots.recordset[0]) {
    // No prior GIN lot — create a return lot so AvailableQty can still move.
    const ins = new sql.Request(tx);
    ins.input('ItemCode', sql.NVarChar, itemCode);
    ins.input('ProjectCode', sql.NVarChar, String(projectCode || 'NA').slice(0, 256));
    ins.input('Qty', sql.Float, quantity);
    ins.input('CreatedDate', sql.NVarChar, formatLegacyDate());
    await ins.query(`
      INSERT INTO ERPInventoryLogs (
        ItemCode, ItemDescription, GINNumber, CreatedDate, POWONumber, ProjectCode,
        ReceivedQtY, RemainingQty, IssuedQty, BatchCode, ItemLocation, VendorCode,
        UnitCost, SGST, CGST, IGST, POAmount, POQty
      )
      SELECT TOP 1
        ItemCode, ItemDescription, 'RETURN', @CreatedDate, 'RETURN', @ProjectCode,
        @Qty, @Qty, 0, 'RETURN', ISNULL(Location, 'MAIN'), '',
        ISNULL(UnitCost, 0), 0, 0, 0, 0, 0
      FROM ItemMaster WHERE ItemCode = @ItemCode
    `);
    return [{ id: null, quantity }];
  }

  const lot = lots.recordset[0];
  const upd = new sql.Request(tx);
  upd.input('Id', sql.Int, lot.Id);
  upd.input('RemainingQty', sql.Float, Number(lot.RemainingQty) + quantity);
  upd.input('IssuedQty', sql.Float, Math.max(0, Number(lot.IssuedQty) - quantity));
  await upd.query(`
    UPDATE ERPInventoryLogs
    SET RemainingQty = @RemainingQty, IssuedQty = @IssuedQty
    WHERE Id = @Id
  `);
  return [{ id: lot.Id, quantity }];
}

export async function listLedger(q) {
  const params = {};
  let where = '1=1';
  if (q) {
    where = `(ItemCode LIKE @q OR ISNULL(ItemDescription, '') LIKE @q)`;
    params.q = `%${q}%`;
  }
  const result = await mssqlQuery(
    `
    SELECT TOP 1000
      ItemCode AS itemCode,
      ItemDescription AS itemDescription,
      Units AS uom,
      ISNULL(AvailableQty, 0) AS quantityOnHand,
      0 AS reservedQty,
      ISNULL(Location, 'MAIN') AS location,
      CreatedDate AS lastTxnDate,
      CreatedDate AS updatedAt
    FROM ItemMaster
    WHERE ${where}
    ORDER BY ItemCode
    `,
    params
  );
  return result.recordset;
}

export async function listTransactions({ type, q } = {}) {
  const params = {};
  const where = [`Action IN ('Item Issue', 'Item Return')`];
  if (type === 'Issue') where[0] = `Action = 'Item Issue'`;
  if (type === 'Return') where[0] = `Action = 'Item Return'`;
  if (q) {
    where.push(`(TransactionNumber LIKE @q OR TransactionBy LIKE @q)`);
    params.q = `%${q}%`;
  }
  const result = await mssqlQuery(
    `
    SELECT TOP 500
      TransactionNumber AS txnNumber,
      CASE WHEN Action = 'Item Issue' THEN 'Issue' ELSE 'Return' END AS txnType,
      TransactionBy AS createdBy,
      TransactionDate AS createdAt,
      Action AS remarks,
      'Posted' AS status
    FROM ERPTransactionLog
    WHERE ${where.join(' AND ')}
    ORDER BY TransactionDateTime DESC
    `,
    params
  );
  return result.recordset;
}

export async function createTransaction(payload, user) {
  const txnType = payload.txnType;
  const itemCode = String(payload.itemCode || '').trim();
  if (!itemCode) throw new Error('itemCode is required');
  const delta = issueDelta(txnType, payload.quantity);
  const qty = Math.abs(delta);
  const projectCode = String(payload.projectCode || '').trim();
  const referenceNo = String(payload.referenceNo || '').trim();
  const byUser = user?.displayName || user?.username || 'web';
  const dateText = formatLegacyDate();

  const pool = await getMssqlPool();
  const tx = new sql.Transaction(pool);
  await tx.begin();
  try {
    const itemReq = new sql.Request(tx);
    itemReq.input('ItemCode', sql.NVarChar, itemCode);
    const itemResult = await itemReq.query(`
      SELECT TOP 1 ItemCode, ItemDescription, Units, ISNULL(AvailableQty, 0) AS AvailableQty,
             ISNULL(UnitCost, 0) AS UnitCost, ISNULL(Location, 'MAIN') AS Location
      FROM ItemMaster WHERE ItemCode = @ItemCode
    `);
    const item = itemResult.recordset[0];
    if (!item) throw new Error(`Item ${itemCode} not found`);

    const available = Number(item.AvailableQty || 0);
    if (txnType === 'Issue' && available + 1e-9 < qty) {
      throw new Error(`Insufficient AvailableQty for ${itemCode} (on hand ${available})`);
    }

    let lots;
    if (txnType === 'Issue') {
      lots = await applyFifoIssue(tx, { itemCode, quantity: qty, projectCode });
    } else {
      lots = await applyReturnToLots(tx, { itemCode, quantity: qty, projectCode, referenceNo });
    }

    const newAvailable = available + delta;
    const stock = new sql.Request(tx);
    stock.input('ItemCode', sql.NVarChar, itemCode);
    stock.input('AvailableQty', sql.Float, newAvailable);
    stock.input('StockValue', sql.Float, Number(item.UnitCost || 0) * newAvailable);
    stock.input('CreatedDate', sql.NVarChar, dateText);
    await stock.query(`
      UPDATE ItemMaster
      SET AvailableQty = @AvailableQty, StockValue = @StockValue, CreatedDate = @CreatedDate
      WHERE ItemCode = @ItemCode
    `);

    const prefix = txnType === 'Issue' ? 'ISS' : 'RET';
    const txnNumber = payload.txnNumber || (await nextTxnNumber(tx, prefix));
    await writeAudit(tx, {
      txnNumber,
      userName: byUser,
      action: txnType === 'Issue' ? 'Item Issue' : 'Item Return',
      dateText,
    });

    await tx.commit();
    return {
      txnNumber,
      txnType,
      itemCode,
      itemDescription: item.ItemDescription,
      quantity: Number(payload.quantity),
      uom: payload.uom || item.Units || 'NOS',
      projectCode: projectCode || null,
      referenceNo: referenceNo || null,
      fromLocation: payload.fromLocation || item.Location,
      toLocation: payload.toLocation || null,
      remarks: payload.remarks || null,
      status: 'Posted',
      createdBy: byUser,
      createdAt: new Date().toISOString(),
      availableQty: newAvailable,
      lots,
    };
  } catch (err) {
    await tx.rollback();
    throw err;
  }
}
