using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using System.Reflection.Emit;
using System.Reflection;
using Babelfisk.Entities;

namespace Babelfisk.BusinessLogic.DataRetrieval
{
    internal class OfflineDataRetrievalClient : IDataClient, BabelfiskService.IDataRetrieval
    {

        #region IDataClient Interface methods


        public void Abort()
        {
            
        }

        public void Close()
        {
           
        }

        public IDataClient SupplyCredentials()
        {
            return this;
        }


        #endregion


        #region IDataRetrieval interface methods


        public Entities.Sprattus.L_Year[] GetTreeViewYears()
        {
            string strOfflineData = Settings.Settings.Instance.OfflineDataPath;

           // var ldv = new LookupDataVersioning();
           // var lst =  ldv.GetLocalLookups(typeof(L_Year).Name);

            List<L_Year> lstYears = new List<L_Year>();

            DirectoryInfo dirInfo = new DirectoryInfo(strOfflineData);
            
            foreach (var dirYear in dirInfo.GetDirectories())
            {
                string strYear = dirYear.Name;

                L_Year ly = new L_Year();
                ly.Year = strYear.ToInt32();

                //Get number of cruises
                ly.CruiseCount = dirYear.GetDirectories().Length;

                lstYears.Add(ly);
            }

            dirInfo = null;

            return lstYears.ToArray();
        }

        public int GetTreeViewTripCount(int intCruiseId, string strTripType)
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, int> GetTreeViewTripCounts(int[] CruiseIds, string strTripType)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Get array of cruises. strTripType is not used in the offline version (since the main tree does not support sorting of triptype)
        /// </summary>
        public Entities.Sprattus.Cruise[] GetTreeViewCruises(int intYear, string strTripType)
        {
            string strOfflineData = Settings.Settings.Instance.OfflineDataPath;
            string strYearPath = Path.Combine(strOfflineData, intYear.ToString());

            if(!Directory.Exists(strYearPath))
                return new Cruise[] {};

            DirectoryInfo dirInfo = new DirectoryInfo(strYearPath);

            //Loop through all cruises
            List<Cruise> lstCruises = new List<Cruise>();
            foreach (var cruiseFolder in dirInfo.GetDirectories())
            {
                string strCruiseFilePath = Path.Combine(cruiseFolder.FullName, "cruise.bin");

                if (!File.Exists(strCruiseFilePath))
                    continue;

                //Retrieve cruise entity
                byte[] arr = File.ReadAllBytes(strCruiseFilePath);
                arr = arr.Decompress();
                Cruise c = arr.ToObjectDataContract<Cruise>();

                //Set trip count property
                string strTripsFolder = Path.Combine(cruiseFolder.FullName, "trips");
                if(Directory.Exists(strTripsFolder))
                    c.TripCount = Directory.GetFiles(strTripsFolder).Length;

                lstCruises.Add(c);
            }

            return lstCruises.ToArray();
        }



        public Entities.Sprattus.Cruise GetCruiseFromCruiseId(int intCruiseId)
        {
            string strOfflineData = Settings.Settings.Instance.OfflineDataPath;
            int? intYear = Offline.OfflineDictionary.Instance.GetYearFromCruiseId(intCruiseId);

            if (!intYear.HasValue)
                return null;

            string strCruisePath = Path.Combine(strOfflineData, intYear.ToString(), intCruiseId.ToString(), "cruise.bin");
            if (!File.Exists(strCruisePath))
                return null;

            //Retrieve cruise entity
            byte[] arr = File.ReadAllBytes(strCruisePath);
            arr = arr.Decompress();
            Cruise c = arr.ToObjectDataContract<Cruise>();

            return c;
        }


        public Entities.Sprattus.Trip[] GetTreeViewTrips(int intCruiseId)
        {
            //Entities.Sprattus.Sample s;
            int? intYear = Offline.OfflineDictionary.Instance.GetYearFromCruiseId(intCruiseId);

            if (!intYear.HasValue)
                new ApplicationException("Year was not found on disk.");

            string strOfflineData = Settings.Settings.Instance.OfflineDataPath;
            string strYearPath = Path.Combine(strOfflineData, intYear.ToString());
            string strCruisePath = Path.Combine(strYearPath, intCruiseId.ToString());
            string strTripsPath = Path.Combine(strCruisePath, "trips");

            //If cruise does not exist, something is wrong.
            if (!Directory.Exists(strCruisePath))
                new ApplicationException("Cruise was not found on disk.");

            //No trips exist for the cruise
            if (!Directory.Exists(strTripsPath))
                return new Entities.Sprattus.Trip[] { };

            List<Trip> lst = new List<Trip>();
            foreach (var trip in Directory.GetFiles(strTripsPath))
            {
                string strId = Path.GetFileNameWithoutExtension(trip);
                int intTripId = strId.ToInt32();
                Trip t = GetTripEntityFromTripId(intTripId);
                //Retrieve trip entity
               /* byte[] arr = File.ReadAllBytes(trip);
                arr = arr.Decompress();
                Trip t = arr.ToObjectDataContract<Trip>();*/
                t.SampleCount = t.Sample == null ? 0 : t.Sample.Count;
                t.SpeciesListCount = t.Sample == null ? 0 : (t.Sample.SelectMany(x => x.SpeciesList).Count());

                lst.Add(t);
            }

            return lst.ToArray();
        }


        public Entities.Sprattus.Sample[] GetTreeViewSamples(int intTripId)
        {
            Entities.Sprattus.Sample[] arrSamples = null;

            Trip t = GetTripEntityFromTripId(intTripId);

            if (t.Sample == null)
                arrSamples = new Sample[] { };
            else
            {
                t.Sample.ToList().ForEach(x =>
                {
                    if (x.SpeciesList != null)
                        x.SpeciesListCount = x.SpeciesList.Count;
                });
                arrSamples = t.Sample.ToArray();
            }

            return arrSamples;
        }


        public Entities.Sprattus.SpeciesList[] GetSpeciesLists(int intSampleId)
        {
            Trip t = GetTripEntityFromSampleId(intSampleId);

            Sample s = t.Sample == null ? null : t.Sample.Where(x => x.sampleId == intSampleId).FirstOrDefault();

            if (s == null || s.SpeciesList == null)
                return new Entities.Sprattus.SpeciesList[] { };

            //10-11-2017: Added cloning of sample before taking specieslist records, since this is required in order to reset any unsaved edits to row.
            s = s.OmitClone("Trip");

            var arr = s.SpeciesList.ToArray();

            return arr;
        }



        public Entities.Sprattus.Animal[] GetAnimals(int intSubSampleId)
        {
            Trip t = GetTripEntityFromSubSampleId(intSubSampleId);

            SubSample ss = t.Sample.SelectMany(x => x.SpeciesList).SelectMany(x => x.SubSample).Where(x => x.subSampleId == intSubSampleId).FirstOrDefault();

            ss = ss.OmitClone("SpeciesList");

            return ss.Animal.ToArray();
        }

        public Entities.Sprattus.Age[] GetAges(int intSubSampleId)
        {
            Trip t = GetTripEntityFromSubSampleId(intSubSampleId);

            SubSample ss = t.Sample.SelectMany(x => x.SpeciesList).SelectMany(x => x.SubSample).Where(x => x.subSampleId == intSubSampleId).FirstOrDefault();

            ss = ss.OmitClone("SpeciesList");

            return ss.Animal.SelectMany(x => x.Age).ToArray();
        }

        public Entities.Sprattus.AnimalFile[] GetAnimalFiles(int intSubSampleId)
        {
            Trip t = GetTripEntityFromSubSampleId(intSubSampleId);

            SubSample ss = t.Sample.SelectMany(x => x.SpeciesList).SelectMany(x => x.SubSample).Where(x => x.subSampleId == intSubSampleId).FirstOrDefault();

            ss = ss.OmitClone("SpeciesList");

            return ss.Animal.SelectMany(x => x.AnimalFiles).ToArray();
        }

        public Entities.Sprattus.AnimalInfo[] GetAnimalInfos(int intSubSampleId)
        {
            Trip t = GetTripEntityFromSubSampleId(intSubSampleId);

            SubSample ss = t.Sample.SelectMany(x => x.SpeciesList).SelectMany(x => x.SubSample).Where(x => x.subSampleId == intSubSampleId).FirstOrDefault();

            ss = ss.OmitClone("SpeciesList");

            return ss.Animal.SelectMany(x => x.AnimalInfo).ToArray();
        }

        public Entities.Sprattus.R_AnimalInfoReference[] GetAnimalInfoReferences(int intSubSampleId)
        {
            Trip t = GetTripEntityFromSubSampleId(intSubSampleId);

            SubSample ss = t.Sample.SelectMany(x => x.SpeciesList).SelectMany(x => x.SubSample).Where(x => x.subSampleId == intSubSampleId).FirstOrDefault();

            ss = ss.OmitClone("SpeciesList");

            return ss.Animal.SelectMany(x => x.AnimalInfo).SelectMany(x => x.R_AnimalInfoReference).ToArray();
        }


        public byte[] GetOfflineSelectionData(int[] lstYears)
        {
            throw new NotImplementedException();
        }

        public byte[] GetOfflineTripData(int[] lstTripIds)
        {
            throw new NotImplementedException();
        }

        byte[] BabelfiskService.IDataRetrieval.GetDataForRaising(int[] lstCruiseIds, int[] lstTripIds, int[] lstSampleIds)
        {
            throw new NotImplementedException();
        }

        public Cruise GetCruise(int year, string cruiseName)
        {
            throw new NotImplementedException();
        }

        public byte[] GetStationsForDataImport(int cruiseId, string trip, string[] stations)
        {
            throw new NotImplementedException();
        }

        #endregion



        public static Trip GetTripEntityFromTripId(int intTripId, bool blnUseFreshCopy = false)
        {
            if ((Offline.OfflineDictionary.LastLoadedTrip != null && Offline.OfflineDictionary.LastLoadedTrip.tripId == intTripId) && !blnUseFreshCopy)
                return Offline.OfflineDictionary.LastLoadedTrip;//.Clone(); //Clone is very slow for large entities, another strategy should be used.

            int? intCruiseId = Offline.OfflineDictionary.Instance.GetCruiseIdFromTripId(intTripId);

            if (intCruiseId == null)
                new ApplicationException("Cruise was not found on disk.");

            int? intYear = Offline.OfflineDictionary.Instance.GetYearFromCruiseId(intCruiseId.Value);

            if (intYear == null)
                new ApplicationException("Year was not found on disk.");


            string strOfflineData = Settings.Settings.Instance.OfflineDataPath;

            string strTripPath = Path.Combine(strOfflineData, intYear.Value.ToString(), intCruiseId.Value.ToString(), "trips", intTripId.ToString() + ".bin");

            if (!File.Exists(strTripPath))
                new ApplicationException("Trip was not found on disk.");

            byte[] arr = File.ReadAllBytes(strTripPath);
            arr = arr.Decompress();
            Trip t = arr.ToObjectDataContract<Trip>();

            Offline.OfflineDictionary.AssignLastLoadedTrip(t, false);

            return t;
        }


        public static Trip GetTripEntityFromSampleId(int intSampleId)
        {
            int? intTripId = Offline.OfflineDictionary.Instance.GetTripIdFromSampleId(intSampleId);

            if (intTripId == null)
                new ApplicationException("Trip was not found on disk.");

            Trip t = GetTripEntityFromTripId(intTripId.Value);

            return t;
        }


        public static Trip GetTripEntityFromSpeciesListId(int intSpeciesListId)
        {
            int? intSampleId = Offline.OfflineDictionary.Instance.GetSampleIdFromSpeciesListId(intSpeciesListId);

            if (intSampleId == null)
                new ApplicationException("Sample was not found on disk.");

            return GetTripEntityFromSampleId(intSampleId.Value);
        }


        public static Trip GetTripEntityFromSubSampleId(int intSubSampleId)
        {
            int? intSpeciesList = Offline.OfflineDictionary.Instance.GetSpeciesListIdFromSubSampleId(intSubSampleId);

            if (intSpeciesList == null)
                new ApplicationException("SpeciesList was not found on disk.");

            return GetTripEntityFromSpeciesListId(intSpeciesList.Value);
        }


        public List<Cruise> GetDataForRaising(List<int> lstCruiseIdsWithNoTrips, List<int> lstTripIdsWithNoSamples, List<int> lstSampleIds)
        {
            Dictionary<int, Cruise> dic = new Dictionary<int, Cruise>();

            OfflineDataRetrievalClient datMan = new OfflineDataRetrievalClient();

            var lstTripsToLoad = lstSampleIds.Select(x => Offline.OfflineDictionary.Instance.GetTripIdFromSampleId(x)).Where(x => x.HasValue).Select(x => x.Value).Distinct().ToList();
            var lstCruisesToLoad = lstTripsToLoad.Select(x => Offline.OfflineDictionary.Instance.GetCruiseIdFromTripId(x)).Where(x => x.HasValue).Select(x => x.Value).Distinct().ToList();

            //Add cruises with no trips as well
            lstCruisesToLoad.AddRange(lstCruiseIdsWithNoTrips);

            //Add trips with no samples as well
            lstTripsToLoad.AddRange(lstTripIdsWithNoSamples);

            //Loop through cruise ids and load cruise entities from disk
            foreach (int intCruiseId in lstCruisesToLoad.Distinct())
            {
                var cruise = datMan.GetCruiseFromCruiseId(intCruiseId);

                //TODO: Handle if cruise is null and is therefore not found on disk

                if(cruise != null)
                    dic.Add(intCruiseId, cruise);
            }

            HashSet<int> hs = lstSampleIds.ToHashSet();
            //Loop through trip ids and load them from disk
            foreach (int intTripId in lstTripsToLoad)
            {
               Trip t = GetTripEntityFromTripId(intTripId);

               //TODO: Handle if trip is null and is therefore not found on disk

               if (t != null)
               {
                   //Added: 09-03-2016. Make sure the grabbed trip is a clone, because removing samples in memory objects, is dangerous for other methods thinking the memory object contains all samples.
                   //This is added because of a critical error occuring when exporting only a subset of all samples in a trip. Afterwards, only the exported ones are in memory (in Offline.OfflineDictionary.LastLoadedTrip).
                   //So make sure all samples are in Offline.OfflineDictionary.LastLoadedTrip after an export, by working with a clone during export.
                   t = t.Clone();

                   if (t.Sample != null && t.Sample.Count > 0)
                   {
                       List<Sample> lstSamples = t.Sample.ToList();

                       //Remove samples that are not in lstSampleIds
                       foreach (var s in lstSamples)
                           if (!hs.Contains(s.sampleId))
                               t.Sample.Remove(s);
                   }

                   //Add trip to cruise entity
                   if (dic.ContainsKey(t.cruiseId))
                       dic[t.cruiseId].Trip.Add(t);
               }
            }

            return dic.Values.ToList();
        }


      
    }
}
