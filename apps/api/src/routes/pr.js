import { Router } from 'express';
import { authRequired, requirePermission } from '../middleware/auth.js';
import { isMssqlMode } from '../db/mssql.js';
import * as prService from '../services/prService.js';
import * as legacy from '../services/mssqlLegacyService.js';
import { PR_STATUS } from '../constants.js';

const router = Router();

router.get('/', authRequired, async (req, res) => {
  try {
    if (isMssqlMode()) {
      return res.json(await legacy.listPurchaseRequests(req.query));
    }
    res.json(prService.listPRs(req.query));
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.get('/statuses', authRequired, (_req, res) => {
  res.json(Object.values(PR_STATUS));
});

router.get('/kanban', authRequired, async (_req, res) => {
  try {
    if (isMssqlMode()) {
      const rows = await legacy.listPurchaseRequests({});
      const columns = {
        Pending: [],
        'On Hold': [],
        Approved: [],
        'Partially Converted': [],
        'Fully Converted': [],
        'PO Pending': [],
        'PO Approved': [],
      };
      for (const pr of rows) {
        if (columns[pr.status]) columns[pr.status].push(pr);
        else columns.Pending.push(pr);
      }
      return res.json(columns);
    }
    res.json(prService.kanbanColumns());
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.post('/kanban/move', authRequired, requirePermission('canApprovePR'), (req, res) => {
  try {
    if (isMssqlMode()) {
      return res.status(501).json({
        error: 'Kanban move write-back to SQL Server is not enabled yet. Use the C# app or SQLite mode for writes.',
      });
    }
    const { prNumber, targetStatus } = req.body || {};
    res.json(prService.moveKanbanCard(prNumber, targetStatus, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.post('/club', authRequired, requirePermission('canApprovePR'), (req, res) => {
  try {
    if (isMssqlMode()) {
      return res.status(501).json({ error: 'PR clubbing write to SQL Server is not enabled yet.' });
    }
    res.status(201).json(prService.clubPRs({ prNumbers: req.body?.prNumbers, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.post('/', authRequired, (req, res) => {
  try {
    if (isMssqlMode()) {
      return res.status(501).json({
        error: 'PR create write to SQL Server is not enabled yet. Reads use ERP_Database; writes still use SQLite mode or the C# app.',
      });
    }
    const result = prService.createPRs({ ...req.body, user: req.user });
    res.status(201).json(result);
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/:prNumber', authRequired, async (req, res) => {
  try {
    if (isMssqlMode()) {
      const pr = await legacy.getPurchaseRequest(req.params.prNumber);
      if (!pr) return res.status(404).json({ error: 'PR not found' });
      return res.json(pr);
    }
    const pr = prService.getPR(req.params.prNumber);
    if (!pr) return res.status(404).json({ error: 'PR not found' });
    res.json(pr);
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.post('/:prNumber/status', authRequired, requirePermission('canApprovePR'), (req, res) => {
  try {
    if (isMssqlMode()) {
      return res.status(501).json({ error: 'PR status write to SQL Server is not enabled yet.' });
    }
    res.json(prService.updatePRStatus(req.params.prNumber, { ...req.body, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

export default router;
