# Auth code backup (removed 2026-07-21)

JWT authentication was removed from the codebase to keep the services simple for now.
Everything needed to re-add it is preserved here — these files are exact copies of what
was deleted, and `Program.cs` / `appsettings.json` show the wiring as it was.

## What was removed

- **JWT auth**: `Microsoft.AspNetCore.Authentication.JwtBearer` (Api) and
  `Microsoft.IdentityModel.JsonWebTokens` (Infrastructure) packages,
  `AddAuthentication/AddJwtBearer` + `UseAuthentication/UseAuthorization` in Program.cs,
  the `Jwt` appsettings section, and all `[Authorize]` / `[AllowAnonymous]` attributes.
- **Users**: `User` entity, `UserRole` enum, `IUserRepository`/`UserRepository`, `DbUser`,
  Users table (dropped by migration `010_RemoveAuth.sql`).
- **Auth feature**: register/login/me endpoints, `AuthService`, PBKDF2 `PasswordHasher`,
  `JwtTokenGenerator`, `ICurrentUser` and its `CurrentUser` implementation,
  Swagger bearer transformer.
- **Identity plumbing in services**: `CreatedBy/UpdatedBy` reverted to "system",
  `CreateListingRequest.VendorId` came back, review requests carry
  `reviewerName`/`reviewerEmail` inline, vendor-portal routes take `{vendorId}`.

## To re-add later

1. Copy these folders back to their original locations (folder names encode the path,
   e.g. `Application.Common` → `src/Findly.Application/Common`).
2. Re-add the two NuGet packages and the Program.cs wiring (see `Program.cs` here).
3. Write a new migration recreating `fin.Users` (see `003_CreateUsers.sql` in the
   migrations history) and reversing `010_RemoveAuth.sql` (Reviews back to UserId FK,
   Leads.BuyerUserId).
4. Seeded dev admin was `admin@findly.local` / `Admin@123!` — hash format
   `{iterations}.{saltB64}.{hashB64}` (PBKDF2-SHA256, 100k iterations); see
   `009_SeedAdminUser.sql`.
