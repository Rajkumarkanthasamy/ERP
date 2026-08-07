# PO Status View

Screen: **Main Menu → 7. PO Status View (all POs)**  
Form: `frmPOStatusView`  
DAL: `DataAccessLayer.fnPOStatusList`, `fnPOStatusLines`, `PORemarksHistory(1, …)`

## Purpose

View the live pipeline status of every Purchase Order in `ERP_Database.dbo.PurchaseOrder` (same table / DAL as the legacy `PurchaseOrder` form).

## Computed statuses

| Status | Meaning |
|--------|---------|
| Created | PO rows exist, early stage |
| Pending PM / Dept Approval | Waiting PM / department |
| Pending MH Approval | PM done, MH pending |
| Pending Purchase Committee | PM+MH done, PC pending |
| Pending OM Approval | PC done, OM pending |
| Pending GM Approval | OM done, high-value GM pending |
| Ready to Generate | Fully approved (`POApproved=1`), not yet generated |
| PO Generated | `POGeneratedBy` set |
| Sent to Vendor | Vendor mail / send flag set |
| Partially Received | Some qty inwarded |
| Fully Received | Remaining qty = 0 |
| Under Review | Sent back (`RejectReason`) |
| Closed / Cancelled / Deleted | Lifecycle end states (`FinalStatus` / Deleted flags) |

## Filters

- PO number, vendor, project (contains)
- Status dropdown
- Optional prepared-date range

## Detail pane

- Approval pipeline chips (Created → PM → MH → PC → OM → GM → Approved → Generated → Vendor → Receipt)
- Line items (`fnPOStatusLines`)
- Remarks timeline (`PORemarksHistory` case 1)

No schema change required if your `PurchaseOrder` table already has the approval / `FinalStatus` / GIN columns used by the legacy ERP.
