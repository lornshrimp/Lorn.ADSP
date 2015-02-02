
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 09/24/2014 13:01:53
-- Generated from EDMX file: C:\Users\Bill\OneDrive\Visual Studio\Projects\Lorn.ADSP\AM\Lorn.ADSP.AM.DataModels\AMData.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Lorn.ADSP.AM];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_AdMaterialAdMaterialFormInfo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdMaterialFormInfoes] DROP CONSTRAINT [FK_AdMaterialAdMaterialFormInfo];
GO
IF OBJECT_ID(N'[dbo].[FK_AdMaterialThirdMonitorCode]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdSpotPlanThirdMonitorCodes] DROP CONSTRAINT [FK_AdMaterialThirdMonitorCode];
GO
IF OBJECT_ID(N'[dbo].[FK_AdSpotPlanDetails_AdSpotPlanGroups]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdSpotPlanDetails] DROP CONSTRAINT [FK_AdSpotPlanDetails_AdSpotPlanGroups];
GO
IF OBJECT_ID(N'[dbo].[FK_AdSpotPlanAdSpotPlanEdition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdSpotPlanEditions] DROP CONSTRAINT [FK_AdSpotPlanAdSpotPlanEdition];
GO
IF OBJECT_ID(N'[dbo].[FK_AdSpotPlanEditionSpotPlanGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdSpotPlanGroups] DROP CONSTRAINT [FK_AdSpotPlanEditionSpotPlanGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_AdSpotPlanEditionThirdMonitorCode]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdSpotPlanThirdMonitorCodes] DROP CONSTRAINT [FK_AdSpotPlanEditionThirdMonitorCode];
GO
IF OBJECT_ID(N'[dbo].[FK_AdSpotPlanTypeAdSpotPlanEdition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdSpotPlanEditions] DROP CONSTRAINT [FK_AdSpotPlanTypeAdSpotPlanEdition];
GO
IF OBJECT_ID(N'[dbo].[FK_IpLibraryAdSpotPlanEdition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdSpotPlanEditions] DROP CONSTRAINT [FK_IpLibraryAdSpotPlanEdition];
GO
IF OBJECT_ID(N'[dbo].[FK_AdSpotPlanRedirctConditionAdSpotPlanRedirctConditionDetail1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RedirectConditionDetails] DROP CONSTRAINT [FK_AdSpotPlanRedirctConditionAdSpotPlanRedirctConditionDetail1];
GO
IF OBJECT_ID(N'[dbo].[FK_SpotPlanGroupAdSpotPlanGroupRedirctCondition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RedirectConditions] DROP CONSTRAINT [FK_SpotPlanGroupAdSpotPlanGroupRedirctCondition];
GO
IF OBJECT_ID(N'[dbo].[FK_AdSpotPlanDetailMaterialAssign]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdMaterialAssigns] DROP CONSTRAINT [FK_AdSpotPlanDetailMaterialAssign];
GO
IF OBJECT_ID(N'[dbo].[FK_AdMaterialMaterialAssign]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdMaterialAssigns] DROP CONSTRAINT [FK_AdMaterialMaterialAssign];
GO
IF OBJECT_ID(N'[dbo].[FK_FrequencyControlAdSpotPlanEdition_FrequencyControl]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FrequencyControlAdSpotPlanEdition] DROP CONSTRAINT [FK_FrequencyControlAdSpotPlanEdition_FrequencyControl];
GO
IF OBJECT_ID(N'[dbo].[FK_FrequencyControlAdSpotPlanEdition_AdSpotPlanEdition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FrequencyControlAdSpotPlanEdition] DROP CONSTRAINT [FK_FrequencyControlAdSpotPlanEdition_AdSpotPlanEdition];
GO
IF OBJECT_ID(N'[dbo].[FK_AdSpotPlanGroupFrequencyControl_AdSpotPlanGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdSpotPlanGroupFrequencyControl] DROP CONSTRAINT [FK_AdSpotPlanGroupFrequencyControl_AdSpotPlanGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_AdSpotPlanGroupFrequencyControl_FrequencyControl]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdSpotPlanGroupFrequencyControl] DROP CONSTRAINT [FK_AdSpotPlanGroupFrequencyControl_FrequencyControl];
GO
IF OBJECT_ID(N'[dbo].[FK_AdMaterialFrequencyControl_AdMaterial]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdMaterialFrequencyControl] DROP CONSTRAINT [FK_AdMaterialFrequencyControl_AdMaterial];
GO
IF OBJECT_ID(N'[dbo].[FK_AdMaterialFrequencyControl_FrequencyControl]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdMaterialFrequencyControl] DROP CONSTRAINT [FK_AdMaterialFrequencyControl_FrequencyControl];
GO
IF OBJECT_ID(N'[dbo].[FK_FrequencyControlFrequencyControlDetail]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FrequencyControlDetails] DROP CONSTRAINT [FK_FrequencyControlFrequencyControlDetail];
GO
IF OBJECT_ID(N'[dbo].[FK_MonitorTypeAdSpotPlanEdition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdSpotPlanEditions] DROP CONSTRAINT [FK_MonitorTypeAdSpotPlanEdition];
GO
IF OBJECT_ID(N'[dbo].[FK_MonitorTypeThirdMonitorCode]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ThirdMonitorCodes] DROP CONSTRAINT [FK_MonitorTypeThirdMonitorCode];
GO
IF OBJECT_ID(N'[dbo].[FK_ThirdMonitorCodeAdSpotPlanThirdMonitorCode]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdSpotPlanThirdMonitorCodes] DROP CONSTRAINT [FK_ThirdMonitorCodeAdSpotPlanThirdMonitorCode];
GO
IF OBJECT_ID(N'[dbo].[FK_ThirdMonitorTypeThirdMonitorCode]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ThirdMonitorCodes] DROP CONSTRAINT [FK_ThirdMonitorTypeThirdMonitorCode];
GO
IF OBJECT_ID(N'[dbo].[FK_AdSpotPlanGroupAdSpotPlanThirdMonitorCode]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AdSpotPlanThirdMonitorCodes] DROP CONSTRAINT [FK_AdSpotPlanGroupAdSpotPlanThirdMonitorCode];
GO
IF OBJECT_ID(N'[dbo].[FK_ThirdMonitorCompanyThirdMonitorCode]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ThirdMonitorCodes] DROP CONSTRAINT [FK_ThirdMonitorCompanyThirdMonitorCode];
GO
IF OBJECT_ID(N'[dbo].[FK_AdSpotPlanEditionRedirctCondition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RedirectConditions] DROP CONSTRAINT [FK_AdSpotPlanEditionRedirctCondition];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AdMaterialFormInfoes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdMaterialFormInfoes];
GO
IF OBJECT_ID(N'[dbo].[AdMaterials]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdMaterials];
GO
IF OBJECT_ID(N'[dbo].[AdSpotPlanDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdSpotPlanDetails];
GO
IF OBJECT_ID(N'[dbo].[AdSpotPlanEditions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdSpotPlanEditions];
GO
IF OBJECT_ID(N'[dbo].[RedirectConditionDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RedirectConditionDetails];
GO
IF OBJECT_ID(N'[dbo].[RedirectConditions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RedirectConditions];
GO
IF OBJECT_ID(N'[dbo].[AdSpotPlanGroups]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdSpotPlanGroups];
GO
IF OBJECT_ID(N'[dbo].[AdSpotPlans]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdSpotPlans];
GO
IF OBJECT_ID(N'[dbo].[AdSpotPlanThirdMonitorCodes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdSpotPlanThirdMonitorCodes];
GO
IF OBJECT_ID(N'[dbo].[AdSpotPlanTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdSpotPlanTypes];
GO
IF OBJECT_ID(N'[dbo].[IpLibraries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[IpLibraries];
GO
IF OBJECT_ID(N'[dbo].[MonitorTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MonitorTypes];
GO
IF OBJECT_ID(N'[dbo].[AdMaterialAssigns]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdMaterialAssigns];
GO
IF OBJECT_ID(N'[dbo].[FrequencyControls]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FrequencyControls];
GO
IF OBJECT_ID(N'[dbo].[FrequencyControlDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FrequencyControlDetails];
GO
IF OBJECT_ID(N'[dbo].[ThirdMonitorCodes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ThirdMonitorCodes];
GO
IF OBJECT_ID(N'[dbo].[ThirdMonitorModes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ThirdMonitorModes];
GO
IF OBJECT_ID(N'[dbo].[ThirdMonitorCompanies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ThirdMonitorCompanies];
GO
IF OBJECT_ID(N'[dbo].[FrequencyControlAdSpotPlanEdition]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FrequencyControlAdSpotPlanEdition];
GO
IF OBJECT_ID(N'[dbo].[AdSpotPlanGroupFrequencyControl]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdSpotPlanGroupFrequencyControl];
GO
IF OBJECT_ID(N'[dbo].[AdMaterialFrequencyControl]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdMaterialFrequencyControl];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AdMaterialFormInfoes'
CREATE TABLE [dbo].[AdMaterialFormInfoes] (
    [AdMaterialFormInfoId] uniqueidentifier  NOT NULL,
    [CreativeTypeFormdefinitionId] uniqueidentifier  NOT NULL,
    [Value] nvarchar(max)  NOT NULL,
    [SortNo] int  NULL,
    [AdMaterial_AdMaterialId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AdMaterials'
CREATE TABLE [dbo].[AdMaterials] (
    [AdMaterialId] uniqueidentifier  NOT NULL,
    [AdMasterPlanId] uniqueidentifier  NULL,
    [MaterialCode] nvarchar(50)  NOT NULL,
    [Enabled] bit  NOT NULL,
    [CreativeTypeId] uniqueidentifier  NOT NULL,
    [TimeLength] time  NULL,
    [Height] int  NULL,
    [Width] int  NULL,
    [AspectRatioId] uniqueidentifier  NULL,
    [ExtData] nvarchar(max)  NULL,
    [UpdateTime] datetime  NULL,
    [MediaId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AdSpotPlanDetails'
CREATE TABLE [dbo].[AdSpotPlanDetails] (
    [AdPositionDetailId] uniqueidentifier  NOT NULL,
    [SpotDate] datetime  NOT NULL,
    [AdSpotPlanGroupId] uniqueidentifier  NOT NULL,
    [AdPositionId] nvarchar(max)  NOT NULL,
    [SaleType] int  NOT NULL,
    [AdPositionFormat] nvarchar(1024)  NULL,
    [AdPositionUrl] nvarchar(1024)  NULL,
    [PlanTrafficRatio] int  NULL,
    [PlanImpressionNumber] bigint  NULL,
    [PlanClickNumber] bigint  NULL,
    [MacroSlideshowSequenceNo] int  NULL,
    [MicroSlideshowType] int  NULL,
    [PublishPrice] decimal(18,0)  NOT NULL,
    [NetPrice] decimal(18,0)  NOT NULL,
    [PriceType] int  NOT NULL,
    [ComsumeType] int  NOT NULL,
    [AdPositionSizeId] uniqueidentifier  NULL,
    [AdLocationId] uniqueidentifier  NULL
);
GO

-- Creating table 'AdSpotPlanEditions'
CREATE TABLE [dbo].[AdSpotPlanEditions] (
    [AdSpotPlanEditionId] uniqueidentifier  NOT NULL,
    [EditionNo] int  NOT NULL,
    [Priority] int  NOT NULL,
    [CategoryExclusive] bit  NOT NULL,
    [ProductCategoryId] uniqueidentifier  NULL,
    [MonitorCompanyId] uniqueidentifier  NULL,
    [StartDate] datetime  NOT NULL,
    [EndDate] datetime  NOT NULL,
    [EditionStatus] int  NOT NULL,
    [BudgetAmount] decimal(18,0)  NOT NULL,
    [PackageDiscount] decimal(18,0)  NOT NULL,
    [Remark] nvarchar(max)  NULL,
    [AdSpotPlan_AdSpotPlanId] uniqueidentifier  NOT NULL,
    [AdSpotPlanType_AdSpotPlanTypeId] uniqueidentifier  NOT NULL,
    [IpLibrary_IpLibraryId] uniqueidentifier  NULL,
    [ValuationMonitorType_MonitorTypeId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'RedirectConditionDetails'
CREATE TABLE [dbo].[RedirectConditionDetails] (
    [RedirectConditionDetailId] uniqueidentifier  NOT NULL,
    [StartValue] float  NULL,
    [EndValue] float  NULL,
    [RedirectCondition_RedirectConditionId] uniqueidentifier  NOT NULL,
    [CustomKey] nvarchar(max)  NULL,
    [CustomValue] nvarchar(max)  NULL,
    [RedirectConditionDefinitionId] uniqueidentifier  NULL
);
GO

-- Creating table 'RedirectConditions'
CREATE TABLE [dbo].[RedirectConditions] (
    [RedirectConditionId] uniqueidentifier  NOT NULL,
    [RedirectDimensionId] uniqueidentifier  NOT NULL,
    [IsExclude] bit  NOT NULL,
    [SpotPlanGroup_SpotPlanGroupId] uniqueidentifier  NULL,
    [AdSpotPlanEdition_AdSpotPlanEditionId] uniqueidentifier  NULL
);
GO

-- Creating table 'AdSpotPlanGroups'
CREATE TABLE [dbo].[AdSpotPlanGroups] (
    [SpotPlanGroupId] uniqueidentifier  NOT NULL,
    [SpotPlanGroupName] nvarchar(50)  NOT NULL,
    [AdSpotPlanEdition_AdSpotPlanEditionId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AdSpotPlans'
CREATE TABLE [dbo].[AdSpotPlans] (
    [AdSpotPlanId] uniqueidentifier  NOT NULL,
    [AdMasterPlanId] uniqueidentifier  NULL,
    [AdSpotPlanName] nvarchar(50)  NOT NULL,
    [MediaId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AdSpotPlanThirdMonitorCodes'
CREATE TABLE [dbo].[AdSpotPlanThirdMonitorCodes] (
    [SpotPlanThirdMonitorCodeId] uniqueidentifier  NOT NULL,
    [AdPositionId] uniqueidentifier  NULL,
    [AdSpotPlanEdition_AdSpotPlanEditionId] uniqueidentifier  NULL,
    [AdMaterial_AdMaterialId] uniqueidentifier  NULL,
    [SpotDate] datetime  NULL,
    [ThirdMonitorCode_ThirdMonitorCodeId] uniqueidentifier  NOT NULL,
    [AdSpotPlanGroup_SpotPlanGroupId] uniqueidentifier  NULL
);
GO

-- Creating table 'AdSpotPlanTypes'
CREATE TABLE [dbo].[AdSpotPlanTypes] (
    [AdSpotPlanTypeId] uniqueidentifier  NOT NULL,
    [AdSpotPlanTypeName] nvarchar(50)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [Priority] smallint  NOT NULL,
    [MediaId] uniqueidentifier  NULL
);
GO

-- Creating table 'IpLibraries'
CREATE TABLE [dbo].[IpLibraries] (
    [IpLibraryId] uniqueidentifier  NOT NULL,
    [IpLibraryCode] nvarchar(50)  NOT NULL,
    [IpLibraryName] nvarchar(50)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [Enabled] bit  NOT NULL,
    [MediaId] uniqueidentifier  NULL
);
GO

-- Creating table 'MonitorTypes'
CREATE TABLE [dbo].[MonitorTypes] (
    [MonitorTypeId] uniqueidentifier  NOT NULL,
    [MonitorTypeName] nvarchar(50)  NOT NULL,
    [MonitorTypeCode] nvarchar(50)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [MediaId] uniqueidentifier  NULL,
    [PriceUnit] int  NOT NULL
);
GO

-- Creating table 'AdMaterialAssigns'
CREATE TABLE [dbo].[AdMaterialAssigns] (
    [MaterialAssignId] uniqueidentifier  NOT NULL,
    [MicroSlideshowSequenceNo] int  NULL,
    [AdSpotPlanDetail_AdPositionDetailId] uniqueidentifier  NOT NULL,
    [AdSpotPlanDetail_SpotDate] datetime  NOT NULL,
    [AdSpotPlanDetail_AdSpotPlanGroupId] uniqueidentifier  NOT NULL,
    [AdMaterial_AdMaterialId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'FrequencyControls'
CREATE TABLE [dbo].[FrequencyControls] (
    [FrequencyControlId] uniqueidentifier  NOT NULL,
    [FrequencyControlName] nvarchar(50)  NOT NULL,
    [FrequencyControlDetail] nvarchar(max)  NULL,
    [MediaId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'FrequencyControlDetails'
CREATE TABLE [dbo].[FrequencyControlDetails] (
    [FrequencyControlDetailId] uniqueidentifier  NOT NULL,
    [FrequencyControlPeriod] int  NOT NULL,
    [PeriodNumber] int  NOT NULL,
    [MaxCount] int  NULL,
    [MinCount] int  NULL,
    [FrequencyControl_FrequencyControlId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ThirdMonitorCodes'
CREATE TABLE [dbo].[ThirdMonitorCodes] (
    [ThirdMonitorCodeId] uniqueidentifier  NOT NULL,
    [MediaId] uniqueidentifier  NOT NULL,
    [AdMasterPlanId] uniqueidentifier  NULL,
    [MonitorCode] nvarchar(max)  NOT NULL,
    [Remark] nvarchar(max)  NULL,
    [MonitorType_MonitorTypeId] uniqueidentifier  NOT NULL,
    [ThirdMonitorMode_ThirdMonitorModeId] uniqueidentifier  NOT NULL,
    [ThirdMonitorCompany_ThirdMonitorCompanyId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ThirdMonitorModes'
CREATE TABLE [dbo].[ThirdMonitorModes] (
    [ThirdMonitorModeId] uniqueidentifier  NOT NULL,
    [ThirdMonitorModeName] nvarchar(50)  NOT NULL,
    [MediaId] uniqueidentifier  NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- Creating table 'ThirdMonitorCompanies'
CREATE TABLE [dbo].[ThirdMonitorCompanies] (
    [ThirdMonitorCompanyId] uniqueidentifier  NOT NULL,
    [MediaId] uniqueidentifier  NULL,
    [ThirdMonitorCompanyName] nvarchar(50)  NOT NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- Creating table 'FrequencyControlAdSpotPlanEdition'
CREATE TABLE [dbo].[FrequencyControlAdSpotPlanEdition] (
    [FrequencyControls_FrequencyControlId] uniqueidentifier  NOT NULL,
    [AdSpotPlanEditions_AdSpotPlanEditionId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AdSpotPlanGroupFrequencyControl'
CREATE TABLE [dbo].[AdSpotPlanGroupFrequencyControl] (
    [AdSpotPlanGroups_SpotPlanGroupId] uniqueidentifier  NOT NULL,
    [FrequencyControls_FrequencyControlId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AdMaterialFrequencyControl'
CREATE TABLE [dbo].[AdMaterialFrequencyControl] (
    [AdMaterials_AdMaterialId] uniqueidentifier  NOT NULL,
    [FrequencyControls_FrequencyControlId] uniqueidentifier  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [AdMaterialFormInfoId] in table 'AdMaterialFormInfoes'
ALTER TABLE [dbo].[AdMaterialFormInfoes]
ADD CONSTRAINT [PK_AdMaterialFormInfoes]
    PRIMARY KEY CLUSTERED ([AdMaterialFormInfoId] ASC);
GO

-- Creating primary key on [AdMaterialId] in table 'AdMaterials'
ALTER TABLE [dbo].[AdMaterials]
ADD CONSTRAINT [PK_AdMaterials]
    PRIMARY KEY CLUSTERED ([AdMaterialId] ASC);
GO

-- Creating primary key on [AdPositionDetailId], [SpotDate], [AdSpotPlanGroupId] in table 'AdSpotPlanDetails'
ALTER TABLE [dbo].[AdSpotPlanDetails]
ADD CONSTRAINT [PK_AdSpotPlanDetails]
    PRIMARY KEY CLUSTERED ([AdPositionDetailId], [SpotDate], [AdSpotPlanGroupId] ASC);
GO

-- Creating primary key on [AdSpotPlanEditionId] in table 'AdSpotPlanEditions'
ALTER TABLE [dbo].[AdSpotPlanEditions]
ADD CONSTRAINT [PK_AdSpotPlanEditions]
    PRIMARY KEY CLUSTERED ([AdSpotPlanEditionId] ASC);
GO

-- Creating primary key on [RedirectConditionDetailId] in table 'RedirectConditionDetails'
ALTER TABLE [dbo].[RedirectConditionDetails]
ADD CONSTRAINT [PK_RedirectConditionDetails]
    PRIMARY KEY CLUSTERED ([RedirectConditionDetailId] ASC);
GO

-- Creating primary key on [RedirectConditionId] in table 'RedirectConditions'
ALTER TABLE [dbo].[RedirectConditions]
ADD CONSTRAINT [PK_RedirectConditions]
    PRIMARY KEY CLUSTERED ([RedirectConditionId] ASC);
GO

-- Creating primary key on [SpotPlanGroupId] in table 'AdSpotPlanGroups'
ALTER TABLE [dbo].[AdSpotPlanGroups]
ADD CONSTRAINT [PK_AdSpotPlanGroups]
    PRIMARY KEY CLUSTERED ([SpotPlanGroupId] ASC);
GO

-- Creating primary key on [AdSpotPlanId] in table 'AdSpotPlans'
ALTER TABLE [dbo].[AdSpotPlans]
ADD CONSTRAINT [PK_AdSpotPlans]
    PRIMARY KEY CLUSTERED ([AdSpotPlanId] ASC);
GO

-- Creating primary key on [SpotPlanThirdMonitorCodeId] in table 'AdSpotPlanThirdMonitorCodes'
ALTER TABLE [dbo].[AdSpotPlanThirdMonitorCodes]
ADD CONSTRAINT [PK_AdSpotPlanThirdMonitorCodes]
    PRIMARY KEY CLUSTERED ([SpotPlanThirdMonitorCodeId] ASC);
GO

-- Creating primary key on [AdSpotPlanTypeId] in table 'AdSpotPlanTypes'
ALTER TABLE [dbo].[AdSpotPlanTypes]
ADD CONSTRAINT [PK_AdSpotPlanTypes]
    PRIMARY KEY CLUSTERED ([AdSpotPlanTypeId] ASC);
GO

-- Creating primary key on [IpLibraryId] in table 'IpLibraries'
ALTER TABLE [dbo].[IpLibraries]
ADD CONSTRAINT [PK_IpLibraries]
    PRIMARY KEY CLUSTERED ([IpLibraryId] ASC);
GO

-- Creating primary key on [MonitorTypeId] in table 'MonitorTypes'
ALTER TABLE [dbo].[MonitorTypes]
ADD CONSTRAINT [PK_MonitorTypes]
    PRIMARY KEY CLUSTERED ([MonitorTypeId] ASC);
GO

-- Creating primary key on [MaterialAssignId] in table 'AdMaterialAssigns'
ALTER TABLE [dbo].[AdMaterialAssigns]
ADD CONSTRAINT [PK_AdMaterialAssigns]
    PRIMARY KEY CLUSTERED ([MaterialAssignId] ASC);
GO

-- Creating primary key on [FrequencyControlId] in table 'FrequencyControls'
ALTER TABLE [dbo].[FrequencyControls]
ADD CONSTRAINT [PK_FrequencyControls]
    PRIMARY KEY CLUSTERED ([FrequencyControlId] ASC);
GO

-- Creating primary key on [FrequencyControlDetailId] in table 'FrequencyControlDetails'
ALTER TABLE [dbo].[FrequencyControlDetails]
ADD CONSTRAINT [PK_FrequencyControlDetails]
    PRIMARY KEY CLUSTERED ([FrequencyControlDetailId] ASC);
GO

-- Creating primary key on [ThirdMonitorCodeId] in table 'ThirdMonitorCodes'
ALTER TABLE [dbo].[ThirdMonitorCodes]
ADD CONSTRAINT [PK_ThirdMonitorCodes]
    PRIMARY KEY CLUSTERED ([ThirdMonitorCodeId] ASC);
GO

-- Creating primary key on [ThirdMonitorModeId] in table 'ThirdMonitorModes'
ALTER TABLE [dbo].[ThirdMonitorModes]
ADD CONSTRAINT [PK_ThirdMonitorModes]
    PRIMARY KEY CLUSTERED ([ThirdMonitorModeId] ASC);
GO

-- Creating primary key on [ThirdMonitorCompanyId] in table 'ThirdMonitorCompanies'
ALTER TABLE [dbo].[ThirdMonitorCompanies]
ADD CONSTRAINT [PK_ThirdMonitorCompanies]
    PRIMARY KEY CLUSTERED ([ThirdMonitorCompanyId] ASC);
GO

-- Creating primary key on [FrequencyControls_FrequencyControlId], [AdSpotPlanEditions_AdSpotPlanEditionId] in table 'FrequencyControlAdSpotPlanEdition'
ALTER TABLE [dbo].[FrequencyControlAdSpotPlanEdition]
ADD CONSTRAINT [PK_FrequencyControlAdSpotPlanEdition]
    PRIMARY KEY CLUSTERED ([FrequencyControls_FrequencyControlId], [AdSpotPlanEditions_AdSpotPlanEditionId] ASC);
GO

-- Creating primary key on [AdSpotPlanGroups_SpotPlanGroupId], [FrequencyControls_FrequencyControlId] in table 'AdSpotPlanGroupFrequencyControl'
ALTER TABLE [dbo].[AdSpotPlanGroupFrequencyControl]
ADD CONSTRAINT [PK_AdSpotPlanGroupFrequencyControl]
    PRIMARY KEY CLUSTERED ([AdSpotPlanGroups_SpotPlanGroupId], [FrequencyControls_FrequencyControlId] ASC);
GO

-- Creating primary key on [AdMaterials_AdMaterialId], [FrequencyControls_FrequencyControlId] in table 'AdMaterialFrequencyControl'
ALTER TABLE [dbo].[AdMaterialFrequencyControl]
ADD CONSTRAINT [PK_AdMaterialFrequencyControl]
    PRIMARY KEY CLUSTERED ([AdMaterials_AdMaterialId], [FrequencyControls_FrequencyControlId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [AdMaterial_AdMaterialId] in table 'AdMaterialFormInfoes'
ALTER TABLE [dbo].[AdMaterialFormInfoes]
ADD CONSTRAINT [FK_AdMaterialAdMaterialFormInfo]
    FOREIGN KEY ([AdMaterial_AdMaterialId])
    REFERENCES [dbo].[AdMaterials]
        ([AdMaterialId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdMaterialAdMaterialFormInfo'
CREATE INDEX [IX_FK_AdMaterialAdMaterialFormInfo]
ON [dbo].[AdMaterialFormInfoes]
    ([AdMaterial_AdMaterialId]);
GO

-- Creating foreign key on [AdMaterial_AdMaterialId] in table 'AdSpotPlanThirdMonitorCodes'
ALTER TABLE [dbo].[AdSpotPlanThirdMonitorCodes]
ADD CONSTRAINT [FK_AdMaterialThirdMonitorCode]
    FOREIGN KEY ([AdMaterial_AdMaterialId])
    REFERENCES [dbo].[AdMaterials]
        ([AdMaterialId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdMaterialThirdMonitorCode'
CREATE INDEX [IX_FK_AdMaterialThirdMonitorCode]
ON [dbo].[AdSpotPlanThirdMonitorCodes]
    ([AdMaterial_AdMaterialId]);
GO

-- Creating foreign key on [AdSpotPlanGroupId] in table 'AdSpotPlanDetails'
ALTER TABLE [dbo].[AdSpotPlanDetails]
ADD CONSTRAINT [FK_AdSpotPlanDetails_AdSpotPlanGroups]
    FOREIGN KEY ([AdSpotPlanGroupId])
    REFERENCES [dbo].[AdSpotPlanGroups]
        ([SpotPlanGroupId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdSpotPlanDetails_AdSpotPlanGroups'
CREATE INDEX [IX_FK_AdSpotPlanDetails_AdSpotPlanGroups]
ON [dbo].[AdSpotPlanDetails]
    ([AdSpotPlanGroupId]);
GO

-- Creating foreign key on [AdSpotPlan_AdSpotPlanId] in table 'AdSpotPlanEditions'
ALTER TABLE [dbo].[AdSpotPlanEditions]
ADD CONSTRAINT [FK_AdSpotPlanAdSpotPlanEdition]
    FOREIGN KEY ([AdSpotPlan_AdSpotPlanId])
    REFERENCES [dbo].[AdSpotPlans]
        ([AdSpotPlanId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdSpotPlanAdSpotPlanEdition'
CREATE INDEX [IX_FK_AdSpotPlanAdSpotPlanEdition]
ON [dbo].[AdSpotPlanEditions]
    ([AdSpotPlan_AdSpotPlanId]);
GO

-- Creating foreign key on [AdSpotPlanEdition_AdSpotPlanEditionId] in table 'AdSpotPlanGroups'
ALTER TABLE [dbo].[AdSpotPlanGroups]
ADD CONSTRAINT [FK_AdSpotPlanEditionSpotPlanGroup]
    FOREIGN KEY ([AdSpotPlanEdition_AdSpotPlanEditionId])
    REFERENCES [dbo].[AdSpotPlanEditions]
        ([AdSpotPlanEditionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdSpotPlanEditionSpotPlanGroup'
CREATE INDEX [IX_FK_AdSpotPlanEditionSpotPlanGroup]
ON [dbo].[AdSpotPlanGroups]
    ([AdSpotPlanEdition_AdSpotPlanEditionId]);
GO

-- Creating foreign key on [AdSpotPlanEdition_AdSpotPlanEditionId] in table 'AdSpotPlanThirdMonitorCodes'
ALTER TABLE [dbo].[AdSpotPlanThirdMonitorCodes]
ADD CONSTRAINT [FK_AdSpotPlanEditionThirdMonitorCode]
    FOREIGN KEY ([AdSpotPlanEdition_AdSpotPlanEditionId])
    REFERENCES [dbo].[AdSpotPlanEditions]
        ([AdSpotPlanEditionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdSpotPlanEditionThirdMonitorCode'
CREATE INDEX [IX_FK_AdSpotPlanEditionThirdMonitorCode]
ON [dbo].[AdSpotPlanThirdMonitorCodes]
    ([AdSpotPlanEdition_AdSpotPlanEditionId]);
GO

-- Creating foreign key on [AdSpotPlanType_AdSpotPlanTypeId] in table 'AdSpotPlanEditions'
ALTER TABLE [dbo].[AdSpotPlanEditions]
ADD CONSTRAINT [FK_AdSpotPlanTypeAdSpotPlanEdition]
    FOREIGN KEY ([AdSpotPlanType_AdSpotPlanTypeId])
    REFERENCES [dbo].[AdSpotPlanTypes]
        ([AdSpotPlanTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdSpotPlanTypeAdSpotPlanEdition'
CREATE INDEX [IX_FK_AdSpotPlanTypeAdSpotPlanEdition]
ON [dbo].[AdSpotPlanEditions]
    ([AdSpotPlanType_AdSpotPlanTypeId]);
GO

-- Creating foreign key on [IpLibrary_IpLibraryId] in table 'AdSpotPlanEditions'
ALTER TABLE [dbo].[AdSpotPlanEditions]
ADD CONSTRAINT [FK_IpLibraryAdSpotPlanEdition]
    FOREIGN KEY ([IpLibrary_IpLibraryId])
    REFERENCES [dbo].[IpLibraries]
        ([IpLibraryId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_IpLibraryAdSpotPlanEdition'
CREATE INDEX [IX_FK_IpLibraryAdSpotPlanEdition]
ON [dbo].[AdSpotPlanEditions]
    ([IpLibrary_IpLibraryId]);
GO

-- Creating foreign key on [RedirectCondition_RedirectConditionId] in table 'RedirectConditionDetails'
ALTER TABLE [dbo].[RedirectConditionDetails]
ADD CONSTRAINT [FK_AdSpotPlanRedirctConditionAdSpotPlanRedirctConditionDetail1]
    FOREIGN KEY ([RedirectCondition_RedirectConditionId])
    REFERENCES [dbo].[RedirectConditions]
        ([RedirectConditionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdSpotPlanRedirctConditionAdSpotPlanRedirctConditionDetail1'
CREATE INDEX [IX_FK_AdSpotPlanRedirctConditionAdSpotPlanRedirctConditionDetail1]
ON [dbo].[RedirectConditionDetails]
    ([RedirectCondition_RedirectConditionId]);
GO

-- Creating foreign key on [SpotPlanGroup_SpotPlanGroupId] in table 'RedirectConditions'
ALTER TABLE [dbo].[RedirectConditions]
ADD CONSTRAINT [FK_SpotPlanGroupAdSpotPlanGroupRedirctCondition]
    FOREIGN KEY ([SpotPlanGroup_SpotPlanGroupId])
    REFERENCES [dbo].[AdSpotPlanGroups]
        ([SpotPlanGroupId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SpotPlanGroupAdSpotPlanGroupRedirctCondition'
CREATE INDEX [IX_FK_SpotPlanGroupAdSpotPlanGroupRedirctCondition]
ON [dbo].[RedirectConditions]
    ([SpotPlanGroup_SpotPlanGroupId]);
GO

-- Creating foreign key on [AdSpotPlanDetail_AdPositionDetailId], [AdSpotPlanDetail_SpotDate], [AdSpotPlanDetail_AdSpotPlanGroupId] in table 'AdMaterialAssigns'
ALTER TABLE [dbo].[AdMaterialAssigns]
ADD CONSTRAINT [FK_AdSpotPlanDetailMaterialAssign]
    FOREIGN KEY ([AdSpotPlanDetail_AdPositionDetailId], [AdSpotPlanDetail_SpotDate], [AdSpotPlanDetail_AdSpotPlanGroupId])
    REFERENCES [dbo].[AdSpotPlanDetails]
        ([AdPositionDetailId], [SpotDate], [AdSpotPlanGroupId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdSpotPlanDetailMaterialAssign'
CREATE INDEX [IX_FK_AdSpotPlanDetailMaterialAssign]
ON [dbo].[AdMaterialAssigns]
    ([AdSpotPlanDetail_AdPositionDetailId], [AdSpotPlanDetail_SpotDate], [AdSpotPlanDetail_AdSpotPlanGroupId]);
GO

-- Creating foreign key on [AdMaterial_AdMaterialId] in table 'AdMaterialAssigns'
ALTER TABLE [dbo].[AdMaterialAssigns]
ADD CONSTRAINT [FK_AdMaterialMaterialAssign]
    FOREIGN KEY ([AdMaterial_AdMaterialId])
    REFERENCES [dbo].[AdMaterials]
        ([AdMaterialId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdMaterialMaterialAssign'
CREATE INDEX [IX_FK_AdMaterialMaterialAssign]
ON [dbo].[AdMaterialAssigns]
    ([AdMaterial_AdMaterialId]);
GO

-- Creating foreign key on [FrequencyControls_FrequencyControlId] in table 'FrequencyControlAdSpotPlanEdition'
ALTER TABLE [dbo].[FrequencyControlAdSpotPlanEdition]
ADD CONSTRAINT [FK_FrequencyControlAdSpotPlanEdition_FrequencyControl]
    FOREIGN KEY ([FrequencyControls_FrequencyControlId])
    REFERENCES [dbo].[FrequencyControls]
        ([FrequencyControlId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [AdSpotPlanEditions_AdSpotPlanEditionId] in table 'FrequencyControlAdSpotPlanEdition'
ALTER TABLE [dbo].[FrequencyControlAdSpotPlanEdition]
ADD CONSTRAINT [FK_FrequencyControlAdSpotPlanEdition_AdSpotPlanEdition]
    FOREIGN KEY ([AdSpotPlanEditions_AdSpotPlanEditionId])
    REFERENCES [dbo].[AdSpotPlanEditions]
        ([AdSpotPlanEditionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FrequencyControlAdSpotPlanEdition_AdSpotPlanEdition'
CREATE INDEX [IX_FK_FrequencyControlAdSpotPlanEdition_AdSpotPlanEdition]
ON [dbo].[FrequencyControlAdSpotPlanEdition]
    ([AdSpotPlanEditions_AdSpotPlanEditionId]);
GO

-- Creating foreign key on [AdSpotPlanGroups_SpotPlanGroupId] in table 'AdSpotPlanGroupFrequencyControl'
ALTER TABLE [dbo].[AdSpotPlanGroupFrequencyControl]
ADD CONSTRAINT [FK_AdSpotPlanGroupFrequencyControl_AdSpotPlanGroup]
    FOREIGN KEY ([AdSpotPlanGroups_SpotPlanGroupId])
    REFERENCES [dbo].[AdSpotPlanGroups]
        ([SpotPlanGroupId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [FrequencyControls_FrequencyControlId] in table 'AdSpotPlanGroupFrequencyControl'
ALTER TABLE [dbo].[AdSpotPlanGroupFrequencyControl]
ADD CONSTRAINT [FK_AdSpotPlanGroupFrequencyControl_FrequencyControl]
    FOREIGN KEY ([FrequencyControls_FrequencyControlId])
    REFERENCES [dbo].[FrequencyControls]
        ([FrequencyControlId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdSpotPlanGroupFrequencyControl_FrequencyControl'
CREATE INDEX [IX_FK_AdSpotPlanGroupFrequencyControl_FrequencyControl]
ON [dbo].[AdSpotPlanGroupFrequencyControl]
    ([FrequencyControls_FrequencyControlId]);
GO

-- Creating foreign key on [AdMaterials_AdMaterialId] in table 'AdMaterialFrequencyControl'
ALTER TABLE [dbo].[AdMaterialFrequencyControl]
ADD CONSTRAINT [FK_AdMaterialFrequencyControl_AdMaterial]
    FOREIGN KEY ([AdMaterials_AdMaterialId])
    REFERENCES [dbo].[AdMaterials]
        ([AdMaterialId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [FrequencyControls_FrequencyControlId] in table 'AdMaterialFrequencyControl'
ALTER TABLE [dbo].[AdMaterialFrequencyControl]
ADD CONSTRAINT [FK_AdMaterialFrequencyControl_FrequencyControl]
    FOREIGN KEY ([FrequencyControls_FrequencyControlId])
    REFERENCES [dbo].[FrequencyControls]
        ([FrequencyControlId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdMaterialFrequencyControl_FrequencyControl'
CREATE INDEX [IX_FK_AdMaterialFrequencyControl_FrequencyControl]
ON [dbo].[AdMaterialFrequencyControl]
    ([FrequencyControls_FrequencyControlId]);
GO

-- Creating foreign key on [FrequencyControl_FrequencyControlId] in table 'FrequencyControlDetails'
ALTER TABLE [dbo].[FrequencyControlDetails]
ADD CONSTRAINT [FK_FrequencyControlFrequencyControlDetail]
    FOREIGN KEY ([FrequencyControl_FrequencyControlId])
    REFERENCES [dbo].[FrequencyControls]
        ([FrequencyControlId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FrequencyControlFrequencyControlDetail'
CREATE INDEX [IX_FK_FrequencyControlFrequencyControlDetail]
ON [dbo].[FrequencyControlDetails]
    ([FrequencyControl_FrequencyControlId]);
GO

-- Creating foreign key on [ValuationMonitorType_MonitorTypeId] in table 'AdSpotPlanEditions'
ALTER TABLE [dbo].[AdSpotPlanEditions]
ADD CONSTRAINT [FK_MonitorTypeAdSpotPlanEdition]
    FOREIGN KEY ([ValuationMonitorType_MonitorTypeId])
    REFERENCES [dbo].[MonitorTypes]
        ([MonitorTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MonitorTypeAdSpotPlanEdition'
CREATE INDEX [IX_FK_MonitorTypeAdSpotPlanEdition]
ON [dbo].[AdSpotPlanEditions]
    ([ValuationMonitorType_MonitorTypeId]);
GO

-- Creating foreign key on [MonitorType_MonitorTypeId] in table 'ThirdMonitorCodes'
ALTER TABLE [dbo].[ThirdMonitorCodes]
ADD CONSTRAINT [FK_MonitorTypeThirdMonitorCode]
    FOREIGN KEY ([MonitorType_MonitorTypeId])
    REFERENCES [dbo].[MonitorTypes]
        ([MonitorTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MonitorTypeThirdMonitorCode'
CREATE INDEX [IX_FK_MonitorTypeThirdMonitorCode]
ON [dbo].[ThirdMonitorCodes]
    ([MonitorType_MonitorTypeId]);
GO

-- Creating foreign key on [ThirdMonitorCode_ThirdMonitorCodeId] in table 'AdSpotPlanThirdMonitorCodes'
ALTER TABLE [dbo].[AdSpotPlanThirdMonitorCodes]
ADD CONSTRAINT [FK_ThirdMonitorCodeAdSpotPlanThirdMonitorCode]
    FOREIGN KEY ([ThirdMonitorCode_ThirdMonitorCodeId])
    REFERENCES [dbo].[ThirdMonitorCodes]
        ([ThirdMonitorCodeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ThirdMonitorCodeAdSpotPlanThirdMonitorCode'
CREATE INDEX [IX_FK_ThirdMonitorCodeAdSpotPlanThirdMonitorCode]
ON [dbo].[AdSpotPlanThirdMonitorCodes]
    ([ThirdMonitorCode_ThirdMonitorCodeId]);
GO

-- Creating foreign key on [ThirdMonitorMode_ThirdMonitorModeId] in table 'ThirdMonitorCodes'
ALTER TABLE [dbo].[ThirdMonitorCodes]
ADD CONSTRAINT [FK_ThirdMonitorTypeThirdMonitorCode]
    FOREIGN KEY ([ThirdMonitorMode_ThirdMonitorModeId])
    REFERENCES [dbo].[ThirdMonitorModes]
        ([ThirdMonitorModeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ThirdMonitorTypeThirdMonitorCode'
CREATE INDEX [IX_FK_ThirdMonitorTypeThirdMonitorCode]
ON [dbo].[ThirdMonitorCodes]
    ([ThirdMonitorMode_ThirdMonitorModeId]);
GO

-- Creating foreign key on [AdSpotPlanGroup_SpotPlanGroupId] in table 'AdSpotPlanThirdMonitorCodes'
ALTER TABLE [dbo].[AdSpotPlanThirdMonitorCodes]
ADD CONSTRAINT [FK_AdSpotPlanGroupAdSpotPlanThirdMonitorCode]
    FOREIGN KEY ([AdSpotPlanGroup_SpotPlanGroupId])
    REFERENCES [dbo].[AdSpotPlanGroups]
        ([SpotPlanGroupId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdSpotPlanGroupAdSpotPlanThirdMonitorCode'
CREATE INDEX [IX_FK_AdSpotPlanGroupAdSpotPlanThirdMonitorCode]
ON [dbo].[AdSpotPlanThirdMonitorCodes]
    ([AdSpotPlanGroup_SpotPlanGroupId]);
GO

-- Creating foreign key on [ThirdMonitorCompany_ThirdMonitorCompanyId] in table 'ThirdMonitorCodes'
ALTER TABLE [dbo].[ThirdMonitorCodes]
ADD CONSTRAINT [FK_ThirdMonitorCompanyThirdMonitorCode]
    FOREIGN KEY ([ThirdMonitorCompany_ThirdMonitorCompanyId])
    REFERENCES [dbo].[ThirdMonitorCompanies]
        ([ThirdMonitorCompanyId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ThirdMonitorCompanyThirdMonitorCode'
CREATE INDEX [IX_FK_ThirdMonitorCompanyThirdMonitorCode]
ON [dbo].[ThirdMonitorCodes]
    ([ThirdMonitorCompany_ThirdMonitorCompanyId]);
GO

-- Creating foreign key on [AdSpotPlanEdition_AdSpotPlanEditionId] in table 'RedirectConditions'
ALTER TABLE [dbo].[RedirectConditions]
ADD CONSTRAINT [FK_AdSpotPlanEditionRedirctCondition]
    FOREIGN KEY ([AdSpotPlanEdition_AdSpotPlanEditionId])
    REFERENCES [dbo].[AdSpotPlanEditions]
        ([AdSpotPlanEditionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AdSpotPlanEditionRedirctCondition'
CREATE INDEX [IX_FK_AdSpotPlanEditionRedirctCondition]
ON [dbo].[RedirectConditions]
    ([AdSpotPlanEdition_AdSpotPlanEditionId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------