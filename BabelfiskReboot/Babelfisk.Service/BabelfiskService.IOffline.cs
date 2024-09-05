using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using Babelfisk.Entities;
using System.Linq.Expressions;

namespace Babelfisk.Service
{
    public partial class BabelfiskService : IOffline
    {
        public byte[] GetOfflineSelectionData(List<int> lstYears)
        {
            byte[] arr = null;

            try
            {
                if (lstYears != null && lstYears.Count > 0)
                {
                    using (var ctx = new SprattusContainer())
                    {
                        ctx.Connection.Open();
                        ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("SELECT c.cruiseId as 'CruiseId', t.tripId as 'TripId', c.year as 'Year', c.cruise as 'CruiseName', t.trip as 'TripName'");
                        sb.AppendLine("FROM Cruise c");
                        sb.AppendLine("LEFT OUTER JOIN Trip t ON c.cruiseId = t.cruiseId");
                        sb.AppendLine(String.Format("WHERE c.year IN ({0})", String.Join(", ", lstYears.ToArray())));

                        var lst = ctx.ExecuteStoreQuery<OfflineSelectionRecord>(sb.ToString()).OfType<OfflineSelectionRecord>().ToList();

                        arr = lst.ToByteArrayDataContract();
                        arr = arr.Compress();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return arr;
        }


        public byte[] GetOfflineSelectionDataWithSamples(List<int> lstYears)
        {
            byte[] arr = null;

            try
            {
                if (lstYears != null && lstYears.Count > 0)
                {
                    using (var ctx = new SprattusContainer())
                    {
                        ctx.Connection.Open();
                        ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                        string strQuery = @"SELECT c.cruiseId as 'CruiseId', t.tripId as 'TripId', c.year as 'Year', c.cruise as 'CruiseName', t.trip as 'TripName', s.sampleId as 'SampleId', s.station as 'SampleName'
                                            FROM Cruise c
                                            LEFT OUTER JOIN Trip t ON c.cruiseId = t.cruiseId
                                            LEFT OUTER JOIN Sample s ON s.tripId = t.tripId
                                            WHERE c.year IN ({0})
                                          ";

                        strQuery = String.Format(strQuery, String.Join(", ", lstYears.ToArray()));

                        var lst = ctx.ExecuteStoreQuery<OfflineSelectionRecord>(strQuery).OfType<OfflineSelectionRecord>().ToList();

                        arr = lst.ToByteArrayDataContract();
                        arr = arr.Compress();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return arr;
        }


        public byte[] GetOfflineCruiseData(List<int> lstCruiseIds)
        {
            byte[] arr = null;

            try
            {
                if (lstCruiseIds != null && lstCruiseIds.Count > 0)
                {
                    using (var ctx = new SprattusContainer())
                    {
                        ctx.Connection.Open();
                        ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                        List<OfflineTripTransfer> lst = new List<OfflineTripTransfer>();

                        foreach (int intCruiseId in lstCruiseIds)
                        {
                            OfflineTripTransfer tt = new OfflineTripTransfer();

                            //Get cruise
                            tt.Cruise = ctx.Cruise.Where(x => x.cruiseId == intCruiseId).FirstOrDefault();
                            ctx.Detach(tt.Cruise); //Detach it, so Trip navigation property wont get loaded (by the query below).

                            lst.Add(tt);
                        }

                        arr = lst.ToByteArrayDataContract();
                        arr = arr.Compress();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return arr;
        }


        public byte[] GetOfflineTripData(List<int> lstTripIds)
        {
            byte[] arr = null;

            //Make sure garbage collection is run before this method, so all potential hanging large objects are collected.
            GC.Collect();

            try
            {
                if (lstTripIds != null && lstTripIds.Count > 0)
                {
                    using (var ctx = new SprattusContainer())
                    {
                        ctx.Connection.Open();
                        ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                        List<OfflineTripTransfer> lst = new List<OfflineTripTransfer>();

                        foreach (int intTripId in lstTripIds)
                        {
                            OfflineTripTransfer tt = new OfflineTripTransfer();

                            int intCruiseId = ctx.Trip.Where(x => x.tripId == intTripId).Select(x => x.cruiseId).FirstOrDefault();

                            //Get cruise
                            tt.Cruise = ctx.Cruise.Where(x => x.cruiseId == intCruiseId).FirstOrDefault();
                            ctx.Detach(tt.Cruise); //Detach it, so Trip navigation property wont get loaded (by the query below).

                            tt.Trip = ctx.Trip
                                         .Include("Sample")
                                         .Include("Sample.R_TargetSpecies")
                                         .Include("Sample.SpeciesList")
                                         .Include("Sample.SpeciesList.SubSample")
                                         .Include("Sample.SpeciesList.SubSample.Animal")
                                         .Include("Sample.SpeciesList.SubSample.Animal.Age")
                                         .Include("Sample.SpeciesList.SubSample.Animal.AnimalFiles")
                                         .Include("Sample.SpeciesList.SubSample.Animal.AnimalInfo")
                                         .Include("Sample.SpeciesList.SubSample.Animal.AnimalInfo.R_AnimalInfoReference")
                                         .Include("Sample.SpeciesList.SubSample.Animal.AnimalInfo.Maturity")
                                          .Include("Sample.SpeciesList.SubSample.Animal.AnimalInfo.Fat")
                                         .Where(x => x.tripId == intTripId).FirstOrDefault();

                            lst.Add(tt);
                        }

                        arr = lst.ToByteArrayDataContract();
                        arr = arr.Compress();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return arr;
        }


        /// <summary>
        /// Method used for testing connection to land database.
        /// </summary>
        public bool IsOnline()
        {
            return true;
        }


        #region Lookup duplicate key validation

        private L_Species ValidateSpeciesDuplicateKey(L_Species species, SprattusContainer ctx)
        {
            bool blnIsNew = species.ChangeTracker.State == ObjectState.Added;

            var q = (from s in ctx.L_Species
                     where s.speciesCode == species.speciesCode &&
                          (s.L_speciesId != species.L_speciesId || blnIsNew)
                     select s);

            return q.FirstOrDefault();
        }


        private DFUPerson ValidateDFUPersonDuplicateKey(DFUPerson species, SprattusContainer ctx)
        {
            bool blnIsNew = species.ChangeTracker.State == ObjectState.Added;

            var q = (from s in ctx.DFUPerson
                     where s.initials == species.initials &&
                          (s.dfuPersonId != species.dfuPersonId || blnIsNew)
                     select s);

            return q.FirstOrDefault();
        }


        private L_GearType ValidateGearTypeDuplicateKey(L_GearType gearType, SprattusContainer ctx)
        {
            bool blnIsNew = gearType.ChangeTracker.State == ObjectState.Added;

            var q = (from g in ctx.L_GearType
                     where g.gearType == gearType.gearType &&
                          (g.L_gearTypeId != gearType.L_gearTypeId || blnIsNew)
                     select g);

            return q.FirstOrDefault();
        }

        private L_Platform ValidatePlatformDuplicateKey(L_Platform platform, SprattusContainer ctx)
        {
            bool blnIsNew = platform.ChangeTracker.State == ObjectState.Added;

            var q = (from p in ctx.L_Platform
                     where p.platform == platform.platform &&
                          (p.L_platformId != platform.L_platformId || blnIsNew)
                     select p);

            return q.FirstOrDefault();
        }

        #endregion



        private void SetupAndValidateLookups(ref List<ILookupEntity> lookups)
        {
            //Info: Offline Id's are handled in the OfflineManager.

            //Handle duplicate lookups
            using (SprattusContainer ctx = new SprattusContainer())
            {
                foreach (var l in lookups)
                {
                    //Don't apply changes to new lookups that already exist in database on land

                    if (l is L_Species)
                    {
                        L_Species existingSpecies = null;
                        if ((existingSpecies = ValidateSpeciesDuplicateKey(l as L_Species, ctx)) != null)
                        {
                            l.AssignNavigationPropertyWithoutChanges("L_speciesId", existingSpecies.L_speciesId);
                            l.MarkAsUnchanged();
                            //l.MarkAsModified();
                        }
                        //Make sure navigation properties are reset
                        l.AssignNavigationPropertyWithoutChanges("L_TreatmentFactorGroup", null);
                    }
                    else if (l is DFUPerson)
                    {
                        DFUPerson existing = null;
                        if ((existing = ValidateDFUPersonDuplicateKey(l as DFUPerson, ctx)) != null)
                        {
                            l.AssignNavigationPropertyWithoutChanges("dfuPersonId", existing.dfuPersonId);
                            l.MarkAsUnchanged();
                            //l.MarkAsModified();
                        }
                        //Make sure navigation properties are reset
                        l.AssignNavigationPropertyWithoutChanges("L_DFUDepartment", null);
                    }
                    else if (l is L_GearType)
                    {
                        L_GearType existingGearType = null;
                        if ((existingGearType = ValidateGearTypeDuplicateKey(l as L_GearType, ctx)) != null)
                        {
                            l.AssignNavigationPropertyWithoutChanges("L_gearTypeId", existingGearType.L_gearTypeId);
                            l.MarkAsUnchanged();
                            //l.MarkAsModified();
                        }
                        //Make sure navigation properties are reset
                        l.AssignNavigationPropertyWithoutChanges("L_SampleType", null);
                    }
                    else if (l is L_Platform)
                    {
                        L_Platform existingPlatform = null;
                        if ((existingPlatform = ValidatePlatformDuplicateKey(l as L_Platform, ctx)) != null)
                        {
                            l.AssignNavigationPropertyWithoutChanges("L_platformId", existingPlatform.L_platformId);
                            l.MarkAsUnchanged();
                            //l.MarkAsModified();
                        }

                        //Make sure navigation properties are reset
                        l.AssignNavigationPropertyWithoutChanges("L_PlatformType", null);
                        l.AssignNavigationPropertyWithoutChanges("L_Nationality", null);
                        l.AssignNavigationPropertyWithoutChanges("L_Gear", null);
                        l.AssignNavigationPropertyWithoutChanges("L_PlatformVersion", null);
                    }
                    //Person lookup can contain duplicate persons, a check is therefore not needed here.
                }
            }

        }


        public SyncDatabaseOperationResult SynchronizeLookups(ref List<ILookupEntity> lookups)
        {
            SetupAndValidateLookups(ref lookups);

            var res = SaveLookups(ref lookups);

            return new SyncDatabaseOperationResult(res);
        }


        public SyncDatabaseOperationResult SynchronizeCruise(ref Cruise c)
        {
            SyncDatabaseOperationResult datRes = SyncDatabaseOperationResult.CreateSuccessResult();

            int intCruiseId = c.cruiseId;

            //Reset cruiseId if it has been given a temporary id when offline.
            if (intCruiseId < 0)
            {
                c.AssignNavigationPropertyWithoutChanges("cruiseId", 0);
                c.OfflineId = intCruiseId;
            }

            //Assign correct changetracker state (so normal cruise add methods can be used)
            if (c.ChangeTracker.State == ObjectState.Unchanged && c.ChangeTracker.State != c.OfflineState)
                c.ChangeTracker.State = c.OfflineState;

            try
            {
                using (SprattusContainer ctx = new SprattusContainer())
                {
                    Cruise existingCruise = null;
                    if ((existingCruise = ValidateCruiseDuplicateKey(c, ctx)) != null)
                    {
                        c.OfflineComparisonEntity = existingCruise;
                        datRes = new SyncDatabaseOperationResult(new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, "DuplicateKey"), existingCruise);

                        return datRes;
                    }
                }

                DatabaseOperationResult cres = SaveCruise(ref c);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SynchronizeCruise() with message: " + ex.Message;
                datRes = new SyncDatabaseOperationResult(new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg));
            }

            return datRes;
        }


        public byte[] SynchronizeTrip(byte[] tbytes, ref SyncDatabaseOperationResult syncRef)
        {
            SyncDatabaseOperationResult datRes = SyncDatabaseOperationResult.CreateSuccessResult();

            //Make garbage collection has run, so all hanging large obejcts are cleared before synchronizing trip
            GC.Collect();

            byte[] resArr = null;

            if (tbytes == null)
            {
                new SyncDatabaseOperationResult(new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, "received trip was null."));
                return null;
            }

            var arr = tbytes.Decompress();
            Trip t = arr.ToObjectDataContract<Trip>();
            try
            {
                

                if (!SetupAndValidateTripAndChilds(ref t))
                {
                    syncRef = new SyncDatabaseOperationResult(new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, "DuplicateKey"));
                    return CompressTrip(t);
                }

                DatabaseOperationResult cres = DatabaseOperationResult.CreateSuccessResult();

                //Save trip if any changes are found
                if (t.IsHVN)
                {
                    Sample s = t.Sample.FirstOrDefault();

                    if (t.ChangeTracker.State != ObjectState.Unchanged || s.ChangeTracker.State != ObjectState.Unchanged)
                        cres = SaveHVN(ref t, ref s);
                }
                else if (t.ChangeTracker.State != ObjectState.Unchanged)
                    cres = SaveTrip(ref t);

                //If trip is not successfully inserted, return
                if (cres.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                {
                    syncRef = new SyncDatabaseOperationResult(cres);
                    return CompressTrip(t);
                }

                //Save sample if any changes are found
                for (int i = 0; i < t.Sample.Count; i++)
                {
                    Sample s = t.Sample[i];
                    if (!t.IsHVN && s.ChangeTracker.State != ObjectState.Unchanged)
                    {
                        //Make sure trip id is correct
                        if(s.tripId != t.tripId)
                            s.AssignNavigationPropertyWithoutChanges("tripId", t.tripId);

                        cres = SaveSample(ref s);
                    }

                    //If sample is not successfully inserted, return
                    if (cres.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                    {
                        syncRef = new SyncDatabaseOperationResult(cres);
                        return CompressTrip(t);
                    }

                    var lstChangedSpeciesLists = s.SpeciesList.Where(x => x.OfflineState != ObjectState.Unchanged).ToList();

                    //Loop through and save SpeciesList
                    for (int j = 0; j < lstChangedSpeciesLists.Count; j++)
                    {
                        SpeciesList sl = lstChangedSpeciesLists[j];
                       // SetupSpeciesList(ref sl);

                        //Make sure sampleId is correct
                        if(sl.sampleId != s.sampleId)
                            sl.AssignNavigationPropertyWithoutChanges("sampleId", s.sampleId);
                    }

                    //Add deleted specieslist as well
                    lstChangedSpeciesLists.AddRange(s.OfflineDeletedEntities.OfType<SpeciesList>());

                    //Save specieslist items and return with updated ids on specieslist (specieslistId) and subsamples (subsampleId)
                    cres = SaveSpeciesListItems(ref lstChangedSpeciesLists);

                    if (cres.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                    {
                        syncRef = new SyncDatabaseOperationResult(cres);
                        return CompressTrip(t);
                    }

                    //Loop through and save SubSamples+Animals
                    foreach (var sl in s.SpeciesList)
                    {
                        for (int k = 0; k < sl.SubSample.Count; k++)
                        {
                            SubSample ss = sl.SubSample[k];

                            List<LavSFTransferItem> lst = new List<LavSFTransferItem>();
                            for (int m = 0; m < ss.Animal.Count; m++)
                            {
                                Animal a = ss.Animal[m];

                                //Make sure subsample is not referenced.
                                //a.SubSample = null;

                                var item = new LavSFTransferItem();

                                item.SubSampleId = ss.subSampleId;
                                item.Ages = a.Age.Where(x => x.ChangeTracker.State != ObjectState.Unchanged).ToList();
                                item.Ages.AddRange(a.OfflineDeletedEntities.OfType<Age>());

                                if (a.AnimalInfo.Count > 0)
                                    item.AnimalInfoReferences = a.AnimalInfo.First().R_AnimalInfoReference.Where(x => x.ChangeTracker.State != ObjectState.Unchanged).ToList();

                                item.AnimalInfoReferences.AddRange(a.OfflineDeletedEntities.OfType<R_AnimalInfoReference>());

                                if (a.ChangeTracker.State != ObjectState.Unchanged || item.Ages.Count > 0 ||  a.AnimalInfo.Where(x => x.ChangeTracker.State != ObjectState.Unchanged).Any() || item.AnimalInfoReferences.Count > 0)
                                {
                                    item.Animal = a;
                                    item.AnimalInfo = a.AnimalInfo.FirstOrDefault();
                                    lst.Add(item);
                                }
                            }

                            //Add deleted SubSamples as well
                            lst.AddRange(ss.OfflineDeletedEntities.OfType<LavSFTransferItem>());

                            if (lst.Count > 0)
                            {
                                cres = SaveLavSFItems(null, lst);

                                if (cres.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                                {
                                    syncRef = new SyncDatabaseOperationResult(cres);
                                    return CompressTrip(t);
                                }
                            }
                        }
                    }
                }

                resArr = CompressTrip(t);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SynchronizeCruise() with message: " + ex.Message;
                resArr = CompressTrip(t);
                datRes = new SyncDatabaseOperationResult(new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg));
            }

            return resArr;
        }

        private byte[] CompressTrip(Trip t)
        {
            var arrT = t.ToByteArrayDataContract();
            return arrT.Compress();
        }


        private bool SetupAndValidateTripAndChilds(ref Trip t)
        {
            bool blnValid = true;

            using (SprattusContainer ctx = new SprattusContainer())
            {
                //Validate Trip entity
                if (!SetupAndValidateTrip(ref t, ctx))
                    blnValid = false;

                //Validate Sample entities
                for (int i = 0; i < t.Sample.Count; i++)
                {
                    Sample s = t.Sample[i];

                    //Make sure tripId is correct, if an existing trip exists, which t would overwrite, this the existing tripId when checking for child samples that
                    //might be overwritten.
                    if (t.OfflineComparisonEntity != null)
                        s.AssignNavigationPropertyWithoutChanges("tripId", t.OfflineComparisonEntity.Cast<Trip>().tripId);

                    if (!SetupAndValidateSample(ref s, ctx))
                        blnValid = false;

                    //Validate SpeciesList
                    for (int j = 0; j < s.SpeciesList.Count; j++)
                    {
                        SpeciesList sl = s.SpeciesList[j];

                        if (s.OfflineComparisonEntity != null)
                            sl.AssignNavigationPropertyWithoutChanges("sampleId", s.OfflineComparisonEntity.Cast<Sample>().sampleId);
                        
                        //Set SL to edit mode, if a duplicate is found (overwriting it in database)
                        if (!SetupAndValidateSpeciesList(ref sl, ctx))
                        {
                            sl.AssignNavigationPropertyWithoutChanges("speciesListId", sl.OfflineComparisonEntity.Cast<SpeciesList>().speciesListId);
                            sl.MarkAsModified();
                        }

                        //Validate SubSample
                        for (int k = 0; k < sl.SubSample.Count; k++)
                        {
                            SubSample ss = sl.SubSample[k];

                            if (sl.OfflineComparisonEntity != null)
                                ss.AssignNavigationPropertyWithoutChanges("speciesListId", sl.OfflineComparisonEntity.Cast<SpeciesList>().speciesListId);

                            //Set SS to edit mode, if a duplicate is found (overwriting it in database)
                            if(!SetupAndValidateSubSample(ref ss, ctx))
                            {
                                ss.AssignNavigationPropertyWithoutChanges("subsampleId", ss.OfflineComparisonEntity.Cast<SubSample>().subSampleId);
                                ss.MarkAsModified();
                            }

                            //Validate Animal
                            for (int l = 0; l < ss.Animal.Count; l++)
                            {
                                Animal a = ss.Animal[l];

                                if (ss.OfflineComparisonEntity != null)
                                    a.AssignNavigationPropertyWithoutChanges("subSampleId", ss.OfflineComparisonEntity.Cast<SubSample>().subSampleId);

                                //Set a to edit mode, if a duplicate is found (overwriting it in database)
                                if (!SetupAndValidateAnimal(ref a, ctx))
                                {
                                    a.AssignNavigationPropertyWithoutChanges("animalId", a.OfflineComparisonEntity.Cast<Animal>().animalId);
                                    a.MarkAsModified();
                                }

                                //Validate Age
                                for (int m = 0; m < a.Age.Count; m++)
                                {
                                    Age age = a.Age[m];

                                    if (a.OfflineComparisonEntity != null)
                                        age.AssignNavigationPropertyWithoutChanges("animalId", a.OfflineComparisonEntity.Cast<Animal>().animalId);

                                    //Set a to edit mode, if a duplicate is found (overwriting it in database)
                                    if (!SetupAndValidateAge(ref age, ctx))
                                    {
                                        age.AssignNavigationPropertyWithoutChanges("ageId", age.OfflineComparisonEntity.Cast<Age>().ageId);
                                        age.MarkAsModified();
                                    }
                                }

                                //Validate AnimalFiles
                                for (int m = 0; m < a.AnimalFiles.Count; m++)
                                {
                                    AnimalFile af = a.AnimalFiles[m];

                                    if (a.OfflineComparisonEntity != null)
                                        af.AssignNavigationPropertyWithoutChanges("animalId", af.OfflineComparisonEntity.Cast<Animal>().animalId);

                                    //Set af to edit mode, if a duplicate is found (overwriting it in database)
                                    if (!SetupAndValidateAnimalFile(ref af, ctx))
                                    {
                                        af.AssignNavigationPropertyWithoutChanges("animalFileId", af.OfflineComparisonEntity.Cast<AnimalFile>().animalFileId);
                                        af.MarkAsModified();
                                    }
                                }

                                //Validate AnimalInfo (there should only be maximum 1 animal info)
                                if (a.AnimalInfo.Count > 0)
                                {
                                    AnimalInfo ai = a.AnimalInfo.First();

                                    if (a.OfflineComparisonEntity != null)
                                        ai.AssignNavigationPropertyWithoutChanges("animalId", a.OfflineComparisonEntity.Cast<Animal>().animalId);

                                    //Set ai to edit mode, if a duplicate is found (overwriting it in database)
                                    if (!SetupAndValidateAnimalInfo(ref ai, ctx))
                                    {
                                        ai.AssignNavigationPropertyWithoutChanges("animalInfoId", ai.OfflineComparisonEntity.Cast<AnimalInfo>().animalInfoId);
                                        ai.MarkAsModified();
                                    }

                                    //Validate R_AnimalInfoReference
                                    for (int m = 0; m < ai.R_AnimalInfoReference.Count; m++)
                                    {
                                        R_AnimalInfoReference air = ai.R_AnimalInfoReference[m];

                                        if (ai.OfflineComparisonEntity != null)
                                            air.AssignNavigationPropertyWithoutChanges("animalInfoId", ai.OfflineComparisonEntity.Cast<AnimalInfo>().animalInfoId);

                                        //Set air to edit mode, if a duplicate is found (overwriting it in database)
                                        if (!SetupAndValidateR_AnimalInfoReference(ref air, ctx))
                                        {
                                            air.AssignNavigationPropertyWithoutChanges("R_animalInfoReferenceId", air.OfflineComparisonEntity.Cast<R_AnimalInfoReference>().R_animalInfoReferenceId);
                                            air.MarkAsModified();
                                        }
                                    }
                                }


                            }
                        }
                    }
                }
            }

            return blnValid;
        }


       


        private void SetupSpeciesList(ref SpeciesList sl)
        {
            SpeciesList sll = sl;
            Entities.ExtensionMethods.HandleOfflineId(ref sl, () => sll.speciesListId);

            for (int i = 0; i < sl.SubSample.Count; i++)
            {
                var ss = sl.SubSample[i];
                Entities.ExtensionMethods.HandleOfflineId(ref ss, () => ss.subSampleId);
            }
        }


        private bool SetupAndValidateTrip(ref Trip t, SprattusContainer ctx)
        {
            Trip tt = t;
            Entities.ExtensionMethods.HandleOfflineId(ref t, () => tt.tripId);

            //If entity is unchaged, do nothing
            if (t.ChangeTracker.State == ObjectState.Unchanged)
                return true;

            Trip existingTrip = null;
            if ((existingTrip = ValidateTripDuplicateKey(t, ctx)) != null)
            {
                //Include Sample if HVN
                if (existingTrip.IsHVN)
                    ctx.LoadProperty(existingTrip, "Sample");

                t.OfflineComparisonEntity = existingTrip;
                return false;
            }

            return true;
        }


        private bool SetupAndValidateSample(ref Sample s, SprattusContainer ctx)
        {
            Sample ss = s;
            Entities.ExtensionMethods.HandleOfflineId(ref s, () => ss.sampleId);

            //If entity is unchaged, do nothing
            if (s.ChangeTracker.State == ObjectState.Unchanged)
                return true;

            Sample existingSample = null;
            if ((existingSample = ValidateSampleDuplicateKey(s, ctx)) != null)
            {
                ctx.LoadProperty(existingSample, "R_TargetSpecies");
                s.OfflineComparisonEntity = existingSample;
                return false;
            }

            return true;
        }



        private bool SetupAndValidateSpeciesList(ref SpeciesList sl, SprattusContainer ctx)
        {
            SpeciesList sll = sl;
            Entities.ExtensionMethods.HandleOfflineId(ref sl, () => sll.speciesListId);

            //If entity is unchaged, do nothing
            if (sl.ChangeTracker.State == ObjectState.Unchanged)
                return true;

            SpeciesList slExisting = null;
            if((slExisting = ctx.SpeciesList.Where(x => x.sampleId == sll.sampleId && 
                                                      (x.speciesCode == sll.speciesCode || (x.speciesCode == null && sll.speciesCode == null)) &&
                                                      (x.landingCategory == sll.landingCategory || (x.landingCategory == null && sll.landingCategory == null)) &&
                                                      (x.sizeSortingDFU == sll.sizeSortingDFU || (x.sizeSortingDFU == null && sll.sizeSortingDFU == null)) &&
                                                      (x.sizeSortingEU == sll.sizeSortingEU || (x.sizeSortingEU == null && sll.sizeSortingEU == null)) &&
                                                      (x.sexCode == sll.sexCode || (x.sexCode == null && sll.sexCode == null)) &&
                                                      (x.ovigorous == sll.ovigorous || (x.ovigorous == null && sll.ovigorous == null)) &&
                                                      (x.cuticulaHardness == sll.cuticulaHardness || (x.cuticulaHardness == null && sll.cuticulaHardness == null))).FirstOrDefault()) != null)
            {
                sl.OfflineComparisonEntity = slExisting;
                return false;
            }

            return true;
        }


        public bool SetupAndValidateSubSample(ref SubSample ss, SprattusContainer ctx)
        {
            SubSample sss = ss;

            Entities.ExtensionMethods.HandleOfflineId(ref ss, () => sss.subSampleId);

            //If entity is unchaged, do nothing
            if (ss.ChangeTracker.State == ObjectState.Unchanged)
                return true;

            SubSample ssExisting = null;
            if ((ssExisting = ctx.SubSample.Where(x => x.speciesListId == sss.speciesListId &&
                                                       x.stepNum == sss.stepNum &&
                                                      (x.representative == sss.representative || (sss.representative == null && sss.representative == null))).FirstOrDefault()) != null)
            {
                ss.OfflineComparisonEntity = ssExisting;
                return false;
            }

            return true;
        }


        public bool SetupAndValidateAnimal(ref Animal a, SprattusContainer ctx)
        {
            Animal aa = a;

            Entities.ExtensionMethods.HandleOfflineId(ref a, () => aa.animalId);

            if (a.ChangeTracker.State == ObjectState.Unchanged)
                return true;

            Animal aExisting = null;
            if ((aExisting = ctx.Animal.Where(x => x.subSampleId == aa.subSampleId &&
                                                  (x.individNum == aa.individNum || (x.individNum == null && aa.individNum == null)) &&
                                                  (x.length == aa.length || (x.length == null && aa.length == null)) &&
                                                  (x.sexCode == aa.sexCode || (x.sexCode == null && aa.sexCode == null)) &&
                                                  (x.broodingPhase == aa.broodingPhase || (x.broodingPhase == null && aa.broodingPhase == null))).FirstOrDefault()) != null)
            {
                a.OfflineComparisonEntity = aExisting;
                return false;
            }


            return true;
        }


        public bool SetupAndValidateAge(ref Age a, SprattusContainer ctx)
        {
            Age aa = a;

            Entities.ExtensionMethods.HandleOfflineId(ref a, () => aa.ageId);

            if (a.ChangeTracker.State == ObjectState.Unchanged)
                return true;

            Age aExisting = null;
            if ((aExisting = ctx.Age.Where(x => x.animalId == aa.animalId &&
                                               (x.age1 == aa.age1 || (x.age1 == null && aa.age1 == null)) &&
                                               (x.ageMeasureMethodId == aa.ageMeasureMethodId) &&
                                               (x.hatchMonth == aa.hatchMonth || (x.hatchMonth == null && aa.hatchMonth == null))).FirstOrDefault()) != null)
            {
                a.OfflineComparisonEntity = aExisting;
                return false;
            }

            return true;
        }


        public bool SetupAndValidateAnimalFile(ref AnimalFile af, SprattusContainer ctx)
        {
            AnimalFile aa = af;

            Entities.ExtensionMethods.HandleOfflineId(ref af, () => aa.animalFileId);

            if (af.ChangeTracker.State == ObjectState.Unchanged)
                return true;

            AnimalFile aExisting = null;
            if ((aExisting = ctx.AnimalFile.Where(x => x.animalId == aa.animalId &&
                                                 (x.autoAdded == aa.autoAdded) &&
                                                 (x.fileType == aa.fileType) &&
                                                 (x.filePath == aa.filePath)).FirstOrDefault()) != null)
            {
                af.OfflineComparisonEntity = aExisting;
                return false;
            }

            return true;
        }


        public bool SetupAndValidateAnimalInfo(ref AnimalInfo ai, SprattusContainer ctx)
        {
            AnimalInfo aii = ai;

            Entities.ExtensionMethods.HandleOfflineId(ref ai, () => aii.animalInfoId);

            if (ai.ChangeTracker.State == ObjectState.Unchanged)
                return true;

            AnimalInfo aExisting;

            if ((aExisting = ctx.AnimalInfo.Where(x => x.animalId == aii.animalId).FirstOrDefault()) != null)
            {
                ai.OfflineComparisonEntity = aExisting;
                return false;
            }

            return true;
        }


        public bool SetupAndValidateR_AnimalInfoReference(ref R_AnimalInfoReference ar, SprattusContainer ctx)
        {
            R_AnimalInfoReference air = ar;

            Entities.ExtensionMethods.HandleOfflineId(ref ar, () => air.R_animalInfoReferenceId);

            if (ar.ChangeTracker.State == ObjectState.Unchanged)
                return true;

            R_AnimalInfoReference aExisting;
            if ((aExisting = ctx.R_AnimalInfoReference.Where(x => x.animalInfoId == air.animalInfoId && x.L_referenceId == air.L_referenceId).FirstOrDefault()) != null)
            {
                ar.OfflineComparisonEntity = aExisting;
                return false;
            }

            return true;
        }
    }
}