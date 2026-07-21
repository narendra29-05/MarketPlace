import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from "react";
import { api, setToken, type AuthResponse } from "./api";

const AUTH_KEY = "findly.auth";

export interface AuthUser {
  userId: number;
  email: string;
  firstName: string;
  lastName: string;
  role: number; // 1 Admin | 2 Vendor | 3 Buyer
  vendorId: number | null;
}

interface AuthContextValue {
  user: AuthUser | null;
  signIn: (email: string, password: string) => Promise<AuthUser>;
  register: (input: {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    role: number;
  }) => Promise<AuthUser>;
  signOut: () => void;
  refresh: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | null>(null);

function loadStored(): AuthUser | null {
  const raw = localStorage.getItem(AUTH_KEY);
  if (!raw) return null;
  try {
    const parsed = JSON.parse(raw) as { token: string; user: AuthUser };
    setToken(parsed.token);
    return parsed.user;
  } catch {
    return null;
  }
}

function store(auth: AuthResponse): AuthUser {
  const user: AuthUser = {
    userId: auth.userId,
    email: auth.email,
    firstName: auth.firstName,
    lastName: auth.lastName,
    role: auth.role,
    vendorId: auth.vendorId,
  };
  setToken(auth.token);
  localStorage.setItem(AUTH_KEY, JSON.stringify({ token: auth.token, user }));
  return user;
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(loadStored);

  const signIn = useCallback(async (email: string, password: string) => {
    const auth = await api.post<AuthResponse>("/auth/login", { email, password });
    const next = store(auth);
    setUser(next);
    return next;
  }, []);

  const register = useCallback(
    async (input: { firstName: string; lastName: string; email: string; password: string; role: number }) => {
      const auth = await api.post<AuthResponse>("/auth/register", input);
      const next = store(auth);
      setUser(next);
      return next;
    },
    [],
  );

  const signOut = useCallback(() => {
    setToken(null);
    localStorage.removeItem(AUTH_KEY);
    setUser(null);
  }, []);

  // Re-reads /auth/me — e.g. after vendor onboarding links a vendorId to the account.
  const refresh = useCallback(async () => {
    const me = await api.get<{
      id: number;
      email: string;
      firstName: string;
      lastName: string;
      role: number;
      vendorId: number | null;
    }>("/auth/me");
    setUser(current => {
      if (!current) return current;
      const next: AuthUser = {
        userId: me.id,
        email: me.email,
        firstName: me.firstName,
        lastName: me.lastName,
        role: me.role,
        vendorId: me.vendorId,
      };
      const raw = localStorage.getItem(AUTH_KEY);
      if (raw) {
        try {
          const parsed = JSON.parse(raw) as { token: string };
          localStorage.setItem(AUTH_KEY, JSON.stringify({ token: parsed.token, user: next }));
        } catch {
          /* keep session in memory only */
        }
      }
      return next;
    });
  }, []);

  const value = useMemo(() => ({ user, signIn, register, signOut, refresh }), [user, signIn, register, signOut, refresh]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used inside AuthProvider");
  return context;
}

export const ROLE_ADMIN = 1;
export const ROLE_VENDOR = 2;
export const ROLE_BUYER = 3;
