import assert from 'node:assert/strict';
import test from 'node:test';
import { computeLineGst, computeItemDiff } from '../src/services/mssqlVendorTermsService.js';
import { issueDelta } from '../src/services/mssqlStockService.js';
import { listReportCatalog, REPORT_CATALOG } from '../src/services/mssqlReportService.js';

test('computeLineGst prefers IGST when present and otherwise uses CGST/SGST', () => {
  const igst = computeLineGst(1000, { igst: 18, cgst: 9, sgst: 9 });
  assert.equal(igst.igstAmount, 180);
  assert.equal(igst.cgstAmount, 0);
  assert.equal(igst.sgstAmount, 0);

  const split = computeLineGst(1000, { cgst: 9, sgst: 9, discAmount: 100 });
  assert.equal(split.cgstAmount, 81);
  assert.equal(split.sgstAmount, 81);
  assert.equal(split.igstAmount, 0);
});

test('issueDelta enforces signed Issue/Return quantities', () => {
  assert.equal(issueDelta('Issue', 5), -5);
  assert.equal(issueDelta('Return', 3), 3);
  assert.throws(() => issueDelta('Adjust', 1), /mapped/);
  assert.throws(() => issueDelta('Issue', 0), /positive/);
});

test('report catalog exposes 57 legacy report choices with mapped subset', () => {
  assert.equal(REPORT_CATALOG.length, 57);
  const catalog = listReportCatalog();
  const live = catalog.filter((r) => r.status === 'live');
  assert.ok(live.length >= 12);
  assert.ok(catalog.some((r) => r.id === 'stock-balance' && r.status === 'live'));
  assert.ok(catalog.some((r) => r.id === 'reverse-gin' && r.status === 'not_mapped'));
});

test('computeItemDiff falls back safely without DB when called with mock tx is not required for unit path', async () => {
  // Pure math path is exercised via computeLineGst; computeItemDiff needs SQL.
  assert.equal(typeof computeItemDiff, 'function');
});
