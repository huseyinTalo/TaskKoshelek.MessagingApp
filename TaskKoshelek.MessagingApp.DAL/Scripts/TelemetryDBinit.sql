-- Create Telemetry Database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MessagingAppTelemetry')
BEGIN
    CREATE DATABASE MessagingAppTelemetry;
END
GO

USE MessagingAppTelemetry;
GO

-- Create OperationLogs table for telemetry logging
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OperationLogs')
BEGIN
    CREATE TABLE OperationLogs (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Operation NVARCHAR(100) NOT NULL,
        Status NVARCHAR(50) NOT NULL,
        Details NVARCHAR(4000) NULL,
        Timestamp DATETIME NOT NULL,
        CorrelationId NVARCHAR(100) NULL,
        ExecutionTimeMs INT NULL
    );
    
    CREATE INDEX IX_OperationLogs_Timestamp ON OperationLogs(Timestamp);
    CREATE INDEX IX_OperationLogs_Operation ON OperationLogs(Operation);
END

-- Create RequestLogs table for HTTP request tracking
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RequestLogs')
BEGIN
    CREATE TABLE RequestLogs (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Path NVARCHAR(500) NOT NULL,
        Method NVARCHAR(10) NOT NULL,
        StatusCode INT NOT NULL,
        StartTime DATETIME NOT NULL,
        EndTime DATETIME NOT NULL,
        DurationMs INT NOT NULL,
        IpAddress NVARCHAR(50) NULL,
        RequestBody NVARCHAR(MAX) NULL,
        ResponseBody NVARCHAR(MAX) NULL,
        CorrelationId NVARCHAR(100) NULL
    );
    
    CREATE INDEX IX_RequestLogs_StartTime ON RequestLogs(StartTime);
    CREATE INDEX IX_RequestLogs_Path ON RequestLogs(Path);
    CREATE INDEX IX_RequestLogs_StatusCode ON RequestLogs(StatusCode);
END

-- Create PerformanceMetrics table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PerformanceMetrics')
BEGIN
    CREATE TABLE PerformanceMetrics (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        MetricName NVARCHAR(100) NOT NULL,
        Value FLOAT NOT NULL,
        Unit NVARCHAR(50) NULL,
        Timestamp DATETIME NOT NULL,
        Context NVARCHAR(500) NULL
    );
    
    CREATE INDEX IX_PerformanceMetrics_Timestamp ON PerformanceMetrics(Timestamp);
    CREATE INDEX IX_PerformanceMetrics_MetricName ON PerformanceMetrics(MetricName);
END