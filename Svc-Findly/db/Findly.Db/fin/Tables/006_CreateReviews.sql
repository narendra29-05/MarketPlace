-- ============================================================
--  Findly Database — Reviews Table
--  Script : 006_CreateReviews.sql
--  Run    : after 005_CreateListingCategories.sql
--  Note   : rating dimensions are Overall, Features,
--           ValueForMoney and CustomerSupport (no ease-of-use).
-- ============================================================

USE [FindlyDb];
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Reviews' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    CREATE TABLE [fin].[Reviews]
    (
        -- Primary Key
        [Id]                    INT             NOT NULL    IDENTITY(1,1),

        -- Foreign Keys
        [ListingId]             INT             NOT NULL,
        [UserId]                INT             NOT NULL,

        -- Ratings (1-5)
        [OverallRating]         INT             NOT NULL,
        [FeaturesRating]        INT             NOT NULL,
        [ValueForMoneyRating]   INT             NOT NULL,
        [CustomerSupportRating] INT             NOT NULL,

        -- Content
        [Title]                 NVARCHAR(200)   NOT NULL,
        [Body]                  NVARCHAR(4000)  NOT NULL,
        [Pros]                  NVARCHAR(2000)  NULL,
        [Cons]                  NVARCHAR(2000)  NULL,

        -- Status: 1 = Pending | 2 = Approved | 3 = Rejected
        [Status]                INT             NOT NULL    DEFAULT 1,
        [RejectionReason]       NVARCHAR(1000)  NULL,

        -- Audit
        [CreatedBy]             NVARCHAR(100)   NULL,
        [CreatedAt]             DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),
        [UpdatedBy]             NVARCHAR(100)   NULL,
        [UpdatedAt]             DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_Reviews]              PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [UQ_Reviews_Listing_User] UNIQUE                ([ListingId], [UserId]),
        CONSTRAINT [FK_Reviews_Listings]     FOREIGN KEY           ([ListingId]) REFERENCES [fin].[Listings] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Reviews_Users]        FOREIGN KEY           ([UserId])    REFERENCES [fin].[Users]    ([Id]),
        CONSTRAINT [CK_Reviews_Status]       CHECK                 ([Status] IN (1, 2, 3)),
        CONSTRAINT [CK_Reviews_Ratings]      CHECK                 ([OverallRating]         BETWEEN 1 AND 5
                                                                AND [FeaturesRating]        BETWEEN 1 AND 5
                                                                AND [ValueForMoneyRating]   BETWEEN 1 AND 5
                                                                AND [CustomerSupportRating] BETWEEN 1 AND 5)
    );

    CREATE NONCLUSTERED INDEX [IX_Reviews_ListingId_Status]
        ON [fin].[Reviews] ([ListingId], [Status]);

    CREATE NONCLUSTERED INDEX [IX_Reviews_Status]
        ON [fin].[Reviews] ([Status]);

    PRINT 'Created [fin].[Reviews]';
END
GO
