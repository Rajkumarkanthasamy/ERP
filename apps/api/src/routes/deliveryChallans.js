import { Router } from 'express';
import { authRequired, requireAnyLegacyPermission } from '../middleware/auth.js';
import * as deliveryChallanService from '../services/deliveryChallanService.js';

const router = Router();
router.use(authRequired, requireAnyLegacyPermission('receipt', 'projectMaster'));

router.get('/', authRequired, (req, res) => {
  res.json(deliveryChallanService.listDeliveryChallans(req.query));
});

router.post('/', authRequired, (req, res) => {
  try {
    res.status(201).json(deliveryChallanService.createDeliveryChallan(req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/:dcNumber', authRequired, (req, res) => {
  const row = deliveryChallanService.getDeliveryChallan(req.params.dcNumber);
  if (!row) return res.status(404).json({ error: 'Delivery challan not found' });
  res.json(row);
});

router.put('/:dcNumber', authRequired, (req, res) => {
  try {
    res.json(deliveryChallanService.updateDeliveryChallan(req.params.dcNumber, req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.post('/:dcNumber/status', authRequired, (req, res) => {
  try {
    res.json(
      deliveryChallanService.updateDeliveryChallanStatus(req.params.dcNumber, { ...req.body, user: req.user })
    );
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

export default router;
