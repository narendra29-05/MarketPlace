-- ============================================================
--  Findly Database — Create Database
--  Script : 000_CreateDatabase.sql
-- ============================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'FindlyDb')
BEGIN
    CREATE DATABASE FindlyDb;
END
GO

USE FindlyDb;
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'fin')
BEGIN
    EXEC('CREATE SCHEMA [fin]');
END
GO
