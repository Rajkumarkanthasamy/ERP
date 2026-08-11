import { Router } from 'express';
import { authRequired, requireAnyLegacyPermission } from '../middleware/auth.js';
import * as complaintService from '../services/complaintService.js';

const router = Router();
router.use(authRequired, requireAnyLegacyPermission('complaint'));

router.get('/', authRequired, (req, res) => {
  res.json(complaintService.listComplaints(req.query));
});

router.post('/', authRequired, (req, res) => {
  try {
    res.status(201).json(complaintService.createComplaint(req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/:complaintNumber', authRequired, (req, res) => {
  const row = complaintService.getComplaint(req.params.complaintNumber);
  if (!row) return res.status(404).json({ error: 'Complaint not found' });
  res.json(row);
});

router.put('/:complaintNumber', authRequired, (req, res) => {
  try {
    res.json(complaintService.updateComplaint(req.params.complaintNumber, req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.post('/:complaintNumber/status', authRequired, (req, res) => {
  try {
    res.json(complaintService.updateComplaintStatus(req.params.complaintNumber, { ...req.body, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

export default router;
