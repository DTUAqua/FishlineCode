using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Babelfisk.BusinessLogic.DataInput;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using Microsoft.Practices.Prism.Commands;
using Babelfisk.Entities;
using System.Windows.Input;
using System.ServiceModel;
using Babelfisk.ViewModels.Lookup;

namespace Babelfisk.ViewModels.Input
{
    public class CruiseViewModel : AInputViewModel
    {
        /// <summary>
        /// Button commands
        /// </summary>
        private DelegateCommand _cmdAddEditProjectLeaders;

        private DelegateCommand<string> _cmdNewTrip;

        /// <summary>
        /// Reference to the year a new Cruise entity should be added to by default (only used during view model initialization)
        /// </summary>
        private int? _intPreselectedYear = null;


        /// <summary>
        /// List of existing years
        /// </summary>
        private List<L_Year> _lstYears;


        /// <summary>
        /// List of currently used cruise names
        /// </summary>
        private List<string> _lstCruiseNames;


        /// <summary>
        /// List of project leaders (DFUPerson entity objects)
        /// </summary>
        private List<DFUPerson> _lstDFUPersons;


        /// <summary>
        /// Reference to current cruise entity being edited.
        /// </summary>
        private Cruise _cruise;


        /// <summary>
        /// Reference to the cruiseid currently being edited. (only used during view model initialization)
        /// </summary>
        private int? _intEditingCruiseId = null;


        /// <summary>
        /// Variables for setting focus on specific UI controls
        /// </summary>
        private bool _blnIsCruiseTitleFocused;
        private bool _blnIsCruiseYearFocused;


        /// <summary>
        /// Member variable used for turning UI validation on and off.
        /// </summary>
        private bool _blnValidate = false;


        private bool _blnUpdatingCollections = false;


        public bool HasEditingRights
        {
            get
            {
                if (User == null || User.Role == null || User.Role.Count == 0) // User.Role.Role1 == null
                    return false;

                return User.IsAdmin || User.IsEditor;
            }
        }


        #region Properties

        public override bool HasUnsavedData
        {
            get
            {
                if (!HasEditingRights || !CanEditOffline)
                    return false;

                return _cruise != null && _cruise.ChangeTracker.State != ObjectState.Unchanged;
            }
        }


        public Cruise Cruise
        {
            get { return _cruise; }
            set
            {
                _cruise = value;
                RefreshAllNotifiableProperties();
            }
        }


        public List<L_Year> Years
        {
            get { return _lstYears; }
            set
            {
                _lstYears = value;
                RaisePropertyChanged(() => Years);
            }
        }


        /// <summary>
        /// Project leaders.
        /// </summary>
        public List<DFUPerson> DFUPersons
        {
            get { return _lstDFUPersons; }
            set
            {
                _lstDFUPersons = value;
                RaisePropertyChanged(() => DFUPersons);
            }
        }


        public List<string> CruiseNames
        {
            get { return _lstCruiseNames; }
            set
            {
                _lstCruiseNames = value;
                RaisePropertyChanged(() => CruiseNames);
            }
        }


        public int? SelectedYear
        {
            get { return _cruise != null ? _cruise.year : new Nullable<int>(); }
            set
            {
                if (_cruise.year != (value.HasValue ? value.Value : 0))
                    _cruise.year = value.HasValue ? value.Value : 0;
                RaisePropertyChanged(() => SelectedYear);
            }
        }


        public string SelectedTitle
        {
            get { return _cruise != null ? _cruise.cruise1 : null; }
            set
            {
                if(_cruise.cruise1 != value)
                    _cruise.cruise1 = value;
                RaisePropertyChanged(() => SelectedTitle);
            }
        }


        /// <summary>
        /// Selected description consists of cruiseTitle and summary concatenated (for old trips).  
        /// For new trips, cruiseTitle will be null and everything put into summary.
        /// </summary>
        public string SelectedDescription
        {
            get 
            {
                if (_cruise == null)
                    return null;

                string str = _cruise.cruiseTitle;
                str += _cruise.summary == null ? "" : (str == null ? "" : " ") + _cruise.summary;

                return str; 
            }
            set
            {
                //Save cruisetitle and summary in the summary field (thereby omitting cruise title)
                if (_cruise.summary != value)
                {
                    _cruise.cruiseTitle = null;
                    _cruise.summary = value;
                }
                RaisePropertyChanged(() => SelectedDescription);
            }
        }


        public DFUPerson SelectedProjectLeader
        {
            get { return _cruise != null ? _cruise.DFUPerson1 : null; }
            set
            {
                if (!_blnUpdatingCollections)
                {
                    if (_cruise.DFUPerson1 != value)
                        _cruise.DFUPerson1 = value;
                }
                RaisePropertyChanged(() => SelectedProjectLeader);
            }
        }


        public string Participants
        {
            get { return _cruise != null ? _cruise.participants : null; }
            set
            {
                if(_cruise.participants != value)
                    _cruise.participants = value;
                RaisePropertyChanged(() => Participants);
            }
        }


        public string Remark
        {
            get { return _cruise != null ? _cruise.remark : null; }
            set
            {
                if(_cruise.remark != value)
                    _cruise.remark = value;
                RaisePropertyChanged(() => Remark);
            }
        }


        public bool IsCruiseTitleFocused
        {
            get { return _blnIsCruiseTitleFocused; }
            set
            {
                _blnIsCruiseTitleFocused = value;
                RaisePropertyChanged(() => IsCruiseTitleFocused);
            }
        }


        public bool IsCruiseYearFocused
        {
            get { return _blnIsCruiseYearFocused; }
            set
            {
                _blnIsCruiseYearFocused = value;
                RaisePropertyChanged(() => IsCruiseYearFocused);
            }
        }


        public override bool CanEditOffline
        {
            get
            {
                return base.CanEditOffline || !IsEdit || (IsLoading || (_cruise != null && _cruise.OfflineState == ObjectState.Added));
            }
        }


        #endregion


        /// <summary>
        /// Cruise constructor.
        /// </summary>
        public CruiseViewModel(int? intCruiseId = null, int? intYear = null)
            : base()
        {
            //Initialize year text box with a default value.
            _intPreselectedYear = intYear;

            //If a cruise is being edited, cruise id has a value.
            _intEditingCruiseId = intCruiseId; 
            IsEdit = intCruiseId.HasValue;
            
            InitializeAsync();
            RegisterToKeyDown();
        }


        #region Initialize methods


        private void InitializeAsync()
        {
            IsLoading = true;
            Task.Factory.StartNew(Initialize).ContinueWith(t => new Action(() => { IsLoading = false; RaisePropertyChanged(() => CanEditOffline); }).Dispatch());
            
            if (IsEdit)
                Task.Factory.StartNew(() => InitializeMap(_intEditingCruiseId, TreeNodeLevel.Cruise));
        }


        private void Initialize()
        {
            try
            {
                var man = new DataInputManager();

                Cruise c = null;

                if (IsEdit)
                {
                    c = man.GetEntity<Cruise>(_intEditingCruiseId.Value);
                    if (c == null)
                    {
                        DispatchMessageBox("Det var ikke muligt at hente det ønskede togt.");
                        return;
                    }
                }
                else
                {
                    c = new Cruise();
                    c.year = _intPreselectedYear.HasValue ? _intPreselectedYear.Value : DateTime.Now.Year;
                }

                //Grab earlier cruise years
                var lstYears = man.GetYears();

                //Grab earlier cruise titles
                var lstCruiseNames = man.GetCruiseNames();

                var lstDFUPersons = new BusinessLogic.LookupManager().GetLookups(typeof(DFUPerson)).OfType<DFUPerson>().OrderBy(x => x.initials).ToList();

                new Action(() =>
                {
                    //First set lookup lists
                    Years = lstYears;
                    CruiseNames = lstCruiseNames;
                    DFUPersons = lstDFUPersons;

                    //Assign cruise (it's important the cruise is assigned after the lookup lists (or the selected values on cruise will be overwritten with null).
                    Cruise = c;

                    AssignSelectedLookups();

                    //Set UI Focus
                    if (!IsEdit)
                    {
                        if (_intPreselectedYear.HasValue)
                            IsCruiseTitleFocused = true;
                        else
                            IsCruiseYearFocused = true;
                    }
                    else if (MapViewModel != null)
                        MapViewModel.WindowTitle = string.Format("Togt: {0}", c.cruise1);


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


        private void AssignSelectedLookups()
        {
            if (_cruise.responsibleId.HasValue && _lstDFUPersons != null)
                SelectedProjectLeader = _lstDFUPersons.Where(x => x.dfuPersonId == _cruise.responsibleId.Value).FirstOrDefault();
        }


        #endregion



        /// <summary>
        /// Validates property with name strFieldName. This method is overriding a base method which is called
        /// whenever a property is changed.
        /// </summary>
        protected override string ValidateField(string strFieldName)
        {
            //Only perform validation when user clicks "Save".
            //if (_blnValidate)
            {
                switch (strFieldName)
                {
                    case "SelectedYear":
                        if (SelectedYear < 1800)
                            return "Angiv venligst et år senere end 1800";
                        break;

                    case "SelectedTitle":
                        //Only perform mandatory null check, when clicking save
                        if (_blnValidate && string.IsNullOrEmpty(SelectedTitle))
                            return "Angiv venligst togt titel";

                        if (SelectedTitle != null && SelectedTitle.Length > 20)
                            return string.Format("Togt titel må kun bestå af 20 tegn. Den består pt. af {0} tegn.", SelectedTitle.Length);
                        break;

                    case "SelectedDescription":
                        if (!string.IsNullOrWhiteSpace(SelectedDescription) && SelectedDescription.Length > 1024)
                            return string.Format("Togt beskrivelse må kun bestå af 1024 tegn. Den består pt. af {0} tegn.", SelectedDescription.Length);
                        break;

                    case "Participants":
                        if (!string.IsNullOrEmpty(Participants) && Participants.Length > 256)
                            return string.Format("Deltager(e) må kun bestå af 256 tegn. Den består pt. af {0} tegn.", Participants.Length);
                        break;

                    case "Remark":
                        //Remark cannot be too long.
                        break;
                }
            }

            return null;
        }



        #region Save Methods


        /// <summary>
        /// Validate and save changes to database.
        /// </summary>
        protected override void ValidateAndSaveAsync()
        {
            if (!HasEditingRights)
            {
                AppRegionManager.ShowMessageBox("Du har ikke rettigheder til at gemme eventuelle ændringer.");
                return;
            }

            //Turn on UI validation (this will also show fire an error on the UI controls causing errors).
            _blnValidate = true;

            //Validate all properties
            ValidateAllProperties();

            //Turn off UI validation
            _blnValidate = false;

            if (HasErrors)
            {
                AppRegionManager.ShowMessageBox(Error, 5);
                return;
            }
             
            IsLoading = true;
            Task.Factory.StartNew(Save).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        /// <summary>
        /// Save changes to database. This method does not validate any fields (this should be done prior to calling the method).
        /// </summary>
        private void Save()
        {
            try
            {
                var man = new DataInputManager();
                DatabaseOperationResult res = man.SaveCruise(ref _cruise);

                //If saving cruise failed, show error message.
                if (res.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                {
                    if (res.DatabaseOperationStatus == DatabaseOperationStatus.ValidationError &&
                       res.Message == "DuplicateKey")
                        DispatchMessageBox(String.Format("En togt med titel '{0}' eksisterer allerede for år '{1}'.", _cruise.cruise1, _cruise.year));
                    else
                        DispatchMessageBox("En uventet fejl opstod. " + res.Message);

                    return;
                }

                new Action(() =>
                {
                    //Reset cruise changetracker.
                    _cruise.AcceptChanges();

                    //Set cruise in edit mode.
                    IsEdit = true;
                    _intEditingCruiseId = _cruise.cruiseId;

                }).Dispatch();

               

                //Call save succeeded event (this makes sure the tree is updated with any changes)
                RaiseSaveSucceeded();

                //Refresh tree
                MainTree.RefreshTreeAsync().ContinueWith(t =>
                {
                    MainTree.SelectTreeNode(_cruise.cruiseId, TreeNodeLevel.Cruise);
                });


                DispatchMessageBox("Togtet blev gemt korrekt.", 2);

                Babelfisk.ViewModels.Security.BackupManager.Instance.Backup();
            }
            catch (Exception e)
            {
                DispatchMessageBox(String.Format("En uventet fejl opstod. {0}", e.Message));
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }

        #endregion


        #region Close Methods

        protected override void CloseViewModel()
        {
            //Redirect to start view.
            Menu.MainMenuViewModel.ShowStart();
        }

        #endregion


        #region New Trip Command


        public DelegateCommand<string> NewTripCommand
        {
            get { return _cmdNewTrip ?? (_cmdNewTrip = new DelegateCommand<string>(tripType => NewTrip(tripType, _cruise.cruiseId))); }
        }


        public static void NewTrip(string strTripType, int intCruiseId)
        {
            TripViewModel t = null;
            switch (strTripType)
            {
                case "HVN":
                     var hvn = new TripHVNViewModel(null, intCruiseId);
                     _appRegionManager.LoadViewFromViewModel(RegionName.MainRegion, hvn);
                    break;

                case "VID":
                    t = new TripViewModel(null, intCruiseId, "VID");
                    _appRegionManager.LoadViewFromViewModel(RegionName.MainRegion, t);
                    break;

                case "SØS":
                    t = new TripViewModel(null, intCruiseId, "SØS");
                    _appRegionManager.LoadViewFromViewModel(RegionName.MainRegion, t);
                    break;

                case "REKTBD":
                    t = new TripViewModel(null, intCruiseId, "REKTBD");
                    _appRegionManager.LoadViewFromViewModel(RegionName.MainRegion, t);
                    break;

                case "REKOMR":
                    t = new TripViewModel(null, intCruiseId, "REKOMR");
                    _appRegionManager.LoadViewFromViewModel(RegionName.MainRegion, t);
                    break;

                case "REKHVN":
                    t = new TripViewModel(null, intCruiseId, "REKHVN");
                    _appRegionManager.LoadViewFromViewModel(RegionName.MainRegion, t);
                    break;
            }

            
        }


        #endregion


        #region Add/Edit Project Leaders Command


        public DelegateCommand AddEditProjectLeadersCommand
        {
            get { return _cmdAddEditProjectLeaders ?? (_cmdAddEditProjectLeaders = new DelegateCommand(AddEditProjectLeaders)); }
        }


        private void AddEditProjectLeaders()
        {
            if (!HasUserViewLookupRights())
                return;

            ViewModels.Lookup.LookupManagerViewModel lm = new Lookup.LookupManagerViewModel(typeof(DFUPerson));
            lm.Closed += lm_Closed;
            AppRegionManager.LoadWindowViewFromViewModel(lm, true, "WindowWithBorderStyle");
        }


        protected void lm_Closed(object arg1, AViewModel arg2)
        {
            if (arg2 is LookupManagerViewModel && !(arg2 as LookupManagerViewModel).ChangesSaved)
                return;

            IsLoading = true;
            //Reload project leaders drop down list (so any changes in the lookup manager are reflected).
            Task.Factory.StartNew(ReloadPersons).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        private void ReloadPersons()
        {
            try
            {
                var man = new DataInputManager();
                var lstDFUPersons = new BusinessLogic.LookupManager().GetLookups(typeof(DFUPerson)).OfType<DFUPerson>().OrderBy(x => x.initials).ToList();

                new Action(() =>
                {
                    _blnUpdatingCollections = true;
                    {
                        //Assign all DFU persons
                        DFUPersons = lstDFUPersons;
                    }
                    _blnUpdatingCollections = false;

                }).Dispatch();
            }
            catch (Exception e)
            {
                DispatchMessageBox("En uventet fejl opstod. " + e.Message);
            }
        }


        #endregion


        protected override void GlobelPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            bool b = Keyboard.IsKeyDown(Key.System | Key.LeftAlt);
            bool b1 = Keyboard.IsKeyDown(Key.System);
            bool b2 = Keyboard.IsKeyDown(Key.System & Key.LeftAlt);

            //Save on Ctrl + S
            if (HasEditingRights
                && ((e.Key == System.Windows.Input.Key.S && Keyboard.Modifiers == ModifierKeys.Control) || ((e.SystemKey == Key.G || e.Key == Key.G) && (Keyboard.Modifiers == ModifierKeys.Alt || Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt))))
                && (Cruise != null && Cruise.ChangeTracker.State != ObjectState.Unchanged))
                ValidateAndSaveAsync();
        }

    }
}
