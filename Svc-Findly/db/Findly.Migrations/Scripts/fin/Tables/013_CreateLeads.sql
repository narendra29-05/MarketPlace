-- ============================================================
--  Findly Database — Leads Table (buyer inquiry to a Vendor)
--  Script : 013_CreateLeads.sql
--  Run    : after 012_CreateReviewVotes.sql
-- ============================================================

USE [FindlyDb];
GO

-- Filtered indexes require these SET options ON at creation time.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Leads' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    CREATE TABLE [fin].[Leads]
    (
        [Id]          INT             NOT NULL    IDENTITY(1,1),
        [ListingId]   INT             NOT NULL,
        [VendorId]    INT             NOT NULL,
        [UserId]      INT             NULL,   -- null when submitted by a guest

        -- Type: 1 DemoRequest | 2 QuoteRequest | 3 ContactRequest | 4 PricingRequest
        [Type]        INT             NOT NULL,
        -- Status: 1 New | 2 Contacted | 3 Qualified | 4 Converted | 5 Closed
        [Status]      INT             NOT NULL    DEFAULT 1,

        [Name]        NVARCHAR(200)   NOT NULL,
        [Email]       NVARCHAR(255)   NOT NULL,
        [Phone]       NVARCHAR(20)    NULL,
        [CompanyName] NVARCHAR(200)   NULL,
        [Message]     NVARCHAR(2000)  NULL,

        [CreatedBy]   NVARCHAR(100)   NULL,
        [CreatedAt]   DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),
        [UpdatedBy]   NVARCHAR(100)   NULL,
        [UpdatedAt]   DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),

        -- Soft Delete
        [IsDeleted]         BIT             NOT NULL    CONSTRAINT [DF_Leads_IsDeleted] DEFAULT (0),
        [DeletedAt]         DATETIME2       NULL,

        CONSTRAINT [PK_Leads]           PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_Leads_Listing]   FOREIGN KEY           ([ListingId]) REFERENCES [fin].[Listings] ([Id]),
        CONSTRAINT [FK_Leads_Vendor]    FOREIGN KEY           ([VendorId])  REFERENCES [fin].[Vendors]  ([Id]),
        CONSTRAINT [FK_Leads_User]      FOREIGN KEY           ([UserId])    REFERENCES [fin].[Users]    ([Id]),
        CONSTRAINT [CK_Leads_Type]      CHECK                 ([Type]   BETWEEN 1 AND 4),
        CONSTRAINT [CK_Leads_Status]    CHECK                 ([Status] BETWEEN 1 AND 5)
    );

    CREATE INDEX [IX_Leads_VendorId]  ON [fin].[Leads] ([VendorId]);
    CREATE INDEX [IX_Leads_ListingId] ON [fin].[Leads] ([ListingId]);
    CREATE INDEX [IX_Leads_Status]    ON [fin].[Leads] ([Status]);

    CREATE INDEX [IX_Leads_IsDeleted] ON [fin].[Leads] ([IsDeleted]) WHERE [IsDeleted] = 0;

    PRINT 'Created [fin].[Leads]';
END
GO
