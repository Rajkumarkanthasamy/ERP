import bcrypt from 'bcryptjs';
import { getDb, closeDb } from './connection.js';

const users = [
  {
    login_id: 'admin',
    username: 'admin',
    password: 'admin123',
    display_name: 'System Admin',
    department: 'IT',
    role: 'General Manager',
    flags: { can_approve_pr: 1, can_generate_po: 1, can_approve_po: 1, is_pm: 1, is_mh: 1, is_gm: 1, is_om: 1, is_pc: 1 },
  },
  {
    login_id: 'purchase',
    username: 'purchase',
    password: 'purchase123',
    display_name: 'Purchase Manager',
    department: 'Purchase',
    role: 'Purchase Manager',
    flags: { can_approve_pr: 1, can_generate_po: 1, can_approve_po: 1, is_pm: 1, is_pc: 1 },
  },
  {
    login_id: 'mh',
    username: 'mh',
    password: 'mh123',
    display_name: 'Manufacturing Head',
    department: 'Manufacturing',
    role: 'Manufacturing Head',
    flags: { can_approve_pr: 1, can_generate_po: 0, can_approve_po: 1, is_mh: 1 },
  },
  {
    login_id: 'om',
    username: 'om',
    password: 'om123',
    display_name: 'Operations Manager',
    department: 'Operations',
    role: 'Operations Manager',
    flags: { can_approve_pr: 0, can_generate_po: 0, can_approve_po: 1, is_om: 1 },
  },
  {
    login_id: 'gm',
    username: 'gm',
    password: 'gm123',
    display_name: 'General Manager',
    department: 'Management',
    role: 'General Manager',
    flags: { can_approve_pr: 1, can_generate_po: 1, can_approve_po: 1, is_gm: 1 },
  },
  {
    login_id: 'user',
    username: 'user',
    password: 'user123',
    display_name: 'Store User',
    department: 'Stores',
    role: 'User',
    flags: { can_approve_pr: 0, can_generate_po: 0, can_approve_po: 0 },
  },
];

const vendors = [
  { vendor_code: 'V001', vendor_name: 'Precision Components Pvt Ltd', city: 'Pune', gstin: '27AAAAA0000A1Z5' },
  { vendor_code: 'V002', vendor_name: 'ElectroTech Supplies', city: 'Bengaluru', gstin: '29BBBBB0000B1Z5' },
  { vendor_code: 'V003', vendor_name: 'MetalCraft Industries', city: 'Chennai', gstin: '33CCCCC0000C1Z5' },
  { vendor_code: 'V004', vendor_name: 'Global Fasteners', city: 'Mumbai', gstin: '27DDDDD0000D1Z5' },
];

const projects = [
  { project_code: 'PRJ-1001', project_name: 'Assembly Line Upgrade', product_no: 'PROD-A1', customer_code: 'C001' },
  { project_code: 'PRJ-1002', project_name: 'Calibration Bench', product_no: 'PROD-B2', customer_code: 'C002' },
  { project_code: 'PRJ-1003', project_name: 'Test Rig Expansion', product_no: 'PROD-C3', customer_code: 'C001' },
];

const items = [
  { item_code: 'ITM-1001', item_description: 'Bearing Housing', specification: 'Cast Iron, Grade FG260', make: 'SKF', mfg_part_no: 'BH-260', uom: 'NOS', hsn_code: '8483', standard_cost: 2450, latest_purchase_price: 2500 },
  { item_code: 'ITM-1002', item_description: 'Proximity Sensor M12', specification: 'PNP NO 10-30VDC', make: 'Omron', mfg_part_no: 'E2E-X10', uom: 'NOS', hsn_code: '9031', standard_cost: 1800, latest_purchase_price: 1750 },
  { item_code: 'ITM-1003', item_description: 'SS Hex Bolt M8x25', specification: 'SS304', make: 'Unbrako', mfg_part_no: 'HB-M8-25', uom: 'NOS', hsn_code: '7318', standard_cost: 12, latest_purchase_price: 14 },
  { item_code: 'ITM-1004', item_description: 'PLC Relay Module', specification: '8 Channel 24V', make: 'Siemens', mfg_part_no: '6ES7-131', uom: 'NOS', hsn_code: '8536', standard_cost: 9200, latest_purchase_price: 9500 },
  { item_code: 'ITM-1005', item_description: 'Cable Gland PG21', specification: 'Nylon IP68', make: 'Lapp', mfg_part_no: 'CG-PG21', uom: 'NOS', hsn_code: '8547', standard_cost: 85, latest_purchase_price: 90 },
  { item_code: 'ITM-1006', item_description: 'Aluminum Extrusion 40x40', specification: 'Anodized, 1m', make: 'Bosch Rexroth', mfg_part_no: 'AE-4040', uom: 'MTR', hsn_code: '7604', standard_cost: 420, latest_purchase_price: 450 },
];

function isEmpty(db, table) {
  return db.prepare(`SELECT COUNT(*) AS c FROM ${table}`).get().c === 0;
}

function seedCore(db) {
  if (!isEmpty(db, 'users')) return false;

  const insertUser = db.prepare(`
    INSERT INTO users (
      login_id, username, password_hash, display_name, department, role, active,
      can_approve_pr, can_generate_po, can_approve_po, is_pm, is_mh, is_gm, is_om, is_pc
    ) VALUES (
      @login_id, @username, @password_hash, @display_name, @department, @role, 1,
      @can_approve_pr, @can_generate_po, @can_approve_po, @is_pm, @is_mh, @is_gm, @is_om, @is_pc
    )
  `);

  const insertVendor = db.prepare(
    'INSERT INTO vendors (vendor_code, vendor_name, city, gstin) VALUES (@vendor_code, @vendor_name, @city, @gstin)'
  );
  const insertProject = db.prepare(
    `INSERT INTO projects (project_code, project_name, product_no, customer_code, approval_status, status)
     VALUES (@project_code, @project_name, @product_no, @customer_code, 'Approved', 'Active')`
  );
  const insertItem = db.prepare(`
    INSERT INTO items (
      item_code, item_description, specification, make, mfg_part_no, uom, hsn_code, standard_cost, latest_purchase_price
    ) VALUES (
      @item_code, @item_description, @specification, @make, @mfg_part_no, @uom, @hsn_code, @standard_cost, @latest_purchase_price
    )
  `);

  for (const u of users) {
    insertUser.run({
      login_id: u.login_id,
      username: u.username,
      password_hash: bcrypt.hashSync(u.password, 8),
      display_name: u.display_name,
      department: u.department,
      role: u.role,
      can_approve_pr: u.flags.can_approve_pr || 0,
      can_generate_po: u.flags.can_generate_po || 0,
      can_approve_po: u.flags.can_approve_po || 0,
      is_pm: u.flags.is_pm || 0,
      is_mh: u.flags.is_mh || 0,
      is_gm: u.flags.is_gm || 0,
      is_om: u.flags.is_om || 0,
      is_pc: u.flags.is_pc || 0,
    });
  }
  for (const v of vendors) insertVendor.run(v);
  for (const p of projects) insertProject.run(p);
  for (const i of items) insertItem.run(i);

  db.prepare(`
    INSERT INTO purchase_requests (
      pr_number, project_code, product_no, vendor_code, vendor_name, total_amount,
      status, requested_by, created_by, remarks
    ) VALUES (
      'PR-2026-08-0000001', 'PRJ-1001', 'PROD-A1', 'V001', 'Precision Components Pvt Ltd', 12500,
      'Pending', 'Store User', 'user', 'Initial stock replenishment'
    )
  `).run();

  const pr = db.prepare("SELECT id FROM purchase_requests WHERE pr_number = 'PR-2026-08-0000001'").get();
  db.prepare(`
    INSERT INTO purchase_request_details (
      pr_id, pr_number, project_code, product_no, item_code, item_description, specification,
      make, mfg_part_no, quantity, uom, unit_cost, total_cost, vendor_code, vendor_name, line_status, created_by
    ) VALUES
    (?, 'PR-2026-08-0000001', 'PRJ-1001', 'PROD-A1', 'ITM-1001', 'Bearing Housing', 'Cast Iron, Grade FG260',
     'SKF', 'BH-260', 5, 'NOS', 2500, 12500, 'V001', 'Precision Components Pvt Ltd', 'Pending', 'user')
  `).run(pr.id);

  db.prepare(`
    INSERT INTO purchase_requests (
      pr_number, project_code, product_no, vendor_code, vendor_name, total_amount,
      status, requested_by, approved_by, approved_date, created_by, remarks
    ) VALUES (
      'PR-2026-08-0000002', 'PRJ-1002', 'PROD-B2', 'V002', 'ElectroTech Supplies', 19000,
      'Approved', 'Store User', 'Purchase Manager', datetime('now'), 'user', 'Sensors for calibration bench'
    )
  `).run();
  const pr2 = db.prepare("SELECT id FROM purchase_requests WHERE pr_number = 'PR-2026-08-0000002'").get();
  db.prepare(`
    INSERT INTO purchase_request_details (
      pr_id, pr_number, project_code, product_no, item_code, item_description, specification,
      make, mfg_part_no, quantity, uom, unit_cost, total_cost, vendor_code, vendor_name, line_status, created_by
    ) VALUES
    (?, 'PR-2026-08-0000002', 'PRJ-1002', 'PROD-B2', 'ITM-1002', 'Proximity Sensor M12', 'PNP NO 10-30VDC',
     'Omron', 'E2E-X10', 8, 'NOS', 1750, 14000, 'V002', 'ElectroTech Supplies', 'Pending', 'user'),
    (?, 'PR-2026-08-0000002', 'PRJ-1002', 'PROD-B2', 'ITM-1005', 'Cable Gland PG21', 'Nylon IP68',
     'Lapp', 'CG-PG21', 50, 'NOS', 100, 5000, 'V002', 'ElectroTech Supplies', 'Pending', 'user')
  `).run(pr2.id, pr2.id);

  db.prepare(`
    INSERT INTO item_code_requests (
      request_no, proposed_code, item_description, specification, make, uom, requested_by, approval_status, remarks
    ) VALUES (
      'ICR-2026-0001', 'ITM-1100', 'Servo Drive 400W', 'Single phase 230V', 'Delta', 'NOS', 'user', 'Pending', 'New drive for test rig'
    )
  `).run();

  db.prepare(`
    INSERT INTO activity_log (entity_type, entity_ref, action, details, by_user)
    VALUES
    ('PR', 'PR-2026-08-0000001', 'Created', 'Sample pending PR seeded', 'system'),
    ('PR', 'PR-2026-08-0000002', 'Approved', 'Sample approved PR seeded', 'system')
  `).run();

  return true;
}

function seedModules(db) {
  const seeded = [];

  if (isEmpty(db, 'cities')) {
    db.prepare(`
      INSERT INTO cities (city_code, city_name, state, country) VALUES
      ('CTY-PUN', 'Pune', 'Maharashtra', 'India'),
      ('CTY-BLR', 'Bengaluru', 'Karnataka', 'India'),
      ('CTY-CHE', 'Chennai', 'Tamil Nadu', 'India'),
      ('CTY-MUM', 'Mumbai', 'Maharashtra', 'India')
    `).run();
    seeded.push('cities');
  }

  if (isEmpty(db, 'customers')) {
    db.prepare(`
      INSERT INTO customers (customer_code, customer_name, city_code, city, contact_person, phone, email) VALUES
      ('C001', 'AutoForge Motors', 'CTY-PUN', 'Pune', 'Ravi Kumar', '9876500001', 'ravi@autoforge.example'),
      ('C002', 'Precision Labs', 'CTY-BLR', 'Bengaluru', 'Anita Shah', '9876500002', 'anita@precision.example'),
      ('C003', 'South Coast Energy', 'CTY-CHE', 'Chennai', 'Suresh Nair', '9876500003', 'suresh@sce.example')
    `).run();
    seeded.push('customers');
  }

  if (isEmpty(db, 'assets')) {
    db.prepare(`
      INSERT INTO assets (asset_code, asset_name, category, location, purchase_date, purchase_value, current_value, status) VALUES
      ('AST-001', 'CNC Lathe Machine', 'Machinery', 'Shop Floor A', '2022-03-15', 1250000, 980000, 'Active'),
      ('AST-002', 'Coordinate Measuring Machine', 'Quality', 'Metrology Lab', '2023-07-01', 850000, 780000, 'Active')
    `).run();
    seeded.push('assets');
  }

  if (isEmpty(db, 'sales_products')) {
    db.prepare(`
      INSERT INTO sales_products (product_code, product_name, description, uom, list_price, peg_rate, is_spare_part) VALUES
      ('SP-100', 'Assembly Cell Standard', 'Standard assembly cell package', 'NOS', 2500000, 1.0, 0),
      ('SP-200', 'Calibration Bench Kit', 'Bench with fixtures', 'NOS', 780000, 1.05, 0),
      ('SP-SPARE-01', 'Sensor Replacement Kit', 'Spare proximity sensors', 'SET', 15000, 1.1, 1)
    `).run();
    seeded.push('sales_products');
  }

  if (isEmpty(db, 'product_boms')) {
    db.prepare(`
      INSERT INTO product_boms (bom_code, product_code, product_name, version, status, created_by)
      VALUES ('BOM-A1', 'PROD-A1', 'Assembly Line Upgrade', '1.0', 'Draft', 'system')
    `).run();
    const bom = db.prepare("SELECT id FROM product_boms WHERE bom_code = 'BOM-A1'").get();
    db.prepare(`
      INSERT INTO product_bom_details (bom_id, bom_code, item_code, item_description, quantity, uom) VALUES
      (?, 'BOM-A1', 'ITM-1001', 'Bearing Housing', 4, 'NOS'),
      (?, 'BOM-A1', 'ITM-1003', 'SS Hex Bolt M8x25', 40, 'NOS'),
      (?, 'BOM-A1', 'ITM-1006', 'Aluminum Extrusion 40x40', 12, 'MTR')
    `).run(bom.id, bom.id, bom.id);
    seeded.push('product_boms');
  }

  if (isEmpty(db, 'stock_ledger')) {
    db.prepare(`
      INSERT INTO stock_ledger (item_code, item_description, uom, quantity_on_hand, location, last_txn_date) VALUES
      ('ITM-1001', 'Bearing Housing', 'NOS', 25, 'MAIN', datetime('now')),
      ('ITM-1002', 'Proximity Sensor M12', 'NOS', 40, 'MAIN', datetime('now')),
      ('ITM-1003', 'SS Hex Bolt M8x25', 'NOS', 500, 'MAIN', datetime('now')),
      ('ITM-1005', 'Cable Gland PG21', 'NOS', 120, 'MAIN', datetime('now'))
    `).run();
    seeded.push('stock_ledger');
  }

  if (isEmpty(db, 'stock_transactions')) {
    db.prepare(`
      INSERT INTO stock_transactions (
        txn_number, txn_type, item_code, item_description, quantity, uom, project_code, remarks, created_by
      ) VALUES
      ('GIN-2026-00001', 'GIN', 'ITM-1001', 'Bearing Housing', 25, 'NOS', 'PRJ-1001', 'Opening stock', 'system'),
      ('ISS-2026-00001', 'Issue', 'ITM-1003', 'SS Hex Bolt M8x25', 20, 'NOS', 'PRJ-1001', 'Shop issue', 'user')
    `).run();
    seeded.push('stock_transactions');
  }

  if (isEmpty(db, 'indents')) {
    db.prepare(`
      INSERT INTO indents (indent_number, project_code, requested_by, required_date, status, remarks, created_by)
      VALUES ('IND-2026-00001', 'PRJ-1001', 'Store User', date('now', '+7 day'), 'Open', 'Material for assembly', 'user')
    `).run();
    const indent = db.prepare("SELECT id FROM indents WHERE indent_number = 'IND-2026-00001'").get();
    db.prepare(`
      INSERT INTO indent_details (indent_id, indent_number, item_code, item_description, quantity, uom) VALUES
      (?, 'IND-2026-00001', 'ITM-1004', 'PLC Relay Module', 2, 'NOS'),
      (?, 'IND-2026-00001', 'ITM-1002', 'Proximity Sensor M12', 6, 'NOS')
    `).run(indent.id, indent.id);
    seeded.push('indents');
  }

  if (isEmpty(db, 'work_orders')) {
    db.prepare(`
      INSERT INTO work_orders (
        wo_number, project_code, product_no, vendor_code, vendor_name, status, start_date, due_date, remarks, created_by
      ) VALUES (
        'WO-2026-00001', 'PRJ-1003', 'PROD-C3', 'V003', 'MetalCraft Industries', 'Open',
        date('now'), date('now', '+21 day'), 'Fabrication work order', 'purchase'
      )
    `).run();
    const wo = db.prepare("SELECT id FROM work_orders WHERE wo_number = 'WO-2026-00001'").get();
    db.prepare(`
      INSERT INTO work_order_details (wo_id, wo_number, item_code, item_description, quantity, uom, unit_cost, total_cost)
      VALUES (?, 'WO-2026-00001', 'ITM-1006', 'Aluminum Extrusion 40x40', 30, 'MTR', 450, 13500)
    `).run(wo.id);
    seeded.push('work_orders');
  }

  if (isEmpty(db, 'sales_enquiries')) {
    db.prepare(`
      INSERT INTO sales_enquiries (
        enquiry_number, customer_code, customer_name, subject, source, status, assigned_to, remarks, created_by
      ) VALUES
      ('ENQ-2026-00001', 'C001', 'AutoForge Motors', 'New assembly cell enquiry', 'Website', 'Open', 'admin', 'Initial contact', 'admin'),
      ('ENQ-2026-00002', 'C002', 'Precision Labs', 'Calibration upgrade', 'Referral', 'Open', 'gm', 'Follow up', 'admin')
    `).run();
    seeded.push('sales_enquiries');
  }

  if (isEmpty(db, 'opportunities')) {
    db.prepare(`
      INSERT INTO opportunities (
        opportunity_number, enquiry_number, customer_code, customer_name, title, stage,
        expected_value, probability, expected_close_date, status, owner, created_by
      ) VALUES (
        'OPP-2026-00001', 'ENQ-2026-00001', 'C001', 'AutoForge Motors', 'Assembly Cell Deal',
        'Proposal', 2500000, 60, date('now', '+45 day'), 'Open', 'admin', 'admin'
      )
    `).run();
    seeded.push('opportunities');
  }

  if (isEmpty(db, 'quotes')) {
    db.prepare(`
      INSERT INTO quotes (
        quote_number, opportunity_number, customer_code, customer_name, valid_until,
        total_amount, status, remarks, created_by
      ) VALUES (
        'QT-2026-00001', 'OPP-2026-00001', 'C001', 'AutoForge Motors', date('now', '+30 day'),
        2500000, 'Draft', 'Standard package quote', 'admin'
      )
    `).run();
    const qt = db.prepare("SELECT id FROM quotes WHERE quote_number = 'QT-2026-00001'").get();
    db.prepare(`
      INSERT INTO quote_details (quote_id, quote_number, product_code, description, quantity, unit_price, amount)
      VALUES (?, 'QT-2026-00001', 'SP-100', 'Assembly Cell Standard', 1, 2500000, 2500000)
    `).run(qt.id);
    seeded.push('quotes');
  }

  if (isEmpty(db, 'service_calls')) {
    db.prepare(`
      INSERT INTO service_calls (
        call_number, customer_code, customer_name, project_code, subject, priority, status, assigned_to, remarks, created_by
      ) VALUES (
        'SC-2026-00001', 'C002', 'Precision Labs', 'PRJ-1002', 'Sensor calibration drift', 'High', 'Open', 'user', 'Urgent site visit', 'admin'
      )
    `).run();
    seeded.push('service_calls');
  }

  if (isEmpty(db, 'timesheets')) {
    db.prepare(`
      INSERT INTO timesheets (
        entry_number, user_name, project_code, work_date, hours, activity, status, created_by
      ) VALUES
      ('TS-2026-00001', 'Store User', 'PRJ-1001', date('now', '-1 day'), 6, 'Material kitting', 'Submitted', 'user'),
      ('TS-2026-00002', 'Store User', 'PRJ-1002', date('now'), 4, 'Stores issue support', 'Draft', 'user')
    `).run();
    seeded.push('timesheets');
  }

  if (isEmpty(db, 'quality_ncs')) {
    db.prepare(`
      INSERT INTO quality_ncs (
        nc_number, project_code, item_code, source, description, severity, status, raised_by, assigned_to, created_by
      ) VALUES (
        'NC-2026-00001', 'PRJ-1001', 'ITM-1001', 'Incoming', 'Surface finish below spec', 'Major', 'Open', 'mh', 'mh', 'mh'
      )
    `).run();
    seeded.push('quality_ncs');
  }

  if (isEmpty(db, 'escalations')) {
    db.prepare(`
      INSERT INTO escalations (
        escalation_number, related_type, related_ref, project_code, title, description, priority, status, raised_by, assigned_to, created_by
      ) VALUES (
        'ESC-2026-00001', 'NC', 'NC-2026-00001', 'PRJ-1001', 'Vendor quality slip', 'Repeat NC on bearing housing', 'High', 'Open', 'mh', 'gm', 'mh'
      )
    `).run();
    seeded.push('escalations');
  }

  if (isEmpty(db, 'complaints')) {
    db.prepare(`
      INSERT INTO complaints (
        complaint_number, customer_code, customer_name, subject, description, category, priority, status, assigned_to, created_by
      ) VALUES (
        'CMP-2026-00001', 'C003', 'South Coast Energy', 'Delayed spare parts', 'Spare kit delayed by 2 weeks', 'Support', 'Normal', 'Open', 'admin', 'admin'
      )
    `).run();
    seeded.push('complaints');
  }

  if (isEmpty(db, 'gate_entries')) {
    db.prepare(`
      INSERT INTO gate_entries (
        entry_number, entry_type, vehicle_no, transporter, vendor_code, vendor_name, purpose, invoice_no, status, created_by
      ) VALUES (
        'GE-2026-00001', 'Inbound', 'MH12AB1234', 'Swift Logistics', 'V001', 'Precision Components Pvt Ltd',
        'Material delivery', 'INV-7781', 'Open', 'user'
      )
    `).run();
    seeded.push('gate_entries');
  }

  if (isEmpty(db, 'delivery_challans')) {
    db.prepare(`
      INSERT INTO delivery_challans (
        dc_number, customer_code, customer_name, project_code, status, vehicle_no, transporter, remarks, created_by
      ) VALUES (
        'DC-2026-00001', 'C001', 'AutoForge Motors', 'PRJ-1001', 'Draft', 'MH14CD5678', 'Swift Logistics', 'Partial shipment', 'user'
      )
    `).run();
    const dc = db.prepare("SELECT id FROM delivery_challans WHERE dc_number = 'DC-2026-00001'").get();
    db.prepare(`
      INSERT INTO delivery_challan_details (dc_id, dc_number, item_code, item_description, quantity, uom)
      VALUES (?, 'DC-2026-00001', 'ITM-1002', 'Proximity Sensor M12', 4, 'NOS')
    `).run(dc.id);
    seeded.push('delivery_challans');
  }

  return seeded;
}

function seed() {
  const db = getDb();
  const tx = db.transaction(() => {
    const core = seedCore(db);
    const modules = seedModules(db);
    return { core, modules };
  });

  const result = tx();
  if (result.core) {
    console.log('Core seed complete. Demo logins:');
    for (const u of users) {
      console.log(`  ${u.username} / ${u.password}  (${u.role})`);
    }
  } else {
    console.log('Core data already present — skipped user/vendor/item/PR seeds.');
  }

  if (result.modules.length) {
    console.log(`Module seed applied for: ${result.modules.join(', ')}`);
  } else {
    console.log('All module tables already have data — nothing new seeded.');
  }

  closeDb();
}

seed();
