import { useEffect, useState, type FormEvent } from "react";

import { Link, useNavigate, useSearchParams } from "react-router-dom";
import { api, PRICING, priceLine, type Category, type ListingSummary, type Paged } from "../api";
import { useCompare } from "../compare";
import { LogoTile, Pager, ScorePanel, scoresOf, Stars } from "../ui";

const SORTS: Array<[string, string]> = [
  ["1", "Highest rated"],
  ["2", "Most reviewed"],
  ["3", "Newest"],
  ["4", "Name A–Z"],
];

function Leaderboard({ leaders }: { leaders: ListingSummary[] }) {
  if (leaders.length === 0) return null;
  return (
    <aside className="hero-panel">
      <div className="hero-panel-head">
        <h3>Top rated right now</h3>
        <span>Live</span>
      </div>
      {leaders.map((listing, index) => (
        <Link to={`/l/${listing.slug}`} className="leader" key={listing.id}>
          <span className="leader-rank">{index + 1}</span>
          <LogoTile name={listing.name} />
          <span>
            <div className="leader-name">{listing.name}</div>
            <div className="leader-cat">{listing.categories[0]?.name ?? "Software"}</div>
            <div className="leader-stars">
              <Stars value={listing.averageRating} />
            </div>
          </span>
          <span className="leader-score">{Number(listing.averageRating).toFixed(1)}</span>
        </Link>
      ))}
      <div className="hero-panel-foot">
        <a href="#results">See the full ranking ↓</a>
      </div>
    </aside>
  );
}

function Hero({
  categories,
  productCount,
  leaders,
  notice,
  onSearch,
  onCategory,
}: {
  categories: Category[];
  productCount: number | null;
  leaders: ListingSummary[];
  notice: string | null;
  onSearch: (term: string) => void;
  onCategory: (slug: string) => void;
}) {
  const [term, setTerm] = useState("");

  function submit(event: FormEvent) {
    event.preventDefault();
    onSearch(term.trim());
  }

  return (
    <div className="hero-band">
      <div className="hero-inner">
        <div>
          <div className="hero-eyebrow">Software comparison platform</div>
          <h1>
            Choose software on <span className="grad">evidence</span>, not ads.
          </h1>
          <p className="hero-sub">
            Every product on Findly is scored by verified reviews across the three things buyers
            actually argue about — features, value for money, and support.
          </p>

          <form className="hero-search" onSubmit={submit}>
            <input
              placeholder="Try “CRM”, “help desk”, “analytics”…"
              value={term}
              onChange={event => setTerm(event.target.value)}
              aria-label="Search software"
            />
            <button className="btn btn-primary">Search</button>
          </form>

          {notice && <div className="hero-notice">{notice}</div>}

          <div className="hero-stats">
            <div className="hero-stat">
              <div className="stat-num">{productCount ?? "—"}</div>
              <div className="stat-label">Products listed</div>
            </div>
            <div className="hero-stat">
              <div className="stat-num">{categories.length || "—"}</div>
              <div className="stat-label">Categories</div>
            </div>
            <div className="hero-stat">
              <div className="stat-num">3</div>
              <div className="stat-label">Rating dimensions</div>
            </div>
          </div>

          {categories.length > 0 && (
            <div className="hero-cats">
              {categories.slice(0, 8).map(category => (
                <button key={category.id} className="chip-cat" onClick={() => onCategory(category.slug)}>
                  {category.name}
                </button>
              ))}
            </div>
          )}
        </div>

        <Leaderboard leaders={leaders} />
      </div>
    </div>
  );
}

/* G2-style "most popular categories", Findly cut: pick a category, see its leaders. */
function CategoryShowcase({ categories, initialSlug }: { categories: Category[]; initialSlug?: string }) {
  const [selected, setSelected] = useState<Category | null>(null);
  const [items, setItems] = useState<ListingSummary[]>([]);
  const active =
    selected ?? categories.find(category => category.slug === initialSlug) ?? categories[0] ?? null;

  useEffect(() => {
    if (!active) return;
    api
      .get<Paged<ListingSummary>>(`/catalog/listings?categorySlug=${active.slug}&sortBy=1&pageSize=3`)
      .then(result => setItems(result.items))
      .catch(() => setItems([]));
  }, [active?.slug]);

  if (categories.length === 0) return null;

  return (
    <section className="section">
      <div className="section-head">
        <div className="section-eyebrow">Browse by category</div>
        <h2>Every category has a winner.</h2>
        <p>Pick a category and meet its highest-scored products — ranked by buyers, not budgets.</p>
      </div>
      <div className="showcase">
        <div className="cat-list">
          {categories.slice(0, 10).map(category => (
            <button
              key={category.id}
              className={`cat-item${active?.id === category.id ? " active" : ""}`}
              onClick={() => setSelected(category)}
            >
              {category.name}
            </button>
          ))}
        </div>
        <div className="showcase-cards">
          {items.map(listing => (
            <Link to={`/l/${listing.slug}`} className="mini-card" key={listing.id}>
              <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start" }}>
                <LogoTile name={listing.name} />
                {listing.reviewCount > 0 && (
                  <span className="mini-badge">{Number(listing.averageRating).toFixed(1)}</span>
                )}
              </div>
              <div>
                <div className="mini-card-name">{listing.name}</div>
                <div className="mini-card-tag">{listing.tagline ?? listing.shortDescription}</div>
              </div>
              <div className="mini-card-meta">
                <Stars value={listing.averageRating} />
                <span className="count">
                  {listing.reviewCount > 0 ? `${listing.reviewCount} review${listing.reviewCount === 1 ? "" : "s"}` : "New"}
                </span>
              </div>
            </Link>
          ))}
          {items.length === 0 && (
            <div className="empty showcase-empty">
              No published products in {active?.name} yet — it's wide open for the first vendor.
            </div>
          )}
        </div>
      </div>
    </section>
  );
}

/* Findly's answer to the G2 Grid: satisfaction (x) vs review volume (y), real data. */
function EvidenceMap({ listings }: { listings: ListingSummary[] }) {
  const rated = listings.filter(listing => listing.reviewCount > 0);
  if (rated.length < 2) return null;

  const maxReviews = Math.max(...rated.map(listing => listing.reviewCount));

  return (
    <section className="section">
      <div className="section-head">
        <div className="section-eyebrow">The evidence map</div>
        <h2>Where every product really stands.</h2>
        <p>
          Buyer satisfaction against review volume — the further top-right, the more proof behind
          the score. No sponsorships, no weighting, just reviews.
        </p>
      </div>
      <div className="map-card">
        <div className="map-frame">
          <div className="map-ylab">Review volume →</div>
          <div className="map-plot">
            <span className="map-quad tl">Crowd-tested</span>
            <span className="map-quad tr">Proven leaders</span>
            <span className="map-quad bl">Emerging</span>
            <span className="map-quad br">Hidden gems</span>
            {(() => {
              // Fan out marks that would land on the exact same spot.
              const occupied: Record<string, number> = {};
              return rated.map(listing => {
                let x = Math.min(94, Math.max(6, (Number(listing.averageRating) / 5) * 100 - 4));
                let y = Math.min(88, Math.max(8, (listing.reviewCount / maxReviews) * 76 + 6));
                const key = `${Math.round(x)}:${Math.round(y)}`;
                const stacked = occupied[key] ?? 0;
                occupied[key] = stacked + 1;
                x = Math.max(6, x - stacked * 6);
                y = Math.min(88, y + (stacked % 2 === 1 ? 7 : 0));
                return { listing, x, y };
              });
            })().map(({ listing, x, y }) => {
              return (
                <Link
                  to={`/l/${listing.slug}`}
                  className="map-dot"
                  key={listing.id}
                  style={{ left: `${x}%`, bottom: `${y}%` }}
                  title={`${listing.name} — ${Number(listing.averageRating).toFixed(1)}/5 from ${listing.reviewCount} review${listing.reviewCount === 1 ? "" : "s"}`}
                >
                  <LogoTile name={listing.name} />
                  <span>{listing.name}</span>
                </Link>
              );
            })}
          </div>
          <div className="map-xlab">Buyer satisfaction →</div>
        </div>
        <p className="map-note">
          {rated.length} rated products plotted · updates live as reviews are approved
        </p>
      </div>
    </section>
  );
}

/* G2-style dual CTA, Findly voice. */
function CtaBand() {
  return (
    <section className="section cta-band">
      <div className="cta-panel">
        <div className="cta-kicker">Using software?</div>
        <h3>Say what actually worked.</h3>
        <p>
          Your review scores a product on features, value and support — and becomes the evidence
          the next buyer relies on. Two minutes, real impact.
        </p>
        <a href="#results" className="btn btn-primary">
          Pick a product to review
        </a>
      </div>
      <div className="cta-panel alt">
        <div className="cta-kicker">Selling software?</div>
        <h3>Meet buyers mid-decision.</h3>
        <p>
          Findly puts your product in front of teams at the exact moment they're comparing options —
          and sends their requests straight to your pipeline.
        </p>
        <Link to="/vendor" className="btn btn-primary">
          List your product
        </Link>
      </div>
    </section>
  );
}

export default function CatalogPage() {
  const [params, setParams] = useSearchParams();
  const navigate = useNavigate();
  const [notice, setNotice] = useState<string | null>(null);
  const [categories, setCategories] = useState<Category[]>([]);
  const [leaders, setLeaders] = useState<ListingSummary[]>([]);
  const [allListings, setAllListings] = useState<ListingSummary[]>([]);
  const [result, setResult] = useState<Paged<ListingSummary> | null>(null);
  const [error, setError] = useState<string | null>(null);
  const compare = useCompare();

  const search = params.get("search") ?? "";
  const categorySlug = params.get("category") ?? "";
  const pricingType = params.get("pricing") ?? "";
  const minRating = params.get("minRating") ?? "";
  const freeTrial = params.get("trial") === "1";
  const sortBy = params.get("sort") ?? "1";
  const page = Number(params.get("page") ?? "1");

  function setParam(key: string, value: string | null) {
    const next = new URLSearchParams(params);
    if (value) next.set(key, value);
    else next.delete(key);
    if (key !== "page") next.delete("page");
    setParams(next);
  }

  useEffect(() => {
    api.get<Category[]>("/catalog/categories").then(setCategories).catch(() => {});
    api
      .get<Paged<ListingSummary>>("/catalog/listings?sortBy=1&pageSize=50")
      .then(top => {
        setLeaders(top.items.slice(0, 3));
        setAllListings(top.items);
      })
      .catch(() => {});
  }, []);

  useEffect(() => {
    const query = new URLSearchParams({ page: String(page), pageSize: "10", sortBy });
    if (search) query.set("search", search);
    if (categorySlug) query.set("categorySlug", categorySlug);
    if (pricingType) query.set("pricingType", pricingType);
    if (minRating) query.set("minRating", minRating);
    if (freeTrial) query.set("hasFreeTrial", "true");

    setError(null);
    api
      .get<Paged<ListingSummary>>(`/catalog/listings?${query}`)
      .then(fetched => {
        setResult(fetched);

        if (search && fetched.totalCount === 0) {
          // Nothing matched — stay in the hero and say so instead of navigating.
          setNotice(`No products match “${search}” yet. Try another term or pick a category below.`);
          return;
        }
        setNotice(null);

        if (search && fetched.totalCount === 1) {
          // One exact match — take the buyer straight to that product.
          navigate(`/l/${fetched.items[0].slug}`);
          return;
        }

        // Multiple matches — land on the data, not the hero.
        if (search || categorySlug) {
          requestAnimationFrame(() =>
            document.getElementById("results")?.scrollIntoView({ behavior: "smooth", block: "start" }),
          );
        }
      })
      .catch(err => setError(err.message));
  }, [search, categorySlug, pricingType, minRating, freeTrial, sortBy, page]);

  const hasFilters = Boolean(search || categorySlug || pricingType || minRating || freeTrial);

  return (
    <>
      <Hero
        categories={categories}
        productCount={result?.totalCount ?? null}
        leaders={leaders}
        notice={notice}
        onSearch={term => {
          setNotice(null);
          setParam("search", term || null);
        }}
        onCategory={slug => {
          setNotice(null);
          setParam("category", slug);
        }}
      />

      <div className="catalog" id="results">
        <aside className="rail">
          <div className="rail-inner">
            <div>
              <h3>Category</h3>
              <select value={categorySlug} onChange={event => setParam("category", event.target.value || null)}>
                <option value="">All categories</option>
                {categories.map(category => (
                  <option key={category.id} value={category.slug}>
                    {category.name}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <h3>Pricing</h3>
              {Object.entries(PRICING).map(([value, label]) => (
                <label key={value}>
                  <input
                    type="radio"
                    name="pricing"
                    checked={pricingType === value}
                    onChange={() => setParam("pricing", value)}
                  />
                  {label}
                </label>
              ))}
              <label>
                <input type="radio" name="pricing" checked={pricingType === ""} onChange={() => setParam("pricing", null)} />
                Any
              </label>
            </div>
            <div>
              <h3>Rating</h3>
              <select value={minRating} onChange={event => setParam("minRating", event.target.value || null)}>
                <option value="">Any rating</option>
                <option value="4">4.0 and up</option>
                <option value="3">3.0 and up</option>
              </select>
            </div>
            <div>
              <label>
                <input type="checkbox" checked={freeTrial} onChange={event => setParam("trial", event.target.checked ? "1" : null)} />
                Free trial available
              </label>
            </div>
            {hasFilters && (
              <button className="btn btn-ghost btn-sm clear" onClick={() => setParams(new URLSearchParams())}>
                Clear all filters
              </button>
            )}
          </div>
        </aside>

        <section>
          <div className="result-meta" style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
            <span>
              {result ? `${result.totalCount} product${result.totalCount === 1 ? "" : "s"}` : "Loading…"}
              {search && <> for “{search}”</>}
            </span>
            <select value={sortBy} onChange={event => setParam("sort", event.target.value)} aria-label="Sort results">
              {SORTS.map(([value, label]) => (
                <option key={value} value={value}>
                  {label}
                </option>
              ))}
            </select>
          </div>

          {error && <div className="form-error">{error}</div>}

          {result && result.items.length === 0 && (
            <div className="empty">
              No software matches these filters yet. Clear a filter or try a different search term.
            </div>
          )}

          <div className="rows">
            {result?.items.map(listing => (
              <article className="row-card" key={listing.id}>
                <LogoTile name={listing.name} />
                <div>
                  <Link className="row-name" to={`/l/${listing.slug}`}>
                    {listing.name}
                  </Link>
                  <div className="row-tag">{listing.tagline ?? listing.shortDescription}</div>
                  <div className="chips">
                    <span className="chip chip-price">{priceLine(listing)}</span>
                    {listing.hasFreeTrial && <span className="chip chip-trial">{listing.freeTrialDays}-day trial</span>}
                    {listing.categories.map(category => (
                      <span className="chip" key={category.id}>
                        {category.name}
                      </span>
                    ))}
                  </div>
                </div>
                <ScorePanel scores={scoresOf(listing)} />
                <div className="row-actions">
                  <Link className="btn btn-primary btn-sm" to={`/l/${listing.slug}`}>
                    View
                  </Link>
                  <label className="compare-check">
                    <input
                      type="checkbox"
                      checked={compare.has(listing.id)}
                      onChange={() => compare.toggle({ id: listing.id, name: listing.name })}
                    />
                    Compare
                  </label>
                </div>
              </article>
            ))}
          </div>

          {result && <Pager page={result.page} totalPages={result.totalPages} onPage={p => setParam("page", String(p))} />}
        </section>
      </div>

      <CategoryShowcase categories={categories} initialSlug={leaders[0]?.categories[0]?.slug} />
      <EvidenceMap listings={allListings} />
      <CtaBand />
    </>
  );
}
