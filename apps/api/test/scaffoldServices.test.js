import assert from 'node:assert/strict';
import { rmSync } from 'node:fs';
import { tmpdir } from 'node:os';
import path from 'node:path';
import test from 'node:test';
import { closeDb } from '../src/db/connection.js';
import { requireSqliteMode } from '../src/middleware/dbMode.js';
import * as gateEntryService from '../src/services/gateEntryService.js';
import * as masterService from '../src/services/masterService.js';
import {
  groupLegacyGateRows,
  validateLegacyGatePayload,
} from '../src/services/mssqlGateEntryService.js';
import * as salesService from '../src/services/salesService.js';

const dbPath = path.join(tmpdir(), `biss-erp-scaffold-${process.pid}.sqlite`);
process.env.DB_PATH = dbPath;
process.env.DB_CLIENT = 'sqlite';

const user = { username: 'test-user', displayName: 'Test User' };

test.after(() => {
  closeDb();
  rmSync(dbPath, { force: true });
});

test('project installation fields persist through the project service', () => {
  masterService.upsertProject(
    {
      projectCode: 'PRJ-TEST',
      projectName: 'Test project',
      customerCode: 'CUST-1',
      installationStatus: 'In Progress',
      shipmentDate: '2026-08-11',
    },
    user
  );

  const project = masterService.getProject('PRJ-TEST');
  assert.equal(project.customerCode, 'CUST-1');
  assert.equal(project.installationStatus, 'In Progress');
  assert.equal(project.shipmentDate, '2026-08-11');
});

test('gate lists isolate inward, outward, and manual records', () => {
  const inward = gateEntryService.createGateEntry(
    {
      entryType: 'Inward',
      documentType: 'Purchase Order',
      documentNo: 'PO-1',
      vendorName: 'A',
      invoiceNo: 'INV-1',
      lines: [{ itemCode: 'I-1', itemDescription: 'Inward item', quantity: 2 }],
    },
    user
  );
  gateEntryService.createGateEntry(
    {
      entryType: 'Outward',
      documentType: 'Delivery Challan',
      documentNo: 'DC-1',
      vendorName: 'B',
      vehicleNo: 'KA01AB1234',
      lrPodNo: 'LR-1',
      lrPodDate: '2026-08-11',
      lines: [{ itemCode: 'I-2', itemDescription: 'Outward item', quantity: 3 }],
    },
    user
  );
  gateEntryService.createGateEntry(
    {
      entryType: 'Manual',
      documentType: 'Manual',
      documentNo: 'MAN-1',
      vendorName: 'C',
      lines: [{ itemDescription: 'Manual item', quantity: 1 }],
    },
    user
  );

  assert.equal(gateEntryService.listGateEntries({ type: 'Inward' }).length, 1);
  assert.equal(gateEntryService.listGateEntries({ type: 'Outward' }).length, 1);
  assert.equal(gateEntryService.listGateEntries({ type: 'Manual' }).length, 1);
  assert.equal(inward.lineCount, 1);
  assert.equal(inward.totalQuantity, 2);
  assert.equal(inward.documentNo, 'PO-1');
});

test('legacy gate adapter groups multi-line SI records without changing stock', () => {
  const rows = [
    {
      id: 1,
      entryNumber: 'SI20200001',
      entryType: 'Inward',
      documentType: 'Purchase Order',
      documentNo: 'PO-1',
      itemCode: 'I-1',
      itemDescription: 'First',
      quantity: 2,
      vendorName: 'Vendor',
    },
    {
      id: 2,
      entryNumber: 'SI20200001',
      entryType: 'Inward',
      documentType: 'Purchase Order',
      documentNo: 'PO-1',
      itemCode: 'I-2',
      itemDescription: 'Second',
      quantity: 3,
      vendorName: 'Vendor',
    },
  ];
  const grouped = groupLegacyGateRows(rows);
  assert.equal(grouped.length, 1);
  assert.equal(grouped[0].lineCount, 2);
  assert.equal(grouped[0].totalQuantity, 5);
  assert.equal(grouped[0].status, 'Recorded');

  assert.throws(
    () =>
      validateLegacyGatePayload({
        entryType: 'Outward',
        documentType: 'Delivery Challan',
        documentNo: 'DC-1',
        vendorName: 'Vendor',
        lines: [{ itemDescription: 'Item', quantity: 1 }],
      }),
    /Vehicle number/
  );
});

test('quote amount can be edited without replacing line items', () => {
  const quote = salesService.createQuote(
    { customerName: 'Customer', totalAmount: 1250 },
    user
  );
  const updated = salesService.updateQuote(
    quote.quoteNumber,
    { totalAmount: 1750 },
    user
  );

  assert.equal(updated.totalAmount, 1750);
});

test('unmapped routes reject SQL Server mode instead of opening SQLite', () => {
  process.env.DB_CLIENT = 'mssql';
  let status;
  let payload;
  let continued = false;
  const response = {
    status(code) {
      status = code;
      return this;
    },
    json(body) {
      payload = body;
      return this;
    },
  };

  try {
    requireSqliteMode(
      { baseUrl: '/api/stock', path: '/' },
      response,
      () => {
        continued = true;
      }
    );
  } finally {
    process.env.DB_CLIENT = 'sqlite';
  }
  assert.equal(continued, false);
  assert.equal(status, 501);
  assert.equal(payload.code, 'MSSQL_WORKFLOW_NOT_MAPPED');
});
