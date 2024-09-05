
-- Make sure all single fish animal records have number = 1.
--SELECT *
UPDATE a
SET a.number = 1
FROM Animal a
WHERE a.individNum IS NOT NULL 
  AND a.individNum > 0
  AND a.number = 0



-- Copy gearTypes from L_Gear to Sample.gearType based on gearId
UPDATE s
SET s.gearType = g.gearType
FROM Sample s
INNER JOIN L_Gear g on g.gearId = s.gearId
WHERE s.gearType IS NULL



-- Copy gearText concatenated with description from L_Gear to Sample.gearRemark based on gearId
UPDATE s
SET s.gearRemark = CASE WHEN g.gearText IS NULL AND g.description IS NOT NULL THEN g.description 
                        WHEN g.gearText IS NOT NULL AND g.description IS NULL THEN g.gearText
                        WHEN g.gearText IS NOT NULL AND g.description IS NOT NULL THEN g.gearText + ', ' + g.description
                   END
FROM Sample s
INNER JOIN L_Gear g on g.gearId = s.gearId
WHERE s.gearRemark IS NULL



-- Transfer all maskWidth values to s.meshSize
UPDATE s
SET s.meshSize = CAST(s.maskWidth as NUMERIC(5,1))
FROM Sample s
INNER JOIN Trip t on t.tripId = s.tripId
WHERE s.maskWidth IS NOT NULL AND s.maskWidth < 9999 
-- make sure there is room for the int value in Numeric(5,1), therefore s.maskWidth < 9999.
-- This also filters away wrong values (typos).




-- For non-HVN (since HVN is NULL for s.gearId), run below script for populating s.meshSize from R_GearInfo

--SELECT s.sampleId, CASE WHEN s2.meshSize = 0 THEN NULL ELSE s2.meshSize END as meshSize, gearInfoT, RowNum
UPDATE s
SET s.meshSize = CASE WHEN s2.meshSize = 0 THEN NULL ELSE s2.meshSize END
FROM
Sample s
INNER JOIN
(
	SELECT si.*, ROW_NUMBER() OVER(PARTITION BY si.SampleId ORDER BY si.gearInfoT) AS RowNum
	FROM
	(
	   SELECT s.SampleId, ISNULL(s.maskWidth, r_gi.gearValue) as meshSize, 
			 (CASE WHEN r_gi.gearInfoType = 'posemaskevid' THEN 1 WHEN r_gi.gearInfoType = 'maskevidde' THEN 2 WHEN r_gi.gearInfoType = 'maskevidde2' THEN 3 END) as 'gearInfoT'
	   FROM Sample s
	   LEFT OUTER JOIN dbo.R_GearInfo r_gi ON r_gi.GearId = s.GearId
       WHERE (r_gi.gearInfoType IN ('posemaskevid', 'maskevidde', 'maskevidde2'))
	) as si
) as s2 ON s.sampleId = s2.sampleId
WHERE s2.RowNum = 1




-- Fix platforms. Find trip.platform1 and trip.platform2 from R_TripPlatformVersion and L_PlatformVersion.

DECLARE @tripId int;
DECLARE @platform1 nvarchar(20);
DECLARE @platform2 nvarchar(20);

DECLARE Trip_Cursor CURSOR FOR
SELECT tripId
FROM Trip

OPEN Trip_Cursor

-- Create a cursor for looping through each trip individually
FETCH NEXT FROM Trip_Cursor
INTO @tripId

WHILE @@FETCH_STATUS = 0
BEGIN 
	SET @platform1 = null
	SET @platform2 = null

	-- Select first platform id out to put in trip.platform1 (as main vessel)
	SELECT @platform1 = pv.platform
	FROM R_TripPlatformVersion tpv
	INNER JOIN L_PlatformVersion pv ON pv.platformVersionId = tpv.platformVersionId
	WHERE tpv.tripId = @tripId
	order by tpv.R_TripPlatformVersionId desc

	-- Select second platform id out
	SELECT @platform2 = pv.platform
	FROM R_TripPlatformVersion tpv
	INNER JOIN L_PlatformVersion pv ON pv.platformVersionId = tpv.platformVersionId
	WHERE tpv.tripId = @tripId
	order by tpv.R_TripPlatformVersionId asc
	
	-- If both platforms are the same, only put one of them on trip (therefore set the second to null).
    IF @platform1 = @platform2
	  BEGIN
		SET @platform2 = NULL
      END

	-- Update trip with trip id @tripId with the found platforms
    --PRINT '' + CAST(@tripId as nvarchar) + ', ' + @platform1 + ', ' + @platform2
    UPDATE t
    SET platform1 = @platform1,
        platform2 = @platform2
    FROM Trip t
    WHERE t.tripId = @tripId
	

	-- Select next trip id into @tripId
	FETCH NEXT FROM Trip_Cursor
	INTO @tripId
END

CLOSE Trip_Cursor
DEALLOCATE Trip_Cursor



