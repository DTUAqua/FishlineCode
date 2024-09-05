using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anchor.Core;
using Anchor.Core.Controls;
using Babelfisk.Entities.Sprattus;
using Microsoft.Practices.Prism.Commands;

namespace Babelfisk.ViewModels.SmartDots
{
    public class SDEventsViewModel : AViewModel
    {
        private DelegateCommand<SDEvent> _cmdSelectEvent;
        private DelegateCommand _cmdAddEvent;
        private DelegateCommand<SDEvent> _cmdEditEvent;
        private DelegateCommand _cmdTest;

        private AViewModel _activeViewModel;

        private List<SDEvent> _lstAllEvents;

        private List<SDEvent> _lstFilteredEvents;

        private string _searchString;

        private List<DropDownListBoxItem> _eventTypes;
        private List<DropDownListBoxItem> _years;
        private List<DropDownListBoxItem> _species;
        private List<DropDownListBoxItem> _areas;

        #region Properties


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


        /// <summary>
        /// Number of total events.
        /// </summary>
        public int AllEventsCount
        {
            get { return _lstAllEvents == null ? 0 : _lstAllEvents.Count; }
        }


        public List<SDEvent> AllEvents
        {
            get { return _lstAllEvents; }
        }


        /// <summary>
        /// Events to show in the UI filtered by the search textbox.
        /// </summary>
        public List<SDEvent> FilteredEvents
        {
            get { return _lstFilteredEvents; }
            set
            {
                _lstFilteredEvents = value;
                RaisePropertyChanged(() => FilteredEvents);
                RaisePropertyChanged(() => HasFilteredEvents);
                RaisePropertyChanged(() => AllEventsCount);
            }
        }


        /// <summary>
        /// Whether or not any events are visible in the list.
        /// </summary>
        public bool HasFilteredEvents
        {
            get { return _lstFilteredEvents != null && _lstFilteredEvents.Count > 0; }
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

                FilterEvents();
            }
        }


        /// <summary>
        /// Return list of Event types for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedEventTypes
        {
            get { return _eventTypes; }
            set
            {
                _eventTypes = value;
                RaisePropertyChanged(() => UsedEventTypes);
            }
        }


        /// <summary>
        /// Return list of Years for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedYears
        {
            get { return _years; }
            set
            {
                _years = value;
                RaisePropertyChanged(() => UsedYears);
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
        /// Return list of Areas for column header drop down. 
        /// </summary>
        public List<DropDownListBoxItem> UsedAreas
        {
            get { return _areas; }
            set
            {
                _areas = value;
                RaisePropertyChanged(() => UsedAreas);
            }
        }


        #endregion


        public SDEventsViewModel()
        {
            WindowWidth = 1200;
            WindowHeight = 650;
            WindowTitle = "Aquadots administration";
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


        /// <summary>
        /// Loading all events asynchronously.
        /// </summary>
        public void InitializeAsync()
        {
            IsLoading = true;
            Task.Factory.StartNew(LoadEvents).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        /// <summary>
        /// Load events synchronously.
        /// </summary>
        private void LoadEvents()
        {
            try
            {
                var man = new BusinessLogic.SmartDots.SmartDotsManager();
                var events = man.GetSDEvents();

                // Performance test with 1000 events
                /*for(int i = 0; i < 1000; i++)
                {
                    var evt = events[0];
                    var sdNew = evt.Clone();
                    sdNew.ChangeTracker.ChangeTrackingEnabled = false;
                    sdNew.sdEventId = sdNew.sdEventId + i + 1;
                    sdNew.ChangeTracker.ChangeTrackingEnabled = true;
                    events.Add(sdNew);
                }*/

                _lstAllEvents = events ?? new List<SDEvent>();

                List<DropDownListBoxItem> lstEventTypes = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> lstYears = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> lstspecies = new List<DropDownListBoxItem>();
                List<DropDownListBoxItem> lstAreas = new List<DropDownListBoxItem>();

                var cmp = new Anchor.Core.Comparers.StringNumberComparer();
                
                //Event types
                lstEventTypes = _lstAllEvents.Where(x => x.L_SDEventType != null)
                                         .DistinctBy(x => x.L_SDEventType)
                                         .Select(item => new DropDownListBoxItem() { Text = item.L_SDEventType.UIDisplay, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();

                //Years
                lstYears = _lstAllEvents.Where(x => x.year != null)
                                         .DistinctBy(x => x.year)
                                         .Select(item => new DropDownListBoxItem() { Text = item.year.ToString(), CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();

                //Species
                lstspecies = _lstAllEvents.Where(x => !string.IsNullOrEmpty(x.speciesCode))
                                         .DistinctBy(x => x.speciesCode)
                                         .Select(item => new DropDownListBoxItem() { Text = item.speciesCode, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();

                //Areas
                lstAreas = _lstAllEvents.Where(x => x.L_DFUAreas != null && x.L_DFUAreas.Count > 0)
                                         .SelectMany(x => x.L_DFUAreas)
                                         .DistinctBy(x => x.DFUArea)
                                         .Select(item => new DropDownListBoxItem() { Text = item.UIDisplay, CheckedChangedMethodReference = OnFilterItemChanged })
                                         .OrderBy(x => x.Text, cmp)
                                         .ToList();


                new Action(() =>
                {
                    UsedEventTypes = lstEventTypes;
                    UsedYears = lstYears;
                    UsedSpecies = lstspecies;
                    UsedAreas = lstAreas;

                    FilterEvents();

                }).Dispatch();
            }
            catch(Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }


        private void OnFilterItemChanged(DropDownListBoxItem lb, bool oldValue, bool newValue)
        {
            FilterEvents();
        }


        /// <summary>
        /// Add or replace an event entity in the list. This should be called after adding or updating an event.
        /// </summary>
        /// <param name="evt"></param>
        public void AddOrReplaceEvent(SDEvent evt)
        {
           var evtExisting = _lstAllEvents.Where(x => x.sdEventId == evt.sdEventId).FirstOrDefault();

            if (evtExisting != null)
                _lstAllEvents.Remove(evtExisting);

            _lstAllEvents.Add(evt);

            FilterEvents();
        }


        /// <summary>
        /// Remove an event from the list.
        /// </summary>
        /// <param name="evt"></param>
        public void RemoveEvent(SDEvent evt)
        {

            var evtExisting = _lstAllEvents.Where(x => x.sdEventId == evt.sdEventId).FirstOrDefault();

            if (evtExisting != null)
                _lstAllEvents.Remove(evtExisting);

            FilterEvents();
        }


        /// <summary>
        /// Filter event by SearchString and sort them according to the SelectedSortByItem.
        /// </summary>
        private void FilterEvents()
        {
            try
            {
                if (_lstAllEvents == null)
                {
                    FilteredEvents = new List<SDEvent>();
                    return;
                }

                IEnumerable<SDEvent> lst = _lstAllEvents;

                if (!string.IsNullOrWhiteSpace(SearchString))
                {
                    var search = SearchString ?? "";
                    lst = lst.Where(x => (x.name ?? "").Contains(search, StringComparison.InvariantCultureIgnoreCase) ||
                                         (x.sdEventId != 0 && (x.sdEventId.ToString() ?? "").Contains(search, StringComparison.InvariantCultureIgnoreCase)) ||
                                         (x.startDate.HasValue && x.startDate.Value.ToString("dd-MM-yyyy").Contains(search, StringComparison.InvariantCultureIgnoreCase)) ||
                                         (x.endDate.HasValue && x.endDate.Value.ToString("dd-MM-yyyy").Contains(search, StringComparison.InvariantCultureIgnoreCase)) ||
                                         (x.createdByUserName != null && x.createdByUserName.Contains(search, StringComparison.InvariantCultureIgnoreCase)) ||
                                         (x.comment ?? "").Contains(search, StringComparison.InvariantCultureIgnoreCase)
                                         );
                }

                #region Old sorting
                /*
                var cmp = new Anchor.Core.Comparers.StringNumberComparer();
                switch (SelectedSortByItem.Value)
                {
                    case "Name":
                        if (SortDescending)
                            lst = lst.OrderByDescending(x => x.name, cmp).ThenBy(x => x.sdEventId);
                        else
                            lst = lst.OrderBy(x => x.name, cmp).ThenBy(x => x.sdEventId);
                        break;

                    case "EventType":
                        if (SortDescending)
                            lst = lst.OrderByDescending(x => x.eventTypeId).ThenBy(x => x.sdEventId);
                        else
                            lst = lst.OrderBy(x => x.eventTypeId).ThenBy(x => x.sdEventId);
                        break;

                    case "Year":
                        if (SortDescending)
                            lst = lst.OrderByDescending(x => x.year).ThenBy(x => x.sdEventId);
                        else
                            lst = lst.OrderBy(x => x.year).ThenBy(x => x.sdEventId);
                        break;

                    case "Species":
                        if (SortDescending)
                            lst = lst.OrderByDescending(x => x.speciesCode, cmp).ThenBy(x => x.sdEventId);
                        else
                            lst = lst.OrderBy(x => x.speciesCode, cmp).ThenBy(x => x.sdEventId);
                        break;

                    case "Area":
                        if (SortDescending)
                            lst = lst.OrderByDescending(x => x.DFUArea, cmp).ThenBy(x => x.sdEventId);
                        else
                            lst = lst.OrderBy(x => x.DFUArea, cmp).ThenBy(x => x.sdEventId);
                        break;

                    case "StartDate":
                        if (SortDescending)
                            lst = lst.OrderByDescending(x => x.startDate).ThenBy(x => x.sdEventId);
                        else
                            lst = lst.OrderBy(x => x.startDate).ThenBy(x => x.sdEventId);
                        break;

                    case "EndDate":
                        if (SortDescending)
                            lst = lst.OrderByDescending(x => x.endDate).ThenBy(x => x.sdEventId);
                        else
                            lst = lst.OrderBy(x => x.endDate).ThenBy(x => x.sdEventId);
                        break;
                    case "Id":
                    default:
                        if (SortDescending)
                            lst = lst.OrderByDescending(x => x.sdEventId);
                        else
                            lst = lst.OrderBy(x => x.sdEventId);
                        break;
                }
                */
                #endregion

                var selectedEventTypes = UsedEventTypes == null ? new HashSet<string>() : UsedEventTypes.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedYears = UsedYears == null ? new HashSet<string>() : UsedYears.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedSpecies = UsedSpecies == null ? new HashSet<string>() : UsedSpecies.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();
                var selectedAreas = UsedAreas == null ? new HashSet<string>() : UsedAreas.Where(x => x.IsChecked).Select(x => x.Text).Distinct().ToHashSet<string>();


                //Event Types
                if (selectedEventTypes.Count > 0)
                    lst = lst.Where(x => x.L_SDEventType != null && selectedEventTypes.Contains(x.L_SDEventType.UIDisplay ?? ""));
                
                //Years
                if (selectedYears.Count > 0)
                    lst = lst.Where(x => x.year != null && selectedYears.Contains(x.year.ToString() ?? ""));
                
                //Species
                if (selectedSpecies.Count > 0)
                    lst = lst.Where(x => selectedSpecies.Contains(x.speciesCode ?? ""));

                //areas
                if (selectedAreas.Count > 0)
                    lst = lst.Where(x => x.L_DFUAreas != null && x.L_DFUAreas.Count > 0 &&  x.L_DFUAreas.Any(a => selectedAreas.Contains(a.UIDisplay ?? "")));

                FilteredEvents = lst.ToList();
            }
            catch { }
        }


        public override void FireClosing(object sender, CancelEventArgs e)
        {
            if (_activeViewModel != null && _activeViewModel != this && _activeViewModel.IsDirty)
            {
                _activeViewModel.FireClosing(sender, e);
            }
        }
       

        #region Add Event Command


        public DelegateCommand AddEventCommand
        {
            get
            {
                if (_cmdAddEvent == null)
                    _cmdAddEvent = new DelegateCommand(() => AddEvent());

                return _cmdAddEvent;
            }
        }


        /// <summary>
        /// Load the Add event view.
        /// </summary>
        private void AddEvent()
        {
            if (!User.HasAddEditSDEventsAndSamplesTask)
            {
                DispatchAccessDeniedMessageBox();
                return;
            }

            try
            {
                var vm = new SmartDots.AddEditSDEventViewModel(this, this);
                vm.InitializeAsync();
                ActivateViewModel(vm);
            }
            catch (Exception e)
            {

                LogAndDispatchUnexpectedErrorMessage(e);
            }
          
        }


        #endregion

        #region Edit Event Command

        public DelegateCommand<SDEvent> EditEventCommand
        {
            get
            {
                if (_cmdEditEvent == null)
                    _cmdEditEvent = new DelegateCommand<SDEvent>(p => EditEvent(p));

                return _cmdEditEvent;
            }
        }


        /// <summary>
        /// Edit an existing event by loading the edit event view.
        /// </summary>
        /// <param name="evt"></param>
        private void EditEvent(SDEvent evt)
        {
            if(!User.HasAddEditSDEventsAndSamplesTask)
            {
                DispatchAccessDeniedMessageBox();
                return;
            }

            try
            {
                var vm = new SmartDots.AddEditSDEventViewModel(this, this, evt);
                vm.InitializeAsync();
                ActivateViewModel(vm);
            }
            catch (Exception e)
            {

                LogAndDispatchUnexpectedErrorMessage(e);
            }
          
        }


        #endregion

        #region Select Event Command
        public DelegateCommand<SDEvent> SelectEventCommand
        {
            get
            {
                if (_cmdSelectEvent == null)
                    _cmdSelectEvent = new DelegateCommand<SDEvent>(p => SelectEvent(p));

                return _cmdSelectEvent;
            }
        }


        /// <summary>
        /// Select an event and load the samples view.
        /// </summary>
        /// <param name="evt"></param>
        public void SelectEvent(SDEvent evt)
        {
            try
            {
                var vm = new SmartDots.SDSamplesViewModel(this, evt);
                vm.InitializeAsync();
                ActivateViewModel(vm);
            }
            catch(Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }

        #endregion



        #region Test Command


        public DelegateCommand TestCommand
        {
            get
            {
                if (_cmdTest == null)
                    _cmdTest = new DelegateCommand(() => Test());

                return _cmdTest;
            }
        }


        public void Test()
        {
            try
            {
                var vm = new SelectFoldersOrFilesViewModel(true, false);
                vm.WindowWidth = 600;
                vm.WindowHeight = 400;
                vm.WindowTitle = "Select folders";

                AppRegionManager.LoadWindowViewFromViewModel(vm, true, "StandardWindowWithBorderStyle");
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }

        #endregion


    }
}
