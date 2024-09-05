using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Anchor.Core;
using Babelfisk.BusinessLogic.Offline;
using Babelfisk.Entities;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.BusinessLogic.DataInput
{
    internal class OfflineDataInputClient : IDataClient, BabelfiskService.IDataInput
    {

        #region IDataClient interface methods


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


        #region IDataInput interface methods


        public Entities.Sprattus.L_Year[] GetYears()
        {
            var lstYears = Offline.OfflineDictionary.Instance.Years;

            return lstYears.ToArray();
        }


        public string[] GetCruiseNames()
        {
            var lstCruiseNames = Offline.OfflineDictionary.Instance.CruiseNames;

            return lstCruiseNames.ToArray();
        }


        /// <summary>
        /// Not available offline.
        /// </summary>
        public Cruise CreateAndGetCruise(int cruiseYear, string cruiseName)
        {
            return null;
        }


        /// <summary>
        /// Retrieve cruise entity from cruise id (loads cruise from disk).
        /// </summary>
        public Entities.Sprattus.Cruise GetCruiseFromId(int intCruiseId)
        {
            int? intYear = Offline.OfflineDictionary.Instance.GetYearFromCruiseId(intCruiseId);

            if (intYear == null)
                throw new ApplicationException("Year was not found on disk");

            string strOfflineData = Settings.Settings.Instance.OfflineDataPath;
            string strCruisePath = Path.Combine(strOfflineData, intYear.ToString(), intCruiseId.ToString(), "cruise.bin");

            if (!File.Exists(strCruisePath))
                throw new ApplicationException("Cruise was not found on disk");

            //Retrieve cruise entity
            byte[] arr = File.ReadAllBytes(strCruisePath);
            arr = arr.Decompress();
            Cruise c = arr.ToObjectDataContract<Cruise>();

            return c;
        }


        /// <summary>
        /// Retrieve Trip entity from trip id (loads trip from disk unless it is in memory already).
        /// </summary>
        public Entities.Sprattus.Trip GetTripFromId(int intTripId, string[] arrIncludes)
        {
            Trip t = DataRetrieval.OfflineDataRetrievalClient.GetTripEntityFromTripId(intTripId);

            t.OfflineInclude(arrIncludes);

            if (arrIncludes != null && arrIncludes.Where(x => x.Equals("Cruise")).Any())
                t.Cruise = GetCruiseFromId(t.cruiseId);

            return t;
        }


        /// <summary>
        /// TODO - this could be added as a list in the Offline dictionary.
        /// </summary>
        public Entities.Sprattus.Person GetLatestPersonFromPlatformId(int intPlatformId)
        {
            return null;
        }


        public Entities.Sprattus.Sample GetSampleFromId(int intSampleId)
        {
            Trip t = DataRetrieval.OfflineDataRetrievalClient.GetTripEntityFromSampleId(intSampleId);

            Sample s = t.Sample.Where(x => x.sampleId == intSampleId).FirstOrDefault();

            //Clone object (so changes wont affect anything)
            var st = s.OmitClone("Trip");

            return st;
        }


        public Entities.Sprattus.Sample[] GetSamplesFromTripId(int intTripId)
        {
            Trip t = DataRetrieval.OfflineDataRetrievalClient.GetTripEntityFromTripId(intTripId);

            return t.Sample.ToArray();
        }


        public Entities.Sprattus.SpeciesList GetSpeciesListFromId(int intSpeciesListId, string[] arrIncludes)
        {
            Trip t = DataRetrieval.OfflineDataRetrievalClient.GetTripEntityFromSpeciesListId(intSpeciesListId);

            var sl = t.Sample.SelectMany(x => x.SpeciesList).Where(x => x.speciesListId == intSpeciesListId).FirstOrDefault();

            //Clone object (so changes wont affect anything)
            sl = sl.OmitClone("Sample");

            sl.OfflineInclude(arrIncludes);

            if (arrIncludes.Contains("Sample.Trip.Cruise"))
            {
                Cruise c = GetCruiseFromId(t.cruiseId);
                t.AssignNavigationPropertyWithoutChanges("Cruise", c);
            }

            return sl;
        }

       
        public Entities.Sprattus.SubSample GetSubSampleFromId(int intSubSampleId)
        {
            Trip t = DataRetrieval.OfflineDataRetrievalClient.GetTripEntityFromSubSampleId(intSubSampleId);

            var ss = t.Sample.SelectMany(x => x.SpeciesList).SelectMany(x => x.SubSample).Where(x => x.subSampleId == intSubSampleId).FirstOrDefault();

            //Clone object (so changes wont affect anything)
            ss = ss.OmitClone("SpeciesList");

            return ss;
        }


        #region Save Cruise methods

        private bool ValidateCruiseDuplicateKey(Cruise cruise, Cruise[] arrExistingCruses)
        {
            //if (cruise.ChangeTracker.State != ObjectState.Added)
            //    return true;
            bool blnIsNew = cruise.ChangeTracker.State == ObjectState.Added;

            var vCruises = from c in arrExistingCruses
                           where c.cruise1.Equals(cruise.cruise1, StringComparison.InvariantCultureIgnoreCase) &&
                                 c.year.Equals(cruise.year) &&
                                 (c.cruiseId != cruise.cruiseId || blnIsNew)
                           select c;

            return vCruises.Count() == 0;
        }


        public Entities.DatabaseOperationResult SaveCruise(ref Entities.Sprattus.Cruise c)
        {
            var lstCruises = new DataRetrieval.OfflineDataRetrievalClient().GetTreeViewCruises(c.year, null);

            if (!ValidateCruiseDuplicateKey(c, lstCruises))
                return new DatabaseOperationResult(DatabaseOperationStatus.ValidationError, "DuplicateKey");

            //Assign temporary id, if it is a new object
            if (c.ChangeTracker.State == ObjectState.Added)
                c.cruiseId = Offline.OfflineDictionary.Instance.CreateNewId();

            int intYear = c.year;

            //Create folder structure to cruise file (if not already there)
            string strOfflineData = Settings.Settings.Instance.OfflineDataPath;
            string strYearFolderPath = Path.Combine(strOfflineData, intYear.ToString());

            if (!Directory.Exists(strYearFolderPath))
                Directory.CreateDirectory(strYearFolderPath);

            string strCruiseFolderPath = Path.Combine(strYearFolderPath, c.cruiseId.ToString());

            if(!Directory.Exists(strCruiseFolderPath))
                Directory.CreateDirectory(strCruiseFolderPath);

            string strCruisePath = Path.Combine(strCruiseFolderPath, "cruise.bin");

            //Update offline changes 
            UpdateEntityOfflineState(c);

            //and reset entity
            c.AcceptChanges();

            //Save entity to disk
            var arr = c.ToByteArrayDataContract();
            arr = arr.Compress();
            File.WriteAllBytes(strCruisePath, arr);

            //Add its id to the Offline dictionary.
            Offline.OfflineDictionary.Instance.AddCruise(c);
            Offline.OfflineDictionary.Instance.SetCruiseChanged(c);
            Offline.OfflineDictionary.Instance.Save();

            return DatabaseOperationResult.CreateSuccessResult();
        }

        #endregion


        #region Save HVN trip


        public Entities.DatabaseOperationResult SaveHVN(ref Entities.Sprattus.Trip t, ref Entities.Sprattus.Sample s)
        {
            var res = SaveTrip(ref t);

            if (res.DatabaseOperationStatus == DatabaseOperationStatus.Successful)
            {
                //Make sure to update trip Id of sample (in case trip and sample are new)
                if (s.tripId != t.tripId)
                    s.tripId = t.tripId;

                //Handle HVN SampleTypes
                string strGearType = s.gearType;
                var lstGearTypes = new LookupManager().GetLookups(typeof(L_GearType)).OfType<L_GearType>();
                var gearType = lstGearTypes.Where(x => x.gearType.Equals(strGearType)).FirstOrDefault();

                if (string.IsNullOrEmpty(s.gearType) || gearType == null)
                    s.sampleType = "X";
                else
                    s.sampleType = gearType.catchOperation;

                res = SaveSample(ref s);
            }

            return res;
        }


        #endregion


        #region Save Trip Methods

        private bool ValidateTripDuplicateKey(Trip trip, Trip[] arrExistingTrips)
        {
            bool blnIsNew = trip.ChangeTracker.State == ObjectState.Added;

            var vTrips = from t in arrExistingTrips
                         where t.trip1.Equals(trip.trip1, StringComparison.InvariantCultureIgnoreCase) &&
                               t.cruiseId.Equals(trip.cruiseId) &&
                               (t.tripId != trip.tripId || blnIsNew)
                         select t;

            return vTrips.Count() == 0;
        }


        public Entities.DatabaseOperationResult SaveTrip(ref Entities.Sprattus.Trip t)
        {
            var lstTrips = new DataRetrieval.OfflineDataRetrievalClient().GetTreeViewTrips(t.cruiseId);

            if (!ValidateTripDuplicateKey(t, lstTrips))
                return new DatabaseOperationResult(DatabaseOperationStatus.ValidationError, "DuplicateKey");

            //Assign temporary id, if it is a new object
            if (t.ChangeTracker.State == ObjectState.Added)
            {
                t.tripId = Offline.OfflineDictionary.Instance.CreateNewId();

                var lstSamplingTypes = new LookupManager().GetLookups(typeof(L_SamplingType)).OfType<L_SamplingType>();

                //Add default sampling type
                if (t.IsSEA)
                    t.samplingTypeId = lstSamplingTypes.Where(x => x.samplingType.Equals("S")).First().samplingTypeId;
            }

            //Update offline changes
            UpdateEntityOfflineState(t);
            
            //Store contact person details.
            var vContactPerson = t.Person;
            t.Person = null;

            if (vContactPerson != null && vContactPerson.ChangeTracker.State != ObjectState.Unchanged)
            {
                List<ILookupEntity> lst = new List<ILookupEntity>(new ILookupEntity[] { vContactPerson });
                new LookupManager().SaveLookups(ref lst);
            } 
            t.contactPersonId = vContactPerson == null ? new Nullable<int>() : vContactPerson.personId;

            //Save trip
            SaveOfflineTrip(t);

            return DatabaseOperationResult.CreateSuccessResult();
        }


        #endregion


        #region Save Sample Methods


        private bool ValidateSampleDuplicateKey(Sample sample, Sample[] lstSamples)
        {
            bool blnIsNew = sample.ChangeTracker.State == ObjectState.Added;

            var v = from s in lstSamples
                    where s.station.Equals(sample.station, StringComparison.InvariantCultureIgnoreCase) &&
                               s.tripId.Equals(sample.tripId) &&
                               (s.sampleId != sample.sampleId || blnIsNew)
                    select s;

            return v.Count() == 0;
        }


        public Entities.DatabaseOperationResult SaveSample(ref Entities.Sprattus.Sample s)
        {
            var lstSamples = new DataRetrieval.OfflineDataRetrievalClient().GetTreeViewSamples(s.tripId);

            if (!ValidateSampleDuplicateKey(s, lstSamples))
                return new DatabaseOperationResult(DatabaseOperationStatus.ValidationError, "DuplicateKey");

            //Assign temporary id, if it is a new object
            if (s.ChangeTracker.State == ObjectState.Added)
                s.sampleId = Offline.OfflineDictionary.Instance.CreateNewId();

            Trip t = GetTripFromId(s.tripId, null);

            //Update offline changes
            UpdateEntityOfflineState(s);

            //Handle a new R_TargetSpecies and update its offline state
            foreach(var rs in s.R_TargetSpecies)
            {
                if(rs.ChangeTracker.State == ObjectState.Added)
                    rs.TargetSpeciesId = Offline.OfflineDictionary.Instance.CreateNewId();
                UpdateEntityOfflineState(rs);
            }

            //Remove old sample, if there.
            if (s.ChangeTracker.State != ObjectState.Added)
            {
                int sampleId = s.sampleId;
                var oldSample = t.Sample.Where(x => x.sampleId == sampleId).FirstOrDefault();

                /*if (oldSample != null)
                    t.Sample.Remove(oldSample);*/
                if (oldSample != null)
                {
                    s.CopyEntityValueTypesTo(oldSample);

                    oldSample.R_TargetSpecies.Clear();
                    oldSample.R_TargetSpecies.AddRange(s.R_TargetSpecies.ToList());
                }
            }
            else
            {
                t.Sample.Add(s);
            }

            SaveOfflineTrip(t);

            //Add its id to the Offline dictionary.
            Offline.OfflineDictionary.Instance.AddSample(s);
            Offline.OfflineDictionary.Instance.Save();

            return DatabaseOperationResult.CreateSuccessResult();
        }


        #endregion


        #region Save species list items


        public Entities.DatabaseOperationResult SaveSpeciesListItems(ref Entities.Sprattus.SpeciesList[] speciesListItems)
        {
            if (speciesListItems == null || speciesListItems.Length == 0)
                return DatabaseOperationResult.CreateSuccessResult();

            var s = GetSampleFromId(speciesListItems.First().sampleId);

            foreach (var sl in speciesListItems)
            {
                //Assign temporary id, if it is a new object
                if (sl.ChangeTracker.State == ObjectState.Added)
                    sl.speciesListId = Offline.OfflineDictionary.Instance.CreateNewId();
                else if (sl.ChangeTracker.State == ObjectState.Deleted) //Remove deleted species list from collection.
                {
                    Offline.OfflineDictionary.Instance.RemoveSpeciesList(sl);
                    var oldSL = s.SpeciesList.Where(x => x.speciesListId == sl.speciesListId).FirstOrDefault();

                    if (oldSL != null)
                        s.SpeciesList.Remove(oldSL);

                    //Add deleted entity to sample
                    if (sl.OfflineState != ObjectState.Added)
                        s.OfflineDeletedEntities.Add(sl);
                    continue;
                }
                

                //Retreive SubSamples with no weight and remove them.
                var noWeight = sl.SubSample.Where(x => x.subSampleWeight == null && x.landingWeight == null && x.sumAnimalWeights == null).ToList();

                //Get subsamples at lower levels than the one supposed to have animals on it and clear their animal list (if any).
                var q = sl.SubSample.Where(x => x.IsRepresentative && (x.subSampleWeight != null || x.landingWeight != null || x.sumAnimalWeights != null)).OrderBy(x => x.stepNum).ToList();

                for (int i = 0; i < q.Count - 1; i++)
                  if (!noWeight.Contains(q[i]))
                      q[i].Animal.Clear(); //Clear animals from subsample

                //Remove subsamples with no wight
                foreach (var ss in noWeight)
                {
                    Offline.OfflineDictionary.Instance.RemoveSubSample(ss);
                    sl.SubSample.Remove(ss);
                }

                //Make sure to add offline ids to sub samples.
                foreach (var ss in sl.SubSample)
                {
                    //Assign temporary id, if it is a new object
                    if (ss.ChangeTracker.State == ObjectState.Added)
                        ss.subSampleId = Offline.OfflineDictionary.Instance.CreateNewId();

                    //Make sure specieslist ids match
                    if (ss.speciesListId != sl.speciesListId)
                        ss.speciesListId = sl.speciesListId;
                    
                    //Update offline changes
                    UpdateEntityOfflineState(ss);
                }

                //Remove old SpeciesList
                if (sl.ChangeTracker.State != ObjectState.Added)
                {
                    var oldSL = s.SpeciesList.Where(x => x.speciesListId == sl.speciesListId).FirstOrDefault();

                    if (oldSL != null) s.SpeciesList.Remove(oldSL);
                }

                //Add species list to sample
                s.SpeciesList.Add(sl);

                //Update offline changes
                UpdateEntityOfflineState(sl);
            }
           

            var t = GetTripFromId(s.tripId, null);

            //Remove old sample, if there.
            if (s.ChangeTracker.State != ObjectState.Added)
            {
                int sampleId = s.sampleId;
                var oldSample = t.Sample.Where(x => x.sampleId == sampleId).FirstOrDefault();

                if (oldSample != null)
                    t.Sample.Remove(oldSample);
            }

            t.Sample.Add(s);

            SaveOfflineTrip(t);

            //Add its id to the Offline dictionary.
            speciesListItems.ToList().ForEach(sl => Offline.OfflineDictionary.Instance.AddSpeciesList(sl));
            speciesListItems.SelectMany(x => x.SubSample).ToList().ForEach(ss => Offline.OfflineDictionary.Instance.AddSubSample(ss));
            Offline.OfflineDictionary.Instance.Save();

            return DatabaseOperationResult.CreateSuccessResult();
        }


        #endregion


        public Entities.DatabaseOperationResult SaveLavSFItems(Entities.Sprattus.SpeciesList sl, Entities.LavSFTransferItem[] lstItems)
        {
            if (sl == null)
                return DatabaseOperationResult.CreateSuccessResult();

            int? intTripId =  OfflineDictionary.Instance.GetTripIdFromSampleId(sl.sampleId);
            var t = GetTripFromId(intTripId.Value, null);

            var s = t.Sample.Where(x => x.sampleId == sl.sampleId).FirstOrDefault();

            if(s == null)
                throw new ApplicationException("Could not locate sample.");

            var slExisting = s.SpeciesList.Where(x => x.speciesListId == sl.speciesListId).FirstOrDefault();

            if(slExisting == null)
                throw new ApplicationException("Could not locate species list.");

            //Update any SpeciesList value type properties
            sl.CopyEntityValueTypesTo(slExisting);

            if (sl.ChangeTracker.State != slExisting.ChangeTracker.State)
                slExisting.ChangeTracker.State = sl.ChangeTracker.State;

            //Update any offline changes to slExisting
            UpdateEntityOfflineState(slExisting);

            foreach (LavSFTransferItem ls in lstItems)
            {
                var ssExisting = slExisting.SubSample.Where(x => x.subSampleId == ls.Animal.subSampleId).FirstOrDefault(); 

                if(ssExisting == null)
                    throw new ApplicationException("Could not locate sub sample.");

                var animal = ssExisting.Animal.Where(x => x.animalId == ls.Animal.animalId).FirstOrDefault();

                UpdateEntityOfflineState(ls.Animal);

                 //If animal is deleted, delete child objects in the right order (not counting on cascade delete to work or be set up)
                if (ls.Animal != null && ls.Animal.ChangeTracker.State == ObjectState.Deleted && ls.AnimalInfo.OfflineState != ObjectState.Added)
                {
                    //Add it to offline deleted entities on SubSample (this will delete the entity when synched)
                    ssExisting.OfflineDeletedEntities.Add(ls);
                }
                else
                {
                    if (ls.Animal.ChangeTracker.State == ObjectState.Added)
                        ls.Animal.animalId = Offline.OfflineDictionary.Instance.CreateNewId();

                    if (ls.AnimalInfo != null && ls.AnimalInfo.ChangeTracker.State == ObjectState.Deleted)
                    {
                        if (animal != null)
                            animal.AnimalInfo.Clear();

                        if (ls.AnimalInfo.OfflineState == ObjectState.Unchanged)
                            ls.Animal.OfflineDeletedEntities.Add(ls.AnimalInfo);
                    }
                    else
                    {
                        if(ls.AnimalInfo.ChangeTracker.State == ObjectState.Added)
                            ls.AnimalInfo.animalInfoId = Offline.OfflineDictionary.Instance.CreateNewId();

                        ls.AnimalInfoReferences.ForEach(x =>
                        {
                            if (x.ChangeTracker.State == ObjectState.Added)
                                x.R_animalInfoReferenceId = Offline.OfflineDictionary.Instance.CreateNewId();

                            UpdateEntityOfflineState(x);
                        });

                        ls.AnimalInfo.R_AnimalInfoReference.Clear();

                        foreach (var R_Ani in ls.AnimalInfoReferences)
                        {
                            if (R_Ani.ChangeTracker.State == ObjectState.Deleted)
                            {
                                if (ls.AnimalInfo.OfflineState == ObjectState.Unchanged)
                                    ls.Animal.OfflineDeletedEntities.Add(R_Ani);
                            }
                            else
                                ls.AnimalInfo.R_AnimalInfoReference.Add(R_Ani);
                        }
                        //ls.AnimalInfo.R_AnimalInfoReference.AddRange(ls.AnimalInfoReferences.Where(x => x.ChangeTracker.State != ObjectState.Deleted));

                        ls.Animal.AnimalInfo.Clear();
                        ls.Animal.AnimalInfo.Add(ls.AnimalInfo);
                    }

                    UpdateEntityOfflineState(ls.AnimalInfo);

                    ls.Animal.Age.Clear();
                    ls.Ages.ForEach(a =>
                    {
                        if (a.ChangeTracker.State == ObjectState.Added)
                            a.ageId = Offline.OfflineDictionary.Instance.CreateNewId();

                        if (a.ageMeasureMethodId == 0)
                            a.ageMeasureMethodId = 1;

                        if (a.ChangeTracker.State != ObjectState.Deleted)
                            ls.Animal.Age.Add(a);
                        else
                        {
                            if (a.OfflineState == ObjectState.Unchanged)
                                ls.Animal.OfflineDeletedEntities.Add(a);
                        }

                        UpdateEntityOfflineState(a);
                    });

                    ls.Animal.AnimalFiles.Clear();
                    ls.AnimalFiles.ForEach(a =>
                    {
                        if (a.ChangeTracker.State == ObjectState.Added)
                            a.animalFileId = Offline.OfflineDictionary.Instance.CreateNewId();

                        if (a.ChangeTracker.State != ObjectState.Deleted)
                            ls.Animal.AnimalFiles.Add(a);
                        else
                        {
                            if (a.OfflineState == ObjectState.Unchanged)
                                ls.Animal.OfflineDeletedEntities.Add(a);
                        }

                        UpdateEntityOfflineState(a);
                    });
                }

                if (animal != null)
                    ssExisting.Animal.Remove(animal);

                if(ls.Animal != null && ls.Animal.ChangeTracker.State != ObjectState.Deleted)
                    ssExisting.Animal.Add(ls.Animal);
            }

            SaveOfflineTrip(t);

            var triptest = GetTripFromId(t.tripId, null);
            return DatabaseOperationResult.CreateSuccessResult();
        }




        private void SaveOfflineTrip(Trip t)
        {
            //Retrieve path to trips folder in cruise folder with id t.cruiseId.
            string strTripsPath = OfflineDictionary.Instance.GetTripsPathFromCruiseId(t.cruiseId);

            //Create trips folder, if not already there.
            if (!Directory.Exists(strTripsPath))
                Directory.CreateDirectory(strTripsPath);

            string strTripPath = Path.Combine(strTripsPath, t.tripId.ToString() + ".bin");

            //Accept all changes of trip
            foreach (var s in t.Sample)
            {
                foreach (var sl in s.SpeciesList)
                {
                    foreach (var ss in sl.SubSample)
                    {
                        foreach (var a in ss.Animal)
                        {
                            foreach (var ai in a.AnimalInfo)
                            {
                                ai.AcceptChanges();
                            }

                            foreach (var age in a.Age)
                                age.AcceptChanges();
                           
                            a.AcceptChanges();
                        }

                        ss.AcceptChanges();
                    }

                    sl.AcceptChanges();
                }

                s.AcceptChanges();
            }

            t.AcceptChanges();

            Cruise c = GetCruiseFromId(t.cruiseId);

            if (c == null)
                throw new ApplicationException(String.Format("Cruise with id '{0}' was not found on disk.", t.cruiseId));

            //Save entity to disk
            var arr = t.ToByteArrayDataContract();
            arr = arr.Compress();
            File.WriteAllBytes(strTripPath, arr);

            //Add its id to the Offline dictionary.
            Offline.OfflineDictionary.Instance.AddTrip(t);           

            //Save the trip as either added or modified.
            if(t.OfflineState == ObjectState.Unchanged)
                Offline.OfflineDictionary.Instance.SetTripChanged(t, c, ObjectState.Modified);
            else //This will be added, deleted or modified
                Offline.OfflineDictionary.Instance.SetTripChanged(t, c);

            //Save offline dictionary.
            Offline.OfflineDictionary.Instance.Save();

            //Optimization: Assign last loaded trip, so it will load faster next time (if the same trip is to be loaded).
            Offline.OfflineDictionary.AssignLastLoadedTrip(t);
        }


        private void UpdateEntityOfflineState(OfflineEntity oe)
        {
            var ioe = oe as IObjectWithChangeTracker;

            //If the state is already set, don't do anything
            if (oe.OfflineState != ObjectState.Unchanged)
                return;

            //If no changes are recorded, do nothing.
            if (ioe.ChangeTracker.State == ObjectState.Unchanged)
                return;

            oe.OfflineState = ioe.ChangeTracker.State;
        }



        public Entities.Sprattus.L_StatisticalRectangle[] GetStatisticalRectangleFromArea(string strAreaCode)
        {
            var lm = new LookupManager();
            var lstRectangles = lm.GetLookups(typeof(L_StatisticalRectangle)).OfType<L_StatisticalRectangle>();
            var lstICES_DFU_Rel = lm.GetLookups(typeof(ICES_DFU_Relation_FF)).OfType<ICES_DFU_Relation_FF>();

            strAreaCode = strAreaCode.ToLower();
            var t = lstICES_DFU_Rel.Where(x => x.area_20_21.ToLower().Equals(strAreaCode)).Select(x => x.statisticalRectangle.ToUpper()).Distinct().ToList();


            var rects = from s in t
                        join stat in lstRectangles on s equals stat.statisticalRectangle.ToUpper()
                        select stat;

            return rects.ToArray();

        }

        public Entities.Sprattus.L_SelectionDevice[] GetSelectionDevicesFromGearType(string strGearType)
        {
            var lm = new LookupManager();
            var lstGearTypes = lm.GetLookups(typeof(R_GearTypeSelectionDevice)).OfType<R_GearTypeSelectionDevice>();
            var lstSelDevs = lm.GetLookups(typeof(L_SelectionDevice)).OfType<L_SelectionDevice>();

            var rects = from gt in lstGearTypes.Where(x => x.gearType.Equals(strGearType))
                        join seldev in lstSelDevs on gt.selectionDevice equals seldev.selectionDevice
                        select seldev;

            return rects.Distinct().ToArray();
        }

        public Entities.Sprattus.MapPoint[] GetMapPositionsFromCruiseId(int intCruiseId)
        {
            var dr = new BusinessLogic.DataRetrieval.OfflineDataRetrievalClient();
            Trip[] trips = dr.GetTreeViewTrips(intCruiseId);

           var arr =  trips.Where(x => !x.tripType.Equals("HVN", StringComparison.InvariantCultureIgnoreCase)).SelectMany(x => x.Sample).Select(s => new MapPoint() { Id = s.sampleId, StationName = s.station, TripName = (s.Trip == null ? "" : s.Trip.trip1), LatitudeStart = s.latPosStartText, LongitudeStart = s.lonPosStartText, LatitudeStop = s.latPosEndText, LongitudeStop = s.lonPosEndText }).ToArray();

           return arr;
        }

        public Entities.Sprattus.MapPoint[] GetMapPositionsFromTripId(int intTripId)
        {
            var t =  BusinessLogic.DataRetrieval.OfflineDataRetrievalClient.GetTripEntityFromTripId(intTripId);

            var arr = t.Sample.Select(s => new MapPoint() { Id = s.sampleId, StationName = s.station, TripName = (s.Trip == null ? "" : s.Trip.trip1), LatitudeStart = s.latPosStartText, LongitudeStart = s.lonPosStartText, LatitudeStop = s.latPosEndText, LongitudeStop = s.lonPosEndText }).ToArray();

            return arr;
        }

        public Entities.Sprattus.MapPoint[] GetMapPositionsFromSampleId(int intSampleId)
        {
            var t = BusinessLogic.DataRetrieval.OfflineDataRetrievalClient.GetTripEntityFromSampleId(intSampleId);

            var s = t.Sample.Where(x => x.sampleId == intSampleId).FirstOrDefault();

            lock (s)
            {
                var mp = new MapPoint() { Id = s.sampleId, StationName = s.station, TripName = (s.Trip == null ? "" : s.Trip.trip1), LatitudeStart = s.latPosStartText, LongitudeStart = s.lonPosStartText, LatitudeStop = s.latPosEndText, LongitudeStop = s.lonPosEndText };
                return new MapPoint[] { mp };
            }
        }

       


        #endregion


        public DatabaseOperationResult SaveCruiseToDataWarehouse(byte[] arrCruise, byte[] arrMessages, bool blnDeleteCruiseBeforeInsert, ref Warehouse.DWMessage[] lstMessagesNew)
        {
            throw new NotImplementedException();
        }



        private void DeleteFolder(string strFolderPath, bool blnIfEmptyOnly = false)
        {
            if (!blnIfEmptyOnly || (Directory.GetDirectories(strFolderPath).Length == 0 && Directory.GetFiles(strFolderPath).Length == 0))
                Directory.Delete(strFolderPath, true);
        }


        public DatabaseOperationResult DeleteCruise(int intCruiseId)
        {
            DatabaseOperationResult res = DatabaseOperationResult.CreateSuccessResult();

            var cruise = GetCruiseFromId(intCruiseId);
            var datRet = new DataRetrieval.OfflineDataRetrievalClient();
            var trips = datRet.GetTreeViewTrips(intCruiseId);

            if (cruise != null)
            {
                string strOfflineData = Settings.Settings.Instance.OfflineDataPath;
                
                string strCruisePath = Path.Combine(strOfflineData, cruise.year.ToString(), intCruiseId.ToString(), "cruise.bin");
                string strCruiseFolder = Path.Combine(strOfflineData, cruise.year.ToString(), intCruiseId.ToString());
                string strYearFolder = Path.Combine(strOfflineData, cruise.year.ToString());
                try
                {
                    DeleteFolder(strCruiseFolder);
                    DeleteFolder(strYearFolder, true);
                }
                catch (Exception e)
                {
                    Anchor.Core.Loggers.Logger.LogError(e);
                    res = new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, e.Message);
                    return res;
                }

                if (trips != null)
                {
                    foreach (var t in trips)
                        Offline.OfflineDictionary.Instance.RemoveChangedTrip(t.tripId);
                }

                Offline.OfflineDictionary.Instance.RemoveChangedCruise(cruise.cruiseId);
                Offline.OfflineDictionary.Instance.RemoveCruise(cruise.cruiseId);
                Offline.OfflineDictionary.Instance.Save();
            }

            return res;
        }


        public DatabaseOperationResult DeleteTrip(int intTripId)
        {
            DatabaseOperationResult res = DatabaseOperationResult.CreateSuccessResult();

            var t = GetTripFromId(intTripId, null);

            if (t == null)
                return res;

            //Retrieve path to trips folder in cruise folder with id t.cruiseId.
            string strTripsPath = OfflineDictionary.Instance.GetTripsPathFromCruiseId(t.cruiseId);

            //Create trips folder, if not already there.
            if (!Directory.Exists(strTripsPath))
                return res;

            string strTripPath = Path.Combine(strTripsPath, t.tripId.ToString() + ".bin");

            if (File.Exists(strTripPath))
            {
                try
                {
                    File.Delete(strTripPath);
                }
                catch (Exception e)
                {
                    res = new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, e.Message);
                    return res;
                }
            }

            Offline.OfflineDictionary.Instance.RemoveChangedTrip(intTripId);
            Offline.OfflineDictionary.Instance.RemoveTrip(intTripId);
            Offline.OfflineDictionary.Instance.Save();

            Offline.OfflineDictionary.AssignLastLoadedTrip(null, false);
            return res;
        }

        public DatabaseOperationResult DeleteSample(int intSampleId)
        {
            DatabaseOperationResult res = DatabaseOperationResult.CreateSuccessResult();

            var s = GetSampleFromId(intSampleId);

            if (s == null)
                return res;


            var t = GetTripFromId(s.tripId, null);
            
            var sdel = t.Sample.Where(x => x.sampleId == intSampleId).FirstOrDefault();

            if (sdel != null)
                t.Sample.Remove(sdel);

            Offline.OfflineDictionary.Instance.RemoveSample(intSampleId);
            Offline.OfflineDictionary.Instance.Save();

            return SaveTrip(ref t);
        }


        public Sample GetLatestSample(int intTripId)
        {
            var lstExistingSampleIds = new List<int>();
            var lstOfflineSampleIds = new List<int>();

            foreach (var kv in Offline.OfflineDictionary.Instance.SampleToTrip)
            {
                if (kv.Value == intTripId)
                {
                    if (kv.Key < 0)
                        lstOfflineSampleIds.Add(kv.Key);
                    else
                        lstExistingSampleIds.Add(kv.Key);
                }
            }

            //If offline ids exist, order them ascending since they are negative. If only existing sample ids exist, order by descending since they are positive.
            var lstSampleIds = lstOfflineSampleIds.Count > 0 ? lstOfflineSampleIds.OrderBy(x => x).ToList() : lstExistingSampleIds.OrderByDescending(x => x).ToList();
            
            int? intSampleId = null;

            if (lstSampleIds.Count > 0)
                intSampleId = lstSampleIds.First();

            if (intSampleId == null)
                return null;

            return GetSampleFromId(intSampleId.Value);
        }


        /// <summary>
        /// This does not work offline, so just return success.
        /// </summary>
        public DatabaseOperationResult RunFileSynchronizer()
        {
            return DatabaseOperationResult.CreateSuccessResult();
        }
    }
}
