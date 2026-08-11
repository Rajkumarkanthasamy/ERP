import assert from 'node:assert/strict';
import { rmSync } from 'node:fs';
import { tmpdir } from 'node:os';
import path from 'node:path';
import test from 'node:test';
import { calculatePriceVariance } from '../src/constants.js';
import { closeDb, getDb } from '../src/db/connection.js';
import * as grnService from '../src/services/grnService.js';
import * as poService from '../src/services/poService.js';
import * as prService from '../src/services/prService.js';

const dbPath = path.join(tmpdir(), `biss-erp-procurement-${process.pid}.sqlite`);
process.env.DB_PATH = dbPath;
process.env.DB_CLIENT = 'sqlite';

const user = { username: 'admin', displayName: 'System Admin' };

test.after(() => {
  closeDb();
  rmSync(dbPath, { force: true });
  rmSync(`${dbPath}-shm`, { force: true });
  rmSync(`${dbPath}-wal`, { force: true });
});

test('price variance uses the configured watch and alert thresholds', () => {
  assert.deepEqual(calculatePriceVariance(104, 100), {
    current: 104,
    baseline: 100,
    variancePct: 4,
    varianceAmount: 4,
    flag: 'OK',
  });
  assert.equal(calculatePriceVariance(105, 100).flag, 'Watch');
  assert.equal(calculatePriceVariance(90, 100).flag, 'ALERT');
});

test('PR to PO to GRN contracts preserve vendor, approval step, and receipt fields', () => {
  const db = getDb();
  db.prepare(
    'INSERT INTO vendors (vendor_code, vendor_name) VALUES (?, ?)'
  ).run('V-TEST', 'Test Vendor');
  db.prepare(
    'INSERT INTO projects (project_code, project_name) VALUES (?, ?)'
  ).run('P-TEST', 'Test Project');
  db.prepare(
    'INSERT INTO items (item_code, item_description, uom) VALUES (?, ?, ?)'
  ).run('I-TEST', 'Test Item', 'NOS');

  const created = prService.createPRs({
    projectCode: 'P-TEST',
    vendorCode: 'V-TEST',
    vendorName: 'Test Vendor',
    lines: [
      {
        itemCode: 'I-TEST',
        quantity: 2,
        uom: 'NOS',
        unitCost: 100,
      },
    ],
    user,
  });
  assert.equal(created.created.length, 1);

  const prNumber = created.created[0].prNumber;
  const pr = prService.getPR(prNumber);
  assert.equal(pr.vendorCode, 'V-TEST');
  assert.equal(pr.lines[0].vendorCode, 'V-TEST');

  prService.updatePRStatus(prNumber, { action: 'approve', user });
  const converted = poService.convertPRsToPO({ prNumbers: [prNumber], user });
  const poRef = converted.poRefs[0];

  let po = poService.getPO(poRef);
  assert.equal(po.nextStep, 'pm');
  po = poService.approvePO(poRef, { step: 'pm', user });
  assert.equal(po.nextStep, 'mh');
  po = poService.approvePO(poRef, { step: 'mh', user });
  assert.equal(po.nextStep, 'pc');
  po = poService.approvePO(poRef, { step: 'pc', user });
  assert.equal(po.poApproved, true);
  assert.equal(po.status, 'Ready to Generate');

  po = poService.approvePO(poRef, { step: 'generate', user });
  assert.equal(po.status, 'PO Generated');
  po = poService.approvePO(poRef, { step: 'send', user });
  assert.equal(po.status, 'Sent to Vendor');

  const openLine = grnService.listOpenPOLines().find((line) => line.poRef === poRef);
  const grn = grnService.createGRN({
    poRef,
    vendorCode: openLine.vendorCode,
    projectCode: openLine.projectCode,
    invoiceNo: 'INV-TEST',
    lines: [{ poId: openLine.id, receivedQty: 1 }],
    user,
  });

  assert.equal(grn.invoiceNo, 'INV-TEST');
  assert.equal(grn.lines[0].receivedQty, 1);
  assert.equal(
    grnService.listOpenPOLines().find((line) => line.id === openLine.id).remainingQty,
    1
  );
});

test('GRN rejects receipt quantities above the remaining PO balance', () => {
  const openLine = grnService.listOpenPOLines()[0];
  assert.throws(
    () =>
      grnService.createGRN({
        poRef: openLine.poRef,
        lines: [{ poId: openLine.id, receivedQty: openLine.remainingQty + 1 }],
        user,
      }),
    /exceeds remaining/
  );
});
