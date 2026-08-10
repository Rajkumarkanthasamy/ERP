import { Router } from 'express';
import { authRequired } from '../middleware/auth.js';
import * as grnService from '../services/grnService.js';

const router = Router();

router.get('/open-po-lines', authRequired, (_req, res) => {
  try {
    res.json(grnService.listOpenPOLines());
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.get('/', authRequired, (_req, res) => {
  try {
    res.json(grnService.listGRNs());
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.get('/:grnNumber', authRequired, (req, res) => {
  try {
    const grn = grnService.getGRN(req.params.grnNumber);
    if (!grn) return res.status(404).json({ error: 'GRN not found' });
    res.json(grn);
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.post('/', authRequired, (req, res) => {
  try {
    res.status(201).json(grnService.createGRN({ ...req.body, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

export default router;
