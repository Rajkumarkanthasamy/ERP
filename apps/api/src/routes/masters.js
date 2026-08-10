import { Router } from 'express';
import { authRequired, requirePermission } from '../middleware/auth.js';
import { isMssqlMode } from '../db/mssql.js';
import * as masterService from '../services/masterService.js';
import * as legacy from '../services/mssqlLegacyService.js';

const router = Router();

router.get('/vendors', authRequired, async (req, res) => {
  try {
    if (isMssqlMode()) return res.json(await legacy.listVendors());
    res.json(masterService.listVendors(req.query.q));
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.post('/vendors', authRequired, (req, res) => {
  try {
    res.status(201).json(masterService.upsertVendor(req.body));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.put('/vendors/:code', authRequired, (req, res) => {
  try {
    res.json(masterService.upsertVendor({ ...req.body, vendorCode: req.params.code }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/projects', authRequired, (req, res) => {
  res.json(masterService.listProjects(req.query.q));
});

router.get('/projects/:code', authRequired, (req, res) => {
  const project = masterService.getProject(req.params.code);
  if (!project) return res.status(404).json({ error: 'Project not found' });
  res.json(project);
});

router.post('/projects', authRequired, (req, res) => {
  try {
    res.status(201).json(masterService.upsertProject(req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.put('/projects/:code', authRequired, (req, res) => {
  try {
    res.json(masterService.upsertProject({ ...req.body, projectCode: req.params.code }, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.post('/projects/:code/approve', authRequired, requirePermission('canApprovePR'), (req, res) => {
  try {
    res.json(masterService.approveProject(req.params.code, { ...req.body, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/items', authRequired, async (req, res) => {
  try {
    if (isMssqlMode()) return res.json(await legacy.listItems(req.query.q));
    res.json(masterService.listItems(req.query.q));
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.post('/items', authRequired, (req, res) => {
  try {
    res.status(201).json(masterService.upsertItem(req.body));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.put('/items/:code', authRequired, (req, res) => {
  try {
    res.json(masterService.upsertItem({ ...req.body, itemCode: req.params.code }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/cities', authRequired, (req, res) => {
  res.json(masterService.listCities(req.query.q));
});

router.post('/cities', authRequired, (req, res) => {
  try {
    res.status(201).json(masterService.upsertCity(req.body));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.put('/cities/:code', authRequired, (req, res) => {
  try {
    res.json(masterService.upsertCity({ ...req.body, cityCode: req.params.code }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/customers', authRequired, (req, res) => {
  res.json(masterService.listCustomers(req.query.q));
});

router.post('/customers', authRequired, (req, res) => {
  try {
    res.status(201).json(masterService.upsertCustomer(req.body));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.put('/customers/:code', authRequired, (req, res) => {
  try {
    res.json(masterService.upsertCustomer({ ...req.body, customerCode: req.params.code }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/assets', authRequired, (req, res) => {
  res.json(masterService.listAssets(req.query.q));
});

router.post('/assets', authRequired, (req, res) => {
  try {
    res.status(201).json(masterService.upsertAsset(req.body));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.put('/assets/:code', authRequired, (req, res) => {
  try {
    res.json(masterService.upsertAsset({ ...req.body, assetCode: req.params.code }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/sales-products', authRequired, (req, res) => {
  res.json(masterService.listSalesProducts(req.query.q));
});

router.post('/sales-products', authRequired, (req, res) => {
  try {
    res.status(201).json(masterService.upsertSalesProduct(req.body));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.put('/sales-products/:code', authRequired, (req, res) => {
  try {
    res.json(masterService.upsertSalesProduct({ ...req.body, productCode: req.params.code }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/boms', authRequired, (req, res) => {
  res.json(masterService.listBoms(req.query.status));
});

router.get('/boms/:code', authRequired, (req, res) => {
  const bom = masterService.getBom(req.params.code);
  if (!bom) return res.status(404).json({ error: 'BOM not found' });
  res.json(bom);
});

router.post('/boms', authRequired, (req, res) => {
  try {
    res.status(201).json(masterService.createBom(req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.post('/boms/:code/approve', authRequired, requirePermission('canApprovePR'), (req, res) => {
  try {
    res.json(masterService.approveBom(req.params.code, { ...req.body, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/users', authRequired, (_req, res) => {
  res.json(masterService.listUsers());
});

router.get('/item-codes', authRequired, (req, res) => {
  res.json(masterService.listItemCodeRequests(req.query.status));
});

router.post('/item-codes', authRequired, (req, res) => {
  try {
    res.status(201).json(masterService.createItemCodeRequest(req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.post('/item-codes/:id/decide', authRequired, requirePermission('canApprovePR'), (req, res) => {
  try {
    res.json(masterService.decideItemCodeRequest(Number(req.params.id), { ...req.body, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

export default router;
