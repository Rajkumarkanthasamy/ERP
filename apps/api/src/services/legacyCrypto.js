import {
  createCipheriv,
  createDecipheriv,
  pbkdf2Sync,
  timingSafeEqual,
} from 'node:crypto';

// Must stay byte-for-byte compatible with Code/Cryptography.cs.
const LEGACY_KEY = '0ERP@2023xxxxxxxxxxtttttuuuuuiiiiio';
const LEGACY_SALT = Buffer.from([
  0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76,
]);

function deriveKeyAndIv() {
  // .NET Framework Rfc2898DeriveBytes(string, byte[]) defaults to
  // PBKDF2-HMAC-SHA1 with 1,000 iterations. Consecutive GetBytes(32) and
  // GetBytes(16) calls are equivalent to deriving 48 bytes once.
  const material = pbkdf2Sync(LEGACY_KEY, LEGACY_SALT, 1000, 48, 'sha1');
  return {
    key: material.subarray(0, 32),
    iv: material.subarray(32, 48),
  };
}

export function encryptLegacyPassword(password) {
  const { key, iv } = deriveKeyAndIv();
  const cipher = createCipheriv('aes-256-cbc', key, iv);
  // Encoding.Unicode in C# is UTF-16 little endian.
  const encrypted = Buffer.concat([
    cipher.update(Buffer.from(String(password), 'utf16le')),
    cipher.final(),
  ]);
  return encrypted.toString('base64');
}

export function decryptLegacyPassword(cipherText) {
  const { key, iv } = deriveKeyAndIv();
  const decipher = createDecipheriv('aes-256-cbc', key, iv);
  const encrypted = Buffer.from(String(cipherText).replace(/ /g, '+'), 'base64');
  return Buffer.concat([decipher.update(encrypted), decipher.final()]).toString('utf16le');
}

export function legacyPasswordMatches(password, storedPassword) {
  if (typeof storedPassword !== 'string') return false;
  const encrypted = encryptLegacyPassword(password);
  const expected = Buffer.from(storedPassword);
  const actual = Buffer.from(encrypted);
  if (expected.length === actual.length && timingSafeEqual(expected, actual)) return true;

  // Some old/test databases contain plain values. Keep compatibility without
  // exposing which storage format was used.
  const plain = Buffer.from(String(password));
  return expected.length === plain.length && timingSafeEqual(expected, plain);
}

export function validateLegacyPasswordPolicy(password) {
  const value = String(password || '');
  return (
    value.length === 12 &&
    /[A-Z]/.test(value) &&
    /[a-z]/.test(value) &&
    /\d/.test(value) &&
    /[^A-Za-z0-9]/.test(value)
  );
}
