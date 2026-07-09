-- ============================================================
--  Findly Database — Bookmarks Table (User saves a Listing)
--  Script : 014_CreateBookmarks.sql
--  Run    : after 013_CreateLeads.sql
-- ============================================================

USE [FindlyDb];
GO

-- Filtered indexes require these SET options ON at creation time.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Bookmarks' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    CREATE TABLE [fin].[Bookmarks]
    (
        [Id]        INT           NOT NULL    IDENTITY(1,1),
        [UserId]    INT           NOT NULL,
        [ListingId] INT           NOT NULL,

        [CreatedBy] NVARCHAR(100) NULL,
        [CreatedAt] DATETIME2     NOT NULL    DEFAULT GETUTCDATE(),
        [UpdatedBy] NVARCHAR(100) NULL,
        [UpdatedAt] DATETIME2     NOT NULL    DEFAULT GETUTCDATE(),

        -- Soft Delete
        [IsDeleted]         BIT             NOT NULL    CONSTRAINT [DF_Bookmarks_IsDeleted] DEFAULT (0),
        [DeletedAt]         DATETIME2       NULL,

        CONSTRAINT [PK_Bookmarks]          PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_Bookmarks_User]     FOREIGN KEY           ([UserId])    REFERENCES [fin].[Users]    ([Id]),
        CONSTRAINT [FK_Bookmarks_Listing]  FOREIGN KEY           ([ListingId]) REFERENCES [fin].[Listings] ([Id])
    );

    CREATE INDEX [IX_Bookmarks_UserId]    ON [fin].[Bookmarks] ([UserId]);
    CREATE INDEX [IX_Bookmarks_ListingId] ON [fin].[Bookmarks] ([ListingId]);

    CREATE UNIQUE INDEX [UX_Bookmarks_UserList] ON [fin].[Bookmarks] ([UserId], [ListingId]) WHERE [IsDeleted] = 0;
    CREATE INDEX [IX_Bookmarks_IsDeleted] ON [fin].[Bookmarks] ([IsDeleted]) WHERE [IsDeleted] = 0;

    PRINT 'Created [fin].[Bookmarks]';
END
GO
