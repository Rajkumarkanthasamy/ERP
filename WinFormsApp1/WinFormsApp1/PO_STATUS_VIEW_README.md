# PO Status View

Screen: **Main Menu → 7. PO Status View (all POs)**  
Form: `frmPOStatusView`  
DAL: `DataAccessLayer.fnPOStatusList`, `fnPOStatusLines`, `fnGetPOAmountWithGST`

## PO amount for approval (includes GST)

```
TotalAmount = SUM(Amount + IGSTAmount + SGSTAmount + CGSTAmount)
```

## Approval thresholds (Amount + GST)

| Total amount | After PM + MH + PC | Who finalizes |
|--------------|--------------------|---------------|
| **≤ ₹50,000** | PC can fully approve (`case 9`) | Purchase Committee |
| **> ₹50,000 and ≤ ₹2,50,000** | Needs **Vivek G / OM** (`case 19`) | OM finalizes, GM skipped |
| **> ₹2,50,000** | Needs OM then **GM** (`case 17` → GM) | GM finalizes |

Helpers:
- `fnGetPOAmountWithGST(po)` → TotalAmount, BaseAmount, GSTAmount, ApprovalTier, PCApprovalCase, OMApprovalCase
- `fnGetPCApprovalCase(po)` → `9` or `10`
- `fnGetOMApprovalCase(po)` → `19` or `17`

## Computed statuses

| Status | Meaning |
|--------|---------|
| Created | PO rows exist, early stage |
| Pending PM / Dept Approval | Waiting PM / department |
| Pending MH Approval | PM done, MH pending |
| Pending Purchase Committee | PM+MH done, PC pending |
| Pending OM Approval | Amount > 50k, PC done, Vivek G/OM pending |
| Pending GM Approval | Amount > 2.5L, OM done, GM pending |
| Ready to Generate | Fully approved (`POApproved=1`), not yet generated |
| PO Generated | `POGeneratedBy` set |
| Sent to Vendor | Vendor mail / send flag set |
| Partially / Fully Received | Receipt progress |
| Under Review / Closed / Cancelled / Deleted | Special / end states |
