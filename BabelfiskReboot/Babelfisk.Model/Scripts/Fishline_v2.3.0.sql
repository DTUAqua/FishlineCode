USE [FishLine]
GO

-- Add maturitySMSF to store maturity stage short code (max 3 chars)
-- Connects to the "SMSF Kode" column for maturity indexes in Fiskeline. 

ALTER TABLE [dbo].[Maturity]
ADD [maturitySMSF] nvarchar(3) NULL
GO