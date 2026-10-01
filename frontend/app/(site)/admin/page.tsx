"use client";

import Link from "next/link";
import { useCallback, useEffect, useState } from "react";
import { api, ApiError, useSession } from "@/lib/api";
import type { Genre } from "@/lib/types";

type FieldType = "text" | "textarea" | "number" | "date" | "url" | "genres";
interface Field { key: string; label: string; type: FieldType; half?: boolean; step?: string }
interface Resource { label: string; path: string; titleKey: string; fields: Field[]; columns: string[] }

const MEDIA_FIELDS: Field[] = [
  { key: "imdbId", label: "IMDB ID", type: "text", half: true },
  { key: "title", label: "Title", type: "text", half: true },
  { key: "releaseDate", label: "Release date", type: "date", half: true },
  { key: "rate", label: "Rate (0–10)", type: "number", half: true, step: "0.1" },
  { key: "posterUrl", label: "Poster URL", type: "url" },
  { key: "description", label: "Description", type: "textarea" },
  { key: "genreIds", label: "Genres", type: "genres" },
];

const RESOURCES: Record<string, Resource> = {
  movies: { label: "Movies", path: "Movie", titleKey: "title", columns: ["title", "releaseDate", "rate"], fields: [...MEDIA_FIELDS.slice(0, 4), { key: "duration", label: "Duration (minutes)", type: "number" }, ...MEDIA_FIELDS.slice(4)] },
  series: { label: "Series", path: "Series", titleKey: "title", columns: ["title", "releaseDate", "rate"], fields: MEDIA_FIELDS },
  genres: { label: "Genres", path: "Genre", titleKey: "title", columns: ["title"], fields: [{ key: "title", label: "Title", type: "text" }] },
  people: {
    label: "People", path: "Person", titleKey: "fullName", columns: ["fullName", "birthDate"],
    fields: [
      { key: "imdbId", label: "IMDB ID", type: "text", half: true },
      { key: "fullName", label: "Full name", type: "text", half: true },
      { key: "birthDate", label: "Birth date", type: "date" },
      { key: "photoUrl", label: "Photo URL", type: "url" },
      { key: "bio", label: "Biography", type: "textarea" },
    ],
  },
};

type Row = Record<string, unknown> & { id: number };

const toInput = (v: unknown, type: FieldType) => (type === "date" && typeof v === "string" ? v.slice(0, 10) : v == null ? "" : String(v));

export default function AdminPage() {
  const { ready, isAdmin } = useSession();
  const [tab, setTab] = useState<keyof typeof RESOURCES>("movies");
  const res = RESOURCES[tab];
  const [rows, setRows] = useState<Row[]>([]);
  const [genres, setGenres] = useState<Genre[]>([]);
  const [editing, setEditing] = useState<Row | "new" | null>(null);
  const [form, setForm] = useState<Record<string, string | number[]>>({});
  const [error, setError] = useState("");

  const load = useCallback(() => { api<Row[]>(res.path).then((d) => setRows(Array.isArray(d) ? d : [])).catch(() => setRows([])); }, [res.path]);
  useEffect(() => { setEditing(null); load(); }, [load]);
  useEffect(() => { api<Genre[]>("Genre").then(setGenres).catch(() => {}); }, []);

  const open = (row: Row | "new") => {
    setError("");
    setEditing(row);
    const next: Record<string, string | number[]> = {};
    for (const f of res.fields) {
      next[f.key] = f.type === "genres"
        ? row === "new" ? [] : ((row.genres as Genre[] | undefined) ?? []).map((g) => g.id)
        : row === "new" ? "" : toInput(row[f.key], f.type);
    }
    setForm(next);
  };

  const save = async (e: React.FormEvent) => {
    e.preventDefault();
    const body: Record<string, unknown> = {};
    for (const f of res.fields) {
      const v = form[f.key];
      body[f.key] = f.type === "number" ? Number(v) : f.type === "date" ? new Date(v as string).toISOString() : v;
    }
    try {
      await api(editing === "new" ? res.path : `${res.path}/${(editing as Row).id}`, { method: editing === "new" ? "POST" : "PUT", body: JSON.stringify(body) });
      setEditing(null);
      load();
    } catch (err) {
      setError(err instanceof ApiError ? err.messages.join(" ") : "Could not save.");
    }
  };

  const remove = async (row: Row) => {
    if (!confirm(`Delete "${row[res.titleKey]}"?`)) return;
    try {
      await api(`${res.path}/${row.id}`, { method: "DELETE" });
      load();
    } catch (err) {
      setError(err instanceof ApiError ? err.messages.join(" ") : "Could not delete.");
    }
  };

  if (!ready) return null;
  if (!isAdmin) return <div className="wrap"><p className="empty">Admin access only. <Link href="/login" style={{ color: "var(--gold)" }}>Sign in</Link> with an admin account.</p></div>;

  return (
    <div className="wrap" style={{ padding: "2rem 1.4rem 4rem" }}>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", flexWrap: "wrap", gap: "1rem" }}>
        <h1>Admin</h1>
        <div className="tabs">
          {Object.entries(RESOURCES).map(([k, r]) => <button key={k} className={tab === k ? "on" : ""} onClick={() => setTab(k)}>{r.label}</button>)}
        </div>
      </div>

      {editing ? (
        <form className="panel form" style={{ marginTop: "1.4rem", maxWidth: 720 }} onSubmit={save}>
          <h2>{editing === "new" ? `New ${res.label.replace(/s$/, "").toLowerCase()}` : `Edit ${String(editing[res.titleKey])}`}</h2>
          {error && <div className="notice error">{error}</div>}
          <div className="grid2">
            {res.fields.map((f) => (
              <div key={f.key} style={f.half || f.type === "number" || f.type === "date" ? undefined : { gridColumn: "1 / -1" }}>
                {f.type === "genres" ? (
                  <label>{f.label}
                    <div className="chips">
                      {genres.map((g) => {
                        const on = (form[f.key] as number[]).includes(g.id);
                        return <button type="button" key={g.id} className={`chip${on ? " on" : ""}`} onClick={() => setForm({ ...form, [f.key]: on ? (form[f.key] as number[]).filter((x) => x !== g.id) : [...(form[f.key] as number[]), g.id] })}>{g.title}</button>;
                      })}
                    </div>
                  </label>
                ) : f.type === "textarea" ? (
                  <label>{f.label}<textarea rows={4} required value={form[f.key] as string} onChange={(e) => setForm({ ...form, [f.key]: e.target.value })} /></label>
                ) : (
                  <label>{f.label}<input type={f.type} step={f.step} required value={form[f.key] as string} onChange={(e) => setForm({ ...form, [f.key]: e.target.value })} /></label>
                )}
              </div>
            ))}
          </div>
          <div className="actions" style={{ justifyContent: "flex-start" }}>
            <button className="btn">Save</button>
            <button type="button" className="btn ghost" onClick={() => setEditing(null)}>Cancel</button>
          </div>
        </form>
      ) : (
        <div className="panel" style={{ marginTop: "1.4rem" }}>
          <div style={{ display: "flex", justifyContent: "space-between", marginBottom: ".8rem" }}>
            <h2 style={{ margin: 0 }}>{res.label} ({rows.length})</h2>
            <button className="btn small" onClick={() => open("new")}>+ Add</button>
          </div>
          {error && <div className="notice error" style={{ marginBottom: ".8rem" }}>{error}</div>}
          <table>
            <thead><tr>{res.columns.map((c) => <th key={c}>{c.replace(/([A-Z])/g, " $1")}</th>)}<th /></tr></thead>
            <tbody>
              {rows.map((r) => (
                <tr key={r.id}>
                  {res.columns.map((c) => <td key={c}>{c.toLowerCase().includes("date") ? new Date(r[c] as string).toLocaleDateString() : String(r[c])}</td>)}
                  <td><div className="actions"><button className="btn ghost small" onClick={() => open(r)}>Edit</button><button className="btn danger small" onClick={() => remove(r)}>Delete</button></div></td>
                </tr>
              ))}
            </tbody>
          </table>
          {rows.length === 0 && <p className="empty">Nothing here yet.</p>}
        </div>
      )}
    </div>
  );
}
