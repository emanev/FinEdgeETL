IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'FinEdgeAnalytics')
BEGIN
    CREATE DATABASE FinEdgeAnalytics;
END
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SourceDB')
BEGIN
    CREATE DATABASE SourceDB;
END
GO

USE FinEdgeAnalytics;
GO

IF OBJECT_ID('dbo.Transactions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Transactions (
        Id INT PRIMARY KEY,
        CustomerId INT NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        TransactionDate DATETIME NOT NULL
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.types WHERE name = 'TransactionType')
BEGIN
    CREATE TYPE dbo.TransactionType AS TABLE
    (
        Id INT PRIMARY KEY,
        CustomerId INT,
        Amount DECIMAL(18,2),
        TransactionDate DATETIME
    );
END
GO

CREATE OR ALTER PROCEDURE dbo.UpsertTransaction
    @TransactionTable TransactionType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    MERGE INTO dbo.Transactions AS target
    USING @TransactionTable AS source
    ON target.Id = source.Id
    WHEN MATCHED THEN 
        UPDATE SET 
            target.CustomerId = source.CustomerId,
            target.Amount = source.Amount,
            target.TransactionDate = source.TransactionDate
    WHEN NOT MATCHED THEN
        INSERT (Id, CustomerId, Amount, TransactionDate)
        VALUES (source.Id, source.CustomerId, source.Amount, source.TransactionDate);
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Transactions WHERE Id = 1)
BEGIN
    INSERT INTO dbo.Transactions (Id, CustomerId, Amount, TransactionDate)
    VALUES 
        (1, 1001, 250.50, '2024-02-20 14:30:00'),
        (2, 1002, 125.00, '2024-02-21 10:15:00'),
        (3, 1003, 75.75, '2024-02-22 09:45:00');
END
GO

USE SourceDB;
GO

IF OBJECT_ID('dbo.SourceTransactions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.SourceTransactions (
        Id INT PRIMARY KEY,
        CustomerId INT NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        TransactionDate DATETIME NOT NULL
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SourceTransactions WHERE Id = 10)
BEGIN
    INSERT INTO dbo.SourceTransactions (Id, CustomerId, Amount, TransactionDate)
    VALUES 
        (10, 1001, 300.75, '2024-02-25 14:00:00'),
		(11, 2001, 410.00, '2024-02-25 14:00:00'),
        (12, 2002, 150.25, '2024-02-26 09:15:00'),
        (13, 2003, 500.00, '2024-02-27 18:45:00');
END
GO

USE FinEdgeAnalytics;
SELECT * FROM dbo.Transactions;
GO

USE SourceDB;
SELECT * FROM dbo.SourceTransactions;
GO
