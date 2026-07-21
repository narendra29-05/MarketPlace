import { useEffect, useState, type FormEvent } from "react";
import { Link, NavLink, Route, Routes, useNavigate } from "react-router-dom";
import { ROLE_ADMIN, ROLE_VENDOR, useAuth } from "./auth";
import { useCompare } from "./compare";
import { Avatar } from "./ui";
import AdminPage from "./pages/Admin";
import CatalogPage from "./pages/Catalog";
import ComparePage from "./pages/Compare";
import ListingDetailPage from "./pages/ListingDetail";
import RegisterPage from "./pages/Register";
import SignInPage from "./pages/SignIn";
import VendorPortalPage from "./pages/VendorPortal";

const THEME_KEY = "findly.theme";

function loadTheme(): string {
  const stored = localStorage.getItem(THEME_KEY);
  if (stored === "dark" || stored === "midnight") return "dark";
  return "light";
}

function useTheme() {
  const [theme, setTheme] = useState<string>(loadTheme);

  useEffect(() => {
    if (theme === "dark") document.documentElement.dataset.theme = "dark";
    else delete document.documentElement.dataset.theme;
    localStorage.setItem(THEME_KEY, theme);
  }, [theme]);

  return { theme, toggle: () => setTheme(current => (current === "dark" ? "light" : "dark")) };
}

function Brand() {
  return (
    <Link to="/" className="brand">
      <span className="brand-mark">f</span>
      findly
    </Link>
  );
}

const ROLE_LABEL: Record<number, string> = { 1: "Admin", 2: "Vendor", 3: "Buyer" };

function ProfileMenu() {
  const navigate = useNavigate();
  const { user, signOut } = useAuth();
  const { theme, toggle } = useTheme();
  const [open, setOpen] = useState(false);

  if (!user) return null;

  return (
    <span className="menu-wrap">
      <button className="avatar-btn" onClick={() => setOpen(current => !current)} aria-expanded={open}>
        <Avatar name={`${user.firstName} ${user.lastName}`} />
        <span>{user.firstName}</span>
      </button>
      {open && (
        <div className="menu" onMouseLeave={() => setOpen(false)}>
          <div className="menu-head">
            <div className="name">
              {user.firstName} {user.lastName}
            </div>
            <div className="mail">{user.email}</div>
            <span className="rolebadge">{ROLE_LABEL[user.role]}</span>
          </div>
          <button className="menu-item" onClick={toggle}>
            Theme
            <span>{theme === "dark" ? "☀" : "☾"}</span>
          </button>
          <button
            className="menu-item danger"
            onClick={() => {
              setOpen(false);
              signOut();
              navigate("/");
            }}
          >
            Sign out
          </button>
        </div>
      )}
    </span>
  );
}

function NavBar() {
  const navigate = useNavigate();
  const { user } = useAuth();
  const { theme, toggle } = useTheme();
  const [search, setSearch] = useState("");

  function submitSearch(event: FormEvent) {
    event.preventDefault();
    navigate(search.trim() ? `/?search=${encodeURIComponent(search.trim())}` : "/");
  }

  return (
    <header className="nav">
      <div className="nav-inner">
        <Brand />
        <form className="nav-search" onSubmit={submitSearch}>
          <input
            placeholder="Search software…"
            value={search}
            onChange={event => setSearch(event.target.value)}
            aria-label="Search software"
          />
        </form>
        <nav className="nav-links">
          <NavLink to="/" end>
            Browse
          </NavLink>
          {(user?.role === ROLE_VENDOR || user?.role === ROLE_ADMIN) && (
            <NavLink to="/vendor">Vendor portal</NavLink>
          )}
          {user?.role === ROLE_ADMIN && <NavLink to="/admin">Admin</NavLink>}
          {user ? (
            <ProfileMenu />
          ) : (
            <>
              <button className="icon-toggle" onClick={toggle} title="Switch theme" aria-label="Switch theme">
                {theme === "dark" ? "☀" : "☾"}
              </button>
              <Link to="/signin" className="btn btn-primary btn-sm">
                Sign in
              </Link>
            </>
          )}
        </nav>
      </div>
    </header>
  );
}

function Footer() {
  return (
    <footer className="footer">
      <div className="footer-inner">
        <div>
          <Brand />
          <p className="footer-tag">
            The comparison desk for business software. Real ratings from real buyers — we measure
            products, we don't sell them.
          </p>
          <div className="footer-dims">
            <span>Features</span>
            <span>Value for money</span>
            <span>Customer support</span>
          </div>
        </div>
        <div>
          <h4>Explore</h4>
          <Link to="/">Browse software</Link>
          <Link to="/compare">Compare products</Link>
          <Link to="/signin">Sign in</Link>
        </div>
        <div>
          <h4>For teams</h4>
          <Link to="/register">Create an account</Link>
          <Link to="/vendor">Vendor portal</Link>
        </div>
        <div className="footer-cta">
          <h4>List your product</h4>
          <p>
            Reach buyers at the exact moment they're comparing options. Register as a vendor and
            publish your first listing today.
          </p>
          <Link to="/register" className="btn btn-primary btn-sm">
            Become a vendor →
          </Link>
        </div>
      </div>
      <div className="footer-bottom">
        <span>© 2026 Findly. Decisions made with evidence.</span>
        <span>Built for buyers, honest with vendors.</span>
      </div>
    </footer>
  );
}

function CompareTray() {
  const { items, remove, clear } = useCompare();
  const navigate = useNavigate();

  if (items.length === 0) return null;

  return (
    <div className="tray">
      <div className="tray-inner">
        <div className="tray-items">
          {items.map(item => (
            <span className="tray-pill" key={item.id}>
              {item.name}
              <button onClick={() => remove(item.id)} aria-label={`Remove ${item.name} from comparison`}>
                ×
              </button>
            </span>
          ))}
        </div>
        {items.length < 2 ? (
          <span className="tray-hint">Pick at least 2 to compare</span>
        ) : (
          <button
            className="btn btn-primary btn-sm"
            onClick={() => navigate(`/compare?ids=${items.map(item => item.id).join(",")}`)}
          >
            Compare {items.length} →
          </button>
        )}
        <button className="btn btn-clear btn-sm" onClick={clear}>
          Clear
        </button>
      </div>
    </div>
  );
}

export default function App() {
  return (
    <div className="shell">
      <NavBar />
      <main className="main">
        <Routes>
          <Route path="/" element={<CatalogPage />} />
          <Route path="/l/:slug" element={<ListingDetailPage />} />
          <Route path="/compare" element={<ComparePage />} />
          <Route path="/signin" element={<SignInPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/vendor" element={<VendorPortalPage />} />
          <Route path="/admin" element={<AdminPage />} />
          <Route path="*" element={<div className="empty">This page does not exist. Head back to Browse.</div>} />
        </Routes>
      </main>
      <Footer />
      <CompareTray />
    </div>
  );
}
