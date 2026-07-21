-- ============================================================
--  Findly Database — Categories Table
--  Script : 004_CreateCategories.sql
--  Run    : after 003_CreateUsers.sql
-- ============================================================

USE [FindlyDb];
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Categories' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    CREATE TABLE [fin].[Categories]
    (
        -- Primary Key
        [Id]            INT             NOT NULL    IDENTITY(1,1),

        [Name]          NVARCHAR(100)   NOT NULL,
        [Slug]          NVARCHAR(100)   NOT NULL,
        [Description]   NVARCHAR(500)   NULL,
        [IconUrl]       NVARCHAR(500)   NULL,
        [IsActive]      BIT             NOT NULL    DEFAULT 1,

        -- Audit
        [CreatedBy]     NVARCHAR(100)   NULL,
        [CreatedAt]     DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),
        [UpdatedBy]     NVARCHAR(100)   NULL,
        [UpdatedAt]     DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_Categories]      PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [UQ_Categories_Name] UNIQUE                ([Name]),
        CONSTRAINT [UQ_Categories_Slug] UNIQUE                ([Slug])
    );

    PRINT 'Created [fin].[Categories]';
END
GO
