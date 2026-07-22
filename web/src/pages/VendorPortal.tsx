import { useCallback, useEffect, useState, type FormEvent } from "react";
import { Link } from "react-router-dom";
import {
  api,
  INDUSTRIES,
  LEAD_NEXT,
  LEAD_STATUS,
  LEAD_TYPE,
  LISTING_STATUS,
  PRICING,
  VENDOR_STATUS,
  type Category,
  type Lead,
  type ListingResponse,
  type Paged,
  type Vendor,
} from "../api";
import { useAuth } from "../auth";
import { Field, Pager, StatusPill, statusTone } from "../ui";

/* ---------- onboarding ---------- */

function OnboardingForm({ onCreated }: { onCreated: () => void }) {
  const { user } = useAuth();
  const [form, setForm] = useState({
    firstName: user?.firstName ?? "",
    lastName: user?.lastName ?? "",
    email: user?.email ?? "",
    phone: "",
    companyName: "",
    companySize: "",
    industry: 2,
    addressLine1: "",
    addressLine2: "",
    city: "",
    state: "",
    country: "",
    postalCode: "",
  });
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function submit(event: FormEvent) {
    event.preventDefault();
    setBusy(true);
    setError(null);
    try {
      await api.post<Vendor>("/vendors", {
        ...form,
        companySize: form.companySize ? Number(form.companySize) : null,
      });
      onCreated();
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="card card-accent">
      <h2>Register your company</h2>
      <p style={{ color: "var(--text-soft)", fontSize: 14, marginBottom: 14 }}>
        Your company profile is linked to this account. Once our team verifies it you can publish
        product listings.
      </p>
      <form className="form" onSubmit={submit}>
        <div className="form-row">
          <Field label="First name">
            <input required value={form.firstName} onChange={e => setForm({ ...form, firstName: e.target.value })} />
          </Field>
          <Field label="Last name">
            <input required value={form.lastName} onChange={e => setForm({ ...form, lastName: e.target.value })} />
          </Field>
        </div>
        <div className="form-row">
          <Field label="Contact email">
            <input type="email" required value={form.email} onChange={e => setForm({ ...form, email: e.target.value })} />
          </Field>
          <Field label="Phone">
            <input required value={form.phone} onChange={e => setForm({ ...form, phone: e.target.value })} placeholder="+14155550101" />
          </Field>
        </div>
        <div className="form-row">
          <Field label="Company name">
            <input required value={form.companyName} onChange={e => setForm({ ...form, companyName: e.target.value })} />
          </Field>
          <Field label="Company size">
            <input type="number" min={1} value={form.companySize} onChange={e => setForm({ ...form, companySize: e.target.value })} />
          </Field>
        </div>
        <Field label="Industry">
          <select value={form.industry} onChange={e => setForm({ ...form, industry: Number(e.target.value) })}>
            {Object.entries(INDUSTRIES).map(([value, label]) => (
              <option key={value} value={value}>
                {label}
              </option>
            ))}
          </select>
        </Field>
        <Field label="Address line 1">
          <input required value={form.addressLine1} onChange={e => setForm({ ...form, addressLine1: e.target.value })} />
        </Field>
        <Field label="Address line 2 (optional)">
          <input value={form.addressLine2} onChange={e => setForm({ ...form, addressLine2: e.target.value })} />
        </Field>
        <div className="form-row">
          <Field label="City">
            <input required value={form.city} onChange={e => setForm({ ...form, city: e.target.value })} />
          </Field>
          <Field label="State">
            <input required value={form.state} onChange={e => setForm({ ...form, state: e.target.value })} />
          </Field>
        </div>
        <div className="form-row">
          <Field label="Country">
            <input required value={form.country} onChange={e => setForm({ ...form, country: e.target.value })} />
          </Field>
          <Field label="Postal code">
            <input required value={form.postalCode} onChange={e => setForm({ ...form, postalCode: e.target.value })} />
          </Field>
        </div>
        {error && <div className="form-error">{error}</div>}
        <button className="btn btn-primary" disabled={busy}>
          {busy ? "Submitting…" : "Submit for verification"}
        </button>
      </form>
    </div>
  );
}

/* ---------- profile tab ---------- */

function ProfileTab({ vendor, missing, onCreated }: { vendor: Vendor | null; missing: boolean; onCreated: () => void }) {
  if (missing) return <OnboardingForm onCreated={onCreated} />;
  if (!vendor) return <div className="empty">Loading…</div>;

  return (
    <div className="card">
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "baseline" }}>
        <h2>{vendor.companyName}</h2>
        <StatusPill label={VENDOR_STATUS[vendor.status]} tone={statusTone(vendor.status)} />
      </div>
      {vendor.status === 1 && (
        <p className="form-ok" style={{ marginTop: 10 }}>
          Your profile is waiting for verification. You can create listings as soon as it's approved.
        </p>
      )}
      {vendor.status === 3 && vendor.rejectionReason && (
        <p className="form-error" style={{ marginTop: 10 }}>
          Verification declined: {vendor.rejectionReason}
        </p>
      )}
      <dl className="fact-list" style={{ marginTop: 14 }}>
        <div className="fact">
          <dt>Contact</dt>
          <dd>
            {vendor.firstName} {vendor.lastName}
          </dd>
        </div>
        <div className="fact">
          <dt>Email</dt>
          <dd>{vendor.email}</dd>
        </div>
        <div className="fact">
          <dt>Phone</dt>
          <dd>{vendor.phone}</dd>
        </div>
        <div className="fact">
          <dt>Industry</dt>
          <dd>{vendor.industry ? INDUSTRIES[vendor.industry] : "—"}</dd>
        </div>
        <div className="fact">
          <dt>Location</dt>
          <dd>{[vendor.city, vendor.country].filter(Boolean).join(", ") || "—"}</dd>
        </div>
      </dl>
    </div>
  );
}

/* ---------- listings tab ---------- */

function slugify(name: string): string {
  return name
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "");
}

function NewListingForm({ categories, onCreated }: { categories: Category[]; onCreated: () => void }) {
  const [form, setForm] = useState({
    name: "",
    slug: "",
    tagline: "",
    shortDescription: "",
    description: "",
    websiteUrl: "",
    demoUrl: "",
    pricingType: 3,
    startingPrice: "",
    pricePerUser: "",
    hasFreeTrial: false,
    freeTrialDays: "",
    categoryIds: [] as number[],
  });
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  function toggleCategory(id: number) {
    setForm(current => ({
      ...current,
      categoryIds: current.categoryIds.includes(id)
        ? current.categoryIds.filter(existing => existing !== id)
        : current.categoryIds.length < 5
          ? [...current.categoryIds, id]
          : current.categoryIds,
    }));
  }

  async function submit(event: FormEvent) {
    event.preventDefault();
    setBusy(true);
    setError(null);
    try {
      await api.post("/listings", {
        name: form.name,
        slug: form.slug,
        tagline: form.tagline || null,
        shortDescription: form.shortDescription,
        description: form.description || null,
        websiteUrl: form.websiteUrl,
        demoUrl: form.demoUrl || null,
        pricingType: form.pricingType,
        startingPrice: form.startingPrice ? Number(form.startingPrice) : null,
        pricePerUser: form.pricePerUser ? Number(form.pricePerUser) : null,
        hasFreeTrial: form.hasFreeTrial,
        freeTrialDays: form.hasFreeTrial && form.freeTrialDays ? Number(form.freeTrialDays) : null,
        categoryIds: form.categoryIds,
      });
      onCreated();
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setBusy(false);
    }
  }

  return (
    <form className="form" onSubmit={submit}>
      <div className="form-row">
        <Field label="Product name">
          <input
            required
            value={form.name}
            onChange={e => setForm({ ...form, name: e.target.value, slug: slugify(e.target.value) })}
          />
        </Field>
        <Field label="URL slug">
          <input required pattern="[a-z0-9-]+" value={form.slug} onChange={e => setForm({ ...form, slug: e.target.value })} />
        </Field>
      </div>
      <Field label="Tagline (optional)">
        <input value={form.tagline} onChange={e => setForm({ ...form, tagline: e.target.value })} />
      </Field>
      <Field label="Short description">
        <input required maxLength={500} value={form.shortDescription} onChange={e => setForm({ ...form, shortDescription: e.target.value })} />
      </Field>
      <Field label="Full description (optional)">
        <textarea value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} />
      </Field>
      <div className="form-row">
        <Field label="Website URL">
          <input type="url" required value={form.websiteUrl} onChange={e => setForm({ ...form, websiteUrl: e.target.value })} />
        </Field>
        <Field label="Demo URL (optional)">
          <input type="url" value={form.demoUrl} onChange={e => setForm({ ...form, demoUrl: e.target.value })} />
        </Field>
      </div>
      <div className="form-row">
        <Field label="Pricing model">
          <select value={form.pricingType} onChange={e => setForm({ ...form, pricingType: Number(e.target.value) })}>
            {Object.entries(PRICING).map(([value, label]) => (
              <option key={value} value={value}>
                {label}
              </option>
            ))}
          </select>
        </Field>
        <Field label="Starting price $/mo (optional)">
          <input type="number" min={0} step="0.01" value={form.startingPrice} onChange={e => setForm({ ...form, startingPrice: e.target.value })} />
        </Field>
      </div>
      <div className="form-row">
        <Field label="Price per user $ (optional)">
          <input type="number" min={0} step="0.01" value={form.pricePerUser} onChange={e => setForm({ ...form, pricePerUser: e.target.value })} />
        </Field>
        <Field label="Free trial">
          <div style={{ display: "flex", gap: 10, alignItems: "center" }}>
            <input
              type="checkbox"
              checked={form.hasFreeTrial}
              onChange={e => setForm({ ...form, hasFreeTrial: e.target.checked })}
            />
            {form.hasFreeTrial && (
              <input
                type="number"
                min={1}
                placeholder="days"
                value={form.freeTrialDays}
                onChange={e => setForm({ ...form, freeTrialDays: e.target.value })}
              />
            )}
          </div>
        </Field>
      </div>
      <Field label={`Categories (${form.categoryIds.length}/5)`}>
        <div className="chips">
          {categories.map(category => (
            <label key={category.id} className="compare-check chip" style={{ cursor: "pointer" }}>
              <input
                type="checkbox"
                checked={form.categoryIds.includes(category.id)}
                onChange={() => toggleCategory(category.id)}
              />
              {category.name}
            </label>
          ))}
        </div>
      </Field>
      {error && <div className="form-error">{error}</div>}
      <button className="btn btn-primary" disabled={busy || form.categoryIds.length === 0}>
        {busy ? "Submitting…" : "Submit for approval"}
      </button>
    </form>
  );
}

function ListingsTab() {
  const [result, setResult] = useState<Paged<ListingResponse> | null>(null);
  const [categories, setCategories] = useState<Category[]>([]);
  const [page, setPage] = useState(1);
  const [showForm, setShowForm] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(() => {
    api
      .get<Paged<ListingResponse>>(`/vendor-portal/listings?page=${page}&pageSize=10`)
      .then(setResult)
      .catch(err => setError(err.message));
  }, [page]);

  useEffect(load, [load]);
  useEffect(() => {
    api.get<Category[]>("/catalog/categories").then(setCategories).catch(() => {});
  }, []);

  async function act(listing: ListingResponse, action: "archive" | "restore") {
    try {
      await api.post(`/listings/${listing.id}/${action}`);
      load();
    } catch (err) {
      setError((err as Error).message);
    }
  }

  return (
    <>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 14 }}>
        <h2 style={{ fontSize: 18 }}>Your listings</h2>
        <button className="btn btn-primary btn-sm" onClick={() => setShowForm(current => !current)}>
          {showForm ? "Close" : "+ New listing"}
        </button>
      </div>
      {showForm && (
        <div className="card" style={{ marginBottom: 16 }}>
          <h2>New listing</h2>
          <NewListingForm
            categories={categories}
            onCreated={() => {
              setShowForm(false);
              load();
            }}
          />
        </div>
      )}
      {error && <div className="form-error">{error}</div>}
      {result && result.items.length === 0 && (
        <div className="empty">No listings yet. Create your first listing to appear in the catalog after approval.</div>
      )}
      {result && result.items.length > 0 && (
        <div className="card" style={{ padding: 0 }}>
          <table className="table">
            <thead>
              <tr>
                <th>Product</th>
                <th>Status</th>
                <th>Reviews</th>
                <th>Rating</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {result.items.map(listing => (
                <tr key={listing.id}>
                  <td>
                    <strong>{listing.name}</strong>
                    <div style={{ color: "var(--text-faint)", fontSize: 13 }}>/{listing.slug}</div>
                    {listing.status === 3 && listing.rejectionReason && (
                      <div style={{ color: "var(--bad)", fontSize: 13 }}>Rejected: {listing.rejectionReason}</div>
                    )}
                  </td>
                  <td>
                    <StatusPill label={LISTING_STATUS[listing.status]} tone={statusTone(listing.status)} />
                  </td>
                  <td>{listing.reviewCount}</td>
                  <td>{listing.reviewCount > 0 ? Number(listing.averageRating).toFixed(1) : "—"}</td>
                  <td className="actions">
                    {listing.status === 2 && (
                      <>
                        <Link className="btn btn-ghost btn-sm" to={`/l/${listing.slug}`}>
                          View live
                        </Link>
                        <button className="btn btn-ghost btn-sm" onClick={() => act(listing, "archive")}>
                          Archive
                        </button>
                      </>
                    )}
                    {listing.status === 4 && (
                      <button className="btn btn-ghost btn-sm" onClick={() => act(listing, "restore")}>
                        Restore
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
      {result && <Pager page={result.page} totalPages={result.totalPages} onPage={setPage} />}
    </>
  );
}

/* ---------- leads tab ---------- */

function LeadsTab() {
  const [result, setResult] = useState<Paged<Lead> | null>(null);
  const [status, setStatus] = useState("");
  const [page, setPage] = useState(1);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(() => {
    const query = new URLSearchParams({ page: String(page), pageSize: "10" });
    if (status) query.set("status", status);
    api
      .get<Paged<Lead>>(`/vendor-portal/leads?${query}`)
      .then(setResult)
      .catch(err => setError(err.message));
  }, [page, status]);

  useEffect(load, [load]);

  async function transition(lead: Lead, next: number) {
    try {
      await api.post(`/vendor-portal/leads/${lead.id}/status`, { status: next });
      load();
    } catch (err) {
      setError((err as Error).message);
    }
  }

  return (
    <>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 14 }}>
        <h2 style={{ fontSize: 18 }}>Leads</h2>
        <select
          value={status}
          onChange={event => {
            setStatus(event.target.value);
            setPage(1);
          }}
        >
          <option value="">All statuses</option>
          {Object.entries(LEAD_STATUS).map(([value, label]) => (
            <option key={value} value={value}>
              {label}
            </option>
          ))}
        </select>
      </div>
      {error && <div className="form-error">{error}</div>}
      {result && result.items.length === 0 && (
        <div className="empty">No leads here yet. Leads arrive when buyers request a demo, quote or contact from your listings.</div>
      )}
      {result && result.items.length > 0 && (
        <div className="card" style={{ padding: 0 }}>
          <table className="table">
            <thead>
              <tr>
                <th>Contact</th>
                <th>Product</th>
                <th>Request</th>
                <th>Status</th>
                <th>Move to</th>
              </tr>
            </thead>
            <tbody>
              {result.items.map(lead => (
                <tr key={lead.id}>
                  <td>
                    <strong>{lead.fullName}</strong>
                    <div style={{ fontSize: 13 }}>
                      <a href={`mailto:${lead.businessEmail}`}>{lead.businessEmail}</a>
                    </div>
                    <div style={{ color: "var(--text-faint)", fontSize: 13 }}>
                      {[lead.company, lead.companySize && `${lead.companySize} people`].filter(Boolean).join(" · ")}
                    </div>
                  </td>
                  <td>{lead.listingName}</td>
                  <td>
                    {LEAD_TYPE[lead.leadType]}
                    {lead.message && <div style={{ color: "var(--text-faint)", fontSize: 13 }}>“{lead.message}”</div>}
                  </td>
                  <td>
                    <StatusPill
                      label={LEAD_STATUS[lead.status]}
                      tone={lead.status === 4 ? "ok" : lead.status === 5 ? "bad" : lead.status === 1 ? "warn" : "mute"}
                    />
                  </td>
                  <td className="actions">
                    {LEAD_NEXT[lead.status].map(next => (
                      <button
                        key={next}
                        className={`btn btn-sm ${next === 5 ? "btn-danger" : "btn-ghost"}`}
                        onClick={() => transition(lead, next)}
                      >
                        {LEAD_STATUS[next]}
                      </button>
                    ))}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
      {result && <Pager page={result.page} totalPages={result.totalPages} onPage={setPage} />}
    </>
  );
}

/* ---------- page ---------- */

export default function VendorPortalPage() {
  const [tab, setTab] = useState<"profile" | "listings" | "leads">("profile");
  const [vendor, setVendor] = useState<Vendor | null>(null);
  const [missing, setMissing] = useState(false);

  const loadProfile = useCallback(() => {
    api
      .get<Vendor>("/vendor-portal/profile")
      .then(profile => {
        setVendor(profile);
        setMissing(false);
      })
      .catch(() => setMissing(true));
  }, []);

  useEffect(() => {
    loadProfile();
  }, [loadProfile]);

  return (
    <>
      <h1 className="page-title">Vendor portal</h1>
      <div className="tabs">
        {(["profile", "listings", "leads"] as const).map(name => (
          <button key={name} className={`tab${tab === name ? " active" : ""}`} onClick={() => setTab(name)}>
            {name[0].toUpperCase() + name.slice(1)}
          </button>
        ))}
      </div>
      {tab === "profile" && (
        <ProfileTab
          vendor={vendor}
          missing={missing}
          onCreated={loadProfile}
        />
      )}
      {tab === "listings" && <ListingsTab />}
      {tab === "leads" && <LeadsTab />}
    </>
  );
}
