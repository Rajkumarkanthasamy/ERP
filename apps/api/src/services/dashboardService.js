import { getDb } from '../db/connection.js';
import { PR_STATUS } from '../constants.js';

export function getDashboard() {
  const db = getDb();

  const pendingPR = db
    .prepare(
      `SELECT COUNT(*) AS count, IFNULL(SUM(total_amount),0) AS amount
       FROM purchase_requests WHERE status = ?`
    )
    .get(PR_STATUS.PENDING);

  const readyForPO = db
    .prepare(
      `SELECT COUNT(*) AS count, IFNULL(SUM(total_amount),0) AS amount
       FROM purchase_requests pr
       WHERE pr.status IN (?, ?)
         AND IFNULL(pr.is_clubbed,0) = 0
         AND EXISTS (
           SELECT 1 FROM purchase_request_details d
           WHERE d.pr_number = pr.pr_number AND IFNULL(d.line_status,'Pending') = 'Pending'
         )`
    )
    .get(PR_STATUS.APPROVED, PR_STATUS.PARTIALLY_CONVERTED);

  const posAwaiting = db
    .prepare(
      `SELECT COUNT(DISTINCT po_ref) AS count, IFNULL(SUM(amount),0) AS amount
       FROM purchase_orders WHERE IFNULL(po_approved,0) = 0`
    )
    .get();

  const latePOs = db
    .prepare(
      `SELECT COUNT(DISTINCT po_ref) AS count
       FROM purchase_orders
       WHERE IFNULL(po_approved,0) = 1
         AND po_sent_to_vendor_by IS NOT NULL
         AND IFNULL(remaining_qty,0) > 0
         AND julianday('now') - julianday(IFNULL(po_sent_vendor_date, prepared_date)) > 14`
    )
    .get();

  const itemCodesPending = db
    .prepare(`SELECT COUNT(*) AS count FROM item_code_requests WHERE approval_status = 'Pending'`)
    .get();

  const openIndents = db
    .prepare(`SELECT COUNT(*) AS count FROM indents WHERE status IN ('Open', 'Pending')`)
    .get();

  const openServiceCalls = db
    .prepare(`SELECT COUNT(*) AS count FROM service_calls WHERE status IN ('Open', 'Assigned', 'In Progress')`)
    .get();

  const pendingNcs = db
    .prepare(`SELECT COUNT(*) AS count FROM quality_ncs WHERE status IN ('Open', 'In Progress', 'Escalated')`)
    .get();

  const openEscalations = db
    .prepare(`SELECT COUNT(*) AS count FROM escalations WHERE status IN ('Open', 'In Progress')`)
    .get();

  const openComplaints = db
    .prepare(`SELECT COUNT(*) AS count FROM complaints WHERE status IN ('Open', 'Assigned', 'In Progress')`)
    .get();

  const openWorkOrders = db
    .prepare(`SELECT COUNT(*) AS count FROM work_orders WHERE status IN ('Open', 'Approved', 'In Progress')`)
    .get();

  const openEnquiries = db
    .prepare(`SELECT COUNT(*) AS count FROM sales_enquiries WHERE status = 'Open'`)
    .get();

  const pendingTimesheets = db
    .prepare(`SELECT COUNT(*) AS count FROM timesheets WHERE status = 'Submitted'`)
    .get();

  const openGateEntries = db
    .prepare(`SELECT COUNT(*) AS count FROM gate_entries WHERE status = 'Open'`)
    .get();

  const pendingProjects = db
    .prepare(`SELECT COUNT(*) AS count FROM projects WHERE IFNULL(approval_status,'Approved') = 'Pending'`)
    .get();

  const agingPRs = db
    .prepare(
      `SELECT pr_number AS prNumber, project_code AS projectCode, vendor_code AS vendorCode,
              total_amount AS totalAmount, status, requested_by AS requestedBy, request_date AS requestDate,
              CAST((julianday('now') - julianday(request_date)) AS INTEGER) AS ageDays
       FROM purchase_requests
       WHERE status IN (?, ?)
       ORDER BY request_date ASC
       LIMIT 20`
    )
    .all(PR_STATUS.PENDING, PR_STATUS.ON_HOLD);

  const recentActivity = db
    .prepare(
      `SELECT entity_type AS type, entity_ref AS refNo, action AS status, details,
              by_user AS byUser, created_at AS activityDate
       FROM activity_log
       ORDER BY id DESC
       LIMIT 20`
    )
    .all();

  return {
    metrics: [
      { metric: 'Pending PR Approvals', count: pendingPR.count, amount: pendingPR.amount },
      { metric: 'Approved PRs Ready for PO', count: readyForPO.count, amount: readyForPO.amount },
      { metric: 'POs Awaiting Approval', count: posAwaiting.count, amount: posAwaiting.amount },
      { metric: 'Late POs (>14 days)', count: latePOs.count, amount: 0 },
      { metric: 'Item Codes Pending', count: itemCodesPending.count, amount: 0 },
      { metric: 'Open Indents', count: openIndents.count, amount: 0 },
      { metric: 'Open Work Orders', count: openWorkOrders.count, amount: 0 },
      { metric: 'Open Service Calls', count: openServiceCalls.count, amount: 0 },
      { metric: 'Pending NCs', count: pendingNcs.count, amount: 0 },
      { metric: 'Open Escalations', count: openEscalations.count, amount: 0 },
      { metric: 'Open Complaints', count: openComplaints.count, amount: 0 },
      { metric: 'Open Enquiries', count: openEnquiries.count, amount: 0 },
      { metric: 'Timesheets Awaiting Approval', count: pendingTimesheets.count, amount: 0 },
      { metric: 'Open Gate Entries', count: openGateEntries.count, amount: 0 },
      { metric: 'Projects Pending Approval', count: pendingProjects.count, amount: 0 },
    ],
    agingPRs,
    recentActivity,
  };
}
