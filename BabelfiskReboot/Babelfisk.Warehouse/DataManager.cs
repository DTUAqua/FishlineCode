using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Data.SqlClient;
using Babelfisk.Warehouse.Model;
using Anchor.Core;
using Babelfisk.Entities;
using System.Data;
using System.ComponentModel;
using System.Globalization;

namespace Babelfisk.Warehouse
{
    public class DataManager
    {
        private List<DWMessage> _lstNewMessages;

        private string _strConnectionString = null;


        public List<DWMessage> NewMessages
        {
            get { return _lstNewMessages; }
        }

        /// <summary>
        /// Insert cruise to data warehouse.
        /// Exceptions are not handled!
        /// </summary>
        /// <param name="cruise"></param>
        /// <param name="blnDeleteCruiseBeforeInsert"></param>
        public void InsertCruise(Model.Cruise cruise, List<DWMessage> lstMessages, bool blnDeleteCruiseBeforeInsert = false)
        {
            _lstNewMessages = new List<DWMessage>();

            using (var ctx = new Model.DataWarehouseContext())
            {
                //Get connection string, before opening connection.
                _strConnectionString = (ctx.Connection as System.Data.EntityClient.EntityConnection).StoreConnection.ConnectionString;
                //Debug output (when using PAssword in the connection string, it is removed from the string after ctx.Connection.Open() unless Persist Security Info=True is set.
                //string str = (ctx.Connection as System.Data.EntityClient.EntityConnection).StoreConnection.ConnectionString;
                //Console.WriteLine(str);
                //Anchor.Core.Loggers.Logger.Log(Anchor.Core.Loggers.LogType.Info, str);

                ctx.Connection.Open();
               // using (TransactionScope scope = new TransactionScope())
                {
                    //Simple add a cruise by deleting any old cruise-data first and inserting the new cruise
                    if (blnDeleteCruiseBeforeInsert)
                    {
                        DeleteCruiseAndAssociatedData(ctx, cruise.cruiseId);
                        InsertCruise(ctx, cruise);
                    }
                    //Synchronize cruise by overwriting any exisiting cruise and/or trip data. Delete any samples
                    //before adding new ones though.
                    else
                    {
                        SynchronizeCruise(ctx, cruise);
                    }

                    if (lstMessages != null && lstMessages.Count > 0)
                        SaveMessages(ctx, lstMessages, blnDeleteCruiseBeforeInsert);
                    else if (blnDeleteCruiseBeforeInsert) //MAke sure error logs are cleared, even though there are no new messages to insert.
                        DeleteMessageForCruise(ctx, cruise.cruiseId);

                    ctx.SaveChanges();
                    //scope.Complete();
                }
            }
        }

        /// <summary>
        /// Synchronize species in supplied dictionary to warehouse.
        /// </summary>
        public void SynchronizeSpecies(Dictionary<string, Entities.Sprattus.L_Species> dic)
        {
            using (var ctx = new Model.DataWarehouseContext())
            {
                ctx.Connection.Open();

                var dicDW = ctx.L_Species.ToDictionary(x => x.speciesCode);

                //Run through and remove species not in FishLine anymore
                foreach (var sDW in dicDW.Values.ToList())
                {
                    if (!dic.ContainsKey(sDW.speciesCode))
                    {
                        ctx.DeleteObject(sDW);
                        dicDW.Remove(sDW.speciesCode);
                    }
                }

                ctx.SaveChanges();

                string[] arrOmittedProperties = new string[] {"Est_MethodStep", "R_SpeciesStock", "R_TargetSpecies", "R_TargetSpecies1", "SpeciesList", "ChangeTracker", "Id", "FilterValue", "treatmentFactorGroupUI", "DefaultSortValue", "UIDisplay", "CompareValue"};
                //Add or update species from FishLine
                foreach (var s in dic.Values)
                {
                    if (dicDW.ContainsKey(s.speciesCode))
                    {
                        //Update species
                        var sDW = dicDW[s.speciesCode];
                        if (s.CopyValueTypesTo(sDW, arrOmittedProperties))
                        {
                            sDW.MarkAsModified();
                            ctx.L_Species.ApplyChanges(sDW);
                        }
                    }
                    else
                    {
                        //Insert new species
                        var sDW = new L_Species();
                        if (s.CopyValueTypesTo(sDW, arrOmittedProperties))
                        {
                            ctx.L_Species.AddObject(sDW);
                        }
                    }
                }

                ctx.SaveChanges();
            }
        }



        public List<int> GetAllCruiseIdsFromTransferQueue()
        {
            List<int> lst = new List<int>();

            using (var ctx = new Model.DataWarehouseContext())
            {
                lst = ctx.CruisesToTransfers.Select(x => x.cruiseId).ToList();
            }

            return lst;
        }



        public void TruncateTransferQueue(List<int> lstCruiseIds)
        {
            using (var ctx = new Model.DataWarehouseContext())
            {
                if (lstCruiseIds != null)
                {
                    foreach (var c in ctx.CruisesToTransfers.Where(x => lstCruiseIds.Contains(x.cruiseId)))
                        ctx.DeleteObject(c);
                }
                else
                {
                    foreach (var c in ctx.CruisesToTransfers)
                        ctx.DeleteObject(c);
                }

                ctx.SaveChanges();
            }
        }



        public void AddCruiseIdToTransferQueue(int intCruiseId)
        {
            using (var ctx = new Model.DataWarehouseContext())
            {
                var cExisting = ctx.CruisesToTransfers.Where(x => x.cruiseId == intCruiseId).FirstOrDefault();

                if (cExisting != null)
                {
                    cExisting.addedDateTime = DateTime.UtcNow;
                    ctx.CruisesToTransfers.ApplyChanges(cExisting);
                }
                else
                {
                    var ctt = new Model.CruisesToTransfer();
                    ctt.cruiseId = intCruiseId;
                    ctt.addedDateTime = DateTime.UtcNow;
                    ctx.CruisesToTransfers.ApplyChanges(ctt);
                }

                ctx.SaveChanges();
            }
        }


        public void AddCruiseIdsToTransferQueue(List<int> lstCruiseIds)
        {
            using (var ctx = new Model.DataWarehouseContext())
            {
                var dicExisting = ctx.CruisesToTransfers.Where(x => lstCruiseIds.Contains(x.cruiseId)).ToDictionary(x => x.cruiseId);
                Warehouse.Model.CruisesToTransfer cExisting = null;

                foreach (var intCruiseId in lstCruiseIds)
                {
                    if (dicExisting.ContainsKey(intCruiseId))
                        cExisting = dicExisting[intCruiseId];

                    if (cExisting != null)
                    {
                        cExisting.addedDateTime = DateTime.UtcNow;
                        ctx.CruisesToTransfers.ApplyChanges(cExisting);
                    }
                    else
                    {
                        var ctt = new Babelfisk.Warehouse.Model.CruisesToTransfer();
                        ctt.cruiseId = intCruiseId;
                        ctt.addedDateTime = DateTime.UtcNow;
                        ctx.CruisesToTransfers.ApplyChanges(ctt);
                    }

                    cExisting = null;
                }

                ctx.SaveChanges();
            }
        }



        public void SaveMessages(List<DWMessage> lstMessages, bool blnDeleteByCruiseIdBeforeInsert)
        {
            using (var ctx = new Model.DataWarehouseContext())
            {
                SaveMessages(ctx, lstMessages, blnDeleteByCruiseIdBeforeInsert);
            }
        }

        private void DeleteMessageForCruise(Model.DataWarehouseContext ctx, int cruiseId)
        {
            ctx.ExecuteStoreCommand(@"DELETE te
                                    FROM  ErrorLog te
                                    WHERE te.cruiseId = @cruiseId
                                    ", new SqlParameter("@cruiseId", cruiseId));
        }

        private void SaveMessages(Model.DataWarehouseContext ctx, List<DWMessage> lstMessages, bool blnDeleteByCruiseIdBeforeInsert)
        {
            if (lstMessages == null || lstMessages.Count == 0)
                return;

            var lstTransferErrors = lstMessages.Select(x => new Model.ErrorLog()
            {
                errorType = x.MessageType.ToString(),
                logTime = DateTime.UtcNow,
                cruiseId = x.CruiseId,
                origin = x.Origin,
                recordType = x.RecordType,
                recordTypeId = x.RecordTypeId,
                description = x.Description
            });

            //Delete previous errors for current transferred cruise.
            if (blnDeleteByCruiseIdBeforeInsert)
            {
                foreach (var intCruiseId in lstMessages.Where(x => x.CruiseId.HasValue).Select(x => x.CruiseId.Value).Distinct())
                {
                    DeleteMessageForCruise(ctx, intCruiseId);
                }
            }

            List<ColumnInfo> lst = GetColumnInformation(ctx, typeof(ErrorLog).Name);
            BulkInsert(ctx, typeof(ErrorLog).Name, lst, lstTransferErrors);
        }



        #region Insert and Synchronize methods for warehouse data


        /// <summary>
        /// Insert a whole cruise structure assuming data does not already exist in database.
        /// </summary>
        private void InsertCruise(Model.DataWarehouseContext ctx, Model.Cruise cruise)
        {
            List<ColumnInfo> lstColumnNames = GetColumnInformation(ctx, typeof(Cruise).Name);
           
            BulkInsert(ctx, typeof(Cruise).Name, lstColumnNames, new Cruise[] { cruise });

            InsertTrip(ctx, cruise.Trips);
        }


        private void InsertTrip(Model.DataWarehouseContext ctx, IEnumerable<Trip> lstTrips)
        {
            if (lstTrips.Count() > 0)
            {
                var lstColumnNames = GetColumnInformation(ctx, typeof(Trip).Name);
                BulkInsert(ctx, typeof(Trip).Name, lstColumnNames, lstTrips);

                var lstSamples = lstTrips.SelectMany(x => x.Samples);

                InsertSamples(ctx, lstSamples);
            }
        }


        private void InsertSamples(Model.DataWarehouseContext ctx, IEnumerable<Sample> lstSamples)
        {
            if (lstSamples.Count() > 0)
            {
                var lstColumnNames = GetColumnInformation(ctx, typeof(Sample).Name);
                BulkInsert(ctx, typeof(Sample).Name, lstColumnNames, lstSamples);

                var lstSpeciesLists = lstSamples.SelectMany(x => x.SpeciesLists);
                if (lstSpeciesLists.Count() > 0)
                {
                    lstColumnNames = GetColumnInformation(ctx, typeof(SpeciesList).Name);
                    BulkInsert(ctx, typeof(SpeciesList).Name, lstColumnNames, lstSpeciesLists);

                    var lstAnimals = lstSpeciesLists.SelectMany(x => x.Animals);

                    if (lstAnimals.Count() > 0)
                    {
                        lstColumnNames = GetColumnInformation(ctx, typeof(Animal).Name);
                        BulkInsert(ctx, typeof(Animal).Name, lstColumnNames, lstAnimals);

                        var lstReferences = lstAnimals.SelectMany(x => x.R_AnimalReference);

                        if (lstReferences.Count() > 0)
                        {
                            lstColumnNames = GetColumnInformation(ctx, typeof(R_AnimalReference).Name);
                            BulkInsert(ctx, typeof(R_AnimalReference).Name, lstColumnNames, lstReferences);
                        }

                        var lstPictureReferences = lstAnimals.SelectMany(x => x.R_AnimalPictureReference);

                        if (lstPictureReferences.Count() > 0)
                        {
                            lstColumnNames = GetColumnInformation(ctx, typeof(R_AnimalPictureReference).Name);
                            BulkInsert(ctx, typeof(R_AnimalPictureReference).Name, lstColumnNames, lstPictureReferences);
                        }

                        var lstAges = lstAnimals.SelectMany(x => x.Ages);

                        if (lstAges.Count() > 0)
                        {
                            lstColumnNames = GetColumnInformation(ctx, typeof(Age).Name);
                            BulkInsert(ctx, typeof(Age).Name, lstColumnNames, lstAges);
                        }
                    }
                }

                var lstSpeciesListRaised = lstSamples.SelectMany(x => x.SpeciesListRaiseds);

                if (lstSpeciesListRaised.Count() > 0)
                {
                    lstColumnNames = GetColumnInformation(ctx, typeof(SpeciesListRaised).Name);
                    BulkInsert(ctx, typeof(SpeciesListRaised).Name, lstColumnNames, lstSpeciesListRaised);

                    var lstAnimalRaised = lstSpeciesListRaised.SelectMany(x => x.AnimalRaiseds);

                    if (lstAnimalRaised.Count() > 0)
                    {
                        lstColumnNames = GetColumnInformation(ctx, typeof(AnimalRaised).Name);
                        BulkInsert(ctx, typeof(AnimalRaised).Name, lstColumnNames, lstAnimalRaised);
                    }
                }
            }
        }



        private void SynchronizeCruise(Model.DataWarehouseContext ctx, Model.Cruise cruise)
        {
            Cruise cExisting = null;
            if ((cExisting = ctx.Cruises.Where(x => x.cruiseId == cruise.cruiseId).FirstOrDefault()) != null)
            {
                cruise.CopyEntityValueTypesTo(cExisting);
                ctx.Cruises.ApplyChanges(cExisting);
               
                foreach (var t in cruise.Trips)
                    SynchronizeTrip(ctx, t);

                ctx.SaveChanges();
                
                var lstSamples = cruise.Trips.SelectMany(x => x.Samples);
                InsertSamples(ctx, lstSamples);
            }
            else
            {
                InsertCruise(ctx, cruise);
            }
        }


        private void SynchronizeTrip(Model.DataWarehouseContext ctx, Model.Trip trip)
        {
            Trip tExisting = null;
            if ((tExisting = ctx.Trips.Where(x => x.tripId == trip.tripId).FirstOrDefault()) != null)
            {
                trip.CopyEntityValueTypesTo(tExisting);
                ctx.Trips.ApplyChanges(tExisting);
            }
            else
            {
                var lstColumnNames = GetColumnInformation(ctx, typeof(Trip).Name);
                BulkInsert(ctx, typeof(Trip).Name, lstColumnNames, new Model.Trip[] { trip});
            }

            foreach (var s in trip.Samples)
            {
                DeleteSampleAndAssociatedData(ctx, s.sampleId);
            }
        }


        /// <summary>
        /// Remember to have the "Distributed Transaction Coordinater"-service running for BulkInsert to work.
        /// </summary>
        internal void BulkInsert<T>(Model.DataWarehouseContext ctx, string strTableName, List<ColumnInfo> lstColumns, IEnumerable<T> lstEntities)
        {
            string strConnectionstring = _strConnectionString;

            if (strConnectionstring == null)
                strConnectionstring = ((System.Data.EntityClient.EntityConnection)ctx.Connection).StoreConnection.ConnectionString;

            using (var bulkCopy = new SqlBulkCopy(strConnectionstring, SqlBulkCopyOptions.KeepNulls & SqlBulkCopyOptions.KeepIdentity & SqlBulkCopyOptions.UseInternalTransaction))
            {
                bulkCopy.BatchSize = lstEntities.Count();
                bulkCopy.DestinationTableName = strTableName;

                string strDecimalTypeName = typeof(decimal).Name;
                string strNullableDecimalTypeName = typeof(Nullable<decimal>).FullName;
                string strStringTypeName = typeof(string).Name;

                var table = new DataTable();
                var props = TypeDescriptor.GetProperties(typeof(T))
                    //Only system datatypes - filter out the relationships/collections
                                           .Cast<PropertyDescriptor>()
                                           .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                           .ToArray();

                List<PropertyDescriptor> lstProperties = new List<PropertyDescriptor>();
                foreach (var cInfo in lstColumns)
                {
                    var strMappedColumn = MapColumn(strTableName, cInfo.ColumnName);
                    var propertyInfo = props.Where(x => x.Name.Equals(strMappedColumn, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (propertyInfo == null)
                    {
                        throw new ApplicationException(String.Format("Could not find column, when copying data from one {0} to another.", strTableName));
                    }

                    lstProperties.Add(propertyInfo);
                    bulkCopy.ColumnMappings.Add(cInfo.ColumnName, cInfo.ColumnName);
                    table.Columns.Add(cInfo.ColumnName, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                }

                var values = new object[lstProperties.Count];
                foreach (var item in lstEntities)
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        values[i] = lstProperties[i].GetValue(item);

                        //Make sure characters are not longer than whats defined for the field in the database.
                        if (values[i] != null && lstColumns[i].CharacterMaxLength.HasValue)
                        {
                            if (lstProperties[i].PropertyType.Name == strStringTypeName)
                            {
                                string str = values[i] as string;
                                if (str.Length > lstColumns[i].CharacterMaxLength.Value)
                                {
                                    DWMessage m = DWMessage.NewWarning(null, "Warehouse.BulkInsert ", strTableName, values[0].ToString(), String.Format("Strengen '{0}' i feltet '{1}' var for lang ({2} karakterer). Varehuset accepterer kun strenge af længden {3} for samme kolonne. Strengen er derfor blevet afkortet, så den kan indsættes varehuset.", str, lstColumns[i].ColumnName, str.Length, lstColumns[i].CharacterMaxLength.Value));
                                    ReportMessage(ctx, m);
                                    values[i] = str.Substring(0, lstColumns[i].CharacterMaxLength.Value);
                                }
                            }
                        }

                        //Test if decimal number is to large to fit in database column, and if se, throw an error and set the value to NULL.
                        //Example of error: If the sql column is numeric(6,3), the maximum value to be inserted is 999.999 or 999
                        //(1000 does for example not work, because that translates to 1000.000, which is > 6).
                        if (values[i] != null && lstColumns[i].NumericPrecision.HasValue)
                        {
                            decimal dec = 0;
                            if (lstProperties[i].PropertyType.Name == strDecimalTypeName)
                                dec = (decimal)values[i];
                            else if (lstProperties[i].PropertyType.FullName == strNullableDecimalTypeName)
                                dec = ((Nullable<decimal>)values[i]).Value;

                            if (dec != 0)
                            {
                                int intPrecision = lstColumns[i].NumericPrecision.Value;

                                if (lstColumns[i].NumericScale.HasValue)
                                {
                                    intPrecision -= lstColumns[i].NumericScale.Value;

                                    dec = (decimal)Math.Round(dec, lstColumns[i].NumericScale.Value);
                                }

                                decimal maxNumber = (decimal)Math.Pow(10, intPrecision);

                                if (Math.Abs(dec) >= maxNumber)
                                {
                                    //error - number is larger than what there is space for
                                    DWMessage m = DWMessage.NewError(null, "Warehouse.BulkInsert ", strTableName, values[0].ToString(), String.Format("Værdien '{0}' for feltet '{1}' var for stor. Varehuset accepterer kun værdier mindre end {2} for det pågældende felt. Værdien er derfor nulstillet til {3}.", dec.ToString(CultureInfo.InvariantCulture), lstColumns[i].ColumnName, maxNumber.ToString(CultureInfo.InvariantCulture), lstColumns[i].IsNullable == "YES" ? "NULL" : "0"));
                                    ReportMessage(ctx, m);

                                    if (lstColumns[i].IsNullable == "YES")
                                        values[i] = null;
                                    else
                                        values[i] = 0M;
                                }
                            }
                        }
                    }

                    table.Rows.Add(values);
                }

                try
                {
                    bulkCopy.WriteToServer(table);
                }
                catch (Exception e)
                {
                    DWMessage m = DWMessage.NewError(null, "Warehouse.BulkInsert ", strTableName, values[0].ToString(), String.Format("Det blev forsøgt at indsætte {0} {1}-rækker, men der opstod en fejl under indsættelse af en eller flere af rækkerne. Sidste række i listen har id {2}.", lstEntities.Count(), strTableName, (values == null || values.Length == 0) ? "N/A" : values[0].ToString()));
                    ReportMessage(ctx, m);
                    throw new ApplicationException(String.Format("An error occrured while saving a '{0}'-entity. ", strTableName) + e.Message);
                }
            }
        }


        private void ReportMessage(DataWarehouseContext ctx, DWMessage m)
        {
            _lstNewMessages.Add(m);
            SaveMessages(ctx, new List<DWMessage>() { m }, false);
        }


        /// <summary>
        /// Some columns needs to be mapped manually because of the entity model is created from the database, which sometimes results in column names with "1" in it
        /// (this happens when you have a column name which is the same as the table name)
        /// </summary>
        private static string MapColumn(string strTable, string strColumn)
        {
            string strName = (strTable + "." + strColumn).ToLower();

            switch (strName)
            {
                case "cruise.cruise":
                    return "cruise1";

                case "trip.trip":
                    return "trip1";

                case "age.age":
                    return "age1";
            }

            return strColumn;
        }


        /// <summary>
        /// Retrieve column names from the table with name strTableName.
        /// </summary>
        private List<ColumnInfo> GetColumnInformation(Model.DataWarehouseContext ctx, string strTableName)
        {
            return ctx.ExecuteStoreQuery<ColumnInfo>(@" SELECT COLUMN_NAME as 'ColumnName', CHARACTER_MAXIMUM_LENGTH as 'CharacterMaxLength', NUMERIC_PRECISION as 'NumericPrecision', NUMERIC_SCALE as 'NumericScale', IS_NULLABLE as 'IsNullable' 
                                                               FROM INFORMATION_SCHEMA.COLUMNS 
                                                               WHERE TABLE_Name=@tableName
                                                               ORDER by ORDINAL_POSITION
                                                               ", new SqlParameter("@tableName", strTableName)).ToList();
        }


        #endregion


        #region Delete methods

        public void DeleteCruiseAndAssociatedData(int intCruiseId)
        {
            using (var ctx = new Model.DataWarehouseContext())
            {
                ctx.Connection.Open();

              //  using (var scope = new TransactionScope())
                {
                    DeleteCruiseAndAssociatedData(ctx, intCruiseId);

                    ctx.SaveChanges();
                //    scope.Complete();
                }
            }
        }


        private void DeleteTripAndAssociatedData(int intTripId)
        {
            using (var ctx = new Model.DataWarehouseContext())
            {
                ctx.Connection.Open();

              //  using (var scope = new TransactionScope())
                {
                    DeleteTripAndAssociatedData(ctx, intTripId);

                    ctx.SaveChanges();
                  //  scope.Complete();
                }
            }
        }


        private void DeleteSampleAndAssociatedData(int intSampleId)
        {
            using (var ctx = new Model.DataWarehouseContext())
            {
                ctx.Connection.Open();

                //using (var scope = new TransactionScope())
                {
                    DeleteSampleAndAssociatedData(ctx, intSampleId);

                    ctx.SaveChanges();
                //    scope.Complete();
                }
            }
        }



        private void DeleteCruiseAndAssociatedData(Model.DataWarehouseContext ctx, int intCruiseId)
        {
            List<int> lstTripIds = ctx.Cruises.Where(x => x.cruiseId == intCruiseId).SelectMany(x => x.Trips).Select(x => x.tripId).ToList();

            //Delete all samples from trip
            foreach (var tId in lstTripIds)
                DeleteTripAndAssociatedData(ctx, tId);

            //Delete sample record
            ctx.ExecuteStoreCommand(@"DELETE c
                                      FROM  Cruise c
                                      WHERE c.cruiseId = @cruiseId
                                     ", new SqlParameter("@cruiseId", intCruiseId));
        }


        private void DeleteTripAndAssociatedData(Model.DataWarehouseContext ctx, int intTripId)
        {
            List<int> lstSampleIds = ctx.Trips.Where(x => x.tripId == intTripId).SelectMany(x => x.Samples).Select(x => x.sampleId).ToList();

            //Delete all samples from trip
            foreach (var sId in lstSampleIds)
                DeleteSampleAndAssociatedData(ctx, sId);

            //Delete sample record
            ctx.ExecuteStoreCommand(@"DELETE t
                                      FROM  Trip t
                                      WHERE t.tripId = @tripId
                                     ", new SqlParameter("@tripId", intTripId));
        }


        private void DeleteSampleAndAssociatedData(Model.DataWarehouseContext ctx, int intSampleId)
        {
            //Below suppression of transaction scope can be tried out, if problems occur with transactions and calling this method.
            //using (TransactionScope tsSuppressed = new TransactionScope(TransactionScopeOption.Suppress))
            {
                //Delete age records
                //Delete R_AnimalReference
                //Delete animal records
                //Delete specieslist records
                //Delete AnimalRaised records
                //Delete SpeciesListRaised records
                //Delete sample record
                ctx.ExecuteStoreCommand(@"DELETE ag
                                      FROM Age ag
                                      INNER JOIN Animal a ON a.animalId = ag.animalId
                                      INNER JOIN SpeciesList sl on sl.speciesListId = a.speciesListId
                                      WHERE sl.sampleId = @sampleId

                                      DELETE ar
                                      FROM R_AnimalReference ar
                                      INNER JOIN Animal a ON a.animalId = ar.animalId
                                      INNER JOIN SpeciesList sl on sl.speciesListId = a.speciesListId
                                      WHERE sl.sampleId = @sampleId

                                      DELETE apf
                                      FROM R_AnimalPictureReference apf
                                      INNER JOIN Animal a ON a.animalId = apf.animalId
                                      INNER JOIN SpeciesList sl on sl.speciesListId = a.speciesListId
                                      WHERE sl.sampleId = @sampleId

                                      DELETE a
                                      FROM Animal a
                                      INNER JOIN SpeciesList sl ON sl.speciesListId = a.speciesListId
                                      WHERE sl.sampleId = @sampleId

                                      DELETE sl
                                      FROM  SpeciesList sl
                                      WHERE sl.sampleId = @sampleId

                                      DELETE ar
                                      FROM  AnimalRaised ar
                                      INNER JOIN SpeciesListRaised slr ON slr.speciesListRaisedId = ar.speciesListRaisedId
                                      WHERE slr.sampleId = @sampleId

                                      DELETE slr
                                      FROM SpeciesListRaised slr
                                      WHERE slr.sampleId = @sampleId

                                      DELETE s
                                      FROM  Sample s
                                      WHERE s.sampleId = @sampleId
                                     ", new SqlParameter("@sampleId", intSampleId));
            }
        }


        #endregion

 
        
    }
}
