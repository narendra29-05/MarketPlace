-- ============================================================
--  Findly Database — Seed Software Categories
--  Script : 008_SeedCategories.sql
--  Run    : after 007_CreateLeads.sql
-- ============================================================

USE [FindlyDb];
GO

IF NOT EXISTS (SELECT 1 FROM [fin].[Categories] WHERE [Slug] = 'crm')
    INSERT INTO [fin].[Categories] ([Name], [Slug], [Description], [CreatedBy])
    VALUES (N'CRM', N'crm', N'Customer relationship management software', N'migration');

IF NOT EXISTS (SELECT 1 FROM [fin].[Categories] WHERE [Slug] = 'project-management')
    INSERT INTO [fin].[Categories] ([Name], [Slug], [Description], [CreatedBy])
    VALUES (N'Project Management', N'project-management', N'Planning, tracking and collaboration tools', N'migration');

IF NOT EXISTS (SELECT 1 FROM [fin].[Categories] WHERE [Slug] = 'accounting')
    INSERT INTO [fin].[Categories] ([Name], [Slug], [Description], [CreatedBy])
    VALUES (N'Accounting', N'accounting', N'Bookkeeping, invoicing and financial reporting', N'migration');

IF NOT EXISTS (SELECT 1 FROM [fin].[Categories] WHERE [Slug] = 'hr')
    INSERT INTO [fin].[Categories] ([Name], [Slug], [Description], [CreatedBy])
    VALUES (N'HR', N'hr', N'Human resources, payroll and talent management', N'migration');

IF NOT EXISTS (SELECT 1 FROM [fin].[Categories] WHERE [Slug] = 'marketing-automation')
    INSERT INTO [fin].[Categories] ([Name], [Slug], [Description], [CreatedBy])
    VALUES (N'Marketing Automation', N'marketing-automation', N'Campaigns, email marketing and lead nurturing', N'migration');

IF NOT EXISTS (SELECT 1 FROM [fin].[Categories] WHERE [Slug] = 'help-desk')
    INSERT INTO [fin].[Categories] ([Name], [Slug], [Description], [CreatedBy])
    VALUES (N'Help Desk', N'help-desk', N'Customer support and ticketing systems', N'migration');

IF NOT EXISTS (SELECT 1 FROM [fin].[Categories] WHERE [Slug] = 'e-commerce')
    INSERT INTO [fin].[Categories] ([Name], [Slug], [Description], [CreatedBy])
    VALUES (N'E-Commerce', N'e-commerce', N'Online store platforms and selling tools', N'migration');

IF NOT EXISTS (SELECT 1 FROM [fin].[Categories] WHERE [Slug] = 'analytics-bi')
    INSERT INTO [fin].[Categories] ([Name], [Slug], [Description], [CreatedBy])
    VALUES (N'Analytics & BI', N'analytics-bi', N'Business intelligence, dashboards and reporting', N'migration');

IF NOT EXISTS (SELECT 1 FROM [fin].[Categories] WHERE [Slug] = 'communication')
    INSERT INTO [fin].[Categories] ([Name], [Slug], [Description], [CreatedBy])
    VALUES (N'Communication', N'communication', N'Team chat, video conferencing and messaging', N'migration');

IF NOT EXISTS (SELECT 1 FROM [fin].[Categories] WHERE [Slug] = 'cybersecurity')
    INSERT INTO [fin].[Categories] ([Name], [Slug], [Description], [CreatedBy])
    VALUES (N'Cybersecurity', N'cybersecurity', N'Security, identity and threat protection', N'migration');

IF NOT EXISTS (SELECT 1 FROM [fin].[Categories] WHERE [Slug] = 'devops')
    INSERT INTO [fin].[Categories] ([Name], [Slug], [Description], [CreatedBy])
    VALUES (N'DevOps', N'devops', N'CI/CD, monitoring and infrastructure tooling', N'migration');

IF NOT EXISTS (SELECT 1 FROM [fin].[Categories] WHERE [Slug] = 'design')
    INSERT INTO [fin].[Categories] ([Name], [Slug], [Description], [CreatedBy])
    VALUES (N'Design', N'design', N'Graphic, UI/UX and prototyping tools', N'migration');
GO
