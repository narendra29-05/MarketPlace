import type { ReactNode } from "react";
import type { ListingSummary } from "./api";

/* ---------- deterministic tile color from a name ---------- */

// Flat, muted product-identity palette — colorful enough to distinguish,
// desaturated enough to sit quietly in the monochrome system.
const TILE_COLORS = [
  "#0e7b54", // emerald
  "#8a5a2b", // umber
  "#3f5bd6", // indigo
  "#0e7490", // teal
  "#a3437c", // plum
  "#475069", // slate
  "#b0641f", // ochre
];

function gradientFor(name: string): string {
  let hash = 0;
  for (const char of name) hash = (hash * 31 + char.charCodeAt(0)) | 0;
  return TILE_COLORS[Math.abs(hash) % TILE_COLORS.length];
}

function initialsOf(name: string): string {
  return (
    name
      .split(/\s+/)
      .slice(0, 2)
      .map(word => word[0]?.toUpperCase() ?? "")
      .join("") || "?"
  );
}

export function LogoTile({ name }: { name: string }) {
  return (
    <div className="logo-tile" style={{ background: gradientFor(name) }} aria-hidden>
      {initialsOf(name)}
    </div>
  );
}

export function Avatar({ name }: { name: string }) {
  return (
    <div className="avatar" style={{ background: gradientFor(name) }} aria-hidden>
      {initialsOf(name)}
    </div>
  );
}

/* ---------- stars ---------- */

export function Stars({ value }: { value: number }) {
  const filled = Math.round(value);
  return (
    <span className="stars" aria-label={`${value} out of 5`}>
      {[1, 2, 3, 4, 5].map(n => (
        <span key={n} className={n <= filled ? "on" : "off"}>
          ★
        </span>
      ))}
    </span>
  );
}

/* ---------- signature: dimension score panel ---------- */

interface Scores {
  averageRating: number;
  featuresRating: number;
  valueForMoneyRating: number;
  customerSupportRating: number;
  reviewCount: number;
}

export function ScorePanel({ scores, large = false }: { scores: Scores; large?: boolean }) {
  const rated = scores.reviewCount > 0;
  const dims: Array<[string, number]> = [
    ["Features", scores.featuresRating],
    ["Value", scores.valueForMoneyRating],
    ["Support", scores.customerSupportRating],
  ];
  return (
    <div className={`score-panel${large ? " large" : ""}`}>
      <div className={`score-badge${rated ? "" : " muted"}`} title="Overall rating">
        {rated ? Number(scores.averageRating).toFixed(1) : "—"}
      </div>
      <div className="dims">
        {dims.map(([label, value]) => (
          <div className="dim" key={label}>
            <span className="dim-label">{label}</span>
            <span className="dim-track">
              <span className="dim-fill" style={{ width: `${(Number(value) / 5) * 100}%` }} />
            </span>
            <span className="dim-num">{rated ? Number(value).toFixed(1) : "–"}</span>
          </div>
        ))}
        <div className="score-count">
          {rated ? `${scores.reviewCount} review${scores.reviewCount === 1 ? "" : "s"}` : "No reviews yet"}
        </div>
      </div>
    </div>
  );
}

export function scoresOf(listing: ListingSummary): Scores {
  return {
    averageRating: listing.averageRating,
    featuresRating: listing.featuresRating,
    valueForMoneyRating: listing.valueForMoneyRating,
    customerSupportRating: listing.customerSupportRating,
    reviewCount: listing.reviewCount,
  };
}

/* ---------- misc ---------- */

export function StatusPill({ label, tone }: { label: string; tone: "ok" | "warn" | "bad" | "mute" }) {
  return <span className={`pill pill-${tone}`}>{label}</span>;
}

export function statusTone(status: number): "ok" | "warn" | "bad" | "mute" {
  if (status === 2) return "ok";
  if (status === 1) return "warn";
  if (status === 3) return "bad";
  return "mute";
}

export function Pager({
  page,
  totalPages,
  onPage,
}: {
  page: number;
  totalPages: number;
  onPage: (page: number) => void;
}) {
  if (totalPages <= 1) return null;
  return (
    <div className="pager">
      <button className="btn btn-ghost btn-sm" disabled={page <= 1} onClick={() => onPage(page - 1)}>
        ← Previous
      </button>
      <span className="info">
        Page {page} of {totalPages}
      </span>
      <button className="btn btn-ghost btn-sm" disabled={page >= totalPages} onClick={() => onPage(page + 1)}>
        Next →
      </button>
    </div>
  );
}

export function Field({ label, children }: { label: string; children: ReactNode }) {
  return (
    <div className="field">
      <label>{label}</label>
      {children}
    </div>
  );
}

export function RatingSelect({
  value,
  onChange,
}: {
  value: number;
  onChange: (value: number) => void;
}) {
  return (
    <select value={value} onChange={event => onChange(Number(event.target.value))}>
      {[5, 4, 3, 2, 1].map(n => (
        <option key={n} value={n}>
          {n} — {["", "Poor", "Fair", "Good", "Very good", "Excellent"][n]}
        </option>
      ))}
    </select>
  );
}
