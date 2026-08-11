import assert from 'node:assert/strict';
import test from 'node:test';
import {
  buildLegacyGinNumber,
  formatBatchCode,
  formatLegacyDate,
  receiptUnitCost,
} from '../src/services/mssqlGinBridgeService.js';

test('legacy GIN helpers match WinForms numbering and cost conversion', () => {
  assert.equal(buildLegacyGinNumber(42), 'ITWGIN42');
  assert.equal(receiptUnitCost(200, 2), 100);
  assert.equal(receiptUnitCost(50, 0), 50);

  const sample = new Date(2026, 7, 11);
  assert.equal(formatLegacyDate(sample), '11-08-2026');
  assert.equal(formatBatchCode(sample), '11082026');
});
