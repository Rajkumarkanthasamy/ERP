import { Router } from 'express';
import { authRequired } from '../middleware/auth.js';
import * as serviceCallService from '../services/serviceCallService.js';

const router = Router();

router.get('/', authRequired, (req, res) => {
  res.json(serviceCallService.listServiceCalls(req.query));
});

router.post('/', authRequired, (req, res) => {
  try {
    res.status(201).json(serviceCallService.createServiceCall(req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/:callNumber', authRequired, (req, res) => {
  const row = serviceCallService.getServiceCall(req.params.callNumber);
  if (!row) return res.status(404).json({ error: 'Service call not found' });
  res.json(row);
});

router.put('/:callNumber', authRequired, (req, res) => {
  try {
    res.json(serviceCallService.updateServiceCall(req.params.callNumber, req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.post('/:callNumber/status', authRequired, (req, res) => {
  try {
    res.json(serviceCallService.updateServiceCallStatus(req.params.callNumber, { ...req.body, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

export default router;
