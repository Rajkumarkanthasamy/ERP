import { Router } from 'express';
import { authRequired, requireAnyLegacyPermission } from '../middleware/auth.js';
import { isMssqlMode } from '../db/mssql.js';
import * as gateEntryService from '../services/gateEntryService.js';
import * as mssqlGateEntryService from '../services/mssqlGateEntryService.js';

const router = Router();
router.use(authRequired, requireAnyLegacyPermission('securityCheck'));

router.get('/', authRequired, async (req, res) => {
  try {
    if (isMssqlMode()) {
      return res.json(await mssqlGateEntryService.listGateEntries(req.query));
    }
    return res.json(gateEntryService.listGateEntries(req.query));
  } catch (err) {
    return res.status(500).json({ error: err.message });
  }
});

router.post('/', authRequired, async (req, res) => {
  try {
    if (isMssqlMode()) {
      return res
        .status(201)
        .json(await mssqlGateEntryService.createGateEntry(req.body, req.user));
    }
    return res.status(201).json(gateEntryService.createGateEntry(req.body, req.user));
  } catch (err) {
    return res.status(400).json({ error: err.message });
  }
});

router.get('/:entryNumber', authRequired, async (req, res) => {
  try {
    const row = isMssqlMode()
      ? await mssqlGateEntryService.getGateEntry(req.params.entryNumber)
      : gateEntryService.getGateEntry(req.params.entryNumber);
    if (!row) return res.status(404).json({ error: 'Gate entry not found' });
    return res.json(row);
  } catch (err) {
    return res.status(500).json({ error: err.message });
  }
});

router.put('/:entryNumber', authRequired, (req, res) => {
  try {
    if (isMssqlMode()) {
      return res.status(405).json({
        error: 'Legacy gate records are immutable; create a correcting SI/SO entry',
        code: 'LEGACY_GATE_IMMUTABLE',
      });
    }
    return res.json(
      gateEntryService.updateGateEntry(req.params.entryNumber, req.body, req.user)
    );
  } catch (err) {
    return res.status(400).json({ error: err.message });
  }
});

router.post('/:entryNumber/status', authRequired, (req, res) => {
  try {
    if (isMssqlMode()) {
      return res.status(405).json({
        error: 'The legacy SecurityInward/SecurityOutward workflow has no status transition',
        code: 'LEGACY_GATE_NO_STATUS',
      });
    }
    return res.json(
      gateEntryService.updateGateEntryStatus(req.params.entryNumber, {
        ...req.body,
        user: req.user,
      })
    );
  } catch (err) {
    return res.status(400).json({ error: err.message });
  }
});

export default router;
