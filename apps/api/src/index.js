import './loadEnv.js';
import express from 'express';
import cors from 'cors';
import path from 'path';
import fs from 'fs';
import { fileURLToPath } from 'url';
import { getDb } from './db/connection.js';
import { isMssqlMode, mssqlHealth, getMssqlPool } from './db/mssql.js';
import { spawnSync } from 'child_process';

import authRoutes from './routes/auth.js';
import dashboardRoutes from './routes/dashboard.js';
import prRoutes from './routes/pr.js';
import poRoutes from './routes/po.js';
import grnRoutes from './routes/grn.js';
import masterRoutes from './routes/masters.js';
import collabRoutes from './routes/collab.js';
import stockRoutes from './routes/stock.js';
import indentRoutes from './routes/indents.js';
import workOrderRoutes from './routes/workOrders.js';
import salesRoutes from './routes/sales.js';
import serviceCallRoutes from './routes/serviceCalls.js';
import timesheetRoutes from './routes/timesheets.js';
import qualityRoutes from './routes/quality.js';
import complaintRoutes from './routes/complaints.js';
import gateEntryRoutes from './routes/gateEntries.js';
import deliveryChallanRoutes from './routes/deliveryChallans.js';
import additionalMasterRoutes from './routes/additionalMasters.js';
import reportRoutes from './routes/reports.js';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const app = express();
const PORT = Number(process.env.PORT || 4000);

app.use(
  cors({
    origin: process.env.CORS_ORIGIN || true,
    credentials: true,
  })
);
app.use(express.json({ limit: '5mb' }));

async function bootDatabase() {
  if (isMssqlMode()) {
    console.log('DB_CLIENT=mssql — using SQL Server ERP_Database');
    try {
      await getMssqlPool();
    } catch (err) {
      console.error('WARNING: Could not connect to SQL Server at startup:', err.message);
      console.error('API will start; /api/health/db shows details. Fix .env / network / SQL Browser.');
    }
    return;
  }

  // SQLite demo mode
  getDb();
  const userCount = getDb().prepare('SELECT COUNT(*) AS c FROM users').get().c;
  const seedPath = path.join(__dirname, 'db/seed.js');
  if (userCount === 0) {
    spawnSync(process.execPath, [seedPath], { stdio: 'inherit', env: process.env });
  } else {
    spawnSync(process.execPath, [seedPath], { stdio: 'inherit', env: process.env });
  }
}

const uploadDir = process.env.UPLOAD_DIR || path.join(process.cwd(), 'uploads');
fs.mkdirSync(uploadDir, { recursive: true });

app.get('/api/health', (_req, res) => {
  res.json({
    ok: true,
    service: 'biss-erp-api',
    dbClient: isMssqlMode() ? 'mssql' : 'sqlite',
    time: new Date().toISOString(),
  });
});

app.get('/api/health/db', async (_req, res) => {
  if (isMssqlMode()) {
    const health = await mssqlHealth();
    return res.status(health.ok ? 200 : 503).json(health);
  }
  try {
    const db = getDb();
    const users = db.prepare('SELECT COUNT(*) AS c FROM users').get().c;
    res.json({
      ok: true,
      client: 'sqlite',
      path: process.env.DB_PATH || 'apps/api/data/erp.db',
      users,
    });
  } catch (err) {
    res.status(503).json({ ok: false, client: 'sqlite', error: err.message });
  }
});

app.use('/api/auth', authRoutes);
app.use('/api/dashboard', dashboardRoutes);
app.use('/api/prs', prRoutes);
app.use('/api/pos', poRoutes);
app.use('/api/grns', grnRoutes);
app.use('/api/masters', masterRoutes);
app.use('/api/additional-masters', additionalMasterRoutes);
app.use('/api/collab', collabRoutes);
app.use('/api/stock', stockRoutes);
app.use('/api/reports', reportRoutes);
app.use('/api/indents', indentRoutes);
app.use('/api/work-orders', workOrderRoutes);
app.use('/api/sales', salesRoutes);
app.use('/api/service-calls', serviceCallRoutes);
app.use('/api/timesheets', timesheetRoutes);
app.use('/api/quality', qualityRoutes);
app.use('/api/complaints', complaintRoutes);
app.use('/api/gate-entries', gateEntryRoutes);
app.use('/api/delivery-challans', deliveryChallanRoutes);

const webDist = path.join(__dirname, '../../web/dist');
if (fs.existsSync(webDist)) {
  app.use(express.static(webDist));
  app.get('*', (req, res, next) => {
    if (req.path.startsWith('/api')) return next();
    res.sendFile(path.join(webDist, 'index.html'));
  });
}

app.use((err, _req, res, _next) => {
  console.error(err);
  res.status(500).json({ error: err.message || 'Server error' });
});

await bootDatabase();

app.listen(PORT, () => {
  console.log(`BISS ERP API listening on http://localhost:${PORT}`);
  console.log(`DB mode: ${isMssqlMode() ? 'SQL Server (ERP_Database)' : 'SQLite (local demo)'}`);
  console.log(`DB health: http://localhost:${PORT}/api/health/db`);
});
