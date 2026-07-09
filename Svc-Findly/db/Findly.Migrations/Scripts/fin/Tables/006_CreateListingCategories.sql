-- ============================================================
--  Findly Database — ListingCategories Table (Listing <-> Category)
--  Script : 006_CreateListingCategories.sql
--  Run    : after 005_CreateTags.sql
-- ============================================================

USE [FindlyDb];
GO

-- Filtered indexes require these SET options ON at creation time.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ListingCategories' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    CREATE TABLE [fin].[ListingCategories]
    (
        [Id]         INT           NOT NULL    IDENTITY(1,1),
        [ListingId]  INT           NOT NULL,
        [CategoryId] INT           NOT NULL,

        [CreatedBy]  NVARCHAR(100) NULL,
        [CreatedAt]  DATETIME2     NOT NULL    DEFAULT GETUTCDATE(),
        [UpdatedBy]  NVARCHAR(100) NULL,
        [UpdatedAt]  DATETIME2     NOT NULL    DEFAULT GETUTCDATE(),

        -- Soft Delete
        [IsDeleted]  BIT           NOT NULL    CONSTRAINT [DF_ListingCategories_IsDeleted] DEFAULT (0),
        [DeletedAt]  DATETIME2     NULL,

        CONSTRAINT [PK_ListingCategories]          PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_ListingCategories_Listing]  FOREIGN KEY           ([ListingId])  REFERENCES [fin].[Listings]   ([Id]),
        CONSTRAINT [FK_ListingCategories_Category] FOREIGN KEY           ([CategoryId]) REFERENCES [fin].[Categories] ([Id])
    );

    CREATE INDEX [IX_ListingCategories_ListingId]  ON [fin].[ListingCategories] ([ListingId]);
    CREATE INDEX [IX_ListingCategories_CategoryId] ON [fin].[ListingCategories] ([CategoryId]);

    CREATE UNIQUE INDEX [UX_ListingCategories_ListingCat] ON [fin].[ListingCategories] ([ListingId], [CategoryId]) WHERE [IsDeleted] = 0;
    CREATE INDEX [IX_ListingCategories_IsDeleted] ON [fin].[ListingCategories] ([IsDeleted]) WHERE [IsDeleted] = 0;

    PRINT 'Created [fin].[ListingCategories]';
END
GO
