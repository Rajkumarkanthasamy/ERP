import { Router } from 'express';
import { authRequired, requireAnyLegacyPermission } from '../middleware/auth.js';
import { isMssqlMode } from '../db/mssql.js';
import * as reports from '../services/mssqlReportService.js';

const router = Router();
const access = requireAnyLegacyPermission('reports', 'purchaseOrder', 'materialLedger');

router.get('/', authRequired, access, (_req, res) => {
  res.json(reports.listReportCatalog());
});

router.get('/:id', authRequired, access, async (req, res) => {
  try {
    if (!isMssqlMode()) {
      return res.status(501).json({
        code: 'MSSQL_WORKFLOW_NOT_MAPPED',
        error: 'Live ERP reports require SQL Server mode',
      });
    }
    res.json(await reports.runReport(req.params.id, req.query));
  } catch (err) {
    if (err.code === 'MSSQL_WORKFLOW_NOT_MAPPED' || /Invalid object name/i.test(err.message)) {
      return res.status(501).json({
        code: 'MSSQL_WORKFLOW_NOT_MAPPED',
        error: err.message,
      });
    }
    res.status(400).json({ error: err.message });
  }
});

export default router;
