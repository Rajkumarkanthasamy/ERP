# ExistERP legacy-to-web feature parity

This document is the migration ledger for the web application. It compares:

- `Code.zip`: the main ExistERP WinForms application (`BiSS ERP v7.5`)
- `WinFormsApp1.zip`: the newer PR / item-code / PR-to-PO prototype
- `apps/web`: React application
- `apps/api`: Node API

## Executive status

The web application is **not yet at full ExistERP parity**.

The legacy repository contains:

- 148 WinForms designer screens
- 160 SQL Server tables in `schema_Database.sql`
- 57 distinct report choices in the ERP report screen
- approximately 70 per-user module/role flags in the `Login` table

The web application currently contains 50 routes and 17 API route groups. Several
routes combine multiple legacy screens, but most modules outside procurement are
simplified SQLite demo CRUD screens.

In SQL Server mode, authentication, dashboard, PR, PO, GRN (including legacy
GIN stock posting), gate inward/outward, city/customer/vendor/item master
writes, project create/update/approve, target-cost history, additional masters
(tax/payment/delivery/currency), Phase3 procurement comments/attachments,
stock ledger + item issue/return FIFO, and a mapped subset of ERP reports
currently have explicit MSSQL code paths. Other route groups return
`501 MSSQL_WORKFLOW_NOT_MAPPED`; they cannot silently read or write the SQLite
demo database while live ERP mode is selected.

### Status labels

| Label | Meaning |
|---|---|
| Live | Reads and writes the matching legacy SQL Server tables |
| Partial | Some workflow/data is implemented, but important legacy behavior is absent |
| Demo | Functional only against the simplified SQLite schema |
| Missing | No equivalent web workflow |
| Legacy stub | Source form exists but is empty, commented, duplicated, or not reachable from the main menu |

## Parity matrix

| Legacy area | Legacy capability | Current web status | Main gaps |
|---|---|---|---|
| Login | AES-encrypted password, active-account check, password expiry, ERP version check, remembered login, delegated authorized users, login tracking, forgot-password email | Partial | AES login/change-password and expiry flag are implemented; version enforcement, delegated authorization and reset flow remain |
| Authorization | Per-user flags for projects, masters, stores, procurement, sales, quality, reports, Kanban, timesheet, assets, and special approver roles | Partial | All legacy flags are now carried in the web token and navigation/direct routes are filtered; API-level permission coverage still needs expansion |
| Home dashboard | Pending approvals, PO aging buckets, project/WIP/shipment/install counts, actionable approval grid | Partial | Most legacy dashboard metrics and role-specific actions are absent |
| City master | Create/update city and state mapping | Partial | Live `CityMaster` create/update with `StateMaster` join; state add UI and full approval workflows remain |
| Customer master | Full address/tax/contact fields and approval | Partial | Live create/update on `CustomerMaster` for core address/contact/GST fields; approval workflow and extended tax fields remain |
| Vendor master | Full address, tax, banking, MSME, currency, approval and search | Partial | Live create/update on `Vendors` for core address/contact/GST; banking, MSME, currency and `VendorDetails` remain |
| Item master | Item create/update, stock fields, UOM, storage, drawing, HSN/SAC, costs and history | Partial | Live create/update on `ItemMaster`; `standardCost` maps to `UnitCost` with `ItemStdCostHistory`; separate `fixedCost` field; full storage/approval fields remain |
| Item-code request/approval | Propose item code, hold/reject/approve, then create the approved `ItemMaster` row | Demo | SQLite workflow exists; live `ItemCodeCreation` mapping and the legacy creator/approval-authority rules remain |
| Standard/target cost | Cost update with audit histories and restricted access | Partial | Live standard-cost updates write `UnitCost` + `ItemStdCostHistory`; live target-cost propose/GM-approve writes `ItemTargetCostHistory` and `ItemMaster.TargetCost` |
| Product master/BOM | Product tree and BOM maintenance | Demo | Live product/BOM tree, versioning and approvals |
| Fixed assets | Asset register, cost centre, barcode/verification and documents | Demo | Full `AssetMaster`, `AssetCostCentre`, `AssetDocumentInfo`, upload/verification |
| Procurement PR | PR creation, approval/reject/hold/release, clubbing and PR-to-PO prototype | Partial | Core web request/action contracts are corrected; collab comments on PR approval; live SQL validation, exact role rules, BOM-origin traceability and complete validation remain |
| Purchase order | PO/WO creation, pricing/tax/terms, approval chain, finalization, update request, cancel/close, repeat order, vendor confirmation, tracking, reminders | Partial | Cancel/close, track, GST/terms-on-convert from `VendorDetails`, `DiffinPer`/`DiffinRs`, `POReport` upsert, and collab comments on PO status are live; update-request, vendor confirm queue, reminders and RepeatOrder remain |
| Work order | Job-work order generation, approval, costs, issue/return and tracking | Demo | Live `WorkOrder`, `WOOtherCost`, `WOApproveList`, job issue/movement |
| Currency/terms | Currency rate, GST/tax, payment, delivery, packing/forwarding and general terms | Partial | Live Additional Masters for tax/CST/others-tax/discount/excise/payment/delivery plus PEG/currency rates and inco terms; packing/forwarding UI remains |
| GIN/receipt | PO/WO receipt, other-item GIN, inspection request, document scanning, tax, service GIN | Partial | Live PO GRN posts `ProcurementGRN` plus `JobMovement`/`Receipt`/`ERPInventoryLogs`/`ItemMaster` qty in one transaction; WO/other-item GIN, inspection, WAR and attachment/scanning remain |
| Reverse GIN | Reverse receipt and inventory effects | Missing | `RiverseGIN`/`ERPReverseInventoryLogs` transaction |
| Item issue/return | Project/job issue, return, stock validation and FIFO logs | Partial | Live FIFO issue/return against `ERPInventoryLogs` + `ItemMaster.AvailableQty` with `ERPTransactionLog`; full project/job UI rules and cycle count remain |
| Inventory | Ledgers, cycle count, location update/history, stock adjustment and grading; the legacy dashboard entry is a dead stub | Partial | Live stock ledger from `ItemMaster`; adjust/production/cycle-count/location still demo or unmapped |
| Delivery challan/DC | DC generation and related project/receipt data | Demo | Legacy document numbering, line rules, printing/export and SQL mapping |
| Gate entry | Inward, outward, manual inward and report | Partial | Live multi-line `SecurityInward`/`SecurityOutward` reads and transactional writes are mapped; external BMS/DC lookup, historical manual-entry identification and report export remain |
| Project create/approve | Full project/customer/product/order metadata and approvals | Partial | Live `ProjectMaster` list/create/update/approve; full field set and product metadata remain |
| Project update | Warranty, short shipment, status, installation, shipment date and invoice updates | Partial | Only simplified project editing/installation view exists |
| Project BOM | Create/import, versions, lock/unlock, change logs, approval and progress | Demo | `ProjectBOM`, `MachineBOM`, lock/log tables and full workflows |
| Project transfer | Transfer request, approval and inventory/project transfer | Missing | `ProjectTransferInfo` and stock transfer transaction |
| Project documents | Upload, browse and tracking | Demo | Network/document storage, metadata, download and permissions |
| Project QR/barcode | Generate and print project QR/barcodes | Missing | QR/PDF/label generation |
| Project close | Warranty expiry close, project closer, revenue recognition and email | Missing | Closure workflow, validation, approval and notices |
| Machine passport | Create/update, dynamic grid, approvals and status view | Missing | `MachinePassport`, headers and `MpApproveList` |
| Packing list | Project packing-list header/details and print/export | Missing | Full packing-list workflow |
| Order entry | Order/billing updates and manager workflow | Missing | Project order-entry tables and approval |
| Capex planning | Project status planning and department/user access | Missing | Planning/history/users/department permissions |
| Sales enquiry | Enquiry register and entry | Demo | Full `EnquiryRegisterDetails` mapping and legacy fields |
| Customer visit | Customer visit capture/view | Missing | `CustomerVisitEntry` workflow |
| Opportunities | Opportunity details/codes | Demo | Live quote opportunity tables and complete workflow |
| Sales quote | Product tree/BOM/options/tax, revisions, approval/view and output | Demo | Complete quotation engine, revisions, print/PDF/email |
| Service quote | Contract values, PI details/items and multiple quote variants | Missing | Service quote and pro-forma invoice workflows |
| Test-lab quote | Test quote and item detail workflow | Missing | Test-lab quote generation/view |
| Spare-parts quote | Spare-parts quotation | Missing | `SparePartQuationDetails` workflow |
| PEG rate | PEG/currency rate maintenance | Partial | Live list/create via Additional Masters currency-rates (`POCurrencyRate`); full PEG UI fields remain |
| Service management | Service order entry and related variants; the main-menu service-call manager is dead/stubbed | Demo | Live service-order mappings and complete implemented variants |
| Quality NC | Raise/view NC, user detail and close/corrective action | Demo | Live `NCForm`, legacy fields, authorization and state transitions |
| Escalations | Escalation levels, reminders, reviews and views | Demo | Live escalation tables, reminders and review meetings |
| SRF calibration | Calibration request/workflow | Missing | Complete SRF screen and data mapping |
| Complaints/support | Raise, route/authorize/resolve, feedback/rating and history | Demo | Live `ComplaintForm`, role actions, feedback/history |
| Kanban electronics | Item master, production, issue, return, stock adjust, BOM, ledger and reports | Missing | Complete electronics Kanban tables/workflows |
| Kanban transducers | Production, issue, return, adjust, BOM, ledger and reports | Missing | Complete transducer Kanban tables/workflows |
| Timesheet | Validation/department timesheet entry, user view, approval and reports | Demo | Live mappings, activity masters, validation/approval and report |
| Machine utilization | Machine master/utilization entry and view | Missing | `TestLabMachinesMaster`/`TestLabMachinesUtilization` |
| User management | Create/update users and all permission flags | Partial | Current list is SQLite-oriented; live CRUD and complete permissions absent |
| Change password | Verify current password and update encrypted password | Partial | Exact policy and AES-compatible update are implemented; live SQL validation is pending network access |
| ERP reports | 57 report types with date/project/vendor/item filters | Partial | Catalog of all 57 choices is exposed; a mapped subset runs live SQL queries; remaining reports return 501 until ported |
| Export/print | Excel/CSV, PDF, QR/barcode, GIN/PO/packing labels and report printing | Missing | Server/browser-safe exporters and templates |
| Email/notifications | Outlook-based quote, PO, credential, closure and reminder mail | Missing | Configurable SMTP/provider integration, templates and audit |
| Documents | File scanning, project/asset attachments and network paths | Partial | Phase3 `ProcurementComment`/`ProcurementAttachment` live for PR/PO; broader storage/download/versioning is absent |
| Audit | ERP transaction log, inventory logs, BOM lock/change logs, cost histories | Partial | Web activity log covers only selected SQLite operations |

## Legacy report inventory

The main report form exposes 57 distinct report choices. They include:

- project issue, project cost, project consumption and project master reports
- receipt/GIN, reverse GIN, vendor GIN and service GIN reports
- stock balance, filtered/consolidated stock, grading and item label printing
- job issue/job work/item usage and PO/WO purchase registers
- pending/GM-pending PO, project PO, number of POs and purchase cost
- transactional/AP spend and department-wise issue/PO/WO
- standard cost, fixed asset, project documents, user access and ERP log reports
- project transfer/status/installation tracking
- machine/man-hour project cost details

The web `ReportsPage` now lists all 57 legacy report choices and runs the
mapped live SQL adapters. Unmapped reports return `501 MSSQL_WORKFLOW_NOT_MAPPED`.
Excel/PDF/label export remains missing.

## Critical correctness findings and current corrections

1. Legacy passwords are encrypted with AES using the algorithm in
   `Code/Cryptography.cs`. AES-compatible login and change-password handling are
   now implemented and covered by a compatibility test.
2. The legacy `Login` table contains approximately 70 module/role flags. Only a
   small procurement subset was previously included; the complete flag map is
   now carried in the token and used by web navigation/direct-route guards.
3. Current MSSQL vendor reads query `VendorMaster`, while the supplied schema
   defines `Vendors`. The read has been corrected to `Vendors`.
4. Current MSSQL item aliases reference columns such as `UOM`, `HSNCode`, and
   `StandardCost`; these aliases have been corrected to the supplied
   `ItemMaster` schema.
5. Current MSSQL PO reads select item/vendor description columns directly from
   `PurchaseOrder`; the read now joins `Vendors` and `ItemMaster`.
6. Most non-procurement API routes previously used SQLite even when
   `DB_CLIENT=mssql`. A database-mode guard now rejects those unmapped routes
   explicitly until their SQL Server adapters are implemented.
7. Generic edit screens previously constructed update URLs using SQLite row IDs
   while their APIs expected business document numbers. The shared resource
   page and all editable route configurations now use the correct entity keys.
8. The former broken change-password URL is now replaced by
   `/api/auth/change-password`.
9. Simplified demo workflows now validate required fields and expose supported
   status transitions, but single-line document entry remains below WinForms
   parity for indents, work orders, quotes, stock documents and delivery
   challans.
10. PR generation now propagates the selected header vendor to every line,
    returns all auto-split PR numbers, and sends the exact approve/hold/release/
    reject actions expected by both database adapters.
11. PO approval now exposes and enforces the next PM, MH, PC, OM or GM step,
    keeps role checks at the API boundary, and permits the post-approval
    generate/send lifecycle. PO status responses and search use the same
    contract in SQLite and SQL Server modes.
12. GRN entry now sends canonical PO-line and received-quantity fields, groups
    selected lines by PO, rejects over-receipt, and persists invoice numbers.
    The complete PR → PO → role approvals → generate → send → GRN flow is
    covered by API regression tests and a browser test in SQLite mode.
13. Price variance now uses the same 5% watch and 10% alert thresholds in both
    modes. The SQL Server adapter compares each PR line with its latest
    `PurchaseOrder` unit price and falls back to `ItemMaster` unit/fixed cost.
14. Gate inward/outward now maps multi-line entries to `SecurityInward` and
    `SecurityOutward`, preserves SI/SO numbering and logistics fields, and
    writes `ERPTransactionLog` in the same transaction. These gate records do
    not mutate inventory balances, matching the legacy workflow.
15. City, customer, vendor and item masters now read and write the matching
    SQL Server tables. Cities resolve `StateMaster` by name or id (legacy stores
    `StateMaster.Id` in `CityMaster.StateCode`). Item `standardCost` maps to
    `UnitCost` and writes `ItemStdCostHistory` when it changes; `fixedCost` maps
    separately to `FixedCost`.
16. Live GRN create now posts legacy inventory in the same SQL transaction:
    allocates `ITWGIN{n}` via `JobMovement`, inserts `Receipt` and
    `ERPInventoryLogs`, updates `ItemMaster` on-hand qty, writes
    `ERPTransactionLog`, and stores `ProcurementGRN.LegacyGinNumber`. Invoice
    numbers are required and vendor+invoice duplicates are rejected. Full
    domestic WAR, WO/other-item GIN and reverse GIN remain.
17. PO convert now loads `VendorDetails` GST/payment/delivery/inco terms, writes
    line GST amounts, `StandardCost`/`DiffinPer`/`DiffinRs`/`LatestPurchasePrice`,
    and upserts `POReport`.
18. Additional Masters (tax/CST/others-tax/discount/excise/payment/delivery),
    PEG/currency rates, project create/update/approve, and target-cost
    propose/GM-approve are live against matching SQL Server tables.
19. Phase3 `ProcurementComment`/`ProcurementAttachment` are dual-mode; PR
    Approval and PO Status expose a collaboration comments panel.
20. Stock ledger reads `ItemMaster`; item issue/return post FIFO updates to
    `ERPInventoryLogs`, adjust `AvailableQty`, and write `ERPTransactionLog`.
21. `/api/reports` exposes the 57-report catalog with live runners for the
    mapped subset (stock, PO/PR registers, masters, ERP log, etc.).

## Definition of full parity

A feature is not marked Live until all of the following are true:

1. It is reachable through permission-aware navigation.
2. All legacy inputs, validations and state transitions are represented.
3. Reads and writes use the matching SQL Server tables inside transactions.
4. Role/approver rules match active legacy behavior.
5. Print/export/email/document behavior has a web-safe equivalent.
6. An audit entry is recorded for a state-changing operation.
7. Automated tests cover valid, invalid and unauthorized transitions.
8. The workflow has been exercised against a copy of `ERP_Database`.

## Migration order

1. Foundation: encrypted legacy auth, complete permission model, SQL schema
   compatibility checks, audit log, files, export/print and notification services.
2. Existing web modules: replace SQLite-only branches with exact SQL Server
   adapters and repair generic update/detail routes.
3. Inventory and procurement: complete PO/WO, GIN, reverse/inspection, issue,
   return, cycle count, location and ledgers.
4. Projects: full project/BOM/locks/transfers/status/documents/QR/closure,
   machine passport, order entry and packing list.
5. Sales and service: visits, enquiries, opportunities, all quote variants,
   service orders and PI.
6. Quality, complaints, Kanban, timesheets and machine utilization.
7. All 57 report queries and their Excel/PDF/label outputs.

Forms that are empty, placeholder-only or duplicate copies should be classified
as Legacy stub rather than migrated as production behavior. Unreachable or
commented menu entries require source review because some contain substantial
working workflows while others are empty.

## Legacy reachability and migration scope

“All features” means all substantial working behavior, including useful forms
that were implemented but never connected to the classic `Form2` menu. It does
not mean reproducing empty event handlers or duplicate source copies.

Implemented orphaned workflows remain migration candidates:

- Additional Masters for tax, payment, delivery, discount, excise, CST and
  other PO terms
- the separate `WinFormsApp1` PR, clubbing, PR-to-PO and item-code suite
- the duplicate-but-functional material item-code screens
- Design BOM, project status planning, project tracker and newer service-order
  variants

The following are recorded as legacy stubs, not production parity requirements:

- classic `POStatus` (test message boxes), Inventory `DashBoard`,
  `frmDeliveryChallan`, `frmServiceCallManager`, `PackingList`,
  `frmInvoiceUpdate`, `frmProjectClosure`, `frmProjectDetails` and `BOMPR`
- dead menu actions for Order Entry, Inventory Dashboard and Service Call
  Manager; substantial underlying Order Entry code remains a migration
  candidate even though its menu launch was commented out
- byte-identical copies, secondary launcher/home forms and load-only placeholder
  views, which do not represent separate business capabilities

## Intentional security changes

Behavior that is unsafe must not be reproduced literally:

- The hard-coded master password in the WinForms login is intentionally excluded.
- Forgot password must use a one-time reset flow; it must not decrypt and email
  the existing password.
- SQL statements must be parameterized rather than assembled from UI text.
- Passwords remain compatible with the existing AES values while the legacy
  database is in use, but a future migration should replace reversible
  encryption with a one-way password hash.
- File paths, mail credentials and SQL credentials must come from environment
  configuration and must never be committed.
