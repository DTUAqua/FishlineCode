using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Babelfisk.ViewModels
{
    public enum RegionName
    {
        MainRegion,
        WindowRegion,
        MenuRegion,
        LeftRegion,
        RightRegion
    }


    public enum TreeNodeLevel
    {
        Cruise,
        Trip,
        Sample,
        SpeciesList
    }


    public enum MapDisplayMode
    {
        Lines,
        Points
    }


    public enum OfflineProcessState
    {
        Idle,
        Analyzing,
        SyncLookups,
        Transferring
    }


    public enum ExportProcessState
    {
        Idle,
        Analyzing,
        Exporting
    }


    public enum OnlineProcessState
    {
        Idle,
        TestingConnectivity,
        SyncLookups,
        SyncCruises,
        SyncTrips
    }


    public enum ReportType
    {
        RScript,
        Document
    }

    public enum OutputFormat
    {
        All,
        PDF,
        Word
    }

    public enum CSVColumnHeader
    {
        SampleID,
        FromFL,
        ImageName,
        Species,
        Cruise,
        Trip,
        Station,
        CatchDate,
        AreaCode,
        StatisticalRectangle,
        Stock,
        LengthMM,
        WeightG,
        SexCode,
        MaturityIndexMethod,
        MturityIndex, 
        AQScore,
        PreparationMethod,
        LightType,
        OtoDescription,
        EdgeType,
        LatPosStartText,
        LonPosStartText,
        CreatedBy,
        Comments
    }

    public enum MessageType
    {
        None = 0,
        Duplicate = 1,
        Warning = 2,
        Error = 4
    }

}
