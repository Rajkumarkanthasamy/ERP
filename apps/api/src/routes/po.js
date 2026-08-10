import { Router } from 'express';
import { authRequired, requirePermission } from '../middleware/auth.js';
import * as poService from '../services/poService.js';

const router = Router();

router.get('/ready-prs', authRequired, (_req, res) => {
  try {
    res.json(poService.listApprovedPRsForPO());
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.post('/convert', authRequired, requirePermission('canGeneratePO'), (req, res) => {
  try {
    res.status(201).json(poService.convertPRsToPO({ ...req.body, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/approvals', authRequired, (_req, res) => {
  try {
    res.json(poService.listPOsForApproval());
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.get('/status', authRequired, (req, res) => {
  try {
    res.json(poService.listPOStatus(req.query));
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.get('/variance/:prNumber', authRequired, (req, res) => {
  try {
    res.json(poService.priceVariance(req.params.prNumber));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/:poRef', authRequired, (req, res) => {
  try {
    const po = poService.getPO(req.params.poRef);
    if (!po) return res.status(404).json({ error: 'PO not found' });
    res.json(po);
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.post('/:poRef/approve', authRequired, requirePermission('canApprovePO'), (req, res) => {
  try {
    res.json(poService.approvePO(req.params.poRef, { ...req.body, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

export default router;
