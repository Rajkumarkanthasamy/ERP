# Phase 1 — Stabilize + Shell + Core Flow

## What shipped

1. **App shell**
   - `frmLogin` → authenticates against `Login` table (local fallback for offline/dev)
   - `frmMainMenu` → opens all procurement screens
   - `AppSession` / `AppConfig` / `PRStatus` shared across forms

2. **Schema alignment**
   - Run `WinFormsApp1/Phase1_Schema_Alignment.sql` on `ERP_Database`
   - Ensures `PurchaseRequestDetailNew`, `LineStatus`, `VendorName`, clubbing columns

3. **Critical DAL fixes**
   - `InsertPRHeader` returns real `SCOPE_IDENTITY()`
   - `InsertPRDetail` returns real identity + sets `LineStatus = Pending`
   - `TrackBOMConversion` uses real PRID (no longer hardcoded `1`)
   - `GetNextPRNumber` returns the *next* number (`PR-YYYY-MM-#######`)
   - Product filter on BOM load is applied
   - Parameterized clubbing queries

4. **Real PR Clubbing**
   - Creates a **new** Approved PR
   - Copies all source line items
   - Marks source PRs as `Clubbed`
   - Enforces same-vendor + 12L limit

5. **PR → PO flow connected**
   - Available from Main Menu and Approval screen
   - Partial conversion supported (`Partially Converted` / `Fully Converted`)
   - Old `frmPRtoPOConversion` excluded from build

6. **UX gap fixes**
   - Session user replaces hardcoded names
   - Approval bulk checkbox enabled
   - Hold → Release back to Pending
   - Select All refreshes amounts
   - BOM CSV export restored

## How to run

1. Run `Phase1_Schema_Alignment.sql` in SSMS
2. Update `AppConfig.ConnectionString` if needed
3. Open `WinFormsApp1.slnx` in Visual Studio
4. Build & run → Login → Main Menu

## Intended flow

```
Login → Main Menu
  → Item Code Creation / Approval
  → PR Generation (BOM → PR)
  → PR Approval
  → PR Clubbing (optional)
  → PR → PO Conversion
```

## Next (Phase 2)

- PO Approval UI (DAL already exists)
- 12L auto-split on PR generation
- Drag-and-drop PR → PO workspace
- Procurement dashboard / inbox
