import { getDb } from '../db/connection.js';
import { logActivity } from './helpers.js';
import { genNumber } from './entityHelpers.js';

function mapEnquiry(row) {
  if (!row) return null;
  return {
    id: row.id,
    enquiryNumber: row.enquiry_number,
    customerCode: row.customer_code,
    customerName: row.customer_name,
    subject: row.subject,
    enquiryDate: row.enquiry_date,
    source: row.source,
    status: row.status,
    assignedTo: row.assigned_to,
    remarks: row.remarks,
    createdBy: row.created_by,
    createdAt: row.created_at,
    modifiedBy: row.modified_by,
    modifiedAt: row.modified_at,
  };
}

function mapOpportunity(row) {
  if (!row) return null;
  return {
    id: row.id,
    opportunityNumber: row.opportunity_number,
    enquiryNumber: row.enquiry_number,
    customerCode: row.customer_code,
    customerName: row.customer_name,
    title: row.title,
    stage: row.stage,
    expectedValue: row.expected_value,
    probability: row.probability,
    expectedCloseDate: row.expected_close_date,
    status: row.status,
    owner: row.owner,
    remarks: row.remarks,
    createdBy: row.created_by,
    createdAt: row.created_at,
    modifiedBy: row.modified_by,
    modifiedAt: row.modified_at,
  };
}

function mapQuote(row) {
  if (!row) return null;
  return {
    id: row.id,
    quoteNumber: row.quote_number,
    opportunityNumber: row.opportunity_number,
    customerCode: row.customer_code,
    customerName: row.customer_name,
    quoteDate: row.quote_date,
    validUntil: row.valid_until,
    totalAmount: row.total_amount,
    currency: row.currency,
    status: row.status,
    approvedBy: row.approved_by,
    approvedDate: row.approved_date,
    remarks: row.remarks,
    createdBy: row.created_by,
    createdAt: row.created_at,
    modifiedBy: row.modified_by,
    modifiedAt: row.modified_at,
  };
}

export function listEnquiries({ status, q } = {}) {
  const db = getDb();
  const where = ['1=1'];
  const params = [];
  if (status && status !== 'All') {
    where.push('status = ?');
    params.push(status);
  }
  if (q) {
    where.push('(enquiry_number LIKE ? OR IFNULL(customer_name,"") LIKE ? OR IFNULL(subject,"") LIKE ?)');
    params.push(`%${q}%`, `%${q}%`, `%${q}%`);
  }
  return db
    .prepare(`SELECT * FROM sales_enquiries WHERE ${where.join(' AND ')} ORDER BY id DESC`)
    .all(...params)
    .map(mapEnquiry);
}

export function getEnquiry(enquiryNumber) {
  return mapEnquiry(getDb().prepare('SELECT * FROM sales_enquiries WHERE enquiry_number = ?').get(enquiryNumber));
}

export function createEnquiry(payload, user) {
  const db = getDb();
  const enquiryNumber = payload.enquiryNumber || genNumber('ENQ-', 'sales_enquiries', 'enquiry_number');
  db.prepare(
    `INSERT INTO sales_enquiries (
      enquiry_number, customer_code, customer_name, subject, source, status, assigned_to, remarks, created_by
    ) VALUES (?, ?, ?, ?, ?, 'Open', ?, ?, ?)`
  ).run(
    enquiryNumber,
    payload.customerCode || null,
    payload.customerName || null,
    payload.subject || null,
    payload.source || null,
    payload.assignedTo || null,
    payload.remarks || null,
    user.username
  );
  logActivity({
    entityType: 'ENQUIRY',
    entityRef: enquiryNumber,
    action: 'Created',
    details: payload.subject || '',
    byUser: user.username,
  });
  return getEnquiry(enquiryNumber);
}

export function updateEnquiry(enquiryNumber, payload, user) {
  const db = getDb();
  const existing = db.prepare('SELECT * FROM sales_enquiries WHERE enquiry_number = ?').get(enquiryNumber);
  if (!existing) throw new Error('Enquiry not found');
  db.prepare(
    `UPDATE sales_enquiries SET customer_code = ?, customer_name = ?, subject = ?, source = ?,
      status = ?, assigned_to = ?, remarks = ?, modified_by = ?, modified_at = datetime('now')
     WHERE enquiry_number = ?`
  ).run(
    payload.customerCode ?? existing.customer_code,
    payload.customerName ?? existing.customer_name,
    payload.subject ?? existing.subject,
    payload.source ?? existing.source,
    payload.status ?? existing.status,
    payload.assignedTo ?? existing.assigned_to,
    payload.remarks ?? existing.remarks,
    user.username,
    enquiryNumber
  );
  return getEnquiry(enquiryNumber);
}

export function listOpportunities({ status, q } = {}) {
  const db = getDb();
  const where = ['1=1'];
  const params = [];
  if (status && status !== 'All') {
    where.push('status = ?');
    params.push(status);
  }
  if (q) {
    where.push('(opportunity_number LIKE ? OR IFNULL(title,"") LIKE ? OR IFNULL(customer_name,"") LIKE ?)');
    params.push(`%${q}%`, `%${q}%`, `%${q}%`);
  }
  return db
    .prepare(`SELECT * FROM opportunities WHERE ${where.join(' AND ')} ORDER BY id DESC`)
    .all(...params)
    .map(mapOpportunity);
}

export function getOpportunity(opportunityNumber) {
  return mapOpportunity(
    getDb().prepare('SELECT * FROM opportunities WHERE opportunity_number = ?').get(opportunityNumber)
  );
}

export function createOpportunity(payload, user) {
  const db = getDb();
  const opportunityNumber =
    payload.opportunityNumber || genNumber('OPP-', 'opportunities', 'opportunity_number');
  db.prepare(
    `INSERT INTO opportunities (
      opportunity_number, enquiry_number, customer_code, customer_name, title, stage,
      expected_value, probability, expected_close_date, status, owner, remarks, created_by
    ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, 'Open', ?, ?, ?)`
  ).run(
    opportunityNumber,
    payload.enquiryNumber || null,
    payload.customerCode || null,
    payload.customerName || null,
    payload.title || null,
    payload.stage || 'Qualification',
    payload.expectedValue || 0,
    payload.probability || 0,
    payload.expectedCloseDate || null,
    payload.owner || user.displayName || user.username,
    payload.remarks || null,
    user.username
  );
  logActivity({
    entityType: 'OPP',
    entityRef: opportunityNumber,
    action: 'Created',
    details: payload.title || '',
    byUser: user.username,
  });
  return getOpportunity(opportunityNumber);
}

export function updateOpportunity(opportunityNumber, payload, user) {
  const db = getDb();
  const existing = db.prepare('SELECT * FROM opportunities WHERE opportunity_number = ?').get(opportunityNumber);
  if (!existing) throw new Error('Opportunity not found');
  db.prepare(
    `UPDATE opportunities SET enquiry_number = ?, customer_code = ?, customer_name = ?, title = ?, stage = ?,
      expected_value = ?, probability = ?, expected_close_date = ?, status = ?, owner = ?, remarks = ?,
      modified_by = ?, modified_at = datetime('now')
     WHERE opportunity_number = ?`
  ).run(
    payload.enquiryNumber ?? existing.enquiry_number,
    payload.customerCode ?? existing.customer_code,
    payload.customerName ?? existing.customer_name,
    payload.title ?? existing.title,
    payload.stage ?? existing.stage,
    payload.expectedValue ?? existing.expected_value,
    payload.probability ?? existing.probability,
    payload.expectedCloseDate ?? existing.expected_close_date,
    payload.status ?? existing.status,
    payload.owner ?? existing.owner,
    payload.remarks ?? existing.remarks,
    user.username,
    opportunityNumber
  );
  return getOpportunity(opportunityNumber);
}

export function listQuotes({ status, q } = {}) {
  const db = getDb();
  const where = ['1=1'];
  const params = [];
  if (status && status !== 'All') {
    where.push('status = ?');
    params.push(status);
  }
  if (q) {
    where.push('(quote_number LIKE ? OR IFNULL(customer_name,"") LIKE ?)');
    params.push(`%${q}%`, `%${q}%`);
  }
  return db
    .prepare(`SELECT * FROM quotes WHERE ${where.join(' AND ')} ORDER BY id DESC`)
    .all(...params)
    .map(mapQuote);
}

export function getQuote(quoteNumber) {
  const db = getDb();
  const header = mapQuote(db.prepare('SELECT * FROM quotes WHERE quote_number = ?').get(quoteNumber));
  if (!header) return null;
  const lines = db
    .prepare(
      `SELECT id, quote_number AS quoteNumber, product_code AS productCode, description,
              quantity, unit_price AS unitPrice, amount, remarks
       FROM quote_details WHERE quote_number = ? ORDER BY id`
    )
    .all(quoteNumber);
  return { ...header, lines };
}

export function createQuote(payload, user) {
  const db = getDb();
  const quoteNumber = payload.quoteNumber || genNumber('QT-', 'quotes', 'quote_number');
  const lines = payload.lines || [];
  const total = lines.reduce((s, l) => s + Number(l.quantity || 0) * Number(l.unitPrice || 0), 0);

  const tx = db.transaction(() => {
    const info = db
      .prepare(
        `INSERT INTO quotes (
          quote_number, opportunity_number, customer_code, customer_name, valid_until,
          total_amount, currency, status, remarks, created_by
        ) VALUES (?, ?, ?, ?, ?, ?, ?, 'Draft', ?, ?)`
      )
      .run(
        quoteNumber,
        payload.opportunityNumber || null,
        payload.customerCode || null,
        payload.customerName || null,
        payload.validUntil || null,
        total || payload.totalAmount || 0,
        payload.currency || 'INR',
        payload.remarks || null,
        user.username
      );

    const insert = db.prepare(
      `INSERT INTO quote_details (quote_id, quote_number, product_code, description, quantity, unit_price, amount, remarks)
       VALUES (?, ?, ?, ?, ?, ?, ?, ?)`
    );
    for (const line of lines) {
      const qty = Number(line.quantity || 1);
      const price = Number(line.unitPrice || 0);
      insert.run(
        info.lastInsertRowid,
        quoteNumber,
        line.productCode || null,
        line.description || null,
        qty,
        price,
        qty * price,
        line.remarks || null
      );
    }
  });
  tx();

  logActivity({
    entityType: 'QUOTE',
    entityRef: quoteNumber,
    action: 'Created',
    details: `₹${total}`,
    byUser: user.username,
  });
  return getQuote(quoteNumber);
}

export function updateQuote(quoteNumber, payload, user) {
  const db = getDb();
  const existing = db.prepare('SELECT * FROM quotes WHERE quote_number = ?').get(quoteNumber);
  if (!existing) throw new Error('Quote not found');
  if (!['Draft', 'Open'].includes(existing.status)) throw new Error('Only Draft quotes can be updated');

  let total = existing.total_amount;
  if (payload.lines) {
    total = payload.lines.reduce((s, l) => s + Number(l.quantity || 0) * Number(l.unitPrice || 0), 0);
  } else if (payload.totalAmount != null) {
    total = Number(payload.totalAmount);
  }

  db.prepare(
    `UPDATE quotes SET opportunity_number = ?, customer_code = ?, customer_name = ?, valid_until = ?,
      total_amount = ?, currency = ?, remarks = ?, modified_by = ?, modified_at = datetime('now')
     WHERE quote_number = ?`
  ).run(
    payload.opportunityNumber ?? existing.opportunity_number,
    payload.customerCode ?? existing.customer_code,
    payload.customerName ?? existing.customer_name,
    payload.validUntil ?? existing.valid_until,
    total,
    payload.currency ?? existing.currency,
    payload.remarks ?? existing.remarks,
    user.username,
    quoteNumber
  );

  if (payload.lines) {
    db.prepare('DELETE FROM quote_details WHERE quote_number = ?').run(quoteNumber);
    const insert = db.prepare(
      `INSERT INTO quote_details (quote_id, quote_number, product_code, description, quantity, unit_price, amount, remarks)
       VALUES (?, ?, ?, ?, ?, ?, ?, ?)`
    );
    for (const line of payload.lines) {
      const qty = Number(line.quantity || 1);
      const price = Number(line.unitPrice || 0);
      insert.run(
        existing.id,
        quoteNumber,
        line.productCode || null,
        line.description || null,
        qty,
        price,
        qty * price,
        line.remarks || null
      );
    }
  }
  return getQuote(quoteNumber);
}

export function updateQuoteStatus(quoteNumber, { action, reason, user }) {
  const db = getDb();
  const row = db.prepare('SELECT * FROM quotes WHERE quote_number = ?').get(quoteNumber);
  if (!row) throw new Error('Quote not found');

  let status = row.status;
  let approvedBy = row.approved_by;
  let approvedDate = row.approved_date;

  switch (action) {
    case 'submit':
      status = 'Submitted';
      break;
    case 'approve':
      status = 'Approved';
      approvedBy = user.displayName || user.username;
      approvedDate = new Date().toISOString();
      break;
    case 'reject':
      status = 'Rejected';
      break;
    case 'win':
      status = 'Won';
      break;
    case 'lose':
      status = 'Lost';
      break;
    default:
      throw new Error('Unknown action');
  }

  db.prepare(
    `UPDATE quotes SET status = ?, approved_by = ?, approved_date = ?,
      modified_by = ?, modified_at = datetime('now') WHERE quote_number = ?`
  ).run(status, approvedBy, approvedDate, user.username, quoteNumber);

  logActivity({
    entityType: 'QUOTE',
    entityRef: quoteNumber,
    action,
    details: reason || status,
    byUser: user.username,
  });
  return getQuote(quoteNumber);
}
