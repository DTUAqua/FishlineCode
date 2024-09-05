using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anchor.Core;
using Microsoft.Practices.Prism.Commands;
using Babelfisk.BusinessLogic.SIS;
using Babelfisk.Entities.Sprattus;
using Babelfisk.BusinessLogic.DataInput;
using System.ServiceModel;
using System.Threading;
using Babelfisk.BusinessLogic.SIS.Model;
using Babelfisk.ViewModels.Input;
using Babelfisk.BusinessLogic;

namespace Babelfisk.ViewModels.Import
{
    public class ImportStationViewModel : AViewModel
    {
        private DelegateCommand _cmdImport;
        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdConnect;


        private int? _intSelectedTripId;

        private bool _blnIsConnected;

        private bool _blnIsConnecting;

        private int _intSelectedYear;

        private List<int> _lstYears;

        private List<CruiseInformation> _lstCruiseInformations;

        private CruiseInformation _selectedCruiseInformation;

        private List<GearData> _lstGearData;

        private GearData _selectedGearData;

        private Trip _trip;

        private static CruiseInformation _previousSelectedCruiseInformation = null;
        private static GearData _previousSelectedGearData = null;

        private bool _blnIsLoadingYears = false;

        private bool _blnIsLoadingCruiseInformation = false;

        private bool _blnIsLoadingGearData = false;

        private string _strStationNumber;

        private List<L_GearType> _lstGearTypes;
        private List<L_TimeZone> _lstTimeZones;

        private string _strGearType;
        private int? _intTimeZone;

        private bool _blnUpdatingCollections;


        #region Properties


        public List<L_GearType> GearTypes
        {
            get
            {
                return _lstGearTypes == null ? null :  _lstGearTypes.Where(x => x.catchOperation.Equals("A", StringComparison.InvariantCultureIgnoreCase) && x.showInVidUI).ToList();
            }
            set
            {
                _lstGearTypes = value;
                RaisePropertyChanged(() => GearTypes);
            }
        }


        public List<L_TimeZone> TimeZones
        {
            get { return _lstTimeZones; }
            set
            {
                _lstTimeZones = value;
                RaisePropertyChanged(() => TimeZones);
            }
        }



        public L_GearType SelectedGearType
        {
            get { return (_strGearType == null || _lstGearTypes == null) ? null : _lstGearTypes.Where(x => x.gearType == _strGearType).FirstOrDefault(); }
            set
            {
                if (!_blnUpdatingCollections)
                {
                    var val = (value == null ? null : value.gearType);
                    if (_strGearType != val)
                        _strGearType = val;
                }

                RaisePropertyChanged(() => SelectedGearType);
            }
        }



        public L_TimeZone SelectedTimeZone
        {
            get { return (_intTimeZone == null || _lstTimeZones == null) ? null : _lstTimeZones.Where(x => x.timeZone == _intTimeZone).FirstOrDefault(); }
            set
            {
                if (!_blnUpdatingCollections)
                {
                    if (value == null || (_intTimeZone != value.timeZone))
                        _intTimeZone = value == null ? new Nullable<int>() : value.timeZone;
                }

                RaisePropertyChanged(() => SelectedTimeZone);
            }
        }



        public string StationNumber
        {
            get { return _strStationNumber; }
            set
            {
                if (_strStationNumber != value)
                    _strStationNumber = value;
                RaisePropertyChanged(() => StationNumber);
            }
        }


        public List<int> Years
        {
            get { return _lstYears; }
            set
            {
                _lstYears = value ;
                RaisePropertyChanged(() => Years);
                RaisePropertyChanged(() => SelectedCruiseInformation);
                RaisePropertyChanged(() => SelectedGearData);
                RaisePropertyChanged(() => IsYearSelected);
                RaisePropertyChanged(() => IsCruiseInformationSelected);
            }
        }



        public List<CruiseInformation> CruiseInformations
        {
            get { return _lstCruiseInformations; }
            set
            {
                _lstCruiseInformations = value;
                RaisePropertyChanged(() => CruiseInformations);
                RaisePropertyChanged(() => SelectedGearData);
                RaisePropertyChanged(() => IsCruiseInformationSelected);
            }
        }


        public List<GearData> GearData
        {
            get { return _lstGearData; }
            set
            {
                _lstGearData = value;
                RaisePropertyChanged(() => GearData);
            }
        }




        public int SelectedYear
        {
            get { return _intSelectedYear; }
            set
            {
                _intSelectedYear = value;

                InitializeSISCruisesAsync();
                RaisePropertyChanged(() => SelectedYear);
                RaisePropertyChanged(() => IsYearSelected);
            }
        }


        public bool IsYearSelected
        {
            get { return _intSelectedYear > -1; }
        }



        public CruiseInformation SelectedCruiseInformation
        {
            get { return _selectedCruiseInformation; }
            set
            {
                _selectedCruiseInformation = value;

                InitializeSISGearDataAsync();
                RaisePropertyChanged(() => SelectedCruiseInformation);
                RaisePropertyChanged(() => IsCruiseInformationSelected);
            }
        }


        public bool IsCruiseInformationSelected
        {
            get { return _selectedCruiseInformation != null; }
        }


        public GearData SelectedGearData
        {
            get { return _selectedGearData; }
            set
            {
                _selectedGearData = value;
                RaisePropertyChanged(() => SelectedGearData);
                RaisePropertyChanged(() => IsGearDataSelected);
            }
        }


        public bool IsGearDataSelected
        {
            get { return _selectedGearData != null; }
        }


        public bool IsConnected
        {
            get { return _blnIsConnected; }
            set
            {
                _blnIsConnected = value;
                RaisePropertyChanged(() => IsConnected);
            }
        }


        public bool IsConnecting
        {
            get { return _blnIsConnecting; }
            set
            {
                _blnIsConnecting = value;
                RaisePropertyChanged(() => IsConnecting);
                RaisePropertyChanged(() => CanConnect);
            }
        }


        public bool CanConnect
        {
            get { return SelectedServer != null && !IsConnecting; }
        }


        public List<BusinessLogic.Settings.ServerName> ServerNames
        {
            get { return BusinessLogic.Settings.Settings.Instance.ServerNames.Names; }
        }


        public BusinessLogic.Settings.ServerName SelectedServer
        {
            get { return BusinessLogic.Settings.Settings.Instance.ServerNames.SelectedServerName; }
            set
            {
                BusinessLogic.Settings.Settings.Instance.ServerNames.SelectedServerName = value;
                
                IsConnected = false;
                RaisePropertyChanged(() => SelectedServer);
                RaisePropertyChanged(() => CanConnect);
                ConnectAsync();
            }
        }


        public bool IsLoadingYears
        {
            get { return _blnIsLoadingYears; }
            set
            {
                _blnIsLoadingYears = value;
                RaisePropertyChanged(() => IsLoadingYears);
            }
        }


        public bool IsLoadingCruiseInformation
        {
            get { return _blnIsLoadingCruiseInformation; }
            set
            {
                _blnIsLoadingCruiseInformation = value;
                RaisePropertyChanged(() => IsLoadingCruiseInformation);
            }
        }


        public bool IsLoadingGearData
        {
            get { return _blnIsLoadingGearData; }
            set
            {
                _blnIsLoadingGearData = value;
                RaisePropertyChanged(() => IsLoadingGearData);
            }
        }


        public GearData PreviousSelectedGearData
        {
            get { return _previousSelectedGearData; }
            set
            {
                _previousSelectedGearData = value;
                RaisePropertyChanged(() => PreviousSelectedGearData);
                RaisePropertyChanged(() => HasPreviousSelectedGearData);
            }
        }


        public bool HasPreviousSelectedGearData
        {
            get { return _previousSelectedGearData != null; }
        }


        #endregion


        public ImportStationViewModel(int? intTripId = null)
        {
            _intSelectedTripId = intTripId;

            WindowWidth = 600;
            WindowHeight = 500;
            WindowTitle = "Importer station fra SIS";
            _intSelectedYear = -1;

            InitializeAsync();
        }


        private void InitializeAsync()
        {
            IsLoading = true;
            Task.Factory.StartNew(Initialize).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }

        private void Initialize()
        {
            try
            {
                var man = new DataInputManager();
                var lookupMan = new BusinessLogic.LookupManager();

                var t = man.GetEntity<Trip>(_intSelectedTripId.Value, "Cruise");

                var lv = new LookupDataVersioning();
                var lstGearTypes = lookupMan.GetLookups(typeof(L_GearType), lv).OfType<L_GearType>().OrderBy(x => x.UIDisplay).ToList();
                var lstTimeZones = lookupMan.GetLookups(typeof(L_TimeZone), lv).OfType<L_TimeZone>().OrderBy(x => x.timeZone).ToList();

                new Action(() =>
                {
                    _blnUpdatingCollections = true;
                    {
                        GearTypes = lstGearTypes;
                        TimeZones = lstTimeZones;
                    }
                    _blnUpdatingCollections = false;

                    _trip = t;

                    //Select UTC timezone as default
                    SelectedTimeZone = lstTimeZones.Where(x => x.timeZone == 0).FirstOrDefault();

                    //Continue with connecting to default server
                    ConnectAsync();
                }).Dispatch();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                if (e is TimeoutException || e is EndpointNotFoundException)
                    DispatchMessageBox("Det var ikke muligt at oprette forbindelse til databasen.");
                else
                    DispatchMessageBox("En uventet fejl opstod. " + e.Message);
            }
        }


        private void InitializeSISYearsAsync()
        {
            IsLoadingYears = true;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    SISDataManager sisMan = new SISDataManager(SelectedServer.EFConnectionString);

                    var lstYears = sisMan.GetCruiseYears();

                    if (lstYears != null)
                        lstYears = lstYears.OrderByDescending(x => x).ToList();

                    new Action(() =>
                    {
                        _selectedCruiseInformation = _previousSelectedCruiseInformation;
                        _selectedGearData = null;
                        Years = lstYears;

                        int intYear = _trip.Cruise.year;

                        SelectedYear = Years.Where(x => x == intYear).Any() ? intYear : -1;

                        //Make sure no cruise information is selected if year is -1
                        if (SelectedYear == -1)
                            SelectedCruiseInformation = null;

                        _previousSelectedCruiseInformation = null;
                    }).Dispatch();
                }
                catch (Exception e)
                {
                    Anchor.Core.Loggers.Logger.LogError(e);
                    DispatchMessageBox("En uventet fejl opstod. " + e.Message);
                }
            }).ContinueWith(t => new Action(() => IsLoadingYears = false).Dispatch());
        }


        private void InitializeSISCruisesAsync()
        {
            IsLoadingCruiseInformation = true;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    List<CruiseInformation> lst = null;

                    SISDataManager sisMan = new SISDataManager(SelectedServer.EFConnectionString);
                    lst = sisMan.GetCruiseInformations(SelectedYear);

                    if (lst != null)
                        lst = lst.OrderByDescending(x => x.cruiseNo).ToList();

                    new Action(() =>
                    {
                        _selectedGearData = null;

                        var prev = _selectedCruiseInformation;
                        CruiseInformations = lst;

                        if (prev != null)
                        {
                            SelectedCruiseInformation = lst.Where(x => x.cruiseID == prev.cruiseID).FirstOrDefault();
                        }

                        //Reset previous imported gear data, if it does not fit with the cruise id
                        if (SelectedCruiseInformation == null ||
                          (_previousSelectedGearData != null && _previousSelectedGearData.cruiseID != SelectedCruiseInformation.cruiseID))
                            PreviousSelectedGearData = null;
                    }).Dispatch();
                }
                catch (Exception e)
                {
                    Anchor.Core.Loggers.Logger.LogError(e);
                    DispatchMessageBox("En uventet fejl opstod. " + e.Message);
                }
            }).ContinueWith(t => new Action(() => IsLoadingCruiseInformation = false).Dispatch());
        }


        private void InitializeSISGearDataAsync()
        {
            IsLoadingGearData = true;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    List<GearData> lst = null;

                    SISDataManager sisMan = new SISDataManager(SelectedServer.EFConnectionString);

                    if (SelectedCruiseInformation != null)
                    {
                        lst = sisMan.GetGearData(SelectedCruiseInformation.cruiseID);
                        if (lst != null)
                            lst = lst.Where(x => x.haulNo != null).OrderByDescending(x => x.haulNo).ToList();
                    }

                    new Action(() =>
                    {
                        GearData = lst;
                    }).Dispatch();
                }
                catch (Exception e)
                {
                    DispatchMessageBox("En uventet fejl opstod. " + e.Message);
                }
            }).ContinueWith(t => new Action(() => IsLoadingGearData = false).Dispatch());
        }



        /// <summary>
        /// Test if there is a connection to the selected server.
        /// </summary>
        private static bool IsServerConnected(string connectionString)
        {
            //Connect to server (with a timeout as specified below). 
            using (SqlConnection con = new SqlConnection(connectionString + "Connection Timeout=10;"))
            {
                try
                {
                    con.Open();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
                finally
                {
                }
            }
        }


        #region Import command


        public DelegateCommand ImportCommand
        {
            get { return _cmdImport ?? (_cmdImport = new DelegateCommand(ImportAsync)); }
        }


        private void ImportAsync()
        {
            if (SelectedGearData == null)
            {
                DispatchMessageBox("Vælg venligst togt-år, togtnummer og træknummer.");
                return;
            }

            if (!IsValid())
                return;

            IsLoading = true;
            Task.Factory.StartNew(Import).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        private bool IsValid()
        {
            bool blnValid = true;

            if (string.IsNullOrEmpty(StationNumber))
            {
                blnValid = false;
                AppRegionManager.ShowMessageBox("Angiv venligst stations nr.");
            }
            else if (StationNumber.Length > 6)
            {
                blnValid = false;
                AppRegionManager.ShowMessageBox(string.Format("Stations nr. må kun bestå af 6 tegn. Det består pt. af {0} tegn.", StationNumber.Length));
            }
            else if (SelectedTimeZone == null)
            {
                blnValid = false;
                AppRegionManager.ShowMessageBox("Angiv venligst en tidszone.");
            }

            return blnValid;
        }

        private void Import()
        {
            var datMan = new DataRetrievalManager();

            List<Sample> lst = datMan.GetTreeViewSamples(_trip.tripId);

            if (lst != null && lst.Count > 0)
            {
                if (lst.Where(x => x.station.Equals(StationNumber, StringComparison.InvariantCultureIgnoreCase)).Any())
                {
                    DispatchMessageBox(String.Format("En station med nummer '{0}' eksisterer allerede, angiv venligst et andet stations nr.", StationNumber));
                    return;
                }
            }

            SISDataManager sisMan = new SISDataManager(SelectedServer.EFConnectionString);

            Sample s = null;
            try
            {
                s = sisMan.GetStation(SelectedGearData.gearDataID);
            }
            catch (Exception e)
            {
                DispatchMessageBox("Det var ikke muligt at importere det valgte slæbnummer. " + e.Message);
                return;
            }

            if (s == null)
            {
                DispatchMessageBox("Det var ikke muligt at importere det valgte slæbnummer, prøv igen.");
                return;
            }

            s.tripId = _trip.tripId;
            if(AppRegionManager.User != null)
                s.datahandlerId =  AppRegionManager.User.UserId;

            s.station = StationNumber;

            if (SelectedTimeZone != null)
                s.timeZone = SelectedTimeZone.timeZone;

            if(SelectedGearType != null)
                s.gearType = SelectedGearType.gearType;

            BusinessLogic.Settings.Settings.Instance.Save();

            _previousSelectedCruiseInformation = SelectedCruiseInformation;
            _previousSelectedGearData = SelectedGearData;

            new Action(() =>
            {
                StationViewModel svm = StationViewModel.NewViewModelFromImportedStation(s);
                _appRegionManager.LoadViewFromViewModel(RegionName.MainRegion, svm);


                Close();
            }).Dispatch();
        }

        #endregion


        #region Connect command


        public DelegateCommand ConnectCommand
        {
            get { return _cmdConnect ?? (_cmdConnect = new DelegateCommand(ConnectAsync)); }
        }


        private Task _conTask = null;
        private CancellationToken _conToken;
        private CancellationTokenSource _conTokenSource = null;

        private void ConnectAsync()
        {
            if (SelectedServer == null)
                return;

            //Wait for previous task to complete.
            if (_conTask != null)
            {
                if(_conTokenSource != null)
                    _conTokenSource.Cancel();
                _conTask.Wait();
            }

            _conTokenSource = new CancellationTokenSource();
            _conToken = _conTokenSource.Token;

            IsConnecting = true;
            _conTask = Task.Factory.StartNew(Connect, _conToken).ContinueWith(t => new Action(() => IsConnecting = false).Dispatch());
        }


        private void Connect()
        {
            bool blnIsConnected = IsServerConnected(SelectedServer.ConnectionString);

            if (_conToken != null && _conToken.IsCancellationRequested)
                return;

            new Action(() =>
            {
                IsConnected = blnIsConnected;
                if (blnIsConnected)
                    InitializeSISYearsAsync();
                else
                {
                    //Show message, if not closed
                    if(!_blnIsClosed)
                        AppRegionManager.ShowMessageBox("Det var ikke muligt at forbinde til den valgte SIS-database, prøv igen.");
                }
            }).Dispatch();
        }


        #endregion


        #region Cancel command


        public DelegateCommand CancelCommand
        {
            get { return _cmdCancel ?? (_cmdCancel = new DelegateCommand(Cancel)); }
        }

        private bool _blnIsClosed = false;
        private void Cancel()
        {
            Close();
            _blnIsClosed = true;
        }

        #endregion
    }
}
