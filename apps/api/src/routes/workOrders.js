import { Router } from 'express';
import {
  authRequired,
  requireAnyLegacyPermission,
  requirePermission,
} from '../middleware/auth.js';
import * as workOrderService from '../services/workOrderService.js';

const router = Router();
router.use(authRequired, requireAnyLegacyPermission('workOrder'));

router.get('/', authRequired, (req, res) => {
  res.json(workOrderService.listWorkOrders(req.query));
});

router.post('/', authRequired, (req, res) => {
  try {
    res.status(201).json(workOrderService.createWorkOrder(req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/:woNumber', authRequired, (req, res) => {
  const row = workOrderService.getWorkOrder(req.params.woNumber);
  if (!row) return res.status(404).json({ error: 'Work order not found' });
  res.json(row);
});

router.put('/:woNumber', authRequired, (req, res) => {
  try {
    res.json(workOrderService.updateWorkOrder(req.params.woNumber, req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.post('/:woNumber/status', authRequired, requirePermission('canApprovePR'), (req, res) => {
  try {
    res.json(workOrderService.updateWorkOrderStatus(req.params.woNumber, { ...req.body, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

export default router;
