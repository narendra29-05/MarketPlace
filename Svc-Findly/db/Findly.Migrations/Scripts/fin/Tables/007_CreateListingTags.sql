-- ============================================================
--  Findly Database — ListingTags Table (Listing <-> Tag)
--  Script : 007_CreateListingTags.sql
--  Run    : after 006_CreateListingCategories.sql
-- ============================================================

USE [FindlyDb];
GO

-- Filtered indexes require these SET options ON at creation time.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ListingTags' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    CREATE TABLE [fin].[ListingTags]
    (
        [Id]        INT           NOT NULL    IDENTITY(1,1),
        [ListingId] INT           NOT NULL,
        [TagId]     INT           NOT NULL,

        [CreatedBy] NVARCHAR(100) NULL,
        [CreatedAt] DATETIME2     NOT NULL    DEFAULT GETUTCDATE(),
        [UpdatedBy] NVARCHAR(100) NULL,
        [UpdatedAt] DATETIME2     NOT NULL    DEFAULT GETUTCDATE(),

        -- Soft Delete
        [IsDeleted] BIT           NOT NULL    CONSTRAINT [DF_ListingTags_IsDeleted] DEFAULT (0),
        [DeletedAt] DATETIME2     NULL,

        CONSTRAINT [PK_ListingTags]         PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_ListingTags_Listing] FOREIGN KEY           ([ListingId]) REFERENCES [fin].[Listings] ([Id]),
        CONSTRAINT [FK_ListingTags_Tag]     FOREIGN KEY           ([TagId])     REFERENCES [fin].[Tags]     ([Id])
    );

    CREATE INDEX [IX_ListingTags_ListingId] ON [fin].[ListingTags] ([ListingId]);
    CREATE INDEX [IX_ListingTags_TagId]     ON [fin].[ListingTags] ([TagId]);

    CREATE UNIQUE INDEX [UX_ListingTags_ListingTag] ON [fin].[ListingTags] ([ListingId], [TagId]) WHERE [IsDeleted] = 0;
    CREATE INDEX [IX_ListingTags_IsDeleted] ON [fin].[ListingTags] ([IsDeleted]) WHERE [IsDeleted] = 0;

    PRINT 'Created [fin].[ListingTags]';
END
GO
