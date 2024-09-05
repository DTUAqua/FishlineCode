USE [FishLineDW]
GO


SET ANSI_NULLS ON
GO


-- Create L_Species in Datawarehouse (with exactly the same columns as in FishLine).

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[L_Species](
	[L_speciesId] [int] NOT NULL,
	[speciesCode] [nvarchar](3) NOT NULL,
	[dkName] [nvarchar](80) NULL,
	[ukName] [nvarchar](80) NULL,
	[nodc] [nvarchar](80) NULL,
	[latin] [nvarchar](80) NULL,
	[icesCode] [nvarchar](3) NULL,
	[treatmentFactorGroup] [nvarchar](3) NULL,
	[dfuFisk_Code] [nvarchar](3) NULL,
	[tsn] [nvarchar](6) NULL,
	[aphiaID] [int] NULL,
	[lengthMin] [int] NULL,
	[lengthMax] [int] NULL,
	[ageMin] [int] NULL,
	[ageMax] [int] NULL,
	[weightMin] [int] NULL,
	[weightMax] [int] NULL,
	[speciesNES] [nvarchar](3) NULL,
	[speciesFAO] [nvarchar](3) NULL,
 CONSTRAINT [PK_L_Species] PRIMARY KEY CLUSTERED 
(
	[speciesCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Length in mm' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'L_Species', @level2type=N'COLUMN',@level2name=N'lengthMin'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Length in mm' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'L_Species', @level2type=N'COLUMN',@level2name=N'lengthMax'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'age in years' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'L_Species', @level2type=N'COLUMN',@level2name=N'ageMin'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'age in years' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'L_Species', @level2type=N'COLUMN',@level2name=N'ageMax'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'weight in grammes' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'L_Species', @level2type=N'COLUMN',@level2name=N'weightMin'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'weight in grammes' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'L_Species', @level2type=N'COLUMN',@level2name=N'weightMax'
GO



USE [FishLineSecurity]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[FishLineTasks](
	[FishLineTaskId] [int] IDENTITY(1,1) NOT NULL,
	[Value] [nvarchar](max) NOT NULL
 CONSTRAINT [PK_KFishTaskSet] PRIMARY KEY CLUSTERED 
(
	[FishLineTaskId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO





CREATE TABLE [dbo].[FishLineTaskRole](
	[FishLineTaskId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
 CONSTRAINT [PK_TaskRole] PRIMARY KEY NONCLUSTERED 
(
	[FishLineTaskId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[FishLineTaskRole]  WITH CHECK ADD  CONSTRAINT [FK_FishLineTaskRole_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([RoleId_PK])
GO

ALTER TABLE [dbo].[FishLineTaskRole] CHECK CONSTRAINT [FK_FishLineTaskRole_Role]
GO

ALTER TABLE [dbo].[FishLineTaskRole]  WITH CHECK ADD  CONSTRAINT [FK_FishLineTaskRole_FishLineTasks] FOREIGN KEY([FishLineTaskId])
REFERENCES [dbo].[FishLineTasks] ([FishLineTaskId])
GO

ALTER TABLE [dbo].[FishLineTaskRole] CHECK CONSTRAINT [FK_FishLineTaskRole_FishLineTasks]
GO





--Remove contraint for groupId column, since a user can now have several roles.
ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_Users_Role]
ALTER TABLE [dbo].[Users] DROP  CONSTRAINT [DF_Users_groupId]

ALTER TABLE [dbo].[Users] ALTER COLUMN groupId_FK INT NULL



CREATE TABLE [dbo].[RoleUser](
	[RolesId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [PK_RoleUser] PRIMARY KEY NONCLUSTERED 
(
	[RolesId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[RoleUser]  WITH CHECK ADD  CONSTRAINT [FK_RoleUserRole] FOREIGN KEY([RolesId])
REFERENCES [dbo].[Role] ([RoleId_PK])
GO

ALTER TABLE [dbo].[RoleUser] CHECK CONSTRAINT [FK_RoleUserRole]
GO

ALTER TABLE [dbo].[RoleUser]  WITH CHECK ADD  CONSTRAINT [FK_RoleUserUser] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO

ALTER TABLE [dbo].[RoleUser] CHECK CONSTRAINT [FK_RoleUserUser]
GO



--Loop through all users and add their groupId to RoleUser entry.
DECLARE @userId int;
DECLARE @rolesId int;

DECLARE User_Cursor CURSOR FOR
SELECT UserId
FROM Users

OPEN User_Cursor

FETCH NEXT FROM User_Cursor
INTO @userId

WHILE @@FETCH_STATUS = 0
BEGIN 
	
	-- Select platform id out to put in trip.platform1 (as main vessel)
	SELECT @rolesId = u.groupId_FK
	FROM Users u
	WHERE u.UserId = @userId

	INSERT INTO RoleUser
    VALUES (@rolesId, @userId)
	
	FETCH NEXT FROM User_Cursor
	INTO @userId
END

CLOSE User_Cursor
DEALLOCATE User_Cursor



--Add security tasks
INSERT INTO FishLineTasks
VALUES ('ReadData'),
       ('EditUsers'),
       ('ViewLookups'),
       ('EditLookups'),
       ('EditSomeLookups'),
       ('ModifyData'),
       ('DeleteData'),
       ('GoOffline'),
       ('ExportData'),
       ('ExportToWarehouse'),
       ('ViewReports'),
       ('EditReports'),
       ('ViewDFADReports')

--Assign tasks to roles
INSERT INTO FishLineTaskRole  --Admin
VALUES (1, 4),
       (2, 4),
	   (3, 4),
	   (4, 4),
	   (5, 4),
	   (6, 4),
	   (7, 4),
	   (8, 4),
	   (9, 4),
	   (10, 4),
	   (11, 4),
	   (12, 4),
	   (13, 4)

INSERT INTO FishLineTaskRole  --Editor
VALUES (1, 3),
	   (3, 3),
	   (5, 3),
	   (6, 3),
	   (8, 3),
	   (9, 3),
	   (10, 3),
	   (11, 3)

INSERT INTO FishLineTaskRole  --Reader
VALUES (1, 2),
	   (3, 2),
	   (8, 2),
	   (9, 2),
	   (11, 2)

INSERT INTO FishLineTaskRole  --Guest
VALUES (1, 1)




USE [FishLineSecurity]
GO


-- Add LoginAttempts column to Users

ALTER TABLE [dbo].[Users]
ADD [LoginAttempts] int NULL
GO

-- Set login attempts for all users to zero
UPDATE [dbo].[Users]
SET [LoginAttempts] = 0
GO

-- Make LoginAttempts column not nullable
ALTER TABLE [dbo].[Users]
ALTER COLUMN [LoginAttempts] int NOT NULL
GO

ALTER TABLE [dbo].[Users]
ALTER COLUMN [Password] [nvarchar](MAX) NOT NULL
GO



-- Add IsActive column to Users

ALTER TABLE [dbo].[Users]
ADD [IsActive] [bit] NULL
GO


UPDATE [dbo].[Users]
SET [IsActive] = 1
GO

ALTER TABLE [dbo].[Users]
ALTER COLUMN [IsActive] [bit] NOT NULL
GO



USE [FishLine]
GO


-- Add a tasks column to report table

ALTER TABLE [dbo].[Report]
ADD [permissionTasks] nvarchar(max) NULL
GO
