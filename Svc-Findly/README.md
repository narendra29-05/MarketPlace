# Findly

A G2/Capterra-style software marketplace API. Vendors onboard and list their products; buyers discover, compare and review them; interest converts into **leads** that vendors manage — the platform sells nothing itself, it provides comparison and discovery.

> **Note:** authentication is intentionally removed for now to keep the services simple — every endpoint is open. The full JWT/auth implementation is preserved in `docs/auth-code-backup/` and can be re-added later.

## Stack

.NET 10 · ASP.NET Core (controllers, API versioning) · Dapper + Dapper.Contrib · SQL Server 2022 · DbUp migrations · Autofac · AutoMapper · FluentValidation.

```
src/
  Findly.Domain           entities, enums, value objects, repository interfaces
  Findly.Contracts        request/response DTOs + FluentValidation validators
  Findly.Application      services (business flows), AutoMapper profiles
  Findly.Infrastructure   Dapper repositories, UnitOfWork (transactions)
  Findly.Api              controllers, exception middleware, CORS
db/
  Findly.Migrations       DbUp runner + embedded SQL scripts (schema `fin`)
  Findly.Db               mirror copies of the SQL scripts
```

## Getting started

```bash
docker compose up -d                 # SQL Server on localhost:14333 (sa / Findly@123!)
cd db/Findly.Migrations && dotnet run && cd ../..   # creates FindlyDb + tables + seeds
dotnet run --project src/Findly.Api  # API on http://localhost:5250, Swagger at /swagger

# frontend (repo root, sibling folder ../web)
cd ../web && npm install && npm run dev   # React app on http://localhost:5173
```

> The migration runner must be started from `db/Findly.Migrations` (it loads `appsettings.json` from the working directory).

Seeded data: 12 software categories (CRM, Project Management, Help Desk, …).

## Tests

```bash
dotnet test tests/Findly.UnitTests           # domain, validators, services (no DB needed)
dotnet test tests/Findly.IntegrationTests    # boots the API in-memory against the SQL container
# or: ./scripts/run-integration-tests.sh
```

Integration tests require the SQL Server container to be up and migrations applied.

## Flows

**Vendor onboarding**: `POST /vendors` (profile) → `POST /vendors/{id}/verify` → vendor can create listings.

**Listing lifecycle**: create with `vendorId` (vendor must be Verified) → Pending → Published via `POST /listings/{id}/approve` / Rejected → edits to a rejected or published listing return it to Pending for re-moderation → Archive/Restore.

**Reviews**: submitted with `reviewerName` + `reviewerEmail` inline (one review per email per listing). Dimensions are Overall, Features, Value for Money and Customer Support (1–5). Moderated Pending → Approved/Rejected; approved reviews recompute the listing's denormalized rating aggregates inside a transaction.

**Leads**: `POST /listings/{id}/leads` (leadType: contact/demo/quote/pricing + contact details). Vendor pipeline: New → Contacted → Qualified → Converted, any non-terminal state → Lost.

## API surface (v1, prefix `/api/v1`)

- `catalog` — `listings` (search/filter/sort/paging), `listings/{slug}`, `listings/{slug}/reviews`, `compare?ids=1,2`, `categories`
- `vendors` — CRUD + verify/reject
- `listings` — CRUD + approve/reject/archive/restore
- `listings/{id}/reviews`, `reviews/{id}` (+ approve/reject) — reviews + moderation
- `listings/{id}/leads` — lead submission
- `vendor-portal/{vendorId}` — profile, listings, leads, lead status updates
- `categories` — read + manage
- `admin` — pending vendors/listings/reviews queues, all leads

Errors are returned as RFC 7807 ProblemDetails (404/409/400 mapped in `ExceptionHandlingMiddleware`; validation failures are 422 from `FluentValidationFilter`).

## Configuration

- `ConnectionStrings:DefaultConnection` (API) and `ConnectionStrings:FindlyDB` (migrations) point at `localhost,14333` — the host port is 14333 because 1433 is commonly taken by other local SQL Server instances.
- CORS allows the frontend origin `http://localhost:5173`.
