USE [FishLine]
GO

-- TODOs after RUN
-- Animal: Move [stomachStatusFirstEvaluation] after weightGonads in FishLineDW.Animal via. SSMS (SQL Server Management Studio).



ALTER TABLE AnimalInfo
ADD  [stomachStatusFirstEvaluationId] int  NULL
GO

-- Creating table 'L_StomachStatus'
CREATE TABLE [dbo].[L_StomachStatus] (
    [L_StomachStatusId] int IDENTITY(1,1) NOT NULL,
    [stomachStatus] nvarchar(20)  NOT NULL,
    [description] nvarchar(500)  NULL,
    [num] int  NULL,
    [showInAnimal] bit  NOT NULL,
    [showInStomach] bit  NOT NULL
);
GO

-- Creating primary key on [L_StomachStatusId] in table 'L_StomachStatus'
ALTER TABLE [dbo].[L_StomachStatus]
ADD CONSTRAINT [PK_L_StomachStatus]
    PRIMARY KEY CLUSTERED ([L_StomachStatusId] ASC);
GO

-- Creating foreign key on [stomachStatusFirstEvaluationId] in table 'AnimalInfo'
ALTER TABLE [dbo].[AnimalInfo]
ADD CONSTRAINT [FK_AnimalInfo_L_StomachStatus]
    FOREIGN KEY ([stomachStatusFirstEvaluationId])
    REFERENCES [dbo].[L_StomachStatus] ([L_StomachStatusId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AnimalInfo_L_StomachStatus'
CREATE INDEX [IX_FK_AnimalInfo_L_StomachStatus]
ON [dbo].[AnimalInfo]
    ([stomachStatusFirstEvaluationId]);
GO





USE [FishLineDW]
GO

ALTER TABLE [dbo].[Animal]
ADD [stomachStatusFirstEvaluation] nvarchar(20) NULL
GO

