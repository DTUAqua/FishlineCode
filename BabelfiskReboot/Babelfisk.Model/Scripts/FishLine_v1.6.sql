USE [FishLine]
GO


ALTER TABLE [dbo].[Trip]
ADD [dateSample] [datetime] NULL,
	[harbourSample] [nvarchar] (4) NULL

GO


ALTER TABLE [dbo].[Trip]  WITH CHECK ADD  CONSTRAINT [FK_Trip_L_Harbour_Sample] FOREIGN KEY([harbourSample])
REFERENCES [dbo].[L_Harbour] ([harbour])
GO



USE [FishLineDW]
GO

ALTER TABLE [dbo].[Trip]
ADD [dateSample] [datetime] NULL,
	[harbourSample] [nvarchar] (4) NULL,
	[nationalityHarbourSample] [nvarchar] (3) NULL

GO