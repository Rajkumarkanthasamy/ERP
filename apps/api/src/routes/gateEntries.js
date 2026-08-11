import { Router } from 'express';
import { authRequired, requireAnyLegacyPermission } from '../middleware/auth.js';
import * as gateEntryService from '../services/gateEntryService.js';

const router = Router();
router.use(authRequired, requireAnyLegacyPermission('securityCheck'));

router.get('/', authRequired, (req, res) => {
  res.json(gateEntryService.listGateEntries(req.query));
});

router.post('/', authRequired, (req, res) => {
  try {
    res.status(201).json(gateEntryService.createGateEntry(req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/:entryNumber', authRequired, (req, res) => {
  const row = gateEntryService.getGateEntry(req.params.entryNumber);
  if (!row) return res.status(404).json({ error: 'Gate entry not found' });
  res.json(row);
});

router.put('/:entryNumber', authRequired, (req, res) => {
  try {
    res.json(gateEntryService.updateGateEntry(req.params.entryNumber, req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.post('/:entryNumber/status', authRequired, (req, res) => {
  try {
    res.json(gateEntryService.updateGateEntryStatus(req.params.entryNumber, { ...req.body, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

export default router;
