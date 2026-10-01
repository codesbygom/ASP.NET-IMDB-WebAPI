"use client";

import { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react";
import type { NewUser } from "./types";

const TOKEN = "imdb_token";

export class ApiError extends Error {
  constructor(public status: number, public messages: string[]) {
    super(messages[0] ?? `Request failed with ${status}`);
  }
}

function read(): string | null {
  try {
    return localStorage.getItem(TOKEN);
  } catch {
    return null;
  }
}

function write(value: string | null) {
  try {
    if (value) localStorage.setItem(TOKEN, value);
    else localStorage.removeItem(TOKEN);
  } catch {
    /* storage blocked: the session just won't survive a reload */
  }
}

// Decodes the JWT payload (not verified: the API verifies it on every call).
function claims(token: string | null): Record<string, unknown> {
  if (!token) return {};
  try {
    return JSON.parse(atob(token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/")));
  } catch {
    return {};
  }
}

const ROLE_KEYS = ["role", "roles", "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
const ID_KEYS = ["nameid", "sub", "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];

function pick(c: Record<string, unknown>, keys: string[]): unknown {
  for (const k of keys) if (c[k] !== undefined) return c[k];
  return undefined;
}

function messagesFrom(body: unknown): string[] {
  const b = body as { errors?: unknown; message?: string; title?: string } | null;
  if (!b) return ["Request failed."];
  if (Array.isArray(b.errors) && b.errors.length) return b.errors.map(String);
  if (b.errors && typeof b.errors === "object") return Object.values(b.errors as Record<string, string[]>).flat();
  return [b.message ?? b.title ?? "Request failed."];
}

// Calls the .NET API through the Next proxy and unwraps { success, data }.
export async function api<T = unknown>(path: string, init: RequestInit = {}): Promise<T> {
  const headers = new Headers(init.headers);
  const token = read();
  if (token) headers.set("Authorization", `Bearer ${token}`);
  if (init.body) headers.set("Content-Type", "application/json");
  const res = await fetch(`/api/${path}`, { ...init, headers });
  const text = await res.text();
  const body = text ? JSON.parse(text) : null;
  if (!res.ok) throw new ApiError(res.status, messagesFrom(body));
  return (body?.data ?? null) as T;
}

interface Session {
  ready: boolean;
  userName: string | null;
  userId: string | null;
  isAdmin: boolean;
  login: (userName: string, password: string) => Promise<void>;
  register: (userName: string, emailAddress: string, password: string) => Promise<void>;
  logout: () => void;
}

const Ctx = createContext<Session | null>(null);

export function SessionProvider({ children }: { children: React.ReactNode }) {
  const [ready, setReady] = useState(false);
  const [token, setToken] = useState<string | null>(null);
  const [userName, setUserName] = useState<string | null>(null);

  useEffect(() => {
    setToken(read());
    try {
      setUserName(localStorage.getItem("imdb_user"));
    } catch { /* ignore */ }
    setReady(true);
  }, []);

  const accept = (user: NewUser) => {
    write(user.token);
    setToken(user.token);
    setUserName(user.userName);
    try {
      localStorage.setItem("imdb_user", user.userName);
    } catch { /* ignore */ }
  };

  const logout = useCallback(() => {
    write(null);
    setToken(null);
    setUserName(null);
  }, []);

  const value = useMemo<Session>(() => {
    const c = claims(token);
    const roles = pick(c, ROLE_KEYS);
    const list = Array.isArray(roles) ? roles : roles ? [roles] : [];
    return {
      ready,
      userName: token ? userName : null,
      userId: (pick(c, ID_KEYS) as string | undefined) ?? null,
      isAdmin: list.includes("Admin"),
      login: async (u, p) => accept(await api<NewUser>("Account/login", { method: "POST", body: JSON.stringify({ userName: u, password: p }) })),
      register: async (u, e, p) => accept(await api<NewUser>("Account/register", { method: "POST", body: JSON.stringify({ username: u, emailAddress: e, password: p }) })),
      logout,
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [ready, token, userName, logout]);

  return <Ctx.Provider value={value}>{children}</Ctx.Provider>;
}

export function useSession(): Session {
  const s = useContext(Ctx);
  if (!s) throw new Error("useSession must be used inside <SessionProvider>");
  return s;
}
