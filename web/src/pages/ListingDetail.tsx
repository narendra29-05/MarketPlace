import { useEffect, useState, type FormEvent } from "react";
import { useParams } from "react-router-dom";
import { Link } from "react-router-dom";
import { api, LEAD_TYPE, PRICING, priceLine, type Lead, type ListingDetail, type Paged, type Review } from "../api";
import { useAuth } from "../auth";
import { Avatar, Field, LogoTile, Pager, RatingSelect, ScorePanel, scoresOf, Stars } from "../ui";

function LeadForm({ listing }: { listing: ListingDetail }) {
  const { user } = useAuth();
  const [form, setForm] = useState({
    leadType: 2,
    fullName: user ? `${user.firstName} ${user.lastName}` : "",
    businessEmail: user?.email ?? "",
    phone: "",
    company: "",
    companySize: "",
    message: "",
  });
  const [sent, setSent] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function submit(event: FormEvent) {
    event.preventDefault();
    setBusy(true);
    setError(null);
    try {
      await api.post<Lead>(`/listings/${listing.id}/leads`, {
        leadType: form.leadType,
        fullName: form.fullName,
        businessEmail: form.businessEmail,
        phone: form.phone || null,
        company: form.company || null,
        companySize: form.companySize ? Number(form.companySize) : null,
        message: form.message || null,
      });
      setSent(true);
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setBusy(false);
    }
  }

  if (sent) {
    return (
      <div className="card">
        <h2>Request sent</h2>
        <p className="form-ok">
          {listing.vendorCompanyName ?? listing.name} has your request and will reach out at {form.businessEmail}.
        </p>
      </div>
    );
  }

  return (
    <div className="card card-accent">
      <h2>Talk to {listing.vendorCompanyName ?? "the vendor"}</h2>
      <form className="form" onSubmit={submit}>
        <Field label="What do you need?">
          <select value={form.leadType} onChange={e => setForm({ ...form, leadType: Number(e.target.value) })}>
            {Object.entries(LEAD_TYPE).map(([value, label]) => (
              <option key={value} value={value}>
                {label}
              </option>
            ))}
          </select>
        </Field>
        <Field label="Full name">
          <input required value={form.fullName} onChange={e => setForm({ ...form, fullName: e.target.value })} />
        </Field>
        <Field label="Work email">
          <input
            required
            type="email"
            value={form.businessEmail}
            onChange={e => setForm({ ...form, businessEmail: e.target.value })}
          />
        </Field>
        <div className="form-row">
          <Field label="Company (optional)">
            <input value={form.company} onChange={e => setForm({ ...form, company: e.target.value })} />
          </Field>
          <Field label="Team size (optional)">
            <input
              type="number"
              min={1}
              value={form.companySize}
              onChange={e => setForm({ ...form, companySize: e.target.value })}
            />
          </Field>
        </div>
        <Field label="Message (optional)">
          <textarea
            value={form.message}
            onChange={e => setForm({ ...form, message: e.target.value })}
            placeholder="What are you evaluating for?"
          />
        </Field>
        {error && <div className="form-error">{error}</div>}
        <button className="btn btn-primary" disabled={busy}>
          {busy ? "Sending…" : "Send request"}
        </button>
      </form>
    </div>
  );
}

function ReviewForm({ listingId, onCreated }: { listingId: number; onCreated: () => void }) {
  const [form, setForm] = useState({
    overallRating: 5,
    featuresRating: 5,
    valueForMoneyRating: 5,
    customerSupportRating: 5,
    title: "",
    body: "",
    pros: "",
    cons: "",
  });
  const [error, setError] = useState<string | null>(null);
  const [done, setDone] = useState(false);
  const [busy, setBusy] = useState(false);

  async function submit(event: FormEvent) {
    event.preventDefault();
    setBusy(true);
    setError(null);
    try {
      await api.post<Review>(`/listings/${listingId}/reviews`, {
        ...form,
        pros: form.pros || null,
        cons: form.cons || null,
      });
      setDone(true);
      onCreated();
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setBusy(false);
    }
  }

  if (done) {
    return <p className="form-ok">Review submitted — it appears here once a moderator approves it.</p>;
  }

  return (
    <form className="form" onSubmit={submit}>
      <div className="form-row">
        <Field label="Overall">
          <RatingSelect value={form.overallRating} onChange={v => setForm({ ...form, overallRating: v })} />
        </Field>
        <Field label="Features">
          <RatingSelect value={form.featuresRating} onChange={v => setForm({ ...form, featuresRating: v })} />
        </Field>
      </div>
      <div className="form-row">
        <Field label="Value for money">
          <RatingSelect value={form.valueForMoneyRating} onChange={v => setForm({ ...form, valueForMoneyRating: v })} />
        </Field>
        <Field label="Customer support">
          <RatingSelect
            value={form.customerSupportRating}
            onChange={v => setForm({ ...form, customerSupportRating: v })}
          />
        </Field>
      </div>
      <Field label="Title">
        <input required maxLength={200} value={form.title} onChange={e => setForm({ ...form, title: e.target.value })} />
      </Field>
      <Field label="Your review">
        <textarea
          required
          maxLength={4000}
          value={form.body}
          onChange={e => setForm({ ...form, body: e.target.value })}
          placeholder="What does your team use it for? What should other buyers know?"
        />
      </Field>
      <div className="form-row">
        <Field label="Pros (optional)">
          <input value={form.pros} onChange={e => setForm({ ...form, pros: e.target.value })} />
        </Field>
        <Field label="Cons (optional)">
          <input value={form.cons} onChange={e => setForm({ ...form, cons: e.target.value })} />
        </Field>
      </div>
      {error && <div className="form-error">{error}</div>}
      <button className="btn btn-primary" disabled={busy}>
        {busy ? "Submitting…" : "Submit review"}
      </button>
    </form>
  );
}

export default function ListingDetailPage() {
  const { slug } = useParams<{ slug: string }>();
  const { user } = useAuth();
  const [listing, setListing] = useState<ListingDetail | null>(null);
  const [reviews, setReviews] = useState<Paged<Review> | null>(null);
  const [reviewPage, setReviewPage] = useState(1);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!slug) return;
    setError(null);
    api
      .get<ListingDetail>(`/catalog/listings/${slug}`)
      .then(setListing)
      .catch(err => setError(err.message));
  }, [slug]);

  useEffect(() => {
    if (!slug) return;
    api
      .get<Paged<Review>>(`/catalog/listings/${slug}/reviews?page=${reviewPage}&pageSize=5`)
      .then(setReviews)
      .catch(() => {});
  }, [slug, reviewPage]);

  if (error) return <div className="empty">{error}</div>;
  if (!listing) return <div className="empty">Loading…</div>;

  return (
    <>
      <nav className="crumb">
        <Link to="/">Browse</Link>
        {" / "}
        {listing.categories[0]?.name ?? "Software"}
        {" / "}
        <span className="here">{listing.name}</span>
      </nav>
      <div className="detail-head">
        <LogoTile name={listing.name} />
        <div className="detail-title">
          <h1>{listing.name}</h1>
          <div className="vendor-line">
            {listing.tagline && <>{listing.tagline} · </>}
            by {listing.vendorCompanyName ?? "Unknown vendor"}
            {listing.foundedYear && <> · since {listing.foundedYear}</>}
          </div>
          <div className="chips" style={{ marginTop: 8 }}>
            {listing.categories.map(category => (
              <span className="chip" key={category.id}>
                {category.name}
              </span>
            ))}
          </div>
        </div>
      </div>

      <div className="detail-grid">
        <div>
          <div className="card">
            <ScorePanel scores={scoresOf(listing)} large />
          </div>

          <div className="card">
            <h2>About</h2>
            <p className="prose">{listing.description ?? listing.shortDescription}</p>
          </div>

          {reviews && reviews.items.some(review => review.pros || review.cons) && (
            <div className="card">
              <h2>What reviewers keep saying</h2>
              <div className="glance">
                <div>
                  {reviews.items
                    .filter(review => review.pros)
                    .map(review => (
                      <div className="pro" key={review.id} style={{ marginBottom: 8 }}>
                        + {review.pros}
                      </div>
                    ))}
                </div>
                <div>
                  {reviews.items
                    .filter(review => review.cons)
                    .map(review => (
                      <div className="con" key={review.id} style={{ marginBottom: 8 }}>
                        − {review.cons}
                      </div>
                    ))}
                </div>
              </div>
            </div>
          )}

          <div className="card">
            <h2>Reviews {reviews && reviews.totalCount > 0 && `(${reviews.totalCount})`}</h2>
            {reviews && reviews.items.length === 0 && (
              <p className="empty">No approved reviews yet. Be the first to share how this works for your team.</p>
            )}
            {reviews?.items.map(review => (
              <div className="review" key={review.id}>
                <div className="review-head">
                  <Avatar name={review.reviewerName} />
                  <div style={{ flex: 1 }}>
                    <div className="review-title">{review.title}</div>
                    <div className="review-meta">
                      {review.reviewerName} · {new Date(review.createdAt).toLocaleDateString()}
                    </div>
                  </div>
                  <Stars value={review.overallRating} />
                </div>
                <p className="review-body">{review.body}</p>
                {(review.pros || review.cons) && (
                  <div className="pros-cons">
                    {review.pros && <div className="pro">+ {review.pros}</div>}
                    {review.cons && <div className="con">− {review.cons}</div>}
                  </div>
                )}
              </div>
            ))}
            {reviews && <Pager page={reviews.page} totalPages={reviews.totalPages} onPage={setReviewPage} />}

            <div className="section-gap">
              <h3 style={{ marginBottom: 10 }}>Write a review</h3>
              <p className="review-meta" style={{ marginBottom: 10 }}>
                Posting as {user.firstName} {user.lastName} ({user.email})
              </p>
              <ReviewForm listingId={listing.id} onCreated={() => setReviewPage(1)} />
            </div>
          </div>
        </div>

        <aside>
          <div className="card">
            <h2>Facts</h2>
            <dl className="fact-list">
              <div className="fact">
                <dt>Pricing</dt>
                <dd>{PRICING[listing.pricingType]}</dd>
              </div>
              <div className="fact">
                <dt>Price</dt>
                <dd>{priceLine(listing)}</dd>
              </div>
              <div className="fact">
                <dt>Free trial</dt>
                <dd>{listing.hasFreeTrial ? `${listing.freeTrialDays} days` : "No"}</dd>
              </div>
              <div className="fact">
                <dt>Website</dt>
                <dd>
                  <a href={listing.websiteUrl} target="_blank" rel="noreferrer">
                    Visit site ↗
                  </a>
                </dd>
              </div>
              {listing.demoUrl && (
                <div className="fact">
                  <dt>Demo</dt>
                  <dd>
                    <a href={listing.demoUrl} target="_blank" rel="noreferrer">
                      Watch demo ↗
                    </a>
                  </dd>
                </div>
              )}
            </dl>
          </div>
          <LeadForm listing={listing} />
        </aside>
      </div>
    </>
  );
}
