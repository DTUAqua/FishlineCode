using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.BusinessLogic.BabelfiskService;
using Babelfisk.Entities;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.BusinessLogic.DataInput
{
    public class DataInputManager
    {
        public T GetEntity<T>(int intId, params string[] include) where T : class, IObjectWithChangeTracker
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                IObjectWithChangeTracker res = null;
                string strName = typeof(T).Name;

                switch (strName)
                {
                    case "Cruise":
                        res = (sv as IDataInput).GetCruiseFromId(intId);
                        break;

                    case "Sample":
                        res = (sv as IDataInput).GetSampleFromId(intId);
                        break;

                    case "Trip":
                        res = (sv as IDataInput).GetTripFromId(intId, include);
                        break;

                    case "SpeciesList":
                        res = (sv as IDataInput).GetSpeciesListFromId(intId, include);
                        break;

                    case "SubSample":
                        res = (sv as IDataInput).GetSubSampleFromId(intId);
                        break;

                    default:
                        throw new ApplicationException("Entity is not yet supported.");
                }

                sv.Close();

                return res as T;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        /// <summary>
        /// Retrieve all years (distinct)
        /// </summary>
        public List<L_Year> GetYears()
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as IDataInput).GetYears();

                sv.Close();

                return arr.ToList();
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        /// <summary>
        /// Retrieve all cruise names (distinct)
        /// </summary>
        public List<string> GetCruiseNames()
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as IDataInput).GetCruiseNames();

                sv.Close();

                return arr.ToList();
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        public Cruise CreateAndGetCruise(int cruiseYear, string cruiseName)
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var cruise = (sv as IDataInput).CreateAndGetCruise(cruiseYear, cruiseName);

                sv.Close();

                return cruise;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        public List<L_StatisticalRectangle> GetStatisticalRectangleFromArea(string strAreaCode)
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as IDataInput).GetStatisticalRectangleFromArea(strAreaCode);

                sv.Close();

                return arr.ToList();
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        public List<L_SelectionDevice> GetSelectionDevicesFromGearType(string strGearType)
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as IDataInput).GetSelectionDevicesFromGearType(strGearType);

                sv.Close();

                return arr.ToList();
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        public DatabaseOperationResult SaveCruise(ref Cruise c)
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var res = (sv as IDataInput).SaveCruise(ref c);

                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }



        public DatabaseOperationResult SaveTrip(ref Trip t)
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var res = (sv as IDataInput).SaveTrip(ref t);

                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        public DatabaseOperationResult SaveSample(ref Sample s)
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var res = (sv as IDataInput).SaveSample(ref s);

                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        public DatabaseOperationResult SaveHVN(ref Trip t, ref Sample s)
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var res = (sv as IDataInput).SaveHVN(ref t, ref s);

                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        public Person GetLatestPersonFromPlatformId(int intPlatformId)
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var res = (sv as IDataInput).GetLatestPersonFromPlatformId(intPlatformId);

                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        public List<Sample> GetSamplesFromTripId(int intTripId)
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var res = (sv as IDataInput).GetSamplesFromTripId(intTripId);

                sv.Close();

                return res == null ? null : res.ToList();
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        public Sample GetLatestSampleFromTripId(int intTripId)
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var res = (sv as IDataInput).GetLatestSample(intTripId);

                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        public List<MapPoint> GetMapPositionsFromCruiseId(int intCruiseId)
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var res = (sv as IDataInput).GetMapPositionsFromCruiseId(intCruiseId);

                sv.Close();

                return res == null ? null : res.ToList();
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        public List<MapPoint> GetMapPositionsFromTripId(int intTripId)
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var res = (sv as IDataInput).GetMapPositionsFromTripId(intTripId);

                sv.Close();

                return res == null ? null : res.ToList();
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        public List<MapPoint> GetMapPositionsFromSampleId(int intSampleId)
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var res = (sv as IDataInput).GetMapPositionsFromSampleId(intSampleId);

                sv.Close();

                return res == null ? null : res.ToList();
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }



        public DatabaseOperationResult SaveSpeciesListItems(ref List<SpeciesList> lstSpeciesListItems)
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var arr = lstSpeciesListItems.ToArray<SpeciesList>();

                var res = (sv as IDataInput).SaveSpeciesListItems(ref arr);

                sv.Close();

                lstSpeciesListItems = arr.OfType<SpeciesList>().ToList();

                return res;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public DatabaseOperationResult SaveLavSFItems(SpeciesList sl, List<LavSFTransferItem> lstLavSFItems)
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var arr = lstLavSFItems.ToArray<LavSFTransferItem>();

                var res = (sv as IDataInput).SaveLavSFItems(sl, arr);

                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }



        #region Delete methods


        /// <summary>
        /// Dete cruise from database.
        /// </summary>
        public DatabaseOperationResult DeleteCruise(int intCruiseId)
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var res = (sv as IDataInput).DeleteCruise(intCruiseId);

                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        /// <summary>
        /// Dete trip from database.
        /// </summary>
        public DatabaseOperationResult DeleteTrip(int intTripId)
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var res = (sv as IDataInput).DeleteTrip(intTripId);

                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        /// <summary>
        /// Dete sample from database.
        /// </summary>
        public DatabaseOperationResult DeleteSample(int intSampleId)
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var res = (sv as IDataInput).DeleteSample(intSampleId);

                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        #endregion



        public DatabaseOperationResult RunFileSynchronizer()
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                sv.SupplyCredentials();

                var res = (sv as IDataInput).RunFileSynchronizer();

                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }
    }
}
