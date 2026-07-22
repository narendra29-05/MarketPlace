import { useCallback, useEffect, useState } from "react";
import {
  api,
  INDUSTRIES,
  LEAD_STATUS,
  LEAD_TYPE,
  type Lead,
  type ListingResponse,
  type Paged,
  type Review,
  type Vendor,
} from "../api";
import { Pager, StatusPill } from "../ui";

function usePaged<T>(path: string) {
  const [result, setResult] = useState<Paged<T> | null>(null);
  const [page, setPage] = useState(1);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(() => {
    setError(null);
    api
      .get<Paged<T>>(`${path}${path.includes("?") ? "&" : "?"}page=${page}&pageSize=10`)
      .then(setResult)
      .catch(err => setError(err.message));
  }, [path, page]);

  useEffect(load, [load]);

  return { result, page, setPage, error, setError, reload: load };
}

function VendorsTab() {
  const { result, page, setPage, error, setError, reload } = usePaged<Vendor>("/admin/vendors/pending");

  async function decide(vendor: Vendor, approve: boolean) {
    try {
      if (approve) {
        await api.post(`/vendors/${vendor.id}/verify`);
      } else {
        const reason = window.prompt(`Why is ${vendor.companyName} being rejected?`);
        if (!reason) return;
        await api.post(`/vendors/${vendor.id}/reject`, { reason });
      }
      reload();
    } catch (err) {
      setError((err as Error).message);
    }
  }

  return (
    <>
      {error && <div className="form-error">{error}</div>}
      {result && result.items.length === 0 && <div className="empty">No vendors waiting for verification.</div>}
      {result && result.items.length > 0 && (
        <div className="card" style={{ padding: 0 }}>
          <table className="table">
            <thead>
              <tr>
                <th>Company</th>
                <th>Contact</th>
                <th>Industry</th>
                <th>Location</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {result.items.map(vendor => (
                <tr key={vendor.id}>
                  <td>
                    <strong>{vendor.companyName}</strong>
                    {vendor.companySize && (
                      <div style={{ color: "var(--ink-soft)", fontSize: 13 }}>{vendor.companySize} people</div>
                    )}
                  </td>
                  <td>
                    {vendor.firstName} {vendor.lastName}
                    <div style={{ fontSize: 13 }}>{vendor.email}</div>
                  </td>
                  <td>{vendor.industry ? INDUSTRIES[vendor.industry] : "—"}</td>
                  <td>{[vendor.city, vendor.country].filter(Boolean).join(", ") || "—"}</td>
                  <td className="actions">
                    <button className="btn btn-primary btn-sm" onClick={() => decide(vendor, true)}>
                      Verify
                    </button>
                    <button className="btn btn-danger btn-sm" onClick={() => decide(vendor, false)}>
                      Reject
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
      {result && <Pager page={page} totalPages={result.totalPages} onPage={setPage} />}
    </>
  );
}

function ListingsTab() {
  const { result, page, setPage, error, setError, reload } = usePaged<ListingResponse>("/admin/listings/pending");

  async function decide(listing: ListingResponse, approve: boolean) {
    try {
      if (approve) {
        await api.post(`/listings/${listing.id}/approve`);
      } else {
        const reason = window.prompt(`Why is ${listing.name} being rejected?`);
        if (!reason) return;
        await api.post(`/listings/${listing.id}/reject`, { reason });
      }
      reload();
    } catch (err) {
      setError((err as Error).message);
    }
  }

  return (
    <>
      {error && <div className="form-error">{error}</div>}
      {result && result.items.length === 0 && <div className="empty">No listings waiting for approval.</div>}
      {result && result.items.length > 0 && (
        <div className="card" style={{ padding: 0 }}>
          <table className="table">
            <thead>
              <tr>
                <th>Product</th>
                <th>Description</th>
                <th>Categories</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {result.items.map(listing => (
                <tr key={listing.id}>
                  <td>
                    <strong>{listing.name}</strong>
                    <div style={{ color: "var(--ink-soft)", fontSize: 13 }}>/{listing.slug}</div>
                    <a href={listing.websiteUrl} target="_blank" rel="noreferrer" style={{ fontSize: 13 }}>
                      Website ↗
                    </a>
                  </td>
                  <td style={{ maxWidth: 320 }}>{listing.shortDescription}</td>
                  <td>{listing.categories.map(category => category.name).join(", ")}</td>
                  <td className="actions">
                    <button className="btn btn-primary btn-sm" onClick={() => decide(listing, true)}>
                      Approve
                    </button>
                    <button className="btn btn-danger btn-sm" onClick={() => decide(listing, false)}>
                      Reject
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
      {result && <Pager page={page} totalPages={result.totalPages} onPage={setPage} />}
    </>
  );
}

function ReviewsTab() {
  const { result, page, setPage, error, setError, reload } = usePaged<Review>("/admin/reviews/pending");

  async function decide(review: Review, approve: boolean) {
    try {
      if (approve) {
        await api.post(`/reviews/${review.id}/approve`);
      } else {
        const reason = window.prompt("Why is this review being rejected?");
        if (!reason) return;
        await api.post(`/reviews/${review.id}/reject`, { reason });
      }
      reload();
    } catch (err) {
      setError((err as Error).message);
    }
  }

  return (
    <>
      {error && <div className="form-error">{error}</div>}
      {result && result.items.length === 0 && <div className="empty">No reviews waiting for moderation.</div>}
      {result?.items.map(review => (
        <div className="card" key={review.id} style={{ marginBottom: 12 }}>
          <div style={{ display: "flex", justifyContent: "space-between", gap: 12 }}>
            <div>
              <strong>{review.title}</strong>
              <div style={{ color: "var(--ink-soft)", fontSize: 13 }}>
                {review.reviewerName} · listing #{review.listingId} · overall {review.overallRating}/5 · features{" "}
                {review.featuresRating}/5 · value {review.valueForMoneyRating}/5 · support {review.customerSupportRating}/5
              </div>
            </div>
            <div className="actions" style={{ display: "flex", gap: 6 }}>
              <button className="btn btn-primary btn-sm" onClick={() => decide(review, true)}>
                Approve
              </button>
              <button className="btn btn-danger btn-sm" onClick={() => decide(review, false)}>
                Reject
              </button>
            </div>
          </div>
          <p style={{ marginTop: 8, fontSize: 14 }}>{review.body}</p>
        </div>
      ))}
      {result && <Pager page={page} totalPages={result.totalPages} onPage={setPage} />}
    </>
  );
}

function LeadsTab() {
  const { result, page, setPage, error } = usePaged<Lead>("/admin/leads");

  return (
    <>
      {error && <div className="form-error">{error}</div>}
      {result && result.items.length === 0 && <div className="empty">No leads recorded yet.</div>}
      {result && result.items.length > 0 && (
        <div className="card" style={{ padding: 0 }}>
          <table className="table">
            <thead>
              <tr>
                <th>Contact</th>
                <th>Product</th>
                <th>Request</th>
                <th>Status</th>
                <th>Received</th>
              </tr>
            </thead>
            <tbody>
              {result.items.map(lead => (
                <tr key={lead.id}>
                  <td>
                    <strong>{lead.fullName}</strong>
                    <div style={{ fontSize: 13 }}>{lead.businessEmail}</div>
                  </td>
                  <td>{lead.listingName}</td>
                  <td>{LEAD_TYPE[lead.leadType]}</td>
                  <td>
                    <StatusPill
                      label={LEAD_STATUS[lead.status]}
                      tone={lead.status === 4 ? "ok" : lead.status === 5 ? "bad" : lead.status === 1 ? "warn" : "mute"}
                    />
                  </td>
                  <td>{new Date(lead.createdAt).toLocaleDateString()}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
      {result && <Pager page={page} totalPages={result.totalPages} onPage={setPage} />}
    </>
  );
}

export default function AdminPage() {
  const [tab, setTab] = useState<"vendors" | "listings" | "reviews" | "leads">("vendors");

  return (
    <>
      <h1 className="page-title">Moderation</h1>
      <div className="tabs">
        {(["vendors", "listings", "reviews", "leads"] as const).map(name => (
          <button key={name} className={`tab${tab === name ? " active" : ""}`} onClick={() => setTab(name)}>
            {name[0].toUpperCase() + name.slice(1)}
          </button>
        ))}
      </div>
      {tab === "vendors" && <VendorsTab />}
      {tab === "listings" && <ListingsTab />}
      {tab === "reviews" && <ReviewsTab />}
      {tab === "leads" && <LeadsTab />}
    </>
  );
}
