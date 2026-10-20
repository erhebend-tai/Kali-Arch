-- Script001_InitialSchema.sql
-- DbUp script naming: ScriptNNN_Description.sql — applied in order,
-- recorded in the SchemaVersions journal table.

CREATE TABLE [dbo].[VirtualMachines] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(63) NOT NULL,
    [Region] NVARCHAR(64) NOT NULL,
    [Size] NVARCHAR(32) NOT NULL,
    [OsType] INT NOT NULL,
    [Status] INT NOT NULL,
    [ResourceGroup] NVARCHAR(256) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(256) NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(256) NULL
);

CREATE TABLE [dbo].[OutboxMessages] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [Type] NVARCHAR(512) NOT NULL,
    [Payload] NVARCHAR(MAX) NOT NULL,
    [OccurredAt] DATETIME2 NOT NULL,
    [ProcessedAt] DATETIME2 NULL,
    [Error] NVARCHAR(2048) NULL
);

CREATE TABLE [dbo].[InboxMessages] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [EventId] NVARCHAR(512) NOT NULL UNIQUE,
    [EventType] NVARCHAR(512) NOT NULL,
    [Payload] NVARCHAR(MAX) NOT NULL,
    [ReceivedAt] DATETIME2 NOT NULL,
    [ProcessedAt] DATETIME2 NULL,
    [Error] NVARCHAR(2048) NULL,
    [AttemptCount] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL,
    [CreatedBy] NVARCHAR(256) NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(256) NULL
);
