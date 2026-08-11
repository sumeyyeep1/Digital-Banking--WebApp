import { createContext, useContext, useMemo, useState } from 'react';
import type { ReactNode } from 'react';
import type { LoginResponse } from '../types/banking';

interface AuthContextValue {
  auth: LoginResponse | null;
  token: string | null;
  setAuth: (auth: LoginResponse) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [auth, setAuthState] = useState<LoginResponse | null>(() => {
    const saved = sessionStorage.getItem('digital-bank-auth');
    return saved ? (JSON.parse(saved) as LoginResponse) : null;
  });

  const setAuth = (nextAuth: LoginResponse) => {
    setAuthState(nextAuth);
    sessionStorage.setItem('digital-bank-auth', JSON.stringify(nextAuth));
  };

  const logout = () => {
    setAuthState(null);
    sessionStorage.removeItem('digital-bank-auth');
  };

  const value = useMemo(() => ({ auth, token: auth?.token ?? null, setAuth, logout }), [auth]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const value = useContext(AuthContext);
  if (!value) throw new Error('useAuth must be used inside AuthProvider');
  return value;
}
