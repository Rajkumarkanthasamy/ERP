import { Router } from 'express';
import {
  authRequired,
  requireAnyLegacyPermission,
} from '../middleware/auth.js';
import { isMssqlMode } from '../db/mssql.js';
import * as additional from '../services/mssqlAdditionalMasterService.js';

const router = Router();
const access = requireAnyLegacyPermission(
  'additionalMaster',
  'purchaseOrder',
  'pegRate'
);

router.get('/types', authRequired, access, (_req, res) => {
  res.json(additional.listMasterTypes());
});

router.get('/inco-terms', authRequired, access, async (_req, res) => {
  try {
    if (!isMssqlMode()) return res.json([]);
    res.json(await additional.listIncoTerms());
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.get('/currency-rates', authRequired, access, async (_req, res) => {
  try {
    if (!isMssqlMode()) return res.json([]);
    res.json(await additional.listPegRates());
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.post('/currency-rates', authRequired, access, async (req, res) => {
  try {
    if (!isMssqlMode()) {
      return res.status(501).json({
        code: 'MSSQL_WORKFLOW_NOT_MAPPED',
        error: 'Currency rates require SQL Server mode',
      });
    }
    res.status(201).json(await additional.upsertPegRate(req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/:type', authRequired, access, async (req, res) => {
  try {
    if (!isMssqlMode()) {
      return res.status(501).json({
        code: 'MSSQL_WORKFLOW_NOT_MAPPED',
        error: 'Additional masters require SQL Server mode',
      });
    }
    res.json(await additional.listAdditionalMasters(req.params.type, req.query.q));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.post('/:type', authRequired, access, async (req, res) => {
  try {
    if (!isMssqlMode()) {
      return res.status(501).json({
        code: 'MSSQL_WORKFLOW_NOT_MAPPED',
        error: 'Additional masters require SQL Server mode',
      });
    }
    res.status(201).json(await additional.upsertAdditionalMaster(req.params.type, req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.put('/:type/:code', authRequired, access, async (req, res) => {
  try {
    if (!isMssqlMode()) {
      return res.status(501).json({
        code: 'MSSQL_WORKFLOW_NOT_MAPPED',
        error: 'Additional masters require SQL Server mode',
      });
    }
    res.json(
      await additional.upsertAdditionalMaster(
        req.params.type,
        { ...req.body, code: req.params.code },
        req.user
      )
    );
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

export default router;
