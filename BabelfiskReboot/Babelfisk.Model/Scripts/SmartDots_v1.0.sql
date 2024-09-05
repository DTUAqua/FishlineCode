USE [FishLine]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO







CREATE TABLE [dbo].[SDEvent](
	[sdEventId] [int] IDENTITY(1,1) NOT NULL,
	[sdEventGuid] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](200) NULL,
	[speciesCode] [nvarchar](3) NULL,            -- L_Species
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[OrganizerEmail] [nvarchar](100) NULL,
	[EventType] [nvarchar](50) NULL,
	[Closed] [bit] NULL
 CONSTRAINT [PK_SDEvent] PRIMARY KEY CLUSTERED 
(
	[sdEventId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SDEvent]  WITH NOCHECK ADD  CONSTRAINT [FK_SDEvent_L_Species] FOREIGN KEY([speciesCode])
REFERENCES [dbo].[L_Species] ([speciesCode])
GO




CREATE TABLE [dbo].[SDSample](
	[sdSampleId] [int] IDENTITY(1,1) NOT NULL,
	[sdSampleGuid] [uniqueidentifier] NOT NULL,
	[sdEventId] [int] NOT NULL,
	[sampleId] [nvarchar](50) NULL,
	[animalId] [nvarchar](50) NULL,
	[catchDate] [datetime] NULL,
	[areaId] [int] NULL,                        -- L_DFUArea
	[statisticalRectangle] [nvarchar](4) NULL,  -- L_StatisticalRectangle
	[latitude] [float] NULL,
	[longitude] [float] NULL,
	[stockId] [int] NULL,                       -- New L_Stock table
	[sexCode] [nvarchar](1) NULL,               -- L_Sex
	[sampleOrigin] [nvarchar(100)] NULL,
	[sampleType] [nvarchar(100)] NULL,
	[preparationMethod] [nvarchar(100)] NULL,
	[FishLengthMM] [int] NULL,
	[FishWeightG] [numeric(10,5)] NULL,
	[maturityScale] [int] NULL,
	[maturittStage] [int] NULL,
	[Comments] [nvarchar](500) NULL,
	[CreatedByUserName] [nvarchar](10) NULL,
	[CreatedTime] [datetime] NULL,
	[ModifiedTime] [datetime] NULL

 CONSTRAINT [PK_SDSample] PRIMARY KEY CLUSTERED 
(
	[sdSampleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SDSample]  WITH NOCHECK ADD  CONSTRAINT [FK_SDSample_SDEvent] FOREIGN KEY([sdEventId])
REFERENCES [dbo].[SDEvent] ([sdEventId])
GO



CREATE TABLE [dbo].[SDFile](
	[sdfileId] [int] IDENTITY(1,1) NOT NULL,
	[sdfileGuid] [uniqueidentifier] NOT NULL,
	[sdSampleId] int NOT NULL,
	[fileName] nvarchar(500) NOT NULL,
	[displayName] nvarchar(500) NULL,
	[path] nvarchar(500) NULL,
	[scale] [float] NULL,
	[imageWidth] [int] NULL,
	[imageHeight] [int] NULL
 CONSTRAINT [PK_SDFile] PRIMARY KEY CLUSTERED 
(
	[sdfileId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SDFile]  WITH NOCHECK ADD  CONSTRAINT [FK_SDFile_SDSample] FOREIGN KEY([sdSampleId])
REFERENCES [dbo].[SDSample] ([sdSampleId])
GO





CREATE TABLE [dbo].[SDAnnotation](
	[sdAnnotationId] [int] IDENTITY(1,1) NOT NULL,
	[sdAnnotationGuid] [uniqueidentifier] NOT NULL,
	[sdFileId] int NOT NULL,
	[CreatedByUserName] [nvarchar](10) NULL,
	[IsApproved] [bit] NULL,
	[IsFixed] [bit] NULL,
	[IsReadOnly] [bit] NULL,
	[CreatedTime] [datetime] NULL,
	[ModifiedTime] [datetime] NULL
 CONSTRAINT [PK_SDAnnotation] PRIMARY KEY CLUSTERED 
(
	[sdAnnotationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SDAnnotation]  WITH NOCHECK ADD  CONSTRAINT [FK_SDAnnotation_SDFile] FOREIGN KEY([sdFileId])
REFERENCES [dbo].[SDFile] ([sdFileId])
GO





CREATE TABLE [dbo].[SDLine](
	[sdLineId] [int] IDENTITY(1,1) NOT NULL,
	[sdLineGuid] [uniqueidentifier] NOT NULL,
	[sdAnnotationId] int NOT NULL,
	[CreatedByUserName] [nvarchar](10) NULL,
	[CreatedTime] [datetime] NULL,
	[Color] [nvarchar](10) NULL,
	[LineIndex] [int] NULL,
	[Width] [int] NULL,
	[X1] [int] NULL,
	[X2] [int] NULL,
	[Y1] [int] NULL,
	[Y2] [int] NULL
 CONSTRAINT [PK_SDLine] PRIMARY KEY CLUSTERED 
(
	[sdLineId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SDLine]  WITH NOCHECK ADD  CONSTRAINT [FK_SDLine_SDAnnotation] FOREIGN KEY([sdAnnotationId])
REFERENCES [dbo].[SDAnnotation] ([sdAnnotationId])
GO





CREATE TABLE [dbo].[SDPoint](
	[sdPointId] [int] IDENTITY(1,1) NOT NULL,
	[sdPointGuid] [uniqueidentifier] NOT NULL,
	[sdAnnotationId] int NOT NULL,
	[CreatedByUserName] [nvarchar](10) NULL,
	[CreatedTime] [datetime] NULL,
	[Color] [nvarchar](10) NULL,
	[PointIndex] [int] NULL,
	[Width] [int] NULL,
	[PointType] [nvarchar](200) NULL,
	[Shape] [nvarchar](200) NULL,
	[X] [int] NULL,
	[Y] [int] NULL,
	
 CONSTRAINT [PK_SDPoint] PRIMARY KEY CLUSTERED 
(
	[sdPointId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SDPoint]  WITH NOCHECK ADD  CONSTRAINT [FK_SDPoint_SDAnnotation] FOREIGN KEY([sdAnnotationId])
REFERENCES [dbo].[SDAnnotation] ([sdAnnotationId])
GO


