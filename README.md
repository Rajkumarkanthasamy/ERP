# BISS ExistERP (Web)

Standalone, free recreation of the legacy C# / .NET WinForms ExistERP as a responsive **React + Material UI** frontend and **Node.js** API. Designed to run locally or in Docker with no paid plugins.

## What’s included

Covers the full ExistERP menu surface (not only procurement):

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

## Quick start (local)

```bash
npm install
npm run seed          # seeds SQLite demo data
npm run dev           # API :4000 + Web :5173
```

Open http://localhost:5173

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

- Same business rules as the stabilized procurement flow: ₹12L PR auto-split/clubbing, PO approval tiers by amount+GST (PC ≤50k, OM ≤2.5L, else GM), Kanban, GRN partial receive, price variance Watch/Alert.
- SQLite keeps the app standalone and zero-cost; schema is modular so a future move to PostgreSQL is straightforward.
- Logo: https://www.biss.in/img/logo-036.png
