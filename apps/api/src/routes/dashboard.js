import { Router } from 'express';
import { authRequired } from '../middleware/auth.js';
import { isMssqlMode } from '../db/mssql.js';
import { getDashboard } from '../services/dashboardService.js';
import * as legacy from '../services/mssqlLegacyService.js';

const router = Router();

router.get('/', authRequired, async (_req, res) => {
  try {
    if (isMssqlMode()) {
      return res.json(await legacy.dashboardSummary());
    }
    res.json(getDashboard());
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

export default router;
