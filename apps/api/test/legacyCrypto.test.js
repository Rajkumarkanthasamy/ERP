import assert from 'node:assert/strict';
import test from 'node:test';
import {
  decryptLegacyPassword,
  encryptLegacyPassword,
  legacyPasswordMatches,
  validateLegacyPasswordPolicy,
} from '../src/services/legacyCrypto.js';

test('legacy AES format is deterministic and reversible', () => {
  const password = 'Abcdef1!2345';
  const encrypted = encryptLegacyPassword(password);

  assert.equal(encrypted, 'brIo7ozcwHbY8SDvd0qpHFJKFnUihge1gzq/mJGpffI=');
  assert.equal(decryptLegacyPassword(encrypted), password);
  assert.equal(legacyPasswordMatches(password, encrypted), true);
  assert.equal(legacyPasswordMatches('WrongPass1!2', encrypted), false);
});

test('legacy password policy matches the WinForms validation', () => {
  assert.equal(validateLegacyPasswordPolicy('Abcdef1!2345'), true);
  assert.equal(validateLegacyPasswordPolicy('abcdef1!2345'), false);
  assert.equal(validateLegacyPasswordPolicy('Abcdefghijkl'), false);
  assert.equal(validateLegacyPasswordPolicy('Ab1!'), false);
});
