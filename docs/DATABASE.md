# Database connection — ExistERP

## Your existing SQL Server (C# / SSMS)

Connection used by the WinForms app:

```
Data Source=GTKA064W111\SQLEXPRESS01;
Initial Catalog=ERP_Database;
User ID=sa;
Password=********
```

### Connect with SQL Server Management Studio

1. Open **SSMS**
2. **Server name:** `GTKA064W111\SQLEXPRESS01`
3. **Authentication:** SQL Server Authentication
4. **Login:** `sa`
5. **Password:** your SA password
6. Connect → expand **Databases** → **ERP_Database**

If connection fails in SSMS:

- Confirm SQL Server Browser is running
- Confirm TCP/IP is enabled for `SQLEXPRESS01` (SQL Server Configuration Manager)
- Confirm Windows Firewall allows the SQL port
- Try `localhost\SQLEXPRESS01` if you are on the same PC as the instance

---

## Connect the new Node / React web app to the same database

The web API supports two modes:

| `DB_CLIENT` | Data |
|-------------|------|
| `sqlite` (default) | Local demo file `apps/api/data/erp.db` |
| `mssql` | Your live `ERP_Database` on SQL Server |

### 1. Create `apps/api/.env`

```bash
cp apps/api/.env.example apps/api/.env
```

Edit `apps/api/.env`:

```env
DB_CLIENT=mssql
MSSQL_SERVER=GTKA064W111\\SQLEXPRESS01
MSSQL_DATABASE=ERP_Database
MSSQL_USER=sa
MSSQL_PASSWORD=your-password-here
MSSQL_ENCRYPT=false
MSSQL_TRUST_SERVER_CERTIFICATE=true
PORT=4000
JWT_SECRET=change-me
CORS_ORIGIN=http://localhost:5173
```

> Do **not** commit `.env`. It is gitignored. Use `.env.example` as the template only.

### 2. Run the API on a PC that can reach SQL Server

The Node process must run on the same network as `GTKA064W111` (usually your office PC). A cloud VM cannot see that host name unless you expose SQL Server safely (VPN / tunnel).

```bash
npm install
npm run dev -w apps/api
```

### 3. Verify the connection

Open:

- http://localhost:4000/api/health  
- http://localhost:4000/api/health/db  

Healthy MSSQL response includes `database: ERP_Database` and `serverName`.

### 4. Login to the web app

With `DB_CLIENT=mssql`, login uses the legacy **`Login`** table and the
AES-compatible password format from `Code/Cryptography.cs`:

- Username = `Login.UserName`
- Enter the user's normal password; the API encrypts it before comparison

Then open the React app (`npm run dev -w apps/web`) and use those ERP users.

### What works today against SQL Server

The mapped web subset currently supports these **reads and writes**:

1. Login (`Login` table — same users/passwords as C#)
2. Dashboard counts
3. **Create PR** → `PurchaseRequest` + `PurchaseRequestDetailNew` (₹12L auto-split)
4. **Approve / Reject / Hold / Release** (+ Kanban moves)
5. **PR Clubbing** (same vendor, ≤ ₹12L)
6. **PR → PO convert** → `PurchaseOrder`
7. **PO Approval** PM → MH → PC → OM/GM (amount + GST tiers)
8. **Generate / Send PO**
9. **PO Status View**
10. **Price variance** against latest PO / ItemMaster cost
11. **GRN** receive into the web `ProcurementGRN` extension (run
    `apps/api/sql/Phase3_Schema_Alignment.sql` once in SSMS) and, in the same
    SQL transaction, post legacy GIN stock to `JobMovement` / `Receipt` /
    `ERPInventoryLogs` / `ItemMaster` with `ERPTransactionLog` audit
12. **Gate Inward / Outward / Manual Inward** → `SecurityInward` /
    `SecurityOutward`
13. Vendors, Items and Projects (read/write + project approve)
14. **City / Customer / Vendor / Item masters** (create and update) →
    `CityMaster` + `StateMaster`, `CustomerMaster`, `Vendors`, `ItemMaster`
    with `ItemStdCostHistory` on standard/`UnitCost` changes
15. **Target cost** propose/GM-approve → `ItemTargetCostHistory` /
    `ItemMaster.TargetCost`
16. **Additional Masters** tax/payment/delivery/currency → matching term
    tables + `POCurrencyRate`
17. **PO convert GST/terms** from `VendorDetails` + `DiffinPer` / `POReport`
18. **Collab** comments/attachments → Phase3 `ProcurementComment` /
    `ProcurementAttachment`
19. **Stock ledger + Issue/Return** → `ItemMaster` / FIFO `ERPInventoryLogs`
20. **ERP reports catalog** (57 choices; mapped subset runs live SQL)

Live GRN requires an invoice number, allocates `ITWGIN{n}` via `JobMovement`,
stores `ProcurementGRN.LegacyGinNumber`, and updates on-hand qty without the
full WinForms domestic WAR recalculation. Reverse GIN, other-item GIN, and
freight/tax distribution remain out of scope. Other unmapped live routes return
`501 MSSQL_WORKFLOW_NOT_MAPPED` rather than falling back to SQLite.

Item master note: web `standardCost` is `ItemMaster.UnitCost` (legacy Standard
Cost screen). `fixedCost` is the separate `ItemMaster.FixedCost` field.

See **[PROCESS.md](PROCESS.md)** for the full flow diagram.

---

## Switch back to local SQLite demo

```env
DB_CLIENT=sqlite
```

Then:

```bash
npm run seed -w apps/api
npm run dev
```

Demo users: `admin` / `admin123`
