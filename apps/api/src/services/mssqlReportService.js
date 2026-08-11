import { mssqlQuery } from '../db/mssql.js';

/**
 * Catalog of the 57 legacy ERP report choices.
 * `queryKey` reports have live SQL adapters; others return not_mapped until ported.
 */
export const REPORT_CATALOG = [
  { id: 'project-issue', name: 'Project Issue Report', queryKey: 'projectIssue', category: 'Stores' },
  { id: 'project-cost', name: 'Project Cost Report', queryKey: null, category: 'Projects' },
  { id: 'project-consumption', name: 'Project Consumption Report', queryKey: null, category: 'Stores' },
  { id: 'project-master', name: 'Project Master Report', queryKey: 'projectMaster', category: 'Projects' },
  { id: 'receipt-gin', name: 'Receipt / GIN Report', queryKey: 'receiptGin', category: 'Stores' },
  { id: 'reverse-gin', name: 'Reverse GIN Report', queryKey: null, category: 'Stores' },
  { id: 'vendor-gin', name: 'Vendor GIN Report', queryKey: null, category: 'Stores' },
  { id: 'service-gin', name: 'Service GIN Report', queryKey: null, category: 'Stores' },
  { id: 'stock-balance', name: 'Stock Balance Report', queryKey: 'stockBalance', category: 'Stores' },
  { id: 'filtered-stock', name: 'Filtered Stock Report', queryKey: null, category: 'Stores' },
  { id: 'consolidated-stock', name: 'Consolidated Stock Report', queryKey: null, category: 'Stores' },
  { id: 'grading', name: 'Grading Report', queryKey: null, category: 'Stores' },
  { id: 'item-label', name: 'Item Label Printing', queryKey: null, category: 'Stores' },
  { id: 'job-issue', name: 'Job Issue Report', queryKey: null, category: 'Stores' },
  { id: 'job-work', name: 'Job Work Report', queryKey: null, category: 'Procurement' },
  { id: 'item-usage', name: 'Item Usage Report', queryKey: null, category: 'Stores' },
  { id: 'po-register', name: 'PO Purchase Register', queryKey: 'poRegister', category: 'Procurement' },
  { id: 'wo-register', name: 'WO Purchase Register', queryKey: null, category: 'Procurement' },
  { id: 'pending-po', name: 'Pending PO Report', queryKey: 'pendingPo', category: 'Procurement' },
  { id: 'gm-pending-po', name: 'GM Pending PO Report', queryKey: 'gmPendingPo', category: 'Procurement' },
  { id: 'project-po', name: 'Project PO Report', queryKey: null, category: 'Procurement' },
  { id: 'number-of-pos', name: 'Number of POs Report', queryKey: null, category: 'Procurement' },
  { id: 'purchase-cost', name: 'Purchase Cost Report', queryKey: null, category: 'Procurement' },
  { id: 'ap-spend', name: 'Transactional / AP Spend', queryKey: null, category: 'Finance' },
  { id: 'dept-issue', name: 'Department-wise Issue', queryKey: null, category: 'Stores' },
  { id: 'dept-po', name: 'Department-wise PO', queryKey: null, category: 'Procurement' },
  { id: 'dept-wo', name: 'Department-wise WO', queryKey: null, category: 'Procurement' },
  { id: 'standard-cost', name: 'Standard Cost Report', queryKey: 'standardCost', category: 'Masters' },
  { id: 'fixed-asset', name: 'Fixed Asset Report', queryKey: null, category: 'Masters' },
  { id: 'project-documents', name: 'Project Documents Report', queryKey: null, category: 'Projects' },
  { id: 'user-access', name: 'User Access Report', queryKey: 'userAccess', category: 'Admin' },
  { id: 'erp-log', name: 'ERP Transaction Log', queryKey: 'erpLog', category: 'Admin' },
  { id: 'project-transfer', name: 'Project Transfer Report', queryKey: null, category: 'Projects' },
  { id: 'project-status', name: 'Project Status Report', queryKey: null, category: 'Projects' },
  { id: 'project-installation', name: 'Project Installation Tracking', queryKey: null, category: 'Projects' },
  { id: 'machine-cost', name: 'Machine Project Cost Details', queryKey: null, category: 'Projects' },
  { id: 'manhour-cost', name: 'Man-hour Project Cost Details', queryKey: null, category: 'Projects' },
  { id: 'pr-status', name: 'PR Status Report', queryKey: 'prStatus', category: 'Procurement' },
  { id: 'vendor-master', name: 'Vendor Master Report', queryKey: 'vendorMaster', category: 'Masters' },
  { id: 'item-master', name: 'Item Master Report', queryKey: 'itemMaster', category: 'Masters' },
  { id: 'customer-master', name: 'Customer Master Report', queryKey: 'customerMaster', category: 'Masters' },
  { id: 'gate-inward', name: 'Gate Inward Report', queryKey: null, category: 'Gate' },
  { id: 'gate-outward', name: 'Gate Outward Report', queryKey: null, category: 'Gate' },
  { id: 'price-variance', name: 'Price Variance Report', queryKey: null, category: 'Procurement' },
  { id: 'target-cost', name: 'Target Cost History', queryKey: 'targetCost', category: 'Masters' },
  { id: 'po-terms', name: 'PO Terms / Tax Summary', queryKey: null, category: 'Procurement' },
  { id: 'open-grn', name: 'Open GRN Lines', queryKey: null, category: 'Procurement' },
  { id: 'inventory-aging', name: 'Inventory Aging', queryKey: null, category: 'Stores' },
  { id: 'slow-moving', name: 'Slow Moving Stock', queryKey: null, category: 'Stores' },
  { id: 'negative-stock', name: 'Negative Stock', queryKey: null, category: 'Stores' },
  { id: 'po-aging', name: 'PO Aging Buckets', queryKey: null, category: 'Procurement' },
  { id: 'approval-pending', name: 'Pending Approvals Summary', queryKey: null, category: 'Procurement' },
  { id: 'currency-rates', name: 'Currency / PEG Rates', queryKey: null, category: 'Masters' },
  { id: 'payment-terms', name: 'Payment Terms Master', queryKey: null, category: 'Masters' },
  { id: 'delivery-terms', name: 'Delivery Terms Master', queryKey: null, category: 'Masters' },
  { id: 'tax-masters', name: 'Tax Masters Summary', queryKey: null, category: 'Masters' },
  { id: 'collab-audit', name: 'Procurement Comments / Attachments', queryKey: null, category: 'Procurement' },
];

export function listReportCatalog() {
  return REPORT_CATALOG.map((r) => ({
    ...r,
    status: r.queryKey ? 'live' : 'not_mapped',
  }));
}

function findReport(id) {
  return REPORT_CATALOG.find((r) => r.id === id);
}

async function runQuery(key, filters = {}) {
  const q = filters.q ? `%${filters.q}%` : null;
  switch (key) {
    case 'stockBalance':
      return (
        await mssqlQuery(
          `
          SELECT TOP 2000 ItemCode AS itemCode, ItemDescription AS itemDescription,
                 Units AS uom, ISNULL(AvailableQty,0) AS availableQty,
                 ISNULL(UnitCost,0) AS unitCost, ISNULL(StockValue,0) AS stockValue,
                 ISNULL(Location,'MAIN') AS location
          FROM ItemMaster
          ${q ? 'WHERE ItemCode LIKE @q OR ItemDescription LIKE @q' : ''}
          ORDER BY ItemCode
          `,
          q ? { q } : {}
        )
      ).recordset;
    case 'projectMaster':
      return (
        await mssqlQuery(
          `
          SELECT TOP 1000 ProjectCode AS projectCode, ProjectDescription AS projectName,
                 Customer AS customer, CustomerCode AS customerCode, Status AS status,
                 ProjectInstallStatus AS installationStatus, ApprovedBy AS approvedBy
          FROM ProjectMaster
          ${q ? 'WHERE ProjectCode LIKE @q OR ProjectDescription LIKE @q' : ''}
          ORDER BY Id DESC
          `,
          q ? { q } : {}
        )
      ).recordset;
    case 'receiptGin':
      return (
        await mssqlQuery(
          `
          SELECT TOP 1000 GINNumber AS ginNumber, ItemCode AS itemCode, Quantity AS quantity,
                 UnitCost AS unitCost, SupplierName AS supplierName, Date AS ginDate,
                 VendorCode AS vendorCode, BOMProjectCode AS projectCode
          FROM Receipt
          ${q ? 'WHERE GINNumber LIKE @q OR ItemCode LIKE @q OR VendorCode LIKE @q' : ''}
          ORDER BY ID DESC
          `,
          q ? { q } : {}
        )
      ).recordset;
    case 'pendingPo':
      return (
        await mssqlQuery(
          `
          SELECT TOP 1000 DBOMNo AS poRef, ItemCode AS itemCode, VendorCode AS vendorCode,
                 RequariedQty AS qty, UnitPrice AS unitPrice, Amount AS amount,
                 ProjectCode AS projectCode, ISNULL(POApproved,0) AS poApproved,
                 ISNULL(POGenerated,0) AS poGenerated, ISNULL(POSent,0) AS poSent
          FROM PurchaseOrder
          WHERE ISNULL(Cancelled,0) = 0 AND ISNULL(Closed,0) = 0
            AND (ISNULL(POGenerated,0) = 0 OR ISNULL(POSent,0) = 0)
          ORDER BY ID DESC
          `
        )
      ).recordset;
    case 'gmPendingPo':
      return (
        await mssqlQuery(
          `
          SELECT TOP 500 DBOMNo AS poRef, ItemCode AS itemCode, VendorCode AS vendorCode,
                 Amount AS amount, ProjectCode AS projectCode
          FROM PurchaseOrder
          WHERE ISNULL(POApproved,0) = 0 AND ISNULL(Cancelled,0) = 0
          ORDER BY ID DESC
          `
        )
      ).recordset;
    case 'poRegister':
      return (
        await mssqlQuery(
          `
          SELECT TOP 1000 DBOMNo AS poRef, ItemCode AS itemCode, VendorCode AS vendorCode,
                 RequariedQty AS qty, UnitPrice AS unitPrice, Amount AS amount,
                 ProjectCode AS projectCode, PODate AS poDate
          FROM PurchaseOrder
          ${q ? 'WHERE DBOMNo LIKE @q OR VendorCode LIKE @q OR ItemCode LIKE @q' : ''}
          ORDER BY ID DESC
          `,
          q ? { q } : {}
        )
      ).recordset;
    case 'prStatus':
      return (
        await mssqlQuery(
          `
          SELECT TOP 1000 PRNumber AS prNumber, ProjectCode AS projectCode, VendorCode AS vendorCode,
                 TotalAmount AS totalAmount, Status AS status, RequestedBy AS requestedBy,
                 RequestDate AS requestDate
          FROM PurchaseRequest
          ${q ? 'WHERE PRNumber LIKE @q OR VendorCode LIKE @q' : ''}
          ORDER BY RequestDate DESC
          `,
          q ? { q } : {}
        )
      ).recordset;
    case 'standardCost':
      return (
        await mssqlQuery(
          `
          SELECT TOP 1000 ItemCode AS itemCode, ItemDescription AS itemDescription,
                 ISNULL(UnitCost,0) AS unitCost, ISNULL(FixedCost,0) AS fixedCost,
                 ISNULL(TargetCost,0) AS targetCost
          FROM ItemMaster
          ${q ? 'WHERE ItemCode LIKE @q OR ItemDescription LIKE @q' : ''}
          ORDER BY ItemCode
          `,
          q ? { q } : {}
        )
      ).recordset;
    case 'targetCost':
      return (
        await mssqlQuery(
          `
          SELECT TOP 500 h.Id AS id, h.ItemCode AS itemCode, h.TargetCost AS targetCost,
                 h.UpdateBy AS updatedBy, h.UpdateDate AS updatedAt, h.GMApprove AS gmApproved
          FROM ItemTargetCostHistory h
          ORDER BY h.Id DESC
          `
        )
      ).recordset;
    case 'vendorMaster':
      return (
        await mssqlQuery(
          `
          SELECT TOP 1000 VendorCode AS vendorCode, VendorName AS vendorName,
                 City AS city, State AS state, GSTIN AS gstin
          FROM Vendors
          ${q ? 'WHERE VendorCode LIKE @q OR VendorName LIKE @q' : ''}
          ORDER BY VendorCode
          `,
          q ? { q } : {}
        )
      ).recordset;
    case 'itemMaster':
      return (
        await mssqlQuery(
          `
          SELECT TOP 1000 ItemCode AS itemCode, ItemDescription AS itemDescription,
                 Units AS uom, ISNULL(AvailableQty,0) AS availableQty, ISNULL(UnitCost,0) AS unitCost
          FROM ItemMaster
          ${q ? 'WHERE ItemCode LIKE @q OR ItemDescription LIKE @q' : ''}
          ORDER BY ItemCode
          `,
          q ? { q } : {}
        )
      ).recordset;
    case 'customerMaster':
      return (
        await mssqlQuery(
          `
          SELECT TOP 1000 CustomerCode AS customerCode, CustomerName AS customerName,
                 City AS city, State AS state, GSTIN AS gstin
          FROM CustomerMaster
          ${q ? 'WHERE CustomerCode LIKE @q OR CustomerName LIKE @q' : ''}
          ORDER BY CustomerCode
          `,
          q ? { q } : {}
        )
      ).recordset;
    case 'userAccess':
      return (
        await mssqlQuery(
          `
          SELECT TOP 500 Username AS username, Name AS displayName,
                 ISNULL(Active,1) AS active, Department AS department
          FROM Login
          ORDER BY Username
          `
        )
      ).recordset;
    case 'erpLog':
      return (
        await mssqlQuery(
          `
          SELECT TOP 500 TransactionNumber AS txnNumber, TransactionBy AS createdBy,
                 TransactionDate AS txnDate, Action AS action, TableName AS tableName,
                 TransactionDateTime AS createdAt
          FROM ERPTransactionLog
          ORDER BY TransactionDateTime DESC
          `
        )
      ).recordset;
    case 'projectIssue':
      return (
        await mssqlQuery(
          `
          SELECT TOP 500 TransactionNumber AS txnNumber, TransactionBy AS createdBy,
                 TransactionDate AS txnDate, Action AS action
          FROM ERPTransactionLog
          WHERE Action IN ('Item Issue', 'Item Return')
          ORDER BY TransactionDateTime DESC
          `
        )
      ).recordset;
    default:
      return null;
  }
}

export async function runReport(reportId, filters = {}) {
  const report = findReport(reportId);
  if (!report) throw new Error(`Unknown report: ${reportId}`);
  if (!report.queryKey) {
    const err = new Error(`Report "${report.name}" is not mapped to live SQL yet`);
    err.code = 'MSSQL_WORKFLOW_NOT_MAPPED';
    throw err;
  }
  const rows = await runQuery(report.queryKey, filters);
  return {
    report: { id: report.id, name: report.name, category: report.category, status: 'live' },
    filters,
    rowCount: rows.length,
    rows,
  };
}
