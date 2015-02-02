
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 09/01/2014 11:03:44
-- Generated from EDMX file: C:\Users\Bill\OneDrive\Visual Studio\Projects\Lorn.ADSP\Common\Lorn.ADSP.Common.Data\AdInfrastructureData.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Lorn.ADSP];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_AdPositionAdPositionAspectRatio_AdPosition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdPositionAdPositionAspectRatio] DROP CONSTRAINT [FK_AdPositionAdPositionAspectRatio_AdPosition];
GO
IF OBJECT_ID(N'[dbo].[FK_AdPositionAdPositionAspectRatio_AdPositionAspectRatio]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdPositionAdPositionAspectRatio] DROP CONSTRAINT [FK_AdPositionAdPositionAspectRatio_AdPositionAspectRatio];
GO
IF OBJECT_ID(N'[dbo].[FK_AdPositionAdPositionGroup_AdPosition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdPositionAdPositionGroup] DROP CONSTRAINT [FK_AdPositionAdPositionGroup_AdPosition];
GO
IF OBJECT_ID(N'[dbo].[FK_AdPositionAdPositionGroup_AdPositionGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdPositionAdPositionGroup] DROP CONSTRAINT [FK_AdPositionAdPositionGroup_AdPositionGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_AdPositionAdTemplateRelationship]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdTemplateRelationships] DROP CONSTRAINT [FK_AdPositionAdTemplateRelationship];
GO
IF OBJECT_ID(N'[dbo].[FK_AdPositionSizeAdPosition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdPositions] DROP CONSTRAINT [FK_AdPositionSizeAdPosition];
GO
IF OBJECT_ID(N'[dbo].[FK_MediaAdPosition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdPositions] DROP CONSTRAINT [FK_MediaAdPosition];
GO
IF OBJECT_ID(N'[dbo].[FK_AdTemplateAdTemplateRelationship]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdTemplateRelationships] DROP CONSTRAINT [FK_AdTemplateAdTemplateRelationship];
GO
IF OBJECT_ID(N'[dbo].[FK_CreativeTypeCreativeTypeFormDefinition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CreativeTypeFormDefinitions] DROP CONSTRAINT [FK_CreativeTypeCreativeTypeFormDefinition];
GO
IF OBJECT_ID(N'[dbo].[FK_MaterialFormatCreativeTypeFormDefinition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CreativeTypeFormDefinitions] DROP CONSTRAINT [FK_MaterialFormatCreativeTypeFormDefinition];
GO
IF OBJECT_ID(N'[dbo].[FK_CreativeTypeAdTemplateRelationship]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdTemplateRelationships] DROP CONSTRAINT [FK_CreativeTypeAdTemplateRelationship];
GO
IF OBJECT_ID(N'[dbo].[FK_RedirectConditionDefinitionAdTemplateRelationship]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdTemplateRelationships] DROP CONSTRAINT [FK_RedirectConditionDefinitionAdTemplateRelationship];
GO
IF OBJECT_ID(N'[dbo].[FK_RedirectConditionDefinitionRedirectConditionDefinition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RedirectConditionDefinitions] DROP CONSTRAINT [FK_RedirectConditionDefinitionRedirectConditionDefinition];
GO
IF OBJECT_ID(N'[dbo].[FK_RedirectDimensionRedirectConditionDefinition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RedirectConditionDefinitions] DROP CONSTRAINT [FK_RedirectDimensionRedirectConditionDefinition];
GO
IF OBJECT_ID(N'[dbo].[FK_AdPositionCreativeType_AdPosition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdPositionCreativeType] DROP CONSTRAINT [FK_AdPositionCreativeType_AdPosition];
GO
IF OBJECT_ID(N'[dbo].[FK_AdPositionCreativeType_CreativeType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdPositionCreativeType] DROP CONSTRAINT [FK_AdPositionCreativeType_CreativeType];
GO
IF OBJECT_ID(N'[dbo].[FK_AdProcessPiplineAdPosition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdPositions] DROP CONSTRAINT [FK_AdProcessPiplineAdPosition];
GO
IF OBJECT_ID(N'[dbo].[FK_CreativeTypeFormDefinitionCreativeTypeFormDefinition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CreativeTypeFormDefinitions] DROP CONSTRAINT [FK_CreativeTypeFormDefinitionCreativeTypeFormDefinition];
GO
IF OBJECT_ID(N'[dbo].[FK_MediumAdPositionSize]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdPositionSizes] DROP CONSTRAINT [FK_MediumAdPositionSize];
GO
IF OBJECT_ID(N'[dbo].[FK_MediumAdTemplate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdTemplates] DROP CONSTRAINT [FK_MediumAdTemplate];
GO
IF OBJECT_ID(N'[dbo].[FK_MediumAdPositionAspectRatio]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdPositionAspectRatios] DROP CONSTRAINT [FK_MediumAdPositionAspectRatio];
GO
IF OBJECT_ID(N'[dbo].[FK_MediumAdProcessPipline]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdProcessPiplines] DROP CONSTRAINT [FK_MediumAdProcessPipline];
GO
IF OBJECT_ID(N'[dbo].[FK_MediaMaterialFormat]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MaterialFormats] DROP CONSTRAINT [FK_MediaMaterialFormat];
GO
IF OBJECT_ID(N'[dbo].[FK_MediaAdPositionGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdPositionGroups] DROP CONSTRAINT [FK_MediaAdPositionGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_MediaCreativeType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CreativeTypes] DROP CONSTRAINT [FK_MediaCreativeType];
GO
IF OBJECT_ID(N'[dbo].[FK_MediaRedirectDimension]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RedirectDimensions] DROP CONSTRAINT [FK_MediaRedirectDimension];
GO
IF OBJECT_ID(N'[dbo].[FK_MediaRedirectConditionDefinition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RedirectConditionDefinitions] DROP CONSTRAINT [FK_MediaRedirectConditionDefinition];
GO
IF OBJECT_ID(N'[dbo].[FK_AdProcessPiplineAdPositionGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdPositionGroups] DROP CONSTRAINT [FK_AdProcessPiplineAdPositionGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_AdLocationAdPosition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdPositions] DROP CONSTRAINT [FK_AdLocationAdPosition];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AdPositionAspectRatios]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdPositionAspectRatios];
GO
IF OBJECT_ID(N'[dbo].[AdPositionGroups]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdPositionGroups];
GO
IF OBJECT_ID(N'[dbo].[AdPositions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdPositions];
GO
IF OBJECT_ID(N'[dbo].[AdPositionSizes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdPositionSizes];
GO
IF OBJECT_ID(N'[dbo].[AdTemplateRelationships]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdTemplateRelationships];
GO
IF OBJECT_ID(N'[dbo].[AdTemplates]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdTemplates];
GO
IF OBJECT_ID(N'[dbo].[CreativeTypeFormDefinitions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CreativeTypeFormDefinitions];
GO
IF OBJECT_ID(N'[dbo].[CreativeTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CreativeTypes];
GO
IF OBJECT_ID(N'[dbo].[MaterialFormats]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MaterialFormats];
GO
IF OBJECT_ID(N'[dbo].[Medium]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Medium];
GO
IF OBJECT_ID(N'[dbo].[RedirectConditionDefinitions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RedirectConditionDefinitions];
GO
IF OBJECT_ID(N'[dbo].[RedirectDimensions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RedirectDimensions];
GO
IF OBJECT_ID(N'[dbo].[AdProcessPiplines]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdProcessPiplines];
GO
IF OBJECT_ID(N'[dbo].[AdLocations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdLocations];
GO
IF OBJECT_ID(N'[dbo].[AdPositionAdPositionAspectRatio]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdPositionAdPositionAspectRatio];
GO
IF OBJECT_ID(N'[dbo].[AdPositionAdPositionGroup]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdPositionAdPositionGroup];
GO
IF OBJECT_ID(N'[dbo].[AdPositionCreativeType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdPositionCreativeType];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AdPositionAspectRatios'
CREATE TABLE [dbo].[AdPositionAspectRatios] (
    [AdPositionAspectRatioId] uniqueidentifier  NOT NULL,
    [AdPositionAspectRatioValue] nvarchar(50)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [Media_MediaId] uniqueidentifier  NULL
);
GO

-- Creating table 'AdPositionGroups'
CREATE TABLE [dbo].[AdPositionGroups] (
    [AdPositionGroupId] uniqueidentifier  NOT NULL,
    [AdPositionGroupName] nvarchar(50)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [AdPositionGroupCode] nvarchar(50)  NOT NULL,
    [Media_MediaId] uniqueidentifier  NOT NULL,
    [AdProcessPipline_AdProcessPiplineId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AdPositions'
CREATE TABLE [dbo].[AdPositions] (
    [AdPositionId] uniqueidentifier  NOT NULL,
    [AdPositionCode] nvarchar(50)  NOT NULL,
    [AdPositionName] nvarchar(50)  NOT NULL,
    [AdPositionUrl] nvarchar(1024)  NULL,
    [AdPositionDesc] nvarchar(max)  NULL,
    [MaxSlideshowNumber] smallint  NULL,
    [AdPositionSize_AdPositionSizeId] uniqueidentifier  NULL,
    [Medium_MediaId] uniqueidentifier  NOT NULL,
    [DefaultPriceUnit] int  NOT NULL,
    [MaxReturnAdCount] int  NOT NULL,
    [PositionType] int  NOT NULL,
    [AdProcessPipline_AdProcessPiplineId] uniqueidentifier  NOT NULL,
    [AdLocation_AdLocationId] uniqueidentifier  NULL
);
GO

-- Creating table 'AdPositionSizes'
CREATE TABLE [dbo].[AdPositionSizes] (
    [AdPositionSizeId] uniqueidentifier  NOT NULL,
    [Height] int  NOT NULL,
    [Width] int  NOT NULL,
    [Media_MediaId] uniqueidentifier  NULL
);
GO

-- Creating table 'AdTemplateRelationships'
CREATE TABLE [dbo].[AdTemplateRelationships] (
    [AdPositionRelationId] uniqueidentifier  NOT NULL,
    [AdPosition_AdPositionId] uniqueidentifier  NULL,
    [RedirectConditionDefinition_RedirectConditionDefinitionId] uniqueidentifier  NULL,
    [CreativeType_CreativeTypeId] uniqueidentifier  NULL,
    [AdTemplate_AdTemplateId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AdTemplates'
CREATE TABLE [dbo].[AdTemplates] (
    [AdTemplateId] uniqueidentifier  NOT NULL,
    [AdTemplateName] nvarchar(50)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [TemplateContent] nvarchar(max)  NULL,
    [SerializerId] uniqueidentifier  NOT NULL,
    [IsAdPositionTemplate] bit  NOT NULL,
    [Media_MediaId] uniqueidentifier  NULL
);
GO

-- Creating table 'CreativeTypeFormDefinitions'
CREATE TABLE [dbo].[CreativeTypeFormDefinitions] (
    [CreativeTypeFormDefinitionId] uniqueidentifier  NOT NULL,
    [CreativeTypeFormDefinitionName] nvarchar(50)  NOT NULL,
    [DataType] int  NOT NULL,
    [MaxLength] int  NULL,
    [SupportMaxNumber] smallint  NULL,
    [Description] nvarchar(max)  NULL,
    [ValidateExpression] nvarchar(1024)  NULL,
    [DisplaySort] int  NULL,
    [CreativeType_CreativeTypeId] uniqueidentifier  NOT NULL,
    [MaterialFormat_MaterialFormatId] uniqueidentifier  NOT NULL,
    [CreativeTypeFormDefinitionCode] nvarchar(50)  NOT NULL,
    [ParentCreativeTypeFormDefinition_CreativeTypeFormDefinitionId] uniqueidentifier  NULL
);
GO

-- Creating table 'CreativeTypes'
CREATE TABLE [dbo].[CreativeTypes] (
    [CreativeTypeId] uniqueidentifier  NOT NULL,
    [CreativeTypeName] nvarchar(50)  NOT NULL,
    [CreativeTypeDesc] nvarchar(max)  NULL,
    [DisplaySort] int  NULL,
    [Media_MediaId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'MaterialFormats'
CREATE TABLE [dbo].[MaterialFormats] (
    [MaterialFormatId] uniqueidentifier  NOT NULL,
    [MaterialFormatName] nvarchar(50)  NOT NULL,
    [MaterialFormatFileExtention] nvarchar(100)  NOT NULL,
    [MaterialFormatDesc] nvarchar(max)  NULL,
    [Media_MediaId] uniqueidentifier  NULL
);
GO

-- Creating table 'Medium'
CREATE TABLE [dbo].[Medium] (
    [MediaId] uniqueidentifier  NOT NULL,
    [MediaName] nvarchar(50)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [SecureKey] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'RedirectConditionDefinitions'
CREATE TABLE [dbo].[RedirectConditionDefinitions] (
    [RedirectConditionDefinitionId] uniqueidentifier  NOT NULL,
    [ConditionName] nvarchar(50)  NOT NULL,
    [ConditionCode] nvarchar(50)  NOT NULL,
    [Value] float  NULL,
    [ConditionDesc] nvarchar(max)  NULL,
    [DisplaySort] int  NULL,
    [IsSystemBuild] bit  NOT NULL,
    [RedirectDimension_RedirectDimensionId] uniqueidentifier  NOT NULL,
    [ParentRedirectConditionDefinition_RedirectConditionDefinitionId] uniqueidentifier  NULL,
    [Media_MediaId] uniqueidentifier  NULL
);
GO

-- Creating table 'RedirectDimensions'
CREATE TABLE [dbo].[RedirectDimensions] (
    [RedirectDimensionId] uniqueidentifier  NOT NULL,
    [RedirectDimensionName] nvarchar(50)  NOT NULL,
    [RedirectDimensionCode] nvarchar(50)  NOT NULL,
    [RedirectDimensionDesc] nvarchar(max)  NULL,
    [DisplaySort] int  NULL,
    [CanCustomKey] bit  NOT NULL,
    [CanCustomValue] bit  NOT NULL,
    [Medium_MediaId] uniqueidentifier  NULL
);
GO

-- Creating table 'AdProcessPiplines'
CREATE TABLE [dbo].[AdProcessPiplines] (
    [AdProcessPiplineId] uniqueidentifier  NOT NULL,
    [PiplineName] nvarchar(50)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [ConfigfileName] nvarchar(100)  NOT NULL,
    [Media_MediaId] uniqueidentifier  NULL
);
GO

-- Creating table 'AdLocations'
CREATE TABLE [dbo].[AdLocations] (
    [AdLocationId] uniqueidentifier  NOT NULL,
    [LocationName] nvarchar(50)  NOT NULL,
    [Value] int  NOT NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- Creating table 'AdPositionAdPositionAspectRatio'
CREATE TABLE [dbo].[AdPositionAdPositionAspectRatio] (
    [AdPositions_AdPositionId] uniqueidentifier  NOT NULL,
    [AdPositionAspectRatios_AdPositionAspectRatioId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AdPositionAdPositionGroup'
CREATE TABLE [dbo].[AdPositionAdPositionGroup] (
    [AdPositions_AdPositionId] uniqueidentifier  NOT NULL,
    [AdPositionGroups_AdPositionGroupId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AdPositionCreativeType'
CREATE TABLE [dbo].[AdPositionCreativeType] (
    [AdPosition_AdPositionId] uniqueidentifier  NOT NULL,
    [CreativeTypes_CreativeTypeId] uniqueidentifier  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [AdPositionAspectRatioId] in table 'AdPositionAspectRatios'
ALTER TABLE [dbo].[AdPositionAspectRatios]
ADD CONSTRAINT [PK_AdPositionAspectRatios]
    PRIMARY KEY CLUSTERED ([AdPositionAspectRatioId] ASC);
GO

-- Creating primary key on [AdPositionGroupId] in table 'AdPositionGroups'
ALTER TABLE [dbo].[AdPositionGroups]
ADD CONSTRAINT [PK_AdPositionGroups]
    PRIMARY KEY CLUSTERED ([AdPositionGroupId] ASC);
GO

-- Creating primary key on [AdPositionId] in table 'AdPositions'
ALTER TABLE [dbo].[AdPositions]
ADD CONSTRAINT [PK_AdPositions]
    PRIMARY KEY CLUSTERED ([AdPositionId] ASC);
GO

-- Creating primary key on [AdPositionSizeId] in table 'AdPositionSizes'
ALTER TABLE [dbo].[AdPositionSizes]
ADD CONSTRAINT [PK_AdPositionSizes]
    PRIMARY KEY CLUSTERED ([AdPositionSizeId] ASC);
GO

-- Creating primary key on [AdPositionRelationId] in table 'AdTemplateRelationships'
ALTER TABLE [dbo].[AdTemplateRelationships]
ADD CONSTRAINT [PK_AdTemplateRelationships]
    PRIMARY KEY CLUSTERED ([AdPositionRelationId] ASC);
GO

-- Creating primary key on [AdTemplateId] in table 'AdTemplates'
ALTER TABLE [dbo].[AdTemplates]
ADD CONSTRAINT [PK_AdTemplates]
    PRIMARY KEY CLUSTERED ([AdTemplateId] ASC);
GO

-- Creating primary key on [CreativeTypeFormDefinitionId] in table 'CreativeTypeFormDefinitions'
ALTER TABLE [dbo].[CreativeTypeFormDefinitions]
ADD CONSTRAINT [PK_CreativeTypeFormDefinitions]
    PRIMARY KEY CLUSTERED ([CreativeTypeFormDefinitionId] ASC);
GO

-- Creating primary key on [CreativeTypeId] in table 'CreativeTypes'
ALTER TABLE [dbo].[CreativeTypes]
ADD CONSTRAINT [PK_CreativeTypes]
    PRIMARY KEY CLUSTERED ([CreativeTypeId] ASC);
GO

-- Creating primary key on [MaterialFormatId] in table 'MaterialFormats'
ALTER TABLE [dbo].[MaterialFormats]
ADD CONSTRAINT [PK_MaterialFormats]
    PRIMARY KEY CLUSTERED ([MaterialFormatId] ASC);
GO

-- Creating primary key on [MediaId] in table 'Medium'
ALTER TABLE [dbo].[Medium]
ADD CONSTRAINT [PK_Medium]
    PRIMARY KEY CLUSTERED ([MediaId] ASC);
GO

-- Creating primary key on [RedirectConditionDefinitionId] in table 'RedirectConditionDefinitions'
ALTER TABLE [dbo].[RedirectConditionDefinitions]
ADD CONSTRAINT [PK_RedirectConditionDefinitions]
    PRIMARY KEY CLUSTERED ([RedirectConditionDefinitionId] ASC);
GO

-- Creating primary key on [RedirectDimensionId] in table 'RedirectDimensions'
ALTER TABLE [dbo].[RedirectDimensions]
ADD CONSTRAINT [PK_RedirectDimensions]
    PRIMARY KEY CLUSTERED ([RedirectDimensionId] ASC);
GO

-- Creating primary key on [AdProcessPiplineId] in table 'AdProcessPiplines'
ALTER TABLE [dbo].[AdProcessPiplines]
ADD CONSTRAINT [PK_AdProcessPiplines]
    PRIMARY KEY CLUSTERED ([AdProcessPiplineId] ASC);
GO

-- Creating primary key on [AdLocationId] in table 'AdLocations'
ALTER TABLE [dbo].[AdLocations]
ADD CONSTRAINT [PK_AdLocations]
    PRIMARY KEY CLUSTERED ([AdLocationId] ASC);
GO

-- Creating primary key on [AdPositions_AdPositionId], [AdPositionAspectRatios_AdPositionAspectRatioId] in table 'AdPositionAdPositionAspectRatio'
ALTER TABLE [dbo].[AdPositionAdPositionAspectRatio]
ADD CONSTRAINT [PK_AdPositionAdPositionAspectRatio]
    PRIMARY KEY CLUSTERED ([AdPositions_AdPositionId], [AdPositionAspectRatios_AdPositionAspectRatioId] ASC);
GO

-- Creating primary key on [AdPositions_AdPositionId], [AdPositionGroups_AdPositionGroupId] in table 'AdPositionAdPositionGroup'
ALTER TABLE [dbo].[AdPositionAdPositionGroup]
ADD CONSTRAINT [PK_AdPositionAdPositionGroup]
    PRIMARY KEY CLUSTERED ([AdPositions_AdPositionId], [AdPositionGroups_AdPositionGroupId] ASC);
GO

-- Creating primary key on [AdPosition_AdPositionId], [CreativeTypes_CreativeTypeId] in table 'AdPositionCreativeType'
ALTER TABLE [dbo].[AdPositionCreativeType]
ADD CONSTRAINT [PK_AdPositionCreativeType]
    PRIMARY KEY CLUSTERED ([AdPosition_AdPositionId], [CreativeTypes_CreativeTypeId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [AdPositions_AdPositionId] in table 'AdPositionAdPositionAspectRatio'
ALTER TABLE [dbo].[AdPositionAdPositionAspectRatio]
ADD CONSTRAINT [FK_AdPositionAdPositionAspectRatio_AdPosition]
    FOREIGN KEY ([AdPositions_AdPositionId])
    REFERENCES [dbo].[AdPositions]
        ([AdPositionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [AdPositionAspectRatios_AdPositionAspectRatioId] in table 'AdPositionAdPositionAspectRatio'
ALTER TABLE [dbo].[AdPositionAdPositionAspectRatio]
ADD CONSTRAINT [FK_AdPositionAdPositionAspectRatio_AdPositionAspectRatio]
    FOREIGN KEY ([AdPositionAspectRatios_AdPositionAspectRatioId])
    REFERENCES [dbo].[AdPositionAspectRatios]
        ([AdPositionAspectRatioId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdPositionAdPositionAspectRatio_AdPositionAspectRatio'
CREATE INDEX [IX_FK_AdPositionAdPositionAspectRatio_AdPositionAspectRatio]
ON [dbo].[AdPositionAdPositionAspectRatio]
    ([AdPositionAspectRatios_AdPositionAspectRatioId]);
GO

-- Creating foreign key on [AdPositions_AdPositionId] in table 'AdPositionAdPositionGroup'
ALTER TABLE [dbo].[AdPositionAdPositionGroup]
ADD CONSTRAINT [FK_AdPositionAdPositionGroup_AdPosition]
    FOREIGN KEY ([AdPositions_AdPositionId])
    REFERENCES [dbo].[AdPositions]
        ([AdPositionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [AdPositionGroups_AdPositionGroupId] in table 'AdPositionAdPositionGroup'
ALTER TABLE [dbo].[AdPositionAdPositionGroup]
ADD CONSTRAINT [FK_AdPositionAdPositionGroup_AdPositionGroup]
    FOREIGN KEY ([AdPositionGroups_AdPositionGroupId])
    REFERENCES [dbo].[AdPositionGroups]
        ([AdPositionGroupId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdPositionAdPositionGroup_AdPositionGroup'
CREATE INDEX [IX_FK_AdPositionAdPositionGroup_AdPositionGroup]
ON [dbo].[AdPositionAdPositionGroup]
    ([AdPositionGroups_AdPositionGroupId]);
GO

-- Creating foreign key on [AdPosition_AdPositionId] in table 'AdTemplateRelationships'
ALTER TABLE [dbo].[AdTemplateRelationships]
ADD CONSTRAINT [FK_AdPositionAdTemplateRelationship]
    FOREIGN KEY ([AdPosition_AdPositionId])
    REFERENCES [dbo].[AdPositions]
        ([AdPositionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdPositionAdTemplateRelationship'
CREATE INDEX [IX_FK_AdPositionAdTemplateRelationship]
ON [dbo].[AdTemplateRelationships]
    ([AdPosition_AdPositionId]);
GO

-- Creating foreign key on [AdPositionSize_AdPositionSizeId] in table 'AdPositions'
ALTER TABLE [dbo].[AdPositions]
ADD CONSTRAINT [FK_AdPositionSizeAdPosition]
    FOREIGN KEY ([AdPositionSize_AdPositionSizeId])
    REFERENCES [dbo].[AdPositionSizes]
        ([AdPositionSizeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdPositionSizeAdPosition'
CREATE INDEX [IX_FK_AdPositionSizeAdPosition]
ON [dbo].[AdPositions]
    ([AdPositionSize_AdPositionSizeId]);
GO

-- Creating foreign key on [Medium_MediaId] in table 'AdPositions'
ALTER TABLE [dbo].[AdPositions]
ADD CONSTRAINT [FK_MediaAdPosition]
    FOREIGN KEY ([Medium_MediaId])
    REFERENCES [dbo].[Medium]
        ([MediaId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MediaAdPosition'
CREATE INDEX [IX_FK_MediaAdPosition]
ON [dbo].[AdPositions]
    ([Medium_MediaId]);
GO

-- Creating foreign key on [AdTemplate_AdTemplateId] in table 'AdTemplateRelationships'
ALTER TABLE [dbo].[AdTemplateRelationships]
ADD CONSTRAINT [FK_AdTemplateAdTemplateRelationship]
    FOREIGN KEY ([AdTemplate_AdTemplateId])
    REFERENCES [dbo].[AdTemplates]
        ([AdTemplateId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdTemplateAdTemplateRelationship'
CREATE INDEX [IX_FK_AdTemplateAdTemplateRelationship]
ON [dbo].[AdTemplateRelationships]
    ([AdTemplate_AdTemplateId]);
GO

-- Creating foreign key on [CreativeType_CreativeTypeId] in table 'CreativeTypeFormDefinitions'
ALTER TABLE [dbo].[CreativeTypeFormDefinitions]
ADD CONSTRAINT [FK_CreativeTypeCreativeTypeFormDefinition]
    FOREIGN KEY ([CreativeType_CreativeTypeId])
    REFERENCES [dbo].[CreativeTypes]
        ([CreativeTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CreativeTypeCreativeTypeFormDefinition'
CREATE INDEX [IX_FK_CreativeTypeCreativeTypeFormDefinition]
ON [dbo].[CreativeTypeFormDefinitions]
    ([CreativeType_CreativeTypeId]);
GO

-- Creating foreign key on [MaterialFormat_MaterialFormatId] in table 'CreativeTypeFormDefinitions'
ALTER TABLE [dbo].[CreativeTypeFormDefinitions]
ADD CONSTRAINT [FK_MaterialFormatCreativeTypeFormDefinition]
    FOREIGN KEY ([MaterialFormat_MaterialFormatId])
    REFERENCES [dbo].[MaterialFormats]
        ([MaterialFormatId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MaterialFormatCreativeTypeFormDefinition'
CREATE INDEX [IX_FK_MaterialFormatCreativeTypeFormDefinition]
ON [dbo].[CreativeTypeFormDefinitions]
    ([MaterialFormat_MaterialFormatId]);
GO

-- Creating foreign key on [CreativeType_CreativeTypeId] in table 'AdTemplateRelationships'
ALTER TABLE [dbo].[AdTemplateRelationships]
ADD CONSTRAINT [FK_CreativeTypeAdTemplateRelationship]
    FOREIGN KEY ([CreativeType_CreativeTypeId])
    REFERENCES [dbo].[CreativeTypes]
        ([CreativeTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CreativeTypeAdTemplateRelationship'
CREATE INDEX [IX_FK_CreativeTypeAdTemplateRelationship]
ON [dbo].[AdTemplateRelationships]
    ([CreativeType_CreativeTypeId]);
GO

-- Creating foreign key on [RedirectConditionDefinition_RedirectConditionDefinitionId] in table 'AdTemplateRelationships'
ALTER TABLE [dbo].[AdTemplateRelationships]
ADD CONSTRAINT [FK_RedirectConditionDefinitionAdTemplateRelationship]
    FOREIGN KEY ([RedirectConditionDefinition_RedirectConditionDefinitionId])
    REFERENCES [dbo].[RedirectConditionDefinitions]
        ([RedirectConditionDefinitionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RedirectConditionDefinitionAdTemplateRelationship'
CREATE INDEX [IX_FK_RedirectConditionDefinitionAdTemplateRelationship]
ON [dbo].[AdTemplateRelationships]
    ([RedirectConditionDefinition_RedirectConditionDefinitionId]);
GO

-- Creating foreign key on [ParentRedirectConditionDefinition_RedirectConditionDefinitionId] in table 'RedirectConditionDefinitions'
ALTER TABLE [dbo].[RedirectConditionDefinitions]
ADD CONSTRAINT [FK_RedirectConditionDefinitionRedirectConditionDefinition]
    FOREIGN KEY ([ParentRedirectConditionDefinition_RedirectConditionDefinitionId])
    REFERENCES [dbo].[RedirectConditionDefinitions]
        ([RedirectConditionDefinitionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RedirectConditionDefinitionRedirectConditionDefinition'
CREATE INDEX [IX_FK_RedirectConditionDefinitionRedirectConditionDefinition]
ON [dbo].[RedirectConditionDefinitions]
    ([ParentRedirectConditionDefinition_RedirectConditionDefinitionId]);
GO

-- Creating foreign key on [RedirectDimension_RedirectDimensionId] in table 'RedirectConditionDefinitions'
ALTER TABLE [dbo].[RedirectConditionDefinitions]
ADD CONSTRAINT [FK_RedirectDimensionRedirectConditionDefinition]
    FOREIGN KEY ([RedirectDimension_RedirectDimensionId])
    REFERENCES [dbo].[RedirectDimensions]
        ([RedirectDimensionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RedirectDimensionRedirectConditionDefinition'
CREATE INDEX [IX_FK_RedirectDimensionRedirectConditionDefinition]
ON [dbo].[RedirectConditionDefinitions]
    ([RedirectDimension_RedirectDimensionId]);
GO

-- Creating foreign key on [AdPosition_AdPositionId] in table 'AdPositionCreativeType'
ALTER TABLE [dbo].[AdPositionCreativeType]
ADD CONSTRAINT [FK_AdPositionCreativeType_AdPosition]
    FOREIGN KEY ([AdPosition_AdPositionId])
    REFERENCES [dbo].[AdPositions]
        ([AdPositionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [CreativeTypes_CreativeTypeId] in table 'AdPositionCreativeType'
ALTER TABLE [dbo].[AdPositionCreativeType]
ADD CONSTRAINT [FK_AdPositionCreativeType_CreativeType]
    FOREIGN KEY ([CreativeTypes_CreativeTypeId])
    REFERENCES [dbo].[CreativeTypes]
        ([CreativeTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdPositionCreativeType_CreativeType'
CREATE INDEX [IX_FK_AdPositionCreativeType_CreativeType]
ON [dbo].[AdPositionCreativeType]
    ([CreativeTypes_CreativeTypeId]);
GO

-- Creating foreign key on [AdProcessPipline_AdProcessPiplineId] in table 'AdPositions'
ALTER TABLE [dbo].[AdPositions]
ADD CONSTRAINT [FK_AdProcessPiplineAdPosition]
    FOREIGN KEY ([AdProcessPipline_AdProcessPiplineId])
    REFERENCES [dbo].[AdProcessPiplines]
        ([AdProcessPiplineId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdProcessPiplineAdPosition'
CREATE INDEX [IX_FK_AdProcessPiplineAdPosition]
ON [dbo].[AdPositions]
    ([AdProcessPipline_AdProcessPiplineId]);
GO

-- Creating foreign key on [ParentCreativeTypeFormDefinition_CreativeTypeFormDefinitionId] in table 'CreativeTypeFormDefinitions'
ALTER TABLE [dbo].[CreativeTypeFormDefinitions]
ADD CONSTRAINT [FK_CreativeTypeFormDefinitionCreativeTypeFormDefinition]
    FOREIGN KEY ([ParentCreativeTypeFormDefinition_CreativeTypeFormDefinitionId])
    REFERENCES [dbo].[CreativeTypeFormDefinitions]
        ([CreativeTypeFormDefinitionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CreativeTypeFormDefinitionCreativeTypeFormDefinition'
CREATE INDEX [IX_FK_CreativeTypeFormDefinitionCreativeTypeFormDefinition]
ON [dbo].[CreativeTypeFormDefinitions]
    ([ParentCreativeTypeFormDefinition_CreativeTypeFormDefinitionId]);
GO

-- Creating foreign key on [Media_MediaId] in table 'AdPositionSizes'
ALTER TABLE [dbo].[AdPositionSizes]
ADD CONSTRAINT [FK_MediumAdPositionSize]
    FOREIGN KEY ([Media_MediaId])
    REFERENCES [dbo].[Medium]
        ([MediaId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MediumAdPositionSize'
CREATE INDEX [IX_FK_MediumAdPositionSize]
ON [dbo].[AdPositionSizes]
    ([Media_MediaId]);
GO

-- Creating foreign key on [Media_MediaId] in table 'AdTemplates'
ALTER TABLE [dbo].[AdTemplates]
ADD CONSTRAINT [FK_MediumAdTemplate]
    FOREIGN KEY ([Media_MediaId])
    REFERENCES [dbo].[Medium]
        ([MediaId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MediumAdTemplate'
CREATE INDEX [IX_FK_MediumAdTemplate]
ON [dbo].[AdTemplates]
    ([Media_MediaId]);
GO

-- Creating foreign key on [Media_MediaId] in table 'AdPositionAspectRatios'
ALTER TABLE [dbo].[AdPositionAspectRatios]
ADD CONSTRAINT [FK_MediumAdPositionAspectRatio]
    FOREIGN KEY ([Media_MediaId])
    REFERENCES [dbo].[Medium]
        ([MediaId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MediumAdPositionAspectRatio'
CREATE INDEX [IX_FK_MediumAdPositionAspectRatio]
ON [dbo].[AdPositionAspectRatios]
    ([Media_MediaId]);
GO

-- Creating foreign key on [Media_MediaId] in table 'AdProcessPiplines'
ALTER TABLE [dbo].[AdProcessPiplines]
ADD CONSTRAINT [FK_MediumAdProcessPipline]
    FOREIGN KEY ([Media_MediaId])
    REFERENCES [dbo].[Medium]
        ([MediaId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MediumAdProcessPipline'
CREATE INDEX [IX_FK_MediumAdProcessPipline]
ON [dbo].[AdProcessPiplines]
    ([Media_MediaId]);
GO

-- Creating foreign key on [Media_MediaId] in table 'MaterialFormats'
ALTER TABLE [dbo].[MaterialFormats]
ADD CONSTRAINT [FK_MediaMaterialFormat]
    FOREIGN KEY ([Media_MediaId])
    REFERENCES [dbo].[Medium]
        ([MediaId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MediaMaterialFormat'
CREATE INDEX [IX_FK_MediaMaterialFormat]
ON [dbo].[MaterialFormats]
    ([Media_MediaId]);
GO

-- Creating foreign key on [Media_MediaId] in table 'AdPositionGroups'
ALTER TABLE [dbo].[AdPositionGroups]
ADD CONSTRAINT [FK_MediaAdPositionGroup]
    FOREIGN KEY ([Media_MediaId])
    REFERENCES [dbo].[Medium]
        ([MediaId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MediaAdPositionGroup'
CREATE INDEX [IX_FK_MediaAdPositionGroup]
ON [dbo].[AdPositionGroups]
    ([Media_MediaId]);
GO

-- Creating foreign key on [Media_MediaId] in table 'CreativeTypes'
ALTER TABLE [dbo].[CreativeTypes]
ADD CONSTRAINT [FK_MediaCreativeType]
    FOREIGN KEY ([Media_MediaId])
    REFERENCES [dbo].[Medium]
        ([MediaId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MediaCreativeType'
CREATE INDEX [IX_FK_MediaCreativeType]
ON [dbo].[CreativeTypes]
    ([Media_MediaId]);
GO

-- Creating foreign key on [Medium_MediaId] in table 'RedirectDimensions'
ALTER TABLE [dbo].[RedirectDimensions]
ADD CONSTRAINT [FK_MediaRedirectDimension]
    FOREIGN KEY ([Medium_MediaId])
    REFERENCES [dbo].[Medium]
        ([MediaId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MediaRedirectDimension'
CREATE INDEX [IX_FK_MediaRedirectDimension]
ON [dbo].[RedirectDimensions]
    ([Medium_MediaId]);
GO

-- Creating foreign key on [Media_MediaId] in table 'RedirectConditionDefinitions'
ALTER TABLE [dbo].[RedirectConditionDefinitions]
ADD CONSTRAINT [FK_MediaRedirectConditionDefinition]
    FOREIGN KEY ([Media_MediaId])
    REFERENCES [dbo].[Medium]
        ([MediaId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MediaRedirectConditionDefinition'
CREATE INDEX [IX_FK_MediaRedirectConditionDefinition]
ON [dbo].[RedirectConditionDefinitions]
    ([Media_MediaId]);
GO

-- Creating foreign key on [AdProcessPipline_AdProcessPiplineId] in table 'AdPositionGroups'
ALTER TABLE [dbo].[AdPositionGroups]
ADD CONSTRAINT [FK_AdProcessPiplineAdPositionGroup]
    FOREIGN KEY ([AdProcessPipline_AdProcessPiplineId])
    REFERENCES [dbo].[AdProcessPiplines]
        ([AdProcessPiplineId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdProcessPiplineAdPositionGroup'
CREATE INDEX [IX_FK_AdProcessPiplineAdPositionGroup]
ON [dbo].[AdPositionGroups]
    ([AdProcessPipline_AdProcessPiplineId]);
GO

-- Creating foreign key on [AdLocation_AdLocationId] in table 'AdPositions'
ALTER TABLE [dbo].[AdPositions]
ADD CONSTRAINT [FK_AdLocationAdPosition]
    FOREIGN KEY ([AdLocation_AdLocationId])
    REFERENCES [dbo].[AdLocations]
        ([AdLocationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdLocationAdPosition'
CREATE INDEX [IX_FK_AdLocationAdPosition]
ON [dbo].[AdPositions]
    ([AdLocation_AdLocationId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------