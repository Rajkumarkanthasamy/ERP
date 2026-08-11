import { Router } from 'express';
import {
  authRequired,
  requireAnyLegacyPermission,
  requirePermission,
  userCanCancelClosePO,
  userCanSendPO,
} from '../middleware/auth.js';
import { isMssqlMode } from '../db/mssql.js';
import * as poService from '../services/poService.js';
import * as legacy from '../services/mssqlLegacyService.js';
import * as processSvc from '../services/mssqlProcessService.js';

const router = Router();
const PO_STEP_ROLE = {
  pm: 'isPm',
  mh: 'isMh',
  pc: 'isPc',
  om: 'isOm',
  gm: 'isGm',
};
router.use(
  authRequired,
  requireAnyLegacyPermission(
    'purchaseOrder',
    'poWoGenerate',
    'poTrack',
    'purchaseManager',
    'manufacturingHead',
    'generalManager',
    'operationManager',
    'financeManager'
  )
);

router.get('/ready-prs', authRequired, async (_req, res) => {
  try {
    if (isMssqlMode()) {
      const rows = await legacy.listPurchaseRequests({ status: 'Approved' });
      const partial = await legacy.listPurchaseRequests({ status: 'Partially Converted' });
      return res.json([...rows, ...partial]);
    }
    res.json(poService.listApprovedPRsForPO());
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.post('/convert', authRequired, requirePermission('canGeneratePO'), async (req, res) => {
  try {
    if (isMssqlMode()) {
      return res.status(201).json(await processSvc.convertPRsToPO({ ...req.body, user: req.user }));
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
      return res.json(
        data.items.filter(
          (po) =>
            !po.poApproved &&
            !['Rejected', 'Cancelled', 'Closed'].includes(po.status)
        )
      );
    }
    res.json(poService.listPOsForApproval());
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.get('/status', authRequired, async (req, res) => {
  try {
    if (isMssqlMode()) return res.json(await legacy.listPurchaseOrders(req.query));
    res.json(poService.listPOStatus(req.query));
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.get('/variance/:prNumber', authRequired, async (req, res) => {
  try {
    if (isMssqlMode()) {
      return res.json(await legacy.priceVariance(req.params.prNumber));
    }
    return res.json(poService.priceVariance(req.params.prNumber));
  } catch (err) {
    return res.status(400).json({ error: err.message });
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

router.post('/:poRef/approve', authRequired, async (req, res) => {
  try {
    const step = req.body?.step || (req.body?.action === 'reject' ? 'reject' : null);
    if (!step) {
      return res.status(400).json({
        error: 'Approval step is required (pm, mh, pc, om, gm, generate, send, or reject)',
      });
    }
    if (step === 'send') {
      if (!userCanSendPO(req.user)) {
        return res.status(403).json({
          error: 'PO Track permission is required to send a PO to the vendor',
        });
      }
    } else if (step === 'generate') {
      if (!req.user?.canGeneratePO) {
        return res.status(403).json({ error: 'Missing permission: canGeneratePO' });
      }
    } else {
      if (!req.user?.canApprovePO) {
        return res.status(403).json({ error: 'Missing permission: canApprovePO' });
      }
      const roleFlag = PO_STEP_ROLE[step];
      if (roleFlag && !req.user?.[roleFlag]) {
        return res.status(403).json({
          error: `The ${step.toUpperCase()} approval role is required for this step`,
        });
      }
    }
    const payload = { ...req.body, step, user: req.user };
    if (isMssqlMode()) {
      return res.json(await processSvc.approvePO(req.params.poRef, payload));
    }
    return res.json(poService.approvePO(req.params.poRef, payload));
  } catch (err) {
    return res.status(400).json({ error: err.message });
  }
});

router.post('/:poRef/cancel', authRequired, async (req, res) => {
  try {
    if (!userCanCancelClosePO(req.user)) {
      return res.status(403).json({ error: 'You are not allowed to cancel purchase orders' });
    }
    const payload = { finalComment: req.body?.finalComment || req.body?.remarks, user: req.user };
    if (isMssqlMode()) return res.json(await processSvc.cancelPO(req.params.poRef, payload));
    return res.json(poService.cancelPO(req.params.poRef, payload));
  } catch (err) {
    return res.status(400).json({ error: err.message });
  }
});

router.post('/:poRef/close', authRequired, async (req, res) => {
  try {
    if (!userCanCancelClosePO(req.user)) {
      return res.status(403).json({ error: 'You are not allowed to close purchase orders' });
    }
    const payload = { finalComment: req.body?.finalComment || req.body?.remarks, user: req.user };
    if (isMssqlMode()) return res.json(await processSvc.closePO(req.params.poRef, payload));
    return res.json(poService.closePO(req.params.poRef, payload));
  } catch (err) {
    return res.status(400).json({ error: err.message });
  }
});

router.patch('/:poRef/track', authRequired, async (req, res) => {
  try {
    if (!userCanSendPO(req.user) && !req.user?.permissions?.purchaseOrder) {
      return res.status(403).json({ error: 'PO Track permission is required' });
    }
    const payload = {
      finalRemarks: req.body?.finalRemarks,
      oaDate: req.body?.oaDate,
      user: req.user,
    };
    if (isMssqlMode()) return res.json(await processSvc.updatePOTrack(req.params.poRef, payload));
    return res.json(poService.updatePOTrack(req.params.poRef, payload));
  } catch (err) {
    return res.status(400).json({ error: err.message });
  }
});

export default router;
