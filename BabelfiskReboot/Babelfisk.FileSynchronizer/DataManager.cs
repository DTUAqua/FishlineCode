using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Configuration;
using System.Data.EntityClient;
using Babelfisk.Entities.Sprattus;
using Babelfisk.FileSynchronizer.Classes;
using Babelfisk.Warehouse;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using Anchor.Core;

namespace Babelfisk.FileSynchronizer
{
    internal class DataManager
    {


        public Dictionary<int, List<DBResultAnimal>> GetAnimalsWithAnimalFiles()
        {
            Dictionary<int, List<DBResultAnimal>> dic = null;
            try
            {
                //Make sure no memory is held up.
                GC.Collect();

                using (var ctx = EntityHelper<SprattusContainer>.CreateInstance())
                {
                    ctx.Connection.Open();
                    ApplyTransactionIsolationLevel(ctx, System.Transactions.IsolationLevel.ReadUncommitted);

                    var res = ctx.ExecuteStoreQuery<DBResultAnimal>(@"SELECT a.animalId, af.animalFileId, af.filePath, af.autoAdded
                                                                      FROM Animal a
                                                                      INNER JOIN AnimalFile af ON af.animalId = a.animalId
                                                                      WHERE af.autoAdded = 1 
                                                                      AND af.fileType = 'OtolithImage'
                                                                     ").ToList();


                    dic = res.GroupBy(x => x.AnimalId).ToDictionary(x => x.Key, g => g.ToList());
                }
            }
            catch (Exception e)
            {
                LogError(e);
                dic = null;
            }

            return dic;
        }

        public HashSet<int> GetAllAnimalIds()
        {
            HashSet<int> lst = null;
            try
            {
                //Make sure no memory is held up.
                GC.Collect();

                using (var ctx = EntityHelper<SprattusContainer>.CreateInstance())
                {
                    ctx.Connection.Open();
                    ApplyTransactionIsolationLevel(ctx, System.Transactions.IsolationLevel.ReadUncommitted);

                    var ls = ctx.ExecuteStoreQuery<int>(@"SELECT DISTINCT a.animalId
                                                          FROM Animal a
                                                          ").ToList();

                    lst = new HashSet<int>(ls);
                }
            }
            catch (Exception e)
            {
                LogError(e);
                lst = null;
            }

            return lst;
        }


        public bool DeleteAnimalFiles(List<DBResultAnimal> lstIn, int intChunkSize = 1000)
        {
            if (lstIn == null || lstIn.Count == 0)
                return true;

            //Delete in chunks, so there query does not get to big.
            foreach (var lst in lstIn.InChunks(intChunkSize))
            {
                string strAnimalFiles = string.Join(",", lst.Select(x => x.AnimalFileId));

                try
                {
                    using (var ctx = EntityHelper<SprattusContainer>.CreateInstance())
                    {
                        ctx.ExecuteStoreCommand(string.Format(@"DELETE af
                                                        FROM  AnimalFile af
                                                        WHERE af.animalFileId IN ({0})", strAnimalFiles));
                    }
                }
                catch (Exception e)
                {
                    LogError(e);
                    return false;
                }
            }

            return true;

        }


        public List<int> GetCruiseIdsFromAnimalIds(List<int> lstAnimalIds, int chunkSize = 1000)
        {
            HashSet<int> lstCruiseIds = new HashSet<int>(); 

            if (lstAnimalIds == null || lstAnimalIds.Count == 0)
                return lstCruiseIds.ToList();

           
            try
            {
                using (var ctx = EntityHelper<SprattusContainer>.CreateInstance())
                {
                    foreach (var lst in lstAnimalIds.InChunks(chunkSize))
                    {
                        var cruiseIds = ctx.Animal.Where(x => lst.Contains(x.animalId)).Select(x => x.SubSample.SpeciesList.Sample.Trip.cruiseId).Distinct().ToList();
                        if(cruiseIds != null && cruiseIds.Count > 0)
                        {
                            foreach(var i in cruiseIds)
                            {
                                if (!lstCruiseIds.Contains(i))
                                    lstCruiseIds.Add(i);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogError(e);
            }

            return lstCruiseIds.ToList();
        }


        public bool InsertAnimalFiles(List<DBResultAnimal> lst)
        {
            if (lst == null || lst.Count == 0)
                return true;

            try
            {
                HashSet<int> hs = GetAllAnimalIds();

                if (hs == null)
                {
                    LogError("Could not retrieve animal ids when inserting animal files.");
                    return false;
                }

                using (var ctx = EntityHelper<SprattusContainer>.CreateInstance())
                {
                    var columns = GetColumnInformation(ctx, typeof(AnimalFile).Name);
                    BulkInsert(ctx, 
                               typeof(AnimalFile).Name, 
                               columns, 
                               lst.Where(x => hs.Contains(x.AnimalId)) //MAke sure the animal actually exists before inserting the record.
                                  .Select(x => new AnimalFile() { animalId = x.AnimalId, filePath = x.FilePath, FileTypeEnum = Entities.AnimalFileType.OtolithImage, autoAdded = true}));
                }
            }
            catch (Exception e)
            {
                LogError(e);
                return false;
            }

            return true;
        }


        public bool InsertDWCruisesToTransfer(List<int> lstAnimalIds, int chunkSize = 1000)
        {
            var lstCruiseIds = GetCruiseIdsFromAnimalIds(lstAnimalIds, chunkSize);

            if (lstCruiseIds == null || lstCruiseIds.Count == 0)
            {
                LogError("No cruise ids found for animal ids: " + string.Join(",", lstAnimalIds));
                return false;
            }

            try
            {
                Warehouse.DataManager datMan = new Warehouse.DataManager();
                datMan.AddCruiseIdsToTransferQueue(lstCruiseIds);
            }
            catch (Exception e)
            {
                LogError(e);
                return false;
            }

            return true;
        }



        /// <summary>
        /// Retrieve column names from the table with name strTableName.
        /// </summary>
        private List<ColumnInfo> GetColumnInformation(ObjectContext ctx, string strTableName)
        {
            return ctx.ExecuteStoreQuery<ColumnInfo>(@" SELECT COLUMN_NAME as 'ColumnName', CHARACTER_MAXIMUM_LENGTH as 'CharacterMaxLength', NUMERIC_PRECISION as 'NumericPrecision', NUMERIC_SCALE as 'NumericScale', IS_NULLABLE as 'IsNullable' 
                                                        FROM INFORMATION_SCHEMA.COLUMNS 
                                                        WHERE TABLE_Name=@tableName
                                                        ORDER by ORDINAL_POSITION
                                                        ", new SqlParameter("@tableName", strTableName)).ToList();
        }

        /// <summary>
        /// Remember to have the "Distributed Transaction Coordinater"-service running for BulkInsert to work.
        /// </summary>
        internal void BulkInsert<T>(ObjectContext ctx, string strTableName, List<ColumnInfo> lstColumns, IEnumerable<T> lstEntities)
        {
            string strConnectionstring = null;

            if (strConnectionstring == null)
            {
                //Create connection string the hard way, since using the ctx connetionstring directly for this, wont work.
                string strCon = ConfigurationManager.ConnectionStrings[ctx.GetType().Name].ConnectionString;
                var csBuilder = new EntityConnectionStringBuilder(strCon);
                strConnectionstring = csBuilder.ProviderConnectionString;
            }

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

        private void ReportMessage(ObjectContext ctx, DWMessage m)
        {
            LogInfo(m.GetSemicolonSeperatedData);
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


        #region Log methods


        public static void LogError(Exception e, string strErrorMessage = null)
        {
            Console.WriteLine("Error: " + (e.Message == null ? "" : e.Message) + " | " + (e.StackTrace == null ? "" : e.StackTrace));
            try
            {
                Anchor.Core.Loggers.Logger.LogError(e, strErrorMessage);
            }
            catch { }
        }


        public static void LogError(string strErrorMessage)
        {
            Console.WriteLine("Error: " + strErrorMessage);
            try
            {
                Anchor.Core.Loggers.Logger.Log(Anchor.Core.Loggers.LogType.Error, strErrorMessage);
            }
            catch { }
        }


        public static void LogInfo(string strInfoMessage)
        {
            Console.WriteLine("Info: " + strInfoMessage);
            try
            {
                Anchor.Core.Loggers.Logger.Log(Anchor.Core.Loggers.LogType.Info, strInfoMessage);
            }
            catch { }
        }

        #endregion



        public static class EntityHelper<T> where T : ObjectContext
        {
            public static T CreateInstance()
            {
                // get the connection string from config file
                string connectionString = ConfigurationManager.ConnectionStrings[typeof(T).Name].ConnectionString;

                // parse the connection string
                var csBuilder = new EntityConnectionStringBuilder(connectionString);

                // replace * by the full name of the containing assembly
                csBuilder.Metadata = csBuilder.Metadata.Replace("res://*/", string.Format("res://{0}/", typeof(Babelfisk.Model.ModelClass).Assembly.FullName));

                // return the object
                return Activator.CreateInstance(typeof(T), csBuilder.ToString()) as T;
            }
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
