using Babelfisk.Entities.Sprattus;
using FishLineMeasure.ViewModels.Infrastructure;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anchor.Core;

namespace FishLineMeasure.ViewModels.Export
{
    public class SelectSpeciesListViewModel : AViewModel
    {
        private DelegateCommand _cmdOK;
        private DelegateCommand _cmdSkip;


        private List<SpeciesList> _lst;

        private SpeciesList _selectedSpeciesList;

        private Dictionary<int, string> _dicApplicationCodes;

        private string _speciesCode;

        private string _landingCatCode;

        private string _sexCode;

        private string _sortingEUCode;

        private string _sortingDFUCode;

        private string _ovirogousCode;

        private string _applicationCode;

        private string _cruise;

        private string _trip;

        private string _station;


        #region Properties


        public List<SpeciesList> SpeciesList
        {
            get { return _lst; }
            set
            { 
                _lst = value;
                RaisePropertyChanged(() => SpeciesList);
            }
        }


        public SpeciesList SelectedSpeciesList
        {
            get { return _selectedSpeciesList; }
            set
            {
                _selectedSpeciesList = value; 
                RaisePropertyChanged(nameof(SelectedSpeciesList));
                RaisePropertyChanged(nameof(HasSelectedSpeciesList));
            }
        }


        public Dictionary<int, string> ApplicationCodes
        {
            get { return _dicApplicationCodes; }
            set
            {
                _dicApplicationCodes = value;
                RaisePropertyChanged(nameof(ApplicationCodes));
            }
        }

        public string Cruise
        {
            get { return _cruise; }
        }

        public string Trip
        {
            get { return _trip; }
        }

        public string Station
        {
            get { return _station; }
        }

        public bool HasSelectedSpeciesList
        {
            get { return _selectedSpeciesList != null; }
        }

        public string SpeciesCode
        {
            get { return _speciesCode; }
        }


        public string LandingCategoryCode
        {
            get { return _landingCatCode; }
        }


        public string SexCode
        {
            get { return _sexCode; }
        }


        public string SortingEUCode
        {
            get { return _sortingEUCode; }
        }

        public string SortingDFUCode
        {
            get { return _sortingDFUCode; }
        }

        public string OvirogousCode
        {
            get { return _ovirogousCode; }
        }

        public string ApplicationCode
        {
            get { return _applicationCode; }
        }


        #endregion



        public SelectSpeciesListViewModel(string cruise, string trip, string station, string speciesCode, string landingCatCode, string sexCode, string sortingEUCode, List<L_Application> lstApplications, params SpeciesList[] speciesLists)
        {
            WindowWidth = 800;
            WindowHeight = 550;
            WindowTitle = "Vælg en artslisterække at eksportere længderne til.";

            base.AdjustWindowWidthHeightToScreen();

            _cruise = cruise;
            _trip = trip;
            _station = station;
            _speciesCode = speciesCode;
            _landingCatCode = landingCatCode;
            _sexCode = sexCode;
            _sortingEUCode = sortingEUCode;

            _lst = speciesLists.ToList();
            _dicApplicationCodes = lstApplications == null ? new Dictionary<int, string>() : lstApplications.DistinctBy(x => x.L_applicationId).ToDictionary(x => x.L_applicationId, x => string.Format("{0} - {1}", x.code, x.description));
        }




        #region Skip Command


        public DelegateCommand SkipCommand
        {
            get { return _cmdSkip ?? (_cmdSkip = new DelegateCommand(Skip)); }
        }


        private void Skip()
        {
            SelectedSpeciesList = null;

            this.Close();
        }


        #endregion



        #region OK Command


        public DelegateCommand OKCommand
        {
            get { return _cmdOK ?? (_cmdOK = new DelegateCommand(OK)); }
        }


        private void OK()
        {
            this.Close();
        }


        #endregion
    }
}
