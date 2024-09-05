using Anchor.Core;
using Anchor.Core.Controls;
using Babelfisk.BusinessLogic;
using Babelfisk.Entities.Sprattus;
using Babelfisk.ViewModels.Input;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Babelfisk.ViewModels.SmartDots
{
    public class SDSelectAnimalsViewModel: AViewModel
    {
        private SDSamplesViewModel  _vmSDSample;

        private DelegateCommand _cmdAddSelectedanimals;
        private DelegateCommand _cmdLoadAnimals;
        private DelegateCommand _cmdLoadAnimal;
        private DelegateCommand _cmdEditImageFolders;


        private SDEvent _sdEvent;
        private string _searchString;

        private string _animalIdString;
        private List<DropDownListBoxItem> _ddlCruisYears;

        private List<SelectionAnimalItem> _lstselectionAnimals;
        private List<SelectionAnimalItem> _filteredAnimals;
        private List<SelectionAnimalItem> _selectedAnimalas;

        private List<DropDownTagListBoxItem<int?>> _lengths;
        private List<DropDownTagListBoxItem<decimal?>> _weights;
        private List<DropDownListBoxItem> _cruise;
        private List<DropDownListBoxItem> _cruiseYear;
        private List<DropDownListBoxItem> _otolithReadingRemarks;
        private List<DropDownTagListBoxItem<int?>> _ages;
        private List<DropDownListBoxItem> _edgeStructures;
        private List<DropDownListBoxItem> _trips;
        private List<DropDownListBoxItem> _stations;
        private List<DropDownListBoxItem> _stocks;
        private List<DropDownListBoxItem> _sexCodes;
        private List<DropDownListBoxItem> _maturities;
        private List<DropDownListBoxItem> _statisticalRectangles;
        private List<DropDownListBoxItem> _tripType;
        private List<DropDownTagListBoxItem<int?>> _quarter;
        private List<DropDownListBoxItem> _areaCodes;
        private List<DropDownListBoxItem> _ligthTypes;
        private List<DropDownListBoxItem> _prepMethods;
        private List<DropDownListBoxItem> _otolithDescriptions;

        private int _selectedCruiseYear;

        private ColumnVisibilityViewModel _vmColumnVisibility;

        private string[] _imagePaths = null;

        private bool loadFromAnimalId = false;


        #region Properties


        public string[] ImagePaths
        {
            get { return _imagePaths; }
            set
            {
                _imagePaths = value;
                RaisePropertyChanged(() => ImagePaths);
                RaisePropertyChanged(() => ImagePathsCount);
                RaisePropertyChanged(() => ImagePathsCountString);
                RaisePropertyChanged(() => HasImagePaths);
            }
        }


        public bool HasImagePaths
        {
            get { return _imagePaths != null && _imagePaths.Length > 0; }
        }


        public int ImagePathsCount
        {
            get { return _imagePaths == null ? 0 : _imagePaths.Length; }
        }


        public string ImagePathsCountString
        {
            get { return ImagePathsCount > 9 ? "9+" : ImagePathsCount.ToString(); }
        }



        public String ColumnVisivilityAny
        {
            get { return "Any"; }
        }


        public ColumnVisibilityViewModel ColumnVisibility
        {
            get { return _vmColumnVisibility; }
            set
            {
                _vmColumnVisibility = value;
                RaisePropertyChanged(() => ColumnVisibility);
                RaisePropertyChanged(() => HasColumnVisibility);
            }
        }


        public bool HasColumnVisibility
        {
            get { return _vmColumnVisibility != null; }
        }


        /// <summary>
        /// Get/Set what to filter the events by. The events are filtered on name, event type, species, and start/stop date.
        /// </summary>
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
                RaisePropertyChanged(() => SearchString);
                FilterAnimals();
            }
        }


        public String AnimalIdString
        {
            get { return _animalIdString; }
            set
            {
                _animalIdString = value;
                RaisePropertyChanged(() => AnimalIdString);
            }
        }


        public List<DropDownListBoxItem> CruisYearsList
        {
            get { return _ddlCruisYears; }
            set
            {
                _ddlCruisYears = value;
                RaisePropertyChanged(()=> CruisYearsList);
                RaisePropertyChanged(() => HasSelectedYears);
            }
        }


        public List<SelectionAnimalItem> AllSelectionAnimals
        {
            get { return _lstselectionAnimals; }
            set
            {
                _lstselectionAnimals = value;
                RaisePropertyChanged(() => AllSelectionAnimals);
                RaisePropertyChanged(() => HasAnimals);
            }
        }


        public List<SelectionAnimalItem> FilteredAnimals
        {
            get { return _filteredAnimals; }
            set
            {
                _filteredAnimals = value;
                RaisePropertyChanged(() => FilteredAnimals);
                RaisePropertyChanged(() => Cruises);
            }
        }


        public List<SelectionAnimalItem> SelectedAnimals
        {
            get { return _selectedAnimalas; }
            set
            {
                _selectedAnimalas = value;
                RaisePropertyChanged(() => SelectedAnimals);
            }
        }


        public bool? IsAllSelected
        {
            get
            {

                if (FilteredAnimals!= null && FilteredAnimals.Count > 0 && FilteredAnimals.Count == FilteredAnimals.Where(x => x.IsSelected).ToList().Count())
                    return true;
                else if (FilteredAnimals != null && FilteredAnimals.Any(x => x.IsSelected))
                    return null;
                else
                    return false;
            }
            set
            {
                SetAllCheckboxes();
                RaisePropertyChanged(() => IsAllSelected);
                
            }
        }


        public bool HasSelectedYears
        {
            get { return CruisYearsList != null && CruisYearsList.Count > 0; }
        }


        public List<DropDownTagListBoxItem<int?>> Lengths
        {
            get { return _lengths; }
            set
            {
                _lengths = value;
                RaisePropertyChanged(() => Lengths);
            }
        }


        public List<DropDownTagListBoxItem<decimal?>> Weights
        {
            get { return _weights; }
            set
            {
                _weights = value;
                RaisePropertyChanged(() => Weights);
            }
        }


        public List<DropDownListBoxItem> OtolithReadingRemarks
        {
            get { return _otolithReadingRemarks; }
            set
            {
                _otolithReadingRemarks = value;
                RaisePropertyChanged(() => OtolithReadingRemarks);
            }
        }


        public List<DropDownTagListBoxItem<int?>> Ages
        {
            get { return _ages; }
            set
            {
                _ages = value;
                RaisePropertyChanged(() => Ages);
            }
        }


        public List<DropDownListBoxItem> EdgeStructures
        {
            get { return _edgeStructures; }
            set
            {
                _edgeStructures = value;
                RaisePropertyChanged(() => EdgeStructures);
            }
        }


        public List<DropDownListBoxItem> Trips
        {
            get { return _trips; }
            set
            {
                _trips = value;
                RaisePropertyChanged(() => Trips);
            }
        }


        public List<DropDownListBoxItem> Stations
        {
            get { return _stations; }
            set
            {
                _stations = value;
                RaisePropertyChanged(() => Stations);
            }
        }


        public List<DropDownListBoxItem> SexCodes
        {
            get { return _sexCodes; }
            set
            {
                _sexCodes = value;
                RaisePropertyChanged(() => SexCodes);
            }
        }


        public List<DropDownListBoxItem> Maturities
        {
            get { return _maturities; }
            set
            {
                _maturities = value;
                RaisePropertyChanged(() => Maturities);
            }
        }

        
        public List<DropDownListBoxItem> StatisticalRectangles
        {
            get { return _statisticalRectangles; }
            set
            {
                _statisticalRectangles = value;
                RaisePropertyChanged(() => StatisticalRectangles);
            }
        }


        /// <summary>
        /// Return list of Stocks for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedStocks
        {
            get { return _stocks; }
            set
            {
                _stocks = value;
                RaisePropertyChanged(() => UsedStocks);
            }
        }


        public List<DropDownListBoxItem> Cruises
        {
            get { return _cruise; }
            set
            {
                _cruise = value;
                RaisePropertyChanged(() => Cruises);
            }
        }


        public List<DropDownListBoxItem> CruiseYears
        {
            get { return _cruiseYear; }
            set
            {
                _cruiseYear = value;
                RaisePropertyChanged(() => CruiseYears);
            }
        }


        public List<DropDownListBoxItem> TripTypes
        {
            get { return _tripType; }
            set
            {
                _tripType = value;
                RaisePropertyChanged(() => TripTypes);
            }
        }


        public List<DropDownTagListBoxItem<int?>> Quarters
        {
            get { return _quarter; }
            set
            {
                _quarter = value;
                RaisePropertyChanged(()=> Quarters);
            }
        }


        public List<DropDownListBoxItem> AreaCodes
        {
            get { return _areaCodes; }
            set
            {
                _areaCodes = value;
                RaisePropertyChanged(()=>AreaCodes);
            }
        }


        public List<DropDownListBoxItem> LightTypes
        {
            get { return _ligthTypes; }
            set
            {
                _ligthTypes = value;
                RaisePropertyChanged(() => LightTypes);
            }
        }


        public List<DropDownListBoxItem> PrepMethods
        {
            get { return _prepMethods; }
            set
            {
                _prepMethods = value;
                RaisePropertyChanged(() => PrepMethods);
            }
        }


        public List<DropDownListBoxItem> OtolithDescriptions
        {
            get { return _otolithDescriptions; }
            set
            {
                _otolithDescriptions = value;
                RaisePropertyChanged(() => OtolithDescriptions);
            }
        }


        public int SelectedCruiseYear
        {
            get { return _selectedCruiseYear;}
            set
            {
                _selectedCruiseYear = value;
                RaisePropertyChanged(()=> SelectedCruiseYear);
            }
        }


        public bool HasAnimals
        {
            get {return _lstselectionAnimals != null && _lstselectionAnimals.Count > 0; }
        }


        public bool HasselectedAnimal
        {
            get { return IsAllSelected == null || IsAllSelected == true; }
        }


        public int SelectedAnimalsCount
        {
            get { return _filteredAnimals != null ? _filteredAnimals.Where(x => x.IsSelected).Count() : 0; }
        }
        

        #endregion



        public SDSelectAnimalsViewModel(SDSamplesViewModel sDSamplesViewModel, SDEvent sDEvent) 
        {
            _vmSDSample = sDSamplesViewModel;

            WindowWidth = 1000;
            WindowHeight = 580;
            try
            {
                WindowTitle = String.Format("Select animals | Species: {0} - {1} | Areas: {2}", sDEvent.L_Species.speciesCode, sDEvent.L_Species.dkName, sDEvent.L_DFUAreas == null || sDEvent.L_DFUAreas.Count == 0 ? "" : string.Join(", ", sDEvent.L_DFUAreas.Select(x => x.DFUArea)));
            }
            catch 
            {
                WindowTitle = String.Format("Select animals");
            }

            _sdEvent = sDEvent;

            _ddlCruisYears = new List<DropDownListBoxItem>();
            _lstselectionAnimals = new List<SelectionAnimalItem>();

            if (_sdEvent != null)
                _imagePaths = _sdEvent.DefaultImageFoldersArray;

            try
            {
                ColumnVisibility = new ColumnVisibilityViewModel(ColumnVisivilityAny, "SDSelectAnimals", ColumnVisibilityChanged);
                ColumnVisibility.WindowTitle = string.Format("Viste kolonner");
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        public void RaiseIsAllSelected()
        {
            RaisePropertyChanged(() => IsAllSelected);
            RaisePropertyChanged(() => HasselectedAnimal);
            RaisePropertyChanged(() => SelectedAnimalsCount);
        }

       
        /// <summary>
        /// Load view model asynchronously loading all drop down list data.
        /// </summary>
        public void InitializeAsync()
        {
            IsLoading = true;
            Task.Factory.StartNew(LoadYears).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }

        /// <summary>
        /// Load years synchronously.
        /// </summary>
        private void LoadYears()
        {
            try
            {
                var man = new BusinessLogic.SmartDots.SmartDotsManager();
                var cruisYears = man.GetCruiseYears();

                if (cruisYears == null)
                {
                    _ddlCruisYears = new List<DropDownListBoxItem>();
                    RaisePropertyChanged(()=>CruisYearsList);
                }  
                else
                {
                    foreach (var item in cruisYears)
                    {
                        _ddlCruisYears.Add(new DropDownListBoxItem() { Text = item.ToString() });
                    }
                    RaisePropertyChanged(()=>CruisYearsList);
                }

                new Action(() =>
                {
                    FilterAnimals();
                }).Dispatch();
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }

        private void ColumnVisibilityChanged(List<Babelfisk.BusinessLogic.Settings.DataGridColumnSettings> cs)
        {
            //Refresh trip type since it is bound to all column, forcing their vibility to be refreshed
            RaisePropertyChanged(() => ColumnVisivilityAny);
        }


        /// <summary>
        /// Filter animals by SearchString and drop down header items 
        /// </summary>
        private void FilterAnimals()
        {
            try
            {
                

                if (_lstselectionAnimals == null)
                {
                    FilteredAnimals = new List<SelectionAnimalItem>();
                    return;
                }

                IEnumerable<SelectionAnimalItem> lst = _lstselectionAnimals;

                if (!string.IsNullOrWhiteSpace(SearchString))
                {
                    var search = SearchString ?? "";
                    lst = lst.Where(x => (x.SelectionAnimal.AnimalId.ToString()).Contains(SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                                         //(x.SelectionAnimal.LengthMM.HasValue && (x.SelectionAnimal.LengthMM.Value.ToString()).Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                         (x.SelectionAnimal.WeightG.HasValue && (x.SelectionAnimal.WeightG.Value.ToString()).Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                         //(x.SelectionAnimal.SexCode ?? "").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                                         //(x.SelectionAnimal.MaturityDescription ?? "").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                                         //(x.SelectionAnimal.Age.HasValue && (x.SelectionAnimal.Age.Value.ToString()).Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                         //(x.SelectionAnimal.OtolithReadingRemarkCode ?? "").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                                         //(x.SelectionAnimal.EdgeStructureCode ?? "").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                                         //(x.SelectionAnimal.AreaCode != null && x.SelectionAnimal.AreaCode.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                         //(x.SelectionAnimal.Latitude ?? "").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                                         //(x.SelectionAnimal.Longitude ?? "").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                                         //(x.SelectionAnimal.StatisticalRectangle ?? "").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                                         //(x.SelectionAnimal.CruiseYear.ToString()).Contains(SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                                         //(x.SelectionAnimal.Cruise ?? "").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                                         //(x.SelectionAnimal.Trip ?? "").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                                         //(x.SelectionAnimal.TripType ?? "").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                                         //(x.SelectionAnimal.Station ?? "").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                                         (x.SelectionAnimal.GearStartDate != null && x.SelectionAnimal.GearStartDate.Value.ToString("dd-MM-yyyy").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                         //(x.SelectionAnimal.Quarter.HasValue && x.SelectionAnimal.Quarter.Value.ToString().Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                         (x.Comments ?? "").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)
                                         );
                }

                var selectedCruises = Cruises == null ? new HashSet<string>() : Cruises.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedTripTypes = TripTypes == null ? new HashSet<string>() : TripTypes.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedQuarters = Quarters == null ? new HashSet<string>() : Quarters.Where(x => x.IsChecked).Select(x => x.Tag.ToString()).Distinct().ToHashSet<string>();
                var selectedAreaCodes = AreaCodes == null ? new HashSet<string>() : AreaCodes.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedLength = Lengths == null ? new HashSet<string>() : Lengths.Where(x => x.IsChecked).Select(x => x.Tag.ToString()).Distinct().ToHashSet<string>();
                var selectedWeights = Weights == null ? new HashSet<string>() : Weights.Where(x => x.IsChecked).Select(x => x.Tag.ToString()).Distinct().ToHashSet<string>();
                var selectedAges = Ages == null ? new HashSet<string>() : Ages.Where(x => x.IsChecked).Select(x => x.Tag.ToString()).Distinct().ToHashSet<string>();
                var selectedOtolithReadingRemarks = OtolithReadingRemarks == null ? new HashSet<string>() : OtolithReadingRemarks.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedEdgeStructures = EdgeStructures == null ? new HashSet<string>() : EdgeStructures.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedTrips = Trips == null ? new HashSet<string>() : Trips.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedStations = Stations == null ? new HashSet<string>() : Stations.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedSexCodes = SexCodes == null ? new HashSet<string>() : SexCodes.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedMaturities = Maturities == null ? new HashSet<string>() : Maturities.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedStatisticalRectangles = StatisticalRectangles == null ? new HashSet<string>() : StatisticalRectangles.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedCruiseYears = CruiseYears == null ? new HashSet<string>() : CruiseYears.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedStocks = UsedStocks == null ? new HashSet<string>() : UsedStocks.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedLightTypes = LightTypes == null ? new HashSet<string>() : LightTypes.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedPrepMethods = PrepMethods == null ? new HashSet<string>() : PrepMethods.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedOtolithDescriptions = OtolithDescriptions == null ? new HashSet<string>() : OtolithDescriptions.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();

                if (selectedCruises.Count > 0)
                    lst = lst.Where(x => selectedCruises.Contains(x.SelectionAnimal.Cruise ?? ""));

                if (selectedCruiseYears.Count > 0)
                    lst = lst.Where(x => x.SelectionAnimal.CruiseYear != 0 && selectedCruiseYears.Contains(x.SelectionAnimal.CruiseYear.ToString() ?? ""));

                if (selectedTripTypes.Count > 0)
                    lst = lst.Where(x => selectedTripTypes.Contains(x.SelectionAnimal.TripType ?? ""));

                if (selectedQuarters.Count > 0)
                    lst = lst.Where(x => x.SelectionAnimal.Quarter != null && selectedQuarters.Contains(x.SelectionAnimal.Quarter.ToString() ?? ""));

                if (selectedAreaCodes.Count > 0)
                    lst = lst.Where(x => selectedAreaCodes.Contains(x.SelectionAnimal.AreaCode ?? ""));

                if (selectedLength.Count > 0)
                    lst = lst.Where(x => x.SelectionAnimal.LengthMM != null && selectedLength.Contains(x.SelectionAnimal.LengthMM.ToString() ?? ""));

                if (selectedWeights.Count > 0)
                    lst = lst.Where(x => x.SelectionAnimal.WeightG != null && selectedWeights.Contains(x.SelectionAnimal.WeightG.ToString() ?? ""));

                if (selectedAges.Count > 0)
                    lst = lst.Where(x => x.SelectionAnimal.Age != null && selectedAges.Contains(x.SelectionAnimal.Age.ToString() ?? ""));

                if (selectedOtolithReadingRemarks.Count > 0)
                    lst = lst.Where(x => selectedOtolithReadingRemarks.Contains(x.SelectionAnimal.OtolithReadingRemarkCode ?? ""));

                if (selectedEdgeStructures.Count > 0)
                    lst = lst.Where(x => selectedEdgeStructures.Contains(x.SelectionAnimal.EdgeStructureCode ?? ""));

                if (selectedTrips.Count > 0)
                    lst = lst.Where(x => selectedTrips.Contains(x.SelectionAnimal.Trip ?? ""));

                if (selectedStations.Count > 0)
                    lst = lst.Where(x => selectedStations.Contains(x.SelectionAnimal.Station ?? ""));

                if (selectedSexCodes.Count > 0)
                    lst = lst.Where(x => selectedSexCodes.Contains(x.SelectionAnimal.SexCode ?? ""));

                if (selectedMaturities.Count > 0)
                    lst = lst.Where(x => selectedMaturities.Contains(x.SelectionAnimal.MaturityDescription ?? ""));

                if (selectedStatisticalRectangles.Count > 0)
                    lst = lst.Where(x => selectedStatisticalRectangles.Contains(x.SelectionAnimal.StatisticalRectangle ?? ""));

                if (selectedStocks.Count > 0)
                    lst = lst.Where(x => selectedStocks.Contains(x.Stock ?? ""));

                if (selectedLightTypes.Count > 0)
                    lst = lst.Where(x => selectedLightTypes.Contains(x.LightType ?? ""));

                if (selectedPrepMethods.Count > 0)
                    lst = lst.Where(x => selectedPrepMethods.Contains(x.PrepMethod ?? ""));

                if (selectedOtolithDescriptions.Count > 0)
                    lst = lst.Where(x => selectedOtolithDescriptions.Contains(x.OtolithDescription ?? ""));

                FilteredAnimals = lst.ToList();
                RaisePropertyChanged(() => HasAnimals);

            }
            catch { }
        }

        private void SetAllCheckboxes()
        {
            if(FilteredAnimals!= null && FilteredAnimals.Count > 0)
            {
                if (FilteredAnimals.Count == FilteredAnimals.Where(x => x.IsSelected).ToList().Count())
                    foreach (var item in FilteredAnimals)
                    {
                         item.SetIsSelected(false);
                    }
                else if (FilteredAnimals.Any(x => x.IsSelected))
                    foreach (var item in FilteredAnimals)
                    {
                        item.SetIsSelected(false);
                    }
                else
                    foreach (var item in FilteredAnimals)
                    {
                        item.SetIsSelected(true);
                    }
            }

            RaisePropertyChanged(() => FilteredAnimals);
            RaiseIsAllSelected();
        }

        private void ClearSelectionAnimals()
        {
            try
            {
                if (_lstselectionAnimals != null)
                {
                    _lstselectionAnimals.Clear();
                    FilterAnimals();
                }
            }
            catch { }
        }

        #region Check box
        private bool IsControlDown()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }
        public void CheckBox_Initialized(object sender, EventArgs e, Key? keyLastPressed)
        {
            CheckBox tb = (sender as CheckBox);
            bool blnIsCtrlDown = IsControlDown();

            new Action(() =>
            {
                if (keyLastPressed.HasValue && keyLastPressed.Value != Key.None && !blnIsCtrlDown && Keyboard.IsKeyDown(keyLastPressed.Value) && keyLastPressed.Value == Key.Space)
                {
                    if (tb.IsThreeState)
                        tb.IsChecked = (tb.IsChecked == null ? false : (tb.IsChecked.Value ? null : new Nullable<bool>(true)));
                    else
                        tb.IsChecked = !tb.IsChecked;
                }
            }).Dispatch();
        }
        #endregion



        /// <summary>
        /// Load animals synchronously.
        /// </summary>
        #region Load Animals Commands
       

        public DelegateCommand LoadAnimalsCommand
        {
            get { return _cmdLoadAnimals ?? (_cmdLoadAnimals = new DelegateCommand(LoadAnimalsTask)); }
        }


        private void LoadAnimalsTask()
        {
            RaisePropertyChanged(() => HasSelectedYears);
           
            int tmp;
            //Get selected yaers.
            var selectedYears = _ddlCruisYears.Where(x => x.IsChecked)
                                              .Where(x => x.Text != null && x.Text.TryParseInt32(out tmp))
                                              .Select(x => x.Text.ToInt32())
                                              .ToList();

            if (selectedYears == null || selectedYears.Count == 0)
            {

                var res = AppRegionManager.ShowMessageBox(Translate("SDSelectAnimalsView", "WarningNoYearSelected"), System.Windows.MessageBoxButton.OK);
                return;
            }

            if ((_imagePaths == null || _imagePaths.Length == 0) && AppRegionManager.ShowMessageBox(Translate("SDSelectAnimalsView", "WarningNoImageFoldersSelected"), MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            ClearSelectionAnimals();
            IsLoading = true;
            Task.Factory.StartNew(() => LoadAnimals(selectedYears)).ContinueWith(t => new Action(() => { IsLoading = false; RaiseIsAllSelected(); }).Dispatch());
        }


        public DelegateCommand LoadAnimalCommand
        {
            get { return _cmdLoadAnimal ?? (_cmdLoadAnimal = new DelegateCommand(LoadAnimalTask)); }
        }


        private void LoadAnimalTask()
        {
            RaisePropertyChanged(() => HasSelectedYears);
            _lstselectionAnimals = new List<SelectionAnimalItem>();
            
            if (string.IsNullOrEmpty(AnimalIdString))
            {

                var res = AppRegionManager.ShowMessageBox(Translate("SDSelectAnimalsView", "WarningNoAnimalId"), System.Windows.MessageBoxButton.OK);
                return;
            }


            if ((_imagePaths == null || _imagePaths.Length == 0) && AppRegionManager.ShowMessageBox(Translate("SDSelectAnimalsView", "WarningNoImageFoldersSelected"), MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            IsLoading = true;
            loadFromAnimalId = true;
            ClearSelectionAnimals();
            Task.Factory.StartNew(() => LoadAnimals()).ContinueWith(t => new Action(() => { IsLoading = false; loadFromAnimalId = false; RaiseIsAllSelected(); }).Dispatch());
        }


        private string GetMaturityKey(int maturityIndex, string maturityIndexMethod)
        {
            return string.Format("{0}-{1}", maturityIndex, maturityIndexMethod);
        }

      
        private void LoadAnimals(List<int> selectedYears = null)
        {
            try
            {
                string[] eventAreas = null;

                if (_sdEvent.L_DFUAreas != null && _sdEvent.L_DFUAreas.Count > 0)
                    eventAreas = _sdEvent.L_DFUAreas.Select(x => x.DFUArea).ToArray();

                var man = new BusinessLogic.SmartDots.SmartDotsManager();
                var selectionAnimals = new List<SelectionAnimal>();

                if (!loadFromAnimalId)
                {
                    selectionAnimals = man.GetSelectionAnimals(_sdEvent.speciesCode, selectedYears.ToArray(), eventAreas, true, _imagePaths);
                }
                else
                {
                    int res;
                    if(int.TryParse(AnimalIdString, out res))
                    {
                        int[] arrAnimalId = new int[1] {res};
                        selectionAnimals = man.GetSelectionAnimals(arrAnimalId, true, _imagePaths);
                    }

                    if(selectionAnimals == null || selectionAnimals.Count == 0)
                    {
                        new Action(() =>
                        {
                            var result = AppRegionManager.ShowMessageBox(string.Format(Translate("SDSelectAnimalsView", "AnimalIdNotFound"), AnimalIdString), System.Windows.MessageBoxButton.OK);

                        }).Dispatch();

                        return;
                    }

                    
                    if(_vmSDSample != null && _vmSDSample.Event != null && (selectionAnimals.FirstOrDefault().SpeciesCode != _vmSDSample.Event.speciesCode)) 
                    {
                        new Action(() =>
                        {
                            var result = AppRegionManager.ShowMessageBox(string.Format(Translate("SDSelectAnimalsView", "WarningAnimalAnotherSpecies"), selectionAnimals.FirstOrDefault().SpeciesCode), System.Windows.MessageBoxButton.OK);

                        }).Dispatch();

                        return;
                    }
                    

                    if(_vmSDSample != null && _vmSDSample.Event != null && _vmSDSample.Event.L_DFUAreas != null && !_vmSDSample.Event.L_DFUAreas.Any(x => x.DFUArea == selectionAnimals.FirstOrDefault().AreaCode))
                    {
                        new Action(() =>
                        {
                            var result = AppRegionManager.ShowMessageBox(string.Format(Translate("SDSelectAnimalsView", "WarningAnimalAnotherArea"), selectionAnimals.FirstOrDefault().AreaCode), System.Windows.MessageBoxButton.OK);

                        }).Dispatch();

                        return;

                    }
                       
                }

                var manLookup = new LookupManager();
                var lv = new LookupDataVersioning();

                //Fetch Maturity (which fits with maturity id)
                var dicMaturity = manLookup.GetLookups(typeof(Maturity), lv)
                                           .OfType<Maturity>()
                                           .DistinctBy(f => GetMaturityKey(f.maturityIndex, f.maturityIndexMethod))
                                           .ToDictionary(f => GetMaturityKey(f.maturityIndex, f.maturityIndexMethod));

                List<DropDownListBoxItem> cruiseList = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> cruiseYearList = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> tripTypeList = new List<DropDownListBoxItem>();
                List<DropDownTagListBoxItem<int?>> quarterList = new List<DropDownTagListBoxItem<int?>>();
                List<DropDownListBoxItem> areCodeList = new List<DropDownListBoxItem>();
                List<DropDownTagListBoxItem<int?>> lengthList = new List<DropDownTagListBoxItem<int?>>();
                List<DropDownTagListBoxItem<decimal?>> weightList = new List<DropDownTagListBoxItem<decimal?>>();
                List<DropDownTagListBoxItem<int?>> agesList = new List<DropDownTagListBoxItem<int?>>();
                List<DropDownListBoxItem> otolithReadingRemarkList = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> edgeStructureList = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> tripsList = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> stationsList = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> sexCodeList = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> maturitiesList = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> statisticalRectngleList = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> stocks = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> lightTypes = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> prepMethods = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> otolithDescriptions = new List<DropDownListBoxItem>();

                if (selectionAnimals != null)
                {
                  
                    try
                    {
                        foreach (var item in selectionAnimals)
                        {
                            if (item.MaturityIndex.HasValue)
                            {
                                var key = GetMaturityKey(item.MaturityIndex.Value, item.MaturityIndexMethod ?? "");
                                if (dicMaturity.ContainsKey(key))
                                    item.MaturityDescription = dicMaturity[key].description;
                            }

                            _lstselectionAnimals.Add(new SelectionAnimalItem(this, item));
                        }

                        if (_lstselectionAnimals != null && _lstselectionAnimals.Count > 0 && _vmSDSample != null)
                        {
                            foreach (var item in _lstselectionAnimals)
                            {
                                if (_vmSDSample.SDSampleList.Any(x => x.animalId == item.SelectionAnimal.AnimalId.ToString() && (string.IsNullOrEmpty(item.PrepMethod) || (x.L_SDPreparationMethod != null && x.L_SDPreparationMethod.preparationMethod == item.SelectionAnimal.PreperationMethod))))
                                    item.IsAdded = true;
                            }
                        }

                        if (_lstselectionAnimals.Count == 1)
                        {
                            _lstselectionAnimals.FirstOrDefault().IsSelected = true;

                            if (_lstselectionAnimals.FirstOrDefault().IsAdded)
                            {
                                new Action(() =>
                                {
                                    var result = AppRegionManager.ShowMessageBox(string.Format(Translate("SDSelectAnimalsView", "AnimalIdAlreadyExist"), AnimalIdString), System.Windows.MessageBoxButton.OK);
                                
                                }).Dispatch();
                               
                            }
                        }
                    }
                    catch (Exception e)
                    {

                        LogAndDispatchUnexpectedErrorMessage(e);
                    }
                    var cmp = new Anchor.Core.Comparers.StringNumberComparer();

                    //Get Unique itel lists
                    //Cruise
                    cruiseList = selectionAnimals.Where(x => !string.IsNullOrWhiteSpace(x.Cruise))
                                              .DistinctBy(x => x.Cruise)
                                              .Select(item => new DropDownListBoxItem() { Text = item.Cruise, CheckedChangedMethodReference = OnFilterItemChanged })
                                              .OrderBy(x => x.Text, cmp)
                                              .ToList();

                    //TripType
                    tripTypeList = selectionAnimals.Where(x => !string.IsNullOrWhiteSpace(x.TripType))
                                             .DistinctBy(x => x.TripType)
                                             .Select(item => new DropDownListBoxItem() { Text = item.TripType, CheckedChangedMethodReference = OnFilterItemChanged })
                                             .OrderBy(x => x.Text, cmp)
                                             .ToList();

                    

                    //Qarter
                    quarterList = selectionAnimals.Where(x => x.Quarter != null)
                                            .DistinctBy(x => x.Quarter)
                                            .Select(item => new DropDownTagListBoxItem<int?>(){ Text = (item.Quarter.HasValue ? item.Quarter.Value.ToString() : ""), Tag = item.Quarter, CheckedChangedMethodReference = OnFilterItemChanged })
                                            .OrderBy(x => x.Tag)
                                            .ToList();



                    //AreaCode
                    areCodeList = selectionAnimals.Where(x => !string.IsNullOrWhiteSpace(x.AreaCode))
                                             .DistinctBy(x => x.AreaCode)
                                             .Select(item => new DropDownListBoxItem() { Text = item.AreaCode, CheckedChangedMethodReference = OnFilterItemChanged })
                                             .OrderBy(x => x.Text, cmp)
                                             .ToList();

                    //Length
                    lengthList = selectionAnimals.Where(x => x.LengthMM != null)
                                            .DistinctBy(x => x.LengthMM)
                                            .Select(item => new DropDownTagListBoxItem<int?>() { Text = (item.LengthMM.HasValue ? item.LengthMM.Value.ToString() : ""), Tag = item.LengthMM, CheckedChangedMethodReference = OnFilterItemChanged })
                                            .OrderBy(x => x.Tag)
                                            .ToList();

                    //Weight
                    weightList = selectionAnimals.Where(x => x.WeightG != null)
                                            .DistinctBy(x => x.WeightG)
                                            .Select(item => new DropDownTagListBoxItem<decimal?>() { Text = (item.WeightG.HasValue ? item.WeightG.Value.ToString() : ""), Tag = item.WeightG, CheckedChangedMethodReference = OnFilterItemChanged })
                                            .OrderBy(x => x.Tag)
                                            .ToList();

                    //Ages
                    agesList = selectionAnimals.Where(x => x.Age != null)
                                            .DistinctBy(x => x.Age)
                                            .Select(item => new DropDownTagListBoxItem<int?>() { Text = (item.Age.HasValue ? item.Age.Value.ToString() : ""), Tag = item.Age, CheckedChangedMethodReference = OnFilterItemChanged })
                                            .OrderBy(x => x.Tag)
                                            .ToList();

                    //Otolith
                    otolithReadingRemarkList = selectionAnimals.Where(x => !string.IsNullOrWhiteSpace(x.OtolithReadingRemarkCode))
                                             .DistinctBy(x => x.OtolithReadingRemarkCode)
                                             .Select(item => new DropDownListBoxItem() { Text = item.OtolithReadingRemarkCode, CheckedChangedMethodReference = OnFilterItemChanged })
                                             .OrderBy(x => x.Text, cmp)
                                             .ToList();

                    //Edge structure
                    edgeStructureList = selectionAnimals.Where(x => !string.IsNullOrWhiteSpace(x.EdgeStructureCode))
                                             .DistinctBy(x => x.EdgeStructureCode)
                                             .Select(item => new DropDownListBoxItem() { Text = item.EdgeStructureCode, CheckedChangedMethodReference = OnFilterItemChanged })
                                             .OrderBy(x => x.Text, cmp)
                                             .ToList();

                    //Trip 
                    tripsList = selectionAnimals.Where(x => !string.IsNullOrWhiteSpace(x.Trip))
                                             .DistinctBy(x => x.Trip)
                                             .Select(item => new DropDownListBoxItem() { Text = item.Trip, CheckedChangedMethodReference = OnFilterItemChanged })
                                             .OrderBy(x => x.Text, cmp)
                                             .ToList();

                    //Cruise Year
                    cruiseYearList = selectionAnimals.Where(x => x.CruiseYear != 0)
                                             .DistinctBy(x => x.CruiseYear)
                                             .Select(item => new DropDownListBoxItem() { Text = item.CruiseYear.ToString(), CheckedChangedMethodReference = OnFilterItemChanged })
                                             .OrderBy(x => x.Text, cmp)
                                             .ToList();

                    //Station
                    stationsList = selectionAnimals.Where(x => !string.IsNullOrWhiteSpace(x.Station))
                                             .DistinctBy(x => x.Station)
                                             .Select(item => new DropDownListBoxItem() { Text = item.Station, CheckedChangedMethodReference = OnFilterItemChanged })
                                             .OrderBy(x => x.Text, cmp)
                                             .ToList();
                    //Sex Code
                    sexCodeList = selectionAnimals.Where(x => !string.IsNullOrWhiteSpace(x.SexCode))
                                             .DistinctBy(x => x.SexCode)
                                             .Select(item => new DropDownListBoxItem() { Text = item.SexCode, CheckedChangedMethodReference = OnFilterItemChanged })
                                             .OrderBy(x => x.Text, cmp)
                                             .ToList();
                    //Maturity
                    maturitiesList = selectionAnimals.Where(x => !string.IsNullOrWhiteSpace(x.MaturityDescription))
                                             .DistinctBy(x => x.MaturityDescription)
                                             .Select(item => new DropDownListBoxItem() { Text = item.MaturityDescription, CheckedChangedMethodReference = OnFilterItemChanged })
                                             .OrderBy(x => x.Text, cmp)
                                             .ToList();
                    //Statistical Rectable
                    statisticalRectngleList = selectionAnimals.Where(x => !string.IsNullOrWhiteSpace(x.StatisticalRectangle))
                                             .DistinctBy(x => x.StatisticalRectangle)
                                             .Select(item => new DropDownListBoxItem() { Text = item.StatisticalRectangle, CheckedChangedMethodReference = OnFilterItemChanged })
                                             .OrderBy(x => x.Text, cmp)
                                             .ToList();

                    //stocks
                    stocks = _lstselectionAnimals.Where(x => !string.IsNullOrWhiteSpace(x.Stock))
                                             .DistinctBy(x => x.Stock)
                                             .Select(item => new DropDownListBoxItem() { Text = item.Stock, CheckedChangedMethodReference = OnFilterItemChanged })
                                             .OrderBy(x => x.Text, cmp)
                                             .ToList();

                    //lightTypes
                    lightTypes = _lstselectionAnimals.Where(x => !string.IsNullOrWhiteSpace(x.LightType))
                                             .DistinctBy(x => x.LightType)
                                             .Select(item => new DropDownListBoxItem() { Text = item.LightType, CheckedChangedMethodReference = OnFilterItemChanged })
                                             .OrderBy(x => x.Text, cmp)
                                             .ToList();

                    //preparation methods
                    prepMethods = _lstselectionAnimals.Where(x => !string.IsNullOrWhiteSpace(x.PrepMethod))
                                             .DistinctBy(x => x.PrepMethod)
                                             .Select(item => new DropDownListBoxItem() { Text = item.PrepMethod, CheckedChangedMethodReference = OnFilterItemChanged })
                                             .OrderBy(x => x.Text, cmp)
                                             .ToList();

                    //otolith descriptions
                    otolithDescriptions = _lstselectionAnimals.Where(x => !string.IsNullOrWhiteSpace(x.OtolithDescription))
                                             .DistinctBy(x => x.OtolithDescription)
                                             .Select(item => new DropDownListBoxItem() { Text = item.OtolithDescription, CheckedChangedMethodReference = OnFilterItemChanged })
                                             .OrderBy(x => x.Text, cmp)
                                             .ToList();

                }

                new Action(() =>
                {
                    RaisePropertyChanged(() => AllSelectionAnimals);
                    Cruises = cruiseList;
                    TripTypes = tripTypeList;
                    Quarters = quarterList;
                    AreaCodes = areCodeList;
                    Lengths = lengthList;
                    Weights = weightList;
                    Ages = agesList;
                    OtolithReadingRemarks = otolithReadingRemarkList;
                    EdgeStructures = edgeStructureList;
                    Trips = tripsList;
                    CruiseYears = cruiseYearList;
                    Stations = stationsList;
                    Maturities = maturitiesList;
                    StatisticalRectangles = statisticalRectngleList;
                    SexCodes = sexCodeList;
                    UsedStocks = stocks;
                    PrepMethods = prepMethods;
                    LightTypes = lightTypes;
                    OtolithDescriptions = otolithDescriptions;

                    FilterAnimals();

                }).Dispatch();
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }

        private void OnFilterItemChanged(DropDownListBoxItem lb, bool oldValue, bool newValue)
        {
            FilterAnimals();
        }


        #endregion


        /// <summary>
        /// Add animals to sample list.
        /// </summary>
        #region Add Animals Command


        public DelegateCommand AddAnimalsCommand
        {
            get { return _cmdAddSelectedanimals ?? (_cmdAddSelectedanimals = new DelegateCommand(AddSelected)); }
        }

        public void AddSelected()
        {
            try
            {
                List<SelectionAnimalItem> lst = AllSelectionAnimals.Where(x => x.IsSelected).ToList();

                if (lst != null && lst.Count > 0)
                {
                    if (_sdEvent.IsYearlyReadingEventType)
                    {
                        if (lst.Where(x => x.IsAdded).Any())
                        {
                            DispatchMessageBox(Translate("SDSelectAnimalsView", "YearlyReadingAlreadyAddedError")); // "It is only allowed to add an animal once for yearly reading events. One or more of the selected animals (the semi-transparent ones), have already been added to the event. Please uncheck the already added ones and try again.");
                            return;
                        }
                        else if (lst.GroupBy(x => x.SelectionAnimal.AnimalId).Where(x => x.Count() > 1).Any())
                        {
                            DispatchMessageBox(Translate("SDSelectAnimalsView", "YearlyReadingDuplicatesSelectedError"));
                            return;
                        }
                    }
                }

                SelectedAnimals = lst;

                Close();
            }
            catch(Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }


        #endregion


        #region Edit Image Folders Command


        public DelegateCommand EditImageFoldersCommand
        {
            get { return _cmdEditImageFolders ?? (_cmdEditImageFolders = new DelegateCommand(EditImageFolders)); }
        }

        public void EditImageFolders()
        {
            try
            {
                var vm = new SelectFoldersOrFilesViewModel(true, false, false, _imagePaths);
                vm.WindowWidth = 600;
                vm.WindowHeight = 400;
                vm.WindowTitle = Translate("AddEditSDEventView", "SelectFoldersHeader"); //"Select folders";

                AppRegionManager.LoadWindowViewFromViewModel(vm, true, "StandardWindowWithBorderStyle");

                //Assign selection, unless Cancel was clicked.
                if (!vm.IsCancelled)
                    ImagePaths = vm.GetSelectedFolders();
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }


        #endregion
    }

}
