import { sql } from '../db/mssql.js';

export function formatLegacyDate(date = new Date()) {
  const d = date instanceof Date ? date : new Date(date);
  const day = String(d.getDate()).padStart(2, '0');
  const month = String(d.getMonth() + 1).padStart(2, '0');
  const year = d.getFullYear();
  return `${day}-${month}-${year}`;
}

export function formatBatchCode(date = new Date()) {
  const d = date instanceof Date ? date : new Date(date);
  return `${String(d.getDate()).padStart(2, '0')}${String(d.getMonth() + 1).padStart(2, '0')}${d.getFullYear()}`;
}

export function buildLegacyGinNumber(id) {
  return `ITWGIN${id}`;
}

export function receiptUnitCost(unitPrice, fxRate) {
  const price = Number(unitPrice || 0);
  const rate = Number(fxRate || 0) || 1;
  return price / rate;
}

/**
 * Allocate next JobMovement ID and insert the GIN header inside an open transaction.
 * Returns { ginId, legacyGinNumber }.
 */
export async function allocateLegacyGin(tx, { poRef, invoiceNo, totalCost, userName, ginDate = new Date() }) {
  const invoice = String(invoiceNo || '').trim();
  if (!invoice) throw new Error('Invoice number is required for live GIN posting');
  if (!poRef) throw new Error('PO reference is required for live GIN posting');

  const lock = new sql.Request(tx);
  const next = await lock.query(`
    SELECT ISNULL(MAX(ID), 0) + 1 AS nextId
    FROM JobMovement WITH (UPDLOCK, HOLDLOCK)
  `);
  const ginId = Number(next.recordset[0].nextId);
  const legacyGinNumber = buildLegacyGinNumber(ginId);
  const dateText = formatLegacyDate(ginDate);
  const byUser = String(userName || 'web').slice(0, 32);

  const insert = new sql.Request(tx);
  insert.input('WONumber', sql.NVarChar, String(poRef).slice(0, 61));
  insert.input('BillNo', sql.NVarChar, invoice.slice(0, 32));
  insert.input('GINDate', sql.NVarChar, dateText);
  insert.input('Cost', sql.Float, Number(totalCost || 0));
  insert.input('JobMovedDate', sql.NVarChar, dateText);
  insert.input('JobMovedBy', sql.NVarChar, byUser);
  insert.input('ID', sql.Int, ginId);
  await insert.query(`
    INSERT INTO JobMovement (WONumber, BillNo, GINDate, Cost, JobMovedDate, JobMovedBy, ID)
    VALUES (@WONumber, @BillNo, @GINDate, @Cost, @JobMovedDate, @JobMovedBy, @ID)
  `);

  return { ginId, legacyGinNumber, ginDate: dateText };
}

export async function assertInvoiceAvailable(tx, { vendorCode, invoiceNo }) {
  const invoice = String(invoiceNo || '').trim();
  const vendor = String(vendorCode || '').trim();
  if (!invoice || !vendor) return;
  const req = new sql.Request(tx);
  req.input('VendorCode', sql.NVarChar, vendor);
  req.input('InvoiceNo', sql.NVarChar, invoice.slice(0, 32));
  const existing = await req.query(`
    SELECT TOP 1 SupplierInvoiceNumber
    FROM Receipt
    WHERE VendorCode = @VendorCode AND SupplierInvoiceNumber = @InvoiceNo
  `);
  if (existing.recordset[0]) {
    throw new Error(`Invoice ${invoice} is already posted for vendor ${vendor}`);
  }
}

export async function resolveVendorName(tx, vendorCode) {
  const code = String(vendorCode || '').trim();
  if (!code) return 'Unknown Vendor';
  const req = new sql.Request(tx);
  req.input('VendorCode', sql.NVarChar, code);
  const result = await req.query(
    `SELECT TOP 1 VendorName FROM Vendors WHERE VendorCode = @VendorCode`
  );
  return result.recordset[0]?.VendorName || code;
}

/**
 * Post one PO receipt line into Receipt + ERPInventoryLogs and bump ItemMaster qty.
 * Does not recalculate WAR (v1): keeps existing UnitCost and updates stock counters only.
 */
export async function postLegacyReceiptLine(
  tx,
  {
    po,
    receivedQty,
    legacyGinNumber,
    ginDate,
    invoiceNo,
    invoiceDate,
    supplierName,
    batchCode,
    itemLocation,
  }
) {
  const qty = Number(receivedQty);
  if (!Number.isFinite(qty) || qty <= 0) throw new Error('Received quantity must be positive');

  const itemReq = new sql.Request(tx);
  itemReq.input('ItemCode', sql.NVarChar, po.ItemCode);
  const itemResult = await itemReq.query(`
    SELECT TOP 1
      ItemCode,
      ItemDescription,
      ISNULL(AvailableQty, 0) AS AvailableQty,
      ISNULL(Receipt, 0) AS Receipt,
      ISNULL(UnitCost, 0) AS UnitCost,
      ISNULL(Location, '') AS Location,
      ISNULL(HSNSACCode, '') AS HSNSACCode,
      ISNULL(Type, '') AS Type
    FROM ItemMaster
    WHERE ItemCode = @ItemCode
  `);
  const item = itemResult.recordset[0];
  if (!item) throw new Error(`Item ${po.ItemCode} was not found in ItemMaster`);

  const fxRate = Number(po.FXRate || po.FxRate || 0) || 1;
  const unitCost = receiptUnitCost(po.UnitPrice, fxRate);
  const dateText = ginDate || formatLegacyDate();
  const invoice = String(invoiceNo || '').trim().slice(0, 32);
  const location = String(itemLocation || item.Location || 'NOLOCATION').slice(0, 256) || 'NOLOCATION';
  const batch = String(batchCode || formatBatchCode()).slice(0, 256);
  const projectCode = String(po.ProjectCode || '').slice(0, 255);
  const supplier = String(supplierName || 'Unknown Vendor').slice(0, 255);

  const receipt = new sql.Request(tx);
  receipt.input('ItemCode', sql.NVarChar, po.ItemCode);
  receipt.input('BOMProjectCode', sql.VarChar, projectCode);
  receipt.input('ProductNo', sql.Int, 0);
  receipt.input('GINNumber', sql.NVarChar, legacyGinNumber);
  receipt.input('Quantity', sql.Float, qty);
  receipt.input('UnitCost', sql.Float, unitCost);
  receipt.input('SupplierName', sql.NVarChar, supplier);
  receipt.input('Date', sql.NVarChar, dateText);
  receipt.input('SupplierInvoiceNumber', sql.NVarChar, invoice);
  receipt.input('Type', sql.NVarChar, 'Boughtout');
  receipt.input('RefNumber', sql.NVarChar, String(po.DBOMNo || '').slice(0, 32));
  receipt.input('InvoiceDate', sql.NVarChar, invoiceDate || dateText);
  receipt.input('VendorCode', sql.NVarChar, String(po.VendorCode || '').slice(0, 30));
  receipt.input('IGSTRate', sql.Float, Number(po.IGSTRate || 0));
  receipt.input('IGSTAmount', sql.Float, 0);
  receipt.input('SGSTRate', sql.Float, Number(po.SGSTRate || 0));
  receipt.input('SGSTAmount', sql.Float, 0);
  receipt.input('CGSTRate', sql.Float, Number(po.CGSTRate || 0));
  receipt.input('CGSTAmount', sql.Float, 0);
  receipt.input('Ledger', sql.NVarChar, String(item.Type || '').slice(0, 255));
  receipt.input('HSNCode', sql.NVarChar, String(item.HSNSACCode || '').slice(0, 100));
  receipt.input('Currency', sql.NVarChar, po.Currency || null);
  receipt.input('FxRate', sql.Float, fxRate);
  receipt.input('POAmount', sql.Float, Number(po.Amount || 0));
  await receipt.query(`
    INSERT INTO Receipt (
      ItemCode, BOMProjectCode, ProductNo, GINNumber, Quantity, UnitCost, SupplierName,
      Date, SupplierInvoiceNumber, Type, RefNumber, InvoiceDate, VendorCode,
      IGSTRate, IGSTAmount, SGSTRate, SGSTAmount, CGSTRate, CGSTAmount,
      Ledger, HSNCode, DiscRate, DiscAmount, TCSAmount,
      CDPercent, CDValue, CDCessRate, CDCessAmount, CDIGSTRate, CDIGSTAmount,
      PackingForwarding, TransportationFreight, Currency, FxRate, POAmount
    ) VALUES (
      @ItemCode, @BOMProjectCode, @ProductNo, @GINNumber, @Quantity, @UnitCost, @SupplierName,
      @Date, @SupplierInvoiceNumber, @Type, @RefNumber, @InvoiceDate, @VendorCode,
      @IGSTRate, @IGSTAmount, @SGSTRate, @SGSTAmount, @CGSTRate, @CGSTAmount,
      @Ledger, @HSNCode, 0, 0, 0,
      0, 0, 0, 0, 0, 0,
      0, 0, @Currency, @FxRate, @POAmount
    )
  `);

  const inv = new sql.Request(tx);
  inv.input('ItemCode', sql.NVarChar, po.ItemCode);
  inv.input('ItemDescription', sql.NVarChar, String(item.ItemDescription || '').slice(0, 300));
  inv.input('GINNumber', sql.NVarChar, legacyGinNumber);
  inv.input('CreatedDate', sql.NVarChar, dateText);
  inv.input('POWONumber', sql.NVarChar, String(po.DBOMNo || '').slice(0, 256));
  inv.input('ProjectCode', sql.NVarChar, projectCode.slice(0, 256) || 'NA');
  inv.input('ReceivedQtY', sql.Float, qty);
  inv.input('RemainingQty', sql.Float, qty);
  inv.input('BatchCode', sql.NVarChar, batch);
  inv.input('ItemLocation', sql.NVarChar, location);
  inv.input('VendorCode', sql.VarChar, String(po.VendorCode || '').slice(0, 50));
  inv.input('UnitCost', sql.Float, unitCost);
  inv.input('SGST', sql.Float, Number(po.SGSTAmount || 0));
  inv.input('CGST', sql.Float, Number(po.CGSTAmount || 0));
  inv.input('IGST', sql.Float, Number(po.IGSTAmount || 0));
  inv.input('POAmount', sql.Float, Number(po.Amount || 0));
  inv.input('POQty', sql.Float, Number(po.RequariedQty || 0));
  await inv.query(`
    INSERT INTO ERPInventoryLogs (
      ItemCode, ItemDescription, GINNumber, CreatedDate, POWONumber, ProjectCode,
      ReceivedQtY, RemainingQty, IssuedQty, BatchCode, ItemLocation, VendorCode,
      UnitCost, SGST, CGST, IGST, POAmount, POQty
    ) VALUES (
      @ItemCode, @ItemDescription, @GINNumber, @CreatedDate, @POWONumber, @ProjectCode,
      @ReceivedQtY, @RemainingQty, 0, @BatchCode, @ItemLocation, @VendorCode,
      @UnitCost, @SGST, @CGST, @IGST, @POAmount, @POQty
    )
  `);

  const availableQty = Number(item.AvailableQty || 0) + qty;
  const receiptQty = Number(item.Receipt || 0) + qty;
  const stockValue = Number(item.UnitCost || 0) * availableQty;
  const stock = new sql.Request(tx);
  stock.input('ItemCode', sql.NVarChar, po.ItemCode);
  stock.input('CreatedDate', sql.NVarChar, dateText);
  stock.input('AvailableQty', sql.Float, availableQty);
  stock.input('Receipt', sql.Float, receiptQty);
  stock.input('StockValue', sql.Float, stockValue);
  await stock.query(`
    UPDATE ItemMaster
    SET CreatedDate = @CreatedDate,
        AvailableQty = @AvailableQty,
        Receipt = @Receipt,
        StockValue = @StockValue
    WHERE ItemCode = @ItemCode
  `);

  return {
    itemCode: po.ItemCode,
    quantity: qty,
    unitCost,
    availableQty,
    location,
    batchCode: batch,
  };
}

export async function writeGinAuditLog(tx, { legacyGinNumber, userName, ginDate }) {
  const audit = new sql.Request(tx);
  audit.input('TransactionNumber', sql.NVarChar, legacyGinNumber);
  audit.input('TransactionBy', sql.NVarChar, String(userName || 'web').slice(0, 256));
  audit.input('TransactionDate', sql.NVarChar, ginDate || formatLegacyDate());
  audit.input('Action', sql.NVarChar, 'Item GIN');
  audit.input('TableName', sql.NVarChar, 'Receipt');
  await audit.query(`
    INSERT INTO ERPTransactionLog
      (TransactionNumber, TransactionBy, TransactionDate, Action, TableName, TransactionDateTime)
    VALUES
      (@TransactionNumber, @TransactionBy, @TransactionDate, @Action, @TableName, GETDATE())
  `);
}
