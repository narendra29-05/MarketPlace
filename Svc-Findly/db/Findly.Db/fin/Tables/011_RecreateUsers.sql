-- ============================================================
--  Findly Database — Recreate Users (auth restored)
--  Script : 011_RecreateUsers.sql
--  Run    : after 010_RemoveAuth.sql
--
--  Real email+password authentication is back. Users carry a
--  role (Admin/Vendor/Buyer) and an optional link to a Vendor.
--  Reviews stay keyed by reviewer email (filled from the JWT).
-- ============================================================

USE [FindlyDb];
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Users' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    CREATE TABLE [fin].[Users]
    (
        [Id]            INT             NOT NULL    IDENTITY(1,1),

        [FirstName]     NVARCHAR(100)   NOT NULL,
        [LastName]      NVARCHAR(100)   NOT NULL,
        [Email]         NVARCHAR(255)   NOT NULL,
        [PasswordHash]  NVARCHAR(500)   NOT NULL,

        -- Role: 1 = Admin | 2 = Vendor | 3 = Buyer
        [Role]          INT             NOT NULL,

        [VendorId]      INT             NULL,
        [IsActive]      BIT             NOT NULL    DEFAULT 1,

        [CreatedBy]     NVARCHAR(100)   NULL,
        [CreatedAt]     DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),
        [UpdatedBy]     NVARCHAR(100)   NULL,
        [UpdatedAt]     DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_Users]         PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [UQ_Users_Email]   UNIQUE                ([Email]),
        CONSTRAINT [FK_Users_Vendors] FOREIGN KEY           ([VendorId]) REFERENCES [fin].[Vendors] ([Id]),
        CONSTRAINT [CK_Users_Role]    CHECK                 ([Role] IN (1, 2, 3))
    );

    CREATE NONCLUSTERED INDEX [IX_Users_VendorId] ON [fin].[Users] ([VendorId]);
    CREATE NONCLUSTERED INDEX [IX_Users_Role]     ON [fin].[Users] ([Role]);

    PRINT 'Recreated [fin].[Users]';
END
GO

-- Development admin: admin@findly.local / Admin@123!
-- Hash format {iterations}.{saltB64}.{hashB64} — must match PasswordHasher (PBKDF2-SHA256, 100k).
IF NOT EXISTS (SELECT 1 FROM [fin].[Users] WHERE [Email] = 'admin@findly.local')
BEGIN
    INSERT INTO [fin].[Users] ([FirstName], [LastName], [Email], [PasswordHash], [Role], [IsActive], [CreatedBy])
    VALUES (
        N'Findly',
        N'Admin',
        N'admin@findly.local',
        N'100000.RmluZGx5U2VlZFNhbHQxNg==.JtbHMFn40N9ehO2O0B7SQl/peqlcWidoeeeKD3zZaKc=',
        1,
        1,
        N'migration');

    PRINT 'Seeded admin user admin@findly.local';
END
GO
