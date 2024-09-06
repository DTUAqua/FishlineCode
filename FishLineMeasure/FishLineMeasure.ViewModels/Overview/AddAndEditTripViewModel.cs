using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Anchor.Core;
using Microsoft.Practices.Prism.Commands;
using System.Text.RegularExpressions;

namespace FishLineMeasure.ViewModels.Overview
{
    public class AddAndEditTripViewModel : AViewModel
    {

        public DelegateCommand _cmdTrip;

        public DelegateCommand _cmdCancel;

        #region Properties
        public string Year { get; set; }
        public string Cruise { get; set; }
        public string Trip { get; set; }
        public string PathCreated { get; set; }
        public string ButtonsName { get; set; }
        public string SelectedConfiguration { get; set; }

        public TripViewModel _tripToChange;
        private TripViewModel BackupOfTripToChange;

        #endregion

        public AddAndEditTripViewModel()
        {
            WindowTitle = "Ny tur";
            WindowWidth = 800;
            WindowHeight = 500;
            MinWindowHeight = 435;
            MinWindowWidth = 600;
            ButtonsName = "Tilføj Togt";
            SelectedConfiguration = "Add";
            IsDirty = false;
        }

        public AddAndEditTripViewModel(TripViewModel selectedTrip)
        {
            _tripToChange = selectedTrip;
            BackupOfTripToChange = selectedTrip;
            WindowTitle = "Rediger Tour";
            WindowWidth = 800;
            WindowHeight = 500;
            MinWindowHeight = 435;
            MinWindowWidth = 600;
            Year = Convert.ToString(selectedTrip.Year);
            Cruise = selectedTrip.Cruise;
            Trip = selectedTrip.Trip;
            ButtonsName = "Gem ændringer";
            SelectedConfiguration = "Edit";
            IsDirty = false;
        }

        /// <summary>
        /// Loads trip from file system async.
        /// </summary>
        public static Task<List<TripViewModel>> LoadTripsAsync()
        {
            return Task.Factory.StartNew(LoadTrips);
        }


        public static List<TripViewModel> LoadTrips()
        {
            List<TripViewModel> lstTrips = new List<TripViewModel>();

            var dirs = Directory.GetDirectories(BusinessLogic.Settings.Settings.Instance.DataRootPath);
           
            if (dirs != null && dirs.Length > 0)
            {
                Dictionary<string, TripViewModel> existingTrips = new Dictionary<string, TripViewModel>();

                foreach(var d in dirs)
                {
                    var vmTrip = new TripViewModel();

                    //If load was successfull, add it as result.
                    if(vmTrip.Load(d))
                    {
                        //If trip has not yet been loaded, load it. 
                        if (!existingTrips.ContainsKey(vmTrip.Key))
                        {
                            lstTrips.Add(vmTrip);
                        }
                        else
                        {
                            //Trip already exists, so reuse tripviewmodel and add loaded stations to it instead.
                            existingTrips[vmTrip.Key].AddStations(vmTrip.Stations, true);
                        }
                    }
                    else
                    {
                        //TODO : Handle a failed trip load.
                    }
                }
            }

            return lstTrips;
        }

        #region Add New Trip Command

        public DelegateCommand AddTourToList
        {
            get { return _cmdTrip ?? (_cmdTrip = new DelegateCommand(AddToListMethod)); }
        }

        private void AddToListMethod()
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                return;
            }
            else
            {
                if(SelectedConfiguration != "Edit")
                    Directory.CreateDirectory(Path.Combine(AppSettings.DataRootPath, $"[{Year}]_[{Cruise}]_[{Trip}]"));

                IsDirty = true;
                this.Close();
            }
        }


        public string IsValidByRegexPattern(string input)
        {
            string validationString = null;
            string pattern = @"(([\\/:*?""'><|])|(\[+)|(\]+))";
            var match = Regex.Matches(input, pattern);

            foreach (Match item in match)
            {
                validationString = validationString + item.Value;
            }

            return validationString;
        }

        protected override string ValidateField(string strFieldName)
        {
            string error = null;

            if (!_blnValidate)
            {
                return error;
            }
            switch (strFieldName)
            {
                case "Year":
                    int year = 0;

                    if (string.IsNullOrWhiteSpace(Year))
                    {
                        error = "År er obligatorisk";
                    }
                    else if (Year.Length != 4)
                    {
                        error = "År er af forkert format";
                    }
                    else if (!Year.TryParseInt32(out year))
                    {
                        error = "År skal være et heltal på 4 cifre";
                    }
                    else if (year < 1900)
                    {
                        error = "År skal være efter år 1900";
                    }
                    break;
                case "Cruise":
                    string validationForTour = null;
                    if (string.IsNullOrWhiteSpace(Cruise))
                    {
                        error = "Tur er obligatorisk";
                    }
                    else
                    {
                        string tempString = $@"{Cruise}";
                        validationForTour = IsValidByRegexPattern(tempString);
                    }
                    if (!string.IsNullOrWhiteSpace(validationForTour))
                    {
                        error = $"tur må ikke indeholde {validationForTour} ";
                    }
                    break;
                case "Trip":
                    string validationForTourNumber = null;
                    if (string.IsNullOrWhiteSpace(Trip))
                    {
                        error = "Turnummer er obligatorisk";
                    }
                    else
                    {
                        string tempString = $@"{Trip}";
                        validationForTourNumber = IsValidByRegexPattern(tempString);
                    }
                    if (!string.IsNullOrWhiteSpace(validationForTourNumber))
                    {
                        error = $"turnummer må ikke indeholder {validationForTourNumber} ";
                    }
                    break;
            }
            PathCreated = $"[{Year}]_[{Cruise}]_[{Trip}]";

            if (Directory.Exists(Path.Combine(AppSettings.DataRootPath, PathCreated)))
            {
                error = "Turen eksisterer allerede";
            }

            if(SelectedConfiguration == "Edit")
            {
                if (BackupOfTripToChange.Year == Convert.ToInt32(Year) && BackupOfTripToChange.Cruise == Cruise && BackupOfTripToChange.Trip == Trip)
                {
                    error = "Der er ingen ændringer at gemme";
                }
            }

            return error;
        }


        #endregion

        #region Cancel add To Trip

        public DelegateCommand CancelThis
        {
            get { return _cmdCancel ?? (_cmdCancel = new DelegateCommand(CancelThisMethod)); }
        }

        private void CancelThisMethod()
        {
            this.Close();
        }

        #endregion
    }
}
