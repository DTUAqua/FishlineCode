using Babelfisk.Entities.Sprattus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anchor.Core;
using Babelfisk.Entities;
using FishLineMeasure.BusinessLogic.FishLineService;

namespace FishLineMeasure.BusinessLogic.Data
{
    public class DataManager
    {

        public Cruise GetCruise(int year, string cruise)
        {
            Cruise c = null;

            var srv = new FishLineService.DataRetrievalClient();
            srv.SupplyCredentials();

            try
            {
                c = srv.GetCruise(year, cruise);

                srv.Close();
            }
            catch (Exception e)
            {
                srv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }

            return c;
        }


        public List<Sample> GetStationsForDataImport(int cruiseId, string trip, List<string> lstStations)
        {
            List<Sample> lst = null;

            var srv = new FishLineService.DataRetrievalClient();
            srv.SupplyCredentials();

            try
            {
                var arrCompressed = srv.GetStationsForDataImport(cruiseId, trip, lstStations.ToArray());
                srv.Close();

                if (arrCompressed != null)
                {
                    var arr = arrCompressed.Decompress();
                    lst = arr.ToObjectDataContract<List<Sample>>();
                }
               
            }
            catch (Exception e)
            {
                srv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }

            return lst;
        }



        public SyncDatabaseOperationResult SynchronizeTrip(ref Trip t)
        {
            SyncDatabaseOperationResult res = SyncDatabaseOperationResult.CreateSuccessResult();
            var sv = new OfflineClient();

            try
            {
               
                sv.SupplyCredentials();

                byte[] arr = t.ToByteArrayDataContract();
                arr = arr.Compress();

                arr = sv.SynchronizeTrip(arr, ref res);

                arr = arr.Decompress();
                t = arr.ToObjectDataContract<Trip>();

                sv.Close();
            }
            catch (Exception e)
            {
                sv.Abort();
                throw e;
            }

            return res;

        }

    }
}
