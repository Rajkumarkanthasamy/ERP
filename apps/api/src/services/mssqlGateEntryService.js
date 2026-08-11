import { getMssqlPool, mssqlQuery, sql } from '../db/mssql.js';

function normalizeLines(payload) {
  const source = Array.isArray(payload.lines) ? payload.lines : [];
  return source
    .map((line) => ({
      itemCode: String(line.itemCode || '').trim(),
      itemDescription: String(line.itemDescription || '').trim(),
      quantity: Number(line.quantity || 0),
    }))
    .filter((line) => line.itemCode || line.itemDescription || line.quantity);
}

export function validateLegacyGatePayload(payload) {
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

export function groupLegacyGateRows(rows) {
  const groups = new Map();
  for (const row of rows) {
    if (!groups.has(row.entryNumber)) {
      groups.set(row.entryNumber, {
        id: row.id,
        entryNumber: row.entryNumber,
        entryType: row.entryType,
        documentType: row.documentType,
        documentNo: row.documentNo,
        vendorName: row.vendorName,
        invoiceNo: row.invoiceNo,
        invoiceDate: row.invoiceDate,
        vehicleNo: row.vehicleNo,
        ewayBillNo: row.ewayBillNo,
        ewayBillDate: row.ewayBillDate,
        lrPodNo: row.lrPodNo,
        lrPodDate: row.lrPodDate,
        remarks: row.remarks,
        createdBy: row.createdBy,
        createdAt: row.createdAt,
        status: 'Recorded',
        lines: [],
        totalQuantity: 0,
      });
    }
    const group = groups.get(row.entryNumber);
    group.lines.push({
      id: row.id,
      itemCode: row.itemCode,
      itemDescription: row.itemDescription,
      quantity: Number(row.quantity || 0),
    });
    group.totalQuantity += Number(row.quantity || 0);
  }
  return [...groups.values()].map((group) => ({
    ...group,
    lineCount: group.lines.length,
    itemCode: group.lines[0]?.itemCode || '',
    itemDescription: group.lines[0]?.itemDescription || '',
    quantity: group.lines[0]?.quantity || 0,
    purpose: group.documentType,
  }));
}

export async function listGateEntries({ status, type, entryType, q } = {}) {
  if (status && !['All', 'Recorded', 'Open'].includes(status)) return [];
  const requestedType = entryType || type || 'All';
  const search = String(q || '').trim();
  const result = await mssqlQuery(
    `
    SELECT TOP 2000 *
    FROM (
    SELECT
      i.id,
      i.InwardNo AS entryNumber,
      CASE WHEN i.InwardItemType = 'Manual' THEN 'Manual' ELSE 'Inward' END AS entryType,
      i.InwardItemType AS documentType,
      i.InwardDocumentNo AS documentNo,
      i.ItemCode AS itemCode,
      i.ItemDescription AS itemDescription,
      i.RecievingQuantity AS quantity,
      i.VendorName AS vendorName,
      i.InvoiceNo AS invoiceNo,
      i.InvoiceDate AS invoiceDate,
      CAST(NULL AS NVARCHAR(80)) AS vehicleNo,
      CAST(NULL AS NVARCHAR(80)) AS ewayBillNo,
      CAST(NULL AS NVARCHAR(80)) AS ewayBillDate,
      CAST(NULL AS NVARCHAR(80)) AS lrPodNo,
      CAST(NULL AS NVARCHAR(80)) AS lrPodDate,
      i.Remarks AS remarks,
      i.InwardBy AS createdBy,
      i.InwardDatetime AS createdAt
    FROM SecurityInward i
    WHERE
      (
        @entryType = 'All'
        OR (@entryType = 'Manual' AND i.InwardItemType = 'Manual')
        OR (@entryType = 'Inward' AND ISNULL(i.InwardItemType, '') <> 'Manual')
      )
      AND (
        @q = ''
        OR i.InwardNo LIKE @likeQ
        OR ISNULL(i.InwardDocumentNo, '') LIKE @likeQ
        OR ISNULL(i.VendorName, '') LIKE @likeQ
        OR ISNULL(i.InvoiceNo, '') LIKE @likeQ
      )

    UNION ALL

    SELECT
      o.id,
      o.OutwardNo AS entryNumber,
      'Outward' AS entryType,
      o.OutwardItemType AS documentType,
      o.OutwardDocumentNo AS documentNo,
      o.ItemCode AS itemCode,
      o.ItemDescription AS itemDescription,
      o.SendingQuantity AS quantity,
      o.VendorName AS vendorName,
      CAST(NULL AS NVARCHAR(50)) AS invoiceNo,
      CAST(NULL AS NVARCHAR(80)) AS invoiceDate,
      o.VehicleNO AS vehicleNo,
      o.EWBNO AS ewayBillNo,
      o.EWBDate AS ewayBillDate,
      o.LRPODNo AS lrPodNo,
      o.LRPODDate AS lrPodDate,
      o.Remarks AS remarks,
      o.OutwardBy AS createdBy,
      o.OutwardDatetime AS createdAt
    FROM SecurityOutward o
    WHERE @entryType IN ('All', 'Outward')
      AND (
        @q = ''
        OR o.OutwardNo LIKE @likeQ
        OR ISNULL(o.OutwardDocumentNo, '') LIKE @likeQ
        OR ISNULL(o.VendorName, '') LIKE @likeQ
        OR ISNULL(o.VehicleNO, '') LIKE @likeQ
        OR ISNULL(o.LRPODNo, '') LIKE @likeQ
      )
    ) gateRows
    ORDER BY createdAt DESC, id DESC
    `,
    {
      entryType: requestedType,
      q: search,
      likeQ: `%${search}%`,
    }
  );
  return groupLegacyGateRows(result.recordset);
}

export async function getGateEntry(entryNumber) {
  const rows = await listGateEntries({ q: entryNumber });
  return rows.find((row) => row.entryNumber === entryNumber) || null;
}

async function nextSequence(transaction, entryType) {
  const outward = entryType === 'Outward';
  const table = outward ? 'SecurityOutward' : 'SecurityInward';
  const column = outward ? 'OutwardSlNo' : 'InwardSlNo';
  const request = new sql.Request(transaction);
  const result = await request.query(`
    SELECT ISNULL(MAX(${column}), 20200000) + 1 AS nextSequence
    FROM ${table} WITH (UPDLOCK, HOLDLOCK)
  `);
  return Number(result.recordset[0].nextSequence);
}

export async function createGateEntry(payload, user) {
  const { entryType, lines } = validateLegacyGatePayload(payload);
  const pool = await getMssqlPool();
  const transaction = new sql.Transaction(pool);
  await transaction.begin(sql.ISOLATION_LEVEL.SERIALIZABLE);
  try {
    const sequence = await nextSequence(transaction, entryType);
    const outward = entryType === 'Outward';
    const entryNumber = `${outward ? 'SO' : 'SI'}${sequence}`;
    const byUser = user.displayName || user.username;
    const documentType = entryType === 'Manual' ? 'Manual' : payload.documentType.trim();

    for (const line of lines) {
      const request = new sql.Request(transaction);
      request.input('EntryNumber', sql.NVarChar, entryNumber);
      request.input('ByUser', sql.NVarChar, byUser);
      request.input('DocumentType', sql.NVarChar, documentType);
      request.input('DocumentNo', sql.NVarChar, payload.documentNo.trim());
      request.input('ItemCode', sql.NVarChar, line.itemCode || null);
      request.input('ItemDescription', sql.NVarChar, line.itemDescription);
      request.input('Quantity', sql.Float, line.quantity);
      request.input('Sequence', sql.Int, sequence);
      request.input('Remarks', sql.NVarChar, payload.remarks || null);
      request.input('VendorName', sql.NVarChar, payload.vendorName.trim());

      if (outward) {
        request.input('EwayBillNo', sql.NVarChar, payload.ewayBillNo || null);
        request.input('EwayBillDate', sql.NVarChar, payload.ewayBillDate || null);
        request.input('VehicleNo', sql.NVarChar, payload.vehicleNo.trim());
        request.input('LrPodNo', sql.NVarChar, payload.lrPodNo.trim());
        request.input('LrPodDate', sql.NVarChar, payload.lrPodDate);
        await request.query(`
          INSERT INTO SecurityOutward
            (OutwardNo, OutwardBy, OutwardItemType, OutwardDocumentNo, ItemCode,
             ItemDescription, SendingQuantity, OutwardSlNo, Remarks, VendorName,
             EWBNO, EWBDate, VehicleNO, LRPODNo, LRPODDate)
          VALUES
            (@EntryNumber, @ByUser, @DocumentType, @DocumentNo, @ItemCode,
             @ItemDescription, @Quantity, @Sequence, @Remarks, @VendorName,
             @EwayBillNo, @EwayBillDate, @VehicleNo, @LrPodNo, @LrPodDate)
        `);
      } else {
        request.input('InvoiceNo', sql.NVarChar, payload.invoiceNo || null);
        request.input('InvoiceDate', sql.NVarChar, payload.invoiceDate || null);
        await request.query(`
          INSERT INTO SecurityInward
            (InwardNo, InwardBy, InwardItemType, InwardDocumentNo, ItemCode,
             ItemDescription, RecievingQuantity, InwardSlNo, Remarks, VendorName,
             InvoiceNo, InvoiceDate)
          VALUES
            (@EntryNumber, @ByUser, @DocumentType, @DocumentNo, @ItemCode,
             @ItemDescription, @Quantity, @Sequence, @Remarks, @VendorName,
             @InvoiceNo, @InvoiceDate)
        `);
      }
    }

    const audit = new sql.Request(transaction);
    audit.input('EntryNumber', sql.NVarChar, entryNumber);
    audit.input('ByUser', sql.NVarChar, byUser);
    audit.input('Action', sql.NVarChar, outward ? 'Item Outward' : 'Item Inward');
    audit.input('TableName', sql.NVarChar, outward ? 'SecurityOutward' : 'SecurityInward');
    await audit.query(`
      INSERT INTO ERPTransactionLog
        (TransactionNumber, TransactionBy, TransactionDate, Action, TableName, TransactionDateTime)
      VALUES
        (@EntryNumber, @ByUser, CONVERT(NVARCHAR(30), GETDATE(), 120),
         @Action, @TableName, GETDATE())
    `);

    await transaction.commit();
    return getGateEntry(entryNumber);
  } catch (error) {
    await transaction.rollback();
    throw error;
  }
}
