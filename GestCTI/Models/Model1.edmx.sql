
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/01/2018 09:32:03
-- Generated from EDMX file: D:\Trabajo\Proyectos\cti\GestCTI\Models\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [DBCTI];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Campaing_CampaingType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Campaign] DROP CONSTRAINT [FK_Campaing_CampaingType];
GO
IF OBJECT_ID(N'[dbo].[FK_Campaing_Company]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Campaign] DROP CONSTRAINT [FK_Campaing_Company];
GO
IF OBJECT_ID(N'[dbo].[FK_CampaingSkills_Campaing]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CampaignSkills] DROP CONSTRAINT [FK_CampaingSkills_Campaing];
GO
IF OBJECT_ID(N'[dbo].[FK_CampaingSkills_Skills]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CampaignSkills] DROP CONSTRAINT [FK_CampaingSkills_Skills];
GO
IF OBJECT_ID(N'[dbo].[FK_Company_Switch]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Company] DROP CONSTRAINT [FK_Company_Switch];
GO
IF OBJECT_ID(N'[dbo].[FK_Company_Users]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Company] DROP CONSTRAINT [FK_Company_Users];
GO
IF OBJECT_ID(N'[dbo].[FK_Users_Company]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_Users_Company];
GO
IF OBJECT_ID(N'[dbo].[FK_Users_Location]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_Users_Location];
GO
IF OBJECT_ID(N'[dbo].[FK_UserSkill_Skill]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserSkill] DROP CONSTRAINT [FK_UserSkill_Skill];
GO
IF OBJECT_ID(N'[dbo].[FK_UserSkill_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserSkill] DROP CONSTRAINT [FK_UserSkill_User];
GO
IF OBJECT_ID(N'[dbo].[FK_VDN_Campaing]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VDN] DROP CONSTRAINT [FK_VDN_Campaing];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Campaign]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Campaign];
GO
IF OBJECT_ID(N'[dbo].[CampaignSkills]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CampaignSkills];
GO
IF OBJECT_ID(N'[dbo].[CampaignType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CampaignType];
GO
IF OBJECT_ID(N'[dbo].[Company]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Company];
GO
IF OBJECT_ID(N'[dbo].[Skills]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Skills];
GO
IF OBJECT_ID(N'[dbo].[Switch]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Switch];
GO
IF OBJECT_ID(N'[dbo].[UserLocation]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserLocation];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[UserSkill]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserSkill];
GO
IF OBJECT_ID(N'[dbo].[VDN]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VDN];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Switch'
CREATE TABLE [dbo].[Switch] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [WebSocketIP] nvarchar(50)  NOT NULL,
    [ApiServerIP] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'UserLocation'
CREATE TABLE [dbo].[UserLocation] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'UserSkill'
CREATE TABLE [dbo].[UserSkill] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IdSkill] int  NOT NULL,
    [IdUser] int  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Username] nvarchar(50)  NOT NULL,
    [Password] nvarchar(50)  NOT NULL,
    [email] nvarchar(50)  NULL,
    [FirstName] nvarchar(50)  NULL,
    [MiddleName] nvarchar(50)  NULL,
    [LastName] nvarchar(50)  NULL,
    [Role] nvarchar(20)  NOT NULL,
    [IdLocation] int  NOT NULL,
    [IdCompany] int  NOT NULL,
    [Active] bit  NOT NULL
);
GO

-- Creating table 'Campaign'
CREATE TABLE [dbo].[Campaign] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(50)  NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [IdType] int  NOT NULL,
    [IdCompany] int  NOT NULL
);
GO

-- Creating table 'CampaignSkills'
CREATE TABLE [dbo].[CampaignSkills] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IdSkill] int  NOT NULL,
    [IdCampaign] int  NOT NULL
);
GO

-- Creating table 'CampaignType'
CREATE TABLE [dbo].[CampaignType] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] int  NOT NULL
);
GO

-- Creating table 'Company'
CREATE TABLE [dbo].[Company] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [SwitchCompanyId] int  NOT NULL,
    [SwitchId] int  NOT NULL,
    [CreateBy] int  NULL
);
GO

-- Creating table 'Skills'
CREATE TABLE [dbo].[Skills] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Value] nvarchar(50)  NOT NULL,
    [Description] nvarchar(50)  NULL
);
GO

-- Creating table 'VDN'
CREATE TABLE [dbo].[VDN] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Value] nvarchar(50)  NOT NULL,
    [Description] nvarchar(50)  NULL,
    [IdCampaign] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Switch'
ALTER TABLE [dbo].[Switch]
ADD CONSTRAINT [PK_Switch]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserLocation'
ALTER TABLE [dbo].[UserLocation]
ADD CONSTRAINT [PK_UserLocation]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserSkill'
ALTER TABLE [dbo].[UserSkill]
ADD CONSTRAINT [PK_UserSkill]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Campaign'
ALTER TABLE [dbo].[Campaign]
ADD CONSTRAINT [PK_Campaign]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CampaignSkills'
ALTER TABLE [dbo].[CampaignSkills]
ADD CONSTRAINT [PK_CampaignSkills]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CampaignType'
ALTER TABLE [dbo].[CampaignType]
ADD CONSTRAINT [PK_CampaignType]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Company'
ALTER TABLE [dbo].[Company]
ADD CONSTRAINT [PK_Company]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Skills'
ALTER TABLE [dbo].[Skills]
ADD CONSTRAINT [PK_Skills]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VDN'
ALTER TABLE [dbo].[VDN]
ADD CONSTRAINT [PK_VDN]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [IdLocation] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_Users_Location]
    FOREIGN KEY ([IdLocation])
    REFERENCES [dbo].[UserLocation]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Users_Location'
CREATE INDEX [IX_FK_Users_Location]
ON [dbo].[Users]
    ([IdLocation]);
GO

-- Creating foreign key on [IdUser] in table 'UserSkill'
ALTER TABLE [dbo].[UserSkill]
ADD CONSTRAINT [FK_UserSkill_User]
    FOREIGN KEY ([IdUser])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserSkill_User'
CREATE INDEX [IX_FK_UserSkill_User]
ON [dbo].[UserSkill]
    ([IdUser]);
GO

-- Creating foreign key on [IdType] in table 'Campaign'
ALTER TABLE [dbo].[Campaign]
ADD CONSTRAINT [FK_Campaing_CampaingType]
    FOREIGN KEY ([IdType])
    REFERENCES [dbo].[CampaignType]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Campaing_CampaingType'
CREATE INDEX [IX_FK_Campaing_CampaingType]
ON [dbo].[Campaign]
    ([IdType]);
GO

-- Creating foreign key on [IdCompany] in table 'Campaign'
ALTER TABLE [dbo].[Campaign]
ADD CONSTRAINT [FK_Campaing_Company]
    FOREIGN KEY ([IdCompany])
    REFERENCES [dbo].[Company]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Campaing_Company'
CREATE INDEX [IX_FK_Campaing_Company]
ON [dbo].[Campaign]
    ([IdCompany]);
GO

-- Creating foreign key on [IdCampaign] in table 'CampaignSkills'
ALTER TABLE [dbo].[CampaignSkills]
ADD CONSTRAINT [FK_CampaingSkills_Campaing]
    FOREIGN KEY ([IdCampaign])
    REFERENCES [dbo].[Campaign]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CampaingSkills_Campaing'
CREATE INDEX [IX_FK_CampaingSkills_Campaing]
ON [dbo].[CampaignSkills]
    ([IdCampaign]);
GO

-- Creating foreign key on [IdCampaign] in table 'VDN'
ALTER TABLE [dbo].[VDN]
ADD CONSTRAINT [FK_VDN_Campaing]
    FOREIGN KEY ([IdCampaign])
    REFERENCES [dbo].[Campaign]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_VDN_Campaing'
CREATE INDEX [IX_FK_VDN_Campaing]
ON [dbo].[VDN]
    ([IdCampaign]);
GO

-- Creating foreign key on [IdSkill] in table 'CampaignSkills'
ALTER TABLE [dbo].[CampaignSkills]
ADD CONSTRAINT [FK_CampaingSkills_Skills]
    FOREIGN KEY ([IdSkill])
    REFERENCES [dbo].[Skills]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CampaingSkills_Skills'
CREATE INDEX [IX_FK_CampaingSkills_Skills]
ON [dbo].[CampaignSkills]
    ([IdSkill]);
GO

-- Creating foreign key on [SwitchId] in table 'Company'
ALTER TABLE [dbo].[Company]
ADD CONSTRAINT [FK_Company_Switch]
    FOREIGN KEY ([SwitchId])
    REFERENCES [dbo].[Switch]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Company_Switch'
CREATE INDEX [IX_FK_Company_Switch]
ON [dbo].[Company]
    ([SwitchId]);
GO

-- Creating foreign key on [CreateBy] in table 'Company'
ALTER TABLE [dbo].[Company]
ADD CONSTRAINT [FK_Company_Users]
    FOREIGN KEY ([CreateBy])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Company_Users'
CREATE INDEX [IX_FK_Company_Users]
ON [dbo].[Company]
    ([CreateBy]);
GO

-- Creating foreign key on [IdCompany] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_Users_Company]
    FOREIGN KEY ([IdCompany])
    REFERENCES [dbo].[Company]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Users_Company'
CREATE INDEX [IX_FK_Users_Company]
ON [dbo].[Users]
    ([IdCompany]);
GO

-- Creating foreign key on [IdSkill] in table 'UserSkill'
ALTER TABLE [dbo].[UserSkill]
ADD CONSTRAINT [FK_UserSkill_Skill]
    FOREIGN KEY ([IdSkill])
    REFERENCES [dbo].[Skills]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserSkill_Skill'
CREATE INDEX [IX_FK_UserSkill_Skill]
ON [dbo].[UserSkill]
    ([IdSkill]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------