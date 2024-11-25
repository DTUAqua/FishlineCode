using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using Babelfisk.Entities;
using System.Data;
using System.Reflection;
using System.Data.Objects;
using System.Runtime.Serialization;
using System.Threading;
using System.IO;

namespace Babelfisk.Service
{
    public partial class BabelfiskService : ILookup
    {
        private static object _objLookupInitLock = new object();
        private static object _objSettingsLock = new object();
        private static bool _blnInitializeLookupVersions = true;
        private static object _objFileLock = new object();
        private static object _objBuldingShapeLibraryLock = new object();
        private static ReaderWriterLock _rwlLookupVersion = new ReaderWriterLock();

        private static DataVersioning _lookupVersions = null;


        #region Lookup version initialization


        /// <summary>
        /// Initialize lookup versions.
        /// </summary>
        private void InitializeLookupVersions()
        {
            lock (_objLookupInitLock)
            {
                if (!_blnInitializeLookupVersions)
                    return;
                else
                    _blnInitializeLookupVersions = false;

                LoadLookupVersions();
                //new Action(LoadLookupVersions).BeginInvoke(null, null);
            }
        }


        /// <summary>
        /// Load all polygon shapefiles
        /// </summary>
        private void LoadLookupVersions()
        {
            //Aquire lock with a 30 min timeout
            _rwlLookupVersion.AcquireWriterLock(new TimeSpan(0, 30, 0));
            try
            {
                string strDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ("Data"));
                string strFile = Path.Combine(strDirectory, "LookupVersions.xml");

                if (!Directory.Exists(strDirectory))
                    Directory.CreateDirectory(strDirectory);

                _lookupVersions = DataVersioning.LoadFromFile(strFile);
            }
            catch(Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
            _rwlLookupVersion.ReleaseWriterLock();
        }


        #endregion



        public DataVersioning GetLookupVersions()
        {
            InitializeLookupVersions();
            return _lookupVersions;
        }


        public byte[] GetLookups(string strEntityType, string[] includes)
        {
            Type t = Type.GetType(strEntityType);
            List<Babelfisk.Entities.ILookupEntity> lst = GetLookupEntities(t, includes);

            byte[] arr = null;

            if (lst != null)
            {
                if(t != null)
                    arr = lst.ToByteArrayDataContract(new Type[] {t});
                else
                    arr = lst.ToByteArrayDataContract();
                arr = arr.Compress();
            }

            return arr;
        }


        private List<ILookupEntity> GetLookupEntities(Type t, string[] includes)
        {
            List<Babelfisk.Entities.ILookupEntity> lst = null;
            try
            {

                using (var ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

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

                    /*
                    if (objectSetLookups != null)
                    {
                        lst = new List<Babelfisk.Entities.ILookupEntity>();
                        foreach (var lookup in objectSetLookups)
                        {
                            if (includes != null)
                                for (int i = 0; i < includes.Length; i++)
                                    ctx.LoadProperty(lookup, includes[i]);

                            lst.Add(lookup as Babelfisk.Entities.ILookupEntity);
                        }
                    }
                    */
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return lst;
        }


       


        public DatabaseOperationResult SaveLookups(ref List<ILookupEntity> lookups)
        {
            try
            {
                string strType = null;
                using (SprattusContainer ctx = new SprattusContainer())
                {
                    ctx.Connection.Open();
                    ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                    foreach (ILookupEntity lookup in lookups)
                    {
                        if (lookup.ChangeTracker.State == ObjectState.Unchanged)
                            continue;

                        strType = lookup.GetType().Name;

                        lookup.BeforeSave();

                        ApplyLookupChanges(ctx, lookup);

                        if (lookup.ChangeTracker.State == ObjectState.Deleted)
                            ctx.DeleteObject(lookup);

                        ctx.ApplyOverwritingMethod(lookup, OverwritingMethod.ClientWins);
                        ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);
                    }

                   //ctx.SaveChangesAndHandleOptimisticConcurrency(OverwritingMethod.ClientWins);
                }

                //Save lookup versions.
                if (strType != null)
                {
                    var v = GetLookupVersions();
                    if(v != null)
                        v.IncrementAndSave(strType);
                }

                return DatabaseOperationResult.CreateSuccessResult();
            }
            catch (OptimisticConcurrencyException ocex)
            {
                string strMsg = "OptimisticConcurrencyException thrown in SaveLookups with message: " + ocex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.ConcurrencyException, strMsg);
            }
            catch (UpdateException uex)
            {
                string strMsg = "UpdateException thrown in SaveLookups with message: " + uex.Message + ", Innerexception: " + (uex.InnerException != null ? uex.InnerException.Message : "");

                return new DatabaseOperationResult(DatabaseOperationStatus.DuplicateRecordException, strMsg);
            }
            catch (Exception ex)
            {
                string strMsg = "Exception thrown in SaveLookups with message: " + ex.Message;

                return new DatabaseOperationResult(DatabaseOperationStatus.UnexpectedException, strMsg);
            }
        }

        /*
                private ILookupEntity AssignLookupChanges(SprattusContainer ctx, ILookupEntity ile)
                {
                    IObjectWithChangeTracker ioe = ile;
                    Type t = ile.GetType();

                    ILookupEntity ileNewEdit;

                    if (ioe.ChangeTracker.State == ObjectState.Added)
                    {
                        var ci = t.GetConstructor(new Type[] { });
                        ileNewEdit = ci.Invoke(new object[] { }) as ILookupEntity;
                    }
                    else
                    {
                        var objectSetLookups = ctx.GetType().GetProperties()
                                                            .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericArguments()[0].Name == t.Name)
                                                            .Select(p => p.GetValue(ctx, null) as IEnumerable).First();
                    }



                }
        */

        private void ApplyLookupChanges(SprattusContainer ctx, ILookupEntity obj)
        {
            string strName = obj.GetType().Name;

            switch (strName)
            {
                case "DFUPerson":
                    ctx.DFUPerson.ApplyChanges(obj as DFUPerson);
                    break;

                case "L_DFUDepartment":
                    ctx.L_DFUDepartment.ApplyChanges(obj as L_DFUDepartment);
                    break;

                case "L_Nationality":
                    ctx.L_Nationality.ApplyChanges(obj as L_Nationality);
                    break;

                case "L_NavigationSystem":
                    ctx.L_NavigationSystem.ApplyChanges(obj as L_NavigationSystem);
                    break;

                case "L_Platform":
                    ctx.L_Platform.ApplyChanges(obj as L_Platform);
                    break;

                case "L_PlatformType":
                    ctx.L_PlatformType.ApplyChanges(obj as L_PlatformType);
                    break;

                case "Person":
                    ctx.Person.ApplyChanges(obj as Person);
                    break;

                case "L_PlatformVersion":
                    ctx.L_PlatformVersion.ApplyChanges(obj as L_PlatformVersion);
                    break;

                case "L_Harbour":
                    ctx.L_Harbour.ApplyChanges(obj as L_Harbour);
                    break;

                case "L_DFUArea":
                    var eArea = obj as L_DFUArea;

                    //Handle if L_DFUArea2 (parentDFUArea) is passed a long. Then remove it, so it does not conflict doing applying changes, and set the parentDFUArea string only.
                    if ((eArea.ChangeTracker.State == ObjectState.Modified || eArea.ChangeTracker.State == ObjectState.Added) && eArea.L_DFUArea2 != null)
                    {
                        var parentArea = eArea.L_DFUArea2.DFUArea;

                        eArea.ChangeTracker.ChangeTrackingEnabled = false;
                        {
                            if (eArea.ChangeTracker.OriginalValues.ContainsKey("L_DFUArea2"))
                                eArea.ChangeTracker.OriginalValues.Remove("L_DFUArea2");
                            eArea.L_DFUArea2 = null;
                            eArea.parentDFUArea = parentArea;
                        }
                        eArea.ChangeTracker.ChangeTrackingEnabled = true;
                    }

                    ctx.L_DFUArea.ApplyChanges(obj as L_DFUArea);
                    break;

                case "L_TreatmentFactorGroup":
                    ctx.L_TreatmentFactorGroup.ApplyChanges(obj as L_TreatmentFactorGroup);
                    break;

                case "L_Species":
                    ctx.L_Species.ApplyChanges(obj as L_Species);
                    break;

                case "L_GearQuality":
                    ctx.L_GearQuality.ApplyChanges(obj as L_GearQuality);
                    break;

                case "L_StatisticalRectangle":
                    ctx.L_StatisticalRectangle.ApplyChanges(obj as L_StatisticalRectangle);
                    break;

                case "L_LengthMeasureType":
                    ctx.L_LengthMeasureType.ApplyChanges(obj as L_LengthMeasureType);
                    break;

                case "L_LengthMeasureUnit":
                    ctx.L_LengthMeasureUnit.ApplyChanges(obj as L_LengthMeasureUnit);
                    break;

                case "L_MaturityIndexMethod":
                    ctx.L_MaturityIndexMethod.ApplyChanges(obj as L_MaturityIndexMethod);
                    break;

                case "L_SampleType":
                    ctx.L_SampleType.ApplyChanges(obj as L_SampleType);
                    break;

                case "L_GearType":
                    ctx.L_GearType.ApplyChanges(obj as L_GearType);
                    break;

                case "L_GearInfoType":
                    ctx.L_GearInfoType.ApplyChanges(obj as L_GearInfoType);
                    break;

                case "L_Gear":
                    ctx.L_Gear.ApplyChanges(obj as L_Gear);
                    break;

                case "R_GearInfo":
                    var gi = obj as R_GearInfo;
                    ctx.R_GearInfo.ApplyChanges(gi);
                    break;

                case "L_SamplingMethod":
                    ctx.L_SamplingMethod.ApplyChanges(obj as L_SamplingMethod);
                    break;

                case "L_SamplingType":
                    ctx.L_SamplingType.ApplyChanges(obj as L_SamplingType);
                    break;

                case "L_Treatment":
                    ctx.L_Treatment.ApplyChanges(obj as L_Treatment);
                    break;

                case "TreatmentFactor":
                    ctx.TreatmentFactor.ApplyChanges(obj as TreatmentFactor);
                    break;

                case "Maturity":
                    ctx.Maturity.ApplyChanges(obj as Maturity);
                    break;

                case "ICES_DFU_Relation_FF":
                    ctx.ICES_DFU_Relation_FF.ApplyChanges(obj as ICES_DFU_Relation_FF);
                    break;

                case "L_SpeciesRegistration":
                    ctx.L_SpeciesRegistration.ApplyChanges(obj as L_SpeciesRegistration);
                    break;

                case "L_CatchRegistration":
                    ctx.L_CatchRegistration.ApplyChanges(obj as L_CatchRegistration);
                    break;

                case "L_SelectionDevice":
                    ctx.L_SelectionDevice.ApplyChanges(obj as L_SelectionDevice);
                    break;

                case "L_SelectionDeviceSource":
                    ctx.L_SelectionDeviceSource.ApplyChanges(obj as L_SelectionDeviceSource);
                    break;

                case "R_GearTypeSelectionDevice":
                    ctx.R_GearTypeSelectionDevice.ApplyChanges(obj as R_GearTypeSelectionDevice);
                    break;

                case "L_FisheryType":
                    ctx.L_FisheryType.ApplyChanges(obj as L_FisheryType);
                    break;

                case "L_LandingCategory":
                    ctx.L_LandingCategory.ApplyChanges(obj as L_LandingCategory);
                    break;

                case "L_ThermoCline":
                    ctx.L_ThermoCline.ApplyChanges(obj as L_ThermoCline);
                    break;

                case "L_HaulType":
                    ctx.L_HaulType.ApplyChanges(obj as L_HaulType);
                    break;

                case "L_Bottomtype":
                    ctx.L_Bottomtype.ApplyChanges(obj as L_Bottomtype);
                    break;

                case "L_TimeZone":
                    ctx.L_TimeZone.ApplyChanges(obj as L_TimeZone);
                    break;

                case "L_WeightEstimationMethod":
                    ctx.L_WeightEstimationMethod.ApplyChanges(obj as L_WeightEstimationMethod);
                    break;

                case "L_BroodingPhase":
                    ctx.L_BroodingPhase.ApplyChanges(obj as L_BroodingPhase);
                    break;

                case "L_OtolithReadingRemark":
                    ctx.L_OtolithReadingRemark.ApplyChanges(obj as L_OtolithReadingRemark);
                    break;

                case "L_EdgeStructure":
                    ctx.L_EdgeStructure.ApplyChanges(obj as L_EdgeStructure);
                    break;

                case "L_Parasite":
                    ctx.L_Parasite.ApplyChanges(obj as L_Parasite);
                    break;

                case "L_Reference":
                    ctx.L_Reference.ApplyChanges(obj as L_Reference);
                    break;

                case "L_SexCode":
                    ctx.L_SexCode.ApplyChanges(obj as L_SexCode);
                    break;

                case "L_SizeSortingDFU":
                    ctx.L_SizeSortingDFU.ApplyChanges(obj as L_SizeSortingDFU);
                    break;

                case "L_SizeSortingEU":
                    ctx.L_SizeSortingEU.ApplyChanges(obj as L_SizeSortingEU);
                    break;

                case "L_Application":
                    ctx.L_Applications.ApplyChanges(obj as L_Application);
                    break;

                case "L_HatchMonthReadability":
                    ctx.L_HatchMonthReadability.ApplyChanges(obj as L_HatchMonthReadability);
                    break;

                case "L_VisualStock":
                    ctx.L_VisualStock.ApplyChanges(obj as L_VisualStock);
                    break;

                case "L_GeneticStock":
                    ctx.L_GeneticStock.ApplyChanges(obj as L_GeneticStock);
                    break;

                case "L_SDEventType":
                    ctx.L_SDEventType.ApplyChanges(obj as L_SDEventType);
                    break;

                case "L_SDPurpose":
                    ctx.L_SDPurpose.ApplyChanges(obj as L_SDPurpose);
                    break;

                case "L_Stock":
                    ctx.L_Stock.ApplyChanges(obj as L_Stock);
                    break;

                case "L_SDSampleType":
                    ctx.L_SDSampleType.ApplyChanges(obj as L_SDSampleType);
                    break;

                case "L_SDLightType":
                    ctx.L_SDLightType.ApplyChanges(obj as L_SDLightType);
                    break;

                case "L_SDOtolithDescription":
                    ctx.L_SDOtolithDescription.ApplyChanges(obj as L_SDOtolithDescription);
                    break;

                case "L_SDPreparationMethod":
                    ctx.L_SDPreparationMethod.ApplyChanges(obj as L_SDPreparationMethod);
                    break;

                case "L_SDReaderExperience":
                    ctx.L_SDReaderExperience.ApplyChanges(obj as L_SDReaderExperience);
                    break;

                case "R_SDReader":
                    ctx.R_SDReader.ApplyChanges(obj as R_SDReader);
                    break;

                case "R_StockSpeciesArea":
                    ctx.R_StockSpeciesArea.ApplyChanges(obj as R_StockSpeciesArea);
                    break;

                case "L_StomachStatus":
                    ctx.L_StomachStatus.ApplyChanges(obj as L_StomachStatus);
                    break;

                default:
                    throw new ApplicationException("BabelfiskService.ILookup->ApplyLookupChanges(): Lookup type has not been mapped.");
            }
        }

        public List<Record> GetDependencies(ILookupEntity objLookup)
        {
            List<Record> lstRes = new List<Record>();
            ILookupEntity lookup = (objLookup as ILookupEntity);

            if (lookup == null)
                return lstRes;

            string strLookupSet = objLookup.GetType().Name;

            string strQuery =
                              "declare @RowId varchar(max) " +
                              "set @RowId = '{0}' " +
                              "declare @TableName sysname " +
                              "set @TableName = '{1}' " +
                              "declare @Command varchar(max) " +
                              "declare @temp table ( Result nvarchar(max))" +

                              "select @Command = isnull(@Command + ' union all ', '') + 'select ''' + object_name(parent_object_id) +  " +
                              "''' where exists(select * from ' + object_name(parent_object_id) + ' where ' + col.name + ' = ''' + @RowId + ''')' " +
                              "from sys.foreign_key_columns fkc " +
                              "join sys.columns col on fkc.parent_object_id = col.object_id and fkc.parent_column_id = col.column_id " +
                              "where object_name(referenced_object_id) = @TableName " +
                              "insert into @temp (Result)" +
                              "execute (@Command) " +
                              "select * from @temp";


            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                string str = string.Format(strQuery, lookup.Id, strLookupSet);
                var res = ctx.ExecuteStoreQuery<Record>(str);
                lstRes = res.ToList();
            }

            return lstRes;
        }


        public List<L_DFUArea> GetRectangleAreas()
        {
            using (var ctx = new SprattusContainer())
            {
                ctx.Connection.Open();
                ctx.ApplyTransactionIsolationLevel(System.Transactions.IsolationLevel.ReadUncommitted);

                var lst = (from a in ctx.ICES_DFU_Relation_FF.Select(x => x.area_20_21.ToUpper())
                          join area in ctx.L_DFUArea on a equals area.DFUArea.ToUpper()
                          select area).Distinct().ToList();

                return lst;
            }
        }
    }
}