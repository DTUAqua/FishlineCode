using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.Prism.Commands;
using System.Threading.Tasks;
using Babelfisk.Entities;
using Anchor.Core;
using Babelfisk.BusinessLogic.Export;
using System.Diagnostics;

namespace Babelfisk.ViewModels.Export
{
    public class ExportDataViewModel : AViewModel
    {
        public enum ExportMethod
        {
            RaiseAndExportToCSV,
            RaiseAndExportToDW
        }

        private DelegateCommand _cmdOK;
        private DelegateCommand _cmdBrowse;
        private DelegateCommand _cmdCancelTransfer;
        private DelegateCommand _cmdOpenExplorer;
        private DelegateCommand _cmdResetPrefix;

        private TreeView.MainTreeViewModel _vmTree;

        private DataExportStatus _enmCruiseProcessState;

        private ExportProcessState _enmExportProcessState;

        private double _dblExportedPercentage = 0;
        private int _intCruisesToExport = 0;
        private int _intCruisesExported = 0;

        private bool _blnCancelled = false;

        private string _strFilenamePrefix;


    

        private ExportMethod _enmSelectedExportMethod;


        #region Properties


        public string FilenamePrefix
        {
            get { return _strFilenamePrefix; }
            set
            {
                if (_strFilenamePrefix != value)
                {
                    _strFilenamePrefix = value;
                    BusinessLogic.Settings.Settings.Instance.ExportFilePrefix = _strFilenamePrefix;
                }
                RaisePropertyChanged(() => FilenamePrefix);
            }
        }


        public ExportMethod SelectedExportMethod
        {
            get { return _enmSelectedExportMethod; }
            set
            {
                if (_enmSelectedExportMethod == value)
                    return;

                _enmSelectedExportMethod = value;
                RaisePropertyChanged(() => SelectedExportMethod);
                RaisePropertyChanged(() => IsRaisingAndExportingToCSV);
                RaisePropertyChanged(() => IsRaisingAndExportingToDW);
            }
        }


        public bool IsRaisingAndExportingToCSV
        {
            get { return SelectedExportMethod == ExportMethod.RaiseAndExportToCSV; }
            set { SelectedExportMethod = value ? ExportMethod.RaiseAndExportToCSV : SelectedExportMethod; }
        }

        public bool IsRaisingAndExportingToDW
        {
            get { return SelectedExportMethod == ExportMethod.RaiseAndExportToDW; }
            set { SelectedExportMethod = value ? ExportMethod.RaiseAndExportToDW : SelectedExportMethod; }
        }



        public bool IsExportCancelled
        {
            get { return _blnCancelled; }
            set
            {
                _blnCancelled = value;
                RaisePropertyChanged(() => IsExportCancelled);
            }
        }


        public double ExportedPercentage
        {
            get { return _dblExportedPercentage; }
            set
            {
                _dblExportedPercentage = value;
                RaisePropertyChanged(() => ExportedPercentage);
            }
        }


        public int CruisesToExport
        {
            get { return _intCruisesToExport; }
            set
            {
                _intCruisesToExport = value;
                RaisePropertyChanged(() => CruisesToExport);
            }
        }


        public int CruisesExported
        {
            get { return _intCruisesExported; }
            set
            {
                _intCruisesExported = value;
                RaisePropertyChanged(() => CruisesExported);
            }
        }


        public TreeView.MainTreeViewModel SelectionTree
        {
            get { return _vmTree; }
            set
            {
                _vmTree = value;
                RaisePropertyChanged(() => MainTree);
            }
        }


        public DataExportStatus CruiseProcessState
        {
            get { return _enmCruiseProcessState; }
            set
            {
                _enmCruiseProcessState = value;
                RaisePropertyChanged(() => CruiseProcessState);
            }
        }


        public ExportProcessState ExportProcessState
        {
            get { return _enmExportProcessState; }
            set
            {
                _enmExportProcessState = value;
                RaisePropertyChanged(() => ExportProcessState);
            }
        }

        
        public string ExportFolderPath
        {
            get { return BusinessLogic.Settings.Settings.Instance.ExportFolderPath; }
            set
            {
                BusinessLogic.Settings.Settings.Instance.ExportFolderPath = value;
                RaisePropertyChanged(() => ExportFolderPath);
                RaisePropertyChanged(() => HasExportFolderPath);
            }
           
        }


        public bool HasExportFolderPath
        {
            get { return !string.IsNullOrWhiteSpace(ExportFolderPath); }
        }


        public bool IsOffline
        {
            get { return BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline; }
        }



        public bool SaveRawDWTables
        {
            get { return BusinessLogic.Settings.Settings.Instance.SaveRawDWTables; }
            set
            {
                BusinessLogic.Settings.Settings.Instance.SaveRawDWTables = value;
                RaisePropertyChanged(() => SaveRawDWTables);
            }
        }


        #endregion


        public ExportDataViewModel(List<int> lstPreselectIds = null)
        {
            WindowWidth = 770;
            WindowHeight = 600;
            WindowTitle = "Eksporter oparbejdet data";
            _enmCruiseProcessState = DataExportStatus.Idle;
            _enmExportProcessState = ViewModels.ExportProcessState.Idle;

            SetDefaultPrefix();

            if (BusinessLogic.Settings.Settings.Instance.ExportFilePrefix != null)
                _strFilenamePrefix = BusinessLogic.Settings.Settings.Instance.ExportFilePrefix;
           
            InitializeTree(lstPreselectIds);
        }


        private void InitializeTree(List<int> lstPreselectIds = null)
        {
            if (lstPreselectIds != null && lstPreselectIds.Count == 2)
            {
                _vmTree = new TreeView.MainTreeViewModel(false, TreeView.MainTreeViewModel.TreeDepth.Sample, false);
                _vmTree.InitializeTreeViewAsync().ContinueWith(t => _vmTree.ExpandToAndSelectCruise(lstPreselectIds[0], lstPreselectIds[1]));
            }
            else if(lstPreselectIds != null && lstPreselectIds.Count == 3)
            {
                _vmTree = new TreeView.MainTreeViewModel(false, TreeView.MainTreeViewModel.TreeDepth.Sample, false);
                _vmTree.InitializeTreeViewAsync().ContinueWith(t => _vmTree.ExpandToAndSelectTrip(lstPreselectIds[0], lstPreselectIds[1], lstPreselectIds[2]));
            }
            else if (lstPreselectIds != null && lstPreselectIds.Count == 4)
            {
                _vmTree = new TreeView.MainTreeViewModel(false, TreeView.MainTreeViewModel.TreeDepth.Sample, false);
                _vmTree.InitializeTreeViewAsync().ContinueWith(t => _vmTree.ExpandToAndSelectSample(lstPreselectIds[0], lstPreselectIds[1], lstPreselectIds[2], lstPreselectIds[3]));
            }
            else
                _vmTree = new TreeView.MainTreeViewModel(false, TreeView.MainTreeViewModel.TreeDepth.Sample);
        }


        #region Browse Command


        public DelegateCommand BrowseCommand
        {
            get { return _cmdBrowse ?? (_cmdBrowse = new DelegateCommand(Browse)); }
        }


        private void Browse()
        {
            FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

            
            dlg.SelectedPath = ExportFolderPath;
            dlg.Description = "Vælg en destination.";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ExportFolderPath = dlg.SelectedPath;
            }
        }


        #endregion


        #region OK Command


        public DelegateCommand OKCommand
        {
            get { return _cmdOK ?? (_cmdOK = new DelegateCommand(OK)); }
        }


        private void OK()
        {
            IsExportCancelled = false;

            if (SelectedExportMethod == ExportMethod.RaiseAndExportToCSV && !Directory.Exists(ExportFolderPath))
            {
                AppRegionManager.ShowMessageBox("Den valgte destination (mappe) eksisterer ikke, vælg venligst en anden destination og prøv igen.");
                return;
            }


            if (SelectedExportMethod == ExportMethod.RaiseAndExportToCSV && Directory.GetFiles(ExportFolderPath).Length > 0 && BusinessLogic.Settings.Settings.Instance.ExportFilePrefix != null)
            {
                if (AppRegionManager.ShowMessageBox("Du har selv angivet et prefix til filerne hvilket betyder at der kan eksistere filer med samme navn i mappen du eksporterer til. Ønsker du at fortsætte og dermed overskrive eventuelle eksisterende filer?", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                    return;
            }


            ResetProgressParameters();
            ExportProcessState = ViewModels.ExportProcessState.Analyzing;
            CruiseProcessState = DataExportStatus.Idle;

            //Start exporting data
            Task.Factory.StartNew(ExportData).ContinueWith(t => SetExportProcessState(ViewModels.ExportProcessState.Idle));
        }



        private void ExportData()
        {
            //Retrieve sample ids
            List<CruiseIdPair> lstCruiseIdsWithNoTrips, lstTripIdsWithNoSamples, lstSampleIds;
            GetSelectedCruiseTripAndSampleIds(out lstCruiseIdsWithNoTrips, out lstTripIdsWithNoSamples, out lstSampleIds);

            if (lstCruiseIdsWithNoTrips.Count == 0 && lstTripIdsWithNoSamples.Count == 0 && lstSampleIds.Count == 0)
            {
                DispatchMessageBox("Vælg venligst det data fra træet, du ønsker at eksportere.");
                return;
            }

            SetExportProcessState(ViewModels.ExportProcessState.Exporting);

            //Export raised data

            List<Babelfisk.Warehouse.DWMessage> lstMessage = new List<Warehouse.DWMessage>();

            try
            {
                var exMan = new ExportManager();

                //Get a unique list of cruise ids
                var lstCruiseIds = lstSampleIds.Select(x => x.CruiseId).Distinct().ToList();
                lstCruiseIds.AddRange(lstTripIdsWithNoSamples.Select(x => x.CruiseId).Distinct());
                lstCruiseIds.AddRange(lstCruiseIdsWithNoTrips.Select(x => x.CruiseId).Distinct());
                lstCruiseIds = lstCruiseIds.Distinct().ToList();

                int intTotal = lstCruiseIds.Count;
                int intCurrentProgress = 0;
               
                //Reset progress.
                ReportProgress(0, intTotal, 0);

                //File prefix for exported files.
                string strPrefix = _strFilenamePrefix ?? "";

                bool blnFirstExport = true;

                //Export per cruise id.
                foreach (int cruiseId in lstCruiseIds)
                {
                    if (IsExportCancelled)
                        break;

                    //Retrieve all ids for current cruise id
                    var lstCWithNoT = lstCruiseIdsWithNoTrips.Where(x => x.CruiseId == cruiseId).Select(x => x.Id).ToList();
                    var lstTWithNoS = lstTripIdsWithNoSamples.Where(x => x.CruiseId == cruiseId).Select(x => x.Id).ToList();
                    var lstSamples = lstSampleIds.Where(x => x.CruiseId == cruiseId).Select(x => x.Id).ToList();

                    //Raise data
                    List<Babelfisk.Warehouse.Model.Cruise> lstRes = null;
                    var lst = exMan.RaiseData(SetCruiseProcessState, lstCWithNoT, lstTWithNoS, lstSamples, out lstRes);

                    if (lstRes != null && lstRes.Count > 1)
                        throw new ApplicationException("Two cruises was raised at the same time, only one is allowed.");

                    if (lst != null && lst.Count > 0)
                        lstMessage.AddRange(lst);

                    if (IsExportCancelled)
                        break;

                    //If exporting to CSV, save data to disk
                    if (IsRaisingAndExportingToCSV)
                    {
                        SetCruiseProcessState(DataExportStatus.SavingData);

                        if(SaveRawDWTables)
                            exMan.SaveDataToCSVTables(ExportFolderPath, strPrefix, lstRes, blnFirstExport);
                        else
                            exMan.SaveDataToCSVCombined(ExportFolderPath, strPrefix, lstRes, blnFirstExport);

                        blnFirstExport = false;
                    }
                    //If exporting to dataware house, insert data into data warehouse.
                    else if (IsRaisingAndExportingToDW && lstRes.Count == 1)
                    {
                        SetCruiseProcessState(DataExportStatus.SavingToDataWarehouse);

                        var c = lstRes.First();

                        List<Babelfisk.Warehouse.DWMessage> lstNewMessages = new List<Warehouse.DWMessage>();
                        var res = exMan.SaveCruiseToDataWarehouse(c, lst, ref lstNewMessages, false);
                        lstMessage.AddRange(lstNewMessages);

                        if (res.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                            throw new ApplicationException(res.Message);
                    }

                    lstRes = null;

                    //Make sure Mode.Cruises are collected, since they can be quite big.
                    GC.Collect();

                    ReportProgress(intCurrentProgress + 1, intTotal, (100.0 / (double)intTotal) * (intCurrentProgress + 1));
                    intCurrentProgress++;
                }

                //MDU 17.09.2018: Changed it so it only saves L_Species lookups to csv, when exporting to csv files (this seem to make most sense?).
                if(IsRaisingAndExportingToCSV)
                    exMan.SaveLookupTable<Babelfisk.Entities.Sprattus.L_Species>(ExportFolderPath, strPrefix, "Est_MethodStep", "R_SpeciesStock", "R_TargetSpecies", "R_TargetSpecies1", "SpeciesList", "ChangeTracker", "Id", "FilterValue", "treatmentFactorGroupUI", "DefaultSortValue", "UIDisplay", "CompareValue", "L_TreatmentFactorGroup", "OfflineState", "OfflineId", "OverwritingMethod", "OfflineComparisonEntity", "OfflineDeletedEntities", "CanDelete", "L_StandardLengthMeasureDisplay", "L_StandardLengthMeasureType", "R_StockSpeciesArea", "R_SDReader", "SDSample", "StandardLengthMeasureType");
            }
            catch (Exception e)
            {
                DispatchMessageBox("En uventet fejl opstod under eksporteringen. " + e.Message);
                Anchor.Core.Loggers.Logger.LogError(e);
                return;
            }

            //Sleep for a while so user can see the progressbar go to 100%
            System.Threading.Thread.Sleep(500);

            if (lstMessage != null && lstMessage.Count > 0)
            {
                //Update indexes (to global indexes over all cruises)
                for (int i = 0; i < lstMessage.Count; i++)
                    lstMessage[i].Index = i + 1;

                DWMessagesViewModel adw = new DWMessagesViewModel(lstMessage);
                new Action(() =>
                {
                    _appRegionManager.LoadWindowViewFromViewModel(adw, false, "WindowWithBorderStyle");
                }).Dispatch();
            }
            else
            {
                if (!IsExportCancelled)
                    DispatchMessageBox("Data blev eksporteret korrekt.", 3);
            }

            //Close export window after finishing export.
            new Action(() =>
            {
                _enmExportProcessState = ViewModels.ExportProcessState.Idle;

                if (!IsExportCancelled)
                    Close();
            }).Dispatch();

        }


        private void ResetProgressParameters()
        {
            ExportedPercentage = 0;
            CruisesExported = 0;
            CruisesToExport = 0;
        }

        private void SetExportProcessState(ExportProcessState status)
        {
            new Action(() =>
            {
                ExportProcessState = status;
            }).Dispatch();
        }

        private void SetCruiseProcessState(DataExportStatus status)
        {
            new Action(() =>
            {
                CruiseProcessState = status;
            }).Dispatch();
        }


        private void ReportProgress(int intCruisesExported, int intCruisesToExport, double dblExportedPercentage)
        {
            new Action(() =>
            {
                CruisesExported = intCruisesExported;

                if (_intCruisesToExport != intCruisesToExport)
                    CruisesToExport = intCruisesToExport;

                ExportedPercentage = dblExportedPercentage;
            }).Dispatch();
        }


        #endregion


        #region Cancel Transfer Command


        public DelegateCommand CancelTransferCommand
        {
            get { return _cmdCancelTransfer ?? (_cmdCancelTransfer = new DelegateCommand(CancelTransfer)); }
        }


        private void CancelTransfer()
        {
            IsExportCancelled = true;
        }


        #endregion


        #region Get selected data methods


        private void GetSelectedCruiseTripAndSampleIds(out List<CruiseIdPair> lstCruiseIds, out List<CruiseIdPair> lstTripIds, out List<CruiseIdPair> lstSampleIds)
        {
            lstCruiseIds = new List<CruiseIdPair>();
            lstTripIds = new List<CruiseIdPair>();
            lstSampleIds = new List<CruiseIdPair>();

            //Retrieve selected years
            List<int> lstYears = _vmTree.Years.Where(y => y.IsChecked == null || y.IsChecked.Value).Select(x => x.YearEntity.Year).Distinct().ToList();

            var datMan = new BusinessLogic.Offline.OfflineManager();
            var lstData = datMan.GetOfflineSelectionDataWithSample(lstYears);

            foreach (var year in _vmTree.Years)
            {
                //If year is partially, step into it and analyze which cruises are checked.
                if (year.IsChecked == null)
                {
                    //only add trips that are checked
                    GetSelectedCruiseTripAndSampleIds(year, lstData, ref lstCruiseIds, ref lstTripIds, ref lstSampleIds);
                }
                else if (year.IsChecked.Value)
                {
                    //Add data from whole year to lists.
                    var lstDataYearFiltered = lstData.Where(x => x.Year == year.YearEntity.Year);
                    lstSampleIds.AddRange(lstDataYearFiltered.Where(x => x.SampleId.HasValue).Select(x => new CruiseIdPair() { CruiseId = x.CruiseId, Id = x.SampleId.Value }).Distinct());

                    //Add trips with no samples
                    lstTripIds.AddRange(lstDataYearFiltered.Where(x => x.TripId.HasValue && !x.SampleId.HasValue).Select(x => new CruiseIdPair() { CruiseId = x.CruiseId, Id = x.TripId.Value }).Distinct());

                    //Add cruises with no trips
                    lstCruiseIds.AddRange(lstDataYearFiltered.Where(x => !x.TripId.HasValue).Select(x => new CruiseIdPair() { CruiseId = x.CruiseId, Id = x.CruiseId }).Distinct());
                }

            }
        }


        private void GetSelectedCruiseTripAndSampleIds(TreeView.YearTreeItemViewModel year, List<Entities.OfflineSelectionRecord> lstData, ref List<CruiseIdPair> lstCruiseIds, ref List<CruiseIdPair> lstTripIds, ref List<CruiseIdPair> lstSampleIds)
        {
            foreach (var cruise in year.Children.OfType<TreeView.CruiseTreeItemViewModel>())
            {
                //If cruise is partially, step into it and analyze which trips and samples are checked.
                if (cruise.IsChecked == null)
                {
                    //lstCruiseIds.Add(cruise.CruiseEntity.cruiseId);
                    GetSelectedCruiseTripAndSampleIds(year, cruise, lstData, ref lstCruiseIds, ref lstTripIds, ref lstSampleIds);
                    // lstTripIds.AddRange(cruise.Children.OfType<TreeView.TripTreeItemViewModel>().Where(x => x.IsChecked != null && x.IsChecked.Value).Select(x => x.TripEntity.tripId));
                }
                else if (cruise.IsChecked.Value)
                {
                    //Add data from whole cruise to lists.
                    var lstDataYearCruiseFiltered = lstData.Where(x => x.Year == year.YearEntity.Year && x.CruiseId == cruise.CruiseEntity.cruiseId);
                    lstSampleIds.AddRange(lstDataYearCruiseFiltered.Where(x => x.SampleId.HasValue).Select(x => new CruiseIdPair() { CruiseId = x.CruiseId, Id = x.SampleId.Value }).Distinct());

                    //Add trips with no samples
                    lstTripIds.AddRange(lstDataYearCruiseFiltered.Where(x => x.TripId.HasValue && !x.SampleId.HasValue).Select(x => new CruiseIdPair() { CruiseId = x.CruiseId, Id = x.TripId.Value }).Distinct());

                    //Ad cruises with no trips
                    lstCruiseIds.AddRange(lstDataYearCruiseFiltered.Where(x => !x.TripId.HasValue).Select(x => new CruiseIdPair() { CruiseId = x.CruiseId, Id = x.CruiseId }).Distinct());
                }

            }
        }


        private void GetSelectedCruiseTripAndSampleIds(TreeView.YearTreeItemViewModel year, TreeView.CruiseTreeItemViewModel cruise, List<Entities.OfflineSelectionRecord> lstData, ref List<CruiseIdPair> lstCruiseIds, ref List<CruiseIdPair> lstTripIds, ref List<CruiseIdPair> lstSampleIds)
        {
            foreach (var trip in cruise.Children.OfType<TreeView.TripTreeItemViewModel>())
            {
                if (trip.IsChecked == null)
                {
                    //Only add checked trips
                    //lstTripIds.Add(trip.TripEntity.tripId);
                    lstSampleIds.AddRange(trip.Children.OfType<TreeView.SampleTreeItemViewModel>().Where(x => x.IsChecked.HasValue && x.IsChecked.Value).Select(x => new CruiseIdPair() { CruiseId = lstData.Where(y => y.SampleId == x.SampleEntity.sampleId).FirstOrDefault().CruiseId, Id = x.SampleEntity.sampleId }));
                }
                else if (trip.IsChecked.Value)
                {
                    //Add data from whole cruise to lists.
                    var lstDataYearCruiseFiltered = lstData.Where(x => x.Year == year.YearEntity.Year && x.CruiseId == cruise.CruiseEntity.cruiseId && x.TripId == trip.TripEntity.tripId);
                    lstSampleIds.AddRange(lstDataYearCruiseFiltered.Where(x => x.SampleId.HasValue).Select(x => new CruiseIdPair() { CruiseId = x.CruiseId, Id = x.SampleId.Value }).Distinct());

                    //Add trips with no samples
                    lstTripIds.AddRange(lstDataYearCruiseFiltered.Where(x => !x.SampleId.HasValue).Select(x => new CruiseIdPair() { CruiseId = x.CruiseId, Id = x.TripId.Value }).Distinct());

                }

            }
        }

        #endregion


        #region Reset Prefix Command


        public DelegateCommand ResetPrefixCommand
        {
            get { return _cmdResetPrefix ?? (_cmdResetPrefix = new DelegateCommand(ResetPrefix)); }
        }


        private void ResetPrefix()
        {
            BusinessLogic.Settings.Settings.Instance.ExportFilePrefix = null;
            SetDefaultPrefix();
            RaisePropertyChanged(() => FilenamePrefix);
        }


        private void SetDefaultPrefix()
        {
            _strFilenamePrefix = " " + DateTime.Now.ToString("dd-MM-yyyy-HHmmss");
        }


        #endregion


        #region Open Explorer Command


        public DelegateCommand OpenExplorerCommand
        {
            get { return _cmdOpenExplorer ?? (_cmdOpenExplorer = new DelegateCommand(OpenExplorer)); }
        }


        private void OpenExplorer()
        {
            if (!HasExportFolderPath || !Directory.Exists(ExportFolderPath))
            {
                AppRegionManager.ShowMessageBox("Den valgte sti (mappe) eksisterer ikke, ret venligst stien og prøv igen.");
                return;
            }

            try
            {
                Process.Start(ExportFolderPath);
            }
            catch { }
        }



        #endregion



        /// <summary>
        /// Event fired when window is about to be closed.
        /// </summary>
        public override void FireClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            base.FireClosing(sender, e);

            if (ExportProcessState != ExportProcessState.Idle)
            {
                e.Cancel = true;
                DispatchMessageBox("Stop venligst overførslen, inden du lukker for vinduet.");
            }
        }



        /// <summary>
        /// Struct used in determining what data is selected in the tree (see Get selected data methods)
        /// </summary>
        private struct CruiseIdPair
        {
            public int CruiseId { get; set; }
            public int Id { get; set; }
        }
    }
}
