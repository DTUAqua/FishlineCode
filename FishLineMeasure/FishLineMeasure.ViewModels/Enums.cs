using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels
{
    public enum RegionName
    {
        MenuRegion,
        MainRegion,
        WindowRegion,
        LeftRegion,
        RightRegion
    }


    public enum LookupType
    {
        Sex,
        Species,
        LandingCategory,
        SizeSortingEU,
        Ovigorous  //Rogn
    }




    public enum ConnectionState
    {
        Unknown,
        Connected,
        AlreadyConnected
    }


    public enum ExportLogState
    {
        Passed,
        Failed,
        Pending,
        Warning,
        Info,
        None
    }
}
