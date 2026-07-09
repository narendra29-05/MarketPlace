-- ============================================================
--  Findly Database — Listings Table
--  Script : 002_CreateListings.sql
--  Run    : after 001_CreateVendors.sql
-- ============================================================

USE [FindlyDb];
GO

-- Filtered indexes require these SET options ON at creation time.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Listings' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    CREATE TABLE [fin].[Listings]
    (
        -- Primary Key
        [Id]                    INT             NOT NULL    IDENTITY(1,1),
        -- Foreign Key
        [VendorId]              INT             NOT NULL,
        -- Identity
        [Name]                  NVARCHAR(200)   NOT NULL,
        [Slug]                  NVARCHAR(200)   NOT NULL,
        [Tagline]               NVARCHAR(300)   NULL,
        [ShortDescription]      NVARCHAR(500)   NOT NULL,
        [Description]           NVARCHAR(MAX)   NULL,
        [LogoUrl]               NVARCHAR(500)   NULL,
        [WebsiteUrl]            NVARCHAR(500)   NOT NULL,
        [DemoUrl]               NVARCHAR(500)   NULL,
        [FoundedYear]           INT             NULL,

        -- Pricing: 1 = Free | 2 = Freemium | 3 = Paid | 4 = ContactVendor
        [PricingType]           INT             NOT NULL,
        [StartingPrice]         DECIMAL(18,2)   NULL,
        [PricePerUser]          DECIMAL(18,2)   NULL,
        [HasFreeTrial]          BIT             NOT NULL    DEFAULT 0,
        [FreeTrialDays]         INT             NULL,

        -- Ratings
        [AverageRating]         DECIMAL(3,2)    NOT NULL    DEFAULT 0,
        [FeaturesRating]        DECIMAL(3,2)    NOT NULL    DEFAULT 0,
        [CustomerSupportRating] DECIMAL(3,2)    NOT NULL    DEFAULT 0,
        [ReviewCount]           INT             NOT NULL    DEFAULT 0,

        -- Status: 1 = Pending | 2 = Published | 3 = Rejected | 4 = Archived
        [Status]                INT             NOT NULL    DEFAULT 1,
        [RejectionReason]       NVARCHAR(1000)  NULL,

        -- Audit
        [CreatedBy]             NVARCHAR(100)   NULL,
        [CreatedAt]             DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),
        [UpdatedBy]             NVARCHAR(100)   NULL,
        [UpdatedAt]             DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),

        -- Soft Delete
        [IsDeleted]             BIT             NOT NULL    CONSTRAINT [DF_Listings_IsDeleted] DEFAULT (0),
        [DeletedAt]             DATETIME2       NULL,

        CONSTRAINT [PK_Listings]                PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_Listings_Vendors]        FOREIGN KEY           ([VendorId]) REFERENCES [fin].[Vendors] ([Id]),
        CONSTRAINT [CK_Listings_Status]         CHECK                 ([Status] IN (1, 2, 3, 4)),
        CONSTRAINT [CK_Listings_PricingType]    CHECK                 ([PricingType] BETWEEN 1 AND 4),
        CONSTRAINT [CK_Listings_AverageRating]  CHECK                 ([AverageRating] BETWEEN 0 AND 5),
        CONSTRAINT [CK_Listings_FreeTrialDays]  CHECK                 ([FreeTrialDays] IS NULL OR [FreeTrialDays] > 0)
    );

    CREATE NONCLUSTERED INDEX [IX_Listings_VendorId]
        ON [fin].[Listings] ([VendorId]);

    CREATE NONCLUSTERED INDEX [IX_Listings_Status]
        ON [fin].[Listings] ([Status]);

    CREATE NONCLUSTERED INDEX [IX_Listings_PricingType]
        ON [fin].[Listings] ([PricingType]);

    CREATE UNIQUE INDEX [UX_Listings_Slug] ON [fin].[Listings] ([Slug]) WHERE [IsDeleted] = 0;
    CREATE INDEX [IX_Listings_IsDeleted] ON [fin].[Listings] ([IsDeleted]) WHERE [IsDeleted] = 0;

    PRINT 'Created [fin].[Listings]';
END
GO
