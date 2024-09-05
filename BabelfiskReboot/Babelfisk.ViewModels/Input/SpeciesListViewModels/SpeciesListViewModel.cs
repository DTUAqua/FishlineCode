using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Anchor.Core;
using Babelfisk.BusinessLogic;
using Babelfisk.BusinessLogic.DataInput;
using Babelfisk.Entities;
using Babelfisk.Entities.Sprattus;
using Babelfisk.ViewModels.TreeView;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;


namespace Babelfisk.ViewModels.Input
{
    public partial class SpeciesListViewModel : AViewModel
    {
        private DelegateCommand _cmdShowCruise;
        private DelegateCommand _cmdShowTrip;
        private DelegateCommand _cmdShowStation;
        private DelegateCommand _cmdShowParent;
        private DelegateCommand _cmdDeleteAllSpeciesListItems;

        private DelegateCommand<SpeciesListItem> _cmdShowLAVRep;
        private DelegateCommand<SpeciesListItem> _cmdShowSFRep;
        private DelegateCommand<SpeciesListItem> _cmdShowSFNotRep;

        protected DelegateCommand _cmdSave;
        protected DelegateCommand _cmdClose;
        
        protected DelegateCommand _cmdNewRow;
        protected DelegateCommand<SpeciesListItem> _cmdRemoveRow;

        private SpeciesListItem _selectedItem;

        private bool _blnAssigningLookups = false;
        private int _intSampleId;

        private Sample _sample;
        private Cruise _cruise;
        private Trip _trip;

        private List<L_Species> _lstSpecies;
        private List<L_LandingCategory> _lstLandingCategories;
        private List<L_SizeSortingEU> _lstSizeSortingsEU;
        private List<L_SizeSortingDFU> _lstSizeSortingsDFU;
        private List<L_Treatment> _lstTreatments;
        private List<L_SexCode> _lstSexCodes;
        private List<L_WeightEstimationMethod> _lstWeightEstimationMethods;
        private List<TreatmentFactor> _lstTreatmentFactors;
        private Dictionary<int, L_Application> _lstApplications;

        private List<SpeciesListItem> _lstAllItems;

        private ObservableCollection<SpeciesListItem> _lstItems;

        /// <summary>
        /// Current sorting state.
        /// </summary>
        private string _strSortMemberPath = null;
        private ListSortDirection? _sortDirection = null;

        private SpeciesListStatisticsViewModel _vmSpeciesListStatistics;

        private bool _blnIsNew = false;

        private ColumnVisibilityViewModel _vmColumnVisibility;


        #region Properties


        public bool CanEditOffline
        {
            get
            {
                //IsNew should not be here, since if the sample is brought from "home", it should not be possible to add or change the specieslist no matter what. If IsNew
                //is wanted later, there should also be added a check whether there exist any species list records with offlinestate == added, if so, it can be saved.
                return !BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline || /*IsNew ||*/ (IsLoading || (_sample != null && _sample.OfflineState == ObjectState.Added));
            }
        }


        public bool HasEditingRights
        {
            get
            {
                return User != null && User.HasTask(SecurityTask.ModifyData);
            }
        }

        public bool IsNew
        {
            get { return _blnIsNew; }
            set
            {
                _blnIsNew = value;
                RaisePropertyChanged(() => IsNew);
            }
        }


        /// <summary>
        /// Implement has unsaved data to incorporate a check that ensures the user knows when he/she is 
        /// moving away from an unsaved species list.
        /// </summary>
        public override bool HasUnsavedData
        {
            get
            {
                if (!HasEditingRights || !CanEditOffline)
                    return false;

                return (_lstItems == null ? false : _lstItems.Where(x => x.HasUnsavedData).Count() > 0) || IsDirty || (_sample != null && _sample.ChangeTracker.State == ObjectState.Modified);
            }
        }


        public string TripType
        {
            get { return _trip == null ? null : _trip.tripType; }
        }


        public Sample Sample
        {
            get { return _sample;  }
            private set
            {
                _sample = value;
                RaisePropertyChanged(() => Sample);
                RaisePropertyChanged(() => WeightEstimationMethod);
                RaisePropertyChanged(() => TotalWeight);
            }
        }


        public Cruise Cruise
        {
            get { return _cruise; }
            private set { _cruise = value; RaisePropertyChanged(() => Cruise); }
        }


        public Trip Trip
        {
            get { return _trip; }
            private set { _trip = value; RaisePropertyChanged(() => Trip); RaisePropertyChanged(() => TripType); }
        }


        public ObservableCollection<SpeciesListItem> Items
        {
            get { return _lstItems; }
            set
            {
                _lstItems = value;
                RaisePropertyChanged(() => Items);
            }
        }


        public List<L_Species> Species
        {
            get { return _lstSpecies == null ? null : _lstSpecies.ToList(); }
            private set
            {
                _lstSpecies = value;
                RaisePropertyChanged(() => Species);
            }
        }



        public List<L_LandingCategory> LandingCategories
        {
            get { return _lstLandingCategories == null ? null : _lstLandingCategories.ToList(); }
            private set
            {
                _lstLandingCategories = value;
                RaisePropertyChanged(() => LandingCategories);
            }
        }


        public List<L_SizeSortingEU> SizeSortingsEU
        {
            get { return _lstSizeSortingsEU == null ? null : _lstSizeSortingsEU.ToList(); }
            private set
            {
                _lstSizeSortingsEU = value;
                RaisePropertyChanged(() => SizeSortingsEU);
            }
        }


        public List<L_SizeSortingDFU> SizeSortingsDFU
        {
            get { return _lstSizeSortingsDFU == null ? null : _lstSizeSortingsDFU.ToList(); }
            private set
            {
                _lstSizeSortingsDFU = value;
                RaisePropertyChanged(() => SizeSortingsDFU);
            }
        }


        public List<L_Treatment> Treatments
        {
            get { return _lstTreatments == null ? null : _lstTreatments.ToList(); }
            private set
            {
                _lstTreatments = value;
                RaisePropertyChanged(() => Treatments);
            }
        }


        public List<L_SexCode> SexCodes
        {
            get { return _lstSexCodes == null ? null : _lstSexCodes.ToList(); }
            private set
            {
                _lstSexCodes = value;
                RaisePropertyChanged(() => SexCodes);
            }
        }


        public List<L_WeightEstimationMethod> WeightEstimationMethods
        {
            get { return _lstWeightEstimationMethods == null ? null : _lstWeightEstimationMethods.ToList(); }
            private set
            {
                _lstWeightEstimationMethods = value;
                RaisePropertyChanged(() => WeightEstimationMethods);
            }
        }

        public List<L_Application> Applications
        {
            get { return _lstApplications == null ? null : _lstApplications.Values.ToList(); }
        }

        public Dictionary<int, L_Application> ApplicationsDic
        {
            get { return _lstApplications; }
            private set
            {
                _lstApplications = value;
                RaisePropertyChanged(() => Applications);
                RaisePropertyChanged(() => ApplicationsDic);
            }
        }


        public SpeciesListItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;

                //Assign statistics
                if (value == null)
                    SpeciesListStatistics = null;
                else
                    SpeciesListStatistics = new SpeciesListStatisticsViewModel(value);

                RaisePropertyChanged(() => SelectedItem);
                RaisePropertyChanged(() => HasSelectedItem);
                RefreshSums();
            }
        }


        public bool HasSelectedItem
        {
            get { return SelectedItem != null; }
        }


        public decimal? TotalWeight
        {
            get { return _sample == null ? null : _sample.totalWeight; }
            set
            {
                if (_sample == null)
                    return;

                _sample.totalWeight = value;
                IsDirty = true;
                RaisePropertyChanged(() => TotalWeight);
                RaisePropertyChanged(() => IsDirty);
            }
        }

        
        public L_WeightEstimationMethod WeightEstimationMethod
        {
            get { return (_sample == null || _lstWeightEstimationMethods == null) ? null : _lstWeightEstimationMethods.Where(x => x.weightEstimationMethod.Equals(_sample.weightEstimationMethod, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(); }
            set
            {
                if (!_blnAssigningLookups)
                {
                    var val = (value == null ? null : value.weightEstimationMethod);
                    if (_sample.weightEstimationMethod != val)
                        _sample.weightEstimationMethod = val;

                    IsDirty = true;
                }
                RaisePropertyChanged(() => WeightEstimationMethod);
                RaisePropertyChanged(() => IsDirty);
            }
        }

       

        public SpeciesListStatisticsViewModel SpeciesListStatistics
        {
            get { return _vmSpeciesListStatistics; }
            set
            {
                _vmSpeciesListStatistics = value;
                RaisePropertyChanged(() => SpeciesListStatistics);
            }
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


        public List<TreatmentFactor> TreatmentFactors
        {
            get { return _lstTreatmentFactors; }
        }


        #endregion



        public SpeciesListViewModel(int intSampleId, bool blnInitialize = true)
        {
            _intSampleId = intSampleId;
            Items = new ObservableCollection<SpeciesListItem>();
           
            if(blnInitialize)
                InitializeAsync();

            RegisterToKeyDown();
        }


        #region Initialization methods


        public Task InitializeAsync(int? intSelectedItemId = null)
        {
            IsLoading = true;
            return Task.Factory.StartNew(() => Initialize(intSelectedItemId)).ContinueWith(t => new Action(() => { IsLoading = false; RaisePropertyChanged(() => CanEditOffline); }).Dispatch());
        }


        private void Initialize(int? intSelectedItemId = null)
        {
            try
            {
                var man = new DataRetrievalManager();
                var manInput = new DataInputManager();
                var manLookup = new LookupManager();

                var lv = new LookupDataVersioning();

                var s = manInput.GetEntity<Sample>(_intSampleId);
                var t = manInput.GetEntity<Trip>(s.tripId, "L_FisheryType");
                var c = manInput.GetEntity<Cruise>(t.cruiseId);

                var lstSpecies = manLookup.GetLookups(typeof(L_Species), lv).OfType<L_Species>().OrderBy(x => x.UIDisplay).ToList();
                var lstSexCodes = manLookup.GetLookups(typeof(L_SexCode), lv).OfType<L_SexCode>().OrderBy(x => x.UIDisplay).ToList();
                List<L_SizeSortingEU> lstSizeSortingsEU = null;
                List<L_SizeSortingDFU> lstSizeSortingsDFU = null;
                List<L_LandingCategory> lstLandingCategories = null;
                List<L_WeightEstimationMethod> lstWeightEstimationMethods = null;
                List<L_Treatment> lstTreatments = null;
                Dictionary<int, L_Application> dicApplications = null;
                List<TreatmentFactor> lstTreatmentFactors = manLookup.GetLookups(typeof(TreatmentFactor), lv).OfType<TreatmentFactor>().OrderBy(x => x.UIDisplay).ToList();
               
                List<SpeciesList> lst = null;

                //10-11-2017: For optimization purposes and in offline mode, reuse above grabbed sample to get the species list items.
                if (BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
                {
                    //The Sample (s) retrieved above is a clone, meaning that all its SpeciesList items are also a clone (from the Trip loaded in memory). This is very important, since all species list items
                    //needs to be a clone from the Trip in memory. This ensures that any unsaved changes discarded by the user, are not saved to the Trip in memory. Because editting species list items directly
                    //from the trip loaded in memory, could result in situations where discarded changes made by the user, are actually not discarded, but instead saved to disk.
                    if (s == null || s.SpeciesList == null)
                        lst = new List<SpeciesList>();
                    else
                        lst = s.SpeciesList.ToList();

                    DataRetrievalManager.CalculateSpeciesListPresentFlags(lst);
                }
                else
                {
                   lst = man.GetSpeciesLists(_intSampleId);
                }
                List<SpeciesListItem> lstItems = null;

                //Initial sort by species list id
                if (lst != null && lst.Count > 0)
                {
                    //Offline specieslists have negative ids, so compensate for that in the default sorting.
                    int intMaxId = lst.Max(x => x.speciesListId);
                    lst = lst.OrderBy(x => x.speciesListId < 0 ? intMaxId + Math.Abs(x.speciesListId) : x.speciesListId).ToList();
                }

                switch (t.tripType.ToLower())
                {
                    case "hvn":
                        lstItems = lst.Select((x, i) => new SpeciesListHVNItem(this, x) { Index = i+1 }).OfType<SpeciesListItem>().ToList();
                        lstSizeSortingsEU = manLookup.GetLookups(typeof(L_SizeSortingEU), lv).OfType<L_SizeSortingEU>().OrderBy(x => x.UIDisplay).ToList();
                        lstLandingCategories = manLookup.GetLookups(typeof(L_LandingCategory), lv).OfType<L_LandingCategory>().OrderBy(x => x.UIDisplay).ToList();
                        lstTreatments = manLookup.GetLookups(typeof(L_Treatment), lv).OfType<L_Treatment>().OrderBy(x => x.UIDisplay).ToList();
                        break;

                    case "vid":
                        lstItems = lst.Select((x, i) => new SpeciesListVIDItem(this, x) { Index = i + 1 }).OfType<SpeciesListItem>().ToList();
                        lstSizeSortingsDFU = manLookup.GetLookups(typeof(L_SizeSortingDFU), lv).OfType<L_SizeSortingDFU>().OrderBy(x => x.UIDisplay).ToList();
                        break;

                    case "søs":
                        lstItems = lst.Select((x, i) => new SpeciesListSEAItem(this, x) { Index = i + 1 }).OfType<SpeciesListItem>().ToList();
                        lstSizeSortingsEU = manLookup.GetLookups(typeof(L_SizeSortingEU), lv).OfType<L_SizeSortingEU>().OrderBy(x => x.UIDisplay).ToList();
                        lstWeightEstimationMethods = manLookup.GetLookups(typeof(L_WeightEstimationMethod), lv).OfType<L_WeightEstimationMethod>().OrderBy(x => x.UIDisplay).ToList();
                        lstLandingCategories = manLookup.GetLookups(typeof(L_LandingCategory), lv).OfType<L_LandingCategory>().OrderBy(x => x.UIDisplay).ToList();
                        lstTreatments = manLookup.GetLookups(typeof(L_Treatment), lv).OfType<L_Treatment>().OrderBy(x => x.UIDisplay).ToList();
                        lstSizeSortingsDFU = manLookup.GetLookups(typeof(L_SizeSortingDFU), lv).OfType<L_SizeSortingDFU>().OrderBy(x => x.UIDisplay).ToList();
                        break;

                    case "rektbd":
                    case "rekhvn":
                    case "rekomr":
                        lstItems = lst.Select((x, i) => new SpeciesListREKItem(this, x) { Index = i + 1 }).OfType<SpeciesListItem>().ToList();
                        lstLandingCategories = manLookup.GetLookups(typeof(L_LandingCategory), lv).OfType<L_LandingCategory>().OrderBy(x => x.UIDisplay).ToList();
                        lstTreatments = manLookup.GetLookups(typeof(L_Treatment), lv).OfType<L_Treatment>().OrderBy(x => x.UIDisplay).ToList();
                        lstSizeSortingsDFU = manLookup.GetLookups(typeof(L_SizeSortingDFU), lv).OfType<L_SizeSortingDFU>().OrderBy(x => x.UIDisplay).ToList();
                        var nc = new Anchor.Core.Comparers.NaturalComparer();
                        var lstAppl = manLookup.GetLookups(typeof(L_Application), lv).OfType<L_Application>().OrderBy(x => x.UIDisplay, nc).DistinctBy(x => x.L_applicationId).ToList();
                        dicApplications = lstAppl.ToDictionary(x => x.L_applicationId);
                        break;

                    default:
                        
                        break;
                }

                new Action(() =>
                {
                    IsDirty = false;

                    IsNew = lst == null || lst.Count == 0;

                    Sample = s;
                    Trip = t;
                    Cruise = c;

                    ColumnVisibility = new ColumnVisibilityViewModel(TripType, "SpeciesList", ColumnVisibilityChanged);
                    ColumnVisibility.WindowTitle = string.Format("Viste kolonner for:  {0}-artslisten", TripType);

                    _blnAssigningLookups = true;
                    {
                        Species = lstSpecies;
                        LandingCategories = lstLandingCategories;
                        SizeSortingsEU = lstSizeSortingsEU;
                        SizeSortingsDFU = lstSizeSortingsDFU;
                        Treatments = lstTreatments;
                        SexCodes = lstSexCodes;
                        WeightEstimationMethods = lstWeightEstimationMethods;
                        ApplicationsDic = dicApplications;
                        _lstTreatmentFactors = lstTreatmentFactors;
                    }
                    _blnAssigningLookups = false;

                    _lstAllItems = lstItems;

                    IsLoading = false;
                    AssignItems(intSelectedItemId);
                    SyncNewRows();

                    RefreshAllNotifiableProperties();
                }).Dispatch();
            }
            catch (Exception e)
            {
                if (e is TimeoutException || e is EndpointNotFoundException)
                    DispatchMessageBox("Det var ikke muligt at oprette forbindelse til databasen.");
                else
                    DispatchMessageBox("En uventet fejl opstod. " + e.Message);
            }
        }

        private void ColumnVisibilityChanged(List<Babelfisk.BusinessLogic.Settings.DataGridColumnSettings> cs)
        {
            //Refresh trip type since it is bound to all column, forcing their vibility to be refreshed
            RaisePropertyChanged(() => TripType);
        }


        private void AssignItems(int? intSelectedItemId)
        {
            IEnumerable<SpeciesListItem> vLst = _lstAllItems.Where(x => x.SpeciesListEntity.ChangeTracker.State != ObjectState.Deleted);

            //Do not reassign the list directly, since this would reset any sortings. Clear the collection instead
            //and add the new items. This ensures that a new ICollectionView will not be re-created on the bound DataGrid.
            Items.Clear();
            Items.AddRange(vLst);
           // Items = vLst.ToObservableCollection();

            if (_strSortMemberPath != null)
                Sort(_strSortMemberPath, _sortDirection);

            if (intSelectedItemId.HasValue)
            {
                SelectedItem = vLst.Where(x => x.SpeciesListEntity != null && x.SpeciesListEntity.speciesListId == intSelectedItemId.Value).FirstOrDefault();

                FocusSelectedItem();
            }
        }


        public void FocusSelectedItem()
        {
            ScrollTo(() => SelectedItem);
        }


        public void SyncNewRows()
        {
            //Add new rows to the bottom of the grid so there will always be some.
            int intWantedNewRows = 4;
            int intExistingNewRowsCount = 0;
            if (_lstAllItems.Count > 0)
            {
                for (int i = _lstAllItems.Count - 1; i >= 0; i--)
                {
                    if (!_lstAllItems[i].HasUnsavedData && _lstAllItems[i].IsNew)
                        intExistingNewRowsCount++;
                    else
                        break;
                }
            }

            for (int i = intExistingNewRowsCount; i < intWantedNewRows; i++)
            {
                 var itm = GetNewSpeciesListItem();

                 _lstAllItems.Add(itm);
                 Items.Add(itm);
            }
        }


        #endregion


        #region Sorting Methods

        public void Sort(string strSortMemberPath, ListSortDirection? direction)
        {
            _strSortMemberPath = strSortMemberPath;
            _sortDirection = direction;

            ObservableCollection<SpeciesListItem> source = _lstItems;

            if (_lstItems == null)
                return;

            for (int i = source.Count - 1; i >= 0; i--)
            {
                var row = source[i];
                //Ignore new empty rows
                if (row.IsNew && !row.HasUnsavedData)
                    continue;

                for (int j = 1; j <= i; j++)
                {
                    var row1 = source[j - 1];
                    var row2 = source[j];

                    var value1 = GetRowValue(strSortMemberPath, row1) as IComparable;
                    var value2 = GetRowValue(strSortMemberPath, row2) as IComparable;

                    int sortResult = 0;

                    if(value2 != null)
                        sortResult = ((value1 == null) ? 1 : value1.CompareTo(value2));

                    sortResult = direction == ListSortDirection.Ascending ? sortResult * 1 : sortResult * -1;

                    if (sortResult > 0)
                    {
                        // Position the item correctly
                        source.Move(j - 1, j);
                    }
                }
            }
        }

        private object GetRowValue(string strProperty, object obj)
        {
            var prop = obj.GetType().GetProperty(strProperty, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            if (prop != null)
                return prop.GetValue(obj, new object[] { });
            else
                return null;
        }

        #endregion


        #region Show Cruise Command


        public DelegateCommand ShowCruiseCommand
        {
            get { return _cmdShowCruise ?? (_cmdShowCruise = new DelegateCommand(ShowCruise)); }
        }


        private void ShowCruise()
        {
            var vm = new ViewModels.Input.CruiseViewModel(_cruise.cruiseId);
            bool blnSuccess = AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, vm);

            if (blnSuccess)
            {
                Task.Factory.StartNew(() =>
                {
                    var node = MainTree.GetCruiseNodeIfLoaded(_cruise.cruiseId);

                    if (node != null)
                        new Action(() => { node.IsSelected = true; MainTree.ExpandToNode(node); }).Dispatch();
                });
            }
        }


        #endregion


        #region Show Trip Command


        public DelegateCommand ShowTripCommand
        {
            get { return _cmdShowTrip ?? (_cmdShowTrip = new DelegateCommand(ShowTrip)); }
        }


        private void ShowTrip()
        {
            AViewModel avm = null;

            if (TripType.Contains("HVN", StringComparison.InvariantCultureIgnoreCase))
                avm = new ViewModels.Input.TripHVNViewModel(_sample.tripId);
            else
                avm = new ViewModels.Input.TripViewModel(_sample.tripId);

            bool blnSuccess = AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, avm);

            if (blnSuccess)
            {
                Task.Factory.StartNew(() =>
                {
                    var node = MainTree.GetTripNodeIfLoaded(_sample.tripId);

                    if (node != null)
                        new Action(() => { node.IsSelected = true; MainTree.ExpandToNode(node); }).Dispatch();
                });
            }
        }


        #endregion


        #region Show Station Command


        public DelegateCommand ShowStationCommand
        {
            get { return _cmdShowStation ?? (_cmdShowStation = new DelegateCommand(ShowStation)); }
        }


        private void ShowStation()
        {
            AViewModel avm = new ViewModels.Input.StationViewModel(_sample.sampleId);

            bool blnSuccess = AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, avm);

            if (blnSuccess)
            {
                Task.Factory.StartNew(() =>
                {
                    var node = MainTree.GetSampleNodeIfLoaded(_sample.sampleId);

                    if (node != null)
                        new Action(() => { node.IsSelected = true; MainTree.ExpandToNode(node); }).Dispatch();
                });
            }
        }


        #endregion


        #region Show Parent Command


        public DelegateCommand ShowParentCommand
        {
            get { return _cmdShowParent ?? (_cmdShowParent = new DelegateCommand(ShowParent)); }
        }


        private void ShowParent()
        {
            if (TripType.IsHVN())
            {
                ShowTrip();
            }
            else
            {
                ShowStation();
            }
        }


        #endregion


        #region Save Command

        public DelegateCommand SaveCommand
        {
            get { return _cmdSave ?? (_cmdSave = new DelegateCommand(() => ValidateAndSaveAsync())); }
        }


        /// <summary>
        /// Validate and save changes to database.
        /// </summary>
        protected virtual void ValidateAndSaveAsync()
        {
            if (!HasEditingRights)
            {
                AppRegionManager.ShowMessageBox("Du har ikke rettigheder til at gemme eventuelle ændringer.");
                return;
            }

            //Validate rows
            string strError = GetRowError();

            //If errors are found, abort saving and show error message.
            if (strError != null)
            {
                AppRegionManager.ShowMessageBox("Der er fundet fejl i tabellen (felter markeret med rødt). Hold musen over et felt for at læse mere om fejlen.");
                return;
            }

            if (!HandleWarningsAndContinue())
                return;

            IsLoading = true;
            Task.Factory.StartNew(ValidateAndSave).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        private string GetRowError()
        {
            string strError = null;
            var items = Items.ToList();

            foreach (var itm in items)
            {
                if (!itm.HasUnsavedData)
                    continue;

                itm.ValidateAllProperties();

                if (itm.HasErrors && strError == null)
                    strError = itm.Error;
            }

            return strError;
        }


        private bool HandleWarningsAndContinue()
        {
            try
            {
                bool blnWarnings = false;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Advarsler:");
                sb.AppendLine("");

                var items = Items.ToList();

                //Check all non empty species list items for empty landingscategories.
                if (!this.TripType.IsVID() && items.Where(x => (x.HasUnsavedData || !x.IsNew) && string.IsNullOrEmpty(x.LandingCategory)).Count() > 0)
                {
                    sb.AppendLine(" - Landingskategori er ikke udfyldt for alle rækker i artslisten.");
                    blnWarnings = true;
                }

                decimal? decSum = Step0SumUR;
                if (!this.TripType.IsHVN() && ((decSum.HasValue && !decSum.Equals(TotalWeight)) || (TotalWeight.HasValue && !TotalWeight.Equals(decSum))))
                {
                    sb.AppendLine(string.Format(" - 'Totalfangst (UR) (kg)' ({0}kg) passer ikke med summen af vægtene (urenset) i Trin0 ({1}kg).", TotalWeight.HasValue ? TotalWeight.Value.ToString("0.####") : "N/A", decSum.HasValue ? decSum.Value.ToString("0.####") : "N/A"));
                    blnWarnings = true;
                }

                if (items.Where(x => (x.HasUnsavedData || !x.IsNew) && !x.IsWeightsSpecified).Count() > 0)
                {
                    sb.AppendLine(" - En eller flere rækker har ikke en trin-vægt angivet.");
                    blnWarnings = true;
                }


                if (blnWarnings)
                {
                    sb.AppendLine("");
                    sb.AppendLine("Er du sikker på du vil fortsætte med at gemme?");

                    var res = AppRegionManager.ShowMessageBox(sb.ToString(), System.Windows.MessageBoxButton.YesNo);
                    if (res == System.Windows.MessageBoxResult.No)
                        return false;
                }
            }
            catch (Exception e)
            {
                var res = AppRegionManager.ShowMessageBox("En uventet fejl opstod under søgningen efter advarsler. Fejl: " + e.Message + Environment.NewLine + "Vil du fortsætte med at gemme artslisten?", System.Windows.MessageBoxButton.YesNo);
                if (res == System.Windows.MessageBoxResult.No)
                    return false;
            }

            return true;
        }


        private void CopySubsamplesToHighestLevel(List<SpeciesList> saveItems)
        {
            foreach(var sl in saveItems)
            {
                var ssOld = sl.SubSample.Where(x => x.IsRepresentative && x.Animal.Count > 0).FirstOrDefault();

                if (ssOld == null)
                    continue;

                var ssNew = sl.SubSample.Where(x => x.IsRepresentative && x.subSampleWeight != null).OrderByDescending(x => x.stepNum).FirstOrDefault();

                if (ssNew == ssOld)
                    continue;

                var lstAnimalToMove = ssOld.Animal.ToList();
                ssOld.Animal.Clear();
                ssNew.Animal.Clear();

                ssNew.Animal.AddRange(lstAnimalToMove);
            }
        }

        protected void ValidateAndSave()
        {
            try
            {
                var man = new DataInputManager();
                DatabaseOperationResult res = DatabaseOperationResult.CreateSuccessResult();

                List<SpeciesList> saveItems = _lstAllItems.Where(x => x.HasUnsavedData).Select(x => x.SpeciesListEntity).ToList();

                //If no changes are recorded, show message saying so, and abort calling webservice.
                if (saveItems.Count == 0 && !((TripType.IsSEA() || TripType.IsVID())  && _sample.ChangeTracker.State == ObjectState.Modified))
                {
                    new Action(() => IsDirty = false).Dispatch();
                    DispatchMessageBox("Der var ingen tilføjelser/ændringer at gemme.");
                    return;
                }

                //Call BeforeSave method on all species list items to save.
                _lstAllItems.Where(x => x.HasUnsavedData).ToList().ForEach(x => x.BeforeSave());

                CopySubsamplesToHighestLevel(saveItems);

                //Save modified/new species list items
                if(saveItems.Count > 0)
                    res = man.SaveSpeciesListItems(ref saveItems);

                if ((TripType.IsSEA() || TripType.IsVID()) && res.DatabaseOperationStatus == DatabaseOperationStatus.Successful)
                    res = man.SaveSample(ref _sample);

                if (res.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                {
                    //  if (res.DatabaseOperationStatus == DatabaseOperationStatus.ValidationError && res.Message == "DuplicateKey")
                    //      DispatchMessageBox(String.Format("En station med nummer '{0}' eksisterer allerede.", _sample.station));
                    //  else
                    DispatchMessageBox("En uventet fejl opstod. " + res.Message);

                    return;
                }
                else
                {
                    //Save local copy of species list items and sample and tripid (so no conflict occurs when intializing data again and refreshing tree)
                    var items = Items.ToList();
                    int intSampleId = _sample.sampleId;
                    int intTripId = _sample.tripId;

                    //Initialize control again (reloading data from db)
                    Initialize();

                    RefreshTree(items, intTripId, intSampleId);

                    Babelfisk.ViewModels.Security.BackupManager.Instance.Backup();
                }
            }
            catch (Exception e)
            {
                DispatchMessageBox(String.Format("En uventet fejl opstod. {0}", e.Message));
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }


        /// <summary>
        /// Refresh tree based on species list items.
        /// </summary>
        private void RefreshTree(List<SpeciesListItem> items, int intTripId, int intSampleId)
        {
            //Refresh tree, if node exists.
            TreeItemViewModel treeNode = null;

            if (TripType.IsHVN())
                treeNode = MainTree.GetTripNodeIfLoaded(intTripId);
            else
                treeNode = MainTree.GetSampleNodeIfLoaded(intSampleId);

            if (treeNode != null)
            {
                //Make sure trip is assigned to have at least one sample (since one has now been created)
                // if (!treeNode.HasChildren)
                {
                    var hasSpeciesList = items.Where(x => !(x.IsNew && !x.HasUnsavedData)).Count() > 0 ? 1 : 0;

                    if (TripType.IsHVN())
                        (treeNode as TripTreeItemViewModel).TripEntity.SampleCount = hasSpeciesList;
                    else
                        (treeNode as SampleTreeItemViewModel).SampleEntity.SpeciesListCount = hasSpeciesList;
                }

                //Refresh tree and select saved sample node if possible.
                treeNode.RefreshTreeAsync().ContinueWith(t =>
                {
                    MainTree.SelectTreeNode(intSampleId, TreeNodeLevel.SpeciesList);
                });
            }
        }


        #endregion


        #region Close Command

        public DelegateCommand CloseCommand
        {
            get { return _cmdClose ?? (_cmdClose = new DelegateCommand(() => CloseViewModel())); }
        }


        protected virtual void CloseViewModel()
        {
            //Redirect to start view.
            Menu.MainMenuViewModel.ShowStart();
        }


        #endregion


        #region New Row Command


        public DelegateCommand AddRowCommand
        {
            get { return _cmdNewRow ?? (_cmdNewRow = new DelegateCommand(() => NewRow())); }
        }


        private SpeciesListItem GetNewSpeciesListItem()
        {
            SpeciesListItem sli = null;

            int intNewIndex = (_lstAllItems == null || _lstAllItems.Count == 0) ? 1 : _lstAllItems.Max(x => x.Index) + 1;

            switch (TripType.ToLower())
            {
                case "hvn":
                    sli = SpeciesListItem.New<SpeciesListHVNItem>(this, Sample.sampleId, intNewIndex);
                    break;

                case "søs":
                    sli = SpeciesListItem.New<SpeciesListSEAItem>(this, Sample.sampleId, intNewIndex);
                    break;

                case "vid":
                    sli = SpeciesListItem.New<SpeciesListVIDItem>(this, Sample.sampleId, intNewIndex);
                    break;

                case "rekhvn":
                case "rekomr":
                case "rektbd":
                    sli = SpeciesListItem.New<SpeciesListREKItem>(this, Sample.sampleId, intNewIndex);
                    break;
            }

            return sli;
        }

        protected void NewRow()
        {
            if (TripType == null)
            {
                DispatchMessageBox("Der kan ikke tilføjes nye rækker før programmet er færdig med at hente data.");
                return;
            }

            SpeciesListItem sli = GetNewSpeciesListItem();

            if (sli != null)
            {
                _lstAllItems.Add(sli);
                Items.Add(sli);

               // if(blnAssignItems)
               //     AssignItems();
            }
        }


        #endregion


        #region Remove Command


        public DelegateCommand<SpeciesListItem> RemoveCommand
        {
            get
            {
                if (_cmdRemoveRow == null)
                    _cmdRemoveRow = new DelegateCommand<SpeciesListItem>(param => RemoveItem(param, true));

                return _cmdRemoveRow;
            }
        }


        private void RemoveItem(SpeciesListItem item, bool blnShowWarningMessage = false)
        {
            if (item == null)
                return;

            //Show confirmation message if lookup to remove is an existing db value.
            if (item.SpeciesListEntity.ChangeTracker.State != ObjectState.Added)
            {
                if (blnShowWarningMessage)
                {
                    var res = AppRegionManager.ShowMessageBox("Er du sikker på du vil fjerne den valgte række", System.Windows.MessageBoxButton.YesNo);

                    if (res == System.Windows.MessageBoxResult.No)
                        return;
                }

                item.SpeciesListEntity.MarkAsDeleted();
                Items.Remove(item);
                IsDirty = true;
            }
            else
            {
                _lstAllItems.Remove(item);
                Items.Remove(item);

                IsDirty = true;
            }

            //AssignItems();
        }

        #endregion


        #region Show LAV Rep Command

        public DelegateCommand<SpeciesListItem> ShowLAVRepCommand
        {
            get { return _cmdShowLAVRep ?? (_cmdShowLAVRep = new DelegateCommand<SpeciesListItem>(param => ShowLAVRep(param))); }
        }


        private void ShowLAVRep(SpeciesListItem item)
        {
            if (item == null)
                return;

            if (item.SpeciesListEntity.SubSample.Where(x => x.IsRepresentative).Count() == 0)
            {
                AppRegionManager.ShowMessageBox("Tilføj venligst vægt(e) til et eller flere trin inden du tilgår 'LAV, rep'.");
                return;
            }

            if (item.HasUnsavedData)
            {
                AppRegionManager.ShowMessageBox("Gem venligst artslisten inden du går videre til 'LAV, rep'.");
                return;
            }

            SubSampleViewModel ssvm = new SubSampleViewModel(SelectedItem.SpeciesListEntity.speciesListId, SubSampleType.LAVRep, this);
            AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, ssvm);
        }

        #endregion


        #region Show SF Rep Command

        public DelegateCommand<SpeciesListItem> ShowSFRepCommand
        {
            get { return _cmdShowSFRep ?? (_cmdShowSFRep = new DelegateCommand<SpeciesListItem>(param => ShowSFRep(param))); }
        }


        private void ShowSFRep(SpeciesListItem item)
        {
            if (item == null)
                return;

            if (item.SpeciesListEntity.SubSample.Where(x => x.IsRepresentative).Count() == 0)
            {
                AppRegionManager.ShowMessageBox("Tilføj venligst vægt(e) til et eller flere trin inden du tilgår 'Enkeltfisk, rep'.");
                return;
            }

            if (item.HasUnsavedData)
            {
                AppRegionManager.ShowMessageBox("Gem venligst artslisten inden du går videre til 'Enkeltfisk, rep'.");
                return;
            }

            //SubSampleViewModel ssvm = new SubSampleViewModel(SelectedItem.SpeciesListEntity.speciesListId, SubSampleType.SFRep, this);
            //AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, ssvm);
            ShowSF(SelectedItem.SpeciesListEntity.speciesListId, SubSampleType.SFRep, this);
        }


        public static void ShowSF(int intSpeciesListId, SubSampleType t, SpeciesListViewModel vm)
        {
            SubSampleViewModel ssvm = new SubSampleViewModel(intSpeciesListId, t, vm);
            vm.AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, ssvm);
        }

        #endregion


        #region Show SF Not Rep Command

        public DelegateCommand<SpeciesListItem> ShowSFNotRepCommand
        {
            get { return _cmdShowSFNotRep ?? (_cmdShowSFNotRep = new DelegateCommand<SpeciesListItem>(param => ShowSFNotRep(param))); }
        }


        private void ShowSFNotRep(SpeciesListItem item)
        {
            if (item == null)
                return;

            if (item.SpeciesListEntity.SubSample.Where(x => !x.IsRepresentative).Count() == 0)
            {
                AppRegionManager.ShowMessageBox("Angiv venligst en vægt i 'Ej rep' inden du tilgår 'Enkeltfisk, ej rep'.");
                return;
            }

            if (item.HasUnsavedData)
            {
                AppRegionManager.ShowMessageBox("Gem venligst artslisten inden du går videre til 'Enkeltfisk, ej rep'.");
                return;
            }

            //SubSampleViewModel ssvm = new SubSampleViewModel(SelectedItem.SpeciesListEntity.speciesListId, SubSampleType.SFNotRep, this);
            //AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, ssvm);
            ShowSF(SelectedItem.SpeciesListEntity.speciesListId, SubSampleType.SFNotRep, this);
        }

        #endregion


        #region Delete All SpeciesLists Command


        public DelegateCommand DeleteAllSpeciesListItemsCommand
        {
            get
            {
                if (_cmdDeleteAllSpeciesListItems == null)
                    _cmdDeleteAllSpeciesListItems = new DelegateCommand(RemoveAllSpeciesListItems);

                return _cmdDeleteAllSpeciesListItems;
            }
        }


        private void RemoveAllSpeciesListItems()
        {
            if (Items != null && Items.Where(x => !x.CanEditOffline).Any())
            {
                AppRegionManager.ShowMessageBox("En eller flere rækker i artslisten kan ikke fjernes (dette skyldes højst sandsynligt at de er blevet oprettet mens Fiskeline var online), handlingen annulleres derfor.");
                return;
            }

            if (AppRegionManager.ShowMessageBox("Er du sikker på alle rækker i artslisten skal slettes?", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;

            foreach (var itm in Items.ToList())
            {
                RemoveItem(itm);
            }

            SyncNewRows();
        }

        #endregion


        public static void LoadSpeciesList(int intSampleId)
        {
            SpeciesListViewModel svm = new SpeciesListViewModel(intSampleId);
            _appRegionManager.LoadViewFromViewModel(RegionName.MainRegion, svm);
        }


        protected override void GlobelPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            //Save on Ctrl + S
            if (HasEditingRights
                && ((e.Key == System.Windows.Input.Key.S && Keyboard.Modifiers == ModifierKeys.Control) || ((e.SystemKey == Key.G || e.Key == Key.G) && (Keyboard.Modifiers == ModifierKeys.Alt || Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt))))
                )
                ValidateAndSaveAsync();

            if ((e.Key == System.Windows.Input.Key.N && Keyboard.Modifiers == ModifierKeys.Control) || ((e.SystemKey == Key.N || e.Key == Key.N) && (Keyboard.Modifiers == ModifierKeys.Alt || Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt))))
            {
                e.Handled = true;
                ScrollTo("NewItem");
            }
        }
    }
}
