-- ============================================================
--  Findly Database — Categories Table
--  Script : 004_CreateCategories.sql
--  Run    : after 003_CreateUsers.sql
-- ============================================================

USE [FindlyDb];
GO

-- Filtered indexes require these SET options ON at creation time.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Categories' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    CREATE TABLE [fin].[Categories]
    (
        [Id]               INT             NOT NULL    IDENTITY(1,1),

        [Name]             NVARCHAR(150)   NOT NULL,
        [Slug]             NVARCHAR(150)   NOT NULL,
        [Description]      NVARCHAR(1000)  NULL,
        [IconUrl]          NVARCHAR(500)   NULL,
        [ParentCategoryId] INT             NULL,
        [DisplayOrder]     INT             NOT NULL    DEFAULT 0,

        -- Status: 1 = Active | 2 = Inactive
        [Status]           INT             NOT NULL    DEFAULT 1,

        [CreatedBy]        NVARCHAR(100)   NULL,
        [CreatedAt]        DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),
        [UpdatedBy]        NVARCHAR(100)   NULL,
        [UpdatedAt]        DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),

        -- Soft Delete
        [IsDeleted]        BIT             NOT NULL    CONSTRAINT [DF_Categories_IsDeleted] DEFAULT (0),
        [DeletedAt]        DATETIME2       NULL,

        CONSTRAINT [PK_Categories]        PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [CK_Categories_Status] CHECK                 ([Status] IN (1, 2)),
        CONSTRAINT [FK_Categories_Parent] FOREIGN KEY           ([ParentCategoryId]) REFERENCES [fin].[Categories] ([Id])
    );

    CREATE INDEX [IX_Categories_ParentCategoryId] ON [fin].[Categories] ([ParentCategoryId]);
    CREATE INDEX [IX_Categories_Status]           ON [fin].[Categories] ([Status]);

    CREATE UNIQUE INDEX [UX_Categories_Slug] ON [fin].[Categories] ([Slug]) WHERE [IsDeleted] = 0;
    CREATE INDEX [IX_Categories_IsDeleted] ON [fin].[Categories] ([IsDeleted]) WHERE [IsDeleted] = 0;

    PRINT 'Created [fin].[Categories]';
END
GO
