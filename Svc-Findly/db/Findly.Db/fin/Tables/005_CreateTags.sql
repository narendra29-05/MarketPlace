-- ============================================================
--  Findly Database — Tags Table
--  Script : 005_CreateTags.sql
--  Run    : after 004_CreateCategories.sql
-- ============================================================

USE [FindlyDb];
GO

-- Filtered indexes require these SET options ON at creation time.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Tags' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    CREATE TABLE [fin].[Tags]
    (
        [Id]        INT           NOT NULL    IDENTITY(1,1),

        [Name]      NVARCHAR(150) NOT NULL,
        [Slug]      NVARCHAR(150) NOT NULL,

        -- Type: 1 Integration | 2 Technology | 3 UseCase | 4 Platform | 5 General
        --       6 Feature | 7 Compliance | 8 Deployment | 9 Language
        [Type]      INT           NOT NULL    DEFAULT 5,

        [CreatedBy] NVARCHAR(100) NULL,
        [CreatedAt] DATETIME2     NOT NULL    DEFAULT GETUTCDATE(),
        [UpdatedBy] NVARCHAR(100) NULL,
        [UpdatedAt] DATETIME2     NOT NULL    DEFAULT GETUTCDATE(),

        -- Soft Delete
        [IsDeleted] BIT           NOT NULL    CONSTRAINT [DF_Tags_IsDeleted] DEFAULT (0),
        [DeletedAt] DATETIME2     NULL,

        CONSTRAINT [PK_Tags]      PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [CK_Tags_Type] CHECK                 ([Type] BETWEEN 1 AND 9)
    );

    CREATE INDEX [IX_Tags_Type] ON [fin].[Tags] ([Type]);

    CREATE UNIQUE INDEX [UX_Tags_Slug] ON [fin].[Tags] ([Slug]) WHERE [IsDeleted] = 0;
    CREATE INDEX [IX_Tags_IsDeleted] ON [fin].[Tags] ([IsDeleted]) WHERE [IsDeleted] = 0;

    PRINT 'Created [fin].[Tags]';
END
GO
