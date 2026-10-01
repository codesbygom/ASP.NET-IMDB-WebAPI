export const year = (iso: string) => new Date(iso).getFullYear();

export const formatDate = (iso: string) => new Date(iso).toLocaleDateString("en-US", { dateStyle: "long" });

export const duration = (minutes: number) => `${Math.floor(minutes / 60)}h ${minutes % 60}m`;

export const timeSince = (iso: string) => {
  const s = Math.max(1, Math.floor((Date.now() - new Date(iso).getTime()) / 1000));
  for (const [size, name] of [[31536000, "year"], [2592000, "month"], [86400, "day"], [3600, "hour"], [60, "minute"]] as const) {
    if (s >= size) return `${Math.floor(s / size)} ${name}${Math.floor(s / size) === 1 ? "" : "s"} ago`;
  }
  return "just now";
};

// Person/poster URLs are free text, so show a neutral placeholder when one is missing.
export const FALLBACK_POSTER = "data:image/svg+xml;utf8," + encodeURIComponent(
  '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 300 450"><rect width="300" height="450" fill="#23262f"/><g fill="none" stroke="#5b6072" stroke-width="10"><rect x="85" y="150" width="130" height="150" rx="10"/><path d="M95 280l40-50 30 35 20-20 25 35"/></g></svg>',
);
