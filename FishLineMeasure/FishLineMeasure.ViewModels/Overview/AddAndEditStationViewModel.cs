using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.Practices.Prism.Commands;
using System.IO;

namespace FishLineMeasure.ViewModels.Overview
{
    public class AddAndEditStationViewModel : AViewModel
    {
        public DelegateCommand _cmdAddStation;

        public DelegateCommand _cmdcancel;

        public StationViewModel _stationToChange;
        public StationViewModel BackupForChanges;


        #region Properties

        public string Year { get; set; }
        public string Cruise { get; set; }
        public string Trip { get; set; }
        public string StationNumber { get; set; }
        public string ButtonsName { get; set; }
        public string SelectedConfiguration { get; set; }

        public TripViewModel Parent { get; set; }

        #endregion

        public AddAndEditStationViewModel(TripViewModel selectedTrip)
        {
            WindowTitle = "Ny Station";
            WindowWidth = 800;
            WindowHeight = 500;
            MinWindowWidth = 600;
            MinWindowHeight = 450;
            Year = Convert.ToString(selectedTrip.Year);
            Cruise = selectedTrip.Cruise;
            Trip = selectedTrip.Trip;
            ButtonsName = "Tilføj Station";
            SelectedConfiguration = "Add";
            Parent = selectedTrip;
            IsDirty = false;
        }

        public AddAndEditStationViewModel(StationViewModel selectedStation)
        {
            _stationToChange = selectedStation;
            BackupForChanges = selectedStation;
            WindowTitle = "Rediger Station";
            WindowWidth = 800;
            WindowHeight = 500;
            MinWindowWidth = 600;
            MinWindowHeight = 450;
            StationNumber = _stationToChange.StationNumber;
            Parent = _stationToChange.ParentTrip;
            Year = Convert.ToString(Parent.Year);
            Cruise = Parent.Cruise;
            Trip = Parent.Trip;
            ButtonsName = "Gem Ændring";
            SelectedConfiguration = "Edit";
            IsDirty = false;
        }

        #region Add New Station Commands

        public DelegateCommand SaveStationCommand
        {
            get { return _cmdAddStation ?? (_cmdAddStation = new DelegateCommand(AddStation)); }
        }

        private void AddStation()
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                return;
            }
            else
            {
                if (IsDirty != true)
                {
                    IsDirty = true;
                }
                
                this.Close();
            }
        }
        public string IsValidByRegexPattern(string input)
        {
            string validationString = null;
            string pattern = @"(([\\/:*?""'><|])|(\[+)|(\]+))";
            MatchCollection match = Regex.Matches(input, pattern);
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
                case "StationNumber":
                    string validationForStationNumber = null;
                    if (string.IsNullOrEmpty(StationNumber))
                    {
                        error = "Stationsnummer er obligatorisk.";
                    }
                    else
                    {
                        IsValidByRegexPattern(StationNumber);
                    }
                    if (!string.IsNullOrEmpty(validationForStationNumber))
                    {
                        error = $"Stationsnummer må ikke indholde {validationForStationNumber}.";
                    }
                    break;
            }

            if (File.Exists(Path.Combine(Parent.Path, $"{Parent.NameOfFolder}_[{StationNumber}].xml")))
            {
                error = "En station med samme nummer eksisterer allerede.";
            }
            else if (SelectedConfiguration == "Edit")
            {
                if (BackupForChanges.StationNumber == StationNumber)
                {
                    error = "Der er ingen ændringer at gemme.";
                }
            }
            return error;
        }


        #endregion

        #region Cancel Operation Command 

        public DelegateCommand CancelCommand
        {
            get { return _cmdcancel ?? (_cmdcancel = new DelegateCommand(CloseWindow)); }
        }

        private void CloseWindow()
        {
            this.Close();
        }

        #endregion
    }
}
