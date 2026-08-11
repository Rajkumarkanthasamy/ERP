import assert from 'node:assert/strict';
import test from 'node:test';
import {
  asActiveStatus,
  asPinCode,
  splitAddress,
} from '../src/services/mssqlMasterService.js';

test('master payload helpers normalize status, pin codes and long addresses', () => {
  assert.equal(asActiveStatus(0), 'Inactive');
  assert.equal(asActiveStatus('inactive'), 'Inactive');
  assert.equal(asActiveStatus(1), 'Active');
  assert.equal(asPinCode('641001'), 641001);
  assert.equal(asPinCode(''), null);
  assert.throws(() => asPinCode('AB12'), /numeric/);

  const short = splitAddress('Line 1');
  assert.equal(short.address1, 'Line 1');
  assert.equal(short.address2, null);

  const long = 'x'.repeat(300);
  const split = splitAddress(long);
  assert.equal(split.address1.length, 255);
  assert.equal(split.address2.length, 45);
});
