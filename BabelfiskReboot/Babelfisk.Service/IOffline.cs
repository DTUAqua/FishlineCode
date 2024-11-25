using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Babelfisk.Entities;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.Service
{
    [ServiceKnownType(typeof(DFUPerson))]
    [ServiceKnownType(typeof(L_DFUDepartment))]
    [ServiceKnownType(typeof(L_Nationality))]
    [ServiceKnownType(typeof(L_NavigationSystem))]
    [ServiceKnownType(typeof(L_Platform))]
    [ServiceKnownType(typeof(L_PlatformType))]
    [ServiceKnownType(typeof(Person))]
    [ServiceKnownType(typeof(L_Harbour))]
    [ServiceKnownType(typeof(L_DFUArea))]
    [ServiceKnownType(typeof(L_TreatmentFactorGroup))]
    [ServiceKnownType(typeof(L_Species))]
    [ServiceKnownType(typeof(L_GearQuality))]
    [ServiceKnownType(typeof(L_StatisticalRectangle))]
    [ServiceKnownType(typeof(L_LengthMeasureType))]
    [ServiceKnownType(typeof(L_LengthMeasureUnit))]
    [ServiceKnownType(typeof(L_MaturityIndexMethod))]
    [ServiceKnownType(typeof(L_SampleType))]
    [ServiceKnownType(typeof(L_GearType))]
    [ServiceKnownType(typeof(L_GearInfoType))]
    [ServiceKnownType(typeof(L_Gear))]
    [ServiceKnownType(typeof(R_GearInfo))]
    [ServiceKnownType(typeof(L_GearQuality))]
    [ServiceKnownType(typeof(L_SamplingMethod))]
    [ServiceKnownType(typeof(L_SamplingType))]
    [ServiceKnownType(typeof(L_Treatment))]
    [ServiceKnownType(typeof(L_Species))]
    [ServiceKnownType(typeof(TreatmentFactor))]
    [ServiceKnownType(typeof(Maturity))]
    [ServiceKnownType(typeof(ICES_DFU_Relation_FF))]
    [ServiceKnownType(typeof(L_SpeciesRegistration))]
    [ServiceKnownType(typeof(L_CatchRegistration))]
    [ServiceKnownType(typeof(L_SelectionDevice))]
    [ServiceKnownType(typeof(L_SelectionDeviceSource))]
    [ServiceKnownType(typeof(R_GearTypeSelectionDevice))]
    [ServiceKnownType(typeof(L_FisheryType))]
    [ServiceKnownType(typeof(L_LandingCategory))]
    [ServiceKnownType(typeof(L_ThermoCline))]
    [ServiceKnownType(typeof(L_HaulType))]
    [ServiceKnownType(typeof(L_Bottomtype))]
    [ServiceKnownType(typeof(L_TimeZone))]
    [ServiceKnownType(typeof(L_SizeSortingEU))]
    [ServiceKnownType(typeof(L_SizeSortingDFU))]
    [ServiceKnownType(typeof(L_SexCode))]
    [ServiceKnownType(typeof(L_WeightEstimationMethod))]
    [ServiceKnownType(typeof(L_BroodingPhase))]
    [ServiceKnownType(typeof(L_OtolithReadingRemark))]
    [ServiceKnownType(typeof(L_EdgeStructure))]
    [ServiceKnownType(typeof(L_Parasite))]
    [ServiceKnownType(typeof(L_Reference))]
    [ServiceKnownType(typeof(DataVersioning))]
    [ServiceKnownType(typeof(OfflineEntity))]
    [ServiceKnownType(typeof(L_SDEventType))]
    [ServiceKnownType(typeof(L_SDPurpose))]
    [ServiceKnownType(typeof(L_SDSampleType))]
    [ServiceKnownType(typeof(L_Stock))]
    [ServiceKnownType(typeof(L_SDLightType))]
    [ServiceKnownType(typeof(L_SDOtolithDescription))]
    [ServiceKnownType(typeof(L_SDPreparationMethod))]
    [ServiceKnownType(typeof(L_StomachStatus))]
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface IOffline
    {
        [OperationContract]
        byte[] GetOfflineSelectionData(List<int> lstYears);

        [OperationContract]
        byte[] GetOfflineSelectionDataWithSamples(List<int> lstYears);

        [OperationContract]
        byte[] GetOfflineCruiseData(List<int> lstCruiseIds);

        [OperationContract]
        byte[] GetOfflineTripData(List<int> lstTripIds);

        [OperationContract]
        bool IsOnline();

        [OperationContract]
        SyncDatabaseOperationResult SynchronizeLookups(ref List<ILookupEntity> lookups);

        [OperationContract]
        SyncDatabaseOperationResult SynchronizeCruise(ref Cruise c);

        [OperationContract]
        //SyncDatabaseOperationResult SynchronizeTrip(ref Trip t);
        //SyncDatabaseOperationResult SynchronizeTrip(ref byte[] tbytes);
        byte[] SynchronizeTrip(byte[] tbytes, ref SyncDatabaseOperationResult syncRef);
    }
}
