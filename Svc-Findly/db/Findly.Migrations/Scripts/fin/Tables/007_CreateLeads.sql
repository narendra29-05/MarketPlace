-- ============================================================
--  Findly Database — Leads Table
--  Script : 007_CreateLeads.sql
--  Run    : after 006_CreateReviews.sql
-- ============================================================

USE [FindlyDb];
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Leads' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    CREATE TABLE [fin].[Leads]
    (
        -- Primary Key
        [Id]            INT             NOT NULL    IDENTITY(1,1),

        -- Foreign Keys
        [ListingId]     INT             NOT NULL,
        [VendorId]      INT             NOT NULL,   -- denormalized from listing for vendor-portal queries
        [BuyerUserId]   INT             NULL,       -- set when a logged-in buyer submits the lead

        -- LeadType: 1 = ContactVendor | 2 = RequestDemo | 3 = GetQuote | 4 = GetPricing
        [LeadType]      INT             NOT NULL,

        -- Contact details
        [FullName]      NVARCHAR(200)   NOT NULL,
        [BusinessEmail] NVARCHAR(255)   NOT NULL,
        [Phone]         NVARCHAR(20)    NULL,
        [Company]       NVARCHAR(200)   NULL,
        [CompanySize]   INT             NULL,
        [Message]       NVARCHAR(2000)  NULL,

        -- Status: 1 = New | 2 = Contacted | 3 = Qualified | 4 = Converted | 5 = Lost
        [Status]        INT             NOT NULL    DEFAULT 1,

        -- Audit
        [CreatedBy]     NVARCHAR(100)   NULL,
        [CreatedAt]     DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),
        [UpdatedBy]     NVARCHAR(100)   NULL,
        [UpdatedAt]     DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_Leads]          PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_Leads_Listings] FOREIGN KEY           ([ListingId])   REFERENCES [fin].[Listings] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Leads_Vendors]  FOREIGN KEY           ([VendorId])    REFERENCES [fin].[Vendors]  ([Id]),
        CONSTRAINT [FK_Leads_Users]    FOREIGN KEY           ([BuyerUserId]) REFERENCES [fin].[Users]    ([Id]),
        CONSTRAINT [CK_Leads_LeadType] CHECK                 ([LeadType] BETWEEN 1 AND 4),
        CONSTRAINT [CK_Leads_Status]   CHECK                 ([Status]   BETWEEN 1 AND 5)
    );

    CREATE NONCLUSTERED INDEX [IX_Leads_VendorId_Status]
        ON [fin].[Leads] ([VendorId], [Status]);

    CREATE NONCLUSTERED INDEX [IX_Leads_ListingId]
        ON [fin].[Leads] ([ListingId]);

    CREATE NONCLUSTERED INDEX [IX_Leads_CreatedAt]
        ON [fin].[Leads] ([CreatedAt]);

    PRINT 'Created [fin].[Leads]';
END
GO
