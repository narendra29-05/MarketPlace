-- ============================================================
--  Findly Database — Vendors Table
--  Script : 001_CreateVendors.sql
--  Run    : after 000_CreateDatabase.sql
-- ============================================================

USE [FindlyDb];
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Vendors' AND schema_id = SCHEMA_ID('fin'))

BEGIN
    CREATE TABLE [fin].[Vendors]
    (
        -- Primary Key
        [Id]                INT             NOT NULL    IDENTITY(1,1),

        -- Personal Info
        [FirstName]         NVARCHAR(100)   NOT NULL,
        [LastName]          NVARCHAR(100)   NOT NULL,
        [Email]             NVARCHAR(255)   NOT NULL,
        [Phone]             NVARCHAR(20)    NOT NULL,

        -- Company Info
        [CompanyName]       NVARCHAR(200)   NOT NULL,
        [CompanySize]       INT             NULL,
        [IndustryType]      INT             NULL,
        [WebsiteUrl]        NVARCHAR(500)   NULL,
        [LogoUrl]           NVARCHAR(500)   NULL,

        -- Status: 1 = Pending | 2 = Verified | 3 = Rejected
        [Status]            INT             NOT NULL    DEFAULT 1,
        [RejectionReason]   NVARCHAR(1000)  NULL,

        -- Address
        [AddressLine1]      NVARCHAR(200)   NULL,
        [AddressLine2]      NVARCHAR(200)   NULL,
        [City]              NVARCHAR(100)   NULL,
        [State]             NVARCHAR(100)   NULL,
        [Country]           NVARCHAR(100)   NULL,
        [PostalCode]        NVARCHAR(20)    NULL,

        -- Audit
        [CreatedBy]         NVARCHAR(100)   NULL,
        [CreatedAt]         DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),
        [UpdatedBy]         NVARCHAR(100)   NULL,
        [UpdatedAt]         DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),

        -- Soft Delete
        [IsDeleted]         BIT             NOT NULL    CONSTRAINT [DF_Vendors_IsDeleted] DEFAULT (0),
        [DeletedAt]         DATETIME2       NULL,

        CONSTRAINT [PK_Vendors]              PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [CK_Vendors_Status]       CHECK                 ([Status] IN (1, 2, 3)),
        CONSTRAINT [CK_Vendors_IndustryType] CHECK                 ([IndustryType] BETWEEN 1 AND 16)
    );

    CREATE  INDEX [IX_Vendors_Status]
        ON [fin].[Vendors] ([Status]);

    CREATE  INDEX [IX_Vendors_CompanyName]
        ON [fin].[Vendors] ([CompanyName]);

    CREATE  INDEX [IX_Vendors_Country]
        ON [fin].[Vendors] ([Country]);

    CREATE UNIQUE INDEX [UX_Vendors_Email]
        ON [fin].[Vendors] ([Email])
        WHERE [IsDeleted] = 0;

    CREATE  INDEX [IX_Vendors_IsDeleted]
        ON [fin].[Vendors] ([IsDeleted])
        WHERE [IsDeleted] = 0;

    PRINT 'Created [fin].[Vendors]';
END
GO
