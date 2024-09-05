using Babelfisk.Entities.Sprattus;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anchor.Core;
using Babelfisk.BusinessLogic;
using Babelfisk.BusinessLogic.SmartDots;
using System.Collections.ObjectModel;
using Babelfisk.ViewModels.Lookup;
using Anchor.Core.Controls;
using System.Windows.Input;
using System.Windows.Controls;
using Babelfisk.Entities;

namespace Babelfisk.ViewModels.SmartDots
{
    public class AddEditSDEventViewModel : Input.AInputViewModel
    {
        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdDelete;
        private DelegateCommand _cmdAddUser;
        private DelegateCommand<R_SDEventSDReader> _cmdDeleteReader;
        private DelegateCommand<string> _cmdAddEditLookups;
        private DelegateCommand _cmdAddImageFolders;


        private List<DropDownTagListBoxItem<L_DFUArea>> _ddlAreas;
        private ObservableCollection<R_SDEventSDReader> _lstSDReaders;

        private SDEventsViewModel _vmEvents;

        private List<L_Species> _lstSpecies;

        private List<L_DFUArea> _lstDFUAreas;

        private List<L_SDEventType> _lstEventTypes;

        private List<L_SDPurpose> _lstPurposes;

        private List<L_SDSampleType> _lstSampleTypes;

        private List<Entities.SprattusSecurity.Users> _lstUsers;

        private List<DropDownTagListBoxItem<SDFilesExtraColumn>> _lstSDFileExtraColumns;

        private SDEvent _originalEntity;

        private SDEvent _entity;

        private bool _updatingCollections;

        private bool _isEndDateChecked;

        private AViewModel _vmReturnTo = null;

        private bool _isEdit;
        #region Properties



        /// <summary>
        /// Return list of SDReaders for the areas drop down. 
        /// </summary>
        public ObservableCollection<R_SDEventSDReader> SDReaderList
        {
            get { return  _entity == null ? null : _entity.SDReaders; }
            set
            {
                if (_entity.SDReaders != value)
                    _entity.SDReaders = (TrackableCollection<R_SDEventSDReader>)value;
                RaisePropertyChanged(()=>SDReaderList);
                RaisePropertyChanged(() => HasSDReaders);
            }
        }

      

        /// <summary>
        /// Whether or not the viewmodel is in Edit mode (otherwise it's "new event mode").
        /// </summary>
        public bool IsEdit
        {
            get { return _isEdit; }
            set
            {
                _isEdit = value;
                RaisePropertyChanged(() => IsEdit);
            }
        }


        /// <summary>
        /// Boolean used when updating the various lookup collections.
        /// </summary>
        private bool UpdatingCollections
        {
            get { return _updatingCollections || _blnValidate; }
        }


        /// <summary>
        /// Return list of events type for the event types drop down.
        /// </summary>
        public List<L_SDEventType> EventTypes
        {
            get { return _lstEventTypes == null ? null : _lstEventTypes.ToList(); }
            private set
            {
                _lstEventTypes = value;
                RaisePropertyChanged(() => EventTypes);

            }
        }


        /// <summary>
        /// Selected event type.
        /// </summary>
        public L_SDEventType SelectedEventType
        {
            get { return _entity == null ? null : _entity.L_SDEventType; }
            set
            {
                if (!UpdatingCollections)
                {
                    if (_entity.L_SDEventType != value)
                        _entity.L_SDEventType = value;
                }

                RaisePropertyChanged(() => SelectedEventType);
                RaisePropertyChanged(() => IsEventNameFieldVisible);
                RaisePropertyChanged(() => IsEndDateEnabled);
                RaisePropertyChanged(() => IsAreaMandatory);
                RaisePropertyChanged(() => IsEndDateMandatory);
                RaisePropertyChanged(() => IsYearMandatory);
            }
        }


        /// <summary>
        /// Selected sample type.
        /// </summary>
        public L_SDSampleType SelectedSampleType
        {
            get { return _entity == null ? null : _entity.L_SDSampleType; }
            set
            {
                if (!UpdatingCollections)
                {
                    if (_entity.L_SDSampleType != value)
                        _entity.L_SDSampleType = value;
                }

                RaisePropertyChanged(() => SelectedSampleType);
            }
        }


        /// <summary>
        /// Return list of events type for the sample types drop down.
        /// </summary>
        public List<L_SDSampleType> SampleTypes
        {
            get { return _lstSampleTypes == null ? null : _lstSampleTypes.ToList(); }
            private set
            {
                _lstSampleTypes = value;
                RaisePropertyChanged(() => SampleTypes);
            }
        }


        /// <summary>
        /// Property holding the event name.
        /// </summary>
        public string EventName
        {
            get { return _entity.name; }
            set
            {
                if (_entity.name != value)
                    _entity.name = value;

                RaisePropertyChanged(() => EventName);
            }
        }


        /// <summary>
        /// Property holding the event year.
        /// </summary>
        public int? Year
        {
            get { return _entity.year; }
            set
            {
                if (_entity.year != value)
                    _entity.year = value;

                RaisePropertyChanged(() => Year);
            }
        }



        /// <summary>
        /// Return list of species for the species drop down. 
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
        /// Selected species property.
        /// </summary>
        public L_Species SelectedSpecies
        {
            get { return _entity == null ? null : _entity.L_Species; }
            set
            {
                if (!UpdatingCollections)
                {
                    if (_entity.L_Species != value)
                        _entity.L_Species = value;
                }

                RaisePropertyChanged(() => SelectedSpecies);
            }
        }


        /// <summary>
        /// Return list of DFU areas for the areas drop down. 
        /// </summary>
        public List<DropDownTagListBoxItem<L_DFUArea>> AreasList
        {
            get { return _ddlAreas; }
            set
            {
                _ddlAreas = value;
                RaisePropertyChanged(() => AreasList);
                RaisePropertyChanged(() => HasSelectedAreas);
            }
        }

        public bool HasSelectedAreas
        {
            get { return AreasList != null && AreasList.Count > 0; }
        }


        public bool IsAreaMandatory
        {
            get
            {
                var et = SelectedEventType;
                if (et == null)
                    return false;

                return SDEvent.IsEventTypeYearlyReading(et.L_sdEventTypeId) || SDEvent.IsEventTypeTogetherReading(et.L_sdEventTypeId);
            }
        }


        public bool IsEndDateMandatory
        {
            get
            {
                var et = SelectedEventType;
                if (et == null)
                    return false;

                return SDEvent.IsEventTypeYearlyReading(et.L_sdEventTypeId);
            }
        }

        public bool IsYearMandatory
        {
            get
            {
                var et = SelectedEventType;
                if (et == null)
                    return false;

                return SDEvent.IsEventTypeYearlyReading(et.L_sdEventTypeId) || SDEvent.IsEventTypeTogetherReading(et.L_sdEventTypeId);
            }
        }

   

        /// <summary>
        /// Property holding the event Id.
        /// </summary>
        public int EventId
        {
            get { return _entity.sdEventId; }
        }

      


        /// <summary>
        /// Property holding the start date (not time, only date).
        /// </summary>
        public DateTime? StartDate
        {
            get { return _entity.startDate; }
            set
            {
                if (_entity.startDate != value)
                    _entity.startDate = value;

                RaisePropertyChanged(() => StartDate);
            }
        }


        /// <summary>
        /// Porperty holding the end date (not time, only date).
        /// </summary>
        public DateTime? EndDate
        {
            get { return _entity.endDate; }
            set
            {
                if(_entity.endDate != value)
                    _entity.endDate = value;

                RaisePropertyChanged(() => EndDate);
            }
        }

        /// <summary>
        /// Property holding the event cpmment.
        /// </summary>
        public string EventComments
        {
            get { return _entity.comment; }
            set
            {
                if (_entity.comment != value)
                    _entity.comment = value;

                RaisePropertyChanged(() => EventComments);
            }
        }


        /// <summary>
        /// Whether or not the event is closed and whether the assigned age readers, can currently do readings.
        /// </summary>
        public bool IsClosed
        {
            get { return _entity.closed; }
            set 
            {
                if(_entity.closed !=  value)
                    _entity.closed = value;

                RaisePropertyChanged(() => IsClosed);
            }
        }



        public string YesString
        {
            get { return string.Format(" ({0})", Translate("Common", "Yes")); }
        }

        public string NoString
        {
            get { return string.Format(" ({0})", Translate("Common", "No")); }
        }


        public string HeaderText
        {
            get { return IsEdit ? string.Format("{0} {1}", Translate("AddEditSDEventView", "EditEventWithInput"), EventName) : Translate("AddEditSDEventView", "AddNewEvent"); }
        }

        public bool IsEventNameFieldVisible
        {
            get { return SelectedEventType != null ? !SDEvent.IsEventTypeYearlyReading(SelectedEventType.L_sdEventTypeId) : false; }
        }

        public bool IsEndDateChecked
        {
            get { return _isEndDateChecked; }
            set 
            {
                _isEndDateChecked = value;
                RaisePropertyChanged(() => IsEndDateChecked);
                RaisePropertyChanged(()=>IsEndDateEnabled);
            }
        }

        public bool IsEndDateEnabled
        {
            get { return (SelectedEventType != null  && SDEvent.IsEventTypeYearlyReading(SelectedEventType.L_sdEventTypeId)) || IsEndDateChecked ; }
        }

        public bool HasSDReaders
        {
            get { return SDReaderList != null && SDReaderList.Count > 0; }
        }


        public List<DropDownTagListBoxItem<SDFilesExtraColumn>> SDFileExtraColumns
        {
            get { return _lstSDFileExtraColumns;  }
            set
            {
                _lstSDFileExtraColumns = value;
                RaisePropertyChanged(() => SDFileExtraColumns);
            }
        }


        public string[] DefaultImageFolders
        {
            get { return _entity.DefaultImageFoldersArray; }
            set
            {
                _entity.DefaultImageFoldersArray = value;
                RaisePropertyChanged(() => DefaultImageFolders);
                RaisePropertyChanged(() => HasDefaultImageFolders);
            }
        }


        public bool HasDefaultImageFolders
        {
            get
            {
                if (_entity == null)
                    return false;

                var arr = _entity.DefaultImageFoldersArray;

                return arr != null && arr.Length > 0;
            }
        }

        #endregion


        public AddEditSDEventViewModel(AViewModel returnToViewModel, SDEventsViewModel sdEventsVM, SDEvent sdEdit = null)
        {
            _vmReturnTo = returnToViewModel;
             _vmEvents = sdEventsVM;
            WindowWidth = 600;
            WindowHeight = 450;


            //If in new event mode or edit mode.
            if (sdEdit == null)
            {
                IsEdit = false;
                _entity = new SDEvent();
                SDReaderList = new TrackableCollection<R_SDEventSDReader>();
                _entity.endDate = DateTime.Now.AddMonths(1).Date;
                _originalEntity = _entity.Clone();
            }
            else 
            {
                //Edit mode.
                if (sdEdit.endDate != null && sdEdit.endDate != default(DateTime))
                    IsEndDateChecked = true;
                
                IsEdit = true;

                _originalEntity = sdEdit;
                _entity = sdEdit.Clone(); //Clone event, so any changes can be discarded if cancel is clicked.
            }

            _ddlAreas = new List<DropDownTagListBoxItem<L_DFUArea>>();

        }


        /// <summary>
        /// Load view model asynchronously loading all drop down list data.
        /// </summary>
        public void InitializeAsync()
        {
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
                var manLookup = new LookupManager();
                var lv = new LookupDataVersioning();

                //Fetch species
                var lstSpecies = manLookup.GetLookups(typeof(L_Species), lv).OfType<L_Species>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch areas
                var lstAreas = manLookup.GetLookups(typeof(L_DFUArea), lv).OfType<L_DFUArea>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch event types
                var lstEventTypes = manLookup.GetLookups(typeof(L_SDEventType), lv).OfType<L_SDEventType>().OrderBy(x => x.num ?? 0).ToList();

                //Fetch purposes
                var lstPurposes = manLookup.GetLookups(typeof(L_SDPurpose), lv).OfType<L_SDPurpose>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch sample types
                var lstSampleTypes = manLookup.GetLookups(typeof(L_SDSampleType), lv).OfType<L_SDSampleType>().OrderBy(x => x.UIDisplay).ToList();

               
                //Populate areas checkbox list.
                List<DropDownTagListBoxItem<L_DFUArea>> areaItems = new List<DropDownTagListBoxItem<L_DFUArea>>();
                if (lstAreas != null && lstAreas.Count > 0)
                {
                    List<L_DFUArea> lstCheckedAreas = null;
                    if (AreasList != null && AreasList.Count > 0)
                        lstCheckedAreas = AreasList.Where(x => x.IsChecked).Select(x => x.Tag).ToList();

                    foreach (var item in lstAreas)
                    {
                        bool isChecked = false;
                        if (lstCheckedAreas == null)
                            isChecked = _entity != null && _entity.L_DFUAreas != null && _entity.L_DFUAreas.Any(x => x.L_DFUAreaId == item.L_DFUAreaId);
                        else
                            isChecked = lstCheckedAreas.Any(x => x.L_DFUAreaId == item.L_DFUAreaId);

                        areaItems.Add(new DropDownTagListBoxItem<L_DFUArea>(item.UIDisplay.ToString(), isChecked, item));
                    }

                    //Order the areas by Text using a string number comparer.
                    var cmp = new Anchor.Core.Comparers.StringNumberComparer();
                    areaItems = areaItems.OrderBy(x => x.Text, cmp).ToList();
                }

                var selectedSDFileExtraColumns = _entity.SDFileExtraColumns;
                List<DropDownTagListBoxItem<SDFilesExtraColumn>> lstExtraColumns = new List<DropDownTagListBoxItem<SDFilesExtraColumn>>();
                foreach(SDFilesExtraColumn c in Enum.GetValues(typeof(SDFilesExtraColumn)))
                {
                    var dd = new DropDownTagListBoxItem<SDFilesExtraColumn>(Translate("SDFilesExtraColumn", c.ToString()), selectedSDFileExtraColumns.Contains(c), c);
                    lstExtraColumns.Add(dd);
                }

                new Action(() =>
                {
                    _updatingCollections = true;
                    {
                        _lstPurposes = lstPurposes;
                        Species = lstSpecies;
                        EventTypes = lstEventTypes;
                        SampleTypes = lstSampleTypes;
                        AreasList = areaItems;
                        SDFileExtraColumns = lstExtraColumns;
                    }
                    _updatingCollections = false;

                    if (SelectedSampleType == null && SampleTypes != null && SampleTypes.Count > 0)
                    {
                        SelectedSampleType = SampleTypes.Where(x => x.Id == "2").FirstOrDefault();
                        _originalEntity.L_SDSampleType = SelectedSampleType;
                    }
                        

                }).Dispatch();
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }



        #region Add Edit Lookups Command


        public DelegateCommand<string> AddEditLookupsCommand
        {
            get { return _cmdAddEditLookups ?? (_cmdAddEditLookups = new DelegateCommand<string>(p => AddEditLookups(p))); }
        }


        /// <summary>
        /// Add/Or edit a lookup list. TODO - implement lookup list refresh after alterations to the list.
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
            catch(Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }


        #endregion


        #region Save Command


        /// <summary>
        /// Validates property with name strFieldName. This method is overriding a base method which is called
        /// whenever a property is changed.
        /// </summary>
        protected override string ValidateField(string strFieldName)
        {
            string strError = null;

            if (!_blnValidate)
                return strError;
            
            switch (strFieldName)
            {
                case "EventName":
                    if (IsEventNameFieldVisible && string.IsNullOrWhiteSpace(EventName))
                        strError = Translate("AddEditSDEventView", "ValidationEventName");
                    break;

                case "SelectedEventType":
                    if (SelectedEventType == null)
                        strError = Translate("AddEditSDEventView", "ValidationEventType");
                    break;

                case "Year":
                    if (SelectedEventType != null && !SDEvent.IsEventTypeReference(SelectedEventType.L_sdEventTypeId) && (Year == null || Year.Value.ToString().Length < 4))
                        strError = Translate("AddEditSDEventView", "ValidationYear");
                    break;

                case "SelectedSpecies":
                    if (SelectedSpecies == null)
                        strError = Translate("AddEditSDEventView", "ValidationSpecies"); 
                    break;

                case "SelectedSampleType":
                    if (SelectedSampleType == null)
                        strError = Translate("AddEditSDEventView", "ValidationSampleType");
                    break;

                case "AreasList":
                    if (SelectedEventType != null && !SDEvent.IsEventTypeReference(SelectedEventType.L_sdEventTypeId) && (GetSelectedAreas() == null || GetSelectedAreas().Count == 0))
                        strError = Translate("AddEditSDEventView", "ValidationArea");
                    break;

                case "StartDate":
                    if(StartDate == null || StartDate.Value.Year <= 1900)
                        strError = Translate("AddEditSDEventView", "ValidationStartDate");
                    break;

                case "EndDate":
                    if (IsEndDateEnabled)
                    {
                        if(EndDate == null || EndDate.Value.Year <= 1900)
                            strError = Translate("AddEditSDEventView", "ValidationEndDate");
                        else if(StartDate != null && EndDate.Value < StartDate)
                            strError = Translate("AddEditSDEventView", "ValidationEndDateAfterStart");
                    }
                    break;
            }

            return strError;
        }


        private List<L_DFUArea> GetSelectedAreas()
        {
            var res = new List<L_DFUArea>();
            if (AreasList == null)
                return res;

            return AreasList.Where(x => x.IsChecked).Select(x => x.Tag).ToList();
        }


        protected override void ValidateAndSaveAsync()
        {
            if (!User.HasAddEditSDEventsAndSamplesTask)
            {
                DispatchAccessDeniedMessageBox();
                return;
            }

            //Turn on UI validation.
            _blnValidate = true;

            //Validate all properties
            ValidateAllProperties();

            //Turn off UI validation
            _blnValidate = false;

            //If any errors, show them.
            if (HasErrors)
            {
                AppRegionManager.ShowMessageBox(Error, 5);
                return;
            }

            if(_entity != null && _entity.SDReaders.Any() && !_entity.SDReaders.Where(x => x.primaryReader).Any() && _entity.IsYearlyReadingEventType)
            {
                if(AppRegionManager.ShowMessageBox(Translate("AddEditSDEventView", "NoPrimaryReaderAssignedWarning"), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                    return;
            }

            if (_entity != null && _entity.DefaultImageFoldersArray.Length == 0)
            {
                if (AppRegionManager.ShowMessageBox(Translate("AddEditSDEventView", "NoImageFoldersAddedWarning"), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                    return;
            }

            IsLoading = true;
            Task.Factory.StartNew(Save).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        private void Save()
        {
            try
            {
                var man = new SmartDotsManager();

                //Hardcode getting the one purpose available (AgeReading). In the future this will be shown as a drop down list in the UI to choose from, but right now, there is only one entry. 
                var purpose = _lstPurposes.FirstOrDefault();

                //Is this a new event or alteration of an existing event.
                bool isNew = _entity.ChangeTracker.State == ObjectState.Added;

                //When Event type is Års aflæsning, name should be made up from the system based on species, year and area (stock).
                if (SelectedEventType != null && SDEvent.IsEventTypeYearlyReading(SelectedEventType.L_sdEventTypeId))
                {
                    var areas = GetSelectedAreas();
                    EventName = ((Year != null ? Year.Value.ToString() : "") + " " + (SelectedSpecies != null ? SelectedSpecies.speciesCode : "") + " " + ((areas != null && areas.Count > 0) ? string.Join(",", areas.Select(x => x.DFUArea)) : "" ) ).Trim();
                }

                //Set end date to null if not mandatory and not enabled
                if (SelectedEventType != null && !SDEvent.IsEventTypeYearlyReading(SelectedEventType.L_sdEventTypeId) && !IsEndDateEnabled)
                    _entity.endDate = null;

                //entity is new, assign the purpose.
                if (isNew)
                {
                    _entity.createdTime = DateTime.UtcNow;
                    _entity.createdById = DFUPersonLogin == null ? null : new Nullable<int>(DFUPersonLogin.dfuPersonId);
                    _entity.createdByUserName = User.UserName;
                    _entity.sdEventGuid = Guid.NewGuid();

                    if (purpose != null)
                        _entity.sdPurposeId = purpose.L_sdPurposeId;
                }

                //Assign selected DFU areas 
                _entity.L_DFUAreas.Clear();
                var areasToAdd = AreasList.Where(x => x.IsChecked).Select(x => x.Tag).ToList();
                if (areasToAdd.Count > 0)
                    _entity.L_DFUAreas.AddRange(areasToAdd);

                _entity.SDFileExtraColumns = SDFileExtraColumns.Where(x => x.IsChecked).Select(x => x.Tag).ToList();

                //Save event to database.
                var res = man.SaveSDEvent(ref _entity);

                //If unsuccessful, show error message.
                if(res.DatabaseOperationStatus != Entities.DatabaseOperationStatus.Successful)
                {
                    LogAndDispatchUnexpectedErrorMessage(res.Message);
                    return;
                }

                //On success, replace the event in the events list (so it is not needed to be reloaded from the database) and either return to the events list or the samples list, based on where this view was loaded from.
                new Action(() =>
                {
                    //Add event to events view model.
                    //check why this is null
                    _vmEvents.AddOrReplaceEvent(_entity);

                    if (isNew)
                    {
                        var vm = new SDSamplesViewModel(_vmEvents, _entity);
                        vm.InitializeAsync(false);

                        //Load samples view for the newly created event.
                        _vmEvents.ActivateViewModel(vm);
                    }
                    else
                    {
                        //Close view and return to events list or samples list, depending on where this view was loaded from. 
                        Return(false);
                    }
                }).Dispatch();
            }
            catch(Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }

        #endregion


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
        /// Cancel any alterations or the new event (depending on IsEdit mode).
        /// </summary>
        private void Return(bool showMessage = true)
        {

            if (showMessage && HasChanges())
            {
                if (AppRegionManager.ShowMessageBox(Translate("AddEditSDEventView", "HasChangesWarning"), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                    return;
            }

            //If this view was opened from the samples view model, return to that. Otherwise return to the events list
            if (_vmReturnTo is SDSamplesViewModel)
            {
                _vmEvents.ActivateViewModel(_vmReturnTo);
            }
            else
            {
                //Return to events list.
                _vmEvents.CloseActiveViewModel();
            }
        }


        #endregion


        #region Delete Command


        public DelegateCommand DeleteCommand
        {
            get
            {
                if (_cmdDelete == null)
                    _cmdDelete = new DelegateCommand(() => DeleteAsync());

                return _cmdDelete;
            }
        }


        /// <summary>
        /// Delete event with samples asynchronously.
        /// </summary>
        private void DeleteAsync()
        {
            if (!User.HasAddEditSDEventsAndSamplesTask)
            {
                DispatchAccessDeniedMessageBox();
                return;
            }

            if (_entity != null && _entity.SamplesCount > 0)
            {
                AppRegionManager.ShowMessageBox(Translate("AddEditSDEventView", "DeleteEventSamplesError"));
                return;
            }

            if (AppRegionManager.ShowMessageBox(Translate("AddEditSDEventView", "DeleteEventWarning"), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;

            if (_entity == null)
            {
                LogAndDispatchUnexpectedErrorMessage("Internal error, _entity was NULL and could not be deleted.");
                return;
            }

            IsLoading = true;

            //Mark entity as deleted on main thread, so any collectionview problems is overcome.
            _entity.MarkAsDeleted();

            Task.Factory.StartNew(Delete).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        private void Delete()
        {
            try
            {
                var man = new SmartDotsManager();
                var res = man.SaveSDEvent(ref _entity);

                if (res.DatabaseOperationStatus != Entities.DatabaseOperationStatus.Successful)
                {
                    LogAndDispatchUnexpectedErrorMessage(res.Message);
                    return;
                }

                _vmEvents.RemoveEvent(_entity);

                _vmEvents.CloseActiveViewModel();

                DispatchMessageBox(Translate("AddEditSDEventView", "InfoMessageEventDeleted"), 3);
            }
            catch(Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }


        #endregion


        #region Add User Command


        public DelegateCommand AddUserCommand
        {
            get
            {
                if (_cmdAddUser == null)
                    _cmdAddUser = new DelegateCommand(() => AddUser());

                return _cmdAddUser;
            }
        }


        /// <summary>
        /// Add a new age reading to the event.
        /// </summary>
        private void AddUser()
        {
            //AccessibleUsers.Add(new AccessibleUserItem(_lstUsers, null));
            try
            {
                var vm = new SmartDots.SelectSDReadersViewModel(this);
                vm.InitializeAsync();
                AppRegionManager.LoadWindowViewFromViewModel(vm, true, "WindowWithBorderStyle");
                if (vm != null && !vm.IsCanceled && vm.SelectedSDReaderItemList != null && vm.SelectedSDReaderItemList.Count > 0)
                {
                   
                    foreach (var selectedItem in vm.SelectedSDReaderItemList)
                    {
                        SDReaderList.Add(new R_SDEventSDReader() { SDEvent = _entity, SDReader = selectedItem.SDReader, primaryReader = false });
                    }

                    RaisePropertyChanged(() => HasSDReaders);
                }
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }

        }


        #endregion


        #region Remove User Command


        public DelegateCommand<R_SDEventSDReader> RemoveUserCommand
        {
            get
            {
                if (_cmdDeleteReader == null)
                    _cmdDeleteReader = new DelegateCommand<R_SDEventSDReader>(u => RemoveUser(u));

                return _cmdDeleteReader;
            }
        }


        /// <summary>
        /// Remove age reader from the event.
        /// </summary>

        private void RemoveUser(R_SDEventSDReader reader)
        {
            if (SDReaderList.Contains(reader))
                SDReaderList.Remove(reader);

            RaisePropertyChanged(() => HasSDReaders);
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


        #region AddImageFoldersCommand



        public DelegateCommand AddImageFoldersCommand
        {
            get { return _cmdAddImageFolders ?? (_cmdAddImageFolders = new DelegateCommand(AddImageFolders)); }
        }


        public void AddImageFolders()
        {
            try
            {
                var vm = new SelectFoldersOrFilesViewModel(true, false, false, DefaultImageFolders);
                vm.WindowWidth = 600;
                vm.WindowHeight = 400;
                vm.WindowTitle = Translate("AddEditSDEventView", "SelectFoldersHeader"); //"Select folders";

                AppRegionManager.LoadWindowViewFromViewModel(vm, true, "StandardWindowWithBorderStyle");

                //Assign selection, unless Cancel was clicked.
                if (!vm.IsCancelled)
                    DefaultImageFolders = vm.GetSelectedFolders();
            }
            catch(Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }


        #endregion


        private bool HasChanges()
        {

            if (_originalEntity != null && _entity != null && _entity.HasEntityValueTypeChanges(_originalEntity))
                return true;

            return false;
        }
    }
}
