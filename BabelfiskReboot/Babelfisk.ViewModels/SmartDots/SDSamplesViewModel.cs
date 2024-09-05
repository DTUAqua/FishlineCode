using Babelfisk.Entities.Sprattus;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anchor.Core;
using Babelfisk.BusinessLogic.SmartDots;
using Babelfisk.ViewModels.Map;
using Babelfisk.BusinessLogic;
using Anchor.Core.Controls;
using System.ComponentModel;
using Babelfisk.ViewModels.Input;
using System.IO;
using System.Windows.Forms;
using Babelfisk.Entities;
using System.Data.Odbc;

namespace Babelfisk.ViewModels.SmartDots
{
    public class SDSamplesViewModel : AViewModel
    {
        private DelegateCommand _cmdReturnToEvents;
        private DelegateCommand _cmdEditEvent;
        private DelegateCommand _cmdShowSelectAnimalsViewCommand;
        private DelegateCommand _cmdShowImportFromCSVViewCommand;
        private DelegateCommand _cmdSaveSamples;
        private DelegateCommand<SDSample> _cmdDeleteSample;
        private DelegateCommand _cmdAddRow;
        private DelegateCommand<SDSample> _cmdEditSample;
        private DelegateCommand _cmdShowMoveCopy;
        private DelegateCommand _cmdCloseMoveCopy;
        private DelegateCommand _cmdCopySamples;
        private DelegateCommand _cmdMoveSamples;
        private DelegateCommand _cmdShowRemoveMultipleSamples;
        private DelegateCommand _cmdCloseDeleteMultipleSamples;
        private DelegateCommand _cmdDeleteMultipleSamples;
        private DelegateCommand _cmdRunInconsistencyCheck;
        private DelegateCommand _cmdCopyAgesToFishLineAnimals;
        private DelegateCommand _cmdCopyScaleToAllSelectedSamples;
        private DelegateCommand _cmdUpdateAgesInFishLine;
        private DelegateCommand _cmdAssignScaleToSamples;
        private DelegateCommand _cmdLookForMissingImages;

        private SDEventsViewModel _vmEvents;

        private SDEvent _sdEvent;
        private string _searchString;

        private SDSample _entity;

        private List<L_DFUArea> _lstDFUArea;
        private List<L_StatisticalRectangle> _lstStatisticalRectangle;
        private List<L_SexCode> _lstSexCode;
        private List<L_MaturityIndexMethod> _lstMaturityIndexMethod;
        private List<Maturity> _lstMaturity;
        private List<L_OtolithReadingRemark> _lstOtolithReadingRemarks;
        private List<L_SDOtolithDescription> _lstSDOtolithDescriptions;
        private List<L_EdgeStructure> _lstEdgeStructures;
        private List<L_SDPreparationMethod> _lstPreparationMethods;
        private List<L_SDLightType> _lstLightTypes;
        private List<L_Stock> _lstStocks;
        private List<L_Species> _lstSpecies;


        private List<DropDownListBoxItem> _lengths;
        private List<DropDownListBoxItem> _weights;
        private List<DropDownListBoxItem> _sexes;
        private List<DropDownListBoxItem> _maturities;
        private List<DropDownListBoxItem> _otolithReadingRemarks;
        private List<DropDownListBoxItem> _otolithDescriotions;
        private List<DropDownListBoxItem> _edgeStructures;
        private List<DropDownListBoxItem> _DFUAreas;
        private List<DropDownListBoxItem> _statisticalRectangles;
        private List<DropDownListBoxItem> _cruises;
        private List<DropDownListBoxItem> _trips;
        private List<DropDownListBoxItem> _stations;
        private List<DropDownListBoxItem> _stocks;
        private List<DropDownListBoxItem> _createdByLst;
        private List<DropDownListBoxItem> _prepMethods;
        private List<DropDownListBoxItem> _catchDates;
        private List<DropDownListBoxItem> _created;
        private List<DropDownListBoxItem> _modified;
        private List<DropDownListBoxItem> _lightType;

        private AViewModel _activeViewModel;

        private List<SDSample> _lstSDSamples;
        private List<SDSample> _lstFilteredSDSamples;
        private List<SDSample> _lstDeletedSDSamples;

        private List<SDEvent> _eventsList;
        private SDEvent _selectedEvent;

        private bool _isAddingSamplesFromFishline;
        private string _addingSamplesFromFishlineLoadingText;

        private bool _updatingCollections;
        private bool _isMenuDropDownOpen;
        private bool _isMoveCopyVisible;
        private bool _isDeleteMultipleVisible;

        private bool _isCopyingAgesToFishLineActivated;
        private bool _isCopyingScaleToSamplesActivated; 

        private ColumnVisibilityViewModel _vmColumnVisibility;

        public bool CancelPressed = false;
        public bool _isCheckingForChanges = false;

        private bool ShouldCheckForChanges = false;

        private HashSet<string> _DFUAreasHashSet;

        private double? _scaleToBeAssigned;

        #region Properties


        public double? ScaleToBeAssigned
        {
            get { return _scaleToBeAssigned; }
            set
            {
                _scaleToBeAssigned = value;
                RaisePropertyChanged(() => ScaleToBeAssigned);
            }
        }


        public bool IsCheckingForChanges
        {
            get { return _isCheckingForChanges; }
            set
            {
                _isCheckingForChanges = value;
                RaisePropertyChanged(() => IsCheckingForChanges);
            }
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

        public override bool IsDirty
        {
            get { return base.IsDirty || _lstSDSamples.Where(x => x.ChangeTracker.State == ObjectState.Unchanged).ToList().Count != _lstSDSamples.Count; }
            set { base.IsDirty = value; }
        }

        /// <summary>
        /// Get/Set what to filter the animals by. The events are filtered on name, event type, species, and start/stop date.
        /// </summary>
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
                RaisePropertyChanged(() => SearchString);
                FilterSDSamples();
            }
        }


        /// <summary>
        /// Reference to the event entity from where the samples are attached.
        /// </summary>
        public SDEvent Event
        {
            get { return _sdEvent; }
        }

        public HashSet<string> DFUAreasHashSet
        {
            get
            {
                if (_DFUAreasHashSet == null && Event != null && Event.L_DFUAreas != null)
                {
                    _DFUAreasHashSet = Event.L_DFUAreas.Select(x => x.DFUArea).Distinct().ToHashSet<string>();
                    return _DFUAreasHashSet;
                }

                return _DFUAreasHashSet;
            }
        }

        /// <summary>
        /// If this property has a value, the associated view to the assigned viewmodel will be shown instead of the events list. When this is set to null, the events list will be shown again.
        /// </summary>
        public AViewModel ActiveViewModel

        {
            get { return _activeViewModel; }
            set
            {
                _activeViewModel = value;
                RaisePropertyChanged(() => ActiveViewModel);
                RaisePropertyChanged(() => HasActiveViewModel);
            }
        }

        /// <summary>
        /// Whether or not ActiveViewModel has a viewmodel assigned.
        /// </summary>
        public bool HasActiveViewModel
        {
            get { return _activeViewModel != null; }
        }


        public bool IsAddingSamplesFromFishline
        {
            get { return _isAddingSamplesFromFishline; }
            set
            {
                _isAddingSamplesFromFishline = value;
                RaisePropertyChanged(() => IsAddingSamplesFromFishline);
            }
        }

        public string AddingSamplesFromFishlineLoadingText
        {
            get { return _addingSamplesFromFishlineLoadingText; }
            set
            {
                _addingSamplesFromFishlineLoadingText = value;
                RaisePropertyChanged(() => AddingSamplesFromFishlineLoadingText);
            }
        }

        public List<SDSample> SDSampleList
        {
            get { return _lstSDSamples; }
            set
            {
                _lstSDSamples = value;
                RaisePropertyChanged(() => SDSampleList);
                RaisePropertyChanged(() => HasSamples);
                RaisePropertyChanged(() => TotalSamples);
            }
        }

        public List<SDSample> FilteredSDSamplesList
        {
            get { return _lstFilteredSDSamples.Where(x => x.ChangeTracker.State != ObjectState.Deleted).ToList(); }
            set
            {
                _lstFilteredSDSamples = value;
                RaisePropertyChanged(() => FilteredSDSamplesList);
                RaisePropertyChanged(() => HasSamples);
                RaisePropertyChanged(() => TotalChangedSamples);
                RaisePropertyChanged(() => TotalVisibleSamples);
            }
        }

        public int TotalChangedSamples
        {
            get { return (_lstFilteredSDSamples == null ? 0 : _lstFilteredSDSamples.Where(x => x.ChangeTracker.State != ObjectState.Unchanged).Count()) + (_lstDeletedSDSamples == null ? 0 : _lstDeletedSDSamples.Count); }
        }

        public int TotalVisibleSamples
        {
            get { return _lstFilteredSDSamples == null ? 0 : _lstFilteredSDSamples.Count; }
        }

        public int TotalSamples
        {
            get { return _lstSDSamples == null ? 0 : _lstSDSamples.Count; }
        }


        public List<SDSample> DeletedSDSamplesList
        {
            get { return _lstDeletedSDSamples; }
            set
            {
                _lstDeletedSDSamples = value;
                RaisePropertyChanged(() => DeletedSDSamplesList);
            }
        }

        public List<SDEvent> EventsList
        {
            get { return _eventsList; }
            set
            {
                _eventsList = value;
                RaisePropertyChanged(() => EventsList);
            }
        }

        public SDEvent SelectedEvent
        {
            get { return _selectedEvent; }
            set
            {
                _selectedEvent = value;
                RaisePropertyChanged(() => SelectedEvent);

            }
        }

        /// <summary>
        /// Return list of DFU Areas for drop down. 
        /// </summary>
        public List<L_DFUArea> DFUAreas
        {
            get { return _lstDFUArea == null ? null : _lstDFUArea.ToList(); }
            private set
            {
                _lstDFUArea = value;
                RaisePropertyChanged(() => DFUAreas);
            }
        }

        /// <summary>
        /// Return list of Statistical Rectangles for drop down. 
        /// </summary>
        public List<L_StatisticalRectangle> StatisticalRectangles
        {
            get { return _lstStatisticalRectangle == null ? null : _lstStatisticalRectangle.ToList(); }
            private set
            {
                _lstStatisticalRectangle = value;
                RaisePropertyChanged(() => StatisticalRectangles);
            }
        }

        /// <summary>
        /// Return list of Sex Codes for drop down. 
        /// </summary>
        public List<L_SexCode> SexCodes
        {
            get { return _lstSexCode == null ? null : _lstSexCode.ToList(); }
            private set
            {
                _lstSexCode = value;
                RaisePropertyChanged(() => SexCodes);
            }
        }

        /// <summary>
        /// Return list of Maturity Index Methods for drop down. 
        /// </summary>
        public List<L_MaturityIndexMethod> MaturityIndexMethods
        {
            get { return _lstMaturityIndexMethod == null ? null : _lstMaturityIndexMethod.ToList(); }
            private set
            {
                _lstMaturityIndexMethod = value;
                RaisePropertyChanged(() => MaturityIndexMethods);
            }
        }

        /// <summary>
        /// Return list of Maturity Index Methods for drop down. 
        /// </summary>
        public List<Maturity> MaturityList
        {
            get { return _lstMaturity == null ? null : _lstMaturity.ToList(); }
            private set
            {
                _lstMaturity = value;
                RaisePropertyChanged(() => MaturityIndexMethods);
            }
        }

        /// <summary>
        /// Return list of Otolith Reading Remarks for drop down. 
        /// </summary>
        public List<L_OtolithReadingRemark> OtolithReadingRemarks
        {
            get { return _lstOtolithReadingRemarks == null ? null : _lstOtolithReadingRemarks.ToList(); }
            private set
            {
                _lstOtolithReadingRemarks = value;
                RaisePropertyChanged(() => OtolithReadingRemarks);
            }
        }

        /// <summary>
        /// Return list of Otolith Descriptions for drop down. 
        /// </summary>
        public List<L_SDOtolithDescription> OtolithDescriptions
        {
            get { return _lstSDOtolithDescriptions == null ? null : _lstSDOtolithDescriptions.ToList(); }
            private set
            {
                _lstSDOtolithDescriptions = value;
                RaisePropertyChanged(() => OtolithDescriptions);
            }
        }


        /// <summary>
        /// Return list of Edge Structures for drop down. 
        /// </summary>
        public List<L_EdgeStructure> EdgeStructures
        {
            get { return _lstEdgeStructures == null ? null : _lstEdgeStructures.ToList(); }
            private set
            {
                _lstEdgeStructures = value;
                RaisePropertyChanged(() => EdgeStructures);
            }
        }

        /// <summary>
        /// Return list of Preparation Methods for drop down. 
        /// </summary>
        public List<L_SDPreparationMethod> PreparationMethods
        {
            get { return _lstPreparationMethods == null ? null : _lstPreparationMethods.ToList(); }
            private set
            {
                _lstPreparationMethods = value;
                RaisePropertyChanged(() => PreparationMethods);
            }
        }

        /// <summary>
        /// Return list of Light Types for drop down. 
        /// </summary>
        public List<L_SDLightType> LightTypes
        {
            get { return _lstLightTypes == null ? null : _lstLightTypes.ToList(); }
            private set
            {
                _lstLightTypes = value;
                RaisePropertyChanged(() => LightTypes);
            }
        }

        /// <summary>
        /// Return list of Species for drop down. 
        /// </summary>
        public List<L_Species> Species

        {
            get { return _lstSpecies == null ? null : _lstSpecies.ToList(); }
            private set
            {
                _lstSpecies = value;
                RaisePropertyChanged(() => Species);
            }
        }

        /// <summary>
        /// Return list of Stocks for drop down. 
        /// </summary>
        public List<L_Stock> Stocks

        {
            get { return _lstStocks == null ? null : _lstStocks.ToList(); }
            private set
            {
                _lstStocks = value;
                RaisePropertyChanged(() => Stocks);
            }
        }

        /// <summary>
        /// Return list of Lengths for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedLengths
        {
            get { return _lengths; }
            set
            {
                _lengths = value;
                RaisePropertyChanged(() => UsedLengths);
            }
        }

        /// <summary>
        /// Return list of Weights for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedWeights
        {
            get { return _weights; }
            set
            {
                _weights = value;
                RaisePropertyChanged(() => UsedWeights);
            }
        }

        /// <summary>
        /// Return list of Sexes for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedSexes
        {
            get { return _sexes; }
            set
            {
                _sexes = value;
                RaisePropertyChanged(() => UsedSexes);
            }
        }

        /// <summary>
        /// Return list of Sexes for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedMaturities
        {
            get { return _maturities; }
            set
            {
                _maturities = value;
                RaisePropertyChanged(() => UsedMaturities);
            }
        }

        /// <summary>
        /// Return list of Otolith Reading Remarks for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedOtolithReadingRemarks
        {
            get { return _otolithReadingRemarks; }
            set
            {
                _otolithReadingRemarks = value;
                RaisePropertyChanged(() => UsedOtolithReadingRemarks);
            }
        }

        /// <summary>
        /// Return list of OtolithDescriptions for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedOtolithDescriptions
        {
            get { return _otolithDescriotions; }
            set
            {
                _otolithDescriotions = value;
                RaisePropertyChanged(() => UsedOtolithDescriptions);
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
        /// Return list of UsedEdge Structures for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedEdgeStructures
        {
            get { return _edgeStructures; }
            set
            {
                _edgeStructures = value;
                RaisePropertyChanged(() => UsedEdgeStructures);
            }
        }

        /// <summary>
        /// Return list of DFU Areas for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedDFUAreas
        {
            get { return _DFUAreas; }
            set
            {
                _DFUAreas = value;
                RaisePropertyChanged(() => UsedDFUAreas);
            }
        }


        /// <summary>
        /// Return list of Statistical Rectangles for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedStatisticalRectangles
        {
            get { return _statisticalRectangles; }
            set
            {
                _statisticalRectangles = value;
                RaisePropertyChanged(() => UsedStatisticalRectangles);
            }
        }

        /// <summary>
        /// Return list of Cruises for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedCruises
        {
            get { return _cruises; }
            set
            {
                _cruises = value;
                RaisePropertyChanged(() => UsedCruises);
            }
        }

        /// <summary>
        /// Return list of Trips for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedTrips
        {
            get { return _trips; }
            set
            {
                _trips = value;
                RaisePropertyChanged(() => UsedTrips);
            }
        }

        /// <summary>
        /// Return list of Stations for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedStations
        {
            get { return _stations; }
            set
            {
                _stations = value;
                RaisePropertyChanged(() => UsedStations);
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
        /// Return list of Catch dates for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedCatchDates
        {
            get { return _catchDates; }
            set
            {
                _catchDates = value;
                RaisePropertyChanged(() => UsedCatchDates);
            }
        }

        /// <summary>
        /// Return list of Created time  for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedCreatedTime
        {
            get { return _created; }
            set
            {
                _created = value;
                RaisePropertyChanged(() => UsedCreatedTime);
            }
        }

        /// <summary>
        /// Return list of Modified time for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedModifiedTime
        {
            get { return _modified; }
            set
            {
                _modified = value;
                RaisePropertyChanged(() => UsedModifiedTime);
            }
        }

        /// <summary>
        /// Return list of Light types for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedLightTypes
        {
            get { return _lightType; }
            set
            {
                _lightType = value;
                RaisePropertyChanged(() => UsedLightTypes);
            }
        }

        /// <summary>
        /// Return list of CreatedBy for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedCreatedBy
        {
            get { return _createdByLst; }
            set
            {
                _createdByLst = value;
                RaisePropertyChanged(() => UsedCreatedBy);
            }
        }

        public bool IsMenuDropDownOpen
        {
            get { return _isMenuDropDownOpen; }
            set
            {
                _isMenuDropDownOpen = value;
                RaisePropertyChanged(() => IsMenuDropDownOpen);
            }
        }

        public bool IsMoveCopyVisible
        {
            get { return _isMoveCopyVisible; }
            set
            {
                _isMoveCopyVisible = value;
                RaisePropertyChanged(() => IsMoveCopyVisible);
                RaisePropertyChanged(() => IsCheckboxColumnVisible);
                RaisePropertyChanged(() => IsPopupFeatureActive);
            }
        }

        public bool IsDeleteMultipleVisible
        {
            get { return _isDeleteMultipleVisible; }
            set
            {
                _isDeleteMultipleVisible = value;
                RaisePropertyChanged(() => IsDeleteMultipleVisible);
                RaisePropertyChanged(() => IsCheckboxColumnVisible);
                RaisePropertyChanged(() => IsPopupFeatureActive);
            }
        }

        public bool IsCopyingAgesToFishLineActivated
        {
            get { return _isCopyingAgesToFishLineActivated; }
            set
            {
                _isCopyingAgesToFishLineActivated = value;
                RaisePropertyChanged(() => IsCopyingAgesToFishLineActivated);
                RaisePropertyChanged(() => IsCheckboxColumnVisible);
                RaisePropertyChanged(() => IsPopupFeatureActive);
            }
        }

        public bool IsCopyingScaleToSamplesActivated
        {
            get { return _isCopyingScaleToSamplesActivated; }
            set
            {
                _isCopyingScaleToSamplesActivated = value;
                RaisePropertyChanged(() => IsCopyingScaleToSamplesActivated);
                RaisePropertyChanged(() => IsCheckboxColumnVisible);
                RaisePropertyChanged(() => IsPopupFeatureActive);
            }
        }

        public bool IsPopupFeatureActive
        {
            get { return _isMoveCopyVisible || _isCopyingAgesToFishLineActivated || _isCopyingScaleToSamplesActivated || _isMoveCopyVisible; }
        }


        public bool IsCheckboxColumnVisible
        {
            get { return _isMoveCopyVisible || _isCopyingAgesToFishLineActivated || _isCopyingScaleToSamplesActivated || _isDeleteMultipleVisible; }
        }

        public bool HasSamples
        {
            get { return _lstSDSamples != null && _lstSDSamples.Count > 0; }
        }

        private bool UpdatingCollections
        {
            get { return _updatingCollections || _blnValidate; }
        }

        public string EventSpeciesString
        {
            get { return Event != null && Event.L_SDEventType != null && !SDEvent.IsEventTypeYearlyReading(Event.L_SDEventType.L_sdEventTypeId) && Event.speciesCode != null ? string.Format("({0})", Event.speciesCode) : ""; }
        }

        public bool IsGridReadOnly
        {
            get { return !User.HasAddEditSDEventsAndSamplesTask; }
        }

        public bool? IsAllSelected
        {
            get
            {

                if (_lstFilteredSDSamples != null && _lstFilteredSDSamples.Count > 0 && _lstFilteredSDSamples.Count == _lstFilteredSDSamples.Where(x => x.IsSelected).ToList().Count())
                    return true;
                else if (_lstFilteredSDSamples != null && _lstFilteredSDSamples.Any(x => x.IsSelected))
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

        public bool HasSelectedSamples
        {
            get { return IsAllSelected == null || IsAllSelected == true; }
        }

        public int SelectedSamplesCount
        {
            get { return _lstFilteredSDSamples != null ? _lstFilteredSDSamples.Where(x => x.IsSelected).Count() : 0; }
        }

        public bool HasInconsistencies
        {
            get { return SDSampleList != null && SDSampleList.Count > 0 ? CheckSampleEventInconsistencies() : false; }
        }

        #endregion




        public SDSamplesViewModel(SDEventsViewModel vmEvents, SDEvent evt)
        {
            WindowWidth = 1100;
            WindowHeight = 500;

            _vmEvents = vmEvents;
            _sdEvent = evt;

            _eventsList = new List<SDEvent>();
 
            _lstDeletedSDSamples = new List<SDSample>();
            _lstFilteredSDSamples = new List<SDSample>();
            _lstSDSamples = new List<SDSample>();

            IsDirty = false;

            try
            {
                ColumnVisibility = new ColumnVisibilityViewModel(ColumnVisivilityAny, "SDSamples", ColumnVisibilityChanged);
                ColumnVisibility.WindowTitle = string.Format("Viste kolonner");
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }


        public void RefreshTotals()
        {
            RaisePropertyChanged(() => TotalVisibleSamples);
            RaisePropertyChanged(() => TotalSamples);
            RaisePropertyChanged(() => TotalChangedSamples);
        }

        /// <summary>
        /// Show the associated to view to the supplied viewmodel in the UI.
        /// </summary>
        /// <param name="vm"></param>
        public void ActivateViewModel(AViewModel vm)
        {
            ActiveViewModel = vm;
        }


        /// <summary>
        /// Close any visible viewmodel and show the events list instead.
        /// </summary>
        public void CloseActiveViewModel()
        {
            ActiveViewModel = null;
        }



        public void InitializeAsync(bool checkForChanges = true)
        {
            ShouldCheckForChanges = checkForChanges;
            IsLoading = true;
            Task.Factory.StartNew(Initialize).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }

        /// <summary>
        /// Load view model synchronously loading all drop down list data.
        /// </summary>
        private void Initialize()
        {
            try
            {
                if (_sdEvent != null && _vmEvents != null && _vmEvents.AllEvents != null && _vmEvents.AllEvents.Count > 0)
                {

                   EventsList = _vmEvents.AllEvents.Where(x => x.sdEventId != _sdEvent.sdEventId).OrderByDescending(x => x.sdEventId).ToList();
                }

                var cmp = new Anchor.Core.Comparers.StringNumberComparer();

                //System.Threading.Thread.Sleep(60000);
                var manLookup = new LookupManager();
                var lv = new LookupDataVersioning();

                //Fetch DFUArea
                var lstDFUArea = manLookup.GetLookups(typeof(L_DFUArea), lv).OfType<L_DFUArea>().OrderBy(x => x.UIDisplay, cmp).ToList();

                //Fetch Statistical Rectangle
                var lstStatisticalRectangle = manLookup.GetLookups(typeof(L_StatisticalRectangle), lv).OfType<L_StatisticalRectangle>().OrderBy(x => x.UIDisplay, cmp).ToList();

                //Fetch Sex Codes
                var lstSexCode = manLookup.GetLookups(typeof(L_SexCode), lv).OfType<L_SexCode>().OrderBy(x => x.UIDisplay, cmp).ToList();

                //Fetch Maturity Index Method
                var lstMaturityIndexMethod = manLookup.GetLookups(typeof(L_MaturityIndexMethod), lv).OfType<L_MaturityIndexMethod>().OrderBy(x => x.UIDisplay, cmp).ToList();

                //Fetch Otolith Reading Remarks
                var lstOtolithReadingRemarks = manLookup.GetLookups(typeof(L_OtolithReadingRemark), lv).OfType<L_OtolithReadingRemark>().OrderBy(x => x.UIDisplay, cmp).ToList();

                //Fetch Otolith Reading Remarks
                var lstOtolithSDDescription = manLookup.GetLookups(typeof(L_SDOtolithDescription), lv).OfType<L_SDOtolithDescription>().OrderBy(x => x.UIDisplay, cmp).ToList();

                //Fetch Edge Structures
                var lstEdgeStructures = manLookup.GetLookups(typeof(L_EdgeStructure), lv).OfType<L_EdgeStructure>().OrderBy(x => x.UIDisplay, cmp).ToList();

                //Fetch Preparation Methods
                var lstPreparationMethod = manLookup.GetLookups(typeof(L_SDPreparationMethod), lv).OfType<L_SDPreparationMethod>().OrderBy(x => x.UIDisplay, cmp).ToList();

                //Fetch Light Types
                var lstLightTypes = manLookup.GetLookups(typeof(L_SDLightType), lv).OfType<L_SDLightType>().OrderBy(x => x.UIDisplay, cmp).ToList();

                //Fetch species
                var lstSpecies = manLookup.GetLookups(typeof(L_Species), lv).OfType<L_Species>().OrderBy(x => x.UIDisplay, cmp).ToList();

                //Fetch stocks
                var lststocks = manLookup.GetLookups(typeof(L_Stock), lv).OfType<L_Stock>().OrderBy(x => x.UIDisplay, cmp).ToList();

                //Fetch Maturity (which fits with maturity id)
                var lstmaturity = manLookup.GetLookups(typeof(Maturity), lv).OfType<Maturity>().OrderBy(x => x.maturityIndexMethod ?? "" , cmp).ThenBy(x => x.maturityIndex).ToList();

                new Action(() =>
                {
                    RaisePropertyChanged(() => EventsList);

                    _updatingCollections = true;
                    {
                        DFUAreas = lstDFUArea;
                        StatisticalRectangles = lstStatisticalRectangle;
                        SexCodes = lstSexCode;
                        MaturityIndexMethods = lstMaturityIndexMethod;
                        MaturityList = lstmaturity;
                        OtolithReadingRemarks = lstOtolithReadingRemarks;
                        OtolithDescriptions = lstOtolithSDDescription;
                        EdgeStructures = lstEdgeStructures;
                        PreparationMethods = lstPreparationMethod;
                        LightTypes = lstLightTypes;
                        Species = lstSpecies;
                        Stocks = lststocks;
                    }
                    _updatingCollections = false;

                }).Dispatch();

               
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }

            LoadSamples();
        }

        /// <summary>
        /// Load samples synchronously.
        /// </summary>
        private void LoadSamples()
        {
            try
            {
                var man = new BusinessLogic.SmartDots.SmartDotsManager();
                var samples = man.GetSDSamples(_sdEvent.sdEventId);


                List<DropDownListBoxItem> lengths = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> weights = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> sexes = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> maturities = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> otolithReadingRemarks = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> otolithDescriptions = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> edgeStructures = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> dfuAreas = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> statisticalRectangles = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> cruises = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> trips = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> stations = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> stocks = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> prepMethods = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> lightTypes = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> catchDates = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> createdTime = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> modifiedTime = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> createdBy = new List<DropDownListBoxItem>();

                var cmp = new Anchor.Core.Comparers.StringNumberComparer();

                if (samples == null)
                    samples = new List<SDSample>();

                // lengths
                lengths = samples.Where(x => x.fishLengthMM != null)
                                         .DistinctBy(x => x.fishLengthMM)
                                         .Select(item => new DropDownListBoxItem() { Text = item.fishLengthMM.ToString(), CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();

                //weights
                weights = samples.Where(x => x.fishWeightG != null)
                                         .DistinctBy(x => x.fishWeightG)
                                         .Select(item => new DropDownListBoxItem() { Text = item.fishWeightG.ToString(), CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();
                //sex
                sexes = samples.Where(x => x.L_SexCode != null)
                                         .DistinctBy(x => x.L_SexCode)
                                         .Select(item => new DropDownListBoxItem() { Text = item.L_SexCode.UIDisplay, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();

                //maturities
                maturities = samples.Where(x => x.Maturity != null)
                                         .DistinctBy(x => x.Maturity)
                                         .Select(item => new DropDownListBoxItem() { Text = item.Maturity.FullUIDisplay, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();

                //DFU Areas
                dfuAreas = samples.Where(x => !string.IsNullOrWhiteSpace(x.DFUArea))
                                         .DistinctBy(x => x.DFUArea)
                                         .Select(item => new DropDownListBoxItem() { Text = item.DFUArea, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();

                //otolithReadingRemarks
                otolithReadingRemarks = samples.Where(x => x.otolithReadingRemarkId != null)
                                         .DistinctBy(x => x.otolithReadingRemarkId)
                                         .Select(item => new DropDownListBoxItem() { Text = item.L_OtolithReadingRemark.UIDisplay, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();

                //otolithDescriptions
                otolithDescriptions = samples.Where(x => x.sdOtolithDescriptionId != null)
                                         .DistinctBy(x => x.sdOtolithDescriptionId)
                                         .Select(item => new DropDownListBoxItem() { Text = item.L_SDOtolithDescription.UIDisplay, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();

                //prepMethods
                prepMethods = samples.Where(x => x.sdPreparationMethodId != null)
                                         .DistinctBy(x => x.sdPreparationMethodId)
                                         .Select(item => new DropDownListBoxItem() { Text = item.L_SDPreparationMethod.UIDisplay, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();
                //ligthTypes
                lightTypes = samples.Where(x => x.sdLightTypeId != null)
                                         .DistinctBy(x => x.sdLightTypeId)
                                         .Select(item => new DropDownListBoxItem() { Text = item.L_SDLightType.UIDisplay, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();

                //edgeStructures
                edgeStructures = samples.Where(x => x.L_EdgeStructure != null)
                                         .DistinctBy(x => x.L_EdgeStructure)
                                         .Select(item => new DropDownListBoxItem() { Text = item.L_EdgeStructure.UIDisplay, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();

                //Statistical Rectangles
                statisticalRectangles = samples.Where(x => !string.IsNullOrWhiteSpace(x.statisticalRectangle))
                                         .DistinctBy(x => x.statisticalRectangle)
                                         .Select(item => new DropDownListBoxItem() { Text = item.statisticalRectangle, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();

                //cruises
                cruises = samples.Where(x => !string.IsNullOrWhiteSpace(x.cruise))
                                         .DistinctBy(x => x.cruise)
                                         .Select(item => new DropDownListBoxItem() { Text = item.cruise, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();
                //trips
                trips = samples.Where(x => !string.IsNullOrWhiteSpace(x.trip))
                                         .DistinctBy(x => x.trip)
                                         .Select(item => new DropDownListBoxItem() { Text = item.trip, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();

                //stations
                stations = samples.Where(x => !string.IsNullOrWhiteSpace(x.station))
                                         .DistinctBy(x => x.station)
                                         .Select(item => new DropDownListBoxItem() { Text = item.station, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();

                //stocks
                stocks = samples.Where(x => x.L_Stock != null)
                                         .DistinctBy(x => x.L_Stock)
                                         .Select(item => new DropDownListBoxItem() { Text = item.L_Stock.UIDisplay, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();
                //catchDates
                catchDates = samples.Where(x => x.catchDate != null && x.catchDate != default(DateTime))
                                         .DistinctBy(x => x.catchDate)
                                         .OrderBy(x => x.catchDate)
                                         .Select(item => new DropDownListBoxItem() { Text = item.catchDate.Value.ToString("dd-MM-yy"), CheckedChangedMethodReference = OnFilterItemChanged })
                                         .ToList();
                //createdTime
                createdTime = samples.Where(x => x.createdTime != null && x.createdTime != default(DateTime))
                                         .DistinctBy(x => x.createdTime)
                                         .OrderBy(x => x.createdTime)
                                         .Select(item => new DropDownListBoxItem() { Text = item.createdTime.Value.ToString("dd-MM-yy"), CheckedChangedMethodReference = OnFilterItemChanged })
                                         .ToList();
                //modifiedTime
                modifiedTime = samples.Where(x => x.modifiedTime != null && x.modifiedTime != default(DateTime))
                                        .DistinctBy(x => x.modifiedTime)
                                        .OrderBy(x => x.modifiedTime)
                                        .Select(item => new DropDownListBoxItem() { Text = item.modifiedTime.Value.ToString("dd-MM-yy"), CheckedChangedMethodReference = OnFilterItemChanged })
                                        .ToList();
                //createdBy
                createdBy = samples.Where(x => !string.IsNullOrWhiteSpace(x.createdByUserName))
                                         .DistinctBy(x => x.createdByUserName)
                                         .Select(item => new DropDownListBoxItem() { Text = item.createdByUserName, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();


                //Make sure deleted samples list is reset when loading data in.
                if (_lstDeletedSDSamples != null)
                    _lstDeletedSDSamples.Clear();

              

                new Action(() =>
                {
                    SDSampleList = samples;
                    UsedLengths = lengths;
                    UsedWeights = weights;
                    UsedSexes = sexes;
                    UsedMaturities = maturities;
                    UsedOtolithReadingRemarks = otolithReadingRemarks;
                    UsedOtolithDescriptions = otolithDescriptions;
                    UsedEdgeStructures = edgeStructures;
                    UsedDFUAreas = dfuAreas;
                    UsedStatisticalRectangles = statisticalRectangles;
                    UsedCruises = cruises;
                    UsedTrips = trips;
                    UsedStations = stations;
                    UsedStocks = stocks;
                    UsedPrepMethods = prepMethods;
                    UsedCatchDates = catchDates;
                    UsedCreatedTime = createdTime;
                    UsedModifiedTime = modifiedTime;
                    UsedCreatedBy = createdBy;
                    UsedLightTypes = lightTypes;

                    FilterSDSamples();
                    RaisePropertyChanged(() => IsDirty);
                    RaisePropertyChanged(() => HasInconsistencies);

                    if (User.HasAddEditSDEventsAndSamplesTask && ShouldCheckForChanges && samples != null && samples.Count > 0)
                    {
                        RunInconsistentyCheckAsync();
                        ShouldCheckForChanges = false;
                    }
                }).Dispatch();
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }

        private void OnFilterItemChanged(DropDownListBoxItem lb, bool oldValue, bool newValue)
        {
            FilterSDSamples();
        }

        private void FilterSDSamples()
        {
            try
            {
                if (_lstSDSamples == null)
                {
                    FilteredSDSamplesList = new List<SDSample>();
                    return;
                }

                IEnumerable<SDSample> lstSamples = SDSampleList;

                if(!string.IsNullOrWhiteSpace(SearchString))
                {
                    var search = SearchString ?? "";
                    lstSamples = lstSamples.Where(x => !string.IsNullOrEmpty(x.animalId) && x.animalId.ToString().Contains(SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                                         (x.fishWeightG.HasValue && x.fishWeightG.Value.ToString().Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                         (x.latitude.HasValue && x.latitude.Value.ToString().Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                         (x.longitude.HasValue && x.longitude.Value.ToString().Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                         (!string.IsNullOrWhiteSpace(x.speciesCode) && x.speciesCode.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                         (x.catchDate!= null && x.catchDate.Value.ToString("dd-MM-yyyy").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                         (x.createdTime != null && x.createdTime.Value.ToString("dd-MM-yyyy").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                         (x.modifiedTime != null && x.modifiedTime.Value.ToString("dd-MM-yyyy").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                         (x.createdByUserName != null && x.createdByUserName.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                         (x.comments ?? "").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)
                                         );
                }

                var selectedLength = UsedLengths == null ? new HashSet<string>() : UsedLengths.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedWeight = UsedWeights == null ? new HashSet<string>() : UsedWeights.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedSex = UsedSexes == null ? new HashSet<string>() : UsedSexes.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedMaturity = UsedMaturities == null ? new HashSet<string>() : UsedMaturities.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedOtolithReadingRemarks = UsedOtolithReadingRemarks == null ? new HashSet<string>() : UsedOtolithReadingRemarks.Where(x => x.IsChecked).Select(x => x.Text.ToString()).Distinct().ToHashSet<string>();
                var selectedOtolithDescriptions = UsedOtolithDescriptions == null ? new HashSet<string>() : UsedOtolithDescriptions.Where(x => x.IsChecked).Select(x => x.Text.ToString()).Distinct().ToHashSet<string>();
                var selectedEdgeStructures = UsedEdgeStructures == null ? new HashSet<string>() : UsedEdgeStructures.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedDFUAreas = UsedDFUAreas == null ? new HashSet<string>() : UsedDFUAreas.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedStatRect = UsedStatisticalRectangles == null ? new HashSet<string>() : UsedStatisticalRectangles.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedCruises = UsedCruises == null ? new HashSet<string>() : UsedCruises.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedTrips = UsedTrips == null ? new HashSet<string>() : UsedTrips.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedStations = UsedStations == null ? new HashSet<string>() : UsedStations.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedStock = UsedStocks == null ? new HashSet<string>() : UsedStocks.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedPrepMethods = UsedPrepMethods == null ? new HashSet<string>() : UsedPrepMethods.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedLigthTypes = UsedLightTypes == null ? new HashSet<string>() : UsedLightTypes.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedCreatedBy = UsedCreatedBy == null ? new HashSet<string>() : UsedCreatedBy.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedCatchDate = UsedCatchDates == null ? new HashSet<string>() : UsedCatchDates.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedCreatedTime = UsedCreatedTime == null ? new HashSet<string>() : UsedCreatedTime.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedModifiedTime = UsedModifiedTime == null ? new HashSet<string>() : UsedModifiedTime.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();

                if (selectedLength.Count > 0)
                    lstSamples = lstSamples.Where(x => x.fishLengthMM != null && selectedLength.Contains(x.fishLengthMM.ToString() ?? ""));

                if (selectedWeight.Count > 0)
                    lstSamples = lstSamples.Where(x => x.fishWeightG != null && selectedWeight.Contains(x.fishWeightG.ToString() ?? ""));

                if (selectedSex.Count > 0)
                    lstSamples = lstSamples.Where(x => x.L_SexCode != null && selectedSex.Contains(x.L_SexCode.UIDisplay ?? ""));

                if (selectedMaturity.Count > 0)
                    lstSamples = lstSamples.Where(x => x.Maturity != null && selectedMaturity.Contains(x.Maturity.FullUIDisplay ?? ""));

                if (selectedOtolithReadingRemarks.Count > 0)
                    lstSamples = lstSamples.Where(x => x.L_OtolithReadingRemark != null && selectedOtolithReadingRemarks.Contains(x.L_OtolithReadingRemark.UIDisplay ?? ""));

                if (selectedOtolithDescriptions.Count > 0)
                    lstSamples = lstSamples.Where(x => x.L_SDOtolithDescription != null && selectedOtolithDescriptions.Contains(x.L_SDOtolithDescription.UIDisplay ?? ""));

                if (selectedEdgeStructures.Count > 0)
                    lstSamples = lstSamples.Where(x => x.L_EdgeStructure != null && selectedEdgeStructures.Contains(x.L_EdgeStructure.UIDisplay ?? ""));

                if (selectedDFUAreas.Count > 0)
                    lstSamples = lstSamples.Where(x => selectedDFUAreas.Contains(x.DFUArea ?? ""));

                if (selectedStatRect.Count > 0)
                    lstSamples = lstSamples.Where(x => selectedStatRect.Contains(x.statisticalRectangle ?? ""));
                
                if (selectedCruises.Count > 0)
                    lstSamples = lstSamples.Where(x => x.cruise != null && selectedCruises.Contains(x.cruise.ToString() ?? ""));

                if (selectedTrips.Count > 0)
                    lstSamples = lstSamples.Where(x => x.trip != null && selectedTrips.Contains(x.trip.ToString() ?? ""));

                if (selectedStations.Count > 0)
                    lstSamples = lstSamples.Where(x => x.station != null && selectedStations.Contains(x.station.ToString() ?? ""));

                if (selectedStock.Count >0)
                    lstSamples = lstSamples.Where(x => x.L_Stock != null &&  selectedStock.Contains(x.L_Stock.UIDisplay ?? ""));

                if (selectedPrepMethods.Count > 0)
                    lstSamples = lstSamples.Where(x => x.L_SDPreparationMethod != null && selectedPrepMethods.Contains(x.L_SDPreparationMethod.UIDisplay.ToString() ?? ""));

                if (selectedLigthTypes.Count > 0)
                    lstSamples = lstSamples.Where(x => x.L_SDLightType != null && selectedLigthTypes.Contains(x.L_SDLightType.UIDisplay ?? ""));

                if (selectedCreatedBy.Count > 0)
                    lstSamples = lstSamples.Where(x => selectedCreatedBy.Contains(x.createdByUserName ?? ""));

                if(selectedCatchDate.Count > 0)
                    lstSamples = lstSamples.Where(x => x.catchDate != null && selectedCatchDate.Contains(x.catchDate.Value.ToString("dd-MM-yy") ?? ""));

                if (selectedCreatedTime.Count > 0)
                    lstSamples = lstSamples.Where(x => x.createdTime != null && selectedCreatedTime.Contains(x.createdTime.Value.ToString("dd-MM-yy") ?? ""));

                if (selectedModifiedTime.Count > 0)
                    lstSamples = lstSamples.Where(x => x.modifiedTime != null && selectedModifiedTime.Contains(x.modifiedTime.Value.ToString("dd-MM-yy") ?? ""));

                //If in this mode, make sure that samples are hidden that did not come from fishline and that all samples has an age.
                if (IsCopyingAgesToFishLineActivated)
                {
                    try
                    {
                        lstSamples = lstSamples.Where(x => x.ImportStatusEnum == Entities.SDSampleImportStatus.ImportedFromFishline && x.SDFilesWithApprovedAnnotations.Any());

                        Dictionary<string, DFUPerson> primaryReaders = new Dictionary<string, DFUPerson>();

                        if (_sdEvent != null && _sdEvent.SDReaders != null)
                        {
                            primaryReaders = _sdEvent.SDReaders.Where(x => x.primaryReader && x.SDReader != null && x.SDReader.DFUPerson != null)
                                                               .Select(x => x.SDReader.DFUPerson)
                                                               .DistinctBy(x => x.initials.ToLower())
                                                               .ToDictionary(x => x.initials.ToLower());
                        }

                        foreach (var s in lstSamples)
                        {
                            s.TemporaryPrimaryReader = primaryReaders;

                            var files = s.SDFilesWithApprovedAnnotations;
                            if (files.Count() == 1)
                                s.SelectedSDFileForAgeTransfer = files.First();
                            else
                                s.SelectedSDFileForAgeTransfer = null;
                        }
                    }
                    catch(Exception xe)
                    {
                        LogError(xe);
                    }
                }
                //If in this mode, make sure to only show samples that have an SD file.
                else if (IsCopyingScaleToSamplesActivated)
                    lstSamples = lstSamples.Where(x => x.SDFile.Any());
                else if (IsDeleteMultipleVisible)
                    lstSamples = lstSamples.Where(x => !x.SDFilesWithApprovedAnnotations.Any());

                FilteredSDSamplesList = lstSamples.ToList(); //lstSamples.OrderByDescending(x => x.createdTime).ToList();
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }

            RaiseIsAllSelected();
        }


        private void ResetSelection()
        {
            try
            {
                if (SDSampleList != null)
                {
                    SDSampleList.ForEach(x =>
                    {
                        if (x.IsSelected)
                            x.IsSelected = false;
                    });
                }
               // RaisePropertyChanged(() => FilteredSDSamplesList);
                RaiseIsAllSelected();
            }
            catch { }
        }


        private void CheckForChanges()
        {
            var man = new BusinessLogic.SmartDots.SmartDotsManager();
            var selectionAnimals = new List<SelectionAnimal>();
            var vm = new SmartDots.ApplyChangesToSamplesViewModel(this); 
            
            try
            {
                var animalIdsLst = new List<int>();
                int idNum = 0;

                //Get all sample added from fishline
                foreach (var sampleItm in SDSampleList)
                {
                    if (sampleItm.ImportStatusEnum == Entities.SDSampleImportStatus.ImportedFromFishline &&  int.TryParse(sampleItm.animalId, out idNum))
                        animalIdsLst.Add(idNum);
                }

                if (animalIdsLst.Count == 0)
                    return;

                var arrAnimalId = animalIdsLst.ToArray();

                //Get selection animals from fishline database by IDs
                selectionAnimals = man.GetSelectionAnimals(arrAnimalId, false);
                if (selectionAnimals == null)
                    return;

                List<ModifiedSampleItem> lstModified = new List<ModifiedSampleItem>();

                //Compare sample items. Create Modified sample list
                foreach (var animalItm in selectionAnimals)
                {
                    foreach (var sampleWithSameId in SDSampleList.Where(x => x.animalId == animalItm.AnimalId.ToString()))
                    {
                        if (HasChangesComareSampleToAnimalItem(sampleWithSameId, animalItm))
                            lstModified.Add(new ModifiedSampleItem(vm, sampleWithSameId, animalItm));
                    }
                }

                //Show new view with datagrid comparing modified sample values
                if(lstModified.Count > 0)
                {
                    new Action(() =>
                    {
                        vm.ModifiedSampleList = lstModified.DistinctBy(x => x.SelectionAnimal.AnimalId).ToList();
                        vm.FilterModifiedSamples();
                        AppRegionManager.LoadWindowViewFromViewModel(vm, true, "StandardWindowWithBorderStyle");
                        //Apply changes for selected sample items
                        if (vm != null && vm.SelectedModifiedSampleList != null )
                        {
                            foreach (var item in vm.SelectedModifiedSampleList)
                            {
                               var oldSamples = SDSampleList.Where(x => x.animalId == item.Sample.animalId).ToList();
                               if(oldSamples != null && oldSamples.Count > 0)
                                {
                                    foreach (var oldSample in oldSamples)
                                    {
                                        var newSample = GetSdSampleFromSelectionAnimal(item.SelectionAnimal, null);
                                        if (newSample == null)
                                            continue;

                                        newSample.CopyEntityValueTypesTo(oldSample, "sdSampleGuid", "sdSampleId", "sdEventId", "createdTime", "CreatedTimeLocal", "createdById", "createdByUserName", "readOnly", "sdPreparationMethodId", "sdLightTypeId", "sdOtolithDescriptionId");
                                        oldSample.MarkAsModified();
                                    }
                                }
                            }

                            //Ignore samples validation here, since they have already been saved and are just modified according to found differences from the warehouse. So if preparation method is for example missing, that is just to bad.
                            SaveSamples(true);
                        }

                    }).Dispatch();
                }
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }

        }

        private bool HasChangesComareSampleToAnimalItem(SDSample sdSampleItem, SelectionAnimal selectionAnimalItem)
        {
            try
            {

                var newSdSmaple = GetSdSampleFromSelectionAnimal(selectionAnimalItem);
                if (sdSampleItem != null && newSdSmaple != null)
                    return sdSampleItem.HasValueTypeChanges(newSdSmaple, "sdPreparationMethodId", "sdLightTypeId", "sdOtolithDescriptionId"); 

            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }

            return false;
        }

        private void AssignSamplesFromAnimals(SDSelectAnimalsViewModel selectAnimalsVM)
        {
            try
            {
                var man = new BusinessLogic.SmartDots.SmartDotsManager();
                var sDSampleList = new List<SDSample>();

                var fileNames = selectAnimalsVM.SelectedAnimals.Where(x => x.SelectionAnimal.HasAnimalImages).SelectMany(x => x.SelectionAnimal.AnimalImageFileNames).Distinct().ToList();
                var fileinformation = man.GetFileInformationFromFileNames(fileNames, selectAnimalsVM.ImagePaths);

                //Load file information 
                //var fileinformation = man.GetFileInformationFromAnimalIds(selectAnimalsVM.SelectedAnimals.Select(x => x.SelectionAnimal.AnimalId.ToString()).ToList());

                //Load sample data
                new Action(() => { AddingSamplesFromFishlineLoadingText = Translate("SDSampleView", "LoadingSampleData"); }).Dispatch();
              
                //Add animals to sample list
                foreach (var animal in selectAnimalsVM.SelectedAnimals)
                {
                    var animalItem = animal.SelectionAnimal;
                    var lstFiles = new List<Entities.OtolithFileInformation>();
                    if (animalItem.AnimalImageFileNames != null)
                    {
                        foreach (var f in animalItem.AnimalImageFileNames)
                        {
                            var fLower = f.ToLowerInvariant();
                            if (fileinformation.ContainsKey(fLower))
                                lstFiles.AddRange(fileinformation[fLower]);
                        }
                    }
                    var sdSample = GetSdSampleFromSelectionAnimal(animalItem, lstFiles);

                    if (sdSample == null)
                        continue;

                    sDSampleList.Insert(0,sdSample);
                }

                if (sDSampleList == null || sDSampleList.Count == 0)
                    return;

                // Save samples to database
                new Action(() => { AddingSamplesFromFishlineLoadingText = Translate("SDSampleView", "SavingSamplesToDb"); }).Dispatch();
                var res = man.SaveSDSamples(sDSampleList);

                //If unsuccessful, show error message.
                if (res.DatabaseOperationStatus != Entities.DatabaseOperationStatus.Successful)
                {
                    LogAndDispatchUnexpectedErrorMessage(res.Message);
                    return;
                }
                else
                {
                    DispatchMessageBox(string.Format(Translate("SDSampleView", "SamplesSuccessfullyAddedMsg"), sDSampleList.Count));
                }

                //Update samples count on the event (so when going back to the events list, it is up to date).
                _sdEvent.SamplesCount = SDSampleList.Count + sDSampleList.Count;
            }
            catch (Exception e)
            {

                LogAndDispatchUnexpectedErrorMessage(e);
            }

            //Reload sample list
            Task.Factory.StartNew(LoadSamples).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }

        private SDSample GetSdSampleFromSelectionAnimal(SelectionAnimal selectionAnimal, List<Entities.OtolithFileInformation> otolithFiles = null)
        {
            var sdSample = new SDSample();

            try
            {
               
                sdSample.sdSampleGuid = Guid.NewGuid();
                sdSample.sdEventId = _sdEvent.sdEventId;

                if (selectionAnimal != null)
                {
                    if (IsAddingSamplesFromFishline)
                        sdSample.readOnly = true;
                    sdSample.animalId = selectionAnimal.AnimalId.ToString();
                    sdSample.cruise = selectionAnimal.Cruise;
                    sdSample.trip = selectionAnimal.Trip;
                    sdSample.station = selectionAnimal.Station;
                    sdSample.DFUArea = selectionAnimal.AreaCode;
                    sdSample.statisticalRectangle = selectionAnimal.StatisticalRectangle;
                    if (!string.IsNullOrWhiteSpace(selectionAnimal.Latitude))
                        sdSample.latitude = MapViewModel.ConvertPositionFromDegreesToDouble(selectionAnimal.Latitude ?? "00.00.000 N");

                    if (!string.IsNullOrWhiteSpace(selectionAnimal.Longitude))
                        sdSample.longitude = MapViewModel.ConvertPositionFromDegreesToDouble(selectionAnimal.Longitude ?? "00.00.000 E");

                    sdSample.sexCode = selectionAnimal.SexCode;
                    sdSample.edgeStructure = selectionAnimal.EdgeStructureCode;

                    int? otolithReadingRemarkId = null;

                    if (!string.IsNullOrWhiteSpace(selectionAnimal.OtolithReadingRemarkCode) && OtolithReadingRemarks != null)
                    {
                        var oto = OtolithReadingRemarks.Where(x => x.otolithReadingRemark.Equals(selectionAnimal.OtolithReadingRemarkCode)).FirstOrDefault();
                        if (oto != null)
                            otolithReadingRemarkId = oto.L_OtolithReadingRemarkID;
                    }

                    sdSample.otolithReadingRemarkId = otolithReadingRemarkId; //animalItem.otolithReadingRemarkId
                    sdSample.fishLengthMM = selectionAnimal.LengthMM;
                    sdSample.fishWeightG = selectionAnimal.WeightG; //Convert to grams
                    sdSample.maturityIndexMethod = selectionAnimal.MaturityIndexMethod;

                    int? maturityId = null;

                    if (!string.IsNullOrWhiteSpace(selectionAnimal.MaturityIndexMethod) && selectionAnimal.MaturityIndex.HasValue && MaturityList != null)
                    {
                        var mat = MaturityList.Where(x => x.maturityIndex == selectionAnimal.MaturityIndex.Value && x.maturityIndexMethod.Equals(selectionAnimal.MaturityIndexMethod)).FirstOrDefault();
                        if (mat != null)
                            maturityId = mat.maturityId;
                    }

                    sdSample.maturityId = maturityId; // animalItem.MaturityId;
                    sdSample.comments = ((selectionAnimal.SpeciesListRemark ?? "") + " " + (selectionAnimal.AnimalRemark ?? "")).Trim();
                    sdSample.createdById = DFUPersonLogin == null ? null : new Nullable<int>(DFUPersonLogin.dfuPersonId);
                    sdSample.createdByUserName = User.UserName;
                    sdSample.createdTime = DateTime.UtcNow;
                    sdSample.modifiedTime = DateTime.UtcNow;
                    sdSample.catchDate = selectionAnimal.GearStartDate;
                    sdSample.ImportStatusEnum = Entities.SDSampleImportStatus.ImportedFromFishline;
                    sdSample.speciesCode = selectionAnimal.SpeciesCode;
                    sdSample.stockId = selectionAnimal.StockId;

                    if (!string.IsNullOrEmpty(selectionAnimal.PreperationMethod))
                        sdSample.L_SDPreparationMethod = PreparationMethods.Where(x => x.preparationMethod == selectionAnimal.PreperationMethod).FirstOrDefault();

                    if (!string.IsNullOrEmpty(selectionAnimal.LightType))
                        sdSample.L_SDLightType = LightTypes.Where(x => x.lightType == selectionAnimal.LightType).FirstOrDefault();

                    if (!string.IsNullOrEmpty(selectionAnimal.OtolithDescription))
                        sdSample.L_SDOtolithDescription = OtolithDescriptions.Where(x => x.otolithDescription == selectionAnimal.OtolithDescription).FirstOrDefault();

                    //Add files to the sample

                    if (otolithFiles != null && otolithFiles.Count > 0)
                    {
                        var files = otolithFiles.Where(x => (x.AnimalId ?? "").Equals(selectionAnimal.AnimalId.ToString(), StringComparison.InvariantCultureIgnoreCase)).ToList();

                        if (files != null && files.Count > 0)
                        {
                            //If more than 1 file is present and the event type is yearly reading, only one file is allowed added.
                            if (_sdEvent.IsYearlyReadingEventType && files.Count > 1)
                            {
                                var relativeFilePaths = files.Select(x => Path.Combine(x.RelativeDirectoryPath, x.FileName)).ToList();
                                SelectOtolithImagesViewModel vmSelectFiles = null;
                                new Action(() =>
                                {
                                    vmSelectFiles = new SmartDots.SelectOtolithImagesViewModel();
                                    vmSelectFiles.IsSingleSelection = true;
                                    vmSelectFiles.ShowMessageWhenNoFilesAreSelected = false;
                                    vmSelectFiles.CancelButtonText = Translate("SDSampleView", "SelectSingleOtolithImageSkipButton");
                                    vmSelectFiles.Description = string.Format(Translate("SDSampleView", "SelectSingleOtolithImageMessage"), relativeFilePaths.Count, selectionAnimal.AnimalId.ToString());
                                    vmSelectFiles.InitializeAsync(relativeFilePaths);
                                    AppRegionManager.LoadWindowViewFromViewModel(vmSelectFiles, true, "WindowWithBorderStyle");
                                }).DispatchInvoke();

                                if (vmSelectFiles != null && !vmSelectFiles.IsCanceled && vmSelectFiles.SelectedOtolithItemList != null && vmSelectFiles.SelectedOtolithItemList.Count > 0)
                                {
                                    var file = vmSelectFiles.SelectedOtolithItemList.First();
                                    var fp = Path.Combine(file.ImageDirectory ?? "", file.ImageName ?? "");
                                    var otolithFileInfo = files.Where(x => Path.Combine((x.RelativeDirectoryPath ?? ""), x.FileName ?? "").Equals(fp, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                    files.Clear();
                                    if (otolithFileInfo != null)
                                        files.Add(otolithFileInfo);
                                }
                                else
                                {
                                    return null;
                                }
                            }

                            if (sdSample.SDFile == null)
                                sdSample.SDFile = new TrackableCollection<SDFile>();

                            foreach (var item in files)
                            {
                                sdSample.SDFile.Add(new SDFile()
                                {
                                    sdFileGuid = Guid.NewGuid(),
                                    fileName = item.FileName,
                                    displayName = Path.GetFileName(item.FileName),
                                    path = item.RelativeDirectoryPath,

                                });
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {

                LogAndDispatchUnexpectedErrorMessage(e);
                return null;
            }
           

            return sdSample;

        }



        private void ColumnVisibilityChanged(List<Babelfisk.BusinessLogic.Settings.DataGridColumnSettings> cs)
        {
            //Refresh trip type since it is bound to all column, forcing their vibility to be refreshed
            RaisePropertyChanged(() => ColumnVisivilityAny);
        }


        public override void FireClosing(object sender, CancelEventArgs e)
        {
            if (AppRegionManager.ShowMessageBox(Translate("SDSampleView", "HasChangesWarning"), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                e.Cancel = true;
        }

        private void SetAllCheckboxes()
        {
            if (_lstFilteredSDSamples != null && _lstFilteredSDSamples.Count > 0)
            {
                if (_lstFilteredSDSamples.Count == _lstFilteredSDSamples.Where(x => x.IsSelected).ToList().Count())
                    foreach (var item in _lstFilteredSDSamples)
                    {
                        item.SetIsSelected(false);
                    }
                else if (_lstFilteredSDSamples.Any(x => x.IsSelected))
                    foreach (var item in _lstFilteredSDSamples)
                    {
                        item.SetIsSelected(false);
                    }
                else
                    foreach (var item in _lstFilteredSDSamples)
                    {
                        item.SetIsSelected(true);
                    }
            }

            RaisePropertyChanged(() => FilteredSDSamplesList);
            RaiseIsAllSelected();
        }

        public void RaiseIsAllSelected()
        {
            RaisePropertyChanged(() => IsAllSelected);
            RaisePropertyChanged(() => HasSelectedSamples);
            RaisePropertyChanged(() => SelectedSamplesCount);
        }

        public static bool IsFileinUse(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }


        private bool CheckSampleEventInconsistencies()
        {
            if(SDSampleList == null || SDSampleList.Count == 0)
                return false;

            var sc = (Event == null || Event.speciesCode == null) ? "" : Event.speciesCode;
            if (SDSampleList.Any(x => (!string.IsNullOrEmpty(x.DFUArea) && !DFUAreasHashSet.Contains(x.DFUArea)) || (!string.IsNullOrWhiteSpace(x.speciesCode) && !x.speciesCode.Equals(sc, StringComparison.InvariantCultureIgnoreCase))))
                return true;

            //Check if all species match

            return false;
        }


        #region Return To Events Command


        public DelegateCommand ReturnToEventsCommand
        {
            get
            {
                if (_cmdReturnToEvents == null)
                    _cmdReturnToEvents = new DelegateCommand(() => ReturnToEvents());

                return _cmdReturnToEvents;
            }
        }


        /// <summary>
        /// Return to the list of events.
        /// </summary>
        private void ReturnToEvents()
        {
            try
            {
                //Show warning if there are unsaved changes.
                if (SDSampleList.Any(x => x.ChangeTracker.State == ObjectState.Modified || x.ChangeTracker.State == ObjectState.Added) || (_lstDeletedSDSamples != null && _lstDeletedSDSamples.Count > 0) || IsDirty)
                {
                    if (AppRegionManager.ShowMessageBox(Translate("SDSampleView", "HasChangesWarning"), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                        return;
                }

                IsDirty = false;
                _vmEvents.CloseActiveViewModel();
            }
            catch(Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }


        #endregion


        #region Edit Event Command
        public DelegateCommand EditEventCommand
        {
            get
            {
                if (_cmdEditEvent == null)
                    _cmdEditEvent = new DelegateCommand(() => EditEvent());

                return _cmdEditEvent;
            }
        }


        /// <summary>
        /// Open the Add/Edit event view
        /// </summary>
        private void EditEvent()
        {
            if (!User.HasAddEditSDEventsAndSamplesTask)
            {
                DispatchAccessDeniedMessageBox();
                return;
            }

            try
            {
                var vm = new SmartDots.AddEditSDEventViewModel(this, _vmEvents, _sdEvent);
                vm.InitializeAsync();
                _vmEvents.ActivateViewModel(vm);
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }


        #endregion


        #region Show Select Animals View Command

        public DelegateCommand ShowSelectAnimalsViewCommand
        {
            get { return _cmdShowSelectAnimalsViewCommand ?? (_cmdShowSelectAnimalsViewCommand = new DelegateCommand(() => ShowSelectAnimalsView())); }
        }

        private void ShowSelectAnimalsView()
        {
            if (!User.HasAddEditSDEventsAndSamplesTask)
            {
                DispatchAccessDeniedMessageBox();
                return;
            }

            try
            {
                //Show warning if there are unsaved changes, since importing new rows, will reload everything, resseting any changes..
                if (SDSampleList.Any(x => x.ChangeTracker.State == ObjectState.Modified || x.ChangeTracker.State == ObjectState.Added) || (_lstDeletedSDSamples != null && _lstDeletedSDSamples.Count > 0))
                {
                    if (AppRegionManager.ShowMessageBox(Translate("SDSampleView", "HasChangesWarning"), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                        return;
                }

                var vm = new SmartDots.SDSelectAnimalsViewModel(this, Event);
                vm.InitializeAsync();
                AppRegionManager.LoadWindowViewFromViewModel(vm, true, "StandardWindowWithBorderStyle");

                if (vm.SelectedAnimals != null && vm.SelectedAnimals.Count > 0)
                {
                    IsDirty = true;
                    IsAddingSamplesFromFishline = true;
                    AddingSamplesFromFishlineLoadingText = Translate("SDSampleView", "LocatingImages");// "locating images for selected animals"
                    Task.Factory.StartNew(() => AssignSamplesFromAnimals(vm)).ContinueWith(t => new Action(() => { IsAddingSamplesFromFishline = false; }).Dispatch());
                }
            }
            catch (Exception e)
            {

                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }

        #endregion


        #region Show Import From CSV View Command

        public DelegateCommand ShowImportFromCSVViewCommand
        {
            get
            {
                return _cmdShowImportFromCSVViewCommand ?? (_cmdShowImportFromCSVViewCommand = new DelegateCommand(() => ShowImportFromCSVView())); ;
            }
        }

        private void ShowImportFromCSVView()
        {
            if (!User.HasAddEditSDEventsAndSamplesTask)
            {
                DispatchAccessDeniedMessageBox();
                return;
            }

            try
            {
                //Show warning if there are unsaved changes, since importing new rows, will reload everything, resseting any changes..
                if (SDSampleList.Any(x => x.ChangeTracker.State == ObjectState.Modified || x.ChangeTracker.State == ObjectState.Added) || (_lstDeletedSDSamples != null && _lstDeletedSDSamples.Count > 0))
                {
                    if (AppRegionManager.ShowMessageBox(Translate("SDSampleView", "HasChangesWarning"), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                        return;
                }

             /*   string fileName = "";
                try
                {
                    
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Title = "Import CSV File";
                    dialog.Filter = "CSV Files (*.csv)|*.csv";
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        fileName = dialog.FileName;
                    }

                    if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                        return;

                    if(IsFileinUse(new FileInfo(fileName)))
                    {
                        var resVal = AppRegionManager.ShowMessageBox(Translate("SDSampleView", "WarningFileIsOpen"), System.Windows.MessageBoxButton.OK);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    LogAndDispatchUnexpectedErrorMessage(ex);
                }*/


                string[] imageFolders = null;
                if (Event != null)
                    imageFolders = Event.DefaultImageFoldersArray;


                SelectImportFileAndImageFoldersViewModel vmSIF = new SelectImportFileAndImageFoldersViewModel(imageFolders);
                vmSIF.WindowWidth = 600;
                vmSIF.WindowHeight = 350;
                vmSIF.WindowTitle = "Import CSV file";

                if (!vmSIF.Initialize())
                    return;

                AppRegionManager.LoadWindowViewFromViewModel(vmSIF, true, "WindowWithBorderStyle");

                if (vmSIF.IsCancelled || string.IsNullOrWhiteSpace(vmSIF.FilePath) || !File.Exists(vmSIF.FilePath))
                    return;

                imageFolders = vmSIF.ImageFolderPaths;

                var vm = new SmartDots.ImportFromCSVViewModel(this, vmSIF.FilePath, imageFolders);
                vm.InitializeAsync();
                AppRegionManager.LoadWindowViewFromViewModel(vm, true, "WindowWithBorderStyle");

                if (vm.SelectedSDSampleItems != null && vm.SelectedSDSampleItems.Count > 0)
                {
                    IsDirty = true;
                    IsAddingSamplesFromFishline = true;
                    AddingSamplesFromFishlineLoadingText = Translate("SDSampleView", "LocatingImages");// "locating images for selected animals"
                    Task.Factory.StartNew(() => AssignSamplesFromCSV(vm)).ContinueWith(t => new Action(() => { IsAddingSamplesFromFishline = false; }).Dispatch());
                }

            }
            catch (Exception e)
            {

                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }


        private void AssignSamplesFromCSV(ImportFromCSVViewModel ifcVM)
        {
            try
            {
                var man = new BusinessLogic.SmartDots.SmartDotsManager();

                int chunkSize = 10;

                int insertedSamples = 0;

                //Add animals to sample list
                foreach (var chunk in ifcVM.SelectedSDSampleItems.InChunks(chunkSize))
                {
                    var sDSampleList = new List<SDSample>();
                    new Action(() => { AddingSamplesFromFishlineLoadingText = string.Format(Translate("SDSampleView", "CSVImportProgressMessage"), insertedSamples + chunk.Count(), ifcVM.SelectedSDSampleItems.Count); }).Dispatch();

                    foreach (var sampleItem in chunk)
                    {
                        var sample = sampleItem.Sample;
                        var sdSample = sample.Clone();
                        sdSample.sdSampleGuid = Guid.NewGuid();
                        sdSample.sdEventId = _sdEvent.sdEventId;

                        //If more than 1 file is present and the event type is yearly reading, only one file is allowed added.
                        if (_sdEvent.IsYearlyReadingEventType && sdSample.SDFile.Count > 1)
                        {
                            var relativeFilePaths = sdSample.SDFile.Select(x => Path.Combine(x.path, x.fileName)).ToList();
                            SelectOtolithImagesViewModel vmSelectFiles = null;
                            new Action(() =>
                            {
                                vmSelectFiles = new SmartDots.SelectOtolithImagesViewModel();
                                vmSelectFiles.IsSingleSelection = true;
                                vmSelectFiles.ShowMessageWhenNoFilesAreSelected = false;
                                vmSelectFiles.CancelButtonText = Translate("SDSampleView", "SelectSingleOtolithImageSkipButton");
                                vmSelectFiles.Description = string.Format(Translate("SDSampleView", "SelectSingleOtolithImageMessage"), relativeFilePaths.Count, sdSample.animalId ?? "");
                                vmSelectFiles.InitializeAsync(relativeFilePaths);
                                AppRegionManager.LoadWindowViewFromViewModel(vmSelectFiles, true, "WindowWithBorderStyle");
                            }).DispatchInvoke();

                            if (vmSelectFiles != null && !vmSelectFiles.IsCanceled && vmSelectFiles.SelectedOtolithItemList != null && vmSelectFiles.SelectedOtolithItemList.Count > 0)
                            {
                                var file = vmSelectFiles.SelectedOtolithItemList.First();
                                var fp = Path.Combine(file.ImageDirectory ?? "", file.ImageName ?? "");
                                var otolithFileInfo = sdSample.SDFile.Where(x => Path.Combine((x.path ?? ""), x.fileName ?? "").Equals(fp, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                if (otolithFileInfo != null)
                                {
                                    sdSample.SDFile.Clear();
                                    sdSample.SDFile.Add(otolithFileInfo);
                                }
                            }
                            else
                            {
                                //Skip adding sample if "Skip" is clicked.
                                continue;
                            }
                        }

                        sDSampleList.Add(sdSample);

                    }

                    if (sDSampleList == null || sDSampleList.Count == 0)
                        continue;

                    // Save samples to database
                    var res = man.SaveSDSamples(sDSampleList);

                    if (res.DatabaseOperationStatus != Entities.DatabaseOperationStatus.Successful)
                    {
                        LogAndDispatchUnexpectedErrorMessage(res.Message);
                        break;
                    }

                    insertedSamples += sDSampleList.Count;
                    GC.Collect();
                }


                //If unsuccessful, show error message.
                if (insertedSamples > 0)
                {
                    DispatchMessageBox(string.Format(Translate("SDSampleView", "CSVImportSuccessMessage"), insertedSamples, ifcVM.SelectedSDSampleItems.Count));
                }

                //Update samples count on the event (so when going back to the events list, it is up to date).
                _sdEvent.SamplesCount = SDSampleList.Count + insertedSamples;
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }

            IsLoading = true;
            //Reload sample list
            Task.Factory.StartNew(LoadSamples).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        /*
        private void AssignSamplesFromCSV(ImportFromCSVViewModel ifcVM)
        {
            try
            {
                var man = new BusinessLogic.SmartDots.SmartDotsManager();
                var sDSampleList = new List<SDSample>();

                //Load sample data
                new Action(() => { AddingSamplesFromFishlineLoadingText = Translate("SDSampleView", "LoadingSampleData"); }).Dispatch();


                //Add animals to sample list
                foreach (var sampleItem in ifcVM.SelectedSDSampleItems)
                {
                    var sample = sampleItem.Sample;
                    var sdSample = sample.Clone();
                    sdSample.sdSampleGuid = Guid.NewGuid();
                    sdSample.sdEventId = _sdEvent.sdEventId;

                    //If more than 1 file is present and the event type is yearly reading, only one file is allowed added.
                    if (_sdEvent.IsYearlyReadingEventType && sdSample.SDFile.Count > 1)
                    {
                        var relativeFilePaths = sdSample.SDFile.Select(x => Path.Combine(x.path, x.fileName)).ToList();
                        SelectOtolithImagesViewModel vmSelectFiles = null;
                        new Action(() =>
                        {
                            vmSelectFiles = new SmartDots.SelectOtolithImagesViewModel();
                            vmSelectFiles.IsSingleSelection = true;
                            vmSelectFiles.ShowMessageWhenNoFilesAreSelected = false;
                            vmSelectFiles.CancelButtonText = Translate("SDSampleView", "SelectSingleOtolithImageSkipButton");
                            vmSelectFiles.Description = string.Format(Translate("SDSampleView", "SelectSingleOtolithImageMessage"), relativeFilePaths.Count, sdSample.animalId ?? "");
                            vmSelectFiles.InitializeAsync(relativeFilePaths);
                            AppRegionManager.LoadWindowViewFromViewModel(vmSelectFiles, true, "WindowWithBorderStyle");
                        }).DispatchInvoke();

                        if (vmSelectFiles != null && !vmSelectFiles.IsCanceled && vmSelectFiles.SelectedOtolithItemList != null && vmSelectFiles.SelectedOtolithItemList.Count > 0)
                        {
                            var file = vmSelectFiles.SelectedOtolithItemList.First();
                            var fp = Path.Combine(file.ImageDirectory ?? "", file.ImageName ?? "");
                            var otolithFileInfo = sdSample.SDFile.Where(x => Path.Combine((x.path ?? ""), x.fileName ?? "").Equals(fp, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                            if (otolithFileInfo != null)
                            {
                                sdSample.SDFile.Clear();
                                sdSample.SDFile.Add(otolithFileInfo);
                            }
                        }
                        else
                        {
                            //Skip adding sample if "Skip" is clicked.
                            continue;
                        }

                    }

                    sDSampleList.Add(sdSample);
                }

                if (sDSampleList == null || sDSampleList.Count == 0)
                    return;

                // Save samples to database
                new Action(() => { AddingSamplesFromFishlineLoadingText = Translate("SDSampleView", "SavingSamplesToDb"); }).Dispatch();
                var res = man.SaveSDSamples(sDSampleList);

                //If unsuccessful, show error message.
                if (res.DatabaseOperationStatus != Entities.DatabaseOperationStatus.Successful)
                {
                    LogAndDispatchUnexpectedErrorMessage(res.Message);
                    return;
                }
                else
                {
                    DispatchMessageBox(string.Format("{0} sample(s) was successfully added to the event.", sDSampleList.Count));
                }

                //Update samples count on the event (so when going back to the events list, it is up to date).
                _sdEvent.SamplesCount = SDSampleList.Count + sDSampleList.Count;
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }

            IsLoading = true;
            //Reload sample list
            Task.Factory.StartNew(LoadSamples).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());


        }*/

        #endregion


        #region Copy Move Samples Commands


        public DelegateCommand CopySamplesCommand
        {
            get
            {
                if (_cmdCopySamples == null)
                    _cmdCopySamples = new DelegateCommand(() => MoveCopySamples(false));
                return _cmdCopySamples;
            }
        }

       
        public DelegateCommand MoveSamplesCommand
        {
            get
            {
                if (_cmdMoveSamples == null)
                    _cmdMoveSamples = new DelegateCommand(() => MoveCopySamples(true));
                return _cmdMoveSamples;
            }
        }


        private void MoveCopySamples(bool isMove = false)
        {
            var selectedEvent = SelectedEvent;

            if (selectedEvent == null)
            {
                var resVal = AppRegionManager.ShowMessageBox(Translate("SDSampleView", "NoEventSelected"), System.Windows.MessageBoxButton.OK);
                return;
            }

            var lst = new List<SDSample>();
            var toEventSamples = new List<SDSample>();

            try
            {
                var man = new BusinessLogic.SmartDots.SmartDotsManager();

                if (FilteredSDSamplesList == null || FilteredSDSamplesList.Count == 0)
                {
                    DispatchMessageBox(Translate("SDSampleView", "ValidationSelectSamples"));
                    return;
                }

                lst = FilteredSDSamplesList.Where(x => x.IsSelected).ToList();

                if (lst.Count == 0)
                {
                    DispatchMessageBox(Translate("SDSampleView", "ValidationSelectSamples"));
                    return;
                }
                
                toEventSamples = man.GetSDSamplesWithIncludes(selectedEvent.sdEventId) ?? new List<SDSample>();

                List<string> eventAreas = (selectedEvent == null || selectedEvent.L_DFUAreas.Count == 0) ? new List<string>() : selectedEvent.L_DFUAreas.Select(x => x.DFUArea).ToList();

                //Perform validation first.
                foreach (var smpl in lst)
                {
                    if (smpl.speciesCode == null || !smpl.speciesCode.Equals(selectedEvent.speciesCode, StringComparison.InvariantCultureIgnoreCase))
                    {
                        DispatchMessageBox(string.Format(Translate("SDSampleView", "ValidationMoveCopySampleSpeciesMismatch"), smpl.animalId, smpl.speciesCode, selectedEvent.speciesCode));
                        return;
                    }

                    if (eventAreas.Count > 0 && !eventAreas.Contains(smpl.DFUArea, new Anchor.Core.Comparers.StringIgnoreCaseIEqualityComparer()))
                    {
                        DispatchMessageBox(string.Format(Translate("SDSampleView", "ValidationMoveCopySampleAreaMismatch"), smpl.animalId, smpl.DFUArea, selectedEvent.DFUAreaString));
                        return;
                    }

                    if (toEventSamples.Where(x => x.animalId != null && x.animalId.Equals(smpl.animalId ?? "")).Any())
                    {
                        DispatchMessageBox(string.Format(Translate("SDSampleView", "ValidationMoveCopySampleAnimalIdDuplicate"), smpl.animalId));
                        return;
                    }
                }

                if (AppRegionManager.ShowMessageBox(string.Format(Translate("SDSampleView", "MoveCopySamplesConfirmation"), lst.Count, selectedEvent.EventIdAndNameString), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                    return;

                //Move or copy
                var lstIds = lst.Select(x => x.sdSampleId).ToList();

                IsLoading = true;

                Task.Factory.StartNew(() =>
                {
                    DatabaseOperationResult res = DatabaseOperationResult.CreateSuccessResult();

                    res = man.MoveOrCopySamplesToEvent(lstIds, selectedEvent.sdEventId, !isMove);

                    //If unsuccessful, show error message.
                    if (res.DatabaseOperationStatus != Entities.DatabaseOperationStatus.Successful)
                    {
                        LogAndDispatchUnexpectedErrorMessage(res.Message);
                        return;
                    }

                    LoadSamples();

                    new Action(() =>
                    {
                        //Update sample count on the event where samples were moved/copied to.
                        if (_vmEvents != null && _vmEvents.AllEvents.Count > 0)
                        {
                            var evnt = _vmEvents.AllEvents.Where(x => x.sdEventId == _selectedEvent.sdEventId).FirstOrDefault();
                            if (evnt != null)
                                evnt.SamplesCount = evnt.SamplesCount += lst.Count();
                        }

                        if (isMove)
                        {
                            _sdEvent.SamplesCount -= lst.Count();
                            var resVal = AppRegionManager.ShowMessageBox(Translate("SDSampleView", "MoveSucceeded"), System.Windows.MessageBoxButton.OK);
                        }
                        else
                        {
                            var resVal = AppRegionManager.ShowMessageBox(Translate("SDSampleView", "CopySucceeded"), System.Windows.MessageBoxButton.OK);
                        }

                        IsMoveCopyVisible = false;
                        SelectedEvent = null;
                    }).Dispatch();

                }).ContinueWith(t => new Action(() => { IsLoading = false; }).Dispatch());              
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }


        #endregion


        #region Show Move Copy Command
        public DelegateCommand ShowMoveCopyCommand
        {
            get
            {
                if (_cmdShowMoveCopy == null)
                    _cmdShowMoveCopy = new DelegateCommand(() => ShowMoveCopy());
                return _cmdShowMoveCopy;
            }
        }

        private void ShowMoveCopy()
        {
            if (!User.HasAddEditSDEventsAndSamplesTask)
            {
                DispatchAccessDeniedMessageBox();
                return;
            }

            if ((_lstSDSamples != null && _lstSDSamples.Count > 0 && !_lstSDSamples.All(x => x.ChangeTracker.State == ObjectState.Unchanged) || (_lstDeletedSDSamples != null && _lstDeletedSDSamples.Count > 0)))
            {
                var resVal = AppRegionManager.ShowMessageBox(Translate("SDSampleView", "SamplesHaveChanges"), System.Windows.MessageBoxButton.OK);
                return;
            }

            ResetSelection();

            IsMoveCopyVisible = true;
            IsMenuDropDownOpen = false;
        }

        #endregion


        #region Show Remove Multiple Samples Command
        public DelegateCommand ShowDeleteMultipleSamplesCommand
        {
            get
            {
                if (_cmdShowRemoveMultipleSamples == null)
                    _cmdShowRemoveMultipleSamples = new DelegateCommand(() => ShowRemoveMultiple());
                
                return _cmdShowRemoveMultipleSamples;
            }
        }

        private void ShowRemoveMultiple()
        {
            if (!User.HasAddEditSDEventsAndSamplesTask)
            {
                DispatchAccessDeniedMessageBox();
                return;
            }

            if ((_lstSDSamples != null && _lstSDSamples.Count > 0 && !_lstSDSamples.All(x => x.ChangeTracker.State == ObjectState.Unchanged) || (_lstDeletedSDSamples != null && _lstDeletedSDSamples.Count > 0)))
            {
                var resVal = AppRegionManager.ShowMessageBox(Translate("SDSampleView", "SamplesHaveChanges"), System.Windows.MessageBoxButton.OK);
                return;
            }

            IsDeleteMultipleVisible = true;
            IsMenuDropDownOpen = false;
            ResetSelection();
            FilterSDSamples();

        }

        #endregion


        #region Look For Missing Images Command
        public DelegateCommand LookForMissingImagesCommand
        {
            get
            {
                if (_cmdLookForMissingImages == null)
                    _cmdLookForMissingImages = new DelegateCommand(() => LookForMissingImagesAsync());

                return _cmdLookForMissingImages;
            }
        }

        public void LookForMissingImagesAsync()
        {
            IsLoading = true;
            Task.Factory.StartNew(LookForMissingImages).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        private void SetPrepMethodLightTypeAndOtolithDescFromFileName(SDSample s, SDFile sf, Dictionary<string, L_SDPreparationMethod> sdPrepMethods, Dictionary<string, L_SDLightType> sdLightTypes, Dictionary<string, L_SDOtolithDescription> sdOtolithDescriptions)
        {
            var ip = Path.GetFileNameWithoutExtension(sf.fileName);
            var arr = ip.Split('_');
            string key = "";

            if (s.L_SDPreparationMethod == null)
            {
                var prepMethod = (arr.Length > 1 && sdPrepMethods.ContainsKey(arr[1].ToUpperInvariant())) ? sdPrepMethods[arr[1].ToUpperInvariant()] : null;
                if (prepMethod != null)
                    s.L_SDPreparationMethod = prepMethod;
            }

            if(s.L_SDLightType == null)
            {
                var lightType = (arr.Length > 2 && sdLightTypes.ContainsKey(arr[2].ToUpperInvariant())) ? sdLightTypes[arr[2].ToUpperInvariant()] : null;
                if (lightType != null)
                    s.L_SDLightType = lightType;
            }

            if (s.L_SDOtolithDescription == null)
            {
                var otoDesc = (arr.Length > 3 && sdOtolithDescriptions.ContainsKey(arr[3].ToUpperInvariant())) ? sdOtolithDescriptions[arr[3].ToUpperInvariant()] : null;
                if (otoDesc != null)
                    s.L_SDOtolithDescription = otoDesc;
            }
        }

        private void LookForMissingImages()
        {
            try
            {
                var samplesToCheck = (SDSampleList ?? new List<SDSample>()).Where(x => !x.HasSDFiles).ToList();

                if (SDSampleList == null || samplesToCheck.Count == 0)
                {
                    DispatchMessageBox(Translate("SDSampleView", "NoSamplesWithMissingFileFound"), 5);
                    return;
                }

                var man = new BusinessLogic.SmartDots.SmartDotsManager();

                string[] imageFolders = null;

                if (Event != null)
                    imageFolders = Event.DefaultImageFoldersArray;

                //Get file information for animal ids
                var fileinformation = man.GetFileInformationFromAnimalIds(samplesToCheck.Select(x => x.animalId), imageFolders);
                int sampleCounter = samplesToCheck.Count;
                int updatedSampleCounter = 0;
                if (fileinformation != null && fileinformation.Count > 0)
                {
                    var ldv = new LookupDataVersioning();
                    LookupManager lMan = new LookupManager();
                    var sdPrepMethods = lMan.GetLookups(typeof(L_SDPreparationMethod), ldv).OfType<L_SDPreparationMethod>().DistinctBy(x => x.preparationMethod).ToDictionary(x => x.preparationMethod.ToUpperInvariant());
                    var sdLightTypes = lMan.GetLookups(typeof(L_SDLightType), ldv).OfType<L_SDLightType>().DistinctBy(x => x.lightType).ToDictionary(x => x.lightType.ToUpperInvariant());
                    var sdOtolithDesc = lMan.GetLookups(typeof(L_SDOtolithDescription), ldv).OfType<L_SDOtolithDescription>().DistinctBy(x => x.otolithDescription).ToDictionary(x => x.otolithDescription.ToUpperInvariant());

                    foreach (var sample in samplesToCheck)
                    {
                        var files = fileinformation.Where(x => x.AnimalId == sample.animalId).ToList();

                        if (files != null && files.Count > 0)
                        {
                            //IF yearly reading event, only one sdfile is allowed. Therefore show box where the user has to choose one.
                            if (_sdEvent.IsYearlyReadingEventType && files.Count > 1)
                            {
                                var relativeFilePaths = files.Select(x => Path.Combine(x.RelativeDirectoryPath, x.FileName)).ToList();
                                SelectOtolithImagesViewModel vmSelectFiles = null;

                                //file selection dialog
                                new Action(() =>
                                {
                                    vmSelectFiles = new SmartDots.SelectOtolithImagesViewModel();
                                    vmSelectFiles.IsSingleSelection = true;
                                    vmSelectFiles.ShowMessageWhenNoFilesAreSelected = false;
                                    vmSelectFiles.CancelButtonText = Translate("SDSampleView", "SelectSingleOtolithImageSkipButton");
                                    vmSelectFiles.Description = string.Format(Translate("SDSampleView", "SelectSingleOtolithImageMessage2"), relativeFilePaths.Count, sample.animalId ?? "");
                                    vmSelectFiles.InitializeAsync(relativeFilePaths);
                                    AppRegionManager.LoadWindowViewFromViewModel(vmSelectFiles, true, "WindowWithBorderStyle");
                                }).DispatchInvoke();

                                //Assign selected image file to SDSample
                                if (vmSelectFiles != null && !vmSelectFiles.IsCanceled && vmSelectFiles.SelectedOtolithItemList != null && vmSelectFiles.SelectedOtolithItemList.Count > 0)
                                {
                                    var selected = vmSelectFiles.SelectedOtolithItemList.First();
                                    var selectedFile = files.Where(x => x.FileName == selected.ImageName && x.RelativeDirectoryPath == selected.ImageDirectory).FirstOrDefault();
                                    if (selectedFile != null)
                                    {
                                        var f = new SDFile()
                                        {
                                            sdFileGuid = Guid.NewGuid(),
                                            fileName = selectedFile.FileName,
                                            displayName = Path.GetFileName(selectedFile.FileName),
                                            path = selectedFile.RelativeDirectoryPath
                                        };
                                        sample.SDFile.Add(f);
                                        SetPrepMethodLightTypeAndOtolithDescFromFileName(sample, f, sdPrepMethods, sdLightTypes, sdOtolithDesc);
                                        IsDirty = true;
                                        sample.ChangeTracker.State = ObjectState.Modified;
                                        updatedSampleCounter++;
                                    }
                                }
                                else
                                {
                                    //Skip adding images if "Skip" is clicked.
                                    continue;
                                }

                            }
                            else
                            {
                                //Assign image files to SDSample
                                foreach (var file in files)
                                {
                                    var f = new SDFile()
                                    {
                                        sdFileGuid = Guid.NewGuid(),
                                        fileName = file.FileName,
                                        displayName = Path.GetFileName(file.FileName),
                                        path = file.RelativeDirectoryPath
                                    };
                                    sample.SDFile.Add(f);

                                    SetPrepMethodLightTypeAndOtolithDescFromFileName(sample, f, sdPrepMethods, sdLightTypes, sdOtolithDesc);
                                    IsDirty = true;
                                }

                                updatedSampleCounter++;
                                sample.ChangeTracker.State = ObjectState.Modified;
                            }
                        }
                    }
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(string.Format(Translate("SDSampleView", "SamplesNoImagesFound"), sampleCounter));
                    sb.AppendLine("");
                    if (imageFolders == null || imageFolders.Length == 0)
                        sb.AppendLine(string.Format("1) {0}", Translate("Common", "AllFolders")));
                    else
                    {
                        for(int i = 0; i < imageFolders.Length; i++)
                            sb.AppendLine(string.Format("{0}) '\\{1}'", i+1, imageFolders[i]));
                    }
                    DispatchMessageBox(sb.ToString());
                    return;
                }

                //Show updated message.
                DispatchMessageBox(string.Format(Translate("SDSampleView", "SamplesImagesUpdated"), updatedSampleCounter, sampleCounter));
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }

        #endregion


        #region Close Delete Multiple Samples Command

        public DelegateCommand CloseDeleteMultipleSamplesCommand
        {
            get
            {
                if (_cmdCloseDeleteMultipleSamples == null)
                    _cmdCloseDeleteMultipleSamples = new DelegateCommand(() => CloseExtraFeature());
                return _cmdCloseDeleteMultipleSamples;
            }
        }

        #endregion


        #region Close Move Copy Command

        public DelegateCommand CloseMoveCopyCommand
        {
            get
            {
                if (_cmdCloseMoveCopy == null)
                    _cmdCloseMoveCopy = new DelegateCommand(() => CloseExtraFeature());
                return _cmdCloseMoveCopy;
            }
        }

        private void CloseExtraFeature()
        {
            IsMoveCopyVisible = false;
            IsCopyingAgesToFishLineActivated = false;
            IsCopyingScaleToSamplesActivated = false;
            IsDeleteMultipleVisible = false;
            SelectedEvent = null;
            ResetSelection();
            FilterSDSamples();
        }


        #endregion


        #region Delete Multiple Samples Command
        public DelegateCommand DeleteMultipleSamplesCommand
        {
            get
            {
                if (_cmdDeleteMultipleSamples == null)
                    _cmdDeleteMultipleSamples = new DelegateCommand(() => DeleteMultipleSamples());

                    return _cmdDeleteMultipleSamples;
            }
        }

        public void DeleteMultipleSamples()
        {
            var lst = new List<SDSample>();

            try
            {

                if (FilteredSDSamplesList != null && FilteredSDSamplesList.Count > 0)
                {
                    if (!FilteredSDSamplesList.Any(x => x.IsSelected))
                    {
                        DispatchMessageBox(Translate("SDSampleView", "ValidationSelectSamples"));
                        return;
                    }

                    lst = FilteredSDSamplesList.Where(x => x.IsSelected).ToList();
                }

                if(lst != null && lst.Count > 0)
                {
                    foreach(var s in lst)
                    {
                        var f = s.SDFile.Where(x => x.SDAnnotation.Any()).FirstOrDefault();
                        //If the sample has any annotations attached to it, cancel the deletion.
                        if(f != null)
                        {
                            DispatchMessageBox(string.Format(Translate("SDSampleView", "DeletionErrorHasAnnotations2"), f.fileName ?? "N/A", s.animalId ?? "N/A"));
                            return;
                        }    
                    }

                    if (AppRegionManager.ShowMessageBox(string.Format(Translate("SDSampleView", "DeleteMultipleConfirmation"), lst.Count), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                        return;

                    foreach (var item in lst)
                    {
                        var state = item.ChangeTracker.State;

                        if (state != ObjectState.Added)
                        {
                            DeletedSDSamplesList.Add(item);
                            _lstSDSamples.Remove(item);
                            IsDirty = true;
                            item.MarkAsDeleted();
                        }
                        else
                        {
                            //if it is added/new, simply remove it from the samples list in memory only (since it does not exist in the DB yet).
                            _lstSDSamples.Remove(item);
                        }
                    }
                    FilterSDSamples();

                    SaveSamples();

                    CloseExtraFeature();
                }
                    

               
            }
            catch (Exception e)
            {

                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }
        #endregion


        #region Add Row Command

        public DelegateCommand AddRowCommand
        {
            get
            {
                if (_cmdAddRow == null)
                    _cmdAddRow = new DelegateCommand(() => AddRow());
                return _cmdAddRow;
            }
        }

        private void AddRow()
        {
            if (!User.HasAddEditSDEventsAndSamplesTask)
            {
                DispatchAccessDeniedMessageBox();
                return;
            }

            IsMenuDropDownOpen = false;

            try
            {
                var sampleItem = new SDSample();
                sampleItem.sdEventId = _sdEvent.sdEventId;
                sampleItem.sdSampleGuid = Guid.NewGuid();
                sampleItem.createdById = DFUPersonLogin == null ? null : new Nullable<int>(DFUPersonLogin.dfuPersonId);
                sampleItem.createdByUserName = User.UserName;
                sampleItem.createdTime = DateTime.UtcNow;
                sampleItem.modifiedTime = DateTime.UtcNow;

                EditSample(sampleItem, true);
            }
            catch (Exception e)
            {

                LogAndDispatchUnexpectedErrorMessage(e);
            }
            
        }

        #endregion


        #region Edit Sample Command
        public DelegateCommand<SDSample> EditSDSampleItemCommand
        {
            get
            {
                if (_cmdEditSample == null)
                    _cmdEditSample = new DelegateCommand<SDSample>(param => EditSample(param));

                return _cmdEditSample;
            }
        }

        private void EditSample(SDSample item, bool isNew = false)
        {
            if (!User.HasAddEditSDEventsAndSamplesTask)
            {
                DispatchAccessDeniedMessageBox();
                return;
            }

            if (item == null)
                return;

            try
            {
                var vm = new SmartDots.AddEditSDSampleViewModel(this, item, isNew);
                vm.InitializeAsync();
                AppRegionManager.LoadWindowViewFromViewModel(vm, true, "StandardWindowWithBorderStyle");
                if (!vm.Canceled)
                {
                    if(isNew)
                        _lstSDSamples.Insert(0, item);

                    FilterSDSamples();
                } 
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }

        #endregion


        #region Save Samples Command


        public DelegateCommand SaveCommand 
        {  
            get
            {
                return _cmdSaveSamples ?? (_cmdSaveSamples = new DelegateCommand(() => SaveSamples()));
            }
        }


        public bool SaveSamples(bool ignoreValidation = false)
        {
            if (!User.HasAddEditSDEventsAndSamplesTask)
            {
                DispatchAccessDeniedMessageBox();
                return false;
            }

            try
            {
                //If all list are empty, return.
                if ((SDSampleList == null || SDSampleList.Count <= 0) && (_lstDeletedSDSamples == null || _lstDeletedSDSamples.Count == 0))
                    return true;

                //If no changes, return.
                if ( !IsDirty && !SDSampleList.Any(x => x.ChangeTracker.State == ObjectState.Modified || x.ChangeTracker.State == ObjectState.Added) && (_lstDeletedSDSamples == null || _lstDeletedSDSamples.Count == 0))
                {
                    var res = AppRegionManager.ShowMessageBox(Translate("Common", "NoChanges"), System.Windows.MessageBoxButton.OK);
                    return false;
                }

                var lstEditedSaples = new List<SDSample>();

                //Add edited samples
                foreach (var item in SDSampleList)
                {
                    if (item.ChangeTracker.State != ObjectState.Unchanged)
                    {
                        string error = null;
                        if(!ignoreValidation && !AddEditSDSampleViewModel.IsSDSampleValid(this, item, ref error))
                        {
                            var t = Translate("SDSampleView", "SDSampleValidationError");
                            string msg = string.Format(t, item.animalId ?? "", error);
                            DispatchMessageBox(msg);
                            return false;
                        }

                        //Make sure modified time is set always on edited item when it was saved. It's not needed for new items, since modified time is set to creation time here.
                        if(item.ChangeTracker.State != ObjectState.Added)
                            item.ModifiedTimeLocal = DateTime.UtcNow;
                        lstEditedSaples.Add(item);
                    }
                }

                //Add deleted samples
                if(_lstDeletedSDSamples != null && _lstDeletedSDSamples.Count > 0)
                    lstEditedSaples.AddRange(_lstDeletedSDSamples);

                if (lstEditedSaples.Count > 0)
                {
                    var man = new BusinessLogic.SmartDots.SmartDotsManager();
                    var result = man.SaveSDSamples(lstEditedSaples);
                    //If unsuccessful, show error message.
                    if (result.DatabaseOperationStatus != Entities.DatabaseOperationStatus.Successful)
                    {
                        if(result.DatabaseOperationStatus == DatabaseOperationStatus.ValidationError && result.Message == "HASANNOTATIIONS")
                            LogAndDispatchUnexpectedErrorMessage(string.Format(Translate("SDSampleView", "DeletionErrorHasAnnotations"), result.GetProperty("FileName"), result.GetProperty("AnimalId")));
                        else
                            LogAndDispatchUnexpectedErrorMessage(result.Message);
                        return false;
                    }
                }

                //Make sure samples to be deleted are cleared.
                if(_lstDeletedSDSamples != null)
                    _lstDeletedSDSamples.Clear();

                //Update samples count on the event (so when going back to the events list, it is up to date).
                _sdEvent.SamplesCount = SDSampleList.Count;

                IsDirty = false;
                InitializeAsync(false);

                return true;
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }

            return false;
        }


        #endregion


        #region Delete SDSample Item Command


        public DelegateCommand<SDSample> DeleteSDSampleItemCommand
        {
            get
            {
                if(_cmdDeleteSample == null)
                    _cmdDeleteSample = new DelegateCommand<SDSample>(param => DeleteSample(param));
                return _cmdDeleteSample;
            }
        }

        private void DeleteSample(SDSample item, bool blnShowWarning = true)
        {
            if (!User.HasAddEditSDEventsAndSamplesTask)
            {
                DispatchAccessDeniedMessageBox();
                return;
            }

            try
            {
                if (item == null)
                    return;

                if(item.SDFile != null && item.SDFile.Any(x => x.SDAnnotation != null && x.SDAnnotation.Count>0))
                {
                    var res = AppRegionManager.ShowMessageBox(string.Format(Translate("SDSampleView", "WarningSampleHasAnnotations"), item.animalId), System.Windows.MessageBoxButton.OK);
                    return;
                }

                var state = item.ChangeTracker.State;

                if (blnShowWarning)
                {
                    var res = AppRegionManager.ShowMessageBox(Translate("Warning", "2"), System.Windows.MessageBoxButton.YesNo);

                    if (res == System.Windows.MessageBoxResult.No)
                        return;
                }


                if (state != ObjectState.Added)
                {

                    DeletedSDSamplesList.Add(_lstSDSamples.Where(x => x.animalId == item.animalId).FirstOrDefault());
                    _lstSDSamples.Remove(_lstSDSamples.Where(x => x.animalId == item.animalId && x.ChangeTracker.State != ObjectState.Deleted).FirstOrDefault());
                    IsDirty = true;
                    DeletedSDSamplesList.Where(x => x.animalId == item.animalId).FirstOrDefault().MarkAsDeleted();
                }
                else
                {
                    //if it is added/new, simply remove it from the samples list in memory only (since it does not exist in the DB yet).
                    //DeletedSDSamplesList.Add(_lstSDSamples.Where(x => x.animalId == item.animalId).FirstOrDefault());
                    _lstSDSamples.Remove(_lstSDSamples.Where(x => x.animalId == item.animalId).FirstOrDefault());
                    //IsDirty = true;
                    // DeletedSDSamplesList.Where(x => x.animalId == item.animalId).FirstOrDefault().MarkAsDeleted();
                }

                FilterSDSamples();
            }
            catch (Exception e)
            {

                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }

        #endregion


        #region Run Inconsistency Check Command


        public DelegateCommand RunInconsistencyCheckCommand
        {
            get { return (_cmdRunInconsistencyCheck ?? (_cmdRunInconsistencyCheck = new DelegateCommand(() => RunInconsistentyCheckAsync()))); }
        }


        public Task RunInconsistentyCheckAsync()
        {
            //Make sure the user has edit rights, or else the user cannot update the samples with the changes.
            if (!User.HasAddEditSDEventsAndSamplesTask)
                return Anchor.Core.GeneralExtensions.TaskFromResult<bool>(true);

            if (_lstSDSamples != null && _lstSDSamples.Count > 0 && !_lstSDSamples.All(x => x.ChangeTracker.State == ObjectState.Unchanged))
            {
                var resVal = AppRegionManager.ShowMessageBox(Translate("SDSampleView", "SamplesHaveChanges"), System.Windows.MessageBoxButton.OK);
                return Anchor.Core.GeneralExtensions.TaskFromResult<bool>(true);
            }

            IsMenuDropDownOpen = false;
            IsCheckingForChanges = true;
            return Task.Factory.StartNew(CheckForChanges).ContinueWith(t => new Action(() => IsCheckingForChanges = false).Dispatch());
        }


        #endregion


        #region Activate Copy Ages To FishLine Animals Command


        public DelegateCommand ActivateCopyAgesToFishLineAnimalsCommand
        {
            get
            {
                if (_cmdCopyAgesToFishLineAnimals == null)
                    _cmdCopyAgesToFishLineAnimals = new DelegateCommand(() => CopyAgesToFishLineAnimals());
                return _cmdCopyAgesToFishLineAnimals;
            }
        }


        private void CopyAgesToFishLineAnimals()
        {
            if (!User.HasAddEditSDEventsAndSamplesTask)
            {
                DispatchAccessDeniedMessageBox();
                return;
            }

            if ((_lstSDSamples != null && _lstSDSamples.Count > 0 && !_lstSDSamples.All(x => x.ChangeTracker.State == ObjectState.Unchanged) || (_lstDeletedSDSamples != null && _lstDeletedSDSamples.Count > 0)))
            {
                var resVal = AppRegionManager.ShowMessageBox(Translate("SDSampleView", "SamplesHaveChanges"), System.Windows.MessageBoxButton.OK);
                return;
            }

            IsCopyingAgesToFishLineActivated = true;
            IsMenuDropDownOpen = false;

            ResetSelection();
            FilterSDSamples();
        }


        #endregion


        #region Update Ages In FishLine Command


        public DelegateCommand UpdateAgesInFishLineCommand
        {
            get
            {
                if (_cmdUpdateAgesInFishLine == null)
                    _cmdUpdateAgesInFishLine = new DelegateCommand(() => UpdateAgesInFishLineAsync());
                return _cmdUpdateAgesInFishLine;
            }
        }


        private void UpdateAgesInFishLineAsync()
        {
            var samples = FilteredSDSamplesList.Where(x => x.IsSelected).ToList();

            if (samples == null || samples.Count == 0)
            {
                DispatchMessageBox(Translate("SDSampleView", "ValidationSelectSamples"));
                return;
            }

            foreach(var s in samples)
            {
                if(!s.HasSelectedSDFileForAgeTransfer || !s.SelectedSDFileForAgeTransfer.HasSelectedSDAnnotationForAgeTransfer)
                {
                    AppRegionManager.ShowMessageBox(string.Format(Translate("SDSampleView", "CopyAgesToFishLineMissingFilesOrAnnotationError"), s.animalId));
                    return;
                }

                if(!s.animalId.CanConvertToInt32())
                {
                    AppRegionManager.ShowMessageBox(string.Format(Translate("SDSampleView", "CopyAgesToFishLineNonIntegerAnimalIdError"), s.animalId));
                    return;
                }
                
                if(!s.SelectedSDFileForAgeTransfer.SelectedSDAnnotationForAgeTransfer.age.HasValue)
                {
                    AppRegionManager.ShowMessageBox(string.Format(Translate("SDSampleView", "CopyAgesToFishLineAgeMissingError"), s.animalId));
                    return;
                }

                if (!s.SelectedSDFileForAgeTransfer.SelectedSDAnnotationForAgeTransfer.otolithReadingRemarkId.HasValue)
                {
                    AppRegionManager.ShowMessageBox(string.Format(Translate("SDSampleView", "CopyAgesToFishLineAQScoreMissingError"), s.animalId));
                    return;
                }
            }
           

            if (AppRegionManager.ShowMessageBox(string.Format(Translate("SDSampleView", "CopyAgesToFishLineConfirmation"), samples.Count), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;

            IsLoading = true;
            Task.Factory.StartNew(() => UpdateAgesInFishLine(samples));
        }


        private void UpdateAgesInFishLine(List<SDSample> samples)
        {
            try
            {
                var man = new BusinessLogic.SmartDots.SmartDotsManager();

                List<SDAnimalAgeItem> lstToUpdate = new List<SDAnimalAgeItem>();
                foreach(var sample in samples)
                {
                    var item = new SDAnimalAgeItem();

                    //The animalId and ages was check in the method calling this one, so it is not required here again.
                    item.AnimalId = sample.animalId.ToInt32();
                    item.SDSampleId = sample.sdSampleId;
                    item.Age = sample.SelectedSDFileForAgeTransfer.SelectedSDAnnotationForAgeTransfer.age.Value;
                    item.ShouldAssignAge = true;

                    //Only assign age if the otolith reading remark says so.
                    if (sample.SelectedSDFileForAgeTransfer.SelectedSDAnnotationForAgeTransfer.L_OtolithReadingRemark != null && !sample.SelectedSDFileForAgeTransfer.SelectedSDAnnotationForAgeTransfer.L_OtolithReadingRemark.transAgeFromAquaDotsToFishLine)
                        item.ShouldAssignAge = false;

                    item.DFUPersonReaderId = sample.SelectedSDFileForAgeTransfer.SelectedSDAnnotationForAgeTransfer.createdById;
                    item.SDAnnotationId = sample.SelectedSDFileForAgeTransfer.SelectedSDAnnotationForAgeTransfer.sdAnnotationId;
                    item.OtolithReadingRemarkId = sample.SelectedSDFileForAgeTransfer.SelectedSDAnnotationForAgeTransfer.otolithReadingRemarkId;
                    item.EdgeStructureCode = sample.SelectedSDFileForAgeTransfer.SelectedSDAnnotationForAgeTransfer.edgeStructure;

                    lstToUpdate.Add(item);
                }

                var res = man.CopyAgesToFishLine(lstToUpdate);

                if (res.DatabaseOperationStatus == DatabaseOperationStatus.Successful)
                {
                    new Action(() =>
                    {
                        try
                        {
                            CloseExtraFeature();

                            int skippedDueToLavRepCount = 0;
                            int tmp;
                            if (res.Properties != null && res.Properties.ContainsKey("SkippedDueToLavRepCount") && res.Properties["SkippedDueToLavRepCount"].TryParseInt32(out tmp))
                                skippedDueToLavRepCount = tmp;

                            string msg = string.Format(Translate("SDSampleView", "CopyAgesToFishLineSuccess"), lstToUpdate.Count - skippedDueToLavRepCount, lstToUpdate.Count);

                            //If any animals was skipped, make sure to notify why.
                            if (skippedDueToLavRepCount > 0)
                                msg += Environment.NewLine + Environment.NewLine + string.Format(Translate("SDSampleView", "CopyAgesToFishLineSkippedDueToLavRep"), skippedDueToLavRepCount);

                            DispatchMessageBox(msg);

                            //Reload samples and set IsLoading flag to false afterwards.
                            InitializeAsync(false);
                        }
                        catch 
                        {
                            IsLoading = false;
                        }
                    }).Dispatch();
                }
                else
                {
                    new Action(() =>
                    {
                        IsLoading = false;
                        switch(res.Message ?? "")
                        {
                            case "AbortOperation":
                            case "NothingToUpdate":
                                DispatchMessageBox(string.Format(Translate("Common", "Error"), res.UIMessage ?? ""));
                                return;
                        }

                        DispatchMessageBox(string.Format(Translate("Common", "Error"), res.Message ?? ""));
                    }).Dispatch();
                }
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);

                new Action(() =>
                {
                    IsLoading = false;
                }).Dispatch();
            }
        }


        #endregion


        #region Activate Assigning Scale To All Selected Samples Command


        public DelegateCommand ActivateCopyScaleToAllSelectedSamplesCommand
        {
            get
            {
                if (_cmdCopyScaleToAllSelectedSamples == null)
                    _cmdCopyScaleToAllSelectedSamples = new DelegateCommand(() => CopyScaleToAllSelectedSamples());
                return _cmdCopyScaleToAllSelectedSamples;
            }
        }


        private void CopyScaleToAllSelectedSamples()
        {
            if (!User.HasAddEditSDEventsAndSamplesTask)
            {
                DispatchAccessDeniedMessageBox();
                return;
            }

            if ((_lstSDSamples != null && _lstSDSamples.Count > 0 && !_lstSDSamples.All(x => x.ChangeTracker.State == ObjectState.Unchanged) || (_lstDeletedSDSamples != null && _lstDeletedSDSamples.Count > 0)))
            {
                var resVal = AppRegionManager.ShowMessageBox(Translate("SDSampleView", "SamplesHaveChanges"), System.Windows.MessageBoxButton.OK);
                return;
            }

            IsCopyingScaleToSamplesActivated = true;
            IsMenuDropDownOpen = false;
            ResetSelection();
            FilterSDSamples();
        }


        #endregion


        #region Assign Scale To Samples Command


        public DelegateCommand AssignScaleToSamplesCommand
        {
            get
            {
                if (_cmdAssignScaleToSamples == null)
                    _cmdAssignScaleToSamples = new DelegateCommand(() => AssignScaleToSamplesAsync());
                return _cmdAssignScaleToSamples;
            }
        }


        private void AssignScaleToSamplesAsync()
        {
            if(!ScaleToBeAssigned.HasValue)
            {
                DispatchMessageBox(Translate("SDSampleView", "ScaleValidationEnterValue"));
                return;
            }

            var samples = FilteredSDSamplesList.Where(x => x.IsSelected).ToList();

            if (samples == null || samples.Count == 0)
            {
                DispatchMessageBox(Translate("SDSampleView", "ValidationSelectSamples"));
                return;
            }

            if (AppRegionManager.ShowMessageBox(string.Format(Translate("SDSampleView", "AssignScaleConfirmation"), ScaleToBeAssigned.Value.ToString(System.Globalization.CultureInfo.InvariantCulture), samples.Count), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;

            IsLoading = true;
            Task.Factory.StartNew(() => AssignScaleToSamples(samples, ScaleToBeAssigned.Value)).ContinueWith(t => new Action(() =>
            {
                IsLoading = false;
            }).Dispatch()); ;
        }


        private void AssignScaleToSamples(List<SDSample> samples, double scale)
        {
            try
            {
                int changedSamplesCount = 0;
                foreach (var s in samples)
                {
                    foreach (var f in s.SDFile)
                        f.scale = scale;

                    s.MarkAsModified();
                    changedSamplesCount++;
                }

                if (SaveSamples())
                {
                    new Action(() =>
                    {
                        CloseExtraFeature();

                        DispatchMessageBox(string.Format(Translate("SDSampleView", "SuccessfullyAssignedScaleMessage"), scale.ToString(System.Globalization.CultureInfo.InvariantCulture), changedSamplesCount));

                    }).Dispatch();
                }
            }
            catch(Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }

        #endregion
    }
}
