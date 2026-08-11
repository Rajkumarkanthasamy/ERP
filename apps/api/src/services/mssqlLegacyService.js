import { mssqlQuery } from '../db/mssql.js';

/** Read helpers against existing ERP_Database tables used by the C# app. */

export async function listPurchaseRequests({ status, vendor, project, q } = {}) {
  const where = ['1=1'];
  const params = {};
  if (status && status !== 'All') {
    where.push('pr.Status = @status');
    params.status = status;
  }
  if (vendor) {
    where.push('(pr.VendorCode LIKE @vendor OR ISNULL(pr.VendorName,\'\') LIKE @vendor)');
    params.vendor = `%${vendor}%`;
  }
  if (project) {
    where.push('pr.ProjectCode LIKE @project');
    params.project = `%${project}%`;
  }
  if (q) {
    where.push('(pr.PRNumber LIKE @q OR ISNULL(pr.Remarks,\'\') LIKE @q)');
    params.q = `%${q}%`;
  }

  const result = await mssqlQuery(
    `
    SELECT TOP 500
      pr.PRID AS id,
      pr.PRNumber AS prNumber,
      pr.ProjectCode AS projectCode,
      detail.ProductNo AS productNo,
      pr.VendorCode AS vendorCode,
      pr.VendorName AS vendorName,
      pr.TotalAmount AS totalAmount,
      pr.Status AS status,
      pr.RequestedBy AS requestedBy,
      pr.RequestDate AS requestDate,
      pr.ApprovedBy AS approvedBy,
      pr.ApprovedDate AS approvedDate,
      pr.RejectionReason AS rejectionReason,
      pr.HoldReason AS holdReason,
      pr.Remarks AS remarks,
      ISNULL(pr.IsClubbed, 0) AS isClubbed,
      pr.ClubbedFromPRIDs AS clubbedFromPrIds,
      DATEDIFF(DAY, pr.RequestDate, GETDATE()) AS ageDays
    FROM PurchaseRequest pr
    OUTER APPLY (
      SELECT TOP 1 d.ProductNo
      FROM PurchaseRequestDetailNew d
      WHERE d.PRNumber = pr.PRNumber
      ORDER BY d.DetailID
    ) detail
    WHERE ${where.join(' AND ')}
    ORDER BY pr.RequestDate DESC, pr.PRID DESC
    `,
    params
  );
  return result.recordset.map((r) => ({ ...r, isClubbed: !!r.isClubbed }));
}

export async function getPurchaseRequest(prNumber) {
  const header = await mssqlQuery(
    `
    SELECT TOP 1
      pr.PRID AS id, pr.PRNumber AS prNumber, pr.ProjectCode AS projectCode,
      detail.ProductNo AS productNo, pr.VendorCode AS vendorCode,
      pr.VendorName AS vendorName, pr.TotalAmount AS totalAmount,
      pr.Status AS status, pr.RequestedBy AS requestedBy, pr.RequestDate AS requestDate,
      pr.ApprovedBy AS approvedBy, pr.ApprovedDate AS approvedDate, pr.Remarks AS remarks
    FROM PurchaseRequest pr
    OUTER APPLY (
      SELECT TOP 1 d.ProductNo
      FROM PurchaseRequestDetailNew d
      WHERE d.PRNumber = pr.PRNumber
      ORDER BY d.DetailID
    ) detail
    WHERE pr.PRNumber = @prNumber
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
    where.push('po.DBOMNo LIKE @poNumber');
    params.poNumber = `%${poNumber}%`;
  }
  if (vendor) {
    where.push('(po.VendorCode LIKE @vendor OR ISNULL(v.VendorName,\'\') LIKE @vendor)');
    params.vendor = `%${vendor}%`;
  }
  if (project) {
    where.push('po.ProjectCode LIKE @project');
    params.project = `%${project}%`;
  }

  const result = await mssqlQuery(
    `
    SELECT TOP 1000
      po.ID AS id,
      po.DBOMNo AS poRef,
      po.ProjectCode AS projectCode,
      po.VendorCode AS vendorCode,
      v.VendorName AS vendorName,
      po.ItemCode AS itemCode,
      i.ItemDescription AS itemDescription,
      po.UOM AS uom,
      po.RequariedQty AS requiredQty,
      ISNULL(po.RemainingQty, po.RequariedQty) AS remainingQty,
      po.UnitPrice AS unitPrice,
      po.Amount AS amount,
      ISNULL(po.IGSTAmount,0) AS igstAmount,
      ISNULL(po.SGSTAmount,0) AS sgstAmount,
      ISNULL(po.CGSTAmount,0) AS cgstAmount,
      ISNULL(po.POApproved,0) AS poApproved,
      po.PMApproved AS pmApproved,
      po.MHApproved AS mhApproved,
      po.OMApproved AS omApproved,
      po.GMCostApproved AS gmApproved,
      po.POGeneratedBy AS poGeneratedBy,
      po.POGeneratedDate AS poGeneratedDate,
      po.POSenttoVendorBy AS poSentToVendorBy,
      po.PreparedBy AS preparedBy,
      po.POPreparedDate AS preparedDate
    FROM PurchaseOrder po
    LEFT JOIN Vendors v ON v.VendorCode = po.VendorCode
    LEFT JOIN ItemMaster i ON i.ItemCode = po.ItemCode
    WHERE ${where.join(' AND ')}
    ORDER BY po.ID DESC
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
    ORDER BY v.VendorName
    `
  );
  return result.recordset.map((row) => ({
    ...row,
    active: Boolean(row.active),
    approved: Boolean(row.approved),
  }));
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
        Units AS uom,
        CAST(NULL AS NVARCHAR(255)) AS make,
        DrawingNo AS drawingNo,
        HSNSACCode AS hsnCode,
        FixedCost AS standardCost,
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
    return result.recordset;
  } catch {
    const result = await mssqlQuery(
      `SELECT TOP 200 ItemCode AS itemCode, ItemDescription AS itemDescription FROM ItemMaster ORDER BY ItemCode`
    );
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
