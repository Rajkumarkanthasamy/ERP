# BISS ExistERP (Web)

Standalone, free recreation of the legacy C# / .NET WinForms ExistERP as a responsive **React + Material UI** frontend and **Node.js** API. Designed to run locally or in Docker with no paid plugins.

## What’s included

Provides a web migration of the main ExistERP menu areas (not only
procurement). This is **not yet full behavioral parity** with every WinForms
screen:

- **Procurement** — Dashboard, Kanban, PR Generation / Approval / Clubbing, PR→PO workspace, PO Approval, PO Status View, GRN, Price Variance, Item Code request/approval, Work Orders (includes advanced flow from `cursor/po-status-view-9db7`)
- **ERP Masters** — Cities, Customers, Vendors, Items, Assets, Standard Cost, Sales Products
- **Stores** — Stock ledger, GIN, Issue, Return, Adjust, Production
- **Gate Entry** — Inward / Outward / Manual inward
- **Projects** — Indent, Project create, BOM approve, Installation, Documents
- **Sales** — Enquiries, Opportunities, Quotes
- **Service** — Service calls
- **Quality** — NC & Escalations
- **Time Sheet**, **Delivery Challan**, **Complaints**, **Users**, **Reports**

Legacy WinForms archives remain in the repo (`Code.zip`, `WinFormsApp1.zip`) as reference.

## Stack (all free / open source)

| Layer | Tech |
|-------|------|
| Web | React 19, Vite, Material UI, React Router |
| API | Node.js, Express, better-sqlite3, JWT, bcryptjs |
| Runtime | Docker Compose (optional) or plain Node |

## Database

- **Default:** SQLite demo (`apps/api/data/erp.db`)
- **Your live ERP data:** SQL Server `ERP_Database` on `GTKA064W111\SQLEXPRESS01` (same as the C# app / SSMS)

See **[docs/DATABASE.md](docs/DATABASE.md)** for SSMS steps and how to point the Node API at SQL Server via `apps/api/.env` (`DB_CLIENT=mssql`).

## Quick start (local SQLite demo)

```bash
npm install
npm run seed          # seeds SQLite demo data
npm run dev           # API :4000 + Web :5173
```

Open http://localhost:5173

### Quick start (your SQL Server)

```bash
cp apps/api/.env.example apps/api/.env
# edit MSSQL_* values, set DB_CLIENT=mssql
npm install
npm run dev -w apps/api
# check http://localhost:4000/api/health/db
```

Login with existing ERP `Login` table users (same as C#).

The newer procurement flow (PR → Approve → Club → PO → Approve → GRN) has
explicit SQL Server adapters. The original WinForms PO/WO, inventory, project,
sales, reporting, permission, print, document and email features are still being
migrated.

See [docs/FEATURE_PARITY.md](docs/FEATURE_PARITY.md) for the evidence-based
screen/workflow comparison and remaining gaps. Procurement process details are
in [docs/PROCESS.md](docs/PROCESS.md).

### Demo logins

| User | Password | Role |
|------|----------|------|
| admin | admin123 | General Manager (full) |
| purchase | purchase123 | Purchase Manager |
| mh | mh123 | Manufacturing Head |
| om | om123 | Operations Manager |
| gm | gm123 | General Manager |
| user | user123 | Store User |

## Docker

```bash
docker compose up --build -d
```

- Web: http://localhost:8080  
- API health: http://localhost:4000/api/health  

SQLite DB and uploads persist in the `erp_data` volume.

## Project layout

```
apps/api   Node Express API + SQLite
apps/web   React MUI SPA
docker-compose.yml
legacy zips (Code.zip, WinFormsApp1.zip)
```

## Notes

- The newer procurement prototype includes ₹12L PR auto-split/clubbing, PO
  approval tiers by amount+GST (PC ≤50k, OM ≤2.5L, else GM), Kanban, GRN
  partial receive and price-variance indicators. It does not replace every
  original `Code.zip` PO/WO/GIN feature.
- SQLite keeps the app standalone and zero-cost; schema is modular so a future move to PostgreSQL is straightforward.
- Logo: https://www.biss.in/img/logo-036.png
