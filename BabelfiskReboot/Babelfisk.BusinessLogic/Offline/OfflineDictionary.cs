using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anchor.Core;
using System.IO;
using Babelfisk.Entities.Sprattus;
using Babelfisk.Entities;
using System.Linq.Expressions;

namespace Babelfisk.BusinessLogic.Offline
{
    [Serializable]
    public class OfflineDictionary
    {
        //Default offline id.
        private int _intId = -1;

        public static OfflineDictionary Instance = Deserialize();

        /// <summary>
        /// Lists for mapping ids
        /// </summary>
        private Dictionary<int, int> _dicCruiseToYear = new Dictionary<int, int>();
        private Dictionary<int, int> _dicTripToCruise = new Dictionary<int, int>();
        private Dictionary<int, int> _dicSampleToTrip = new Dictionary<int, int>();
        private Dictionary<int, int> _dicSpeciesListToSample = new Dictionary<int, int>();
        private Dictionary<int, int> _dicSubSampleToSpeciesList = new Dictionary<int, int>();

        private Dictionary<int, OfflineDictionaryItem> _dicCruisesChanged = new Dictionary<int, OfflineDictionaryItem>();
        private Dictionary<int, OfflineDictionaryItem> _dicTripsChanged = new Dictionary<int, OfflineDictionaryItem>();
        private Dictionary<int, OfflineDictionaryItem> _dicLookupsChanged = new Dictionary<int, OfflineDictionaryItem>();

        /// <summary>
        /// Lists for UI suggestions.
        /// </summary>
        private List<L_Year> _lstYears = new List<L_Year>();
        private List<string> _lstCruiseNames = new List<string>();

        [NonSerialized]
        public static Trip LastLoadedTrip = null;


        private Dictionary<int, int> _dicOfflineToOnlineIdMap = new Dictionary<int, int>();


        /// <summary>
        /// Create a new unique id used for offline entities.
        /// </summary>
        public int CreateNewId()
        {
            return _intId--;
        }


        /// <summary>
        /// Assign last loaded trip (for fast retrieval). 
        /// This is for optimization reasons only.
        /// </summary>
        public static void AssignLastLoadedTrip(Trip t, bool blnClone = true)
        {
            LastLoadedTrip = blnClone ? t.Clone() : t;
        }


        #region Properties

        public List<L_Year> Years
        {
            get { return _lstYears; }
            set { _lstYears = value; }
        }

        public List<string> CruiseNames
        {
            get { return _lstCruiseNames; }
            set { _lstCruiseNames = value; }
        }


        public Dictionary<int, int> CruiseToYear
        {
            get { return _dicSampleToTrip; }
            set { _dicSampleToTrip = value; }
        }


        public Dictionary<int, int> TripToCruise
        {
            get { return _dicSampleToTrip; }
            set { _dicSampleToTrip = value; }
        }


        public Dictionary<int, int> SampleToTrip
        {
            get { return _dicSampleToTrip; }
            set { _dicSampleToTrip = value; }
        }


        public Dictionary<int, int> SpeciesListToSample
        {
            get { return _dicSpeciesListToSample; }
            set { _dicSpeciesListToSample = value; }
        }


        public Dictionary<int, int> SubSampleToSpeciesList
        {
            get { return _dicSubSampleToSpeciesList; }
            set { _dicSubSampleToSpeciesList = value; }
        }


        public Dictionary<int, OfflineDictionaryItem> ChangedCruises
        {
            get { return _dicCruisesChanged; }
        }


        public Dictionary<int, OfflineDictionaryItem> ChangedTrips
        {
            get { return _dicTripsChanged; }
        }

        public Dictionary<int, OfflineDictionaryItem> ChangedLookups
        {
            get { return _dicLookupsChanged; }
        }


        #endregion


        /// <summary>
        /// Singleton private constructor.
        /// </summary>
        private OfflineDictionary()
        {
            LastLoadedTrip = null;
        }


        public void ReloadDictionaryFromDisk()
        {
            Instance = Deserialize();
        }


        /// <summary>
        /// Load OfflineDictionary from disk.
        /// </summary>
        private static OfflineDictionary Deserialize()
        {
            OfflineDictionary od = new OfflineDictionary();
            try
            {
                string strOfflinePath = BusinessLogic.Settings.Settings.Instance.OfflineFolderPath;
                string strFilePath = Path.Combine(strOfflinePath, "OfflineDictionary.bin");

                if (File.Exists(strFilePath))
                {
                    byte[] arr = File.ReadAllBytes(strFilePath);
                    arr = arr.Decompress();
                    od = arr.ToObject<OfflineDictionary>();
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return od;
        }


        /// <summary>
        /// Add an added/changed trip to a dictionary, so it can be easily found when synchronizing data when going online.
        /// </summary>
        public void SetTripChanged(Trip t, Cruise c, ObjectState? overrideState = null)
        {
            string strName = String.Format("{0} -> {1} -> {2} - {3}", c.year, c.cruise1, t.tripType, t.trip1);
            if (!_dicTripsChanged.ContainsKey(t.tripId))
                _dicTripsChanged.Add(t.tripId, OfflineDictionaryItem.Create(t.tripId, strName, overrideState == null ? t.OfflineState : overrideState.Value, typeof(Trip)));

            else
                _dicTripsChanged[t.tripId].Name = strName;
        }


        public void RemoveChangedTrip(int intOfflineId)
        {
            if (_dicTripsChanged.ContainsKey(intOfflineId))
                _dicTripsChanged.Remove(intOfflineId);
        }


        /// <summary>
        /// Add an added/changed cruise to a dictionary, so it can be easily found when synchronizing data when going online.
        /// </summary>
        public void SetCruiseChanged(Cruise c)
        {
            string strName = String.Format("{0} -> {1}", c.year, c.cruise1);
            if (!_dicCruisesChanged.ContainsKey(c.cruiseId))
                _dicCruisesChanged.Add(c.cruiseId, OfflineDictionaryItem.Create(c.cruiseId, strName, c.OfflineState, typeof(Cruise)));
            else
                _dicCruisesChanged[c.cruiseId].Name = strName;
        }


        public void RemoveChangedCruise(int intOfflineId)
        {
            if (_dicCruisesChanged.ContainsKey(intOfflineId))
                _dicCruisesChanged.Remove(intOfflineId);
        }


        /// <summary>
        /// Add an added/changed cruise to a dictionary, so it can be easily found when synchronizing data when going online.
        /// </summary>
        public void SetLookupChanged<T>(T l, int intId) where T : OfflineEntity, ILookupEntity
        {
            string strName = String.Format("{0}", l.UIDisplay);
            if (!_dicLookupsChanged.ContainsKey(intId))
                _dicLookupsChanged.Add(intId, OfflineDictionaryItem.Create(intId, strName, l.OfflineState, typeof(T)));
            else
                _dicLookupsChanged[intId].Name = strName;
        }


        public void RemoveChangedLookup(int intId)
        {
            if (_dicLookupsChanged.ContainsKey(intId))
                _dicLookupsChanged.Remove(intId);
        }



        #region Add/Remove methods for id mapping


        public void AddCruise(Cruise c)
        {
            if (!_dicCruiseToYear.ContainsKey(c.cruiseId))
                _dicCruiseToYear.Add(c.cruiseId, c.year);

            if (!_lstCruiseNames.Contains(c.cruise1))
                _lstCruiseNames.Add(c.cruise1);

            if (!_lstYears.Where(y => y.Year == c.year).Any())
                _lstYears.Add(new L_Year() { Year = c.year, CruiseCount = 0 });
        }


        public void RemoveCruise(int intCruiseId)
        {
            foreach (var t in _dicTripToCruise.Where(x => x.Value == intCruiseId).ToList())
                RemoveTrip(t.Key);

            if (_dicCruiseToYear.ContainsKey(intCruiseId))
                _dicCruiseToYear.Remove(intCruiseId);
        }


        public void RemoveTrip(int intTripId)
        {
            foreach (var s in _dicSampleToTrip.Where(x => x.Value == intTripId).ToList())
                RemoveSample(s.Key);

            if (_dicTripToCruise.ContainsKey(intTripId))
                _dicTripToCruise.Remove(intTripId);
        }


        public void RemoveSample(int intSampleId)
        {
            foreach (var sl in _dicSpeciesListToSample.Where(x => x.Value == intSampleId).ToList())
                RemoveSpeciesList(sl.Key);

            if (_dicSampleToTrip.ContainsKey(intSampleId))
                _dicSampleToTrip.Remove(intSampleId);
        }


        public void RemoveSpeciesList(int intSpeciesListId)
        {
            foreach (var ss in _dicSubSampleToSpeciesList.Where(x => x.Value == intSpeciesListId).ToList())
            {
                if (_dicSubSampleToSpeciesList.ContainsKey(ss.Key))
                    _dicSubSampleToSpeciesList.Remove(ss.Key);
            }

            if(_dicSpeciesListToSample.ContainsKey(intSpeciesListId))
                _dicSpeciesListToSample.Remove(intSpeciesListId);
        }


        public void AddTrip(Trip t)
        {
            if (!_dicTripToCruise.ContainsKey(t.tripId))
                _dicTripToCruise.Add(t.tripId, t.cruiseId);
        }


        public void AddSample(Sample s)
        {
            if (!_dicSampleToTrip.ContainsKey(s.sampleId))
                _dicSampleToTrip.Add(s.sampleId, s.tripId);
        }


        public void AddSpeciesList(SpeciesList s)
        {
            if (!_dicSpeciesListToSample.ContainsKey(s.speciesListId))
                _dicSpeciesListToSample.Add(s.speciesListId, s.sampleId);
        }


        internal void RemoveSpeciesList(SpeciesList s)
        {
            RemoveSpeciesList(s.speciesListId);
        }


        public void AddSubSample(SubSample s)
        {
            if (!_dicSubSampleToSpeciesList.ContainsKey(s.subSampleId))
                _dicSubSampleToSpeciesList.Add(s.subSampleId, s.speciesListId);
        }


        internal void RemoveSubSample(SubSample s)
        {
            if (_dicSubSampleToSpeciesList.ContainsKey(s.subSampleId))
                _dicSubSampleToSpeciesList.Remove(s.subSampleId);
        }


        #endregion


        #region Get methods for id retrieval


        public int? GetYearFromCruiseId(int intCruiseId)
        {
            if (_dicCruiseToYear.ContainsKey(intCruiseId))
                return _dicCruiseToYear[intCruiseId];

            return null;
        }


        public int? GetCruiseIdFromTripId(int intTripId)
        {
            if (_dicTripToCruise.ContainsKey(intTripId))
                return _dicTripToCruise[intTripId];

            return null;
        }


        public int? GetTripIdFromSampleId(int intSampleId)
        {
            if (_dicSampleToTrip.ContainsKey(intSampleId))
                return _dicSampleToTrip[intSampleId];

            return null;
        }


        public int? GetSampleIdFromSpeciesListId(int intSpeciesListId)
        {
            if (_dicSpeciesListToSample.ContainsKey(intSpeciesListId))
                return _dicSpeciesListToSample[intSpeciesListId];

            return null;
        }


        public int? GetSpeciesListIdFromSubSampleId(int intSubSampleId)
        {
            if (_dicSubSampleToSpeciesList.ContainsKey(intSubSampleId))
                return _dicSubSampleToSpeciesList[intSubSampleId];

            return null;
        }


        #endregion


        /// <summary>
        /// Retrieve trips folder path for a specific cruise (id)
        /// </summary>
        public string GetTripsPathFromCruiseId(int intCruiseId)
        {
            if (!_dicCruiseToYear.ContainsKey(intCruiseId))
                throw new ApplicationException(string.Format("Cruise id '{0}' was not found in offline dictionary.", intCruiseId));

            var intYear = _dicCruiseToYear[intCruiseId];

            string strOfflineData = Settings.Settings.Instance.OfflineDataPath;
            string strYearFolderPath = Path.Combine(strOfflineData, intYear.ToString());
            string strCruiseFolderPath = Path.Combine(strYearFolderPath, intCruiseId.ToString());
            string strTripsFolderPath = Path.Combine(strCruiseFolderPath, "trips");

            return strTripsFolderPath;
        }



        /// <summary>
        /// Retrieve file path for a specific trip id.
        /// </summary>
        public string GetTripsPathFromTripId(int intTripId)
        {
            if(!_dicTripToCruise.ContainsKey(intTripId))
                throw new ApplicationException(string.Format("Trip id '{0}' was not found in offline dictionary.", intTripId));

            var intCruiseId = _dicCruiseToYear[intTripId];

            return GetTripsPathFromCruiseId(intCruiseId);

        }


        /// <summary>
        /// Reset OfflineDictionary.
        /// </summary>
        public static OfflineDictionary New()
        {
            Instance = new OfflineDictionary();
            return Instance;
        }


        /// <summary>
        /// Update offline lists used for Offline UI-suggestions and validation
        /// </summary>
        public void UpdateLists()
        {
            DataInput.DataInputManager datMan = new DataInput.DataInputManager();
            _lstYears = datMan.GetYears();
            _lstCruiseNames = datMan.GetCruiseNames();
        }


        /// <summary>
        /// Update offline lists used for OFfline UI-suggestions and validation.
        /// Save OFflineDictionary to disk afterwards.
        /// </summary>
        public void UpdateListsAndSave()
        {
            UpdateLists();
            Save();
        }


        /// <summary>
        /// Save offline dictionary to disk.
        /// </summary>
        public void Save()
        {
            try
            {
                string strOfflinePath = BusinessLogic.Settings.Settings.Instance.OfflineFolderPath;

                byte[] arr = this.ToByteArray();
                arr = arr.Compress();

                File.WriteAllBytes(Path.Combine(strOfflinePath, "OfflineDictionary.bin"), arr);
            }
            catch(Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                throw e;
            }
        }


        /// <summary>
        /// Merge offline dictionary with this one. (used when importing offline data)
        /// </summary>
        public void Merge(OfflineDictionary od)
        {
            //Merge temporary ids
            //_intId = Math.Min(od._intId, _intId); this should not be done. Instead all new ids from the imported data, shoule be regenerated from _intId of this OFflineDictionary

            //Add data from od to current dictionary
        }


        public void AddOfflineToOnlineId(int intOfflineId, int intOnlineId)
        {
            if (!_dicOfflineToOnlineIdMap.ContainsKey(intOfflineId))
                _dicOfflineToOnlineIdMap.Add(intOfflineId, intOnlineId);
            else
                _dicOfflineToOnlineIdMap[intOfflineId] = intOnlineId;
        }


        public int? GetOnlineId(int intOfflineId)
        {
            if (_dicOfflineToOnlineIdMap.ContainsKey(intOfflineId))
                return _dicOfflineToOnlineIdMap[intOfflineId];

            return null;
        }


        public List<OfflineSelectionRecord> GetOfflineSelectionDataWithSamples(List<int> lstYears)
        {
            List<OfflineSelectionRecord> lst = new List<OfflineSelectionRecord>();

            foreach (var kvYear in _dicCruiseToYear)
            {
                if(lstYears.Contains(kvYear.Value))
                {
                    var trips = _dicTripToCruise.Where(x => x.Value == kvYear.Key);

                    if (trips.Count() > 0)
                    {
                        foreach (var kvTrip in trips)
                        {
                            var samples = _dicSampleToTrip.Where(x => x.Value == kvTrip.Key);
                            if (samples.Count() > 0)
                            {
                                foreach (var kvSample in samples)
                                {
                                    lst.Add(new OfflineSelectionRecord() { Year = kvYear.Value, CruiseId = kvYear.Key, TripId = kvTrip.Key, SampleId = kvSample.Key });
                                }
                            }
                            else
                                lst.Add(new OfflineSelectionRecord() { Year = kvYear.Value, CruiseId = kvYear.Key, TripId = kvTrip.Key }); 
                        }
                    }
                    else
                        lst.Add(new OfflineSelectionRecord() { Year = kvYear.Value, CruiseId = kvYear.Key }); 
                }
            }

            return lst;
        }
    }
}
