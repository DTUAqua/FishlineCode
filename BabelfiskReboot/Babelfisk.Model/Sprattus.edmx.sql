
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 03/19/2024 08:19:57
-- Generated from EDMX file: C:\Mads\Development\TeamProjects\anchorlabcloud\Babelfisk\BabelfiskReboot\Babelfisk.Model\Sprattus.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [FishLine];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Age_Animal]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Age] DROP CONSTRAINT [FK_Age_Animal];
GO
IF OBJECT_ID(N'[dbo].[FK_Age_L_AgeMeasure]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Age] DROP CONSTRAINT [FK_Age_L_AgeMeasure];
GO
IF OBJECT_ID(N'[dbo].[FK_Age_L_OtolithReadingRemark]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Age] DROP CONSTRAINT [FK_Age_L_OtolithReadingRemark];
GO
IF OBJECT_ID(N'[dbo].[FK_Animal_L_BroodingPhase]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Animal] DROP CONSTRAINT [FK_Animal_L_BroodingPhase];
GO
IF OBJECT_ID(N'[dbo].[FK_Animal_L_LengthMeasureUnit]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Animal] DROP CONSTRAINT [FK_Animal_L_LengthMeasureUnit];
GO
IF OBJECT_ID(N'[dbo].[FK_Animal_L_SexCode]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Animal] DROP CONSTRAINT [FK_Animal_L_SexCode];
GO
IF OBJECT_ID(N'[dbo].[FK_Animal_SubSample]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Animal] DROP CONSTRAINT [FK_Animal_SubSample];
GO
IF OBJECT_ID(N'[dbo].[FK_AnimalInfo_Animal]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AnimalInfo] DROP CONSTRAINT [FK_AnimalInfo_Animal];
GO
IF OBJECT_ID(N'[dbo].[FK_AnimalInfo_Fat]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AnimalInfo] DROP CONSTRAINT [FK_AnimalInfo_Fat];
GO
IF OBJECT_ID(N'[dbo].[FK_AnimalInfo_Maturity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AnimalInfo] DROP CONSTRAINT [FK_AnimalInfo_Maturity];
GO
IF OBJECT_ID(N'[dbo].[FK_Cruise_DFUPerson]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Cruise] DROP CONSTRAINT [FK_Cruise_DFUPerson];
GO
IF OBJECT_ID(N'[dbo].[FK_Cruise_L_CruiseStatus]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Cruise] DROP CONSTRAINT [FK_Cruise_L_CruiseStatus];
GO
IF OBJECT_ID(N'[dbo].[FK_Cruise_L_DfuPerson]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Cruise] DROP CONSTRAINT [FK_Cruise_L_DfuPerson];
GO
IF OBJECT_ID(N'[dbo].[FK_Cruise_L_Institute]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Cruise] DROP CONSTRAINT [FK_Cruise_L_Institute];
GO
IF OBJECT_ID(N'[dbo].[FK_Est_Method_Cruise]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Est_Method] DROP CONSTRAINT [FK_Est_Method_Cruise];
GO
IF OBJECT_ID(N'[dbo].[FK_Est_Strata_Cruise]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Est_Strata] DROP CONSTRAINT [FK_Est_Strata_Cruise];
GO
IF OBJECT_ID(N'[dbo].[FK_Est_Strata_Cruise1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Est_Strata] DROP CONSTRAINT [FK_Est_Strata_Cruise1];
GO
IF OBJECT_ID(N'[dbo].[FK_R_CruiseUsabilityParam_Cruise]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_CruiseUsabilityParam] DROP CONSTRAINT [FK_R_CruiseUsabilityParam_Cruise];
GO
IF OBJECT_ID(N'[dbo].[FK_Trip_Cruise]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Trip] DROP CONSTRAINT [FK_Trip_Cruise];
GO
IF OBJECT_ID(N'[dbo].[FK_DFUPerson_L_DFUDepartment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DFUPerson] DROP CONSTRAINT [FK_DFUPerson_L_DFUDepartment];
GO
IF OBJECT_ID(N'[dbo].[FK_Est_Strata_DFUPerson]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Est_Strata] DROP CONSTRAINT [FK_Est_Strata_DFUPerson];
GO
IF OBJECT_ID(N'[dbo].[FK_Sample_DFUPerson]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_Sample_DFUPerson];
GO
IF OBJECT_ID(N'[dbo].[FK_Sample_DFUPerson1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_Sample_DFUPerson1];
GO
IF OBJECT_ID(N'[dbo].[FK_SpeciesList_L_DfuPerson]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SpeciesList] DROP CONSTRAINT [FK_SpeciesList_L_DfuPerson];
GO
IF OBJECT_ID(N'[dbo].[FK_SpeciesList_L_DfuPerson1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SpeciesList] DROP CONSTRAINT [FK_SpeciesList_L_DfuPerson1];
GO
IF OBJECT_ID(N'[dbo].[FK_Trip_L_DfuPerson]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Trip] DROP CONSTRAINT [FK_Trip_L_DfuPerson];
GO
IF OBJECT_ID(N'[dbo].[FK_Trip_L_DfuPerson1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Trip] DROP CONSTRAINT [FK_Trip_L_DfuPerson1];
GO
IF OBJECT_ID(N'[dbo].[FK_DFUSubArea_L_DFUArea]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DFUSubArea] DROP CONSTRAINT [FK_DFUSubArea_L_DFUArea];
GO
IF OBJECT_ID(N'[dbo].[FK_Est_MethodStep_Est_Method]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Est_MethodStep] DROP CONSTRAINT [FK_Est_MethodStep_Est_Method];
GO
IF OBJECT_ID(N'[dbo].[FK_Est_MethodStep_Est_Strata]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Est_MethodStep] DROP CONSTRAINT [FK_Est_MethodStep_Est_Strata];
GO
IF OBJECT_ID(N'[dbo].[FK_Est_MethodStep_L_DFU_Category]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Est_MethodStep] DROP CONSTRAINT [FK_Est_MethodStep_L_DFU_Category];
GO
IF OBJECT_ID(N'[dbo].[FK_Est_MethodStep_L_Species]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Est_MethodStep] DROP CONSTRAINT [FK_Est_MethodStep_L_Species];
GO
IF OBJECT_ID(N'[dbo].[FK_Est_MethodStep_L_YesNo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Est_MethodStep] DROP CONSTRAINT [FK_Est_MethodStep_L_YesNo];
GO
IF OBJECT_ID(N'[dbo].[FK_Est_Strata_L_DFU_Category]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Est_Strata] DROP CONSTRAINT [FK_Est_Strata_L_DFU_Category];
GO
IF OBJECT_ID(N'[dbo].[FK_Est_Strata_L_YesNo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Est_Strata] DROP CONSTRAINT [FK_Est_Strata_L_YesNo];
GO
IF OBJECT_ID(N'[dbo].[FK_Est_Strata_L_YesNo1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Est_Strata] DROP CONSTRAINT [FK_Est_Strata_L_YesNo1];
GO
IF OBJECT_ID(N'[dbo].[FK_Est_Strata_Sample]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Est_Strata] DROP CONSTRAINT [FK_Est_Strata_Sample];
GO
IF OBJECT_ID(N'[dbo].[FK_Est_Strata_Trip]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Est_Strata] DROP CONSTRAINT [FK_Est_Strata_Trip];
GO
IF OBJECT_ID(N'[dbo].[FK_L_Fatindex_L_FatIndexType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fat] DROP CONSTRAINT [FK_L_Fatindex_L_FatIndexType];
GO
IF OBJECT_ID(N'[dbo].[FK_Sample_L_CatchRegistration]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_Sample_L_CatchRegistration];
GO
IF OBJECT_ID(N'[dbo].[FK_L_TripType_L_CruiseType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[L_TripType] DROP CONSTRAINT [FK_L_TripType_L_CruiseType];
GO
IF OBJECT_ID(N'[dbo].[FK_SpeciesList_L_CuticulaHardness]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SpeciesList] DROP CONSTRAINT [FK_SpeciesList_L_CuticulaHardness];
GO
IF OBJECT_ID(N'[dbo].[FK_L_DFUArea_L_DFUArea]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[L_DFUArea] DROP CONSTRAINT [FK_L_DFUArea_L_DFUArea];
GO
IF OBJECT_ID(N'[dbo].[FK_Sample_L_DFUArea]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_Sample_L_DFUArea];
GO
IF OBJECT_ID(N'[dbo].[FK_SpeciesList_L_DFU_Category]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SpeciesList] DROP CONSTRAINT [FK_SpeciesList_L_DFU_Category];
GO
IF OBJECT_ID(N'[dbo].[FK_Sample_L_FishingActivityCategory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_Sample_L_FishingActivityCategory];
GO
IF OBJECT_ID(N'[dbo].[FK_L_Gear_L_GearType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[L_Gear] DROP CONSTRAINT [FK_L_Gear_L_GearType];
GO
IF OBJECT_ID(N'[dbo].[FK_L_Gear_L_Platform1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[L_Gear] DROP CONSTRAINT [FK_L_Gear_L_Platform1];
GO
IF OBJECT_ID(N'[dbo].[FK_R_GearInfo_L_Gear]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_GearInfo] DROP CONSTRAINT [FK_R_GearInfo_L_Gear];
GO
IF OBJECT_ID(N'[dbo].[FK_Sample_L_Gear]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_Sample_L_Gear];
GO
IF OBJECT_ID(N'[dbo].[FK_R_GearInfo_L_GearParam]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_GearInfo] DROP CONSTRAINT [FK_R_GearInfo_L_GearParam];
GO
IF OBJECT_ID(N'[dbo].[FK_L_GearType_L_CatchOperation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[L_GearType] DROP CONSTRAINT [FK_L_GearType_L_CatchOperation];
GO
IF OBJECT_ID(N'[dbo].[FK_Sample_L_GearType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_Sample_L_GearType];
GO
IF OBJECT_ID(N'[dbo].[FK_L_Harbour_L_Nationality]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[L_Harbour] DROP CONSTRAINT [FK_L_Harbour_L_Nationality];
GO
IF OBJECT_ID(N'[dbo].[FK_Trip_L_Harbour]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Trip] DROP CONSTRAINT [FK_Trip_L_Harbour];
GO
IF OBJECT_ID(N'[dbo].[FK_SpeciesList_L_Category]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SpeciesList] DROP CONSTRAINT [FK_SpeciesList_L_Category];
GO
IF OBJECT_ID(N'[dbo].[FK_R_MaturityMaturityIndex_L_MaturityIndex]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Maturity] DROP CONSTRAINT [FK_R_MaturityMaturityIndex_L_MaturityIndex];
GO
IF OBJECT_ID(N'[dbo].[FK_L_Platform_L_Nationality]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[L_Platform] DROP CONSTRAINT [FK_L_Platform_L_Nationality];
GO
IF OBJECT_ID(N'[dbo].[FK_Trip_L_Nationality]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Trip] DROP CONSTRAINT [FK_Trip_L_Nationality];
GO
IF OBJECT_ID(N'[dbo].[FK_L_PlatformVersion_L_NavigationSystem]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[L_PlatformVersion] DROP CONSTRAINT [FK_L_PlatformVersion_L_NavigationSystem];
GO
IF OBJECT_ID(N'[dbo].[FK_L_Platform_L_PlatformType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[L_Platform] DROP CONSTRAINT [FK_L_Platform_L_PlatformType];
GO
IF OBJECT_ID(N'[dbo].[FK_L_Platform_Person]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[L_Platform] DROP CONSTRAINT [FK_L_Platform_Person];
GO
IF OBJECT_ID(N'[dbo].[FK_L_PlatformVersion_L_Platform]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[L_PlatformVersion] DROP CONSTRAINT [FK_L_PlatformVersion_L_Platform];
GO
IF OBJECT_ID(N'[dbo].[FK_R_platformVersion_L_PlatformVersion]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_TripPlatformVersion] DROP CONSTRAINT [FK_R_platformVersion_L_PlatformVersion];
GO
IF OBJECT_ID(N'[dbo].[FK_Sample_L_SampleStatus]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_Sample_L_SampleStatus];
GO
IF OBJECT_ID(N'[dbo].[FK_Trip_L_SamplingMethod]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Trip] DROP CONSTRAINT [FK_Trip_L_SamplingMethod];
GO
IF OBJECT_ID(N'[dbo].[FK_Trip_L_SamplingType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Trip] DROP CONSTRAINT [FK_Trip_L_SamplingType];
GO
IF OBJECT_ID(N'[dbo].[FK_SpeciesList_L_SexCode]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SpeciesList] DROP CONSTRAINT [FK_SpeciesList_L_SexCode];
GO
IF OBJECT_ID(N'[dbo].[FK_SpeciesList_L_SizeSortingDFU]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SpeciesList] DROP CONSTRAINT [FK_SpeciesList_L_SizeSortingDFU];
GO
IF OBJECT_ID(N'[dbo].[FK_SpeciesList_L_SizeSortingEU]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SpeciesList] DROP CONSTRAINT [FK_SpeciesList_L_SizeSortingEU];
GO
IF OBJECT_ID(N'[dbo].[FK_L_Species_L_CleaningGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[L_Species] DROP CONSTRAINT [FK_L_Species_L_CleaningGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_R_targetSpecies_L_Species]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_TargetSpecies] DROP CONSTRAINT [FK_R_targetSpecies_L_Species];
GO
IF OBJECT_ID(N'[dbo].[FK_R_targetSpecies_L_Species1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_TargetSpecies] DROP CONSTRAINT [FK_R_targetSpecies_L_Species1];
GO
IF OBJECT_ID(N'[dbo].[FK_SpeciesList_L_Species]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SpeciesList] DROP CONSTRAINT [FK_SpeciesList_L_Species];
GO
IF OBJECT_ID(N'[dbo].[FK_Sample_L_SpeciesRegistration]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_Sample_L_SpeciesRegistration];
GO
IF OBJECT_ID(N'[dbo].[FK_Sample_L_StatisticalRectangle]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_Sample_L_StatisticalRectangle];
GO
IF OBJECT_ID(N'[dbo].[FK_SpeciesList_L_Treatment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SpeciesList] DROP CONSTRAINT [FK_SpeciesList_L_Treatment];
GO
IF OBJECT_ID(N'[dbo].[FK_TreatmentFactor_L_Treatment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TreatmentFactor] DROP CONSTRAINT [FK_TreatmentFactor_L_Treatment];
GO
IF OBJECT_ID(N'[dbo].[FK_TreatmentFactor_L_treatmentFactorGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TreatmentFactor] DROP CONSTRAINT [FK_TreatmentFactor_L_treatmentFactorGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_Trip_L_TripType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Trip] DROP CONSTRAINT [FK_Trip_L_TripType];
GO
IF OBJECT_ID(N'[dbo].[FK_R_CruiseUsabilityParam_UsabilityParam]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_CruiseUsabilityParam] DROP CONSTRAINT [FK_R_CruiseUsabilityParam_UsabilityParam];
GO
IF OBJECT_ID(N'[dbo].[FK_R_SampleUsabilityParam_L_UsabilityParam]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_SampleUsabilityParam] DROP CONSTRAINT [FK_R_SampleUsabilityParam_L_UsabilityParam];
GO
IF OBJECT_ID(N'[dbo].[FK_R_TripUsabilityParam_L_UsabilityParam]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_TripUsabilityParam] DROP CONSTRAINT [FK_R_TripUsabilityParam_L_UsabilityParam];
GO
IF OBJECT_ID(N'[dbo].[FK_R_UsabilityParamUsabilityGrp_UsabilityParam]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_UsabilityParamUsabilityGrp] DROP CONSTRAINT [FK_R_UsabilityParamUsabilityGrp_UsabilityParam];
GO
IF OBJECT_ID(N'[dbo].[FK_R_UsabilityParamUsabilityGrp_L_UsabilityParamGrp]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_UsabilityParamUsabilityGrp] DROP CONSTRAINT [FK_R_UsabilityParamUsabilityGrp_L_UsabilityParamGrp];
GO
IF OBJECT_ID(N'[dbo].[FK_SubSample_L_YesNo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SubSample] DROP CONSTRAINT [FK_SubSample_L_YesNo];
GO
IF OBJECT_ID(N'[dbo].[FK_NumOfStationsPerTrip_Trip]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NumberOfStationsPerTrip] DROP CONSTRAINT [FK_NumOfStationsPerTrip_Trip];
GO
IF OBJECT_ID(N'[dbo].[FK_Trip_L_Person]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Trip] DROP CONSTRAINT [FK_Trip_L_Person];
GO
IF OBJECT_ID(N'[dbo].[FK_R_SampleUsabilityParam_Sample]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_SampleUsabilityParam] DROP CONSTRAINT [FK_R_SampleUsabilityParam_Sample];
GO
IF OBJECT_ID(N'[dbo].[FK_R_targetSpecies_Sample]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_TargetSpecies] DROP CONSTRAINT [FK_R_targetSpecies_Sample];
GO
IF OBJECT_ID(N'[dbo].[FK_R_platformVersion_Trip]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_TripPlatformVersion] DROP CONSTRAINT [FK_R_platformVersion_Trip];
GO
IF OBJECT_ID(N'[dbo].[FK_R_TripUsabilityParam_Trip]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_TripUsabilityParam] DROP CONSTRAINT [FK_R_TripUsabilityParam_Trip];
GO
IF OBJECT_ID(N'[dbo].[FK_Sample_Trip]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_Sample_Trip];
GO
IF OBJECT_ID(N'[dbo].[FK_SpeciesList_Sample]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SpeciesList] DROP CONSTRAINT [FK_SpeciesList_Sample];
GO
IF OBJECT_ID(N'[dbo].[FK_TrawlOpr_Sample]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TrawlOperation] DROP CONSTRAINT [FK_TrawlOpr_Sample];
GO
IF OBJECT_ID(N'[dbo].[FK_SubSample_SpeciesList]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SubSample] DROP CONSTRAINT [FK_SubSample_SpeciesList];
GO
IF OBJECT_ID(N'[dbo].[FK_Trip_L_Platform1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Trip] DROP CONSTRAINT [FK_Trip_L_Platform1];
GO
IF OBJECT_ID(N'[dbo].[FK_Trip_L_Platform2]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Trip] DROP CONSTRAINT [FK_Trip_L_Platform2];
GO
IF OBJECT_ID(N'[dbo].[FK_Sample_L_GearType_2]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_Sample_L_GearType_2];
GO
IF OBJECT_ID(N'[dbo].[FK_Sample_L_SelectionDevice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_Sample_L_SelectionDevice];
GO
IF OBJECT_ID(N'[dbo].[FK_R_GearType_L_GearType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_GearTypeSelectionDevice] DROP CONSTRAINT [FK_R_GearType_L_GearType];
GO
IF OBJECT_ID(N'[dbo].[FK_R_SampleGear_L_SelectionDevice]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_GearTypeSelectionDevice] DROP CONSTRAINT [FK_R_SampleGear_L_SelectionDevice];
GO
IF OBJECT_ID(N'[dbo].[FK_L_FisheryType_L_LandingCategory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[L_FisheryType] DROP CONSTRAINT [FK_L_FisheryType_L_LandingCategory];
GO
IF OBJECT_ID(N'[dbo].[FK_Trip_L_FisheryType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Trip] DROP CONSTRAINT [FK_Trip_L_FisheryType];
GO
IF OBJECT_ID(N'[dbo].[FK_Sample_L_HaulType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_Sample_L_HaulType];
GO
IF OBJECT_ID(N'[dbo].[FK_Sample_L_ThermoCline]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_Sample_L_ThermoCline];
GO
IF OBJECT_ID(N'[dbo].[FK_Sample_L_TimeZone]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_Sample_L_TimeZone];
GO
IF OBJECT_ID(N'[dbo].[FK_Trip_L_TimeZone]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Trip] DROP CONSTRAINT [FK_Trip_L_TimeZone];
GO
IF OBJECT_ID(N'[dbo].[FK_Sample_L_WeightEstimationMethod]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_Sample_L_WeightEstimationMethod];
GO
IF OBJECT_ID(N'[dbo].[FK_SpeciesList_L_WeightEstimationMethod]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SpeciesList] DROP CONSTRAINT [FK_SpeciesList_L_WeightEstimationMethod];
GO
IF OBJECT_ID(N'[dbo].[FK_Age_L_EdgeStructure]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Age] DROP CONSTRAINT [FK_Age_L_EdgeStructure];
GO
IF OBJECT_ID(N'[dbo].[FK_AnimalInfo_L_Parasite]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AnimalInfo] DROP CONSTRAINT [FK_AnimalInfo_L_Parasite];
GO
IF OBJECT_ID(N'[dbo].[FK_R_AnimalInfoReference_AnimalInfo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_AnimalInfoReference] DROP CONSTRAINT [FK_R_AnimalInfoReference_AnimalInfo];
GO
IF OBJECT_ID(N'[dbo].[FK_R_AnimalInfoReference_L_Reference]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_AnimalInfoReference] DROP CONSTRAINT [FK_R_AnimalInfoReference_L_Reference];
GO
IF OBJECT_ID(N'[dbo].[FK_SpeciesList_DFUPerson_MaturityReader]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SpeciesList] DROP CONSTRAINT [FK_SpeciesList_DFUPerson_MaturityReader];
GO
IF OBJECT_ID(N'[dbo].[FK_SpeciesList_DFUPerson_MontReader]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SpeciesList] DROP CONSTRAINT [FK_SpeciesList_DFUPerson_MontReader];
GO
IF OBJECT_ID(N'[dbo].[FK_ReportingTreeNode_ReportingTreeNode]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ReportingTreeNode] DROP CONSTRAINT [FK_ReportingTreeNode_ReportingTreeNode];
GO
IF OBJECT_ID(N'[dbo].[FK_R_ReportingTreeNodeReport_Report]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_ReportingTreeNodeReport] DROP CONSTRAINT [FK_R_ReportingTreeNodeReport_Report];
GO
IF OBJECT_ID(N'[dbo].[FK_R_ReportingTreeNodeReport_ReportingTreeNode]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_ReportingTreeNodeReport] DROP CONSTRAINT [FK_R_ReportingTreeNodeReport_ReportingTreeNode];
GO
IF OBJECT_ID(N'[dbo].[FK_AnimalFile_Animal]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AnimalFile] DROP CONSTRAINT [FK_AnimalFile_Animal];
GO
IF OBJECT_ID(N'[dbo].[FK_Trip_L_Harbour_Sample]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Trip] DROP CONSTRAINT [FK_Trip_L_Harbour_Sample];
GO
IF OBJECT_ID(N'[dbo].[FK_L_ApplicationSpeciesList]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SpeciesList] DROP CONSTRAINT [FK_L_ApplicationSpeciesList];
GO
IF OBJECT_ID(N'[dbo].[FK_L_HatchMonthReadabilityAge]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Age] DROP CONSTRAINT [FK_L_HatchMonthReadabilityAge];
GO
IF OBJECT_ID(N'[dbo].[FK_R_L_VisualStock_L_Species]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[L_VisualStock] DROP CONSTRAINT [FK_R_L_VisualStock_L_Species];
GO
IF OBJECT_ID(N'[dbo].[FK_R_L_GeneticStock_L_Species]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[L_GeneticStock] DROP CONSTRAINT [FK_R_L_GeneticStock_L_Species];
GO
IF OBJECT_ID(N'[dbo].[FK_Age_L_VisualStock]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Age] DROP CONSTRAINT [FK_Age_L_VisualStock];
GO
IF OBJECT_ID(N'[dbo].[FK_Age_L_GeneticStock]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Age] DROP CONSTRAINT [FK_Age_L_GeneticStock];
GO
IF OBJECT_ID(N'[dbo].[FK_SDSample_L_DFUArea]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDSample] DROP CONSTRAINT [FK_SDSample_L_DFUArea];
GO
IF OBJECT_ID(N'[dbo].[FK_SDSample_L_MaturityIndexMethod]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDSample] DROP CONSTRAINT [FK_SDSample_L_MaturityIndexMethod];
GO
IF OBJECT_ID(N'[dbo].[FK_SDEvent_L_SDEventType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDEvent] DROP CONSTRAINT [FK_SDEvent_L_SDEventType];
GO
IF OBJECT_ID(N'[dbo].[FK_SDEvent_L_SDPurpose]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDEvent] DROP CONSTRAINT [FK_SDEvent_L_SDPurpose];
GO
IF OBJECT_ID(N'[dbo].[FK_SDSample_L_SexCode]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDSample] DROP CONSTRAINT [FK_SDSample_L_SexCode];
GO
IF OBJECT_ID(N'[dbo].[FK_SDEvent_L_Species]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDEvent] DROP CONSTRAINT [FK_SDEvent_L_Species];
GO
IF OBJECT_ID(N'[dbo].[FK_SDSample_L_StatisticalRectangle]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDSample] DROP CONSTRAINT [FK_SDSample_L_StatisticalRectangle];
GO
IF OBJECT_ID(N'[dbo].[FK_SDSample_L_Stock]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDSample] DROP CONSTRAINT [FK_SDSample_L_Stock];
GO
IF OBJECT_ID(N'[dbo].[FK_SDSample_Maturity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDSample] DROP CONSTRAINT [FK_SDSample_Maturity];
GO
IF OBJECT_ID(N'[dbo].[FK_SDAnnotation_SDFile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDAnnotation] DROP CONSTRAINT [FK_SDAnnotation_SDFile];
GO
IF OBJECT_ID(N'[dbo].[FK_SDLine_SDAnnotation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDLine] DROP CONSTRAINT [FK_SDLine_SDAnnotation];
GO
IF OBJECT_ID(N'[dbo].[FK_SDPoint_SDAnnotation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDPoint] DROP CONSTRAINT [FK_SDPoint_SDAnnotation];
GO
IF OBJECT_ID(N'[dbo].[FK_SDSample_SDEvent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDSample] DROP CONSTRAINT [FK_SDSample_SDEvent];
GO
IF OBJECT_ID(N'[dbo].[FK_SDFile_SDSample]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDFile] DROP CONSTRAINT [FK_SDFile_SDSample];
GO
IF OBJECT_ID(N'[dbo].[FK_SDSample_L_EdgeStructure]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDSample] DROP CONSTRAINT [FK_SDSample_L_EdgeStructure];
GO
IF OBJECT_ID(N'[dbo].[FK_SDSample_L_OtolithReadingRemark]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDSample] DROP CONSTRAINT [FK_SDSample_L_OtolithReadingRemark];
GO
IF OBJECT_ID(N'[dbo].[FK_SDSample_L_SDLightType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDSample] DROP CONSTRAINT [FK_SDSample_L_SDLightType];
GO
IF OBJECT_ID(N'[dbo].[FK_SDSample_L_SDOtolithDescription]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDSample] DROP CONSTRAINT [FK_SDSample_L_SDOtolithDescription];
GO
IF OBJECT_ID(N'[dbo].[FK_SDSample_L_SDPreparationMethod]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDSample] DROP CONSTRAINT [FK_SDSample_L_SDPreparationMethod];
GO
IF OBJECT_ID(N'[dbo].[FK_SDEvent_L_SDSampleType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDEvent] DROP CONSTRAINT [FK_SDEvent_L_SDSampleType];
GO
IF OBJECT_ID(N'[dbo].[FK_R_StockSpeciesArea_L_DFUArea]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_StockSpeciesArea] DROP CONSTRAINT [FK_R_StockSpeciesArea_L_DFUArea];
GO
IF OBJECT_ID(N'[dbo].[FK_R_StockSpeciesArea_L_Species]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_StockSpeciesArea] DROP CONSTRAINT [FK_R_StockSpeciesArea_L_Species];
GO
IF OBJECT_ID(N'[dbo].[FK_R_StockSpeciesArea_L_StatisticalRectangle]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_StockSpeciesArea] DROP CONSTRAINT [FK_R_StockSpeciesArea_L_StatisticalRectangle];
GO
IF OBJECT_ID(N'[dbo].[FK_R_StockSpeciesArea_L_Stock]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_StockSpeciesArea] DROP CONSTRAINT [FK_R_StockSpeciesArea_L_Stock];
GO
IF OBJECT_ID(N'[dbo].[FK_R_SDEventDFUArea_L_DFUArea]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_SDEventDFUArea] DROP CONSTRAINT [FK_R_SDEventDFUArea_L_DFUArea];
GO
IF OBJECT_ID(N'[dbo].[FK_R_SDEventDFUArea_SDEvent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_SDEventDFUArea] DROP CONSTRAINT [FK_R_SDEventDFUArea_SDEvent];
GO
IF OBJECT_ID(N'[dbo].[FK_R_SDReader_DFUPerson]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_SDReader] DROP CONSTRAINT [FK_R_SDReader_DFUPerson];
GO
IF OBJECT_ID(N'[dbo].[FK_R_SDReader_L_SDPreparationMethod]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_SDReader] DROP CONSTRAINT [FK_R_SDReader_L_SDPreparationMethod];
GO
IF OBJECT_ID(N'[dbo].[FK_R_SDReader_L_SDReaderExperience]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_SDReader] DROP CONSTRAINT [FK_R_SDReader_L_SDReaderExperience];
GO
IF OBJECT_ID(N'[dbo].[FK_R_SDReader_L_Species]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_SDReader] DROP CONSTRAINT [FK_R_SDReader_L_Species];
GO
IF OBJECT_ID(N'[dbo].[FK_R_SDReader_L_Stock]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_SDReader] DROP CONSTRAINT [FK_R_SDReader_L_Stock];
GO
IF OBJECT_ID(N'[dbo].[FK_R_SDEventSDReader_R_SDReader]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_SDEventSDReader] DROP CONSTRAINT [FK_R_SDEventSDReader_R_SDReader];
GO
IF OBJECT_ID(N'[dbo].[FK_R_SDEventSDReader_SDEvent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_SDEventSDReader] DROP CONSTRAINT [FK_R_SDEventSDReader_SDEvent];
GO
IF OBJECT_ID(N'[dbo].[FK_SDAnnotation_L_OtolithReadingRemark]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDAnnotation] DROP CONSTRAINT [FK_SDAnnotation_L_OtolithReadingRemark];
GO
IF OBJECT_ID(N'[dbo].[FK_SDAnnotation_L_SDAnalysisParameter]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDAnnotation] DROP CONSTRAINT [FK_SDAnnotation_L_SDAnalysisParameter];
GO
IF OBJECT_ID(N'[dbo].[FK_SDEvent_L_SDEventType1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDEvent] DROP CONSTRAINT [FK_SDEvent_L_SDEventType1];
GO
IF OBJECT_ID(N'[dbo].[FK_SDSample_L_SDLightType1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDSample] DROP CONSTRAINT [FK_SDSample_L_SDLightType1];
GO
IF OBJECT_ID(N'[dbo].[FK_SDSample_L_SDOtolithDescription1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDSample] DROP CONSTRAINT [FK_SDSample_L_SDOtolithDescription1];
GO
IF OBJECT_ID(N'[dbo].[FK_R_SDReader_L_SDPreparationMethod1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_SDReader] DROP CONSTRAINT [FK_R_SDReader_L_SDPreparationMethod1];
GO
IF OBJECT_ID(N'[dbo].[FK_SDSample_L_SDPreparationMethod1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDSample] DROP CONSTRAINT [FK_SDSample_L_SDPreparationMethod1];
GO
IF OBJECT_ID(N'[dbo].[FK_SDEvent_L_SDPurpose1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDEvent] DROP CONSTRAINT [FK_SDEvent_L_SDPurpose1];
GO
IF OBJECT_ID(N'[dbo].[FK_R_SDReader_L_SDReaderExperience1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[R_SDReader] DROP CONSTRAINT [FK_R_SDReader_L_SDReaderExperience1];
GO
IF OBJECT_ID(N'[dbo].[FK_SDEvent_L_SDSampleType1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDEvent] DROP CONSTRAINT [FK_SDEvent_L_SDSampleType1];
GO
IF OBJECT_ID(N'[dbo].[FK_SDAnnotation_L_EdgeStructure]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDAnnotation] DROP CONSTRAINT [FK_SDAnnotation_L_EdgeStructure];
GO
IF OBJECT_ID(N'[dbo].[FK_SDSample_L_Species]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDSample] DROP CONSTRAINT [FK_SDSample_L_Species];
GO
IF OBJECT_ID(N'[dbo].[FK_Age_SDAnnotation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Age] DROP CONSTRAINT [FK_Age_SDAnnotation];
GO
IF OBJECT_ID(N'[dbo].[FK_Age_DFUPerson]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Age] DROP CONSTRAINT [FK_Age_DFUPerson];
GO
IF OBJECT_ID(N'[dbo].[FK_SDAnnotation_DFUPerson]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDAnnotation] DROP CONSTRAINT [FK_SDAnnotation_DFUPerson];
GO
IF OBJECT_ID(N'[dbo].[FK_SDEvent_DFUPerson]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDEvent] DROP CONSTRAINT [FK_SDEvent_DFUPerson];
GO
IF OBJECT_ID(N'[dbo].[FK_SDLine_DFUPerson]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDLine] DROP CONSTRAINT [FK_SDLine_DFUPerson];
GO
IF OBJECT_ID(N'[dbo].[FK_SDPoint_DFUPerson]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDPoint] DROP CONSTRAINT [FK_SDPoint_DFUPerson];
GO
IF OBJECT_ID(N'[dbo].[FK_SDSample_DFUPerson]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SDSample] DROP CONSTRAINT [FK_SDSample_DFUPerson];
GO
IF OBJECT_ID(N'[dbo].[FK_L_SelectionDeviceSourceSample]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sample] DROP CONSTRAINT [FK_L_SelectionDeviceSourceSample];
GO
IF OBJECT_ID(N'[dbo].[FK_L_LengthMeasureTypeAnimal]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Animal] DROP CONSTRAINT [FK_L_LengthMeasureTypeAnimal];
GO
IF OBJECT_ID(N'[dbo].[FK_L_LengthMeasureTypeL_Species]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[L_Species] DROP CONSTRAINT [FK_L_LengthMeasureTypeL_Species];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[ActivityLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ActivityLog];
GO
IF OBJECT_ID(N'[dbo].[Age]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Age];
GO
IF OBJECT_ID(N'[dbo].[Animal]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Animal];
GO
IF OBJECT_ID(N'[dbo].[AnimalInfo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AnimalInfo];
GO
IF OBJECT_ID(N'[dbo].[Cruise]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Cruise];
GO
IF OBJECT_ID(N'[dbo].[DFUPerson]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DFUPerson];
GO
IF OBJECT_ID(N'[dbo].[DFUSubArea]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DFUSubArea];
GO
IF OBJECT_ID(N'[dbo].[Est_Method]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Est_Method];
GO
IF OBJECT_ID(N'[dbo].[Est_MethodStep]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Est_MethodStep];
GO
IF OBJECT_ID(N'[dbo].[Est_Strata]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Est_Strata];
GO
IF OBJECT_ID(N'[dbo].[Fat]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Fat];
GO
IF OBJECT_ID(N'[dbo].[FishingActivityOnSample]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FishingActivityOnSample];
GO
IF OBJECT_ID(N'[dbo].[L_Activity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_Activity];
GO
IF OBJECT_ID(N'[dbo].[L_AgeMeasureMethod]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_AgeMeasureMethod];
GO
IF OBJECT_ID(N'[dbo].[L_BITS_cruises]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_BITS_cruises];
GO
IF OBJECT_ID(N'[dbo].[L_Bottomtype]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_Bottomtype];
GO
IF OBJECT_ID(N'[dbo].[L_BroodingPhase]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_BroodingPhase];
GO
IF OBJECT_ID(N'[dbo].[L_CatchRegistration]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_CatchRegistration];
GO
IF OBJECT_ID(N'[dbo].[L_CruiseStatus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_CruiseStatus];
GO
IF OBJECT_ID(N'[dbo].[L_CruiseType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_CruiseType];
GO
IF OBJECT_ID(N'[dbo].[L_CuticulaHardness]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_CuticulaHardness];
GO
IF OBJECT_ID(N'[dbo].[L_DFUArea]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_DFUArea];
GO
IF OBJECT_ID(N'[dbo].[L_DFUBase_Category]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_DFUBase_Category];
GO
IF OBJECT_ID(N'[dbo].[L_DFUDepartment]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_DFUDepartment];
GO
IF OBJECT_ID(N'[dbo].[L_FatIndexMethod]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_FatIndexMethod];
GO
IF OBJECT_ID(N'[dbo].[L_Fejlkategorier]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_Fejlkategorier];
GO
IF OBJECT_ID(N'[dbo].[L_FishingActivityCategory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_FishingActivityCategory];
GO
IF OBJECT_ID(N'[dbo].[L_FishingActivityCategory_temp]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_FishingActivityCategory_temp];
GO
IF OBJECT_ID(N'[dbo].[L_FishingActivityNational]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_FishingActivityNational];
GO
IF OBJECT_ID(N'[dbo].[L_Gear]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_Gear];
GO
IF OBJECT_ID(N'[dbo].[L_GearInfoType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_GearInfoType];
GO
IF OBJECT_ID(N'[dbo].[L_GearQuality]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_GearQuality];
GO
IF OBJECT_ID(N'[dbo].[L_GearType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_GearType];
GO
IF OBJECT_ID(N'[dbo].[L_Harbour]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_Harbour];
GO
IF OBJECT_ID(N'[dbo].[L_Harbour2]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_Harbour2];
GO
IF OBJECT_ID(N'[dbo].[L_IBTS_cruises]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_IBTS_cruises];
GO
IF OBJECT_ID(N'[dbo].[L_LandingCategory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_LandingCategory];
GO
IF OBJECT_ID(N'[dbo].[L_LengthMeasureUnit]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_LengthMeasureUnit];
GO
IF OBJECT_ID(N'[dbo].[L_MaturityIndexMethod]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_MaturityIndexMethod];
GO
IF OBJECT_ID(N'[dbo].[L_MissingLookup]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_MissingLookup];
GO
IF OBJECT_ID(N'[dbo].[L_Nationality]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_Nationality];
GO
IF OBJECT_ID(N'[dbo].[L_NavigationSystem]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_NavigationSystem];
GO
IF OBJECT_ID(N'[dbo].[L_OtolithReadingRemark]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_OtolithReadingRemark];
GO
IF OBJECT_ID(N'[dbo].[L_Platform]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_Platform];
GO
IF OBJECT_ID(N'[dbo].[L_PlatformType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_PlatformType];
GO
IF OBJECT_ID(N'[dbo].[L_PlatformVersion]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_PlatformVersion];
GO
IF OBJECT_ID(N'[dbo].[L_SampleStatus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SampleStatus];
GO
IF OBJECT_ID(N'[dbo].[L_SampleType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SampleType];
GO
IF OBJECT_ID(N'[dbo].[L_SamplingMethod]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SamplingMethod];
GO
IF OBJECT_ID(N'[dbo].[L_SamplingType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SamplingType];
GO
IF OBJECT_ID(N'[dbo].[L_SelectPanel]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SelectPanel];
GO
IF OBJECT_ID(N'[dbo].[L_SexCode]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SexCode];
GO
IF OBJECT_ID(N'[dbo].[L_ShovelType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_ShovelType];
GO
IF OBJECT_ID(N'[dbo].[L_SizeSortingDFU]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SizeSortingDFU];
GO
IF OBJECT_ID(N'[dbo].[L_SizeSortingEU]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SizeSortingEU];
GO
IF OBJECT_ID(N'[dbo].[L_Species]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_Species];
GO
IF OBJECT_ID(N'[dbo].[L_SpeciesRegistration]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SpeciesRegistration];
GO
IF OBJECT_ID(N'[dbo].[L_StatisticalRectangle]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_StatisticalRectangle];
GO
IF OBJECT_ID(N'[dbo].[L_Stock]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_Stock];
GO
IF OBJECT_ID(N'[dbo].[L_SubAreaType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SubAreaType];
GO
IF OBJECT_ID(N'[dbo].[L_TreadMaterial]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_TreadMaterial];
GO
IF OBJECT_ID(N'[dbo].[L_TreadType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_TreadType];
GO
IF OBJECT_ID(N'[dbo].[L_Treatment]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_Treatment];
GO
IF OBJECT_ID(N'[dbo].[L_TreatmentFactorGroup]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_TreatmentFactorGroup];
GO
IF OBJECT_ID(N'[dbo].[L_TripType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_TripType];
GO
IF OBJECT_ID(N'[dbo].[L_UsabilityParam]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_UsabilityParam];
GO
IF OBJECT_ID(N'[dbo].[L_UsabilityParamGrp]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_UsabilityParamGrp];
GO
IF OBJECT_ID(N'[dbo].[L_YesNo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_YesNo];
GO
IF OBJECT_ID(N'[dbo].[Maturity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Maturity];
GO
IF OBJECT_ID(N'[dbo].[NumberOfStationsPerTrip]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NumberOfStationsPerTrip];
GO
IF OBJECT_ID(N'[dbo].[Person]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Person];
GO
IF OBJECT_ID(N'[dbo].[Queries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Queries];
GO
IF OBJECT_ID(N'[dbo].[QueryTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[QueryTypes];
GO
IF OBJECT_ID(N'[dbo].[R_CruiseUsabilityParam]', 'U') IS NOT NULL
    DROP TABLE [dbo].[R_CruiseUsabilityParam];
GO
IF OBJECT_ID(N'[dbo].[R_GearInfo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[R_GearInfo];
GO
IF OBJECT_ID(N'[dbo].[R_Maturity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[R_Maturity];
GO
IF OBJECT_ID(N'[dbo].[R_SampleUsabilityParam]', 'U') IS NOT NULL
    DROP TABLE [dbo].[R_SampleUsabilityParam];
GO
IF OBJECT_ID(N'[dbo].[R_TargetSpecies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[R_TargetSpecies];
GO
IF OBJECT_ID(N'[dbo].[R_TripPlatformVersion]', 'U') IS NOT NULL
    DROP TABLE [dbo].[R_TripPlatformVersion];
GO
IF OBJECT_ID(N'[dbo].[R_TripUsabilityParam]', 'U') IS NOT NULL
    DROP TABLE [dbo].[R_TripUsabilityParam];
GO
IF OBJECT_ID(N'[dbo].[R_UsabilityParamUsabilityGrp]', 'U') IS NOT NULL
    DROP TABLE [dbo].[R_UsabilityParamUsabilityGrp];
GO
IF OBJECT_ID(N'[dbo].[replicationLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[replicationLog];
GO
IF OBJECT_ID(N'[dbo].[Sample]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sample];
GO
IF OBJECT_ID(N'[dbo].[SpeciesList]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SpeciesList];
GO
IF OBJECT_ID(N'[dbo].[SubSample]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SubSample];
GO
IF OBJECT_ID(N'[dbo].[Test_Niels]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Test_Niels];
GO
IF OBJECT_ID(N'[dbo].[TrawlOperation]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TrawlOperation];
GO
IF OBJECT_ID(N'[dbo].[TreatmentFactor]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TreatmentFactor];
GO
IF OBJECT_ID(N'[dbo].[Trip]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Trip];
GO
IF OBJECT_ID(N'[dbo].[ICES_DFU_Relation_FF]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ICES_DFU_Relation_FF];
GO
IF OBJECT_ID(N'[dbo].[L_SelectionDevice]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SelectionDevice];
GO
IF OBJECT_ID(N'[dbo].[R_GearTypeSelectionDevice]', 'U') IS NOT NULL
    DROP TABLE [dbo].[R_GearTypeSelectionDevice];
GO
IF OBJECT_ID(N'[dbo].[L_FisheryType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_FisheryType];
GO
IF OBJECT_ID(N'[dbo].[L_HaulType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_HaulType];
GO
IF OBJECT_ID(N'[dbo].[L_ThermoCline]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_ThermoCline];
GO
IF OBJECT_ID(N'[dbo].[L_TimeZone]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_TimeZone];
GO
IF OBJECT_ID(N'[dbo].[L_WeightEstimationMethod]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_WeightEstimationMethod];
GO
IF OBJECT_ID(N'[dbo].[L_EdgeStructure]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_EdgeStructure];
GO
IF OBJECT_ID(N'[dbo].[L_Parasite]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_Parasite];
GO
IF OBJECT_ID(N'[dbo].[L_Reference]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_Reference];
GO
IF OBJECT_ID(N'[dbo].[R_AnimalInfoReference]', 'U') IS NOT NULL
    DROP TABLE [dbo].[R_AnimalInfoReference];
GO
IF OBJECT_ID(N'[dbo].[Report]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Report];
GO
IF OBJECT_ID(N'[dbo].[ReportingTreeNode]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ReportingTreeNode];
GO
IF OBJECT_ID(N'[dbo].[AnimalFile]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AnimalFile];
GO
IF OBJECT_ID(N'[dbo].[L_Applications]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_Applications];
GO
IF OBJECT_ID(N'[dbo].[L_HatchMonthReadability]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_HatchMonthReadability];
GO
IF OBJECT_ID(N'[dbo].[L_VisualStock]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_VisualStock];
GO
IF OBJECT_ID(N'[dbo].[L_GeneticStock]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_GeneticStock];
GO
IF OBJECT_ID(N'[dbo].[L_SDEventType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SDEventType];
GO
IF OBJECT_ID(N'[dbo].[L_SDPurpose]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SDPurpose];
GO
IF OBJECT_ID(N'[dbo].[SDAnnotation]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SDAnnotation];
GO
IF OBJECT_ID(N'[dbo].[SDEvent]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SDEvent];
GO
IF OBJECT_ID(N'[dbo].[SDFile]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SDFile];
GO
IF OBJECT_ID(N'[dbo].[SDLine]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SDLine];
GO
IF OBJECT_ID(N'[dbo].[SDPoint]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SDPoint];
GO
IF OBJECT_ID(N'[dbo].[SDSample]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SDSample];
GO
IF OBJECT_ID(N'[dbo].[L_SDLightType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SDLightType];
GO
IF OBJECT_ID(N'[dbo].[L_SDOtolithDescription]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SDOtolithDescription];
GO
IF OBJECT_ID(N'[dbo].[L_SDPreparationMethod]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SDPreparationMethod];
GO
IF OBJECT_ID(N'[dbo].[L_SDSampleType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SDSampleType];
GO
IF OBJECT_ID(N'[dbo].[R_StockSpeciesArea]', 'U') IS NOT NULL
    DROP TABLE [dbo].[R_StockSpeciesArea];
GO
IF OBJECT_ID(N'[dbo].[L_SDReaderExperience]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SDReaderExperience];
GO
IF OBJECT_ID(N'[dbo].[R_SDEventSDReader]', 'U') IS NOT NULL
    DROP TABLE [dbo].[R_SDEventSDReader];
GO
IF OBJECT_ID(N'[dbo].[R_SDReader]', 'U') IS NOT NULL
    DROP TABLE [dbo].[R_SDReader];
GO
IF OBJECT_ID(N'[dbo].[L_SDAnalysisParameter]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SDAnalysisParameter];
GO
IF OBJECT_ID(N'[dbo].[L_SelectionDeviceSource]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_SelectionDeviceSource];
GO
IF OBJECT_ID(N'[dbo].[L_LengthMeasureType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[L_LengthMeasureType];
GO
IF OBJECT_ID(N'[dbo].[R_ReportingTreeNodeReport]', 'U') IS NOT NULL
    DROP TABLE [dbo].[R_ReportingTreeNodeReport];
GO
IF OBJECT_ID(N'[dbo].[R_SDEventDFUArea]', 'U') IS NOT NULL
    DROP TABLE [dbo].[R_SDEventDFUArea];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'ActivityLog'
CREATE TABLE [dbo].[ActivityLog] (
    [ActivityLogId] int IDENTITY(1,1) NOT NULL,
    [Time] datetime  NOT NULL,
    [UserName] nvarchar(10)  NOT NULL,
    [Activity] nvarchar(10)  NOT NULL,
    [DataTable] nvarchar(50)  NOT NULL,
    [DataNaturalKey] nvarchar(200)  NOT NULL,
    [Remarks] nvarchar(80)  NULL
);
GO

-- Creating table 'Age'
CREATE TABLE [dbo].[Age] (
    [ageId] int IDENTITY(1,1) NOT NULL,
    [animalId] int  NOT NULL,
    [age1] int  NULL,
    [otolithReadingRemarkId] int  NULL,
    [number] int  NOT NULL,
    [ageMeasureMethodId] int  NOT NULL,
    [hatchMonth] int  NULL,
    [otolithWeight] decimal(7,5)  NULL,
    [remark] nvarchar(max)  NULL,
    [edgeStructure] nvarchar(5)  NULL,
    [genetics] bit  NULL,
    [hatchMonthReadabilityId] int  NULL,
    [visualStockId] int  NULL,
    [geneticStockId] int  NULL,
    [sdAgeInfoUpdated] bit  NULL,
    [sdAnnotationId] int  NULL,
    [sdAgeReadId] int  NULL
);
GO

-- Creating table 'Animal'
CREATE TABLE [dbo].[Animal] (
    [animalId] int IDENTITY(1,1) NOT NULL,
    [subSampleId] int  NOT NULL,
    [individNum] int  NULL,
    [length] int  NULL,
    [lengthMeasureUnit] nvarchar(2)  NULL,
    [lengthMeasureTypeId] int  NULL,
    [sexCode] nvarchar(1)  NULL,
    [broodingPhase] nvarchar(4)  NULL,
    [dfuBase_lfRecordId] int  NULL,
    [number] int  NOT NULL,
    [weight] decimal(10,5)  NULL,
    [dataHandlerId] int  NULL,
    [remark] nvarchar(max)  NULL,
    [catchNum] int  NULL,
    [otolithFinScale] bit  NULL
);
GO

-- Creating table 'AnimalInfo'
CREATE TABLE [dbo].[AnimalInfo] (
    [animalInfoId] int IDENTITY(1,1) NOT NULL,
    [animalId] int  NOT NULL,
    [heigth] int  NULL,
    [width] int  NULL,
    [maturityId] int  NULL,
    [I1] int  NULL,
    [fatId] int  NULL,
    [numVertebra] int  NULL,
    [weightGutted] decimal(10,5)  NULL,
    [weightLiver] decimal(9,5)  NULL,
    [weightGonads] decimal(9,5)  NULL,
    [pictureReference] nvarchar(80)  NULL,
    [netPlaceVertical] nvarchar(1)  NULL,
    [netPlaceHorisontal] nvarchar(1)  NULL,
    [remark] nvarchar(max)  NULL,
    [parasiteId] int  NULL
);
GO

-- Creating table 'Cruise'
CREATE TABLE [dbo].[Cruise] (
    [cruiseId] int IDENTITY(1,1) NOT NULL,
    [year] int  NOT NULL,
    [cruise1] nvarchar(20)  NOT NULL,
    [cruiseTitle] nvarchar(30)  NULL,
    [responsibleId] int  NULL,
    [participants] nvarchar(256)  NULL,
    [reportFile] nvarchar(35)  NULL,
    [summary] nvarchar(1024)  NULL,
    [DFUDepartment] nvarchar(3)  NULL,
    [cruiseStatus] nvarchar(1)  NULL,
    [dateEstimCruise] datetime  NULL,
    [dateControl] datetime  NULL,
    [dateUpdate] datetime  NULL,
    [dataHandlerId] int  NULL,
    [remark] nvarchar(max)  NULL
);
GO

-- Creating table 'DFUPerson'
CREATE TABLE [dbo].[DFUPerson] (
    [dfuPersonId] int IDENTITY(1,1) NOT NULL,
    [initials] nvarchar(10)  NOT NULL,
    [dfuDepartment] nvarchar(3)  NULL,
    [name] nvarchar(80)  NOT NULL
);
GO

-- Creating table 'DFUSubArea'
CREATE TABLE [dbo].[DFUSubArea] (
    [dfuSubAreaId] int IDENTITY(1,1) NOT NULL,
    [DFUArea] nvarchar(3)  NOT NULL,
    [northDefinitionLatitudeText] nchar(12)  NOT NULL,
    [southDefinitionLatitudeText] nchar(12)  NOT NULL,
    [westDefinitionLongitudeText] nchar(13)  NOT NULL,
    [eastDefinitionLongitudeText] nchar(13)  NOT NULL,
    [SubAreaType] nvarchar(11)  NOT NULL,
    [northDefinitionLatitudeDec] decimal(9,6)  NOT NULL,
    [southDefinitionLatitudeDec] decimal(9,6)  NOT NULL,
    [westDefinitionLongitudeDec] decimal(9,6)  NOT NULL,
    [eastDefinitionLongitudeDec] decimal(9,6)  NOT NULL
);
GO

-- Creating table 'Est_Method'
CREATE TABLE [dbo].[Est_Method] (
    [Est_MethodId] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(20)  NOT NULL,
    [cruiseId] int  NOT NULL
);
GO

-- Creating table 'Est_MethodStep'
CREATE TABLE [dbo].[Est_MethodStep] (
    [Est_MethodStepId] int IDENTITY(1,1) NOT NULL,
    [Est_MethodId] int  NOT NULL,
    [stepNumber] int  NOT NULL,
    [Est_StrataId] int  NULL,
    [overwrites] nvarchar(3)  NOT NULL,
    [SpeciesCode] nvarchar(3)  NULL,
    [dfubase_category] nvarchar(3)  NULL,
    [sizeSortingEU] int  NULL,
    [sampleId] int  NULL,
    [tripId] int  NULL,
    [datahandlerId] int  NOT NULL,
    [disabled] nvarchar(3)  NOT NULL
);
GO

-- Creating table 'Est_Strata'
CREATE TABLE [dbo].[Est_Strata] (
    [Est_StrataId] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(20)  NOT NULL,
    [cruiseId] int  NOT NULL,
    [TripId] int  NULL,
    [SampleId] int  NULL,
    [dfubase_category] nvarchar(3)  NULL,
    [sizeSortingEU] int  NULL,
    [representative] nvarchar(3)  NOT NULL,
    [statisticalRectangle] nchar(4)  NULL,
    [disabled] nvarchar(3)  NOT NULL,
    [datahandlerId] int  NOT NULL,
    [delete_CruiseId] int  NOT NULL
);
GO

-- Creating table 'Fat'
CREATE TABLE [dbo].[Fat] (
    [fatId] int IDENTITY(1,1) NOT NULL,
    [fatIndex] nvarchar(3)  NOT NULL,
    [fatIndexMethod] nvarchar(1)  NOT NULL,
    [description] nvarchar(80)  NULL
);
GO

-- Creating table 'FishingActivityOnSample'
CREATE TABLE [dbo].[FishingActivityOnSample] (
    [fishingActivityId] int IDENTITY(1,1) NOT NULL,
    [sampleId] int  NULL,
    [fishingActivity] nvarchar(50)  NULL
);
GO

-- Creating table 'L_Activity'
CREATE TABLE [dbo].[L_Activity] (
    [L_activityid] int IDENTITY(1,1) NOT NULL,
    [activity] nvarchar(10)  NOT NULL,
    [description] nvarchar(80)  NOT NULL
);
GO

-- Creating table 'L_AgeMeasureMethod'
CREATE TABLE [dbo].[L_AgeMeasureMethod] (
    [L_ageMeasureMethodId] int IDENTITY(1,1) NOT NULL,
    [ageMeasureMethod] nvarchar(20)  NOT NULL,
    [description] nvarchar(80)  NULL
);
GO

-- Creating table 'L_BITS_cruises'
CREATE TABLE [dbo].[L_BITS_cruises] (
    [cruiseID] int  NOT NULL,
    [shipCode] char(4)  NOT NULL,
    [cruiseYear] int  NOT NULL,
    [cruiseNo] int  NOT NULL,
    [cruiseDays] smallint  NULL,
    [cruiseName] varchar(50)  NULL,
    [projectType] varchar(50)  NULL,
    [cruiseLeader] varchar(50)  NULL,
    [assistCruiseLeader] varchar(50)  NULL,
    [captain] varchar(50)  NULL,
    [technician] varchar(50)  NULL,
    [cruiseArea] varchar(50)  NULL,
    [startDate] datetime  NULL,
    [endDate] datetime  NULL,
    [institution] varchar(25)  NULL,
    [institute] varchar(25)  NULL,
    [projectArea] varchar(10)  NULL,
    [projectNo] varchar(20)  NULL,
    [remarks] varchar(50)  NULL,
    [status] bit  NOT NULL
);
GO

-- Creating table 'L_Bottomtype'
CREATE TABLE [dbo].[L_Bottomtype] (
    [L_bottomTypeId] int IDENTITY(1,1) NOT NULL,
    [bottomtype] nvarchar(3)  NOT NULL,
    [description] nvarchar(80)  NOT NULL
);
GO

-- Creating table 'L_BroodingPhase'
CREATE TABLE [dbo].[L_BroodingPhase] (
    [L_broodingPhaseId] int IDENTITY(1,1) NOT NULL,
    [broodingPhase] nvarchar(4)  NOT NULL,
    [description] nvarchar(80)  NOT NULL,
    [num] int  NULL
);
GO

-- Creating table 'L_CatchRegistration'
CREATE TABLE [dbo].[L_CatchRegistration] (
    [catchRegistrationId] int IDENTITY(1,1) NOT NULL,
    [catchRegistration] nvarchar(3)  NOT NULL,
    [description] nvarchar(50)  NULL,
    [num] int  NULL
);
GO

-- Creating table 'L_CruiseStatus'
CREATE TABLE [dbo].[L_CruiseStatus] (
    [L_cruiseStatusId] int  NOT NULL,
    [cruiseStatus] nvarchar(1)  NOT NULL,
    [description] nvarchar(80)  NOT NULL
);
GO

-- Creating table 'L_CruiseType'
CREATE TABLE [dbo].[L_CruiseType] (
    [L_cruiseTypeId] int IDENTITY(1,1) NOT NULL,
    [cruiseType] nvarchar(3)  NOT NULL,
    [description] nvarchar(80)  NOT NULL
);
GO

-- Creating table 'L_CuticulaHardness'
CREATE TABLE [dbo].[L_CuticulaHardness] (
    [L_cuticulaHardnessId] int IDENTITY(1,1) NOT NULL,
    [cuticulaHardness] nvarchar(4)  NOT NULL,
    [description] nvarchar(80)  NOT NULL
);
GO

-- Creating table 'L_DFUArea'
CREATE TABLE [dbo].[L_DFUArea] (
    [L_DFUAreaId] int IDENTITY(1,1) NOT NULL,
    [DFUArea] nvarchar(3)  NOT NULL,
    [description] nvarchar(80)  NOT NULL,
    [parentDFUArea] nvarchar(3)  NULL,
    [areaNES] nvarchar(10)  NULL,
    [areaICES] nvarchar(20)  NULL
);
GO

-- Creating table 'L_DFUBase_Category'
CREATE TABLE [dbo].[L_DFUBase_Category] (
    [L_dfuBase_CategoryId] int IDENTITY(1,1) NOT NULL,
    [dfuBase_Category] nvarchar(3)  NOT NULL,
    [description] nvarchar(70)  NULL
);
GO

-- Creating table 'L_DFUDepartment'
CREATE TABLE [dbo].[L_DFUDepartment] (
    [L_dfuDepartmentId] int IDENTITY(1,1) NOT NULL,
    [dfuDepartment] nvarchar(3)  NOT NULL,
    [description] nvarchar(80)  NOT NULL
);
GO

-- Creating table 'L_FatIndexMethod'
CREATE TABLE [dbo].[L_FatIndexMethod] (
    [L_fatIndexMethodId] int IDENTITY(1,1) NOT NULL,
    [fatIndexMethod] nvarchar(1)  NOT NULL,
    [description] nvarchar(30)  NOT NULL
);
GO

-- Creating table 'L_Fejlkategorier'
CREATE TABLE [dbo].[L_Fejlkategorier] (
    [FejlkategoriId] int IDENTITY(1,1) NOT NULL,
    [FejlkategoriTekst] nvarchar(1000)  NULL
);
GO

-- Creating table 'L_FishingActivityCategory'
CREATE TABLE [dbo].[L_FishingActivityCategory] (
    [Id] int  NOT NULL,
    [Code] nvarchar(50)  NOT NULL,
    [Description] nvarchar(255)  NOT NULL,
    [FishingActivityLevel] int  NOT NULL,
    [ParentFishingActivityCategory] int  NOT NULL,
    [FishingActivityCategoryType] int  NOT NULL
);
GO

-- Creating table 'L_FishingActivityCategory_temp'
CREATE TABLE [dbo].[L_FishingActivityCategory_temp] (
    [Id] int  NOT NULL,
    [Code] nvarchar(50)  NOT NULL,
    [Description] nvarchar(255)  NOT NULL,
    [FishingActivityLevel] int  NOT NULL,
    [ParentFishingActivityCategory] int  NOT NULL,
    [FishingActivityCategoryType] int  NOT NULL
);
GO

-- Creating table 'L_FishingActivityNational'
CREATE TABLE [dbo].[L_FishingActivityNational] (
    [fishingActivityId] int IDENTITY(1,1) NOT NULL,
    [fishingActivity] nvarchar(50)  NOT NULL,
    [description] nvarchar(50)  NULL
);
GO

-- Creating table 'L_Gear'
CREATE TABLE [dbo].[L_Gear] (
    [gearId] int IDENTITY(1,1) NOT NULL,
    [platform] nvarchar(20)  NOT NULL,
    [gear] nvarchar(80)  NOT NULL,
    [gearText] nvarchar(80)  NULL,
    [version] int  NOT NULL,
    [gearType] nvarchar(50)  NOT NULL,
    [description] nvarchar(160)  NULL
);
GO

-- Creating table 'L_GearInfoType'
CREATE TABLE [dbo].[L_GearInfoType] (
    [L_gearInfoTypeId] int IDENTITY(1,1) NOT NULL,
    [gearInfoType] nvarchar(20)  NOT NULL,
    [description] nvarchar(80)  NOT NULL,
    [dfuBase_dfuref] nvarchar(50)  NULL,
    [unit] nvarchar(50)  NULL
);
GO

-- Creating table 'L_GearQuality'
CREATE TABLE [dbo].[L_GearQuality] (
    [L_gearQualityId] int IDENTITY(1,1) NOT NULL,
    [gearQuality] nvarchar(1)  NOT NULL,
    [description] nvarchar(80)  NOT NULL,
    [num] int  NULL
);
GO

-- Creating table 'L_GearType'
CREATE TABLE [dbo].[L_GearType] (
    [L_gearTypeId] int IDENTITY(1,1) NOT NULL,
    [gearType] nvarchar(50)  NOT NULL,
    [description] nvarchar(80)  NOT NULL,
    [fmCode] int  NULL,
    [logbookCode] nvarchar(50)  NULL,
    [catchOperation] nvarchar(1)  NULL,
    [showInVidUI] bit  NOT NULL,
    [showInSeaHvnUI] bit  NOT NULL
);
GO

-- Creating table 'L_Harbour'
CREATE TABLE [dbo].[L_Harbour] (
    [L_harbourId] int IDENTITY(1,1) NOT NULL,
    [harbour] nvarchar(4)  NOT NULL,
    [description] nvarchar(80)  NULL,
    [nationality] nvarchar(3)  NULL,
    [harbourNES] nvarchar(8)  NULL,
    [harbourEU] nvarchar(8)  NULL
);
GO

-- Creating table 'L_Harbour2'
CREATE TABLE [dbo].[L_Harbour2] (
    [L_harbourId] int IDENTITY(1,1) NOT NULL,
    [harbour] nvarchar(4)  NOT NULL,
    [description] nvarchar(80)  NULL
);
GO

-- Creating table 'L_IBTS_cruises'
CREATE TABLE [dbo].[L_IBTS_cruises] (
    [cruiseID] int  NOT NULL,
    [shipCode] char(4)  NOT NULL,
    [cruiseYear] int  NOT NULL,
    [cruiseNo] int  NOT NULL,
    [cruiseDays] smallint  NULL,
    [cruiseName] varchar(50)  NULL,
    [projectType] varchar(50)  NULL,
    [cruiseLeader] varchar(50)  NULL,
    [assistCruiseLeader] varchar(50)  NULL,
    [captain] varchar(50)  NULL,
    [technician] varchar(50)  NULL,
    [cruiseArea] varchar(50)  NULL,
    [startDate] datetime  NULL,
    [endDate] datetime  NULL,
    [institution] varchar(25)  NULL,
    [institute] varchar(25)  NULL,
    [projectArea] varchar(10)  NULL,
    [projectNo] varchar(20)  NULL,
    [remarks] varchar(50)  NULL,
    [status] bit  NOT NULL
);
GO

-- Creating table 'L_LandingCategory'
CREATE TABLE [dbo].[L_LandingCategory] (
    [L_landingCategoryId] int IDENTITY(1,1) NOT NULL,
    [landingCategory] nvarchar(3)  NOT NULL,
    [description] nvarchar(70)  NULL,
    [num] int  NULL
);
GO

-- Creating table 'L_LengthMeasureUnit'
CREATE TABLE [dbo].[L_LengthMeasureUnit] (
    [L_lengthMeasureUnitId] int IDENTITY(1,1) NOT NULL,
    [lengthMeasureUnit] nvarchar(2)  NOT NULL,
    [description] nvarchar(80)  NULL
);
GO

-- Creating table 'L_MaturityIndexMethod'
CREATE TABLE [dbo].[L_MaturityIndexMethod] (
    [L_maturityIndexMethodId] int IDENTITY(1,1) NOT NULL,
    [maturityIndexMethod] nvarchar(1)  NOT NULL,
    [description] nvarchar(80)  NOT NULL
);
GO

-- Creating table 'L_MissingLookup'
CREATE TABLE [dbo].[L_MissingLookup] (
    [dataTable] nvarchar(50)  NOT NULL,
    [dataField] nvarchar(50)  NOT NULL,
    [dataValue] nvarchar(160)  NOT NULL,
    [keydata] nvarchar(160)  NOT NULL,
    [cruiseId] int  NULL,
    [tripId] int  NULL,
    [sampleId] int  NULL,
    [specieslistId] int  NULL,
    [subsampleId] int  NULL,
    [animalId] int  NULL,
    [ageId] int  NULL,
    [LFrecordId] int  NULL,
    [artsrecordId] int  NULL,
    [errorType] nvarchar(20)  NULL
);
GO

-- Creating table 'L_Nationality'
CREATE TABLE [dbo].[L_Nationality] (
    [L_nationalityId] int IDENTITY(1,1) NOT NULL,
    [nationality] nvarchar(3)  NOT NULL,
    [description] nvarchar(15)  NOT NULL
);
GO

-- Creating table 'L_NavigationSystem'
CREATE TABLE [dbo].[L_NavigationSystem] (
    [L_navigationSystemId] int IDENTITY(1,1) NOT NULL,
    [navigationSystem] nvarchar(3)  NOT NULL,
    [description] nvarchar(80)  NOT NULL
);
GO

-- Creating table 'L_OtolithReadingRemark'
CREATE TABLE [dbo].[L_OtolithReadingRemark] (
    [L_OtolithReadingRemarkID] int IDENTITY(1,1) NOT NULL,
    [otolithReadingRemark] nvarchar(50)  NOT NULL,
    [description] nvarchar(250)  NULL,
    [num] int  NULL,
    [transAgeFromAquaDotsToFishLine] bit  NOT NULL
);
GO

-- Creating table 'L_Platform'
CREATE TABLE [dbo].[L_Platform] (
    [L_platformId] int IDENTITY(1,1) NOT NULL,
    [platform] nvarchar(20)  NOT NULL,
    [platformType] nvarchar(2)  NULL,
    [name] nvarchar(30)  NULL,
    [nationality] nvarchar(3)  NULL,
    [boatIdentity] nvarchar(20)  NULL,
    [contactPersonId] int  NULL,
    [description] nvarchar(80)  NULL
);
GO

-- Creating table 'L_PlatformType'
CREATE TABLE [dbo].[L_PlatformType] (
    [L_platformTypeId] int IDENTITY(1,1) NOT NULL,
    [platformType] nvarchar(2)  NOT NULL,
    [description] nvarchar(80)  NOT NULL
);
GO

-- Creating table 'L_PlatformVersion'
CREATE TABLE [dbo].[L_PlatformVersion] (
    [platformVersionId] int IDENTITY(1,1) NOT NULL,
    [platform] nvarchar(20)  NOT NULL,
    [version] int  NOT NULL,
    [revisionYear] int  NULL,
    [navigationSystem] nvarchar(3)  NULL,
    [registerTons] int  NULL,
    [length] decimal(6,2)  NULL,
    [power] int  NULL,
    [crew] int  NULL,
    [radiosignal] nvarchar(50)  NULL,
    [description] nvarchar(240)  NULL
);
GO

-- Creating table 'L_SampleStatus'
CREATE TABLE [dbo].[L_SampleStatus] (
    [L_sampleStatusId] int IDENTITY(1,1) NOT NULL,
    [sampleStatus] nvarchar(1)  NOT NULL,
    [description] nvarchar(80)  NOT NULL
);
GO

-- Creating table 'L_SampleType'
CREATE TABLE [dbo].[L_SampleType] (
    [L_sampleTypeId] int IDENTITY(1,1) NOT NULL,
    [sampleType] nvarchar(1)  NOT NULL,
    [description] nvarchar(80)  NOT NULL
);
GO

-- Creating table 'L_SamplingMethod'
CREATE TABLE [dbo].[L_SamplingMethod] (
    [samplingMethodId] int IDENTITY(1,1) NOT NULL,
    [samplingMethod] nvarchar(12)  NOT NULL,
    [description] nvarchar(50)  NULL,
    [num] int  NULL
);
GO

-- Creating table 'L_SamplingType'
CREATE TABLE [dbo].[L_SamplingType] (
    [samplingTypeId] int IDENTITY(1,1) NOT NULL,
    [samplingType] nvarchar(1)  NOT NULL,
    [description] nvarchar(50)  NULL,
    [num] int  NULL
);
GO

-- Creating table 'L_SelectPanel'
CREATE TABLE [dbo].[L_SelectPanel] (
    [L_selectPanelId] int IDENTITY(1,1) NOT NULL,
    [selectPanel] nvarchar(50)  NOT NULL,
    [description] nvarchar(80)  NOT NULL
);
GO

-- Creating table 'L_SexCode'
CREATE TABLE [dbo].[L_SexCode] (
    [L_sexCodeId] int IDENTITY(1,1) NOT NULL,
    [sexCode] nvarchar(1)  NOT NULL,
    [description] nvarchar(15)  NOT NULL,
    [num] int  NULL
);
GO

-- Creating table 'L_ShovelType'
CREATE TABLE [dbo].[L_ShovelType] (
    [L_shovelTypeId] int IDENTITY(1,1) NOT NULL,
    [shovelType] nvarchar(50)  NOT NULL,
    [description] nvarchar(80)  NOT NULL
);
GO

-- Creating table 'L_SizeSortingDFU'
CREATE TABLE [dbo].[L_SizeSortingDFU] (
    [L_sizeSortingDFUId] int IDENTITY(1,1) NOT NULL,
    [sizeSortingDFU] nvarchar(3)  NOT NULL,
    [description] nvarchar(80)  NULL,
    [num] int  NULL
);
GO

-- Creating table 'L_SizeSortingEU'
CREATE TABLE [dbo].[L_SizeSortingEU] (
    [L_sizeSortingEUId] int IDENTITY(1,1) NOT NULL,
    [sizeSortingEU] int  NOT NULL,
    [description] nvarchar(80)  NULL
);
GO

-- Creating table 'L_Species'
CREATE TABLE [dbo].[L_Species] (
    [L_speciesId] int IDENTITY(1,1) NOT NULL,
    [speciesCode] nvarchar(3)  NOT NULL,
    [dkName] nvarchar(80)  NULL,
    [ukName] nvarchar(80)  NULL,
    [nodc] nvarchar(80)  NULL,
    [latin] nvarchar(80)  NULL,
    [icesCode] nvarchar(3)  NULL,
    [treatmentFactorGroup] nvarchar(3)  NULL,
    [dfuFisk_Code] nvarchar(3)  NULL,
    [tsn] nvarchar(6)  NULL,
    [aphiaID] int  NULL,
    [standardLengthMeasureTypeId] int  NULL,
    [lengthMin] int  NULL,
    [lengthMax] int  NULL,
    [ageMin] int  NULL,
    [ageMax] int  NULL,
    [weightMin] int  NULL,
    [weightMax] int  NULL,
    [speciesNES] nvarchar(3)  NULL,
    [speciesFAO] nvarchar(3)  NULL
);
GO

-- Creating table 'L_SpeciesRegistration'
CREATE TABLE [dbo].[L_SpeciesRegistration] (
    [speciesRegistrationId] int IDENTITY(1,1) NOT NULL,
    [speciesRegistration] nvarchar(3)  NOT NULL,
    [description] nvarchar(50)  NULL,
    [num] int  NULL
);
GO

-- Creating table 'L_StatisticalRectangle'
CREATE TABLE [dbo].[L_StatisticalRectangle] (
    [L_statisticalRectangleId] int IDENTITY(1,1) NOT NULL,
    [statisticalRectangle] nchar(4)  NOT NULL,
    [latitudeDecMin] decimal(9,6)  NULL,
    [latitudeDecMax] decimal(9,6)  NULL,
    [longitudeDecMin] decimal(9,6)  NULL,
    [longitudeDecMax] decimal(9,6)  NULL,
    [latitudeDecMid] decimal(9,6)  NULL,
    [longitudeDecMid] decimal(9,6)  NULL,
    [latitudeTextMin] nchar(12)  NULL,
    [latitudeTextMax] nchar(12)  NULL,
    [longitudeTextMin] nchar(13)  NULL,
    [longitudeTextMax] nchar(13)  NULL,
    [latitudeTextMid] nchar(12)  NULL,
    [longitudeTextMid] nchar(13)  NULL
);
GO

-- Creating table 'L_Stock'
CREATE TABLE [dbo].[L_Stock] (
    [L_stockId] int IDENTITY(1,1) NOT NULL,
    [stockCode] nvarchar(50)  NOT NULL,
    [description] nvarchar(1000)  NOT NULL
);
GO

-- Creating table 'L_SubAreaType'
CREATE TABLE [dbo].[L_SubAreaType] (
    [L_SubAreaTypeId] int IDENTITY(1,1) NOT NULL,
    [SubAreaType] nvarchar(11)  NOT NULL,
    [description] nvarchar(80)  NULL
);
GO

-- Creating table 'L_TreadMaterial'
CREATE TABLE [dbo].[L_TreadMaterial] (
    [L_treadMaterialId] int IDENTITY(1,1) NOT NULL,
    [treadMaterial] nvarchar(50)  NOT NULL,
    [description] nvarchar(80)  NOT NULL
);
GO

-- Creating table 'L_TreadType'
CREATE TABLE [dbo].[L_TreadType] (
    [L_treadTypeId] int IDENTITY(1,1) NOT NULL,
    [treadType] nvarchar(50)  NOT NULL,
    [description] nvarchar(80)  NOT NULL
);
GO

-- Creating table 'L_Treatment'
CREATE TABLE [dbo].[L_Treatment] (
    [L_treatmentId] int IDENTITY(1,1) NOT NULL,
    [treatment] nvarchar(2)  NOT NULL,
    [description] nvarchar(80)  NOT NULL,
    [num] int  NULL
);
GO

-- Creating table 'L_TreatmentFactorGroup'
CREATE TABLE [dbo].[L_TreatmentFactorGroup] (
    [L_treatmentFactorGroupId] int IDENTITY(1,1) NOT NULL,
    [treatmentFactorGroup] nvarchar(3)  NOT NULL,
    [description] nvarchar(80)  NOT NULL
);
GO

-- Creating table 'L_TripType'
CREATE TABLE [dbo].[L_TripType] (
    [L_tripTypeId] int IDENTITY(1,1) NOT NULL,
    [tripType] nvarchar(6)  NOT NULL,
    [description] nvarchar(80)  NOT NULL,
    [cruiseType] nvarchar(3)  NOT NULL
);
GO

-- Creating table 'L_UsabilityParam'
CREATE TABLE [dbo].[L_UsabilityParam] (
    [usabilityParamId] int IDENTITY(1,1) NOT NULL,
    [usabilityParam] nvarchar(100)  NULL
);
GO

-- Creating table 'L_UsabilityParamGrp'
CREATE TABLE [dbo].[L_UsabilityParamGrp] (
    [usabilityParamGrpId] int IDENTITY(1,1) NOT NULL,
    [usabilityGrp] nvarchar(10)  NOT NULL
);
GO

-- Creating table 'L_YesNo'
CREATE TABLE [dbo].[L_YesNo] (
    [L_YesNoId] int IDENTITY(1,1) NOT NULL,
    [YesNo] nvarchar(3)  NOT NULL
);
GO

-- Creating table 'Maturity'
CREATE TABLE [dbo].[Maturity] (
    [maturityId] int IDENTITY(1,1) NOT NULL,
    [maturityIndex] int  NOT NULL,
    [maturityIndexMethod] nvarchar(1)  NOT NULL,
    [description] nvarchar(80)  NULL
);
GO

-- Creating table 'NumberOfStationsPerTrip'
CREATE TABLE [dbo].[NumberOfStationsPerTrip] (
    [numberOfStationsPerTripId] int IDENTITY(1,1) NOT NULL,
    [tripId] int  NULL,
    [numStnPerTrip] int  NULL
);
GO

-- Creating table 'Person'
CREATE TABLE [dbo].[Person] (
    [personId] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(80)  NOT NULL,
    [organization] nvarchar(60)  NULL,
    [address] nvarchar(60)  NULL,
    [zipTown] nvarchar(30)  NULL,
    [telephone] nvarchar(30)  NULL,
    [SEno] nvarchar(15)  NULL,
    [bankAccountNo] nvarchar(15)  NULL,
    [telephonePrivate] nvarchar(30)  NULL,
    [telephoneMobile] nvarchar(30)  NULL,
    [email] nvarchar(150)  NULL,
    [facebook] nvarchar(150)  NULL
);
GO

-- Creating table 'Queries'
CREATE TABLE [dbo].[Queries] (
    [usercode] nvarchar(50)  NOT NULL,
    [queryName] nvarchar(50)  NOT NULL,
    [querySQL] nvarchar(512)  NOT NULL
);
GO

-- Creating table 'QueryTypes'
CREATE TABLE [dbo].[QueryTypes] (
    [QueryTypeID] int IDENTITY(1,1) NOT NULL,
    [tblAlias] nvarchar(50)  NULL,
    [tblName] nvarchar(50)  NULL,
    [selectType] nvarchar(50)  NULL,
    [selectWhat] nvarchar(50)  NULL,
    [whereClause] nvarchar(50)  NULL
);
GO

-- Creating table 'R_CruiseUsabilityParam'
CREATE TABLE [dbo].[R_CruiseUsabilityParam] (
    [R_CruiseUsbilityParamId] int IDENTITY(1,1) NOT NULL,
    [cruiseId] int  NOT NULL,
    [usabilityParamId] int  NOT NULL
);
GO

-- Creating table 'R_GearInfo'
CREATE TABLE [dbo].[R_GearInfo] (
    [R_gearInfoID] int IDENTITY(1,1) NOT NULL,
    [gearId] int  NOT NULL,
    [gearInfoType] nvarchar(20)  NOT NULL,
    [gearValue] nvarchar(80)  NOT NULL
);
GO

-- Creating table 'R_Maturity'
CREATE TABLE [dbo].[R_Maturity] (
    [r_MaturityId] int IDENTITY(1,1) NOT NULL,
    [maturityId_PK] int  NOT NULL,
    [maturityId_FK] int  NOT NULL,
    [description] nvarchar(80)  NULL
);
GO

-- Creating table 'R_SampleUsabilityParam'
CREATE TABLE [dbo].[R_SampleUsabilityParam] (
    [R_SampleUsabilityParamId] int IDENTITY(1,1) NOT NULL,
    [SampleId] int  NOT NULL,
    [usabilityParamId] int  NOT NULL
);
GO

-- Creating table 'R_TargetSpecies'
CREATE TABLE [dbo].[R_TargetSpecies] (
    [TargetSpeciesId] int IDENTITY(1,1) NOT NULL,
    [sampleId] int  NOT NULL,
    [speciesCode] nvarchar(3)  NOT NULL
);
GO

-- Creating table 'R_TripPlatformVersion'
CREATE TABLE [dbo].[R_TripPlatformVersion] (
    [R_TripPlatformVersionId] int IDENTITY(1,1) NOT NULL,
    [tripId] int  NOT NULL,
    [platformVersionId] int  NOT NULL
);
GO

-- Creating table 'R_TripUsabilityParam'
CREATE TABLE [dbo].[R_TripUsabilityParam] (
    [R_tripUsabilityParamId] int IDENTITY(1,1) NOT NULL,
    [tripId] int  NOT NULL,
    [usabilityParamId] int  NOT NULL
);
GO

-- Creating table 'R_UsabilityParamUsabilityGrp'
CREATE TABLE [dbo].[R_UsabilityParamUsabilityGrp] (
    [R_UsabilityParamUsabilityGrpId] int IDENTITY(1,1) NOT NULL,
    [usabilityParamId] int  NOT NULL,
    [usabilityParamGrpId] int  NOT NULL
);
GO

-- Creating table 'replicationLog'
CREATE TABLE [dbo].[replicationLog] (
    [replicationLogId] int IDENTITY(1,1) NOT NULL,
    [logType] nvarchar(50)  NOT NULL,
    [time] datetime  NOT NULL,
    [source] nvarchar(50)  NOT NULL,
    [systemText] nvarchar(500)  NOT NULL,
    [userText] nvarchar(500)  NOT NULL,
    [year] int  NULL,
    [cruise] nvarchar(50)  NULL,
    [trip] nvarchar(50)  NULL,
    [station] nvarchar(50)  NULL,
    [species] nvarchar(50)  NULL
);
GO

-- Creating table 'Sample'
CREATE TABLE [dbo].[Sample] (
    [sampleId] int IDENTITY(1,1) NOT NULL,
    [tripId] int  NOT NULL,
    [station] nvarchar(6)  NOT NULL,
    [stationName] nvarchar(10)  NULL,
    [virtual] nvarchar(3)  NOT NULL,
    [gearId] int  NULL,
    [sampleType] nvarchar(1)  NOT NULL,
    [dateGearStart] datetime  NOT NULL,
    [dateGearEnd] datetime  NOT NULL,
    [timeZone] int  NULL,
    [fishingtime] int  NULL,
    [night] nvarchar(3)  NULL,
    [latPosStartText] nchar(12)  NULL,
    [lonPosStartText] nchar(13)  NULL,
    [latPosEndText] nchar(12)  NULL,
    [lonPosEndText] nchar(13)  NULL,
    [fishingActivityId] int  NULL,
    [fishingActivityNationalId] int  NULL,
    [catchRegistrationId] int  NULL,
    [speciesRegistrationId] int  NULL,
    [HVN_geartype] nvarchar(50)  NULL,
    [distance] decimal(6,3)  NULL,
    [dfuArea] nvarchar(3)  NULL,
    [statisticalRectangle] nchar(4)  NULL,
    [depthAvg] decimal(5,1)  NULL,
    [bottomType] nvarchar(3)  NULL,
    [windDirection] smallint  NULL,
    [windSpeed] int  NULL,
    [currentDirectionSrf] smallint  NULL,
    [currentSpeedSrf] decimal(4,1)  NULL,
    [currentDirectionBot] smallint  NULL,
    [currentSpeedBot] decimal(4,1)  NULL,
    [waveDirection] smallint  NULL,
    [waveHeigth] decimal(3,1)  NULL,
    [startlog] int  NULL,
    [endlog] int  NULL,
    [datahandlerId] int  NULL,
    [gearQuality] nvarchar(1)  NULL,
    [numNets] int  NULL,
    [lostNets] int  NULL,
    [tidTamp] int  NULL,
    [flyerTrack] nvarchar(3)  NULL,
    [released] nvarchar(3)  NULL,
    [sampleTaken] nvarchar(3)  NULL,
    [constantFish] nvarchar(3)  NULL,
    [haulSpeedMean] decimal(3,1)  NULL,
    [haulDirection] smallint  NULL,
    [haulSpeedBot] decimal(3,1)  NULL,
    [haulSpeedWat] decimal(3,1)  NULL,
    [wireLength] int  NULL,
    [wingSpread] decimal(3,1)  NULL,
    [numSamples] int  NULL,
    [courseTrack] int  NULL,
    [shovelDist] decimal(5,1)  NULL,
    [depthAveGear] decimal(5,1)  NULL,
    [netOpening] decimal(3,1)  NULL,
    [depthAbove] int  NULL,
    [landing] decimal(8,1)  NULL,
    [maskWidth] int  NULL,
    [externalJournalNum] nvarchar(15)  NULL,
    [samplePersonId] int  NULL,
    [arrivalWeight] decimal(8,1)  NULL,
    [analysisPersonId] int  NULL,
    [deadline] datetime  NULL,
    [sampleStatus] nvarchar(1)  NULL,
    [robLength] decimal(5,1)  NULL,
    [hydroStnRef] nvarchar(6)  NULL,
    [remark] nvarchar(450)  NULL,
    [labJournalNum] nvarchar(6)  NULL,
    [gearType] nvarchar(50)  NULL,
    [selectionDevice] nvarchar(30)  NULL,
    [selectionDeviceSourceId] int  NULL,
    [meshSize] decimal(5,1)  NULL,
    [numberTrawls] int  NULL,
    [heightNets] decimal(5,1)  NULL,
    [lengthNets] decimal(5,1)  NULL,
    [lengthRopeFlyer] decimal(5,1)  NULL,
    [numberHooks] int  NULL,
    [gearRemark] nvarchar(450)  NULL,
    [widthRopeFlyer] decimal(5,1)  NULL,
    [lengthBeam] decimal(5,1)  NULL,
    [haulType] nvarchar(1)  NULL,
    [thermoCline] nvarchar(10)  NULL,
    [temperatureSrf] decimal(6,2)  NULL,
    [temperatureBot] decimal(6,2)  NULL,
    [oxygenSrf] decimal(6,2)  NULL,
    [oxygenBot] decimal(6,2)  NULL,
    [thermoClineDepth] decimal(6,2)  NULL,
    [salinitySrf] decimal(6,2)  NULL,
    [salinityBot] decimal(6,2)  NULL,
    [totalWeight] decimal(11,4)  NULL,
    [weightEstimationMethod] nvarchar(1)  NULL,
    [sgId] nvarchar(200)  NULL,
    [weekdayWeekend] nvarchar(200)  NULL
);
GO

-- Creating table 'SpeciesList'
CREATE TABLE [dbo].[SpeciesList] (
    [speciesListId] int IDENTITY(1,1) NOT NULL,
    [sampleId] int  NOT NULL,
    [speciesCode] nvarchar(3)  NOT NULL,
    [landingCategory] nvarchar(3)  NULL,
    [dfuBase_Category] nvarchar(3)  NULL,
    [sizeSortingEU] int  NULL,
    [sizeSortingDFU] nvarchar(3)  NULL,
    [sexCode] nvarchar(1)  NULL,
    [ovigorous] nvarchar(3)  NULL,
    [cuticulaHardness] nvarchar(4)  NULL,
    [dfuBase_ArtRecordId] int  NULL,
    [treatment] nvarchar(2)  NOT NULL,
    [agePlusGroup] int  NULL,
    [ageReadId] int  NULL,
    [datahandlerId] int  NULL,
    [stockId] int  NULL,
    [remark] nvarchar(max)  NULL,
    [number] int  NULL,
    [bmsNonRep] decimal(11,4)  NULL,
    [weightEstimationMethod] nvarchar(1)  NULL,
    [hatchMonthReaderId] int  NULL,
    [maturityReaderId] int  NULL,
    [applicationId] int  NULL
);
GO

-- Creating table 'SubSample'
CREATE TABLE [dbo].[SubSample] (
    [subSampleId] int IDENTITY(1,1) NOT NULL,
    [speciesListId] int  NOT NULL,
    [stepNum] int  NOT NULL,
    [representative] nvarchar(3)  NOT NULL,
    [subSampleWeight] decimal(11,4)  NULL,
    [landingWeight] decimal(11,4)  NULL,
    [remark] nvarchar(max)  NULL,
    [sumAnimalWeights] bit  NULL
);
GO

-- Creating table 'Test_Niels'
CREATE TABLE [dbo].[Test_Niels] (
    [station] nvarchar(6)  NOT NULL,
    [dateGearStart] datetime  NOT NULL,
    [dateGearEnd] datetime  NOT NULL,
    [fishingtime] int  NULL,
    [timeZone] int  NULL,
    [gearQuality] nvarchar(1)  NULL
);
GO

-- Creating table 'TrawlOperation'
CREATE TABLE [dbo].[TrawlOperation] (
    [trawlOperationId] int IDENTITY(1,1) NOT NULL,
    [sampleId] int  NOT NULL,
    [operationNum] int  NOT NULL,
    [netOpening] float  NULL,
    [depthOver] int  NULL,
    [totalDepth] decimal(5,1)  NULL,
    [operationTime] int  NULL
);
GO

-- Creating table 'TreatmentFactor'
CREATE TABLE [dbo].[TreatmentFactor] (
    [treatmentFactorId] int IDENTITY(1,1) NOT NULL,
    [treatmentFactorGroup] nvarchar(3)  NOT NULL,
    [treatment] nvarchar(2)  NOT NULL,
    [factor] decimal(6,3)  NOT NULL,
    [description] nvarchar(80)  NULL,
    [versioningDate] datetime  NOT NULL
);
GO

-- Creating table 'Trip'
CREATE TABLE [dbo].[Trip] (
    [tripId] int IDENTITY(1,1) NOT NULL,
    [cruiseId] int  NOT NULL,
    [trip1] nvarchar(10)  NOT NULL,
    [tripType] nvarchar(6)  NULL,
    [samplingTypeId] int  NULL,
    [samplingMethodId] int  NULL,
    [numOfHaulsPerTrip] int  NULL,
    [manualCount] int  NULL,
    [stationStart] nvarchar(10)  NULL,
    [dateStart] datetime  NULL,
    [timeZone] int  NULL,
    [stationEnd] nvarchar(10)  NULL,
    [dateEnd] datetime  NULL,
    [nationality] nvarchar(3)  NULL,
    [tripLeaderId] int  NULL,
    [harbourLanding] nvarchar(4)  NULL,
    [contactPersonId] int  NULL,
    [dataHandlerId] int  NULL,
    [dateEstimTrip] datetime  NULL,
    [dateUpdated] datetime  NULL,
    [remark] nvarchar(max)  NULL,
    [logBldNr] nvarchar(10)  NULL,
    [fDFVessel] bit  NULL,
    [platform1] nvarchar(20)  NULL,
    [platform2] nvarchar(20)  NULL,
    [fisheryType] nvarchar(6)  NULL,
    [dateSample] datetime  NULL,
    [harbourSample] nvarchar(4)  NULL,
    [sgTripId] nvarchar(200)  NULL,
    [tripNum] nvarchar(200)  NULL,
    [placeName] nvarchar(200)  NULL,
    [placeCode] nvarchar(200)  NULL,
    [postalCode] int  NULL,
    [numberInPlace] int  NULL,
    [respYes] int  NULL,
    [respNo] int  NULL,
    [respTot] int  NULL
);
GO

-- Creating table 'ICES_DFU_Relation_FF'
CREATE TABLE [dbo].[ICES_DFU_Relation_FF] (
    [statisticalRectangle] nvarchar(255)  NULL,
    [Area] nvarchar(255)  NULL,
    [area_20_21] nvarchar(50)  NULL,
    [ICES_DFU_Relation_FFId] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'L_SelectionDevice'
CREATE TABLE [dbo].[L_SelectionDevice] (
    [L_selectionDeviceId] int IDENTITY(1,1) NOT NULL,
    [selectionDevice] nvarchar(30)  NOT NULL,
    [description] nvarchar(80)  NULL
);
GO

-- Creating table 'R_GearTypeSelectionDevice'
CREATE TABLE [dbo].[R_GearTypeSelectionDevice] (
    [R_GearTypeSelectionDeviceId] int IDENTITY(1,1) NOT NULL,
    [gearType] nvarchar(50)  NOT NULL,
    [selectionDevice] nvarchar(30)  NOT NULL
);
GO

-- Creating table 'L_FisheryType'
CREATE TABLE [dbo].[L_FisheryType] (
    [L_fisheryTypeId] int IDENTITY(1,1) NOT NULL,
    [fisheryType] nvarchar(6)  NOT NULL,
    [landingCategory] nvarchar(3)  NULL,
    [description] nvarchar(80)  NULL
);
GO

-- Creating table 'L_HaulType'
CREATE TABLE [dbo].[L_HaulType] (
    [L_haulTypeId] int IDENTITY(1,1) NOT NULL,
    [haulType] nvarchar(1)  NOT NULL,
    [num] int  NULL,
    [description] nvarchar(80)  NULL
);
GO

-- Creating table 'L_ThermoCline'
CREATE TABLE [dbo].[L_ThermoCline] (
    [L_thermoClineId] int IDENTITY(1,1) NOT NULL,
    [thermoCline] nvarchar(10)  NOT NULL,
    [num] int  NULL,
    [description] nvarchar(80)  NULL
);
GO

-- Creating table 'L_TimeZone'
CREATE TABLE [dbo].[L_TimeZone] (
    [L_timeZoneId] int IDENTITY(1,1) NOT NULL,
    [timeZone] int  NOT NULL,
    [description] nvarchar(150)  NULL
);
GO

-- Creating table 'L_WeightEstimationMethod'
CREATE TABLE [dbo].[L_WeightEstimationMethod] (
    [L_weightEstimationMethodId] int IDENTITY(1,1) NOT NULL,
    [weightEstimationMethod] nvarchar(1)  NOT NULL,
    [num] int  NULL,
    [description] nvarchar(80)  NULL
);
GO

-- Creating table 'L_EdgeStructure'
CREATE TABLE [dbo].[L_EdgeStructure] (
    [L_edgeStructureId] int IDENTITY(1,1) NOT NULL,
    [edgeStructure] nvarchar(5)  NOT NULL,
    [num] int  NULL,
    [description] nvarchar(250)  NULL
);
GO

-- Creating table 'L_Parasite'
CREATE TABLE [dbo].[L_Parasite] (
    [L_parasiteId] int IDENTITY(1,1) NOT NULL,
    [num] int  NOT NULL,
    [description] nvarchar(100)  NULL
);
GO

-- Creating table 'L_Reference'
CREATE TABLE [dbo].[L_Reference] (
    [L_referenceId] int IDENTITY(1,1) NOT NULL,
    [reference] nvarchar(20)  NOT NULL,
    [description] nvarchar(150)  NULL
);
GO

-- Creating table 'R_AnimalInfoReference'
CREATE TABLE [dbo].[R_AnimalInfoReference] (
    [R_animalInfoReferenceId] int IDENTITY(1,1) NOT NULL,
    [animalInfoId] int  NOT NULL,
    [L_referenceId] int  NOT NULL
);
GO

-- Creating table 'Report'
CREATE TABLE [dbo].[Report] (
    [reportId] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(500)  NOT NULL,
    [description] nvarchar(max)  NULL,
    [type] nvarchar(150)  NOT NULL,
    [data] nvarchar(max)  NULL,
    [isAvailableOffline] bit  NOT NULL,
    [outputFormat] nvarchar(150)  NULL,
    [permissionTasks] nvarchar(max)  NULL,
    [outputPath] nvarchar(max)  NULL,
    [outputPathRestriction] nvarchar(max)  NULL
);
GO

-- Creating table 'ReportingTreeNode'
CREATE TABLE [dbo].[ReportingTreeNode] (
    [reportingTreeNodeId] int IDENTITY(1,1) NOT NULL,
    [parentTreeNodeId] int  NULL,
    [name] nvarchar(500)  NOT NULL,
    [description] nvarchar(max)  NULL
);
GO

-- Creating table 'AnimalFile'
CREATE TABLE [dbo].[AnimalFile] (
    [animalFileId] int IDENTITY(1,1) NOT NULL,
    [animalId] int  NOT NULL,
    [filePath] nvarchar(1000)  NOT NULL,
    [fileType] nvarchar(100)  NOT NULL,
    [autoAdded] bit  NOT NULL
);
GO

-- Creating table 'L_Applications'
CREATE TABLE [dbo].[L_Applications] (
    [L_applicationId] int IDENTITY(1,1) NOT NULL,
    [code] nvarchar(200)  NOT NULL,
    [description] nvarchar(300)  NULL,
    [num] int  NULL
);
GO

-- Creating table 'L_HatchMonthReadability'
CREATE TABLE [dbo].[L_HatchMonthReadability] (
    [L_HatchMonthReadabilityId] int IDENTITY(1,1) NOT NULL,
    [hatchMonthRemark] nvarchar(10)  NOT NULL,
    [description] nvarchar(250)  NULL,
    [num] int  NULL
);
GO

-- Creating table 'L_VisualStock'
CREATE TABLE [dbo].[L_VisualStock] (
    [L_visualStockId] int IDENTITY(1,1) NOT NULL,
    [speciesCode] nvarchar(3)  NOT NULL,
    [visualStock] nvarchar(50)  NOT NULL,
    [description] nvarchar(100)  NULL,
    [num] int  NULL
);
GO

-- Creating table 'L_GeneticStock'
CREATE TABLE [dbo].[L_GeneticStock] (
    [L_geneticStockId] int IDENTITY(1,1) NOT NULL,
    [speciesCode] nvarchar(3)  NOT NULL,
    [geneticStock] nvarchar(50)  NOT NULL,
    [description] nvarchar(100)  NULL,
    [num] int  NULL
);
GO

-- Creating table 'L_SDEventType'
CREATE TABLE [dbo].[L_SDEventType] (
    [L_sdEventTypeId] int IDENTITY(1,1) NOT NULL,
    [eventType] nvarchar(100)  NOT NULL,
    [description] nvarchar(500)  NULL,
    [ageUpdatingMethod] nvarchar(max)  NULL,
    [num] int  NULL
);
GO

-- Creating table 'L_SDPurpose'
CREATE TABLE [dbo].[L_SDPurpose] (
    [L_sdPurposeId] int IDENTITY(1,1) NOT NULL,
    [purpose] nvarchar(100)  NOT NULL,
    [description] nvarchar(500)  NULL
);
GO

-- Creating table 'SDAnnotation'
CREATE TABLE [dbo].[SDAnnotation] (
    [sdAnnotationId] int IDENTITY(1,1) NOT NULL,
    [sdAnnotationGuid] uniqueidentifier  NOT NULL,
    [sdFileId] int  NOT NULL,
    [createdById] int  NULL,
    [createdByUserName] nvarchar(10)  NULL,
    [isApproved] bit  NULL,
    [isFixed] bit  NULL,
    [isReadOnly] bit  NULL,
    [createdTime] datetime  NULL,
    [modifiedTime] datetime  NULL,
    [sdAnalysisParameterId] int  NULL,
    [otolithReadingRemarkId] int  NULL,
    [comment] nvarchar(1000)  NULL,
    [edgeStructure] nvarchar(5)  NULL,
    [age] int  NULL
);
GO

-- Creating table 'SDEvent'
CREATE TABLE [dbo].[SDEvent] (
    [sdEventId] int IDENTITY(1,1) NOT NULL,
    [sdEventGuid] uniqueidentifier  NOT NULL,
    [name] nvarchar(200)  NOT NULL,
    [speciesCode] nvarchar(3)  NULL,
    [startDate] datetime  NULL,
    [endDate] datetime  NULL,
    [sdPurposeId] int  NOT NULL,
    [sdEventTypeId] int  NOT NULL,
    [createdById] int  NULL,
    [createdByUserName] nvarchar(10)  NULL,
    [closed] bit  NOT NULL,
    [createdTime] datetime  NOT NULL,
    [year] int  NULL,
    [sdSampleTypeId] int  NOT NULL,
    [comment] nvarchar(1000)  NULL,
    [uiSDFileExtraColumns] nvarchar(1000)  NULL,
    [defaultImageFolders] nvarchar(max)  NULL
);
GO

-- Creating table 'SDFile'
CREATE TABLE [dbo].[SDFile] (
    [sdFileId] int IDENTITY(1,1) NOT NULL,
    [sdFileGuid] uniqueidentifier  NOT NULL,
    [sdSampleId] int  NOT NULL,
    [fileName] nvarchar(500)  NOT NULL,
    [displayName] nvarchar(500)  NULL,
    [path] nvarchar(500)  NULL,
    [scale] float  NULL,
    [imageWidth] int  NULL,
    [imageHeight] int  NULL
);
GO

-- Creating table 'SDLine'
CREATE TABLE [dbo].[SDLine] (
    [sdLineId] int IDENTITY(1,1) NOT NULL,
    [sdLineGuid] uniqueidentifier  NOT NULL,
    [sdAnnotationId] int  NOT NULL,
    [createdById] int  NULL,
    [createdByUserName] nvarchar(10)  NULL,
    [createdTime] datetime  NULL,
    [color] nvarchar(10)  NULL,
    [lineIndex] int  NULL,
    [width] int  NULL,
    [X1] int  NULL,
    [X2] int  NULL,
    [Y1] int  NULL,
    [Y2] int  NULL
);
GO

-- Creating table 'SDPoint'
CREATE TABLE [dbo].[SDPoint] (
    [sdPointId] int IDENTITY(1,1) NOT NULL,
    [sdPointGuid] uniqueidentifier  NOT NULL,
    [sdAnnotationId] int  NOT NULL,
    [createdById] int  NULL,
    [createdByUserName] nvarchar(10)  NULL,
    [createdTime] datetime  NULL,
    [color] nvarchar(10)  NULL,
    [pointIndex] int  NULL,
    [width] int  NULL,
    [pointType] nvarchar(200)  NULL,
    [shape] nvarchar(200)  NULL,
    [X] int  NULL,
    [Y] int  NULL
);
GO

-- Creating table 'SDSample'
CREATE TABLE [dbo].[SDSample] (
    [sdSampleId] int IDENTITY(1,1) NOT NULL,
    [sdSampleGuid] uniqueidentifier  NOT NULL,
    [sdEventId] int  NOT NULL,
    [animalId] nvarchar(50)  NULL,
    [catchDate] datetime  NULL,
    [DFUArea] nvarchar(3)  NULL,
    [statisticalRectangle] nchar(4)  NULL,
    [latitude] float  NULL,
    [longitude] float  NULL,
    [stockId] int  NULL,
    [sexCode] nvarchar(1)  NULL,
    [fishLengthMM] int  NULL,
    [fishWeightG] decimal(10,5)  NULL,
    [maturityIndexMethod] nvarchar(1)  NULL,
    [maturityId] int  NULL,
    [comments] nvarchar(max)  NULL,
    [createdById] int  NULL,
    [createdByUserName] nvarchar(10)  NULL,
    [createdTime] datetime  NULL,
    [modifiedTime] datetime  NULL,
    [cruise] nvarchar(20)  NULL,
    [trip] nvarchar(10)  NULL,
    [station] nvarchar(6)  NULL,
    [sdPreparationMethodId] int  NULL,
    [sdLightTypeId] int  NULL,
    [sdOtolithDescriptionId] int  NULL,
    [otolithReadingRemarkId] int  NULL,
    [edgeStructure] nvarchar(5)  NULL,
    [readOnly] bit  NOT NULL,
    [importStatus] nvarchar(100)  NULL,
    [speciesCode] nvarchar(3)  NULL
);
GO

-- Creating table 'L_SDLightType'
CREATE TABLE [dbo].[L_SDLightType] (
    [L_sdLightTypeId] int IDENTITY(1,1) NOT NULL,
    [lightType] nvarchar(15)  NOT NULL,
    [dkDescription] nvarchar(500)  NULL,
    [ukDescription] nvarchar(500)  NULL
);
GO

-- Creating table 'L_SDOtolithDescription'
CREATE TABLE [dbo].[L_SDOtolithDescription] (
    [L_sdOtolithDescriptionId] int IDENTITY(1,1) NOT NULL,
    [otolithDescription] nvarchar(15)  NOT NULL,
    [dkDescription] nvarchar(500)  NULL,
    [ukDescription] nvarchar(500)  NULL
);
GO

-- Creating table 'L_SDPreparationMethod'
CREATE TABLE [dbo].[L_SDPreparationMethod] (
    [L_sdPreparationMethodId] int IDENTITY(1,1) NOT NULL,
    [preparationMethod] nvarchar(10)  NOT NULL,
    [dkDescription] nvarchar(500)  NULL,
    [ukDescription] nvarchar(500)  NULL
);
GO

-- Creating table 'L_SDSampleType'
CREATE TABLE [dbo].[L_SDSampleType] (
    [L_sdSampleTypeId] int IDENTITY(1,1) NOT NULL,
    [sampleType] nvarchar(100)  NOT NULL,
    [description] nvarchar(500)  NULL
);
GO

-- Creating table 'R_StockSpeciesArea'
CREATE TABLE [dbo].[R_StockSpeciesArea] (
    [r_StockSpeciesAreaId] int IDENTITY(1,1) NOT NULL,
    [L_stockId] int  NOT NULL,
    [DFUArea] nvarchar(3)  NOT NULL,
    [speciesCode] nvarchar(3)  NOT NULL,
    [statisticalRectangle] nchar(4)  NULL,
    [quarter] int  NULL
);
GO

-- Creating table 'L_SDReaderExperience'
CREATE TABLE [dbo].[L_SDReaderExperience] (
    [L_SDReaderExperienceId] int IDENTITY(1,1) NOT NULL,
    [readerExperience] nvarchar(50)  NOT NULL,
    [description] nvarchar(500)  NULL
);
GO

-- Creating table 'R_SDEventSDReader'
CREATE TABLE [dbo].[R_SDEventSDReader] (
    [sdEventId] int  NOT NULL,
    [sdReaderId] int  NOT NULL,
    [primaryReader] bit  NOT NULL
);
GO

-- Creating table 'R_SDReader'
CREATE TABLE [dbo].[R_SDReader] (
    [r_SDReaderId] int IDENTITY(1,1) NOT NULL,
    [dfuPersonId] int  NOT NULL,
    [speciesCode] nvarchar(3)  NULL,
    [stockId] int  NULL,
    [firstYearAgeReadingGeneral] int  NULL,
    [firstYearAgeReadingCurrent] int  NULL,
    [sdReaderExperienceId] int  NULL,
    [sdPreparationMethodId] int  NULL,
    [comment] nvarchar(1000)  NULL
);
GO

-- Creating table 'L_SDAnalysisParameter'
CREATE TABLE [dbo].[L_SDAnalysisParameter] (
    [L_sdAnalysisParameterId] int IDENTITY(1,1) NOT NULL,
    [analysisParameter] nvarchar(100)  NOT NULL,
    [description] nchar(500)  NULL
);
GO

-- Creating table 'L_SelectionDeviceSource'
CREATE TABLE [dbo].[L_SelectionDeviceSource] (
    [L_selectionDeviceSourceId] int IDENTITY(1,1) NOT NULL,
    [selectionDeviceSource] nvarchar(2)  NOT NULL,
    [description] nvarchar(500)  NULL,
    [num] int  NULL
);
GO

-- Creating table 'L_LengthMeasureType'
CREATE TABLE [dbo].[L_LengthMeasureType] (
    [L_lengthMeasureTypeId] int IDENTITY(1,1) NOT NULL,
    [lengthMeasureType] nvarchar(5)  NOT NULL,
    [description] nvarchar(500)  NULL,
    [num] int  NULL,
    [RDBES] nvarchar(50)  NULL
);
GO

-- Creating table 'R_ReportingTreeNodeReport'
CREATE TABLE [dbo].[R_ReportingTreeNodeReport] (
    [Reports_reportId] int  NOT NULL,
    [ReportingTreeNodes_reportingTreeNodeId] int  NOT NULL
);
GO

-- Creating table 'R_SDEventDFUArea'
CREATE TABLE [dbo].[R_SDEventDFUArea] (
    [L_DFUAreas_DFUArea] nvarchar(3)  NOT NULL,
    [SDEvent_sdEventId] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ActivityLogId] in table 'ActivityLog'
ALTER TABLE [dbo].[ActivityLog]
ADD CONSTRAINT [PK_ActivityLog]
    PRIMARY KEY CLUSTERED ([ActivityLogId] ASC);
GO

-- Creating primary key on [ageId] in table 'Age'
ALTER TABLE [dbo].[Age]
ADD CONSTRAINT [PK_Age]
    PRIMARY KEY CLUSTERED ([ageId] ASC);
GO

-- Creating primary key on [animalId] in table 'Animal'
ALTER TABLE [dbo].[Animal]
ADD CONSTRAINT [PK_Animal]
    PRIMARY KEY CLUSTERED ([animalId] ASC);
GO

-- Creating primary key on [animalInfoId] in table 'AnimalInfo'
ALTER TABLE [dbo].[AnimalInfo]
ADD CONSTRAINT [PK_AnimalInfo]
    PRIMARY KEY CLUSTERED ([animalInfoId] ASC);
GO

-- Creating primary key on [cruiseId] in table 'Cruise'
ALTER TABLE [dbo].[Cruise]
ADD CONSTRAINT [PK_Cruise]
    PRIMARY KEY CLUSTERED ([cruiseId] ASC);
GO

-- Creating primary key on [dfuPersonId] in table 'DFUPerson'
ALTER TABLE [dbo].[DFUPerson]
ADD CONSTRAINT [PK_DFUPerson]
    PRIMARY KEY CLUSTERED ([dfuPersonId] ASC);
GO

-- Creating primary key on [dfuSubAreaId] in table 'DFUSubArea'
ALTER TABLE [dbo].[DFUSubArea]
ADD CONSTRAINT [PK_DFUSubArea]
    PRIMARY KEY CLUSTERED ([dfuSubAreaId] ASC);
GO

-- Creating primary key on [Est_MethodId] in table 'Est_Method'
ALTER TABLE [dbo].[Est_Method]
ADD CONSTRAINT [PK_Est_Method]
    PRIMARY KEY CLUSTERED ([Est_MethodId] ASC);
GO

-- Creating primary key on [Est_MethodStepId] in table 'Est_MethodStep'
ALTER TABLE [dbo].[Est_MethodStep]
ADD CONSTRAINT [PK_Est_MethodStep]
    PRIMARY KEY CLUSTERED ([Est_MethodStepId] ASC);
GO

-- Creating primary key on [Est_StrataId] in table 'Est_Strata'
ALTER TABLE [dbo].[Est_Strata]
ADD CONSTRAINT [PK_Est_Strata]
    PRIMARY KEY CLUSTERED ([Est_StrataId] ASC);
GO

-- Creating primary key on [fatId] in table 'Fat'
ALTER TABLE [dbo].[Fat]
ADD CONSTRAINT [PK_Fat]
    PRIMARY KEY CLUSTERED ([fatId] ASC);
GO

-- Creating primary key on [fishingActivityId] in table 'FishingActivityOnSample'
ALTER TABLE [dbo].[FishingActivityOnSample]
ADD CONSTRAINT [PK_FishingActivityOnSample]
    PRIMARY KEY CLUSTERED ([fishingActivityId] ASC);
GO

-- Creating primary key on [L_activityid] in table 'L_Activity'
ALTER TABLE [dbo].[L_Activity]
ADD CONSTRAINT [PK_L_Activity]
    PRIMARY KEY CLUSTERED ([L_activityid] ASC);
GO

-- Creating primary key on [L_ageMeasureMethodId] in table 'L_AgeMeasureMethod'
ALTER TABLE [dbo].[L_AgeMeasureMethod]
ADD CONSTRAINT [PK_L_AgeMeasureMethod]
    PRIMARY KEY CLUSTERED ([L_ageMeasureMethodId] ASC);
GO

-- Creating primary key on [cruiseID], [shipCode], [cruiseYear], [cruiseNo], [status] in table 'L_BITS_cruises'
ALTER TABLE [dbo].[L_BITS_cruises]
ADD CONSTRAINT [PK_L_BITS_cruises]
    PRIMARY KEY CLUSTERED ([cruiseID], [shipCode], [cruiseYear], [cruiseNo], [status] ASC);
GO

-- Creating primary key on [L_bottomTypeId] in table 'L_Bottomtype'
ALTER TABLE [dbo].[L_Bottomtype]
ADD CONSTRAINT [PK_L_Bottomtype]
    PRIMARY KEY CLUSTERED ([L_bottomTypeId] ASC);
GO

-- Creating primary key on [broodingPhase] in table 'L_BroodingPhase'
ALTER TABLE [dbo].[L_BroodingPhase]
ADD CONSTRAINT [PK_L_BroodingPhase]
    PRIMARY KEY CLUSTERED ([broodingPhase] ASC);
GO

-- Creating primary key on [catchRegistrationId] in table 'L_CatchRegistration'
ALTER TABLE [dbo].[L_CatchRegistration]
ADD CONSTRAINT [PK_L_CatchRegistration]
    PRIMARY KEY CLUSTERED ([catchRegistrationId] ASC);
GO

-- Creating primary key on [cruiseStatus] in table 'L_CruiseStatus'
ALTER TABLE [dbo].[L_CruiseStatus]
ADD CONSTRAINT [PK_L_CruiseStatus]
    PRIMARY KEY CLUSTERED ([cruiseStatus] ASC);
GO

-- Creating primary key on [cruiseType] in table 'L_CruiseType'
ALTER TABLE [dbo].[L_CruiseType]
ADD CONSTRAINT [PK_L_CruiseType]
    PRIMARY KEY CLUSTERED ([cruiseType] ASC);
GO

-- Creating primary key on [cuticulaHardness] in table 'L_CuticulaHardness'
ALTER TABLE [dbo].[L_CuticulaHardness]
ADD CONSTRAINT [PK_L_CuticulaHardness]
    PRIMARY KEY CLUSTERED ([cuticulaHardness] ASC);
GO

-- Creating primary key on [DFUArea] in table 'L_DFUArea'
ALTER TABLE [dbo].[L_DFUArea]
ADD CONSTRAINT [PK_L_DFUArea]
    PRIMARY KEY CLUSTERED ([DFUArea] ASC);
GO

-- Creating primary key on [dfuBase_Category] in table 'L_DFUBase_Category'
ALTER TABLE [dbo].[L_DFUBase_Category]
ADD CONSTRAINT [PK_L_DFUBase_Category]
    PRIMARY KEY CLUSTERED ([dfuBase_Category] ASC);
GO

-- Creating primary key on [dfuDepartment] in table 'L_DFUDepartment'
ALTER TABLE [dbo].[L_DFUDepartment]
ADD CONSTRAINT [PK_L_DFUDepartment]
    PRIMARY KEY CLUSTERED ([dfuDepartment] ASC);
GO

-- Creating primary key on [fatIndexMethod] in table 'L_FatIndexMethod'
ALTER TABLE [dbo].[L_FatIndexMethod]
ADD CONSTRAINT [PK_L_FatIndexMethod]
    PRIMARY KEY CLUSTERED ([fatIndexMethod] ASC);
GO

-- Creating primary key on [FejlkategoriId] in table 'L_Fejlkategorier'
ALTER TABLE [dbo].[L_Fejlkategorier]
ADD CONSTRAINT [PK_L_Fejlkategorier]
    PRIMARY KEY CLUSTERED ([FejlkategoriId] ASC);
GO

-- Creating primary key on [Id] in table 'L_FishingActivityCategory'
ALTER TABLE [dbo].[L_FishingActivityCategory]
ADD CONSTRAINT [PK_L_FishingActivityCategory]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id], [Code], [Description], [FishingActivityLevel], [ParentFishingActivityCategory], [FishingActivityCategoryType] in table 'L_FishingActivityCategory_temp'
ALTER TABLE [dbo].[L_FishingActivityCategory_temp]
ADD CONSTRAINT [PK_L_FishingActivityCategory_temp]
    PRIMARY KEY CLUSTERED ([Id], [Code], [Description], [FishingActivityLevel], [ParentFishingActivityCategory], [FishingActivityCategoryType] ASC);
GO

-- Creating primary key on [fishingActivityId] in table 'L_FishingActivityNational'
ALTER TABLE [dbo].[L_FishingActivityNational]
ADD CONSTRAINT [PK_L_FishingActivityNational]
    PRIMARY KEY CLUSTERED ([fishingActivityId] ASC);
GO

-- Creating primary key on [gearId] in table 'L_Gear'
ALTER TABLE [dbo].[L_Gear]
ADD CONSTRAINT [PK_L_Gear]
    PRIMARY KEY CLUSTERED ([gearId] ASC);
GO

-- Creating primary key on [gearInfoType] in table 'L_GearInfoType'
ALTER TABLE [dbo].[L_GearInfoType]
ADD CONSTRAINT [PK_L_GearInfoType]
    PRIMARY KEY CLUSTERED ([gearInfoType] ASC);
GO

-- Creating primary key on [gearQuality] in table 'L_GearQuality'
ALTER TABLE [dbo].[L_GearQuality]
ADD CONSTRAINT [PK_L_GearQuality]
    PRIMARY KEY CLUSTERED ([gearQuality] ASC);
GO

-- Creating primary key on [gearType] in table 'L_GearType'
ALTER TABLE [dbo].[L_GearType]
ADD CONSTRAINT [PK_L_GearType]
    PRIMARY KEY CLUSTERED ([gearType] ASC);
GO

-- Creating primary key on [harbour] in table 'L_Harbour'
ALTER TABLE [dbo].[L_Harbour]
ADD CONSTRAINT [PK_L_Harbour]
    PRIMARY KEY CLUSTERED ([harbour] ASC);
GO

-- Creating primary key on [harbour] in table 'L_Harbour2'
ALTER TABLE [dbo].[L_Harbour2]
ADD CONSTRAINT [PK_L_Harbour2]
    PRIMARY KEY CLUSTERED ([harbour] ASC);
GO

-- Creating primary key on [cruiseID], [shipCode], [cruiseYear], [cruiseNo], [status] in table 'L_IBTS_cruises'
ALTER TABLE [dbo].[L_IBTS_cruises]
ADD CONSTRAINT [PK_L_IBTS_cruises]
    PRIMARY KEY CLUSTERED ([cruiseID], [shipCode], [cruiseYear], [cruiseNo], [status] ASC);
GO

-- Creating primary key on [landingCategory] in table 'L_LandingCategory'
ALTER TABLE [dbo].[L_LandingCategory]
ADD CONSTRAINT [PK_L_LandingCategory]
    PRIMARY KEY CLUSTERED ([landingCategory] ASC);
GO

-- Creating primary key on [lengthMeasureUnit] in table 'L_LengthMeasureUnit'
ALTER TABLE [dbo].[L_LengthMeasureUnit]
ADD CONSTRAINT [PK_L_LengthMeasureUnit]
    PRIMARY KEY CLUSTERED ([lengthMeasureUnit] ASC);
GO

-- Creating primary key on [maturityIndexMethod] in table 'L_MaturityIndexMethod'
ALTER TABLE [dbo].[L_MaturityIndexMethod]
ADD CONSTRAINT [PK_L_MaturityIndexMethod]
    PRIMARY KEY CLUSTERED ([maturityIndexMethod] ASC);
GO

-- Creating primary key on [dataTable], [dataField], [dataValue], [keydata] in table 'L_MissingLookup'
ALTER TABLE [dbo].[L_MissingLookup]
ADD CONSTRAINT [PK_L_MissingLookup]
    PRIMARY KEY CLUSTERED ([dataTable], [dataField], [dataValue], [keydata] ASC);
GO

-- Creating primary key on [nationality] in table 'L_Nationality'
ALTER TABLE [dbo].[L_Nationality]
ADD CONSTRAINT [PK_L_Nationality]
    PRIMARY KEY CLUSTERED ([nationality] ASC);
GO

-- Creating primary key on [navigationSystem] in table 'L_NavigationSystem'
ALTER TABLE [dbo].[L_NavigationSystem]
ADD CONSTRAINT [PK_L_NavigationSystem]
    PRIMARY KEY CLUSTERED ([navigationSystem] ASC);
GO

-- Creating primary key on [L_OtolithReadingRemarkID] in table 'L_OtolithReadingRemark'
ALTER TABLE [dbo].[L_OtolithReadingRemark]
ADD CONSTRAINT [PK_L_OtolithReadingRemark]
    PRIMARY KEY CLUSTERED ([L_OtolithReadingRemarkID] ASC);
GO

-- Creating primary key on [platform] in table 'L_Platform'
ALTER TABLE [dbo].[L_Platform]
ADD CONSTRAINT [PK_L_Platform]
    PRIMARY KEY CLUSTERED ([platform] ASC);
GO

-- Creating primary key on [platformType] in table 'L_PlatformType'
ALTER TABLE [dbo].[L_PlatformType]
ADD CONSTRAINT [PK_L_PlatformType]
    PRIMARY KEY CLUSTERED ([platformType] ASC);
GO

-- Creating primary key on [platformVersionId] in table 'L_PlatformVersion'
ALTER TABLE [dbo].[L_PlatformVersion]
ADD CONSTRAINT [PK_L_PlatformVersion]
    PRIMARY KEY CLUSTERED ([platformVersionId] ASC);
GO

-- Creating primary key on [sampleStatus] in table 'L_SampleStatus'
ALTER TABLE [dbo].[L_SampleStatus]
ADD CONSTRAINT [PK_L_SampleStatus]
    PRIMARY KEY CLUSTERED ([sampleStatus] ASC);
GO

-- Creating primary key on [sampleType] in table 'L_SampleType'
ALTER TABLE [dbo].[L_SampleType]
ADD CONSTRAINT [PK_L_SampleType]
    PRIMARY KEY CLUSTERED ([sampleType] ASC);
GO

-- Creating primary key on [samplingMethodId] in table 'L_SamplingMethod'
ALTER TABLE [dbo].[L_SamplingMethod]
ADD CONSTRAINT [PK_L_SamplingMethod]
    PRIMARY KEY CLUSTERED ([samplingMethodId] ASC);
GO

-- Creating primary key on [samplingTypeId] in table 'L_SamplingType'
ALTER TABLE [dbo].[L_SamplingType]
ADD CONSTRAINT [PK_L_SamplingType]
    PRIMARY KEY CLUSTERED ([samplingTypeId] ASC);
GO

-- Creating primary key on [selectPanel] in table 'L_SelectPanel'
ALTER TABLE [dbo].[L_SelectPanel]
ADD CONSTRAINT [PK_L_SelectPanel]
    PRIMARY KEY CLUSTERED ([selectPanel] ASC);
GO

-- Creating primary key on [sexCode] in table 'L_SexCode'
ALTER TABLE [dbo].[L_SexCode]
ADD CONSTRAINT [PK_L_SexCode]
    PRIMARY KEY CLUSTERED ([sexCode] ASC);
GO

-- Creating primary key on [shovelType] in table 'L_ShovelType'
ALTER TABLE [dbo].[L_ShovelType]
ADD CONSTRAINT [PK_L_ShovelType]
    PRIMARY KEY CLUSTERED ([shovelType] ASC);
GO

-- Creating primary key on [sizeSortingDFU] in table 'L_SizeSortingDFU'
ALTER TABLE [dbo].[L_SizeSortingDFU]
ADD CONSTRAINT [PK_L_SizeSortingDFU]
    PRIMARY KEY CLUSTERED ([sizeSortingDFU] ASC);
GO

-- Creating primary key on [sizeSortingEU] in table 'L_SizeSortingEU'
ALTER TABLE [dbo].[L_SizeSortingEU]
ADD CONSTRAINT [PK_L_SizeSortingEU]
    PRIMARY KEY CLUSTERED ([sizeSortingEU] ASC);
GO

-- Creating primary key on [speciesCode] in table 'L_Species'
ALTER TABLE [dbo].[L_Species]
ADD CONSTRAINT [PK_L_Species]
    PRIMARY KEY CLUSTERED ([speciesCode] ASC);
GO

-- Creating primary key on [speciesRegistrationId] in table 'L_SpeciesRegistration'
ALTER TABLE [dbo].[L_SpeciesRegistration]
ADD CONSTRAINT [PK_L_SpeciesRegistration]
    PRIMARY KEY CLUSTERED ([speciesRegistrationId] ASC);
GO

-- Creating primary key on [statisticalRectangle] in table 'L_StatisticalRectangle'
ALTER TABLE [dbo].[L_StatisticalRectangle]
ADD CONSTRAINT [PK_L_StatisticalRectangle]
    PRIMARY KEY CLUSTERED ([statisticalRectangle] ASC);
GO

-- Creating primary key on [L_stockId] in table 'L_Stock'
ALTER TABLE [dbo].[L_Stock]
ADD CONSTRAINT [PK_L_Stock]
    PRIMARY KEY CLUSTERED ([L_stockId] ASC);
GO

-- Creating primary key on [L_SubAreaTypeId] in table 'L_SubAreaType'
ALTER TABLE [dbo].[L_SubAreaType]
ADD CONSTRAINT [PK_L_SubAreaType]
    PRIMARY KEY CLUSTERED ([L_SubAreaTypeId] ASC);
GO

-- Creating primary key on [treadMaterial] in table 'L_TreadMaterial'
ALTER TABLE [dbo].[L_TreadMaterial]
ADD CONSTRAINT [PK_L_TreadMaterial]
    PRIMARY KEY CLUSTERED ([treadMaterial] ASC);
GO

-- Creating primary key on [treadType] in table 'L_TreadType'
ALTER TABLE [dbo].[L_TreadType]
ADD CONSTRAINT [PK_L_TreadType]
    PRIMARY KEY CLUSTERED ([treadType] ASC);
GO

-- Creating primary key on [treatment] in table 'L_Treatment'
ALTER TABLE [dbo].[L_Treatment]
ADD CONSTRAINT [PK_L_Treatment]
    PRIMARY KEY CLUSTERED ([treatment] ASC);
GO

-- Creating primary key on [treatmentFactorGroup] in table 'L_TreatmentFactorGroup'
ALTER TABLE [dbo].[L_TreatmentFactorGroup]
ADD CONSTRAINT [PK_L_TreatmentFactorGroup]
    PRIMARY KEY CLUSTERED ([treatmentFactorGroup] ASC);
GO

-- Creating primary key on [tripType] in table 'L_TripType'
ALTER TABLE [dbo].[L_TripType]
ADD CONSTRAINT [PK_L_TripType]
    PRIMARY KEY CLUSTERED ([tripType] ASC);
GO

-- Creating primary key on [usabilityParamId] in table 'L_UsabilityParam'
ALTER TABLE [dbo].[L_UsabilityParam]
ADD CONSTRAINT [PK_L_UsabilityParam]
    PRIMARY KEY CLUSTERED ([usabilityParamId] ASC);
GO

-- Creating primary key on [usabilityParamGrpId] in table 'L_UsabilityParamGrp'
ALTER TABLE [dbo].[L_UsabilityParamGrp]
ADD CONSTRAINT [PK_L_UsabilityParamGrp]
    PRIMARY KEY CLUSTERED ([usabilityParamGrpId] ASC);
GO

-- Creating primary key on [YesNo] in table 'L_YesNo'
ALTER TABLE [dbo].[L_YesNo]
ADD CONSTRAINT [PK_L_YesNo]
    PRIMARY KEY CLUSTERED ([YesNo] ASC);
GO

-- Creating primary key on [maturityId] in table 'Maturity'
ALTER TABLE [dbo].[Maturity]
ADD CONSTRAINT [PK_Maturity]
    PRIMARY KEY CLUSTERED ([maturityId] ASC);
GO

-- Creating primary key on [numberOfStationsPerTripId] in table 'NumberOfStationsPerTrip'
ALTER TABLE [dbo].[NumberOfStationsPerTrip]
ADD CONSTRAINT [PK_NumberOfStationsPerTrip]
    PRIMARY KEY CLUSTERED ([numberOfStationsPerTripId] ASC);
GO

-- Creating primary key on [personId] in table 'Person'
ALTER TABLE [dbo].[Person]
ADD CONSTRAINT [PK_Person]
    PRIMARY KEY CLUSTERED ([personId] ASC);
GO

-- Creating primary key on [usercode], [queryName] in table 'Queries'
ALTER TABLE [dbo].[Queries]
ADD CONSTRAINT [PK_Queries]
    PRIMARY KEY CLUSTERED ([usercode], [queryName] ASC);
GO

-- Creating primary key on [QueryTypeID] in table 'QueryTypes'
ALTER TABLE [dbo].[QueryTypes]
ADD CONSTRAINT [PK_QueryTypes]
    PRIMARY KEY CLUSTERED ([QueryTypeID] ASC);
GO

-- Creating primary key on [R_CruiseUsbilityParamId] in table 'R_CruiseUsabilityParam'
ALTER TABLE [dbo].[R_CruiseUsabilityParam]
ADD CONSTRAINT [PK_R_CruiseUsabilityParam]
    PRIMARY KEY CLUSTERED ([R_CruiseUsbilityParamId] ASC);
GO

-- Creating primary key on [R_gearInfoID] in table 'R_GearInfo'
ALTER TABLE [dbo].[R_GearInfo]
ADD CONSTRAINT [PK_R_GearInfo]
    PRIMARY KEY CLUSTERED ([R_gearInfoID] ASC);
GO

-- Creating primary key on [r_MaturityId] in table 'R_Maturity'
ALTER TABLE [dbo].[R_Maturity]
ADD CONSTRAINT [PK_R_Maturity]
    PRIMARY KEY CLUSTERED ([r_MaturityId] ASC);
GO

-- Creating primary key on [R_SampleUsabilityParamId], [SampleId], [usabilityParamId] in table 'R_SampleUsabilityParam'
ALTER TABLE [dbo].[R_SampleUsabilityParam]
ADD CONSTRAINT [PK_R_SampleUsabilityParam]
    PRIMARY KEY CLUSTERED ([R_SampleUsabilityParamId], [SampleId], [usabilityParamId] ASC);
GO

-- Creating primary key on [TargetSpeciesId] in table 'R_TargetSpecies'
ALTER TABLE [dbo].[R_TargetSpecies]
ADD CONSTRAINT [PK_R_TargetSpecies]
    PRIMARY KEY CLUSTERED ([TargetSpeciesId] ASC);
GO

-- Creating primary key on [R_TripPlatformVersionId] in table 'R_TripPlatformVersion'
ALTER TABLE [dbo].[R_TripPlatformVersion]
ADD CONSTRAINT [PK_R_TripPlatformVersion]
    PRIMARY KEY CLUSTERED ([R_TripPlatformVersionId] ASC);
GO

-- Creating primary key on [R_tripUsabilityParamId], [tripId], [usabilityParamId] in table 'R_TripUsabilityParam'
ALTER TABLE [dbo].[R_TripUsabilityParam]
ADD CONSTRAINT [PK_R_TripUsabilityParam]
    PRIMARY KEY CLUSTERED ([R_tripUsabilityParamId], [tripId], [usabilityParamId] ASC);
GO

-- Creating primary key on [R_UsabilityParamUsabilityGrpId] in table 'R_UsabilityParamUsabilityGrp'
ALTER TABLE [dbo].[R_UsabilityParamUsabilityGrp]
ADD CONSTRAINT [PK_R_UsabilityParamUsabilityGrp]
    PRIMARY KEY CLUSTERED ([R_UsabilityParamUsabilityGrpId] ASC);
GO

-- Creating primary key on [replicationLogId], [logType], [time], [source], [systemText], [userText] in table 'replicationLog'
ALTER TABLE [dbo].[replicationLog]
ADD CONSTRAINT [PK_replicationLog]
    PRIMARY KEY CLUSTERED ([replicationLogId], [logType], [time], [source], [systemText], [userText] ASC);
GO

-- Creating primary key on [sampleId] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [PK_Sample]
    PRIMARY KEY CLUSTERED ([sampleId] ASC);
GO

-- Creating primary key on [speciesListId] in table 'SpeciesList'
ALTER TABLE [dbo].[SpeciesList]
ADD CONSTRAINT [PK_SpeciesList]
    PRIMARY KEY CLUSTERED ([speciesListId] ASC);
GO

-- Creating primary key on [subSampleId] in table 'SubSample'
ALTER TABLE [dbo].[SubSample]
ADD CONSTRAINT [PK_SubSample]
    PRIMARY KEY CLUSTERED ([subSampleId] ASC);
GO

-- Creating primary key on [station], [dateGearStart], [dateGearEnd] in table 'Test_Niels'
ALTER TABLE [dbo].[Test_Niels]
ADD CONSTRAINT [PK_Test_Niels]
    PRIMARY KEY CLUSTERED ([station], [dateGearStart], [dateGearEnd] ASC);
GO

-- Creating primary key on [trawlOperationId] in table 'TrawlOperation'
ALTER TABLE [dbo].[TrawlOperation]
ADD CONSTRAINT [PK_TrawlOperation]
    PRIMARY KEY CLUSTERED ([trawlOperationId] ASC);
GO

-- Creating primary key on [treatmentFactorId] in table 'TreatmentFactor'
ALTER TABLE [dbo].[TreatmentFactor]
ADD CONSTRAINT [PK_TreatmentFactor]
    PRIMARY KEY CLUSTERED ([treatmentFactorId] ASC);
GO

-- Creating primary key on [tripId] in table 'Trip'
ALTER TABLE [dbo].[Trip]
ADD CONSTRAINT [PK_Trip]
    PRIMARY KEY CLUSTERED ([tripId] ASC);
GO

-- Creating primary key on [ICES_DFU_Relation_FFId] in table 'ICES_DFU_Relation_FF'
ALTER TABLE [dbo].[ICES_DFU_Relation_FF]
ADD CONSTRAINT [PK_ICES_DFU_Relation_FF]
    PRIMARY KEY CLUSTERED ([ICES_DFU_Relation_FFId] ASC);
GO

-- Creating primary key on [selectionDevice] in table 'L_SelectionDevice'
ALTER TABLE [dbo].[L_SelectionDevice]
ADD CONSTRAINT [PK_L_SelectionDevice]
    PRIMARY KEY CLUSTERED ([selectionDevice] ASC);
GO

-- Creating primary key on [R_GearTypeSelectionDeviceId] in table 'R_GearTypeSelectionDevice'
ALTER TABLE [dbo].[R_GearTypeSelectionDevice]
ADD CONSTRAINT [PK_R_GearTypeSelectionDevice]
    PRIMARY KEY CLUSTERED ([R_GearTypeSelectionDeviceId] ASC);
GO

-- Creating primary key on [fisheryType] in table 'L_FisheryType'
ALTER TABLE [dbo].[L_FisheryType]
ADD CONSTRAINT [PK_L_FisheryType]
    PRIMARY KEY CLUSTERED ([fisheryType] ASC);
GO

-- Creating primary key on [haulType] in table 'L_HaulType'
ALTER TABLE [dbo].[L_HaulType]
ADD CONSTRAINT [PK_L_HaulType]
    PRIMARY KEY CLUSTERED ([haulType] ASC);
GO

-- Creating primary key on [thermoCline] in table 'L_ThermoCline'
ALTER TABLE [dbo].[L_ThermoCline]
ADD CONSTRAINT [PK_L_ThermoCline]
    PRIMARY KEY CLUSTERED ([thermoCline] ASC);
GO

-- Creating primary key on [timeZone] in table 'L_TimeZone'
ALTER TABLE [dbo].[L_TimeZone]
ADD CONSTRAINT [PK_L_TimeZone]
    PRIMARY KEY CLUSTERED ([timeZone] ASC);
GO

-- Creating primary key on [weightEstimationMethod] in table 'L_WeightEstimationMethod'
ALTER TABLE [dbo].[L_WeightEstimationMethod]
ADD CONSTRAINT [PK_L_WeightEstimationMethod]
    PRIMARY KEY CLUSTERED ([weightEstimationMethod] ASC);
GO

-- Creating primary key on [edgeStructure] in table 'L_EdgeStructure'
ALTER TABLE [dbo].[L_EdgeStructure]
ADD CONSTRAINT [PK_L_EdgeStructure]
    PRIMARY KEY CLUSTERED ([edgeStructure] ASC);
GO

-- Creating primary key on [L_parasiteId] in table 'L_Parasite'
ALTER TABLE [dbo].[L_Parasite]
ADD CONSTRAINT [PK_L_Parasite]
    PRIMARY KEY CLUSTERED ([L_parasiteId] ASC);
GO

-- Creating primary key on [L_referenceId] in table 'L_Reference'
ALTER TABLE [dbo].[L_Reference]
ADD CONSTRAINT [PK_L_Reference]
    PRIMARY KEY CLUSTERED ([L_referenceId] ASC);
GO

-- Creating primary key on [R_animalInfoReferenceId] in table 'R_AnimalInfoReference'
ALTER TABLE [dbo].[R_AnimalInfoReference]
ADD CONSTRAINT [PK_R_AnimalInfoReference]
    PRIMARY KEY CLUSTERED ([R_animalInfoReferenceId] ASC);
GO

-- Creating primary key on [reportId] in table 'Report'
ALTER TABLE [dbo].[Report]
ADD CONSTRAINT [PK_Report]
    PRIMARY KEY CLUSTERED ([reportId] ASC);
GO

-- Creating primary key on [reportingTreeNodeId] in table 'ReportingTreeNode'
ALTER TABLE [dbo].[ReportingTreeNode]
ADD CONSTRAINT [PK_ReportingTreeNode]
    PRIMARY KEY CLUSTERED ([reportingTreeNodeId] ASC);
GO

-- Creating primary key on [animalFileId] in table 'AnimalFile'
ALTER TABLE [dbo].[AnimalFile]
ADD CONSTRAINT [PK_AnimalFile]
    PRIMARY KEY CLUSTERED ([animalFileId] ASC);
GO

-- Creating primary key on [L_applicationId] in table 'L_Applications'
ALTER TABLE [dbo].[L_Applications]
ADD CONSTRAINT [PK_L_Applications]
    PRIMARY KEY CLUSTERED ([L_applicationId] ASC);
GO

-- Creating primary key on [L_HatchMonthReadabilityId] in table 'L_HatchMonthReadability'
ALTER TABLE [dbo].[L_HatchMonthReadability]
ADD CONSTRAINT [PK_L_HatchMonthReadability]
    PRIMARY KEY CLUSTERED ([L_HatchMonthReadabilityId] ASC);
GO

-- Creating primary key on [L_visualStockId] in table 'L_VisualStock'
ALTER TABLE [dbo].[L_VisualStock]
ADD CONSTRAINT [PK_L_VisualStock]
    PRIMARY KEY CLUSTERED ([L_visualStockId] ASC);
GO

-- Creating primary key on [L_geneticStockId] in table 'L_GeneticStock'
ALTER TABLE [dbo].[L_GeneticStock]
ADD CONSTRAINT [PK_L_GeneticStock]
    PRIMARY KEY CLUSTERED ([L_geneticStockId] ASC);
GO

-- Creating primary key on [L_sdEventTypeId] in table 'L_SDEventType'
ALTER TABLE [dbo].[L_SDEventType]
ADD CONSTRAINT [PK_L_SDEventType]
    PRIMARY KEY CLUSTERED ([L_sdEventTypeId] ASC);
GO

-- Creating primary key on [L_sdPurposeId] in table 'L_SDPurpose'
ALTER TABLE [dbo].[L_SDPurpose]
ADD CONSTRAINT [PK_L_SDPurpose]
    PRIMARY KEY CLUSTERED ([L_sdPurposeId] ASC);
GO

-- Creating primary key on [sdAnnotationId] in table 'SDAnnotation'
ALTER TABLE [dbo].[SDAnnotation]
ADD CONSTRAINT [PK_SDAnnotation]
    PRIMARY KEY CLUSTERED ([sdAnnotationId] ASC);
GO

-- Creating primary key on [sdEventId] in table 'SDEvent'
ALTER TABLE [dbo].[SDEvent]
ADD CONSTRAINT [PK_SDEvent]
    PRIMARY KEY CLUSTERED ([sdEventId] ASC);
GO

-- Creating primary key on [sdFileId] in table 'SDFile'
ALTER TABLE [dbo].[SDFile]
ADD CONSTRAINT [PK_SDFile]
    PRIMARY KEY CLUSTERED ([sdFileId] ASC);
GO

-- Creating primary key on [sdLineId] in table 'SDLine'
ALTER TABLE [dbo].[SDLine]
ADD CONSTRAINT [PK_SDLine]
    PRIMARY KEY CLUSTERED ([sdLineId] ASC);
GO

-- Creating primary key on [sdPointId] in table 'SDPoint'
ALTER TABLE [dbo].[SDPoint]
ADD CONSTRAINT [PK_SDPoint]
    PRIMARY KEY CLUSTERED ([sdPointId] ASC);
GO

-- Creating primary key on [sdSampleId] in table 'SDSample'
ALTER TABLE [dbo].[SDSample]
ADD CONSTRAINT [PK_SDSample]
    PRIMARY KEY CLUSTERED ([sdSampleId] ASC);
GO

-- Creating primary key on [L_sdLightTypeId] in table 'L_SDLightType'
ALTER TABLE [dbo].[L_SDLightType]
ADD CONSTRAINT [PK_L_SDLightType]
    PRIMARY KEY CLUSTERED ([L_sdLightTypeId] ASC);
GO

-- Creating primary key on [L_sdOtolithDescriptionId] in table 'L_SDOtolithDescription'
ALTER TABLE [dbo].[L_SDOtolithDescription]
ADD CONSTRAINT [PK_L_SDOtolithDescription]
    PRIMARY KEY CLUSTERED ([L_sdOtolithDescriptionId] ASC);
GO

-- Creating primary key on [L_sdPreparationMethodId] in table 'L_SDPreparationMethod'
ALTER TABLE [dbo].[L_SDPreparationMethod]
ADD CONSTRAINT [PK_L_SDPreparationMethod]
    PRIMARY KEY CLUSTERED ([L_sdPreparationMethodId] ASC);
GO

-- Creating primary key on [L_sdSampleTypeId] in table 'L_SDSampleType'
ALTER TABLE [dbo].[L_SDSampleType]
ADD CONSTRAINT [PK_L_SDSampleType]
    PRIMARY KEY CLUSTERED ([L_sdSampleTypeId] ASC);
GO

-- Creating primary key on [r_StockSpeciesAreaId] in table 'R_StockSpeciesArea'
ALTER TABLE [dbo].[R_StockSpeciesArea]
ADD CONSTRAINT [PK_R_StockSpeciesArea]
    PRIMARY KEY CLUSTERED ([r_StockSpeciesAreaId] ASC);
GO

-- Creating primary key on [L_SDReaderExperienceId] in table 'L_SDReaderExperience'
ALTER TABLE [dbo].[L_SDReaderExperience]
ADD CONSTRAINT [PK_L_SDReaderExperience]
    PRIMARY KEY CLUSTERED ([L_SDReaderExperienceId] ASC);
GO

-- Creating primary key on [sdEventId], [sdReaderId] in table 'R_SDEventSDReader'
ALTER TABLE [dbo].[R_SDEventSDReader]
ADD CONSTRAINT [PK_R_SDEventSDReader]
    PRIMARY KEY CLUSTERED ([sdEventId], [sdReaderId] ASC);
GO

-- Creating primary key on [r_SDReaderId] in table 'R_SDReader'
ALTER TABLE [dbo].[R_SDReader]
ADD CONSTRAINT [PK_R_SDReader]
    PRIMARY KEY CLUSTERED ([r_SDReaderId] ASC);
GO

-- Creating primary key on [L_sdAnalysisParameterId] in table 'L_SDAnalysisParameter'
ALTER TABLE [dbo].[L_SDAnalysisParameter]
ADD CONSTRAINT [PK_L_SDAnalysisParameter]
    PRIMARY KEY CLUSTERED ([L_sdAnalysisParameterId] ASC);
GO

-- Creating primary key on [L_selectionDeviceSourceId] in table 'L_SelectionDeviceSource'
ALTER TABLE [dbo].[L_SelectionDeviceSource]
ADD CONSTRAINT [PK_L_SelectionDeviceSource]
    PRIMARY KEY CLUSTERED ([L_selectionDeviceSourceId] ASC);
GO

-- Creating primary key on [L_lengthMeasureTypeId] in table 'L_LengthMeasureType'
ALTER TABLE [dbo].[L_LengthMeasureType]
ADD CONSTRAINT [PK_L_LengthMeasureType]
    PRIMARY KEY CLUSTERED ([L_lengthMeasureTypeId] ASC);
GO

-- Creating primary key on [Reports_reportId], [ReportingTreeNodes_reportingTreeNodeId] in table 'R_ReportingTreeNodeReport'
ALTER TABLE [dbo].[R_ReportingTreeNodeReport]
ADD CONSTRAINT [PK_R_ReportingTreeNodeReport]
    PRIMARY KEY CLUSTERED ([Reports_reportId], [ReportingTreeNodes_reportingTreeNodeId] ASC);
GO

-- Creating primary key on [L_DFUAreas_DFUArea], [SDEvent_sdEventId] in table 'R_SDEventDFUArea'
ALTER TABLE [dbo].[R_SDEventDFUArea]
ADD CONSTRAINT [PK_R_SDEventDFUArea]
    PRIMARY KEY CLUSTERED ([L_DFUAreas_DFUArea], [SDEvent_sdEventId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [animalId] in table 'Age'
ALTER TABLE [dbo].[Age]
ADD CONSTRAINT [FK_Age_Animal]
    FOREIGN KEY ([animalId])
    REFERENCES [dbo].[Animal]
        ([animalId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Age_Animal'
CREATE INDEX [IX_FK_Age_Animal]
ON [dbo].[Age]
    ([animalId]);
GO

-- Creating foreign key on [ageMeasureMethodId] in table 'Age'
ALTER TABLE [dbo].[Age]
ADD CONSTRAINT [FK_Age_L_AgeMeasure]
    FOREIGN KEY ([ageMeasureMethodId])
    REFERENCES [dbo].[L_AgeMeasureMethod]
        ([L_ageMeasureMethodId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Age_L_AgeMeasure'
CREATE INDEX [IX_FK_Age_L_AgeMeasure]
ON [dbo].[Age]
    ([ageMeasureMethodId]);
GO

-- Creating foreign key on [otolithReadingRemarkId] in table 'Age'
ALTER TABLE [dbo].[Age]
ADD CONSTRAINT [FK_Age_L_OtolithReadingRemark]
    FOREIGN KEY ([otolithReadingRemarkId])
    REFERENCES [dbo].[L_OtolithReadingRemark]
        ([L_OtolithReadingRemarkID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Age_L_OtolithReadingRemark'
CREATE INDEX [IX_FK_Age_L_OtolithReadingRemark]
ON [dbo].[Age]
    ([otolithReadingRemarkId]);
GO

-- Creating foreign key on [broodingPhase] in table 'Animal'
ALTER TABLE [dbo].[Animal]
ADD CONSTRAINT [FK_Animal_L_BroodingPhase]
    FOREIGN KEY ([broodingPhase])
    REFERENCES [dbo].[L_BroodingPhase]
        ([broodingPhase])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Animal_L_BroodingPhase'
CREATE INDEX [IX_FK_Animal_L_BroodingPhase]
ON [dbo].[Animal]
    ([broodingPhase]);
GO

-- Creating foreign key on [lengthMeasureUnit] in table 'Animal'
ALTER TABLE [dbo].[Animal]
ADD CONSTRAINT [FK_Animal_L_LengthMeasureUnit]
    FOREIGN KEY ([lengthMeasureUnit])
    REFERENCES [dbo].[L_LengthMeasureUnit]
        ([lengthMeasureUnit])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Animal_L_LengthMeasureUnit'
CREATE INDEX [IX_FK_Animal_L_LengthMeasureUnit]
ON [dbo].[Animal]
    ([lengthMeasureUnit]);
GO

-- Creating foreign key on [sexCode] in table 'Animal'
ALTER TABLE [dbo].[Animal]
ADD CONSTRAINT [FK_Animal_L_SexCode]
    FOREIGN KEY ([sexCode])
    REFERENCES [dbo].[L_SexCode]
        ([sexCode])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Animal_L_SexCode'
CREATE INDEX [IX_FK_Animal_L_SexCode]
ON [dbo].[Animal]
    ([sexCode]);
GO

-- Creating foreign key on [subSampleId] in table 'Animal'
ALTER TABLE [dbo].[Animal]
ADD CONSTRAINT [FK_Animal_SubSample]
    FOREIGN KEY ([subSampleId])
    REFERENCES [dbo].[SubSample]
        ([subSampleId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Animal_SubSample'
CREATE INDEX [IX_FK_Animal_SubSample]
ON [dbo].[Animal]
    ([subSampleId]);
GO

-- Creating foreign key on [animalId] in table 'AnimalInfo'
ALTER TABLE [dbo].[AnimalInfo]
ADD CONSTRAINT [FK_AnimalInfo_Animal]
    FOREIGN KEY ([animalId])
    REFERENCES [dbo].[Animal]
        ([animalId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AnimalInfo_Animal'
CREATE INDEX [IX_FK_AnimalInfo_Animal]
ON [dbo].[AnimalInfo]
    ([animalId]);
GO

-- Creating foreign key on [fatId] in table 'AnimalInfo'
ALTER TABLE [dbo].[AnimalInfo]
ADD CONSTRAINT [FK_AnimalInfo_Fat]
    FOREIGN KEY ([fatId])
    REFERENCES [dbo].[Fat]
        ([fatId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AnimalInfo_Fat'
CREATE INDEX [IX_FK_AnimalInfo_Fat]
ON [dbo].[AnimalInfo]
    ([fatId]);
GO

-- Creating foreign key on [maturityId] in table 'AnimalInfo'
ALTER TABLE [dbo].[AnimalInfo]
ADD CONSTRAINT [FK_AnimalInfo_Maturity]
    FOREIGN KEY ([maturityId])
    REFERENCES [dbo].[Maturity]
        ([maturityId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AnimalInfo_Maturity'
CREATE INDEX [IX_FK_AnimalInfo_Maturity]
ON [dbo].[AnimalInfo]
    ([maturityId]);
GO

-- Creating foreign key on [dataHandlerId] in table 'Cruise'
ALTER TABLE [dbo].[Cruise]
ADD CONSTRAINT [FK_Cruise_DFUPerson]
    FOREIGN KEY ([dataHandlerId])
    REFERENCES [dbo].[DFUPerson]
        ([dfuPersonId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Cruise_DFUPerson'
CREATE INDEX [IX_FK_Cruise_DFUPerson]
ON [dbo].[Cruise]
    ([dataHandlerId]);
GO

-- Creating foreign key on [cruiseStatus] in table 'Cruise'
ALTER TABLE [dbo].[Cruise]
ADD CONSTRAINT [FK_Cruise_L_CruiseStatus]
    FOREIGN KEY ([cruiseStatus])
    REFERENCES [dbo].[L_CruiseStatus]
        ([cruiseStatus])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Cruise_L_CruiseStatus'
CREATE INDEX [IX_FK_Cruise_L_CruiseStatus]
ON [dbo].[Cruise]
    ([cruiseStatus]);
GO

-- Creating foreign key on [responsibleId] in table 'Cruise'
ALTER TABLE [dbo].[Cruise]
ADD CONSTRAINT [FK_Cruise_L_DfuPerson]
    FOREIGN KEY ([responsibleId])
    REFERENCES [dbo].[DFUPerson]
        ([dfuPersonId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Cruise_L_DfuPerson'
CREATE INDEX [IX_FK_Cruise_L_DfuPerson]
ON [dbo].[Cruise]
    ([responsibleId]);
GO

-- Creating foreign key on [DFUDepartment] in table 'Cruise'
ALTER TABLE [dbo].[Cruise]
ADD CONSTRAINT [FK_Cruise_L_Institute]
    FOREIGN KEY ([DFUDepartment])
    REFERENCES [dbo].[L_DFUDepartment]
        ([dfuDepartment])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Cruise_L_Institute'
CREATE INDEX [IX_FK_Cruise_L_Institute]
ON [dbo].[Cruise]
    ([DFUDepartment]);
GO

-- Creating foreign key on [cruiseId] in table 'Est_Method'
ALTER TABLE [dbo].[Est_Method]
ADD CONSTRAINT [FK_Est_Method_Cruise]
    FOREIGN KEY ([cruiseId])
    REFERENCES [dbo].[Cruise]
        ([cruiseId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Est_Method_Cruise'
CREATE INDEX [IX_FK_Est_Method_Cruise]
ON [dbo].[Est_Method]
    ([cruiseId]);
GO

-- Creating foreign key on [cruiseId] in table 'Est_Strata'
ALTER TABLE [dbo].[Est_Strata]
ADD CONSTRAINT [FK_Est_Strata_Cruise]
    FOREIGN KEY ([cruiseId])
    REFERENCES [dbo].[Cruise]
        ([cruiseId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Est_Strata_Cruise'
CREATE INDEX [IX_FK_Est_Strata_Cruise]
ON [dbo].[Est_Strata]
    ([cruiseId]);
GO

-- Creating foreign key on [delete_CruiseId] in table 'Est_Strata'
ALTER TABLE [dbo].[Est_Strata]
ADD CONSTRAINT [FK_Est_Strata_Cruise1]
    FOREIGN KEY ([delete_CruiseId])
    REFERENCES [dbo].[Cruise]
        ([cruiseId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Est_Strata_Cruise1'
CREATE INDEX [IX_FK_Est_Strata_Cruise1]
ON [dbo].[Est_Strata]
    ([delete_CruiseId]);
GO

-- Creating foreign key on [cruiseId] in table 'R_CruiseUsabilityParam'
ALTER TABLE [dbo].[R_CruiseUsabilityParam]
ADD CONSTRAINT [FK_R_CruiseUsabilityParam_Cruise]
    FOREIGN KEY ([cruiseId])
    REFERENCES [dbo].[Cruise]
        ([cruiseId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_CruiseUsabilityParam_Cruise'
CREATE INDEX [IX_FK_R_CruiseUsabilityParam_Cruise]
ON [dbo].[R_CruiseUsabilityParam]
    ([cruiseId]);
GO

-- Creating foreign key on [cruiseId] in table 'Trip'
ALTER TABLE [dbo].[Trip]
ADD CONSTRAINT [FK_Trip_Cruise]
    FOREIGN KEY ([cruiseId])
    REFERENCES [dbo].[Cruise]
        ([cruiseId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Trip_Cruise'
CREATE INDEX [IX_FK_Trip_Cruise]
ON [dbo].[Trip]
    ([cruiseId]);
GO

-- Creating foreign key on [dfuDepartment] in table 'DFUPerson'
ALTER TABLE [dbo].[DFUPerson]
ADD CONSTRAINT [FK_DFUPerson_L_DFUDepartment]
    FOREIGN KEY ([dfuDepartment])
    REFERENCES [dbo].[L_DFUDepartment]
        ([dfuDepartment])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DFUPerson_L_DFUDepartment'
CREATE INDEX [IX_FK_DFUPerson_L_DFUDepartment]
ON [dbo].[DFUPerson]
    ([dfuDepartment]);
GO

-- Creating foreign key on [datahandlerId] in table 'Est_Strata'
ALTER TABLE [dbo].[Est_Strata]
ADD CONSTRAINT [FK_Est_Strata_DFUPerson]
    FOREIGN KEY ([datahandlerId])
    REFERENCES [dbo].[DFUPerson]
        ([dfuPersonId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Est_Strata_DFUPerson'
CREATE INDEX [IX_FK_Est_Strata_DFUPerson]
ON [dbo].[Est_Strata]
    ([datahandlerId]);
GO

-- Creating foreign key on [samplePersonId] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [FK_Sample_DFUPerson]
    FOREIGN KEY ([samplePersonId])
    REFERENCES [dbo].[DFUPerson]
        ([dfuPersonId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sample_DFUPerson'
CREATE INDEX [IX_FK_Sample_DFUPerson]
ON [dbo].[Sample]
    ([samplePersonId]);
GO

-- Creating foreign key on [analysisPersonId] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [FK_Sample_DFUPerson1]
    FOREIGN KEY ([analysisPersonId])
    REFERENCES [dbo].[DFUPerson]
        ([dfuPersonId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sample_DFUPerson1'
CREATE INDEX [IX_FK_Sample_DFUPerson1]
ON [dbo].[Sample]
    ([analysisPersonId]);
GO

-- Creating foreign key on [ageReadId] in table 'SpeciesList'
ALTER TABLE [dbo].[SpeciesList]
ADD CONSTRAINT [FK_SpeciesList_L_DfuPerson]
    FOREIGN KEY ([ageReadId])
    REFERENCES [dbo].[DFUPerson]
        ([dfuPersonId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SpeciesList_L_DfuPerson'
CREATE INDEX [IX_FK_SpeciesList_L_DfuPerson]
ON [dbo].[SpeciesList]
    ([ageReadId]);
GO

-- Creating foreign key on [datahandlerId] in table 'SpeciesList'
ALTER TABLE [dbo].[SpeciesList]
ADD CONSTRAINT [FK_SpeciesList_L_DfuPerson1]
    FOREIGN KEY ([datahandlerId])
    REFERENCES [dbo].[DFUPerson]
        ([dfuPersonId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SpeciesList_L_DfuPerson1'
CREATE INDEX [IX_FK_SpeciesList_L_DfuPerson1]
ON [dbo].[SpeciesList]
    ([datahandlerId]);
GO

-- Creating foreign key on [tripLeaderId] in table 'Trip'
ALTER TABLE [dbo].[Trip]
ADD CONSTRAINT [FK_Trip_L_DfuPerson]
    FOREIGN KEY ([tripLeaderId])
    REFERENCES [dbo].[DFUPerson]
        ([dfuPersonId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Trip_L_DfuPerson'
CREATE INDEX [IX_FK_Trip_L_DfuPerson]
ON [dbo].[Trip]
    ([tripLeaderId]);
GO

-- Creating foreign key on [dataHandlerId] in table 'Trip'
ALTER TABLE [dbo].[Trip]
ADD CONSTRAINT [FK_Trip_L_DfuPerson1]
    FOREIGN KEY ([dataHandlerId])
    REFERENCES [dbo].[DFUPerson]
        ([dfuPersonId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Trip_L_DfuPerson1'
CREATE INDEX [IX_FK_Trip_L_DfuPerson1]
ON [dbo].[Trip]
    ([dataHandlerId]);
GO

-- Creating foreign key on [DFUArea] in table 'DFUSubArea'
ALTER TABLE [dbo].[DFUSubArea]
ADD CONSTRAINT [FK_DFUSubArea_L_DFUArea]
    FOREIGN KEY ([DFUArea])
    REFERENCES [dbo].[L_DFUArea]
        ([DFUArea])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DFUSubArea_L_DFUArea'
CREATE INDEX [IX_FK_DFUSubArea_L_DFUArea]
ON [dbo].[DFUSubArea]
    ([DFUArea]);
GO

-- Creating foreign key on [Est_MethodId] in table 'Est_MethodStep'
ALTER TABLE [dbo].[Est_MethodStep]
ADD CONSTRAINT [FK_Est_MethodStep_Est_Method]
    FOREIGN KEY ([Est_MethodId])
    REFERENCES [dbo].[Est_Method]
        ([Est_MethodId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Est_MethodStep_Est_Method'
CREATE INDEX [IX_FK_Est_MethodStep_Est_Method]
ON [dbo].[Est_MethodStep]
    ([Est_MethodId]);
GO

-- Creating foreign key on [Est_StrataId] in table 'Est_MethodStep'
ALTER TABLE [dbo].[Est_MethodStep]
ADD CONSTRAINT [FK_Est_MethodStep_Est_Strata]
    FOREIGN KEY ([Est_StrataId])
    REFERENCES [dbo].[Est_Strata]
        ([Est_StrataId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Est_MethodStep_Est_Strata'
CREATE INDEX [IX_FK_Est_MethodStep_Est_Strata]
ON [dbo].[Est_MethodStep]
    ([Est_StrataId]);
GO

-- Creating foreign key on [dfubase_category] in table 'Est_MethodStep'
ALTER TABLE [dbo].[Est_MethodStep]
ADD CONSTRAINT [FK_Est_MethodStep_L_DFU_Category]
    FOREIGN KEY ([dfubase_category])
    REFERENCES [dbo].[L_DFUBase_Category]
        ([dfuBase_Category])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Est_MethodStep_L_DFU_Category'
CREATE INDEX [IX_FK_Est_MethodStep_L_DFU_Category]
ON [dbo].[Est_MethodStep]
    ([dfubase_category]);
GO

-- Creating foreign key on [SpeciesCode] in table 'Est_MethodStep'
ALTER TABLE [dbo].[Est_MethodStep]
ADD CONSTRAINT [FK_Est_MethodStep_L_Species]
    FOREIGN KEY ([SpeciesCode])
    REFERENCES [dbo].[L_Species]
        ([speciesCode])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Est_MethodStep_L_Species'
CREATE INDEX [IX_FK_Est_MethodStep_L_Species]
ON [dbo].[Est_MethodStep]
    ([SpeciesCode]);
GO

-- Creating foreign key on [overwrites] in table 'Est_MethodStep'
ALTER TABLE [dbo].[Est_MethodStep]
ADD CONSTRAINT [FK_Est_MethodStep_L_YesNo]
    FOREIGN KEY ([overwrites])
    REFERENCES [dbo].[L_YesNo]
        ([YesNo])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Est_MethodStep_L_YesNo'
CREATE INDEX [IX_FK_Est_MethodStep_L_YesNo]
ON [dbo].[Est_MethodStep]
    ([overwrites]);
GO

-- Creating foreign key on [dfubase_category] in table 'Est_Strata'
ALTER TABLE [dbo].[Est_Strata]
ADD CONSTRAINT [FK_Est_Strata_L_DFU_Category]
    FOREIGN KEY ([dfubase_category])
    REFERENCES [dbo].[L_DFUBase_Category]
        ([dfuBase_Category])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Est_Strata_L_DFU_Category'
CREATE INDEX [IX_FK_Est_Strata_L_DFU_Category]
ON [dbo].[Est_Strata]
    ([dfubase_category]);
GO

-- Creating foreign key on [representative] in table 'Est_Strata'
ALTER TABLE [dbo].[Est_Strata]
ADD CONSTRAINT [FK_Est_Strata_L_YesNo]
    FOREIGN KEY ([representative])
    REFERENCES [dbo].[L_YesNo]
        ([YesNo])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Est_Strata_L_YesNo'
CREATE INDEX [IX_FK_Est_Strata_L_YesNo]
ON [dbo].[Est_Strata]
    ([representative]);
GO

-- Creating foreign key on [disabled] in table 'Est_Strata'
ALTER TABLE [dbo].[Est_Strata]
ADD CONSTRAINT [FK_Est_Strata_L_YesNo1]
    FOREIGN KEY ([disabled])
    REFERENCES [dbo].[L_YesNo]
        ([YesNo])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Est_Strata_L_YesNo1'
CREATE INDEX [IX_FK_Est_Strata_L_YesNo1]
ON [dbo].[Est_Strata]
    ([disabled]);
GO

-- Creating foreign key on [SampleId] in table 'Est_Strata'
ALTER TABLE [dbo].[Est_Strata]
ADD CONSTRAINT [FK_Est_Strata_Sample]
    FOREIGN KEY ([SampleId])
    REFERENCES [dbo].[Sample]
        ([sampleId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Est_Strata_Sample'
CREATE INDEX [IX_FK_Est_Strata_Sample]
ON [dbo].[Est_Strata]
    ([SampleId]);
GO

-- Creating foreign key on [TripId] in table 'Est_Strata'
ALTER TABLE [dbo].[Est_Strata]
ADD CONSTRAINT [FK_Est_Strata_Trip]
    FOREIGN KEY ([TripId])
    REFERENCES [dbo].[Trip]
        ([tripId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Est_Strata_Trip'
CREATE INDEX [IX_FK_Est_Strata_Trip]
ON [dbo].[Est_Strata]
    ([TripId]);
GO

-- Creating foreign key on [fatIndexMethod] in table 'Fat'
ALTER TABLE [dbo].[Fat]
ADD CONSTRAINT [FK_L_Fatindex_L_FatIndexType]
    FOREIGN KEY ([fatIndexMethod])
    REFERENCES [dbo].[L_FatIndexMethod]
        ([fatIndexMethod])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_Fatindex_L_FatIndexType'
CREATE INDEX [IX_FK_L_Fatindex_L_FatIndexType]
ON [dbo].[Fat]
    ([fatIndexMethod]);
GO

-- Creating foreign key on [catchRegistrationId] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [FK_Sample_L_CatchRegistration]
    FOREIGN KEY ([catchRegistrationId])
    REFERENCES [dbo].[L_CatchRegistration]
        ([catchRegistrationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sample_L_CatchRegistration'
CREATE INDEX [IX_FK_Sample_L_CatchRegistration]
ON [dbo].[Sample]
    ([catchRegistrationId]);
GO

-- Creating foreign key on [cruiseType] in table 'L_TripType'
ALTER TABLE [dbo].[L_TripType]
ADD CONSTRAINT [FK_L_TripType_L_CruiseType]
    FOREIGN KEY ([cruiseType])
    REFERENCES [dbo].[L_CruiseType]
        ([cruiseType])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_TripType_L_CruiseType'
CREATE INDEX [IX_FK_L_TripType_L_CruiseType]
ON [dbo].[L_TripType]
    ([cruiseType]);
GO

-- Creating foreign key on [cuticulaHardness] in table 'SpeciesList'
ALTER TABLE [dbo].[SpeciesList]
ADD CONSTRAINT [FK_SpeciesList_L_CuticulaHardness]
    FOREIGN KEY ([cuticulaHardness])
    REFERENCES [dbo].[L_CuticulaHardness]
        ([cuticulaHardness])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SpeciesList_L_CuticulaHardness'
CREATE INDEX [IX_FK_SpeciesList_L_CuticulaHardness]
ON [dbo].[SpeciesList]
    ([cuticulaHardness]);
GO

-- Creating foreign key on [parentDFUArea] in table 'L_DFUArea'
ALTER TABLE [dbo].[L_DFUArea]
ADD CONSTRAINT [FK_L_DFUArea_L_DFUArea]
    FOREIGN KEY ([parentDFUArea])
    REFERENCES [dbo].[L_DFUArea]
        ([DFUArea])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_DFUArea_L_DFUArea'
CREATE INDEX [IX_FK_L_DFUArea_L_DFUArea]
ON [dbo].[L_DFUArea]
    ([parentDFUArea]);
GO

-- Creating foreign key on [dfuArea] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [FK_Sample_L_DFUArea]
    FOREIGN KEY ([dfuArea])
    REFERENCES [dbo].[L_DFUArea]
        ([DFUArea])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sample_L_DFUArea'
CREATE INDEX [IX_FK_Sample_L_DFUArea]
ON [dbo].[Sample]
    ([dfuArea]);
GO

-- Creating foreign key on [dfuBase_Category] in table 'SpeciesList'
ALTER TABLE [dbo].[SpeciesList]
ADD CONSTRAINT [FK_SpeciesList_L_DFU_Category]
    FOREIGN KEY ([dfuBase_Category])
    REFERENCES [dbo].[L_DFUBase_Category]
        ([dfuBase_Category])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SpeciesList_L_DFU_Category'
CREATE INDEX [IX_FK_SpeciesList_L_DFU_Category]
ON [dbo].[SpeciesList]
    ([dfuBase_Category]);
GO

-- Creating foreign key on [fishingActivityId] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [FK_Sample_L_FishingActivityCategory]
    FOREIGN KEY ([fishingActivityId])
    REFERENCES [dbo].[L_FishingActivityCategory]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sample_L_FishingActivityCategory'
CREATE INDEX [IX_FK_Sample_L_FishingActivityCategory]
ON [dbo].[Sample]
    ([fishingActivityId]);
GO

-- Creating foreign key on [gearType] in table 'L_Gear'
ALTER TABLE [dbo].[L_Gear]
ADD CONSTRAINT [FK_L_Gear_L_GearType]
    FOREIGN KEY ([gearType])
    REFERENCES [dbo].[L_GearType]
        ([gearType])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_Gear_L_GearType'
CREATE INDEX [IX_FK_L_Gear_L_GearType]
ON [dbo].[L_Gear]
    ([gearType]);
GO

-- Creating foreign key on [platform] in table 'L_Gear'
ALTER TABLE [dbo].[L_Gear]
ADD CONSTRAINT [FK_L_Gear_L_Platform1]
    FOREIGN KEY ([platform])
    REFERENCES [dbo].[L_Platform]
        ([platform])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_Gear_L_Platform1'
CREATE INDEX [IX_FK_L_Gear_L_Platform1]
ON [dbo].[L_Gear]
    ([platform]);
GO

-- Creating foreign key on [gearId] in table 'R_GearInfo'
ALTER TABLE [dbo].[R_GearInfo]
ADD CONSTRAINT [FK_R_GearInfo_L_Gear]
    FOREIGN KEY ([gearId])
    REFERENCES [dbo].[L_Gear]
        ([gearId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_GearInfo_L_Gear'
CREATE INDEX [IX_FK_R_GearInfo_L_Gear]
ON [dbo].[R_GearInfo]
    ([gearId]);
GO

-- Creating foreign key on [gearId] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [FK_Sample_L_Gear]
    FOREIGN KEY ([gearId])
    REFERENCES [dbo].[L_Gear]
        ([gearId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sample_L_Gear'
CREATE INDEX [IX_FK_Sample_L_Gear]
ON [dbo].[Sample]
    ([gearId]);
GO

-- Creating foreign key on [gearInfoType] in table 'R_GearInfo'
ALTER TABLE [dbo].[R_GearInfo]
ADD CONSTRAINT [FK_R_GearInfo_L_GearParam]
    FOREIGN KEY ([gearInfoType])
    REFERENCES [dbo].[L_GearInfoType]
        ([gearInfoType])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_GearInfo_L_GearParam'
CREATE INDEX [IX_FK_R_GearInfo_L_GearParam]
ON [dbo].[R_GearInfo]
    ([gearInfoType]);
GO

-- Creating foreign key on [catchOperation] in table 'L_GearType'
ALTER TABLE [dbo].[L_GearType]
ADD CONSTRAINT [FK_L_GearType_L_CatchOperation]
    FOREIGN KEY ([catchOperation])
    REFERENCES [dbo].[L_SampleType]
        ([sampleType])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_GearType_L_CatchOperation'
CREATE INDEX [IX_FK_L_GearType_L_CatchOperation]
ON [dbo].[L_GearType]
    ([catchOperation]);
GO

-- Creating foreign key on [HVN_geartype] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [FK_Sample_L_GearType]
    FOREIGN KEY ([HVN_geartype])
    REFERENCES [dbo].[L_GearType]
        ([gearType])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sample_L_GearType'
CREATE INDEX [IX_FK_Sample_L_GearType]
ON [dbo].[Sample]
    ([HVN_geartype]);
GO

-- Creating foreign key on [nationality] in table 'L_Harbour'
ALTER TABLE [dbo].[L_Harbour]
ADD CONSTRAINT [FK_L_Harbour_L_Nationality]
    FOREIGN KEY ([nationality])
    REFERENCES [dbo].[L_Nationality]
        ([nationality])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_Harbour_L_Nationality'
CREATE INDEX [IX_FK_L_Harbour_L_Nationality]
ON [dbo].[L_Harbour]
    ([nationality]);
GO

-- Creating foreign key on [harbourLanding] in table 'Trip'
ALTER TABLE [dbo].[Trip]
ADD CONSTRAINT [FK_Trip_L_Harbour]
    FOREIGN KEY ([harbourLanding])
    REFERENCES [dbo].[L_Harbour]
        ([harbour])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Trip_L_Harbour'
CREATE INDEX [IX_FK_Trip_L_Harbour]
ON [dbo].[Trip]
    ([harbourLanding]);
GO

-- Creating foreign key on [landingCategory] in table 'SpeciesList'
ALTER TABLE [dbo].[SpeciesList]
ADD CONSTRAINT [FK_SpeciesList_L_Category]
    FOREIGN KEY ([landingCategory])
    REFERENCES [dbo].[L_LandingCategory]
        ([landingCategory])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SpeciesList_L_Category'
CREATE INDEX [IX_FK_SpeciesList_L_Category]
ON [dbo].[SpeciesList]
    ([landingCategory]);
GO

-- Creating foreign key on [maturityIndexMethod] in table 'Maturity'
ALTER TABLE [dbo].[Maturity]
ADD CONSTRAINT [FK_R_MaturityMaturityIndex_L_MaturityIndex]
    FOREIGN KEY ([maturityIndexMethod])
    REFERENCES [dbo].[L_MaturityIndexMethod]
        ([maturityIndexMethod])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_MaturityMaturityIndex_L_MaturityIndex'
CREATE INDEX [IX_FK_R_MaturityMaturityIndex_L_MaturityIndex]
ON [dbo].[Maturity]
    ([maturityIndexMethod]);
GO

-- Creating foreign key on [nationality] in table 'L_Platform'
ALTER TABLE [dbo].[L_Platform]
ADD CONSTRAINT [FK_L_Platform_L_Nationality]
    FOREIGN KEY ([nationality])
    REFERENCES [dbo].[L_Nationality]
        ([nationality])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_Platform_L_Nationality'
CREATE INDEX [IX_FK_L_Platform_L_Nationality]
ON [dbo].[L_Platform]
    ([nationality]);
GO

-- Creating foreign key on [nationality] in table 'Trip'
ALTER TABLE [dbo].[Trip]
ADD CONSTRAINT [FK_Trip_L_Nationality]
    FOREIGN KEY ([nationality])
    REFERENCES [dbo].[L_Nationality]
        ([nationality])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Trip_L_Nationality'
CREATE INDEX [IX_FK_Trip_L_Nationality]
ON [dbo].[Trip]
    ([nationality]);
GO

-- Creating foreign key on [navigationSystem] in table 'L_PlatformVersion'
ALTER TABLE [dbo].[L_PlatformVersion]
ADD CONSTRAINT [FK_L_PlatformVersion_L_NavigationSystem]
    FOREIGN KEY ([navigationSystem])
    REFERENCES [dbo].[L_NavigationSystem]
        ([navigationSystem])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_PlatformVersion_L_NavigationSystem'
CREATE INDEX [IX_FK_L_PlatformVersion_L_NavigationSystem]
ON [dbo].[L_PlatformVersion]
    ([navigationSystem]);
GO

-- Creating foreign key on [platformType] in table 'L_Platform'
ALTER TABLE [dbo].[L_Platform]
ADD CONSTRAINT [FK_L_Platform_L_PlatformType]
    FOREIGN KEY ([platformType])
    REFERENCES [dbo].[L_PlatformType]
        ([platformType])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_Platform_L_PlatformType'
CREATE INDEX [IX_FK_L_Platform_L_PlatformType]
ON [dbo].[L_Platform]
    ([platformType]);
GO

-- Creating foreign key on [contactPersonId] in table 'L_Platform'
ALTER TABLE [dbo].[L_Platform]
ADD CONSTRAINT [FK_L_Platform_Person]
    FOREIGN KEY ([contactPersonId])
    REFERENCES [dbo].[Person]
        ([personId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_Platform_Person'
CREATE INDEX [IX_FK_L_Platform_Person]
ON [dbo].[L_Platform]
    ([contactPersonId]);
GO

-- Creating foreign key on [platform] in table 'L_PlatformVersion'
ALTER TABLE [dbo].[L_PlatformVersion]
ADD CONSTRAINT [FK_L_PlatformVersion_L_Platform]
    FOREIGN KEY ([platform])
    REFERENCES [dbo].[L_Platform]
        ([platform])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_PlatformVersion_L_Platform'
CREATE INDEX [IX_FK_L_PlatformVersion_L_Platform]
ON [dbo].[L_PlatformVersion]
    ([platform]);
GO

-- Creating foreign key on [platformVersionId] in table 'R_TripPlatformVersion'
ALTER TABLE [dbo].[R_TripPlatformVersion]
ADD CONSTRAINT [FK_R_platformVersion_L_PlatformVersion]
    FOREIGN KEY ([platformVersionId])
    REFERENCES [dbo].[L_PlatformVersion]
        ([platformVersionId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_platformVersion_L_PlatformVersion'
CREATE INDEX [IX_FK_R_platformVersion_L_PlatformVersion]
ON [dbo].[R_TripPlatformVersion]
    ([platformVersionId]);
GO

-- Creating foreign key on [sampleStatus] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [FK_Sample_L_SampleStatus]
    FOREIGN KEY ([sampleStatus])
    REFERENCES [dbo].[L_SampleStatus]
        ([sampleStatus])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sample_L_SampleStatus'
CREATE INDEX [IX_FK_Sample_L_SampleStatus]
ON [dbo].[Sample]
    ([sampleStatus]);
GO

-- Creating foreign key on [samplingMethodId] in table 'Trip'
ALTER TABLE [dbo].[Trip]
ADD CONSTRAINT [FK_Trip_L_SamplingMethod]
    FOREIGN KEY ([samplingMethodId])
    REFERENCES [dbo].[L_SamplingMethod]
        ([samplingMethodId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Trip_L_SamplingMethod'
CREATE INDEX [IX_FK_Trip_L_SamplingMethod]
ON [dbo].[Trip]
    ([samplingMethodId]);
GO

-- Creating foreign key on [samplingTypeId] in table 'Trip'
ALTER TABLE [dbo].[Trip]
ADD CONSTRAINT [FK_Trip_L_SamplingType]
    FOREIGN KEY ([samplingTypeId])
    REFERENCES [dbo].[L_SamplingType]
        ([samplingTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Trip_L_SamplingType'
CREATE INDEX [IX_FK_Trip_L_SamplingType]
ON [dbo].[Trip]
    ([samplingTypeId]);
GO

-- Creating foreign key on [sexCode] in table 'SpeciesList'
ALTER TABLE [dbo].[SpeciesList]
ADD CONSTRAINT [FK_SpeciesList_L_SexCode]
    FOREIGN KEY ([sexCode])
    REFERENCES [dbo].[L_SexCode]
        ([sexCode])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SpeciesList_L_SexCode'
CREATE INDEX [IX_FK_SpeciesList_L_SexCode]
ON [dbo].[SpeciesList]
    ([sexCode]);
GO

-- Creating foreign key on [sizeSortingDFU] in table 'SpeciesList'
ALTER TABLE [dbo].[SpeciesList]
ADD CONSTRAINT [FK_SpeciesList_L_SizeSortingDFU]
    FOREIGN KEY ([sizeSortingDFU])
    REFERENCES [dbo].[L_SizeSortingDFU]
        ([sizeSortingDFU])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SpeciesList_L_SizeSortingDFU'
CREATE INDEX [IX_FK_SpeciesList_L_SizeSortingDFU]
ON [dbo].[SpeciesList]
    ([sizeSortingDFU]);
GO

-- Creating foreign key on [sizeSortingEU] in table 'SpeciesList'
ALTER TABLE [dbo].[SpeciesList]
ADD CONSTRAINT [FK_SpeciesList_L_SizeSortingEU]
    FOREIGN KEY ([sizeSortingEU])
    REFERENCES [dbo].[L_SizeSortingEU]
        ([sizeSortingEU])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SpeciesList_L_SizeSortingEU'
CREATE INDEX [IX_FK_SpeciesList_L_SizeSortingEU]
ON [dbo].[SpeciesList]
    ([sizeSortingEU]);
GO

-- Creating foreign key on [treatmentFactorGroup] in table 'L_Species'
ALTER TABLE [dbo].[L_Species]
ADD CONSTRAINT [FK_L_Species_L_CleaningGroup]
    FOREIGN KEY ([treatmentFactorGroup])
    REFERENCES [dbo].[L_TreatmentFactorGroup]
        ([treatmentFactorGroup])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_Species_L_CleaningGroup'
CREATE INDEX [IX_FK_L_Species_L_CleaningGroup]
ON [dbo].[L_Species]
    ([treatmentFactorGroup]);
GO

-- Creating foreign key on [speciesCode] in table 'R_TargetSpecies'
ALTER TABLE [dbo].[R_TargetSpecies]
ADD CONSTRAINT [FK_R_targetSpecies_L_Species]
    FOREIGN KEY ([speciesCode])
    REFERENCES [dbo].[L_Species]
        ([speciesCode])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_targetSpecies_L_Species'
CREATE INDEX [IX_FK_R_targetSpecies_L_Species]
ON [dbo].[R_TargetSpecies]
    ([speciesCode]);
GO

-- Creating foreign key on [speciesCode] in table 'R_TargetSpecies'
ALTER TABLE [dbo].[R_TargetSpecies]
ADD CONSTRAINT [FK_R_targetSpecies_L_Species1]
    FOREIGN KEY ([speciesCode])
    REFERENCES [dbo].[L_Species]
        ([speciesCode])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_targetSpecies_L_Species1'
CREATE INDEX [IX_FK_R_targetSpecies_L_Species1]
ON [dbo].[R_TargetSpecies]
    ([speciesCode]);
GO

-- Creating foreign key on [speciesCode] in table 'SpeciesList'
ALTER TABLE [dbo].[SpeciesList]
ADD CONSTRAINT [FK_SpeciesList_L_Species]
    FOREIGN KEY ([speciesCode])
    REFERENCES [dbo].[L_Species]
        ([speciesCode])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SpeciesList_L_Species'
CREATE INDEX [IX_FK_SpeciesList_L_Species]
ON [dbo].[SpeciesList]
    ([speciesCode]);
GO

-- Creating foreign key on [speciesRegistrationId] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [FK_Sample_L_SpeciesRegistration]
    FOREIGN KEY ([speciesRegistrationId])
    REFERENCES [dbo].[L_SpeciesRegistration]
        ([speciesRegistrationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sample_L_SpeciesRegistration'
CREATE INDEX [IX_FK_Sample_L_SpeciesRegistration]
ON [dbo].[Sample]
    ([speciesRegistrationId]);
GO

-- Creating foreign key on [statisticalRectangle] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [FK_Sample_L_StatisticalRectangle]
    FOREIGN KEY ([statisticalRectangle])
    REFERENCES [dbo].[L_StatisticalRectangle]
        ([statisticalRectangle])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sample_L_StatisticalRectangle'
CREATE INDEX [IX_FK_Sample_L_StatisticalRectangle]
ON [dbo].[Sample]
    ([statisticalRectangle]);
GO

-- Creating foreign key on [treatment] in table 'SpeciesList'
ALTER TABLE [dbo].[SpeciesList]
ADD CONSTRAINT [FK_SpeciesList_L_Treatment]
    FOREIGN KEY ([treatment])
    REFERENCES [dbo].[L_Treatment]
        ([treatment])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SpeciesList_L_Treatment'
CREATE INDEX [IX_FK_SpeciesList_L_Treatment]
ON [dbo].[SpeciesList]
    ([treatment]);
GO

-- Creating foreign key on [treatment] in table 'TreatmentFactor'
ALTER TABLE [dbo].[TreatmentFactor]
ADD CONSTRAINT [FK_TreatmentFactor_L_Treatment]
    FOREIGN KEY ([treatment])
    REFERENCES [dbo].[L_Treatment]
        ([treatment])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TreatmentFactor_L_Treatment'
CREATE INDEX [IX_FK_TreatmentFactor_L_Treatment]
ON [dbo].[TreatmentFactor]
    ([treatment]);
GO

-- Creating foreign key on [treatmentFactorGroup] in table 'TreatmentFactor'
ALTER TABLE [dbo].[TreatmentFactor]
ADD CONSTRAINT [FK_TreatmentFactor_L_treatmentFactorGroup]
    FOREIGN KEY ([treatmentFactorGroup])
    REFERENCES [dbo].[L_TreatmentFactorGroup]
        ([treatmentFactorGroup])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TreatmentFactor_L_treatmentFactorGroup'
CREATE INDEX [IX_FK_TreatmentFactor_L_treatmentFactorGroup]
ON [dbo].[TreatmentFactor]
    ([treatmentFactorGroup]);
GO

-- Creating foreign key on [tripType] in table 'Trip'
ALTER TABLE [dbo].[Trip]
ADD CONSTRAINT [FK_Trip_L_TripType]
    FOREIGN KEY ([tripType])
    REFERENCES [dbo].[L_TripType]
        ([tripType])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Trip_L_TripType'
CREATE INDEX [IX_FK_Trip_L_TripType]
ON [dbo].[Trip]
    ([tripType]);
GO

-- Creating foreign key on [usabilityParamId] in table 'R_CruiseUsabilityParam'
ALTER TABLE [dbo].[R_CruiseUsabilityParam]
ADD CONSTRAINT [FK_R_CruiseUsabilityParam_UsabilityParam]
    FOREIGN KEY ([usabilityParamId])
    REFERENCES [dbo].[L_UsabilityParam]
        ([usabilityParamId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_CruiseUsabilityParam_UsabilityParam'
CREATE INDEX [IX_FK_R_CruiseUsabilityParam_UsabilityParam]
ON [dbo].[R_CruiseUsabilityParam]
    ([usabilityParamId]);
GO

-- Creating foreign key on [usabilityParamId] in table 'R_SampleUsabilityParam'
ALTER TABLE [dbo].[R_SampleUsabilityParam]
ADD CONSTRAINT [FK_R_SampleUsabilityParam_L_UsabilityParam]
    FOREIGN KEY ([usabilityParamId])
    REFERENCES [dbo].[L_UsabilityParam]
        ([usabilityParamId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_SampleUsabilityParam_L_UsabilityParam'
CREATE INDEX [IX_FK_R_SampleUsabilityParam_L_UsabilityParam]
ON [dbo].[R_SampleUsabilityParam]
    ([usabilityParamId]);
GO

-- Creating foreign key on [usabilityParamId] in table 'R_TripUsabilityParam'
ALTER TABLE [dbo].[R_TripUsabilityParam]
ADD CONSTRAINT [FK_R_TripUsabilityParam_L_UsabilityParam]
    FOREIGN KEY ([usabilityParamId])
    REFERENCES [dbo].[L_UsabilityParam]
        ([usabilityParamId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_TripUsabilityParam_L_UsabilityParam'
CREATE INDEX [IX_FK_R_TripUsabilityParam_L_UsabilityParam]
ON [dbo].[R_TripUsabilityParam]
    ([usabilityParamId]);
GO

-- Creating foreign key on [usabilityParamId] in table 'R_UsabilityParamUsabilityGrp'
ALTER TABLE [dbo].[R_UsabilityParamUsabilityGrp]
ADD CONSTRAINT [FK_R_UsabilityParamUsabilityGrp_UsabilityParam]
    FOREIGN KEY ([usabilityParamId])
    REFERENCES [dbo].[L_UsabilityParam]
        ([usabilityParamId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_UsabilityParamUsabilityGrp_UsabilityParam'
CREATE INDEX [IX_FK_R_UsabilityParamUsabilityGrp_UsabilityParam]
ON [dbo].[R_UsabilityParamUsabilityGrp]
    ([usabilityParamId]);
GO

-- Creating foreign key on [usabilityParamGrpId] in table 'R_UsabilityParamUsabilityGrp'
ALTER TABLE [dbo].[R_UsabilityParamUsabilityGrp]
ADD CONSTRAINT [FK_R_UsabilityParamUsabilityGrp_L_UsabilityParamGrp]
    FOREIGN KEY ([usabilityParamGrpId])
    REFERENCES [dbo].[L_UsabilityParamGrp]
        ([usabilityParamGrpId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_UsabilityParamUsabilityGrp_L_UsabilityParamGrp'
CREATE INDEX [IX_FK_R_UsabilityParamUsabilityGrp_L_UsabilityParamGrp]
ON [dbo].[R_UsabilityParamUsabilityGrp]
    ([usabilityParamGrpId]);
GO

-- Creating foreign key on [representative] in table 'SubSample'
ALTER TABLE [dbo].[SubSample]
ADD CONSTRAINT [FK_SubSample_L_YesNo]
    FOREIGN KEY ([representative])
    REFERENCES [dbo].[L_YesNo]
        ([YesNo])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SubSample_L_YesNo'
CREATE INDEX [IX_FK_SubSample_L_YesNo]
ON [dbo].[SubSample]
    ([representative]);
GO

-- Creating foreign key on [tripId] in table 'NumberOfStationsPerTrip'
ALTER TABLE [dbo].[NumberOfStationsPerTrip]
ADD CONSTRAINT [FK_NumOfStationsPerTrip_Trip]
    FOREIGN KEY ([tripId])
    REFERENCES [dbo].[Trip]
        ([tripId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_NumOfStationsPerTrip_Trip'
CREATE INDEX [IX_FK_NumOfStationsPerTrip_Trip]
ON [dbo].[NumberOfStationsPerTrip]
    ([tripId]);
GO

-- Creating foreign key on [contactPersonId] in table 'Trip'
ALTER TABLE [dbo].[Trip]
ADD CONSTRAINT [FK_Trip_L_Person]
    FOREIGN KEY ([contactPersonId])
    REFERENCES [dbo].[Person]
        ([personId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Trip_L_Person'
CREATE INDEX [IX_FK_Trip_L_Person]
ON [dbo].[Trip]
    ([contactPersonId]);
GO

-- Creating foreign key on [SampleId] in table 'R_SampleUsabilityParam'
ALTER TABLE [dbo].[R_SampleUsabilityParam]
ADD CONSTRAINT [FK_R_SampleUsabilityParam_Sample]
    FOREIGN KEY ([SampleId])
    REFERENCES [dbo].[Sample]
        ([sampleId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_SampleUsabilityParam_Sample'
CREATE INDEX [IX_FK_R_SampleUsabilityParam_Sample]
ON [dbo].[R_SampleUsabilityParam]
    ([SampleId]);
GO

-- Creating foreign key on [sampleId] in table 'R_TargetSpecies'
ALTER TABLE [dbo].[R_TargetSpecies]
ADD CONSTRAINT [FK_R_targetSpecies_Sample]
    FOREIGN KEY ([sampleId])
    REFERENCES [dbo].[Sample]
        ([sampleId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_targetSpecies_Sample'
CREATE INDEX [IX_FK_R_targetSpecies_Sample]
ON [dbo].[R_TargetSpecies]
    ([sampleId]);
GO

-- Creating foreign key on [tripId] in table 'R_TripPlatformVersion'
ALTER TABLE [dbo].[R_TripPlatformVersion]
ADD CONSTRAINT [FK_R_platformVersion_Trip]
    FOREIGN KEY ([tripId])
    REFERENCES [dbo].[Trip]
        ([tripId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_platformVersion_Trip'
CREATE INDEX [IX_FK_R_platformVersion_Trip]
ON [dbo].[R_TripPlatformVersion]
    ([tripId]);
GO

-- Creating foreign key on [tripId] in table 'R_TripUsabilityParam'
ALTER TABLE [dbo].[R_TripUsabilityParam]
ADD CONSTRAINT [FK_R_TripUsabilityParam_Trip]
    FOREIGN KEY ([tripId])
    REFERENCES [dbo].[Trip]
        ([tripId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_TripUsabilityParam_Trip'
CREATE INDEX [IX_FK_R_TripUsabilityParam_Trip]
ON [dbo].[R_TripUsabilityParam]
    ([tripId]);
GO

-- Creating foreign key on [tripId] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [FK_Sample_Trip]
    FOREIGN KEY ([tripId])
    REFERENCES [dbo].[Trip]
        ([tripId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sample_Trip'
CREATE INDEX [IX_FK_Sample_Trip]
ON [dbo].[Sample]
    ([tripId]);
GO

-- Creating foreign key on [sampleId] in table 'SpeciesList'
ALTER TABLE [dbo].[SpeciesList]
ADD CONSTRAINT [FK_SpeciesList_Sample]
    FOREIGN KEY ([sampleId])
    REFERENCES [dbo].[Sample]
        ([sampleId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SpeciesList_Sample'
CREATE INDEX [IX_FK_SpeciesList_Sample]
ON [dbo].[SpeciesList]
    ([sampleId]);
GO

-- Creating foreign key on [sampleId] in table 'TrawlOperation'
ALTER TABLE [dbo].[TrawlOperation]
ADD CONSTRAINT [FK_TrawlOpr_Sample]
    FOREIGN KEY ([sampleId])
    REFERENCES [dbo].[Sample]
        ([sampleId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TrawlOpr_Sample'
CREATE INDEX [IX_FK_TrawlOpr_Sample]
ON [dbo].[TrawlOperation]
    ([sampleId]);
GO

-- Creating foreign key on [speciesListId] in table 'SubSample'
ALTER TABLE [dbo].[SubSample]
ADD CONSTRAINT [FK_SubSample_SpeciesList]
    FOREIGN KEY ([speciesListId])
    REFERENCES [dbo].[SpeciesList]
        ([speciesListId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SubSample_SpeciesList'
CREATE INDEX [IX_FK_SubSample_SpeciesList]
ON [dbo].[SubSample]
    ([speciesListId]);
GO

-- Creating foreign key on [platform1] in table 'Trip'
ALTER TABLE [dbo].[Trip]
ADD CONSTRAINT [FK_Trip_L_Platform1]
    FOREIGN KEY ([platform1])
    REFERENCES [dbo].[L_Platform]
        ([platform])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Trip_L_Platform1'
CREATE INDEX [IX_FK_Trip_L_Platform1]
ON [dbo].[Trip]
    ([platform1]);
GO

-- Creating foreign key on [platform2] in table 'Trip'
ALTER TABLE [dbo].[Trip]
ADD CONSTRAINT [FK_Trip_L_Platform2]
    FOREIGN KEY ([platform2])
    REFERENCES [dbo].[L_Platform]
        ([platform])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Trip_L_Platform2'
CREATE INDEX [IX_FK_Trip_L_Platform2]
ON [dbo].[Trip]
    ([platform2]);
GO

-- Creating foreign key on [gearType] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [FK_Sample_L_GearType_2]
    FOREIGN KEY ([gearType])
    REFERENCES [dbo].[L_GearType]
        ([gearType])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sample_L_GearType_2'
CREATE INDEX [IX_FK_Sample_L_GearType_2]
ON [dbo].[Sample]
    ([gearType]);
GO

-- Creating foreign key on [selectionDevice] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [FK_Sample_L_SelectionDevice]
    FOREIGN KEY ([selectionDevice])
    REFERENCES [dbo].[L_SelectionDevice]
        ([selectionDevice])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sample_L_SelectionDevice'
CREATE INDEX [IX_FK_Sample_L_SelectionDevice]
ON [dbo].[Sample]
    ([selectionDevice]);
GO

-- Creating foreign key on [gearType] in table 'R_GearTypeSelectionDevice'
ALTER TABLE [dbo].[R_GearTypeSelectionDevice]
ADD CONSTRAINT [FK_R_GearType_L_GearType]
    FOREIGN KEY ([gearType])
    REFERENCES [dbo].[L_GearType]
        ([gearType])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_GearType_L_GearType'
CREATE INDEX [IX_FK_R_GearType_L_GearType]
ON [dbo].[R_GearTypeSelectionDevice]
    ([gearType]);
GO

-- Creating foreign key on [selectionDevice] in table 'R_GearTypeSelectionDevice'
ALTER TABLE [dbo].[R_GearTypeSelectionDevice]
ADD CONSTRAINT [FK_R_SampleGear_L_SelectionDevice]
    FOREIGN KEY ([selectionDevice])
    REFERENCES [dbo].[L_SelectionDevice]
        ([selectionDevice])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_SampleGear_L_SelectionDevice'
CREATE INDEX [IX_FK_R_SampleGear_L_SelectionDevice]
ON [dbo].[R_GearTypeSelectionDevice]
    ([selectionDevice]);
GO

-- Creating foreign key on [landingCategory] in table 'L_FisheryType'
ALTER TABLE [dbo].[L_FisheryType]
ADD CONSTRAINT [FK_L_FisheryType_L_LandingCategory]
    FOREIGN KEY ([landingCategory])
    REFERENCES [dbo].[L_LandingCategory]
        ([landingCategory])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_FisheryType_L_LandingCategory'
CREATE INDEX [IX_FK_L_FisheryType_L_LandingCategory]
ON [dbo].[L_FisheryType]
    ([landingCategory]);
GO

-- Creating foreign key on [fisheryType] in table 'Trip'
ALTER TABLE [dbo].[Trip]
ADD CONSTRAINT [FK_Trip_L_FisheryType]
    FOREIGN KEY ([fisheryType])
    REFERENCES [dbo].[L_FisheryType]
        ([fisheryType])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Trip_L_FisheryType'
CREATE INDEX [IX_FK_Trip_L_FisheryType]
ON [dbo].[Trip]
    ([fisheryType]);
GO

-- Creating foreign key on [haulType] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [FK_Sample_L_HaulType]
    FOREIGN KEY ([haulType])
    REFERENCES [dbo].[L_HaulType]
        ([haulType])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sample_L_HaulType'
CREATE INDEX [IX_FK_Sample_L_HaulType]
ON [dbo].[Sample]
    ([haulType]);
GO

-- Creating foreign key on [thermoCline] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [FK_Sample_L_ThermoCline]
    FOREIGN KEY ([thermoCline])
    REFERENCES [dbo].[L_ThermoCline]
        ([thermoCline])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sample_L_ThermoCline'
CREATE INDEX [IX_FK_Sample_L_ThermoCline]
ON [dbo].[Sample]
    ([thermoCline]);
GO

-- Creating foreign key on [timeZone] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [FK_Sample_L_TimeZone]
    FOREIGN KEY ([timeZone])
    REFERENCES [dbo].[L_TimeZone]
        ([timeZone])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sample_L_TimeZone'
CREATE INDEX [IX_FK_Sample_L_TimeZone]
ON [dbo].[Sample]
    ([timeZone]);
GO

-- Creating foreign key on [timeZone] in table 'Trip'
ALTER TABLE [dbo].[Trip]
ADD CONSTRAINT [FK_Trip_L_TimeZone]
    FOREIGN KEY ([timeZone])
    REFERENCES [dbo].[L_TimeZone]
        ([timeZone])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Trip_L_TimeZone'
CREATE INDEX [IX_FK_Trip_L_TimeZone]
ON [dbo].[Trip]
    ([timeZone]);
GO

-- Creating foreign key on [weightEstimationMethod] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [FK_Sample_L_WeightEstimationMethod]
    FOREIGN KEY ([weightEstimationMethod])
    REFERENCES [dbo].[L_WeightEstimationMethod]
        ([weightEstimationMethod])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sample_L_WeightEstimationMethod'
CREATE INDEX [IX_FK_Sample_L_WeightEstimationMethod]
ON [dbo].[Sample]
    ([weightEstimationMethod]);
GO

-- Creating foreign key on [weightEstimationMethod] in table 'SpeciesList'
ALTER TABLE [dbo].[SpeciesList]
ADD CONSTRAINT [FK_SpeciesList_L_WeightEstimationMethod]
    FOREIGN KEY ([weightEstimationMethod])
    REFERENCES [dbo].[L_WeightEstimationMethod]
        ([weightEstimationMethod])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SpeciesList_L_WeightEstimationMethod'
CREATE INDEX [IX_FK_SpeciesList_L_WeightEstimationMethod]
ON [dbo].[SpeciesList]
    ([weightEstimationMethod]);
GO

-- Creating foreign key on [edgeStructure] in table 'Age'
ALTER TABLE [dbo].[Age]
ADD CONSTRAINT [FK_Age_L_EdgeStructure]
    FOREIGN KEY ([edgeStructure])
    REFERENCES [dbo].[L_EdgeStructure]
        ([edgeStructure])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Age_L_EdgeStructure'
CREATE INDEX [IX_FK_Age_L_EdgeStructure]
ON [dbo].[Age]
    ([edgeStructure]);
GO

-- Creating foreign key on [parasiteId] in table 'AnimalInfo'
ALTER TABLE [dbo].[AnimalInfo]
ADD CONSTRAINT [FK_AnimalInfo_L_Parasite]
    FOREIGN KEY ([parasiteId])
    REFERENCES [dbo].[L_Parasite]
        ([L_parasiteId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AnimalInfo_L_Parasite'
CREATE INDEX [IX_FK_AnimalInfo_L_Parasite]
ON [dbo].[AnimalInfo]
    ([parasiteId]);
GO

-- Creating foreign key on [animalInfoId] in table 'R_AnimalInfoReference'
ALTER TABLE [dbo].[R_AnimalInfoReference]
ADD CONSTRAINT [FK_R_AnimalInfoReference_AnimalInfo]
    FOREIGN KEY ([animalInfoId])
    REFERENCES [dbo].[AnimalInfo]
        ([animalInfoId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_AnimalInfoReference_AnimalInfo'
CREATE INDEX [IX_FK_R_AnimalInfoReference_AnimalInfo]
ON [dbo].[R_AnimalInfoReference]
    ([animalInfoId]);
GO

-- Creating foreign key on [L_referenceId] in table 'R_AnimalInfoReference'
ALTER TABLE [dbo].[R_AnimalInfoReference]
ADD CONSTRAINT [FK_R_AnimalInfoReference_L_Reference]
    FOREIGN KEY ([L_referenceId])
    REFERENCES [dbo].[L_Reference]
        ([L_referenceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_AnimalInfoReference_L_Reference'
CREATE INDEX [IX_FK_R_AnimalInfoReference_L_Reference]
ON [dbo].[R_AnimalInfoReference]
    ([L_referenceId]);
GO

-- Creating foreign key on [maturityReaderId] in table 'SpeciesList'
ALTER TABLE [dbo].[SpeciesList]
ADD CONSTRAINT [FK_SpeciesList_DFUPerson_MaturityReader]
    FOREIGN KEY ([maturityReaderId])
    REFERENCES [dbo].[DFUPerson]
        ([dfuPersonId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SpeciesList_DFUPerson_MaturityReader'
CREATE INDEX [IX_FK_SpeciesList_DFUPerson_MaturityReader]
ON [dbo].[SpeciesList]
    ([maturityReaderId]);
GO

-- Creating foreign key on [hatchMonthReaderId] in table 'SpeciesList'
ALTER TABLE [dbo].[SpeciesList]
ADD CONSTRAINT [FK_SpeciesList_DFUPerson_MontReader]
    FOREIGN KEY ([hatchMonthReaderId])
    REFERENCES [dbo].[DFUPerson]
        ([dfuPersonId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SpeciesList_DFUPerson_MontReader'
CREATE INDEX [IX_FK_SpeciesList_DFUPerson_MontReader]
ON [dbo].[SpeciesList]
    ([hatchMonthReaderId]);
GO

-- Creating foreign key on [parentTreeNodeId] in table 'ReportingTreeNode'
ALTER TABLE [dbo].[ReportingTreeNode]
ADD CONSTRAINT [FK_ReportingTreeNode_ReportingTreeNode]
    FOREIGN KEY ([parentTreeNodeId])
    REFERENCES [dbo].[ReportingTreeNode]
        ([reportingTreeNodeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ReportingTreeNode_ReportingTreeNode'
CREATE INDEX [IX_FK_ReportingTreeNode_ReportingTreeNode]
ON [dbo].[ReportingTreeNode]
    ([parentTreeNodeId]);
GO

-- Creating foreign key on [Reports_reportId] in table 'R_ReportingTreeNodeReport'
ALTER TABLE [dbo].[R_ReportingTreeNodeReport]
ADD CONSTRAINT [FK_R_ReportingTreeNodeReport_Report]
    FOREIGN KEY ([Reports_reportId])
    REFERENCES [dbo].[Report]
        ([reportId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [ReportingTreeNodes_reportingTreeNodeId] in table 'R_ReportingTreeNodeReport'
ALTER TABLE [dbo].[R_ReportingTreeNodeReport]
ADD CONSTRAINT [FK_R_ReportingTreeNodeReport_ReportingTreeNode]
    FOREIGN KEY ([ReportingTreeNodes_reportingTreeNodeId])
    REFERENCES [dbo].[ReportingTreeNode]
        ([reportingTreeNodeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_ReportingTreeNodeReport_ReportingTreeNode'
CREATE INDEX [IX_FK_R_ReportingTreeNodeReport_ReportingTreeNode]
ON [dbo].[R_ReportingTreeNodeReport]
    ([ReportingTreeNodes_reportingTreeNodeId]);
GO

-- Creating foreign key on [animalId] in table 'AnimalFile'
ALTER TABLE [dbo].[AnimalFile]
ADD CONSTRAINT [FK_AnimalFile_Animal]
    FOREIGN KEY ([animalId])
    REFERENCES [dbo].[Animal]
        ([animalId])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AnimalFile_Animal'
CREATE INDEX [IX_FK_AnimalFile_Animal]
ON [dbo].[AnimalFile]
    ([animalId]);
GO

-- Creating foreign key on [harbourSample] in table 'Trip'
ALTER TABLE [dbo].[Trip]
ADD CONSTRAINT [FK_Trip_L_Harbour_Sample]
    FOREIGN KEY ([harbourSample])
    REFERENCES [dbo].[L_Harbour]
        ([harbour])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Trip_L_Harbour_Sample'
CREATE INDEX [IX_FK_Trip_L_Harbour_Sample]
ON [dbo].[Trip]
    ([harbourSample]);
GO

-- Creating foreign key on [applicationId] in table 'SpeciesList'
ALTER TABLE [dbo].[SpeciesList]
ADD CONSTRAINT [FK_L_ApplicationSpeciesList]
    FOREIGN KEY ([applicationId])
    REFERENCES [dbo].[L_Applications]
        ([L_applicationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_ApplicationSpeciesList'
CREATE INDEX [IX_FK_L_ApplicationSpeciesList]
ON [dbo].[SpeciesList]
    ([applicationId]);
GO

-- Creating foreign key on [hatchMonthReadabilityId] in table 'Age'
ALTER TABLE [dbo].[Age]
ADD CONSTRAINT [FK_L_HatchMonthReadabilityAge]
    FOREIGN KEY ([hatchMonthReadabilityId])
    REFERENCES [dbo].[L_HatchMonthReadability]
        ([L_HatchMonthReadabilityId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_HatchMonthReadabilityAge'
CREATE INDEX [IX_FK_L_HatchMonthReadabilityAge]
ON [dbo].[Age]
    ([hatchMonthReadabilityId]);
GO

-- Creating foreign key on [speciesCode] in table 'L_VisualStock'
ALTER TABLE [dbo].[L_VisualStock]
ADD CONSTRAINT [FK_R_L_VisualStock_L_Species]
    FOREIGN KEY ([speciesCode])
    REFERENCES [dbo].[L_Species]
        ([speciesCode])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_L_VisualStock_L_Species'
CREATE INDEX [IX_FK_R_L_VisualStock_L_Species]
ON [dbo].[L_VisualStock]
    ([speciesCode]);
GO

-- Creating foreign key on [speciesCode] in table 'L_GeneticStock'
ALTER TABLE [dbo].[L_GeneticStock]
ADD CONSTRAINT [FK_R_L_GeneticStock_L_Species]
    FOREIGN KEY ([speciesCode])
    REFERENCES [dbo].[L_Species]
        ([speciesCode])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_L_GeneticStock_L_Species'
CREATE INDEX [IX_FK_R_L_GeneticStock_L_Species]
ON [dbo].[L_GeneticStock]
    ([speciesCode]);
GO

-- Creating foreign key on [visualStockId] in table 'Age'
ALTER TABLE [dbo].[Age]
ADD CONSTRAINT [FK_Age_L_VisualStock]
    FOREIGN KEY ([visualStockId])
    REFERENCES [dbo].[L_VisualStock]
        ([L_visualStockId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Age_L_VisualStock'
CREATE INDEX [IX_FK_Age_L_VisualStock]
ON [dbo].[Age]
    ([visualStockId]);
GO

-- Creating foreign key on [geneticStockId] in table 'Age'
ALTER TABLE [dbo].[Age]
ADD CONSTRAINT [FK_Age_L_GeneticStock]
    FOREIGN KEY ([geneticStockId])
    REFERENCES [dbo].[L_GeneticStock]
        ([L_geneticStockId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Age_L_GeneticStock'
CREATE INDEX [IX_FK_Age_L_GeneticStock]
ON [dbo].[Age]
    ([geneticStockId]);
GO

-- Creating foreign key on [DFUArea] in table 'SDSample'
ALTER TABLE [dbo].[SDSample]
ADD CONSTRAINT [FK_SDSample_L_DFUArea]
    FOREIGN KEY ([DFUArea])
    REFERENCES [dbo].[L_DFUArea]
        ([DFUArea])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDSample_L_DFUArea'
CREATE INDEX [IX_FK_SDSample_L_DFUArea]
ON [dbo].[SDSample]
    ([DFUArea]);
GO

-- Creating foreign key on [maturityIndexMethod] in table 'SDSample'
ALTER TABLE [dbo].[SDSample]
ADD CONSTRAINT [FK_SDSample_L_MaturityIndexMethod]
    FOREIGN KEY ([maturityIndexMethod])
    REFERENCES [dbo].[L_MaturityIndexMethod]
        ([maturityIndexMethod])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDSample_L_MaturityIndexMethod'
CREATE INDEX [IX_FK_SDSample_L_MaturityIndexMethod]
ON [dbo].[SDSample]
    ([maturityIndexMethod]);
GO

-- Creating foreign key on [sdEventTypeId] in table 'SDEvent'
ALTER TABLE [dbo].[SDEvent]
ADD CONSTRAINT [FK_SDEvent_L_SDEventType]
    FOREIGN KEY ([sdEventTypeId])
    REFERENCES [dbo].[L_SDEventType]
        ([L_sdEventTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDEvent_L_SDEventType'
CREATE INDEX [IX_FK_SDEvent_L_SDEventType]
ON [dbo].[SDEvent]
    ([sdEventTypeId]);
GO

-- Creating foreign key on [sdPurposeId] in table 'SDEvent'
ALTER TABLE [dbo].[SDEvent]
ADD CONSTRAINT [FK_SDEvent_L_SDPurpose]
    FOREIGN KEY ([sdPurposeId])
    REFERENCES [dbo].[L_SDPurpose]
        ([L_sdPurposeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDEvent_L_SDPurpose'
CREATE INDEX [IX_FK_SDEvent_L_SDPurpose]
ON [dbo].[SDEvent]
    ([sdPurposeId]);
GO

-- Creating foreign key on [sexCode] in table 'SDSample'
ALTER TABLE [dbo].[SDSample]
ADD CONSTRAINT [FK_SDSample_L_SexCode]
    FOREIGN KEY ([sexCode])
    REFERENCES [dbo].[L_SexCode]
        ([sexCode])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDSample_L_SexCode'
CREATE INDEX [IX_FK_SDSample_L_SexCode]
ON [dbo].[SDSample]
    ([sexCode]);
GO

-- Creating foreign key on [speciesCode] in table 'SDEvent'
ALTER TABLE [dbo].[SDEvent]
ADD CONSTRAINT [FK_SDEvent_L_Species]
    FOREIGN KEY ([speciesCode])
    REFERENCES [dbo].[L_Species]
        ([speciesCode])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDEvent_L_Species'
CREATE INDEX [IX_FK_SDEvent_L_Species]
ON [dbo].[SDEvent]
    ([speciesCode]);
GO

-- Creating foreign key on [statisticalRectangle] in table 'SDSample'
ALTER TABLE [dbo].[SDSample]
ADD CONSTRAINT [FK_SDSample_L_StatisticalRectangle]
    FOREIGN KEY ([statisticalRectangle])
    REFERENCES [dbo].[L_StatisticalRectangle]
        ([statisticalRectangle])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDSample_L_StatisticalRectangle'
CREATE INDEX [IX_FK_SDSample_L_StatisticalRectangle]
ON [dbo].[SDSample]
    ([statisticalRectangle]);
GO

-- Creating foreign key on [stockId] in table 'SDSample'
ALTER TABLE [dbo].[SDSample]
ADD CONSTRAINT [FK_SDSample_L_Stock]
    FOREIGN KEY ([stockId])
    REFERENCES [dbo].[L_Stock]
        ([L_stockId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDSample_L_Stock'
CREATE INDEX [IX_FK_SDSample_L_Stock]
ON [dbo].[SDSample]
    ([stockId]);
GO

-- Creating foreign key on [maturityId] in table 'SDSample'
ALTER TABLE [dbo].[SDSample]
ADD CONSTRAINT [FK_SDSample_Maturity]
    FOREIGN KEY ([maturityId])
    REFERENCES [dbo].[Maturity]
        ([maturityId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDSample_Maturity'
CREATE INDEX [IX_FK_SDSample_Maturity]
ON [dbo].[SDSample]
    ([maturityId]);
GO

-- Creating foreign key on [sdFileId] in table 'SDAnnotation'
ALTER TABLE [dbo].[SDAnnotation]
ADD CONSTRAINT [FK_SDAnnotation_SDFile]
    FOREIGN KEY ([sdFileId])
    REFERENCES [dbo].[SDFile]
        ([sdFileId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDAnnotation_SDFile'
CREATE INDEX [IX_FK_SDAnnotation_SDFile]
ON [dbo].[SDAnnotation]
    ([sdFileId]);
GO

-- Creating foreign key on [sdAnnotationId] in table 'SDLine'
ALTER TABLE [dbo].[SDLine]
ADD CONSTRAINT [FK_SDLine_SDAnnotation]
    FOREIGN KEY ([sdAnnotationId])
    REFERENCES [dbo].[SDAnnotation]
        ([sdAnnotationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDLine_SDAnnotation'
CREATE INDEX [IX_FK_SDLine_SDAnnotation]
ON [dbo].[SDLine]
    ([sdAnnotationId]);
GO

-- Creating foreign key on [sdAnnotationId] in table 'SDPoint'
ALTER TABLE [dbo].[SDPoint]
ADD CONSTRAINT [FK_SDPoint_SDAnnotation]
    FOREIGN KEY ([sdAnnotationId])
    REFERENCES [dbo].[SDAnnotation]
        ([sdAnnotationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDPoint_SDAnnotation'
CREATE INDEX [IX_FK_SDPoint_SDAnnotation]
ON [dbo].[SDPoint]
    ([sdAnnotationId]);
GO

-- Creating foreign key on [sdEventId] in table 'SDSample'
ALTER TABLE [dbo].[SDSample]
ADD CONSTRAINT [FK_SDSample_SDEvent]
    FOREIGN KEY ([sdEventId])
    REFERENCES [dbo].[SDEvent]
        ([sdEventId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDSample_SDEvent'
CREATE INDEX [IX_FK_SDSample_SDEvent]
ON [dbo].[SDSample]
    ([sdEventId]);
GO

-- Creating foreign key on [sdSampleId] in table 'SDFile'
ALTER TABLE [dbo].[SDFile]
ADD CONSTRAINT [FK_SDFile_SDSample]
    FOREIGN KEY ([sdSampleId])
    REFERENCES [dbo].[SDSample]
        ([sdSampleId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDFile_SDSample'
CREATE INDEX [IX_FK_SDFile_SDSample]
ON [dbo].[SDFile]
    ([sdSampleId]);
GO

-- Creating foreign key on [edgeStructure] in table 'SDSample'
ALTER TABLE [dbo].[SDSample]
ADD CONSTRAINT [FK_SDSample_L_EdgeStructure]
    FOREIGN KEY ([edgeStructure])
    REFERENCES [dbo].[L_EdgeStructure]
        ([edgeStructure])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDSample_L_EdgeStructure'
CREATE INDEX [IX_FK_SDSample_L_EdgeStructure]
ON [dbo].[SDSample]
    ([edgeStructure]);
GO

-- Creating foreign key on [otolithReadingRemarkId] in table 'SDSample'
ALTER TABLE [dbo].[SDSample]
ADD CONSTRAINT [FK_SDSample_L_OtolithReadingRemark]
    FOREIGN KEY ([otolithReadingRemarkId])
    REFERENCES [dbo].[L_OtolithReadingRemark]
        ([L_OtolithReadingRemarkID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDSample_L_OtolithReadingRemark'
CREATE INDEX [IX_FK_SDSample_L_OtolithReadingRemark]
ON [dbo].[SDSample]
    ([otolithReadingRemarkId]);
GO

-- Creating foreign key on [sdLightTypeId] in table 'SDSample'
ALTER TABLE [dbo].[SDSample]
ADD CONSTRAINT [FK_SDSample_L_SDLightType]
    FOREIGN KEY ([sdLightTypeId])
    REFERENCES [dbo].[L_SDLightType]
        ([L_sdLightTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDSample_L_SDLightType'
CREATE INDEX [IX_FK_SDSample_L_SDLightType]
ON [dbo].[SDSample]
    ([sdLightTypeId]);
GO

-- Creating foreign key on [sdOtolithDescriptionId] in table 'SDSample'
ALTER TABLE [dbo].[SDSample]
ADD CONSTRAINT [FK_SDSample_L_SDOtolithDescription]
    FOREIGN KEY ([sdOtolithDescriptionId])
    REFERENCES [dbo].[L_SDOtolithDescription]
        ([L_sdOtolithDescriptionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDSample_L_SDOtolithDescription'
CREATE INDEX [IX_FK_SDSample_L_SDOtolithDescription]
ON [dbo].[SDSample]
    ([sdOtolithDescriptionId]);
GO

-- Creating foreign key on [sdPreparationMethodId] in table 'SDSample'
ALTER TABLE [dbo].[SDSample]
ADD CONSTRAINT [FK_SDSample_L_SDPreparationMethod]
    FOREIGN KEY ([sdPreparationMethodId])
    REFERENCES [dbo].[L_SDPreparationMethod]
        ([L_sdPreparationMethodId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDSample_L_SDPreparationMethod'
CREATE INDEX [IX_FK_SDSample_L_SDPreparationMethod]
ON [dbo].[SDSample]
    ([sdPreparationMethodId]);
GO

-- Creating foreign key on [sdSampleTypeId] in table 'SDEvent'
ALTER TABLE [dbo].[SDEvent]
ADD CONSTRAINT [FK_SDEvent_L_SDSampleType]
    FOREIGN KEY ([sdSampleTypeId])
    REFERENCES [dbo].[L_SDSampleType]
        ([L_sdSampleTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDEvent_L_SDSampleType'
CREATE INDEX [IX_FK_SDEvent_L_SDSampleType]
ON [dbo].[SDEvent]
    ([sdSampleTypeId]);
GO

-- Creating foreign key on [DFUArea] in table 'R_StockSpeciesArea'
ALTER TABLE [dbo].[R_StockSpeciesArea]
ADD CONSTRAINT [FK_R_StockSpeciesArea_L_DFUArea]
    FOREIGN KEY ([DFUArea])
    REFERENCES [dbo].[L_DFUArea]
        ([DFUArea])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_StockSpeciesArea_L_DFUArea'
CREATE INDEX [IX_FK_R_StockSpeciesArea_L_DFUArea]
ON [dbo].[R_StockSpeciesArea]
    ([DFUArea]);
GO

-- Creating foreign key on [speciesCode] in table 'R_StockSpeciesArea'
ALTER TABLE [dbo].[R_StockSpeciesArea]
ADD CONSTRAINT [FK_R_StockSpeciesArea_L_Species]
    FOREIGN KEY ([speciesCode])
    REFERENCES [dbo].[L_Species]
        ([speciesCode])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_StockSpeciesArea_L_Species'
CREATE INDEX [IX_FK_R_StockSpeciesArea_L_Species]
ON [dbo].[R_StockSpeciesArea]
    ([speciesCode]);
GO

-- Creating foreign key on [statisticalRectangle] in table 'R_StockSpeciesArea'
ALTER TABLE [dbo].[R_StockSpeciesArea]
ADD CONSTRAINT [FK_R_StockSpeciesArea_L_StatisticalRectangle]
    FOREIGN KEY ([statisticalRectangle])
    REFERENCES [dbo].[L_StatisticalRectangle]
        ([statisticalRectangle])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_StockSpeciesArea_L_StatisticalRectangle'
CREATE INDEX [IX_FK_R_StockSpeciesArea_L_StatisticalRectangle]
ON [dbo].[R_StockSpeciesArea]
    ([statisticalRectangle]);
GO

-- Creating foreign key on [L_stockId] in table 'R_StockSpeciesArea'
ALTER TABLE [dbo].[R_StockSpeciesArea]
ADD CONSTRAINT [FK_R_StockSpeciesArea_L_Stock]
    FOREIGN KEY ([L_stockId])
    REFERENCES [dbo].[L_Stock]
        ([L_stockId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_StockSpeciesArea_L_Stock'
CREATE INDEX [IX_FK_R_StockSpeciesArea_L_Stock]
ON [dbo].[R_StockSpeciesArea]
    ([L_stockId]);
GO

-- Creating foreign key on [L_DFUAreas_DFUArea] in table 'R_SDEventDFUArea'
ALTER TABLE [dbo].[R_SDEventDFUArea]
ADD CONSTRAINT [FK_R_SDEventDFUArea_L_DFUArea]
    FOREIGN KEY ([L_DFUAreas_DFUArea])
    REFERENCES [dbo].[L_DFUArea]
        ([DFUArea])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [SDEvent_sdEventId] in table 'R_SDEventDFUArea'
ALTER TABLE [dbo].[R_SDEventDFUArea]
ADD CONSTRAINT [FK_R_SDEventDFUArea_SDEvent]
    FOREIGN KEY ([SDEvent_sdEventId])
    REFERENCES [dbo].[SDEvent]
        ([sdEventId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_SDEventDFUArea_SDEvent'
CREATE INDEX [IX_FK_R_SDEventDFUArea_SDEvent]
ON [dbo].[R_SDEventDFUArea]
    ([SDEvent_sdEventId]);
GO

-- Creating foreign key on [dfuPersonId] in table 'R_SDReader'
ALTER TABLE [dbo].[R_SDReader]
ADD CONSTRAINT [FK_R_SDReader_DFUPerson]
    FOREIGN KEY ([dfuPersonId])
    REFERENCES [dbo].[DFUPerson]
        ([dfuPersonId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_SDReader_DFUPerson'
CREATE INDEX [IX_FK_R_SDReader_DFUPerson]
ON [dbo].[R_SDReader]
    ([dfuPersonId]);
GO

-- Creating foreign key on [sdPreparationMethodId] in table 'R_SDReader'
ALTER TABLE [dbo].[R_SDReader]
ADD CONSTRAINT [FK_R_SDReader_L_SDPreparationMethod]
    FOREIGN KEY ([sdPreparationMethodId])
    REFERENCES [dbo].[L_SDPreparationMethod]
        ([L_sdPreparationMethodId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_SDReader_L_SDPreparationMethod'
CREATE INDEX [IX_FK_R_SDReader_L_SDPreparationMethod]
ON [dbo].[R_SDReader]
    ([sdPreparationMethodId]);
GO

-- Creating foreign key on [sdReaderExperienceId] in table 'R_SDReader'
ALTER TABLE [dbo].[R_SDReader]
ADD CONSTRAINT [FK_R_SDReader_L_SDReaderExperience]
    FOREIGN KEY ([sdReaderExperienceId])
    REFERENCES [dbo].[L_SDReaderExperience]
        ([L_SDReaderExperienceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_SDReader_L_SDReaderExperience'
CREATE INDEX [IX_FK_R_SDReader_L_SDReaderExperience]
ON [dbo].[R_SDReader]
    ([sdReaderExperienceId]);
GO

-- Creating foreign key on [speciesCode] in table 'R_SDReader'
ALTER TABLE [dbo].[R_SDReader]
ADD CONSTRAINT [FK_R_SDReader_L_Species]
    FOREIGN KEY ([speciesCode])
    REFERENCES [dbo].[L_Species]
        ([speciesCode])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_SDReader_L_Species'
CREATE INDEX [IX_FK_R_SDReader_L_Species]
ON [dbo].[R_SDReader]
    ([speciesCode]);
GO

-- Creating foreign key on [stockId] in table 'R_SDReader'
ALTER TABLE [dbo].[R_SDReader]
ADD CONSTRAINT [FK_R_SDReader_L_Stock]
    FOREIGN KEY ([stockId])
    REFERENCES [dbo].[L_Stock]
        ([L_stockId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_SDReader_L_Stock'
CREATE INDEX [IX_FK_R_SDReader_L_Stock]
ON [dbo].[R_SDReader]
    ([stockId]);
GO

-- Creating foreign key on [sdReaderId] in table 'R_SDEventSDReader'
ALTER TABLE [dbo].[R_SDEventSDReader]
ADD CONSTRAINT [FK_R_SDEventSDReader_R_SDReader]
    FOREIGN KEY ([sdReaderId])
    REFERENCES [dbo].[R_SDReader]
        ([r_SDReaderId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_SDEventSDReader_R_SDReader'
CREATE INDEX [IX_FK_R_SDEventSDReader_R_SDReader]
ON [dbo].[R_SDEventSDReader]
    ([sdReaderId]);
GO

-- Creating foreign key on [sdEventId] in table 'R_SDEventSDReader'
ALTER TABLE [dbo].[R_SDEventSDReader]
ADD CONSTRAINT [FK_R_SDEventSDReader_SDEvent]
    FOREIGN KEY ([sdEventId])
    REFERENCES [dbo].[SDEvent]
        ([sdEventId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [otolithReadingRemarkId] in table 'SDAnnotation'
ALTER TABLE [dbo].[SDAnnotation]
ADD CONSTRAINT [FK_SDAnnotation_L_OtolithReadingRemark]
    FOREIGN KEY ([otolithReadingRemarkId])
    REFERENCES [dbo].[L_OtolithReadingRemark]
        ([L_OtolithReadingRemarkID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDAnnotation_L_OtolithReadingRemark'
CREATE INDEX [IX_FK_SDAnnotation_L_OtolithReadingRemark]
ON [dbo].[SDAnnotation]
    ([otolithReadingRemarkId]);
GO

-- Creating foreign key on [sdAnalysisParameterId] in table 'SDAnnotation'
ALTER TABLE [dbo].[SDAnnotation]
ADD CONSTRAINT [FK_SDAnnotation_L_SDAnalysisParameter]
    FOREIGN KEY ([sdAnalysisParameterId])
    REFERENCES [dbo].[L_SDAnalysisParameter]
        ([L_sdAnalysisParameterId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDAnnotation_L_SDAnalysisParameter'
CREATE INDEX [IX_FK_SDAnnotation_L_SDAnalysisParameter]
ON [dbo].[SDAnnotation]
    ([sdAnalysisParameterId]);
GO

-- Creating foreign key on [sdEventTypeId] in table 'SDEvent'
ALTER TABLE [dbo].[SDEvent]
ADD CONSTRAINT [FK_SDEvent_L_SDEventType1]
    FOREIGN KEY ([sdEventTypeId])
    REFERENCES [dbo].[L_SDEventType]
        ([L_sdEventTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDEvent_L_SDEventType1'
CREATE INDEX [IX_FK_SDEvent_L_SDEventType1]
ON [dbo].[SDEvent]
    ([sdEventTypeId]);
GO

-- Creating foreign key on [sdLightTypeId] in table 'SDSample'
ALTER TABLE [dbo].[SDSample]
ADD CONSTRAINT [FK_SDSample_L_SDLightType1]
    FOREIGN KEY ([sdLightTypeId])
    REFERENCES [dbo].[L_SDLightType]
        ([L_sdLightTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDSample_L_SDLightType1'
CREATE INDEX [IX_FK_SDSample_L_SDLightType1]
ON [dbo].[SDSample]
    ([sdLightTypeId]);
GO

-- Creating foreign key on [sdOtolithDescriptionId] in table 'SDSample'
ALTER TABLE [dbo].[SDSample]
ADD CONSTRAINT [FK_SDSample_L_SDOtolithDescription1]
    FOREIGN KEY ([sdOtolithDescriptionId])
    REFERENCES [dbo].[L_SDOtolithDescription]
        ([L_sdOtolithDescriptionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDSample_L_SDOtolithDescription1'
CREATE INDEX [IX_FK_SDSample_L_SDOtolithDescription1]
ON [dbo].[SDSample]
    ([sdOtolithDescriptionId]);
GO

-- Creating foreign key on [sdPreparationMethodId] in table 'R_SDReader'
ALTER TABLE [dbo].[R_SDReader]
ADD CONSTRAINT [FK_R_SDReader_L_SDPreparationMethod1]
    FOREIGN KEY ([sdPreparationMethodId])
    REFERENCES [dbo].[L_SDPreparationMethod]
        ([L_sdPreparationMethodId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_SDReader_L_SDPreparationMethod1'
CREATE INDEX [IX_FK_R_SDReader_L_SDPreparationMethod1]
ON [dbo].[R_SDReader]
    ([sdPreparationMethodId]);
GO

-- Creating foreign key on [sdPreparationMethodId] in table 'SDSample'
ALTER TABLE [dbo].[SDSample]
ADD CONSTRAINT [FK_SDSample_L_SDPreparationMethod1]
    FOREIGN KEY ([sdPreparationMethodId])
    REFERENCES [dbo].[L_SDPreparationMethod]
        ([L_sdPreparationMethodId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDSample_L_SDPreparationMethod1'
CREATE INDEX [IX_FK_SDSample_L_SDPreparationMethod1]
ON [dbo].[SDSample]
    ([sdPreparationMethodId]);
GO

-- Creating foreign key on [sdPurposeId] in table 'SDEvent'
ALTER TABLE [dbo].[SDEvent]
ADD CONSTRAINT [FK_SDEvent_L_SDPurpose1]
    FOREIGN KEY ([sdPurposeId])
    REFERENCES [dbo].[L_SDPurpose]
        ([L_sdPurposeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDEvent_L_SDPurpose1'
CREATE INDEX [IX_FK_SDEvent_L_SDPurpose1]
ON [dbo].[SDEvent]
    ([sdPurposeId]);
GO

-- Creating foreign key on [sdReaderExperienceId] in table 'R_SDReader'
ALTER TABLE [dbo].[R_SDReader]
ADD CONSTRAINT [FK_R_SDReader_L_SDReaderExperience1]
    FOREIGN KEY ([sdReaderExperienceId])
    REFERENCES [dbo].[L_SDReaderExperience]
        ([L_SDReaderExperienceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_R_SDReader_L_SDReaderExperience1'
CREATE INDEX [IX_FK_R_SDReader_L_SDReaderExperience1]
ON [dbo].[R_SDReader]
    ([sdReaderExperienceId]);
GO

-- Creating foreign key on [sdSampleTypeId] in table 'SDEvent'
ALTER TABLE [dbo].[SDEvent]
ADD CONSTRAINT [FK_SDEvent_L_SDSampleType1]
    FOREIGN KEY ([sdSampleTypeId])
    REFERENCES [dbo].[L_SDSampleType]
        ([L_sdSampleTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDEvent_L_SDSampleType1'
CREATE INDEX [IX_FK_SDEvent_L_SDSampleType1]
ON [dbo].[SDEvent]
    ([sdSampleTypeId]);
GO

-- Creating foreign key on [edgeStructure] in table 'SDAnnotation'
ALTER TABLE [dbo].[SDAnnotation]
ADD CONSTRAINT [FK_SDAnnotation_L_EdgeStructure]
    FOREIGN KEY ([edgeStructure])
    REFERENCES [dbo].[L_EdgeStructure]
        ([edgeStructure])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDAnnotation_L_EdgeStructure'
CREATE INDEX [IX_FK_SDAnnotation_L_EdgeStructure]
ON [dbo].[SDAnnotation]
    ([edgeStructure]);
GO

-- Creating foreign key on [speciesCode] in table 'SDSample'
ALTER TABLE [dbo].[SDSample]
ADD CONSTRAINT [FK_SDSample_L_Species]
    FOREIGN KEY ([speciesCode])
    REFERENCES [dbo].[L_Species]
        ([speciesCode])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDSample_L_Species'
CREATE INDEX [IX_FK_SDSample_L_Species]
ON [dbo].[SDSample]
    ([speciesCode]);
GO

-- Creating foreign key on [sdAnnotationId] in table 'Age'
ALTER TABLE [dbo].[Age]
ADD CONSTRAINT [FK_Age_SDAnnotation]
    FOREIGN KEY ([sdAnnotationId])
    REFERENCES [dbo].[SDAnnotation]
        ([sdAnnotationId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Age_SDAnnotation'
CREATE INDEX [IX_FK_Age_SDAnnotation]
ON [dbo].[Age]
    ([sdAnnotationId]);
GO

-- Creating foreign key on [sdAgeReadId] in table 'Age'
ALTER TABLE [dbo].[Age]
ADD CONSTRAINT [FK_Age_DFUPerson]
    FOREIGN KEY ([sdAgeReadId])
    REFERENCES [dbo].[DFUPerson]
        ([dfuPersonId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Age_DFUPerson'
CREATE INDEX [IX_FK_Age_DFUPerson]
ON [dbo].[Age]
    ([sdAgeReadId]);
GO

-- Creating foreign key on [createdById] in table 'SDAnnotation'
ALTER TABLE [dbo].[SDAnnotation]
ADD CONSTRAINT [FK_SDAnnotation_DFUPerson]
    FOREIGN KEY ([createdById])
    REFERENCES [dbo].[DFUPerson]
        ([dfuPersonId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDAnnotation_DFUPerson'
CREATE INDEX [IX_FK_SDAnnotation_DFUPerson]
ON [dbo].[SDAnnotation]
    ([createdById]);
GO

-- Creating foreign key on [createdById] in table 'SDEvent'
ALTER TABLE [dbo].[SDEvent]
ADD CONSTRAINT [FK_SDEvent_DFUPerson]
    FOREIGN KEY ([createdById])
    REFERENCES [dbo].[DFUPerson]
        ([dfuPersonId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDEvent_DFUPerson'
CREATE INDEX [IX_FK_SDEvent_DFUPerson]
ON [dbo].[SDEvent]
    ([createdById]);
GO

-- Creating foreign key on [createdById] in table 'SDLine'
ALTER TABLE [dbo].[SDLine]
ADD CONSTRAINT [FK_SDLine_DFUPerson]
    FOREIGN KEY ([createdById])
    REFERENCES [dbo].[DFUPerson]
        ([dfuPersonId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDLine_DFUPerson'
CREATE INDEX [IX_FK_SDLine_DFUPerson]
ON [dbo].[SDLine]
    ([createdById]);
GO

-- Creating foreign key on [createdById] in table 'SDPoint'
ALTER TABLE [dbo].[SDPoint]
ADD CONSTRAINT [FK_SDPoint_DFUPerson]
    FOREIGN KEY ([createdById])
    REFERENCES [dbo].[DFUPerson]
        ([dfuPersonId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDPoint_DFUPerson'
CREATE INDEX [IX_FK_SDPoint_DFUPerson]
ON [dbo].[SDPoint]
    ([createdById]);
GO

-- Creating foreign key on [createdById] in table 'SDSample'
ALTER TABLE [dbo].[SDSample]
ADD CONSTRAINT [FK_SDSample_DFUPerson]
    FOREIGN KEY ([createdById])
    REFERENCES [dbo].[DFUPerson]
        ([dfuPersonId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SDSample_DFUPerson'
CREATE INDEX [IX_FK_SDSample_DFUPerson]
ON [dbo].[SDSample]
    ([createdById]);
GO

-- Creating foreign key on [selectionDeviceSourceId] in table 'Sample'
ALTER TABLE [dbo].[Sample]
ADD CONSTRAINT [FK_L_SelectionDeviceSourceSample]
    FOREIGN KEY ([selectionDeviceSourceId])
    REFERENCES [dbo].[L_SelectionDeviceSource]
        ([L_selectionDeviceSourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_SelectionDeviceSourceSample'
CREATE INDEX [IX_FK_L_SelectionDeviceSourceSample]
ON [dbo].[Sample]
    ([selectionDeviceSourceId]);
GO

-- Creating foreign key on [lengthMeasureTypeId] in table 'Animal'
ALTER TABLE [dbo].[Animal]
ADD CONSTRAINT [FK_L_LengthMeasureTypeAnimal]
    FOREIGN KEY ([lengthMeasureTypeId])
    REFERENCES [dbo].[L_LengthMeasureType]
        ([L_lengthMeasureTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_LengthMeasureTypeAnimal'
CREATE INDEX [IX_FK_L_LengthMeasureTypeAnimal]
ON [dbo].[Animal]
    ([lengthMeasureTypeId]);
GO

-- Creating foreign key on [standardLengthMeasureTypeId] in table 'L_Species'
ALTER TABLE [dbo].[L_Species]
ADD CONSTRAINT [FK_L_LengthMeasureTypeL_Species]
    FOREIGN KEY ([standardLengthMeasureTypeId])
    REFERENCES [dbo].[L_LengthMeasureType]
        ([L_lengthMeasureTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_L_LengthMeasureTypeL_Species'
CREATE INDEX [IX_FK_L_LengthMeasureTypeL_Species]
ON [dbo].[L_Species]
    ([standardLengthMeasureTypeId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------