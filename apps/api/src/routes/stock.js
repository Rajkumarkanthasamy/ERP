import { Router } from 'express';
import { authRequired, requireAnyLegacyPermission } from '../middleware/auth.js';
import { isMssqlMode } from '../db/mssql.js';
import { requireSqliteMode } from '../middleware/dbMode.js';
import * as stockService from '../services/stockService.js';
import * as mssqlStock from '../services/mssqlStockService.js';

const router = Router();
router.use(
  authRequired,
  requireAnyLegacyPermission(
    'receipt',
    'issue',
    'materialLedger',
    'kanbanItems',
    'kanbanIssue',
    'kanbanItemReturn',
    'kanbanStockAdjust'
  )
);

function notMapped(res, feature) {
  return res.status(501).json({
    code: 'MSSQL_WORKFLOW_NOT_MAPPED',
    error: `${feature} is not mapped for SQL Server mode yet`,
  });
}

function typedList(type) {
  return async (req, res) => {
    try {
      if (isMssqlMode()) {
        if (type === 'Issue' || type === 'Return') {
          return res.json(await mssqlStock.listTransactions({ ...req.query, type }));
        }
        return notMapped(res, type);
      }
      res.json(stockService.listTransactions({ ...req.query, type }));
    } catch (err) {
      res.status(500).json({ error: err.message });
    }
  };
}

function typedCreate(type) {
  return async (req, res) => {
    try {
      const body = { ...(req.body || {}), txnType: type };
      if (isMssqlMode()) {
        if (type === 'Issue' || type === 'Return') {
          return res.status(201).json(await mssqlStock.createTransaction(body, req.user));
        }
        return notMapped(res, type);
      }
      res.status(201).json(stockService.createTransaction(body, req.user));
    } catch (err) {
      res.status(400).json({ error: err.message });
    }
  };
}

router.get('/', async (req, res) => {
  try {
    if (isMssqlMode()) return res.json(await mssqlStock.listLedger(req.query.q));
    res.json(stockService.listLedger(req.query.q));
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.get('/ledger', async (req, res) => {
  try {
    if (isMssqlMode()) return res.json(await mssqlStock.listLedger(req.query.q));
    res.json(stockService.listLedger(req.query.q));
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.get('/transactions', async (req, res) => {
  try {
    if (isMssqlMode()) {
      if (req.query.type && !['Issue', 'Return', 'All'].includes(req.query.type)) {
        return notMapped(res, req.query.type);
      }
      return res.json(await mssqlStock.listTransactions(req.query));
    }
    res.json(stockService.listTransactions(req.query));
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.get('/txn-types', (_req, res) => {
  res.json(stockService.getTxnTypes());
});

router.get('/transactions/:txnNumber', requireSqliteMode, (req, res) => {
  const txn = stockService.getTransaction(req.params.txnNumber);
  if (!txn) return res.status(404).json({ error: 'Transaction not found' });
  res.json(txn);
});

router.post('/transactions', async (req, res) => {
  try {
    if (isMssqlMode()) {
      const type = req.body?.txnType;
      if (type !== 'Issue' && type !== 'Return') return notMapped(res, type || 'stock transaction');
      return res.status(201).json(await mssqlStock.createTransaction(req.body, req.user));
    }
    res.status(201).json(stockService.createTransaction(req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/gin', typedList('GIN'));
router.post('/gin', typedCreate('GIN'));
router.get('/issue', typedList('Issue'));
router.post('/issue', typedCreate('Issue'));
router.get('/return', typedList('Return'));
router.post('/return', typedCreate('Return'));
router.get('/adjust', typedList('Adjust'));
router.post('/adjust', typedCreate('Adjust'));
router.get('/production', typedList('Production'));
router.post('/production', typedCreate('Production'));

export default router;
