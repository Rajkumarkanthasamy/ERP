import { Router } from 'express';
import { authRequired, requirePermission } from '../middleware/auth.js';
import * as prService from '../services/prService.js';
import { PR_STATUS } from '../constants.js';

const router = Router();

router.get('/', authRequired, (req, res) => {
  try {
    res.json(prService.listPRs(req.query));
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.get('/statuses', authRequired, (_req, res) => {
  res.json(Object.values(PR_STATUS));
});

router.get('/kanban', authRequired, (_req, res) => {
  try {
    res.json(prService.kanbanColumns());
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.post('/kanban/move', authRequired, requirePermission('canApprovePR'), (req, res) => {
  try {
    const { prNumber, targetStatus } = req.body || {};
    res.json(prService.moveKanbanCard(prNumber, targetStatus, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.post('/club', authRequired, requirePermission('canApprovePR'), (req, res) => {
  try {
    res.status(201).json(prService.clubPRs({ prNumbers: req.body?.prNumbers, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.post('/', authRequired, (req, res) => {
  try {
    const result = prService.createPRs({ ...req.body, user: req.user });
    res.status(201).json(result);
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/:prNumber', authRequired, (req, res) => {
  try {
    const pr = prService.getPR(req.params.prNumber);
    if (!pr) return res.status(404).json({ error: 'PR not found' });
    res.json(pr);
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.post('/:prNumber/status', authRequired, requirePermission('canApprovePR'), (req, res) => {
  try {
    res.json(prService.updatePRStatus(req.params.prNumber, { ...req.body, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

export default router;
