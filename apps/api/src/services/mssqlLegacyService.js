import { mssqlQuery } from '../db/mssql.js';

/** Read helpers against existing ERP_Database tables used by the C# app. */

export async function listPurchaseRequests({ status, vendor, project, q } = {}) {
  const where = ['1=1'];
  const params = {};
  if (status && status !== 'All') {
    where.push('Status = @status');
    params.status = status;
  }
  if (vendor) {
    where.push('(VendorCode LIKE @vendor OR ISNULL(VendorName,\'\') LIKE @vendor)');
    params.vendor = `%${vendor}%`;
  }
  if (project) {
    where.push('ProjectCode LIKE @project');
    params.project = `%${project}%`;
  }
  if (q) {
    where.push('(PRNumber LIKE @q OR ISNULL(Remarks,\'\') LIKE @q)');
    params.q = `%${q}%`;
  }

  const result = await mssqlQuery(
    `
    SELECT TOP 500
      PRID AS id,
      PRNumber AS prNumber,
      ProjectCode AS projectCode,
      ProductNo AS productNo,
      VendorCode AS vendorCode,
      VendorName AS vendorName,
      TotalAmount AS totalAmount,
      Status AS status,
      RequestedBy AS requestedBy,
      RequestDate AS requestDate,
      ApprovedBy AS approvedBy,
      ApprovedDate AS approvedDate,
      RejectionReason AS rejectionReason,
      HoldReason AS holdReason,
      Remarks AS remarks,
      ISNULL(IsClubbed, 0) AS isClubbed,
      ClubbedFromPRIDs AS clubbedFromPrIds,
      DATEDIFF(DAY, RequestDate, GETDATE()) AS ageDays
    FROM PurchaseRequest
    WHERE ${where.join(' AND ')}
    ORDER BY RequestDate DESC, PRID DESC
    `,
    params
  );
  return result.recordset.map((r) => ({ ...r, isClubbed: !!r.isClubbed }));
}

export async function getPurchaseRequest(prNumber) {
  const header = await mssqlQuery(
    `
    SELECT TOP 1
      PRID AS id, PRNumber AS prNumber, ProjectCode AS projectCode, ProductNo AS productNo,
      VendorCode AS vendorCode, VendorName AS vendorName, TotalAmount AS totalAmount,
      Status AS status, RequestedBy AS requestedBy, RequestDate AS requestDate,
      ApprovedBy AS approvedBy, ApprovedDate AS approvedDate, Remarks AS remarks
    FROM PurchaseRequest WHERE PRNumber = @prNumber
    `,
    { prNumber }
  );
  if (!header.recordset[0]) return null;

  let lines = [];
  try {
    const detail = await mssqlQuery(
      `
      SELECT
        DetailID AS id, PRID AS prId, PRNumber AS prNumber, ItemCode AS itemCode,
        ItemDescription AS itemDescription, Specification AS specification, Make AS make,
        MfgPartNo AS mfgPartNo, Quantity AS quantity, UOM AS uom, UnitCost AS unitCost,
        TotalCost AS totalCost, VendorCode AS vendorCode, VendorName AS vendorName,
        ISNULL(LineStatus, 'Pending') AS lineStatus
      FROM PurchaseRequestDetailNew
      WHERE PRNumber = @prNumber
      ORDER BY DetailID
      `,
      { prNumber }
    );
    lines = detail.recordset;
  } catch {
    // Fallback if only PurchaseRequestDetail exists
    try {
      const detail = await mssqlQuery(
        `
        SELECT DetailID AS id, PRID AS prId, ItemCode AS itemCode,
          ItemDescription AS itemDescription, Quantity AS quantity, UOM AS uom,
          UnitCost AS unitCost, TotalCost AS totalCost
        FROM PurchaseRequestDetail
        WHERE PRID = @prId
        ORDER BY DetailID
        `,
        { prId: header.recordset[0].id }
      );
      lines = detail.recordset;
    } catch {
      lines = [];
    }
  }

  return { ...header.recordset[0], lines };
}

export async function listPurchaseOrders({ status, poNumber, vendor, project } = {}) {
  const where = ['1=1'];
  const params = {};
  if (poNumber) {
    where.push('DBOMNo LIKE @poNumber');
    params.poNumber = `%${poNumber}%`;
  }
  if (vendor) {
    where.push('(VendorCode LIKE @vendor OR ISNULL(VendorName,\'\') LIKE @vendor)');
    params.vendor = `%${vendor}%`;
  }
  if (project) {
    where.push('ProjectCode LIKE @project');
    params.project = `%${project}%`;
  }

  const result = await mssqlQuery(
    `
    SELECT TOP 1000
      ID AS id,
      DBOMNo AS poRef,
      ProjectCode AS projectCode,
      VendorCode AS vendorCode,
      VendorName AS vendorName,
      ItemCode AS itemCode,
      ItemDescription AS itemDescription,
      RequariedQty AS requiredQty,
      ISNULL(RemainingQty, RequariedQty) AS remainingQty,
      UnitPrice AS unitPrice,
      Amount AS amount,
      ISNULL(IGSTAmount,0) AS igstAmount,
      ISNULL(SGSTAmount,0) AS sgstAmount,
      ISNULL(CGSTAmount,0) AS cgstAmount,
      ISNULL(POApproved,0) AS poApproved,
      PMApproved AS pmApproved,
      MHApproved AS mhApproved,
      OMApproved AS omApproved,
      GMCostApproved AS gmApproved,
      POGeneratedBy AS poGeneratedBy,
      POGeneratedDate AS poGeneratedDate,
      POSenttoVendorBy AS poSentToVendorBy,
      PreparedBy AS preparedBy,
      POPreparedDate AS preparedDate
    FROM PurchaseOrder
    WHERE ${where.join(' AND ')}
    ORDER BY ID DESC
    `,
    params
  );

  // Group by poRef like the web UI expects
  const map = new Map();
  for (const row of result.recordset) {
    const key = row.poRef || String(row.id);
    if (!map.has(key)) {
      map.set(key, {
        poRef: key,
        vendorCode: row.vendorCode,
        vendorName: row.vendorName,
        projectCode: row.projectCode,
        poApproved: !!row.poApproved,
        pmApproved: !!row.pmApproved,
        mhApproved: !!row.mhApproved,
        omApproved: !!row.omApproved,
        gmApproved: !!row.gmApproved,
        poGeneratedBy: row.poGeneratedBy,
        preparedBy: row.preparedBy,
        preparedDate: row.preparedDate,
        lines: [],
        baseAmount: 0,
        gstAmount: 0,
        totalAmount: 0,
      });
    }
    const g = map.get(key);
    g.lines.push(row);
    g.baseAmount += Number(row.amount || 0);
    g.gstAmount +=
      Number(row.igstAmount || 0) + Number(row.sgstAmount || 0) + Number(row.cgstAmount || 0);
    g.totalAmount = g.baseAmount + g.gstAmount;
  }

  let items = [...map.values()];
  if (status && status !== 'All') {
    // Soft filter — keep all if status computation differs
    items = items.filter((g) => {
      if (status === 'Ready to Generate') return g.poApproved && !g.poGeneratedBy;
      if (status === 'PO Generated') return !!g.poGeneratedBy;
      return true;
    });
  }
  return {
    statusOptions: [
      'All',
      'Ready to Generate',
      'PO Generated',
      'Pending PM / Dept Approval',
      'Pending MH Approval',
    ],
    items,
  };
}

export async function listVendors() {
  try {
    const result = await mssqlQuery(
      `
      SELECT TOP 500
        VendorCode AS vendorCode,
        VendorName AS vendorName,
        City AS city,
        GSTNo AS gstin
      FROM VendorMaster
      ORDER BY VendorName
      `
    );
    return result.recordset;
  } catch {
    // Column names vary across ERP versions
    const result = await mssqlQuery(`SELECT TOP 200 * FROM VendorMaster`);
    return result.recordset;
  }
}

export async function listItems(q) {
  const params = {};
  let where = '1=1';
  if (q) {
    where = '(ItemCode LIKE @q OR ISNULL(ItemDescription,\'\') LIKE @q)';
    params.q = `%${q}%`;
  }
  try {
    const result = await mssqlQuery(
      `
      SELECT TOP 500
        ItemCode AS itemCode,
        ItemDescription AS itemDescription,
        UOM AS uom,
        Make AS make,
        HSNCode AS hsnCode,
        StandardCost AS standardCost
      FROM ItemMaster
      WHERE ${where}
      ORDER BY ItemCode
      `,
      params
    );
    return result.recordset;
  } catch {
    const result = await mssqlQuery(`SELECT TOP 200 * FROM ItemMaster`);
    return result.recordset;
  }
}

export async function dashboardSummary() {
  const pending = await mssqlQuery(
    `SELECT COUNT(*) AS count, ISNULL(SUM(TotalAmount),0) AS amount FROM PurchaseRequest WHERE Status = 'Pending'`
  );
  const approved = await mssqlQuery(
    `SELECT COUNT(*) AS count, ISNULL(SUM(TotalAmount),0) AS amount FROM PurchaseRequest WHERE Status IN ('Approved','Partially Converted')`
  );
  let posAwaiting = { count: 0, amount: 0 };
  try {
    const r = await mssqlQuery(
      `SELECT COUNT(DISTINCT DBOMNo) AS count, ISNULL(SUM(Amount),0) AS amount FROM PurchaseOrder WHERE ISNULL(POApproved,0) = 0`
    );
    posAwaiting = r.recordset[0];
  } catch {
    /* ignore */
  }

  return {
    metrics: [
      {
        metric: 'Pending PR Approvals',
        count: pending.recordset[0].count,
        amount: pending.recordset[0].amount,
      },
      {
        metric: 'Approved PRs Ready for PO',
        count: approved.recordset[0].count,
        amount: approved.recordset[0].amount,
      },
      {
        metric: 'POs Awaiting Approval',
        count: posAwaiting.count,
        amount: posAwaiting.amount,
      },
      { metric: 'Item Codes Pending', count: 0, amount: 0 },
    ],
    agingPRs: (
      await mssqlQuery(
        `
        SELECT TOP 20
          PRNumber AS prNumber, ProjectCode AS projectCode, VendorCode AS vendorCode,
          TotalAmount AS totalAmount, Status AS status, RequestedBy AS requestedBy,
          RequestDate AS requestDate, DATEDIFF(DAY, RequestDate, GETDATE()) AS ageDays
        FROM PurchaseRequest
        WHERE Status IN ('Pending','On Hold')
        ORDER BY RequestDate ASC
        `
      )
    ).recordset,
    recentActivity: [],
    source: 'mssql',
  };
}
