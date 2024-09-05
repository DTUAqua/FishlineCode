using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Babelfisk.Entities
{
    [DataContract()]
    public enum DatabaseOperationStatus
    {
        [EnumMember]
        Successful,
        [EnumMember]
        DuplicateRecordException,
        [EnumMember]
        ConcurrencyException,
        [EnumMember]
        UnexpectedException,
        [EnumMember]
        ValidationError
    }

    [DataContract()]
    public enum OverwritingMethod
    {
        [EnumMember]
        None,
        [EnumMember]
        ClientWins,
        [EnumMember]
        ServerWins
    }

    [DataContract()]
    public enum SecurityTask
    {
        [EnumMember] ReadData,
        [EnumMember] EditUsers,
        [EnumMember] ViewLookups,
        [EnumMember] EditLookups,
        [EnumMember] EditSomeLookups,
        [EnumMember] ModifyData,
        [EnumMember] DeleteData,
        [EnumMember] ExportToWarehouse,
        [EnumMember] GoOffline,
        [EnumMember] ExportData,
        [EnumMember] ViewReports,
        [EnumMember] EditReports,
        [EnumMember] ViewDFADReports,
        [EnumMember] AddEditSDEventsAndSamples,
        [EnumMember] ViewSDEventsAndSamples,
        [EnumMember] EditSDAnnotations,
        [EnumMember] DeleteAnimals
    }


    [DataContract()]
    public enum SubSampleType
    {
        [EnumMember]
        LAVRep,
        [EnumMember]
        SFRep,
        [EnumMember]
        SFNotRep,
        [EnumMember]
        Other,
        [EnumMember]
        Unknown
    }

    [DataContract()]
    public enum DataExportStatus
    {
        [EnumMember]
        Idle,
        [EnumMember]
        RetrievingData,
        [EnumMember]
        RaiseData,
        [EnumMember]
        OrganizeData,
        [EnumMember]
        SavingData,
        [EnumMember]
        SavingToDataWarehouse
    }


    [DataContract()]
    public enum ReportResultType
    {
        PDF,
        Word
    }

    [DataContract]
    public enum AnimalFileType
    {
        [EnumMember]
        OtolithImage
    }


    [DataContract]
    public enum SDSampleImportStatus
    {
        [EnumMember]
        ImportedFromFishline
    }

    [DataContract]
    public enum SDEventTypeAgeUpdatingMethod
    {
        [EnumMember]
        NeverUpdateAges,

        [EnumMember]
        UpdateAges
    }

    [DataContract]
    public enum SDFilesExtraColumn
    {
        [EnumMember]
        CatchDate,
        [EnumMember]
        Length,
        [EnumMember]
        Area,
        [EnumMember]
        Stock,
        [EnumMember]
        StatisticalRectangle,
        [EnumMember]
        Sex,
        [EnumMember]
        SampleOrigin,
        [EnumMember]
        Maturity,
        [EnumMember]
        PreperationMethod
    }

    [DataContract]
    public enum FileSystemType
    {
        [EnumMember]
        File = 1,
        [EnumMember]
        Directory = 2
    }



}
