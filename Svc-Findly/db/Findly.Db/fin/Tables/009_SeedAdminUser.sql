-- ============================================================
--  Findly Database — Seed Admin User (development only)
--  Script : 009_SeedAdminUser.sql
--  Run    : after 008_SeedCategories.sql
--
--  Credentials : admin@findly.local / Admin@123!
--  Hash format : {iterations}.{saltBase64}.{hashBase64}
--                PBKDF2-SHA256, 100,000 iterations, 16-byte salt, 32-byte key
--                (must match Findly.Infrastructure.Security.PasswordHasher)
--  IMPORTANT   : rotate these credentials outside Development.
-- ============================================================

USE [FindlyDb];
GO

IF NOT EXISTS (SELECT 1 FROM [fin].[Users] WHERE [Email] = 'admin@findly.local')
BEGIN
    INSERT INTO [fin].[Users] ([FirstName], [LastName], [Email], [PasswordHash], [Role], [IsActive], [CreatedBy])
    VALUES (
        N'Findly',
        N'Admin',
        N'admin@findly.local',
        N'100000.RmluZGx5U2VlZFNhbHQxNg==.JtbHMFn40N9ehO2O0B7SQl/peqlcWidoeeeKD3zZaKc=',
        1,   -- Admin
        1,
        N'migration');

    PRINT 'Seeded admin user admin@findly.local';
END
GO
