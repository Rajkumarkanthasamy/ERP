import { getDb } from '../db/connection.js';
import { logActivity } from './helpers.js';

export function listVendors(q) {
  const db = getDb();
  if (q) {
    return db
      .prepare(
        `SELECT id, vendor_code AS vendorCode, vendor_name AS vendorName, city, gstin, address,
                contact_person AS contactPerson, phone, email, active
         FROM vendors
         WHERE active = 1 AND (vendor_code LIKE ? OR vendor_name LIKE ? OR city LIKE ?)
         ORDER BY vendor_name`
      )
      .all(`%${q}%`, `%${q}%`, `%${q}%`);
  }
  return db
    .prepare(
      `SELECT id, vendor_code AS vendorCode, vendor_name AS vendorName, city, gstin, address,
              contact_person AS contactPerson, phone, email, active
       FROM vendors WHERE active = 1 ORDER BY vendor_name`
    )
    .all();
}

export function upsertVendor(payload) {
  const db = getDb();
  if (!payload.vendorCode || !payload.vendorName) throw new Error('vendorCode and vendorName are required');
  const existing = db.prepare('SELECT id FROM vendors WHERE vendor_code = ?').get(payload.vendorCode);
  if (existing) {
    db.prepare(
      `UPDATE vendors SET vendor_name = ?, city = ?, gstin = ?, address = ?, contact_person = ?, phone = ?, email = ?, active = ?
       WHERE vendor_code = ?`
    ).run(
      payload.vendorName,
      payload.city || null,
      payload.gstin || null,
      payload.address || null,
      payload.contactPerson || null,
      payload.phone || null,
      payload.email || null,
      payload.active === 0 ? 0 : 1,
      payload.vendorCode
    );
  } else {
    db.prepare(
      `INSERT INTO vendors (vendor_code, vendor_name, city, gstin, address, contact_person, phone, email, active)
       VALUES (?, ?, ?, ?, ?, ?, ?, ?, 1)`
    ).run(
      payload.vendorCode,
      payload.vendorName,
      payload.city || null,
      payload.gstin || null,
      payload.address || null,
      payload.contactPerson || null,
      payload.phone || null,
      payload.email || null
    );
  }
  return listVendors().find((v) => v.vendorCode === payload.vendorCode);
}

export function listProjects(q) {
  const db = getDb();
  const rows = q
    ? db
        .prepare(
          `SELECT * FROM projects
           WHERE project_code LIKE ? OR project_name LIKE ? OR IFNULL(customer_code,'') LIKE ?
           ORDER BY project_code`
        )
        .all(`%${q}%`, `%${q}%`, `%${q}%`)
    : db.prepare('SELECT * FROM projects ORDER BY project_code').all();
  return rows.map(mapProject);
}

function mapProject(row) {
  if (!row) return null;
  return {
    id: row.id,
    projectCode: row.project_code,
    projectName: row.project_name,
    productNo: row.product_no,
    status: row.status,
    customerCode: row.customer_code,
    pmName: row.pm_name,
    approvalStatus: row.approval_status,
    approvedBy: row.approved_by,
    approvedDate: row.approved_date,
    startDate: row.start_date,
    endDate: row.end_date,
    remarks: row.remarks,
  };
}

export function getProject(projectCode) {
  return mapProject(getDb().prepare('SELECT * FROM projects WHERE project_code = ?').get(projectCode));
}

export function upsertProject(payload, user) {
  const db = getDb();
  if (!payload.projectCode || !payload.projectName) throw new Error('projectCode and projectName are required');
  const existing = db.prepare('SELECT id FROM projects WHERE project_code = ?').get(payload.projectCode);
  if (existing) {
    db.prepare(
      `UPDATE projects SET project_name = ?, product_no = ?, status = ?, customer_code = ?, pm_name = ?,
        start_date = ?, end_date = ?, remarks = ?
       WHERE project_code = ?`
    ).run(
      payload.projectName,
      payload.productNo || null,
      payload.status || 'Active',
      payload.customerCode || null,
      payload.pmName || null,
      payload.startDate || null,
      payload.endDate || null,
      payload.remarks || null,
      payload.projectCode
    );
  } else {
    db.prepare(
      `INSERT INTO projects (
        project_code, project_name, product_no, status, customer_code, pm_name,
        approval_status, start_date, end_date, remarks
      ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)`
    ).run(
      payload.projectCode,
      payload.projectName,
      payload.productNo || null,
      payload.status || 'Draft',
      payload.customerCode || null,
      payload.pmName || null,
      payload.approvalStatus || 'Pending',
      payload.startDate || null,
      payload.endDate || null,
      payload.remarks || null
    );
    logActivity({
      entityType: 'PROJECT',
      entityRef: payload.projectCode,
      action: 'Created',
      details: payload.projectName,
      byUser: user?.username,
    });
  }
  return getProject(payload.projectCode);
}

export function approveProject(projectCode, { action, reason, user }) {
  const db = getDb();
  const row = db.prepare('SELECT * FROM projects WHERE project_code = ?').get(projectCode);
  if (!row) throw new Error('Project not found');

  if (action === 'approve') {
    db.prepare(
      `UPDATE projects SET approval_status = 'Approved', status = 'Active',
        approved_by = ?, approved_date = datetime('now') WHERE project_code = ?`
    ).run(user.displayName || user.username, projectCode);
  } else if (action === 'reject') {
    db.prepare(
      `UPDATE projects SET approval_status = 'Rejected', status = 'Rejected', remarks = ?
       WHERE project_code = ?`
    ).run(reason || row.remarks || 'Rejected', projectCode);
  } else {
    throw new Error('Unknown action');
  }

  logActivity({
    entityType: 'PROJECT',
    entityRef: projectCode,
    action,
    details: reason || action,
    byUser: user.username,
  });
  return getProject(projectCode);
}

export function listItems(q) {
  const db = getDb();
  if (q) {
    return db
      .prepare(
        `SELECT id, item_code AS itemCode, item_description AS itemDescription, specification, make,
                mfg_part_no AS mfgPartNo, uom, hsn_code AS hsnCode, standard_cost AS standardCost,
                latest_purchase_price AS latestPurchasePrice, category, target_cost AS targetCost,
                is_sales_product AS isSalesProduct
         FROM items
         WHERE active = 1 AND (item_code LIKE ? OR item_description LIKE ? OR make LIKE ?)
         ORDER BY item_code`
      )
      .all(`%${q}%`, `%${q}%`, `%${q}%`);
  }
  return db
    .prepare(
      `SELECT id, item_code AS itemCode, item_description AS itemDescription, specification, make,
              mfg_part_no AS mfgPartNo, uom, hsn_code AS hsnCode, standard_cost AS standardCost,
              latest_purchase_price AS latestPurchasePrice, category, target_cost AS targetCost,
              is_sales_product AS isSalesProduct
       FROM items WHERE active = 1 ORDER BY item_code`
    )
    .all();
}

export function upsertItem(payload) {
  const db = getDb();
  if (!payload.itemCode) throw new Error('itemCode is required');
  const existing = db.prepare('SELECT id FROM items WHERE item_code = ?').get(payload.itemCode);
  if (existing) {
    db.prepare(
      `UPDATE items SET item_description = ?, specification = ?, make = ?, mfg_part_no = ?, uom = ?,
        hsn_code = ?, standard_cost = ?, latest_purchase_price = ?, category = ?, target_cost = ?,
        is_sales_product = ?, active = ?
       WHERE item_code = ?`
    ).run(
      payload.itemDescription || null,
      payload.specification || null,
      payload.make || null,
      payload.mfgPartNo || null,
      payload.uom || 'NOS',
      payload.hsnCode || null,
      payload.standardCost || 0,
      payload.latestPurchasePrice || 0,
      payload.category || 'General',
      payload.targetCost || 0,
      payload.isSalesProduct ? 1 : 0,
      payload.active === 0 ? 0 : 1,
      payload.itemCode
    );
  } else {
    db.prepare(
      `INSERT INTO items (
        item_code, item_description, specification, make, mfg_part_no, uom, hsn_code,
        standard_cost, latest_purchase_price, category, target_cost, is_sales_product, active
      ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, 1)`
    ).run(
      payload.itemCode,
      payload.itemDescription || null,
      payload.specification || null,
      payload.make || null,
      payload.mfgPartNo || null,
      payload.uom || 'NOS',
      payload.hsnCode || null,
      payload.standardCost || 0,
      payload.latestPurchasePrice || 0,
      payload.category || 'General',
      payload.targetCost || 0,
      payload.isSalesProduct ? 1 : 0
    );
  }
  return listItems().find((i) => i.itemCode === payload.itemCode);
}

export function listCities(q) {
  const db = getDb();
  if (q) {
    return db
      .prepare(
        `SELECT id, city_code AS cityCode, city_name AS cityName, state, country, active
         FROM cities WHERE active = 1 AND (city_code LIKE ? OR city_name LIKE ? OR IFNULL(state,'') LIKE ?)
         ORDER BY city_name`
      )
      .all(`%${q}%`, `%${q}%`, `%${q}%`);
  }
  return db
    .prepare(
      `SELECT id, city_code AS cityCode, city_name AS cityName, state, country, active
       FROM cities WHERE active = 1 ORDER BY city_name`
    )
    .all();
}

export function upsertCity(payload) {
  const db = getDb();
  if (!payload.cityCode || !payload.cityName) throw new Error('cityCode and cityName are required');
  const existing = db.prepare('SELECT id FROM cities WHERE city_code = ?').get(payload.cityCode);
  if (existing) {
    db.prepare(
      `UPDATE cities SET city_name = ?, state = ?, country = ?, active = ? WHERE city_code = ?`
    ).run(payload.cityName, payload.state || null, payload.country || 'India', payload.active === 0 ? 0 : 1, payload.cityCode);
  } else {
    db.prepare(
      `INSERT INTO cities (city_code, city_name, state, country, active) VALUES (?, ?, ?, ?, 1)`
    ).run(payload.cityCode, payload.cityName, payload.state || null, payload.country || 'India');
  }
  return listCities().find((c) => c.cityCode === payload.cityCode);
}

export function listCustomers(q) {
  const db = getDb();
  if (q) {
    return db
      .prepare(
        `SELECT id, customer_code AS customerCode, customer_name AS customerName, city_code AS cityCode,
                city, address, gstin, contact_person AS contactPerson, phone, email, active
         FROM customers
         WHERE active = 1 AND (customer_code LIKE ? OR customer_name LIKE ? OR IFNULL(city,'') LIKE ?)
         ORDER BY customer_name`
      )
      .all(`%${q}%`, `%${q}%`, `%${q}%`);
  }
  return db
    .prepare(
      `SELECT id, customer_code AS customerCode, customer_name AS customerName, city_code AS cityCode,
              city, address, gstin, contact_person AS contactPerson, phone, email, active
       FROM customers WHERE active = 1 ORDER BY customer_name`
    )
    .all();
}

export function upsertCustomer(payload) {
  const db = getDb();
  if (!payload.customerCode || !payload.customerName) throw new Error('customerCode and customerName are required');
  const existing = db.prepare('SELECT id FROM customers WHERE customer_code = ?').get(payload.customerCode);
  if (existing) {
    db.prepare(
      `UPDATE customers SET customer_name = ?, city_code = ?, city = ?, address = ?, gstin = ?,
        contact_person = ?, phone = ?, email = ?, active = ?
       WHERE customer_code = ?`
    ).run(
      payload.customerName,
      payload.cityCode || null,
      payload.city || null,
      payload.address || null,
      payload.gstin || null,
      payload.contactPerson || null,
      payload.phone || null,
      payload.email || null,
      payload.active === 0 ? 0 : 1,
      payload.customerCode
    );
  } else {
    db.prepare(
      `INSERT INTO customers (
        customer_code, customer_name, city_code, city, address, gstin, contact_person, phone, email, active
      ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, 1)`
    ).run(
      payload.customerCode,
      payload.customerName,
      payload.cityCode || null,
      payload.city || null,
      payload.address || null,
      payload.gstin || null,
      payload.contactPerson || null,
      payload.phone || null,
      payload.email || null
    );
  }
  return listCustomers().find((c) => c.customerCode === payload.customerCode);
}

export function listAssets(q) {
  const db = getDb();
  if (q) {
    return db
      .prepare(
        `SELECT id, asset_code AS assetCode, asset_name AS assetName, category, location,
                purchase_date AS purchaseDate, purchase_value AS purchaseValue, current_value AS currentValue,
                status, remarks
         FROM assets WHERE asset_code LIKE ? OR asset_name LIKE ? OR IFNULL(category,'') LIKE ?
         ORDER BY asset_code`
      )
      .all(`%${q}%`, `%${q}%`, `%${q}%`);
  }
  return db
    .prepare(
      `SELECT id, asset_code AS assetCode, asset_name AS assetName, category, location,
              purchase_date AS purchaseDate, purchase_value AS purchaseValue, current_value AS currentValue,
              status, remarks
       FROM assets ORDER BY asset_code`
    )
    .all();
}

export function upsertAsset(payload) {
  const db = getDb();
  if (!payload.assetCode || !payload.assetName) throw new Error('assetCode and assetName are required');
  const existing = db.prepare('SELECT id FROM assets WHERE asset_code = ?').get(payload.assetCode);
  if (existing) {
    db.prepare(
      `UPDATE assets SET asset_name = ?, category = ?, location = ?, purchase_date = ?,
        purchase_value = ?, current_value = ?, status = ?, remarks = ?
       WHERE asset_code = ?`
    ).run(
      payload.assetName,
      payload.category || null,
      payload.location || null,
      payload.purchaseDate || null,
      payload.purchaseValue || 0,
      payload.currentValue || 0,
      payload.status || 'Active',
      payload.remarks || null,
      payload.assetCode
    );
  } else {
    db.prepare(
      `INSERT INTO assets (
        asset_code, asset_name, category, location, purchase_date, purchase_value, current_value, status, remarks
      ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)`
    ).run(
      payload.assetCode,
      payload.assetName,
      payload.category || null,
      payload.location || null,
      payload.purchaseDate || null,
      payload.purchaseValue || 0,
      payload.currentValue || 0,
      payload.status || 'Active',
      payload.remarks || null
    );
  }
  return listAssets().find((a) => a.assetCode === payload.assetCode);
}

export function listSalesProducts(q) {
  const db = getDb();
  if (q) {
    return db
      .prepare(
        `SELECT id, product_code AS productCode, product_name AS productName, description, uom,
                list_price AS listPrice, peg_rate AS pegRate, is_spare_part AS isSparePart, active
         FROM sales_products
         WHERE active = 1 AND (product_code LIKE ? OR product_name LIKE ?)
         ORDER BY product_code`
      )
      .all(`%${q}%`, `%${q}%`);
  }
  return db
    .prepare(
      `SELECT id, product_code AS productCode, product_name AS productName, description, uom,
              list_price AS listPrice, peg_rate AS pegRate, is_spare_part AS isSparePart, active
       FROM sales_products WHERE active = 1 ORDER BY product_code`
    )
    .all();
}

export function upsertSalesProduct(payload) {
  const db = getDb();
  if (!payload.productCode || !payload.productName) throw new Error('productCode and productName are required');
  const existing = db.prepare('SELECT id FROM sales_products WHERE product_code = ?').get(payload.productCode);
  if (existing) {
    db.prepare(
      `UPDATE sales_products SET product_name = ?, description = ?, uom = ?, list_price = ?, peg_rate = ?,
        is_spare_part = ?, active = ? WHERE product_code = ?`
    ).run(
      payload.productName,
      payload.description || null,
      payload.uom || 'NOS',
      payload.listPrice || 0,
      payload.pegRate || 0,
      payload.isSparePart ? 1 : 0,
      payload.active === 0 ? 0 : 1,
      payload.productCode
    );
  } else {
    db.prepare(
      `INSERT INTO sales_products (product_code, product_name, description, uom, list_price, peg_rate, is_spare_part, active)
       VALUES (?, ?, ?, ?, ?, ?, ?, 1)`
    ).run(
      payload.productCode,
      payload.productName,
      payload.description || null,
      payload.uom || 'NOS',
      payload.listPrice || 0,
      payload.pegRate || 0,
      payload.isSparePart ? 1 : 0
    );
  }
  return listSalesProducts().find((p) => p.productCode === payload.productCode);
}

function mapBom(row) {
  if (!row) return null;
  return {
    id: row.id,
    bomCode: row.bom_code,
    productCode: row.product_code,
    productName: row.product_name,
    version: row.version,
    status: row.status,
    approvedBy: row.approved_by,
    approvedDate: row.approved_date,
    createdBy: row.created_by,
    createdAt: row.created_at,
    remarks: row.remarks,
  };
}

export function listBoms(status) {
  const db = getDb();
  const rows =
    status && status !== 'All'
      ? db.prepare('SELECT * FROM product_boms WHERE status = ? ORDER BY id DESC').all(status)
      : db.prepare('SELECT * FROM product_boms ORDER BY id DESC').all();
  return rows.map(mapBom);
}

export function getBom(bomCode) {
  const db = getDb();
  const header = mapBom(db.prepare('SELECT * FROM product_boms WHERE bom_code = ?').get(bomCode));
  if (!header) return null;
  const lines = db
    .prepare(
      `SELECT id, bom_code AS bomCode, item_code AS itemCode, item_description AS itemDescription,
              quantity, uom, remarks
       FROM product_bom_details WHERE bom_code = ? ORDER BY id`
    )
    .all(bomCode);
  return { ...header, lines };
}

export function createBom(payload, user) {
  const db = getDb();
  if (!payload.bomCode || !payload.productCode) throw new Error('bomCode and productCode are required');
  const tx = db.transaction(() => {
    db.prepare(
      `INSERT INTO product_boms (bom_code, product_code, product_name, version, status, created_by, remarks)
       VALUES (?, ?, ?, ?, 'Draft', ?, ?)`
    ).run(
      payload.bomCode,
      payload.productCode,
      payload.productName || null,
      payload.version || '1.0',
      user.username,
      payload.remarks || null
    );
    const bom = db.prepare('SELECT id FROM product_boms WHERE bom_code = ?').get(payload.bomCode);
    const insert = db.prepare(
      `INSERT INTO product_bom_details (bom_id, bom_code, item_code, item_description, quantity, uom, remarks)
       VALUES (?, ?, ?, ?, ?, ?, ?)`
    );
    for (const line of payload.lines || []) {
      insert.run(
        bom.id,
        payload.bomCode,
        line.itemCode,
        line.itemDescription || null,
        line.quantity || 1,
        line.uom || 'NOS',
        line.remarks || null
      );
    }
  });
  tx();
  logActivity({
    entityType: 'BOM',
    entityRef: payload.bomCode,
    action: 'Created',
    details: payload.productCode,
    byUser: user.username,
  });
  return getBom(payload.bomCode);
}

export function approveBom(bomCode, { action, user }) {
  const db = getDb();
  const bom = db.prepare('SELECT * FROM product_boms WHERE bom_code = ?').get(bomCode);
  if (!bom) throw new Error('BOM not found');
  if (action === 'approve') {
    db.prepare(
      `UPDATE product_boms SET status = 'Approved', approved_by = ?, approved_date = datetime('now') WHERE bom_code = ?`
    ).run(user.displayName || user.username, bomCode);
  } else if (action === 'reject') {
    db.prepare(`UPDATE product_boms SET status = 'Rejected' WHERE bom_code = ?`).run(bomCode);
  } else {
    throw new Error('Unknown action');
  }
  logActivity({ entityType: 'BOM', entityRef: bomCode, action, details: action, byUser: user.username });
  return getBom(bomCode);
}

export function listItemCodeRequests(status) {
  const db = getDb();
  const rows = status && status !== 'All'
    ? db
        .prepare(
          `SELECT id, request_no AS requestNo, proposed_code AS proposedCode, item_description AS itemDescription,
                  specification, make, uom, requested_by AS requestedBy, request_date AS requestDate,
                  approval_status AS approvalStatus, approved_by AS approvedBy, approved_date AS approvedDate,
                  rejection_reason AS rejectionReason, remarks
           FROM item_code_requests WHERE approval_status = ? ORDER BY id DESC`
        )
        .all(status)
    : db
        .prepare(
          `SELECT id, request_no AS requestNo, proposed_code AS proposedCode, item_description AS itemDescription,
                  specification, make, uom, requested_by AS requestedBy, request_date AS requestDate,
                  approval_status AS approvalStatus, approved_by AS approvedBy, approved_date AS approvedDate,
                  rejection_reason AS rejectionReason, remarks
           FROM item_code_requests ORDER BY id DESC`
        )
        .all();
  return rows;
}

export function createItemCodeRequest(payload, user) {
  const db = getDb();
  const now = new Date();
  const prefix = `ICR-${now.getFullYear()}-`;
  const row = db
    .prepare(`SELECT request_no FROM item_code_requests WHERE request_no LIKE ? ORDER BY request_no DESC LIMIT 1`)
    .get(`${prefix}%`);
  let n = 1;
  if (row?.request_no) n = (parseInt(row.request_no.split('-').pop(), 10) || 0) + 1;
  const requestNo = `${prefix}${String(n).padStart(4, '0')}`;

  db.prepare(
    `INSERT INTO item_code_requests
      (request_no, proposed_code, item_description, specification, make, uom, requested_by, remarks)
     VALUES (?, ?, ?, ?, ?, ?, ?, ?)`
  ).run(
    requestNo,
    payload.proposedCode || null,
    payload.itemDescription,
    payload.specification || null,
    payload.make || null,
    payload.uom || 'NOS',
    user.displayName || user.username,
    payload.remarks || null
  );

  logActivity({
    entityType: 'ICR',
    entityRef: requestNo,
    action: 'Created',
    details: payload.itemDescription,
    byUser: user.username,
  });

  return listItemCodeRequests().find((r) => r.requestNo === requestNo);
}

export function decideItemCodeRequest(id, { action, reason, approvedCode, user }) {
  const db = getDb();
  const req = db.prepare('SELECT * FROM item_code_requests WHERE id = ?').get(id);
  if (!req) throw new Error('Request not found');
  if (req.approval_status !== 'Pending') throw new Error('Request already decided');

  if (action === 'reject') {
    db.prepare(
      `UPDATE item_code_requests SET approval_status = 'Rejected', rejection_reason = ?,
        approved_by = ?, approved_date = datetime('now') WHERE id = ?`
    ).run(reason || 'Rejected', user.displayName || user.username, id);
  } else if (action === 'approve') {
    const code = approvedCode || req.proposed_code;
    if (!code) throw new Error('Approved item code is required');
    db.prepare(
      `UPDATE item_code_requests SET approval_status = 'Approved', proposed_code = ?,
        approved_by = ?, approved_date = datetime('now') WHERE id = ?`
    ).run(code, user.displayName || user.username, id);
    db.prepare(
      `INSERT OR IGNORE INTO items (item_code, item_description, specification, make, uom)
       VALUES (?, ?, ?, ?, ?)`
    ).run(code, req.item_description, req.specification, req.make, req.uom);
  } else {
    throw new Error('Unknown action');
  }

  return listItemCodeRequests().find((r) => r.id === id);
}

export function listUsers() {
  return getDb()
    .prepare(
      `SELECT id, login_id AS loginId, username, display_name AS displayName, department, role, active,
              can_approve_pr AS canApprovePR, can_generate_po AS canGeneratePO, can_approve_po AS canApprovePO,
              is_pm AS isPm, is_mh AS isMh, is_gm AS isGm, is_om AS isOm, is_pc AS isPc
       FROM users ORDER BY username`
    )
    .all();
}
