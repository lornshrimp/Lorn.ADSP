
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/02/2014 16:56:26
-- Generated from EDMX file: C:\Users\兵\SkyDrive\Visual Studio\Projects\Lorn.ADSP\Permissions\Lorn.Permissions.DataModels\PermissionsData.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Lorn.ADSP.Identity];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_BusinessWorkerPermission]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Permissions] DROP CONSTRAINT [FK_BusinessWorkerPermission];
GO
IF OBJECT_ID(N'[dbo].[FK_BusinessWorkerRBusinessWorker]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RBusinessWorkers] DROP CONSTRAINT [FK_BusinessWorkerRBusinessWorker];
GO
IF OBJECT_ID(N'[dbo].[FK_RBusinessWorkerBusinessWorkers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RBusinessWorkers] DROP CONSTRAINT [FK_RBusinessWorkerBusinessWorkers];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[BusinessWorkers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BusinessWorkers];
GO
IF OBJECT_ID(N'[dbo].[Permissions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Permissions];
GO
IF OBJECT_ID(N'[dbo].[PreLoadPermissions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PreLoadPermissions];
GO
IF OBJECT_ID(N'[dbo].[RBusinessWorkers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RBusinessWorkers];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'BusinessWorkers'
CREATE TABLE [dbo].[BusinessWorkers] (
    [BusinessWorkerId] uniqueidentifier  NOT NULL,
    [Enabled] bit  NOT NULL
);
GO

-- Creating table 'Permissions'
CREATE TABLE [dbo].[Permissions] (
    [PermissionId] uniqueidentifier  NOT NULL,
    [PermissionName] nvarchar(max)  NULL,
    [ControlBusinessObjectType] int  NOT NULL,
    [ControlBusinessObjectId] uniqueidentifier  NOT NULL,
    [StartDate] datetime  NOT NULL,
    [EndDate] datetime  NULL,
    [IsDeny] bit  NOT NULL,
    [Remark] nvarchar(max)  NOT NULL,
    [ModuleId] uniqueidentifier  NOT NULL,
    [OperationId] uniqueidentifier  NULL,
    [BusinessWorker_BusinessWorkerId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'PreLoadPermissions'
CREATE TABLE [dbo].[PreLoadPermissions] (
    [PreLoadPermissionId] uniqueidentifier  NOT NULL,
    [ControlBusinessObjectType] int  NOT NULL,
    [ControlBusinessObjectId] uniqueidentifier  NOT NULL,
    [ModuleId] uniqueidentifier  NOT NULL,
    [OperationId] uniqueidentifier  NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- Creating table 'RBusinessWorkers'
CREATE TABLE [dbo].[RBusinessWorkers] (
    [RBusinessWokerId] uniqueidentifier  NOT NULL,
    [StartDate] datetime  NOT NULL,
    [EndDate] datetime  NULL,
    [IsDeny] bit  NOT NULL,
    [Remark] nvarchar(max)  NULL,
    [ParentBusinessWorker_BusinessWorkerId] uniqueidentifier  NOT NULL,
    [ChildBusinessWorker_BusinessWorkerId] uniqueidentifier  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [BusinessWorkerId] in table 'BusinessWorkers'
ALTER TABLE [dbo].[BusinessWorkers]
ADD CONSTRAINT [PK_BusinessWorkers]
    PRIMARY KEY CLUSTERED ([BusinessWorkerId] ASC);
GO

-- Creating primary key on [PermissionId] in table 'Permissions'
ALTER TABLE [dbo].[Permissions]
ADD CONSTRAINT [PK_Permissions]
    PRIMARY KEY CLUSTERED ([PermissionId] ASC);
GO

-- Creating primary key on [PreLoadPermissionId] in table 'PreLoadPermissions'
ALTER TABLE [dbo].[PreLoadPermissions]
ADD CONSTRAINT [PK_PreLoadPermissions]
    PRIMARY KEY CLUSTERED ([PreLoadPermissionId] ASC);
GO

-- Creating primary key on [RBusinessWokerId] in table 'RBusinessWorkers'
ALTER TABLE [dbo].[RBusinessWorkers]
ADD CONSTRAINT [PK_RBusinessWorkers]
    PRIMARY KEY CLUSTERED ([RBusinessWokerId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [BusinessWorker_BusinessWorkerId] in table 'Permissions'
ALTER TABLE [dbo].[Permissions]
ADD CONSTRAINT [FK_BusinessWorkerPermission]
    FOREIGN KEY ([BusinessWorker_BusinessWorkerId])
    REFERENCES [dbo].[BusinessWorkers]
        ([BusinessWorkerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_BusinessWorkerPermission'
CREATE INDEX [IX_FK_BusinessWorkerPermission]
ON [dbo].[Permissions]
    ([BusinessWorker_BusinessWorkerId]);
GO

-- Creating foreign key on [ChildBusinessWorker_BusinessWorkerId] in table 'RBusinessWorkers'
ALTER TABLE [dbo].[RBusinessWorkers]
ADD CONSTRAINT [FK_BusinessWorkerRBusinessWorker]
    FOREIGN KEY ([ChildBusinessWorker_BusinessWorkerId])
    REFERENCES [dbo].[BusinessWorkers]
        ([BusinessWorkerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_BusinessWorkerRBusinessWorker'
CREATE INDEX [IX_FK_BusinessWorkerRBusinessWorker]
ON [dbo].[RBusinessWorkers]
    ([ChildBusinessWorker_BusinessWorkerId]);
GO

-- Creating foreign key on [ParentBusinessWorker_BusinessWorkerId] in table 'RBusinessWorkers'
ALTER TABLE [dbo].[RBusinessWorkers]
ADD CONSTRAINT [FK_RBusinessWorkerBusinessWorkers]
    FOREIGN KEY ([ParentBusinessWorker_BusinessWorkerId])
    REFERENCES [dbo].[BusinessWorkers]
        ([BusinessWorkerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RBusinessWorkerBusinessWorkers'
CREATE INDEX [IX_FK_RBusinessWorkerBusinessWorkers]
ON [dbo].[RBusinessWorkers]
    ([ParentBusinessWorker_BusinessWorkerId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------