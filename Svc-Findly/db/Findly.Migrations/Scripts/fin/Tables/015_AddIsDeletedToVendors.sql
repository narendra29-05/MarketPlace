-- ============================================================
--  Findly Database — Add soft-delete columns to Vendors
--  Script : 015_AddIsDeletedToVendors.sql
--  Run    : after 014_CreateBookmarks.sql
--  Reason : Vendors are referenced by Listings/Leads (FK, no
--           cascade), so a hard DELETE fails once a vendor owns
--           any child rows. Switch to soft delete instead.
-- ============================================================

USE [FindlyDb];
GO

-- Filtered indexes require these SET options to be ON at creation time.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF COL_LENGTH('fin.Vendors', 'IsDeleted') IS NULL
BEGIN
    ALTER TABLE [fin].[Vendors]
        ADD [IsDeleted] BIT NOT NULL CONSTRAINT [DF_Vendors_IsDeleted] DEFAULT (0);

    PRINT 'Added [fin].[Vendors].[IsDeleted]';
END
GO

IF COL_LENGTH('fin.Vendors', 'DeletedAt') IS NULL
BEGIN
    ALTER TABLE [fin].[Vendors]
        ADD [DeletedAt] DATETIME2 NULL;

    PRINT 'Added [fin].[Vendors].[DeletedAt]';
END
GO

-- Filtered index so the common "active vendors" reads stay cheap.
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Vendors_IsDeleted' AND object_id = OBJECT_ID('fin.Vendors'))
BEGIN
    CREATE INDEX [IX_Vendors_IsDeleted]
        ON [fin].[Vendors] ([IsDeleted])
        WHERE [IsDeleted] = 0;

    PRINT 'Created index [IX_Vendors_IsDeleted]';
END
GO
