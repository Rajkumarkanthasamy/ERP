import { Router } from 'express';
import {
  authRequired,
  requireAnyLegacyPermission,
  requirePermission,
} from '../middleware/auth.js';
import * as salesService from '../services/salesService.js';

const router = Router();
router.use(
  authRequired,
  requireAnyLegacyPermission(
    'enquiryRegister',
    'opportunityDetails',
    'salesQuote',
    'quotation',
    'customerVisit',
    'serviceQuote',
    'testingLabQuote'
  )
);

// Enquiries
router.get('/enquiries', authRequired, (req, res) => {
  res.json(salesService.listEnquiries(req.query));
});

router.post('/enquiries', authRequired, (req, res) => {
  try {
    res.status(201).json(salesService.createEnquiry(req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/enquiries/:enquiryNumber', authRequired, (req, res) => {
  const row = salesService.getEnquiry(req.params.enquiryNumber);
  if (!row) return res.status(404).json({ error: 'Enquiry not found' });
  res.json(row);
});

router.put('/enquiries/:enquiryNumber', authRequired, (req, res) => {
  try {
    res.json(salesService.updateEnquiry(req.params.enquiryNumber, req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

// Opportunities
router.get('/opportunities', authRequired, (req, res) => {
  res.json(salesService.listOpportunities(req.query));
});

router.post('/opportunities', authRequired, (req, res) => {
  try {
    res.status(201).json(salesService.createOpportunity(req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/opportunities/:opportunityNumber', authRequired, (req, res) => {
  const row = salesService.getOpportunity(req.params.opportunityNumber);
  if (!row) return res.status(404).json({ error: 'Opportunity not found' });
  res.json(row);
});

router.put('/opportunities/:opportunityNumber', authRequired, (req, res) => {
  try {
    res.json(salesService.updateOpportunity(req.params.opportunityNumber, req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

// Quotes
router.get('/quotes', authRequired, (req, res) => {
  res.json(salesService.listQuotes(req.query));
});

router.post('/quotes', authRequired, (req, res) => {
  try {
    res.status(201).json(salesService.createQuote(req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/quotes/:quoteNumber', authRequired, (req, res) => {
  const row = salesService.getQuote(req.params.quoteNumber);
  if (!row) return res.status(404).json({ error: 'Quote not found' });
  res.json(row);
});

router.put('/quotes/:quoteNumber', authRequired, (req, res) => {
  try {
    res.json(salesService.updateQuote(req.params.quoteNumber, req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.post('/quotes/:quoteNumber/status', authRequired, requirePermission('canApprovePR'), (req, res) => {
  try {
    res.json(salesService.updateQuoteStatus(req.params.quoteNumber, { ...req.body, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

export default router;
