USE [sprattus]
GO


-- Below code for creating a test table
/*
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[L_HarbourTest](
	[L_harbourTestId] [int] IDENTITY(1,1) NOT NULL,
	[harbour] [nvarchar](4) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[description] [nvarchar](80) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[nationality] [nvarchar](3) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_L_HarbourTest_1] PRIMARY KEY CLUSTERED 
(
	[harbour] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[L_HarbourTest]  WITH CHECK ADD  CONSTRAINT [FK_L_HarbourTest_L_Nationality] FOREIGN KEY([nationality])
REFERENCES [dbo].[L_Nationality] ([nationality])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[L_HarbourTest] CHECK CONSTRAINT [FK_L_HarbourTest_L_Nationality]

-- Insert test data into L_HarbourTest
INSERT INTO L_HarbourTest
VALUES ('ÅBEN', 'Åbenrå', 'DNK')



-- Code for adding 2 new columns to L_HarbourTest
ALTER TABLE [dbo].[L_HarbourTest]
ADD [harbourNES] [nvarchar] (5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [harbourEU] [nvarchar] (5) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
	

ALTER TABLE [dbo].[L_HarbourTest]
ADD [platform1] [nvarchar] (6) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [platform2] [nvarchar] (6) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO

ALTER TABLE [dbo].[L_HarbourTest] WITH CHECK ADD CONSTRAINT [FK_L_HarbourTest_L_Platform1] FOREIGN KEY([platform1])
REFERENCES [dbo].[L_Platform] ([platform])
GO
ALTER TABLE [dbo].[L_HarbourTest] CHECK CONSTRAINT [FK_L_HarbourTest_L_Platform1]
GO

ALTER TABLE [dbo].[L_HarbourTest] WITH CHECK ADD CONSTRAINT [FK_L_HarbourTest_L_Platform2] FOREIGN KEY([platform2])
REFERENCES [dbo].[L_Platform] ([platform])
GO
ALTER TABLE [dbo].[L_HarbourTest] CHECK CONSTRAINT [FK_L_HarbourTest_L_Platform2]
*/

-- Below code for deleting test table
--DROP TABLE [dbo].[L_HarbourTest]



/*
CREATE TABLE [dbo].[L_HarbourTest](
	[L_harbourTestId] [int] IDENTITY(1,1) NOT NULL,
	[harbour] [nvarchar](5) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [timeZone] int NULL,
    [gearOut] datetime NULL,
    [gearIn] datetime NULL
 CONSTRAINT [PK_L_HarbourTest_1] PRIMARY KEY CLUSTERED 
(
	[harbour] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


INSERT INTO L_HarbourTest
VALUES ('ÅBEN', 0, '08/30/2006 16:34:34', '08/30/2006 22:34:34')
INSERT INTO L_HarbourTest
VALUES ('ÅBEN2', -1, '08/30/2006 16:34:34', '08/30/2006 22:34:34')
INSERT INTO L_HarbourTest
VALUES ('ÅBEN3', -2, '08/30/2006 16:34:34', '08/30/2006 22:34:34')
INSERT INTO L_HarbourTest
VALUES ('ÅBEN4', NULL, '08/30/2006 16:34:34', '08/30/2006 22:34:34')
INSERT INTO L_HarbourTest
VALUES ('ÅBEN5', 2, '08/30/2006 16:34:34', '08/30/2006 22:34:34')
GO

SELECT *
FROM L_HarbourTest
GO

UPDATE ht
SET ht.gearOut = DATEADD(hour, case when ISNULL(ht.timeZone, 0) < 0 then ISNULL(ht.timeZone, 0) else 0 end, ht.gearOut), 
    ht.gearIn = DATEADD(hour, case when ISNULL(ht.timeZone, 0) < 0 then ISNULL(ht.timeZone, 0) else 0 end, ht.gearIn), 
    ht.timeZone = case when ht.timeZone < 0 then -ht.timeZone else ht.timeZone end
FROM [L_HarbourTest] ht

SELECT *
FROM L_HarbourTest

-- Below code for deleting test table
DROP TABLE [dbo].[L_HarbourTest]


*/


DECLARE @tripId int;
DECLARE @platformId int;

DECLARE Trip_Cursor CURSOR FOR
SELECT tripId
FROM Trip

OPEN Trip_Cursor

FETCH NEXT FROM Trip_Cursor
INTO @tripId

WHILE @@FETCH_STATUS = 0
BEGIN 
	
	-- Select platform id out to put in trip.platform1 (as main vessel)
	SELECT @platformId = pp.L_platformId
	FROM 
	(
		SELECT TOP 1 p.L_platformId
		FROM L_Platform p
		INNER JOIN L_PlatformVersion pv on pv.platform = p.platform
		INNER JOIN R_TripPlatformVersion t ON t.platformVersionId = pv.platformVersionId
		WHERE t.tripId = 5877
		ORDER BY pv.platformVersionId DESC
	) as pp

	--SELECT *
    --FROM Trip
	--WHERE Trip.tripId = @tripId
	
	
	FETCH NEXT FROM Trip_Cursor
	INTO @tripId
END

CLOSE Trip_Cursor
DEALLOCATE Trip_Cursor



