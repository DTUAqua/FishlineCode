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
using Babelfisk.BusinessLogic;
using Babelfisk.ViewModels.Lookup;
using System.Text.RegularExpressions;

namespace Babelfisk.ViewModels.Input
{
    public class StationViewModel : AInputViewModel
    {
        private DelegateCommand _cmdShowCruise;
        private DelegateCommand _cmdShowTrip;
        private DelegateCommand _cmdSpeciesList;
        private DelegateCommand _cmdNewStation;

        private DelegateCommand<string> _cmdAddEditLookups;

        private static string _strDefaultLatCoordinates = "00°00.0000' N";
        private static string _strDefaultLonCoordinates = "000°00.0000' E";

        private int? _intEditingSampleId;
        private int? _intTripId;

        private bool _blnIsSquareLoading;
        private bool _blnIsGearTypesLoading;
        private bool _blnIsStationNumberFocused;
        private bool _blnIsSGIdFocused;
        private bool _blnIsCTDInfoVisible;

        private bool _blnIsImported = false;

        private Sample _sample;
        private Cruise _cruise;
        private Trip _trip;

        private L_Species _selectedSpecies;

        private List<L_TimeZone> _lstTimeZones;
        private List<L_SampleType> _lstSampleTypes;
        private List<L_GearQuality> _lstGearQuality;
        private List<L_Species> _lstTargetSpecies;
        private List<L_DFUArea> _lstAreas;
        private List<L_StatisticalRectangle> _lstStatisticalRectangles;
        private List<L_CatchRegistration> _lstCatchRegistrations;
        private List<L_SpeciesRegistration> _lstSpeciesRegistrations;
        private List<L_GearType> _lstGearTypes;
        private List<L_SelectionDevice> _lstSelectionDevices;
        private List<L_SelectionDeviceSource> _lstSelectionDeviceSources;
        private List<L_HaulType> _lstHaulTypes;
        private List<L_ThermoCline> _lstThermoClines;
        private List<L_Bottomtype> _lstBottomTypes;
        private List<DFUPerson> _lstDFUPersons;


        private bool _blnUpdatingCollections = false;


        #region Properties

        public override bool CanEditOffline
        {
            get
            {
                return base.CanEditOffline || !IsEdit || (IsLoading || (_sample != null && _sample.OfflineState == ObjectState.Added));
            }
        }


        public bool HasEditingRights
        {
            get
            {
                return User != null && User.HasTask(SecurityTask.ModifyData);
            }
        }

        public override bool HasUnsavedData
        {
            get
            {
                if (!HasEditingRights || !CanEditOffline)
                    return false;

                return _sample != null && (_sample.ChangeTracker.State != ObjectState.Unchanged);
            }
        }

        #region List properties


        public List<L_Bottomtype> BottomTypes
        {
            get { return _lstBottomTypes; }
            set
            {
                _lstBottomTypes = value;
                RaisePropertyChanged(() => BottomTypes);
            }
        }

        public List<L_HaulType> HaulTypes
        {
            get { return _lstHaulTypes; }
            set
            {
                _lstHaulTypes = value;
                RaisePropertyChanged(() => HaulTypes);
            }
        }


        public List<L_ThermoCline> ThermoClines
        {
            get { return _lstThermoClines; }
            set
            {
                _lstThermoClines = value;
                RaisePropertyChanged(() => ThermoClines);
            }
        }


        public List<L_SampleType> SampleTypes
        {
            get { return _lstSampleTypes; }
            set
            {
                _lstSampleTypes = value;
                RaisePropertyChanged(() => SampleTypes);
            }
        }


        public List<L_GearQuality> GearQualities
        {
            get { return _lstGearQuality; }
            set
            {
                _lstGearQuality = value;
                RaisePropertyChanged(() => GearQualities);
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


        public List<L_TimeZone> TimeZones
        {
            get { return _lstTimeZones; }
            set
            {
                _lstTimeZones = value;
                RaisePropertyChanged(() => TimeZones);
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

        public List<L_StatisticalRectangle> StatisticalRectangles
        {
            get { return _lstStatisticalRectangles; }
            set
            {
                _lstStatisticalRectangles = value;
                RaisePropertyChanged(() => StatisticalRectangles);
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

        public List<L_GearType> GearTypes
        {
            get { return SelectedSampleType == null ? new List<L_GearType>() : _lstGearTypes.Where(x => x.catchOperation.Equals(SelectedSampleType.sampleType, StringComparison.InvariantCultureIgnoreCase) &&
                                                                                                        ((IsScientific && x.showInVidUI) || (!IsScientific && x.showInSeaHvnUI))).ToList(); }
            set
            {
                _lstGearTypes = value;
                RaisePropertyChanged(() => GearTypes);
            }
        }

        public List<L_SelectionDevice> SelectionDevices
        {
            get { return _lstSelectionDevices; }
            set
            {
                _lstSelectionDevices = value;
                RaisePropertyChanged(() => SelectionDevices);
            }
        }

        public List<L_SelectionDeviceSource> SelectionDeviceSources
        {
            get { return _lstSelectionDeviceSources; }
            set
            {
                _lstSelectionDeviceSources = value;
                RaisePropertyChanged(() => SelectionDeviceSources);
            }
        }

        #endregion


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

        public string SGId
        {
            get { return _sample == null ? null : _sample.sgId; }
            set
            {
                if (_sample.sgId != value)
                    _sample.sgId = value;
                RaisePropertyChanged(() => SGId);
            }
        }

        List<string> _lstWeekdayWeekendList = new List<string>() { "Hverdag", "Weekend" };

        public List<string> WeekdayWeekendList
        {
            get { return _lstWeekdayWeekendList; }
            set
            {
                _lstWeekdayWeekendList = value;
                RaisePropertyChanged(() => WeekdayWeekendList);
            }
        }

        public string WeekdayWeekend
        {
            get { return _sample == null ? null : _sample.weekdayWeekend; }
            set
            {
                if (_sample.weekdayWeekend != value)
                    _sample.weekdayWeekend = value;
                RaisePropertyChanged(() => WeekdayWeekend);
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


        public DFUPerson SelectedSamplePerson
        {
            get { return _sample == null ? null : _sample.DFUPerson; }
            set
            {
                if (!_blnUpdatingCollections)
                {
                    _sample.DFUPerson = value;
                }
                RaisePropertyChanged(() => SelectedSamplePerson);
            }
        }


        public string StationNumber
        {
            get { return _sample == null ? null : _sample.station; }
            set
            {
                if (_sample.station != value)
                    _sample.station = value;
                RaisePropertyChanged(() => StationNumber);
            }
        }


        public string StationName
        {
            get { return _sample == null ? null : _sample.stationName; }
            set
            {
                if (_sample.stationName != value)
                    _sample.stationName = value;

                RaisePropertyChanged(() => StationName);
            }
        }


        public string LabJournalNumber
        {
            get { return _sample == null ? null : _sample.labJournalNum; }
            set
            {
                if (_sample.labJournalNum != value)
                    _sample.labJournalNum = value;

                RaisePropertyChanged(() => LabJournalNumber);
            }
        }


        public L_SampleType SelectedSampleType
        {
            get { return (_sample == null || _lstSampleTypes == null || _sample.sampleType == null) ? null : _lstSampleTypes.Where(x => x.sampleType.Equals(_sample.sampleType, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(); }
            set
            {
                if (!_blnUpdatingCollections)
                {
                    var val = (value == null ? "" : value.sampleType);
                    if (_sample.sampleType != val)
                        _sample.sampleType = val;
                }

                RaisePropertyChanged(() => SelectedSampleType);
                RaisePropertyChanged(() => HasSelectedSampleType);
                RaisePropertyChanged(() => GearTypes);
                RaisePropertyChanged(() => FishingTime);
            }
        }

      

        public bool HasSelectedSampleType
        {
            get { return SelectedSampleType != null; }
        }


        public L_GearQuality SelectedGearQuality
        {
            get { return (_sample == null || _lstGearQuality == null || _sample.gearQuality == null) ? null : _lstGearQuality.Where(x => x.gearQuality.Equals(_sample.gearQuality, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(); }
            set
            {
                if (!_blnUpdatingCollections)
                {
                    var val = (value == null ? "" : value.gearQuality);
                    if (_sample.gearQuality != val)
                        _sample.gearQuality = val;
                }

                RaisePropertyChanged(() => SelectedGearQuality);
            }
        }


        public L_Species SelectedTargetSpecies
        {
            get { return _selectedSpecies; }
            set
            {
                if (!_blnUpdatingCollections)
                {
                    if((_selectedSpecies == null && value != null) || (_selectedSpecies != null && !_selectedSpecies.Equals(value)))
                    {
                        _selectedSpecies = value;
                        if (_sample != null && IsEdit)
                            _sample.MarkAsModified();
                    }
                }

                RaisePropertyChanged(() => SelectedTargetSpecies);
            }
        }


        public L_TimeZone SelectedTimeZone
        {
            get { return (_sample == null || _lstTimeZones == null) ? null : _lstTimeZones.Where(x => x.timeZone == _sample.timeZone).FirstOrDefault(); }
            set
            {
                if (!_blnUpdatingCollections)
                {
                    if (value == null || (_sample.timeZone != value.timeZone))
                        _sample.timeZone = value == null ? new Nullable<int>() : value.timeZone;
                }

                RaisePropertyChanged(() => SelectedTimeZone);
            }
        }


        private int? _fishingTimeSeconds = null;
        public string FishingTime
        {
            get 
            {
                string str = "N/A";
                if (_sample != null)
                {
                    if (GearStartDateTime.HasValue && GearEndDateTime.HasValue)
                        UpdateFishingTime();

                    if (_fishingTimeSeconds.HasValue)
                        str = SecondsToMinutesAndSeconds(_fishingTimeSeconds.Value);
                }

                return str;
            }
        }


        private DateTime? _gearStartDateTime;
        public DateTime? GearStartDateTime
        {
            get { return _gearStartDateTime; }
            set
            {
                _gearStartDateTime = value;

                //Default end time to start time
                if (!_blnIsImported && !IsEdit)
                {
                    GearEndDateTime = _gearStartDateTime;
                }

                UpdateFishingTime();
                RaisePropertyChanged(() => GearStartDateTime);
                RaisePropertyChanged(() => FishingTime);

                if (IsEdit)
                    _sample.MarkAsModified();
            }
        }

        private DateTime? _gearEndDateTime;
        public DateTime? GearEndDateTime
        {
            get { return _gearEndDateTime; }
            set
            {
                _gearEndDateTime = value;

                UpdateFishingTime();
                RaisePropertyChanged(() => GearEndDateTime);
                RaisePropertyChanged(() => FishingTime);

                if (IsEdit)
                    _sample.MarkAsModified();
            }
        }


        public string StartLatitude
        {
            get { return (_sample == null || _sample.latPosStartText == null) ? _strDefaultLatCoordinates : RefitLatLon(_sample.latPosStartText, true); }
            set
            {
                if (_sample.latPosStartText != value)
                    _sample.latPosStartText = value == null ? null : value.Replace('°', '.').Replace("'", "");

                //Default end latitude to start latitude if it has not been set yet.
                if (/*!IsEdit || */_sample.latPosEndText == null)
                    EndLatitude = value;

                RaisePropertyChanged(() => StartLatitude);
            }
        }


        public string EndLatitude
        {
            get { return (_sample == null || _sample.latPosEndText == null) ? _strDefaultLatCoordinates : RefitLatLon(_sample.latPosEndText, true); }
            set
            {
                if (_sample.latPosEndText != value)
                    _sample.latPosEndText = value == null ? null : value.Replace('°', '.').Replace("'", "");

                RaisePropertyChanged(() => EndLatitude);
            }
        }


        public string StartLongitude
        {
            get { return (_sample == null || _sample.lonPosStartText == null) ? _strDefaultLonCoordinates : RefitLatLon(_sample.lonPosStartText, false); }
            set
            {
                if (_sample.lonPosStartText != value )
                    _sample.lonPosStartText = value == null ? null : value.Replace('°', '.').Replace("'", "");

                //Default end longitude to start longitude if it has not been set yet
                if (/*!IsEdit || */_sample.lonPosEndText == null)
                    EndLongitude = value;

                RaisePropertyChanged(() => StartLongitude);
            }
        }


        public string EndLongitude
        {
            get { return (_sample == null || _sample.lonPosEndText == null) ? _strDefaultLonCoordinates : RefitLatLon(_sample.lonPosEndText, false); }
            set
            {
                if (_sample.lonPosEndText != value)
                    _sample.lonPosEndText = value == null ? null : value.Replace('°', '.').Replace("'", "");

                RaisePropertyChanged(() => EndLongitude);
            }
        }


        public L_DFUArea SelectedArea
        {
            get { return (_sample == null || _lstAreas == null || _sample.dfuArea == null) ? null : _lstAreas.Where(x => x.DFUArea.Equals(_sample.dfuArea, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(); }
            set
            {
                if (!_blnUpdatingCollections)
                {
                    var val = (value == null ? null : value.DFUArea);
                    if (_sample.dfuArea != val)
                        _sample.dfuArea = val;
                }

                RaisePropertyChanged(() => SelectedArea);
            }
        }


        public L_StatisticalRectangle SelectedRectangle
        {
            get { return (_sample == null || _lstStatisticalRectangles == null || _sample.statisticalRectangle == null) ? null : _lstStatisticalRectangles.Where(x => x.statisticalRectangle.Equals(_sample.statisticalRectangle, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault(); }
            set
            {
                if (!_blnUpdatingCollections)
                {
                    var val = (value == null ? null : value.statisticalRectangle);
                    if (_sample.statisticalRectangle != val)
                        _sample.statisticalRectangle = val;
                }

                RaisePropertyChanged(() => SelectedRectangle);
            }
        }

        private bool _blnIsCoordinatesNotKnown;
        public bool IsCoordinatesNotKnown
        {
            get { return _blnIsCoordinatesNotKnown; }
            set
            {
                _blnIsCoordinatesNotKnown = value;
                RaisePropertyChanged(() => IsCoordinatesNotKnown);
            }
        }


        public L_CatchRegistration SelectedCatchRegistration
        {
            get { return (_sample == null || _lstCatchRegistrations == null || _sample.catchRegistrationId == null) ? null : _lstCatchRegistrations.Where(x => x.catchRegistrationId ==_sample.catchRegistrationId).FirstOrDefault(); }
            set
            {
                if (!_blnUpdatingCollections)
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
                if (!_blnUpdatingCollections)
                {
                    var val = (value == null ? new Nullable<int>() : value.speciesRegistrationId);
                    if (_sample.speciesRegistrationId != val)
                        _sample.speciesRegistrationId = val;
                }

                RaisePropertyChanged(() => SelectedSpeciesRegistration);
            }
        }


        public L_Bottomtype SelectedBottomType
        {
            get { return (_sample == null || _lstBottomTypes == null || _sample.bottomType == null) ? null : _lstBottomTypes.Where(x => x.bottomtype == _sample.bottomType).FirstOrDefault(); }
            set
            {
                if (!_blnUpdatingCollections)
                {
                    var val = (value == null ? null : value.bottomtype);
                    if (_sample.bottomType != val)
                        _sample.bottomType = val;
                }

                RaisePropertyChanged(() => SelectedBottomType);
            }
        }


        public L_HaulType SelectedHaulType
        {
            get { return (_sample == null || _lstHaulTypes == null || _sample.haulType == null) ? null : _lstHaulTypes.Where(x => x.haulType == _sample.haulType).FirstOrDefault(); }
            set
            {
                if (!_blnUpdatingCollections)
                {
                    var val = (value == null ? null : value.haulType);
                    if (_sample.haulType != val)
                        _sample.haulType = val;
                }

                RaisePropertyChanged(() => SelectedHaulType);
            }
        }


        public L_ThermoCline SelectedThermoCline
        {
            get { return (_sample == null || _lstThermoClines == null || _sample.thermoCline == null) ? null : _lstThermoClines.Where(x => x.thermoCline == _sample.thermoCline).FirstOrDefault(); }
            set
            {
                if (!_blnUpdatingCollections)
                {
                    var val = (value == null ? null : value.thermoCline);
                    if (_sample.thermoCline != val)
                        _sample.thermoCline = val;
                }

                RaisePropertyChanged(() => SelectedThermoCline);
            }
        }


        public short? WindDirection
        {
            get { return _sample == null ? null : _sample.windDirection; }
            set
            {
                if (_sample.windDirection != value)
                    _sample.windDirection = value;
                RaisePropertyChanged(() => WindDirection);
            }
        }


        public int? WindSpeed
        {
            get { return _sample == null ? null : _sample.windSpeed; }
            set
            {
                if (_sample.windSpeed != value)
                    _sample.windSpeed = value;

                RaisePropertyChanged(() => WindSpeed);
            }
        }


        public L_GearType SelectedGearType
        {
            get { return (_sample == null || _lstGearTypes == null || _sample.gearType == null) ? null : _lstGearTypes.Where(x => x.gearType == _sample.gearType).FirstOrDefault(); }
            set
            {
                if (!_blnUpdatingCollections)
                {
                    var val = (value == null ? null : value.gearType);
                    if (_sample.gearType != val)
                        _sample.gearType = val;
                }

                RaisePropertyChanged(() => SelectedGearType);
                RaisePropertyChanged(() => HasSelectedGearType);
            }
        }


        public bool HasSelectedGearType
        {
            get { return SelectedGearType != null; }
        }


        public L_SelectionDevice SelectedSelectionDevice
        {
            get { return (_sample == null || _lstSelectionDevices == null || _sample.selectionDevice == null) ? null : _lstSelectionDevices.Where(x => x.selectionDevice == _sample.selectionDevice).FirstOrDefault(); }
            set
            {
                if (!_blnUpdatingCollections)
                {
                    var val = (value == null ? null : value.selectionDevice);
                    if (_sample.selectionDevice != val)
                        _sample.selectionDevice = val;
                }

                 RaisePropertyChanged(() => SelectedSelectionDevice);
            }
        }

        public L_SelectionDeviceSource SelectedSelectionDeviceSource
        {
            get { return (_sample == null || _lstSelectionDeviceSources == null || _sample.selectionDeviceSourceId == null) ? null : _lstSelectionDeviceSources.Where(x => x.L_selectionDeviceSourceId == _sample.selectionDeviceSourceId).FirstOrDefault(); }
            set
            {
                if (!_blnUpdatingCollections)
                {
                    var val = (value == null ? new Nullable<int>() : value.L_selectionDeviceSourceId);
                    if (_sample.selectionDeviceSourceId != val)
                        _sample.selectionDeviceSourceId = val;
                }

                RaisePropertyChanged(() => SelectedSelectionDeviceSource);
            }
        }

        /* public decimal? MeshSize
         {
             get { return _sample == null ? null : _sample.meshSize; }
             set
             {
                 if (_sample.meshSize != value)
                     _sample.meshSize = value;
                 RaisePropertyChanged(() => MeshSize);
             }
         }*/

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


        public int? NumberTrawls
        {
            get { return _sample == null ? null : _sample.numberTrawls; }
            set
            {
                if (_sample.numberTrawls != value)
                    _sample.numberTrawls = value;
                RaisePropertyChanged(() => NumberTrawls);
            }
        }


        public decimal? HeightNets
        {
            get { return _sample == null ? null : _sample.heightNets; }
            set
            {
                if (_sample.heightNets != value)
                    _sample.heightNets = value;
                RaisePropertyChanged(() => HeightNets);
            }
        }


        public decimal? LengthNets
        {
            get { return _sample == null ? null : _sample.lengthNets; }
            set
            {
                if (_sample.lengthNets != value)
                    _sample.lengthNets = value;
                RaisePropertyChanged(() => LengthNets);
            }
        }


        public decimal? LengthBeam
        {
            get { return _sample == null ? null : _sample.lengthBeam; }
            set
            {
                if (_sample.lengthBeam != value)
                    _sample.lengthBeam = value;
                RaisePropertyChanged(() => LengthBeam);
            }
        }


        public decimal? LengthRopeFlyer
        {
            get { return _sample == null ? null : _sample.lengthRopeFlyer; }
            set
            {
                if (_sample.lengthRopeFlyer != value)
                    _sample.lengthRopeFlyer = value;
                RaisePropertyChanged(() => LengthRopeFlyer);
            }
        }


        public decimal? WidthRopeFlyer
        {
            get { return _sample == null ? null : _sample.widthRopeFlyer; }
            set
            {
                if (_sample.widthRopeFlyer != value)
                    _sample.widthRopeFlyer = value;
                RaisePropertyChanged(() => WidthRopeFlyer);
            }
        }


        public decimal? DepthAveGear
        {
            get { return _sample == null ? null : _sample.depthAveGear; }
            set
            {
                if (_sample.depthAveGear != value)
                    _sample.depthAveGear = value;
                RaisePropertyChanged(() => DepthAveGear);
            }
        }


        public decimal? HaulSpeedBot
        {
            get { return _sample == null ? null : _sample.haulSpeedBot; }
            set
            {
                if (_sample.haulSpeedBot != value)
                    _sample.haulSpeedBot = value;
                RaisePropertyChanged(() => HaulSpeedBot);
            }
        }


        public int? NumberHooks
        {
            get { return _sample == null ? null : _sample.numberHooks; }
            set
            {
                if (_sample.numberHooks != value)
                    _sample.numberHooks = value;
                RaisePropertyChanged(() => NumberHooks);
            }
        }


        public int? NumNets
        {
            get { return _sample == null ? null : _sample.numNets; }
            set
            {
                if (_sample.numNets != value)
                    _sample.numNets = value;
                RaisePropertyChanged(() => NumNets);
            }
        }


        public int? LostNets
        {
            get { return _sample == null ? null : _sample.lostNets; }
            set
            {
                if (_sample.lostNets != value)
                    _sample.lostNets = value;
                RaisePropertyChanged(() => LostNets);
            }
        }



        public string GearRemark
        {
            get { return _sample == null ? null : _sample.gearRemark; }
            set
            {
                if (_sample.gearRemark != value)
                    _sample.gearRemark = value;
                RaisePropertyChanged(() => GearRemark);
            }
        }


        public string Remark
        {
            get { return _sample == null ? null : _sample.remark; }
            set
            {
                if (_sample.remark != value)
                    _sample.remark = value;
                RaisePropertyChanged(() => Remark);
            }
        }


        public string HydroStation
        {
            get { return _sample == null ? null : _sample.hydroStnRef; }
            set
            {
                if (_sample.hydroStnRef != value)
                    _sample.hydroStnRef = value;

                RaisePropertyChanged(() => HydroStation);
            }
        }

        
        public bool IsCTDInfoVisible
        {
            get { return _blnIsCTDInfoVisible; }
            set
            {
                _blnIsCTDInfoVisible = value;
                RaisePropertyChanged(() => IsCTDInfoVisible);
            }
        }


        public decimal? TemperatureSrf
        {
            get { return _sample == null ? null : _sample.temperatureSrf; }
            set
            {
                if(_sample.temperatureSrf != value)
                    _sample.temperatureSrf = value;

                RaisePropertyChanged(() => TemperatureSrf);
            }
        }


        public decimal? TemperatureBot
        {
            get { return _sample == null ? null : _sample.temperatureBot; }
            set
            {
                if (_sample.temperatureBot != value)
                    _sample.temperatureBot = value;

                RaisePropertyChanged(() => TemperatureBot);
            }
        }


        public decimal? OxygenSrf
        {
            get { return _sample == null ? null : _sample.oxygenSrf; }
            set
            {
                if (_sample.oxygenSrf != value)
                    _sample.oxygenSrf = value;

                RaisePropertyChanged(() => OxygenSrf);
            }
        }


         public decimal? OxygenBot
         {
             get { return _sample == null ? null : _sample.oxygenBot; }
             set
             {
                 if (_sample.oxygenBot != value)
                     _sample.oxygenBot = value;

                 RaisePropertyChanged(() => OxygenBot);
             }
         }


         public decimal? ThermoClineDepth
         {
             get { return _sample == null ? null : _sample.thermoClineDepth; }
             set
             {
                 if (_sample.thermoClineDepth != value)
                     _sample.thermoClineDepth = value;

                 RaisePropertyChanged(() => ThermoClineDepth);
             }
         }


         public decimal? SalinitySrf
         {
             get { return _sample == null ? null : _sample.salinitySrf; }
             set
             {
                 if (_sample.salinitySrf != value)
                     _sample.salinitySrf = value;

                 RaisePropertyChanged(() => SalinitySrf);
             }
         }


         public decimal? SalinityBot
         {
             get { return _sample == null ? null : _sample.salinityBot; }
             set
             {
                 if (_sample.salinityBot != value)
                     _sample.salinityBot = value;

                 RaisePropertyChanged(() => SalinityBot);
             }
         }


         public short? WaveDirection
         {
             get { return _sample == null ? null : _sample.waveDirection; }
             set
             {
                 if (_sample.waveDirection != value)
                     _sample.waveDirection = value;
                 RaisePropertyChanged(() => WaveDirection);
             }
         }


         public decimal? WaveHeight
         {
             get { return _sample == null ? null : _sample.waveHeigth; }
             set
             {
                 if (_sample.waveHeigth != value)
                     _sample.waveHeigth = value;
                 RaisePropertyChanged(() => WaveHeight);
             }
         }

         public short? CurrentDirectionSrf
         {
             get { return _sample == null ? null : _sample.currentDirectionSrf; }
             set
             {
                 if (_sample.currentDirectionSrf != value)
                     _sample.currentDirectionSrf = value;
                 RaisePropertyChanged(() => CurrentDirectionSrf);
             }
         }


         public short? CurrentDirectionBot
         {
             get { return _sample == null ? null : _sample.currentDirectionBot; }
             set
             {
                 if (_sample.currentDirectionBot != value)
                     _sample.currentDirectionBot = value;
                 RaisePropertyChanged(() => CurrentDirectionBot);
             }
         }


         public decimal? CurrentSpeedSrf
         {
             get { return _sample == null ? null : _sample.currentSpeedSrf; }
             set
             {
                 if (_sample.currentSpeedSrf != value)
                     _sample.currentSpeedSrf = value;
                 RaisePropertyChanged(() => CurrentSpeedSrf);
             }
         }


         public decimal? CurrentSpeedBot
         {
             get { return _sample == null ? null : _sample.currentSpeedBot; }
             set
             {
                 if (_sample.currentSpeedBot != value)
                     _sample.currentSpeedBot = value;
                 RaisePropertyChanged(() => CurrentSpeedBot);
             }
         }


         public decimal? DepthAvg
         {
             get { return _sample == null ? null : _sample.depthAvg; }
             set
             {
                 if (_sample.depthAvg != value)
                     _sample.depthAvg = value;
                 RaisePropertyChanged(() => DepthAvg);
             }
         }


         public int? CourseTrack
         {
             get { return _sample == null ? null : _sample.courseTrack; }
             set
             {
                 if (_sample.courseTrack != value)
                     _sample.courseTrack = value;
                 RaisePropertyChanged(() => CourseTrack);
             }
         }

         public decimal? HaulSpeedWat
         {
             get { return _sample == null ? null : _sample.haulSpeedWat; }
             set
             {
                 if (_sample.haulSpeedWat != value)
                     _sample.haulSpeedWat = value;
                 RaisePropertyChanged(() => HaulSpeedWat);
             }
         }


         public short? HaulDirection
         {
             get { return _sample == null ? null : _sample.haulDirection; }
             set
             {
                 if (_sample.haulDirection != value)
                     _sample.haulDirection = value;
                 RaisePropertyChanged(() => HaulDirection);
             }
         }


         public decimal? NetOpening
         {
             get { return _sample == null ? null : _sample.netOpening; }
             set
             {
                 if (_sample.netOpening != value)
                     _sample.netOpening = value;
                 RaisePropertyChanged(() => NetOpening);
             }
         }


         public decimal? ShovelDist
         {
             get { return _sample == null ? null : _sample.shovelDist; }
             set
             {
                 if (_sample.shovelDist != value)
                     _sample.shovelDist = value;
                 RaisePropertyChanged(() => ShovelDist);
             }
         }


         public int? WireLength
         {
             get { return _sample == null ? null : _sample.wireLength; }
             set
             {
                 if (_sample.wireLength != value)
                     _sample.wireLength = value;
                 RaisePropertyChanged(() => WireLength);
             }
         }


         public decimal? WingSpread
         {
             get { return _sample == null ? null : _sample.wingSpread; }
             set
             {
                 if (_sample.wingSpread != value)
                     _sample.wingSpread = value;
                 RaisePropertyChanged(() => WingSpread);
             }
         }


        #region Is focused properties


        public bool IsStationNumberFocused
        {
            get { return _blnIsStationNumberFocused; }
            set
            {
                _blnIsStationNumberFocused = value;
                RaisePropertyChanged(() => IsStationNumberFocused);
            }
        }

        
        public bool IsSGIdFocused
        {
            get { return _blnIsSGIdFocused; }
            set
            {
                _blnIsSGIdFocused = value;
                RaisePropertyChanged(() => IsSGIdFocused);
            }
        }


        #endregion


        #region Is loading properties


        public bool IsSquareLoading
        {
            get { return _blnIsSquareLoading; }
            set
            {
                _blnIsSquareLoading = value;
                RaisePropertyChanged(() => IsSquareLoading);
            }
        }



        public bool IsGearTypesLoading
        {
            get { return _blnIsGearTypesLoading; }
            set
            {
                _blnIsGearTypesLoading = value;
                RaisePropertyChanged(() => IsGearTypesLoading);
            }
        }


        #endregion


        #region Info header properties

        public string CruiseTitle
        {
            get { return _cruise == null ? null : _cruise.cruise1; }
        }

        public int? CruiseYear
        {
            get { return _cruise == null ? new Nullable<int>() : _cruise.year; }
        }

        public string TripNumber
        {
            get { return _trip == null ? null : _trip.trip1; }
        }

        public string TripType
        {
            get { return _trip == null ? null : _trip.tripType; }
        }

        public bool IsScientific
        {
            get { return TripType != null && TripType.Equals("vid", StringComparison.InvariantCultureIgnoreCase); }
        }

        public bool IsRecreationalFishery
        {
            get { return TripType != null && TripType.IsREK(); }
        }

        #endregion


        #endregion


        public StationViewModel(int? intSampleId = null, int? intTripId = null)
            : base()
        {
            _intEditingSampleId = intSampleId;
            IsEdit = intSampleId.HasValue;
            _intTripId = intTripId;

            if (!intSampleId.HasValue && !intTripId.HasValue)
                throw new ApplicationException("Cannot create a new StationViewModel without a trip id.");

            InitializeAsync();
            RegisterToKeyDown();
        }


        /// <summary>
        /// Constructor should only be used for creating a StationViewModel from an imported Sample record (from sis)
        /// </summary>
        private StationViewModel(Sample s) 
            : base()
        {
            _blnIsImported = true;
            _intEditingSampleId = null;
            IsEdit = _intEditingSampleId.HasValue;
            _intTripId = s.tripId;

            _sample = s;

            InitializeAsync();
            RegisterToKeyDown();
        }


        public static StationViewModel NewViewModelFromImportedStation(Sample s)
        {
            return new StationViewModel(s);
        }



        #region Initialize methods


        private Task InitializeAsync()
        {
            IsLoading = true;
            var tt = Task.Factory.StartNew(() => Initialize()).ContinueWith(t => new Action(() => { IsLoading = false; ScrollTo("Home"); RaisePropertyChanged(() => CanEditOffline); }).Dispatch());

            if (IsEdit)
                Task.Factory.StartNew(() => InitializeMap(_intEditingSampleId, TreeNodeLevel.Sample));

            return tt;
        }


        private void CopyValuesFromLatestSample(DataInputManager datMan, ref Sample s)
        {
            try
            {
                Sample sLatest = datMan.GetLatestSampleFromTripId(s.tripId);

                if (sLatest != null)
                {
                    s.sampleType = sLatest.sampleType;
                    s.gearType = sLatest.gearType;
                    s.selectionDevice = sLatest.selectionDevice;
                    s.meshSize = sLatest.meshSize;
                    s.numberTrawls = sLatest.numberTrawls;
                    s.lengthBeam = sLatest.lengthBeam;
                    s.heightNets = sLatest.heightNets;
                    s.lengthNets = sLatest.lengthNets;
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }

        private void Initialize(bool blnFullInitialize = true)
        {
            try
            {
                var man = new DataInputManager();

                Sample s = _sample;
         
                if (blnFullInitialize)
                {
                    if (IsEdit)
                    {
                        s = man.GetEntity<Sample>(_intEditingSampleId.Value);
                        if (s == null)
                        {
                            DispatchMessageBox("Det var ikke muligt at hente den ønskede station.");
                            return;
                        }
                    }
                    else
                    {
                        if (_blnIsImported)
                            s = _sample;
                        else
                        {
                            s = new Sample();
                            s.dateGearStart = DateTime.UtcNow.AddMinutes(-1);
                            s.dateGearEnd = DateTime.UtcNow;
                            s.@virtual = "nej";
                            s.tripId = _intTripId.Value;

                            CopyValuesFromLatestSample(man, ref s);
                        }
                    }
                }

                if (blnFullInitialize)
                {
                    _trip = man.GetEntity<Trip>(s.tripId);
                    _cruise = man.GetEntity<Cruise>(_trip.cruiseId);
                }

                var lookupMan = new BusinessLogic.LookupManager();

                List<L_HaulType> lstHaulTypes = null;
                List<L_ThermoCline> lstThermoClines = null;
                List<L_DFUArea> lstAreas = null;
                List<L_StatisticalRectangle> lstRectangles = null;
                List<L_Species> lstSpecies = null;
                List<L_CatchRegistration> lstCatchRegistrations = null;
                List<L_Bottomtype> lstBottomTypes = null;
                List<DFUPerson> lstDFUPersons = null;

                var lv = new LookupDataVersioning();

                var lstSampleTypes = lookupMan.GetLookups(typeof(L_SampleType), lv).OfType<L_SampleType>().OrderBy(x => x.UIDisplay).ToList();
                var lstGearQualities = lookupMan.GetLookups(typeof(L_GearQuality), lv).OfType<L_GearQuality>().OrderBy(x => x.UIDisplay).ToList();
                var lstSpeciesRegistrations = lookupMan.GetLookups(typeof(L_SpeciesRegistration), lv).OfType<L_SpeciesRegistration>().OrderBy(x => x.UIDisplay).ToList();
                var lstGearTypes = lookupMan.GetLookups(typeof(L_GearType), lv).OfType<L_GearType>().OrderBy(x => x.UIDisplay).ToList();
                var lstSelectionDevices = s.gearType != null ? man.GetSelectionDevicesFromGearType(s.gearType) : new List<L_SelectionDevice>();
                var lstTimeZones = lookupMan.GetLookups(typeof(L_TimeZone), lv).OfType<L_TimeZone>().OrderBy(x => x.timeZone).ToList();
                var lstSelectionDeviceSources = lookupMan.GetLookups(typeof(L_SelectionDeviceSource), lv).OfType<L_SelectionDeviceSource>().OrderBy(x => x.num).ThenBy(x => x.UIDisplay).ToList();

                if (IsScientific)
                {
                    lstHaulTypes = lookupMan.GetLookups(typeof(L_HaulType), lv).OfType<L_HaulType>().OrderBy(x => x.UIDisplay).ToList();
                    lstThermoClines = lookupMan.GetLookups(typeof(L_ThermoCline), lv).OfType<L_ThermoCline>().OrderBy(x => x.UIDisplay).ToList();
                    lstBottomTypes = lookupMan.GetLookups(typeof(L_Bottomtype), lv).OfType<L_Bottomtype>().OrderBy(x => x.UIDisplay).ToList();
                }
                else
                {
                    lstAreas = lookupMan.GetRectangleAreas().OrderBy(x => x.UIDisplay).ToList();
                    lstRectangles = s.dfuArea != null ? man.GetStatisticalRectangleFromArea(s.dfuArea) : new List<L_StatisticalRectangle>();
                    lstSpecies = lookupMan.GetLookups(typeof(L_Species), lv).OfType<L_Species>().OrderBy(x => x.UIDisplay).ToList();
                    lstCatchRegistrations = lookupMan.GetLookups(typeof(L_CatchRegistration), lv).OfType<L_CatchRegistration>().OrderBy(x => x.UIDisplay).ToList();
                }

                if(IsRecreationalFishery)
                {
                    lstDFUPersons = lookupMan.GetLookups(typeof(DFUPerson), lv).OfType<DFUPerson>().OrderBy(x => x.initials).ToList();
                }

                new Action(() =>
                {
                    _blnUpdatingCollections = true;

                    //First set lookup lists
                    SampleTypes = lstSampleTypes;
                    GearQualities = lstGearQualities;
                    TargetSpecies = lstSpecies;
                    Areas = lstAreas;
                    StatisticalRectangles = lstRectangles;
                    SpeciesRegistrations = lstSpeciesRegistrations;
                    CatchRegistrations = lstCatchRegistrations;
                    GearTypes = lstGearTypes;
                    SelectionDevices = lstSelectionDevices;
                    TimeZones = lstTimeZones;

                    SelectionDeviceSources = lstSelectionDeviceSources;
                    
                    HaulTypes = lstHaulTypes;
                    ThermoClines = lstThermoClines;
                    BottomTypes = lstBottomTypes;

                    DFUPersons = lstDFUPersons;

                    _blnUpdatingCollections = false;


                    //Assign trip (it's important the trip is assigned after the lookup lists (or the selected values on trip will be overwritten with null).
                    if(blnFullInitialize)
                        Sample = s;

                    AssignSelectedLookups();

                    if (blnFullInitialize)
                    {
                        //Assign date and time properties.
                        _gearStartDateTime = Sample.dateGearStart;
                        _gearEndDateTime = Sample.dateGearEnd;

                        if (_gearStartDateTime.HasValue && SelectedTimeZone != null)
                            GearStartDateTime = _gearStartDateTime.Value.AddHours(SelectedTimeZone.timeZone);
                        else
                            RaisePropertyChanged(() => GearStartDateTime);

                        if (_gearEndDateTime.HasValue && SelectedTimeZone != null)
                            GearEndDateTime = _gearEndDateTime.Value.AddHours(SelectedTimeZone.timeZone);
                        else
                            RaisePropertyChanged(() => GearEndDateTime);

                        //Set UI Focus
                        if (!IsEdit)
                        {
                            if (IsRecreationalFishery)
                                IsSGIdFocused = true;
                            else
                                IsStationNumberFocused = true;

                            //Default to Trip timezone
                            if (!_blnIsImported && _trip.timeZone.HasValue)
                               SelectedTimeZone = TimeZones.Where(x => x.timeZone == _trip.timeZone.Value).FirstOrDefault();

                            //Default start time to trip start time (end time is set automatically in the GearStartDateTime property)
                            if (!_blnIsImported && _trip.dateStart.HasValue && _trip.timeZone.HasValue)
                                GearStartDateTime = _trip.dateStart.Value.AddHours(_trip.timeZone.Value);
                            else
                                RaisePropertyChanged(() => GearStartDateTime);
                        }
                        else //Reset any changes done to trip during initialization (so it is not dirty)
                        {
                            if(MapViewModel != null)
                                MapViewModel.WindowTitle = string.Format("Togt: {0}, Tur: {1}, Station: {2}", _cruise == null ? "" : _cruise.cruise1, _trip == null ? "" : _trip.trip1, s.station);

                            if (!TripType.IsVID() && Sample.latPosStartText == null && Sample.latPosEndText == null && Sample.lonPosEndText == null && Sample.lonPosStartText == null)
                                IsCoordinatesNotKnown = true;

                            if (Sample.temperatureSrf.HasValue || Sample.temperatureBot.HasValue || Sample.oxygenSrf.HasValue || Sample.oxygenBot.HasValue ||
                               !string.IsNullOrEmpty(Sample.thermoCline) || Sample.thermoClineDepth.HasValue || Sample.salinitySrf.HasValue || Sample.salinityBot.HasValue)
                                IsCTDInfoVisible = true;

                            Sample.AcceptChanges();
                            RaisePropertyChanged(() => Sample);
                        }
                    }

                    RaisePropertyChanged(() => TripType);
                    RaisePropertyChanged(() => IsScientific);
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
            if ((!IsEdit || _sample.timeZone.HasValue) && _lstTimeZones != null)
            {
                int intTimeZone = _sample.timeZone.HasValue ? _sample.timeZone.Value : 0;
                SelectedTimeZone = _lstTimeZones.Where(x => x.timeZone == intTimeZone).FirstOrDefault();
            }

            if (_sample.sampleType != null && _lstSampleTypes != null)
                SelectedSampleType = _lstSampleTypes.Where(x => x.sampleType == _sample.sampleType).FirstOrDefault();

            if (_sample.gearQuality != null && _lstGearQuality != null)
                SelectedGearQuality = _lstGearQuality.Where(x => x.gearQuality == _sample.gearQuality).FirstOrDefault();

            if (_sample.R_TargetSpecies != null && _sample.R_TargetSpecies.Count > 0 && _lstTargetSpecies != null)
            {
                var targetSpecies = _sample.R_TargetSpecies.FirstOrDefault();
                SelectedTargetSpecies = _lstTargetSpecies.Where(x => targetSpecies.speciesCode.Equals(x.speciesCode)).FirstOrDefault();
            }

            if (_sample.dfuArea != null && _lstAreas != null)
                SelectedArea = _lstAreas.Where(x => x.DFUArea == _sample.dfuArea).FirstOrDefault();

            if(_sample.statisticalRectangle != null && _lstStatisticalRectangles != null)
                SelectedRectangle = _lstStatisticalRectangles.Where(x => x.statisticalRectangle == _sample.statisticalRectangle).FirstOrDefault();

            if (_sample.speciesRegistrationId != null && _lstSpeciesRegistrations != null)
                SelectedSpeciesRegistration = _lstSpeciesRegistrations.Where(x => x.speciesRegistrationId == _sample.speciesRegistrationId).FirstOrDefault();

            if (_sample.catchRegistrationId != null && _lstCatchRegistrations != null)
                SelectedCatchRegistration = _lstCatchRegistrations.Where(x => x.catchRegistrationId == _sample.catchRegistrationId).FirstOrDefault();

            if (_sample.gearType != null && _lstGearTypes != null)
                SelectedGearType = _lstGearTypes.Where(x => x.gearType.Equals(_sample.gearType)).FirstOrDefault();

            if (_sample.selectionDevice != null && _lstSelectionDevices != null)
                SelectedSelectionDevice = _lstSelectionDevices.Where(x => x.selectionDevice == _sample.selectionDevice).FirstOrDefault();

            if (_sample.selectionDeviceSourceId != null && _lstSelectionDeviceSources != null)
                SelectedSelectionDeviceSource = _lstSelectionDeviceSources.Where(x => x.L_selectionDeviceSourceId == _sample.selectionDeviceSourceId).FirstOrDefault();

            if (_sample.bottomType != null && _lstBottomTypes != null)
                SelectedBottomType = _lstBottomTypes.Where(x => x.bottomtype == _sample.bottomType).FirstOrDefault();

            if (_sample.haulType != null && _lstHaulTypes != null)
                SelectedHaulType = _lstHaulTypes.Where(x => x.haulType == _sample.haulType).FirstOrDefault();

            if (_sample.thermoCline != null && _lstThermoClines != null)
                SelectedThermoCline = _lstThermoClines.Where(x => x.thermoCline == _sample.thermoCline).FirstOrDefault();

            if (_sample.samplePersonId.HasValue && _lstDFUPersons != null)
                SelectedSamplePerson = _lstDFUPersons.Where(x => x.dfuPersonId == _sample.samplePersonId.Value).FirstOrDefault();

        }


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



        public void LoadSelectionDevicesAsync(string strGearType)
        {
            IsGearTypesLoading = true;
            Task.Factory.StartNew(() => LoadSelectionDevices(strGearType)).ContinueWith(t => new Action(() => { IsGearTypesLoading = false; }).Dispatch());
        }

        private void LoadSelectionDevices(string strGearType)
        {
            var man = new BusinessLogic.DataInput.DataInputManager();
            var lst = strGearType == null ? null : man.GetSelectionDevicesFromGearType(strGearType);

            new Action(() =>
            {
                _blnUpdatingCollections = true;
                {
                SelectionDevices = lst;
                }
                _blnUpdatingCollections = false;

                if (_sample.selectionDevice != null && _lstSelectionDevices != null)
                    SelectedSelectionDevice = _lstSelectionDevices.Where(x => x.selectionDevice == _sample.selectionDevice).FirstOrDefault();

                RaisePropertyChanged(() => HasSelectedGearType);

            }).Dispatch();
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

            //MainTree.DeselectAllAsync();

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
            var vm = new ViewModels.Input.TripViewModel(_sample.tripId);
            bool blnSuccess = AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, vm);
            //MainTree.DeselectAllAsync();

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
            TripViewModel.NewStation(_trip.tripId);
        }


        #endregion


        /// <summary>
        /// Database Numeric(6,2) validator (number of decimals is actual not important, since these are rounded on database insert)
        /// </summary>
        static Regex _regNumeric62 = new Regex(@"^-?\d{0,4}(\.\d*)?$");

        /// <summary>
        /// Database Numeric(5,1) validator (number of decimals is actual not important, since these are rounded on database insert)
        /// </summary>
        static Regex _regNumeric51 = new Regex(@"^-?\d{0,4}(\.\d*)?$");

        /// <summary>
        /// Database Numeric(5,2) validator (number of decimals is actual not important, since these are rounded on database insert)
        /// </summary>
        static Regex _regNumeric52 = new Regex(@"^-?\d{0,3}(\.\d*)?$");

        /// <summary>
        /// Database Numeric(4,1) validator (number of decimals is actual not important, since these are rounded on database insert)
        /// </summary>
        static Regex _regNumeric41 = new Regex(@"^-?\d{0,3}(\.\d*)?$");

        /// <summary>
        /// Database Numeric(4,2) validator (number of decimals is actual not important, since these are rounded on database insert)
        /// </summary>
        static Regex _regNumeric42 = new Regex(@"^-?\d{0,2}(\.\d*)?$");

        /// <summary>
        /// Database Numeric(3,1) validator (number of decimals is actual not important, since these are rounded on database insert)
        /// </summary>
        static Regex _regNumeric31 = new Regex(@"^-?\d{0,2}(\.\d*)?$");


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
                            strError = IsRecreationalFishery ? "Angiv venligst respondent nr." : "Angiv venligst stations nr.";
                        else if (StationNumber != null && StationNumber.Length > 6)
                            strError = string.Format("'Stations nr.' må kun bestå af 6 tegn. Det består pt. af {0} tegn.", StationNumber.Length);

                        if (_blnValidate && strError != null)
                            ScrollTo(() => StationNumber);
                        break;

                    case "SGId":
                        if (_blnValidate && string.IsNullOrWhiteSpace(SGId) && IsRecreationalFishery)
                            strError = "Angiv venligst survey gizmo id.";
                        else if(SGId != null && SGId.Length > 200)
                            strError = string.Format("'Survey gizmo id' må kun bestå af 200 tegn. Det består pt. af {0} tegn.", SGId.Length);

                        if (_blnValidate && strError != null)
                            ScrollTo(() => SGId);
                        break;

                    case "LabJournalNumber":
                        if (LabJournalNumber != null && LabJournalNumber.Length > 6)
                            strError = string.Format("'Laboratorium journal nr.' må kun bestå af 6 tegn. Det består pt. af {0} tegn.", LabJournalNumber.Length);

                        if (_blnValidate && strError != null)
                            ScrollTo(() => LabJournalNumber);
                        break;

                    case "StationName":
                        if (StationName != null && StationName.Length > 10)
                            strError = string.Format("'Fast station' må kun bestå af 10 tegn. Det består pt. af {0} tegn.", StationName.Length);

                        if (_blnValidate && strError != null)
                            ScrollTo(() => StationName);
                        break;

                    case "HydroStation":
                        if (HydroStation != null && HydroStation.Length > 6)
                            strError = string.Format("'CTD station' må kun bestå af 6 tegn. Det består pt. af {0} tegn.", HydroStation.Length);

                        if (_blnValidate && strError != null)
                            ScrollTo(() => HydroStation);
                        break;


                    case "SelectedSampleType":
                        if (_blnValidate && SelectedSampleType == null)
                            strError = "Angiv venligst en redskabsgruppe.";

                        if (_blnValidate && strError != null)
                            ScrollTo(() => SelectedSampleType);
                        break;

                    case "GearStartDateTime":
                        if (_blnValidate && GearStartDateTime == null)
                            strError = "Angiv venligst dato og tid for redskab sat.";

                        if (_blnValidate && strError != null)
                            ScrollTo(() => GearStartDateTime);
                        else if (strError == null) //Handle warnings
                        {
                            if (_trip != null && _trip.dateStart.HasValue && _trip.dateEnd.HasValue && SelectedTimeZone != null)
                            {
                                //Convert trip times to same timezone as specified here
                                DateTime tripStart = _trip.dateStart.Value.AddHours(SelectedTimeZone.timeZone);
                                DateTime tripStop = _trip.dateEnd.Value.AddHours(SelectedTimeZone.timeZone);

                                if (GearStartDateTime.HasValue && (GearStartDateTime < tripStart || GearStartDateTime > tripStop))
                                    strError = String.Format("{0}Redskab sat er udenfor turens start ({1}) og stop ({2}) tid.", WarningPrefix, tripStart.ToString("dd-MM-yyyy HH:mm"), tripStop.ToString("dd-MM-yyyy HH:mm"));
                            }
                        }
                        break;

                    case "GearEndDateTime":
                        if (_blnValidate && GearEndDateTime == null)
                            strError = "Angiv venligst dato og tid for redskab bjærget.";
                        else if(GearStartDateTime != null && GearEndDateTime != null && GearStartDateTime.Value > GearEndDateTime.Value)
                            strError = "Redskab sat skal komme efter redskab bjærget.";

                        if (_blnValidate && strError != null)
                            ScrollTo(() => GearEndDateTime);
                        else if (strError == null) //Handle warnings
                        {
                            if (_trip != null && _trip.dateStart.HasValue && _trip.dateEnd.HasValue && SelectedTimeZone != null)
                            {
                                //Convert trip times to same timezone as specified here
                                DateTime tripStart = _trip.dateStart.Value.AddHours(SelectedTimeZone.timeZone);
                                DateTime tripStop = _trip.dateEnd.Value.AddHours(SelectedTimeZone.timeZone);

                                if (GearEndDateTime.HasValue && (GearEndDateTime < tripStart || GearEndDateTime > tripStop))
                                    strError = String.Format(string.Format("{0}Redskab bjærget er udenfor turens start ({1}) og stop ({2}) tid.", WarningPrefix, tripStart.ToString("dd-MM-yyyy HH:mm"), tripStop.ToString("dd-MM-yyyy HH:mm")));
                            }
                        }
                        break;

                    case "SelectedTimeZone":
                        if (SelectedTimeZone == null && (GearStartDateTime.HasValue || GearEndDateTime.HasValue))
                        {
                            strError = "Vælg venligst en tidszone for den indtastede tid.";

                            if(_blnValidate)
                                ScrollTo(() => SelectedSampleType);
                        }

                        break;

                    case "SelectedCatchRegistration":
                        if (TripType.IsSEA() && SelectedCatchRegistration != null && !SelectedCatchRegistration.catchRegistration.Equals("non", StringComparison.InvariantCultureIgnoreCase) &&
                            SelectedSpeciesRegistration != null && SelectedSpeciesRegistration.speciesRegistration.Equals("non", StringComparison.InvariantCultureIgnoreCase))
                            strError = string.Format("Når oparbejdning af arter er 'NON', skal prøvetagningsniveau også være 'NON'.", CruiseYear.Value);

                        if (_blnValidate && strError != null)
                            ScrollTo("CatchRegistration");
                        break;

                    case "SelectedSpeciesRegistration":
                        if (TripType.IsSEA() &&  SelectedCatchRegistration != null && SelectedCatchRegistration.catchRegistration.Equals("non", StringComparison.InvariantCultureIgnoreCase) &&
                          (SelectedSpeciesRegistration == null || !SelectedSpeciesRegistration.speciesRegistration.Equals("non", StringComparison.InvariantCultureIgnoreCase)))
                            strError = string.Format("Når prøvetagningsniveau er 'NON', skal oparbejdning af arter også sættes til 'NON'.", CruiseYear.Value);

                        if (_blnValidate && strError != null)
                            ScrollTo("SpeciesRegistration");
                        break;

                    case "WindSpeed":
                        if (WindSpeed != null && WindSpeed.Value > 50)
                            strError = "'Vindhastighed' skal være mellem 0 og 50 m/s.";

                        if (_blnValidate && strError != null)
                            ScrollTo("WindSpeed");
                        break;

                    case "WindDirection":
                        if (WindDirection != null && (WindDirection.Value < 0 || WindDirection.Value > 359))
                            strError = "'Vindretning' skal være mellem 0° og 359°.";

                        if (_blnValidate && strError != null)
                            ScrollTo("WindDirection");
                        break;

                    case "CurrentDirectionSrf":
                        if(CurrentDirectionSrf != null && (CurrentDirectionSrf.Value < 0 || CurrentDirectionSrf.Value > 359))
                            strError = "'Strømretning, overflade' skal være mellem 0° og 359°.";

                        if (_blnValidate && strError != null)
                            ScrollTo("CurrentDirectionSrf");
                        break;

                    case "CurrentSpeedSrf":
                        if (CurrentSpeedSrf.HasValue && !_regNumeric41.IsMatch(CurrentSpeedSrf.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Strømhast. overfl. (m/s)' må ikke være større end 999.9.";

                        if (_blnValidate && strError != null)
                            ScrollTo("CurrentSpeedSrf");
                        break;

                    case "CurrentSpeedBot":
                        if (CurrentSpeedBot.HasValue && !_regNumeric41.IsMatch(CurrentSpeedBot.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Strømhast. bund (m/s)' må ikke være større end 999.9.";

                        if (_blnValidate && strError != null)
                            ScrollTo("CurrentSpeedBot");
                        break;

                    case "CurrentDirectionBot":
                        if (CurrentDirectionBot != null && (CurrentDirectionBot.Value < 0 || CurrentDirectionBot.Value > 359))
                            strError = "'Strømretning, bund' skal være mellem 0° og 359°.";

                        if (_blnValidate && strError != null)
                            ScrollTo("CurrentDirectionBot");
                        break;

                    case "WaveDirection":
                        if (WaveDirection != null && (WaveDirection.Value < 0 || WaveDirection.Value > 359))
                            strError = "'Bølgeretning' skal være mellem 0° og 359°.";

                        if (_blnValidate && strError != null)
                            ScrollTo("WaveDirection");
                        break;

                    case "WaveHeight":
                        if (WaveHeight.HasValue && !_regNumeric31.IsMatch(WaveHeight.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Bølgehøjdeequiv. (Bf)' må ikke være større end 99.9.";

                         if (_blnValidate && strError != null)
                             ScrollTo("WaveHeight");
                        break;

                    case "DepthAvg":
                        if (DepthAvg.HasValue && !_regNumeric51.IsMatch(DepthAvg.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Dybde, bund (m)' må ikke være større end 9999.9.";

                         if (_blnValidate && strError != null)
                             ScrollTo("DepthAvg");
                        break;

                    case "SalinityBot":
                        if (SalinityBot.HasValue && !_regNumeric62.IsMatch(SalinityBot.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Salinitet, bund (PSU)' må ikke være større end 9999.99.";

                        if (_blnValidate && strError != null)
                            ScrollTo("SalinityBot");
                        break;

                    case "SalinitySrf":
                        if (SalinitySrf.HasValue && !_regNumeric62.IsMatch(SalinitySrf.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Salinitet, overflade (PSU)' må ikke være større end 9999.99.";

                        if (_blnValidate && strError != null)
                            ScrollTo("SalinitySrf");
                        break;

                    case "OxygenBot":
                        if (OxygenBot.HasValue && !_regNumeric62.IsMatch(OxygenBot.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                             strError = "'Ilt, bund (ml/l)' må ikke være større end 9999.99.";

                        if (_blnValidate && strError != null)
                            ScrollTo("OxygenBot");
                        break;

                    case "OxygenSrf":
                        if (OxygenSrf.HasValue && !_regNumeric62.IsMatch(OxygenSrf.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Ilt, overflade (ml/l)' må ikke være større end 9999.99.";

                        if (_blnValidate && strError != null)
                            ScrollTo("OxygenSrf");
                        break;

                    case "ThermoClineDepth":
                        if (ThermoClineDepth.HasValue && !_regNumeric62.IsMatch(ThermoClineDepth.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Springlag, dybde (m)' må ikke være større end 9999.99.";

                        if (_blnValidate && strError != null)
                            ScrollTo("ThermoClineDepth");
                        break;

                    case "TemperatureBot":
                        if (TemperatureBot.HasValue && !_regNumeric62.IsMatch(TemperatureBot.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Temperatur, bund (°)' må ikke være større end 9999.99.";

                        if (_blnValidate && strError != null)
                            ScrollTo("TemperatureBot");
                        break;

                    case "TemperatureSrf":
                        if (TemperatureSrf.HasValue && !_regNumeric62.IsMatch(TemperatureSrf.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Temperatur, overflade (°)' må ikke være større end 9999.99.";

                        if (_blnValidate && strError != null)
                            ScrollTo("TemperatureSrf");
                        break;

                    case "MeshSize":
                        if (MeshSize.HasValue && !_regNumeric51.IsMatch(MeshSize.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Maskevidde (helmasker, mm)' må ikke være større end 9999.";

                        if (_blnValidate && strError != null)
                            ScrollTo("MeshSize");
                        else if (strError == null) //Handle warnings, if no errors was found
                        {
                            if (SelectedSampleType != null && !IsRecreationalFishery)
                            {
                                if (!MeshSize.HasValue && !SelectedSampleType.sampleType.Equals("K", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    string strMeshSizeName = (SelectedSampleType.sampleType.Equals("A", StringComparison.InvariantCultureIgnoreCase) ||
                                                              SelectedSampleType.sampleType.Equals("C", StringComparison.InvariantCultureIgnoreCase) ||
                                                              SelectedSampleType.sampleType.Equals("E", StringComparison.InvariantCultureIgnoreCase)) ? "Posemaskevidde" : "Maskevidde";
                                    strError = String.Format(string.Format("{0}{1} er ikke udfyldt.", WarningPrefix, strMeshSizeName));
                                }
                            }
                        }
                        break;

                    case "HeightNets":
                        if (HeightNets.HasValue && !_regNumeric51.IsMatch(HeightNets.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Højde i garn (antal knuder)' må ikke være større end 9999.9";

                        if (_blnValidate && strError != null)
                            ScrollTo("HeightNets");
                        break;

                    case "LengthNets":
                        if (LengthNets.HasValue && !_regNumeric51.IsMatch(LengthNets.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Længde af garn (m)' må ikke være større end 9999.9";

                        if (_blnValidate && strError != null)
                            ScrollTo("LengthNets");
                        break;

                    case "LengthBeam":
                        if (LengthBeam.HasValue && !_regNumeric51.IsMatch(LengthBeam.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Længde af bom (m)' må ikke være større end 9999.9";

                        if (_blnValidate && strError != null)
                            ScrollTo("LengthBeam");
                        break;


                    case "HaulSpeedBot":
                        if (HaulSpeedBot.HasValue && !_regNumeric31.IsMatch(HaulSpeedBot.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Trækhastighed, bund (kn)' må ikke være større end 99.9";

                        if (_blnValidate && strError != null)
                            ScrollTo("HaulSpeedBot");
                        break;

                    case "HaulSpeedWat":
                        if (HaulSpeedWat.HasValue && !_regNumeric31.IsMatch(HaulSpeedWat.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Trækhastighed, vand (kn)' må ikke være større end 99.9";

                        if (_blnValidate && strError != null)
                            ScrollTo("HaulSpeedWat");
                        break;

                    case "HaulDirection":
                        if (HaulDirection != null && (HaulDirection.Value < 0 || HaulDirection.Value > 359))
                            strError = "'Trækretning (°)' skal være mellem 0° og 359°.";

                        if (_blnValidate && strError != null)
                            ScrollTo("HaulDirection");
                        break;

                    case "DepthAveGear":
                        if (DepthAveGear.HasValue && !_regNumeric51.IsMatch(DepthAveGear.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Fiskedybde (m)' må ikke være større end 9999.9";

                        if (_blnValidate && strError != null)
                            ScrollTo("DepthAveGear");
                        break;

                    case "NetOpening":
                        if (NetOpening.HasValue && !_regNumeric31.IsMatch(NetOpening.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Netåbning (m)' må ikke være større end 99.9";

                        if (_blnValidate && strError != null)
                            ScrollTo("NetOpening");
                        break;

                    case "ShovelDist":
                        if (ShovelDist.HasValue && !_regNumeric51.IsMatch(ShovelDist.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Skovlafstand (m)' må ikke være større end 9999.9";

                        if (_blnValidate && strError != null)
                            ScrollTo("ShovelDist");
                        break;

                    case "WingSpread":
                        if (WingSpread.HasValue && !_regNumeric31.IsMatch(WingSpread.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Vingebredde (m)' må ikke være større end 99.9";

                        if (_blnValidate && strError != null)
                            ScrollTo("WingSpread");
                        break;

                    case "LengthRopeFlyer":
                        if (LengthRopeFlyer.HasValue && !_regNumeric51.IsMatch(LengthRopeFlyer.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Meter tov' må ikke være større end 9999.9";

                        if (_blnValidate && strError != null)
                            ScrollTo("LengthRopeFlyer");
                        break;

                    case "WidthRopeFlyer":
                        if (WidthRopeFlyer.HasValue && !_regNumeric51.IsMatch(WidthRopeFlyer.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)))
                            strError = "'Tov tykkelse (mm)' må ikke være større end 9999.9";

                        if (_blnValidate && strError != null)
                            ScrollTo("WidthRopeFlyer");
                        break;

                    case "GearRemark":
                        if (GearRemark != null && GearRemark.Length > 450)
                            strError = string.Format("'Redskabsbemærkninger' må kun bestå af 450 tegn. Det består pt. af {0} tegn.", GearRemark.Length);

                        if (_blnValidate && strError != null)
                            ScrollTo("GearRemark");
                        break;

                    case "Remark":
                        if (Remark != null && Remark.Length > 450)
                            strError = string.Format("'Bemærkninger' må kun bestå af 450 tegn. Det består pt. af {0} tegn.", Remark.Length);

                        if (_blnValidate && strError != null)
                            ScrollTo("Remark");
                        break;

                    case "FishingTime":
                        if (_fishingTimeSeconds.HasValue)
                        {
                            if (SelectedSampleType != null && SelectedSampleType.sampleType != null && !IsRecreationalFishery &&
                                                         (SelectedSampleType.sampleType.Equals("A", StringComparison.InvariantCultureIgnoreCase) ||
                                                          SelectedSampleType.sampleType.Equals("C", StringComparison.InvariantCultureIgnoreCase) ||
                                                          SelectedSampleType.sampleType.Equals("D", StringComparison.InvariantCultureIgnoreCase) ||
                                                          SelectedSampleType.sampleType.Equals("E", StringComparison.InvariantCultureIgnoreCase) ||
                                                          SelectedSampleType.sampleType.Equals("G", StringComparison.InvariantCultureIgnoreCase) ||
                                                          SelectedSampleType.sampleType.Equals("K", StringComparison.InvariantCultureIgnoreCase) ||
                                                          SelectedSampleType.sampleType.Equals("M", StringComparison.InvariantCultureIgnoreCase) ||
                                                          SelectedSampleType.sampleType.Equals("T", StringComparison.InvariantCultureIgnoreCase)) &&
                                                          (_fishingTimeSeconds.Value < 0 || _fishingTimeSeconds.Value > (12 * 60 * 60)))
                            {
                                strError = String.Format(string.Format("{0}Fisketiden er udenfor det forventede (0 < fisketid < 720 min).", WarningPrefix));
                            }
                            else if (SelectedSampleType != null && SelectedSampleType.sampleType != null && !IsRecreationalFishery &&
                                                         (SelectedSampleType.sampleType.Equals("B", StringComparison.InvariantCultureIgnoreCase) ||
                                                          SelectedSampleType.sampleType.Equals("F", StringComparison.InvariantCultureIgnoreCase) ||
                                                          SelectedSampleType.sampleType.Equals("H", StringComparison.InvariantCultureIgnoreCase) ||
                                                          SelectedSampleType.sampleType.Equals("I", StringComparison.InvariantCultureIgnoreCase) ||
                                                          SelectedSampleType.sampleType.Equals("X", StringComparison.InvariantCultureIgnoreCase)) &&
                                                          (_fishingTimeSeconds.Value < 0 || _fishingTimeSeconds.Value > (36 * 60 * 60)))
                            {
                                strError = String.Format(string.Format("{0}Fisketiden er udenfor det forventede (0 < fisketid < 2160 min).", WarningPrefix));
                            }
                            else if (_fishingTimeSeconds.Value == 0)
                            {
                                strError = String.Format(string.Format("{0}Fisketiden er 0.", WarningPrefix));
                            }
                        }
                        break;

                    case "SelectedGearQuality":
                        //Show warning if saving or adding a new one.
                        if (SelectedGearQuality == null && !IsRecreationalFishery)
                            strError = String.Format(string.Format("{0}Redskabskvalitet er ikke angivet.", WarningPrefix));
                        break;

                    case "SelectedGearType":
                        if (SelectedSampleType != null && !IsRecreationalFishery)
                        {
                            //Show warning if saving or adding a new one.
                            if (SelectedGearType == null)
                                strError = String.Format(string.Format("{0}Redskabstype er ikke angivet.", WarningPrefix));
                        }
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

            //Check all non empty species list items for empty landingscategories.
            if (SelectedGearQuality == null && !IsRecreationalFishery)
            {
                sb.AppendLine(" - Redskabskvalitet er ikke angivet.");
                blnWarnings = true;
            }

            if (_trip.dateStart.HasValue && _trip.dateEnd.HasValue && SelectedTimeZone != null)
            {
                //Convert trip times to same timezone as specified here
                DateTime tripStart = _trip.dateStart.Value.AddHours(SelectedTimeZone.timeZone);
                DateTime tripStop = _trip.dateEnd.Value.AddHours(SelectedTimeZone.timeZone);
                bool blnDateHeaderAdded = false;
                string strDateHeader = String.Format("(Turen startede d. {0} og sluttede d. {1})", tripStart.ToString("dd-MM-yyyy HH:mm"), tripStop.ToString("dd-MM-yyyy HH:mm"));

                if (GearStartDateTime.HasValue && (GearStartDateTime < tripStart || GearStartDateTime > tripStop))
                {
                    sb.AppendLine(strDateHeader); blnDateHeaderAdded = true;
                    sb.AppendLine(String.Format(" - Redskab sat er udenfor turens start og stop tid."));
                    blnWarnings = true;
                }

                if (GearEndDateTime.HasValue && (GearEndDateTime < tripStart || GearEndDateTime > tripStop))
                {
                    if(!blnDateHeaderAdded) sb.AppendLine(strDateHeader); 
                    sb.AppendLine(String.Format(" - Redskab bjærget er udenfor turens start og stop tid."));
                    blnWarnings = true;
                }
            }

            if (SelectedSampleType != null)
            {
                if (SelectedGearType == null && !IsRecreationalFishery)
                {
                    sb.AppendLine(" - Redskabstype er ikke angivet.");
                    blnWarnings = true;
                }

                if (!MeshSize.HasValue && !SelectedSampleType.sampleType.Equals("K", StringComparison.InvariantCultureIgnoreCase) && !IsRecreationalFishery)
                {
                    string strMeshSizeName = (SelectedSampleType.sampleType.Equals("A", StringComparison.InvariantCultureIgnoreCase) ||
                                              SelectedSampleType.sampleType.Equals("C", StringComparison.InvariantCultureIgnoreCase) ||
                                              SelectedSampleType.sampleType.Equals("E", StringComparison.InvariantCultureIgnoreCase)) ? "Posemaskevidde" : "Maskevidde";
                    sb.AppendLine(string.Format(" - {0} er ikke udfyldt.", strMeshSizeName));
                    blnWarnings = true;
                }
            }

            //Give warning if fishingtime is 0 or more than 12 hours
            if (_fishingTimeSeconds.HasValue && !IsRecreationalFishery)
            {
                if (SelectedSampleType != null && SelectedSampleType.sampleType != null &&  
                                             (SelectedSampleType.sampleType.Equals("A", StringComparison.InvariantCultureIgnoreCase) ||
                                              SelectedSampleType.sampleType.Equals("C", StringComparison.InvariantCultureIgnoreCase) ||
                                              SelectedSampleType.sampleType.Equals("D", StringComparison.InvariantCultureIgnoreCase) ||
                                              SelectedSampleType.sampleType.Equals("E", StringComparison.InvariantCultureIgnoreCase) ||
                                              SelectedSampleType.sampleType.Equals("G", StringComparison.InvariantCultureIgnoreCase) ||
                                              SelectedSampleType.sampleType.Equals("K", StringComparison.InvariantCultureIgnoreCase) ||
                                              SelectedSampleType.sampleType.Equals("M", StringComparison.InvariantCultureIgnoreCase) ||
                                              SelectedSampleType.sampleType.Equals("T", StringComparison.InvariantCultureIgnoreCase)) &&
                                              _fishingTimeSeconds.Value < 0 || _fishingTimeSeconds.Value > (12 * 60 * 60))
                {
                    sb.AppendLine(" - Fisketiden er udenfor det forventede (0 < fisketid < 720 min).");
                }
                else if(SelectedSampleType != null && SelectedSampleType.sampleType != null &&  
                                             (SelectedSampleType.sampleType.Equals("B", StringComparison.InvariantCultureIgnoreCase) ||
                                              SelectedSampleType.sampleType.Equals("F", StringComparison.InvariantCultureIgnoreCase) ||
                                              SelectedSampleType.sampleType.Equals("H", StringComparison.InvariantCultureIgnoreCase) ||
                                              SelectedSampleType.sampleType.Equals("I", StringComparison.InvariantCultureIgnoreCase) ||
                                              SelectedSampleType.sampleType.Equals("X", StringComparison.InvariantCultureIgnoreCase)) &&
                                              _fishingTimeSeconds.Value < 0 || _fishingTimeSeconds.Value > (36 * 60 * 60))
                {
                    sb.AppendLine(" - Fisketiden er udenfor det forventede (0 < fisketid < 2160 min).");
                }
                else if (_fishingTimeSeconds.Value == 0)
                {
                    sb.AppendLine(" - Fisketiden er 0.");
                    blnWarnings = true;
                }

                /*if (_fishingTimeSeconds.Value > (12 * 60 * 60))
                {
                    sb.AppendLine(" - Fisketiden er mere end 12 timer.");
                    blnWarnings = true;
                }*/
            }


            if (blnWarnings)
            {
                sb.AppendLine("");
                sb.AppendLine("Er du sikker på du vil fortsætte og gemme stationen?");

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

        private void ResetHiddenFields(Sample sample)
        {
            if (!PushUIReturnMessage<bool>("IsShovelDistVisible"))
                sample.shovelDist = null;

            if (!PushUIReturnMessage<bool>("IsDepthAveGearVisible"))
                sample.depthAveGear = null;

            if (!PushUIReturnMessage<bool>("IsNetOpeningVisible"))
                sample.netOpening = null;

            if (!PushUIReturnMessage<bool>("IsHaulDirectionVisible"))
                sample.haulDirection = null;

            if (!PushUIReturnMessage<bool>("IsWaterSpeedVisible"))
                sample.haulSpeedWat = null;

            if (!PushUIReturnMessage<bool>("IsHaulSpeedBotVisible"))
                sample.haulSpeedBot = null;

            if (!PushUIReturnMessage<bool>("IsLengthBeamVisible"))
                sample.lengthBeam = null;

            if (!PushUIReturnMessage<bool>("IsNumberTrawlsNonScientificVisible") && !PushUIReturnMessage<bool>("IsNumberTrawlsScientificVisible"))
                sample.numberTrawls = null;

            if (!PushUIReturnMessage<bool>("IsLengthNetsVisible"))
                sample.lengthNets = null;

            if (!PushUIReturnMessage<bool>("IsNumNetsVisible"))
                sample.numNets = null;
            
            if (!PushUIReturnMessage<bool>("IsHeightNetsVisible"))
                sample.heightNets = null;

            if (!PushUIReturnMessage<bool>("IsLostNetsVisible"))
                sample.lostNets = null;

            if (!PushUIReturnMessage<bool>("IsCourseTrackVisible"))
                sample.courseTrack = null;

            if (!PushUIReturnMessage<bool>("IsMeshSizeVisible"))
                sample.meshSize = null;

            if (!PushUIReturnMessage<bool>("IsWireLengthVisible"))
                sample.wireLength = null;

            if (!PushUIReturnMessage<bool>("IsWingSpreadVisible"))
                sample.wingSpread = null;

            if (!PushUIReturnMessage<bool>("IsLengthRopeFlyerVisible"))
                sample.lengthRopeFlyer = null;

            if (!PushUIReturnMessage<bool>("IsWidthRopeFlyerVisible"))
                sample.widthRopeFlyer = null;

            if (!PushUIReturnMessage<bool>("IsNumberHooksVisible"))
                sample.numberHooks = null;
        }

        /// <summary>
        /// Save changes to database. This method does not validate any fields (this should be done prior to calling the method).
        /// </summary>
        private void Save()
        {
            try
            {
                var man = new DataInputManager();

                AssignSpeciesInformation();

                //Make sure values that cannot be specified at the same time, are not.
                if (IsCoordinatesNotKnown)
                    _sample.latPosEndText = _sample.latPosStartText = _sample.lonPosEndText = _sample.lonPosStartText = null;
                else
                    _sample.dfuArea = _sample.statisticalRectangle = null;

                //Convert to UTC.
                if (GearStartDateTime != null)
                    _sample.dateGearStart = GearStartDateTime.Value.AddHours(-SelectedTimeZone.timeZone);

                if (GearEndDateTime != null)
                    _sample.dateGearEnd = GearEndDateTime.Value.AddHours(-SelectedTimeZone.timeZone);

                //Set fishing time
                if (_fishingTimeSeconds != _sample.fishingtime)
                    _sample.fishingtime = _fishingTimeSeconds;

                ResetHiddenFields(_sample);

                DatabaseOperationResult res = man.SaveSample(ref _sample);

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
                    _intEditingSampleId = _sample.sampleId;

                    //Reset change tracker.
                    _sample.AcceptChanges();

                    InitializeAsync().ContinueWith(t =>
                    {
                        DispatchMessageBox("Stationen blev gemt korrekt.", 2);
                    });
                }).Dispatch();

                //Call save succeeded event (this makes sure the tree is updated with any changes)
                RaiseSaveSucceeded();

                //Refresh tree, if node exists.
                var treeNode = MainTree.GetTripNodeIfLoaded(_sample.tripId);
                if (treeNode != null)
                {
                    //Make sure trip is assigned to have at least one sample (since one has now been created)
                    if (!treeNode.TripEntity.HasSamples)
                        treeNode.TripEntity.SampleCount = 1;

                    //Refresh tree and select saved sample node if possible.
                    treeNode.RefreshTreeAsync().ContinueWith(t =>
                    {
                        MainTree.SelectTreeNode(_sample.sampleId, TreeNodeLevel.Sample);
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


        #region Conversion methods


        public static string RefitLatLon(string str, bool blnIsLat)
        {
            string strTemplate = blnIsLat ? _strDefaultLatCoordinates : _strDefaultLonCoordinates;

            char[] strCharToIgnore = new char[] { '\'' }; 
            StringBuilder sb = new StringBuilder(strTemplate);

            if ((sb.Length - strCharToIgnore.Length) != str.Length)
                throw new ApplicationException("Position format is invalid");

            //Take the template string and replace values with values from str.
            for (int i = 0, j=0; i < sb.Length; i++, j++)
            {
                if (sb[i] == '0' || sb[i] == 'N' || sb[i] == 'S' || sb[i] == 'E' || sb[i] == 'W')
                    sb[i] = str[j] == ' ' ? '0' : str[j];

                //Handle characters in the default position string that are not in str.
                if (strCharToIgnore.Contains(sb[i]))
                    j--;
            }
            
            return sb.ToString();
        }

        private void UpdateFishingTime()
        {
            int? intFishingTime = null;
            if (GearStartDateTime.HasValue && GearEndDateTime.HasValue)
            {
                TimeSpan ts = GearEndDateTime.Value - GearStartDateTime.Value;

                intFishingTime = (int)ts.TotalSeconds;
            }


            _fishingTimeSeconds = intFishingTime;
            //if (_sample.fishingtime != intFishingTime)
            //    _sample.fishingtime = intFishingTime;
        }


        private string SecondsToHoursAndMinutes(int intSeconds)
        {
            int intHours = (int)Math.Floor((double)intSeconds / 3600.0);
            int intMin = (int)Math.Floor(((double)intSeconds / 60.0) % 60.0);

            return string.Format("{0:00}:{1:00}", intHours, intMin);
        }

        private string SecondsToHoursMinutesAndSeconds(int intSeconds)
        {
            int intHours = (int)Math.Floor((double)intSeconds / 3600.0);
            int intMin = (int)Math.Floor(((double)intSeconds / 60.0) % 60.0);
            int intSec = intSeconds % 60;

            return string.Format("{0:00}:{1:00}:{2:00}", intHours, intMin, intSec);
        }

        private string SecondsToMinutesAndSeconds(int intSeconds)
        {
            double decimalMin = (double)intSeconds / 60.0;
            int intMin = (int)Math.Floor(decimalMin);
            double decimalPart = decimalMin - (double)intMin;
            int intSec = (int)Math.Round(decimalPart * 60.0);

            return string.Format("{0:00}:{1:00}", intMin, intSec);
        }


        #endregion


        public void RefreshGearTypes()
        {
            RaisePropertyChanged(() => GearTypes);
        }

        protected override void GlobelPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            //Save on Ctrl + S
            if (HasEditingRights
                && ((e.Key == System.Windows.Input.Key.S && Keyboard.Modifiers == ModifierKeys.Control) || ((e.SystemKey == Key.G || e.Key == Key.G) && (Keyboard.Modifiers == ModifierKeys.Alt || Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt))))
                && HasUnsavedData)
                ValidateAndSaveAsync();

            if (!(_trip != null && _trip.IsHVN) && e.Key == System.Windows.Input.Key.I && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                TreeView.TripTreeItemViewModel.ImportStation(_trip.tripId);
            }
            else if (!(_trip != null && _trip.IsHVN) && ((e.SystemKey == Key.N || e.Key == Key.N) && (Keyboard.Modifiers == ModifierKeys.Alt || Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt))))
            {
                e.Handled = true;
                if (IsLoading)
                {
                    AppRegionManager.ShowMessageBox("Vent venligst med at oprette en ny station til formen er færdig med at arbejde.");
                    return;
                }

                NewStation();
            }
            else if ((e.SystemKey == Key.A || e.Key == Key.A) && (Keyboard.Modifiers == ModifierKeys.Alt || Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt)))
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
