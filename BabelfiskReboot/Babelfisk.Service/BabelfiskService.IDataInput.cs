using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using System.Web;
using Babelfisk.Entities;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using System.Linq.Expressions;

namespace Babelfisk.Service
{
    public partial class BabelfiskService : IDataInput
    {


        #region Cruise methods


        public List<L_Year> GetYears()
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var years = from c in ctx.Cruise
                            group c by c.year into gYears
                            select new { Year = gYears.Key, Cruises = gYears };

                var lst = years.Select(x => new L_Year() { Year = x.Year, CruiseCount = x.Cruises.Count() }).ToList();

                return lst;
            }
        }


        public List<string> GetCruiseNames()
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var cruiseNames = ctx.Cruise.Select(x => x.cruise1).Distinct().ToList();

                return cruiseNames;
            }
        }


        public Cruise GetCruiseFromId(int intCruiseId)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                //DFUPerson corresponds to responsibleId
                var cruise = ctx.Cruise/*.Include("DFUPerson").Include("DFUPerson1").*/.Where(x => x.cruiseId == intCruiseId).FirstOrDefault();

                return cruise;
            }
        }


        public Cruise CreateAndGetCruise(int cruiseYear, string cruiseName)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                //DFUPerson corresponds to responsibleId
                var cruise = ctx.Cruise.Where(c => c.year == cruiseYear && c.cruise1 != null && c.cruise1.ToLower() == cruiseName.ToLower()).FirstOrDefault();

                //Create a new cruise if it does not exist.
                if(cruise == null)
                {
                    cruise = new Cruise();
                    cruise.year = cruiseYear;
                    cruise.cruise1 = cruiseName;
                    SaveCruise(ref cruise);
                }

                return cruise;
            }
        }


        public DatabaseOperationResult SaveCruise(ref Cruise c)
        {
            if (c.ChangeTracker.State == ObjectState.Added)
                return SaveNewCruise(ref c);

            //Save changes to an existing cruise.
            try
            {
                using (SprattusContainer ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();

                    if (ValidateCruiseDuplicateKey(c, ctx) != null)
                        return new DatabaseOperationResult(DatabaseOperationStatus.ValidationError, "DuplicateKey");

                    int intCruiseId = c.cruiseId;
                    var cruiseEdit = ctx.Cruise.Where(x => x.cruiseId == intCruiseId).FirstOrDefault();

                    if (cruiseEdit == null)
                        return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, "Could not find cruise to edit.");

                    if (!(c.OverwritingMethod.HasValue && c.OverwritingMethod.Value == OverwritingMethod.ServerWins))
                    {
                        if (c.cruise1 != cruiseEdit.cruise1)
                            cruiseEdit.cruise1 = c.cruise1;

                        if (cruiseEdit.cruiseTitle != c.cruiseTitle)
                            cruiseEdit.cruiseTitle = c.cruiseTitle;

                        if (cruiseEdit.summary != c.summary)
                            cruiseEdit.summary = c.summary;

                        if (cruiseEdit.responsibleId != c.responsibleId)
                            cruiseEdit.responsibleId = c.responsibleId;

                        if (cruiseEdit.participants != c.participants)
                            cruiseEdit.participants = c.participants;

                        if (cruiseEdit.remark != c.remark)
                            cruiseEdit.remark = c.remark;
                    }

                    ctx.Cruise.ApplyChanges(cruiseEdit);

                    ctx.SaveChangesAndHandleOptimisticConcurrency(c.OverwritingMethod.HasValue ? c.OverwritingMethod.Value : OverwritingMethod.ClientWins);

                    c.AcceptChanges();

                    AddCruiseToDataWarehouseTransferQueue(c.cruiseId);
                }
                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in SaveCruise with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in SaveCruise with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SaveCruise with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }


        private Cruise ValidateCruiseDuplicateKey(Cruise cruise, SprattusContainer ctx)
        {
            //if (cruise.ChangeTracker.State != ObjectState.Added)
            //    return true;
            bool blnIsNew = cruise.ChangeTracker.State == ObjectState.Added;

            var vCruises = (from c in ctx.Cruise
                           where c.cruise1.Equals(cruise.cruise1, StringComparison.InvariantCultureIgnoreCase) &&
                                 c.year.Equals(cruise.year) &&
                                 (c.cruiseId != cruise.cruiseId || blnIsNew)
                           select c);

            return vCruises.FirstOrDefault();
        }


        private DatabaseOperationResult SaveNewCruise(ref Cruise c)
        {
            try
            {
                using (SprattusContainer ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();

                    if (ValidateCruiseDuplicateKey(c, ctx) != null)
                        return new DatabaseOperationResult(DatabaseOperationStatus.ValidationError, "DuplicateKey");

                    ctx.Cruise.ApplyChanges(c);

                    ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);

                    c.AcceptChanges();

                    AddCruiseToDataWarehouseTransferQueue(c.cruiseId);
                }
                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in SaveNewCruise with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in SaveNewCruise with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SaveNewCruise with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }



        public DatabaseOperationResult DeleteCruise(int intCruiseId)
        {
            try
            {
                using (SprattusContainer ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();

                    DeleteCruiseFromId(ctx, intCruiseId);

                    ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);

                    AddCruiseToDataWarehouseTransferQueue(intCruiseId);
                }
                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in DeleteCruise with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in DeleteCruise with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in DeleteCruise with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }





        #endregion


        #region Trip methods


        public Trip GetTripFromId(int intTripId, string[] arrIncludes)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);
                
                var trip = ctx.Trip.Include(arrIncludes)/*.Include("DFUPerson")
                                   .Include("DFUPerson1")
                                   .Include("L_SamplingMethod")
                                   .Include("L_Harbour")
                                   .Include("L_Platform")
                                   .Include("L_Platform1")
                                   .Include("Person")
                                   .Include("Person")*/
                                   .Where(x => x.tripId == intTripId).FirstOrDefault();

                return trip;
            }
        }


        public Person GetLatestPersonFromPlatformId(int intPlatformId)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var p = ctx.Trip.Include("Person").Where(x => x.L_Platform.L_platformId == intPlatformId && x.contactPersonId != null).OrderByDescending(t => t.tripId).FirstOrDefault();
                
                return p == null ? null : p.Person;
            }
        }

        private void AssignTripChanges(SprattusContainer ctx, Trip tripEdit, Trip t)
        {
            if (tripEdit.cruiseId != t.cruiseId)
                tripEdit.cruiseId = t.cruiseId;

            if (tripEdit.trip1 != t.trip1)
                tripEdit.trip1 = t.trip1;

            if (tripEdit.logBldNr != t.logBldNr)
                tripEdit.logBldNr = t.logBldNr;

            if (tripEdit.tripType != t.tripType)
                tripEdit.tripType = t.tripType;

            if (tripEdit.samplingMethodId != t.samplingMethodId)
                tripEdit.samplingMethodId = t.samplingMethodId;

            if (tripEdit.samplingTypeId != t.samplingTypeId)
                tripEdit.samplingTypeId = t.samplingTypeId;

            if (tripEdit.timeZone != t.timeZone)
                tripEdit.timeZone = t.timeZone;

            if (tripEdit.dateStart != t.dateStart)
                tripEdit.dateStart = t.dateStart;

            if (tripEdit.dateEnd != t.dateEnd)
                tripEdit.dateEnd = t.dateEnd;

            if (tripEdit.harbourLanding != t.harbourLanding)
                tripEdit.harbourLanding = t.harbourLanding;

            if (tripEdit.dateSample != t.dateSample)
                tripEdit.dateSample = t.dateSample;

            if (tripEdit.harbourSample != t.harbourSample)
                tripEdit.harbourSample = t.harbourSample;

            if (tripEdit.platform1 != t.platform1)
                tripEdit.platform1 = t.platform1;

            if (tripEdit.platform2 != t.platform2)
                tripEdit.platform2 = t.platform2;

            if (tripEdit.fisheryType != t.fisheryType)
                tripEdit.fisheryType = t.fisheryType;

            if (tripEdit.tripNum != t.tripNum)
                tripEdit.tripNum = t.tripNum;

            if (tripEdit.placeName != t.placeName)
                tripEdit.placeName = t.placeName;

            if (tripEdit.placeCode != t.placeCode)
                tripEdit.placeCode = t.placeCode;

            if (tripEdit.postalCode != t.postalCode)
                tripEdit.postalCode = t.postalCode;

            if (tripEdit.numberInPlace != t.numberInPlace)
                tripEdit.numberInPlace = t.numberInPlace;

            if (tripEdit.respYes != t.respYes)
                tripEdit.respYes = t.respYes;

            if (tripEdit.respNo != t.respNo)
                tripEdit.respNo = t.respNo;

            if (tripEdit.respTot != t.respTot)
                tripEdit.respTot = t.respTot;

            if (tripEdit.sgTripId != t.sgTripId)
                tripEdit.sgTripId = t.sgTripId;

            //Store contact person details.
            if (t.Person != null)
            {
                var vContactPerson = t.Person;
                t.Person = null;

                if (vContactPerson != null && vContactPerson.ChangeTracker.State != ObjectState.Unchanged)
                {
                    List<ILookupEntity> lst = new List<ILookupEntity>(new ILookupEntity[] { vContactPerson });
                    SaveLookups(ref lst);
                }

                tripEdit.contactPersonId = vContactPerson == null ? new Nullable<int>() : vContactPerson.personId;
            }
            else
            {
                if(tripEdit.contactPersonId != t.contactPersonId)
                    tripEdit.contactPersonId = t.contactPersonId;
            }


            if (tripEdit.fDFVessel != t.fDFVessel)
                tripEdit.fDFVessel = t.fDFVessel;

            //Assign trip leader details
            if (tripEdit.tripLeaderId != t.tripLeaderId)
                tripEdit.tripLeaderId = t.tripLeaderId;

            if (tripEdit.dataHandlerId != t.dataHandlerId)
                tripEdit.dataHandlerId = t.dataHandlerId;

            if (tripEdit.remark != t.remark)
                tripEdit.remark = t.remark;
        }


        public DatabaseOperationResult SaveTrip(ref Trip t)
        {
            if (t.ChangeTracker.State == ObjectState.Added)
                return SaveNewTrip(ref t);

            //Save changes to an existing trip.
            try
            {
                using (SprattusContainer ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();

                    if (ValidateTripDuplicateKey(t, ctx) != null)
                        return new DatabaseOperationResult(DatabaseOperationStatus.ValidationError, "DuplicateKey");

                    int intTripId = t.tripId;
                    var tripEdit = ctx.Trip.Where(x => x.tripId == intTripId).FirstOrDefault();

                    if (tripEdit == null)
                        return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, "Could not find trip to edit.");
                    
                    if (!(t.OverwritingMethod.HasValue && t.OverwritingMethod.Value == OverwritingMethod.ServerWins))
                        AssignTripChanges(ctx, tripEdit, t);
                   
                    ctx.Trip.ApplyChanges(tripEdit);

                    ctx.SaveChangesAndHandleOptimisticConcurrency(t.OverwritingMethod.HasValue ? t.OverwritingMethod.Value : OverwritingMethod.ClientWins);

                    t.AcceptChanges();

                    AddCruiseToDataWarehouseTransferQueue(t.cruiseId);
                }
                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in SaveTrip with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in SaveTrip with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SaveTrip with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }


        private Trip ValidateTripDuplicateKey(Trip trip, SprattusContainer ctx)
        {
            bool blnIsNew = trip.ChangeTracker.State == ObjectState.Added;

            var vTrips = from t in ctx.Trip
                           where t.trip1.Equals(trip.trip1, StringComparison.InvariantCultureIgnoreCase) &&
                                 t.cruiseId.Equals(trip.cruiseId) &&
                                 (t.tripId != trip.tripId || blnIsNew)
                           select t;

            return vTrips.FirstOrDefault();
        }


        private DatabaseOperationResult SaveNewTrip(ref Trip t)
        {
            try
            {
                using (SprattusContainer ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();

                    if (ValidateTripDuplicateKey(t, ctx) != null)
                        return new DatabaseOperationResult(DatabaseOperationStatus.ValidationError, "DuplicateKey");

                    //Create a new entity, to ensure correct data insertion.
                    Trip tNew = new Trip();
                    AssignTripChanges(ctx, tNew, t);

                    var lstSamplingTypes = ctx.L_SamplingType.ToList();

                    switch (t.tripType)
                    {
                        case "VID":
                            //do nothing (it should be null)
                            break;

                        case "SØS":
                            tNew.samplingTypeId = lstSamplingTypes.Where(x => x.samplingType.Equals("S")).First().samplingTypeId;
                            t.samplingTypeId = tNew.samplingTypeId;
                            break;
                    }

                    ctx.Trip.ApplyChanges(tNew);

                    ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);

                    //Make sure referenced trip has correct id
                    t.AssignNavigationPropertyWithoutChanges("tripId", tNew.tripId);

                    tNew.AcceptChanges();
                    t.AcceptChanges();

                    AddCruiseToDataWarehouseTransferQueue(t.cruiseId);
                }
                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in SaveNewTrip with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in SaveNewTrip with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SaveNewTrip with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }


        public DatabaseOperationResult DeleteTrip(int intTripId)
        {
            try
            {
                using (SprattusContainer ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();

                    AddCruiseByTripIdToDataWarehouseTransferQueue(ctx, intTripId);

                    DeleteTripFromId(ctx, intTripId);

                    ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);
                }
                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in DeleteTrip with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in DeleteTrip with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in DeleteTrip with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }

        #endregion


        #region Sample Methods (station)


        public Sample GetSampleFromId(int intSampleId)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var sample = ctx.Sample.Include("R_TargetSpecies").Where(x => x.sampleId == intSampleId).FirstOrDefault();

                return sample;
            }
        }


        /// <summary>
        /// Retrieve latest sample from database (the sample created last)
        /// </summary>
        public Sample GetLatestSample(int intTripId)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var sample = ctx.Sample.Include("R_TargetSpecies").Where(x => x.tripId == intTripId).OrderByDescending(x => x.sampleId).FirstOrDefault();

                return sample;
            }
        }


        public List<Sample> GetSamplesFromTripId(int intTripId)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var samples = (from s in ctx.Sample
                                            .Include("R_TargetSpecies")
                               where s.tripId == intTripId
                               select s).ToList();

                return samples.ToList();
            }
        }


        private void AssignSampleChanges(SprattusContainer ctx, Sample sEdit, Sample s)
        {
            if (sEdit.tripId != s.tripId)
                sEdit.tripId = s.tripId;

            if (sEdit.station != s.station)
                sEdit.station = s.station;

            if (sEdit.stationName != s.stationName)
                sEdit.stationName = s.stationName;

            if (sEdit.@virtual != s.@virtual)
                sEdit.@virtual = s.@virtual;

            if (sEdit.sampleType != s.sampleType)
                sEdit.sampleType = s.sampleType;

            if (sEdit.dateGearStart != s.dateGearStart)
                sEdit.dateGearStart = s.dateGearStart;

            if (sEdit.dateGearEnd != s.dateGearEnd)
                sEdit.dateGearEnd = s.dateGearEnd;

            if (sEdit.timeZone != s.timeZone)
                sEdit.timeZone = s.timeZone;

            if (sEdit.fishingtime != s.fishingtime)
                sEdit.fishingtime = s.fishingtime;

            if (sEdit.latPosStartText != s.latPosStartText)
                sEdit.latPosStartText = s.latPosStartText;

            if (sEdit.latPosEndText != s.latPosEndText)
                sEdit.latPosEndText = s.latPosEndText;

            if (sEdit.lonPosStartText != s.lonPosStartText)
                sEdit.lonPosStartText = s.lonPosStartText;

            if (sEdit.lonPosEndText != s.lonPosEndText)
                sEdit.lonPosEndText = s.lonPosEndText;

            if (sEdit.catchRegistrationId != s.catchRegistrationId)
                sEdit.catchRegistrationId = s.catchRegistrationId;

            if (sEdit.speciesRegistrationId != s.speciesRegistrationId)
                sEdit.speciesRegistrationId = s.speciesRegistrationId;

            if (sEdit.dfuArea != s.dfuArea)
                sEdit.dfuArea = s.dfuArea;

            if (sEdit.statisticalRectangle != s.statisticalRectangle)
                sEdit.statisticalRectangle = s.statisticalRectangle;

            if (sEdit.gearType != s.gearType)
                sEdit.gearType = s.gearType;

            if (sEdit.selectionDevice != s.selectionDevice)
                sEdit.selectionDevice = s.selectionDevice;
            
            if (sEdit.selectionDeviceSourceId != s.selectionDeviceSourceId)
                sEdit.selectionDeviceSourceId = s.selectionDeviceSourceId;

            if (sEdit.labJournalNum != s.labJournalNum)
                sEdit.labJournalNum = s.labJournalNum;

            if (sEdit.windDirection != s.windDirection)
                sEdit.windDirection = s.windDirection;

            if (sEdit.windSpeed != s.windSpeed)
                sEdit.windSpeed = s.windSpeed;

            if (sEdit.meshSize != s.meshSize)
                sEdit.meshSize = s.meshSize;

            if (sEdit.numberTrawls != s.numberTrawls)
                sEdit.numberTrawls = s.numberTrawls;

            if (sEdit.heightNets != s.heightNets)
                sEdit.heightNets = s.heightNets;

            if (sEdit.lengthNets != s.lengthNets)
                sEdit.lengthNets = s.lengthNets;

            if (sEdit.lengthRopeFlyer != s.lengthRopeFlyer)
                sEdit.lengthRopeFlyer = s.lengthRopeFlyer;

            if (sEdit.widthRopeFlyer != s.widthRopeFlyer)
                sEdit.widthRopeFlyer = s.widthRopeFlyer;

            if (sEdit.numberHooks != s.numberHooks)
                sEdit.numberHooks = s.numberHooks;

            if (sEdit.lengthBeam != s.lengthBeam)
                sEdit.lengthBeam = s.lengthBeam;

            if (sEdit.haulSpeedBot != s.haulSpeedBot)
                sEdit.haulSpeedBot = s.haulSpeedBot;

            if (sEdit.depthAveGear != s.depthAveGear)
                sEdit.depthAveGear = s.depthAveGear;

            if (sEdit.numNets != s.numNets)
                sEdit.numNets = s.numNets;

            if (sEdit.lostNets != s.lostNets)
                sEdit.lostNets = s.lostNets;

            if (sEdit.gearRemark != s.gearRemark)
                sEdit.gearRemark = s.gearRemark;

            if (sEdit.remark != s.remark)
                sEdit.remark = s.remark;

            if (sEdit.samplePersonId != s.samplePersonId)
                sEdit.samplePersonId = s.samplePersonId;

            if (sEdit.analysisPersonId != s.analysisPersonId)
                sEdit.analysisPersonId = s.analysisPersonId;

            if (sEdit.haulType != s.haulType)
                sEdit.haulType = s.haulType;

            if (sEdit.hydroStnRef != s.hydroStnRef)
                sEdit.hydroStnRef = s.hydroStnRef;

            if (sEdit.temperatureSrf != s.temperatureSrf)
                sEdit.temperatureSrf = s.temperatureSrf;

            if (sEdit.temperatureBot != s.temperatureBot)
                sEdit.temperatureBot = s.temperatureBot;

            if (sEdit.oxygenSrf != s.oxygenSrf)
                sEdit.oxygenSrf = s.oxygenSrf;

            if (sEdit.oxygenBot != s.oxygenBot)
                sEdit.oxygenBot = s.oxygenBot;

            if (sEdit.thermoCline != s.thermoCline)
                sEdit.thermoCline = s.thermoCline;

            if (sEdit.thermoClineDepth != s.thermoClineDepth)
                sEdit.thermoClineDepth = s.thermoClineDepth;

            if (sEdit.salinitySrf != s.salinitySrf)
                sEdit.salinitySrf = s.salinitySrf;

            if (sEdit.salinityBot != s.salinityBot)
                sEdit.salinityBot = s.salinityBot;

            if (sEdit.waveDirection != s.waveDirection)
                sEdit.waveDirection = s.waveDirection;

            if (sEdit.waveHeigth != s.waveHeigth)
                sEdit.waveHeigth = s.waveHeigth;

            if (sEdit.currentDirectionSrf != s.currentDirectionSrf)
                sEdit.currentDirectionSrf = s.currentDirectionSrf;

            if (sEdit.currentDirectionBot != s.currentDirectionBot)
                sEdit.currentDirectionBot = s.currentDirectionBot;

            if (sEdit.currentSpeedSrf != s.currentSpeedSrf)
                sEdit.currentSpeedSrf = s.currentSpeedSrf;

            if (sEdit.currentSpeedBot != s.currentSpeedBot)
                sEdit.currentSpeedBot = s.currentSpeedBot;

            if (sEdit.bottomType != s.bottomType)
                sEdit.bottomType = s.bottomType;

            if (sEdit.depthAvg != s.depthAvg)
                sEdit.depthAvg = s.depthAvg;

            if (sEdit.courseTrack != s.courseTrack)
                sEdit.courseTrack = s.courseTrack;

            if (sEdit.haulSpeedWat != s.haulSpeedWat)
                sEdit.haulSpeedWat = s.haulSpeedWat;

            if (sEdit.haulDirection != s.haulDirection)
                sEdit.haulDirection = s.haulDirection;

            if (sEdit.netOpening != s.netOpening)
                sEdit.netOpening = s.netOpening;

            if (sEdit.shovelDist != s.shovelDist)
                sEdit.shovelDist = s.shovelDist;

            if (sEdit.wireLength != s.wireLength)
                sEdit.wireLength = s.wireLength;

            if (sEdit.wingSpread != s.wingSpread)
                sEdit.wingSpread = s.wingSpread;

            if (sEdit.gearQuality != s.gearQuality)
                sEdit.gearQuality = s.gearQuality;

            if (sEdit.weightEstimationMethod != s.weightEstimationMethod)
                sEdit.weightEstimationMethod = s.weightEstimationMethod;

            if (sEdit.totalWeight != s.totalWeight)
                sEdit.totalWeight = s.totalWeight;

            if (sEdit.sgId != s.sgId)
                sEdit.sgId = s.sgId;

            if (sEdit.weekdayWeekend != s.weekdayWeekend)
                sEdit.weekdayWeekend = s.weekdayWeekend;

            HandleSampleTargetSpecies(ctx, s);

            if (s.R_TargetSpecies.Count > 0 && s.R_TargetSpecies.First().ChangeTracker.State == ObjectState.Added)
            {
                sEdit.R_TargetSpecies.Add(new R_TargetSpecies() { speciesCode = s.R_TargetSpecies.First().speciesCode });
            }
        }

        public DatabaseOperationResult SaveSample(ref Sample s)
        {
            if (s.ChangeTracker.State == ObjectState.Added)
                return SaveNewSample(ref s);

            //Save changes to an existing sample.
            try
            {
                using (SprattusContainer ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();

                    if (ValidateSampleDuplicateKey(s, ctx) != null)
                        return new DatabaseOperationResult(DatabaseOperationStatus.ValidationError, "DuplicateKey", "Sample");

                    int intSampleId = s.sampleId;
                    var sEdit = ctx.Sample.Where(x => x.sampleId == intSampleId).FirstOrDefault();

                    if (sEdit == null)
                        return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, "Could not find sample to edit.");

                    if (!(s.OverwritingMethod.HasValue && s.OverwritingMethod.Value == OverwritingMethod.ServerWins))
                        AssignSampleChanges(ctx, sEdit, s);

                    ctx.Sample.ApplyChanges(sEdit);

                    ctx.SaveChangesAndHandleOptimisticConcurrency(s.OverwritingMethod.HasValue ? s.OverwritingMethod.Value : OverwritingMethod.ClientWins);

                    s.AcceptChanges();

                    AddCruiseBySampleIdToDataWarehouseTransferQueue(ctx, s.sampleId);
                }
                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in SaveTrip with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in SaveTrip with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SaveTrip with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }


        private void HandleSampleTargetSpecies(SprattusContainer ctx, Sample s)
        {
            if (s.R_TargetSpecies.Count > 0)
            {
                var ts = s.R_TargetSpecies.First();

                if (ts.ChangeTracker.State == ObjectState.Unchanged || ts.ChangeTracker.State == ObjectState.Added)
                    return;

                var tsEdit = ctx.R_TargetSpecies.Where(x => x.TargetSpeciesId == ts.TargetSpeciesId).First();

                if (tsEdit == null)
                    return;

               tsEdit.speciesCode = ts.speciesCode;
                

                ctx.R_TargetSpecies.ApplyChanges(tsEdit);
                ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);
            }
            else if (s.ChangeTracker.ObjectsRemovedFromCollectionProperties.SelectMany(x => x.Value).OfType<R_TargetSpecies>().Count() > 0)
            {
                var lst = s.ChangeTracker.ObjectsRemovedFromCollectionProperties.SelectMany(x => x.Value).OfType<R_TargetSpecies>().ToList();

                foreach (var ts in lst)
                {
                    if (ts.ChangeTracker.State != ObjectState.Deleted)
                        continue;

                    var tsEdit = ctx.R_TargetSpecies.Where(x => x.TargetSpeciesId == ts.TargetSpeciesId).First();

                    if (tsEdit == null)
                        continue;

                    ctx.DeleteObject(tsEdit);
                }
            }
        }


        private Sample ValidateSampleDuplicateKey(Sample sample, SprattusContainer ctx)
        {
            bool blnIsNew = sample.ChangeTracker.State == ObjectState.Added;

            var v = from t in ctx.Sample
                    where t.station.Equals(sample.station, StringComparison.InvariantCultureIgnoreCase) &&
                               t.tripId.Equals(sample.tripId) &&
                               (t.sampleId != sample.sampleId || blnIsNew)
                         select t;

            return v.FirstOrDefault();
        }


        private DatabaseOperationResult SaveNewSample(ref Sample sample)
        {
            try
            {
                using (SprattusContainer ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();

                    if (ValidateSampleDuplicateKey(sample, ctx) != null)
                        return new DatabaseOperationResult(DatabaseOperationStatus.ValidationError, "DuplicateKey");

                    Sample sNew = new Sample();
                    AssignSampleChanges(ctx, sNew, sample);
                   
                    ctx.Sample.ApplyChanges(sNew);

                    ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);

                    sample.AssignNavigationPropertyWithoutChanges("sampleId", sNew.sampleId);
                    sample.AcceptChanges();
                    sNew.AcceptChanges();

                    AddCruiseBySampleIdToDataWarehouseTransferQueue(ctx, sNew.sampleId);
                }
                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in SaveNewSample with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in SaveNewSample with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SaveNewSample with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }



        public List<L_StatisticalRectangle> GetStatisticalRectangleFromArea(string strAreaCode)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                strAreaCode = strAreaCode.ToLower();
                var t = ctx.ICES_DFU_Relation_FF.Where(x => x.area_20_21.ToLower().Equals(strAreaCode)).Select(x => x.statisticalRectangle.ToUpper()).Distinct().ToList();


                var rects = from s in t //.Where(x => x.statisticalRectangle.ToUpper().Equals(strAreaCode)).FirstOrDefault();
                            join stat in ctx.L_StatisticalRectangle on s equals stat.statisticalRectangle.ToUpper()
                            select stat;

                return rects.ToList();
            }
        }


        public List<L_SelectionDevice> GetSelectionDevicesFromGearType(string strGearType)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var lst = ctx.R_GearTypeSelectionDevice.Include("L_SelectionDevice").Where(x => x.gearType.Equals(strGearType)).Select(x => x.L_SelectionDevice).Distinct().ToList();
                return lst;
            }
        }


        public DatabaseOperationResult DeleteSample(int intSampleId)
        {
            try
            {
                using (SprattusContainer ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();

                    AddCruiseBySampleIdToDataWarehouseTransferQueue(ctx, intSampleId);

                    DeleteSampleFromId(ctx, intSampleId);

                    ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);
                }
                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in DeleteSample with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in DeleteSample with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in DeleteSample with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }


        #endregion


        #region HVN methods

        public DatabaseOperationResult SaveHVN(ref Trip t, ref Sample s)
        {
            if (s.ChangeTracker.State == ObjectState.Added)
                return SaveNewHVN(ref t, ref s);

            //Save changes to an existing trip.
            try
            {
                using (SprattusContainer ctx = new SprattusContainer())
                {
                    //Use a transaction since trip and sample is saved seperately.
                    using (TransactionScope ts = new TransactionScope())
                    {
                        ctx.Connection.Open();

                        if (ValidateTripDuplicateKey(t, ctx) != null || ValidateSampleDuplicateKey(s, ctx) != null)
                            return new DatabaseOperationResult(DatabaseOperationStatus.ValidationError, "DuplicateKey");

                        //Get trip out.
                        int intTripId = t.tripId;
                        var tripEdit = ctx.Trip.Where(x => x.tripId == intTripId).FirstOrDefault();

                        if (tripEdit == null)
                            return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, "Could not find trip to edit.");

                        if (!(t.OverwritingMethod.HasValue && t.OverwritingMethod.Value == OverwritingMethod.ServerWins))
                            AssignTripChanges(ctx, tripEdit, t);
                        
                        //Save trip
                        ctx.Trip.ApplyChanges(tripEdit);
                        ctx.SaveChangesAndHandleOptimisticConcurrency(t.OverwritingMethod.HasValue ? t.OverwritingMethod.Value : OverwritingMethod.ClientWins);
                        t.AcceptChanges();

                        //Get sample to edit
                        int intSampleId = s.sampleId;
                        var sEdit = ctx.Sample.Where(x => x.sampleId == intSampleId).FirstOrDefault();

                        if (sEdit == null)
                            return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, "Could not find sample to edit.");

                        if (!(t.OverwritingMethod.HasValue && t.OverwritingMethod.Value == OverwritingMethod.ServerWins))
                            AssignSampleChanges(ctx, sEdit, s);

                        if (sEdit.tripId != t.tripId)
                            sEdit.tripId = t.tripId;

                        HandleSampleTypeFromGearTypeHVN(ctx, sEdit, s);

                        //Save sample
                        ctx.Sample.ApplyChanges(sEdit);
                        ctx.SaveChangesAndHandleOptimisticConcurrency(s.OverwritingMethod.HasValue ? s.OverwritingMethod.Value : OverwritingMethod.ClientWins);
                        s.AcceptChanges();

                        ts.Complete();   
                    }

                    AddCruiseToDataWarehouseTransferQueue(t.cruiseId);
                }
                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in SaveTrip with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in SaveTrip with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SaveTrip with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }

        private void HandleSampleTypeFromGearTypeHVN(SprattusContainer ctx, Sample sEdit, Sample s)
        {
            var gearType = ctx.L_GearType.Where(x => x.gearType.Equals(s.gearType)).FirstOrDefault();

            if (string.IsNullOrEmpty(s.gearType) || gearType == null)
                sEdit.sampleType = "X";
            else
                sEdit.sampleType = gearType.catchOperation;
        }


        public DatabaseOperationResult SaveNewHVN(ref Trip t, ref Sample s)
        {
            try
            {
                using (SprattusContainer ctx = new SprattusContainer())
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        ctx.Connection.Open();

                        if (ValidateSampleDuplicateKey(s, ctx) != null || ValidateTripDuplicateKey(t, ctx) != null)
                            return new DatabaseOperationResult(DatabaseOperationStatus.ValidationError, "DuplicateKey");

                        //Create a new entity, to ensure correct data insertion (the state of the passed in Trip cannot be guarenteed).
                        Trip tNew = new Trip();
                        AssignTripChanges(ctx, tNew, t);

                        ctx.Trip.ApplyChanges(tNew);
                        ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);

                        t.AssignNavigationPropertyWithoutChanges("tripId", tNew.tripId);
                        t.AcceptChanges();
                        tNew.AcceptChanges();

                        s.tripId = tNew.tripId;

                        //Create a new entity, to ensure correct data insertion (the state of the passed in Sample cannot be guarenteed).
                        Sample sNew = new Sample();
                        AssignSampleChanges(ctx, sNew, s);
                        HandleSampleTypeFromGearTypeHVN(ctx, s, s); //Update referenced Sample as well
                        HandleSampleTypeFromGearTypeHVN(ctx, sNew, sNew);

                        ctx.Sample.ApplyChanges(sNew);
                        ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);

                        s.AssignNavigationPropertyWithoutChanges("sampleId", sNew.sampleId);
                        s.AcceptChanges();
                        sNew.AcceptChanges();

                        ts.Complete();
                    }

                    AddCruiseToDataWarehouseTransferQueue(t.cruiseId);
                }
                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in SaveNewSample with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in SaveNewSample with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SaveNewSample with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }

        #endregion


        #region Map methods


        public List<MapPoint> GetMapPositionsFromCruiseId(int intCruiseId)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var lst = ctx.ExecuteStoreQuery<MapPoint>("SELECT s.sampleId as Id, s.station as StationName, t.trip as TripName, s.latPosStartText as LatitudeStart, s.lonPosStartText as LongitudeStart, s.latPosEndText as LatitudeStop, s.lonPosEndText as LongitudeStop " +
                                                          " FROM Trip t " + 
                                                          " INNER JOIN Sample s ON s.tripId = t.tripId " + 
                                                          " WHERE t.cruiseId = {0} AND t.tripType <> 'HVN' ", intCruiseId).ToList();
                           
                return lst.ToList();
            }
        }

        public List<MapPoint> GetMapPositionsFromTripId(int intTripId)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var lst = ctx.ExecuteStoreQuery<MapPoint>("SELECT s.sampleId as Id, s.station as StationName, t.trip as TripName, s.latPosStartText as LatitudeStart, s.lonPosStartText as LongitudeStart, s.latPosEndText as LatitudeStop, s.lonPosEndText as LongitudeStop " +
                                                          " FROM Sample s " +
                                                          " INNER JOIN Trip t ON s.tripId = t.tripId " + 
                                                          " WHERE s.tripId = {0} ", intTripId).ToList();

                return lst.ToList();
            }
        }


        public List<MapPoint> GetMapPositionsFromSampleId(int intSampleId)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var lst = ctx.ExecuteStoreQuery<MapPoint>("SELECT s.sampleId as Id, s.station as StationName, t.trip as TripName, s.latPosStartText as LatitudeStart, s.lonPosStartText as LongitudeStart, s.latPosEndText as LatitudeStop, s.lonPosEndText as LongitudeStop " +
                                                          " FROM Sample s " +
                                                          " INNER JOIN Trip t ON s.tripId = t.tripId " + 
                                                          " WHERE s.sampleId = {0} ", intSampleId).ToList();

                return lst.ToList();
            }
        }


        #endregion


        #region Species List Methods


        public SpeciesList GetSpeciesListFromId(int intSpeciesListId, string[] arrIncludes)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var s = ctx.SpeciesList.Include(arrIncludes)
                           .Where(x => x.speciesListId == intSpeciesListId).FirstOrDefault();

                return s;
            }
        }


        public SubSample GetSubSampleFromId(int intSubSampleId)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var s = ctx.SubSample/*.Include("L_Species")*/
                           .Where(x => x.subSampleId == intSubSampleId).FirstOrDefault();

                return s;
            }
        }


        private void AssignSpeciesListChanges(SprattusContainer ctx, SpeciesList sEdit, SpeciesList s)
        {
            s.CopyEntityValueTypesTo(sEdit);

            //Dont remove any old subsample records, since these will be cleaned up afterwards.

            var lstAnimals = sEdit.SubSample.SelectMany(x => x.Animal).ToList();

            //Add new subsamples and update existing ones.
            foreach (var ss in s.SubSample)
            {
                SubSample ssNewEdit = null;

                if ((ssNewEdit = sEdit.SubSample.Where(x => x.ChangeTracker.State != ObjectState.Added && x.subSampleId == ss.subSampleId).FirstOrDefault()) == null)
                {
                    ssNewEdit = new SubSample();
                    sEdit.SubSample.Add(ssNewEdit);
                }

                ss.CopyEntityValueTypesTo<SubSample, int>(ssNewEdit, () => ss.speciesListId);

               /* //Delete old ones
                foreach(var a in ssNewEdit.Animal.ToArray())
                {
                    if (!ss.Animal.Where(x => x.animalId == a.animalId).Any())
                        ssNewEdit.Animal.Remove(a);
                }

                //Add new ones
                foreach(var a in ss.Animal.ToArray())
                {
                    Animal aa;
                    if (!ssNewEdit.Animal.Where(x => x.animalId == a.animalId).Any() && (aa = lstAnimals.Where(x => x.animalId == a.animalId).FirstOrDefault()) != null)
                        ssNewEdit.Animal.Add(aa);
                }*/

                ss.OfflineComparisonEntity = ssNewEdit;
            }

        }



        /// <summary>
        /// Make sure animals are put on the right representative subsample. They have to be situated on the SubSample with highest stepNum. 
        /// s contains the animals on the highest stepNum, but sEdit does not necessarily - below method therefore makes sure that the animals
        /// are placed the same way as s. 
        /// </summary>
        private void FixSubSampleAnimals(SprattusContainer ctx, SpeciesList sEdit, SpeciesList s)
        {
            //Get all animals from sEdit, since they are fresh from the DB and in the current context (these are the ones to add later on). 
            var lstAnimals = sEdit.SubSample.SelectMany(x => x.Animal).ToList();
            bool changes = false;

            foreach (var ss in s.SubSample)
            {
                SubSample ssNewEdit = null;

                ssNewEdit = sEdit.SubSample.Where(x => x.subSampleId == ss.subSampleId).FirstOrDefault();

                if (ssNewEdit == null)
                    continue;

                //Delete old ones
                foreach (var a in ssNewEdit.Animal.ToArray())
                {
                    if (!ss.Animal.Where(x => x.animalId == a.animalId).Any())
                    {
                        ssNewEdit.Animal.Remove(a);
                        changes = true;
                    }
                }

                //Add new ones
                foreach (var a in ss.Animal.ToArray())
                {
                    Animal aa;
                    if (!ssNewEdit.Animal.Where(x => x.animalId == a.animalId).Any() && (aa = lstAnimals.Where(x => x.animalId == a.animalId).FirstOrDefault()) != null)
                    {
                        ssNewEdit.Animal.Add(aa);
                        changes = true;
                    }
                }
            }

            if (changes)
                ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);
        }


        public DatabaseOperationResult SaveSpeciesListItems(ref List<SpeciesList> speciesListItems)
        {
            try
            {
                using (SprattusContainer ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    using (TransactionScope scope = new TransactionScope())
                    {
                        foreach (SpeciesList sl in speciesListItems)
                        {
                            SpeciesList slNewEdit = null;
                            bool blnDeleted = sl.ChangeTracker.State == ObjectState.Deleted;

                            //If species list is deleted, delete it and all its children.
                            if (blnDeleted)
                                DeleteSpeciesListFromId(ctx, sl.speciesListId);
                            else //Else apply changes to it
                            {
                                //Don't count on sl being in a state that can be directly applied (ApplyChanges)
                                if (sl.ChangeTracker.State == ObjectState.Added)
                                    slNewEdit = new SpeciesList();
                                else
                                {
                                    slNewEdit = ctx.SpeciesList.Include("SubSample").Include("SubSample.Animal").Where(x => x.speciesListId == sl.speciesListId).FirstOrDefault();
                                    if (slNewEdit == null)
                                        slNewEdit = new SpeciesList();
                                }

                                AssignSpeciesListChanges(ctx, slNewEdit, sl);
                                ctx.SpeciesList.ApplyChanges(slNewEdit);
                            }

                            ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);

                            //Update ids on ref'ed specieslist object
                            if (slNewEdit != null)
                            {
                                //Update specieslistId on ref'ed species list
                                sl.AssignNavigationPropertyWithoutChanges("speciesListId", slNewEdit.speciesListId);

                                //Update specieslistId & subsampleId on ref'ed subsamples
                                foreach (var ss in sl.SubSample)
                                {
                                    if (ss.speciesListId != sl.speciesListId)
                                        ss.speciesListId = sl.speciesListId;

                                    if (ss.OfflineComparisonEntity != null && ss.OfflineComparisonEntity.Cast<SubSample>().subSampleId != ss.subSampleId)
                                        ss.subSampleId = ss.OfflineComparisonEntity.Cast<SubSample>().subSampleId;
                                    ss.AcceptChanges();
                                }

                                sl.AcceptChanges();

                                FixSubSampleAnimals(ctx, slNewEdit, sl);
                            }

                            //Make sure old SubSamples are not hanging after updating changes
                            if (!blnDeleted)
                            {
                                CleanUpSubSamples(ctx, sl.speciesListId);
                            }
                        }

                        ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);

                        scope.Complete();
                    }

                    if (speciesListItems != null && speciesListItems.Count > 0)
                        AddCruiseBySampleIdToDataWarehouseTransferQueue(ctx, speciesListItems.First().sampleId);
                }
                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in SaveSpeciesListItems with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in SaveSpeciesListItems with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SaveSpeciesListItems with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }


        private void CleanUpSubSamples(SprattusContainer ctx, int intSpeciesListId)
        {
            var subSamples = ctx.SubSample.Where(x => x.speciesListId == intSpeciesListId).ToList();

            List<int> lstToDelete = new List<int>();

            //Retreive SubSamples with no weight and remove them.
            var noWeight = subSamples.Where(x => x.subSampleWeight == null && x.landingWeight == null && x.sumAnimalWeights == null).Select(x => x.subSampleId).ToList();
            lstToDelete.AddRange(noWeight);
          
            //Get subsamples at lower levels than the one supposed to have animals on it and clear their animal list (if any).
            var q = subSamples.Where(x => x.IsRepresentative && (x.subSampleWeight != null || x.landingWeight != null || x.sumAnimalWeights != null)).OrderBy(x => x.stepNum).ToList();

            for (int i = 0; i < q.Count-1; i++)
            {
                if (!lstToDelete.Contains(q[i].subSampleId))
                    DeleteAnimalsFromSubSampleId(ctx, q[i].subSampleId); //Clear animals from subsample
            }

            //Delete subsamples with no weight.
            foreach (int id in lstToDelete)
                DeleteSubSamplesFromId(ctx, id);
        }


       

        #endregion


        #region LAV SF Methods


        public DatabaseOperationResult SaveLavSFItems(SpeciesList sl, List<LavSFTransferItem> lstItems)
        {
            try
            {
                using (SprattusContainer ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);
                    using (TransactionScope scope = new TransactionScope())
                    {
                        if (sl != null && sl.ChangeTracker.State != ObjectState.Unchanged)
                        {
                            ctx.SpeciesList.ApplyChanges(sl);

                            if (sl.ChangeTracker.State == ObjectState.Deleted)
                                ctx.DeleteObject(sl);

                            ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);
                        }

                        foreach (LavSFTransferItem ls in lstItems)
                        {
                            //If animal is deleted, delete child objects in the right order (not counting on cascade delete to work or be set up)
                            if (ls.Animal != null && ls.Animal.ChangeTracker.State == ObjectState.Deleted)
                            {
                                int intAnimalInfoId = ls.AnimalInfo == null ? -1 : ls.AnimalInfo.animalInfoId;
                                int intAnimalId = ls.Animal.animalId;

                                SaveAnimalInfoReferences(ctx, ls.AnimalInfoReferences, intAnimalInfoId);
                                if(ls.AnimalInfo != null && ls.AnimalInfo.ChangeTracker.State != ObjectState.Added)
                                    SaveAnimalInfo(ctx, ls.AnimalInfo, intAnimalId);
                                SaveAges(ctx, ls.Ages, intAnimalId);
                                SaveAnimalFiles(ctx, ls.AnimalFiles, intAnimalId);
                                SaveAnimal(ctx, ls.Animal, ls.SubSampleId);
                            }
                            else
                            {
                                int intAnimalId = SaveAnimal(ctx, ls.Animal, ls.SubSampleId);
                                if (ls.AnimalInfo != null && ls.AnimalInfo.ChangeTracker.State == ObjectState.Deleted)
                                {
                                    int intAnimalInfoId = ls.AnimalInfo == null ? -1 : ls.AnimalInfo.animalInfoId;
                                    SaveAnimalInfoReferences(ctx, ls.AnimalInfoReferences, intAnimalInfoId);
                                    SaveAnimalInfo(ctx, ls.AnimalInfo, intAnimalId);
                                }
                                else
                                {
                                    if (ls.AnimalInfo != null)
                                    {
                                        int intAnimalInfoId = SaveAnimalInfo(ctx, ls.AnimalInfo, intAnimalId);
                                        SaveAnimalInfoReferences(ctx, ls.AnimalInfoReferences, intAnimalInfoId);
                                    }
                                }
                                SaveAges(ctx, ls.Ages, intAnimalId);
                                SaveAnimalFiles(ctx, ls.AnimalFiles, intAnimalId);
                            }
                        }

                        scope.Complete();
                    }

                    //Add cruise to dataware house transfer queue
                    if (sl != null)
                        AddCruiseBySampleIdToDataWarehouseTransferQueue(ctx, sl.sampleId);
                    else if (lstItems != null && lstItems.Count > 0)
                        AddCruiseBySubSampleIdToDataWarehouseTransferQueue(ctx, lstItems.First().SubSampleId);
                }
                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in SaveLavSFItems with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in SaveLavSFItems with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SaveLavSFItems with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }


        private int SaveAnimal(SprattusContainer ctx, Animal a, int intSubSampleId)
        {
            int intAnimalId = a.animalId;

            if (a.ChangeTracker.State != ObjectState.Unchanged)
            {
                if (a.subSampleId != intSubSampleId && a.ChangeTracker.State != ObjectState.Deleted)
                    a.subSampleId = intSubSampleId;

                Animal aNewEdit = null;
                if (a.ChangeTracker.State != ObjectState.Added)
                    aNewEdit = ctx.Animal.Where(x => x.animalId == a.animalId).FirstOrDefault();

                if (a.ChangeTracker.State == ObjectState.Deleted && aNewEdit != null)
                    ctx.DeleteObject(aNewEdit);
                else
                {
                    if (aNewEdit == null)
                        aNewEdit = new Animal();

                    a.CopyEntityValueTypesTo(aNewEdit);
                    ctx.Animal.ApplyChanges(aNewEdit);
                }

                ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);
                intAnimalId = aNewEdit.animalId;
            }

            return intAnimalId;
        }


        private int SaveAnimalInfo(SprattusContainer ctx, AnimalInfo a, int intAnimalId)
        {
            int intAnimalInfoId = a.animalInfoId;
            if (a.ChangeTracker.State != ObjectState.Unchanged)
            {
                if (a.animalId != intAnimalId && a.ChangeTracker.State != ObjectState.Deleted)
                    a.animalId = intAnimalId;

                AnimalInfo aNewEdit = null;
                if (a.ChangeTracker.State != ObjectState.Added)
                    aNewEdit = ctx.AnimalInfo.Where(x => x.animalInfoId == a.animalInfoId).FirstOrDefault();
                
                if (a.ChangeTracker.State == ObjectState.Deleted && aNewEdit != null)
                    ctx.DeleteObject(aNewEdit);
                else
                {
                    if(aNewEdit == null)
                        aNewEdit = new AnimalInfo();

                    a.CopyEntityValueTypesTo(aNewEdit);
                    ctx.AnimalInfo.ApplyChanges(aNewEdit);
                }

                ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);
                intAnimalInfoId = aNewEdit.animalInfoId;
            }

            return intAnimalInfoId;
        }


        private void SaveAnimalFiles(SprattusContainer ctx, List<AnimalFile> lstFiles, int intAnimalId)
        {
            foreach (var af in lstFiles)
            {
                if(af.ChangeTracker.State != ObjectState.Unchanged)
                {
                    if (af.animalId != intAnimalId && af.ChangeTracker.State != ObjectState.Deleted)
                        af.animalId = intAnimalId;

                    AnimalFile aNewEdit = null;
                    if (af.ChangeTracker.State != ObjectState.Added)
                        aNewEdit = ctx.AnimalFile.Where(x => x.animalFileId == af.animalFileId).FirstOrDefault();

                    if (af.ChangeTracker.State == ObjectState.Deleted && aNewEdit != null)
                        ctx.DeleteObject(aNewEdit);
                    else
                    {
                        if (aNewEdit == null)
                            aNewEdit = new AnimalFile();

                        af.CopyEntityValueTypesTo(aNewEdit);
                        ctx.AnimalFile.ApplyChanges(aNewEdit);
                    }

                    ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);
                }
            }
        }

        private void SaveAges(SprattusContainer ctx, List<Age> lstAges, int intAnimalId)
        {
            foreach (var a in lstAges)
            {
                if (a.ChangeTracker.State != ObjectState.Unchanged) 
                {
                    if (a.animalId != intAnimalId && a.ChangeTracker.State != ObjectState.Deleted)
                        a.animalId = intAnimalId;

                    if (a.ageMeasureMethodId == 0)
                        a.ageMeasureMethodId = 1;

                    Age aNewEdit = null;
                    if (a.ChangeTracker.State != ObjectState.Added)
                        aNewEdit = ctx.Age.Where(x => x.ageId == a.ageId).FirstOrDefault();

                    if (a.ChangeTracker.State == ObjectState.Deleted && aNewEdit != null)
                        ctx.DeleteObject(aNewEdit);
                    else
                    {
                        if (aNewEdit == null)
                            aNewEdit = new Age();

                        a.CopyEntityValueTypesTo(aNewEdit);
                        ctx.Age.ApplyChanges(aNewEdit);
                    }

                    ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);
                }
            }
        }


        private void SaveAnimalInfoReferences(SprattusContainer ctx, List<R_AnimalInfoReference> lstAnimalInfoReferences, int intAnimalInfoId)
        {
            foreach (var a in lstAnimalInfoReferences)
            {
                if (a.ChangeTracker.State != ObjectState.Unchanged)
                {
                    if (a.animalInfoId != intAnimalInfoId && a.ChangeTracker.State != ObjectState.Deleted)
                        a.animalInfoId = intAnimalInfoId;

                    R_AnimalInfoReference aNewEdit = null;
                    if (a.ChangeTracker.State != ObjectState.Added)
                        aNewEdit = ctx.R_AnimalInfoReference.Where(x => x.R_animalInfoReferenceId == a.R_animalInfoReferenceId).FirstOrDefault();
                    
                    if (a.ChangeTracker.State == ObjectState.Deleted && aNewEdit != null)
                        ctx.DeleteObject(aNewEdit);
                    else
                    {
                        if (aNewEdit == null)
                            aNewEdit = new R_AnimalInfoReference();

                        a.CopyEntityValueTypesTo(aNewEdit);
                        ctx.R_AnimalInfoReference.ApplyChanges(aNewEdit);
                    }

                    ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);
                }
            }
        }




        #endregion


        #region Delete private methods


        private void DeleteCruiseFromId(SprattusContainer ctx, int intCruiseId)
        {
            foreach (var t in ctx.Trip.Where(x => x.cruiseId == intCruiseId))
                DeleteTripFromId(ctx, t.tripId);

            var c = ctx.Cruise.Where(x => x.cruiseId == intCruiseId).FirstOrDefault();

            if (c != null)
                ctx.DeleteObject(c);
        }


        private void DeleteTripFromId(SprattusContainer ctx, int intTripId)
        {
            foreach (var s in ctx.Sample.Where(x => x.tripId == intTripId))
                DeleteSampleFromId(ctx, s.sampleId);

            var t = ctx.Trip.Where(x => x.tripId == intTripId).FirstOrDefault();

            if (t != null)
                ctx.DeleteObject(t);
        }


        private void DeleteSampleFromId(SprattusContainer ctx, int intSampleId)
        {
            foreach (var sl in ctx.SpeciesList.Where(x => x.sampleId == intSampleId))
                DeleteSpeciesListFromId(ctx, sl.speciesListId);

            var s = ctx.Sample.Where(x => x.sampleId == intSampleId).FirstOrDefault();

            if (s != null)
                ctx.DeleteObject(s);
        }


        private void DeleteSpeciesListFromId(SprattusContainer ctx, int intSpeciesListId)
        {
            foreach (var sub in ctx.SubSample.Where(x => x.speciesListId == intSpeciesListId))
                DeleteSubSamplesFromId(ctx, sub.subSampleId);

            //Delete subSampleId
            var sln = ctx.SpeciesList.Where(x => x.speciesListId == intSpeciesListId).FirstOrDefault();
            if (sln != null)
                ctx.DeleteObject(sln);
        }


        private void DeleteSubSamplesFromId(SprattusContainer ctx, int intSubSampleId)
        {
            DeleteAnimalsFromSubSampleId(ctx, intSubSampleId);

            //Delete subSampleId
            var sub = ctx.SubSample.Where(x => x.subSampleId == intSubSampleId).FirstOrDefault();
            if (sub != null)
                ctx.DeleteObject(sub);
        }


        private void DeleteAnimalsFromSubSampleId(SprattusContainer ctx, int intSubSampleId)
        {
            //Delete animal infos references
            var animalInfoRefs = ctx.R_AnimalInfoReference.Where(x => x.AnimalInfo.Animal.subSampleId == intSubSampleId);

            foreach (var a in animalInfoRefs)
                ctx.DeleteObject(a);

            //Delete animal infos
            var animalInfos = ctx.AnimalInfo.Where(x => x.Animal.subSampleId == intSubSampleId);

            foreach (var a in animalInfos)
                ctx.DeleteObject(a);

            //Delete ages
            var ages = ctx.Age.Where(x => x.Animal.subSampleId == intSubSampleId);

            foreach (var a in ages)
                ctx.DeleteObject(a);

            //Delete animal files
            var animalFiles = ctx.AnimalFile.Where(x => x.Animal.subSampleId == intSubSampleId);

            foreach (var af in animalFiles)
                ctx.DeleteObject(af);

            //Delete animals
            var animals = ctx.Animal.Where(x => x.subSampleId == intSubSampleId);

            foreach (var a in animals)
                ctx.DeleteObject(a);
        }


        #endregion



        public DatabaseOperationResult SaveCruiseToDataWarehouse(byte[] arrCruise, byte[] arrMessages, bool blnDeleteCruiseBeforeInsert, ref List<Babelfisk.Warehouse.DWMessage> lstMessagesNew)
        {
            try
            {
                //Make sure no memory is held up.
                GC.Collect();

                var arr = arrCruise.Decompress();
                var cruise = arr.ToObjectDataContract<Babelfisk.Warehouse.Model.Cruise>();
                arr = null;

                //Make sure arr is collected
                GC.Collect();

                var arrM = arrMessages.Decompress();
                var lstMessages = arrM.ToObjectDataContract<List<Babelfisk.Warehouse.DWMessage>>();
                arrM = null;

                //Make sure arrM is collected
                GC.Collect();

                Babelfisk.Warehouse.DataManager datMan = new Warehouse.DataManager();
                datMan.InsertCruise(cruise, lstMessages, blnDeleteCruiseBeforeInsert);

                //Add any new errors.
                lstMessagesNew.AddRange(datMan.NewMessages);

                datMan = null;
                cruise = null;

                //Make sure cruise collected
                GC.Collect();

                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in SaveCruiseToDataWarehouse with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in SaveCruiseToDataWarehouse with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SaveCruiseToDataWarehouse with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }



        private void AddCruiseBySubSampleIdToDataWarehouseTransferQueue(SprattusContainer ctx, int intSubSampleId)
        {
            int? intCruiseId = null;
            try
            {
                var q = (from s in ctx.SubSample
                         where s.subSampleId == intSubSampleId
                         select s.SpeciesList.Sample.Trip.cruiseId);

                if (q.Count() == 1)
                    intCruiseId = q.FirstOrDefault();
            }
            catch (Exception e)
            {
                try { Anchor.Core.Loggers.Logger.LogError(e); }
                catch { }
                return;
            }

            if (intCruiseId.HasValue)
                AddCruiseToDataWarehouseTransferQueue(intCruiseId.Value);
        }


        private void AddCruiseBySampleIdToDataWarehouseTransferQueue(SprattusContainer ctx, int intSampleId)
        {
            int? intCruiseId = null;
            try
            {
                var q = (from s in ctx.Sample
                         where s.sampleId == intSampleId
                         select s.Trip.cruiseId);

                if (q.Count() == 1)
                    intCruiseId = q.FirstOrDefault();
            }
            catch (Exception e)
            {
                try { Anchor.Core.Loggers.Logger.LogError(e); } catch { }
                return;
            }

            if(intCruiseId.HasValue)
                AddCruiseToDataWarehouseTransferQueue(intCruiseId.Value);
        }


        private void AddCruiseByTripIdToDataWarehouseTransferQueue(SprattusContainer ctx, int intTripId)
        {
            int? intCruiseId = null;
            try
            {
                var q = (from s in ctx.Trip
                         where s.tripId == intTripId
                         select s.cruiseId);

                if (q.Count() == 1)
                    intCruiseId = q.FirstOrDefault();
            }
            catch (Exception e)
            {
                try { Anchor.Core.Loggers.Logger.LogError(e); }
                catch { }
                return;
            }

            if (intCruiseId.HasValue)
                AddCruiseToDataWarehouseTransferQueue(intCruiseId.Value);
        }




        private void AddCruiseToDataWarehouseTransferQueue(int intCruiseId)
        {
            try
            {
                Babelfisk.Warehouse.DataManager datMan = new Warehouse.DataManager();
                datMan.AddCruiseIdToTransferQueue(intCruiseId);
            }
            catch (Exception e)
            {
                try
                {
                    Anchor.Core.Loggers.Logger.LogError(e);
                }
                catch { }
            }
        }


        /*
           -- Below scripts makes sure the server has xp_cmdshell enabled
           -- To allow advanced options to be changed.
           EXEC sp_configure 'show advanced options', 1
           GO
           -- To update the currently configured value for advanced options.
           RECONFIGURE
           GO
           -- To enable the feature.
           EXEC sp_configure 'xp_cmdshell', 1
           GO
           -- To update the currently configured value for this feature.
           RECONFIGURE
           GO
        */
        /// <summary>
        /// Synchronize otolith file paths
        /// </summary>
        public DatabaseOperationResult RunFileSynchronizer()
        {
            var datRes = DatabaseOperationResult.CreateSuccessResult();
            try
            {
                var strFileSynchronizerJobName = System.Configuration.ConfigurationManager.AppSettings["DBServerFileSynchronizerJobName"] == null ? null : System.Configuration.ConfigurationManager.AppSettings["DBServerFileSynchronizerJobName"].ToString();

                if (!string.IsNullOrEmpty(strFileSynchronizerJobName))
                {
                    using (SprattusContainer ctx = new SprattusContainer())
                    {
                        ctx.Connection.Open();

                      //  string strCmd = "EXEC xp_cmdshell '\"{0}\"'";
                        // int iRows = ctx.ExecuteStoreCommand(string.Format(strCmd, strFileSynchronizerPath));
                        string strCmd = string.Format("EXEC [dbo].[StartAgentJobAndWait] @job = N'{0}', @maxwaitmins = 10", strFileSynchronizerJobName);
                        int iRows = ctx.ExecuteStoreCommand(strCmd);
                    }
                }
                else
                {
                    datRes = new DatabaseOperationResult(DatabaseOperationStatus.ValidationError, "FileSynchronizer file path was not found in config file.");
                }
            }
            catch (Exception e)
            {
                datRes = new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, e.Message);
            }

            return datRes;
        }
    }
}