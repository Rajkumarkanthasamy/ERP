export const PR_STATUS = {
  PENDING: 'Pending',
  APPROVED: 'Approved',
  REJECTED: 'Rejected',
  ON_HOLD: 'On Hold',
  CLUBBED: 'Clubbed',
  PARTIALLY_CONVERTED: 'Partially Converted',
  FULLY_CONVERTED: 'Fully Converted',
};

export const PR_LINE_STATUS = {
  PENDING: 'Pending',
  CONVERTED_TO_PO: 'Converted to PO',
  CANCELLED: 'Cancelled',
};

export const PR_LIMIT = 1_200_000; // ₹12L clubbing / auto-split limit

export const PO_APPROVAL_TIERS = {
  PC_MAX: 50_000,
  OM_MAX: 250_000,
};

export const VARIANCE = {
  WATCH: 5,
  ALERT: 10,
};

export const PO_STATUS_OPTIONS = [
  'All',
  'Created',
  'Pending PM / Dept Approval',
  'Pending MH Approval',
  'Pending Purchase Committee',
  'Pending OM Approval',
  'Pending GM Approval',
  'Ready to Generate',
  'PO Generated',
  'Sent to Vendor',
  'Partially Received',
  'Fully Received',
  'Under Review',
  'Closed',
  'Cancelled',
  'Deleted',
];

export function computePoAmounts(lines) {
  const baseAmount = lines.reduce((s, l) => s + Number(l.amount || 0), 0);
  const gstAmount = lines.reduce(
    (s, l) => s + Number(l.igst_amount || 0) + Number(l.sgst_amount || 0) + Number(l.cgst_amount || 0),
    0
  );
  const totalAmount = baseAmount + gstAmount;
  let approvalTier = 'PC';
  if (totalAmount > PO_APPROVAL_TIERS.OM_MAX) approvalTier = 'GM';
  else if (totalAmount > PO_APPROVAL_TIERS.PC_MAX) approvalTier = 'OM';
  return { baseAmount, gstAmount, totalAmount, approvalTier };
}

export function computePoStatus(poGroup) {
  const lines = poGroup.lines || [];
  if (!lines.length) return 'Created';

  const first = lines[0];
  if (first.cancelled_by) return 'Cancelled';
  if (first.closed_by) return 'Closed';

  const ordered = lines.reduce((s, l) => s + Number(l.required_qty || 0), 0);
  const remaining = lines.reduce((s, l) => s + Number(l.remaining_qty ?? l.required_qty ?? 0), 0);
  const received = ordered - remaining;

  if (first.po_sent_to_vendor_by) {
    if (received <= 0) return 'Sent to Vendor';
    if (remaining <= 0) return 'Fully Received';
    return 'Partially Received';
  }

  if (first.po_generated_by) return 'PO Generated';
  if (first.po_approved) return 'Ready to Generate';

  if (!first.pm_approved) return 'Pending PM / Dept Approval';
  if (!first.mh_approved) return 'Pending MH Approval';
  if (!first.pc_approved) return 'Pending Purchase Committee';

  const { approvalTier } = computePoAmounts(lines);
  if (approvalTier === 'OM' || approvalTier === 'GM') {
    if (!first.om_approved) return 'Pending OM Approval';
  }
  if (approvalTier === 'GM' && !first.gm_approved) return 'Pending GM Approval';

  return 'Created';
}
