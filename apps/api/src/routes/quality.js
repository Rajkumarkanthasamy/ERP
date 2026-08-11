import { Router } from 'express';
import { authRequired, requireAnyLegacyPermission } from '../middleware/auth.js';
import * as qualityService from '../services/qualityService.js';

const router = Router();
router.use(authRequired, requireAnyLegacyPermission('qualityManagement'));

// Non-conformances
router.get('/ncs', authRequired, (req, res) => {
  res.json(qualityService.listNcs(req.query));
});

router.post('/ncs', authRequired, (req, res) => {
  try {
    res.status(201).json(qualityService.createNc(req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/ncs/:ncNumber', authRequired, (req, res) => {
  const row = qualityService.getNc(req.params.ncNumber);
  if (!row) return res.status(404).json({ error: 'NC not found' });
  res.json(row);
});

router.put('/ncs/:ncNumber', authRequired, (req, res) => {
  try {
    res.json(qualityService.updateNc(req.params.ncNumber, req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.post('/ncs/:ncNumber/status', authRequired, (req, res) => {
  try {
    res.json(qualityService.updateNcStatus(req.params.ncNumber, { ...req.body, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

// Escalations
router.get('/escalations', authRequired, (req, res) => {
  res.json(qualityService.listEscalations(req.query));
});

router.post('/escalations', authRequired, (req, res) => {
  try {
    res.status(201).json(qualityService.createEscalation(req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/escalations/:escalationNumber', authRequired, (req, res) => {
  const row = qualityService.getEscalation(req.params.escalationNumber);
  if (!row) return res.status(404).json({ error: 'Escalation not found' });
  res.json(row);
});

router.put('/escalations/:escalationNumber', authRequired, (req, res) => {
  try {
    res.json(qualityService.updateEscalation(req.params.escalationNumber, req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.post('/escalations/:escalationNumber/status', authRequired, (req, res) => {
  try {
    res.json(
      qualityService.updateEscalationStatus(req.params.escalationNumber, { ...req.body, user: req.user })
    );
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

export default router;
