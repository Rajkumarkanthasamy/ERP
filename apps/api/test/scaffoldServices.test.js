import assert from 'node:assert/strict';
import { rmSync } from 'node:fs';
import { tmpdir } from 'node:os';
import path from 'node:path';
import test from 'node:test';
import { closeDb } from '../src/db/connection.js';
import { requireSqliteMode } from '../src/middleware/dbMode.js';
import * as gateEntryService from '../src/services/gateEntryService.js';
import * as masterService from '../src/services/masterService.js';
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
  gateEntryService.createGateEntry({ entryType: 'Inward', vendorName: 'A' }, user);
  gateEntryService.createGateEntry({ entryType: 'Outward', vendorName: 'B' }, user);
  gateEntryService.createGateEntry({ entryType: 'Manual', vendorName: 'C' }, user);

  assert.equal(gateEntryService.listGateEntries({ type: 'Inward' }).length, 1);
  assert.equal(gateEntryService.listGateEntries({ type: 'Outward' }).length, 1);
  assert.equal(gateEntryService.listGateEntries({ type: 'Manual' }).length, 1);
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
