using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Babelfisk.Entities;
using Anchor.Core;
using Babelfisk.BusinessLogic;
using Babelfisk.BusinessLogic.DataInput;
using Babelfisk.Entities.Sprattus;
using System.ServiceModel;
using Microsoft.Practices.Prism.Commands;
using Babelfisk.ViewModels.TreeView;
using Babelfisk.ViewModels.Common;
using System.Windows.Input;

namespace Babelfisk.ViewModels.Input
{
    public class SubSampleViewModel : AViewModel
    {
        private DelegateCommand _cmdShowCruise;
        private DelegateCommand _cmdShowTrip;
        private DelegateCommand _cmdShowStation;
        private DelegateCommand _cmdShowParent;
        private DelegateCommand _cmdShowSpeciesList;

        protected DelegateCommand _cmdSave;
        protected DelegateCommand _cmdClose;

        private int _intSpeciesListId;

         private bool _blnAssigningLookups = false;

        private SubSampleType _enmSubSampleType;

        private SpeciesList _speciesList;
        private Sample _sample;
        private Trip _trip;
        private Cruise _cruise;
        private SubSample _subSample;

        private List<HeaderItem> _lstHeaders;

        private SpeciesListViewModel _speciesListViewModel;

        private ALavSFViewModel _aLavSFViewModel;

        private List<L_SexCode> _lstSexCodes;
        private List<L_BroodingPhase> _lstBroodingPhases;
        private List<Maturity> _lstMaturity;
        private List<L_OtolithReadingRemark> _lstOtolithReadingRemarks;
        private List<L_EdgeStructure> _lstEdgeStructures;
        private List<L_Parasite> _lstParasites;
        private List<L_Reference> _lstReferences;
        private List<DFUPerson> _lstDFUPersons;
        private List<L_LengthMeasureUnit> _lstLengthMeasureUnits;
        private List<L_MaturityIndexMethod> _lstMaturityIndexMethods;
        private List<L_HatchMonthReadability> _lstHatchMonthReadabilities;
        private List<L_VisualStock> _lstVisualStocks;
        private List<L_GeneticStock> _lstGeneticStocks;

        private ColumnVisibilityViewModel _vmColumnVisibility;
        private int _columnCount;
        private int _columnsWidth;

        private decimal? _treatmentFactor;

        private L_Species _species;

        protected static Dictionary<string, int> _lengthUnitOrder = new Dictionary<string, int>() { { "mm", 1 }, { "sc", 2 }, { "cm", 3 } };


        #region Properties


        public bool CanEditOffline
        {
            get
            {
                return !BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline || (IsLoading || (_subSample != null && _subSample.OfflineState == ObjectState.Added));
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

                return _aLavSFViewModel != null && _aLavSFViewModel.HasUnsavedData;
            }
        }


        public decimal? TreatmentFactor
        {
            get { return _treatmentFactor; }
        }


        public L_Species Species
        {
            get { return _species; }
        }


        public string TripType
        {
            get { return _trip == null ? null : _trip.tripType; }
        }


        public string OtolithFinScaleHeader
        {
            get
            {
                string res = "Otolith";

                if (_cruise != null && _cruise.cruise1 != null && _cruise.cruise1.Contains("Laks", StringComparison.InvariantCultureIgnoreCase))
                    res = "Fin/Skæl";

                return res;
            }
        }


        public List<HeaderItem> Headers
        {
            get { return _lstHeaders; }
            set
            {
                _lstHeaders = value;
                RaisePropertyChanged(() => Headers);
            }
        }

        public bool IsAssigningLookups
        {
            get { return _blnAssigningLookups; }
        }


        public SpeciesList SpeciesList
        {
            get { return _speciesList;  }
            private set { _speciesList = value; RaisePropertyChanged(() => SpeciesList); }
        }

        public Sample Sample
        {
            get { return _sample;  }
            private set { _sample = value; RaisePropertyChanged(() => Sample); }
        }


        public Cruise Cruise
        {
            get { return _cruise; }
            private set
            {
                _cruise = value;
                RaisePropertyChanged(() => Cruise);
                RaisePropertyChanged(() => OtolithFinScaleHeader);
            }
        }


        public Trip Trip
        {
            get { return _trip; }
            private set { _trip = value; RaisePropertyChanged(() => Trip); RaisePropertyChanged(() => TripType); }
        }


        public string SubSampleTypeString
        {
            get { return _enmSubSampleType.ToString(); }
        }


        public ALavSFViewModel LavSFViewModel
        {
            get { return _aLavSFViewModel; }
            set
            {
                _aLavSFViewModel = value;
                RaisePropertyChanged(() => LavSFViewModel);
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


        public List<L_BroodingPhase> BroodingPhases
        {
            get { return _lstBroodingPhases == null ? null : _lstBroodingPhases.ToList(); }
            private set
            {
                _lstBroodingPhases = value;
                RaisePropertyChanged(() => BroodingPhases);
            }
        }


        public List<Maturity> MaturityList
        {
            get { return _lstMaturity == null ? null : _lstMaturity.ToList(); }
            private set
            {
                _lstMaturity = value;
                RaisePropertyChanged(() => MaturityList);
            }
        }

        public List<L_OtolithReadingRemark> OtolithReadingRemarks
        {
            get { return _lstOtolithReadingRemarks == null ? null : _lstOtolithReadingRemarks.ToList(); }
            private set
            {
                _lstOtolithReadingRemarks = value;
                RaisePropertyChanged(() => OtolithReadingRemarks);
            }
        }


        public List<L_VisualStock> VisualStocks
        {
            get { return _lstVisualStocks == null ? null : _lstVisualStocks.ToList(); }
            private set
            {
                _lstVisualStocks = value;
                RaisePropertyChanged(() => VisualStocks);
            }
        }


        public List<L_GeneticStock> GeneticStocks
        {
            get { return _lstGeneticStocks == null ? null : _lstGeneticStocks.ToList(); }
            private set
            {
                _lstGeneticStocks = value;
                RaisePropertyChanged(() => GeneticStocks);
            }
        }



        public List<L_HatchMonthReadability> HatchMonthReadabilities
        {
            get { return _lstHatchMonthReadabilities == null ? null : _lstHatchMonthReadabilities.ToList(); }
            private set
            {
                _lstHatchMonthReadabilities = value;
                RaisePropertyChanged(() => HatchMonthReadabilities);
            }
        }


        public List<L_EdgeStructure> EdgeStructures
        {
            get { return _lstEdgeStructures == null ? null : _lstEdgeStructures.ToList(); }
            private set
            {
                _lstEdgeStructures = value;
                RaisePropertyChanged(() => EdgeStructures);
            }
        }


        public List<L_Parasite> Parasites
        {
            get { return _lstParasites == null ? null : _lstParasites.ToList(); }
            private set
            {
                _lstParasites = value;
                RaisePropertyChanged(() => Parasites);
            }
        }


        public List<L_Reference> References
        {
            get { return _lstReferences == null ? null : _lstReferences.ToList(); }
            private set
            {
                _lstReferences = value;
                RaisePropertyChanged(() => References);
            }
        }

        public List<DFUPerson> DFUPersons
        {
            get { return _lstDFUPersons == null ? null : _lstDFUPersons.ToList(); }
            private set
            {
                _lstDFUPersons = value;
                RaisePropertyChanged(() => DFUPersons);
            }
        }


        public List<L_LengthMeasureUnit> LengthMeasureUnits
        {
            get { return _lstLengthMeasureUnits == null ? null : _lstLengthMeasureUnits.ToList(); }
            private set
            {
                _lstLengthMeasureUnits = value;
                RaisePropertyChanged(() => LengthMeasureUnits);
            }
        }

        public List<L_MaturityIndexMethod> MaturityIndexMethods
        {
            get { return _lstMaturityIndexMethods == null ? null : _lstMaturityIndexMethods.ToList(); }
            private set
            {
                _lstMaturityIndexMethods = value;
                RaisePropertyChanged(() => MaturityIndexMethods);
            }
        }

        public int ColumnCount
        {
            get { return _columnCount; }
            set
            {
                _columnCount = value;
                RaisePropertyChanged(() => ColumnCount);
            }
        }

        public int ColumnsWidth
        {
            get { return _columnsWidth; }
            set
            {
                _columnsWidth = value;
                RaisePropertyChanged(() => ColumnsWidth);
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

        #endregion


        /// <summary>
        /// Create a new instance of SubSampleViewModel.
        /// If slvm is given, that SpeciesListViewModel is loaded when the user click the "back" button on the UI. If slvm
        /// is null, a new SpeciesListViewMOdel is created using the sample id.
        /// </summary>
        public SubSampleViewModel(int intSpeciesListId, SubSampleType enmSubSampleType, SpeciesListViewModel slvm)
        {
            _intSpeciesListId = intSpeciesListId;
            _enmSubSampleType = enmSubSampleType;
            _speciesListViewModel = slvm;

            InitializeColumnVisibility();

            InitializeAsync();

            RegisterToKeyDown();
        }


        private void InitializeColumnVisibility()
        {
            var tt = _speciesListViewModel.TripType;

            string group = "SingleFish";
            if(tt != null)
            {
                switch(tt.ToLower())
                {
                    case "rekhvn":
                    case "rektbd":
                    case "rekomr":
                        //Since it is only SFNotRep that has it's own UI currently, use "SingleFish" for REK data for lav rep.
                        if(_enmSubSampleType != SubSampleType.LAVRep)
                            group = "REKSingleFish";
                        break;
                }
            }

            _vmColumnVisibility = new ColumnVisibilityViewModel(SubSampleTypeString, group, ColumnVisibilityChanged);
            string strType = "LAV, rep";

            switch (_enmSubSampleType)
            {
                case SubSampleType.SFRep:
                    strType = "Enkeltfisk, rep";
                    ColumnCount = 3;
                    ColumnsWidth = 1050;
                    break;

                case SubSampleType.SFNotRep:
                    strType = "Enkeltfisk, ej rep";
                    ColumnCount = 3;
                    ColumnsWidth = 1050;
                    break;

                default:
                    ColumnCount = 2;
                    ColumnsWidth = 750;
                    break;
            }

            ColumnVisibility.WindowTitle = string.Format("Viste kolonner for:  {0}", strType);
        }


        private void ColumnVisibilityChanged(List<Babelfisk.BusinessLogic.Settings.DataGridColumnSettings> cs)
        {
            //Refresh trip type since it is bound to all column, forcing their vibility to be refreshed
            RaisePropertyChanged(() => SubSampleTypeString);
        }


        #region Initialize methods


        private Task InitializeAsync()
        {
            IsLoading = true;
            return Task.Factory.StartNew(() => Initialize()).ContinueWith(t => new Action(() => { IsLoading = false; RaisePropertyChanged(() => CanEditOffline); }).Dispatch());
        }


        private void Initialize()
        {
            try
            {
                var man = new DataRetrievalManager();
                var manInput = new DataInputManager();
                var manLookup = new LookupManager();

                //Retrieve whole hierarchy from specieslist and up
                var sl = manInput.GetEntity<SpeciesList>(_intSpeciesListId, "L_Species", "L_SizeSortingEU", "L_SizeSortingDFU", "L_Treatment", "L_SexCode", "L_LandingCategory", "SubSample", "Sample", "Sample.Trip", "Sample.Trip.Cruise");
                var s = sl.Sample;
                var t = sl.Sample.Trip;
                var c = sl.Sample.Trip.Cruise;

                SubSample sub = GetSubSample(sl);

                if (sub == null)
                {
                    DispatchMessageBox("Det trin hvor de individuelle længder og vægte skulle hægtes på, blev ikke fundet. Gå venligst tilbage til Artslisten og prøv at åbne denne form igen.");
                    return;
                }

                //Load header information
                LoadHeaders(sl, sub);
                var lv = new LookupDataVersioning();

                //Shared lookups (between LAV and SF)
                var lstSexCodes = manLookup.GetLookups(typeof(L_SexCode), lv).OfType<L_SexCode>().OrderBy(x => x.UIDisplay).ToList();
                var lstDFUPersons = manLookup.GetLookups(typeof(DFUPerson), lv).OfType<DFUPerson>().OrderBy(x => x.UIDisplay).ToList();
                _lstDFUPersons = lstDFUPersons; //Assign _lstReferences since it is used in IntializeLavSF
                var lstLengthMeasureUnits = manLookup.GetLookups(typeof(L_LengthMeasureUnit), lv).OfType<L_LengthMeasureUnit>().OrderBy(x => x.UIDisplay).ToList();

                List<L_BroodingPhase> lstBroodingPhases = null;
                List<Maturity> lstMaturity = null;
                List<L_OtolithReadingRemark> lstOtolithReadingRemark = null;
                List<L_EdgeStructure> lstEdgeStructures = null;
                List<L_Parasite> lstParasites = null;
                List<L_Reference> lstReferences = null;
                List<L_MaturityIndexMethod> lstMaturityIndexMethods = null;
                List<L_HatchMonthReadability> lstHatchMonthReadabilities = null;
                List<L_VisualStock> lstVisualStocks = null;
                List<L_GeneticStock> lstGeneticStocks = null;

                switch (_enmSubSampleType)
                {
                    case SubSampleType.LAVRep:
                        lstBroodingPhases = manLookup.GetLookups(typeof(L_BroodingPhase), lv).OfType<L_BroodingPhase>().OrderBy(x => x.UIDisplay).ToList();
                        break;

                    case SubSampleType.SFRep:
                    case SubSampleType.SFNotRep:
                        lstMaturity = manLookup.GetLookups(typeof(Maturity), lv).OfType<Maturity>().OrderBy(x => x.UIDisplay).ToList();
                        _lstMaturity = lstMaturity; //Assign _lstReferences since it is used in IntializeLavSF
                        lstOtolithReadingRemark = manLookup.GetLookups(typeof(L_OtolithReadingRemark), lv).OfType<L_OtolithReadingRemark>().OrderBy(x => x.UIDisplay).ToList();
                        lstEdgeStructures = manLookup.GetLookups(typeof(L_EdgeStructure), lv).OfType<L_EdgeStructure>().OrderBy(x => x.UIDisplay).ToList();
                        lstParasites = manLookup.GetLookups(typeof(L_Parasite), lv).OfType<L_Parasite>().OrderBy(x => x.UIDisplay).ToList();
                        lstReferences = manLookup.GetLookups(typeof(L_Reference), lv).OfType<L_Reference>().OrderBy(x => x.UIDisplay).ToList();
                        _lstReferences = lstReferences; //Assign _lstReferences since it is used in IntializeLavSF
                        lstMaturityIndexMethods = manLookup.GetLookups(typeof(L_MaturityIndexMethod), lv).OfType<L_MaturityIndexMethod>().OrderBy(x => x.UIDisplay).ToList();
                        lstHatchMonthReadabilities = manLookup.GetLookups(typeof(L_HatchMonthReadability), lv).OfType<L_HatchMonthReadability>().OrderBy(x => x.UIDisplay).ToList();
                        lstVisualStocks = manLookup.GetLookups(typeof(L_VisualStock), lv).OfType<L_VisualStock>().Where(x => x.speciesCode == sl.speciesCode).OrderBy(x => x.UIDisplay).ToList();
                        lstGeneticStocks = manLookup.GetLookups(typeof(L_GeneticStock), lv).OfType<L_GeneticStock>().Where(x => x.speciesCode == sl.speciesCode).OrderBy(x => x.UIDisplay).ToList();
                        break;
                }

                //Retrieve subsample to edit
                var subSample = manInput.GetEntity<SubSample>(sub.subSampleId);

                //Initialize correct viewmodel for editing the sub sample
                InitializeLavSF(subSample, manInput);

                //Assgin UI properties
                new Action(() =>
                {
                    IsDirty = false;

                    SpeciesList = sl;
                    Sample = s;
                    Trip = t;
                    Cruise = c;
                    _subSample = subSample;
                    

                    _blnAssigningLookups = true;
                    {
                        SexCodes = lstSexCodes;
                        BroodingPhases = lstBroodingPhases;
                        MaturityList = lstMaturity;
                        OtolithReadingRemarks = lstOtolithReadingRemark;
                        EdgeStructures = lstEdgeStructures;
                        Parasites = lstParasites;
                        References = lstReferences;
                        DFUPersons = lstDFUPersons;
                        LengthMeasureUnits = lstLengthMeasureUnits;
                        MaturityIndexMethods = lstMaturityIndexMethods;
                        HatchMonthReadabilities = lstHatchMonthReadabilities;
                        VisualStocks = lstVisualStocks;
                        GeneticStocks = lstGeneticStocks;
                    }
                    _blnAssigningLookups = false;

                     _treatmentFactor = GetTreatmentFactor();


                    if (_aLavSFViewModel != null)
                        _aLavSFViewModel.SetDefaultLengthMeasureType();

                    RaisePropertyChanged(() => LavSFViewModel);

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

        private decimal GetTreatmentFactor()
        {
            if (_speciesListViewModel == null || _sample == null || _speciesList == null)
                return 1;

            if (_speciesList.treatment == null || _speciesListViewModel.TreatmentFactors == null || _speciesListViewModel.Species == null)
                return 1;

            var species = _speciesListViewModel.Species.Where(x => x.speciesCode == _speciesList.speciesCode).FirstOrDefault();

            if (species == null)
                return 1;

            _species = species;

            var factor = Babelfisk.Warehouse.EntityFactory.GetTreatmentFactor(_sample.dateGearStart, species.treatmentFactorGroup, _speciesList.treatment, _speciesListViewModel.TreatmentFactors);

            if (factor == null)
                return 1;

            return factor.Value;
        }


        /// <summary>
        /// Load header information
        /// </summary>
        private void LoadHeaders(SpeciesList sl, SubSample ss)
        {
            if (_lstHeaders != null)
                return;

            List<HeaderItem> lst = new List<HeaderItem>();

            //Add Species
            lst.Add(new HeaderItem("Art", sl.speciesCode, sl.L_Species != null ? sl.L_Species.dkName : null));

            //Add Landing Category
            lst.Add(new HeaderItem("Landingskategori", string.IsNullOrWhiteSpace(sl.landingCategory) ? "-" : sl.landingCategory, sl.L_LandingCategory != null ? sl.L_LandingCategory.description : null));

            //Add Size Sorting EU
            lst.Add(new HeaderItem("Sortering", sl.sizeSortingEU.HasValue ? sl.sizeSortingEU.Value.ToString() : "-", sl.L_SizeSortingEU != null ? sl.L_SizeSortingEU.description : null));

            //Add Size Sorting DFU
            lst.Add(new HeaderItem("Opdeling", string.IsNullOrWhiteSpace(sl.sizeSortingDFU) ? "-" : sl.sizeSortingDFU, sl.L_SizeSortingDFU != null ? sl.L_SizeSortingDFU.description : null));

            //Add Treatment
            lst.Add(new HeaderItem("Behandling", string.IsNullOrWhiteSpace(sl.treatment) ? "-" : sl.treatment, sl.L_Treatment != null ? sl.L_Treatment.description : null));

            //Add sex code
            lst.Add(new HeaderItem("Køn", string.IsNullOrWhiteSpace(sl.sexCode) ? "-" : sl.sexCode, sl.L_SexCode != null ? sl.L_SexCode.description : null));

            //Add ovigorous
            lst.Add(new HeaderItem("Rogn", string.IsNullOrWhiteSpace(sl.ovigorous) ? "-" : sl.ovigorous, sl.ovigorous));

            //Add representative
            lst.Add(new HeaderItem("Representativ", string.IsNullOrWhiteSpace(ss.representative) ? "-" : ss.representative, ss.representative));

            //Add step num
            lst.Add(new HeaderItem("Trin", ss.stepNum.ToString(), ss.stepNum.ToString()));

            new Action(() =>
            {
                Headers = lst;
            }).Dispatch();
        }


        /// <summary>
        /// Retrieve subsample to edit.
        /// </summary>
        private SubSample GetSubSample(SpeciesList sl)
        {
            if (sl.SubSample == null || sl.SubSample.Count == 0)
                return null;

            SubSample sub = null;
            switch (_enmSubSampleType)
            {
                case SubSampleType.LAVRep:
                case SubSampleType.SFRep:
                    sub = sl.SubSample.OrderByDescending(x => x.stepNum).Where(x => x.IsRepresentative && (x.subSampleWeight.HasValue || x.landingWeight.HasValue || (x.sumAnimalWeights.HasValue && x.sumAnimalWeights.Value))).FirstOrDefault();
                    break;

                case SubSampleType.SFNotRep:
                    var q = sl.SubSample.Where(x => !x.IsRepresentative && (x.subSampleWeight.HasValue || (x.sumAnimalWeights.HasValue && x.sumAnimalWeights.Value)) );

                    if (q.Count() == 1)
                        sub = q.FirstOrDefault();
                    break;
            }

            return sub;
        }


        /// <summary>
        /// Initialize child view models (either LAV or SingleFish depending on _enmSubSampleType)
        /// </summary>
        private void InitializeLavSF(SubSample ss, DataInputManager datMan)
        {
            //Create proper child view model (if not already initialized).
            if (_aLavSFViewModel == null)
            {
                switch (_enmSubSampleType)
                {
                    case SubSampleType.LAVRep:
                        _aLavSFViewModel = new LAVViewModel(this);
                        break;

                    case SubSampleType.SFRep:
                    case SubSampleType.SFNotRep:
                         if(_speciesListViewModel.TripType.IsREK())
                            _aLavSFViewModel = new REKSFViewModel(this, _enmSubSampleType);
                         else
                            _aLavSFViewModel = new SFViewModel(this, _enmSubSampleType);
                        break;
                }
            }

            //Initialize child view model.
            if (_aLavSFViewModel != null)
                _aLavSFViewModel.Initialize(ss);
        }


        #endregion


        #region Show Species List Command


        public DelegateCommand ShowSpeciesListCommand
        {
            get { return _cmdShowSpeciesList ?? (_cmdShowSpeciesList = new DelegateCommand(ShowSpeciesList)); }
        }


        private void ShowSpeciesList()
        {
            var vm = _speciesListViewModel;

            if (vm == null)
                vm = new ViewModels.Input.SpeciesListViewModel(_sample.sampleId, false);
            else
                vm.RegisterToKeyDown();

            //Grab the selected species list item id, so it can be pre-selected after viewmodel has been initialized.
            int? intSelectedItemId = (vm.SelectedItem != null && !vm.SelectedItem.IsNew && vm.SelectedItem.SpeciesListEntity != null) ? vm.SelectedItem.SpeciesListEntity.speciesListId : new Nullable<int>();

            bool blnSuccess = AppRegionManager.LoadViewFromViewModel(RegionName.MainRegion, vm);

            if (blnSuccess)
                vm.InitializeAsync(intSelectedItemId);
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
            string strError = GetRowErrors();

            //If errors are found, abort saving and show error message.
            if (strError != null && !strError.Contains(AViewModel.WarningPrefix))
            {
                DispatchMessageBox("Der er fundet fejl på siden (felter markeret med rødt). Hold musen over et rødt felt for at læse mere om fejlen.");
                return;
            }

            if (!HandleWarningsAndContinue())
                return;

            IsLoading = true;
            Task.Factory.StartNew(ValidateAndSave).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        private bool HandleWarningsAndContinue()
        {
            try
            {
                bool blnWarnings = false;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Advarsler:");
                sb.AppendLine("");

                var items = _aLavSFViewModel.Items.ToList();

                //Check that SOW = SUM(min(SL.trin).Weight) 
                decimal? decSOW = _aLavSFViewModel.WeightSum;
                decimal? decSampleWeight = _subSample.subSampleWeight;
                
                if (decSOW != null)
                    decSOW = (decSOW.Value * 1000); //Convert from kilograms to grams

                if (decSampleWeight != null)
                    decSampleWeight = (decSampleWeight.Value * 1000); //Convert from kilograms to grams

                if (decSampleWeight.HasValue 
                    && ((decSOW.HasValue && !decSOW.Equals(decSampleWeight)) || (decSampleWeight.HasValue && !decSampleWeight.Equals(decSOW)))
                    && !((_aLavSFViewModel is LAVViewModel) && (!decSOW.HasValue || decSOW.Value == 0)) //If SOW = 0 or null for LAV-rep, don't give warning
                   )
                {
                    if ((_aLavSFViewModel is LAVViewModel) || (_aLavSFViewModel is SFViewModel && (_aLavSFViewModel as SFViewModel).SubSampleType != SubSampleType.SFNotRep))
                        sb.AppendLine(String.Format(" - Summen af 'Vægt (g)' ({0}g) er forskellig for trin-vægten ({1}g) i artslisten.", decSOW.HasValue ? decSOW.Value.ToString("0.####") : "0", decSampleWeight.HasValue ? decSampleWeight.Value.ToString("0.####") : "N/A"));
                    else
                        sb.AppendLine(String.Format(" - Summen af 'Vægt (g)' ({0}g) er forskellig for 'ej, rep'-vægten ({1}g) i artslisten.", decSOW.HasValue ? decSOW.Value.ToString("0.####") : "0", decSampleWeight.HasValue ? decSampleWeight.Value.ToString("0.####") : "N/A"));
                    blnWarnings = true;
                }


                if ((_enmSubSampleType == SubSampleType.LAVRep || _enmSubSampleType == SubSampleType.SFRep) && !this.TripType.IsHVN())
                {
                    int? intNumberSum = _aLavSFViewModel.NumberSum;
                    int? intNumberSL = _speciesList.number;

                    if (intNumberSL.HasValue && intNumberSL.Value > 0 && //Only show warning if SL number is specified.
                        ((intNumberSum.HasValue && !intNumberSum.Equals(intNumberSL)) || (intNumberSL.HasValue && !intNumberSL.Equals(intNumberSum))))
                    {
                        sb.AppendLine(String.Format(" - Summen af 'Antal' ({0}) er forskellig for antallet ({1}) angivet i artslisten.", intNumberSum.HasValue ? intNumberSum.Value : 0, intNumberSL.HasValue ? intNumberSL.Value : 0));
                        blnWarnings = true;
                    }
                }


                if (_aLavSFViewModel is LAVViewModel)
                {
                    //If Sum of ages on an animal is larger than the length, throw error.
                    foreach (var itm in _aLavSFViewModel.Items)
                    {
                        var ageSum = itm.AgeSum;

                        if (itm.Length.HasValue && ageSum > itm.Number)
                        {
                            sb.AppendLine(String.Format(" - Summen af aldre ({0}) er større end antal ({1}) for længde: {2}.", ageSum, itm.Number, itm.Length.Value));
                            blnWarnings = true;
                        }
                    }
                }

                //If weights are out of bounds, show message
                if (items.Where(x => x.HasUnsavedData && x.GetWeightOutOfBoundsWarning() != null).Any())
                {
                    sb.AppendLine(String.Format(" - En eller flere rækker har en 'Vægt (g)' som er udenfor den forventede min/max værdi for arten."));
                    blnWarnings = true;
                }

                //If lengths are out of bounds, show message
                if (items.Where(x => x.HasUnsavedData && x.GetLengthOutOfBoundsWarning() != null).Any())
                {
                    sb.AppendLine(String.Format(" - En eller flere rækker har en 'Længde' som er udenfor den forventede min/max værdi for arten."));
                    blnWarnings = true;
                }

                if (items.Where(x => x.HasUnsavedData && x.IsAnyAgesOutOfBounds()).Any())
                {
                    if(_enmSubSampleType == SubSampleType.LAVRep)
                        sb.AppendLine(String.Format(" - En eller flere rækker har aldre angivet som er udenfor den forventede min/max værdi for arten."));
                    else
                        sb.AppendLine(String.Format(" - En eller flere rækker har en 'Alder' angivet som er udenfor den forventede min/max værdi for arten."));
                    blnWarnings = true;
                }

                if (_aLavSFViewModel is SFViewModel && !string.IsNullOrWhiteSpace((_aLavSFViewModel as SFViewModel).MaturityIndexMethod) && !items.Where(x => x.MaturityId.HasValue).Any())
                {
                    sb.AppendLine(String.Format(" - Der er valgt modenhedsindeks, men ikke modenhed for et eneste individ. Det valgte modenhedsindeks vil derfor ikke blive gemt."));
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


        private string GetRowErrors()
        {
            string strError = null;

            if (_aLavSFViewModel != null)
            {
                strError = _aLavSFViewModel.GetRowErrors();
            }

            return strError;
        }


        protected void ValidateAndSave()
        {
            try
            {
                var man = new DataInputManager();
                DatabaseOperationResult res = DatabaseOperationResult.CreateSuccessResult();

                SpeciesList sl = null;
                List<LavSFTransferItem> lst = _aLavSFViewModel.GetUnsavedData(ref sl);

                //If no changes are recorded, show message saying so, and abort calling webservice.
                if (lst.Count == 0 &&
                   (sl == null || (sl != null && sl.ChangeTracker.State == ObjectState.Unchanged)))
                {
                    new Action(() => IsDirty = false).Dispatch();
                    DispatchMessageBox("Der var ingen tilføjelser/ændringer at gemme.");
                    return;
                }

                if (sl != null && (_enmSubSampleType == SubSampleType.LAVRep || _enmSubSampleType == SubSampleType.SFRep) && !this.TripType.IsHVN())
                {
                    int? intNumberSum = _aLavSFViewModel.NumberSum;
                    int? intNumberSL = sl.number;

                    if ((intNumberSL == null && intNumberSum.HasValue) || (intNumberSL.HasValue && intNumberSum.HasValue && intNumberSL.Value != intNumberSum.Value))
                    {
                        sl.number = intNumberSum.Value;
                    }
                }

                //Added 31-08-2016: When converting from one length measure unit to another, it is important in which order the lengths are converted to ensure
                //that they are updated in the database to a value which does not already exist. This is not necessary in offline mode.
                if (!BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline &&
                    _aLavSFViewModel != null && _aLavSFViewModel.LastLengthMeasureUnit != null && _aLavSFViewModel.LastLengthMeasureUnit != _aLavSFViewModel.LengthMeasureUnit)
                {
                    try
                    {
                        bool blnAscending = SortAniamlItemsAscendingByLengthUnit(_aLavSFViewModel.LastLengthMeasureUnit, _aLavSFViewModel.LengthMeasureUnit);

                        if (blnAscending)
                            lst = lst.OrderBy(x => (x.Animal == null || !x.Animal.length.HasValue) ? 0 : x.Animal.length.Value).ToList();
                        else
                            lst = lst.OrderByDescending(x => (x.Animal == null || !x.Animal.length.HasValue) ? 0 : x.Animal.length.Value).ToList();
                    }
                    catch (Exception e)
                    {
                        Anchor.Core.Loggers.Logger.LogError(e, "An error occured when figuring out how to order the animal records, based on length measure unit conversion.");
                    }
                }

               res =  man.SaveLavSFItems(sl, lst);

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
                    //int intSampleId = _sample.sampleId;
                    //int intTripId = _sample.tripId;

                    if(_aLavSFViewModel != null)
                    {
                        _aLavSFViewModel.LastLengthMeasureUnit = _aLavSFViewModel.LengthMeasureUnit;
                        _aLavSFViewModel.LastLengthMeasureTypeId = _aLavSFViewModel.LengthMeasureTypeId;
                    }
                        
                    //Initialize control again (reloading data from db)
                    Initialize();

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
        /// Return which way to sort the animal items based on length unit conversion. Because when converting from SC to CM for example,
        /// problems can occur, because on animal item is updated at a time. So one converted animal item updated in databse, could be conflicting with another existing
        /// animal item, that has not been converted yet.
        /// </summary>
        public bool SortAniamlItemsAscendingByLengthUnit(string strPrevLengthUnit, string strNewLengthUnit)
        {
            bool blnRes = true;

            if (string.IsNullOrWhiteSpace(strPrevLengthUnit) || string.IsNullOrWhiteSpace(strNewLengthUnit))
                return blnRes;

            string strPrev = strPrevLengthUnit.ToLower();
            string strNew = strNewLengthUnit.ToLower();

            if (_lengthUnitOrder.ContainsKey(strPrev) && _lengthUnitOrder.ContainsKey(strNew))
                blnRes = _lengthUnitOrder[strPrev] > _lengthUnitOrder[strNew];

            return blnRes;
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


        protected override void GlobelPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            //Save on Ctrl + S
            if (HasEditingRights
                && ((e.Key == System.Windows.Input.Key.S && Keyboard.Modifiers == ModifierKeys.Control) || ((e.SystemKey == Key.G || e.Key == Key.G) && (Keyboard.Modifiers == ModifierKeys.Alt || Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt)))) 
                )
                ValidateAndSaveAsync();
        }


        public void ReloadSubSampleViewModel()
        {
            SpeciesListViewModel.ShowSF(_intSpeciesListId, _enmSubSampleType, _speciesListViewModel);
        }


        public override void Dispose()
        {
            base.Dispose();

            try
            {
                if (_aLavSFViewModel != null)
                {
                    _aLavSFViewModel.Dispose();
                }
            }
            catch { }

            _speciesListViewModel = null;
        }
    }
}
