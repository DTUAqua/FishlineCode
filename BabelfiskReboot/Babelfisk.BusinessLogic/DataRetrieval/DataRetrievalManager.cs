using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.BusinessLogic.BabelfiskService;
using Babelfisk.Entities;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;

namespace Babelfisk.BusinessLogic
{
    public partial class DataRetrievalManager
    {

        public string GetConnectedEndPoint()
        {
            var sv = DataClientFactory.CreateDataRetrievalClient();
            string strEndpoint = "Offline";

            try
            {
                if(sv is BabelfiskService.DataRetrievalClient)
                    strEndpoint = (sv as BabelfiskService.DataRetrievalClient).Endpoint.Address.ToString();

                sv.Close();
            }
            catch 
            {
                try { sv.Abort(); }
                catch { }
            }

            return strEndpoint;
        }


        public List<SpeciesList> GetSpeciesLists(int intSampleId)
        {
            var sv = DataClientFactory.CreateDataRetrievalClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as IDataRetrieval).GetSpeciesLists(intSampleId);

                sv.Close();

                List<SpeciesList> lst = arr.ToList();

                CalculateSpeciesListPresentFlags(lst);

                return lst;
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                sv.Abort();
                throw e;
            }
        }


        public static void CalculateSpeciesListPresentFlags(List<SpeciesList> lst)
        {
            if (lst == null || lst.Count == 0)
                return;

            //Assign LAV and SF
            lst.ForEach(s =>
            {
                s.IsLAVPresent = s.SubSample.SelectMany(x => x.Animal).Where(a => !a.individNum.HasValue || a.individNum == 0).Count() > 0;
                s.IsSFPresent = s.SubSample.SelectMany(x => x.Animal).Where(a => a.individNum.HasValue && a.individNum > 0).Count() > 0;
            });
        }


        public List<Animal> GetAnimals(int intSubSampleId)
        {
            var sv = DataClientFactory.CreateDataRetrievalClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as IDataRetrieval).GetAnimals(intSubSampleId);

                sv.Close();

                return arr.ToList();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                sv.Abort();
                throw e;
            }
        }


        public List<Age> GetAges(int intSubSampleId)
        {
            var sv = DataClientFactory.CreateDataRetrievalClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as IDataRetrieval).GetAges(intSubSampleId);

                sv.Close();

                return arr.ToList();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                sv.Abort();
                throw e;
            }
        }


        public List<AnimalFile> GetAnimalFiles(int intSubSampleId)
        {
            var sv = DataClientFactory.CreateDataRetrievalClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as IDataRetrieval).GetAnimalFiles(intSubSampleId);

                sv.Close();

                return arr.ToList();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                sv.Abort();
                throw e;
            }
        }


        public List<AnimalInfo> GetAnimalInfos(int intSubSampleId)
        {
            var sv = DataClientFactory.CreateDataRetrievalClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as IDataRetrieval).GetAnimalInfos(intSubSampleId);

                sv.Close();

                return arr.ToList();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                sv.Abort();
                throw e;
            }
        }


        public List<R_AnimalInfoReference> GetAnimalInfoReferences(int intSubSampleId)
        {
            var sv = DataClientFactory.CreateDataRetrievalClient();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as IDataRetrieval).GetAnimalInfoReferences(intSubSampleId);

                sv.Close();

                return arr.ToList();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                sv.Abort();
                throw e;
            }
        }


        public List<Cruise> GetDataForRaising(List<int> lstCruiseIds, List<int> lstTripIds, List<int> lstSampleIds)
        {
            var sv = DataClientFactory.CreateDataRetrievalClient();

            try
            {
                sv.SupplyCredentials();

                List<Cruise> lst = new List<Cruise>();

                if (BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
                {
                    lst = (sv as Babelfisk.BusinessLogic.DataRetrieval.OfflineDataRetrievalClient).GetDataForRaising(lstCruiseIds, lstTripIds, lstSampleIds);
                }
                else
                {
                    var arr = (sv as IDataRetrieval).GetDataForRaising(lstCruiseIds.ToArray(), lstTripIds.ToArray(), lstSampleIds.ToArray());

                    arr = arr.Decompress();
                    lst = arr.ToObjectDataContract<List<Cruise>>();
                }

                sv.Close();

                return lst;
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                sv.Abort();
                throw e;
            }
        }

    }
}
