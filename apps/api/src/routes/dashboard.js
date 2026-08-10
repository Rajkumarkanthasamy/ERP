import { Router } from 'express';
import { authRequired } from '../middleware/auth.js';
import { getDashboard } from '../services/dashboardService.js';

const router = Router();

router.get('/', authRequired, (_req, res) => {
  try {
    res.json(getDashboard());
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

export default router;
