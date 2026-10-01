"use client";

import { useParams, useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import MediaExtras from "@/components/MediaExtras";
import { api } from "@/lib/api";
import { FALLBACK_POSTER, formatDate } from "@/lib/format";
import type { Season, Series } from "@/lib/types";

export default function SeriesDetailPage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();
  const [series, setSeries] = useState<Series | null>(null);
  const [seasons, setSeasons] = useState<Season[]>([]);

  useEffect(() => {
    api<Series>(`Series/${id}`).then(setSeries).catch(() => router.replace("/"));
    api<Season[]>("Season").then((all) => setSeasons(all.filter((s) => s.seriesId === Number(id)).sort((a, b) => a.seasonNumber - b.seasonNumber))).catch(() => {});
  }, [id, router]);
  if (!series) return <p className="empty">Loading…</p>;

  return (
    <div className="wrap">
      <div className="detail">
        <img className="poster" src={series.posterUrl || FALLBACK_POSTER} alt={series.title} onError={(e) => { e.currentTarget.src = FALLBACK_POSTER; }} />
        <div>
          <h1>{series.title}</h1>
          <div className="facts"><span>{formatDate(series.releaseDate)}</span><span>{seasons.length} seasons</span><span>{series.imdbId}</span></div>
          <div className="chips" style={{ marginBottom: "1rem" }}>{series.genres.map((g) => <span className="chip" key={g.id}>{g.title}</span>)}</div>
          <div className="rating big-rate">{series.rate.toFixed(1)}<small style={{ color: "var(--muted)", fontSize: ".9rem" }}> / 10</small></div>
          <p style={{ marginTop: "1rem", maxWidth: 680 }}>{series.description}</p>
        </div>
      </div>

      {seasons.length > 0 && (
        <section className="section">
          <h2>Seasons</h2>
          {seasons.map((s) => (
            <details className="season" key={s.id} open={seasons.length === 1}>
              <summary><span>Season {s.seasonNumber} · {s.title}</span><span className="rating">{s.rate.toFixed(1)}</span></summary>
              {s.episodes.sort((a, b) => a.episodeNumber - b.episodeNumber).map((e) => (
                <div className="episode" key={e.id}>
                  <b style={{ color: "var(--gold)" }}>E{e.episodeNumber}</b>
                  <div><b>{e.title}</b><p>{e.description}</p></div>
                  <span className="rating">{e.rate.toFixed(1)}</span>
                </div>
              ))}
              {s.episodes.length === 0 && <p className="empty">No episodes yet.</p>}
            </details>
          ))}
        </section>
      )}
      <MediaExtras mediaId={series.id} />
    </div>
  );
}
