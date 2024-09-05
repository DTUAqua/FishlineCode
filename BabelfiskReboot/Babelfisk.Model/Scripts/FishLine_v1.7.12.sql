USE [FishLine]
GO



-- Add a tasks column to report table

ALTER TABLE [dbo].[Report]
ADD [outputPath] nvarchar(max) NULL
GO

ALTER TABLE [dbo].[Report]
ADD [outputPathRestriction] nvarchar(max) NULL
GO
