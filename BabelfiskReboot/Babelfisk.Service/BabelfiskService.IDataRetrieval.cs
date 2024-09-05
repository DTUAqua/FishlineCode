using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using System.Data.Objects;
using Babelfisk.Entities;
using System.Text;

namespace Babelfisk.Service
{
    public partial class BabelfiskService : IDataRetrieval
    {

        #region Tree view methods


        public List<L_Year> GetTreeViewYears()
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var years = from c in ctx.Cruise
                          group c by c.year into gYears
                          select new { Year = gYears.Key, Cruises = gYears };

                var lst = years.Select(x => new L_Year() { Year =x.Year, CruiseCount = x.Cruises.Count()}).ToList();

                return lst;
            }
        }


        public int GetTreeViewTripCount(int intCruiseId, string strTripType)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                string strLowerTripType = null;
                bool blnUseTripType = !string.IsNullOrEmpty(strTripType);

                if (blnUseTripType)
                    strLowerTripType = strTripType.ToLower();

                var trips = (from t in ctx.Trip
                             where t.cruiseId == intCruiseId && (!blnUseTripType || t.tripType.Equals(strLowerTripType))
                             select t);

                return trips.Count();
            }
        }


        public Dictionary<int, int> GetTreeViewTripCounts(int[] CruiseIds, string strTripType)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                //Hash set for selecting out specific cruises.
                var hset = CruiseIds.Distinct().ToHashSet();

                string strLowerTripType = null;
                bool blnUseTripType = !string.IsNullOrEmpty(strTripType);

                if (blnUseTripType)
                    strLowerTripType = strTripType.ToLower();

                //Select cruises and their trip counts
                var cruises = from c in ctx.Cruise.Where(x => hset.Contains(x.cruiseId))
                              select new { CruiseId = c.cruiseId, TripCount = c.Trip.Where(t => (!blnUseTripType || t.tripType.Equals(strLowerTripType))).Count() };
               
                Dictionary<int, int> dic = cruises.ToDictionary(x => x.CruiseId, y => y.TripCount);

                return dic;
            }
        }


        public List<Cruise> GetTreeViewCruises(int intYear, string strTripType)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                string strLowerTripType = null;
                bool blnUseTripType = !string.IsNullOrEmpty(strTripType);

                if (blnUseTripType)
                    strLowerTripType = strTripType.ToLower();

                var cruises = (from c in ctx.Cruise
                          where c.year == intYear
                          select new { Cruise = c, TripsCount = c.Trip.Where(t => !blnUseTripType || t.tripType.Equals(strLowerTripType)).Count() }).ToList();

                cruises.ForEach(x => x.Cruise.TripCount = x.TripsCount);


                return cruises.Select(x => x.Cruise).ToList();
            }
        }


        public List<Trip> GetTreeViewTrips(int intCruiseId)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var trips = (from t in ctx.Trip
                             where t.cruiseId == intCruiseId
                             select new { Trip = t, SampleCount = t.Sample.Count, SpeciesListCount = t.Sample.SelectMany(x => x.SpeciesList).Count() }).ToList();

                trips.ForEach(t => 
                { 
                    t.Trip.SampleCount = t.SampleCount;
                    t.Trip.SpeciesListCount = t.SpeciesListCount;

                });

                return trips.Select(x => x.Trip).ToList();
            }
        }


        public List<Sample> GetTreeViewSamples(int intTripId)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var samples = (from s in ctx.Sample
                             where s.tripId == intTripId
                             select new { Sample = s, SpeciesListCount = s.SpeciesList.Count() }).ToList();

                samples.ForEach(x => x.Sample.SpeciesListCount = x.SpeciesListCount);


                return samples.Select(x => x.Sample).ToList();
            }
        }


        public List<SubSample> GetTreeViewSubSample(int intSampleId)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var res = ctx.ExecuteFunction<SIA_GetSampleGrandChildren_Result>("SIA_GetSampleGrandChildren", new ObjectParameter("strSampleId", intSampleId.ToString()));

                var lst = res.Select(x => new SubSample() { landingWeight = x.landingWeight,
                                                            representative = x.representative,
                                                            speciesListId = x.speciesListId,
                                                            stepNum = x.stepNum,
                                                            subSampleId = x.subSampleId,
                                                            subSampleWeight = x.subSampleWeight}).ToList();

                //Load Animal list and SpeciesList properties on subsample
                foreach (var subsample in lst)
                {
                    ctx.AttachTo("SubSample", subsample);
                    ctx.LoadProperty(subsample, "Animal");
                    ctx.LoadProperty(subsample, "SpeciesList");
                }

                return lst;
            }
        }


        #endregion


        public List<SpeciesList> GetSpeciesLists(int intSampleId)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var sl = (
                          from s in ctx.SpeciesList
                                       .Include("SubSample")
                                       .Include("SubSample.Animals")
                          where s.sampleId == intSampleId
                          select new { 
                                       SL = s, 
                                       SubSamples = s.SubSample, //Select SubSamples out as well to make sure s.SubSample is populated
                                       Animals = s.SubSample.SelectMany(x => x.Animal), //Select Animals out as well to make sure s.SubSample.Animal is populated
                                     }

                          ).ToList();

                return sl.Select(x => x.SL).ToList();
            }
        }

        
        public List<Animal> GetAnimals(int intSubSampleId)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var animals = (
                                  from a in ctx.Animal
                                  where a.subSampleId == intSubSampleId
                                  select a
                              ).ToList();


                return animals;
            }
        }


        public List<Age> GetAges(int intSubSampleId)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var ages = (
                                  from a in ctx.Age
                                  where a.Animal.subSampleId == intSubSampleId
                                  select a
                              ).ToList();


                return ages;
            }
        }


        public List<AnimalFile> GetAnimalFiles(int intSubSampleId)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var files = ctx.AnimalFile
                               .Where(a => a.Animal.subSampleId == intSubSampleId)
                               .ToList();

                return files;
            }
        }


        public List<AnimalInfo> GetAnimalInfos(int intSubSampleId)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var ai = (
                                  from a in ctx.AnimalInfo
                                  where a.Animal.subSampleId == intSubSampleId
                                  select a
                              ).ToList();


                return ai;
            }
        }


        public List<R_AnimalInfoReference> GetAnimalInfoReferences(int intSubSampleId)
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var ai = (from r in ctx.R_AnimalInfoReference
                          where r.AnimalInfo.Animal.subSampleId == intSubSampleId
                          select r).ToList();

                return ai;
            }
        }



        #region Data Export methods


        public byte[] GetDataForRaising(List<int> lstCruiseIdsWithNoSamples, List<int> lstTripIdsWithNoSamples, List<int> lstSampleIds)
        {
            //Make sure no memory is held up.
            GC.Collect();

            List<Cruise> lstCruises = new List<Cruise>();
            using (var ctx = new SprattusContainer())
            {
                ctx.CommandTimeout = 5 * 60;
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                //Select samples
                var samples = ctx.Sample.Include("Trip")
                                  .Include("Trip.Cruise")
                                  .Include("R_TargetSpecies")
                                  .Include("SpeciesList")
                                  .Include("SpeciesList.SubSample")
                                  .Include("SpeciesList.SubSample.Animal")
                                  .Include("SpeciesList.SubSample.Animal.Age")
                                  .Include("SpeciesList.SubSample.Animal.AnimalFiles")
                                  .Include("SpeciesList.SubSample.Animal.AnimalInfo")
                                  .Include("SpeciesList.SubSample.Animal.AnimalInfo.R_AnimalInfoReference")
                                  .Include("SpeciesList.SubSample.Animal.AnimalInfo.Maturity")
                                  .Include("SpeciesList.SubSample.Animal.AnimalInfo.Fat")
                                  .Where(x => lstSampleIds.Contains(x.sampleId)).ToList();

                lstCruises.AddRange(samples.Select(x => x.Trip.Cruise).DistinctBy(x => x.cruiseId));

                //Add trips with no samples
                if (lstTripIdsWithNoSamples.Count > 0)
                {
                    //Load trips with no samples. The trips are automatically added to the correct existing cruises in lstCruises, because it is in the same context.
                    var trips = ctx.Trip.Include("Cruise")
                                        .Where(x => lstTripIdsWithNoSamples.Contains(x.tripId)).ToList();

                    //Add missing cruises to lstCruises
                    foreach (var c in trips.Select(x => x.Cruise).DistinctBy(x => x.cruiseId))
                        if (!lstCruises.Where(x => x.cruiseId == c.cruiseId).Any())
                            lstCruises.Add(c);
                }

                //Add cruises with no trips
                if (lstCruiseIdsWithNoSamples.Count > 0)
                {
                    var cruises = ctx.Cruise.Where(x => lstCruiseIdsWithNoSamples.Contains(x.cruiseId)).ToList();
                    foreach(var c in cruises)
                        if (!lstCruises.Where(x => x.cruiseId == c.cruiseId).Any())
                            lstCruises.Add(c);
                }
            }

            byte[] arr = lstCruises.ToByteArrayDataContract();
            arr = arr.Compress();

            lstCruises = null;

            //Make sure no memory is held up.
            GC.Collect();

            return arr;
        }


        #endregion


        public Cruise GetCruise(int year, string cruiseName)
        {
            Cruise c = null;
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                c = ctx.Cruise.Where(x => x.year == year && x.cruise1.Equals(cruiseName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            }

            return c;
        }


        public byte[] GetStationsForDataImport(int cruiseId, string trip, List<string> stations)
        {
            byte[] arr = null;

            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                Trip t = ctx.Trip.Where(x => x.cruiseId == cruiseId && x.trip1.Equals(trip, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                if(t != null)
                {
                    var tripId = t.tripId;
                    string[] arrStations = stations.ToArray();

                    //the contains method below does not need and ignore case, since the collation on the database is CI (case insensitive).
                    var stati = ctx.Sample.Include("Trip")
                                     //     .Include("Trip.Cruise")
                                          .Include("SpeciesList")
                                          .Include("SpeciesList.SubSample")
                                          .Include("SpeciesList.SubSample.Animal")
                                          .Where(x => x.tripId == tripId && arrStations.Contains(x.station))
                                          .ToList();

                    var arrList = stati.ToByteArrayDataContract();
                    arr = arrList.Compress();
                }
            }

            return arr;
        }

    }
}