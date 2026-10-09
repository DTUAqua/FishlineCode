USE [FishLine]
GO


/* ============================================================
   1. Species groups
   ============================================================ */

CREATE TABLE [dbo].[L_SpeciesGroup](
    [L_speciesGroupId] [int] IDENTITY(1,1) NOT NULL,
    [speciesGroup] [nvarchar](100) NOT NULL,
    [description] [nvarchar](500) NULL,

    CONSTRAINT [PK_L_SpeciesGroup]
        PRIMARY KEY CLUSTERED ([L_speciesGroupId] ASC),

    CONSTRAINT [UQ_L_SpeciesGroup_speciesGroup]
        UNIQUE ([speciesGroup])
)
GO


/* ============================================================
   2. Add species grouping to L_Species

   Both are nullable because not every species necessarily has
   to be classified immediately.
   ============================================================ */

ALTER TABLE [dbo].[L_Species]
ADD
    [speciesGroupId] [int] NULL,
    [preySpeciesGroupId] [int] NULL
GO


ALTER TABLE [dbo].[L_Species]
ADD CONSTRAINT [FK_L_Species_L_SpeciesGroup]
    FOREIGN KEY ([speciesGroupId])
    REFERENCES [dbo].[L_SpeciesGroup] ([L_speciesGroupId])
GO


ALTER TABLE [dbo].[L_Species]
ADD CONSTRAINT [FK_L_Species_L_SpeciesGroup_Prey]
    FOREIGN KEY ([preySpeciesGroupId])
    REFERENCES [dbo].[L_SpeciesGroup] ([L_speciesGroupId])
GO


/* ============================================================
   3. Digestion stage code list
   ============================================================ */

CREATE TABLE [dbo].[L_DigestionStage](
    [L_digestionStageId] [int] IDENTITY(1,1) NOT NULL,
    [digestionStage] [nvarchar](100) NOT NULL,
    [description] [nvarchar](500) NULL,
    [num] [int] NULL,
    [digestionStageICES] [int] NULL,

    CONSTRAINT [PK_L_DigestionStage]
        PRIMARY KEY CLUSTERED ([L_digestionStageId] ASC)
)
GO


/* ============================================================
   4. Add stomach header information to AnimalInfo

   stomachStatusFirstEvaluationId already exists on AnimalInfo.
   ============================================================ */

ALTER TABLE [dbo].[AnimalInfo]
ADD
    [stomachStatusSecondEvaluationId] [int] NULL,
    [stomachHandlerId] [int] NULL--,
   -- [stomachFullWeight] [numeric](10,5) NULL,    --Maybe remove
   -- [stomachEmptyWeight] [numeric](10,5) NULL,   --Maybe remove
   -- [stomachContentWeight] [numeric](10,5) NULL  --Maybe remove
GO


ALTER TABLE [dbo].[AnimalInfo]
ADD CONSTRAINT [FK_AnimalInfo_L_StomachStatus_SecondEvaluation]
    FOREIGN KEY ([stomachStatusSecondEvaluationId])
    REFERENCES [dbo].[L_StomachStatus] ([L_StomachStatusId])
GO


ALTER TABLE [dbo].[AnimalInfo]
ADD CONSTRAINT [FK_AnimalInfo_DFUPerson_StomachHandler]
    FOREIGN KEY ([stomachHandlerId])
    REFERENCES [dbo].[DFUPerson] ([dfuPersonId])
GO


/* ============================================================
   5. Stomach

   One row = one prey/content registration.

   The stomach-level information is stored on AnimalInfo.
   ============================================================ */

CREATE TABLE [dbo].[Stomach](
    [stomachId] [int] IDENTITY(1,1) NOT NULL,

    [animalId] [int] NOT NULL,

    [preyNum] [int] NOT NULL,

    [preySpeciesCode] [nvarchar](3) NOT NULL,

    [preyDigestionStageId] [int] NULL,

    [preyNumber] [int] NULL,

    [preyWeight] [numeric](10,5) NULL,

    [preyLength] [int] NULL,

    [preyLengthMeasureUnit] [nvarchar](2) NULL,

    [preyLengthMeasureTypeId] [int] NULL,

    [stomachRemark] [nvarchar](250) NULL,

    CONSTRAINT [PK_Stomach]
        PRIMARY KEY CLUSTERED ([stomachId] ASC),

    CONSTRAINT [UQ_Stomach_Animal_PreyNum]
        UNIQUE ([animalId], [preyNum])
)
GO


/* ============================================================
   6. Stomach foreign keys
   ============================================================ */

ALTER TABLE [dbo].[Stomach]
ADD CONSTRAINT [FK_Stomach_Animal]
    FOREIGN KEY ([animalId])
    REFERENCES [dbo].[Animal] ([animalId])
    ON DELETE CASCADE
GO


/*
    L_Species uses speciesCode as its actual primary key in the
    existing FishLine database, so use that rather than
    L_speciesId.
*/
ALTER TABLE [dbo].[Stomach]
ADD CONSTRAINT [FK_Stomach_L_Species]
    FOREIGN KEY ([preySpeciesCode])
    REFERENCES [dbo].[L_Species] ([speciesCode])
GO


ALTER TABLE [dbo].[Stomach]
ADD CONSTRAINT [FK_Stomach_L_DigestionStage]
    FOREIGN KEY ([preyDigestionStageId])
    REFERENCES [dbo].[L_DigestionStage] ([L_digestionStageId])
GO


ALTER TABLE [dbo].[Stomach]
ADD CONSTRAINT [FK_Stomach_L_LengthMeasureType]
    FOREIGN KEY ([preyLengthMeasureTypeId])
    REFERENCES [dbo].[L_LengthMeasureType] ([L_lengthMeasureTypeId])
GO


/*
    Existing FishLine uses the textual lengthMeasureUnit as the
    key referenced from data tables rather than the identity ID.
*/
ALTER TABLE [dbo].[Stomach]
ADD CONSTRAINT [FK_Stomach_L_LengthMeasureUnit]
    FOREIGN KEY ([preyLengthMeasureUnit])
    REFERENCES [dbo].[L_LengthMeasureUnit] ([lengthMeasureUnit])
GO


/* ============================================================
   7. Indexes for foreign keys / common lookups
   ============================================================ */

CREATE NONCLUSTERED INDEX [IX_L_Species_speciesGroupId]
ON [dbo].[L_Species] ([speciesGroupId])
GO

CREATE NONCLUSTERED INDEX [IX_L_Species_preySpeciesGroupId]
ON [dbo].[L_Species] ([preySpeciesGroupId])
GO

CREATE NONCLUSTERED INDEX [IX_AnimalInfo_stomachStatusSecondEvaluationId]
ON [dbo].[AnimalInfo] ([stomachStatusSecondEvaluationId])
GO

CREATE NONCLUSTERED INDEX [IX_AnimalInfo_stomachHandlerId]
ON [dbo].[AnimalInfo] ([stomachHandlerId])
GO

CREATE NONCLUSTERED INDEX [IX_Stomach_preySpeciesCode]
ON [dbo].[Stomach] ([preySpeciesCode])
GO

CREATE NONCLUSTERED INDEX [IX_Stomach_preyDigestionStageId]
ON [dbo].[Stomach] ([preyDigestionStageId])
GO