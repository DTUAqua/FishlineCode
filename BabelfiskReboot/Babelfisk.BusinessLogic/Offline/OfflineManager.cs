using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.BusinessLogic.BabelfiskService;
using Babelfisk.Entities;
using Anchor.Core;
using Babelfisk.Entities.Sprattus;
using System.Linq.Expressions;

namespace Babelfisk.BusinessLogic.Offline
{
    public class OfflineManager
    {

        public void SaveLookups(Type t, LookupDataVersioning ldv = null, params string[] includes) //where T : class
        {
            List<ILookupEntity> lst = new List<ILookupEntity>();

            if (ldv == null)
                ldv = new LookupDataVersioning();

            if (!Settings.Settings.Instance.OfflineStatus.IsOffline && ldv.IsLocalLookupsExpired(t.Name))
            {
                var ser = new Babelfisk.BusinessLogic.Lookup.ServiceLookupClient();
                try
                {
                    ser.SupplyCredentials();
                    var arr = ser.GetLookups(t.AssemblyQualifiedName, includes);
                    ser.Close();

                    if (arr != null)
                    {
                        arr = arr.Decompress();
                        lst = arr.ToObjectDataContract<List<ILookupEntity>>(new Type[] { t });
                    }
                }
                catch (Exception e)
                {
                    ser.Abort();
                    throw e;
                }

                if (lst != null)
                    ldv.SetLocalLookups(t.Name, lst);
            }
        }


        public List<OfflineSelectionRecord> GetOfflineSelectionData(List<int> lstYears)
        {
            var sv = new BabelfiskService.OfflineClient();
            List<OfflineSelectionRecord> lst = new List<OfflineSelectionRecord>();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as IOffline).GetOfflineSelectionData(lstYears.ToArray());

                sv.Close();

                if (arr != null)
                {
                    arr = arr.Decompress();
                    lst = arr.ToObjectDataContract<List<OfflineSelectionRecord>>();
                }

                return lst;
            }
            catch (Exception e)
            {
                sv.Abort();
                throw e;
            }
        }


        public List<OfflineSelectionRecord> GetOfflineSelectionDataWithSample(List<int> lstYears)
        {
            var sv = new BabelfiskService.OfflineClient();
            List<OfflineSelectionRecord> lst = new List<OfflineSelectionRecord>();

            try
            {
                sv.SupplyCredentials();

                //If application is offline, retrieve selection data from offline dictionary
                if (BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
                {
                    lst = OfflineDictionary.Instance.GetOfflineSelectionDataWithSamples(lstYears);
                    sv.Close();
                }
                //Else retrieve selection data from server
                else
                {
                    var arr = (sv as IOffline).GetOfflineSelectionDataWithSamples(lstYears.ToArray());

                    sv.Close();

                    if (arr != null)
                    {
                        arr = arr.Decompress();
                        lst = arr.ToObjectDataContract<List<OfflineSelectionRecord>>();
                    }
                }

                return lst;
            }
            catch (Exception e)
            {
                sv.Abort();
                throw e;
            }
        }


        public List<OfflineTripTransfer> GetOfflineCruiseData(List<int> lstCruises)
        {
            var sv = new BabelfiskService.OfflineClient();
            List<OfflineTripTransfer> lst = new List<OfflineTripTransfer>();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as IOffline).GetOfflineCruiseData(lstCruises.ToArray());

                sv.Close();

                if (arr != null)
                {
                    arr = arr.Decompress();
                    lst = arr.ToObjectDataContract<List<OfflineTripTransfer>>();
                }

                return lst;
            }
            catch (Exception e)
            {
                sv.Abort();
                throw e;
            }
        }


        public List<OfflineTripTransfer> GetOfflineTripData(List<int> lstTrips)
        {
            var sv = new BabelfiskService.OfflineClient();
            List<OfflineTripTransfer> lst = new List<OfflineTripTransfer>();

            try
            {
                sv.SupplyCredentials();

                var arr = (sv as IOffline).GetOfflineTripData(lstTrips.ToArray());

                sv.Close();

                if (arr != null)
                {
                    arr = arr.Decompress();
                    lst = arr.ToObjectDataContract<List<OfflineTripTransfer>>();
                }

                return lst;
            }
            catch (Exception e)
            {
                sv.Abort();
                throw e;
            }
        }


        public bool IsLandDatabaseAvailable()
        {
            var sv = new BabelfiskService.OfflineClient();

            try
            {
                sv.SupplyCredentials();

                //If below call succeeds without exceptions, a connection to the land database is available.
                bool bln = sv.IsOnline();

                sv.Close();
            }
            catch (Exception e)
            {
                sv.Abort();
                Anchor.Core.Loggers.Logger.LogError(e, "IsLandDataBaseAvailable(). Failed to establish connection to land database.");
                return false;
            }

            return true;
        }



        public SyncDatabaseOperationResult SynchronizeLookups()
        {
            var sv = new BabelfiskService.OfflineClient();

            SyncDatabaseOperationResult res = SyncDatabaseOperationResult.CreateSuccessResult();

            var lm = new LookupManager();
            try
            {
                sv.SupplyCredentials();

                LookupDataVersioning ldv = new LookupDataVersioning();
                bool blnChanges = false;

                //Synchronize DFUPerson
                res = SyncOfflineLookups<DFUPerson>(sv, lm, ldv, () => new DFUPerson().dfuPersonId, ref blnChanges);
                if (res.DatabaseOperationResult.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                    return res;

                if (blnChanges)
                    lm.UpdateLookup(typeof(DFUPerson), ldv);

                //Synchronize Person
                res = SyncOfflineLookups<Person>(sv, lm, ldv, () => new Person().personId, ref blnChanges);
                if (res.DatabaseOperationResult.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                    return res;

                if (blnChanges)
                    lm.UpdateLookup(typeof(Person), ldv);

                //Synchronize Species
                res = SyncOfflineLookups<L_Species>(sv, lm, ldv, () => new L_Species().L_speciesId, ref blnChanges);
                if (res.DatabaseOperationResult.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                    return res;

                if (blnChanges)
                    lm.UpdateLookup(typeof(L_Species), ldv);

                //Synchronize L_Platform
                res = SyncOfflineLookups<L_Platform>(sv, lm, ldv, () => new L_Platform().L_platformId, ref blnChanges);
                if (res.DatabaseOperationResult.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                    return res;

                if (blnChanges)
                    lm.UpdateLookup(typeof(L_Platform), ldv);

                //Synchronize GearType
                res = SyncOfflineLookups<L_GearType>(sv, lm, ldv, () => new L_GearType().L_gearTypeId, ref blnChanges);
                if (res.DatabaseOperationResult.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                    return res;

                if (blnChanges)
                    lm.UpdateLookup(typeof(L_GearType), ldv);
            }
            catch (Exception e)
            {
                sv.Abort();
                throw e;
            }
            finally
            {
                if(sv != null && sv.State == System.ServiceModel.CommunicationState.Opened)
                    sv.Close();
            }

            return res;
        }


        private SyncDatabaseOperationResult SyncOfflineLookups<T>(BabelfiskService.OfflineClient sv, LookupManager lm, LookupDataVersioning ldv, Expression<Func<int>> lookupIdProperty, ref bool blnSavedChanges) where T : OfflineEntity, IObjectWithChangeTracker
        {
            blnSavedChanges = false;
            SyncDatabaseOperationResult res = SyncDatabaseOperationResult.CreateSuccessResult();

            string strLookupIdPropertyName = lookupIdProperty.ExtractPropertyName();

            var lst = ldv.GetLocalLookups(typeof(T).Name);
            var lstLocalLookups = lst.OfType<T>().Where(x => x.OfflineState != ObjectState.Unchanged || x.OfflineState != 0).ToList();

            if (lstLocalLookups != null && lstLocalLookups.Count > 0)
            {
                List<T> lstLookups = new List<T>();
                for (int i = 0; i < lstLocalLookups.Count; i++)
                {
                    var p = lstLocalLookups[i];

                    if (p.OfflineState == ObjectState.Unchanged || p.OfflineState == 0)
                        continue;

                    Entities.ExtensionMethods.HandleOfflineId(ref p, lookupIdProperty);
                    lstLookups.Add(p);
                }

                if (lstLookups.Count > 0)
                {
                    var arrLookups = lstLookups.ToArray<object>();
                    res = sv.SynchronizeLookups(ref arrLookups);

                    if (res.DatabaseOperationResult.DatabaseOperationStatus == DatabaseOperationStatus.Successful)
                    {
                        blnSavedChanges = true;
                        for (int i = 0; i < arrLookups.Length; i++)
                        {
                            var l = arrLookups[i] as OfflineEntity;
                            //Populate mapping of temporary offline id to new id
                            if (l.OfflineId.HasValue)
                            {
                                //p.OfflineId -> p.personId
                                OfflineDictionary.Instance.AddOfflineToOnlineId(l.OfflineId.Value, (int)l.GetNavigationProperty(strLookupIdPropertyName));

                                //Remove the lookup as being added/changed, so Babelfisk knows not to synchronize this lookup again.
                                OfflineDictionary.Instance.RemoveChangedLookup(l.OfflineId.Value);
                            }  
                        }

                    }
                }
            }

            return res;
        }


        /// <summary>
        /// Synchronize a single cruise with the land database.
        /// </summary>
        public SyncDatabaseOperationResult SynchronizeCruise(ref Cruise c)
        {
            var sv = new BabelfiskService.OfflineClient();

            SyncDatabaseOperationResult res = SyncDatabaseOperationResult.CreateSuccessResult();
            try
            {
                if (c.dataHandlerId.HasValue && c.dataHandlerId.Value < 0)
                    c.AssignNavigationPropertyWithoutChanges("dataHandlerId", OfflineDictionary.Instance.GetOnlineId(c.dataHandlerId.Value));

                if (c.responsibleId.HasValue && c.responsibleId.Value < 0)
                    c.AssignNavigationPropertyWithoutChanges("responsibleId", OfflineDictionary.Instance.GetOnlineId(c.responsibleId.Value));

                sv.SupplyCredentials();
                res = sv.SynchronizeCruise(ref c);

                sv.Close();
                //Populate mapping of temporary offline id to new id
                if (res.DatabaseOperationResult.DatabaseOperationStatus == DatabaseOperationStatus.Successful)
                {
                    //c.OfflineId -> c.cruiseId
                    if (c.OfflineId.HasValue)
                        OfflineDictionary.Instance.AddOfflineToOnlineId(c.OfflineId.Value, c.cruiseId);

                    //TODO - Save cruise to local disk as now being unchanged (so if user needs to re-synchronize, this cruise will not show up
                    //ResetOfflineValues(ref t);

                    //Remove cruise from changes dictionary.
                    Offline.OfflineDictionary.Instance.RemoveChangedCruise(c.OfflineId.HasValue ? c.OfflineId.Value : c.cruiseId);

                }
            }
            catch (Exception e)
            {
                sv.Abort();
                throw e;
            }

            return res;
        }



        public SyncDatabaseOperationResult SynchronizeTrip(ref Trip t)
        {
            var sv = new BabelfiskService.OfflineClient();

            SyncDatabaseOperationResult res = SyncDatabaseOperationResult.CreateSuccessResult();

            try
            {
                HandleTripLookupOfflineIds(ref t);

                //Update cruise id, in case the cruise is new and has a new id.
                int? intCruiseId;
                if ((intCruiseId = OfflineDictionary.Instance.GetOnlineId(t.cruiseId)).HasValue && intCruiseId != t.cruiseId)
                    t.AssignNavigationPropertyWithoutChanges("cruiseId", intCruiseId.Value);

                sv.SupplyCredentials();

                byte[] arr = t.ToByteArrayDataContract();
                arr = arr.Compress();

                arr = sv.SynchronizeTrip(arr, ref res);

                arr = arr.Decompress();
                t = arr.ToObjectDataContract<Trip>();

                sv.Close();
                //Reset trip 
                if (res.DatabaseOperationResult.DatabaseOperationStatus == DatabaseOperationStatus.Successful)
                {
                    //TODO - Save trip to local disk as now being unchanged (so if user needs to re-synchronize, this trip will not show up
                    //ResetOfflineValues(ref t);
                    
                    //Remove trip from changes dictionary.
                    Offline.OfflineDictionary.Instance.RemoveChangedTrip(t.OfflineId.HasValue ? t.OfflineId.Value : t.tripId);
                }
            }
            catch (Exception e)
            {
                sv.Abort();
                throw e;
            }

            return res;

        }

        private void ResetOfflineValues(ref Cruise c)
        {
            ResetOfflineValues(c);
        }

        private void ResetOfflineValues(ref Trip t)
        {
            ResetOfflineValues(t);

            foreach (var s in t.Sample)
            {
                ResetOfflineValues(s);

                foreach (var sl in s.SpeciesList)
                {
                    ResetOfflineValues(sl);

                    foreach (var ss in sl.SubSample)
                    {
                        ResetOfflineValues(ss);

                        foreach (var a in ss.Animal)
                        {
                            ResetOfflineValues(a);

                            foreach (var age in a.Age)
                                ResetOfflineValues(age);

                            foreach (var ai in a.AnimalInfo)
                            {
                                ResetOfflineValues(ai);

                                foreach (var r_a in ai.R_AnimalInfoReference)
                                    ResetOfflineValues(r_a);
                            }
                        }
                    }
                }
            }
        }

        private void ResetOfflineValues(OfflineEntity oe)
        {
            oe.OfflineId = null;
            oe.OfflineDeletedEntities = new List<object>();
            oe.OfflineComparisonEntity = null;
            oe.OfflineState = ObjectState.Unchanged;
            oe.OverwritingMethod = null;
        }


        private void HandleTripLookupOfflineIds(ref Trip t)
        {
            //Handle lookup ids (if there are any new lookups added)
            if (t.contactPersonId.HasValue && t.contactPersonId.Value < 0)
            {
                t.AssignNavigationPropertyWithoutChanges("contactPersonId", OfflineDictionary.Instance.GetOnlineId(t.contactPersonId.Value));
                if (t.Person != null)
                    t.Person.AssignNavigationPropertyWithoutChanges("personId", t.contactPersonId.Value);
            }

            if (t.tripLeaderId.HasValue && t.tripLeaderId.Value < 0)
                t.AssignNavigationPropertyWithoutChanges("tripLeaderId", OfflineDictionary.Instance.GetOnlineId(t.tripLeaderId.Value));

            if (t.dataHandlerId.HasValue && t.dataHandlerId.Value < 0)
                t.AssignNavigationPropertyWithoutChanges("dataHandlerId", OfflineDictionary.Instance.GetOnlineId(t.dataHandlerId.Value));

            foreach (var s in t.Sample)
            {
                if (s.samplePersonId.HasValue && s.samplePersonId.Value < 0)
                    s.AssignNavigationPropertyWithoutChanges("samplePersonId", OfflineDictionary.Instance.GetOnlineId(s.samplePersonId.Value));

                if (s.analysisPersonId.HasValue && s.analysisPersonId.Value < 0)
                    s.AssignNavigationPropertyWithoutChanges("analysisPersonId", OfflineDictionary.Instance.GetOnlineId(s.analysisPersonId.Value));

                foreach (var ss in s.SpeciesList)
                {
                    if (ss.maturityReaderId.HasValue && ss.maturityReaderId.Value < 0)
                        ss.AssignNavigationPropertyWithoutChanges("maturityReaderId", OfflineDictionary.Instance.GetOnlineId(ss.maturityReaderId.Value));

                    if (ss.hatchMonthReaderId.HasValue && ss.hatchMonthReaderId.Value < 0)
                        ss.AssignNavigationPropertyWithoutChanges("hatchMonthReaderId", OfflineDictionary.Instance.GetOnlineId(ss.hatchMonthReaderId.Value));

                    if (ss.ageReadId.HasValue && ss.ageReadId.Value < 0)
                        ss.AssignNavigationPropertyWithoutChanges("ageReadId", OfflineDictionary.Instance.GetOnlineId(ss.ageReadId.Value));

                    if (ss.datahandlerId.HasValue && ss.datahandlerId.Value < 0)
                        ss.AssignNavigationPropertyWithoutChanges("datahandlerId", OfflineDictionary.Instance.GetOnlineId(ss.datahandlerId.Value));
                }
            }
        }
      

    }
}
