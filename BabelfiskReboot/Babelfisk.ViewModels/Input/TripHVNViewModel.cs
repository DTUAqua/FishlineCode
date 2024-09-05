using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using Babelfisk.BusinessLogic.DataInput;
using Microsoft.Practices.Prism.Commands;
using Babelfisk.Entities;
using System.Windows.Input;
using System.ServiceModel;
using Babelfisk.ViewModels.Lookup;
using Babelfisk.BusinessLogic;
using System.Text.RegularExpressions;

namespace Babelfisk.ViewModels.Input
{
    public class TripHVNViewModel : AInputViewModel
    {
        private DelegateCommand<string> _cmdAddEditLookups;
        private DelegateCommand _cmdShowCruise;
        private DelegateCommand _cmdSpeciesList;
        private DelegateCommand _cmdNewStation;


        private List<L_DFUArea> _lstAreas;
        private List<L_Harbour> _lstHarbours;
        private List<L_StatisticalRectangle> _lstStatisticalRectangles;
        private List<L_Platform> _lstPlatforms;
        private List<L_GearType> _lstGearTypes;
        private List<DFUPerson> _lstDFUPersons;
        private List<L_SamplingMethod> _lstSamplingMethods;
        private List<L_SamplingType> _lstSamplingTypes;
        private List<L_CatchRegistration> _lstCatchRegistrations;
        private List<L_SpeciesRegistration> _lstSpeciesRegistrations;
        private List<L_FisheryType> _lstFisheryTypes;
        private List<L_Species> _lstTargetSpecies;

        private bool _blnUpdatingCollections = false;

        private int? _intCruiseId;
        private int? _intTripId;

        private Sample _sample;
        private Trip _trip;

        //Used for header display
        private Cruise _cruise;

        private string _strTripType = "HVN";

        private bool _blnIsSquareLoading = false;
        private bool _blnIsStationNumberFocused = false;

        private L_Species _selectedSpecies;

        /// <summary>
        /// Database Numeric(5,1) validator (number of decimals is actual not important, since these are rounded on database insert)
        /// </summary>
        static Regex _regNumeric51 = new Regex(@"^-?\d{0,4}(\.\d*)?$");


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

                return (_sample != null && (_sample.ChangeTracker.State != ObjectState.Unchanged)) ||
                       (_trip != null && (_trip.ChangeTracker.State != ObjectState.Unchanged))
                        ;
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


        public Sample Sample
        {
            get { return _sample; }
            set
            {
                _sample = value;

                RaisePropertyChanged(() => Sample);
                RefreshAllNotifiableProperties();
            }
        }


        public Cruise Cruise
        {
            get { return _cruise; }
            set
            {
                _cruise = value;
                RaisePropertyChanged(() => Cruise);
                RaisePropertyChanged(() => LogbookNumber);
                RaisePropertyChanged(() => CruiseTitle);
            }
        }



        #region List properties


        public List<L_FisheryType> FisheryTypes
        {
            get { return _lstFisheryTypes; }
            set
            {
                _lstFisheryTypes = value;
                RaisePropertyChanged(() => FisheryTypes);
            }
        }

        public List<L_DFUArea> Areas
        {
            get { return _lstAreas; }
            set
            {
                _lstAreas = value;
                RaisePropertyChanged(() => Areas);
            }
        }


        public List<L_Harbour> Harbours
        {
            get { return _lstHarbours == null ? null : _lstHarbours.ToList(); }
            set
            {
                _lstHarbours = value;
                RaisePropertyChanged(() => Harbours);
            }
        }


        public List<L_StatisticalRectangle> StatisticalRectangles
        {
            get { return _lstStatisticalRectangles; }
            set
            {
                _lstStatisticalRectangles = value;
                RaisePropertyChanged(() => StatisticalRectangles);
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


        public List<L_GearType> GearTypes
        {
            get { return _lstGearTypes == null ? null : _lstGearTypes.Where(x => x.showInSeaHvnUI).ToList(); }
            set
            {
                _lstGearTypes = value;
                RaisePropertyChanged(() => GearTypes);
            }
        }


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


        public List<L_SamplingType> SamplingTypes
        {
            get { return _lstSamplingTypes; }
            set
            {
                _lstSamplingTypes = value;
                RaisePropertyChanged(() => SamplingTypes);
            }
        }


        public List<L_CatchRegistration> CatchRegistrations
        {
            get { return _lstCatchRegistrations; }
            set
            {
                _lstCatchRegistrations = value;
                RaisePropertyChanged(() => CatchRegistrations);
            }
        }


        public List<L_SpeciesRegistration> SpeciesRegistrations
        {
            get { return _lstSpeciesRegistrations; }
            set
            {
                _lstSpeciesRegistrations = value;
                RaisePropertyChanged(() => SpeciesRegistrations);
            }
        }


        public List<L_Species> TargetSpecies
        {
            get { return _lstTargetSpecies; }
            set
            {
                _lstTargetSpecies = value;
                RaisePropertyChanged(() => TargetSpecies);
            }
        }


        #endregion


        public string StationNumber
        {
            get { return _sample == null ? null : _sample.station; }
            set
            {
                if (_sample.station != value)
                    _sample.station = value;
                if (_trip.trip1 != value)
                    _trip.trip1 = value;

                RaisePropertyChanged(() => StationNumber);
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


        public DateTime? SampleDate
        {
            get { return _trip == null ? null : _trip.dateSample; }
            set
            {
                if (_trip.dateSample != value)
                    _trip.dateSample = value;

                RaisePropertyChanged(() => SampleDate);
            }
        }



        public DateTime? LandingDate
        {
            get { return _trip == null ? null : _trip.dateEnd; }
            set
            {
                if (_trip.dateEnd != value)
                    _trip.dateEnd = value;

                RaisePropertyChanged(() => LandingDate);
            }
        }


        public L_DFUArea SelectedArea
        {
            get { return (_sample == null || _lstAreas == null || _sample.dfuArea == null) ? null : _lstAreas.Where(x => x.DFUArea.Equals(_sample.dfuArea, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(); }
            set
            {
                if (!UpdatingCollections)
                {
                    var val = (value == null ? null : value.DFUArea);
                    if (_sample.dfuArea != val)
                        _sample.dfuArea = val;
                }

                RaisePropertyChanged(() => SelectedArea);
            }
        }


        public L_Harbour SelectedHarbourLanding
        {
            get { return (_trip == null || _lstHarbours == null) ? null : _lstHarbours.Where(x => x.harbour.Equals(_trip.harbourLanding, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(); }
            set
            {
                if (!UpdatingCollections)
                {
                    var val = (value == null ? null : value.harbour);
                    if (_trip.harbourLanding != val)
                        _trip.harbourLanding = val;
                }

                RaisePropertyChanged(() => SelectedHarbourLanding);
            }
        }


        public L_Harbour SelectedHarbourSample
        {
            get { return (_trip == null || _lstHarbours == null) ? null : _lstHarbours.Where(x => x.harbour.Equals(_trip.harbourSample, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(); }
            set
            {
                if (!UpdatingCollections)
                {
                    var val = (value == null ? null : value.harbour);
                    if (_trip.harbourSample != val)
                        _trip.harbourSample = val;
                }

                RaisePropertyChanged(() => SelectedHarbourSample);
            }
        }


        public L_StatisticalRectangle SelectedRectangle
        {
            get { return (_sample == null || _lstStatisticalRectangles == null || _sample.statisticalRectangle == null) ? null : _lstStatisticalRectangles.Where(x => x.statisticalRectangle.Equals(_sample.statisticalRectangle, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(); }
            set
            {
                if (!UpdatingCollections)
                {
                    var val = (value == null ? null : value.statisticalRectangle);
                    if (_sample.statisticalRectangle != val)
                        _sample.statisticalRectangle = val;
                }

                RaisePropertyChanged(() => SelectedRectangle);
            }
        }


        public L_Species SelectedTargetSpecies
        {
            get { return _selectedSpecies; }
            set
            {
                if (!_blnUpdatingCollections)
                {
                    if ((_selectedSpecies == null && value != null) || (_selectedSpecies != null && !_selectedSpecies.Equals(value)))
                    {
                        _selectedSpecies = value;
                        if (_sample != null && IsEdit)
                            _sample.MarkAsModified();
                    }
                }

                RaisePropertyChanged(() => SelectedTargetSpecies);
            }
        }


        public L_Platform SelectedPlatform1
        {
            get { return (_trip == null || _lstPlatforms == null) ? null : _lstPlatforms.Where(x => x.platform.Equals(_trip.platform1, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(); }
            set
            {

                if (!UpdatingCollections)
                {
                    var val = (value == null ? null : value.platform);
                    if (_trip.platform1 != val)
                        _trip.platform1 = val;
                }

                RaisePropertyChanged(() => SelectedPlatform1);
            }
        }


        public L_Platform SelectedPlatform2
        {
            get { return (_trip == null || _lstPlatforms == null) ? null : _lstPlatforms.Where(x => x.platform.Equals(_trip.platform2, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(); }
            set
            {
                if (!UpdatingCollections)
                {
                    var val = (value == null ? null : value.platform);
                    if (_trip.platform2 != val)
                        _trip.platform2 = val;
                }

                RaisePropertyChanged(() => SelectedPlatform1);
            }
        }


        public L_GearType SelectedGearType
        {
            get { return (_sample == null || _lstGearTypes == null || _sample.gearType == null) ? null : _lstGearTypes.Where(x => x.gearType == _sample.gearType).FirstOrDefault(); }
            set
            {
                if (!UpdatingCollections)
                {
                    var val = (value == null ? null : value.gearType);
                    if (_sample.gearType != val)
                        _sample.gearType = val;
                }

                RaisePropertyChanged(() => SelectedGearType);
            }
        }


        public DFUPerson SelectedCollectorPerson
        {
            get { return (_sample == null || _lstDFUPersons == null || _sample.samplePersonId == null) ? null : _lstDFUPersons.Where(x => x.dfuPersonId == _sample.samplePersonId).FirstOrDefault(); }
            set
            {
                if (!UpdatingCollections)
                {
                    var val = (value == null ? new Nullable<int>() : value.dfuPersonId);
                    if (_sample.samplePersonId != val)
                        _sample.samplePersonId = val;
                }

                RaisePropertyChanged(() => SelectedCollectorPerson);
            }
        }


        public DFUPerson SelectedAnalyzedByPerson
        {
            get { return (_sample == null || _lstDFUPersons == null || _sample.analysisPersonId == null) ? null : _lstDFUPersons.Where(x => x.dfuPersonId == _sample.analysisPersonId).FirstOrDefault(); }
            set
            {
                if (!UpdatingCollections)
                {
                    var val = (value == null ? new Nullable<int>() : value.dfuPersonId);
                    if (_sample.analysisPersonId != val)
                        _sample.analysisPersonId = val;
                }

                RaisePropertyChanged(() => SelectedAnalyzedByPerson);
            }
        }

        public DFUPerson SelectedOperatorPerson
        {
            get { return (_trip == null || _lstDFUPersons == null || _trip.dataHandlerId == null) ? null : _lstDFUPersons.Where(x => x.dfuPersonId == _trip.dataHandlerId).FirstOrDefault(); }
            set
            {
                if (!UpdatingCollections)
                {
                    var val = (value == null ? new Nullable<int>() : value.dfuPersonId);
                    if (_trip.dataHandlerId != val)
                        _trip.dataHandlerId = val;
                }

                RaisePropertyChanged(() => SelectedOperatorPerson);
            }
        }


        public L_SamplingType SelectedSamplingType
        {
            get { return (_trip == null || _lstSamplingTypes == null || _trip.samplingTypeId == null) ? null : _lstSamplingTypes.Where(x => x.samplingTypeId == _trip.samplingTypeId).FirstOrDefault(); }
            set
            {
                if (!UpdatingCollections)
                {
                    var val = (value == null ? new Nullable<int>() : value.samplingTypeId);
                    if (_trip.samplingTypeId != val)
                        _trip.samplingTypeId = val;
                }

                RaisePropertyChanged(() => SelectedSamplingType);
            }
        }


        public L_SamplingMethod SelectedSamplingMethod
        {
            get { return (_trip == null || _lstSamplingMethods == null || _trip.samplingMethodId == null) ? null : _lstSamplingMethods.Where(x => x.samplingMethodId == _trip.samplingMethodId).FirstOrDefault(); }
            set
            {
                if (!UpdatingCollections)
                {
                    var val = (value == null ? new Nullable<int>() : value.samplingMethodId);
                    if (_trip.samplingMethodId != val)
                        _trip.samplingMethodId = val;
                }

                RaisePropertyChanged(() => SelectedSamplingMethod);
            }
        }


        public L_CatchRegistration SelectedCatchRegistration
        {
            get { return (_sample == null || _lstCatchRegistrations == null || _sample.catchRegistrationId == null) ? null : _lstCatchRegistrations.Where(x => x.catchRegistrationId == _sample.catchRegistrationId).FirstOrDefault(); }
            set
            {
                if (!UpdatingCollections)
                {
                    var val = (value == null ? new Nullable<int>() : value.catchRegistrationId);
                    if (_sample.catchRegistrationId != val)
                        _sample.catchRegistrationId = val;
                }

                RaisePropertyChanged(() => SelectedCatchRegistration);
            }
        }


        public L_SpeciesRegistration SelectedSpeciesRegistration
        {
            get { return (_sample == null || _lstSpeciesRegistrations == null || _sample.speciesRegistrationId == null) ? null : _lstSpeciesRegistrations.Where(x => x.speciesRegistrationId == _sample.speciesRegistrationId).FirstOrDefault(); }
            set
            {
                if (!UpdatingCollections)
                {
                    var val = (value == null ? new Nullable<int>() : value.speciesRegistrationId);
                    if (_sample.speciesRegistrationId != val)
                        _sample.speciesRegistrationId = val;
                }

                RaisePropertyChanged(() => SelectedSpeciesRegistration);
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


        public int? MeshSize
        {
            get 
            {
                int? iMesh = null;

                if (_sample != null && _sample.meshSize.HasValue)
                    iMesh = Convert.ToInt32(_sample.meshSize.Value);

                return iMesh; 
            }
            set
            {
                if (_sample.meshSize != value)
                    _sample.meshSize = value;

                RaisePropertyChanged(() => MeshSize);
            }
        }


        public string Remark
        {
            get { return _trip == null ? null : _trip.remark; }
            set
            {
                if(_trip.remark != value)
                    _trip.remark = value;
                RaisePropertyChanged(() => Remark);
            }
        }



        public bool IsSquareLoading
        {
            get { return _blnIsSquareLoading; }
            set
            {
                _blnIsSquareLoading = value;
                RaisePropertyChanged(() => IsSquareLoading);
            }
        }


        public bool IsStationNumberFocused
        {
            get { return _blnIsStationNumberFocused; }
            set
            {
                _blnIsStationNumberFocused = value;
                RaisePropertyChanged(() => IsStationNumberFocused);
            }
        }


        public string CruiseTitle
        {
            get { return _cruise == null ? null : _cruise.cruiseTitle; }
        }


        public int? CruiseYear
        {
            get { return _cruise == null ? new Nullable<int>() : _cruise.year; }
        }


        #endregion



        public TripHVNViewModel(int? intTripId, int? intCruiseId = null)
        {
            _intCruiseId = intCruiseId;
            _intTripId = intTripId;

            IsEdit = _intTripId.HasValue;

            InitializeAsync();
            RegisterToKeyDown();
        }


        #region Initialization methods


        private Task InitializeAsync()
        {
            IsLoading = true;
            return Task.Factory.StartNew(() => Initialize()).ContinueWith(t => new Action(() => { IsLoading = false; ScrollTo("Home"); RaisePropertyChanged(() => CanEditOffline); }).Dispatch());
        }


        private void Initialize(bool blnFullInitialize = true)
        {
            try
            {
                var man = new DataInputManager();

                Trip t = null;
                Sample s = null;


                if (blnFullInitialize)
                {
                    if (IsEdit)
                    {
                        t = man.GetEntity<Trip>(_intTripId.Value);
                        var samples = man.GetSamplesFromTripId(_intTripId.Value);
                        s = samples == null ? null : samples.FirstOrDefault();

                        if (t == null)
                        {
                            DispatchMessageBox("Det var ikke muligt at hente den ønskede tur.");
                            new Action(() => Menu.MainMenuViewModel.ShowStart()).Dispatch();
                            return;
                        }

                        if (samples == null || samples.Count == 0)
                        {
                            DispatchMessageBox("Sample-delen mangler for den valgte havneindsamling (hvilket ikke burde være muligt). Fejlen håndteres ved at programmet opretter en ny sample-del til havneindsamlingen når der klikkes 'Gem'.");
                            s = new Sample();
                            s.station = t.trip1; //Note: Maximum 6 characters can be saved in station (trip1 can have 10).
                            s.tripId = t.tripId;
                        }

                        if (samples.Count > 1)
                        {
                            DispatchMessageBox("Turen indeholder mere end én sample-række, hvilket ikke burde være muligt for havneindsamlinger.");
                            Anchor.Core.Loggers.Logger.Log(Anchor.Core.Loggers.LogType.Error, string.Format("TripHVNViewModel->Initialize(). The HVN trip contains more than one ({0}) sample-row where only one was expected.", samples.Count));
                            new Action(() => Menu.MainMenuViewModel.ShowStart()).Dispatch();
                            return;
                        }

                        _intCruiseId = t.cruiseId;

                        if (BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
                        {
                            //Make sure it is a clone for offline data, so changes to it without saving, won't cause a problem.
                            try
                            {
                                t = t.Clone();
                                s = s.Clone();
                            }
                            catch (Exception e)
                            {
                                Anchor.Core.Loggers.Logger.LogError(e);
                            }
                        }
                    }
                    else
                    {
                        t = new Trip();
                        t.cruiseId = _intCruiseId.Value;
                        t.tripType = _strTripType;

                        s = new Sample();
                    }
                }


                var lookupMan = new BusinessLogic.LookupManager();
                var lv = new LookupDataVersioning();

                var lstAreas = lookupMan.GetLookups(typeof(L_DFUArea), lv).OfType<L_DFUArea>().OrderBy(x => x.UIDisplay).ToList();
                var lstHarbours = lookupMan.GetLookups(typeof(L_Harbour), lv).OfType<L_Harbour>().OrderBy(x => string.IsNullOrEmpty(x.nationality) ? "Z" : x.nationality).ThenBy(x => x.description).ToList();
                //Squares are loaded after an area has been selected
                //var lstSquares = lookupMan.GetLookups(typeof(L_StatisticalRectangle)).OfType<L_StatisticalRectangle>().OrderBy(x => x.UIDisplay).ToList();
                var lstPlatforms = lookupMan.GetLookups(typeof(L_Platform), lv).OfType<L_Platform>().OrderBy(x => x.UIString).ToList();
                var lstGearTypes = lookupMan.GetLookups(typeof(L_GearType), lv).OfType<L_GearType>().OrderBy(x => x.UIDisplay).ToList();
                var lstDFUPersons = lookupMan.GetLookups(typeof(DFUPerson), lv).OfType<DFUPerson>().OrderBy(x => x.initials).ToList();
                var lstSamplingMethods = lookupMan.GetLookups(typeof(L_SamplingMethod), lv).OfType<L_SamplingMethod>().OrderBy(x => x.UIDisplay).ToList();
                var lstSamplingTypes = lookupMan.GetLookups(typeof(L_SamplingType), lv).OfType<L_SamplingType>().OrderBy(x => x.UIDisplay).ToList();
                var lstCatchRegistrations = lookupMan.GetLookups(typeof(L_CatchRegistration), lv).OfType<L_CatchRegistration>().OrderBy(x => x.UIDisplay).ToList();
                var lstSpeciesRegistrations = lookupMan.GetLookups(typeof(L_SpeciesRegistration), lv).OfType<L_SpeciesRegistration>().OrderBy(x => x.UIDisplay).ToList();
                var lstFisheryTypes = lookupMan.GetLookups(typeof(L_FisheryType), lv).OfType<L_FisheryType>().OrderBy(x => x.UIDisplay).ToList();
                var lstSpecies = lookupMan.GetLookups(typeof(L_Species), lv).OfType<L_Species>().OrderBy(x => x.UIDisplay).ToList();

                Cruise cruise = null;

                if(blnFullInitialize)
                    cruise = man.GetEntity<Cruise>(t.cruiseId);

                new Action(() =>
                {
                    //First set lookup lists
                    _blnUpdatingCollections = true;
                    {
                        DFUPersons = lstDFUPersons;
                        Harbours = lstHarbours;
                        TargetSpecies = lstSpecies;
                        SamplingMethods = lstSamplingMethods;
                        Areas = lstAreas;
                        Platforms = lstPlatforms;
                        GearTypes = lstGearTypes;
                        SamplingTypes = lstSamplingTypes;
                        CatchRegistrations = lstCatchRegistrations;
                        SpeciesRegistrations = lstSpeciesRegistrations;
                        FisheryTypes = lstFisheryTypes;
                    }
                    _blnUpdatingCollections = false;

                    if (cruise != null)
                        Cruise = cruise;

                    if (blnFullInitialize)
                    {
                        //Assign trip (it's important the trip is assigned after the lookup lists (or the selected values on trip will be overwritten with null).
                        Trip = t;
                        Sample = s;

                        AssignSelectedLookups();

                        //Set UI Focus
                        if (!IsEdit)
                        {
                            IsStationNumberFocused = true;

                            try
                            {
                                DateTime dt = DateTime.Now;
                                LandingDate = new DateTime(CruiseYear.HasValue ? Cruise.year : dt.Year, dt.Month, dt.Day);
                            }
                            catch { }

                            try
                            {
                                DateTime dt = DateTime.Now;
                                SampleDate = new DateTime(CruiseYear.HasValue ? Cruise.year : dt.Year, dt.Month, dt.Day);
                            }
                            catch { }
                        }
                        else //Reset any changes done to trip during initialization (so it is not dirty)
                        {
                            Sample.AcceptChanges();
                            Trip.AcceptChanges();
                            RaisePropertyChanged(() => Trip);
                            RaisePropertyChanged(() => Sample);
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

                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }


        private bool _blnAssigningLookups = false;
        private void AssignSelectedLookups()
        {
            _blnAssigningLookups = true;
            {
                if (_sample.R_TargetSpecies != null && _sample.R_TargetSpecies.Count > 0 && _lstTargetSpecies != null)
                {
                    var targetSpecies = _sample.R_TargetSpecies.FirstOrDefault();
                    SelectedTargetSpecies = _lstTargetSpecies.Where(x => targetSpecies.speciesCode.Equals(x.speciesCode)).FirstOrDefault();
                }
            }
            _blnAssigningLookups = false;
        }


        #endregion


        #region Load statistical rectangle methods

        public void LoadStatisticalRectanglesAsync(string strArea)
        {
            IsSquareLoading = true;
            Task.Factory.StartNew(() => LoadStatisticalRectangles(strArea)).ContinueWith(t => new Action(() => { IsSquareLoading = false; }).Dispatch());
        }

        private void LoadStatisticalRectangles(string strArea)
        {
            if (strArea == null)
                return;

            var man = new BusinessLogic.DataInput.DataInputManager();
            var lst = man.GetStatisticalRectangleFromArea(strArea);

            new Action(() =>
            {
                _blnUpdatingCollections = true;
                {
                    StatisticalRectangles = lst;
                }
                _blnUpdatingCollections = false;

                if (_sample.statisticalRectangle != null && _lstStatisticalRectangles != null)
                    SelectedRectangle = _lstStatisticalRectangles.Where(x => x.statisticalRectangle == _sample.statisticalRectangle).FirstOrDefault();

            }).Dispatch();
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
            Task.Factory.StartNew(() => Initialize(false)).ContinueWith(t => new Action(() => 
                {
                    IsLoading = false;
                    RefreshAllNotifiableProperties();
                    ResetLoadingMessage();
                }
            ).Dispatch());
        }


     //   private void ReloadLookups(AViewModel avm


        #endregion



        #region Close Methods

        protected override void CloseViewModel()
        {
            //Redirect to start view.
            Menu.MainMenuViewModel.ShowStart();
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
            //if (_blnValidate)
            {
                switch (strFieldName)
                {
                    case "StationNumber":
                        if (_blnValidate && string.IsNullOrEmpty(StationNumber))
                            strError = "Angiv venligst Journal nr.";
                        else if (StationNumber != null && StationNumber.Length > 6)
                            strError = string.Format("Journal nr. må kun bestå af 6 tegn. Det består pt. af {0} tegn.", StationNumber.Length);

                        if (_blnValidate && strError != null)
                            ScrollTo(() => StationNumber);
                        break;

                    case "LogbookNumber":
                        if (!string.IsNullOrWhiteSpace(LogbookNumber) && LogbookNumber.Length > 10)
                            strError = string.Format("Logbogsbladnummer må kun bestå af 10 tegn. Det består lige nu af {0} tegn.", LogbookNumber.Length);

                        if (_blnValidate && strError != null)
                            ScrollTo("LogbookNumber");
                        break;

                    case "SampleDate":
                        //Only make sample date mandatory for new HVNs
                        if (_blnValidate && SampleDate == null && !IsEdit)
                            strError = "Angiv venligst indsamlingsdato.";

                        if (_blnValidate && strError != null)
                            ScrollTo("Home");
                        else if (strError == null && (IsEdit || _blnValidate)) //Handle warnings if no errors found
                        {
                            if (SampleDate == null)
                            {
                                strError = String.Format(string.Format("{0}Indsamlingsdato er ikke angivet.", WarningPrefix));
                            }
                            else if (SampleDate.HasValue && CruiseYear.HasValue && SampleDate.Value.Year != CruiseYear.Value)
                            {
                                strError = String.Format(string.Format("{0}Året for indsamlingsdato er forskellig fra året for togtet ({1}).", WarningPrefix, CruiseYear.Value));
                            }
                        }
                        break;

                    case "SelectedHarbourSample":
                        //Only make sample date mandatory for new HVNs
                        if (_blnValidate && SelectedHarbourSample == null && !IsEdit)
                            strError = "Angiv venligst indsamlingshavn.";

                        if (_blnValidate && strError != null)
                            ScrollTo("Home");
                        else if (strError == null && SelectedHarbourSample == null && (IsEdit || _blnValidate)) //Show warning, if editting a trip or clicking save
                        {
                            strError = String.Format(string.Format("{0}Indsamlingshavn er ikke angivet.", WarningPrefix));
                        }
                        break;

                    case "LandingDate":
                        //Only make landing date mandatory for new HVNs
                        if (_blnValidate && LandingDate == null && !IsEdit)
                            strError = "Angiv venligst landingsdato.";
                        else if (LandingDate.HasValue && CruiseYear.HasValue && LandingDate.Value.Year != CruiseYear.Value)
                            strError = string.Format("Året for landingsdato skal være lig med året for togtet ({0}).", CruiseYear.Value);

                        if (_blnValidate && strError != null)
                            ScrollTo("Home");
                        break;

                    case "SelectedCatchRegistration":
                        if (SelectedCatchRegistration != null && !SelectedCatchRegistration.catchRegistration.Equals("non", StringComparison.InvariantCultureIgnoreCase) &&
                            SelectedSpeciesRegistration != null && SelectedSpeciesRegistration.speciesRegistration.Equals("non", StringComparison.InvariantCultureIgnoreCase))
                            strError = "Når oparbejdning af arter er 'NON', skal prøvetagningsniveau også være 'NON'.";

                        if (_blnValidate && strError != null)
                            ScrollTo("CatchRegistration");
                        break;

                    case "SelectedSpeciesRegistration":
                        if(SelectedCatchRegistration != null && SelectedCatchRegistration.catchRegistration.Equals("non", StringComparison.InvariantCultureIgnoreCase) &&
                          (SelectedSpeciesRegistration == null || !SelectedSpeciesRegistration.speciesRegistration.Equals("non", StringComparison.InvariantCultureIgnoreCase)))
                            strError = "Når prøvetagningsniveau er 'NON', skal oparbejdning af arter også sættes til 'NON'.";

                        if (strError != null)
                            ScrollTo("SpeciesRegistration");
                        break;

                    case "MeshSize":
                        if (MeshSize.HasValue && !_regNumeric51.IsMatch(MeshSize.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Maskevidde (helmasker, mm)' må ikke være større end 9999.";

                        if (_blnValidate && strError != null)
                            ScrollTo("MeshSize");
                        else if (strError == null && MeshSize == null) //Show warning for new HVNs
                        {
                            strError = String.Format(string.Format("{0}Maskevidde er ikke angivet.", WarningPrefix));
                        }
                        break;

                    case "SelectedGearType":
                        if (SelectedGearType == null) //Show warning for new HVNs
                        {
                            strError = String.Format(string.Format("{0}Redskabtype er ikke angivet.", WarningPrefix));
                        }
                        break;

                    case "SelectedFisheryType":
                        if (SelectedFisheryType == null) //Show warning for new HVNs
                        {
                            strError = String.Format(string.Format("{0}Fiskeritype er ikke angivet.", WarningPrefix));
                        }
                        break;

                }
            }

            return strError;
        }


        #region Save Methods

        private void AssignSpeciesInformation()
        {
            if (SelectedTargetSpecies != null)
            {
                if (_sample.R_TargetSpecies != null && _sample.R_TargetSpecies.Count > 0)
                {
                    var s = _sample.R_TargetSpecies.First();
                    if (s.speciesCode != SelectedTargetSpecies.speciesCode)
                        s.speciesCode = SelectedTargetSpecies.speciesCode;
                }
                else
                {
                    _sample.R_TargetSpecies.Add(new R_TargetSpecies() { sampleId = _sample.sampleId, speciesCode = SelectedTargetSpecies.speciesCode });
                }
            }
            else
            {
                if (_sample.R_TargetSpecies != null && _sample.R_TargetSpecies.Count > 0)
                    _sample.R_TargetSpecies.First().MarkAsDeleted();
            }
        }


        private bool HandleWarningsAndContinue()
        {
            bool blnWarnings = false;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Advarsler:");
            sb.AppendLine("");

            if (SampleDate == null)
            {
                sb.AppendLine("- Indsamlingsdato er ikke angivet.");
                blnWarnings = true;
            }

            if (SampleDate.HasValue && CruiseYear.HasValue && CruiseYear.HasValue && SampleDate.Value.Year != CruiseYear.Value)
            {
                sb.AppendLine(string.Format("- Året for indsamlingsdato er forskellig fra året for togtet ({0}).", CruiseYear.Value));
                blnWarnings = true;
            }

            if (SelectedHarbourSample == null)
            {
                sb.AppendLine("- Indsamlingshavn er ikke angivet.");
                blnWarnings = true;
            }
          
            if (SelectedGearType == null)
            {
                sb.AppendLine("- Redskabtype er ikke angivet.");
                blnWarnings = true;
            }

            if (!MeshSize.HasValue)
            {
                sb.AppendLine("- Maskevidde er ikke udfyldt.");
                blnWarnings = true;
            }

            if (SelectedFisheryType == null)
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

                AssignSpeciesInformation();

                var man = new DataInputManager();

                if (string.IsNullOrWhiteSpace(_sample.@virtual))
                    _sample.@virtual = "nej";

                if (_trip.dateEnd.HasValue)
                    _sample.dateGearStart = _sample.dateGearEnd = _trip.dateEnd.Value;

                DatabaseOperationResult res = man.SaveHVN(ref _trip, ref _sample);

                //If saving cruise failed, show error message.
                if (res.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                {
                    if (res.DatabaseOperationStatus == DatabaseOperationStatus.ValidationError &&
                       res.Message == "DuplicateKey")
                        DispatchMessageBox(String.Format("En station med nummer '{0}' eksisterer allerede.", _sample.station));
                    else
                        DispatchMessageBox("En uventet fejl opstod. " + res.Message);

                    new Action(() => IsLoading = false).Dispatch();
                    return;
                }

                new Action(() =>
                {
                    IsEdit = true;
                    _intTripId = _trip.tripId;

                    //Reset change tracker.
                    _sample.AcceptChanges();
                    _trip.AcceptChanges();
                    IsLoading = false;

                    RaisePropertyChanged(() => LogbookNumber);
                    RaisePropertyChanged(() => MeshSize);
                   // InitializeAsync().ContinueWith(t =>
                   // {
                        DispatchMessageBox("Havneindsamlingen blev gemt korrekt.", 2);
                   // });
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


        #region Show Cruise Command


        public DelegateCommand ShowCruiseCommand
        {
            get { return _cmdShowCruise ?? (_cmdShowCruise = new DelegateCommand(ShowCruise)); }
        }


        private void ShowCruise()
        {
            if (!_intCruiseId.HasValue)
            {
                DispatchMessageBox("Kunne ikke vise togten da cruiseId er null (kontakt venligst en administrator med denne fejl).");
                return;
            }

            var vm = new ViewModels.Input.CruiseViewModel(_intCruiseId);
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


        #region SpeciesList Command


        public DelegateCommand SpeciesListCommand
        {
            get { return _cmdSpeciesList ?? (_cmdSpeciesList = new DelegateCommand(() => ShowSpeciesList())); }
        }


        public void ShowSpeciesList()
        {
            SpeciesListViewModel slvm = new SpeciesListViewModel(Sample.sampleId);
            bool blnSuccess = AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, slvm);

            if (blnSuccess && _sample != null && IsEdit)
            {
                Task.Factory.StartNew(() =>
                {
                    var node = MainTree.GetSpeciesListNodeIfLoaded(_sample.sampleId);

                    if (node != null)
                        new Action(() => { node.IsSelected = true; MainTree.ExpandToNode(node); }).Dispatch();
                });
            }
        }


        #endregion


        #region New Station Command

        public DelegateCommand NewStationCommand
        {
            get { return _cmdNewStation ?? (_cmdNewStation = new DelegateCommand(NewStation)); }
        }

        private void NewStation()
        {
            CruiseViewModel.NewTrip(_strTripType, _cruise.cruiseId);
        }

        #endregion


        protected override void GlobelPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            //Save on Ctrl + S
            if (HasEditingRights
                && ((e.Key == System.Windows.Input.Key.S && Keyboard.Modifiers == ModifierKeys.Control) || ((e.SystemKey == Key.G || e.Key == Key.G) && (Keyboard.Modifiers == ModifierKeys.Alt || Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt))))
                && HasUnsavedData)
                ValidateAndSaveAsync();

            if ((e.SystemKey == Key.A || e.Key == Key.A) && (Keyboard.Modifiers == ModifierKeys.Alt || Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt)))
            {
                e.Handled = true;
                if (IsLoading)
                {
                    AppRegionManager.ShowMessageBox("Vent venligst med at gå til artslisten, indtil formen er færdig med at arbejde.", 5);
                    return;
                }

                if (!IsEdit)
                {
                    AppRegionManager.ShowMessageBox("Gem venligst turen først, inden du går til artslisten.", 5);
                    return;
                }

                ShowSpeciesList();
            }
        }
    }

}
