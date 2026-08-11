import dayjs from 'dayjs';

export function formatINR(value) {
  const n = Number(value || 0);
  return new Intl.NumberFormat('en-IN', {
    style: 'currency',
    currency: 'INR',
    maximumFractionDigits: 2,
  }).format(n);
}

export function formatDate(value, fallback = '—') {
  if (!value) return fallback;
  const d = dayjs(value);
  return d.isValid() ? d.format('DD MMM YYYY') : fallback;
}

export function formatDateTime(value, fallback = '—') {
  if (!value) return fallback;
  const d = dayjs(value);
  return d.isValid() ? d.format('DD MMM YYYY HH:mm') : fallback;
}

export function pick(obj, keys) {
  const out = {};
  keys.forEach((k) => {
    if (obj[k] !== undefined) out[k] = obj[k];
  });
  return out;
}

export function statusTone(status = '') {
  const s = String(status).toLowerCase();
  if (['approved', 'active', 'closed', 'fully received', 'completed', 'done', 'resolved'].some((x) => s.includes(x))) {
    return 'success';
  }
  if (['reject', 'cancel', 'fail', 'escalat', 'nc'].some((x) => s.includes(x))) {
    return 'error';
  }
  if (['hold', 'pending', 'await', 'open', 'draft', 'partial'].some((x) => s.includes(x))) {
    return 'warning';
  }
  return 'default';
}
