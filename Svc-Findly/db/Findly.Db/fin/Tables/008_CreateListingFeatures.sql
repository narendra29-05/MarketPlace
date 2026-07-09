-- ============================================================
--  Findly Database — ListingFeatures Table
--  Script : 008_CreateListingFeatures.sql
--  Run    : after 007_CreateListingTags.sql
-- ============================================================

USE [FindlyDb];
GO

-- Filtered indexes require these SET options ON at creation time.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ListingFeatures' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    CREATE TABLE [fin].[ListingFeatures]
    (
        [Id]            INT           NOT NULL    IDENTITY(1,1),
        [ListingId]     INT           NOT NULL,

        [Name]          NVARCHAR(200) NOT NULL,
        [Description]   NVARCHAR(1000) NULL,
        [IsHighlighted] BIT           NOT NULL    DEFAULT 0,

        [CreatedBy]     NVARCHAR(100) NULL,
        [CreatedAt]     DATETIME2     NOT NULL    DEFAULT GETUTCDATE(),
        [UpdatedBy]     NVARCHAR(100) NULL,
        [UpdatedAt]     DATETIME2     NOT NULL    DEFAULT GETUTCDATE(),

        -- Soft Delete
        [IsDeleted]         BIT             NOT NULL    CONSTRAINT [DF_ListingFeatures_IsDeleted] DEFAULT (0),
        [DeletedAt]         DATETIME2       NULL,

        CONSTRAINT [PK_ListingFeatures]         PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_ListingFeatures_Listing] FOREIGN KEY           ([ListingId]) REFERENCES [fin].[Listings] ([Id])
    );

    CREATE INDEX [IX_ListingFeatures_ListingId] ON [fin].[ListingFeatures] ([ListingId]);

    CREATE INDEX [IX_ListingFeatures_IsDeleted] ON [fin].[ListingFeatures] ([IsDeleted]) WHERE [IsDeleted] = 0;

    PRINT 'Created [fin].[ListingFeatures]';
END
GO
