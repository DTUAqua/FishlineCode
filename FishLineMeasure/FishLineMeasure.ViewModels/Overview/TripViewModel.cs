using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Anchor.Core;

namespace FishLineMeasure.ViewModels.Overview
{
    public class TripViewModel : AViewModel
    {
        private ObservableCollection<StationViewModel> _lstStations;

        private int _year;

        private string _cruise;

        private string _trip;

        private string _path;

        private string _nameOfFolder;

        public string UIproperty { get; set; }

        #region Properties


        public ObservableCollection<StationViewModel> Stations
        {
            get { return _lstStations; }
            set
            {
                _lstStations = value;
                RaisePropertyChanged(nameof(Stations));
            }
        }

        public int Year
        {
            get { return _year; }
            set
            {
                _year = value;
                RaisePropertyChanged(nameof(Year));
            }
        }

        public string Cruise
        {
            get { return _cruise; }
            set
            {
                _cruise = value;
                RaisePropertyChanged(nameof(Cruise));
            }
        }

        public string Trip
        {
            get { return _trip; }
            set
            {
                _trip = value;
                RaisePropertyChanged(nameof(Trip));
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged(nameof(Path));
            }
        }

        public string NameOfFolder
        {
            get { return _nameOfFolder; }
            set
            {
                _nameOfFolder = value;
                RaisePropertyChanged(nameof(NameOfFolder));
            }
        }

        public string Key
        {
            get { return string.Format("{0} | {1} | {2}", Year, Cruise, Trip); }
        }

        #endregion

        public TripViewModel()
        {
            //Initialize stations
            _lstStations = new ObservableCollection<StationViewModel>();
        }


        public bool Load(string directoryPath)
        {
            bool res = true;

            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    LogError(string.Format("TripViewModel.Load(). Directory '{0}' is not found.", directoryPath));
                    return false;
                }

                string lastFolderName = new DirectoryInfo(directoryPath).Name;
                string[] tempArray = lastFolderName.Split(new[] { "]_[", "[", "]" }, StringSplitOptions.None);
                tempArray = tempArray.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

                Year = Convert.ToInt32(tempArray[0]);
                Cruise = tempArray[1];
                Trip = tempArray[2];
                Path = directoryPath;
                NameOfFolder = $@"[{tempArray[0]}]_[{tempArray[1]}]_[{tempArray[2]}]";

                var files = Directory.GetFiles(directoryPath);
                foreach (var station in files)
                {
                    var vmStation = new StationViewModel();
                    vmStation.ParentTrip = this;

                    if (vmStation.LoadData(station))
                        Stations.Add(vmStation);
                }

                OrderStations();
            }
            catch(Exception e)
            {
                LogError(e);
                res = false;
                return res;
            }

            res = true;
            return res;
        }


        private void OrderStations()
        {
            try
            {
                Stations = GetOrderedStations();
            }
            catch(Exception e)
            {
                LogError(e);
            }
        }


        public ObservableCollection<StationViewModel> GetOrderedStations()
        {
            return Stations.OrderByDescending(x => x.StationNumber).ToObservableCollection();
        }


        public void AddStation(StationViewModel station, bool orderAfterAdd = true)
        {
            Stations.Add(station);

            if (orderAfterAdd)
                OrderStations();
        }

        public void AddStations(IEnumerable<StationViewModel> lst, bool orderAfterAdd = true)
        {
            if (lst == null)
                return;

            Stations.AddRange(lst);

            if(orderAfterAdd)
                OrderStations();
        }
    }
}
