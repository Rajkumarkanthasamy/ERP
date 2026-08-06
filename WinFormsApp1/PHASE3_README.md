# Phase 3 — Kanban, Price Variance, Collaboration, GRN

## What shipped

1. **Kanban Pipeline Board** (`frmKanbanBoard`)
   - Columns: Pending → On Hold → Approved → Partially/Fully Converted → PO Pending → PO Approved
   - Drag PR cards between Pending / On Hold / Approved / Rejected-compatible moves
   - Double-click opens Comments / Attachments
   - Age highlighting (7d / 14d)

2. **Price Variance / Last-PO Compare** (`frmPriceVariance` + `PriceIntelligenceDAL`)
   - Compares PR unit cost vs latest PO / Receipt price
   - Flags Watch (≥5%) and ALERT (≥10%)
   - Printable variance report
   - Advisory prompt before Generate PO in drag-drop workspace

3. **Comments & Attachments** (`frmEntityCollaboration`)
   - Threaded comments for PR / PO / GRN
   - File attachments stored under Documents\ERP_Procurement_Attachments
   - Print pack (comments + attachment list)

4. **Goods Receipt (GRN)** (`frmGRN` + `GRNDAL`)
   - Load approved PO lines with remaining qty
   - Partial receive support
   - Writes `ProcurementGRN` / `ProcurementGRNDetail`
   - Reduces `PurchaseOrder.RemainingQty`
   - Best-effort insert into legacy `Receipt` table

5. **Schema** — run `Phase3_Schema_Alignment.sql`

6. **Main Menu / Dashboard** updated with Kanban, GRN, Price Variance shortcuts  
   PR Approval gains **Comments / Files** and **Price Variance** buttons

## Setup

1. Run `Phase1_Schema_Alignment.sql` (if not done)
2. Run `Phase3_Schema_Alignment.sql`
3. Build & run → Login → explore Phase 3 tiles

## Suggested walkthrough

1. Dashboard → Kanban (drag a Pending PR to Approved)
2. Double-click card → add comment + attach a quote PDF
3. Price Variance on that PR
4. Drag-Drop Workspace → Generate PO (variance advisory appears if flagged)
5. PO Approval → Final Approve
6. GRN → receive partial qty → Post GRN

## Next ideas (Phase 4)

- Invoice 3-way match
- Vendor portal acknowledgement
- Full PDF templates with company letterhead
- Real-time notification inbox
