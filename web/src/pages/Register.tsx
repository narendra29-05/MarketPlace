import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { ROLE_BUYER, ROLE_VENDOR, useAuth } from "../auth";
import { Field } from "../ui";

const ROLES = [
  { role: ROLE_BUYER, icon: "🛒", label: "Buyer", hint: "Compare & review" },
  { role: ROLE_VENDOR, icon: "🏪", label: "Vendor", hint: "List & get leads" },
];

export default function RegisterPage() {
  const { register } = useAuth();
  const navigate = useNavigate();
  const [role, setRole] = useState(ROLE_BUYER);
  const [form, setForm] = useState({ firstName: "", lastName: "", email: "", password: "" });
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function submit(event: FormEvent) {
    event.preventDefault();
    setBusy(true);
    setError(null);
    try {
      const user = await register({ ...form, role });
      navigate(user.role === ROLE_VENDOR ? "/vendor" : "/");
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="card card-accent auth-card">
      <h1>Create your account</h1>
      <p className="sub">Admins are appointed, not self-registered — accounts here are buyers or vendors.</p>
      <form className="form" onSubmit={submit}>
        <div className="field">
          <label>I am a</label>
          <div className="role-pick" style={{ gridTemplateColumns: "1fr 1fr" }}>
            {ROLES.map(option => (
              <button
                type="button"
                key={option.role}
                className={`role-opt${role === option.role ? " active" : ""}`}
                onClick={() => setRole(option.role)}
              >
                <span className="role-icon">{option.icon}</span>
                {option.label}
                <small>{option.hint}</small>
              </button>
            ))}
          </div>
        </div>
        <div className="form-row">
          <Field label="First name">
            <input required value={form.firstName} onChange={e => setForm({ ...form, firstName: e.target.value })} />
          </Field>
          <Field label="Last name">
            <input required value={form.lastName} onChange={e => setForm({ ...form, lastName: e.target.value })} />
          </Field>
        </div>
        <Field label="Work email">
          <input type="email" required value={form.email} onChange={e => setForm({ ...form, email: e.target.value })} />
        </Field>
        <Field label="Password">
          <input
            type="password"
            required
            minLength={8}
            value={form.password}
            onChange={e => setForm({ ...form, password: e.target.value })}
            placeholder="8+ characters with upper, lower and a digit"
          />
        </Field>
        {error && <div className="form-error">{error}</div>}
        <button className="btn btn-primary" disabled={busy}>
          {busy ? "Creating…" : "Create account"}
        </button>
      </form>
      <p className="sub" style={{ marginTop: 16, marginBottom: 0 }}>
        Already have an account? <Link to="/signin">Sign in</Link>
      </p>
    </div>
  );
}
