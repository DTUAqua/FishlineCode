USE [FishLine]
GO

-- TODOs
-- Sample: Move selectionDeviceSource after selectionDevice
-- DW Sample: Move selectionDeviceSource after selectionDevice 
-- Animal: Move lengthMeasureTypeId after lengthMeasureUnit
-- L_Species: Move standardLengthMeasureTypeId before lengthMin
-- DW Animal: Move lengthMeasureType after lengthMeasureUnit
-- DW AnimalRaised: Move lengthMeasureType after lengthMeasureUnit
-- DW Age: Move lengthMeasureType after Length

ALTER TABLE AnimalInfo
DROP COLUMN [lengthNoseAnus]
GO


ALTER TABLE AnimalInfo
DROP COLUMN [lengthCircum]
GO


ALTER TABLE Animal
ADD [lengthMeasureTypeId] int  NULL
GO


ALTER TABLE L_OtolithReadingRemark
ADD [transAgeFromAquaDotsToFishLine] bit NULL
GO


UPDATE L_OtolithReadingRemark
SET [transAgeFromAquaDotsToFishLine] = 1
GO


UPDATE L_OtolithReadingRemark
SET [transAgeFromAquaDotsToFishLine] = 0
WHERE [otolithReadingRemark] = 'AQ3'
GO


ALTER TABLE L_OtolithReadingRemark
ALTER COLUMN [transAgeFromAquaDotsToFishLine] bit NOT NULL
GO


ALTER TABLE L_Species
ADD [standardLengthMeasureTypeId] int  NULL
GO


ALTER TABLE Sample
ADD  [selectionDeviceSourceId] int  NULL
GO



-- Creating table 'L_SelectionDeviceSource'
CREATE TABLE [dbo].[L_SelectionDeviceSource] (
    [L_selectionDeviceSourceId] int IDENTITY(1,1) NOT NULL,
    [selectionDeviceSource] nvarchar(2)  NOT NULL,
    [description] nvarchar(500)  NULL,
    [num] int  NULL
);
GO

-- Creating table 'L_LengthMeasureType'
CREATE TABLE [dbo].[L_LengthMeasureType] (
    [L_lengthMeasureTypeId] int IDENTITY(1,1) NOT NULL,
    [lengthMeasureType] nvarchar(5)  NOT NULL,
    [description] nvarchar(500)  NULL,
    [num] int  NULL,
    [RDBES] nvarchar(50)  NULL
);
GO

-- Creating primary key on [L_selectionDeviceSourceId] in table 'L_SelectionDeviceSource'
ALTER TABLE [dbo].[L_SelectionDeviceSource]
ADD CONSTRAINT [PK_L_SelectionDeviceSource]
    PRIMARY KEY CLUSTERED ([L_selectionDeviceSourceId] ASC);
GO

-- Creating primary key on [L_lengthMeasureTypeId] in table 'L_LengthMeasureType'
ALTER TABLE [dbo].[L_LengthMeasureType]
ADD CONSTRAINT [PK_L_LengthMeasureType]
    PRIMARY KEY CLUSTERED ([L_lengthMeasureTypeId] ASC);
GO



-- Creating foreign key on [selectionDeviceSourceId] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [FK_L_SelectionDeviceSourceSample]
    FOREIGN KEY ([selectionDeviceSourceId])
    REFERENCES [dbo].[L_SelectionDeviceSource]
        ([L_selectionDeviceSourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_SelectionDeviceSourceSample'
CREATE INDEX [IX_FK_L_SelectionDeviceSourceSample]
ON [dbo].[Sample]
    ([selectionDeviceSourceId]);
GO

-- Creating foreign key on [lengthMeasureTypeId] in table 'Animal'
ALTER TABLE [dbo].[Animal]
ADD CONSTRAINT [FK_L_LengthMeasureTypeAnimal]
    FOREIGN KEY ([lengthMeasureTypeId])
    REFERENCES [dbo].[L_LengthMeasureType]
        ([L_lengthMeasureTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_LengthMeasureTypeAnimal'
CREATE INDEX [IX_FK_L_LengthMeasureTypeAnimal]
ON [dbo].[Animal]
    ([lengthMeasureTypeId]);
GO

-- Creating foreign key on [standardLengthMeasureTypeId] in table 'L_Species'
ALTER TABLE [dbo].[L_Species]
ADD CONSTRAINT [FK_L_LengthMeasureTypeL_Species]
    FOREIGN KEY ([standardLengthMeasureTypeId])
    REFERENCES [dbo].[L_LengthMeasureType]
        ([L_lengthMeasureTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_LengthMeasureTypeL_Species'
CREATE INDEX [IX_FK_L_LengthMeasureTypeL_Species]
ON [dbo].[L_Species]
    ([standardLengthMeasureTypeId]);
GO


-- prefill/select default length measure type 
INSERT INTO [dbo].[L_LengthMeasureType]
           ([lengthMeasureType],[description],[num],[RDBES])
VALUES ('TL', 'Total længde', 1, NULL)
GO

DECLARE @intMeasureTypeId int

SELECT @intMeasureTypeId =  L_lengthMeasureTypeId
FROM [dbo].[L_LengthMeasureType]
WHERE lengthMeasureType = 'TL'

UPDATE [dbo].[L_Species] 
SET [standardLengthMeasureTypeId] = @intMeasureTypeId
FROM [dbo].[L_Species] 
GO


USE [FishLineDW]
GO

-- Add sdAgeInfoUpdated column.
ALTER TABLE [dbo].[Sample]
ADD [selectionDeviceSource] nvarchar(2) NULL
GO

ALTER TABLE [dbo].[Animal]
ADD [lengthMeasureType] nvarchar(5) NULL
GO

ALTER TABLE [dbo].[AnimalRaised]
ADD [lengthMeasureType] nvarchar(5) NULL
GO

ALTER TABLE [dbo].[Age]
ADD [lengthMeasureType] nvarchar(5) NULL
GO
