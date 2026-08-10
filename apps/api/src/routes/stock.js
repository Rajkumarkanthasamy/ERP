import { Router } from 'express';
import { authRequired } from '../middleware/auth.js';
import * as stockService from '../services/stockService.js';

const router = Router();

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

export default router;
