using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Anchor.Core;
using Microsoft.Practices.Prism.Commands;
using System.ComponentModel;
using System.Xml.Linq;
using FishLineMeasure.ViewModels.Lenghts;
using FishLineMeasure.ViewModels.Export;

namespace FishLineMeasure.ViewModels.Overview
{
    public class OverviewViewModel : AViewModel
    {
        private DelegateCommand _cmdNewTrip;
        private DelegateCommand _cmdDeleteTrip;
        private DelegateCommand _cmdNewStation;
        private DelegateCommand _cmdDeleteStation;
        private DelegateCommand _cmdEditTrip;
        private DelegateCommand _cmdEditStation;
        private DelegateCommand _cmdLenghtView;
        private DelegateCommand _cmdCreateCSV;

        #region Properties

        private ObservableCollection<TripViewModel> _lstTrips;

        private TripViewModel _selectedTrip;

        private StationViewModel _selectedStation;

        public ObservableCollection<TripViewModel> Trips
        {
            get { return _lstTrips; }
            set
            {
                _lstTrips = value;
                RaisePropertyChanged(nameof(Trips));
            }
        }


        public TripViewModel SelectedTrip
        {
            get { return _selectedTrip; }
            set
            {
                _selectedTrip = value;
                RaiseSelectedTrip();
                onSelectedTripChanged();
            }
        }

        private void RaiseSelectedTrip()
        {
            RaisePropertyChanged(nameof(SelectedTrip));
            RaisePropertyChanged(nameof(HasSelectedTrip));
        }


        public bool HasSelectedTrip
        {
            get { return _selectedTrip != null; }
        }


        public StationViewModel SelectedStation
        {
            get { return _selectedStation; }
            set
            {
                _selectedStation = value;
                RaiseSelectedStation();
                onSelectedStationChanged();
            }
        }

        private void RaiseSelectedStation()
        {
            RaisePropertyChanged(nameof(SelectedStation));
            RaisePropertyChanged(nameof(HasSelectedStation));
        }

        public bool HasSelectedStation
        {
            get { return _selectedStation != null; }
        }

        public bool MoreStations { get; set; }
        

        #endregion

        public OverviewViewModel()
        {
            _lstTrips = new ObservableCollection<TripViewModel>();
        }


        public OverviewViewModel(OverviewViewModel copy)
        {
            MoreStations = true;
            int indexOfStation = copy.SelectedTrip.Stations.IndexOf(copy.SelectedStation);
            SelectedStation = copy.SelectedTrip.Stations[indexOfStation + 1];
            if (indexOfStation + 2 >= copy.SelectedTrip.Stations.Count)
            {
                MoreStations = false;
            }
            SelectedTrip = copy.SelectedTrip;
        }

        #region SelectedChanged 

        private void onSelectedTripChanged()
        {
            if (SelectedTrip != null)
            {
                AppSettings.DefaultTrip = SelectedTrip.Key;
            }
            AppSettings.Save();
        }
        private void onSelectedStationChanged()
        {
            if (SelectedTrip != null)
            {
                AppSettings.DefaultTrip = SelectedTrip.Key;
                if (SelectedStation != null)
                {
                    AppSettings.DefaultStation = SelectedStation.StationNumber;
                }
            }
            AppSettings.Save();
        }


        #endregion

        #region On Application Startup 

        private void SetSelectedFromLastUse()
        {
            if (!string.IsNullOrEmpty(AppSettings.DefaultTrip))
            {
                _selectedTrip = Trips.Where(trip => trip.Key == AppSettings.DefaultTrip).FirstOrDefault();
                RaiseSelectedTrip();

                if (_selectedTrip != null && !string.IsNullOrEmpty(AppSettings.DefaultStation))
                {
                    if (_selectedTrip.Stations.Any(s => s.StationNumber == AppSettings.DefaultStation))
                    {
                        foreach (var station in _selectedTrip.Stations)
                        {
                            if (station.StationNumber == AppSettings.DefaultStation)
                            {
                                _selectedStation = station;
                                RaiseSelectedStation();
                                break;
                            }
                        }
                    }

                }
            }
        }


        public Task InitializeAsync()
        {
            IsLoading = true;

            var t = AddAndEditTripViewModel.LoadTripsAsync();

            //Make sure to reset loading flag (on main thread, therefore the Dispatch), when done loading trips. 
            var tDone = t.ContinueWith(th => new Action(() =>
            {
                Trips = t.Result.OrderByDescending(trip => trip.Year).ThenByDescending(trip => trip.Cruise).ThenByDescending(trip => trip.Trip).ToObservableCollection();
                foreach (var aTrip in Trips)
                {
                    aTrip.Stations = aTrip.Stations.OrderByDescending(x => x.StationNumber).ToObservableCollection();
                }
                SetSelectedFromLastUse();
                IsLoading = false;
            }).Dispatch())
            .ContinueWith(th => new Action(() =>
            {
                if (AppSettings.UpdateLookupsAfterStartup || !Directory.Exists(AppSettings.OfflineLookupDataPath) || Directory.GetFiles(AppSettings.OfflineLookupDataPath).Length == 0)
                {
                    if (!AppSettings.UpdateLookupsAfterStartup && AppRegionManager.ShowMessageBox("Programmet mangler at hente lookup-tabeller. Ønsker du at gøre dette nu (kræver en internetforbindelse)?", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                        return;

                    var vmLookups = new Lookups.LookupsViewModel();
                    var task = vmLookups.SyncLookupsAsync();
                    if (!AppSettings.UpdateLookupsAfterStartup)
                    {
                        task.ContinueWith(ta => new Action(() =>
                        {
                            vmLookups.Close();
                        }).Dispatch());
                        AppRegionManager.LoadWindowViewFromViewModel(vmLookups);
                    }
                }
            }).Dispatch());

            return tDone;
        }
        #endregion

        #region New Trip Command

        public DelegateCommand NewTripCommand
        {
            get { return _cmdNewTrip ?? (_cmdNewTrip = new DelegateCommand(NewTrip)); }
        }


        private void OrderTrips()
        {
            if (Trips == null)
                return;

            Trips = Trips.OrderByDescending(t => t.Year).ThenByDescending(t => t.Cruise).ThenByDescending(t => t.Trip).ToObservableCollection();
        }

        public void NewTrip()
        {
            try
            {
                var vmTrip = new AddAndEditTripViewModel();
                AppRegionManager.LoadWindowViewFromViewModel(vmTrip, true, "WindowToolBox");
                if (vmTrip.IsDirty == true)
                {
                    if (vmTrip.SelectedConfiguration == "Add")
                    {
                        Directory.CreateDirectory(Path.Combine(AppSettings.DataRootPath, $"[{vmTrip.Year}]_[{vmTrip.Cruise}]_[{vmTrip.Trip}]"));

                        var trip = new TripViewModel()
                        {
                            Year = vmTrip.Year.ToInt32(),
                            Cruise = vmTrip.Cruise,
                            Trip = vmTrip.Trip,
                            Path = $@"{AppSettings.DataRootPath}\[{vmTrip.Year}]_[{vmTrip.Cruise}]_[{vmTrip.Trip}]",
                            NameOfFolder = $@"[{vmTrip.Year}]_[{vmTrip.Cruise}]_[{vmTrip.Trip}]"

                        };

                        Directory.CreateDirectory(Path.Combine(AppSettings.DataRootPath, trip.NameOfFolder));
                        _lstTrips.Add(trip);

                        OrderTrips();
                        SelectedTrip = trip;
                    }
                    else
                    {
                        DispatchMessageBox("En uventet fejl opstod under tilføjelse af den nye tur");
                    }
                }
            }
            catch(Exception e)
            {
                LogError(e);

                DispatchMessageBox($"En uventet fejl opstod. {e.Message ?? ""}");
            }
        }
        #endregion

        #region Edit Selected Trip Command

        public DelegateCommand EditTripCommand
        {
            get { return _cmdEditTrip ?? (_cmdEditTrip = new DelegateCommand(EditTrip)); }
        }

        private void EditTrip()
        {
            try
            {
                if (SelectedTrip != null)
                {
                    int indexOfTrips = Trips.IndexOf(SelectedTrip);
                    var vmTrip = new AddAndEditTripViewModel(SelectedTrip);
                    AppRegionManager.LoadWindowViewFromViewModel(vmTrip, true, "WindowToolBox");
                    if (vmTrip.IsDirty == true)
                    {
                        IsLoading = true;

                        Task.Factory.StartNew(() =>
                        {
                            var selectedStation = SelectedStation;
                            SelectedTrip = null;
                            string moveTo = Path.Combine(AppSettings.DataRootPath, vmTrip.PathCreated);
                            string moveFrom = Trips[indexOfTrips].Path;

                            if (Directory.Exists(moveTo))
                                Directory.Delete(moveTo);

                            Directory.Move(moveFrom, moveTo);

                            var currentTrip = Trips[indexOfTrips];

                            currentTrip.Year = Convert.ToInt32(vmTrip.Year);
                            currentTrip.Cruise = vmTrip.Cruise;
                            currentTrip.Trip = vmTrip.Trip;
                            currentTrip.Path = moveTo;
                            currentTrip.NameOfFolder = vmTrip.PathCreated;

                            var files = Directory.GetFiles(moveTo);
                            if (files.Length != 0)
                            {
                                for (int i = 0; i < files.Length; i++)
                                {
                                    MakeChangesToXMLviaTrip(files[i], currentTrip);
                                }
                            }

                            new Action(() =>
                            {
                                // removes and adds it to trigger Key to change
                                Trips.Remove(currentTrip);
                                Trips.Add(currentTrip);
                                OrderTrips();

                                SelectedTrip = currentTrip;
                                SelectedStation = selectedStation;
                            }).Dispatch();

                        }).
                        ContinueWith(t =>
                        {
                            new Action(() =>
                            {
                                IsLoading = false;
                            }).Dispatch();
                        });
                    }
                }
            }
            catch(Exception e )
            {
                LogError(e);
                DispatchMessageBox($"En uventet fejl opstod. {e.Message ?? ""}");
            }
        }

        private void MakeChangesToXMLviaTrip(string v, TripViewModel tripToChange)
        {
            string stationFileName = Path.GetFileName(v);
            string stationName = stationFileName.Split(new string[] { "[", "]_[", "]" }, StringSplitOptions.None)[4];
            var xElmRoot = XElement.Load(v);
            var xHeader = xElmRoot.Element("Header");

            int intYear = tripToChange.Year;
            string cruise = tripToChange.Cruise;
            string trip = tripToChange.Trip;
            xHeader.Element("Year").Value = Convert.ToString(intYear);
            xHeader.Element("Cruise").Value = cruise;
            xHeader.Element("Trip").Value = trip;
            xElmRoot.Save(v);
            string newNameAndPath = Path.Combine(AppSettings.DataRootPath, Path.Combine(tripToChange.NameOfFolder, $"{tripToChange.NameOfFolder}_[{stationName}].xml"));
            File.Move(v, newNameAndPath);


            foreach (var station in tripToChange.Stations)
            {
                if (station.StationNumber == stationName)
                {
                    station.FilePath = newNameAndPath;
                }
            }

        }

        #endregion

        #region Delete Selected Trip Command

        public DelegateCommand DeleteTripCommand
        {
            get { return _cmdDeleteTrip ?? (_cmdDeleteTrip = new DelegateCommand(DeleteTrip)); }
        }

        private void DeleteTrip()
        {
            if (AppRegionManager.ShowMessageBox($"Er du sikker på du vil slette {SelectedTrip.Key}", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
            {
                return;
            }
            var tripToDelete = SelectedTrip;
            var amountOfBackups = Directory.GetFiles(AppSettings.BackupRootPath);
            if (amountOfBackups.Length >= AppSettings.NumberOfBackupFiles)
            {
                try
                {
                    if (amountOfBackups.Length == AppSettings.NumberOfBackupFiles)
                    {
                        FileSystemInfo fileInfo = new DirectoryInfo(AppSettings.BackupRootPath).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).First();
                        File.Delete(fileInfo.FullName);
                    }
                    else
                    {
                        int amountToDelete = AppSettings.NumberOfBackupFiles - amountOfBackups.Length;
                        for (int i = 0; i < amountToDelete; i++)
                        {
                            FileSystemInfo fileInfo = new DirectoryInfo(AppSettings.BackupRootPath).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).First();
                            File.Delete(fileInfo.FullName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DispatchMessageBox("Kunne ikke slette backups");
                }

            }
            List<string> ErrorsFound = new List<string>();

            foreach (string file_name in Directory.GetFiles(tripToDelete.Path))
            {
                try
                {
                    int index = tripToDelete.Stations.ToList().FindIndex(x => x.FilePath == file_name);    
                    string stationName = Path.GetFileName(file_name).Split(new string[] { "[", "]_[", "]" }, StringSplitOptions.None).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray()[3];
                    string utCNow = DateTime.UtcNow.ToString().Replace("/", ".").Replace(":", ".");
                    string newName = Path.Combine(Path.Combine(AppSettings.BackupRootPath), Path.GetFileNameWithoutExtension(tripToDelete.Stations[index].FilePath) + $" - {utCNow}.xml");
                    File.Copy(file_name, newName);
                    File.SetCreationTime(newName, DateTime.Now);
                    tripToDelete.Stations.Remove(tripToDelete.Stations[index]);
                    File.Delete(file_name);
                }
                catch (Exception)
                {
                    string stationName =
                    Path.GetFileName(file_name).Split(new string[] { "[", "]_[", "]" }, StringSplitOptions.None).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray()[3];
                    if (ErrorsFound.Count == 0)
                    {
                        ErrorsFound.Add("Denne stationFejlede");
                    }
                    ErrorsFound.Add(stationName);
                    if (ErrorsFound.Count == 3)
                    {
                        ErrorsFound[0] = "Disse stationer fejlede";
                    }
                }

            }
            
            if (ErrorsFound.Count == 0 )
            {
                Directory.Delete(tripToDelete.Path, true);
                SelectedTrip = null;
                Trips.Remove(tripToDelete);
            }
            else
            {
                DispatchMessageBox(String.Join(Environment.NewLine, ErrorsFound.Where(x => x != null)));
            }

         
        }

        #endregion

        #region New Station Command

        public DelegateCommand AddNewStationCommand
        {
            get { return _cmdNewStation ?? (_cmdNewStation = new DelegateCommand(NewStation)); }
        }

        private void NewStation()
        {
            var vmStation = new AddAndEditStationViewModel(SelectedTrip);
            AppRegionManager.LoadWindowViewFromViewModel(vmStation, true, "WindowToolBox");
            if (vmStation.IsDirty == true)
            {
                if (CreateXMLFormat(vmStation.StationNumber) == true)
                {
                    var station = new StationViewModel()
                    {
                        StationNumber = vmStation.StationNumber,
                        FilePath = Path.Combine(vmStation.Parent.Path, $"[{vmStation.Parent.Year}]_[{vmStation.Cruise}]_[{vmStation.Trip}]_[{vmStation.StationNumber}].xml"),
                        ParentTrip = SelectedTrip
                    };

                    SelectedTrip.AddStation(station, true);
                    SelectedStation = station;
                }
            }
        }

        private bool CreateXMLFormat(string stationNumber)
        {
            try
            {
                XDocument xmlDocument = new XDocument(
                new XDeclaration("1,0", "utf-8", "yes"),
                new XElement("Data", new XAttribute("Version", 1),
                   new XElement("Header",
                       new XElement("Year", $"{SelectedTrip.Year}"),
                       new XElement("Cruise", $"{SelectedTrip.Cruise}"),
                       new XElement("Trip", $"{SelectedTrip.Trip}"),
                       new XElement("StationNumber", $"{stationNumber}")),

                   new XElement("Measurements"))
               );

                string path = Path.Combine(AppSettings.DataRootPath, Path.Combine(SelectedTrip.NameOfFolder, $"[{SelectedTrip.Year}]_[{SelectedTrip.Cruise}]_[{SelectedTrip.Trip}]_[{stationNumber}].xml"));
                xmlDocument.Save(path);
            }
            catch (Exception)
            {
                DispatchMessageBox("Kunne ikke lave stationen");
                return false;
            }
            return true;
        }

        #endregion

        #region Edit Selected Station Command

        public DelegateCommand EditStationCommand
        {
            get { return _cmdEditStation ?? (_cmdEditStation = new DelegateCommand(EditStation)); }
        }

        private void EditStation()
        {
            if (SelectedTrip != null)
            {
                if (SelectedStation != null)
                {
                    var vmStation = new AddAndEditStationViewModel(SelectedStation);
                    AppRegionManager.LoadWindowViewFromViewModel(vmStation, true, "WindowToolBox");
                    if (vmStation.IsDirty == true)
                    {
                        int indexOfTrips = Trips.IndexOf(SelectedTrip);
                        string indexOfStaion = null;
                        for (int i = 0; i < SelectedTrip.Stations.Count; i++)
                        {
                            if (SelectedStation.StationNumber == SelectedTrip.Stations[i].StationNumber)
                            {
                                indexOfStaion = Convert.ToString(i);
                                break;
                            }
                        }
                        if (!string.IsNullOrEmpty(indexOfStaion))
                        {
                            string path = SelectedStation.FilePath;
                            MakeChangesToXMLviaStation(path, vmStation.StationNumber, vmStation.Parent);
                            Trips[indexOfTrips].Stations[Convert.ToInt32(indexOfStaion)].StationNumber = vmStation.StationNumber;
                            File.Delete(path);
                            SelectedStation.FilePath = Path.Combine(vmStation.Parent.Path, $"{vmStation.Parent.NameOfFolder}_[{SelectedStation.StationNumber}].xml");
                        }
                    }
                }
            }

        }

        private void MakeChangesToXMLviaStation(string path, string stationNumber, TripViewModel parent)
        {
            string stationFileName = Path.GetFileName(path);
            string newPath = Path.Combine(AppSettings.DataRootPath, Path.Combine(parent.NameOfFolder, $"{parent.NameOfFolder}_[{stationNumber}].xml"));
            var xElmRoot = XElement.Load(path);
            XElement xStationNumber = null;

            var xElmHeader = xElmRoot.Element("Header");
            if (xElmHeader != null)
                xStationNumber = xElmHeader.Element("StationNumber");
            else
            {
                xStationNumber = new XElement("StationNumber");
                xElmHeader.Add(xStationNumber);
            }

            //Legacy, when station number was placed the wrong place.
            if (xElmRoot.Element("StationNumber") != null)
                xElmRoot.Element("StationNumber").Remove();

            xStationNumber.Value = stationNumber;
            xElmRoot.Save(newPath);
        }



        #endregion

        #region Delete Seleceted Station COmmand

        public DelegateCommand DeleteStationCommand
        {
            get { return _cmdDeleteStation ?? (_cmdDeleteStation = new DelegateCommand(DeleteStation)); }
        }

        private void DeleteStation()
        {
            if (AppRegionManager.ShowMessageBox($"Er du sikker på du vil slette {SelectedStation.StationNumber}", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
            {
                return; 
            }
            var stationToDelete = SelectedStation;
            var amountOfBackups = Directory.GetFiles(AppSettings.BackupRootPath);
            if (amountOfBackups.Length >= AppSettings.NumberOfBackupFiles)
            {
                try
                {
                    if (amountOfBackups.Length == AppSettings.NumberOfBackupFiles)
                    {
                        FileSystemInfo fileInfo = new DirectoryInfo(AppSettings.BackupRootPath).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).First();
                        File.Delete(fileInfo.FullName);
                    }
                    else
                    {
                        int amountToDelete = AppSettings.NumberOfBackupFiles - amountOfBackups.Length;
                        for (int i = 0; i < amountToDelete; i++)
                        {
                            FileSystemInfo fileInfo = new DirectoryInfo(AppSettings.BackupRootPath).GetFileSystemInfos().OrderBy(fi => fi.CreationTime).First();
                            File.Delete(fileInfo.FullName);
                        }
                    }
                }
                catch (Exception)
                {
                    DispatchMessageBox("Kunne ikke slette backups");
                }
            }
            try
            {
                string utCNow = DateTime.UtcNow.ToString().Replace("/",".").Replace(":",".");
                string newName = Path.Combine(Path.Combine(AppSettings.BackupRootPath), Path.GetFileNameWithoutExtension(stationToDelete.FilePath) + $" - {utCNow}.xml");
                File.Copy(stationToDelete.FilePath, newName);
                File.SetCreationTime(newName, DateTime.Now);
                SelectedStation = null;
                File.Delete(stationToDelete.FilePath);
                SelectedTrip.Stations.Remove(stationToDelete);

            }
            catch (Exception eX)
            {
                var m = eX.Message;
                DispatchMessageBox($"Kunne ikke fjerne {SelectedStation.StationNumber}");
                
            }
    

        }

        #endregion

        public DelegateCommand CreateCSVFile
        {
            get { return _cmdCreateCSV ?? (_cmdCreateCSV = new DelegateCommand(CSVFile)); }
        }

        private void CSVFile()
        {
            if(IsLoading)
            {
                AppRegionManager.ShowMessageBox("Vent venligst til formen er loadet færdig, før der eksporteres.");
                return;
            }

            if (SelectedTrip == null)
            {
                AppRegionManager.ShowMessageBox("Vælg venligst en tur, inden der eksporteres.");
                return;
            }

            if (SelectedTrip.Stations == null || SelectedTrip.Stations.Count == 0)
            {
                AppRegionManager.ShowMessageBox("Den valgte tur har ingen stationer at eksportere, vælg venligst en anden tur først.");
                return;
            }

            /* if (SelectedStation == null)
             {
                 var amount = SelectedTrip.Stations.Count();
                 if (amount > 0)
                     SelectedStation = SelectedTrip.Stations[0];
             }*/


            var VM = new ExpoertViewModel(SelectedTrip, SelectedStation);
            AppRegionManager.LoadWindowViewFromViewModel(VM, true, "WindowToolBox");
            //XMLConvertCSV converter = new XMLConvertCSV(SelectedStation);
        }

        #region Open Lenght ViewWindow 

        public DelegateCommand LenghtViewCommand
        {
            get { return _cmdLenghtView ?? (_cmdLenghtView = new DelegateCommand(LenghtWindowMain)); }
        }

        private void LenghtWindowMain()
        {
            var change = new LenghtViewModel(SelectedTrip,SelectedStation,this);
            change.InitializeAsync();
            AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, change);
        }


        #endregion

    }
}
