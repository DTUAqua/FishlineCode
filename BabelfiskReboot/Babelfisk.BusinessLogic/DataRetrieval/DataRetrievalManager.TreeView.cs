using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using Babelfisk.BusinessLogic.BabelfiskService;

namespace Babelfisk.BusinessLogic
{
    public partial class DataRetrievalManager
    {

        public List<L_Year> GetTreeViewYears()
        {

            var sv = DataClientFactory.CreateDataRetrievalClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as IDataRetrieval).GetTreeViewYears();
               
                sv.Close();

                return arr.ToList();
            }
            catch (Exception e)
            {
                sv.Abort();
                throw e;
            }
        }


        public List<Cruise> GetTreeViewCruises(int intYear, string strTripType = null)
        {

            var sv = DataClientFactory.CreateDataRetrievalClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as IDataRetrieval).GetTreeViewCruises(intYear, strTripType);

                sv.Close();

                return arr.ToList();
            }
            catch (Exception e)
            {
                sv.Abort();
                throw e;
            }
        }


        public int GetTreeViewTripCount(int intCruiseId, string strTripType = null)
        {
            var sv = DataClientFactory.CreateDataRetrievalClient();

            try
            {
                sv.SupplyCredentials();

                var c = (sv as IDataRetrieval).GetTreeViewTripCount(intCruiseId, strTripType);

                sv.Close();

                return c;
            }
            catch (Exception e)
            {
                sv.Abort();
                throw e;
            }
        }


        public Dictionary<int, int> GetTreeViewTripCounts(int[] CruiseIds, string strTripType = null)
        {
            var sv = DataClientFactory.CreateDataRetrievalClient();

            try
            {
                sv.SupplyCredentials();

                var c = (sv as IDataRetrieval).GetTreeViewTripCounts(CruiseIds, strTripType);

                sv.Close();

                return c;
            }
            catch (Exception e)
            {
                sv.Abort();
                throw e;
            }
        }


        public List<Trip> GetTreeViewTrips(int intCruiseId)
        {

            var sv = DataClientFactory.CreateDataRetrievalClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as IDataRetrieval).GetTreeViewTrips(intCruiseId);

                sv.Close();

                return arr.ToList();
            }
            catch (Exception e)
            {
                sv.Abort();
                throw e;
            }
        }


        public List<Sample> GetTreeViewSamples(int intTripId)
        {

            var sv = DataClientFactory.CreateDataRetrievalClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as IDataRetrieval).GetTreeViewSamples(intTripId);

                sv.Close();

                return arr.ToList();
            }
            catch (Exception e)
            {
                sv.Abort();
                throw e;
            }
        }
    }
}
