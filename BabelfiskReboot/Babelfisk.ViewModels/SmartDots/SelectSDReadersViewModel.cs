using Anchor.Core;
using Anchor.Core.Controls;
using Babelfisk.BusinessLogic;
using Babelfisk.Entities.Sprattus;
using Babelfisk.ViewModels.Input;
using Babelfisk.ViewModels.Lookup;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Babelfisk.ViewModels.SmartDots
{
    public class SelectSDReadersViewModel: AInputViewModel
    {
        private AddEditSDEventViewModel _addEditSDEventVM;

        private List<SDReaderItem> _sdReaderItemList;
        private List<SDReaderItem> _filteredSDReaderItemList;
        private List<SDReaderItem> _selectedSDReaderItemList;

        private string _searchString;

        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdSave;
        private DelegateCommand<string> _cmdAddEditLookups;

        private List<DropDownListBoxItem> _species;
        private List<DropDownListBoxItem> _prepMethods;
        private List<DropDownListBoxItem> _stocks;
        private List<DropDownListBoxItem> _experiences;
        private List<DropDownListBoxItem> _firstYearReadingGeneral;
        private List<DropDownListBoxItem> _firstYearReadingSpeciesStock;

        public bool IsCanceled = true;

        #region Properties

        public List<SDReaderItem> SDReaderItemList
        {
            get { return _sdReaderItemList; }
            set
            {
                _sdReaderItemList = value;
                RaisePropertyChanged(() => SDReaderItemList);
            }
        }

        public List<SDReaderItem> FilteredSDReaderItemList
        {
            get { return _filteredSDReaderItemList; }
            set
            {
                _filteredSDReaderItemList = value;
                RaisePropertyChanged(() => FilteredSDReaderItemList);
            }
        }

        public List<SDReaderItem> SelectedSDReaderItemList
        {
            get { return _selectedSDReaderItemList; }
            set
            {
                _selectedSDReaderItemList = value;
                RaisePropertyChanged(() => SelectedSDReaderItemList);
            }
        }

        /// <summary>
        /// Return list of Species for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedSpecies
        {
            get { return _species; }
            set
            {
                _species = value;
                RaisePropertyChanged(() => UsedSpecies);
            }
        }

        /// <summary>
        /// Return list of Preparation methods for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedPrepMethods
        {
            get { return _prepMethods; }
            set
            {
                _prepMethods = value;
                RaisePropertyChanged(() => UsedPrepMethods);
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

        /// <summary>
        /// Return list of Experiences for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedExperiences
        {
            get { return _experiences; }
            set
            {
                _experiences = value;
                RaisePropertyChanged(() => UsedExperiences);
            }
        }

        /// <summary>
        /// Return list of First year readings (general) for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedFirstYearReadingGeneral
        {
            get { return _firstYearReadingGeneral; }
            set
            {
                _firstYearReadingGeneral = value;
                RaisePropertyChanged(() => UsedFirstYearReadingGeneral);
            }
        }

        /// <summary>
        /// Return list of First year readings (species/stock) for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedFirstYearReadingSpeciesStock
        {
            get { return _firstYearReadingSpeciesStock; }
            set
            {
                _firstYearReadingSpeciesStock = value;
                RaisePropertyChanged(() => UsedFirstYearReadingSpeciesStock);
            }
        }

        public string SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
                FilterSDReaderItems();
                RaisePropertyChanged(() => SearchString);
                RaisePropertyChanged(() => SearchStringHasValue);
            }
        }

        public bool SearchStringHasValue
        {
            get { return !string.IsNullOrEmpty(_searchString); }
        }

        public bool HasSDReaders
        {
            get { return _sdReaderItemList != null && _sdReaderItemList.Count > 0; }
        }

        public bool HasSelectedSDReaders
        {
            get { return _sdReaderItemList != null && _sdReaderItemList.Count > 0 ? _sdReaderItemList.Any(x => x.IsSelected) : false; }
        }


        public int SelectedSDReadersCount
        {
            get { return _filteredSDReaderItemList != null ? _filteredSDReaderItemList.Where(x => x.IsSelected).Count() : 0; }
        }
        #endregion

        public SelectSDReadersViewModel(AddEditSDEventViewModel addEditSDEventVM)
        {
            WindowWidth = 1150;
            WindowHeight = 535;

            _addEditSDEventVM = addEditSDEventVM;

            SDReaderItemList = new List<SDReaderItem>();
            FilteredSDReaderItemList = new List<SDReaderItem>();
            SelectedSDReaderItemList = new List<SDReaderItem>();

        }

        public void InitializeAsync()
        {
            IsLoading = true;
            Task.Factory.StartNew(Initialize).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }

        private void Initialize()
        {
            try
            {
                var manLookup = new LookupManager();
                var lv = new LookupDataVersioning();
                string[] paramString = { "DFUPerson", "L_Stock", "L_SDReaderExperience", "L_SDPreparationMethod" };

                //Fetch SDReaders
                var lstSDReaders = manLookup.GetLookups(typeof(R_SDReader), lv, paramString).OfType<R_SDReader>().OrderBy(x => x.UIDisplay).ToList();
                if (lstSDReaders == null)
                    return;


                string speciesCode = _addEditSDEventVM != null && _addEditSDEventVM.SelectedSpecies != null ? _addEditSDEventVM.SelectedSpecies.speciesCode : null;
                
                if (speciesCode != null)
                    lstSDReaders = lstSDReaders.Where(x => x.speciesCode == speciesCode).ToList();

                List<DropDownListBoxItem> species = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> prepMethods = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> stocks = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> experiences = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> firstYearReadingGeneral = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> firstYearReadingSpeciesStock = new List<DropDownListBoxItem>();

                var cmp = new Anchor.Core.Comparers.StringNumberComparer();

                var tempSDReaderItemList = new List<SDReaderItem>();

                var sdMan = new BusinessLogic.SmartDots.SmartDotsManager();
                var dicStat = sdMan.GetSDReaderStatistics();
                //lstSDReaders.Select(x => x.dfuPersonId).Distinct();


                foreach (var sdReader in lstSDReaders)
                {
                    var isAdded = false;
                    if (_addEditSDEventVM != null && _addEditSDEventVM.SDReaderList != null && _addEditSDEventVM.SDReaderList.Count > 0 && _addEditSDEventVM.SDReaderList.Any(x => x.SDReader != null && x.SDReader.Id == sdReader.Id))
                        isAdded = true;

                    int noReadings = 0;

                    if(dicStat.ContainsKey(sdReader.r_SDReaderId))
                    {
                        var stat = dicStat[sdReader.r_SDReaderId];
                        noReadings = stat.NumberOfReadings;
                    }
                    tempSDReaderItemList.Add(new SDReaderItem(this, sdReader) {  IsAdded = isAdded, NumberOfReadings = noReadings });
                    
                }

                //preparation methods
                species = tempSDReaderItemList.Where(x => x.SDReader != null && !string.IsNullOrEmpty(x.SDReader.speciesCode))
                                         .DistinctBy(x => x.SDReader.speciesCode)
                                         .Select(item => new DropDownListBoxItem() { Text = item.SDReader.speciesCode, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();


                //preparation methods
                prepMethods = tempSDReaderItemList.Where(x => x.SDReader != null && x.SDReader.L_SDPreparationMethod != null)
                                         .DistinctBy(x => x.SDReader.L_SDPreparationMethod)
                                         .Select(item => new DropDownListBoxItem() { Text = item.SDReader.L_SDPreparationMethod.UIDisplay, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();

                //stocks
                stocks = tempSDReaderItemList.Where(x => x.SDReader != null && x.SDReader.L_Stock != null)
                                         .DistinctBy(x => x.SDReader.L_Stock)
                                         .Select(item => new DropDownListBoxItem() { Text = item.SDReader.L_Stock.UIDisplay, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();

                //experiences
                experiences = tempSDReaderItemList.Where(x => x.SDReader != null && x.SDReader.L_SDReaderExperience != null)
                                         .DistinctBy(x => x.SDReader.L_SDReaderExperience)
                                         .Select(item => new DropDownListBoxItem() { Text = item.SDReader.L_SDReaderExperience.UIDisplay, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();

                //first year readings general
                firstYearReadingGeneral = tempSDReaderItemList.Where(x => x.SDReader != null && x.SDReader.firstYearAgeReadingGeneral != null && x.SDReader.firstYearAgeReadingGeneral != 0)
                                         .DistinctBy(x => x.SDReader.firstYearAgeReadingGeneral)
                                         .Select(item => new DropDownListBoxItem() { Text = item.SDReader.firstYearAgeReadingGeneral.ToString(), CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();

                //first year readings species/stock
                firstYearReadingSpeciesStock = tempSDReaderItemList.Where(x => x.SDReader != null &&  x.SDReader.firstYearAgeReadingCurrent != null && x.SDReader.firstYearAgeReadingCurrent != 0)
                                         .DistinctBy(x => x.SDReader.firstYearAgeReadingCurrent)
                                         .Select(item => new DropDownListBoxItem() { Text = item.SDReader.firstYearAgeReadingCurrent.ToString(), CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();

                new Action(() =>
                {
                    UsedSpecies = species;
                    UsedPrepMethods = prepMethods;
                    UsedStocks = stocks;
                    UsedExperiences = experiences;
                    UsedFirstYearReadingGeneral = firstYearReadingGeneral;
                    UsedFirstYearReadingSpeciesStock = firstYearReadingSpeciesStock;

                    SDReaderItemList = tempSDReaderItemList;
                    FilterSDReaderItems();

                }).Dispatch();

            }
            catch (Exception e)
            {

                LogAndDispatchUnexpectedErrorMessage(e);
            }

        }

        public void FilterSDReaderItems()
        {
            try
            {

                if (_sdReaderItemList == null)
                {
                    FilteredSDReaderItemList = new List<SDReaderItem>();
                    return;
                }

                IEnumerable<SDReaderItem> lst = _sdReaderItemList;

                if (!string.IsNullOrWhiteSpace(_searchString))
                {
                    var search = _searchString ?? "";
                    lst = lst.Where(x => (x.SDReader.DFUPerson != null && x.SDReader.DFUPerson.UIDisplay.Contains(search, StringComparison.InvariantCultureIgnoreCase)) ||
                                         (!string.IsNullOrEmpty(x.SDReader.comment) && x.SDReader.comment.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                    );
                }

                var selectedSPecies = UsedSpecies == null ? new HashSet<string>() : UsedSpecies.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedPrepMethods = UsedPrepMethods == null ? new HashSet<string>() : UsedPrepMethods.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedStock = UsedStocks == null ? new HashSet<string>() : UsedStocks.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedExperiences = UsedExperiences == null ? new HashSet<string>() : UsedExperiences.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedFirstYearReadingGeneral = UsedFirstYearReadingGeneral == null ? new HashSet<string>() : UsedFirstYearReadingGeneral.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedFirstYearReadingSpeciesStock = UsedFirstYearReadingSpeciesStock == null ? new HashSet<string>() : UsedFirstYearReadingSpeciesStock.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();


                if (selectedSPecies.Count > 0)
                    lst = lst.Where(x => x.SDReader != null && !string.IsNullOrEmpty(x.SDReader.speciesCode) && selectedSPecies.Contains(x.SDReader.speciesCode ?? ""));

                if (selectedPrepMethods.Count > 0)
                    lst = lst.Where(x => x.SDReader != null && x.SDReader.L_SDPreparationMethod != null && selectedPrepMethods.Contains(x.SDReader.L_SDPreparationMethod.UIDisplay ?? ""));

                if (selectedStock.Count > 0)
                    lst = lst.Where(x => x.SDReader != null && x.SDReader.L_Stock != null && selectedStock.Contains(x.SDReader.L_Stock.UIDisplay ?? ""));

                if (selectedExperiences.Count > 0)
                    lst = lst.Where(x => x.SDReader != null && x.SDReader.L_SDReaderExperience != null && selectedExperiences.Contains(x.SDReader.L_SDReaderExperience.UIDisplay ?? ""));

                if (selectedFirstYearReadingGeneral.Count > 0)
                    lst = lst.Where(x => x.SDReader != null && x.SDReader.firstYearAgeReadingGeneral != null && selectedFirstYearReadingGeneral.Contains(x.SDReader.firstYearAgeReadingGeneral.ToString() ?? ""));

                if (selectedFirstYearReadingSpeciesStock.Count > 0)
                    lst = lst.Where(x => x.SDReader != null && x.SDReader.firstYearAgeReadingCurrent != null && selectedFirstYearReadingSpeciesStock.Contains(x.SDReader.firstYearAgeReadingCurrent.ToString() ?? ""));


                FilteredSDReaderItemList = lst.ToList();
            }
            catch (Exception e)
            {

                LogAndDispatchUnexpectedErrorMessage(e);
            }

        }

        public void RaiseIsAllSelected()
        {
            RaisePropertyChanged(() => HasSelectedSDReaders);
            RaisePropertyChanged(() => SelectedSDReadersCount);
        }


        #region Cancel Command


        public DelegateCommand CancelCommand
        {
            get
            {
                if (_cmdCancel == null)
                    _cmdCancel = new DelegateCommand(() => Return());

                return _cmdCancel;
            }
        }


        /// <summary>
        /// Cancel any alterations or the new sample.
        /// </summary>
        private void Return()
        {
            IsCanceled = true;
            Close();
        }


        //Window closed button same logic as cancel button
        public override void FireClosing(object sender, CancelEventArgs e)
        {
            try
            {
                if (!IsCanceled)
                    return;

                //check for changes
                if (HasSelectedSDReaders)
                {
                    if (AppRegionManager.ShowMessageBox(Translate("SelectSDReadersView", "WarningHasSelectedReaders"), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                    {
                        e.Cancel = true;
                        return;
                    }

                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        #endregion

        #region Add Readers Command
        public DelegateCommand AddReadersCommand
        {
            get
            {
                if (_cmdSave == null)
                    _cmdSave = new DelegateCommand(() => AddReaders());

                return _cmdSave;
            }
        }

        private void OnFilterItemChanged(DropDownListBoxItem lb, bool oldValue, bool newValue)
        {
            FilterSDReaderItems();
        }
        private void AddReaders()
        {
            try
            {
                if (SelectedSDReadersCount == 0)
                {
                    if (AppRegionManager.ShowMessageBox(Translate("SelectSDReadersView", "WarningNothingSelected"), System.Windows.MessageBoxButton.OK) == System.Windows.MessageBoxResult.OK)
                        return;
                }
                else
                {
                    if (SDReaderItemList != null && SDReaderItemList.Count > 0 && SDReaderItemList.Any(x => x.IsSelected))
                        SelectedSDReaderItemList = SDReaderItemList.Where(x => x.IsSelected).ToList();

                    if (SelectedSDReaderItemList != null && SelectedSDReaderItemList.Count > 10)
                    {
                        if (AppRegionManager.ShowMessageBox(string.Format(Translate("SelectSDReadersView", "WarningTenOrMoreSelected"), SelectedSDReaderItemList.Count), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                        {
                            return;
                        }
                    }

                    R_SDEventSDReader alreadyAdded = null;

                    //If a reader has already been added, don't allow it to be added again. Otherwise you won't know whether the user is an advanced or basic or other if they are added twice. Also the statistics of how many readings a user has
                    //done won't be accurate any more. 
                    if(SelectedSDReaderItemList != null && SelectedSDReaderItemList.Count > 0 &&
                        _addEditSDEventVM != null && (alreadyAdded =_addEditSDEventVM.SDReaderList.Where(r => r.SDReader != null && r.SDReader.DFUPerson != null && SelectedSDReaderItemList.Where(x => x.SDReader != null && x.SDReader.dfuPersonId ==  r.SDReader.dfuPersonId).Any()).FirstOrDefault()) != null)
                    {
                        DispatchMessageBox(string.Format(Translate("SelectSDReadersView", "ReaderAlreadyExists"), alreadyAdded.SDReader.DFUPerson.UIDisplay));
                        return;
                    }

                    IsCanceled = false;
                    Close();
                }
            }
            catch (Exception e)
            {

                LogAndDispatchUnexpectedErrorMessage(e);
            }
            
        }
        #endregion

        #region Add Edit Lookups Command
        public DelegateCommand<string> AddEditLookupsCommand
        {
            get { return _cmdAddEditLookups ?? (_cmdAddEditLookups = new DelegateCommand<string>(p => AddEditLookups(p))); }
        }


        /// <summary>
        /// Add/Or edit a lookup list.
        /// </summary>
        /// <param name="strType"></param>
        private void AddEditLookups(string strType)
        {
            try
            {
                if (!HasUserViewLookupRights())
                    return;

                ViewModels.Lookup.LookupManagerViewModel lm = GetLookupManagerViewModel(strType);

                if (lm == null)
                    throw new ApplicationException("Lookup type unrecognized.");

                Action<object, AViewModel> evtClosed = null;
                evtClosed = (obj, vm) =>
                {
                    lm.Closed -= evtClosed;
                    if (vm is LookupManagerViewModel && !(vm as LookupManagerViewModel).ChangesSaved)
                        return;

                    IsLoading = true;
                    //Reload project leaders drop down list (so any changes in the lookup manager are reflected).
                    Task.Factory.StartNew(() => Initialize()).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
                };

                lm.Closed += evtClosed;
                AppRegionManager.LoadWindowViewFromViewModel(lm, true, "WindowWithBorderStyle");
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }
        #endregion

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
    }
}
