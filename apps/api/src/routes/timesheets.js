import { Router } from 'express';
import { authRequired, requirePermission } from '../middleware/auth.js';
import * as timesheetService from '../services/timesheetService.js';

const router = Router();

router.get('/', authRequired, (req, res) => {
  res.json(timesheetService.listTimesheets(req.query));
});

router.post('/', authRequired, (req, res) => {
  try {
    res.status(201).json(timesheetService.createTimesheet(req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.get('/:entryNumber', authRequired, (req, res) => {
  const row = timesheetService.getTimesheet(req.params.entryNumber);
  if (!row) return res.status(404).json({ error: 'Timesheet not found' });
  res.json(row);
});

router.put('/:entryNumber', authRequired, (req, res) => {
  try {
    res.json(timesheetService.updateTimesheet(req.params.entryNumber, req.body, req.user));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

router.post('/:entryNumber/status', authRequired, requirePermission('canApprovePR'), (req, res) => {
  try {
    res.json(timesheetService.updateTimesheetStatus(req.params.entryNumber, { ...req.body, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

export default router;
