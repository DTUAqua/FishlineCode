using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities;
using System.IO;
using Anchor.Core;
using Babelfisk.Entities.Sprattus;
using System.Threading;
using System.Reflection;

namespace Babelfisk.BusinessLogic
{
    public class LookupDataVersioning
    {
        private DataVersioning _localVersions;

        private static Dictionary<string, List<ILookupEntity>> _dicLocalLookups;

        /// <summary>
        /// Lock used to make sure _dicLocalLookups is not read before it has been fully initialized.
        /// </summary>
        private static ReaderWriterLock _rwlLocalLookups = new ReaderWriterLock();

        private DataVersioning _serverVersions;

        private object _objLock = new object();

        #region Known types

        private static Type[] arrKnownTypes = new Type[] { typeof(DFUPerson),
                                                            typeof(L_DFUDepartment),
                                                            typeof(L_Nationality),
                                                            typeof(L_NavigationSystem),
                                                            typeof(L_Platform),
                                                            typeof(L_PlatformType),
                                                            typeof(Person),
                                                            typeof(L_Harbour),
                                                            typeof(L_DFUArea),
                                                            typeof(L_TreatmentFactorGroup),
                                                            typeof(L_Species),
                                                            typeof(L_GearQuality),
                                                            typeof(L_StatisticalRectangle),
                                                            typeof(L_LengthMeasureType),
                                                            typeof(L_LengthMeasureUnit),
                                                            typeof(L_MaturityIndexMethod),
                                                            typeof(L_SampleType),
                                                            typeof(L_GearType),
                                                            typeof(L_GearInfoType),
                                                            typeof(L_Gear),
                                                            typeof(R_GearInfo),
                                                            typeof(L_GearQuality),
                                                            typeof(L_SamplingMethod),
                                                            typeof(L_SamplingType),
                                                            typeof(L_Treatment),
                                                            typeof(L_Species),
                                                            typeof(TreatmentFactor),
                                                            typeof(Maturity),
                                                            typeof(ICES_DFU_Relation_FF),
                                                            typeof(L_SpeciesRegistration),
                                                            typeof(L_CatchRegistration),
                                                            typeof(L_SelectionDevice),
                                                            typeof(L_SelectionDeviceSource),
                                                            typeof(R_GearTypeSelectionDevice),
                                                            typeof(L_FisheryType),
                                                            typeof(L_LandingCategory),
                                                            typeof(L_ThermoCline),
                                                            typeof(L_HaulType),
                                                            typeof(L_Bottomtype),
                                                            typeof(L_TimeZone),
                                                            typeof(L_SizeSortingEU),
                                                            typeof(L_SizeSortingDFU),
                                                            typeof(L_SexCode),
                                                            typeof(L_WeightEstimationMethod),
                                                            typeof(L_BroodingPhase),
                                                            typeof(L_OtolithReadingRemark),
                                                            typeof(L_EdgeStructure),
                                                            typeof(L_Parasite),
                                                            typeof(L_Reference),
                                                            typeof(L_SDEventType),
                                                            typeof(L_SDPurpose),
                                                            typeof(L_SDSampleType),
                                                            typeof(L_SDLightType),
                                                            typeof(L_SDOtolithDescription),
                                                            typeof(L_SDPreparationMethod),
                                                            typeof(L_Stock),
                                                            typeof(L_StomachStatus)

        };
        #endregion

        public LookupDataVersioning()
        {
            LoadLocalVersions();
            LoadServerVersions();
        }


        public bool IsLocalLookupsExpired(string strType)
        {
            //If the local lookups does not exist yet, grab them from server
            if (!(_localVersions != null && _localVersions.ContainsType(strType)) || !_dicLocalLookups.ContainsKey(strType))
                return true;

           // if (!_serverVersions.ContainsType(strType))
           //     return false;

            long lngLocal = _localVersions.GetVersion(strType);
            //Default server version to 0, they have not been changed ever.
            long lngServer = _serverVersions.ContainsType(strType) ? _serverVersions.GetVersion(strType) : 0;

            return lngLocal != lngServer;
        }



        public List<ILookupEntity> GetLocalLookups(string strType, params string[] includes)
        {
            _rwlLocalLookups.AcquireReaderLock(20 * 1000);

            List<ILookupEntity> lst = new List<ILookupEntity>();

            if (_dicLocalLookups.ContainsKey(strType))
                lst = _dicLocalLookups[strType];

            lst = lst.Clone(arrKnownTypes);

            _rwlLocalLookups.ReleaseReaderLock();

            HandleLookupIncludes(strType, ref lst, includes);

            return lst;
        }


        private void HandleLookupIncludes(string strLookupType, ref List<ILookupEntity> lst, params string[] includes)
        {
            if (lst == null || lst.Count == 0 || includes == null || includes.Length == 0)
                return;

            var lookupMan = new LookupManager();
            foreach (var strIncludeProperty in includes)
            {
                if (!strIncludeProperty.StartsWith("L_") && !strIncludeProperty.Contains("DFUPerson"))
                    continue;

                Type objType = lst.First().GetType();

                string strCodeColumn = strIncludeProperty.Replace("L_", "");

                var navProp = objType.GetField("_" + strIncludeProperty, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
                var propIdOrCode = objType.GetField(OfflineMapIncludeField(strLookupType, strCodeColumn), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase);

                if (navProp == null)
                    throw new ApplicationException(String.Format("Property '{0}' does not exist on object '{0}'.", strIncludeProperty, objType.Name));

                if (propIdOrCode == null)
                    throw new ApplicationException(String.Format("Property '{0}' does not exist on object '{0}'.", OfflineMapIncludeField(strLookupType, strCodeColumn), objType.Name));
               

                var lstLookups = lookupMan.GetLookups(navProp.FieldType, this);

                for (int i = 0; i < lst.Count; i++)
                {
                    var objId = propIdOrCode.GetValue(lst[i]);

                    //ignore navigation property if there is none assigned
                    if (objId == null)
                        continue;

                    string strId = objId.ToString();
                    var val = lstLookups.Where(x => x.Id.Equals(strId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                    if (val == null)
                        throw new ApplicationException(String.Format("Property '{0}' was null for object '{0}'.", strCodeColumn, objType.Name));

                    navProp.SetValue(lst[i], val);
                }
            }
        }

        private string OfflineMapIncludeField(string strTypeName, string strCodeColumn)
        {
            switch (strCodeColumn)
            {
                case "Species":
                    return "_speciesCode";

                case "SampleType":
                    if(strTypeName == "L_GearType")
                        return "_catchOperation";
                    break;

                case "DFUArea2":
                    if(strTypeName == "L_DFUArea")
                        return "_parentDFUArea";
                    break;

                case "Stock":
                    if (strTypeName == "R_SDReader")
                        return "_stockId";
                    break;

                case "SDReaderExperience":
                    if (strTypeName == "R_SDReader")
                        return "_sdReaderExperienceId";
                    break;

                case "SDPreparationMethod":
                    if (strTypeName == "R_SDReader")
                        return "_sdPreparationMethodId";

                    break;

                case "DFUPerson":
                    if (strTypeName == "R_SDReader")
                        return "_dfuPersonId";

                    break;
            }

            return "_" + strCodeColumn;
        }



        public void SetLocalLookups(string strType, List<ILookupEntity> lstLookups)
        {
            //Make sure only one thread is assigning a lookup type at a time.
            lock (_objLock)
            {
                //Make sure _dicLocalLookups is not read/written to before it has been fully loaded.
                _rwlLocalLookups.AcquireWriterLock(180 * 1000);

                try
                {
                    long lngServerVersion = _serverVersions.GetVersion(strType);

                    _localVersions.Set(strType, lngServerVersion);

                    if (!_dicLocalLookups.ContainsKey(strType))
                        _dicLocalLookups.Add(strType, lstLookups);
                    else
                        _dicLocalLookups[strType] = lstLookups;

                    string strPath = Path.Combine(Settings.Settings.Instance.OfflineLookupDataPath, strType + ".xml");
                    var kv = new KeyValuePair<string, List<ILookupEntity>>(strType, lstLookups);

                    byte[] arr = kv.ToByteArrayDataContract(arrKnownTypes);
                    arr = arr.Compress();
                    File.WriteAllBytes(strPath, arr);
                }
                catch(Exception e)
                {
                    Anchor.Core.Loggers.Logger.LogError(e, String.Format("SetLocalLookups({0}, lst)", strType));
                }

                _rwlLocalLookups.ReleaseReaderLock();
            }
        }
 

        public bool UpdateLocalLookupList(ILookupEntity ile)
        {
            return true;
        }


        private void LoadLocalVersions()
        {
            //Make sure _dicLocalLookups is not read before it has been fully loaded.
            _rwlLocalLookups.AcquireWriterLock(180 * 1000);

            _localVersions = Settings.Settings.Instance.LookupVersions;

            if (_dicLocalLookups == null)
            {
                _dicLocalLookups = new Dictionary<string, List<ILookupEntity>>();

                string strOfflineLookupPath = Settings.Settings.Instance.OfflineLookupDataPath;

                var arrFiles = Directory.GetFiles(strOfflineLookupPath);

                foreach (var strFile in arrFiles)
                {
                    try
                    {
                        byte[] arr = File.ReadAllBytes(strFile);
                        arr = arr.Decompress();
                        var kv = arr.ToObjectDataContract<KeyValuePair<string, List<ILookupEntity>>>(arrKnownTypes);

                        _dicLocalLookups.Add(kv.Key, kv.Value);
                    }
                    catch(Exception e)
                    {
                        Anchor.Core.Loggers.Logger.LogError(e, String.Format("Could not deserialize lookup-file '{0}'.", strFile));
                    }
                }
            }

            _rwlLocalLookups.ReleaseWriterLock();
        }




        private void LoadServerVersions()
        {
            var ser = BusinessLogic.DataClientFactory.CreateLookupClient();

            try
            {
                ser.SupplyCredentials();

                _serverVersions = (ser as Babelfisk.BusinessLogic.BabelfiskService.ILookup).GetLookupVersions();

                ser.Close();
            }
            catch(Exception e)
            {
                ser.Abort();
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }

    }
}
