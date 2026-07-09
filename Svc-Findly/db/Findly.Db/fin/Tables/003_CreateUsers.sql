-- ============================================================
--  Findly Database — Users Table
--  Script : 003_CreateUsers.sql
--  Run    : after 002_CreateListings.sql
-- ============================================================

USE [FindlyDb];
GO

-- Filtered indexes require these SET options ON at creation time.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Users' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    CREATE TABLE [fin].[Users]
    (
        [Id]              INT             NOT NULL    IDENTITY(1,1),

        [FirstName]       NVARCHAR(100)   NOT NULL,
        [LastName]        NVARCHAR(100)   NOT NULL,
        [Email]           NVARCHAR(255)   NOT NULL,
        [PasswordHash]    NVARCHAR(500)   NOT NULL,
        [AvatarUrl]       NVARCHAR(500)   NULL,
        [JobTitle]        NVARCHAR(150)   NULL,
        [CompanyName]     NVARCHAR(200)   NULL,
        [IndustryType]    INT             NULL,

        -- Role: 1 = Buyer | 2 = Vendor | 3 = Admin
        [Role]            INT             NOT NULL    DEFAULT 1,
        [IsEmailVerified] BIT             NOT NULL    DEFAULT 0,

        [CreatedBy]       NVARCHAR(100)   NULL,
        [CreatedAt]       DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),
        [UpdatedBy]       NVARCHAR(100)   NULL,
        [UpdatedAt]       DATETIME2       NOT NULL    DEFAULT GETUTCDATE(),

        -- Soft Delete
        [IsDeleted]       BIT             NOT NULL    CONSTRAINT [DF_Users_IsDeleted] DEFAULT (0),
        [DeletedAt]       DATETIME2       NULL,

        CONSTRAINT [PK_Users]              PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [CK_Users_Role]         CHECK                 ([Role] IN (1, 2, 3)),
        CONSTRAINT [CK_Users_IndustryType] CHECK                 ([IndustryType] IS NULL OR [IndustryType] BETWEEN 1 AND 16)
    );

    CREATE INDEX [IX_Users_Role] ON [fin].[Users] ([Role]);

    CREATE UNIQUE INDEX [UX_Users_Email] ON [fin].[Users] ([Email]) WHERE [IsDeleted] = 0;
    CREATE INDEX [IX_Users_IsDeleted] ON [fin].[Users] ([IsDeleted]) WHERE [IsDeleted] = 0;

    PRINT 'Created [fin].[Users]';
END
GO
