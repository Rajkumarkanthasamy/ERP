/**
 * Full ExistERP procurement process against live SQL Server tables
 * (mirrors WinForms PurchaseRequestDAL / ApprovalDAL / PRtoPODAL / POApprovalDAL / GRNDAL).
 */
import { getMssqlPool, mssqlQuery, sql } from '../db/mssql.js';
import {
  PR_LIMIT,
  PR_LINE_STATUS,
  PR_STATUS,
  computePoAmounts,
  getNextPoApprovalStep,
} from '../constants.js';

async function nextPrNumber(transaction) {
  const now = new Date();
  const prefix = `PR-${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-`;
  const req = new sql.Request(transaction);
  req.input('Prefix', sql.NVarChar, prefix);
  const result = await req.query(`
    SELECT MAX(PRNumber) AS lastNo
    FROM PurchaseRequest
    WHERE PRNumber LIKE @Prefix + '%'
  `);
  let next = 1;
  const last = result.recordset[0]?.lastNo;
  if (last) {
    const parts = String(last).split('-');
    const n = parseInt(parts[parts.length - 1], 10);
    if (!Number.isNaN(n)) next = n + 1;
  }
  return `${prefix}${String(next).padStart(7, '0')}`;
}

async function nextPoRef(transaction) {
  const now = new Date();
  const prefix = `PO-${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-`;
  const req = new sql.Request(transaction);
  req.input('Prefix', sql.NVarChar, prefix);
  const result = await req.query(`
    SELECT MAX(DBOMNo) AS lastNo
    FROM PurchaseOrder
    WHERE DBOMNo LIKE @Prefix + '%'
  `);
  let next = 1;
  const last = result.recordset[0]?.lastNo;
  if (last) {
    const parts = String(last).split('-');
    const n = parseInt(parts[parts.length - 1], 10);
    if (!Number.isNaN(n)) next = n + 1;
  }
  return `${prefix}${String(next).padStart(5, '0')}`;
}

async function getVendorName(vendorCode) {
  try {
    const r = await mssqlQuery(
      `SELECT TOP 1 VendorName FROM Vendors WHERE VendorCode = @VendorCode`,
      { VendorCode: vendorCode }
    );
    if (r.recordset[0]?.VendorName) return r.recordset[0].VendorName;
  } catch {
    /* try VendorMaster */
  }
  try {
    const r = await mssqlQuery(
      `SELECT TOP 1 VendorName FROM VendorMaster WHERE VendorCode = @VendorCode`,
      { VendorCode: vendorCode }
    );
    return r.recordset[0]?.VendorName || vendorCode;
  } catch {
    return vendorCode;
  }
}

function packByVendorLimit(lines) {
  const byVendor = new Map();
  for (const line of lines) {
    const key = line.vendorCode;
    if (!byVendor.has(key)) byVendor.set(key, []);
    byVendor.get(key).push(line);
  }
  const packs = [];
  for (const [vendorCode, vendorLines] of byVendor) {
    let current = [];
    let currentTotal = 0;
    for (const line of vendorLines) {
      const cost = Number(line.quantity) * Number(line.unitCost);
      if (current.length && currentTotal + cost > PR_LIMIT) {
        packs.push({ vendorCode, lines: current, total: currentTotal, overLimit: false });
        current = [];
        currentTotal = 0;
      }
      current.push(line);
      currentTotal += cost;
    }
    if (current.length) {
      packs.push({
        vendorCode,
        lines: current,
        total: currentTotal,
        overLimit: currentTotal > PR_LIMIT,
      });
    }
  }
  return packs;
}

export async function createPRs({
  projectCode,
  productNo,
  vendorCode,
  vendorName,
  lines,
  remarks,
  user,
}) {
  if (!projectCode) throw new Error('Project code is required');
  if (!lines?.length) throw new Error('At least one line item is required');

  const normalizedLines = lines.map((line) => ({
    ...line,
    vendorCode: line.vendorCode || vendorCode,
    vendorName: line.vendorName || vendorName,
  }));
  if (normalizedLines.some((line) => !line.vendorCode)) {
    throw new Error('Vendor code is required for every PR line');
  }
  if (
    normalizedLines.some(
      (line) =>
        !line.itemCode || Number(line.quantity) <= 0 || Number(line.unitCost) < 0
    )
  ) {
    throw new Error('Every PR line requires an item, positive quantity, and valid unit cost');
  }

  const packs = packByVendorLimit(normalizedLines);
  const pool = await getMssqlPool();
  const created = [];

  const tx = new sql.Transaction(pool);
  await tx.begin();
  try {
    for (const pack of packs) {
      const prNumber = await nextPrNumber(tx);
      const resolvedVendorName =
        pack.lines.find((line) => line.vendorName)?.vendorName ||
        (await getVendorName(pack.vendorCode));
      const headerReq = new sql.Request(tx);
      headerReq.input('PRNumber', sql.NVarChar, prNumber);
      headerReq.input('ProjectCode', sql.NVarChar, projectCode);
      headerReq.input('VendorCode', sql.NVarChar, pack.vendorCode);
      headerReq.input('VendorName', sql.NVarChar, resolvedVendorName);
      headerReq.input('TotalAmount', sql.Decimal(18, 2), pack.total);
      headerReq.input('Status', sql.NVarChar, PR_STATUS.PENDING);
      headerReq.input(
        'Remarks',
        sql.NVarChar,
        remarks || (pack.overLimit ? 'Warning: pack exceeds ₹12L limit' : null)
      );
      headerReq.input('CreatedBy', sql.NVarChar, user.displayName || user.username);
      const header = await headerReq.query(`
        INSERT INTO PurchaseRequest
          (PRNumber, ProjectCode, VendorCode, VendorName, TotalAmount, Status, Remarks,
           CreatedBy, RequestedBy, RequestDate, ModifiedBy, ModifiedDate, IsClubbed)
        VALUES
          (@PRNumber, @ProjectCode, @VendorCode, @VendorName, @TotalAmount, @Status, @Remarks,
           @CreatedBy, @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 0);
        SELECT CAST(SCOPE_IDENTITY() AS INT) AS id;
      `);
      const prId = header.recordset[0].id;

      for (const line of pack.lines) {
        const totalCost = Number(line.quantity) * Number(line.unitCost);
        const d = new sql.Request(tx);
        d.input('PRID', sql.NVarChar, String(prId));
        d.input('PRNumber', sql.NVarChar, prNumber);
        d.input('ProjectCode', sql.NVarChar, projectCode);
        d.input('ProductNo', sql.NVarChar, productNo || null);
        d.input('ItemCode', sql.NVarChar, line.itemCode);
        d.input('ItemDescription', sql.NVarChar, line.itemDescription || null);
        d.input('Specification', sql.NVarChar, line.specification || null);
        d.input('Make', sql.NVarChar, line.make || null);
        d.input('MfgPartNo', sql.NVarChar, line.mfgPartNo || null);
        d.input('Quantity', sql.Decimal(18, 3), line.quantity);
        d.input('UOM', sql.NVarChar, line.uom || 'NOS');
        d.input('UnitCost', sql.Decimal(18, 2), line.unitCost);
        d.input('TotalCost', sql.Decimal(18, 2), totalCost);
        d.input('VendorCode', sql.NVarChar, pack.vendorCode);
        d.input('VendorName', sql.NVarChar, resolvedVendorName);
        d.input('HSNCode', sql.NVarChar, line.hsnCode || null);
        d.input('LineStatus', sql.NVarChar, PR_LINE_STATUS.PENDING);
        d.input('CreatedBy', sql.NVarChar, user.username);
        await d.query(`
          INSERT INTO PurchaseRequestDetailNew
            (PRID, PRNumber, ProjectCode, ProductNo, ItemCode, ItemDescription, Specification,
             Make, MfgPartNo, Quantity, UOM, UnitCost, TotalCost, VendorCode, VendorName,
             HSNCode, LineStatus, CreatedBy, ModifiedBy, CreatedDate)
          VALUES
            (@PRID, @PRNumber, @ProjectCode, @ProductNo, @ItemCode, @ItemDescription, @Specification,
             @Make, @MfgPartNo, @Quantity, @UOM, @UnitCost, @TotalCost, @VendorCode, @VendorName,
             @HSNCode, @LineStatus, @CreatedBy, @CreatedBy, GETDATE())
        `);
      }

      created.push({
        prNumber,
        vendorCode: pack.vendorCode,
        totalAmount: pack.total,
        lineCount: pack.lines.length,
        overLimit: pack.overLimit,
      });
    }
    await tx.commit();
  } catch (err) {
    await tx.rollback();
    throw err;
  }

  return { created, autoSplit: created.length > 1 };
}

export async function updatePRStatus(prNumber, { action, reason, user }) {
  const name = user.displayName || user.username;
  if (action === 'approve') {
    await mssqlQuery(
      `
      UPDATE PurchaseRequest
      SET Status = @Status,
          ApprovedBy = @ApprovedBy,
          ApprovedDate = GETDATE(),
          ModifiedBy = @ApprovedBy,
          ModifiedDate = GETDATE(),
          Remarks = CASE WHEN @Remarks = '' THEN Remarks ELSE ISNULL(Remarks,'') + ' | ' + @Remarks END
      WHERE PRNumber = @PRNumber AND Status IN ('Pending','On Hold')
      `,
      {
        Status: PR_STATUS.APPROVED,
        ApprovedBy: name,
        Remarks: reason || '',
        PRNumber: prNumber,
      }
    );
  } else if (action === 'reject') {
    await mssqlQuery(
      `
      UPDATE PurchaseRequest
      SET Status = @Status,
          RejectionReason = @Reason,
          ModifiedBy = @ByUser,
          ModifiedDate = GETDATE()
      WHERE PRNumber = @PRNumber
      `,
      { Status: PR_STATUS.REJECTED, Reason: reason || 'Rejected', ByUser: name, PRNumber: prNumber }
    );
  } else if (action === 'hold') {
    await mssqlQuery(
      `
      UPDATE PurchaseRequest
      SET Status = @Status,
          HoldReason = @Reason,
          ModifiedBy = @ByUser,
          ModifiedDate = GETDATE()
      WHERE PRNumber = @PRNumber
      `,
      { Status: PR_STATUS.ON_HOLD, Reason: reason || 'On hold', ByUser: name, PRNumber: prNumber }
    );
  } else if (action === 'release') {
    await mssqlQuery(
      `
      UPDATE PurchaseRequest
      SET Status = @Status,
          HoldReason = NULL,
          ModifiedBy = @ByUser,
          ModifiedDate = GETDATE()
      WHERE PRNumber = @PRNumber AND Status = 'On Hold'
      `,
      { Status: PR_STATUS.PENDING, ByUser: name, PRNumber: prNumber }
    );
  } else {
    throw new Error('Unknown action');
  }

  const { getPurchaseRequest } = await import('./mssqlLegacyService.js');
  return getPurchaseRequest(prNumber);
}

export async function clubPRs({ prNumbers, user }) {
  if (!prNumbers || prNumbers.length < 2) throw new Error('Select at least two PRs to club');
  const pool = await getMssqlPool();
  const tx = new sql.Transaction(pool);
  await tx.begin();
  try {
    const placeholders = prNumbers.map((_, i) => `@p${i}`).join(',');
    const listReq = new sql.Request(tx);
    prNumbers.forEach((n, i) => listReq.input(`p${i}`, sql.NVarChar, n));
    const list = await listReq.query(`
      SELECT PRID, PRNumber, ProjectCode, VendorCode, VendorName, TotalAmount, Status, IsClubbed
      FROM PurchaseRequest WHERE PRNumber IN (${placeholders})
    `);
    const prs = list.recordset;
    if (prs.length !== prNumbers.length) throw new Error('One or more PRs not found');
    if (prs.some((p) => p.Status !== PR_STATUS.APPROVED)) {
      throw new Error('Only Approved PRs can be clubbed');
    }
    const vendorCode = prs[0].VendorCode;
    if (prs.some((p) => p.VendorCode !== vendorCode)) {
      throw new Error('All PRs must belong to the same vendor');
    }
    const total = prs.reduce((s, p) => s + Number(p.TotalAmount || 0), 0);
    if (total > PR_LIMIT) throw new Error(`Clubbed total exceeds ₹12,00,000 limit`);

    const newPRNumber = await nextPrNumber(tx);
    const name = user.displayName || user.username;
    const projectCode = [...new Set(prs.map((p) => p.ProjectCode))].join(',');
    const hdr = new sql.Request(tx);
    hdr.input('PRNumber', sql.NVarChar, newPRNumber);
    hdr.input('ProjectCode', sql.NVarChar, projectCode);
    hdr.input('VendorCode', sql.NVarChar, vendorCode);
    hdr.input('VendorName', sql.NVarChar, prs[0].VendorName);
    hdr.input('TotalAmount', sql.Decimal(18, 2), total);
    hdr.input('Status', sql.NVarChar, PR_STATUS.APPROVED);
    hdr.input('Remarks', sql.NVarChar, `Clubbed from: ${prNumbers.join(', ')}`);
    hdr.input('CreatedBy', sql.NVarChar, name);
    hdr.input('ApprovedBy', sql.NVarChar, name);
    hdr.input('ClubbedFromPRIDs', sql.NVarChar, prNumbers.join(','));
    const inserted = await hdr.query(`
      INSERT INTO PurchaseRequest
        (PRNumber, ProjectCode, VendorCode, VendorName, TotalAmount, Status, Remarks,
         CreatedBy, RequestedBy, RequestDate, ApprovedBy, ApprovedDate, ModifiedBy, ModifiedDate,
         IsClubbed, ClubbedFromPRIDs)
      VALUES
        (@PRNumber, @ProjectCode, @VendorCode, @VendorName, @TotalAmount, @Status, @Remarks,
         @CreatedBy, @CreatedBy, GETDATE(), @ApprovedBy, GETDATE(), @CreatedBy, GETDATE(),
         0, @ClubbedFromPRIDs);
      SELECT CAST(SCOPE_IDENTITY() AS INT) AS id;
    `);
    const newId = inserted.recordset[0].id;

    for (const pr of prs) {
      const copy = new sql.Request(tx);
      copy.input('NewPRID', sql.NVarChar, String(newId));
      copy.input('NewPRNumber', sql.NVarChar, newPRNumber);
      copy.input('SourcePR', sql.NVarChar, pr.PRNumber);
      copy.input('LineStatus', sql.NVarChar, PR_LINE_STATUS.PENDING);
      copy.input('CreatedBy', sql.NVarChar, name);
      await copy.query(`
        INSERT INTO PurchaseRequestDetailNew
          (PRID, PRNumber, ProjectBOMCode, ProjectCode, ProductCode, ProductNo, ItemCode,
           ItemDescription, Specification, Make, MfgPartNo, Quantity, UOM, UnitCost, TotalCost,
           VendorCode, VendorName, DrawingNo, Location, HSNCode, BOMQuantity,
           AlreadyPurchasedQty, BalanceQty, LineStatus, Remarks, CreatedBy, ModifiedBy, CreatedDate)
        SELECT
          @NewPRID, @NewPRNumber, ProjectBOMCode, ProjectCode, ProductCode, ProductNo, ItemCode,
          ItemDescription, Specification, Make, MfgPartNo, Quantity, UOM, UnitCost, TotalCost,
          VendorCode, VendorName, DrawingNo, Location, HSNCode, BOMQuantity,
          AlreadyPurchasedQty, BalanceQty, @LineStatus,
          'Clubbed from ' + PRNumber, @CreatedBy, @CreatedBy, GETDATE()
        FROM PurchaseRequestDetailNew
        WHERE PRNumber = @SourcePR
      `);

      const mark = new sql.Request(tx);
      mark.input('ClubbedStatus', sql.NVarChar, PR_STATUS.CLUBBED);
      mark.input('NewPRNumber', sql.NVarChar, newPRNumber);
      mark.input('SourcePR', sql.NVarChar, pr.PRNumber);
      mark.input('ByUser', sql.NVarChar, name);
      await mark.query(`
        UPDATE PurchaseRequest
        SET IsClubbed = 1,
            Status = @ClubbedStatus,
            ClubbedFromPRIDs = @NewPRNumber,
            ModifiedBy = @ByUser,
            ModifiedDate = GETDATE()
        WHERE PRNumber = @SourcePR
      `);
    }

    await tx.commit();
    const { getPurchaseRequest } = await import('./mssqlLegacyService.js');
    return getPurchaseRequest(newPRNumber);
  } catch (err) {
    await tx.rollback();
    throw err;
  }
}

export async function convertPRsToPO({ prNumbers, lineIds, user }) {
  if (!prNumbers?.length) throw new Error('Select at least one PR');
  const pool = await getMssqlPool();
  const tx = new sql.Transaction(pool);
  await tx.begin();
  try {
    const placeholders = prNumbers.map((_, i) => `@p${i}`).join(',');
    const req = new sql.Request(tx);
    prNumbers.forEach((n, i) => req.input(`p${i}`, sql.NVarChar, n));
    req.input('Pending', sql.NVarChar, PR_LINE_STATUS.PENDING);
    let linesResult = await req.query(`
      SELECT d.DetailID, d.PRID, d.PRNumber, d.ProjectCode, d.ProductNo, d.ItemCode,
             d.ItemDescription, d.Specification, d.Make, d.MfgPartNo, d.Quantity, d.UOM,
             d.UnitCost, d.TotalCost, d.VendorCode, d.VendorName, d.LineStatus,
             pr.VendorCode AS HdrVendor, pr.VendorName AS HdrVendorName, pr.ProjectCode AS HdrProject
      FROM PurchaseRequestDetailNew d
      JOIN PurchaseRequest pr ON pr.PRNumber = d.PRNumber
      WHERE d.PRNumber IN (${placeholders})
        AND ISNULL(d.LineStatus, 'Pending') = @Pending
    `);
    let lines = linesResult.recordset;
    if (lineIds?.length) {
      const set = new Set(lineIds.map(Number));
      lines = lines.filter((l) => set.has(Number(l.DetailID)));
    }
    if (!lines.length) throw new Error('No pending lines to convert');

    const byVendor = new Map();
    for (const line of lines) {
      const key = line.VendorCode || line.HdrVendor;
      if (!byVendor.has(key)) byVendor.set(key, []);
      byVendor.get(key).push(line);
    }

    const poRefs = [];
    const name = user.displayName || user.username;
    const today = new Date().toISOString().slice(0, 10);

    for (const [, vendorLines] of byVendor) {
      const poRef = await nextPoRef(tx);
      poRefs.push(poRef);
      for (const line of vendorLines) {
        const amount = Number(line.TotalCost) || Number(line.Quantity) * Number(line.UnitCost);
        const ins = new sql.Request(tx);
        ins.input('DBOMNo', sql.NVarChar, poRef);
        ins.input('ProjectCode', sql.NVarChar, line.ProjectCode || line.HdrProject);
        ins.input('VendorCode', sql.NVarChar, line.VendorCode || line.HdrVendor);
        ins.input('PreparedBy', sql.NVarChar, name);
        ins.input('ItemCode', sql.NVarChar, line.ItemCode);
        ins.input('UOM', sql.NVarChar, line.UOM || 'Nos');
        ins.input('RequariedQty', sql.Float, line.Quantity);
        ins.input('UnitPrice', sql.Float, line.UnitCost);
        ins.input('Amount', sql.Float, amount);
        ins.input('RemainingQty', sql.Float, line.Quantity);
        ins.input('Remarks', sql.NVarChar, `From PR ${line.PRNumber}`);
        ins.input('POPreparedDate', sql.NVarChar, today);
        ins.input('Currency', sql.NVarChar, 'INR');
        ins.input('FXRate', sql.Float, 1);
        await ins.query(`
          INSERT INTO PurchaseOrder
            (DBOMNo, ProjectCode, VendorCode, PreparedBy, ItemCode, UOM, RequariedQty,
             UnitPrice, Amount, RemainingQty, Remarks, POPreparedDate, POApproved, Currency, FXRate)
          VALUES
            (@DBOMNo, @ProjectCode, @VendorCode, @PreparedBy, @ItemCode, @UOM, @RequariedQty,
             @UnitPrice, @Amount, @RemainingQty, @Remarks, @POPreparedDate, 0, @Currency, @FXRate)
        `);

        const upd = new sql.Request(tx);
        upd.input('DetailID', sql.Int, line.DetailID);
        upd.input('LineStatus', sql.NVarChar, PR_LINE_STATUS.CONVERTED_TO_PO);
        await upd.query(`
          UPDATE PurchaseRequestDetailNew
          SET LineStatus = @LineStatus, ModifiedDate = GETDATE()
          WHERE DetailID = @DetailID
        `);
      }

      for (const prNumber of new Set(vendorLines.map((l) => l.PRNumber))) {
        const st = new sql.Request(tx);
        st.input('PRNumber', sql.NVarChar, prNumber);
        st.input('Pending', sql.NVarChar, PR_LINE_STATUS.PENDING);
        const counts = await st.query(`
          SELECT
            SUM(CASE WHEN ISNULL(LineStatus,'Pending') = @Pending THEN 1 ELSE 0 END) AS PendingLines,
            COUNT(*) AS TotalLines
          FROM PurchaseRequestDetailNew WHERE PRNumber = @PRNumber
        `);
        const pending = Number(counts.recordset[0]?.PendingLines || 0);
        const total = Number(counts.recordset[0]?.TotalLines || 0);
        const status =
          total > 0 && pending === 0
            ? PR_STATUS.FULLY_CONVERTED
            : pending < total
              ? PR_STATUS.PARTIALLY_CONVERTED
              : PR_STATUS.APPROVED;
        const up = new sql.Request(tx);
        up.input('Status', sql.NVarChar, status);
        up.input('PRNumber', sql.NVarChar, prNumber);
        await up.query(`
          UPDATE PurchaseRequest SET Status = @Status, ModifiedDate = GETDATE()
          WHERE PRNumber = @PRNumber
        `);
      }
    }

    await tx.commit();
    return { poRefs };
  } catch (err) {
    await tx.rollback();
    throw err;
  }
}

async function updatePoByRef(poRef, setSql, params) {
  const pool = await getMssqlPool();
  const req = pool.request();
  req.input('PORef', sql.NVarChar, poRef);
  for (const [k, v] of Object.entries(params)) {
    req.input(k, v);
  }
  await req.query(`UPDATE PurchaseOrder SET ${setSql} WHERE DBOMNo = @PORef`);
}

export async function approvePO(poRef, { step, action, remarks, user }) {
  const name = user.displayName || user.username;
  const lines = (
    await mssqlQuery(`SELECT * FROM PurchaseOrder WHERE DBOMNo = @PORef`, { PORef: poRef })
  ).recordset;
  if (!lines.length) throw new Error('PO not found');

  const mapped = lines.map((r) => ({
    amount: r.Amount,
    igst_amount: r.IGSTAmount,
    sgst_amount: r.SGSTAmount,
    cgst_amount: r.CGSTAmount,
  }));
  const amounts = computePoAmounts(mapped);
  const first = lines[0];
  const approvalStep =
    step ||
    (action === 'reject'
      ? 'reject'
      : action === 'approve'
        ? getNextPoApprovalStep({
            pmApproved: !!first.PMApproved,
            mhApproved: !!first.MHApproved,
            pcApproved: !!(first.PurchaseCommitee || first.PCAuthoriseDate),
            omApproved: !!first.OMApproved,
            gmApproved: !!first.GMCostApproved,
            approvalTier: amounts.approvalTier,
          })
        : null);
  if (!approvalStep) throw new Error('PO has no pending approval step');
  if (first.POApproved && !['generate', 'send'].includes(approvalStep)) {
    throw new Error('PO already fully approved');
  }

  if (approvalStep === 'reject') {
    await updatePoByRef(
      poRef,
      `RejectReason = @Reason, Remarks = ISNULL(Remarks,'') + ' | Rejected by ' + @ByUser`,
      { Reason: remarks || 'Rejected', ByUser: name }
    );
  } else if (approvalStep === 'pm') {
    await updatePoByRef(
      poRef,
      `PMName = @ByUser, PMApproved = 1, PMApprovedDate = CONVERT(NVARCHAR(20), GETDATE(), 103), PMRemarks = @Remarks`,
      { ByUser: name, Remarks: remarks || '' }
    );
  } else if (approvalStep === 'mh') {
    if (!first.PMApproved) throw new Error('PM approval required first');
    await updatePoByRef(
      poRef,
      `MHName = @ByUser, MHApproved = 1, MHApprovedDate = CONVERT(NVARCHAR(20), GETDATE(), 103), PMMHRemarks = @Remarks`,
      { ByUser: name, Remarks: remarks || '' }
    );
  } else if (approvalStep === 'pc') {
    if (!first.PMApproved || !first.MHApproved) throw new Error('PM and MH approval required first');
    const finalize = amounts.approvalTier === 'PC';
    await updatePoByRef(
      poRef,
      `PurchaseCommitee = @ByUser, PCRemarks = @Remarks, PCAuthoriseDate = CONVERT(NVARCHAR(64), GETDATE(), 120)
       ${finalize ? ', POApproved = 1, AuthoriedBy = @ByUser, POAuthoriseDate = CONVERT(NVARCHAR(64), GETDATE(), 120)' : ''}`,
      { ByUser: name, Remarks: remarks || '' }
    );
  } else if (approvalStep === 'om') {
    if (!(first.PurchaseCommitee || first.PCAuthoriseDate)) {
      throw new Error('Purchase Committee approval required first');
    }
    if (amounts.approvalTier === 'PC') throw new Error('OM not required for this PO amount');
    const finalize = amounts.approvalTier === 'OM';
    await updatePoByRef(
      poRef,
      `OMName = @ByUser, OMApproved = 1, OMApprovedDate = CONVERT(NVARCHAR(20), GETDATE(), 103), OMRemarks = @Remarks
       ${finalize ? ', POApproved = 1, AuthoriedBy = @ByUser, POAuthoriseDate = CONVERT(NVARCHAR(64), GETDATE(), 120)' : ''}`,
      { ByUser: name, Remarks: remarks || '' }
    );
  } else if (approvalStep === 'gm') {
    if (amounts.approvalTier !== 'GM') throw new Error('GM not required for this PO amount');
    if (!first.OMApproved) throw new Error('OM approval required first');
    await updatePoByRef(
      poRef,
      `GMName = @ByUser, GMCostApproved = 1, GMApprovedDate = CONVERT(NVARCHAR(20), GETDATE(), 103),
       POApproved = 1, AuthoriedBy = @ByUser, POAuthoriseDate = CONVERT(NVARCHAR(64), GETDATE(), 120)`,
      { ByUser: name }
    );
  } else if (approvalStep === 'generate') {
    if (!first.POApproved) throw new Error('PO must be fully approved before generate');
    if (first.POGeneratedBy) throw new Error('PO is already generated');
    await updatePoByRef(
      poRef,
      `POGeneratedBy = @ByUser, POGeneratedDate = CONVERT(NVARCHAR(64), GETDATE(), 120)`,
      { ByUser: name }
    );
  } else if (approvalStep === 'send') {
    if (!first.POGeneratedBy) throw new Error('Generate PO first');
    if (first.POSenttoVendorBy) throw new Error('PO is already sent to the vendor');
    await updatePoByRef(
      poRef,
      `POSenttoVendorBy = @ByUser, POSentVendorDate = CONVERT(NVARCHAR(64), GETDATE(), 120)`,
      { ByUser: name }
    );
  } else {
    throw new Error('Unknown approval step');
  }

  const { listPurchaseOrders } = await import('./mssqlLegacyService.js');
  const data = await listPurchaseOrders({ poNumber: poRef });
  return data.items.find((p) => p.poRef === poRef) || data.items[0];
}

export async function createGRN({
  poRef,
  vendorCode,
  projectCode,
  invoiceNo,
  remarks,
  lines,
  user,
}) {
  if (!lines?.length) throw new Error('No lines to receive');
  const normalizedLines = lines
    .map((line) => ({
      ...line,
      poId: line.poId ?? line.detailId ?? line.id,
      receivedQty: Number(line.receivedQty ?? line.quantity ?? 0),
    }))
    .filter((line) => line.receivedQty > 0);
  if (!normalizedLines.length) throw new Error('At least one positive receipt quantity is required');
  if (normalizedLines.some((line) => !line.poId)) {
    throw new Error('Every GRN line requires a PO line identifier');
  }

  const pool = await getMssqlPool();
  const tx = new sql.Transaction(pool);
  await tx.begin();
  try {
    const firstPoReq = new sql.Request(tx);
    firstPoReq.input('POID', sql.Int, normalizedLines[0].poId);
    const firstPoResult = await firstPoReq.query(
      `SELECT * FROM PurchaseOrder WHERE ID = @POID`
    );
    const firstPo = firstPoResult.recordset[0];
    if (!firstPo) throw new Error(`PO line ${normalizedLines[0].poId} not found`);
    const resolvedPoRef = poRef || firstPo.DBOMNo;
    const resolvedVendorCode = vendorCode || firstPo.VendorCode;
    const resolvedProjectCode = projectCode || firstPo.ProjectCode;

    const now = new Date();
    const prefix = `GRN-${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-`;
    const nreq = new sql.Request(tx);
    nreq.input('Prefix', sql.NVarChar, prefix);
    const last = await nreq.query(
      `SELECT MAX(GRNNumber) AS lastNo FROM ProcurementGRN WHERE GRNNumber LIKE @Prefix + '%'`
    );
    let seq = 1;
    if (last.recordset[0]?.lastNo) {
      const p = String(last.recordset[0].lastNo).split('-');
      seq = (parseInt(p[p.length - 1], 10) || 0) + 1;
    }
    const grnNumber = `${prefix}${String(seq).padStart(5, '0')}`;
    const name = user.displayName || user.username;

    const hdr = new sql.Request(tx);
    hdr.input('GRN', sql.NVarChar, grnNumber);
    hdr.input('PORef', sql.NVarChar, resolvedPoRef);
    hdr.input('Vendor', sql.NVarChar, resolvedVendorCode || null);
    hdr.input('Project', sql.NVarChar, resolvedProjectCode || null);
    hdr.input('InvoiceNo', sql.NVarChar, invoiceNo || null);
    hdr.input('By', sql.NVarChar, name);
    hdr.input('Remarks', sql.NVarChar, remarks || null);
    const inserted = await hdr.query(`
      INSERT INTO ProcurementGRN
        (GRNNumber, PORef, VendorCode, ProjectCode, InvoiceNo, ReceivedBy, Remarks, CreatedBy, Status)
      VALUES
        (@GRN, @PORef, @Vendor, @Project, @InvoiceNo, @By, @Remarks, @By, 'Received');
      SELECT CAST(SCOPE_IDENTITY() AS INT) AS id;
    `);
    const grnId = inserted.recordset[0].id;

    for (const line of normalizedLines) {
      const recvQty = line.receivedQty;
      const poReq = new sql.Request(tx);
      poReq.input('POID', sql.Int, line.poId);
      const poRow = await poReq.query(`SELECT * FROM PurchaseOrder WHERE ID = @POID`);
      const po = poRow.recordset[0];
      if (!po) throw new Error(`PO line ${line.poId} not found`);
      if (po.DBOMNo !== resolvedPoRef) {
        throw new Error('A GRN can only receive lines from one purchase order');
      }
      const remaining = Number(po.RemainingQty ?? po.RequariedQty ?? 0);
      if (recvQty > remaining) {
        throw new Error(`Received qty exceeds remaining for ${po.ItemCode}`);
      }

      const d = new sql.Request(tx);
      d.input('GRNID', sql.Int, grnId);
      d.input('GRN', sql.NVarChar, grnNumber);
      d.input('POID', sql.Int, po.ID);
      d.input('Item', sql.NVarChar, po.ItemCode);
      d.input('Ordered', sql.Decimal(18, 3), po.RequariedQty);
      d.input('Recv', sql.Decimal(18, 3), recvQty);
      d.input('Price', sql.Decimal(18, 2), po.UnitPrice);
      d.input('Amount', sql.Decimal(18, 2), +(recvQty * Number(po.UnitPrice)).toFixed(2));
      d.input('UOM', sql.NVarChar, po.UOM);
      d.input('Remarks', sql.NVarChar, line.remarks || null);
      await d.query(`
        INSERT INTO ProcurementGRNDetail
          (GRNID, GRNNumber, POID, ItemCode, OrderedQty, ReceivedQty, UnitPrice, Amount, UOM, Remarks)
        VALUES
          (@GRNID, @GRN, @POID, @Item, @Ordered, @Recv, @Price, @Amount, @UOM, @Remarks)
      `);

      const up = new sql.Request(tx);
      up.input('Recv', sql.Float, recvQty);
      up.input('POID', sql.Int, po.ID);
      await up.query(`
        UPDATE PurchaseOrder
        SET RemainingQty = CASE
              WHEN ISNULL(RemainingQty, RequariedQty) - @Recv < 0 THEN 0
              ELSE ISNULL(RemainingQty, RequariedQty) - @Recv
            END
        WHERE ID = @POID
      `);
    }

    await tx.commit();
    return {
      grnNumber,
      poRef: resolvedPoRef,
      invoiceNo: invoiceNo || null,
      status: 'Received',
    };
  } catch (err) {
    await tx.rollback();
    // If ProcurementGRN table missing, surface clear message
    if (/Invalid object name|Invalid column name 'InvoiceNo'/i.test(err.message)) {
      throw new Error(
        `${err.message}. Run apps/api/sql/Phase3_Schema_Alignment.sql on ERP_Database first.`
      );
    }
    throw err;
  }
}

export async function listOpenPOLines() {
  const result = await mssqlQuery(`
    SELECT
      ID AS id,
      DBOMNo AS poRef,
      ProjectCode AS projectCode,
      VendorCode AS vendorCode,
      ItemCode AS itemCode,
      UOM AS uom,
      RequariedQty AS orderedQty,
      ISNULL(RemainingQty, RequariedQty) AS remainingQty,
      UnitPrice AS unitPrice,
      Amount AS amount
    FROM PurchaseOrder
    WHERE POApproved = 1
      AND ISNULL(RemainingQty, RequariedQty) > 0
    ORDER BY ID DESC
  `);
  return result.recordset;
}
