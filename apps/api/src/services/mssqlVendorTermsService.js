import { sql } from '../db/mssql.js';

export function computeLineGst(amount, { igst = 0, cgst = 0, sgst = 0, discAmount = 0 } = {}) {
  const base = Math.max(0, Number(amount || 0) - Number(discAmount || 0));
  const igstRate = Number(igst || 0);
  const cgstRate = Number(cgst || 0);
  const sgstRate = Number(sgst || 0);
  if (igstRate > 0) {
    return {
      igstRate,
      igstAmount: +((base * igstRate) / 100).toFixed(2),
      cgstRate: 0,
      cgstAmount: 0,
      sgstRate: 0,
      sgstAmount: 0,
    };
  }
  return {
    igstRate: 0,
    igstAmount: 0,
    cgstRate,
    cgstAmount: +((base * cgstRate) / 100).toFixed(2),
    sgstRate,
    sgstAmount: +((base * sgstRate) / 100).toFixed(2),
  };
}

export async function getVendorTaxTerms(vendorCode, tx) {
  const code = String(vendorCode || '').trim();
  const empty = {
    igst: 0,
    cgst: 0,
    sgst: 0,
    paymentTerm: null,
    deliveryTerm: null,
    transactionType: null,
    natureTransaction: null,
    natureSupply: null,
    incoTerm: null,
    incoDetails: null,
    fxRate: 1,
    currency: 'INR',
  };
  if (!code) return empty;

  let details = null;
  try {
    const req = new sql.Request(tx);
    req.input('VendorCode', sql.NVarChar, code);
    const result = await req.query(
      `SELECT TOP 1 * FROM VendorDetails WHERE VendorCode = @VendorCode`
    );
    details = result.recordset[0] || null;
  } catch {
    details = null;
  }

  let fxRate = 1;
  try {
    const fx = new sql.Request(tx);
    const result = await fx.query(`
      SELECT TOP 1 Rate FROM POCurrencyRate WHERE Currency = 'INR' ORDER BY ID DESC
    `);
    if (result.recordset[0]?.Rate) fxRate = Number(result.recordset[0].Rate) || 1;
  } catch {
    fxRate = 1;
  }

  return {
    igst: Number(details?.IGST || 0),
    cgst: Number(details?.CGST || 0),
    sgst: Number(details?.SGST || 0),
    paymentTerm: details?.PaymentTerms || null,
    deliveryTerm: details?.DeliveryTerms || null,
    transactionType: details?.TransactionType || null,
    natureTransaction: details?.NatureTransaction || null,
    natureSupply: details?.NatureSupply || null,
    incoTerm: details?.Incoterms || null,
    incoDetails: details?.Blank || null,
    fxRate,
    currency: 'INR',
  };
}

export async function upsertPoReport(tx, { poRef, terms, invoiceTotal = 0 }) {
  try {
    const existing = new sql.Request(tx);
    existing.input('PONo', sql.NVarChar, poRef);
    const found = await existing.query(`SELECT TOP 1 ID FROM POReport WHERE PONo = @PONo`);
    const req = new sql.Request(tx);
    req.input('PONo', sql.NVarChar, poRef);
    req.input('TransactionType', sql.NVarChar, terms.transactionType || null);
    req.input('NatureofTransaction', sql.NVarChar, terms.natureTransaction || null);
    req.input('NatureofSupply', sql.NVarChar, terms.natureSupply || null);
    req.input('DeliveryTerm', sql.NVarChar, terms.deliveryTerm || null);
    req.input('PaymentTerm', sql.NVarChar, terms.paymentTerm || null);
    req.input('Incoterm', sql.NVarChar, terms.incoTerm || null);
    req.input('Incodetails', sql.NVarChar, terms.incoDetails || null);
    req.input('InvoiceTotal', sql.Float, invoiceTotal);
    if (found.recordset[0]) {
      await req.query(`
        UPDATE POReport SET
          TransactionType = @TransactionType,
          NatureofTransaction = @NatureofTransaction,
          NatureofSupply = @NatureofSupply,
          DeliveryTerm = @DeliveryTerm,
          PaymentTerm = @PaymentTerm,
          Incoterm = @Incoterm,
          Incodetails = @Incodetails,
          InvoiceTotal = @InvoiceTotal
        WHERE PONo = @PONo
      `);
    } else {
      await req.query(`
        INSERT INTO POReport
          (PONo, TransactionType, NatureofTransaction, NatureofSupply, DeliveryTerm,
           PaymentTerm, Incoterm, Incodetails, InvoiceTotal, AmountPaid, BalanceDue)
        VALUES
          (@PONo, @TransactionType, @NatureofTransaction, @NatureofSupply, @DeliveryTerm,
           @PaymentTerm, @Incoterm, @Incodetails, @InvoiceTotal, 0, @InvoiceTotal)
      `);
    }
  } catch {
    // POReport may be missing on older DBs; GST on PurchaseOrder still applies.
  }
}

export async function computeItemDiff(tx, itemCode, unitPrice) {
  try {
    const req = new sql.Request(tx);
    req.input('ItemCode', sql.NVarChar, itemCode);
    const result = await req.query(`
      SELECT TOP 1 UnitCost, FixedCost FROM ItemMaster WHERE ItemCode = @ItemCode
    `);
    const row = result.recordset[0];
    const baseline = Number(row?.UnitCost || row?.FixedCost || 0);
    const price = Number(unitPrice || 0);
    if (!baseline) {
      return { standardCost: baseline, diffInPer: 0, diffInRs: 0, latestPurchasePrice: price };
    }
    const diffInRs = +(price - baseline).toFixed(2);
    const diffInPer = +(((price - baseline) / baseline) * 100).toFixed(2);
    return {
      standardCost: baseline,
      diffInPer,
      diffInRs,
      latestPurchasePrice: price,
    };
  } catch {
    return { standardCost: 0, diffInPer: 0, diffInRs: 0, latestPurchasePrice: Number(unitPrice || 0) };
  }
}
