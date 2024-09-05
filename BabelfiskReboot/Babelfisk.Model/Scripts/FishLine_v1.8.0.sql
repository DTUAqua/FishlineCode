USE [FishLine]
GO


-- Change L_TripType.tripType from nvarchar(3) to nvarchar(6) (and associated relationship columns)
-- Remove all constraints for tripType on L_TripType
BEGIN
	ALTER TABLE [dbo].[Trip] DROP CONSTRAINT [FK_Trip_L_TripType]
	ALTER TABLE [dbo].[L_TripType] DROP CONSTRAINT [PK_L_TripType]
END


-- Alter tripType sizes on all tables
BEGIN
    ALTER TABLE [dbo].[L_TripType]
	ALTER COLUMN [tripType] [nvarchar](6) NOT NULL

	ALTER TABLE [dbo].[Trip]
	ALTER COLUMN [tripType] [nvarchar](6) NULL
END


-- Re-add contraints
BEGIN
    ALTER TABLE [dbo].[L_TripType]
    ADD CONSTRAINT [PK_L_TripType] PRIMARY KEY CLUSTERED 
	(
		[tripType] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

    ALTER TABLE [dbo].[Trip]  WITH NOCHECK ADD  CONSTRAINT [FK_Trip_L_TripType] FOREIGN KEY([tripType]) 
    REFERENCES [dbo].[L_TripType] ([tripType])
END


-- Add new trip type lookups
 INSERT INTO [dbo].[L_TripType] ([tripType],[description],[cruiseType])
 VALUES ('REKHVN', 'Rekreativ, havneindsamling', 'VID'),
        ('REKTBD', 'Rekreativ, turbåd', 'VID'),
	    ('REKOMR', 'Rekreativ, område', 'VID')
GO



ALTER TABLE [dbo].[Trip]
ADD [sgTripId] nvarchar(200)  NULL,
    [tripNum] nvarchar(200)  NULL,
    [placeName] nvarchar(200)  NULL,
    [placeCode] nvarchar(200)  NULL,
    [postalCode] int  NULL,
    [numberInPlace] int  NULL,
    [respYes] int  NULL,
    [respNo] int  NULL,
    [respTot] int  NULL
    
GO



-- Add extra Animal fields
ALTER TABLE [dbo].[Animal]
ADD [catchNum] int  NULL,
    [otolithFinScale] bit  NULL
GO


ALTER TABLE [dbo].[Sample]
ADD [sgId] nvarchar(200)  NULL,
    [weekdayWeekend] nvarchar(200)  NULL
GO

-- Creating table 'L_Applications'
CREATE TABLE [dbo].[L_Applications] (
    [L_applicationId] int IDENTITY(1,1) NOT NULL,
    [code] nvarchar(200)  NOT NULL,
    [description] nvarchar(300)  NULL,
    [num] int  NULL
);
GO

-- Creating primary key on [L_applicationId] in table 'L_Applications'
ALTER TABLE [dbo].[L_Applications]
ADD CONSTRAINT [PK_L_Applications]
    PRIMARY KEY CLUSTERED ([L_applicationId] ASC);
GO

	
ALTER TABLE [dbo].[SpeciesList]
ADD [applicationId] int  NULL
GO


-- Creating foreign key on [applicationId] in table 'SpeciesList'
ALTER TABLE [dbo].[SpeciesList]
ADD CONSTRAINT [FK_L_ApplicationSpeciesList]
    FOREIGN KEY ([applicationId])
    REFERENCES [dbo].[L_Applications] ([L_applicationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_L_ApplicationSpeciesList'
CREATE INDEX [IX_FK_L_ApplicationSpeciesList]
ON [dbo].[SpeciesList] ([applicationId]);
GO


-- Add application id to constraint for unique rows
BEGIN
	ALTER TABLE [dbo].[SpeciesList] DROP CONSTRAINT [IX_SpeciesList]
END

-- Re-add contraints
BEGIN
    ALTER TABLE [dbo].[SpeciesList]
    ADD CONSTRAINT [IX_SpeciesList] UNIQUE NONCLUSTERED 
	(
	[sampleId] ASC,
	[speciesCode] ASC,
	[landingCategory] ASC,
	[sizeSortingEU] ASC,
	[sizeSortingDFU] ASC,
	[sexCode] ASC,
	[ovigorous] ASC,
	[cuticulaHardness] ASC,
	[applicationId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
END

GO



USE [FishLineDW]
GO

-- Alter trip type from 3 to 6 characters.
ALTER TABLE [dbo].[Trip]
ALTER COLUMN [tripType] [nvarchar](6) NULL
GO

ALTER TABLE [dbo].[Sample]
ALTER COLUMN [tripType] [nvarchar](6) NULL
GO

ALTER TABLE [dbo].[SpeciesList]
ALTER COLUMN [tripType] [nvarchar](6) NULL
GO

ALTER TABLE [dbo].[SpeciesList]
ALTER COLUMN [tripType] [nvarchar](6) NULL
GO

ALTER TABLE [dbo].[SpeciesListRaised]
ALTER COLUMN [tripType] [nvarchar](6) NULL
GO

ALTER TABLE [dbo].[Animal]
ALTER COLUMN [tripType] [nvarchar](6) NULL
GO

ALTER TABLE [dbo].[AnimalRaised]
ALTER COLUMN [tripType] [nvarchar](6) NULL
GO

ALTER TABLE [dbo].[Age]
ALTER COLUMN [tripType] [nvarchar](6) NULL
GO



ALTER TABLE [dbo].[Trip]
ADD [sgTripId] nvarchar(200)  NULL,
    [tripNum] nvarchar(200)  NULL,
    [placeName] nvarchar(200)  NULL,
    [placeCode] nvarchar(200)  NULL,
    [postalCode] int  NULL,
    [numberInPlace] int  NULL,
    [respYes] int  NULL,
    [respNo] int  NULL,
    [respTot] int  NULL
    
GO

-- Add extra Animal fields
ALTER TABLE [dbo].[Animal]
ADD [catchNum] int  NULL,
    [otolithFinScale] bit  NULL
GO

ALTER TABLE [dbo].[Sample]
ADD [sgId] nvarchar(200)  NULL,
    [weekdayWeekend] nvarchar(200)  NULL
GO

ALTER TABLE [dbo].[SpeciesList]
ADD  [applicationCode] nvarchar(200)  NULL
GO

-- Add application id to constraint for unique rows
BEGIN
	ALTER TABLE [dbo].[SpeciesList] DROP CONSTRAINT [IXX_SpeciesList]
END

-- Re-add contraints
BEGIN
    ALTER TABLE [dbo].[SpeciesList]
    ADD CONSTRAINT [IXX_SpeciesList] UNIQUE NONCLUSTERED 
	(
	[sampleId] ASC,
	[speciesCode] ASC,
	[landingCategory] ASC,
	[sizeSortingEU] ASC,
	[sizeSortingDFU] ASC,
	[sexCode] ASC,
	[ovigorous] ASC,
	[cuticulaHardness] ASC,
	[applicationCode] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
END



use [FishLine]
GO

-- Add hatch month readability lookup table 'L_HatchMonthReadability'
CREATE TABLE [dbo].[L_HatchMonthReadability] (
    [L_HatchMonthReadabilityId] int IDENTITY(1,1) NOT NULL,
    [hatchMonthRemark] nvarchar(10)  NOT NULL,
    [description] nvarchar(250)  NULL,
    [num] int  NULL
);
GO

-- Creating primary key on [L_HatchMonthReadabilityId] in table 'L_HatchMonthReadability'
ALTER TABLE [dbo].[L_HatchMonthReadability]
ADD CONSTRAINT [PK_L_HatchMonthReadability]
    PRIMARY KEY CLUSTERED ([L_HatchMonthReadabilityId] ASC);
GO

-- Add hatch month readability 
ALTER TABLE [dbo].[Age]
ADD [hatchMonthReadabilityId] int  NULL
GO


-- Creating foreign key on [hatchMonthReadabilityId] in table 'Age'
ALTER TABLE [dbo].[Age]
ADD CONSTRAINT [FK_L_HatchMonthReadabilityAge]
    FOREIGN KEY ([hatchMonthReadabilityId])
    REFERENCES [dbo].[L_HatchMonthReadability]
        ([L_HatchMonthReadabilityId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_L_HatchMonthReadabilityAge'
CREATE INDEX [IX_FK_L_HatchMonthReadabilityAge]
ON [dbo].[Age]
    ([hatchMonthReadabilityId]);
GO



INSERT INTO [FishLine].[dbo].[L_HatchMonthReadability]
           ([hatchMonthRemark],[description],[num])
VALUES
           ('A','Let læselig med tydelige strukturer, ingen tvivl om klækningsmåneden',1),
		   ('B','Ikke så let at læse, men dog klar nok til, at der ingen tvivl er om klækningsmåneden',2),
		   ('C','Ikke let at læse, utydelige strukturer, nogen tvivl om klækningsmåneden',3),
		   ('D','Helt umulig at læse, ingen tydelige strukturer, klækningsmåneden udeladt/gættet',4)
GO



use [FishLineDW]
GO

-- Remember to move the hatchMonthRemark column up under hatchMonth in DW in Management Studio.
ALTER TABLE [dbo].[Age]
ADD [hatchMonthRemark] nvarchar(10) NULL
GO



