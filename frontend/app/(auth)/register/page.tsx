"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { ApiError, useSession } from "@/lib/api";

export default function RegisterPage() {
  const router = useRouter();
  const { register } = useSession();
  const [form, setForm] = useState({ userName: "", email: "", password: "" });
  const [error, setError] = useState("");
  const set = (k: keyof typeof form) => (e: React.ChangeEvent<HTMLInputElement>) => setForm({ ...form, [k]: e.target.value });

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await register(form.userName, form.email, form.password);
      router.replace("/");
    } catch (err) {
      setError(err instanceof ApiError ? err.messages.join(" ") : "Could not reach the server.");
    }
  };

  return (
    <div className="auth">
      <div className="panel">
        <Link href="/" className="logo">IMDB</Link>
        <h1>Create account</h1>
        <form className="form" onSubmit={submit}>
          {error && <div className="notice error">{error}</div>}
          <label>Username<input required minLength={3} pattern="[a-zA-Z0-9_]+" title="Letters, numbers and underscores" value={form.userName} onChange={set("userName")} autoComplete="username" /></label>
          <label>Email<input type="email" required value={form.email} onChange={set("email")} autoComplete="email" /></label>
          <label>Password (min 8 characters)<input type="password" required minLength={8} value={form.password} onChange={set("password")} autoComplete="new-password" /></label>
          <button className="btn">Create account</button>
        </form>
        <p className="alt">Already registered? <Link href="/login">Sign in</Link></p>
      </div>
    </div>
  );
}
