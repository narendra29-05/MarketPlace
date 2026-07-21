import { useEffect, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { api, PRICING, type Category } from "../api";
import { LogoTile } from "../ui";

interface CompareItem {
  id: number;
  name: string;
  slug: string;
  logoUrl: string | null;
  websiteUrl: string;
  pricingType: number;
  startingPrice: number | null;
  pricePerUser: number | null;
  hasFreeTrial: boolean;
  freeTrialDays: number | null;
  averageRating: number;
  featuresRating: number;
  valueForMoneyRating: number;
  customerSupportRating: number;
  reviewCount: number;
  categories: Category[];
}

interface CompareResponse {
  listings: CompareItem[];
}

const SCORE_ROWS: Array<[string, keyof CompareItem]> = [
  ["Overall", "averageRating"],
  ["Features", "featuresRating"],
  ["Value for money", "valueForMoneyRating"],
  ["Customer support", "customerSupportRating"],
];

export default function ComparePage() {
  const [params] = useSearchParams();
  const ids = params.get("ids") ?? "";
  const [data, setData] = useState<CompareResponse | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!ids) return;
    setError(null);
    api
      .get<CompareResponse>(`/catalog/compare?ids=${ids}`)
      .then(setData)
      .catch(err => setError(err.message));
  }, [ids]);

  if (!ids) {
    return (
      <div className="empty">
        Nothing to compare yet. <Link to="/">Browse the catalog</Link> and tick “Compare” on 2–4 products.
      </div>
    );
  }
  if (error) return <div className="empty">{error}</div>;
  if (!data) return <div className="empty">Loading…</div>;

  const listings = data.listings;

  function bestValue(key: keyof CompareItem): number {
    return Math.max(...listings.map(listing => Number(listing[key]) || 0));
  }

  return (
    <>
      <h1 className="page-title">Side by side</h1>
      <div className="compare-scroller card" style={{ padding: 0 }}>
        <table className="compare-table">
          <thead>
            <tr>
              <th className="dim-col">Compared on</th>
              {listings.map(listing => (
                <th key={listing.id}>
                  <div style={{ display: "flex", gap: 10, alignItems: "center" }}>
                    <LogoTile name={listing.name} />
                    <div>
                      <div className="compare-name">{listing.name}</div>
                      <Link to={`/l/${listing.slug}`} style={{ fontSize: 13 }}>
                        View profile
                      </Link>
                    </div>
                  </div>
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {SCORE_ROWS.map(([label, key]) => {
              const best = bestValue(key);
              return (
                <tr key={label}>
                  <td className="dim-col">{label}</td>
                  {listings.map(listing => {
                    const value = Number(listing[key]);
                    const isBest = listing.reviewCount > 0 && value === best && best > 0;
                    return (
                      <td key={listing.id} className={isBest ? "best" : undefined}>
                        <span className="val">{listing.reviewCount > 0 ? value.toFixed(2) : "—"}</span>
                      </td>
                    );
                  })}
                </tr>
              );
            })}
            <tr>
              <td className="dim-col">Reviews</td>
              {listings.map(listing => (
                <td key={listing.id}>{listing.reviewCount}</td>
              ))}
            </tr>
            <tr>
              <td className="dim-col">Pricing model</td>
              {listings.map(listing => (
                <td key={listing.id}>{PRICING[listing.pricingType]}</td>
              ))}
            </tr>
            <tr>
              <td className="dim-col">Starting price</td>
              {listings.map(listing => (
                <td key={listing.id}>{listing.startingPrice != null ? `$${listing.startingPrice}/mo` : "—"}</td>
              ))}
            </tr>
            <tr>
              <td className="dim-col">Per user</td>
              {listings.map(listing => (
                <td key={listing.id}>{listing.pricePerUser != null ? `$${listing.pricePerUser}` : "—"}</td>
              ))}
            </tr>
            <tr>
              <td className="dim-col">Free trial</td>
              {listings.map(listing => (
                <td key={listing.id}>{listing.hasFreeTrial ? `${listing.freeTrialDays} days` : "No"}</td>
              ))}
            </tr>
            <tr>
              <td className="dim-col">Categories</td>
              {listings.map(listing => (
                <td key={listing.id}>{listing.categories.map(category => category.name).join(", ") || "—"}</td>
              ))}
            </tr>
          </tbody>
        </table>
      </div>
    </>
  );
}
