import { getMssqlPool, mssqlQuery, sql } from '../db/mssql.js';

export function trim(value) {
  if (value == null) return '';
  return String(value).trim();
}

export function optionalText(value, max) {
  const text = trim(value);
  if (!text) return null;
  return max ? text.slice(0, max) : text;
}

export function asActiveStatus(value) {
  if (value === 0 || value === false || String(value).toLowerCase() === 'inactive') {
    return 'Inactive';
  }
  return 'Active';
}

export function asPinCode(value) {
  const text = trim(value);
  if (!text) return null;
  const n = Number(text);
  if (!Number.isFinite(n)) throw new Error('Pin code must be numeric');
  return Math.trunc(n);
}

export function splitAddress(address) {
  const text = trim(address);
  if (!text) return { address1: null, address2: null };
  if (text.length <= 255) return { address1: text, address2: null };
  return { address1: text.slice(0, 255), address2: text.slice(255, 510) };
}

function mapVendorRow(row) {
  return {
    ...row,
    active: Boolean(row.active),
    approved: Boolean(row.approved),
  };
}

function mapCustomerRow(row) {
  return {
    ...row,
    active: Boolean(row.active),
    approved: Boolean(row.approved),
  };
}

function mapItemRow(row) {
  return {
    ...row,
    active: row.active === 0 || row.active === false ? 0 : 1,
    // Legacy Standard Cost screen updates UnitCost; FixedCost is a separate part-master field.
    unitCost: Number(row.unitCost ?? row.standardCost ?? 0),
    standardCost: Number(row.standardCost ?? row.unitCost ?? 0),
    fixedCost: Number(row.fixedCost ?? 0),
    latestPurchasePrice: Number(row.latestPurchasePrice ?? row.unitCost ?? 0),
    targetCost: Number(row.targetCost ?? 0),
  };
}

export async function resolveStateId(payload = {}) {
  const rawCode = trim(payload.stateCode);
  if (rawCode) {
    const id = Number(rawCode);
    if (!Number.isFinite(id)) throw new Error('stateCode must be numeric');
    const result = await mssqlQuery(
      `
      SELECT TOP 1 Id AS id, StateCode AS stateCode, StateName AS stateName, CountryName AS country
      FROM StateMaster
      WHERE Id = @id OR StateCode = @id
      ORDER BY CASE WHEN Id = @id THEN 0 ELSE 1 END
      `,
      { id }
    );
    if (!result.recordset[0]) throw new Error(`State code ${rawCode} was not found`);
    return result.recordset[0];
  }

  const stateName = trim(payload.state);
  if (!stateName) throw new Error('state or stateCode is required');
  const result = await mssqlQuery(
    `
    SELECT TOP 1 Id AS id, StateCode AS stateCode, StateName AS stateName, CountryName AS country
    FROM StateMaster
    WHERE StateName = @stateName
    ORDER BY Id
    `,
    { stateName }
  );
  if (!result.recordset[0]) {
    throw new Error(`State "${stateName}" was not found in StateMaster`);
  }
  return result.recordset[0];
}

export async function listStates(q) {
  const params = {};
  let where = '1=1';
  if (q) {
    where = '(StateName LIKE @q OR CountryName LIKE @q OR CAST(ISNULL(StateCode,0) AS NVARCHAR(20)) LIKE @q)';
    params.q = `%${q}%`;
  }
  const result = await mssqlQuery(
    `
    SELECT TOP 500
      Id AS id,
      StateCode AS stateCode,
      StateName AS stateName,
      CountryName AS country
    FROM StateMaster
    WHERE ${where}
    ORDER BY StateName
    `,
    params
  );
  return result.recordset;
}

export async function listCities(q) {
  const params = {};
  let where = '1=1';
  if (q) {
    where = `(
      c.CityName LIKE @q
      OR CAST(c.ID AS NVARCHAR(20)) LIKE @q
      OR ISNULL(s.StateName, '') LIKE @q
      OR ISNULL(s.CountryName, '') LIKE @q
    )`;
    params.q = `%${q}%`;
  }
  const result = await mssqlQuery(
    `
    SELECT TOP 500
      c.ID AS id,
      CAST(c.ID AS NVARCHAR(32)) AS cityCode,
      c.CityName AS cityName,
      c.StateCode AS stateCode,
      s.StateName AS state,
      ISNULL(s.CountryName, 'India') AS country,
      1 AS active
    FROM CityMaster c
    LEFT JOIN StateMaster s ON s.Id = c.StateCode
    WHERE ${where}
    ORDER BY c.CityName, c.ID DESC
    `,
    params
  );
  return result.recordset;
}

export async function getCity(cityCode) {
  const id = Number(cityCode);
  if (!Number.isFinite(id)) return null;
  const result = await mssqlQuery(
    `
    SELECT TOP 1
      c.ID AS id,
      CAST(c.ID AS NVARCHAR(32)) AS cityCode,
      c.CityName AS cityName,
      c.StateCode AS stateCode,
      s.StateName AS state,
      ISNULL(s.CountryName, 'India') AS country,
      1 AS active
    FROM CityMaster c
    LEFT JOIN StateMaster s ON s.Id = c.StateCode
    WHERE c.ID = @id
    `,
    { id }
  );
  return result.recordset[0] || null;
}

export async function upsertCity(payload, user) {
  const cityName = trim(payload.cityName);
  if (!cityName) throw new Error('cityName is required');
  const state = await resolveStateId(payload);
  const byUser = user?.displayName || user?.username || 'web';
  const existingCode = trim(payload.cityCode);

  if (existingCode) {
    const id = Number(existingCode);
    if (!Number.isFinite(id)) throw new Error('cityCode must be numeric for live CityMaster updates');
    const existing = await getCity(id);
    if (!existing) throw new Error('City not found');
    await mssqlQuery(
      `
      UPDATE CityMaster
      SET CityName = @cityName, StateCode = @stateCode
      WHERE ID = @id
      `,
      { id, cityName, stateCode: state.id }
    );
    return getCity(id);
  }

  const insert = await mssqlQuery(
    `
    INSERT INTO CityMaster (CityName, StateCode, AddedBy)
    OUTPUT INSERTED.ID AS id
    VALUES (@cityName, @stateCode, @addedBy)
    `,
    { cityName, stateCode: state.id, addedBy: byUser }
  );
  return getCity(insert.recordset[0].id);
}

export async function listCustomers(q) {
  const params = {};
  let where = '1=1';
  if (q) {
    where = `(
      CusCode LIKE @q
      OR ISNULL(CustomerName, '') LIKE @q
      OR ISNULL(District, '') LIKE @q
      OR ISNULL(GSTCode, '') LIKE @q
    )`;
    params.q = `%${q}%`;
  }
  const result = await mssqlQuery(
    `
    SELECT TOP 500
      id,
      CusCode AS customerCode,
      CustomerName AS customerName,
      District AS city,
      CONCAT(
        ISNULL(Address1, ''),
        CASE WHEN NULLIF(Address2, '') IS NULL THEN '' ELSE ', ' + Address2 END
      ) AS address,
      GSTCode AS gstin,
      ContactPerson AS contactPerson,
      COALESCE(NULLIF(MobileNo, ''), PhoneNo) AS phone,
      EmailAddress AS email,
      CASE WHEN ISNULL(Status, 'Active') = 'Inactive' THEN 0 ELSE 1 END AS active,
      ISNULL(Approved, 0) AS approved,
      Status AS status
    FROM CustomerMaster
    WHERE ${where}
    ORDER BY CustomerName, CusCode
    `,
    params
  );
  return result.recordset.map(mapCustomerRow);
}

export async function getCustomer(customerCode) {
  const code = trim(customerCode);
  if (!code) return null;
  const result = await mssqlQuery(
    `
    SELECT TOP 1
      id,
      CusCode AS customerCode,
      CustomerName AS customerName,
      District AS city,
      CONCAT(
        ISNULL(Address1, ''),
        CASE WHEN NULLIF(Address2, '') IS NULL THEN '' ELSE ', ' + Address2 END
      ) AS address,
      GSTCode AS gstin,
      ContactPerson AS contactPerson,
      COALESCE(NULLIF(MobileNo, ''), PhoneNo) AS phone,
      EmailAddress AS email,
      CASE WHEN ISNULL(Status, 'Active') = 'Inactive' THEN 0 ELSE 1 END AS active,
      ISNULL(Approved, 0) AS approved,
      Status AS status
    FROM CustomerMaster
    WHERE CusCode = @customerCode
    `,
    { customerCode: code }
  );
  return result.recordset[0] ? mapCustomerRow(result.recordset[0]) : null;
}

export async function upsertCustomer(payload, user) {
  const customerCode = trim(payload.customerCode);
  const customerName = trim(payload.customerName);
  if (!customerCode || !customerName) {
    throw new Error('customerCode and customerName are required');
  }

  const { address1, address2 } = splitAddress(payload.address);
  const status = asActiveStatus(payload.active ?? payload.status);
  const byUser = user?.displayName || user?.username || 'web';
  const existing = await getCustomer(customerCode);
  const params = {
    customerCode,
    customerName,
    contactPerson: optionalText(payload.contactPerson, 255),
    address1,
    address2,
    phoneNo: optionalText(payload.phone, 64),
    email: optionalText(payload.email, 255),
    mobileNo: optionalText(payload.mobile || payload.phone, 64),
    district: optionalText(payload.city, 255),
    state: optionalText(payload.state, 255),
    country: optionalText(payload.country, 255) || 'India',
    pinCode: asPinCode(payload.pinCode),
    gstin: optionalText(payload.gstin, 100),
    status,
  };

  if (existing) {
    await mssqlQuery(
      `
      UPDATE CustomerMaster
      SET CustomerName = @customerName,
          ContactPerson = @contactPerson,
          Address1 = @address1,
          Address2 = @address2,
          PhoneNo = @phoneNo,
          EmailAddress = @email,
          MobileNo = @mobileNo,
          District = @district,
          State = @state,
          Country = @country,
          PinCode = @pinCode,
          Status = @status,
          GSTCode = @gstin
      WHERE CusCode = @customerCode
      `,
      params
    );
  } else {
    await mssqlQuery(
      `
      INSERT INTO CustomerMaster (
        CusCode, CustomerName, ContactPerson, Address1, Address2, PhoneNo, EmailAddress,
        FaxNo, MobileNo, District, State, Country, PinCode, TINNO, Status, CSTNo,
        CompanyURL, VatNumber, CustomerType, Others, CreatedDate, EccNumber,
        ServiceTaxRegNo, CIN, PAN, GSTCode, Approved, CreatedBy
      ) VALUES (
        @customerCode, @customerName, @contactPerson, @address1, @address2, @phoneNo, @email,
        NULL, @mobileNo, @district, @state, @country, @pinCode, NULL, @status, NULL,
        NULL, NULL, NULL, NULL, CONVERT(NVARCHAR(30), GETDATE(), 120), NULL,
        NULL, NULL, NULL, @gstin, 0, @createdBy
      )
      `,
      { ...params, createdBy: byUser }
    );
  }

  return getCustomer(customerCode);
}

export async function listVendors(q) {
  const params = {};
  let where = '1=1';
  if (q) {
    where = `(
      VendorCode LIKE @q
      OR ISNULL(VendorName, '') LIKE @q
      OR ISNULL(District, '') LIKE @q
      OR ISNULL(GSTCode, '') LIKE @q
    )`;
    params.q = `%${q}%`;
  }
  const result = await mssqlQuery(
    `
    SELECT TOP 500
      v.id,
      v.VendorCode AS vendorCode,
      v.VendorName AS vendorName,
      v.District AS city,
      v.GSTCode AS gstin,
      CONCAT(
        ISNULL(v.Address1, ''),
        CASE WHEN NULLIF(v.Address2, '') IS NULL THEN '' ELSE ', ' + v.Address2 END,
        CASE WHEN NULLIF(v.Address3, '') IS NULL THEN '' ELSE ', ' + v.Address3 END
      ) AS address,
      v.ContactPerson AS contactPerson,
      COALESCE(NULLIF(v.MobileNo, ''), v.PhoneNo) AS phone,
      v.EmailAddress AS email,
      CASE WHEN ISNULL(v.Status, 'Active') = 'Inactive' THEN 0 ELSE 1 END AS active,
      ISNULL(v.Approved, 0) AS approved
    FROM Vendors v
    WHERE ${where}
    ORDER BY v.VendorName, v.VendorCode
    `,
    params
  );
  return result.recordset.map(mapVendorRow);
}

export async function getVendor(vendorCode) {
  const code = trim(vendorCode);
  if (!code) return null;
  const rows = await listVendors(code);
  return rows.find((row) => row.vendorCode === code) || null;
}

export async function upsertVendor(payload, user) {
  const vendorCode = trim(payload.vendorCode);
  const vendorName = trim(payload.vendorName);
  if (!vendorCode || !vendorName) {
    throw new Error('vendorCode and vendorName are required');
  }

  const { address1, address2 } = splitAddress(payload.address);
  const status = asActiveStatus(payload.active ?? payload.status);
  const byUser = user?.displayName || user?.username || 'web';
  const existing = await mssqlQuery(
    `SELECT TOP 1 VendorCode FROM Vendors WHERE VendorCode = @vendorCode`,
    { vendorCode }
  );
  const params = {
    vendorCode,
    vendorName,
    contactPerson: optionalText(payload.contactPerson, 255),
    address1,
    address2,
    phoneNo: optionalText(payload.phone, 255),
    email: optionalText(payload.email, 255),
    mobileNo: optionalText(payload.mobile || payload.phone, 255),
    district: optionalText(payload.city, 255),
    state: optionalText(payload.state, 255),
    country: optionalText(payload.country, 255) || 'India',
    pinCode: optionalText(payload.pinCode, 255),
    stateCode: optionalText(payload.stateCode, 50),
    gstin: optionalText(payload.gstin, 255),
    status,
  };

  if (existing.recordset[0]) {
    await mssqlQuery(
      `
      UPDATE Vendors
      SET Approved = 1,
          VendorName = @vendorName,
          ContactPerson = @contactPerson,
          Address1 = @address1,
          Address2 = @address2,
          PhoneNo = @phoneNo,
          EmailAddress = @email,
          MobileNo = @mobileNo,
          District = @district,
          State = @state,
          Country = @country,
          PinCode = @pinCode,
          StateCode = @stateCode,
          Status = @status,
          GSTCode = @gstin
      WHERE VendorCode = @vendorCode
      `,
      params
    );
  } else {
    await mssqlQuery(
      `
      INSERT INTO Vendors (
        VendorCode, VendorName, ContactPerson, Address1, Address2, PhoneNo, EmailAddress,
        FaxNo, MobileNo, District, State, Country, PinCode, StateCode, Status,
        ShippedCountry, ShippedStateCode, ShippedAddress, GSTCode, ShippedGSTCode,
        Approved, CreatedBy, CreatedDate, MSMERegistered, AccountNo, BeneficiaryName,
        IFSCCode, BankBranch, SwiftCode, CINNo, PANNo, PaymentTerm, [Currency], [FX Rate]
      ) VALUES (
        @vendorCode, @vendorName, @contactPerson, @address1, @address2, @phoneNo, @email,
        NULL, @mobileNo, @district, @state, @country, @pinCode, @stateCode, @status,
        @country, @stateCode, @address1, @gstin, @gstin,
        0, @createdBy, CONVERT(NVARCHAR(30), GETDATE(), 120), NULL, NULL, NULL,
        NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL
      )
      `,
      { ...params, createdBy: byUser }
    );
  }

  return getVendor(vendorCode);
}

export async function listItems(q) {
  const params = {};
  let where = '1=1';
  if (q) {
    where = `(ItemCode LIKE @q OR ISNULL(ItemDescription, '') LIKE @q OR ISNULL(DrawingNo, '') LIKE @q)`;
    params.q = `%${q}%`;
  }
  try {
    const result = await mssqlQuery(
      `
      SELECT TOP 500
        ItemCode AS itemCode,
        ItemDescription AS itemDescription,
        Units AS uom,
        CAST(NULL AS NVARCHAR(255)) AS make,
        CAST(NULL AS NVARCHAR(255)) AS specification,
        CAST(NULL AS NVARCHAR(255)) AS mfgPartNo,
        DrawingNo AS drawingNo,
        HSNSACCode AS hsnCode,
        UnitCost AS unitCost,
        UnitCost AS standardCost,
        FixedCost AS fixedCost,
        UnitCost AS latestPurchasePrice,
        Type AS category,
        TargetCost AS targetCost,
        CASE WHEN ISNULL(Status, 'Active') = 'Inactive' THEN 0 ELSE 1 END AS active
      FROM ItemMaster
      WHERE ${where}
      ORDER BY ItemCode
      `,
      params
    );
    return result.recordset.map(mapItemRow);
  } catch {
    const result = await mssqlQuery(
      `SELECT TOP 200 ItemCode AS itemCode, ItemDescription AS itemDescription FROM ItemMaster ORDER BY ItemCode`
    );
    return result.recordset.map((row) =>
      mapItemRow({
        ...row,
        uom: null,
        unitCost: 0,
        standardCost: 0,
        fixedCost: 0,
        latestPurchasePrice: 0,
        targetCost: 0,
        active: 1,
      })
    );
  }
}

export async function getItem(itemCode) {
  const code = trim(itemCode);
  if (!code) return null;
  const rows = await listItems(code);
  return rows.find((row) => row.itemCode === code) || null;
}

export async function upsertItem(payload, user) {
  const itemCode = trim(payload.itemCode);
  if (!itemCode) throw new Error('itemCode is required');
  const itemDescription = trim(payload.itemDescription) || itemCode;
  const uom = optionalText(payload.uom, 15) || 'NOS';
  const status = asActiveStatus(payload.active ?? payload.status);

  // standardCost / unitCost → ItemMaster.UnitCost (legacy frmStandardCost)
  // fixedCost → ItemMaster.FixedCost (part master); defaults to unit cost on insert
  const hasStandard =
    payload.standardCost != null && payload.standardCost !== ''
      ? true
      : payload.unitCost != null && payload.unitCost !== '';
  const standardCost = Number(
    payload.standardCost != null && payload.standardCost !== ''
      ? payload.standardCost
      : payload.unitCost != null && payload.unitCost !== ''
        ? payload.unitCost
        : 0
  );
  const hasFixed = payload.fixedCost != null && payload.fixedCost !== '';
  const fixedCost = Number(hasFixed ? payload.fixedCost : standardCost || 0.01);

  if (!Number.isFinite(standardCost) || standardCost < 0) {
    throw new Error('standardCost must be a non-negative number');
  }
  if (!Number.isFinite(fixedCost) || fixedCost < 0) {
    throw new Error('fixedCost must be a non-negative number');
  }

  const byUser = user?.displayName || user?.username || 'web';
  const pool = await getMssqlPool();
  const transaction = new sql.Transaction(pool);
  await transaction.begin();

  try {
    const existingReq = new sql.Request(transaction);
    existingReq.input('ItemCode', sql.NVarChar, itemCode);
    const existing = await existingReq.query(
      `SELECT TOP 1 ItemCode, UnitCost, FixedCost FROM ItemMaster WHERE ItemCode = @ItemCode`
    );
    const current = existing.recordset[0];

    if (current) {
      const previousUnit = Number(current.UnitCost || 0);
      const previousFixed = Number(current.FixedCost || 0);
      const nextUnit = hasStandard ? standardCost : previousUnit;
      const nextFixed = hasFixed ? fixedCost : previousFixed;

      const update = new sql.Request(transaction);
      update.input('ItemCode', sql.NVarChar, itemCode);
      update.input('ItemDescription', sql.NVarChar, itemDescription);
      update.input('Status', sql.NVarChar, status);
      update.input('UnitCost', sql.Float, nextUnit || 0.01);
      update.input('FixedCost', sql.Decimal(18, 2), nextFixed || 0.01);
      update.input('Units', sql.NVarChar, uom);
      update.input('HSNSACCode', sql.NVarChar, optionalText(payload.hsnCode, 255));
      update.input('DrawingNo', sql.NVarChar, optionalText(payload.drawingNo || payload.mfgPartNo, 255));
      update.input('Type', sql.NVarChar, optionalText(payload.category, 255) || 'General');
      update.input('Remarks', sql.NVarChar, optionalText(payload.specification || payload.remarks, 255));
      await update.query(`
        UPDATE ItemMaster
        SET ItemDescription = @ItemDescription,
            Status = @Status,
            UnitCost = @UnitCost,
            FixedCost = @FixedCost,
            Units = @Units,
            HSNSACCode = @HSNSACCode,
            DrawingNo = @DrawingNo,
            Type = @Type,
            Remarks = @Remarks
        WHERE ItemCode = @ItemCode
      `);

      if (hasStandard && previousUnit !== nextUnit) {
        const history = new sql.Request(transaction);
        history.input('ItemCode', sql.NVarChar, itemCode);
        history.input('StdCost', sql.Float, nextUnit);
        history.input('UpdateBy', sql.VarChar, String(byUser).slice(0, 50));
        history.input(
          'Remarks',
          sql.NVarChar,
          optionalText(payload.costRemarks || payload.remarks || 'Standard cost update', 255)
        );
        await history.query(`
          INSERT INTO ItemStdCostHistory (ItemCode, StdCost, UpdateBy, Remarks)
          VALUES (@ItemCode, @StdCost, @UpdateBy, @Remarks)
        `);
      }
    } else {
      const insertUnit = standardCost || 0.01;
      const insertFixed = hasFixed ? fixedCost : insertUnit;
      const insert = new sql.Request(transaction);
      insert.input('ItemCode', sql.NVarChar, itemCode);
      insert.input('ItemDescription', sql.NVarChar, itemDescription);
      insert.input('UnitCost', sql.Float, insertUnit);
      insert.input('Type', sql.NVarChar, optionalText(payload.category, 255) || 'General');
      insert.input('Status', sql.NVarChar, status);
      insert.input('Remarks', sql.NVarChar, optionalText(payload.specification || payload.remarks, 255));
      insert.input('Units', sql.NVarChar, uom);
      insert.input('Others', sql.VarChar, optionalText(payload.make, 255));
      insert.input('PurchaseType', sql.NVarChar, optionalText(payload.purchaseType, 64) || 'Bought Out');
      insert.input('ItemTypeCode', sql.NVarChar, optionalText(payload.itemTypeCode, 8) || 'GEN');
      insert.input('CreatedBy', sql.NVarChar, String(byUser).slice(0, 64));
      insert.input('HSNSACCode', sql.NVarChar, optionalText(payload.hsnCode, 255));
      insert.input('DrawingNo', sql.NVarChar, optionalText(payload.drawingNo || payload.mfgPartNo, 255));
      insert.input('TypeofStorage', sql.NVarChar, optionalText(payload.storageType, 40));
      insert.input('FixedCost', sql.Decimal(18, 2), insertFixed);
      await insert.query(`
        INSERT INTO ItemMaster (
          ItemCode, ItemDescription, CreatedDate, UnitCost, Type, Status, Remarks, Units,
          Others, OpeningQuantity, PurchaseType, ItemTypeCode, CreatedBy, AvailableQty,
          Location, HSNSACCode, DrawingNo, TypeofStorage, FixedCost
        ) VALUES (
          @ItemCode, @ItemDescription, CONVERT(NVARCHAR(30), GETDATE(), 120), @UnitCost, @Type,
          @Status, @Remarks, @Units, @Others, 0, @PurchaseType, @ItemTypeCode, @CreatedBy, 0.0,
          NULL, @HSNSACCode, @DrawingNo, @TypeofStorage, @FixedCost
        )
      `);

      if (insertUnit > 0) {
        const history = new sql.Request(transaction);
        history.input('ItemCode', sql.NVarChar, itemCode);
        history.input('StdCost', sql.Float, insertUnit);
        history.input('UpdateBy', sql.VarChar, String(byUser).slice(0, 50));
        history.input('Remarks', sql.NVarChar, 'Initial standard cost');
        await history.query(`
          INSERT INTO ItemStdCostHistory (ItemCode, StdCost, UpdateBy, Remarks)
          VALUES (@ItemCode, @StdCost, @UpdateBy, @Remarks)
        `);
      }
    }

    await transaction.commit();
  } catch (error) {
    await transaction.rollback();
    throw error;
  }

  return getItem(itemCode);
}

export async function listProjects(q) {
  const where = [];
  const params = {};
  if (q) {
    where.push(
      `(ProjectCode LIKE @q OR ISNULL(ProjectDescription, '') LIKE @q OR ISNULL(Customer, '') LIKE @q)`
    );
    params.q = `%${q}%`;
  }
  const result = await mssqlQuery(
    `
    SELECT TOP 1000
      Id AS id,
      ProjectCode AS projectCode,
      ProjectDescription AS projectName,
      SystemSubType AS productNo,
      CustomerCode AS customerCode,
      Customer AS customerName,
      Status AS status,
      ProjectInstallStatus AS installationStatus,
      ShipmentDate AS shipmentDate,
      DateCreated AS startDate,
      Remarks AS remarks,
      ApprovedBy AS approvedBy,
      ApprovedDate AS approvedDate,
      CASE
        WHEN NULLIF(ApprovedBy, '') IS NULL THEN 'Pending'
        ELSE 'Approved'
      END AS approvalStatus
    FROM ProjectMaster
    ${where.length ? `WHERE ${where.join(' AND ')}` : ''}
    ORDER BY Id DESC
    `,
    params
  );
  return result.recordset;
}

export async function upsertProject(payload, user) {
  const projectCode = trim(payload.projectCode);
  const projectName = trim(payload.projectName);
  if (!projectCode || !projectName) throw new Error('projectCode and projectName are required');
  const byUser = user?.displayName || user?.username || 'web';
  const existing = await mssqlQuery(
    `SELECT TOP 1 ProjectCode FROM ProjectMaster WHERE ProjectCode = @projectCode`,
    { projectCode }
  );
  const params = {
    projectCode,
    projectName,
    customer: optionalText(payload.customerName || payload.customer, 255),
    customerCode: optionalText(payload.customerCode, 55),
    status: optionalText(payload.status, 255) || 'Active',
    remarks: optionalText(payload.remarks, 255),
    installStatus: optionalText(payload.installationStatus, 100),
    shipmentDate: optionalText(payload.shipmentDate, 40),
    createdBy: byUser,
  };
  if (existing.recordset[0]) {
    await mssqlQuery(
      `
      UPDATE ProjectMaster
      SET ProjectDescription = @projectName,
          Customer = @customer,
          CustomerCode = @customerCode,
          Status = @status,
          Remarks = @remarks,
          ProjectInstallStatus = @installStatus,
          ShipmentDate = @shipmentDate
      WHERE ProjectCode = @projectCode
      `,
      params
    );
  } else {
    await mssqlQuery(
      `
      INSERT INTO ProjectMaster
        (ProjectCode, ProjectDescription, Customer, CustomerCode, Status, Remarks,
         CreatedBy, DateCreated, ProjectInstallStatus, ShipmentDate, ShortShipment, ProjectTransfer, ProjectTransfered)
      VALUES
        (@projectCode, @projectName, @customer, @customerCode, @status, @remarks,
         @createdBy, CONVERT(NVARCHAR(30), GETDATE(), 120), @installStatus, @shipmentDate, 0, 0, 0)
      `,
      params
    );
  }
  const rows = await listProjects(projectCode);
  return rows.find((p) => p.projectCode === projectCode) || rows[0];
}

export async function approveProject(projectCode, { action, user }) {
  const code = trim(projectCode);
  if (!code) throw new Error('projectCode is required');
  if (action !== 'approve' && action !== 'reject') throw new Error('Unknown action');
  const byUser = user?.displayName || user?.username || 'web';
  if (action === 'approve') {
    await mssqlQuery(
      `
      UPDATE ProjectMaster
      SET ApprovedBy = @byUser,
          ApprovedDate = CONVERT(NVARCHAR(30), GETDATE(), 120),
          Status = 'Active'
      WHERE ProjectCode = @code
      `,
      { byUser, code }
    );
  } else {
    await mssqlQuery(
      `
      UPDATE ProjectMaster
      SET Status = 'Rejected',
          Remarks = ISNULL(Remarks, '') + ' | Rejected by ' + @byUser
      WHERE ProjectCode = @code
      `,
      { byUser, code }
    );
  }
  const rows = await listProjects(code);
  return rows.find((p) => p.projectCode === code) || rows[0];
}

export async function listTargetCostHistory(q) {
  const params = {};
  let where = '1=1';
  if (q) {
    where = '(h.ItemCode LIKE @q)';
    params.q = `%${q}%`;
  }
  const result = await mssqlQuery(
    `
    SELECT TOP 500
      h.Id AS id,
      h.ItemCode AS itemCode,
      i.ItemDescription AS itemDescription,
      h.TargetCost AS targetCost,
      h.UpdateBy AS updatedBy,
      h.UpdateDate AS updatedAt,
      h.Remarks AS remarks,
      h.GMApprove AS gmApproved,
      h.GMApproveDate AS gmApprovedDate
    FROM ItemTargetCostHistory h
    LEFT JOIN ItemMaster i ON i.ItemCode = h.ItemCode
    WHERE ${where}
    ORDER BY h.Id DESC
    `,
    params
  );
  return result.recordset.map((row) => ({
    ...row,
    gmApproved: row.gmApproved == null ? null : Boolean(row.gmApproved),
  }));
}

export async function proposeTargetCost(payload, user) {
  const itemCode = trim(payload.itemCode);
  const targetCost = Number(payload.targetCost);
  if (!itemCode) throw new Error('itemCode is required');
  if (!Number.isFinite(targetCost) || targetCost < 0) throw new Error('targetCost must be non-negative');
  await mssqlQuery(
    `
    INSERT INTO ItemTargetCostHistory (ItemCode, TargetCost, UpdateBy, Remarks, UpdateDate)
    VALUES (@itemCode, @targetCost, @byUser, @remarks, GETDATE())
    `,
    {
      itemCode,
      targetCost,
      byUser: String(user?.displayName || user?.username || 'web').slice(0, 50),
      remarks: optionalText(payload.remarks, 500),
    }
  );
  const rows = await listTargetCostHistory(itemCode);
  return rows[0];
}

export async function decideTargetCost(id, { action, user }) {
  const historyId = Number(id);
  if (!Number.isFinite(historyId)) throw new Error('Invalid target cost id');
  if (!['approve', 'reject'].includes(action)) throw new Error('Unknown action');
  const existing = await mssqlQuery(
    `SELECT TOP 1 * FROM ItemTargetCostHistory WHERE Id = @id`,
    { id: historyId }
  );
  const row = existing.recordset[0];
  if (!row) throw new Error('Target cost request not found');
  if (row.GMApprove != null) throw new Error('Target cost already decided');

  if (action === 'approve') {
    const pool = await getMssqlPool();
    const tx = new sql.Transaction(pool);
    await tx.begin();
    try {
      const upd = new sql.Request(tx);
      upd.input('id', sql.Int, historyId);
      upd.input('byUser', sql.VarChar, String(user?.displayName || user?.username || 'web').slice(0, 50));
      await upd.query(`
        UPDATE ItemTargetCostHistory
        SET GMApprove = 1, GMApproveDate = CAST(GETDATE() AS date)
        WHERE Id = @id
      `);
      const item = new sql.Request(tx);
      item.input('ItemCode', sql.NVarChar, row.ItemCode);
      item.input('TargetCost', sql.Float, row.TargetCost);
      await item.query(`UPDATE ItemMaster SET TargetCost = @TargetCost WHERE ItemCode = @ItemCode`);
      await tx.commit();
    } catch (err) {
      await tx.rollback();
      throw err;
    }
  } else {
    await mssqlQuery(
      `
      UPDATE ItemTargetCostHistory
      SET GMApprove = 0, GMApproveDate = CAST(GETDATE() AS date),
          Remarks = ISNULL(Remarks, '') + ' | Rejected'
      WHERE Id = @id
      `,
      { id: historyId }
    );
  }
  const rows = await listTargetCostHistory(row.ItemCode);
  return rows.find((r) => r.id === historyId) || rows[0];
}
