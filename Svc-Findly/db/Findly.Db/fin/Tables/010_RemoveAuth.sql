-- ============================================================
--  Findly Database — Remove auth/users (auth deferred for now)
--  Script : 010_RemoveAuth.sql
--  Run    : after 009_SeedAdminUser.sql
--
--  Reviews keep reviewer identity inline (name + email) instead
--  of a Users FK. Leads lose the optional buyer-user link.
--  The Users table is dropped entirely.
-- ============================================================

USE [FindlyDb];
GO

-- Reviews: add inline reviewer identity
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('fin.Reviews') AND name = 'UserId')
BEGIN
    ALTER TABLE [fin].[Reviews] ADD
        [ReviewerName]  NVARCHAR(200) NOT NULL CONSTRAINT [DF_Reviews_ReviewerName]  DEFAULT N'',
        [ReviewerEmail] NVARCHAR(255) NOT NULL CONSTRAINT [DF_Reviews_ReviewerEmail] DEFAULT N'';
END
GO

-- Backfill reviewer identity from Users before the link is dropped
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('fin.Reviews') AND name = 'UserId')
   AND EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Users' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    EXEC('UPDATE r
          SET r.ReviewerName  = u.FirstName + N'' '' + u.LastName,
              r.ReviewerEmail = u.Email
          FROM [fin].[Reviews] r
          JOIN [fin].[Users]   u ON u.Id = r.UserId');
END
GO

-- Reviews: drop the Users link, re-key uniqueness on reviewer email
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('fin.Reviews') AND name = 'UserId')
BEGIN
    ALTER TABLE [fin].[Reviews] DROP CONSTRAINT [UQ_Reviews_Listing_User];
    ALTER TABLE [fin].[Reviews] DROP CONSTRAINT [FK_Reviews_Users];
    ALTER TABLE [fin].[Reviews] DROP COLUMN [UserId];
    ALTER TABLE [fin].[Reviews] ADD CONSTRAINT [UQ_Reviews_Listing_Reviewer] UNIQUE ([ListingId], [ReviewerEmail]);

    PRINT 'Reviews re-keyed to inline reviewer identity';
END
GO

-- Leads: drop the optional buyer-user link
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('fin.Leads') AND name = 'BuyerUserId')
BEGIN
    ALTER TABLE [fin].[Leads] DROP CONSTRAINT [FK_Leads_Users];
    ALTER TABLE [fin].[Leads] DROP COLUMN [BuyerUserId];

    PRINT 'Dropped Leads.BuyerUserId';
END
GO

-- Users table goes away entirely
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Users' AND schema_id = SCHEMA_ID('fin'))
BEGIN
    DROP TABLE [fin].[Users];
    PRINT 'Dropped [fin].[Users]';
END
GO
