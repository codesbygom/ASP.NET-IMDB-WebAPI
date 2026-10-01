"use client";

import Link from "next/link";
import { useCallback, useEffect, useState } from "react";
import { api, ApiError, useSession } from "@/lib/api";
import { FALLBACK_POSTER, timeSince } from "@/lib/format";
import { SCORES, type Cast, type Comment, type Rate } from "@/lib/types";

// Cast, your rating and comments for one movie/series (the API keys all
// three by media id).
export default function MediaExtras({ mediaId }: { mediaId: number }) {
  const { userName, userId } = useSession();
  const [cast, setCast] = useState<Cast[]>([]);
  const [rates, setRates] = useState<Rate[]>([]);
  const [comments, setComments] = useState<Comment[]>([]);
  const [text, setText] = useState("");
  const [error, setError] = useState("");

  const load = useCallback(() => {
    api<Cast[]>(`Cast/media/${mediaId}`).then((c) => setCast(Array.isArray(c) ? c : [])).catch(() => {});
    api<Rate[]>(`Rate/media/${mediaId}`).then(setRates).catch(() => {});
    api<Comment[]>(`Comment/media/${mediaId}`).then(setComments).catch(() => {});
  }, [mediaId]);
  useEffect(load, [load]);

  const mine = userName ? rates.find((r) => r.userName === userName) : undefined;
  const myScore = mine ? SCORES.indexOf(mine.score as (typeof SCORES)[number]) : 0;
  const average = rates.length ? rates.reduce((s, r) => s + SCORES.indexOf(r.score as (typeof SCORES)[number]), 0) / rates.length : 0;

  const guard = async (fn: () => Promise<unknown>) => {
    try {
      setError("");
      await fn();
      load();
    } catch (e) {
      setError(e instanceof ApiError ? e.messages.join(" ") : "Something went wrong.");
    }
  };

  const rate = (score: number) =>
    guard(() => (mine
      ? api(`Rate/${mine.id}`, { method: "PUT", body: JSON.stringify({ score: SCORES[score] }) })
      : api(`Rate/media/${mediaId}`, { method: "POST", body: JSON.stringify({ score: SCORES[score] }) })));

  const comment = (e: React.FormEvent) => {
    e.preventDefault();
    guard(async () => {
      await api(`Comment/media/${mediaId}`, { method: "POST", body: JSON.stringify({ text }) });
      setText("");
    });
  };

  return (
    <>
      {cast.length > 0 && (
        <section className="section">
          <h2>Cast &amp; crew</h2>
          <div className="cast">
            {cast.map((c) => (
              <Link key={c.id} href={`/people/${c.personId}`} className="person">
                <img src={c.person?.photoUrl || FALLBACK_POSTER} alt="" onError={(e) => { e.currentTarget.src = FALLBACK_POSTER; }} />
                <b>{c.person?.fullName ?? `Person #${c.personId}`}</b>
                <small>{c.role}</small>
              </Link>
            ))}
          </div>
        </section>
      )}

      <section className="section">
        <h2>Your rating</h2>
        {userName ? (
          <div style={{ display: "flex", gap: "1rem", alignItems: "center" }}>
            <div className="stars">
              {[1, 2, 3, 4, 5].map((n) => <button key={n} className={n <= myScore ? "on" : ""} aria-label={`${n} stars`} onClick={() => rate(n)}>★</button>)}
            </div>
            <span style={{ color: "var(--muted)" }}>
              {rates.length ? `Community average ${average.toFixed(1)} / 5 (${rates.length} ${rates.length === 1 ? "vote" : "votes"})` : "Be the first to rate"}
            </span>
          </div>
        ) : (
          <p style={{ color: "var(--muted)" }}><Link href="/login" style={{ color: "var(--gold)" }}>Sign in</Link> to rate.</p>
        )}
      </section>

      <section className="section">
        <h2>Comments ({comments.length})</h2>
        {error && <div className="notice error" style={{ marginBottom: ".8rem" }}>{error}</div>}
        {comments.map((c) => (
          <div className="comment" key={c.id}>
            <div className="avatar">{(c.userName ?? "?")[0]?.toUpperCase()}</div>
            <div><b>{c.userName}</b> <small>{timeSince(c.createdOn)}</small><p>{c.text}</p></div>
            {userId === c.userId && <button className="btn danger small" onClick={() => guard(() => api(`Comment/${c.id}`, { method: "DELETE" }))}>Delete</button>}
          </div>
        ))}
        {comments.length === 0 && <p className="empty">No comments yet.</p>}
        {userName ? (
          <form className="comment-form" onSubmit={comment}>
            <textarea rows={3} required maxLength={1000} placeholder="Share your thoughts…" value={text} onChange={(e) => setText(e.target.value)} />
            <div><button className="btn">Post comment</button></div>
          </form>
        ) : (
          <p style={{ color: "var(--muted)" }}><Link href="/login" style={{ color: "var(--gold)" }}>Sign in</Link> to comment.</p>
        )}
      </section>
    </>
  );
}
