-- ============================================================
--  Findly Database — ReviewVotes Table (helpful / not helpful)
--  Script : 012_CreateReviewVotes.sql
--  Run    : after 011_CreateReviewResponses.sql
-- ============================================================

USE [FindlyDb];
GO

-- Filtered indexes require these SET options ON at creation time.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ReviewVotes' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    CREATE TABLE [fin].[ReviewVotes]
    (
        [Id]        INT           NOT NULL    IDENTITY(1,1),
        [ReviewId]  INT           NOT NULL,
        [UserId]    INT           NOT NULL,

        -- Vote: 1 = Helpful | 2 = NotHelpful
        [Vote]      INT           NOT NULL,

        [CreatedBy] NVARCHAR(100) NULL,
        [CreatedAt] DATETIME2     NOT NULL    DEFAULT GETUTCDATE(),
        [UpdatedBy] NVARCHAR(100) NULL,
        [UpdatedAt] DATETIME2     NOT NULL    DEFAULT GETUTCDATE(),

        -- Soft Delete
        [IsDeleted]         BIT             NOT NULL    CONSTRAINT [DF_ReviewVotes_IsDeleted] DEFAULT (0),
        [DeletedAt]         DATETIME2       NULL,

        CONSTRAINT [PK_ReviewVotes]          PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_ReviewVotes_Review]   FOREIGN KEY           ([ReviewId]) REFERENCES [fin].[Reviews] ([Id]),
        CONSTRAINT [FK_ReviewVotes_User]     FOREIGN KEY           ([UserId])   REFERENCES [fin].[Users]   ([Id]),
        CONSTRAINT [CK_ReviewVotes_Vote]     CHECK                 ([Vote] IN (1, 2))
    );

    CREATE INDEX [IX_ReviewVotes_ReviewId] ON [fin].[ReviewVotes] ([ReviewId]);

    CREATE UNIQUE INDEX [UX_ReviewVotes_ReviewUser] ON [fin].[ReviewVotes] ([ReviewId], [UserId]) WHERE [IsDeleted] = 0;
    CREATE INDEX [IX_ReviewVotes_IsDeleted] ON [fin].[ReviewVotes] ([IsDeleted]) WHERE [IsDeleted] = 0;

    PRINT 'Created [fin].[ReviewVotes]';
END
GO
