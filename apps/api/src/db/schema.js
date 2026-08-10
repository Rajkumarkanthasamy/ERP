function ensureColumn(db, table, column, definition) {
  const cols = db.prepare(`PRAGMA table_info(${table})`).all().map((c) => c.name);
  if (!cols.includes(column)) {
    db.exec(`ALTER TABLE ${table} ADD COLUMN ${column} ${definition}`);
  }
}

export function applySchema(db) {
  db.exec(`
    CREATE TABLE IF NOT EXISTS users (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      login_id TEXT NOT NULL UNIQUE,
      username TEXT NOT NULL UNIQUE,
      password_hash TEXT NOT NULL,
      display_name TEXT NOT NULL,
      department TEXT DEFAULT '',
      role TEXT DEFAULT 'User',
      active INTEGER DEFAULT 1,
      can_approve_pr INTEGER DEFAULT 0,
      can_generate_po INTEGER DEFAULT 0,
      can_approve_po INTEGER DEFAULT 0,
      is_pm INTEGER DEFAULT 0,
      is_mh INTEGER DEFAULT 0,
      is_gm INTEGER DEFAULT 0,
      is_om INTEGER DEFAULT 0,
      is_pc INTEGER DEFAULT 0,
      created_at TEXT DEFAULT (datetime('now'))
    );

    CREATE TABLE IF NOT EXISTS vendors (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      vendor_code TEXT NOT NULL UNIQUE,
      vendor_name TEXT NOT NULL,
      city TEXT,
      gstin TEXT,
      active INTEGER DEFAULT 1
    );

    CREATE TABLE IF NOT EXISTS projects (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      project_code TEXT NOT NULL UNIQUE,
      project_name TEXT NOT NULL,
      product_no TEXT,
      status TEXT DEFAULT 'Active'
    );

    CREATE TABLE IF NOT EXISTS items (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      item_code TEXT NOT NULL UNIQUE,
      item_description TEXT,
      specification TEXT,
      make TEXT,
      mfg_part_no TEXT,
      uom TEXT DEFAULT 'NOS',
      hsn_code TEXT,
      standard_cost REAL DEFAULT 0,
      latest_purchase_price REAL DEFAULT 0,
      active INTEGER DEFAULT 1
    );

    CREATE TABLE IF NOT EXISTS purchase_requests (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      pr_number TEXT NOT NULL UNIQUE,
      project_code TEXT NOT NULL,
      product_no TEXT,
      vendor_code TEXT NOT NULL,
      vendor_name TEXT,
      total_amount REAL DEFAULT 0,
      status TEXT DEFAULT 'Pending',
      requested_by TEXT,
      request_date TEXT DEFAULT (datetime('now')),
      approved_by TEXT,
      approved_date TEXT,
      rejection_reason TEXT,
      hold_reason TEXT,
      remarks TEXT,
      is_clubbed INTEGER DEFAULT 0,
      clubbed_from_pr_ids TEXT,
      created_by TEXT,
      created_at TEXT DEFAULT (datetime('now')),
      modified_by TEXT,
      modified_at TEXT
    );

    CREATE TABLE IF NOT EXISTS purchase_request_details (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      pr_id INTEGER NOT NULL,
      pr_number TEXT NOT NULL,
      project_code TEXT,
      product_no TEXT,
      item_code TEXT NOT NULL,
      item_description TEXT,
      specification TEXT,
      make TEXT,
      mfg_part_no TEXT,
      quantity REAL DEFAULT 0,
      uom TEXT,
      unit_cost REAL DEFAULT 0,
      total_cost REAL DEFAULT 0,
      vendor_code TEXT,
      vendor_name TEXT,
      bom_code TEXT,
      hsn_code TEXT,
      line_status TEXT DEFAULT 'Pending',
      remarks TEXT,
      created_by TEXT,
      created_at TEXT DEFAULT (datetime('now')),
      FOREIGN KEY (pr_id) REFERENCES purchase_requests(id) ON DELETE CASCADE
    );

    CREATE TABLE IF NOT EXISTS purchase_orders (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      po_ref TEXT NOT NULL,
      pr_id INTEGER,
      pr_number TEXT,
      detail_id INTEGER,
      project_code TEXT,
      vendor_code TEXT,
      vendor_name TEXT,
      item_code TEXT NOT NULL,
      item_description TEXT,
      specification TEXT,
      make TEXT,
      mfg_part_no TEXT,
      required_qty REAL DEFAULT 0,
      remaining_qty REAL DEFAULT 0,
      uom TEXT,
      unit_price REAL DEFAULT 0,
      amount REAL DEFAULT 0,
      igst_rate REAL DEFAULT 0,
      igst_amount REAL DEFAULT 0,
      sgst_rate REAL DEFAULT 0,
      sgst_amount REAL DEFAULT 0,
      cgst_rate REAL DEFAULT 0,
      cgst_amount REAL DEFAULT 0,
      currency TEXT DEFAULT 'INR',
      prepared_by TEXT,
      prepared_date TEXT,
      po_approved INTEGER DEFAULT 0,
      po_generated_by TEXT,
      po_generated_date TEXT,
      po_sent_to_vendor_by TEXT,
      po_sent_vendor_date TEXT,
      pm_name TEXT,
      pm_approved INTEGER DEFAULT 0,
      pm_approved_date TEXT,
      mh_name TEXT,
      mh_approved INTEGER DEFAULT 0,
      mh_approved_date TEXT,
      pc_name TEXT,
      pc_approved INTEGER DEFAULT 0,
      pc_approved_date TEXT,
      pc_remarks TEXT,
      om_name TEXT,
      om_approved INTEGER DEFAULT 0,
      om_approved_date TEXT,
      om_remarks TEXT,
      gm_name TEXT,
      gm_approved INTEGER DEFAULT 0,
      gm_approved_date TEXT,
      finalized_by TEXT,
      finalized_date TEXT,
      final_status TEXT,
      reject_reason TEXT,
      cancelled_by TEXT,
      cancelled_date TEXT,
      closed_by TEXT,
      closed_date TEXT,
      remarks TEXT,
      created_at TEXT DEFAULT (datetime('now')),
      modified_at TEXT
    );

    CREATE TABLE IF NOT EXISTS procurement_grn (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      grn_number TEXT NOT NULL UNIQUE,
      po_ref TEXT,
      vendor_code TEXT,
      project_code TEXT,
      received_by TEXT,
      received_date TEXT DEFAULT (datetime('now')),
      status TEXT DEFAULT 'Received',
      remarks TEXT,
      created_by TEXT,
      created_at TEXT DEFAULT (datetime('now'))
    );

    CREATE TABLE IF NOT EXISTS procurement_grn_details (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      grn_id INTEGER NOT NULL,
      grn_number TEXT NOT NULL,
      po_id INTEGER,
      item_code TEXT NOT NULL,
      ordered_qty REAL DEFAULT 0,
      received_qty REAL DEFAULT 0,
      unit_price REAL DEFAULT 0,
      amount REAL DEFAULT 0,
      uom TEXT,
      remarks TEXT,
      FOREIGN KEY (grn_id) REFERENCES procurement_grn(id) ON DELETE CASCADE
    );

    CREATE TABLE IF NOT EXISTS procurement_comments (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      entity_type TEXT NOT NULL,
      entity_ref TEXT NOT NULL,
      comment_text TEXT NOT NULL,
      created_by TEXT,
      created_at TEXT DEFAULT (datetime('now'))
    );

    CREATE TABLE IF NOT EXISTS procurement_attachments (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      entity_type TEXT NOT NULL,
      entity_ref TEXT NOT NULL,
      file_name TEXT NOT NULL,
      file_path TEXT NOT NULL,
      file_size_kb REAL,
      uploaded_by TEXT,
      uploaded_at TEXT DEFAULT (datetime('now')),
      remarks TEXT
    );

    CREATE TABLE IF NOT EXISTS item_code_requests (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      request_no TEXT NOT NULL UNIQUE,
      proposed_code TEXT,
      item_description TEXT NOT NULL,
      specification TEXT,
      make TEXT,
      uom TEXT DEFAULT 'NOS',
      requested_by TEXT,
      request_date TEXT DEFAULT (datetime('now')),
      approval_status TEXT DEFAULT 'Pending',
      approved_by TEXT,
      approved_date TEXT,
      rejection_reason TEXT,
      remarks TEXT
    );

    CREATE TABLE IF NOT EXISTS activity_log (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      entity_type TEXT,
      entity_ref TEXT,
      action TEXT,
      details TEXT,
      by_user TEXT,
      created_at TEXT DEFAULT (datetime('now'))
    );

    CREATE TABLE IF NOT EXISTS cities (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      city_code TEXT NOT NULL UNIQUE,
      city_name TEXT NOT NULL,
      state TEXT,
      country TEXT DEFAULT 'India',
      active INTEGER DEFAULT 1
    );

    CREATE TABLE IF NOT EXISTS customers (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      customer_code TEXT NOT NULL UNIQUE,
      customer_name TEXT NOT NULL,
      city_code TEXT,
      city TEXT,
      address TEXT,
      gstin TEXT,
      contact_person TEXT,
      phone TEXT,
      email TEXT,
      active INTEGER DEFAULT 1,
      created_at TEXT DEFAULT (datetime('now'))
    );

    CREATE TABLE IF NOT EXISTS assets (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      asset_code TEXT NOT NULL UNIQUE,
      asset_name TEXT NOT NULL,
      category TEXT,
      location TEXT,
      purchase_date TEXT,
      purchase_value REAL DEFAULT 0,
      current_value REAL DEFAULT 0,
      status TEXT DEFAULT 'Active',
      remarks TEXT,
      created_at TEXT DEFAULT (datetime('now'))
    );

    CREATE TABLE IF NOT EXISTS product_boms (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      bom_code TEXT NOT NULL UNIQUE,
      product_code TEXT NOT NULL,
      product_name TEXT,
      version TEXT DEFAULT '1.0',
      status TEXT DEFAULT 'Draft',
      approved_by TEXT,
      approved_date TEXT,
      created_by TEXT,
      created_at TEXT DEFAULT (datetime('now')),
      remarks TEXT
    );

    CREATE TABLE IF NOT EXISTS product_bom_details (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      bom_id INTEGER NOT NULL,
      bom_code TEXT NOT NULL,
      item_code TEXT NOT NULL,
      item_description TEXT,
      quantity REAL DEFAULT 1,
      uom TEXT DEFAULT 'NOS',
      remarks TEXT,
      FOREIGN KEY (bom_id) REFERENCES product_boms(id) ON DELETE CASCADE
    );

    CREATE TABLE IF NOT EXISTS sales_products (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      product_code TEXT NOT NULL UNIQUE,
      product_name TEXT NOT NULL,
      description TEXT,
      uom TEXT DEFAULT 'NOS',
      list_price REAL DEFAULT 0,
      peg_rate REAL DEFAULT 0,
      is_spare_part INTEGER DEFAULT 0,
      active INTEGER DEFAULT 1
    );

    CREATE TABLE IF NOT EXISTS stock_ledger (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      item_code TEXT NOT NULL UNIQUE,
      item_description TEXT,
      uom TEXT DEFAULT 'NOS',
      quantity_on_hand REAL DEFAULT 0,
      reserved_qty REAL DEFAULT 0,
      location TEXT DEFAULT 'MAIN',
      last_txn_date TEXT,
      updated_at TEXT DEFAULT (datetime('now'))
    );

    CREATE TABLE IF NOT EXISTS stock_transactions (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      txn_number TEXT NOT NULL UNIQUE,
      txn_type TEXT NOT NULL,
      item_code TEXT NOT NULL,
      item_description TEXT,
      quantity REAL DEFAULT 0,
      uom TEXT,
      project_code TEXT,
      reference_no TEXT,
      from_location TEXT,
      to_location TEXT,
      remarks TEXT,
      status TEXT DEFAULT 'Posted',
      created_by TEXT,
      created_at TEXT DEFAULT (datetime('now'))
    );

    CREATE TABLE IF NOT EXISTS indents (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      indent_number TEXT NOT NULL UNIQUE,
      project_code TEXT,
      requested_by TEXT,
      request_date TEXT DEFAULT (datetime('now')),
      required_date TEXT,
      status TEXT DEFAULT 'Open',
      approved_by TEXT,
      approved_date TEXT,
      rejection_reason TEXT,
      remarks TEXT,
      created_by TEXT,
      created_at TEXT DEFAULT (datetime('now')),
      modified_by TEXT,
      modified_at TEXT
    );

    CREATE TABLE IF NOT EXISTS indent_details (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      indent_id INTEGER NOT NULL,
      indent_number TEXT NOT NULL,
      item_code TEXT NOT NULL,
      item_description TEXT,
      quantity REAL DEFAULT 0,
      uom TEXT DEFAULT 'NOS',
      remarks TEXT,
      FOREIGN KEY (indent_id) REFERENCES indents(id) ON DELETE CASCADE
    );

    CREATE TABLE IF NOT EXISTS work_orders (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      wo_number TEXT NOT NULL UNIQUE,
      project_code TEXT,
      product_no TEXT,
      vendor_code TEXT,
      vendor_name TEXT,
      status TEXT DEFAULT 'Open',
      start_date TEXT,
      due_date TEXT,
      approved_by TEXT,
      approved_date TEXT,
      remarks TEXT,
      created_by TEXT,
      created_at TEXT DEFAULT (datetime('now')),
      modified_by TEXT,
      modified_at TEXT
    );

    CREATE TABLE IF NOT EXISTS work_order_details (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      wo_id INTEGER NOT NULL,
      wo_number TEXT NOT NULL,
      item_code TEXT NOT NULL,
      item_description TEXT,
      quantity REAL DEFAULT 0,
      uom TEXT DEFAULT 'NOS',
      unit_cost REAL DEFAULT 0,
      total_cost REAL DEFAULT 0,
      remarks TEXT,
      FOREIGN KEY (wo_id) REFERENCES work_orders(id) ON DELETE CASCADE
    );

    CREATE TABLE IF NOT EXISTS sales_enquiries (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      enquiry_number TEXT NOT NULL UNIQUE,
      customer_code TEXT,
      customer_name TEXT,
      subject TEXT,
      enquiry_date TEXT DEFAULT (datetime('now')),
      source TEXT,
      status TEXT DEFAULT 'Open',
      assigned_to TEXT,
      remarks TEXT,
      created_by TEXT,
      created_at TEXT DEFAULT (datetime('now')),
      modified_by TEXT,
      modified_at TEXT
    );

    CREATE TABLE IF NOT EXISTS opportunities (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      opportunity_number TEXT NOT NULL UNIQUE,
      enquiry_number TEXT,
      customer_code TEXT,
      customer_name TEXT,
      title TEXT,
      stage TEXT DEFAULT 'Qualification',
      expected_value REAL DEFAULT 0,
      probability REAL DEFAULT 0,
      expected_close_date TEXT,
      status TEXT DEFAULT 'Open',
      owner TEXT,
      remarks TEXT,
      created_by TEXT,
      created_at TEXT DEFAULT (datetime('now')),
      modified_by TEXT,
      modified_at TEXT
    );

    CREATE TABLE IF NOT EXISTS quotes (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      quote_number TEXT NOT NULL UNIQUE,
      opportunity_number TEXT,
      customer_code TEXT,
      customer_name TEXT,
      quote_date TEXT DEFAULT (datetime('now')),
      valid_until TEXT,
      total_amount REAL DEFAULT 0,
      currency TEXT DEFAULT 'INR',
      status TEXT DEFAULT 'Draft',
      approved_by TEXT,
      approved_date TEXT,
      remarks TEXT,
      created_by TEXT,
      created_at TEXT DEFAULT (datetime('now')),
      modified_by TEXT,
      modified_at TEXT
    );

    CREATE TABLE IF NOT EXISTS quote_details (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      quote_id INTEGER NOT NULL,
      quote_number TEXT NOT NULL,
      product_code TEXT,
      description TEXT,
      quantity REAL DEFAULT 1,
      unit_price REAL DEFAULT 0,
      amount REAL DEFAULT 0,
      remarks TEXT,
      FOREIGN KEY (quote_id) REFERENCES quotes(id) ON DELETE CASCADE
    );

    CREATE TABLE IF NOT EXISTS service_calls (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      call_number TEXT NOT NULL UNIQUE,
      customer_code TEXT,
      customer_name TEXT,
      project_code TEXT,
      subject TEXT,
      priority TEXT DEFAULT 'Normal',
      status TEXT DEFAULT 'Open',
      reported_date TEXT DEFAULT (datetime('now')),
      assigned_to TEXT,
      resolved_date TEXT,
      resolution TEXT,
      remarks TEXT,
      created_by TEXT,
      created_at TEXT DEFAULT (datetime('now')),
      modified_by TEXT,
      modified_at TEXT
    );

    CREATE TABLE IF NOT EXISTS timesheets (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      entry_number TEXT NOT NULL UNIQUE,
      user_name TEXT NOT NULL,
      project_code TEXT,
      work_date TEXT NOT NULL,
      hours REAL DEFAULT 0,
      activity TEXT,
      status TEXT DEFAULT 'Draft',
      approved_by TEXT,
      approved_date TEXT,
      remarks TEXT,
      created_by TEXT,
      created_at TEXT DEFAULT (datetime('now')),
      modified_by TEXT,
      modified_at TEXT
    );

    CREATE TABLE IF NOT EXISTS quality_ncs (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      nc_number TEXT NOT NULL UNIQUE,
      project_code TEXT,
      item_code TEXT,
      source TEXT,
      description TEXT NOT NULL,
      severity TEXT DEFAULT 'Minor',
      status TEXT DEFAULT 'Open',
      raised_by TEXT,
      raised_date TEXT DEFAULT (datetime('now')),
      assigned_to TEXT,
      closed_by TEXT,
      closed_date TEXT,
      corrective_action TEXT,
      remarks TEXT,
      created_by TEXT,
      created_at TEXT DEFAULT (datetime('now')),
      modified_by TEXT,
      modified_at TEXT
    );

    CREATE TABLE IF NOT EXISTS escalations (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      escalation_number TEXT NOT NULL UNIQUE,
      related_type TEXT,
      related_ref TEXT,
      project_code TEXT,
      title TEXT NOT NULL,
      description TEXT,
      priority TEXT DEFAULT 'High',
      status TEXT DEFAULT 'Open',
      raised_by TEXT,
      raised_date TEXT DEFAULT (datetime('now')),
      assigned_to TEXT,
      resolved_by TEXT,
      resolved_date TEXT,
      remarks TEXT,
      created_by TEXT,
      created_at TEXT DEFAULT (datetime('now')),
      modified_by TEXT,
      modified_at TEXT
    );

    CREATE TABLE IF NOT EXISTS complaints (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      complaint_number TEXT NOT NULL UNIQUE,
      customer_code TEXT,
      customer_name TEXT,
      subject TEXT NOT NULL,
      description TEXT,
      category TEXT DEFAULT 'Support',
      priority TEXT DEFAULT 'Normal',
      status TEXT DEFAULT 'Open',
      reported_date TEXT DEFAULT (datetime('now')),
      assigned_to TEXT,
      resolved_date TEXT,
      resolution TEXT,
      remarks TEXT,
      created_by TEXT,
      created_at TEXT DEFAULT (datetime('now')),
      modified_by TEXT,
      modified_at TEXT
    );

    CREATE TABLE IF NOT EXISTS gate_entries (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      entry_number TEXT NOT NULL UNIQUE,
      entry_type TEXT DEFAULT 'Inbound',
      vehicle_no TEXT,
      transporter TEXT,
      vendor_code TEXT,
      vendor_name TEXT,
      customer_code TEXT,
      customer_name TEXT,
      purpose TEXT,
      invoice_no TEXT,
      status TEXT DEFAULT 'Open',
      in_time TEXT DEFAULT (datetime('now')),
      out_time TEXT,
      remarks TEXT,
      created_by TEXT,
      created_at TEXT DEFAULT (datetime('now')),
      modified_by TEXT,
      modified_at TEXT
    );

    CREATE TABLE IF NOT EXISTS delivery_challans (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      dc_number TEXT NOT NULL UNIQUE,
      customer_code TEXT,
      customer_name TEXT,
      project_code TEXT,
      dc_date TEXT DEFAULT (datetime('now')),
      status TEXT DEFAULT 'Draft',
      vehicle_no TEXT,
      transporter TEXT,
      gin_number TEXT,
      remarks TEXT,
      created_by TEXT,
      created_at TEXT DEFAULT (datetime('now')),
      modified_by TEXT,
      modified_at TEXT
    );

    CREATE TABLE IF NOT EXISTS delivery_challan_details (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      dc_id INTEGER NOT NULL,
      dc_number TEXT NOT NULL,
      item_code TEXT NOT NULL,
      item_description TEXT,
      quantity REAL DEFAULT 0,
      uom TEXT DEFAULT 'NOS',
      remarks TEXT,
      FOREIGN KEY (dc_id) REFERENCES delivery_challans(id) ON DELETE CASCADE
    );

    CREATE INDEX IF NOT EXISTS ix_pr_status ON purchase_requests(status);
    CREATE INDEX IF NOT EXISTS ix_pr_vendor ON purchase_requests(vendor_code);
    CREATE INDEX IF NOT EXISTS ix_prd_pr ON purchase_request_details(pr_number);
    CREATE INDEX IF NOT EXISTS ix_po_ref ON purchase_orders(po_ref);
    CREATE INDEX IF NOT EXISTS ix_po_approved ON purchase_orders(po_approved);
    CREATE INDEX IF NOT EXISTS ix_comments_entity ON procurement_comments(entity_type, entity_ref);
    CREATE INDEX IF NOT EXISTS ix_indent_status ON indents(status);
    CREATE INDEX IF NOT EXISTS ix_wo_status ON work_orders(status);
    CREATE INDEX IF NOT EXISTS ix_stock_txn_type ON stock_transactions(txn_type);
    CREATE INDEX IF NOT EXISTS ix_service_status ON service_calls(status);
    CREATE INDEX IF NOT EXISTS ix_nc_status ON quality_ncs(status);
    CREATE INDEX IF NOT EXISTS ix_complaint_status ON complaints(status);
    CREATE INDEX IF NOT EXISTS ix_gate_status ON gate_entries(status);
  `);

  // Expand existing master tables (idempotent column adds)
  ensureColumn(db, 'vendors', 'address', 'TEXT');
  ensureColumn(db, 'vendors', 'contact_person', 'TEXT');
  ensureColumn(db, 'vendors', 'phone', 'TEXT');
  ensureColumn(db, 'vendors', 'email', 'TEXT');

  ensureColumn(db, 'items', 'category', "TEXT DEFAULT 'General'");
  ensureColumn(db, 'items', 'target_cost', 'REAL DEFAULT 0');
  ensureColumn(db, 'items', 'is_sales_product', 'INTEGER DEFAULT 0');

  ensureColumn(db, 'projects', 'customer_code', 'TEXT');
  ensureColumn(db, 'projects', 'customer_name', 'TEXT');
  ensureColumn(db, 'projects', 'approval_status', "TEXT DEFAULT 'Approved'");
  ensureColumn(db, 'projects', 'approved_by', 'TEXT');
  ensureColumn(db, 'projects', 'approved_date', 'TEXT');
  ensureColumn(db, 'projects', 'start_date', 'TEXT');
  ensureColumn(db, 'projects', 'end_date', 'TEXT');
  ensureColumn(db, 'projects', 'remarks', 'TEXT');
}
