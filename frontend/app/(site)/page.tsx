"use client";

import Link from "next/link";
import { useEffect, useMemo, useState } from "react";
import { api } from "@/lib/api";
import { FALLBACK_POSTER, year } from "@/lib/format";
import type { Genre, Media, Movie, Series } from "@/lib/types";

type Kind = "movies" | "series";

export default function BrowsePage() {
  const [kind, setKind] = useState<Kind>("movies");
  const [movies, setMovies] = useState<Movie[] | null>(null);
  const [series, setSeries] = useState<Series[] | null>(null);
  const [genres, setGenres] = useState<Genre[]>([]);
  const [genre, setGenre] = useState<number | null>(null);
  const [query, setQuery] = useState("");
  const [sort, setSort] = useState<"rate" | "new" | "title">("rate");

  useEffect(() => {
    api<Movie[]>("Movie").then(setMovies).catch(() => setMovies([]));
    api<Series[]>("Series").then(setSeries).catch(() => setSeries([]));
    api<Genre[]>("Genre").then(setGenres).catch(() => {});
  }, []);

  const items: Media[] | null = kind === "movies" ? movies : series;
  const shown = useMemo(() => {
    if (!items) return null;
    const q = query.trim().toLowerCase();
    return items
      .filter((m) => (!q || m.title.toLowerCase().includes(q)) && (genre === null || m.genres.some((g) => g.id === genre)))
      .sort((a, b) => (sort === "rate" ? b.rate - a.rate : sort === "new" ? +new Date(b.releaseDate) - +new Date(a.releaseDate) : a.title.localeCompare(b.title)));
  }, [items, query, genre, sort]);

  return (
    <>
      <section className="hero">
        <div className="wrap">
          <h1>Discover movies &amp; series</h1>
          <p>Browse the catalogue, read the cast and leave your own rating and comments.</p>
        </div>
      </section>
      <div className="wrap">
        <div className="toolbar">
          <div className="tabs">
            <button className={kind === "movies" ? "on" : ""} onClick={() => setKind("movies")}>Movies</button>
            <button className={kind === "series" ? "on" : ""} onClick={() => setKind("series")}>Series</button>
          </div>
          <input placeholder="Search titles…" value={query} onChange={(e) => setQuery(e.target.value)} style={{ flex: 1, minWidth: 200 }} />
          <select value={sort} onChange={(e) => setSort(e.target.value as typeof sort)}>
            <option value="rate">Top rated</option>
            <option value="new">Newest</option>
            <option value="title">Title A–Z</option>
          </select>
        </div>
        <div className="chips" style={{ marginBottom: "1.4rem" }}>
          <button className={`chip${genre === null ? " on" : ""}`} onClick={() => setGenre(null)}>All</button>
          {genres.map((g) => <button key={g.id} className={`chip${genre === g.id ? " on" : ""}`} onClick={() => setGenre(genre === g.id ? null : g.id)}>{g.title}</button>)}
        </div>

        {!shown && <p className="empty">Loading…</p>}
        {shown && shown.length === 0 && <p className="empty">Nothing matches your filters.</p>}
        <div className="grid">
          {shown?.map((m) => (
            <Link key={m.id} href={`/${kind}/${m.id}`} className="tile">
              <img src={m.posterUrl || FALLBACK_POSTER} alt={m.title} onError={(e) => { e.currentTarget.src = FALLBACK_POSTER; }} />
              <div className="meta">
                <b>{m.title}</b>
                <small>{year(m.releaseDate)} · <span className="rating">{m.rate.toFixed(1)}</span></small>
              </div>
            </Link>
          ))}
        </div>
      </div>
    </>
  );
}
