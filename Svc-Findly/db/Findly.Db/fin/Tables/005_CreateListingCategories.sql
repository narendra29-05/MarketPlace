-- ============================================================
--  Findly Database — ListingCategories Join Table
--  Script : 005_CreateListingCategories.sql
--  Run    : after 004_CreateCategories.sql
-- ============================================================

USE [FindlyDb];
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ListingCategories' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    CREATE TABLE [fin].[ListingCategories]
    (
        [ListingId]     INT             NOT NULL,
        [CategoryId]    INT             NOT NULL,
        [CreatedAt]     DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_ListingCategories]            PRIMARY KEY CLUSTERED ([ListingId], [CategoryId]),
        CONSTRAINT [FK_ListingCategories_Listings]   FOREIGN KEY ([ListingId])  REFERENCES [fin].[Listings]   ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ListingCategories_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [fin].[Categories] ([Id]) ON DELETE CASCADE
    );

    CREATE NONCLUSTERED INDEX [IX_ListingCategories_CategoryId]
        ON [fin].[ListingCategories] ([CategoryId]);

    PRINT 'Created [fin].[ListingCategories]';
END
GO
