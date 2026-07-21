-- ============================================================
--  Findly Database — Users Table
--  Script : 003_CreateUsers.sql
--  Run    : after 002_CreateListings.sql
-- ============================================================

USE [FindlyDb];
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Users' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    CREATE TABLE [fin].[Users]
    (
        -- Primary Key
        [Id]            INT             NOT NULL    IDENTITY(1,1),

        -- Identity
        [FirstName]     NVARCHAR(100)   NOT NULL,
        [LastName]      NVARCHAR(100)   NOT NULL,
        [Email]         NVARCHAR(255)   NOT NULL,
        [PasswordHash]  NVARCHAR(500)   NOT NULL,

        -- Role: 1 = Admin | 2 = Vendor | 3 = Buyer
        [Role]          INT             NOT NULL,

        -- Vendor link (set after vendor onboarding)
        [VendorId]      INT             NULL,

        [IsActive]      BIT             NOT NULL    DEFAULT 1,

        -- Audit
        [CreatedBy]     NVARCHAR(100)   NULL,
        [CreatedAt]     DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),
        [UpdatedBy]     NVARCHAR(100)   NULL,
        [UpdatedAt]     DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_Users]         PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [UQ_Users_Email]   UNIQUE                ([Email]),
        CONSTRAINT [FK_Users_Vendors] FOREIGN KEY           ([VendorId]) REFERENCES [fin].[Vendors] ([Id]),
        CONSTRAINT [CK_Users_Role]    CHECK                 ([Role] IN (1, 2, 3))
    );

    CREATE NONCLUSTERED INDEX [IX_Users_VendorId]
        ON [fin].[Users] ([VendorId]);

    CREATE NONCLUSTERED INDEX [IX_Users_Role]
        ON [fin].[Users] ([Role]);

    PRINT 'Created [fin].[Users]';
END
GO
