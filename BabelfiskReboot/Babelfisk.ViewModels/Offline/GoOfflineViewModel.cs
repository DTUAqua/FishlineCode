using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using Anchor.Core;
using System.IO;
using Babelfisk.BusinessLogic;
using System.Reflection;

namespace Babelfisk.ViewModels.Offline
{
    public class GoOfflineViewModel : AViewModel
    {
        private DelegateCommand _cmdGoOffline;
        private DelegateCommand _cmdCancelTransfer;

        private TreeView.MainTreeViewModel _vmTree;

        private OfflineProcessState _enmProcessState;

        private double _dblTransferredPercentage = 0;

        private int _intTripsToTransfer = 0;
        private int _intTripsTransferred = 0;

        private bool _blnCancelled = false;


        #region Properties


        public TreeView.MainTreeViewModel SelectionTree
        {
            get { return _vmTree; }
            set
            {
                _vmTree = value;
                RaisePropertyChanged(() => MainTree);
            }
        }


        public OfflineProcessState ProcessState
        {
            get { return _enmProcessState; }
            set
            {
                _enmProcessState = value;
                RaisePropertyChanged(() => ProcessState);
            }
        }


        public double TransferredPercentage
        {
            get { return _dblTransferredPercentage; }
            set
            {
                _dblTransferredPercentage = value;
                RaisePropertyChanged(() => TransferredPercentage);
            }
        }


        public int TripsToTransfer
        {
            get { return _intTripsToTransfer; }
            set
            {
                _intTripsToTransfer = value;
                RaisePropertyChanged(() => TripsToTransfer);
            }
        }


        public int TripsTransffered
        {
            get { return _intTripsTransferred; }
            set
            {
                _intTripsTransferred = value;
                RaisePropertyChanged(() => TripsTransffered);
            }
        }


        public bool IsTransferCancelled
        {
            get { return _blnCancelled; }
            set
            {
                _blnCancelled = value;
                RaisePropertyChanged(() => IsTransferCancelled);
            }
        }


        #endregion


        public GoOfflineViewModel()
        {
            WindowWidth = 700;
            WindowHeight = 450;
            WindowTitle = "Gå offline";
            _enmProcessState = OfflineProcessState.Idle;

            _vmTree = new TreeView.MainTreeViewModel(false, TreeView.MainTreeViewModel.TreeDepth.Trip);
        }


        #region Go Offline Command


        public DelegateCommand GoOfflineCommand
        {
            get { return _cmdGoOffline ?? (_cmdGoOffline = new DelegateCommand(GoOfflineAsync)); }
        }


        private void GoOfflineAsync()
        {
            ProcessState = OfflineProcessState.Analyzing;
            IsTransferCancelled = false;

            Task.Factory.StartNew(GoOffline).ContinueWith(t => new Action(() => ProcessState = OfflineProcessState.Idle).Dispatch());
        }


        private void GoOffline()
        {
            try
            {
                List<int> lstTripIds, lstCruiseIds;
                GetSelectedTripIds(out lstCruiseIds, out lstTripIds);

                //Delete any existing data
                string strOfflineDataPath = BusinessLogic.Settings.Settings.Instance.OfflineDataPath;

                int intDeleteTries = 2;
                bool blnDeleteSuccess = false;

                //Give it two tries in deleting the folder.
                while (intDeleteTries-- > 0)
                {
                    try
                    {
                        DirectoryInfo dir = new DirectoryInfo(strOfflineDataPath);
                        System.Threading.Thread.Sleep(500);
                        dir.Delete(true);
                        dir.Create();
                        blnDeleteSuccess = true;
                        break;
                    }
                    catch { }
                }

                if (!blnDeleteSuccess)
                    throw new ApplicationException("Tidligere offline data kunne ikke slettes, prøv venligst igen.");

                BusinessLogic.Offline.OfflineDictionary.New();

                //Copy over lookups
                if (!SyncLookups())
                    return;

                int intTotal = lstCruiseIds.Count + lstTripIds.Count;

                ReportProgress(0, intTotal, 0, OfflineProcessState.Transferring);

                if (!SyncCruises(lstCruiseIds, 0, intTotal))
                    return;

                //Copy over selected trips;
                if (!SyncTrips(lstTripIds, lstCruiseIds.Count, intTotal))
                    return;

                BusinessLogic.Offline.OfflineDictionary.Instance.UpdateListsAndSave();

                BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline = true;
                BusinessLogic.Settings.Settings.Instance.Save();
            }
            catch (Exception e)
            {
                DispatchMessageBox("En uventet fejl opstod. " + e.Message);
                return;
            }


            new Action(() =>
            {
                BusinessLogic.Settings.Settings.Instance.OfflineStatus.RaiseAllPropertiesChanged();
                MainTree.InitializeTreeViewAsync();
                AppRegionManager.RefreshWindowTitle();
                ProcessState = OfflineProcessState.Idle;
                Close();
            }).Dispatch();
        }


        private bool SyncLookups()
        {
            var om = new BusinessLogic.Offline.OfflineManager();
            LookupDataVersioning ldv = new LookupDataVersioning();

            Type lookupType = typeof(Babelfisk.Entities.ILookupEntity);
            var types = lookupType.Assembly.GetTypes().Where(t => lookupType.IsAssignableFrom(t) && !t.IsInterface).ToList();

            //Set initial transfer states
            ReportProgress(0, types.Count, 0, OfflineProcessState.SyncLookups);

            for(int i = 0; i < types.Count; i++)
            {
                om.SaveLookups(types[i], ldv);
                ReportProgress(i+1, types.Count, (100.0 / (double)types.Count) * (i + 1), OfflineProcessState.SyncLookups);

                if (_blnCancelled)
                    return false;
            }

            System.Threading.Thread.Sleep(500);

            return true;
        }


        /// <summary>
        /// Copy cruises to disk
        /// </summary>
        private bool SyncCruises(List<int> lstCruiseIds, int intCurrentProgress, int intTotal)
        {
            var datMan = new BusinessLogic.Offline.OfflineManager();
            List<int> lstTmp = new List<int>();
            int intChunkSize = GetChunkSize(lstCruiseIds.Count);
            for (int i = 0; i < lstCruiseIds.Count; i++)
            {
                lstTmp.Add(lstCruiseIds[i]);

                if (lstTmp.Count == intChunkSize || i >= lstCruiseIds.Count - 1)
                {
                    var data = datMan.GetOfflineCruiseData(lstTmp);
                    SaveOfflineData(data);

                    ReportProgress(intCurrentProgress + i + 1, intTotal, (100.0 / (double)intTotal) * (intCurrentProgress + i + 1));
                    lstTmp.Clear();
                }

                if (_blnCancelled)
                    return false;
            }

            return true;
        }

        private int GetChunkSize(int intAllCount)
        {
            if(intAllCount < 10)
                return 1;

            if(intAllCount < 50)
                return 5;

            return 10;
        }


        /// <summary>
        /// Copy trips and associated cruises to disk.
        /// </summary>
        private bool SyncTrips(List<int> lstTripIds, int intCurrentProgress, int intTotal)
        {
            var datMan = new BusinessLogic.Offline.OfflineManager();
            List<int> lstTmp = new List<int>();
            int intChunkSize = GetChunkSize(lstTripIds.Count);
            for (int i = 0; i < lstTripIds.Count; i++)
            {
                lstTmp.Add(lstTripIds[i]);

                if (lstTmp.Count == intChunkSize || i >= lstTripIds.Count - 1)
                {
                    var data = datMan.GetOfflineTripData(lstTmp);
                    SaveOfflineData(data);

                    ReportProgress(intCurrentProgress + i + 1, intTotal, (100.0 / (double)intTotal) * (intCurrentProgress + i + 1));
                    lstTmp.Clear();
                }

                if (_blnCancelled)
                    return false;
            }

            return true;
        }



        private void SaveOfflineData(List<Entities.OfflineTripTransfer> lstOfflineData)
        {
            string strOfflineDataPath = BusinessLogic.Settings.Settings.Instance.OfflineDataPath;

            BusinessLogic.Offline.OfflineDictionary od = BusinessLogic.Offline.OfflineDictionary.Instance;

            foreach (var ott in lstOfflineData)
            {
                //1) Create year folder, if it does not exist.
                string strYearFolder = Path.Combine(strOfflineDataPath, ott.Cruise.year.ToString());
                if (!Directory.Exists(strYearFolder))
                    Directory.CreateDirectory(strYearFolder);

                //2) Create cruise folder (with cruiseid)
                string strCruiseFolder = Path.Combine(strYearFolder, ott.Cruise.cruiseId.ToString());

                if(!Directory.Exists(strCruiseFolder))
                    Directory.CreateDirectory(strCruiseFolder);

                //3) Save cruise file, if not already there.
                string strCruiseFile = Path.Combine(strCruiseFolder, "cruise.bin");

                if (!File.Exists(strCruiseFile))
                {
                    od.AddCruise(ott.Cruise);
                    SaveFile(strCruiseFile, ott.Cruise);
                }

                //3) Create trip folder
                string strTripFolder = Path.Combine(strCruiseFolder, "trips");

                if (!Directory.Exists(strTripFolder))
                    Directory.CreateDirectory(strTripFolder);

                if (ott.Trip == null)
                    continue;

                //4) Create trip file, if not already there.
                string strTripFile = Path.Combine(strTripFolder, ott.Trip.tripId + ".bin");

                if (!File.Exists(strTripFile))
                {
                    od.AddTrip(ott.Trip);
                    SaveFile(strTripFile, ott.Trip);
                }

                //Add offline data mappings.
                foreach (Entities.Sprattus.Sample s in ott.Trip.Sample)
                {
                    od.AddSample(s);

                    foreach (Entities.Sprattus.SpeciesList sl in s.SpeciesList)
                    {
                        od.AddSpeciesList(sl);

                        foreach (Entities.Sprattus.SubSample ss in sl.SubSample)
                            od.AddSubSample(ss);
                    }
                }

               
                //5) Create samples folder in trip folder
               /* string strSampleFolder = Path.Combine(strTripFolder, "samples");

                if (ott.Samples.Count > 0)
                {
                    if (!Directory.Exists(strSampleFolder))
                        Directory.CreateDirectory(strSampleFolder);


                    //6) Create all sample files
                    string strSamplefile = Path.Combine(strSampleFolder, "samples.bin");

                    SaveFile(strSamplefile, ott.Samples);
                }*/

               /* foreach (var sample in ott.Samples)
                {
                    //Create sample
                    string strSamplefile = Path.Combine(strSampleFolder, sample.station.ToString() + ".bin");

                    if (!File.Exists(strSamplefile))
                        SaveFile(strSamplefile, sample);
                }*/
            }

           
        }


        private void SaveFile(string strFilePath, object obj)
        {
            byte[] arr = obj.ToByteArrayDataContract(new Type[] {typeof(List<Entities.Sprattus.Sample>)});
            arr = arr.Compress();
            File.WriteAllBytes(strFilePath, arr);
        }


        private void ReportProgress(int intTripTransffered, int intTripsToTransfer, double dblTransferredPercentage, OfflineProcessState enmState = OfflineProcessState.Transferring)
        {
            new Action(() =>
            {
                TripsTransffered = intTripTransffered;

                if(_intTripsToTransfer != intTripsToTransfer)
                    TripsToTransfer = intTripsToTransfer;

                TransferredPercentage = dblTransferredPercentage;

                if (ProcessState != enmState)
                    ProcessState = enmState;
            }).Dispatch();
        }


        private void GetSelectedTripIds(out List<int> lstCruiseIds, out List<int> lstTripIds)
        {
            lstCruiseIds = new List<int>();
            lstTripIds = new List<int>();

           //Retrieve selected years
           List<int> lstYears = _vmTree.Years.Where(y => y.IsChecked == null || y.IsChecked.Value).Select(x => x.YearEntity.Year).Distinct().ToList();

           var datMan = new BusinessLogic.Offline.OfflineManager();
           var lstData = datMan.GetOfflineSelectionData(lstYears);

            foreach(var year in _vmTree.Years)
            {
                //If year is partially or fully checked, step into it and analyze which trips are checked.
                if (year.IsChecked == null)
                {
                    //only add trips that are checked
                    //lst.AddRange(GetSelectedTripsFromYear(year, lstData));
                    GetSelectedTripsFromYear(year, lstData, ref lstCruiseIds, ref lstTripIds);
                }
                else if (year.IsChecked.Value)
                {
                    //Add trips from a full year
                    //lst.AddRange(lstData.Where(d => d.Year == year.YearEntity.Year).Select(x => x.TripId).Distinct());
                    lstTripIds.AddRange(lstData.Where(d => d.Year == year.YearEntity.Year && d.TripId.HasValue).Select(x => x.TripId.Value).Distinct());
                    //Add cruise ids that have no trips
                    lstCruiseIds.AddRange(lstData.Where(d => d.Year == year.YearEntity.Year && !d.TripId.HasValue).Select(x => x.CruiseId).Distinct());
                }
                
            }
        }


        private void GetSelectedTripsFromYear(TreeView.YearTreeItemViewModel year, List<Entities.OfflineSelectionRecord> lstData, ref List<int> lstCruiseIds, ref List<int> lstTripIds)
        {
            foreach (var cruise in year.Children.OfType<TreeView.CruiseTreeItemViewModel>())
            {
                if (cruise.IsChecked == null)
                {
                    //Only add checked trips
                    lstTripIds.AddRange(cruise.Children.OfType<TreeView.TripTreeItemViewModel>().Where(x => x.IsChecked != null && x.IsChecked.Value).Select(x => x.TripEntity.tripId));
                }
                else if (cruise.IsChecked.Value)
                {
                    //Add all trips from checked cruise
                    lstTripIds.AddRange(lstData.Where(x => x.Year == year.YearEntity.Year && x.CruiseId == cruise.CruiseId && x.TripId.HasValue).Select(x => x.TripId.Value).Distinct());
                    //Add cruise ids with no trips
                    lstCruiseIds.AddRange(lstData.Where(x => x.Year == year.YearEntity.Year && x.CruiseId == cruise.CruiseId && !x.TripId.HasValue).Select(x => x.CruiseId).Distinct());
                }
                
            }
        }

        
        #endregion 


        #region Cancel Transfer Command


        public DelegateCommand CancelTransferCommand
        {
            get { return _cmdCancelTransfer ?? (_cmdCancelTransfer = new DelegateCommand(CancelTransfer)); }
        }


        private void CancelTransfer()
        {
            IsTransferCancelled = true;
        }


        #endregion


        public override void FireClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            base.FireClosing(sender, e);

            if (ProcessState != OfflineProcessState.Idle)
            {
                e.Cancel = true;
                DispatchMessageBox("Stop venligst overførslen, inden du lukker for vinduet.");
            }
        }

    }
}
