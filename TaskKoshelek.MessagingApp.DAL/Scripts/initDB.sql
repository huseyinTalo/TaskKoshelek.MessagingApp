-- Create database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TaskKoshelek.MessagingApp.DB')
BEGIN
    CREATE DATABASE [TaskKoshelek.MessagingApp.DB]
END
GO

USE [TaskKoshelek.MessagingApp.DB]
GO

-- Create Messages table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Messages')
BEGIN
    CREATE TABLE Messages
    (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Body NVARCHAR(MAX) NOT NULL,
        OrderNumber INT NOT NULL,
        CreatedTime DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    )
END