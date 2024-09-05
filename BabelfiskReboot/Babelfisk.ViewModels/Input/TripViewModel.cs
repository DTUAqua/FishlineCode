using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using Anchor.Core;
using Babelfisk.BusinessLogic.DataInput;
using Babelfisk.Entities.Sprattus;
using Babelfisk.Entities;
using System.Windows.Input;
using System.ServiceModel;
using Babelfisk.BusinessLogic;
using Babelfisk.ViewModels.Lookup;

namespace Babelfisk.ViewModels.Input
{
    public class TripViewModel : AInputViewModel
    {
        private DelegateCommand _cmdShowCruise;
        private DelegateCommand _cmdNewStation;

        private DelegateCommand<string> _cmdAddEditLookups;

        private bool _blnUpdatingCollections = false;

        private int? _intEditingTripId;

        private int? _intCruiseId;

        private Trip _trip;

        private string _strTripType;


        /// <summary>
        /// Variables for setting focus on specific UI controls
        /// </summary>
        private bool _blnIsTripNumberFocused;

        private bool _blnIsRekSGTripIdFocused;

        private bool _blnIsCompanyFocused;

        private bool _blnIsContactPersonLoading;


        /// <summary>
        /// List of project leaders (DFUPerson entity objects)
        /// </summary>
        private List<DFUPerson> _lstDFUPersons;
        private List<L_SamplingMethod> _lstSamplingMethods;
        private List<L_TimeZone> _lstTimeZones;
        private List<L_Harbour> _lstHarbours;
        private List<L_Platform> _lstPlatforms;
        private List<L_FisheryType> _lstFisheryTypes;
        private List<Person> _lstPersons;


        private string _strCruiseTitle;

        private int? _intCruiseYear;

        private string _strContactPersonName;


        #region Properties

        public override bool CanEditOffline
        {
            get
            {
                return base.CanEditOffline || !IsEdit || (IsLoading || (_trip != null && _trip.OfflineState == ObjectState.Added));
            }
        }

        private bool UpdatingCollections
        {
            get { return _blnUpdatingCollections || _blnValidate; }
        }


        public bool HasEditingRights
        {
            get
            {
                if (User == null || User.Role == null || User.Role.Count == 0) // User.Role.Role1 == null
                    return false;

                return User.IsAdmin || User.IsEditor;
            }
        }


        public override bool HasUnsavedData
        {
            get
            {
                if (!HasEditingRights || !CanEditOffline)
                    return false;

                return _trip != null && (_trip.ChangeTracker.State != ObjectState.Unchanged || (SelectedContactPerson != null && SelectedContactPerson.ChangeTracker.State != ObjectState.Unchanged));
            }
        }


        public string CruiseTitle
        {
            get { return _strCruiseTitle; }
            set
            {
                _strCruiseTitle = value;
                RaisePropertyChanged(() => CruiseTitle);
            }
        }


        public int? CruiseYear
        {
            get { return _intCruiseYear; }
            set
            {
                _intCruiseYear = value;
                RaisePropertyChanged(() => CruiseYear);
            }
        }


        /// <summary>
        /// Project leaders.
        /// </summary>
        public List<DFUPerson> DFUPersons
        {
            get { return _lstDFUPersons == null ? null : _lstDFUPersons.ToList(); }
            set
            {
                _lstDFUPersons = value;
                RaisePropertyChanged(() => DFUPersons);
            }
        }


        public List<L_SamplingMethod> SamplingMethods
        {
            get { return _lstSamplingMethods; }
            set
            {
                _lstSamplingMethods = value;
                RaisePropertyChanged(() => SamplingMethods);
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


        public List<L_Harbour> Harbours
        {
            get { return _lstHarbours; }
            set
            {
                _lstHarbours = value;
                RaisePropertyChanged(() => Harbours);
            }
        }

        public List<L_Platform> Platforms
        {
            get { return _lstPlatforms == null ? null : _lstPlatforms.ToList(); }
            set
            {
                _lstPlatforms = value;
                RaisePropertyChanged(() => Platforms);
            }
        }


        public List<Person> Persons
        {
            get { return _lstPersons; }
            set
            {
                _lstPersons = value;
                RaisePropertyChanged(() => Persons);
            }
        }


        public List<L_FisheryType> FisheryTypes
        {
            get { return _lstFisheryTypes; }
            set
            {
                _lstFisheryTypes = value;
                RaisePropertyChanged(() => FisheryTypes);
            }
        }


        public Trip Trip
        {
            get { return _trip; }
            set
            {
                _trip = value;

                RaisePropertyChanged(() => Trip);
                RefreshAllNotifiableProperties();
            }
        }


        public string TripNumber
        {
            get { return _trip == null ? null : _trip.trip1; }
            set
            {
                if (_trip.trip1 != value)
                    _trip.trip1 = value;
                RaisePropertyChanged(() => TripNumber);
            }
        }

        public string REKTripNumber
        {
            get { return _trip == null ? null : _trip.tripNum; }
            set
            {
                if (_trip.tripNum != value)
                    _trip.tripNum = value;
                RaisePropertyChanged(() => REKTripNumber);
            }
        }
        
        public string RekSGTrId
        {
            get { return _trip == null ? null : _trip.sgTripId; }
            set
            {
                if (_trip.sgTripId != value)
                    _trip.sgTripId = value;
                RaisePropertyChanged(() => RekSGTrId);
            }
        }

        public string PlaceName
        {
            get { return _trip == null ? null : _trip.placeName; }
            set
            {
                if (_trip.placeName != value)
                    _trip.placeName = value;
                RaisePropertyChanged(() => PlaceName);
            }
        }

        public string PlaceCode
        {
            get { return _trip == null ? null : _trip.placeCode; }
            set
            {
                if (_trip.placeCode != value)
                    _trip.placeCode = value;
                RaisePropertyChanged(() => PlaceCode);
            }
        }

        public int? PostalCode
        {
            get { return _trip == null ? null : _trip.postalCode; }
            set
            {
                if (_trip.postalCode != value)
                    _trip.postalCode = value;
                RaisePropertyChanged(() => PostalCode);
            }
        }

        public int? NumberInPlace
        {
            get { return _trip == null ? null : _trip.numberInPlace; }
            set
            {
                if (_trip.numberInPlace != value)
                    _trip.numberInPlace = value;
                RaisePropertyChanged(() => NumberInPlace);
            }
        }

        public int? RespYes
        {
            get { return _trip == null ? null : _trip.respYes; }
            set
            {
                if (_trip.respYes != value)
                    _trip.respYes = value;
                RaisePropertyChanged(() => RespYes);
            }
        }

        public int? RespNo
        {
            get { return _trip == null ? null : _trip.respNo; }
            set
            {
                if (_trip.respNo != value)
                    _trip.respNo = value;
                RaisePropertyChanged(() => RespNo);
            }
        }

        public int? RespTotal
        {
            get { return _trip == null ? null : _trip.respTot; }
            set
            {
                if (_trip.respTot != value)
                    _trip.respTot = value;
                RaisePropertyChanged(() => RespTotal);
            }
        }


        public string LogbookNumber
        {
            get { return _trip == null ? null : _trip.logBldNr; }
            set
            {
                if (_trip.logBldNr != value)
                    _trip.logBldNr = value;
                RaisePropertyChanged(() => LogbookNumber);
            }
        }


        public L_SamplingMethod SelectedSamplingMethod
        {
            get { return _trip == null ? null : _trip.L_SamplingMethod; }
            set
            {
                if (!UpdatingCollections)
                {
                    if (_trip.L_SamplingMethod != value)
                        _trip.L_SamplingMethod = value;
                }

                RaisePropertyChanged(() => SelectedSamplingMethod);
            }
        }


        public L_TimeZone SelectedTimeZone
        {
            get { return (_trip == null || _lstTimeZones == null) ? null : _lstTimeZones.Where(x => x.timeZone == _trip.timeZone).FirstOrDefault(); }
            set
            {
                if (!UpdatingCollections)
                {
                    if (value == null || (_trip.timeZone != value.timeZone))
                        _trip.timeZone = value == null ? new Nullable<int>() : value.timeZone;
                }

                RaisePropertyChanged(() => SelectedTimeZone);
            }
        }


        private DateTime? _departureDateTime;
        public DateTime? DepartureDateTime
        {
            get { return _departureDateTime; }
            set
            {
                _departureDateTime = value;
                RaisePropertyChanged(() => DepartureDateTime);

                if (IsEdit)
                    _trip.MarkAsModified();
            }
        }

        private DateTime? _arrivalDateTime;
        public DateTime? ArrivalDateTime
        {
            get { return _arrivalDateTime; }
            set
            {
                _arrivalDateTime = value;
                RaisePropertyChanged(() => ArrivalDateTime);

                if (IsEdit)
                    _trip.MarkAsModified();
            }
        }


        public L_Harbour SelectedHarbourLanding
        {
            get { return _trip == null ? null : _trip.L_Harbour; }
            set
            {
                if (!UpdatingCollections)
                {
                    _trip.L_Harbour = value;
                }

                RaisePropertyChanged(() => SelectedHarbourLanding);
            }
        }


        public L_Platform SelectedPlatform1
        {
            get { return _trip == null ? null : _trip.L_Platform; }
            set
            {
                if (!UpdatingCollections)
                {
                    if (_trip != null)
                    {
                        if (TripType == "SØS" && !_blnAssigningLookups && ((_trip.L_Platform1 == null && value != null) ||
                            (_trip.L_Platform1 != null && value != null && _trip.L_Platform1.L_platformId != value.L_platformId) ||
                            (_trip.L_Platform1 == null && value != null)))
                        {
                            LoadDefaultContactPersonAsync(value.L_platformId);
                        }

                        _trip.L_Platform = value;
                    }
                }

                
                RaisePropertyChanged(() => SelectedPlatform1);
            }
        }


        public L_Platform SelectedPlatform2
        {
            get { return _trip == null ? null : _trip.L_Platform1; }
            set
            {
                if (!UpdatingCollections)
                {
                    if (_trip != null)
                        _trip.L_Platform1 = value;
                }

                RaisePropertyChanged(() => SelectedPlatform2);
            }
        }


        public bool? HasCamera
        {
            get { return (_trip == null || !_trip.fDFVessel.HasValue) ? false : _trip.fDFVessel.Value; }
            set
            {
                if (_trip.fDFVessel != value)
                    _trip.fDFVessel = value;

                RaisePropertyChanged(() => HasCamera);
            }
        }


        public Person SelectedContactPerson
        {
            get { return _trip == null ? null : _trip.Person; }
            set
            {
               if (!UpdatingCollections)
                {
                    if (_trip != null)
                        _trip.Person = value;
                }

                RaisePropertyChanged(() => SelectedContactPerson);
                RaisePropertyChanged(() => IsContactPersonDetailsEnabled);
                RaisePropertyChanged(() => ContactPersonOrganization);
                RaisePropertyChanged(() => ContactPersonAddress);
                RaisePropertyChanged(() => ContactPersonZipTown);
                RaisePropertyChanged(() => ContactPersonTelephone);
                RaisePropertyChanged(() => ContactPersonTelephonePrivate);
                RaisePropertyChanged(() => ContactPersonTelephoneMobile);
                RaisePropertyChanged(() => ContactPersonEmail);
                RaisePropertyChanged(() => ContactPersonFacebook);
            }
        }

        private Person _tmpNewPerson = null;
        public string ContactPersonName
        {
            get { return _strContactPersonName; }
            set
            {
                _strContactPersonName = value;

                //Create a new contact person if no existing one has been selected
                if (SelectedContactPerson == null && !string.IsNullOrWhiteSpace(value))
                {
                    SelectedContactPerson = _tmpNewPerson ?? (_tmpNewPerson = new Person());
                    IsCompanyFocused = true;
                }

                //Apply the name to the selected contact person
                if(SelectedContactPerson != null)
                    SelectedContactPerson.name = value;

                RaisePropertyChanged(() => ContactPersonName);
            }
        }

        public string ContactPersonOrganization
        {
            get { return SelectedContactPerson != null ? SelectedContactPerson.organization : null; }
            set
            {
                if (SelectedContactPerson != null)
                    SelectedContactPerson.organization = value;

                RaisePropertyChanged(() => ContactPersonOrganization);
            }
        }

        public string ContactPersonAddress
        {
            get { return SelectedContactPerson != null ? SelectedContactPerson.address : null; }
            set
            {
                if (SelectedContactPerson != null)
                    SelectedContactPerson.address = value;

                RaisePropertyChanged(() => ContactPersonAddress);
            }
        }

        public string ContactPersonZipTown
        {
            get { return SelectedContactPerson != null ? SelectedContactPerson.zipTown : null; }
            set
            {
                if (SelectedContactPerson != null)
                    SelectedContactPerson.zipTown = value;

                RaisePropertyChanged(() => ContactPersonZipTown);
            }
        }

        public string ContactPersonTelephone
        {
            get { return SelectedContactPerson != null ? SelectedContactPerson.telephone : null; }
            set
            {
                if (SelectedContactPerson != null)
                    SelectedContactPerson.telephone = value;

                RaisePropertyChanged(() => ContactPersonTelephone);
            }
        }

        public string ContactPersonTelephonePrivate
        {
            get { return SelectedContactPerson != null ? SelectedContactPerson.telephonePrivate : null; }
            set
            {
                if (SelectedContactPerson != null)
                    SelectedContactPerson.telephonePrivate = value;

                RaisePropertyChanged(() => ContactPersonTelephonePrivate);
            }
        }

        public string ContactPersonTelephoneMobile
        {
            get { return SelectedContactPerson != null ? SelectedContactPerson.telephoneMobile : null; }
            set
            {
                if (SelectedContactPerson != null)
                    SelectedContactPerson.telephoneMobile = value;

                RaisePropertyChanged(() => ContactPersonTelephoneMobile);
            }
        }

        public string ContactPersonEmail
        {
            get { return SelectedContactPerson != null ? SelectedContactPerson.email : null; }
            set
            {
                if (SelectedContactPerson != null)
                    SelectedContactPerson.email = value;

                RaisePropertyChanged(() => ContactPersonEmail);
            }
        }

        public string ContactPersonFacebook
        {
            get { return SelectedContactPerson != null ? SelectedContactPerson.facebook : null; }
            set
            {
                if (SelectedContactPerson != null)
                    SelectedContactPerson.facebook = value;

                RaisePropertyChanged(() => ContactPersonFacebook);
            }
        }
        

        public bool IsContactPersonDetailsEnabled
        {
            get { return SelectedContactPerson != null; }
        }



        public DFUPerson SelectedTripLeader
        {
            get { return _trip == null ? null : _trip.DFUPerson; }
            set
            {
                if (!UpdatingCollections)
                {
                    _trip.DFUPerson = value;
                }
                RaisePropertyChanged(() => SelectedTripLeader);
            }
        }


        public DFUPerson SelectedInputPerson
        {
            get { return _trip == null ? null : _trip.DFUPerson1; }
            set
            {
                if (!UpdatingCollections)
                {
                    _trip.DFUPerson1 = value;
                }
                RaisePropertyChanged(() => SelectedInputPerson);
            }
        }


        public L_FisheryType SelectedFisheryType
        {
            get { return (_trip == null || _lstFisheryTypes == null || _trip.fisheryType == null) ? null : _lstFisheryTypes.Where(x => x.fisheryType == _trip.fisheryType).FirstOrDefault(); }
            set
            {
                if (!UpdatingCollections)
                {
                    var val = (value == null ? null : value.fisheryType);
                    if (_trip.fisheryType != val)
                        _trip.fisheryType = val;
                }

                RaisePropertyChanged(() => SelectedFisheryType);
            }
        }


        public string Remark
        {
            get { return _trip == null ? null : _trip.remark; }
            set
            {
                _trip.remark = value;
                RaisePropertyChanged(() => Remark);
            }
        }


        public bool IsTripNumberFocused
        {
            get { return _blnIsTripNumberFocused; }
            set
            {
                _blnIsTripNumberFocused = value;
                RaisePropertyChanged(() => IsTripNumberFocused);
            }
        }


        public bool IsCompanyFocused
        {
            get { return _blnIsCompanyFocused; }
            set
            {
                _blnIsCompanyFocused = value;
                RaisePropertyChanged(() => IsCompanyFocused);
            }
        }


        public bool IsRekSGTripIdFocused
        {
            get { return _blnIsRekSGTripIdFocused; }
            set
            {
                _blnIsRekSGTripIdFocused = value;
                RaisePropertyChanged(() => IsRekSGTripIdFocused);
            }
        }


        public string TripType
        {
            get { return _strTripType; }
            set
            {
                _strTripType = value;
                RaisePropertyChanged(() => TripType);
            }
        }



        public bool IsContactPersonLoading
        {
            get { return _blnIsContactPersonLoading; }
            set
            {
                _blnIsContactPersonLoading = value;
                RaisePropertyChanged(() => IsContactPersonLoading);
            }
        }



       


        #region Visibility properties


        public bool IsLogbookVisible
        {
            get { return _strTripType != null && _strTripType.IsSEA(); }
        }


        public bool IsSamplingMethodVisible
        {
            get { return _strTripType != null && _strTripType.IsSEA(); }
        }


        public bool IsHarbourVisible
        {
            get { return _strTripType != null && _strTripType.IsSEA(); }
        }


        public bool IsHasCameraVisible
        {
            get { return _strTripType != null && _strTripType.IsSEA(); }
        }


        public bool IsPlatform2Visible
        {
            get { return _strTripType != null && _strTripType.IsSEA(); }
        }


        public bool IsContactPersonVisible
        {
            get { return _strTripType != null && _strTripType.IsSEA(); }
        }


        public bool IsFisheryTypeVisible
        {
            get { return _strTripType != null && _strTripType.IsSEA(); }
        }


        public bool IsRecreationalFishery
        {
            get { return _strTripType != null && _strTripType.IsREK(); }
        }


        public bool IsArrivalTimeVisible
        {
            get { return _strTripType != null && !_strTripType.IsREK(); }
        }


        public bool IsPrimaryVesselVisible
        {
            get { return _strTripType != null && !_strTripType.IsREK(); }
        }


        public bool IsPersonDetailsVisible
        {
            get { return _strTripType != null && !_strTripType.IsREK(); }
        }
        

        #endregion



        #endregion



        public TripViewModel(int? intTripId = null, int? intCruiseId = null, string strTripType = null)
            : base()
        {
            _intEditingTripId = intTripId;
            IsEdit = intTripId.HasValue;
            _intCruiseId = intCruiseId;
            _strTripType = strTripType;

            if (!intTripId.HasValue && !intCruiseId.HasValue)
                throw new ApplicationException("Cannot create a new TripViewModel without a cruise id.");

            InitializeAsync();
            RegisterToKeyDown();
        }



        #region Initialize methods


        private Task InitializeAsync()
        {
            IsLoading = true;
            Task tt = Task.Factory.StartNew(() => Initialize()).ContinueWith(t => new Action(() => { IsLoading = false; ScrollTo("Home"); RaisePropertyChanged(() => CanEditOffline); }).Dispatch());

            if (IsEdit)
                Task.Factory.StartNew(() => InitializeMap(_intEditingTripId, TreeNodeLevel.Trip));

            return tt;
        }


        private void Initialize(bool blnFullInitialize = true)
        {
            try
            {
                var man = new DataInputManager();

                Trip t = null;

                if (blnFullInitialize)
                {
                    if (IsEdit)
                    {
                        t = man.GetEntity<Trip>(_intEditingTripId.Value);
                        if (t == null)
                        {
                            DispatchMessageBox("Det var ikke muligt at hente den ønskede tur.");
                            return;
                        }
                        else if (BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
                        {
                            //Make sure it is a clone for offline data, so changes to it without saving, won't cause a problem.
                            try
                            {
                                t = t.OmitClone("Sample");
                            }
                            catch (Exception e)
                            {
                                Anchor.Core.Loggers.Logger.LogError(e);
                            }
                        }
                        _strTripType = t.tripType;
                    }
                    else
                    {
                        t = new Trip();
                        t.cruiseId = _intCruiseId.Value;
                        t.tripType = _strTripType;
                    }
                }
                else
                    t = Trip;

                var lookupMan = new BusinessLogic.LookupManager();
                var lv = new LookupDataVersioning();

                var lstDFUPersons = lookupMan.GetLookups(typeof(DFUPerson), lv).OfType<DFUPerson>().OrderBy(x => x.initials).ToList();
                var lstSamplingMethods = IsSamplingMethodVisible ? lookupMan.GetLookups(typeof(L_SamplingMethod), lv).OfType<L_SamplingMethod>().OrderBy(x => x.UIDisplay).ToList() : null;
                var lstHarbours = IsHarbourVisible ? lookupMan.GetLookups(typeof(L_Harbour), lv).OfType<L_Harbour>().OrderBy(x => string.IsNullOrEmpty(x.nationality) ? "Z" : x.nationality).ThenBy(x => x.description).ToList() : null;
                var lstPlatforms = lookupMan.GetLookups(typeof(L_Platform), lv).OfType<L_Platform>().OrderBy(x => x.UIString).ToList();
                var lstPersons = IsContactPersonVisible ? lookupMan.GetLookups(typeof(Person), lv).OfType<Person>().OrderBy(x => x.UIDisplay).ToList() : null;
                var lstTimeZones = lookupMan.GetLookups(typeof(L_TimeZone), lv).OfType<L_TimeZone>().OrderBy(x => x.timeZone).ToList();
                var lstFisheryTypes = lookupMan.GetLookups(typeof(L_FisheryType), lv).OfType<L_FisheryType>().OrderBy(x => x.UIDisplay).ToList();

                //Only show DNK platforms for SØS
                if (t.IsSEA)
                    lstPlatforms = lstPlatforms.Where(x => string.IsNullOrWhiteSpace(x.nationality) || x.nationality.Equals("dnk", StringComparison.InvariantCultureIgnoreCase)).ToList();

                Cruise cruise = null;
                
                if(blnFullInitialize)
                    cruise = new DataInputManager().GetEntity<Cruise>(t.cruiseId);

                new Action(() =>
                {
                    _blnUpdatingCollections = true;

                    //First set lookup lists
                    DFUPersons = lstDFUPersons;
                    SamplingMethods = lstSamplingMethods;
                    Harbours = lstHarbours;
                    Platforms = lstPlatforms;
                    Persons = lstPersons;
                    TimeZones = lstTimeZones;
                    FisheryTypes = lstFisheryTypes;

                    _blnUpdatingCollections = false;

                    if (cruise != null)
                    {
                        CruiseTitle = cruise.cruise1;
                        CruiseYear = cruise.year;
                    }

                    //Assign trip (it's important the trip is assigned after the lookup lists (or the selected values on trip will be overwritten with null).
                    if(blnFullInitialize)
                        Trip = t;

                    AssignSelectedLookups();
                    //ReasignSelectedLookups();

                    if (blnFullInitialize)
                    {
                        //Set UI Focus
                        if (!IsEdit)
                        {
                            if (IsRecreationalFishery)
                                IsRekSGTripIdFocused = true;
                            else
                                IsTripNumberFocused = true;
                            try { AssignTimeZoneAndDates(); }
                            catch { }
                        }
                        else //Reset any changes done to trip during initialization (so it is not dirty)
                        {
                            if(MapViewModel != null)
                                MapViewModel.WindowTitle = string.Format("Togt: {0}, Tur: {1}", cruise == null ? "" : cruise.cruise1, t == null ? "" : t.trip1);

                            _departureDateTime = Trip.dateStart;
                            _arrivalDateTime = Trip.dateEnd;

                            if (_departureDateTime.HasValue && SelectedTimeZone != null)
                                DepartureDateTime = _departureDateTime.Value.AddHours(SelectedTimeZone.timeZone);
                            else
                                RaisePropertyChanged(() => DepartureDateTime);

                            if (_arrivalDateTime.HasValue && SelectedTimeZone != null)
                                ArrivalDateTime = _arrivalDateTime.Value.AddHours(SelectedTimeZone.timeZone);
                            else
                                RaisePropertyChanged(() => ArrivalDateTime);

                            Trip.AcceptChanges();
                            RaisePropertyChanged(() => Trip);
                        }
                    }
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


        private void AssignTimeZoneAndDates()
        {
            if (TripType.IsSEA())
            {
                //Retrieve UTC timezone offset
                TimeSpan tsUTCOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);

                //Preselect timezone
                SelectedTimeZone = TimeZones.Where(tz => tz.timeZone == tsUTCOffset.Hours).FirstOrDefault();

                //Set departure and arrival time to the same value
                DepartureDateTime = DateTime.Now;
                ArrivalDateTime = DepartureDateTime;
            }
            else if (TripType.IsVID())
            {
                //Preselect timezone
                SelectedTimeZone = TimeZones.Where(tz => tz.timeZone == 0).FirstOrDefault();

                DateTime dtUtc = DateTime.UtcNow;
                DepartureDateTime = new DateTime(CruiseYear.HasValue ? CruiseYear.Value : dtUtc.Year, dtUtc.Month, dtUtc.Day, dtUtc.Hour, dtUtc.Minute, dtUtc.Second, dtUtc.Millisecond, DateTimeKind.Utc);
                ArrivalDateTime = new DateTime(Math.Max(CruiseYear.HasValue ? CruiseYear.Value : dtUtc.Year, dtUtc.Year), dtUtc.Month, dtUtc.Day, dtUtc.Hour, dtUtc.Minute, dtUtc.Second, dtUtc.Millisecond, DateTimeKind.Utc);
            }
            else if(TripType.IsREK())
            {
                SelectedTimeZone = TimeZones.Where(tz => tz.timeZone == 0).FirstOrDefault();
                DateTime dtUtc = DateTime.UtcNow;
                DepartureDateTime = new DateTime(CruiseYear.HasValue ? CruiseYear.Value : dtUtc.Year, dtUtc.Month, dtUtc.Day, dtUtc.Hour, dtUtc.Minute, dtUtc.Second, dtUtc.Millisecond, DateTimeKind.Utc);
                ArrivalDateTime = null;
            }
        }


        private bool _blnAssigningLookups = false;

        private void AssignSelectedLookups()
        {
            _blnAssigningLookups = true;
            {
                if ((!IsEdit || _trip.timeZone.HasValue) && _lstTimeZones != null)
                {
                    int intTimeZone = _trip.timeZone.HasValue ? _trip.timeZone.Value : 0;
                    SelectedTimeZone = _lstTimeZones.Where(x => x.timeZone == intTimeZone).FirstOrDefault();
                }

                if (_trip.samplingMethodId.HasValue && _lstSamplingMethods != null)
                    SelectedSamplingMethod = _lstSamplingMethods.Where(x => x.samplingMethodId == _trip.samplingMethodId.Value).FirstOrDefault();

                if (_trip.platform1 != null && _lstPlatforms != null)
                    SelectedPlatform1 = _lstPlatforms.Where(x => x.platform == _trip.platform1).FirstOrDefault();

                if (_trip.platform2 != null && _lstPlatforms != null)
                    SelectedPlatform2 = _lstPlatforms.Where(x => x.platform == _trip.platform2).FirstOrDefault();

                if (_trip.harbourLanding != null && _lstHarbours != null)
                    SelectedHarbourLanding = _lstHarbours.Where(x => x.harbour == _trip.harbourLanding).FirstOrDefault();

                if (_trip.contactPersonId.HasValue && _lstPersons != null)
                {
                    SelectedContactPerson = _lstPersons.Where(x => x.personId == _trip.contactPersonId.Value).FirstOrDefault();
                    if (SelectedContactPerson != null)
                    {
                        _strContactPersonName = SelectedContactPerson.name;
                        RaisePropertyChanged(() => ContactPersonName);
                    }
                }

                if (_trip.tripLeaderId.HasValue && _lstDFUPersons != null)
                    SelectedTripLeader = _lstDFUPersons.Where(x => x.dfuPersonId == _trip.tripLeaderId.Value).FirstOrDefault();

                if (_trip.dataHandlerId.HasValue && _lstDFUPersons != null)
                    SelectedInputPerson = _lstDFUPersons.Where(x => x.dfuPersonId == _trip.dataHandlerId.Value).FirstOrDefault();

                if (_trip.fisheryType != null && _lstFisheryTypes != null)
                    SelectedFisheryType = _lstFisheryTypes.Where(x => x.fisheryType == _trip.fisheryType).FirstOrDefault();
            }
            _blnAssigningLookups = false;
        }


        /// <summary>
        /// Load contact person asynchronously
        /// </summary>
        private Task LoadDefaultContactPersonAsync(int platformId)
        {
            IsContactPersonLoading = true;
            return Task.Factory.StartNew(() => LoadDefaultContactPerson(platformId)).ContinueWith(t => new Action(() => { IsContactPersonLoading = false; }).Dispatch());
        }


        private void LoadDefaultContactPerson(int platformId)
        {
            try
            {
                var man = new BusinessLogic.DataInput.DataInputManager();
                var person = man.GetLatestPersonFromPlatformId(platformId);

                if (person == null)
                    return;

                int intPersonId = person.personId;

                new Action(() =>
                {
                    SelectedContactPerson = _lstPersons.Where(x => x.personId == intPersonId).FirstOrDefault();
                    if (SelectedContactPerson != null)
                    {
                        _strContactPersonName = SelectedContactPerson.name;
                        RaisePropertyChanged(() => ContactPersonName);
                    }
                }).Dispatch();
            }
            catch { }
        }


        #endregion


        /// <summary>
        /// Validates property with name strFieldName. This method is overriding a base method which is called
        /// whenever a property is changed.
        /// </summary>
        protected override string ValidateField(string strFieldName)
        {
            string strError = null;

            //Only perform validation when user clicks "Save".
           // if (_blnValidate)
            {
                switch (strFieldName)
                {
                    case "TripNumber":
                        if (_blnValidate && string.IsNullOrEmpty(TripNumber))
                            strError = IsRecreationalFishery ? "Angiv venligst tur-id." : "Angiv venligst tur nr.";
                        else if (TripNumber != null && TripNumber.Length > 10)
                            strError = string.Format("'{0}' må kun bestå af 10 tegn. Den består pt. af {1} tegn.", IsRecreationalFishery ? "Tur-id" : "Tur nr", TripNumber.Length);

                        if (_blnValidate && strError != null)
                            ScrollTo("TripNumber");
                        break;

                    case "REKTripNumber":
                        
                        if (_blnValidate && string.IsNullOrEmpty(REKTripNumber) && IsRecreationalFishery)
                            strError = "Angiv venligst turnummer.";
                        else if (REKTripNumber != null && REKTripNumber.Length > 200)
                            strError = string.Format("'{0}' må kun bestå af 200 tegn. Den består pt. af {1} tegn.", "Turnummer", REKTripNumber.Length);
                        
                        if (_blnValidate && strError != null)
                            ScrollTo(() => REKTripNumber);
                        break;

                        

                    case "RekSGTrId":
                       /* if (_blnValidate && string.IsNullOrWhiteSpace(RekSGTrId) && IsRecreationalFishery)
                            strError = "Angiv venligst survey gizmo id.";
                        else*/ if (RekSGTrId != null && RekSGTrId.Length > 200)
                            strError = string.Format("'Survey gizmo id' må kun bestå af 200 tegn. Det består pt. af {0} tegn.", RekSGTrId.Length);

                        if (_blnValidate && strError != null)
                            ScrollTo(() => RekSGTrId);
                        break;



                    case "SelectedPlatform1":
                        if (_blnValidate && SelectedPlatform1 == null && !IsEdit && !IsRecreationalFishery)
                            strError = "Angiv venligst et kutter nr. (under primært fartøj).";

                        if (_blnValidate && strError != null)
                            ScrollTo("SelectedPlatform1");
                        else if (strError == null && (IsEdit || _blnValidate) && SelectedPlatform1 == null) //Show warning for existing trips
                        {
                            strError = String.Format(string.Format("{0}'Kutter nr.' er ikke angivet.", WarningPrefix));
                        }
                        break;

                    case "ContactPersonName":
                        if(SelectedContactPerson != null && string.IsNullOrWhiteSpace(SelectedContactPerson.name))
                        {
                            strError = "Angiv venligst et navn til kontaktpersonen.";
                            
                            if(_blnValidate)
                                ScrollTo("ContactPersonName");
                        }
                        break;

                    case "ContactPersonOrganization":
                        if (ContactPersonOrganization != null && ContactPersonOrganization.Length > 60)
                        {
                            strError = string.Format("'Firma/Organisation' for kontaktpersonen, må kun bestå af 60 tegn. Den består pt. af {0} tegn.", ContactPersonOrganization.Length);

                            if (_blnValidate)
                                ScrollTo("ContactPersonOrganization");
                        }
                        break;

                    case "ContactPersonAddress":
                        if (ContactPersonAddress != null && ContactPersonAddress.Length > 60)
                        {
                            strError = string.Format("'Adresse' for kontaktpersonen, må kun bestå af 60 tegn. Den består pt. af {0} tegn.", ContactPersonAddress.Length);

                            if (_blnValidate)
                                ScrollTo("ContactPersonAddress");
                        }
                        break;

                    case "ContactPersonZipTown":
                        if (ContactPersonZipTown != null && ContactPersonZipTown.Length > 30)
                        {
                            strError = string.Format("'Postnummer og by' for kontaktpersonen, må kun bestå af 30 tegn. Den består pt. af {0} tegn.", ContactPersonZipTown.Length);

                            if (_blnValidate)
                                ScrollTo("ContactPersonZipTown");
                        }
                        break;

                    case "ContactPersonTelephone":
                        if (ContactPersonTelephone != null && ContactPersonTelephone.Length > 30)
                        {
                            strError = string.Format("'1. Telefon (skib)' for kontaktpersonen, må kun bestå af 30 tegn. Den består pt. af {0} tegn.", ContactPersonTelephone.Length);

                            if (_blnValidate)
                                ScrollTo("ContactPersonTelephone");
                        }
                        break;

                    case "ContactPersonTelephonePrivate":
                        if (ContactPersonTelephonePrivate != null && ContactPersonTelephonePrivate.Length > 30)
                        {
                            strError = string.Format("'2. Telefon (privat)' for kontaktpersonen, må kun bestå af 30 tegn. Den består pt. af {0} tegn.", ContactPersonTelephonePrivate.Length);

                            if (_blnValidate)
                                ScrollTo("ContactPersonTelephonePrivate");
                        }
                        break;

                    case "ContactPersonTelephoneMobile":
                        if (ContactPersonTelephoneMobile != null && ContactPersonTelephoneMobile.Length > 30)
                        {
                            strError = string.Format("'3. Telefon (mobil)' for kontaktpersonen, må kun bestå af 30 tegn. Den består pt. af {0} tegn.", ContactPersonTelephoneMobile.Length);

                            if (_blnValidate)
                                ScrollTo("ContactPersonTelephoneMobile");
                        }
                        break;

                    case "ContactPersonEmail":
                        if (ContactPersonEmail != null && ContactPersonEmail.Length > 150)
                        {
                            strError = string.Format("'Email' for kontaktpersonen, må kun bestå af 150 tegn. Den består pt. af {0} tegn.", ContactPersonEmail.Length);

                            if (_blnValidate)
                                ScrollTo("ContactPersonEmail");
                        }
                        break;

                    case "ContactPersonFacebook":
                        if (ContactPersonFacebook != null && ContactPersonFacebook.Length > 150)
                        {
                            strError = string.Format("'Facebook' for kontaktpersonen, må kun bestå af 150 tegn. Den består pt. af {0} tegn.", ContactPersonFacebook.Length);

                            if (_blnValidate)
                                ScrollTo("ContactPersonFacebook");
                        }
                        break;

                    case "LogbookNumber":
                        if (!string.IsNullOrWhiteSpace(LogbookNumber) && LogbookNumber.Length > 10)
                            strError = string.Format("'Logbogsbladnummer' må kun bestå af 10 tegn. Det består lige nu af {0} tegn.", LogbookNumber.Length);

                        if (_blnValidate && strError != null)
                            ScrollTo("LogbookNumber");
                        break;

                    case "DepartureDateTime":
                        if (DepartureDateTime.HasValue && CruiseYear.HasValue)
                        {
                            if (DepartureDateTime.Value.Year < CruiseYear.Value)
                            {
                                strError = string.Format("Året for afgangstid skal være lig med året for togtet ({0}).", CruiseYear.Value);

                                if (_blnValidate)
                                    ScrollTo("DepartureDateTime");
                            }
                        }
                        break;

                    case "ArrivalDateTime":
                        if (DepartureDateTime.HasValue && ArrivalDateTime.HasValue)
                        {
                            if (DepartureDateTime.Value > ArrivalDateTime.Value)
                            {
                                strError = "Ankomstdato skal komme efter afrejsedato.";

                                if (_blnValidate)
                                    ScrollTo("ArrivalDateTime");
                            }
                        }
                        break;

                    case "SelectedTimeZone":
                        if (SelectedTimeZone == null && (ArrivalDateTime.HasValue || DepartureDateTime.HasValue))
                        {
                            strError = "Vælg venligst en tidszone for den indtastede tid.";

                            if (_blnValidate)
                                ScrollTo("TripNumber");
                        }

                        break;

                    case "SelectedFisheryType":
                        if (IsFisheryTypeVisible && SelectedFisheryType == null) //Show warning for existing trips
                        {
                            strError = String.Format(string.Format("{0}'Fiskeritype' er ikke angivet.", WarningPrefix));
                        }
                        break;

                    case "Remark":
                        //Remark cannot be too long.
                        break;
                }
            }

            return strError;
        }


        #region Save Methods


        private bool HandleWarningsAndContinue()
        {
            bool blnWarnings = false;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Advarsler:");
            sb.AppendLine("");

 
            if (SelectedPlatform1 == null && IsEdit && !IsRecreationalFishery)
            {
                sb.AppendLine("- Kutter nr. (under primært fartøj), er ikke angivet.");
                blnWarnings = true;
            }

            if (IsFisheryTypeVisible && SelectedFisheryType == null)
            {
                sb.AppendLine("- Fiskeritype er ikke angivet.");
                blnWarnings = true;
            }

            if (blnWarnings)
            {
                sb.AppendLine("");
                sb.AppendLine("Er du sikker på du vil fortsætte og gemme turen?");

                var res = AppRegionManager.ShowMessageBox(sb.ToString(), System.Windows.MessageBoxButton.YesNo);
                if (res == System.Windows.MessageBoxResult.No)
                    return false;
            }

            return true;
        }



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

            if (!HandleWarningsAndContinue())
                return;

            IsLoading = true;
            Task.Factory.StartNew(Save);
        }


        /// <summary>
        /// Save changes to database. This method does not validate any fields (this should be done prior to calling the method).
        /// </summary>
        private void Save()
        {
            try
            {
                //Add running zeros to logBldNr.
                if (!string.IsNullOrWhiteSpace(_trip.logBldNr) && _trip.logBldNr.Length < 10)
                {
                    string s = _trip.logBldNr;
                    while (s.Length < 10)
                        s = "0" + s;
                    _trip.logBldNr = s;
                }

                //Convert to UTC.
                if (DepartureDateTime != null)
                    _trip.dateStart = DepartureDateTime.Value.AddHours(-SelectedTimeZone.timeZone);

                if (ArrivalDateTime != null)
                    _trip.dateEnd = ArrivalDateTime.Value.AddHours(-SelectedTimeZone.timeZone);

                var man = new DataInputManager();
                DatabaseOperationResult res = man.SaveTrip(ref _trip);

                //If saving cruise failed, show error message.
                if (res.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                {
                    if (res.DatabaseOperationStatus == DatabaseOperationStatus.ValidationError &&
                       res.Message == "DuplicateKey")
                        DispatchMessageBox(String.Format("En tur med nummer '{0}' eksisterer allerede.", _trip.trip1));
                    else
                        DispatchMessageBox("En uventet fejl opstod. " + res.Message);

                    new Action(() => IsLoading = false).Dispatch();
                    return;
                }

                new Action(() =>
                {
                    IsEdit = true;

                    _intEditingTripId = _trip.tripId;

                    //Reset trip changetracker
                    _trip.AcceptChanges();

                    RaisePropertyChanged(() => LogbookNumber);

                    InitializeAsync().ContinueWith(t =>
                    {
                        DispatchMessageBox("Turen blev gemt korrekt.", 2);
                    });
                   
                }).Dispatch();

                //Call save succeeded event (this makes sure the tree is updated with any changes)
                RaiseSaveSucceeded();

                //Refresh tree, if node exists.
                var treeNodeCruise = MainTree.GetCruiseNodeIfLoaded(_trip.cruiseId);
                if (treeNodeCruise != null)
                {
                    //Make sure cruise is assigned to have at least on trip (since one has now been created)
                    if (!treeNodeCruise.CruiseEntity.HasTrips)
                        treeNodeCruise.CruiseEntity.TripCount = 1;

                    treeNodeCruise.RefreshTreeAsync().ContinueWith(t =>
                    {
                        MainTree.SelectTreeNode(_trip.tripId, TreeNodeLevel.Trip);
                    });
                }

                Babelfisk.ViewModels.Security.BackupManager.Instance.Backup();
            }
            catch (Exception e)
            {
                DispatchMessageBox(String.Format("En uventet fejl opstod. {0}", e.Message));
                Anchor.Core.Loggers.Logger.LogError(e);
                new Action(() => IsLoading = false).Dispatch();
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


        #region Add Edit Lookups Command


        public DelegateCommand<string> AddEditLookupsCommand
        {
            get { return _cmdAddEditLookups ?? (_cmdAddEditLookups = new DelegateCommand<string>(p => AddEditLookups(p))); }
        }


        private void AddEditLookups(string strType)
        {
            if (!HasUserViewLookupRights())
                return;
            
            ViewModels.Lookup.LookupManagerViewModel lm = GetLookupManagerViewModel(strType);

            if (lm == null)
                throw new ApplicationException("Lookup type unrecognized.");

            lm.Closed += lm_Closed;
            AppRegionManager.LoadWindowViewFromViewModel(lm, true, "WindowWithBorderStyle");
        }

        protected void lm_Closed(object arg1, AViewModel arg2)
        {
            if (arg2 is LookupManagerViewModel && !(arg2 as LookupManagerViewModel).ChangesSaved)
                return;

            LoadingMessage = "Opdaterer kodelister, vent venligst...";
            IsLoading = true;
            
            //Reload project leaders drop down list (so any changes in the lookup manager are reflected).
            Task.Factory.StartNew(() => Initialize(false)).ContinueWith(t => new Action(() => { IsLoading = false; ResetLoadingMessage(); }).Dispatch());
        }


        #endregion


        #region New Station Command


        public DelegateCommand NewStationCommand
        {
            get { return _cmdNewStation ?? (_cmdNewStation = new DelegateCommand(NewStation)); }
        }

        private void NewStation()
        {
            NewStation(_trip.tripId);
        }

        public static void NewStation(int intTripId)
        {
            StationViewModel svm = new StationViewModel(null, intTripId);
            _appRegionManager.LoadViewFromViewModel(RegionName.MainRegion, svm);
        }


        #endregion


        #region Show Cruise Command


        public DelegateCommand ShowCruiseCommand
        {
            get { return _cmdShowCruise ?? (_cmdShowCruise = new DelegateCommand(ShowCruise)); }
        }


        private void ShowCruise()
        {
            var vm = new ViewModels.Input.CruiseViewModel(_trip.cruiseId);
            bool blnSuccess = AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, vm);
            //MainTree.DeselectAllAsync();

            if (blnSuccess)
            {
                Task.Factory.StartNew(() =>
                {
                    var node = MainTree.GetCruiseNodeIfLoaded(_trip.cruiseId);

                    if (node != null)
                        new Action(() => { node.IsSelected = true; MainTree.ExpandToNode(node); }).Dispatch();
                });
            }
        }


        #endregion


        protected override void GlobelPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            //Save on Ctrl + S
            if (HasEditingRights
                && ((e.Key == System.Windows.Input.Key.S && Keyboard.Modifiers == ModifierKeys.Control) || ((e.SystemKey == Key.G || e.Key == Key.G) && (Keyboard.Modifiers == ModifierKeys.Alt || Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt))))
                && ((Trip != null && Trip.ChangeTracker.State != ObjectState.Unchanged) || (SelectedContactPerson != null && SelectedContactPerson.ChangeTracker.State != ObjectState.Unchanged)))
                ValidateAndSaveAsync();

            if (!(_trip != null && _trip.IsHVN) && e.Key == System.Windows.Input.Key.I && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (IsLoading)
                {
                    AppRegionManager.ShowMessageBox("Vent venligst med at importere en station, til formen er færdig med at arbejde.", 5);
                    return;
                }

                if (!IsEdit)
                {
                    AppRegionManager.ShowMessageBox("Gem venligst turen først, inden du importere en station fra SIS.", 5);
                    return;
                } 

                TreeView.TripTreeItemViewModel.ImportStation(_trip.tripId);
            }else if (!(_trip != null && _trip.IsHVN) && ((e.SystemKey == Key.N || e.Key == Key.N) && (Keyboard.Modifiers == ModifierKeys.Alt || Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt))))
            {
                e.Handled = true;
                if (IsLoading)
                {
                    AppRegionManager.ShowMessageBox("Vent venligst med at oprette en ny station til formen er færdig med at arbejde.", 5);
                    return;
                }

                if (!IsEdit)
                {
                    AppRegionManager.ShowMessageBox("Gem venligst turen først, inden du opretter en ny station.", 5);
                    return;
                }

                NewStation(_trip.tripId);
            }

        }

    }
}
