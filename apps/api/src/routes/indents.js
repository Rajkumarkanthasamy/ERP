import { Router } from 'express';
import {
  authRequired,
  requireAnyLegacyPermission,
  requirePermission,
} from '../middleware/auth.js';
import { requireSqliteMode } from '../middleware/dbMode.js';
import * as indentService from '../services/indentService.js';

const router = Router();
router.use(authRequired, requireAnyLegacyPermission('indent'));
router.use(requireSqliteMode);

router.get('/', authRequired, (req, res) => {
  res.json(indentService.listIndents(req.query));
});

router.post('/', authRequired, (req, res) => {
  try {
    res.status(201).json(indentService.createIndent(req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/:indentNumber', authRequired, (req, res) => {
  const row = indentService.getIndent(req.params.indentNumber);
  if (!row) return res.status(404).json({ error: 'Indent not found' });
  res.json(row);
});

router.put('/:indentNumber', authRequired, (req, res) => {
  try {
    res.json(indentService.updateIndent(req.params.indentNumber, req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.post('/:indentNumber/status', authRequired, requirePermission('canApprovePR'), (req, res) => {
  try {
    res.json(indentService.updateIndentStatus(req.params.indentNumber, { ...req.body, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

export default router;
