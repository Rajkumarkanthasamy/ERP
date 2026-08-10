import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { apiGet, apiPost, clearAuthSession, getStoredUser, getToken, setAuthSession } from '../api/client';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [token, setToken] = useState(() => getToken());
  const [user, setUser] = useState(() => getStoredUser());
  const [booting, setBooting] = useState(Boolean(getToken()));

  const logout = useCallback(() => {
    clearAuthSession();
    setToken(null);
    setUser(null);
  }, []);

  const login = useCallback(async (username, password) => {
    const data = await apiPost('/auth/login', { username, password }, { auth: false });
    setAuthSession({ token: data.token, user: data.user });
    setToken(data.token);
    setUser(data.user);
    return data.user;
  }, []);

  const refreshMe = useCallback(async () => {
    if (!getToken()) {
      setBooting(false);
      return null;
    }
    try {
      const data = await apiGet('/auth/me');
      if (data?.user) {
        setUser(data.user);
        setAuthSession({ token: getToken(), user: data.user });
      }
      return data?.user || null;
    } catch {
      // Keep local session if /me is unavailable; only clear on hard 401 from api client
      return getStoredUser();
    } finally {
      setBooting(false);
    }
  }, []);

  useEffect(() => {
    refreshMe();
  }, [refreshMe]);

  const value = useMemo(
    () => ({
      token,
      user,
      isAuthenticated: Boolean(token),
      booting,
      login,
      logout,
      refreshMe,
      setUser,
    }),
    [token, user, booting, login, logout, refreshMe]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
