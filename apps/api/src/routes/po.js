import { Router } from 'express';
import { authRequired, requirePermission } from '../middleware/auth.js';
import { isMssqlMode } from '../db/mssql.js';
import * as poService from '../services/poService.js';
import * as legacy from '../services/mssqlLegacyService.js';

const router = Router();

router.get('/ready-prs', authRequired, async (_req, res) => {
  try {
    if (isMssqlMode()) {
      const rows = await legacy.listPurchaseRequests({ status: 'Approved' });
      return res.json(rows);
    }
    res.json(poService.listApprovedPRsForPO());
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.post('/convert', authRequired, requirePermission('canGeneratePO'), (req, res) => {
  try {
    if (isMssqlMode()) {
      return res.status(501).json({ error: 'PR→PO convert write to SQL Server is not enabled yet.' });
    }
    res.status(201).json(poService.convertPRsToPO({ ...req.body, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/approvals', authRequired, async (_req, res) => {
  try {
    if (isMssqlMode()) {
      const data = await legacy.listPurchaseOrders({});
      return res.json(data.items.filter((p) => !p.poApproved));
    }
    res.json(poService.listPOsForApproval());
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.get('/status', authRequired, async (req, res) => {
  try {
    if (isMssqlMode()) {
      return res.json(await legacy.listPurchaseOrders(req.query));
    }
    res.json(poService.listPOStatus(req.query));
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.get('/variance/:prNumber', authRequired, (req, res) => {
  try {
    if (isMssqlMode()) {
      return res.status(501).json({ error: 'Price variance against SQL Server is not enabled yet.' });
    }
    res.json(poService.priceVariance(req.params.prNumber));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/:poRef', authRequired, async (req, res) => {
  try {
    if (isMssqlMode()) {
      const data = await legacy.listPurchaseOrders({ poNumber: req.params.poRef });
      const po = data.items.find((p) => p.poRef === req.params.poRef) || data.items[0];
      if (!po) return res.status(404).json({ error: 'PO not found' });
      return res.json(po);
    }
    const po = poService.getPO(req.params.poRef);
    if (!po) return res.status(404).json({ error: 'PO not found' });
    res.json(po);
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.post('/:poRef/approve', authRequired, requirePermission('canApprovePO'), (req, res) => {
  try {
    if (isMssqlMode()) {
      return res.status(501).json({ error: 'PO approval write to SQL Server is not enabled yet.' });
    }
    res.json(poService.approvePO(req.params.poRef, { ...req.body, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

export default router;
