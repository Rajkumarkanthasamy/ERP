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
  'Rejected',
  'Closed',
  'Cancelled',
  'Deleted',
];

export function computePoAmounts(lines) {
  const baseAmount = lines.reduce((s, l) => s + Number(l.amount || 0), 0);
  const gstAmount = lines.reduce(
    (s, l) =>
      s +
      Number(l.igst_amount ?? l.igstAmount ?? 0) +
      Number(l.sgst_amount ?? l.sgstAmount ?? 0) +
      Number(l.cgst_amount ?? l.cgstAmount ?? 0),
    0
  );
  const totalAmount = baseAmount + gstAmount;
  let approvalTier = 'PC';
  if (totalAmount > PO_APPROVAL_TIERS.OM_MAX) approvalTier = 'GM';
  else if (totalAmount > PO_APPROVAL_TIERS.PC_MAX) approvalTier = 'OM';
  return { baseAmount, gstAmount, totalAmount, approvalTier };
}

export function getNextPoApprovalStep({
  pmApproved,
  mhApproved,
  pcApproved,
  omApproved,
  gmApproved,
  approvalTier,
}) {
  if (!pmApproved) return 'pm';
  if (!mhApproved) return 'mh';
  if (!pcApproved) return 'pc';
  if (['OM', 'GM'].includes(approvalTier) && !omApproved) return 'om';
  if (approvalTier === 'GM' && !gmApproved) return 'gm';
  return null;
}

export function computePoStatus(poGroup) {
  const lines = poGroup.lines || [];
  if (!lines.length) return 'Created';

  const first = lines[0];
  if (first.cancelled_by || first.cancelledBy) return 'Cancelled';
  if (first.closed_by || first.closedBy) return 'Closed';
  if (
    first.final_status === 'Rejected' ||
    first.finalStatus === 'Rejected' ||
    first.reject_reason ||
    first.rejectReason
  ) {
    return 'Rejected';
  }

  const ordered = lines.reduce(
    (s, l) => s + Number(l.required_qty ?? l.requiredQty ?? l.orderedQty ?? 0),
    0
  );
  const remaining = lines.reduce(
    (s, l) =>
      s +
      Number(
        l.remaining_qty ??
          l.remainingQty ??
          l.required_qty ??
          l.requiredQty ??
          l.orderedQty ??
          0
      ),
    0
  );
  const received = ordered - remaining;

  if (first.po_sent_to_vendor_by || first.poSentToVendorBy) {
    if (received <= 0) return 'Sent to Vendor';
    if (remaining <= 0) return 'Fully Received';
    return 'Partially Received';
  }

  if (first.po_generated_by || first.poGeneratedBy) return 'PO Generated';
  if (first.po_approved || first.poApproved) return 'Ready to Generate';

  if (!(first.pm_approved || first.pmApproved)) return 'Pending PM / Dept Approval';
  if (!(first.mh_approved || first.mhApproved)) return 'Pending MH Approval';
  if (!(first.pc_approved || first.pcApproved)) return 'Pending Purchase Committee';

  const { approvalTier } = computePoAmounts(lines);
  if (approvalTier === 'OM' || approvalTier === 'GM') {
    if (!(first.om_approved || first.omApproved)) return 'Pending OM Approval';
  }
  if (approvalTier === 'GM' && !(first.gm_approved || first.gmApproved)) {
    return 'Pending GM Approval';
  }

  return 'Created';
}
