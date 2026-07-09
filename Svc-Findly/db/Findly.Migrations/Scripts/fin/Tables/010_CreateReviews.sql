-- ============================================================
--  Findly Database — Reviews Table
--  Script : 010_CreateReviews.sql
--  Run    : after 009_CreateListingMedia.sql
-- ============================================================

USE [FindlyDb];
GO

-- Filtered indexes require these SET options ON at creation time.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Reviews' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    CREATE TABLE [fin].[Reviews]
    (
        [Id]                    INT             NOT NULL    IDENTITY(1,1),
        [ListingId]             INT             NOT NULL,
        [UserId]                INT             NOT NULL,

        [Title]                 NVARCHAR(200)   NOT NULL,
        [Pros]                  NVARCHAR(2000)  NULL,
        [Cons]                  NVARCHAR(2000)  NULL,
        [Comment]               NVARCHAR(MAX)   NULL,

        [OverallRating]         DECIMAL(3,2)    NOT NULL,
        [FeaturesRating]        DECIMAL(3,2)    NOT NULL,
        [CustomerSupportRating] DECIMAL(3,2)    NOT NULL,

        -- Status: 1 = Pending | 2 = Published | 3 = Rejected
        [Status]                INT             NOT NULL    DEFAULT 1,
        [RejectionReason]       NVARCHAR(1000)  NULL,

        [CreatedBy]             NVARCHAR(100)   NULL,
        [CreatedAt]             DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),
        [UpdatedBy]             NVARCHAR(100)   NULL,
        [UpdatedAt]             DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),

        -- Soft Delete
        [IsDeleted]         BIT             NOT NULL    CONSTRAINT [DF_Reviews_IsDeleted] DEFAULT (0),
        [DeletedAt]         DATETIME2       NULL,

        CONSTRAINT [PK_Reviews]              PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_Reviews_Listing]      FOREIGN KEY           ([ListingId]) REFERENCES [fin].[Listings] ([Id]),
        CONSTRAINT [FK_Reviews_User]         FOREIGN KEY           ([UserId])    REFERENCES [fin].[Users]    ([Id]),
        CONSTRAINT [CK_Reviews_Status]       CHECK                 ([Status] IN (1, 2, 3)),
        CONSTRAINT [CK_Reviews_Overall]      CHECK                 ([OverallRating]         BETWEEN 1 AND 5),
        CONSTRAINT [CK_Reviews_Features]     CHECK                 ([FeaturesRating]        BETWEEN 1 AND 5),
        CONSTRAINT [CK_Reviews_Support]      CHECK                 ([CustomerSupportRating] BETWEEN 1 AND 5)
    );

    CREATE INDEX [IX_Reviews_ListingId] ON [fin].[Reviews] ([ListingId]);
    CREATE INDEX [IX_Reviews_UserId]    ON [fin].[Reviews] ([UserId]);
    CREATE INDEX [IX_Reviews_Status]    ON [fin].[Reviews] ([Status]);

    CREATE UNIQUE INDEX [UX_Reviews_ListingUser] ON [fin].[Reviews] ([ListingId], [UserId]) WHERE [IsDeleted] = 0;
    CREATE INDEX [IX_Reviews_IsDeleted] ON [fin].[Reviews] ([IsDeleted]) WHERE [IsDeleted] = 0;

    PRINT 'Created [fin].[Reviews]';
END
GO
