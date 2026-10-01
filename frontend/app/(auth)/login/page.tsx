"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { ApiError, useSession } from "@/lib/api";

export default function LoginPage() {
  const router = useRouter();
  const { login } = useSession();
  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await login(userName, password);
      router.replace("/");
    } catch (err) {
      setError(err instanceof ApiError ? err.messages.join(" ") : "Could not reach the server.");
    }
  };

  return (
    <div className="auth">
      <div className="panel">
        <Link href="/" className="logo">IMDB</Link>
        <h1>Sign in</h1>
        <form className="form" onSubmit={submit}>
          {error && <div className="notice error">{error}</div>}
          <label>Username<input required value={userName} onChange={(e) => setUserName(e.target.value)} autoComplete="username" /></label>
          <label>Password<input type="password" required value={password} onChange={(e) => setPassword(e.target.value)} autoComplete="current-password" /></label>
          <button className="btn">Sign in</button>
        </form>
        <p className="alt">New here? <Link href="/register">Create an account</Link></p>
      </div>
    </div>
  );
}
