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

In SQL Server mode, only authentication, dashboard, PR, PO, GRN, and parts of the
vendor/item master currently have explicit MSSQL code paths. Other route groups
still call the SQLite services.

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
| City master | Create/update city and state mapping | Demo | MSSQL `CityMaster` mapping |
| Customer master | Full address/tax/contact fields and approval | Demo | MSSQL `CustomerMaster`, approval and complete field set |
| Vendor master | Full address, tax, banking, MSME, currency, approval and search | Partial | Live basic read is schema-aligned; create/update/approve, banking and `VendorDetails` remain |
| Item master | Item create/update, stock fields, UOM, storage, drawing, HSN/SAC, costs and history | Partial | Live basic read is schema-aligned; writes, full fields, cost history and item approval remain |
| Standard/target cost | Cost update with audit histories and restricted access | Demo | MSSQL updates to `ItemMaster`, `ItemStdCostHistory`, `ItemTargetCostHistory` |
| Product master/BOM | Product tree and BOM maintenance | Demo | Live product/BOM tree, versioning and approvals |
| Fixed assets | Asset register, cost centre, barcode/verification and documents | Demo | Full `AssetMaster`, `AssetCostCentre`, `AssetDocumentInfo`, upload/verification |
| Procurement PR | PR creation, approval/reject/hold/release, clubbing and PR-to-PO prototype | Partial | SQL query/schema alignment, exact role rules, BOM-origin traceability and complete validation |
| Purchase order | PO/WO creation, pricing/tax/terms, approval chain, finalization, update request, cancel/close, repeat order, vendor confirmation, tracking, reminders | Partial | Web covers a reduced PR-to-PO/approval/status path; most original PO screens and actions are missing |
| Work order | Job-work order generation, approval, costs, issue/return and tracking | Demo | Live `WorkOrder`, `WOOtherCost`, `WOApproveList`, job issue/movement |
| Currency/terms | Currency rate, GST/tax, payment, delivery, packing/forwarding and general terms | Missing | All related master screens and PO integration |
| GIN/receipt | PO/WO receipt, other-item GIN, inspection request, document scanning, tax, service GIN | Demo | Live `Receipt`, `GINOtherItemReceipt`, inspection and attachment/scanning workflow |
| Reverse GIN | Reverse receipt and inventory effects | Missing | `RiverseGIN`/`ERPReverseInventoryLogs` transaction |
| Item issue/return | Project/job issue, return, stock validation and FIFO logs | Demo | Live atomic inventory transactions and project/job rules |
| Inventory | Ledgers, cycle count, location update/history, stock adjustment, grading and dashboard | Demo | Live inventory calculations, audit logs, cycle count and location workflows |
| Delivery challan/DC | DC generation and related project/receipt data | Demo | Legacy document numbering, line rules, printing/export and SQL mapping |
| Gate entry | Inward, outward, manual inward and report | Demo | `SecurityInward`/`SecurityOutward` mappings, complete fields and report |
| Project create/approve | Full project/customer/product/order metadata and approvals | Demo | `ProjectMaster` mapping, role approvals and full fields |
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
| PEG rate | PEG/currency rate maintenance | Missing | `PegRate` UI/API |
| Service management | Service call manager and service order entry | Demo | Live service workflow and service-order variants |
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
| ERP reports | 57 report types with date/project/vendor/item filters | Missing | Current page is only KPI shortcuts |
| Export/print | Excel/CSV, PDF, QR/barcode, GIN/PO/packing labels and report printing | Missing | Server/browser-safe exporters and templates |
| Email/notifications | Outlook-based quote, PO, credential, closure and reminder mail | Missing | Configurable SMTP/provider integration, templates and audit |
| Documents | File scanning, project/asset attachments and network paths | Partial | Procurement attachment metadata exists; broader storage/download/versioning is absent |
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

The web `ReportsPage` currently links to six existing list pages; it does not
implement the legacy report queries or export/print behavior.

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
6. Most non-procurement API routes always use SQLite even when
   `DB_CLIENT=mssql`.
7. Several generic edit screens construct an update URL using SQLite row IDs
   while their APIs expect business document numbers.
8. The former broken change-password URL is now replaced by
   `/api/auth/change-password`.

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

Forms that are commented out, empty, duplicated, or unreachable from the
WinForms main menu should be classified as Legacy stub rather than migrated as
production behavior.

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
