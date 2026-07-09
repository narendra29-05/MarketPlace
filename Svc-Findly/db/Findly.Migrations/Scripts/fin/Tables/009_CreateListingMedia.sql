-- ============================================================
--  Findly Database — ListingMedia Table
--  Script : 009_CreateListingMedia.sql
--  Run    : after 008_CreateListingFeatures.sql
-- ============================================================

USE [FindlyDb];
GO

-- Filtered indexes require these SET options ON at creation time.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ListingMedia' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    CREATE TABLE [fin].[ListingMedia]
    (
        [Id]           INT           NOT NULL    IDENTITY(1,1),
        [ListingId]    INT           NOT NULL,

        -- Type: 1 = Image | 2 = Video
        [Type]         INT           NOT NULL    DEFAULT 1,
        [Url]          NVARCHAR(500) NOT NULL,
        [Caption]      NVARCHAR(300) NULL,
        [DisplayOrder] INT           NOT NULL    DEFAULT 0,

        [CreatedBy]    NVARCHAR(100) NULL,
        [CreatedAt]    DATETIME2     NOT NULL    DEFAULT GETUTCDATE(),
        [UpdatedBy]    NVARCHAR(100) NULL,
        [UpdatedAt]    DATETIME2     NOT NULL    DEFAULT GETUTCDATE(),

        -- Soft Delete
        [IsDeleted]         BIT             NOT NULL    CONSTRAINT [DF_ListingMedia_IsDeleted] DEFAULT (0),
        [DeletedAt]         DATETIME2       NULL,

        CONSTRAINT [PK_ListingMedia]         PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_ListingMedia_Listing] FOREIGN KEY           ([ListingId]) REFERENCES [fin].[Listings] ([Id]),
        CONSTRAINT [CK_ListingMedia_Type]    CHECK                 ([Type] IN (1, 2))
    );

    CREATE INDEX [IX_ListingMedia_ListingId] ON [fin].[ListingMedia] ([ListingId]);

    CREATE INDEX [IX_ListingMedia_IsDeleted] ON [fin].[ListingMedia] ([IsDeleted]) WHERE [IsDeleted] = 0;

    PRINT 'Created [fin].[ListingMedia]';
END
GO
