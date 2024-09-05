using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.Sprattus;
using System.Data.Objects;
using System.Configuration;
using System.Data.EntityClient;
using Babelfisk.Entities;
using System.Collections;

namespace Babelfisk.DataTransfer
{
    internal class DataManager
    {

        public Cruise GetDataForRaising(int intCruiseId, out bool blnExceptionThrown)
        {
            blnExceptionThrown = false;
            Cruise c = null;
            try
            {
                //Make sure no memory is held up.
                GC.Collect();

                using (var ctx = EntityHelper<SprattusContainer>.CreateInstance())
                {
                    ctx.Connection.Open();
                    ctx.CommandTimeout = 60 * 10; //10 minuttes
                    ApplyTransactionIsolationLevel(ctx, System.Transactions.IsolationLevel.ReadUncommitted);

                    c = ctx.Cruise.Include("Trip")
                                  .Include("Trip.Sample")
                                  .Include("Trip.Sample.R_TargetSpecies")
                                  .Include("Trip.Sample.SpeciesList")
                                  .Include("Trip.Sample.SpeciesList.SubSample")
                                  .Include("Trip.Sample.SpeciesList.SubSample.Animal")
                                  .Include("Trip.Sample.SpeciesList.SubSample.Animal.Age")
                                  .Include("Trip.Sample.SpeciesList.SubSample.Animal.AnimalFiles")
                                  .Include("Trip.Sample.SpeciesList.SubSample.Animal.AnimalInfo")
                                  .Include("Trip.Sample.SpeciesList.SubSample.Animal.AnimalInfo.R_AnimalInfoReference")
                                  .Include("Trip.Sample.SpeciesList.SubSample.Animal.AnimalInfo.Maturity")
                                  .Include("Trip.Sample.SpeciesList.SubSample.Animal.AnimalInfo.Fat")
                                  .Where(x => x.cruiseId == intCruiseId).FirstOrDefault();
                }

                return c;
            }
            catch (Exception e)
            {
                LogError(e);
                blnExceptionThrown = true;
            }

            return c;
        }


        public bool HasCruise(int intCruiseId, out bool blnExceptionThrown)
        {
            bool blnRes = false;
            blnExceptionThrown = false;

            try
            {
                using (var ctx = EntityHelper<SprattusContainer>.CreateInstance())
                {
                    ctx.Connection.Open();
                    ApplyTransactionIsolationLevel(ctx, System.Transactions.IsolationLevel.ReadUncommitted);

                    blnRes = ctx.Cruise.Where(x => x.cruiseId == intCruiseId).Any();
                }
            }
            catch (Exception e)
            {
                LogError(e);
                blnExceptionThrown = true;
            }

            return blnRes;
        }


       

        public List<int> GetCruiseIdsToTransfer()
        {
            List<int> lst = new List<int>();

            Babelfisk.Warehouse.DataManager datMan = new Warehouse.DataManager();

            try
            {
                lst = datMan.GetAllCruiseIdsFromTransferQueue();
            }
            catch (Exception e)
            {
                LogError(e);
              
            }

            return lst;
        }


        public static void LogError(Exception e, int? intCruiseId = null)
        {
            Babelfisk.Warehouse.DataManager datMan = new Warehouse.DataManager();

            Console.WriteLine("Error: " + (e.Message == null ? "" : e.Message)  + " | " + (e.StackTrace == null ? "" : e.StackTrace));
            try
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
            catch { }

            try
            {
                Warehouse.DWMessage dw = Warehouse.DWMessage.NewError(intCruiseId, "DataTransfer->DataManager->LogError()", "", "", (e.Message == null ? "" : e.Message) + " | " + (e.StackTrace == null ? "" : e.StackTrace));
                datMan.SaveMessages(new List<Warehouse.DWMessage>() { dw }, false);
            }
            catch { }
        }


        public static void LogError(string strErrorMessage, int? intCruiseId = null)
        {
            Babelfisk.Warehouse.DataManager datMan = new Warehouse.DataManager();

            try
            {
                Anchor.Core.Loggers.Logger.Log(Anchor.Core.Loggers.LogType.Error, strErrorMessage);
            }
            catch { }

            try
            {
                Warehouse.DWMessage dw = Warehouse.DWMessage.NewError(intCruiseId, "DataTransfer->DataManager->LogError()", "", "", strErrorMessage);
                datMan.SaveMessages(new List<Warehouse.DWMessage>() { dw }, false);
            }
            catch { }
        }


        public static void LogWarning(string strInfoMessage, int? intCruiseId = null)
        {
            Babelfisk.Warehouse.DataManager datMan = new Warehouse.DataManager();

            try
            {
                Anchor.Core.Loggers.Logger.Log(Anchor.Core.Loggers.LogType.Info, strInfoMessage);
            }
            catch { }

            try
            {
                Warehouse.DWMessage dw = Warehouse.DWMessage.NewWarning(intCruiseId, "DataTransfer->DataManager->LogInfo()", "", "", strInfoMessage);
                datMan.SaveMessages(new List<Warehouse.DWMessage>() { dw }, false);
            }
            catch { }
        }


        public static class EntityHelper<T> where T : ObjectContext
        {
            public static T CreateInstance()
            {
                // get the connection string from config file
                string connectionString = ConfigurationManager.ConnectionStrings[typeof(T).Name].ConnectionString;

                // parse the connection string
                var csBuilder = new EntityConnectionStringBuilder(connectionString);

                // replace * by the full name of the containing assembly
                csBuilder.Metadata = csBuilder.Metadata.Replace(
                    "res://*/",
                    string.Format("res://{0}/", typeof(Babelfisk.Model.ModelClass).Assembly.FullName));

                // return the object
                return Activator.CreateInstance(typeof(T), csBuilder.ToString()) as T;
            }
        }


        /// <summary>
        /// Retrieve treatment factors from FishLine.
        /// </summary>
        public List<Babelfisk.Entities.Sprattus.TreatmentFactor> GetTreatmentFactors()
        {
            List<Babelfisk.Entities.Sprattus.TreatmentFactor> lstTreatmentFactors = null;

            try
            {
               lstTreatmentFactors = GetLookups<Babelfisk.Entities.Sprattus.TreatmentFactor>();
            }
            catch (Exception e)
            {
                DataManager.LogError(e);
            }

            return lstTreatmentFactors;
        }


        /// <summary>
        /// Retrieve stocks from FishLine.
        /// </summary>
        public List<Babelfisk.Entities.Sprattus.L_Stock> GetStocks()
        {
            List<Babelfisk.Entities.Sprattus.L_Stock> lstStocks = null;

            try
            {
                lstStocks = GetLookups<Babelfisk.Entities.Sprattus.L_Stock>();
            }
            catch (Exception e)
            {
                DataManager.LogError(e);
            }

            return lstStocks;
        }


        /// <summary>
        /// Retrieve species area stock relations from FishLine.
        /// </summary>
        public List<Babelfisk.Entities.Sprattus.R_StockSpeciesArea> GetSpeciesAreaStocks()
        {
            List<Babelfisk.Entities.Sprattus.R_StockSpeciesArea> lstSpeciesAreaStocks = null;

            try
            {
                lstSpeciesAreaStocks = GetLookups<Babelfisk.Entities.Sprattus.R_StockSpeciesArea>();
            }
            catch (Exception e)
            {
                DataManager.LogError(e);
            }

            return lstSpeciesAreaStocks;
        }



        public List<T> GetLookups<T>(params string[] includes)
        {
            var lst = GetLookups(typeof(T).AssemblyQualifiedName, includes);

            if (lst == null)
                return null;

            return lst.OfType<T>().ToList();
        }



        private List<Babelfisk.Entities.ILookupEntity> GetLookups(string strEntityType, string[] includes)
        {
            List<Babelfisk.Entities.ILookupEntity> lst = null;
            Type t = null;
            try
            {
                t = Type.GetType(strEntityType);

                using (var ctx = EntityHelper<SprattusContainer>.CreateInstance())
                {
                    ctx.Connection.Open();
                    ApplyTransactionIsolationLevel(ctx, System.Transactions.IsolationLevel.ReadUncommitted);

                    //Get platform the quickest way as possible (in this case by the manual query below)
                    if (t.Name == "L_Platform" && (includes == null || includes.Length == 0))
                    {
                        lst = ctx.ExecuteStoreQuery<L_Platform>("SELECT L_platformId, platform, platformType, name, nationality, boatIdentity, contactPersonId, description "
                                                                 + "FROM L_Platform").OfType<ILookupEntity>().ToList();
                    }
                    else
                    {
                        var objectSetLookups = ctx.GetType().GetProperties()
                                                  .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericArguments()[0].Name == t.Name)
                                                  .Select(p => p.GetValue(ctx, null) as IEnumerable).First();

                        var query = objectSetLookups;

                        int i = 0;
                        while (includes != null && i < includes.Length)
                            query = ((dynamic)query).Include(includes[i++]);

                        lst = query.OfType<ILookupEntity>().ToList();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return lst;
        }



        public static void ApplyTransactionIsolationLevel(ObjectContext ctx, System.Transactions.IsolationLevel isoLevel)
        {
            //Set default transaction level
            string str = "SERIALIZABLE";

            switch (isoLevel)
            {
                case System.Transactions.IsolationLevel.ReadUncommitted:
                    str = "READ UNCOMMITTED";
                    break;

                case System.Transactions.IsolationLevel.ReadCommitted:
                    str = "READ COMMITTED";
                    break;

                case System.Transactions.IsolationLevel.Serializable:
                    str = "SERIALIZABLE";
                    break;

                case System.Transactions.IsolationLevel.RepeatableRead:
                    str = "REPEATABLE READ";
                    break;

                case System.Transactions.IsolationLevel.Snapshot:
                    str = "SNAPSHOT";
                    break;
            }

            str = "SET TRANSACTION ISOLATION LEVEL " + str;

            ctx.ExecuteStoreCommand(str);
        }

    }
}
