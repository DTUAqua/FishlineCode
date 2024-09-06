using Anchor.Core;
using FishLineMeasure.ViewModels.Overview;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels.Export
{
    public class ExpoertViewModel : AViewModel
    {
        private DelegateCommand _CMDSetFilePathCommand;
        private DelegateCommand _CMDCloseCommand;
        private DelegateCommand _CMDExport;


        private string _Path;

        private bool _CSVChecked;

        private ObservableCollection<TripItemsHandler> _TreeViewTop;


        #region Properties


        public ObservableCollection<TripItemsHandler> TreeViewTop
        {
            get { return _TreeViewTop; }
            set
            {
                _TreeViewTop = value;
                RaisePropertyChanged(nameof(TreeViewTop));
            }
        }

      
        public string Path
        {
            get { return _Path; }
            set
            {
                _Path = value;
                RaisePropertyChanged(nameof(Path));
                PushUIMessage($"PathChanged_{Path.Length}");
            }
        }


        public bool CSVChecked
        {
            get { return _CSVChecked; }
            set
            {
                _CSVChecked = value;
                RaisePropertyChanged(nameof(CSVChecked));
            }
        }


        #endregion


        /// <summary>
        /// This constructor is for UI design view only.
        /// </summary>
       /* private ExpoertViewModel()
        {
            WindowHeight = 550;
            WindowWidth = 860;
            AdjustWindowWidthHeightToScreen();
            CSVChecked = AppSettings.ExportToCSVAsDefault;
            TreeViewTop = new ObservableCollection<TripItemsHandler>();
            TreeViewTop.Add(new TripItemsHandler(new TripViewModel(){ Trip = "1", Stations = new ObservableCollection<StationViewModel>() { new StationViewModel() { StationNumber = "1" } } }));
            TreeViewTop.Add(new TripItemsHandler(new TripViewModel() { Trip = "2", Stations = new ObservableCollection<StationViewModel>() { new StationViewModel() { StationNumber = "1" }, new StationViewModel() { StationNumber = "2" } } }));
        }*/


        public ExpoertViewModel(TripViewModel trip, StationViewModel Selected)
        {
            WindowHeight = 550;
            WindowWidth = 860;
            WindowTitle = "Eksporter en eller flere stationer";
            AdjustWindowWidthHeightToScreen();
            CSVChecked = AppSettings.ExportToCSVAsDefault;
           // var c = new Anchor.Core.Comparers.NaturalComparer();
            TreeViewTop = trip == null ? new ObservableCollection<TripItemsHandler>() : new ObservableCollection<TripItemsHandler>() { new TripItemsHandler(trip) { IsExpanded = true } };
            MarkSelectedTrip(Selected);
        }


        private void MarkSelectedTrip(StationViewModel selected)
        {
            if (selected == null)
                return;

            var itm = TreeViewTop.SelectMany(x => x.tStations).Where(x => x.Name == selected.StationNumber).FirstOrDefault();

            //Make sure selected trip is expanded
            if (itm != null)
            {
                if(itm.Parent != null)
                    itm.Parent.IsExpanded = true;

                itm.IsChecked = true;
            }
        }



        #region Set File Path Command


        public DelegateCommand SetFilePathCommand
        {
            get { return _CMDSetFilePathCommand ?? (_CMDSetFilePathCommand = new DelegateCommand(OpenFileDialogC)); }
        }


        private void OpenFileDialogC()
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog1 = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog1.Filter = "csv Fil|*.csv";
            saveFileDialog1.Title = "Gem CSV-fil";
            string DatetimeNow = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            var tripItem = TreeViewTop.First();
            var trip = tripItem.Trip;
            saveFileDialog1.FileName = $"Tur_{trip.Year}-{trip.Cruise}-{trip.Trip}_{DatetimeNow}";
            Nullable<bool> result = saveFileDialog1.ShowDialog();

            if (result == true)
            {
                Path = saveFileDialog1.FileName;
            }
        }


        #endregion


        #region Close Command


        public DelegateCommand CloseCommand
        {
            get { return _CMDCloseCommand ?? (_CMDCloseCommand = new DelegateCommand(CloseThisWindow)); }
        }


        private void CloseThisWindow()
        {
            this.Close();
        }


        #endregion


        #region Export Command

        public DelegateCommand ExportCommand
        {
            get { return _CMDExport ?? (_CMDExport = new DelegateCommand(Save)); }
        }



        private void Save()
        {
            if(!TreeViewTop.SelectMany(x => x.tStations).Where(x => x.IsChecked.HasValue && x.IsChecked.Value).Any())
            {
                DispatchMessageBox("Vælg venligst en eller flere stationer før der eksporteres.");
                return;
            }

            //Store selection for next time.
            AppSettings.ExportToCSVAsDefault = CSVChecked;

            if (CSVChecked == true)
                SaveAsCSV();
            else
                ExportToServer();
        }


        public void SaveAsCSV()
        {
            if (Path != null)
            {
                try
                {
                    var tempList = new List<StationViewModel>();

                    //Select all checked stations.
                    tempList = TreeViewTop.SelectMany(x => x.tStations)
                                          .Where(x => x.IsChecked.HasValue && x.IsChecked.Value)
                                          .Select(x => x.VmStation)
                                          .ToList();

                    var Converter = new XMLConvertCSV(tempList, Path);

                    DispatchMessageBox("CSV-filen blev gemt korrekt.");
                }
                catch (Exception ex)
                {
                    DispatchMessageBox("En uventet fejl opstod. \r\n" + $"{ex.Message}");
                }

            }
            else
            {
                DispatchMessageBox("Vælg venligst en destination til csv-filen, inden der eksporteres.");
            }

        }

        public void ExportToServer()
        {
            //Log in, if user is not logged in or if user has not logged in within the last 30 minutes.
            if (BusinessLogic.Settings.Settings.CurrentUser == null || !BusinessLogic.Settings.Settings.Instance.LastLoginTime.HasValue || Math.Abs((DateTime.UtcNow - BusinessLogic.Settings.Settings.Instance.LastLoginTime.Value).TotalMinutes) > 30)
            {
                var vmLogin = new FishLineLoginViewModel();

                AppRegionManager.LoadWindowViewFromViewModel(vmLogin, true, "WindowToolBox");

                if (vmLogin.LoggedInUser == null)
                    return;
            }

            var stations    = TreeViewTop.SelectMany(x => x.tStations)
                                         .Where(x => x.IsChecked.HasValue && x.IsChecked.Value)
                                         .Select(x => x.VmStation)
                                         .ToList();


            var vmExport = new FishLineExportViewModel(stations);

            //Start analysis of stations and existing FishLine data.
            vmExport.AnalyzeDataAsync();

            AppRegionManager.LoadWindowViewFromViewModel(vmExport, true, "WindowToolBox");
        }



        #endregion

      

      


      
    }
}
