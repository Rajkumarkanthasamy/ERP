import { Router } from 'express';
import { authRequired, requireAnyLegacyPermission } from '../middleware/auth.js';
import { isMssqlMode } from '../db/mssql.js';
import * as grnService from '../services/grnService.js';
import * as processSvc from '../services/mssqlProcessService.js';
import { mssqlQuery } from '../db/mssql.js';

const router = Router();
router.use(authRequired, requireAnyLegacyPermission('receipt', 'purchaseOrder'));

router.get('/open-po-lines', authRequired, async (_req, res) => {
  try {
    if (isMssqlMode()) return res.json(await processSvc.listOpenPOLines());
    res.json(grnService.listOpenPOLines());
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.get('/', authRequired, async (_req, res) => {
  try {
    if (isMssqlMode()) {
      try {
        const result = await mssqlQuery(`
          SELECT TOP 100
            g.GRNID AS id, g.GRNNumber AS grnNumber, g.PORef AS poRef,
            g.VendorCode AS vendorCode, g.ProjectCode AS projectCode,
            g.InvoiceNo AS invoiceNo, g.LegacyGinNumber AS legacyGinNumber,
            g.ReceivedBy AS receivedBy,
            g.ReceivedDate AS receivedDate, g.Status AS status, g.Remarks AS remarks,
            ISNULL((SELECT SUM(d.Amount) FROM ProcurementGRNDetail d
                    WHERE d.GRNNumber = g.GRNNumber), 0) AS totalAmount
          FROM ProcurementGRN g
          ORDER BY g.GRNID DESC
        `);
        return res.json(result.recordset);
      } catch (err) {
        if (/Invalid object name/i.test(err.message)) {
          return res.json([]);
        }
        if (/Invalid column name 'LegacyGinNumber'/i.test(err.message)) {
          const result = await mssqlQuery(`
            SELECT TOP 100
              g.GRNID AS id, g.GRNNumber AS grnNumber, g.PORef AS poRef,
              g.VendorCode AS vendorCode, g.ProjectCode AS projectCode,
              g.InvoiceNo AS invoiceNo, g.ReceivedBy AS receivedBy,
              g.ReceivedDate AS receivedDate, g.Status AS status, g.Remarks AS remarks,
              ISNULL((SELECT SUM(d.Amount) FROM ProcurementGRNDetail d
                      WHERE d.GRNNumber = g.GRNNumber), 0) AS totalAmount
            FROM ProcurementGRN g
            ORDER BY g.GRNID DESC
          `);
          return res.json(result.recordset);
        }
        throw err;
      }
    }
    res.json(grnService.listGRNs());
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.get('/:grnNumber', authRequired, async (req, res) => {
  try {
    if (isMssqlMode()) {
      const header = await mssqlQuery(
        `
        SELECT GRNID AS id, GRNNumber AS grnNumber, PORef AS poRef, VendorCode AS vendorCode,
               ProjectCode AS projectCode, InvoiceNo AS invoiceNo,
               LegacyGinNumber AS legacyGinNumber,
               ReceivedBy AS receivedBy, ReceivedDate AS receivedDate,
               Status AS status, Remarks AS remarks
        FROM ProcurementGRN WHERE GRNNumber = @GRN
        `,
        { GRN: req.params.grnNumber }
      );
      if (!header.recordset[0]) return res.status(404).json({ error: 'GRN not found' });
      const details = await mssqlQuery(
        `
        SELECT GRNDetailID AS id, POID AS poId, ItemCode AS itemCode, OrderedQty AS orderedQty,
               ReceivedQty AS receivedQty, UnitPrice AS unitPrice, Amount AS amount, UOM AS uom, Remarks AS remarks
        FROM ProcurementGRNDetail WHERE GRNNumber = @GRN
        `,
        { GRN: req.params.grnNumber }
      );
      return res.json({ ...header.recordset[0], lines: details.recordset });
    }
    const grn = grnService.getGRN(req.params.grnNumber);
    if (!grn) return res.status(404).json({ error: 'GRN not found' });
    res.json(grn);
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

router.post('/', authRequired, async (req, res) => {
  try {
    if (isMssqlMode()) {
      return res.status(201).json(await processSvc.createGRN({ ...req.body, user: req.user }));
    }
    res.status(201).json(grnService.createGRN({ ...req.body, user: req.user }));
  } catch (err) {
    res.status(400).json({ error: err.message });
  }
});

export default router;
