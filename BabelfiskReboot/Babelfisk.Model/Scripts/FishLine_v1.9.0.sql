USE [FishLine]
GO

-- Add Visual Stock
CREATE TABLE [dbo].[L_VisualStock](
	[L_visualStockId] [int] IDENTITY(1,1) NOT NULL,
	[speciesCode] [nvarchar](3) NOT NULL,
	[visualStock] [nvarchar](50) NOT NULL,
	[description] [nvarchar](100) NULL,
	[num] int null,
 CONSTRAINT [PK_L_VisualStock] PRIMARY KEY CLUSTERED 
(
	[L_visualStockId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_L_VisualStock] UNIQUE NONCLUSTERED 
(
	[speciesCode] ASC,
	[visualStock] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[L_VisualStock]  WITH NOCHECK ADD  CONSTRAINT [FK_R_L_VisualStock_L_Species] FOREIGN KEY([speciesCode])
REFERENCES [dbo].[L_Species] ([speciesCode])
GO

ALTER TABLE [dbo].[L_VisualStock] CHECK CONSTRAINT [FK_R_L_VisualStock_L_Species]
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_L_VisualStock_L_Species'
CREATE INDEX [IX_FK_R_L_VisualStock_L_Species] ON [dbo].[L_VisualStock] ([speciesCode]);
GO


--Add Genetic Stock
CREATE TABLE [dbo].[L_GeneticStock](
	[L_geneticStockId] [int] IDENTITY(1,1) NOT NULL,
	[speciesCode] [nvarchar](3) NOT NULL,
	[geneticStock] [nvarchar](50) NOT NULL,
	[description] [nvarchar](100) NULL,
	[num] int null,
 CONSTRAINT [PK_L_GeneticStock] PRIMARY KEY CLUSTERED 
(
	[L_geneticStockId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_L_GeneticStock] UNIQUE NONCLUSTERED 
(
	[speciesCode] ASC,
	[geneticStock] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[L_GeneticStock]  WITH NOCHECK ADD  CONSTRAINT [FK_R_L_GeneticStock_L_Species] FOREIGN KEY([speciesCode])
REFERENCES [dbo].[L_Species] ([speciesCode])
GO

ALTER TABLE [dbo].[L_GeneticStock] CHECK CONSTRAINT [FK_R_L_GeneticStock_L_Species]
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_L_GeneticStock_L_Species'
CREATE INDEX [IX_FK_R_L_GeneticStock_L_Species] ON [dbo].[L_GeneticStock] ([speciesCode]);
GO


-- Add Visual and Genetic Stock to Age table.
ALTER TABLE [dbo].[Age]
ADD [visualStockId] int NULL,
    [geneticStockId] int NULL
GO

ALTER TABLE [dbo].[Age]  WITH CHECK ADD  CONSTRAINT [FK_Age_L_VisualStock] FOREIGN KEY([visualStockId])
REFERENCES [dbo].[L_VisualStock] ([L_visualStockId])
GO

ALTER TABLE [dbo].[Age] CHECK CONSTRAINT [FK_Age_L_VisualStock]
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Age_L_VisualStock'
CREATE INDEX [IX_FK_Age_L_VisualStock] ON [dbo].[Age] ([visualStockId]);
GO


ALTER TABLE [dbo].[Age]  WITH CHECK ADD  CONSTRAINT [FK_Age_L_GeneticStock] FOREIGN KEY([geneticStockId])
REFERENCES [dbo].[L_GeneticStock] ([L_geneticStockId])
GO

ALTER TABLE [dbo].[Age] CHECK CONSTRAINT [FK_Age_L_GeneticStock]
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Age_L_GeneticStock'
CREATE INDEX [IX_FK_Age_L_GeneticStock] ON [dbo].[Age] ([geneticStockId]);
GO
 
-- Add BMS Ej Rep to SpeciesList (remember to move it up efter number manually)
ALTER TABLE [dbo].[SpeciesList]
ADD [bmsNonRep] numeric(11,4) NULL
GO



USE [FishLineDW]
GO

-- Add visual stock and genetic stock to Age table.
ALTER TABLE [dbo].[Age]
ADD [visualStock] [nvarchar](50) NULL,
    [geneticStock] [nvarchar](50) NULL
GO



-- Add BMS Ej Rep to SpeciesList (remember to move it up efter weightNonRep manually)
ALTER TABLE [dbo].[SpeciesList]
ADD [bmsNonRep] numeric(11,4) NULL
GO




