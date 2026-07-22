const API = "http://localhost:5250/api/v1";

async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
  const response = await fetch(`${API}${path}`, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...options.headers,
    },
  });

  if (!response.ok) {
    let message = `Request failed (${response.status})`;
    try {
      const body = await response.json();
      if (body.errors) message = Object.values<string[]>(body.errors).flat().join(" ");
      else message = body.detail || body.title || message;
    } catch {
      /* non-JSON error body */
    }
    throw new Error(message);
  }

  if (response.status === 204) return undefined as T;
  return response.json();
}

export const api = {
  get: <T>(path: string) => request<T>(path),
  post: <T>(path: string, body?: unknown) =>
    request<T>(path, { method: "POST", body: body === undefined ? undefined : JSON.stringify(body) }),
  put: <T>(path: string, body: unknown) =>
    request<T>(path, { method: "PUT", body: JSON.stringify(body) }),
  del: <T>(path: string) => request<T>(path, { method: "DELETE" }),
};

/* ---------- types (mirror of Findly.Contracts) ---------- */

export interface Paged<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNext: boolean;
  hasPrev: boolean;
}

export interface Category {
  id: number;
  name: string;
  slug: string;
  description: string | null;
  iconUrl: string | null;
  isActive: boolean;
}

export interface ListingSummary {
  id: number;
  name: string;
  slug: string;
  tagline: string | null;
  shortDescription: string;
  logoUrl: string | null;
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

export interface ListingDetail extends ListingSummary {
  description: string | null;
  websiteUrl: string;
  demoUrl: string | null;
  foundedYear: number | null;
  vendorId: number;
  vendorCompanyName: string | null;
}

export interface ListingResponse extends ListingSummary {
  websiteUrl: string;
  description: string | null;
  status: number;
  rejectionReason: string | null;
  vendorId: number;
}

export interface Review {
  id: number;
  listingId: number;
  reviewerName: string;
  overallRating: number;
  featuresRating: number;
  valueForMoneyRating: number;
  customerSupportRating: number;
  title: string;
  body: string;
  pros: string | null;
  cons: string | null;
  status: number;
  createdAt: string;
}

export interface Lead {
  id: number;
  listingId: number;
  listingName: string | null;
  vendorId: number;
  leadType: number;
  fullName: string;
  businessEmail: string;
  phone: string | null;
  company: string | null;
  companySize: number | null;
  message: string | null;
  status: number;
  createdAt: string;
}

export interface Vendor {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  companyName: string;
  companySize: number | null;
  industry: number | null;
  websiteUrl: string | null;
  status: number;
  rejectionReason: string | null;
  city: string | null;
  country: string | null;
}

/* ---------- enum labels (values mirror Findly.Domain.Enums) ---------- */

export const PRICING: Record<number, string> = {
  1: "Free",
  2: "Freemium",
  3: "Paid",
  4: "Contact vendor",
};

export const LEAD_TYPE: Record<number, string> = {
  1: "Contact vendor",
  2: "Request a demo",
  3: "Get a quote",
  4: "Get pricing",
};

export const LEAD_STATUS: Record<number, string> = {
  1: "New",
  2: "Contacted",
  3: "Qualified",
  4: "Converted",
  5: "Lost",
};

/** Allowed next states, mirroring Lead.TransitionTo. */
export const LEAD_NEXT: Record<number, number[]> = {
  1: [2, 5],
  2: [3, 5],
  3: [4, 5],
  4: [],
  5: [],
};

export const LISTING_STATUS: Record<number, string> = {
  1: "Pending",
  2: "Published",
  3: "Rejected",
  4: "Archived",
};

export const VENDOR_STATUS: Record<number, string> = {
  1: "Pending",
  2: "Verified",
  3: "Rejected",
};

export const INDUSTRIES: Record<number, string> = {
  1: "Manufacturing",
  2: "IT & software services",
  3: "Financial services",
  4: "Healthcare & pharma",
  5: "Retail & e-commerce",
  6: "Professional services",
  7: "Real estate & construction",
  8: "Transport & logistics",
  9: "Education",
  10: "Hospitality & travel",
  11: "Media & entertainment",
  12: "Agriculture",
  13: "Energy & utilities",
  14: "Telecommunications",
  15: "Non-profit / NGO",
  16: "Other",
};

export function priceLine(l: ListingSummary): string {
  if (l.pricingType === 1) return "Free";
  if (l.pricingType === 4) return "Contact vendor";
  const parts: string[] = [];
  if (l.startingPrice != null) parts.push(`from $${l.startingPrice}/mo`);
  if (l.pricePerUser != null) parts.push(`$${l.pricePerUser}/user`);
  return parts.length ? parts.join(" · ") : PRICING[l.pricingType];
}
