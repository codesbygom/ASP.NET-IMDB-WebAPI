"use client";

import { useParams, useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import MediaExtras from "@/components/MediaExtras";
import { api } from "@/lib/api";
import { duration, FALLBACK_POSTER, formatDate } from "@/lib/format";
import type { Movie } from "@/lib/types";

export default function MovieDetailPage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();
  const [movie, setMovie] = useState<Movie | null>(null);

  useEffect(() => { api<Movie>(`Movie/${id}`).then(setMovie).catch(() => router.replace("/")); }, [id, router]);
  if (!movie) return <p className="empty">Loading…</p>;

  return (
    <div className="wrap">
      <div className="detail">
        <img className="poster" src={movie.posterUrl || FALLBACK_POSTER} alt={movie.title} onError={(e) => { e.currentTarget.src = FALLBACK_POSTER; }} />
        <div>
          <h1>{movie.title}</h1>
          <div className="facts"><span>{formatDate(movie.releaseDate)}</span><span>{duration(movie.duration)}</span><span>{movie.imdbId}</span></div>
          <div className="chips" style={{ marginBottom: "1rem" }}>{movie.genres.map((g) => <span className="chip" key={g.id}>{g.title}</span>)}</div>
          <div className="rating big-rate">{movie.rate.toFixed(1)}<small style={{ color: "var(--muted)", fontSize: ".9rem" }}> / 10</small></div>
          <p style={{ marginTop: "1rem", maxWidth: 680 }}>{movie.description}</p>
        </div>
      </div>
      <MediaExtras mediaId={movie.id} />
    </div>
  );
}
