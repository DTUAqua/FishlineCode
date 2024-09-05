using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.BusinessLogic
{
    public class DataClientFactory
    {

        public static IDataClient CreateDataInputClient()
        {
            if (BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
                return new DataInput.OfflineDataInputClient();
            else
                return new DataInput.ServiceDataInputClient();
        }


        public static IDataClient CreateDataRetrievalClient()
        {
            if (BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
                return new DataRetrieval.OfflineDataRetrievalClient();
            else
                return new DataRetrieval.ServiceDataRetrievalClient();
        }


        public static IDataClient CreateSecurityClient()
        {
            if (BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
                return new Security.OfflineSecurityClient();
            else
                return new Security.ServiceSecurityClient();
           
        }


        public static IDataClient CreateLookupClient()
        {
            if (BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
                return new Lookup.OfflineLookupClient();
            else
                return new Lookup.ServiceLookupClient();
        }


        public static IDataClient CreateReportingClient()
        {
            if (BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
                return new Reporting.ServiceReportingClient();
            else
                return new Reporting.ServiceReportingClient();
        }


        public static IDataClient CreateFishLineSmartDotsClient()
        {
            return new SmartDots.FishLineSmartDotsClient();
        }

    }
}
