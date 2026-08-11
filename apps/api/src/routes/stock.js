import { Router } from 'express';
import { authRequired, requireAnyLegacyPermission } from '../middleware/auth.js';
import { requireSqliteMode } from '../middleware/dbMode.js';
import * as stockService from '../services/stockService.js';

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
router.use(requireSqliteMode);

function typedList(type) {
  return (req, res) => {
    res.json(stockService.listTransactions({ ...req.query, type }));
  };
}

function typedCreate(type) {
  return (req, res) => {
    try {
      const body = { ...(req.body || {}), txnType: type };
      res.status(201).json(stockService.createTransaction(body, req.user));
    } catch (err) {
      res.status(400).json({ error: err.message });
    }
  };
}

router.get('/', authRequired, (req, res) => {
  res.json(stockService.listLedger(req.query.q));
});

router.get('/ledger', authRequired, (req, res) => {
  res.json(stockService.listLedger(req.query.q));
});

router.get('/transactions', authRequired, (req, res) => {
  res.json(stockService.listTransactions(req.query));
});

router.get('/txn-types', authRequired, (_req, res) => {
  res.json(stockService.getTxnTypes());
});

router.get('/transactions/:txnNumber', authRequired, (req, res) => {
  const txn = stockService.getTransaction(req.params.txnNumber);
  if (!txn) return res.status(404).json({ error: 'Transaction not found' });
  res.json(txn);
});

router.post('/transactions', authRequired, (req, res) => {
  try {
    res.status(201).json(stockService.createTransaction(req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/gin', authRequired, typedList('GIN'));
router.post('/gin', authRequired, typedCreate('GIN'));
router.get('/issue', authRequired, typedList('Issue'));
router.post('/issue', authRequired, typedCreate('Issue'));
router.get('/return', authRequired, typedList('Return'));
router.post('/return', authRequired, typedCreate('Return'));
router.get('/adjust', authRequired, typedList('Adjust'));
router.post('/adjust', authRequired, typedCreate('Adjust'));
router.get('/production', authRequired, typedList('Production'));
router.post('/production', authRequired, typedCreate('Production'));

export default router;
