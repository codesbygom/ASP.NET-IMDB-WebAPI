"use client";

import { useParams, useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { api } from "@/lib/api";
import { FALLBACK_POSTER, formatDate } from "@/lib/format";
import type { Person } from "@/lib/types";

export default function PersonPage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();
  const [person, setPerson] = useState<Person | null>(null);

  useEffect(() => { api<Person>(`Person/${id}`).then(setPerson).catch(() => router.replace("/")); }, [id, router]);
  if (!person) return <p className="empty">Loading…</p>;

  return (
    <div className="wrap">
      <div className="detail">
        <img className="poster" style={{ aspectRatio: "1", borderRadius: "50%" }} src={person.photoUrl || FALLBACK_POSTER} alt={person.fullName} onError={(e) => { e.currentTarget.src = FALLBACK_POSTER; }} />
        <div>
          <h1>{person.fullName}</h1>
          <div className="facts"><span>Born {formatDate(person.birthDate)}</span><span>{person.imdbId}</span></div>
          <p style={{ maxWidth: 680 }}>{person.bio}</p>
        </div>
      </div>
    </div>
  );
}
