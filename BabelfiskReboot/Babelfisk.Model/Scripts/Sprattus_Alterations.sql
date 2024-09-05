USE [sprattus]
GO

-- Adding 2 new columns to L_Harbour
ALTER TABLE [dbo].[L_Harbour]
ADD [harbourNES] [nvarchar] (8) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [harbourEU] [nvarchar] (8) COLLATE SQL_Latin1_General_CP1_CI_AS NULL


-- ALTER Trip table with new columns (4 new) 
ALTER TABLE [dbo].[Trip]
ADD [logBldNr] [nvarchar] (10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

ALTER TABLE [dbo].[Trip]
ADD [fDFVessel] [bit] NULL
GO

ALTER TABLE [dbo].[Trip]
ADD [platform1] [nvarchar] (6) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [platform2] [nvarchar] (6) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

ALTER TABLE [dbo].[Trip] WITH CHECK ADD CONSTRAINT [FK_Trip_L_Platform1] FOREIGN KEY([platform1])
REFERENCES [dbo].[L_Platform] ([platform])
GO
ALTER TABLE [dbo].[Trip] CHECK CONSTRAINT [FK_Trip_L_Platform1]
GO

ALTER TABLE [dbo].[Trip] WITH CHECK ADD CONSTRAINT [FK_Trip_L_Platform2] FOREIGN KEY([platform2])
REFERENCES [dbo].[L_Platform] ([platform])
GO
ALTER TABLE [dbo].[Trip] CHECK CONSTRAINT [FK_Trip_L_Platform2]



-- ALTER Person table with new columns (4 new)
ALTER TABLE [dbo].[Person]
ADD [telephonePrivate] [nvarchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [telephoneMobile] [nvarchar] (30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[email] [nvarchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[facebook] [nvarchar] (150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO


-- ADD new L_SelectionDevice table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[L_SelectionDevice](
	[L_selectionDeviceId] [int] IDENTITY(1,1) NOT NULL,
	[selectionDevice] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[description] [nvarchar](80) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_L_SelectionDevice] PRIMARY KEY CLUSTERED 
(
	[selectionDevice] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


-- ALTER Sample table with new columns (new)
ALTER TABLE [dbo].[Sample]
ADD [labJournalNum] [nvarchar] (6) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [gearType] [nvarchar](3) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[selectionDevice] [nvarchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[meshSize] [numeric](5,1) NULL,
    [numberTrawls] [int] NULL,
	[heightNets] [numeric](5,1) NULL,
	[lengthNets] [numeric](5,1) NULL,
	[lengthRopeFlyer] [numeric](5,1) NULL,
	[widthRopeFlyer] [numeric](5,1) NULL,
	[numberHooks] [int] NULL,
	[gearRemark] [nvarchar](450) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[lengthBeam] [numeric](5,1) NULL
GO




ALTER TABLE [dbo].[Sample]  WITH NOCHECK ADD  CONSTRAINT [FK_Sample_L_GearType_2] FOREIGN KEY([gearType])
REFERENCES [dbo].[L_GearType] ([gearType])
GO
ALTER TABLE [dbo].[Sample] CHECK CONSTRAINT [FK_Sample_L_GearType_2]
GO

ALTER TABLE [dbo].[Sample]  WITH NOCHECK ADD  CONSTRAINT [FK_Sample_L_SelectionDevice] FOREIGN KEY([selectionDevice])
REFERENCES [dbo].[L_SelectionDevice] ([selectionDevice])
GO
ALTER TABLE [dbo].[Sample] CHECK CONSTRAINT [FK_Sample_L_SelectionDevice]
GO


-- ALTER ICES_DFU_Relation_FF (add primary key column)
ALTER TABLE [dbo].[ICES_DFU_Relation_FF]
ADD ICES_DFU_Relation_FFId INT IDENTITY

ALTER TABLE [dbo].[ICES_DFU_Relation_FF]
ADD CONSTRAINT PK_ICES_DFU_Relation_FF
PRIMARY KEY(ICES_DFU_Relation_FFId)



-- ADD new R_GearTypeSelectionDevice table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[R_GearTypeSelectionDevice](
	[R_GearTypeSelectionDeviceId] [int] IDENTITY(1,1) NOT NULL,
	[gearType] [nvarchar](3) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[selectionDevice] [nvarchar](30) NOT NULL,
 CONSTRAINT [PK_R_GearTypeSelectionDevice] PRIMARY KEY CLUSTERED 
(
	[R_GearTypeSelectionDeviceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_R_GearTypeSelectionDevice] UNIQUE NONCLUSTERED 
(
	[gearType] ASC,
	[selectionDevice] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[R_GearTypeSelectionDevice]  WITH NOCHECK ADD  CONSTRAINT [FK_R_GearType_L_GearType] FOREIGN KEY([gearType])
REFERENCES [dbo].[L_GearType] ([gearType])
GO
ALTER TABLE [dbo].[R_GearTypeSelectionDevice] CHECK CONSTRAINT [FK_R_GearType_L_GearType]
GO

ALTER TABLE [dbo].[R_GearTypeSelectionDevice]  WITH NOCHECK ADD  CONSTRAINT [FK_R_SampleGear_L_SelectionDevice] FOREIGN KEY([selectionDevice])
REFERENCES [dbo].[L_SelectionDevice] ([selectionDevice])
GO
ALTER TABLE [dbo].[R_GearTypeSelectionDevice] CHECK CONSTRAINT [FK_R_SampleGear_L_SelectionDevice]
GO


-- FIX timezones on Trip and Sample
UPDATE t
SET t.dateStart = DATEADD(hour, case when ISNULL(t.timeZone, 0) < 0 then ISNULL(t.timeZone, 0) else 0 end, t.dateStart), 
    t.dateEnd = DATEADD(hour, case when ISNULL(t.timeZone, 0) < 0 then ISNULL(t.timeZone, 0) else 0 end, t.dateEnd), 
    t.timeZone = case when t.timeZone < 0 then -t.timeZone else t.timeZone end
FROM [Trip] t


UPDATE t
SET t.dateGearStart = DATEADD(hour, case when ISNULL(t.timeZone, 0) < 0 then ISNULL(t.timeZone, 0) else 0 end, t.dateGearStart), 
    t.dateGearEnd = DATEADD(hour, case when ISNULL(t.timeZone, 0) < 0 then ISNULL(t.timeZone, 0) else 0 end, t.dateGearEnd), 
    t.timeZone = case when t.timeZone < 0 then -t.timeZone else t.timeZone end
FROM [Sample] t



-- ADD new L_FisheryType table 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[L_FisheryType](
	[L_fisheryTypeId] [int] IDENTITY(1,1) NOT NULL,
	[fisheryType] [nvarchar](6) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[landingCategory] [nvarchar](3) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[description] [nvarchar](80) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_L_FisheryType] PRIMARY KEY CLUSTERED 
(
	[fisheryType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[L_FisheryType]  WITH NOCHECK ADD  CONSTRAINT [FK_L_FisheryType_L_LandingCategory] FOREIGN KEY([landingCategory])
REFERENCES [dbo].[L_LandingCategory] ([landingCategory])
GO
ALTER TABLE [dbo].[L_FisheryType] CHECK CONSTRAINT [FK_L_FisheryType_L_LandingCategory]
GO


-- Add field and relation to L_FisheryType from Trip 
ALTER TABLE [dbo].[Trip]
ADD [fisheryType] [nvarchar](6) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO


ALTER TABLE [dbo].[Trip]  WITH NOCHECK ADD  CONSTRAINT [FK_Trip_L_FisheryType] FOREIGN KEY([fisheryType])
REFERENCES [dbo].L_FisheryType ([fisheryType])
GO
ALTER TABLE [dbo].[Trip] CHECK CONSTRAINT [FK_Trip_L_FisheryType]
GO


-- Update dateEnd with values from dateStart for HVN Trip records 
UPDATE t
SET t.dateEnd = t.dateStart
FROM [Trip] t
WHERE t.tripType = 'HVN'

-- Update gearType with HVN_gearType (since HVN_gearType will not be used anymore)
UPDATE s
SET s.gearType = s.HVN_gearType
FROM [Sample] s
WHERE s.HVN_gearType IS NOT NULL



-- Add L_HaulType lookup
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[L_HaulType](
	[L_haulTypeId] [int] IDENTITY(1,1) NOT NULL,
	[haulType] [nvarchar](1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[num] [int] NULL,
	[description] [nvarchar](80) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_L_HaulType] PRIMARY KEY CLUSTERED 
(
	[haulType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[L_HaulType] (haulType, num, description)
VALUES ('A', 1, 'Normalt træk')
INSERT INTO [dbo].[L_HaulType] (haulType, num, description)
VALUES ('K', 2, 'Kalibreringstræk')
INSERT INTO [dbo].[L_HaulType] (haulType, num, description)
VALUES ('F', 3, 'Fiskertræk')


-- Add L_ThermoCline lookup
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[L_ThermoCline](
	[L_thermoClineId] [int] IDENTITY(1,1) NOT NULL,
	[thermoCline] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[num] [int] NULL,
	[description] [nvarchar](80) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_L_ThermoCline] PRIMARY KEY CLUSTERED 
(
	[thermoCline] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[L_ThermoCline] (thermoCline, num, description)
VALUES ('Ja', 1, NULL)
INSERT INTO [dbo].[L_ThermoCline] (thermoCline, num, description)
VALUES ('Nej', 2, NULL)


-- ALTER Sample table with new columns (new)
ALTER TABLE [dbo].[Sample]
ADD [haulType] [nvarchar](1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [thermoCline] [nvarchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO


ALTER TABLE [dbo].[Sample]  WITH NOCHECK ADD  CONSTRAINT [FK_Sample_L_HaulType] FOREIGN KEY([haulType])
REFERENCES [dbo].[L_HaulType] ([haulType])
GO
ALTER TABLE [dbo].[Sample] CHECK CONSTRAINT [FK_Sample_L_HaulType]
GO

ALTER TABLE [dbo].[Sample]  WITH NOCHECK ADD  CONSTRAINT [FK_Sample_L_ThermoCline] FOREIGN KEY([thermoCline])
REFERENCES [dbo].[L_ThermoCline] ([thermoCline])
GO
ALTER TABLE [dbo].[Sample] CHECK CONSTRAINT [FK_Sample_L_ThermoCline]
GO


-- ALTER Sample table with new columns (new)
ALTER TABLE [dbo].[Sample]
ADD [temperatureSrf] [numeric](6,2) NULL,
    [temperatureBot] [numeric](6,2) NULL,
	[oxygenSrf] [numeric](6,2) NULL,
	[oxygenBot] [numeric](6,2) NULL,
	[thermoClineDepth] [numeric](6,2) NULL,
	[salinitySrf] [numeric](6,2) NULL,
	[salinityBot] [numeric](6,2) NULL

GO


-- Change L_GearType.gearType from nvarchar(3) to nvarchar(50) (and associated relationship columns)
-- Remove all constraints for gearType on L_GearType
BEGIN
	ALTER TABLE R_GearTypeSelectionDevice DROP CONSTRAINT [FK_R_GearType_L_GearType]
	ALTER TABLE R_GearTypeSelectionDevice DROP CONSTRAINT [IX_R_GearTypeSelectionDevice]
	ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_Sample_L_GearType]
	ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_Sample_L_GearType_2]
	ALTER TABLE [dbo].[L_Gear] DROP CONSTRAINT [FK_L_Gear_L_GearType]
	ALTER TABLE L_GearType DROP CONSTRAINT [PK_L_GearType]
END

-- Alter gearType sizes on all tables
BEGIN
	ALTER TABLE L_GearType 
	ALTER COLUMN gearType nvarchar(50) NOT NULL

	ALTER TABLE [dbo].[R_GearTypeSelectionDevice]
	ALTER COLUMN gearType nvarchar(50)  NOT NULL

	ALTER TABLE [dbo].[Sample]
	ALTER COLUMN HVN_geartype nvarchar(50)

	ALTER TABLE [dbo].[Sample]
	ALTER COLUMN gearType nvarchar(50)

	ALTER TABLE [dbo].[L_Gear]
	ALTER COLUMN gearType nvarchar(50) NOT NULL
END

-- Add contraints for gearType on all tables again
BEGIN
	ALTER TABLE L_GearType
	ADD CONSTRAINT [PK_L_GearType] PRIMARY KEY CLUSTERED 
	(
		[gearType] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

	ALTER TABLE [dbo].[R_GearTypeSelectionDevice]  WITH NOCHECK ADD  CONSTRAINT [FK_R_GearType_L_GearType] FOREIGN KEY([gearType])
	REFERENCES [dbo].[L_GearType] ([gearType])

	ALTER TABLE  [dbo].[R_GearTypeSelectionDevice]
	ADD CONSTRAINT [IX_R_GearTypeSelectionDevice] UNIQUE NONCLUSTERED 
	(
		[gearType] ASC,
		[selectionDevice] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

	ALTER TABLE [dbo].[Sample]  WITH NOCHECK ADD  CONSTRAINT [FK_Sample_L_GearType] FOREIGN KEY([HVN_geartype])
	REFERENCES [dbo].[L_GearType] ([gearType])

	ALTER TABLE [dbo].[Sample]  WITH NOCHECK ADD  CONSTRAINT [FK_Sample_L_GearType_2] FOREIGN KEY([gearType])
	REFERENCES [dbo].[L_GearType] ([gearType])

	ALTER TABLE [dbo].[L_Gear]  WITH NOCHECK ADD  CONSTRAINT [FK_L_Gear_L_GearType] FOREIGN KEY([gearType])
	REFERENCES [dbo].[L_GearType] ([gearType])
END



-- Change L_Platform.platform from nvarchar(6) to nvarchar(20) (and associated relationship columns)
-- Remove all constraints for platform on L_Platform
BEGIN
	ALTER TABLE [dbo].[L_PlatformVersion] DROP CONSTRAINT [FK_L_PlatformVersion_L_Platform]
	ALTER TABLE [dbo].[Trip] DROP CONSTRAINT [FK_Trip_L_Platform1]
	ALTER TABLE [dbo].[Trip] DROP CONSTRAINT [FK_Trip_L_Platform2]
	ALTER TABLE [dbo].[L_Gear] DROP CONSTRAINT [FK_L_Gear_L_Platform1]
	ALTER TABLE [dbo].[L_Gear] DROP CONSTRAINT [IX_L_Gear]
	ALTER TABLE [dbo].[L_Platform] DROP CONSTRAINT [PK_L_Platform]
END

-- Alter gearType sizes on all tables
BEGIN
	ALTER TABLE [dbo].[L_Platform]
	ALTER COLUMN [platform] nvarchar(20) NOT NULL

	ALTER TABLE [dbo].[L_Gear]
	ALTER COLUMN [platform] nvarchar(20) NOT NULL

	ALTER TABLE [dbo].[Trip]
	ALTER COLUMN [platform1] nvarchar(20) 

	ALTER TABLE [dbo].[Trip]
	ALTER COLUMN [platform2] nvarchar(20) 

	ALTER TABLE [dbo].[L_PlatformVersion]
	ALTER COLUMN [platform] nvarchar(20) NOT NULL
END

-- Add contraints for gearType on all tables again
BEGIN
	ALTER TABLE [dbo].[L_Platform]
	ADD CONSTRAINT [PK_L_Platform] PRIMARY KEY CLUSTERED 
	(
		[platform] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]


	ALTER TABLE [dbo].[L_Gear]  WITH NOCHECK ADD  CONSTRAINT [FK_L_Gear_L_Platform1] FOREIGN KEY([platform])
	REFERENCES [dbo].[L_Platform] ([platform])
	ON UPDATE CASCADE
	ON DELETE CASCADE

	ALTER TABLE  [dbo].[L_Gear]
	ADD CONSTRAINT [IX_L_Gear] UNIQUE NONCLUSTERED 
	(
		[platform] ASC,
		[gear] ASC,
		[version] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

	ALTER TABLE [dbo].[Trip]  WITH CHECK ADD  CONSTRAINT [FK_Trip_L_Platform1] FOREIGN KEY([platform1])
	REFERENCES [dbo].[L_Platform] ([platform])

	ALTER TABLE [dbo].[Trip]  WITH CHECK ADD  CONSTRAINT [FK_Trip_L_Platform2] FOREIGN KEY([platform2])
	REFERENCES [dbo].[L_Platform] ([platform])

	ALTER TABLE [dbo].[L_PlatformVersion]  WITH NOCHECK ADD  CONSTRAINT [FK_L_PlatformVersion_L_Platform] FOREIGN KEY([platform])
	REFERENCES [dbo].[L_Platform] ([platform])
	ON UPDATE CASCADE
	ON DELETE CASCADE
END


-- Add L_TimeZone lookup
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[L_TimeZone](
	[L_timeZoneId] [int] IDENTITY(1,1) NOT NULL,
	[timeZone] [int] NOT NULL,
	[description] [nvarchar](150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_L_TimeZone] PRIMARY KEY CLUSTERED 
(
	[timeZone] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[L_TimeZone] ([timeZone], description)
VALUES (-3, 'Grønland (Nuuk), Brasilien')
INSERT INTO [dbo].[L_TimeZone] ([timeZone], description)
VALUES (-2, 'Grytviken, Sør-Georgia og Sør-Sandwichøyene')
INSERT INTO [dbo].[L_TimeZone] ([timeZone], description)
VALUES (-1, 'Grønland (Ittoqqortoormiit), Azorerne, Kap Verde')
INSERT INTO [dbo].[L_TimeZone] ([timeZone], description)
VALUES (0, 'Engelsk vintertid, Island, Færøerne, Portugal')
INSERT INTO [dbo].[L_TimeZone] ([timeZone], description)
VALUES (1, 'Dansk vintertid, Engelsk sommertid')
INSERT INTO [dbo].[L_TimeZone] ([timeZone], description)
VALUES (2, 'Dansk sommertid')
INSERT INTO [dbo].[L_TimeZone] ([timeZone], description)
VALUES (3, 'Baghdad, Kuwait, Riyailh, Moscow, St. Petersburg, Volgolgrad, Nairobi')
INSERT INTO [dbo].[L_TimeZone] ([timeZone], description)
VALUES (4, 'Baku, Tbilisi, Yerevan, Muscat')
INSERT INTO [dbo].[L_TimeZone] ([timeZone], description)
VALUES (5, 'Islamabad, Karachi, Tashkent, Ekaterinburg')
INSERT INTO [dbo].[L_TimeZone] ([timeZone], description)
VALUES (6, 'Kazakhstan, Bangladesh, Bhutan, Russia')
INSERT INTO [dbo].[L_TimeZone] ([timeZone], description)
VALUES (10, 'Brisbane, Canberra, Melbourne, Sydney, Guam, Port Marsbey, Hobart, Vladivostok')
INSERT INTO [dbo].[L_TimeZone] ([timeZone], description)
VALUES (20, '20 Dummy')
INSERT INTO [dbo].[L_TimeZone] ([timeZone], description)
VALUES (30, '30 Dummy')
INSERT INTO [dbo].[L_TimeZone] ([timeZone], description)
VALUES (55, '55 Dummy')


ALTER TABLE [dbo].[Trip]  WITH CHECK ADD  CONSTRAINT [FK_Trip_L_TimeZone] FOREIGN KEY([timeZone])
REFERENCES [dbo].[L_TimeZone] ([timeZone])
GO
ALTER TABLE [dbo].[Trip] CHECK CONSTRAINT [FK_Trip_L_TimeZone]
GO

ALTER TABLE [dbo].[Sample]  WITH CHECK ADD  CONSTRAINT [FK_Sample_L_TimeZone] FOREIGN KEY([timeZone])
REFERENCES [dbo].[L_TimeZone] ([timeZone])
GO
ALTER TABLE [dbo].[Sample] CHECK CONSTRAINT [FK_Sample_L_TimeZone]
GO



-- Add DateTime to TreatmentFactor for versioning 
ALTER TABLE [dbo].[TreatmentFactor]
ADD [versioningDate] [datetime] NULL
GO

-- Remove old compound key constraint
ALTER TABLE [dbo].[TreatmentFactor] DROP CONSTRAINT [IX_TreatmentFactor]

UPDATE t
SET t.[versioningDate] = '01/01/1900 00:00:00'
FROM [dbo].[TreatmentFactor] t

ALTER TABLE [dbo].[TreatmentFactor]
ALTER COLUMN [versioningDate] [datetime] NOT NULL
GO

-- Add new compound key constraint that now includes versioningDate
ALTER TABLE  [dbo].[TreatmentFactor]
ADD CONSTRAINT [IX_TreatmentFactor] UNIQUE NONCLUSTERED 
(
	[treatmentFactorGroup] ASC,
	[treatment] ASC,
	[versioningDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]



-- Add ShowInVid and ShowInSeaHvn columns to L_GearType  
ALTER TABLE [dbo].[L_GearType]
ADD [showInVidUI] [bit] NULL,
    [showInSeaHvnUI] [bit] NULL
GO

UPDATE gt
SET gt.showInVidUI = 1, 
    gt.showInSeaHvnUI = 1
FROM [dbo].[L_GearType] gt

ALTER TABLE [dbo].[L_GearType]
ALTER COLUMN [showInVidUI] [bit] NOT NULL

ALTER TABLE [dbo].[L_GearType]
ALTER COLUMN [showInSeaHvnUI] [bit] NOT NULL
GO


-- Add new L_WeightEstimationMethod 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[L_WeightEstimationMethod](
	[L_weightEstimationMethodId] [int] IDENTITY(1,1) NOT NULL,
	[weightEstimationMethod] [nvarchar](1) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[num] [int] NULL,
	[description] [nvarchar](80) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_L_WeightEstimationMethod] PRIMARY KEY CLUSTERED 
(
	[weightEstimationMethod] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[L_WeightEstimationMethod] (weightEstimationMethod, num, description)
VALUES ('V', 1, 'Vejet')
INSERT INTO [dbo].[L_WeightEstimationMethod] (weightEstimationMethod, num, description)
VALUES ('F', 2, 'Fiskerestimat')
INSERT INTO [dbo].[L_WeightEstimationMethod] (weightEstimationMethod, num, description)
VALUES ('O', 3, 'Observatorestimat')



-- Add Changes to Sample (Add TotalWeight and WeightEstimationMethod)  
ALTER TABLE [dbo].[Sample]
ADD [totalWeight] [numeric](11, 3) NULL,
    [weightEstimationMethod] [nvarchar](1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
GO


ALTER TABLE [dbo].[Sample]  WITH NOCHECK ADD  CONSTRAINT [FK_Sample_L_WeightEstimationMethod] FOREIGN KEY([weightEstimationMethod])
REFERENCES [dbo].[L_WeightEstimationMethod] ([weightEstimationMethod])
GO
ALTER TABLE [dbo].[Sample] CHECK CONSTRAINT [FK_Sample_L_WeightEstimationMethod]
GO


-- Add Changes to SpeciesList (Add number and WeightEstimationMethod) 
ALTER TABLE [dbo].[SpeciesList]
ADD [number] [int] NULL,
    [weightEstimationMethod] [nvarchar](1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
GO


ALTER TABLE [dbo].[SpeciesList]  WITH NOCHECK ADD  CONSTRAINT [FK_SpeciesList_L_WeightEstimationMethod] FOREIGN KEY([weightEstimationMethod])
REFERENCES [dbo].[L_WeightEstimationMethod] ([weightEstimationMethod])
GO
ALTER TABLE [dbo].[SpeciesList] CHECK CONSTRAINT [FK_SpeciesList_L_WeightEstimationMethod]
GO


-- Add L_FisheryType Codes
INSERT INTO [dbo].[L_FisheryType] ([fisheryType], [landingCategory], [description])
VALUES ('1', 'KON', 'Konsum')
INSERT INTO [dbo].[L_FisheryType] ([fisheryType], [landingCategory], [description])
VALUES ('2', 'IND', 'Industri')




-- Add L_EdgeStructure Lookup   

CREATE TABLE [dbo].[L_EdgeStructure](
	[L_edgeStructureId] [int] IDENTITY(1,1) NOT NULL,
	[edgeStructure] [nvarchar](5) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[num] [int] NULL,
	[description] [nvarchar](250) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_L_EdgeStructure] PRIMARY KEY CLUSTERED 
(
	[edgeStructure] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[L_EdgeStructure] ([edgeStructure], [num], [description])
VALUES ('WR', 0, 'Hel vinterring på kanten, dvs. sidste årring er helt ud til kanten og væksten er gennemsigtig')
INSERT INTO [dbo].[L_EdgeStructure] ([edgeStructure], [num], [description])
VALUES ('SR', 1, 'Ingen vinterring på kanten, dvs. opag vækst på kanten')
INSERT INTO [dbo].[L_EdgeStructure] ([edgeStructure], [num], [description])
VALUES ('HR', 2, 'Halv vinterring på kanten, dvs. væksten er gennemsigtig, men ikke bred nok til at vinterringen er færdigdannet')


-- Include edgeStructure in Age table    
ALTER TABLE [dbo].[Age]
ADD [edgeStructure] [nvarchar](5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO


ALTER TABLE [dbo].[Age]  WITH CHECK ADD  CONSTRAINT [FK_Age_L_EdgeStructure] FOREIGN KEY([edgeStructure])
REFERENCES [dbo].[L_EdgeStructure] ([edgeStructure])
GO
ALTER TABLE [dbo].[Age] CHECK CONSTRAINT [FK_Age_L_EdgeStructure]
GO




-- Add L_Parasite Lookup   
CREATE TABLE [dbo].[L_Parasite](
	[L_parasiteId] [int] IDENTITY(1,1) NOT NULL,
	[num] [int] NOT NULL,
	[description] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_L_Parasite] PRIMARY KEY CLUSTERED 
(
	[L_parasiteId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[L_Parasite] ([num], [description])
VALUES (0, 'Ingen orme')
INSERT INTO [dbo].[L_Parasite] ([num], [description])
VALUES (1, '1-5 orme')
INSERT INTO [dbo].[L_Parasite] ([num], [description])
VALUES (2, '>5 orme')


-- Include edgeStructure in Age table   
ALTER TABLE [dbo].[AnimalInfo]
ADD [parasiteId] [int] NULL
GO


ALTER TABLE [dbo].[AnimalInfo]  WITH CHECK ADD  CONSTRAINT [FK_AnimalInfo_L_Parasite] FOREIGN KEY([parasiteId])
REFERENCES [dbo].[L_Parasite] ([L_parasiteId])
GO
ALTER TABLE [dbo].[AnimalInfo] CHECK CONSTRAINT [FK_AnimalInfo_L_Parasite]
GO




-- Add L_Reference Lookup   
CREATE TABLE [dbo].[L_Reference](
	[L_referenceId] [int] IDENTITY(1,1) NOT NULL,
	[reference] [nvarchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[description] [nvarchar](150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_L_Reference] PRIMARY KEY CLUSTERED 
(
	[L_referenceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[L_Reference] ([reference], [description])
VALUES ('Mave', 'Mave beskrivelse')
INSERT INTO [dbo].[L_Reference] ([reference], [description])
VALUES ('Genetik', 'Genetik beskrivelse')
INSERT INTO [dbo].[L_Reference] ([reference], [description])
VALUES ('Billede', 'Billede beskrivelse')




-- Add L_Reference and AnimalInfo relationship table 
CREATE TABLE [dbo].[R_AnimalInfoReference](
	[R_animalInfoReferenceId] [int] IDENTITY(1,1) NOT NULL,
	[animalInfoId] [int] NOT NULL,
	[L_referenceId] [int] NOT NULL,
 CONSTRAINT [PK_R_AnimalInfoReference] PRIMARY KEY CLUSTERED 
(
	[R_animalInfoReferenceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[R_AnimalInfoReference]  WITH NOCHECK ADD  CONSTRAINT [FK_R_AnimalInfoReference_AnimalInfo] FOREIGN KEY([animalInfoId])
REFERENCES [dbo].[AnimalInfo] ([animalInfoId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[R_AnimalInfoReference] CHECK CONSTRAINT [FK_R_AnimalInfoReference_AnimalInfo]
GO

ALTER TABLE [dbo].[R_AnimalInfoReference]  WITH NOCHECK ADD  CONSTRAINT [FK_R_AnimalInfoReference_L_Reference] FOREIGN KEY([L_referenceId])
REFERENCES [dbo].[L_Reference] ([L_referenceId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[R_AnimalInfoReference] CHECK CONSTRAINT [FK_R_AnimalInfoReference_L_Reference]



-- Add Person addtions to SpeciesList (Add number and WeightEstimationMethod) 
ALTER TABLE [dbo].[SpeciesList]
ADD [hatchMonthReaderId] [int] NULL,
    [maturityReaderId] [int] NULL
GO

ALTER TABLE [dbo].[SpeciesList]  WITH NOCHECK ADD  CONSTRAINT [FK_SpeciesList_DFUPerson_MontReader] FOREIGN KEY([hatchMonthReaderId])
REFERENCES [dbo].[DFUPerson] ([dfuPersonId])
GO
ALTER TABLE [dbo].[SpeciesList] CHECK CONSTRAINT [FK_SpeciesList_DFUPerson_MontReader]
GO

ALTER TABLE [dbo].[SpeciesList]  WITH NOCHECK ADD  CONSTRAINT [FK_SpeciesList_DFUPerson_MaturityReader] FOREIGN KEY([maturityReaderId])
REFERENCES [dbo].[DFUPerson] ([dfuPersonId])
GO
ALTER TABLE [dbo].[SpeciesList] CHECK CONSTRAINT [FK_SpeciesList_DFUPerson_MaturityReader]
GO

--- Alter boatIdentity of L_Platform from 6 characters to 20.
ALTER TABLE [dbo].[L_Platform]
ALTER COLUMN [boatIdentity] nvarchar(20) NULL
GO

-- ALTER subsampleweight and landingweight precision from 11,3 to 11,4 on SubSample
ALTER TABLE [dbo].[SubSample]
ALTER COLUMN [subSampleWeight] [numeric](11, 4) NULL
GO

ALTER TABLE [dbo].[SubSample]
ALTER COLUMN [landingWeight] [numeric](11, 4) NULL
GO


-- ALTER totalWeight precision from 11,3 to 11,4 in Sample
ALTER TABLE [dbo].[Sample]
ALTER COLUMN [totalWeight] [numeric](11, 4) NULL
GO


-- Add Number column to many tables.  
ALTER TABLE [dbo].[L_SpeciesRegistration]
ADD [num] [int] NULL
GO

ALTER TABLE [dbo].[L_Treatment]
ADD [num] [int] NULL
GO

ALTER TABLE [dbo].[L_CatchRegistration]
ADD [num] [int] NULL
GO

ALTER TABLE [dbo].[L_SamplingMethod]
ADD [num] [int] NULL
GO

ALTER TABLE [dbo].[L_SexCode]
ADD [num] [int] NULL
GO

ALTER TABLE [dbo].[L_LandingCategory]
ADD [num] [int] NULL
GO

ALTER TABLE [dbo].[L_SizeSortingDFU]
ADD [num] [int] NULL
GO

ALTER TABLE [dbo].[L_OtolithReadingRemark]
ADD [num] [int] NULL
GO

ALTER TABLE [dbo].[L_SamplingType]
ADD [num] [int] NULL
GO

ALTER TABLE [dbo].[L_GearQuality]
ADD [num] [int] NULL
GO

ALTER TABLE [dbo].[L_BroodingPhase]
ADD [num] [int] NULL
GO


ALTER TABLE [dbo].[L_DFUArea]
ADD [areaNES] nvarchar(10)  NULL
GO


ALTER TABLE [dbo].[L_Species]
ADD [speciesNES] nvarchar(3)  NULL,
    [speciesFAO] nvarchar(3)  NULL
GO



-- Add sumAnimalWeights column to show when user enter an 'x' in SubSample subSampleWeight
ALTER TABLE [dbo].[SubSample]
ADD [sumAnimalWeights] bit NULL
GO

-- set sumAnimalWeights to 1 for all SubSamples where subSampleWeight is null
UPDATE ss
SET ss.sumAnimalWeights = 1
FROM [SubSample] ss
WHERE ss.subSampleWeight IS NULL
GO

UPDATE s
SET s.fishingTime = s.fishingTime * 60
FROM [Sample] s
WHERE s.fishingTime IS NOT NULL
GO


--Alter shovelDist on Sample
ALTER TABLE [dbo].[Sample]
ALTER COLUMN shovelDist Numeric(5, 1) NULL

--DW
--ALTER TABLE [dbo].[Sample]
--ALTER COLUMN otterBoardDist Numeric(5, 1) NULL



--!!!!!!!!!!! Drop contraints concerning L_TreatmentFactorGroup key so key-length can be changed
BEGIN
	ALTER TABLE L_Species DROP CONSTRAINT [FK_L_Species_L_CleaningGroup]
	ALTER TABLE TreatmentFactor DROP CONSTRAINT [FK_TreatmentFactor_L_treatmentFactorGroup]
	ALTER TABLE TreatmentFactor DROP CONSTRAINT [IX_TreatmentFactor]
	ALTER TABLE L_TreatmentFactorGroup DROP CONSTRAINT [PK_L_CleaningGroup]
END

-- Change length of treatmentFactorGroup on all tables using it
BEGIN
	ALTER TABLE [dbo].[L_TreatmentFactorGroup] 
	ALTER COLUMN [treatmentFactorGroup] [nvarchar](3) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

	ALTER TABLE [dbo].[TreatmentFactor]
	ALTER COLUMN [treatmentFactorGroup] [nvarchar](3) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL

	ALTER TABLE [dbo].[L_Species]
	ALTER COLUMN [treatmentFactorGroup] [nvarchar](3) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
END

-- Add contraints again
BEGIN
    --L_TreatmentFactorGroup table
	ALTER TABLE [dbo].[L_TreatmentFactorGroup]
	ADD  CONSTRAINT [PK_L_CleaningGroup] PRIMARY KEY CLUSTERED 
	(
		[treatmentFactorGroup] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

    -- TreatmentFactor table
	ALTER TABLE [dbo].[TreatmentFactor]  WITH NOCHECK ADD  CONSTRAINT [FK_TreatmentFactor_L_treatmentFactorGroup] FOREIGN KEY([treatmentFactorGroup])
	REFERENCES [dbo].[L_TreatmentFactorGroup] ([treatmentFactorGroup])
	ON UPDATE CASCADE
	ON DELETE CASCADE

	ALTER TABLE [dbo].[TreatmentFactor] CHECK CONSTRAINT [FK_TreatmentFactor_L_treatmentFactorGroup]

	ALTER TABLE  [dbo].[TreatmentFactor]
	ADD  CONSTRAINT [IX_TreatmentFactor] UNIQUE NONCLUSTERED 
	(
		[treatmentFactorGroup] ASC,
		[treatment] ASC,
		[versioningDate] ASC
	) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

    -- L_Species table
	ALTER TABLE [dbo].[L_Species]  WITH NOCHECK ADD  CONSTRAINT [FK_L_Species_L_CleaningGroup] FOREIGN KEY([treatmentFactorGroup])
	REFERENCES [dbo].[L_TreatmentFactorGroup] ([treatmentFactorGroup])
	ON UPDATE CASCADE
	ON DELETE CASCADE

	ALTER TABLE [dbo].[L_Species] CHECK CONSTRAINT [FK_L_Species_L_CleaningGroup]
END


ALTER TABLE [dbo].[Sample]
ADD [wingSpread] [numeric](3,1) NULL
GO

-- BELOW has not been transferred to video production server.
