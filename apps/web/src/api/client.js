const TOKEN_KEY = 'biss_erp_token';
const USER_KEY = 'biss_erp_user';

export function getToken() {
  return localStorage.getItem(TOKEN_KEY);
}

export function setAuthSession({ token, user }) {
  if (token) localStorage.setItem(TOKEN_KEY, token);
  if (user) localStorage.setItem(USER_KEY, JSON.stringify(user));
}

export function clearAuthSession() {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(USER_KEY);
}

export function getStoredUser() {
  try {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) : null;
  } catch {
    return null;
  }
}

const BASE_URL = import.meta.env.VITE_API_URL || '/api';

function buildUrl(path, query) {
  const normalized = path.startsWith('http')
    ? path
    : `${BASE_URL}${path.startsWith('/') ? path : `/${path}`}`;
  if (!query || typeof query !== 'object') return normalized;
  const params = new URLSearchParams();
  Object.entries(query).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== '') {
      params.set(key, String(value));
    }
  });
  const qs = params.toString();
  return qs ? `${normalized}?${qs}` : normalized;
}

export class ApiError extends Error {
  constructor(message, { status, data } = {}) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.data = data;
  }
}

export async function api(path, options = {}) {
  const {
    method = 'GET',
    body,
    query,
    headers = {},
    auth = true,
    formData = false,
  } = options;

  const finalHeaders = { ...headers };
  if (auth) {
    const token = getToken();
    if (token) finalHeaders.Authorization = `Bearer ${token}`;
  }
  if (body && !formData && !(body instanceof FormData)) {
    finalHeaders['Content-Type'] = 'application/json';
  }

  let response;
  try {
    response = await fetch(buildUrl(path, query), {
      method,
      headers: finalHeaders,
      body: body
        ? formData || body instanceof FormData
          ? body
          : JSON.stringify(body)
        : undefined,
    });
  } catch (err) {
    throw new ApiError(err.message || 'Network error', { status: 0 });
  }

  const contentType = response.headers.get('content-type') || '';
  const isJson = contentType.includes('application/json');
  const data = isJson ? await response.json().catch(() => null) : await response.text();

  if (!response.ok) {
    const message =
      (data && typeof data === 'object' && (data.error || data.message)) ||
      (typeof data === 'string' && data) ||
      `Request failed (${response.status})`;
    if (response.status === 401 && auth) {
      clearAuthSession();
    }
    throw new ApiError(message, { status: response.status, data });
  }

  return data;
}

export const apiGet = (path, query, options) => api(path, { ...options, method: 'GET', query });
export const apiPost = (path, body, options) => api(path, { ...options, method: 'POST', body });
export const apiPut = (path, body, options) => api(path, { ...options, method: 'PUT', body });
export const apiPatch = (path, body, options) => api(path, { ...options, method: 'PATCH', body });
export const apiDelete = (path, options) => api(path, { ...options, method: 'DELETE' });

/** Try multiple candidate paths until one succeeds (for parallel backend evolution). */
export async function apiGetFirst(paths, query, options) {
  let lastError;
  for (const path of paths) {
    try {
      return await apiGet(path, query, options);
    } catch (err) {
      lastError = err;
      if (err.status && err.status !== 404) throw err;
    }
  }
  throw lastError || new ApiError('No endpoint available', { status: 404 });
}

export function normalizeList(data) {
  if (Array.isArray(data)) return data;
  if (!data || typeof data !== 'object') return [];
  if (Array.isArray(data.items)) return data.items;
  if (Array.isArray(data.data)) return data.data;
  if (Array.isArray(data.rows)) return data.rows;
  if (Array.isArray(data.results)) return data.results;
  return [];
}
