import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { ROLE_ADMIN, ROLE_VENDOR, useAuth } from "../auth";
import { Field } from "../ui";

export default function SignInPage() {
  const { signIn } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function submit(event: FormEvent) {
    event.preventDefault();
    setBusy(true);
    setError(null);
    try {
      const user = await signIn(email, password);
      navigate(user.role === ROLE_ADMIN ? "/admin" : user.role === ROLE_VENDOR ? "/vendor" : "/");
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="card card-accent auth-card">
      <h1>Sign in</h1>
      <p className="sub">
        One login for everyone — your account's role decides what you can do: buyers review and
        compare, vendors manage listings and leads, admins moderate.
      </p>
      <form className="form" onSubmit={submit}>
        <Field label="Email">
          <input type="email" required value={email} onChange={e => setEmail(e.target.value)} autoFocus />
        </Field>
        <Field label="Password">
          <input type="password" required value={password} onChange={e => setPassword(e.target.value)} />
        </Field>
        {error && <div className="form-error">{error}</div>}
        <button className="btn btn-primary" disabled={busy}>
          {busy ? "Signing in…" : "Sign in"}
        </button>
      </form>
      <p className="sub" style={{ marginTop: 16, marginBottom: 0 }}>
        New to Findly? <Link to="/register">Create an account</Link>
      </p>
    </div>
  );
}
