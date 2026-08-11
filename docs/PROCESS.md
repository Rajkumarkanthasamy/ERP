# ExistERP process in the React + Node app

The web app follows the **same procurement process** as the C# ExistERP / WinForms flow, against your SQL Server `ERP_Database` when `DB_CLIENT=mssql`.

## End-to-end flow

```
Login (Login table)
  → Dashboard
  → PR Generation (from items / BOM selection in UI)
  → PR Approval (Approve / Reject / Hold / Release)  [also Kanban drag]
  → PR Clubbing (same vendor, ≤ ₹12L)
  → PR → PO Workspace (convert pending lines → PurchaseOrder)
  → PO Approval (PM → MH → PC → OM/GM by amount+GST)
  → Generate PO / Send to vendor
  → PO Status View
  → GRN (partial receive, reduces RemainingQty)
```

## Amount rules (same as C# / PO Status View)

| Total (Amount + GST) | Final approver |
|----------------------|----------------|
| ≤ ₹50,000 | Purchase Committee (PC) |
| > ₹50,000 and ≤ ₹2,50,000 | OM |
| > ₹2,50,000 | GM |

PR clubbing / auto-split limit: **₹12,00,000** per vendor pack.

## SQL Server tables used

| Process | Tables |
|---------|--------|
| Login | `Login` |
| PR | `PurchaseRequest`, `PurchaseRequestDetailNew` |
| Clubbing | same + marks source PRs `Clubbed` |
| PO | `PurchaseOrder` |
| GRN | `ProcurementGRN`, `ProcurementGRNDetail`, `JobMovement`, `Receipt`, `ERPInventoryLogs`, `ItemMaster`, `ERPTransactionLog` |
| Masters | `CityMaster`/`StateMaster`, `CustomerMaster`, `Vendors`, `ItemMaster`, `ItemStdCostHistory` |
| Gate | `SecurityInward`, `SecurityOutward`, `ERPTransactionLog` |

If GRN tables are missing, run in SSMS:

`apps/api/sql/Phase3_Schema_Alignment.sql`

## How to run the full process on your PC

1. SSMS can connect to `GTKA064W111\SQLEXPRESS01` / `ERP_Database`
2. Create `apps/api/.env` with `DB_CLIENT=mssql` and your SA credentials (see `docs/DATABASE.md`)
3. Optional: run Phase3 SQL script for GRN tables
4. `npm install && npm run dev`
5. Login with an existing ERP user from the `Login` table
6. Walk the menu: Procurement Dashboard → PR → Approval → Workspace → PO Approval → Status → GRN

## Other ERP modules (Masters, Stores, Projects, Sales, Quality, …)

The React app includes screens for the **full ExistERP menu**.  
With `DB_CLIENT=mssql`, **procurement is fully live** (read + write) on your database.  
Other modules continue to expand onto the matching legacy tables; until each is mapped, use the C# app for those specific write operations or run SQLite demo mode for UI walkthroughs.
