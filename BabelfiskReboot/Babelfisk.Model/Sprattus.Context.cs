﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.EntityClient;
using System.Data.Metadata.Edm;
using System.Data.Objects.DataClasses;
using System.Data.Objects;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Babelfisk.Entities.Sprattus
{
    public partial class SprattusContainer : ObjectContext
    {
        public const string ConnectionString = "name=SprattusContainer";
        public const string ContainerName = "SprattusContainer";
    
        #region Constructors
    
        public SprattusContainer()
            : base(ConnectionString, ContainerName)
        {
            Initialize();
        }
    
        public SprattusContainer(string connectionString)
            : base(connectionString, ContainerName)
        {
            Initialize();
        }
    
        public SprattusContainer(EntityConnection connection)
            : base(connection, ContainerName)
        {
            Initialize();
        }
    
        private void Initialize()
        {
            // Creating proxies requires the use of the ProxyDataContractResolver and
            // may allow lazy loading which can expand the loaded graph during serialization.
            ContextOptions.ProxyCreationEnabled = false;
            ObjectMaterialized += new ObjectMaterializedEventHandler(HandleObjectMaterialized);
        }
    
        private void HandleObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            var entity = e.Entity as IObjectWithChangeTracker;
            if (entity != null)
            {
                bool changeTrackingEnabled = entity.ChangeTracker.ChangeTrackingEnabled;
                try
                {
                    entity.MarkAsUnchanged();
                }
                finally
                {
                    entity.ChangeTracker.ChangeTrackingEnabled = changeTrackingEnabled;
                }
                this.StoreReferenceKeyValues(entity);
            }
        }
    
        #endregion
    
        #region ObjectSet Properties
    
        public ObjectSet<ActivityLog> ActivityLog
        {
            get { return _activityLog  ?? (_activityLog = CreateObjectSet<ActivityLog>("ActivityLog")); }
        }
        private ObjectSet<ActivityLog> _activityLog;
    
        public ObjectSet<Age> Age
        {
            get { return _age  ?? (_age = CreateObjectSet<Age>("Age")); }
        }
        private ObjectSet<Age> _age;
    
        public ObjectSet<Animal> Animal
        {
            get { return _animal  ?? (_animal = CreateObjectSet<Animal>("Animal")); }
        }
        private ObjectSet<Animal> _animal;
    
        public ObjectSet<AnimalInfo> AnimalInfo
        {
            get { return _animalInfo  ?? (_animalInfo = CreateObjectSet<AnimalInfo>("AnimalInfo")); }
        }
        private ObjectSet<AnimalInfo> _animalInfo;
    
        public ObjectSet<Cruise> Cruise
        {
            get { return _cruise  ?? (_cruise = CreateObjectSet<Cruise>("Cruise")); }
        }
        private ObjectSet<Cruise> _cruise;
    
        public ObjectSet<DFUPerson> DFUPerson
        {
            get { return _dFUPerson  ?? (_dFUPerson = CreateObjectSet<DFUPerson>("DFUPerson")); }
        }
        private ObjectSet<DFUPerson> _dFUPerson;
    
        public ObjectSet<DFUSubArea> DFUSubArea
        {
            get { return _dFUSubArea  ?? (_dFUSubArea = CreateObjectSet<DFUSubArea>("DFUSubArea")); }
        }
        private ObjectSet<DFUSubArea> _dFUSubArea;
    
        public ObjectSet<Est_Method> Est_Method
        {
            get { return _est_Method  ?? (_est_Method = CreateObjectSet<Est_Method>("Est_Method")); }
        }
        private ObjectSet<Est_Method> _est_Method;
    
        public ObjectSet<Est_MethodStep> Est_MethodStep
        {
            get { return _est_MethodStep  ?? (_est_MethodStep = CreateObjectSet<Est_MethodStep>("Est_MethodStep")); }
        }
        private ObjectSet<Est_MethodStep> _est_MethodStep;
    
        public ObjectSet<Est_Strata> Est_Strata
        {
            get { return _est_Strata  ?? (_est_Strata = CreateObjectSet<Est_Strata>("Est_Strata")); }
        }
        private ObjectSet<Est_Strata> _est_Strata;
    
        public ObjectSet<Fat> Fat
        {
            get { return _fat  ?? (_fat = CreateObjectSet<Fat>("Fat")); }
        }
        private ObjectSet<Fat> _fat;
    
        public ObjectSet<FishingActivityOnSample> FishingActivityOnSample
        {
            get { return _fishingActivityOnSample  ?? (_fishingActivityOnSample = CreateObjectSet<FishingActivityOnSample>("FishingActivityOnSample")); }
        }
        private ObjectSet<FishingActivityOnSample> _fishingActivityOnSample;
    
        public ObjectSet<L_Activity> L_Activity
        {
            get { return _l_Activity  ?? (_l_Activity = CreateObjectSet<L_Activity>("L_Activity")); }
        }
        private ObjectSet<L_Activity> _l_Activity;
    
        public ObjectSet<L_AgeMeasureMethod> L_AgeMeasureMethod
        {
            get { return _l_AgeMeasureMethod  ?? (_l_AgeMeasureMethod = CreateObjectSet<L_AgeMeasureMethod>("L_AgeMeasureMethod")); }
        }
        private ObjectSet<L_AgeMeasureMethod> _l_AgeMeasureMethod;
    
        public ObjectSet<L_BITS_cruises> L_BITS_cruises
        {
            get { return _l_BITS_cruises  ?? (_l_BITS_cruises = CreateObjectSet<L_BITS_cruises>("L_BITS_cruises")); }
        }
        private ObjectSet<L_BITS_cruises> _l_BITS_cruises;
    
        public ObjectSet<L_Bottomtype> L_Bottomtype
        {
            get { return _l_Bottomtype  ?? (_l_Bottomtype = CreateObjectSet<L_Bottomtype>("L_Bottomtype")); }
        }
        private ObjectSet<L_Bottomtype> _l_Bottomtype;
    
        public ObjectSet<L_BroodingPhase> L_BroodingPhase
        {
            get { return _l_BroodingPhase  ?? (_l_BroodingPhase = CreateObjectSet<L_BroodingPhase>("L_BroodingPhase")); }
        }
        private ObjectSet<L_BroodingPhase> _l_BroodingPhase;
    
        public ObjectSet<L_CatchRegistration> L_CatchRegistration
        {
            get { return _l_CatchRegistration  ?? (_l_CatchRegistration = CreateObjectSet<L_CatchRegistration>("L_CatchRegistration")); }
        }
        private ObjectSet<L_CatchRegistration> _l_CatchRegistration;
    
        public ObjectSet<L_CruiseStatus> L_CruiseStatus
        {
            get { return _l_CruiseStatus  ?? (_l_CruiseStatus = CreateObjectSet<L_CruiseStatus>("L_CruiseStatus")); }
        }
        private ObjectSet<L_CruiseStatus> _l_CruiseStatus;
    
        public ObjectSet<L_CruiseType> L_CruiseType
        {
            get { return _l_CruiseType  ?? (_l_CruiseType = CreateObjectSet<L_CruiseType>("L_CruiseType")); }
        }
        private ObjectSet<L_CruiseType> _l_CruiseType;
    
        public ObjectSet<L_CuticulaHardness> L_CuticulaHardness
        {
            get { return _l_CuticulaHardness  ?? (_l_CuticulaHardness = CreateObjectSet<L_CuticulaHardness>("L_CuticulaHardness")); }
        }
        private ObjectSet<L_CuticulaHardness> _l_CuticulaHardness;
    
        public ObjectSet<L_DFUArea> L_DFUArea
        {
            get { return _l_DFUArea  ?? (_l_DFUArea = CreateObjectSet<L_DFUArea>("L_DFUArea")); }
        }
        private ObjectSet<L_DFUArea> _l_DFUArea;
    
        public ObjectSet<L_DFUBase_Category> L_DFUBase_Category
        {
            get { return _l_DFUBase_Category  ?? (_l_DFUBase_Category = CreateObjectSet<L_DFUBase_Category>("L_DFUBase_Category")); }
        }
        private ObjectSet<L_DFUBase_Category> _l_DFUBase_Category;
    
        public ObjectSet<L_DFUDepartment> L_DFUDepartment
        {
            get { return _l_DFUDepartment  ?? (_l_DFUDepartment = CreateObjectSet<L_DFUDepartment>("L_DFUDepartment")); }
        }
        private ObjectSet<L_DFUDepartment> _l_DFUDepartment;
    
        public ObjectSet<L_FatIndexMethod> L_FatIndexMethod
        {
            get { return _l_FatIndexMethod  ?? (_l_FatIndexMethod = CreateObjectSet<L_FatIndexMethod>("L_FatIndexMethod")); }
        }
        private ObjectSet<L_FatIndexMethod> _l_FatIndexMethod;
    
        public ObjectSet<L_Fejlkategorier> L_Fejlkategorier
        {
            get { return _l_Fejlkategorier  ?? (_l_Fejlkategorier = CreateObjectSet<L_Fejlkategorier>("L_Fejlkategorier")); }
        }
        private ObjectSet<L_Fejlkategorier> _l_Fejlkategorier;
    
        public ObjectSet<L_FishingActivityCategory> L_FishingActivityCategory
        {
            get { return _l_FishingActivityCategory  ?? (_l_FishingActivityCategory = CreateObjectSet<L_FishingActivityCategory>("L_FishingActivityCategory")); }
        }
        private ObjectSet<L_FishingActivityCategory> _l_FishingActivityCategory;
    
        public ObjectSet<L_FishingActivityCategory_temp> L_FishingActivityCategory_temp
        {
            get { return _l_FishingActivityCategory_temp  ?? (_l_FishingActivityCategory_temp = CreateObjectSet<L_FishingActivityCategory_temp>("L_FishingActivityCategory_temp")); }
        }
        private ObjectSet<L_FishingActivityCategory_temp> _l_FishingActivityCategory_temp;
    
        public ObjectSet<L_FishingActivityNational> L_FishingActivityNational
        {
            get { return _l_FishingActivityNational  ?? (_l_FishingActivityNational = CreateObjectSet<L_FishingActivityNational>("L_FishingActivityNational")); }
        }
        private ObjectSet<L_FishingActivityNational> _l_FishingActivityNational;
    
        public ObjectSet<L_Gear> L_Gear
        {
            get { return _l_Gear  ?? (_l_Gear = CreateObjectSet<L_Gear>("L_Gear")); }
        }
        private ObjectSet<L_Gear> _l_Gear;
    
        public ObjectSet<L_GearInfoType> L_GearInfoType
        {
            get { return _l_GearInfoType  ?? (_l_GearInfoType = CreateObjectSet<L_GearInfoType>("L_GearInfoType")); }
        }
        private ObjectSet<L_GearInfoType> _l_GearInfoType;
    
        public ObjectSet<L_GearQuality> L_GearQuality
        {
            get { return _l_GearQuality  ?? (_l_GearQuality = CreateObjectSet<L_GearQuality>("L_GearQuality")); }
        }
        private ObjectSet<L_GearQuality> _l_GearQuality;
    
        public ObjectSet<L_GearType> L_GearType
        {
            get { return _l_GearType  ?? (_l_GearType = CreateObjectSet<L_GearType>("L_GearType")); }
        }
        private ObjectSet<L_GearType> _l_GearType;
    
        public ObjectSet<L_Harbour> L_Harbour
        {
            get { return _l_Harbour  ?? (_l_Harbour = CreateObjectSet<L_Harbour>("L_Harbour")); }
        }
        private ObjectSet<L_Harbour> _l_Harbour;
    
        public ObjectSet<L_Harbour2> L_Harbour2
        {
            get { return _l_Harbour2  ?? (_l_Harbour2 = CreateObjectSet<L_Harbour2>("L_Harbour2")); }
        }
        private ObjectSet<L_Harbour2> _l_Harbour2;
    
        public ObjectSet<L_IBTS_cruises> L_IBTS_cruises
        {
            get { return _l_IBTS_cruises  ?? (_l_IBTS_cruises = CreateObjectSet<L_IBTS_cruises>("L_IBTS_cruises")); }
        }
        private ObjectSet<L_IBTS_cruises> _l_IBTS_cruises;
    
        public ObjectSet<L_LandingCategory> L_LandingCategory
        {
            get { return _l_LandingCategory  ?? (_l_LandingCategory = CreateObjectSet<L_LandingCategory>("L_LandingCategory")); }
        }
        private ObjectSet<L_LandingCategory> _l_LandingCategory;
    
        public ObjectSet<L_LengthMeasureUnit> L_LengthMeasureUnit
        {
            get { return _l_LengthMeasureUnit  ?? (_l_LengthMeasureUnit = CreateObjectSet<L_LengthMeasureUnit>("L_LengthMeasureUnit")); }
        }
        private ObjectSet<L_LengthMeasureUnit> _l_LengthMeasureUnit;
    
        public ObjectSet<L_MaturityIndexMethod> L_MaturityIndexMethod
        {
            get { return _l_MaturityIndexMethod  ?? (_l_MaturityIndexMethod = CreateObjectSet<L_MaturityIndexMethod>("L_MaturityIndexMethod")); }
        }
        private ObjectSet<L_MaturityIndexMethod> _l_MaturityIndexMethod;
    
        public ObjectSet<L_MissingLookup> L_MissingLookup
        {
            get { return _l_MissingLookup  ?? (_l_MissingLookup = CreateObjectSet<L_MissingLookup>("L_MissingLookup")); }
        }
        private ObjectSet<L_MissingLookup> _l_MissingLookup;
    
        public ObjectSet<L_Nationality> L_Nationality
        {
            get { return _l_Nationality  ?? (_l_Nationality = CreateObjectSet<L_Nationality>("L_Nationality")); }
        }
        private ObjectSet<L_Nationality> _l_Nationality;
    
        public ObjectSet<L_NavigationSystem> L_NavigationSystem
        {
            get { return _l_NavigationSystem  ?? (_l_NavigationSystem = CreateObjectSet<L_NavigationSystem>("L_NavigationSystem")); }
        }
        private ObjectSet<L_NavigationSystem> _l_NavigationSystem;
    
        public ObjectSet<L_OtolithReadingRemark> L_OtolithReadingRemark
        {
            get { return _l_OtolithReadingRemark  ?? (_l_OtolithReadingRemark = CreateObjectSet<L_OtolithReadingRemark>("L_OtolithReadingRemark")); }
        }
        private ObjectSet<L_OtolithReadingRemark> _l_OtolithReadingRemark;
    
        public ObjectSet<L_Platform> L_Platform
        {
            get { return _l_Platform  ?? (_l_Platform = CreateObjectSet<L_Platform>("L_Platform")); }
        }
        private ObjectSet<L_Platform> _l_Platform;
    
        public ObjectSet<L_PlatformType> L_PlatformType
        {
            get { return _l_PlatformType  ?? (_l_PlatformType = CreateObjectSet<L_PlatformType>("L_PlatformType")); }
        }
        private ObjectSet<L_PlatformType> _l_PlatformType;
    
        public ObjectSet<L_PlatformVersion> L_PlatformVersion
        {
            get { return _l_PlatformVersion  ?? (_l_PlatformVersion = CreateObjectSet<L_PlatformVersion>("L_PlatformVersion")); }
        }
        private ObjectSet<L_PlatformVersion> _l_PlatformVersion;
    
        public ObjectSet<L_SampleStatus> L_SampleStatus
        {
            get { return _l_SampleStatus  ?? (_l_SampleStatus = CreateObjectSet<L_SampleStatus>("L_SampleStatus")); }
        }
        private ObjectSet<L_SampleStatus> _l_SampleStatus;
    
        public ObjectSet<L_SampleType> L_SampleType
        {
            get { return _l_SampleType  ?? (_l_SampleType = CreateObjectSet<L_SampleType>("L_SampleType")); }
        }
        private ObjectSet<L_SampleType> _l_SampleType;
    
        public ObjectSet<L_SamplingMethod> L_SamplingMethod
        {
            get { return _l_SamplingMethod  ?? (_l_SamplingMethod = CreateObjectSet<L_SamplingMethod>("L_SamplingMethod")); }
        }
        private ObjectSet<L_SamplingMethod> _l_SamplingMethod;
    
        public ObjectSet<L_SamplingType> L_SamplingType
        {
            get { return _l_SamplingType  ?? (_l_SamplingType = CreateObjectSet<L_SamplingType>("L_SamplingType")); }
        }
        private ObjectSet<L_SamplingType> _l_SamplingType;
    
        public ObjectSet<L_SelectPanel> L_SelectPanel
        {
            get { return _l_SelectPanel  ?? (_l_SelectPanel = CreateObjectSet<L_SelectPanel>("L_SelectPanel")); }
        }
        private ObjectSet<L_SelectPanel> _l_SelectPanel;
    
        public ObjectSet<L_SexCode> L_SexCode
        {
            get { return _l_SexCode  ?? (_l_SexCode = CreateObjectSet<L_SexCode>("L_SexCode")); }
        }
        private ObjectSet<L_SexCode> _l_SexCode;
    
        public ObjectSet<L_ShovelType> L_ShovelType
        {
            get { return _l_ShovelType  ?? (_l_ShovelType = CreateObjectSet<L_ShovelType>("L_ShovelType")); }
        }
        private ObjectSet<L_ShovelType> _l_ShovelType;
    
        public ObjectSet<L_SizeSortingDFU> L_SizeSortingDFU
        {
            get { return _l_SizeSortingDFU  ?? (_l_SizeSortingDFU = CreateObjectSet<L_SizeSortingDFU>("L_SizeSortingDFU")); }
        }
        private ObjectSet<L_SizeSortingDFU> _l_SizeSortingDFU;
    
        public ObjectSet<L_SizeSortingEU> L_SizeSortingEU
        {
            get { return _l_SizeSortingEU  ?? (_l_SizeSortingEU = CreateObjectSet<L_SizeSortingEU>("L_SizeSortingEU")); }
        }
        private ObjectSet<L_SizeSortingEU> _l_SizeSortingEU;
    
        public ObjectSet<L_Species> L_Species
        {
            get { return _l_Species  ?? (_l_Species = CreateObjectSet<L_Species>("L_Species")); }
        }
        private ObjectSet<L_Species> _l_Species;
    
        public ObjectSet<L_SpeciesRegistration> L_SpeciesRegistration
        {
            get { return _l_SpeciesRegistration  ?? (_l_SpeciesRegistration = CreateObjectSet<L_SpeciesRegistration>("L_SpeciesRegistration")); }
        }
        private ObjectSet<L_SpeciesRegistration> _l_SpeciesRegistration;
    
        public ObjectSet<L_StatisticalRectangle> L_StatisticalRectangle
        {
            get { return _l_StatisticalRectangle  ?? (_l_StatisticalRectangle = CreateObjectSet<L_StatisticalRectangle>("L_StatisticalRectangle")); }
        }
        private ObjectSet<L_StatisticalRectangle> _l_StatisticalRectangle;
    
        public ObjectSet<L_Stock> L_Stock
        {
            get { return _l_Stock  ?? (_l_Stock = CreateObjectSet<L_Stock>("L_Stock")); }
        }
        private ObjectSet<L_Stock> _l_Stock;
    
        public ObjectSet<L_SubAreaType> L_SubAreaType
        {
            get { return _l_SubAreaType  ?? (_l_SubAreaType = CreateObjectSet<L_SubAreaType>("L_SubAreaType")); }
        }
        private ObjectSet<L_SubAreaType> _l_SubAreaType;
    
        public ObjectSet<L_TreadMaterial> L_TreadMaterial
        {
            get { return _l_TreadMaterial  ?? (_l_TreadMaterial = CreateObjectSet<L_TreadMaterial>("L_TreadMaterial")); }
        }
        private ObjectSet<L_TreadMaterial> _l_TreadMaterial;
    
        public ObjectSet<L_TreadType> L_TreadType
        {
            get { return _l_TreadType  ?? (_l_TreadType = CreateObjectSet<L_TreadType>("L_TreadType")); }
        }
        private ObjectSet<L_TreadType> _l_TreadType;
    
        public ObjectSet<L_Treatment> L_Treatment
        {
            get { return _l_Treatment  ?? (_l_Treatment = CreateObjectSet<L_Treatment>("L_Treatment")); }
        }
        private ObjectSet<L_Treatment> _l_Treatment;
    
        public ObjectSet<L_TreatmentFactorGroup> L_TreatmentFactorGroup
        {
            get { return _l_TreatmentFactorGroup  ?? (_l_TreatmentFactorGroup = CreateObjectSet<L_TreatmentFactorGroup>("L_TreatmentFactorGroup")); }
        }
        private ObjectSet<L_TreatmentFactorGroup> _l_TreatmentFactorGroup;
    
        public ObjectSet<L_TripType> L_TripType
        {
            get { return _l_TripType  ?? (_l_TripType = CreateObjectSet<L_TripType>("L_TripType")); }
        }
        private ObjectSet<L_TripType> _l_TripType;
    
        public ObjectSet<L_UsabilityParam> L_UsabilityParam
        {
            get { return _l_UsabilityParam  ?? (_l_UsabilityParam = CreateObjectSet<L_UsabilityParam>("L_UsabilityParam")); }
        }
        private ObjectSet<L_UsabilityParam> _l_UsabilityParam;
    
        public ObjectSet<L_UsabilityParamGrp> L_UsabilityParamGrp
        {
            get { return _l_UsabilityParamGrp  ?? (_l_UsabilityParamGrp = CreateObjectSet<L_UsabilityParamGrp>("L_UsabilityParamGrp")); }
        }
        private ObjectSet<L_UsabilityParamGrp> _l_UsabilityParamGrp;
    
        public ObjectSet<L_YesNo> L_YesNo
        {
            get { return _l_YesNo  ?? (_l_YesNo = CreateObjectSet<L_YesNo>("L_YesNo")); }
        }
        private ObjectSet<L_YesNo> _l_YesNo;
    
        public ObjectSet<Maturity> Maturity
        {
            get { return _maturity  ?? (_maturity = CreateObjectSet<Maturity>("Maturity")); }
        }
        private ObjectSet<Maturity> _maturity;
    
        public ObjectSet<NumberOfStationsPerTrip> NumberOfStationsPerTrip
        {
            get { return _numberOfStationsPerTrip  ?? (_numberOfStationsPerTrip = CreateObjectSet<NumberOfStationsPerTrip>("NumberOfStationsPerTrip")); }
        }
        private ObjectSet<NumberOfStationsPerTrip> _numberOfStationsPerTrip;
    
        public ObjectSet<Person> Person
        {
            get { return _person  ?? (_person = CreateObjectSet<Person>("Person")); }
        }
        private ObjectSet<Person> _person;
    
        public ObjectSet<Queries> Queries
        {
            get { return _queries  ?? (_queries = CreateObjectSet<Queries>("Queries")); }
        }
        private ObjectSet<Queries> _queries;
    
        public ObjectSet<QueryTypes> QueryTypes
        {
            get { return _queryTypes  ?? (_queryTypes = CreateObjectSet<QueryTypes>("QueryTypes")); }
        }
        private ObjectSet<QueryTypes> _queryTypes;
    
        public ObjectSet<R_CruiseUsabilityParam> R_CruiseUsabilityParam
        {
            get { return _r_CruiseUsabilityParam  ?? (_r_CruiseUsabilityParam = CreateObjectSet<R_CruiseUsabilityParam>("R_CruiseUsabilityParam")); }
        }
        private ObjectSet<R_CruiseUsabilityParam> _r_CruiseUsabilityParam;
    
        public ObjectSet<R_GearInfo> R_GearInfo
        {
            get { return _r_GearInfo  ?? (_r_GearInfo = CreateObjectSet<R_GearInfo>("R_GearInfo")); }
        }
        private ObjectSet<R_GearInfo> _r_GearInfo;
    
        public ObjectSet<R_Maturity> R_Maturity
        {
            get { return _r_Maturity  ?? (_r_Maturity = CreateObjectSet<R_Maturity>("R_Maturity")); }
        }
        private ObjectSet<R_Maturity> _r_Maturity;
    
        public ObjectSet<R_SampleUsabilityParam> R_SampleUsabilityParam
        {
            get { return _r_SampleUsabilityParam  ?? (_r_SampleUsabilityParam = CreateObjectSet<R_SampleUsabilityParam>("R_SampleUsabilityParam")); }
        }
        private ObjectSet<R_SampleUsabilityParam> _r_SampleUsabilityParam;
    
        public ObjectSet<R_TargetSpecies> R_TargetSpecies
        {
            get { return _r_TargetSpecies  ?? (_r_TargetSpecies = CreateObjectSet<R_TargetSpecies>("R_TargetSpecies")); }
        }
        private ObjectSet<R_TargetSpecies> _r_TargetSpecies;
    
        public ObjectSet<R_TripPlatformVersion> R_TripPlatformVersion
        {
            get { return _r_TripPlatformVersion  ?? (_r_TripPlatformVersion = CreateObjectSet<R_TripPlatformVersion>("R_TripPlatformVersion")); }
        }
        private ObjectSet<R_TripPlatformVersion> _r_TripPlatformVersion;
    
        public ObjectSet<R_TripUsabilityParam> R_TripUsabilityParam
        {
            get { return _r_TripUsabilityParam  ?? (_r_TripUsabilityParam = CreateObjectSet<R_TripUsabilityParam>("R_TripUsabilityParam")); }
        }
        private ObjectSet<R_TripUsabilityParam> _r_TripUsabilityParam;
    
        public ObjectSet<R_UsabilityParamUsabilityGrp> R_UsabilityParamUsabilityGrp
        {
            get { return _r_UsabilityParamUsabilityGrp  ?? (_r_UsabilityParamUsabilityGrp = CreateObjectSet<R_UsabilityParamUsabilityGrp>("R_UsabilityParamUsabilityGrp")); }
        }
        private ObjectSet<R_UsabilityParamUsabilityGrp> _r_UsabilityParamUsabilityGrp;
    
        public ObjectSet<replicationLog> replicationLog
        {
            get { return _replicationLog  ?? (_replicationLog = CreateObjectSet<replicationLog>("replicationLog")); }
        }
        private ObjectSet<replicationLog> _replicationLog;
    
        public ObjectSet<Sample> Sample
        {
            get { return _sample  ?? (_sample = CreateObjectSet<Sample>("Sample")); }
        }
        private ObjectSet<Sample> _sample;
    
        public ObjectSet<SpeciesList> SpeciesList
        {
            get { return _speciesList  ?? (_speciesList = CreateObjectSet<SpeciesList>("SpeciesList")); }
        }
        private ObjectSet<SpeciesList> _speciesList;
    
        public ObjectSet<SubSample> SubSample
        {
            get { return _subSample  ?? (_subSample = CreateObjectSet<SubSample>("SubSample")); }
        }
        private ObjectSet<SubSample> _subSample;
    
        public ObjectSet<Test_Niels> Test_Niels
        {
            get { return _test_Niels  ?? (_test_Niels = CreateObjectSet<Test_Niels>("Test_Niels")); }
        }
        private ObjectSet<Test_Niels> _test_Niels;
    
        public ObjectSet<TrawlOperation> TrawlOperation
        {
            get { return _trawlOperation  ?? (_trawlOperation = CreateObjectSet<TrawlOperation>("TrawlOperation")); }
        }
        private ObjectSet<TrawlOperation> _trawlOperation;
    
        public ObjectSet<TreatmentFactor> TreatmentFactor
        {
            get { return _treatmentFactor  ?? (_treatmentFactor = CreateObjectSet<TreatmentFactor>("TreatmentFactor")); }
        }
        private ObjectSet<TreatmentFactor> _treatmentFactor;
    
        public ObjectSet<Trip> Trip
        {
            get { return _trip  ?? (_trip = CreateObjectSet<Trip>("Trip")); }
        }
        private ObjectSet<Trip> _trip;
    
        public ObjectSet<ICES_DFU_Relation_FF> ICES_DFU_Relation_FF
        {
            get { return _iCES_DFU_Relation_FF  ?? (_iCES_DFU_Relation_FF = CreateObjectSet<ICES_DFU_Relation_FF>("ICES_DFU_Relation_FF")); }
        }
        private ObjectSet<ICES_DFU_Relation_FF> _iCES_DFU_Relation_FF;
    
        public ObjectSet<L_SelectionDevice> L_SelectionDevice
        {
            get { return _l_SelectionDevice  ?? (_l_SelectionDevice = CreateObjectSet<L_SelectionDevice>("L_SelectionDevice")); }
        }
        private ObjectSet<L_SelectionDevice> _l_SelectionDevice;
    
        public ObjectSet<R_GearTypeSelectionDevice> R_GearTypeSelectionDevice
        {
            get { return _r_GearTypeSelectionDevice  ?? (_r_GearTypeSelectionDevice = CreateObjectSet<R_GearTypeSelectionDevice>("R_GearTypeSelectionDevice")); }
        }
        private ObjectSet<R_GearTypeSelectionDevice> _r_GearTypeSelectionDevice;
    
        public ObjectSet<L_FisheryType> L_FisheryType
        {
            get { return _l_FisheryType  ?? (_l_FisheryType = CreateObjectSet<L_FisheryType>("L_FisheryType")); }
        }
        private ObjectSet<L_FisheryType> _l_FisheryType;
    
        public ObjectSet<L_HaulType> L_HaulType
        {
            get { return _l_HaulType  ?? (_l_HaulType = CreateObjectSet<L_HaulType>("L_HaulType")); }
        }
        private ObjectSet<L_HaulType> _l_HaulType;
    
        public ObjectSet<L_ThermoCline> L_ThermoCline
        {
            get { return _l_ThermoCline  ?? (_l_ThermoCline = CreateObjectSet<L_ThermoCline>("L_ThermoCline")); }
        }
        private ObjectSet<L_ThermoCline> _l_ThermoCline;
    
        public ObjectSet<L_TimeZone> L_TimeZone
        {
            get { return _l_TimeZone  ?? (_l_TimeZone = CreateObjectSet<L_TimeZone>("L_TimeZone")); }
        }
        private ObjectSet<L_TimeZone> _l_TimeZone;
    
        public ObjectSet<L_WeightEstimationMethod> L_WeightEstimationMethod
        {
            get { return _l_WeightEstimationMethod  ?? (_l_WeightEstimationMethod = CreateObjectSet<L_WeightEstimationMethod>("L_WeightEstimationMethod")); }
        }
        private ObjectSet<L_WeightEstimationMethod> _l_WeightEstimationMethod;
    
        public ObjectSet<L_EdgeStructure> L_EdgeStructure
        {
            get { return _l_EdgeStructure  ?? (_l_EdgeStructure = CreateObjectSet<L_EdgeStructure>("L_EdgeStructure")); }
        }
        private ObjectSet<L_EdgeStructure> _l_EdgeStructure;
    
        public ObjectSet<L_Parasite> L_Parasite
        {
            get { return _l_Parasite  ?? (_l_Parasite = CreateObjectSet<L_Parasite>("L_Parasite")); }
        }
        private ObjectSet<L_Parasite> _l_Parasite;
    
        public ObjectSet<L_Reference> L_Reference
        {
            get { return _l_Reference  ?? (_l_Reference = CreateObjectSet<L_Reference>("L_Reference")); }
        }
        private ObjectSet<L_Reference> _l_Reference;
    
        public ObjectSet<R_AnimalInfoReference> R_AnimalInfoReference
        {
            get { return _r_AnimalInfoReference  ?? (_r_AnimalInfoReference = CreateObjectSet<R_AnimalInfoReference>("R_AnimalInfoReference")); }
        }
        private ObjectSet<R_AnimalInfoReference> _r_AnimalInfoReference;
    
        public ObjectSet<Report> Report
        {
            get { return _report  ?? (_report = CreateObjectSet<Report>("Report")); }
        }
        private ObjectSet<Report> _report;
    
        public ObjectSet<ReportingTreeNode> ReportingTreeNode
        {
            get { return _reportingTreeNode  ?? (_reportingTreeNode = CreateObjectSet<ReportingTreeNode>("ReportingTreeNode")); }
        }
        private ObjectSet<ReportingTreeNode> _reportingTreeNode;
    
        public ObjectSet<AnimalFile> AnimalFile
        {
            get { return _animalFile  ?? (_animalFile = CreateObjectSet<AnimalFile>("AnimalFile")); }
        }
        private ObjectSet<AnimalFile> _animalFile;
    
        public ObjectSet<L_Application> L_Applications
        {
            get { return _l_Applications  ?? (_l_Applications = CreateObjectSet<L_Application>("L_Applications")); }
        }
        private ObjectSet<L_Application> _l_Applications;
    
        public ObjectSet<L_HatchMonthReadability> L_HatchMonthReadability
        {
            get { return _l_HatchMonthReadability  ?? (_l_HatchMonthReadability = CreateObjectSet<L_HatchMonthReadability>("L_HatchMonthReadability")); }
        }
        private ObjectSet<L_HatchMonthReadability> _l_HatchMonthReadability;
    
        public ObjectSet<L_VisualStock> L_VisualStock
        {
            get { return _l_VisualStock  ?? (_l_VisualStock = CreateObjectSet<L_VisualStock>("L_VisualStock")); }
        }
        private ObjectSet<L_VisualStock> _l_VisualStock;
    
        public ObjectSet<L_GeneticStock> L_GeneticStock
        {
            get { return _l_GeneticStock  ?? (_l_GeneticStock = CreateObjectSet<L_GeneticStock>("L_GeneticStock")); }
        }
        private ObjectSet<L_GeneticStock> _l_GeneticStock;
    
        public ObjectSet<L_SDEventType> L_SDEventType
        {
            get { return _l_SDEventType  ?? (_l_SDEventType = CreateObjectSet<L_SDEventType>("L_SDEventType")); }
        }
        private ObjectSet<L_SDEventType> _l_SDEventType;
    
        public ObjectSet<L_SDPurpose> L_SDPurpose
        {
            get { return _l_SDPurpose  ?? (_l_SDPurpose = CreateObjectSet<L_SDPurpose>("L_SDPurpose")); }
        }
        private ObjectSet<L_SDPurpose> _l_SDPurpose;
    
        public ObjectSet<SDAnnotation> SDAnnotation
        {
            get { return _sDAnnotation  ?? (_sDAnnotation = CreateObjectSet<SDAnnotation>("SDAnnotation")); }
        }
        private ObjectSet<SDAnnotation> _sDAnnotation;
    
        public ObjectSet<SDEvent> SDEvent
        {
            get { return _sDEvent  ?? (_sDEvent = CreateObjectSet<SDEvent>("SDEvent")); }
        }
        private ObjectSet<SDEvent> _sDEvent;
    
        public ObjectSet<SDFile> SDFile
        {
            get { return _sDFile  ?? (_sDFile = CreateObjectSet<SDFile>("SDFile")); }
        }
        private ObjectSet<SDFile> _sDFile;
    
        public ObjectSet<SDLine> SDLine
        {
            get { return _sDLine  ?? (_sDLine = CreateObjectSet<SDLine>("SDLine")); }
        }
        private ObjectSet<SDLine> _sDLine;
    
        public ObjectSet<SDPoint> SDPoint
        {
            get { return _sDPoint  ?? (_sDPoint = CreateObjectSet<SDPoint>("SDPoint")); }
        }
        private ObjectSet<SDPoint> _sDPoint;
    
        public ObjectSet<SDSample> SDSample
        {
            get { return _sDSample  ?? (_sDSample = CreateObjectSet<SDSample>("SDSample")); }
        }
        private ObjectSet<SDSample> _sDSample;
    
        public ObjectSet<L_SDLightType> L_SDLightType
        {
            get { return _l_SDLightType  ?? (_l_SDLightType = CreateObjectSet<L_SDLightType>("L_SDLightType")); }
        }
        private ObjectSet<L_SDLightType> _l_SDLightType;
    
        public ObjectSet<L_SDOtolithDescription> L_SDOtolithDescription
        {
            get { return _l_SDOtolithDescription  ?? (_l_SDOtolithDescription = CreateObjectSet<L_SDOtolithDescription>("L_SDOtolithDescription")); }
        }
        private ObjectSet<L_SDOtolithDescription> _l_SDOtolithDescription;
    
        public ObjectSet<L_SDPreparationMethod> L_SDPreparationMethod
        {
            get { return _l_SDPreparationMethod  ?? (_l_SDPreparationMethod = CreateObjectSet<L_SDPreparationMethod>("L_SDPreparationMethod")); }
        }
        private ObjectSet<L_SDPreparationMethod> _l_SDPreparationMethod;
    
        public ObjectSet<L_SDSampleType> L_SDSampleType
        {
            get { return _l_SDSampleType  ?? (_l_SDSampleType = CreateObjectSet<L_SDSampleType>("L_SDSampleType")); }
        }
        private ObjectSet<L_SDSampleType> _l_SDSampleType;
    
        public ObjectSet<R_StockSpeciesArea> R_StockSpeciesArea
        {
            get { return _r_StockSpeciesArea  ?? (_r_StockSpeciesArea = CreateObjectSet<R_StockSpeciesArea>("R_StockSpeciesArea")); }
        }
        private ObjectSet<R_StockSpeciesArea> _r_StockSpeciesArea;
    
        public ObjectSet<L_SDReaderExperience> L_SDReaderExperience
        {
            get { return _l_SDReaderExperience  ?? (_l_SDReaderExperience = CreateObjectSet<L_SDReaderExperience>("L_SDReaderExperience")); }
        }
        private ObjectSet<L_SDReaderExperience> _l_SDReaderExperience;
    
        public ObjectSet<R_SDEventSDReader> R_SDEventSDReader
        {
            get { return _r_SDEventSDReader  ?? (_r_SDEventSDReader = CreateObjectSet<R_SDEventSDReader>("R_SDEventSDReader")); }
        }
        private ObjectSet<R_SDEventSDReader> _r_SDEventSDReader;
    
        public ObjectSet<R_SDReader> R_SDReader
        {
            get { return _r_SDReader  ?? (_r_SDReader = CreateObjectSet<R_SDReader>("R_SDReader")); }
        }
        private ObjectSet<R_SDReader> _r_SDReader;
    
        public ObjectSet<L_SDAnalysisParameter> L_SDAnalysisParameter
        {
            get { return _l_SDAnalysisParameter  ?? (_l_SDAnalysisParameter = CreateObjectSet<L_SDAnalysisParameter>("L_SDAnalysisParameter")); }
        }
        private ObjectSet<L_SDAnalysisParameter> _l_SDAnalysisParameter;
    
        public ObjectSet<L_SelectionDeviceSource> L_SelectionDeviceSource
        {
            get { return _l_SelectionDeviceSource  ?? (_l_SelectionDeviceSource = CreateObjectSet<L_SelectionDeviceSource>("L_SelectionDeviceSource")); }
        }
        private ObjectSet<L_SelectionDeviceSource> _l_SelectionDeviceSource;
    
        public ObjectSet<L_LengthMeasureType> L_LengthMeasureType
        {
            get { return _l_LengthMeasureType  ?? (_l_LengthMeasureType = CreateObjectSet<L_LengthMeasureType>("L_LengthMeasureType")); }
        }
        private ObjectSet<L_LengthMeasureType> _l_LengthMeasureType;
    
        public ObjectSet<L_StomachStatus> L_StomachStatus
        {
            get { return _l_StomachStatus  ?? (_l_StomachStatus = CreateObjectSet<L_StomachStatus>("L_StomachStatus")); }
        }
        private ObjectSet<L_StomachStatus> _l_StomachStatus;

        #endregion

        #region Function Imports
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        /// <param name="strSampleId">No Metadata Documentation available.</param>
        public virtual ObjectResult<SIA_GetSampleGrandChildren_Result> SIA_GetSampleGrandChildren(string strSampleId)
        {
    
            ObjectParameter strSampleIdParameter;
    
            if (strSampleId != null)
            {
                strSampleIdParameter = new ObjectParameter("strSampleId", strSampleId);
            }
            else
            {
                strSampleIdParameter = new ObjectParameter("strSampleId", typeof(string));
            }
            return base.ExecuteFunction<SIA_GetSampleGrandChildren_Result>("SIA_GetSampleGrandChildren", strSampleIdParameter);
        }

        #endregion

    }
}
