import { mssqlQuery, sql, getMssqlPool } from '../db/mssql.js';

function trim(value) {
  return value == null ? '' : String(value).trim();
}

function asStatus(value) {
  if (value === 0 || value === false || String(value).toLowerCase() === 'inactive') {
    return 'Inactive';
  }
  return 'Active';
}

const MASTER_CONFIG = {
  tax: {
    table: 'TaxMaster',
    codeCol: 'TaxCode',
    nameCol: 'TaxName',
    pctCol: 'TaxPercentage',
    listMap: (r) => ({
      code: r.TaxCode,
      name: r.TaxName,
      percentage: Number(r.TaxPercentage || 0),
      status: r.Status,
      createdBy: r.CreatedBy,
      createdDate: r.CreatedDate,
    }),
  },
  cst: {
    table: 'CSTMaster',
    codeCol: 'CSTCode',
    nameCol: 'CSTName',
    pctCol: 'CSTPercentage',
    listMap: (r) => ({
      code: r.CSTCode,
      name: r.CSTName,
      percentage: Number(r.CSTPercentage || 0),
      status: r.Status,
      createdBy: r.CreatedBy,
      createdDate: r.CreatedDate,
    }),
  },
  'others-tax': {
    table: 'OthersTaxMaster',
    codeCol: 'OTCode',
    nameCol: 'OTName',
    pctCol: 'OTPercentage',
    listMap: (r) => ({
      code: r.OTCode,
      name: r.OTName,
      percentage: Number(r.OTPercentage || 0),
      status: r.Status,
      createdBy: r.CreatedBy,
      createdDate: r.CreatedDate,
    }),
  },
  discount: {
    table: 'DiscountMaster',
    codeCol: 'DiscountCode',
    nameCol: 'DiscountName',
    pctCol: 'DiscountPercentage',
    listMap: (r) => ({
      code: r.DiscountCode,
      name: r.DiscountName,
      percentage: Number(r.DiscountPercentage || 0),
      status: r.Status,
      createdBy: r.CreatedBy,
      createdDate: r.CreatedDate,
    }),
  },
  excise: {
    table: 'ExciseDutyMaster',
    codeCol: 'ExciseDutyCode',
    nameCol: 'ExciseDutyName',
    pctCol: 'ExciseDutyPercentage',
    listMap: (r) => ({
      code: r.ExciseDutyCode,
      name: r.ExciseDutyName,
      percentage: Number(r.ExciseDutyPercentage || 0),
      status: r.Status,
      createdBy: r.CreatedBy,
      createdDate: r.CreatedDate,
    }),
  },
  payment: {
    table: 'PaymentTermMaster',
    codeCol: 'PaymentCode',
    nameCol: 'PaymentTerms',
    pctCol: null,
    listMap: (r) => ({
      code: r.PaymentCode,
      name: r.PaymentTerms,
      status: r.Status,
      createdBy: r.CreatedBy,
      createdDate: r.CreatedDate,
    }),
  },
  delivery: {
    table: 'DeliveryTermMaster',
    codeCol: 'DeliveryCode',
    nameCol: 'DeliveryTerms',
    pctCol: null,
    listMap: (r) => ({
      code: r.DeliveryCode,
      name: r.DeliveryTerms,
      status: r.Status,
      minDays: r.MinDays,
      maxDays: r.MaxDays,
      createdBy: r.CreatedBy,
      createdDate: r.CreatedDate,
    }),
  },
};

export function listMasterTypes() {
  return Object.keys(MASTER_CONFIG);
}

export async function listAdditionalMasters(type, q) {
  const cfg = MASTER_CONFIG[type];
  if (!cfg) throw new Error(`Unknown additional master type: ${type}`);
  const params = {};
  let where = '1=1';
  if (q) {
    where = `(${cfg.nameCol} LIKE @q OR ${cfg.codeCol} LIKE @q)`;
    params.q = `%${q}%`;
  }
  const result = await mssqlQuery(
    `
    SELECT TOP 500 *
    FROM ${cfg.table}
    WHERE ${where}
    ORDER BY ID DESC
    `,
    params
  );
  return result.recordset.map(cfg.listMap);
}

export async function upsertAdditionalMaster(type, payload, user) {
  const cfg = MASTER_CONFIG[type];
  if (!cfg) throw new Error(`Unknown additional master type: ${type}`);
  const name = trim(payload.name || payload[cfg.nameCol]);
  if (!name) throw new Error('Name is required');
  const status = asStatus(payload.status ?? payload.active);
  const byUser = user?.displayName || user?.username || 'web';
  const code = trim(payload.code);
  const percentage = cfg.pctCol == null ? null : Number(payload.percentage ?? 0);

  if (code) {
    const params = { name, status, code };
    if (cfg.pctCol != null) params.percentage = percentage;
    await mssqlQuery(
      `
      UPDATE ${cfg.table}
      SET ${cfg.nameCol} = @name
          ${cfg.pctCol ? `, ${cfg.pctCol} = @percentage` : ''}
          , Status = @status
      WHERE ${cfg.codeCol} = @code
      `,
      params
    );
    const rows = await listAdditionalMasters(type, code);
    return rows.find((r) => r.code === code) || rows[0];
  }

  const params = { name, status, createdBy: byUser };
  if (cfg.pctCol != null) params.percentage = percentage;
  const insertCols =
    cfg.pctCol == null
      ? `(${cfg.nameCol}, CreatedDate, CreatedBy, Status)`
      : `(${cfg.nameCol}, ${cfg.pctCol}, CreatedDate, CreatedBy, Status)`;
  const insertVals =
    cfg.pctCol == null
      ? `(@name, CONVERT(NVARCHAR(30), GETDATE(), 120), @createdBy, @status)`
      : `(@name, @percentage, CONVERT(NVARCHAR(30), GETDATE(), 120), @createdBy, @status)`;
  await mssqlQuery(
    `INSERT INTO ${cfg.table} ${insertCols} VALUES ${insertVals}`,
    params
  );
  const rows = await listAdditionalMasters(type, name);
  return rows.find((r) => r.name === name) || rows[0];
}

export async function listIncoTerms() {
  try {
    const result = await mssqlQuery(`SELECT IncoTerm AS name FROM POProjectIncoTerms ORDER BY ID`);
    return result.recordset;
  } catch {
    return [];
  }
}

export async function listPegRates() {
  try {
    const result = await mssqlQuery(`
      SELECT TOP 100 ID AS id, Currency AS currency, Rate AS rate, Date AS rateDate,
             COUNTRY AS country, [Currency Name] AS currencyName, TO_INR AS toInr
      FROM POCurrencyRate
      ORDER BY ID DESC
    `);
    return result.recordset;
  } catch {
    return [];
  }
}

export async function upsertPegRate(payload, user) {
  const currency = trim(payload.currency);
  const rate = Number(payload.rate);
  if (!currency) throw new Error('Currency is required');
  if (!Number.isFinite(rate) || rate <= 0) throw new Error('Rate must be a positive number');
  const pool = await getMssqlPool();
  const req = pool.request();
  req.input('Currency', sql.VarChar, currency);
  req.input('Rate', sql.Float, rate);
  await req.query(`INSERT INTO POCurrencyRate (Currency, Rate) VALUES (@Currency, @Rate)`);
  return listPegRates();
}
