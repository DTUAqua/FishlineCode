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
using Microsoft.Win32;
using System.IO;
using System.ComponentModel;

namespace Babelfisk.ViewModels.SmartDots
{
    public class AddEditSDSampleViewModel : Input.AInputViewModel
    {

        private DelegateCommand<SDFile> _cmdRemoveImage;
        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdSave;
        private DelegateCommand _cmdAddImage;
        

        private ObservableCollection<SDFile> _lstfiles;
        private ObservableCollection<SDFile> _lstAddedFiles;
        private ObservableCollection<SDFile> _lstRemovedFiles;
        private SDFile _selectedImage;
        private DelegateCommand<string> _cmdAddEditLookups;


        private List<L_SexCode> _lstSexCode;
        private List<Maturity> _lstMaturity;
        private List<L_OtolithReadingRemark> _lstOtolithReadingRemarks;
        private List<L_SDOtolithDescription> _lstOtolithDescriptions;
        private List<L_EdgeStructure> _lstEdgeStructures;
        private List<L_DFUArea> _lstDFUArea;
        private List<L_StatisticalRectangle> _lstStatisticalRectangles;
        private List<L_Species> _lstSpecies;
        private List<L_Stock> _lstStock;
        private List<L_SDPreparationMethod> _lstPreparationMethods;
        private List<L_SDLightType> _lstLightTypes;
        

        private bool _updatingCollections;
        private bool _canEditSample;
        private AViewModel _vmSDSample = null;
        private SDSample _originalSample;
        private SDSample _entity;

        public bool _isNew = false;
        public bool Canceled = true;


        #region Properties

        /// <summary>
        /// Boolean used when updating the various lookup collections.
        /// </summary>
        private bool UpdatingCollections
        {
            get { return _updatingCollections || _blnValidate; }
        }


        /// <summary>
        /// Boolean used to check if sample can be edited.
        /// </summary>
        private bool CanEditSample
        {
            get { return _canEditSample; }
            set
            {
                _canEditSample = value;
                RaisePropertyChanged(() => CanEditSample);
            }
        }

        /// <summary>
        /// Return list of Sex codes for drop down. 
        /// </summary>
        public List<L_SexCode> Sexes
        {
            get { return _lstSexCode == null ? null : _lstSexCode.ToList(); }
            private set
            {
                _lstSexCode = value;
                RaisePropertyChanged(() => Sexes);
            }
        }

        /// <summary>
        /// Return list of Maturities for drop down. 
        /// </summary>
        public List<Maturity> Maturities
        {
            get { return _lstMaturity == null ? null : _lstMaturity.ToList(); }
            private set
            {
                _lstMaturity = value;
                RaisePropertyChanged(() => Maturities);
            }
        }

        /// <summary>
        /// Return list of Otholith Reading Remarks  for drop down. 
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
        /// Return list of Otholith Reading Remarks  for drop down. 
        /// </summary>
        public List<L_SDOtolithDescription> OtolithDescriptions
        {
            get { return _lstOtolithDescriptions == null ? null : _lstOtolithDescriptions.ToList(); }
            private set
            {
                _lstOtolithDescriptions = value;
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
        /// Return list of Static Rectangles for drop down. 
        /// </summary>
        public List<L_StatisticalRectangle> StatisticalRectangles
        {
            get { return _lstStatisticalRectangles == null ? null : _lstStatisticalRectangles.ToList(); }
            private set
            {
                _lstStatisticalRectangles = value;
                RaisePropertyChanged(() => StatisticalRectangles);
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
        /// Return list of Stock for drop down. 
        /// </summary>
        public List<L_Stock> Stocks
        {
            get { return _lstStock == null ? null : _lstStock.ToList(); }
            private set
            {
                _lstStock = value;
                RaisePropertyChanged(() => Stocks);
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
        /// Return list of Light types for drop down. 
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
        /// Property holding the Sample Id.
        /// </summary>
        public string AnimalId
        {
            get { return _entity.animalId; }
            set
            {
                if (_entity.animalId != value)
                    _entity.animalId = value;

                RaisePropertyChanged(() => AnimalId);
            }
            
        }


        /// <summary>
        /// Property holding the Catch Date.
        /// </summary>
        public DateTime? CatchDate
        {
            get { return _entity.catchDate; }
            set
            {
                if (_entity.catchDate != value)
                    _entity.catchDate = value;

                RaisePropertyChanged(() => CatchDate);
            }
        }


        /// <summary>
        /// Selected DFU area property.
        /// </summary>
        public L_DFUArea SelectedDFUArea
        {
            get { return _entity == null ? null : _entity.L_DFUArea; }
            set
            {
                if (!UpdatingCollections)
                {
                    if (_entity.L_DFUArea != value)
                        _entity.L_DFUArea = value;
                }

                RaisePropertyChanged(() => SelectedDFUArea);
            }
        }


        /// <summary>
        /// Property holding the Length in mm.
        /// </summary>
        public int? LengthMM
        {
            get { return _entity.fishLengthMM; }
            set
            {
                if (_entity.fishLengthMM != value)
                    _entity.fishLengthMM = value;

                RaisePropertyChanged(() => LengthMM);
            }
        }

        /// <summary>
        /// Selected Preparation Method property.
        /// </summary>
        public L_SDPreparationMethod SelectedPreparationMethod
        {
            get { return _entity == null ? null : _entity.L_SDPreparationMethod; }
            set
            {
                if (!UpdatingCollections)
                {
                    if (_entity.L_SDPreparationMethod != value)
                        _entity.L_SDPreparationMethod = value;
                }

                RaisePropertyChanged(() => SelectedPreparationMethod);
            }
        }


        /// <summary>
        /// Property holding the Length in mm.
        /// </summary>
        public decimal? WeightG
        {
            get { return _entity.fishWeightG; }
            set
            {
                if (_entity.fishWeightG != value)
                    _entity.fishWeightG = value;

                RaisePropertyChanged(() => WeightG);
            }
        }


        /// <summary>
        /// Selected sex code property.
        /// </summary>
        public L_SexCode SelectedSexCode
        {
            get { return _entity == null ? null : _entity.L_SexCode; }
            set
            {
                if (!UpdatingCollections)
                {
                    if (_entity.L_SexCode != value)
                    {
                        _entity.L_SexCode = value;
                    }
                }

                RaisePropertyChanged(() => SelectedSexCode);
            }
        }

        /// <summary>
        /// Selected maturity property.
        /// </summary>
        public Maturity SelectedMaturity
        {
            get { return _entity == null ? null : _entity.Maturity; }
            set
            {
                if (!UpdatingCollections)
                {
                    if (_entity.Maturity != value)
                        _entity.Maturity = value;
                }

                RaisePropertyChanged(() => SelectedMaturity);
            }
        }

        /// <summary>
        /// Selected Otolith ReadingRemark property.
        /// </summary>
        public L_OtolithReadingRemark SelectedOtolithReadingRemark
        {
            get { return _entity == null ? null : _entity.L_OtolithReadingRemark; }
            set
            {
                if (!UpdatingCollections)
                {
                    if (_entity.L_OtolithReadingRemark != value)
                        _entity.L_OtolithReadingRemark = value;
                }

                RaisePropertyChanged(() => SelectedOtolithReadingRemark);
            }
        }


        /// <summary>
        /// Selected Otolith ReadingRemark property.
        /// </summary>
        public L_SDOtolithDescription SelectedOtolithDescription
        {
            get { return _entity == null ? null : _entity.L_SDOtolithDescription; }
            set
            {
                if (!UpdatingCollections)
                {
                    if (_entity.L_SDOtolithDescription != value)
                        _entity.L_SDOtolithDescription = value;
                }

                RaisePropertyChanged(() => SelectedOtolithDescription);
            }
        }

        /// <summary>
        /// Selected Edge Structure property.
        /// </summary>
        public L_EdgeStructure SelectedEdgeStructure
        {
            get { return _entity == null ? null : _entity.L_EdgeStructure; }
            set
            {
                if (!UpdatingCollections)
                {
                    if (_entity.L_EdgeStructure != value)
                        _entity.L_EdgeStructure = value;
                }

                RaisePropertyChanged(() => SelectedEdgeStructure);
            }
        }


       

        /// <summary>
        /// Selected Statical Rectangle property.
        /// </summary>
        public L_StatisticalRectangle SelectedStaticRectangle
        {
            get { return _entity == null ? null : _entity.L_StatisticalRectangle; }
            set
            {
                if (!UpdatingCollections)
                {
                    if (_entity.L_StatisticalRectangle != value)
                        _entity.L_StatisticalRectangle = value;
                }

                RaisePropertyChanged(() => SelectedStaticRectangle);
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
        /// Selected Stock property.
        /// </summary>
        public L_Stock SelectedStock
        {
            get { return _entity == null ? null : _entity.L_Stock; }
            set
            {
                if (!UpdatingCollections)
                {
                    if (_entity.L_Stock != value)
                        _entity.L_Stock = value;
                }

                RaisePropertyChanged(() => SelectedStock);
            }
        }

      

        /// <summary>
        /// Selected Light Type property.
        /// </summary>
        public L_SDLightType SelectedLightType
        {
            get { return _entity == null ? null : _entity.L_SDLightType; }
            set
            {
                if (!UpdatingCollections)
                {
                    if (_entity.L_SDLightType != value)
                        _entity.L_SDLightType = value;
                }

                RaisePropertyChanged(() => SelectedPreparationMethod);
            }
        }


        /// <summary>
        /// Property holding the Latitude.
        /// </summary>
        public Nullable<double> Latitude
        {
            get { return _entity.latitude; }
            set
            {
                if (_entity.latitude != value)
                    _entity.latitude = value;

                RaisePropertyChanged(() => Latitude);
            }
        }

        /// <summary>
        /// Property holding the Latitude.
        /// </summary>
        public Nullable<double> Longitude
        {
            get { return _entity.longitude; }
            set
            {
                if (_entity.longitude != value)
                    _entity.longitude = value;

                RaisePropertyChanged(() => Longitude);
            }
        }

        /// <summary>
        /// Property holding the Stock.
        /// </summary>
        public int? Stock
        {
            get { return _entity.stockId; }
            set
            {
                if (_entity.stockId != value)
                    _entity.stockId = value;

                RaisePropertyChanged(() => Stock);
            }
        }

        /// <summary>
        /// Property holding Cruise.
        /// </summary>
        public string Cruise
        {
            get { return _entity.cruise; }
            set
            {
                if (_entity.cruise != value)
                    _entity.cruise = value;

                RaisePropertyChanged(() => Cruise);
            }
        }

        /// <summary>
        /// Property holding Trip.
        /// </summary>
        public string Trip
        {
            get { return _entity.trip; }
            set
            {
                if (_entity.trip != value)
                    _entity.trip = value;

                RaisePropertyChanged(() => Trip);
            }
        }

        // <summary>
        /// Property holding Station.
        /// </summary>
        public string Station
        {
            get { return _entity.station; }
            set
            {
                if (_entity.station != value)
                    _entity.station = value;

                RaisePropertyChanged(() => Station);
            }
        }


        /// <summary>
        /// Property holding the Created By User Name.
        /// </summary>
        public string CreatedByUserName
        {
            get { return _entity.createdByUserName; }
           
        }

       

        /// <summary>
        /// Property holding the Catch Date.
        /// </summary>
        public DateTime? Created
        {
            get { return _entity.createdTime.HasValue ? new Nullable<DateTime>(_entity.createdTime.Value.ToLocalTime()) : null; }
            
        }

        /// <summary>
        /// Property holding the Catch Date.
        /// </summary>
        public DateTime? Modified
        {
            get { return _entity.modifiedTime.HasValue ? new Nullable<DateTime>(_entity.modifiedTime.Value.ToLocalTime()) : null; }

        }

        /// <summary>
        /// Property holding the Comments Date.
        /// </summary>
        public string Comments
        {
            get { return _entity.comments; }
            set
            {
                if (_entity.comments != value)
                    _entity.comments = value;

                RaisePropertyChanged(() => Comments);
            }

        }

        /// <summary>
        /// Assigned Sample Images.
        /// </summary>
        public ObservableCollection<SDFile> SampleImages
        {
            get { return _lstfiles; }
            set
            {
                _lstfiles = value;
                RaisePropertyChanged(() => SampleImages);
            }
        }


        /// <summary>
        /// Property holding the Latitude.
        /// </summary>
        public bool ReadOnly
        {
            get { return _entity.readOnly; }
            set
            {
                if (_entity.readOnly != value)
                {
                    _entity.readOnly = value;
                    IsDirty = true;
                }
                    

                RaisePropertyChanged(() => ReadOnly);
            }
        }

        public SDFile SelectedImage
        {
            get { return _selectedImage;}
            set
            {
                _selectedImage = value;
                RaisePropertyChanged(() => SelectedImage);
            }
        }

        public SDSample SampleEntity
        {
            get { return _entity; }
        }


        public bool IsNew
        {
            get { return _isNew; }
            set
            {
                _isNew = value;
                RaisePropertyChanged(() => IsNew);
            }
        }

        #endregion


        private AddEditSDSampleViewModel()
        { }

        public AddEditSDSampleViewModel(AViewModel returnToViewModel, SDSample sample, bool isNew = false)
        {
            WindowWidth = 800;
            WindowHeight = 650;
            _vmSDSample = returnToViewModel;

            IsNew = isNew;

            _originalSample = sample;
            _entity = sample.Clone();

            if (sample.SDFile == null)
                _lstfiles = new ObservableCollection<SDFile>();

            _lstfiles = sample.SDFile.ToObservableCollection();
            _lstAddedFiles = new ObservableCollection<SDFile>();
            _lstRemovedFiles = new ObservableCollection<SDFile>();
        }


      
        public void InitializeAsync()
        {
            IsLoading = true;
            Task.Factory.StartNew(Initialize).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }

        private void Initialize()
        {
            try
            {
                var manLookup = new LookupManager();
                var lv = new LookupDataVersioning();

                //Fetch Sex codes
                var lstSexCodes = manLookup.GetLookups(typeof(L_SexCode), lv).OfType<L_SexCode>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch Maturities
                var lstMatuties = manLookup.GetLookups(typeof(Maturity), lv).OfType<Maturity>().OrderBy(x => x.maturityIndexMethod ?? "").ThenBy(x => x.maturityIndex).ToList();

                //Fetch Otolith Reading Remarks
                var lstOtolithReadingRemarks = manLookup.GetLookups(typeof(L_OtolithReadingRemark), lv).OfType<L_OtolithReadingRemark>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch Otolith Reading Remarks
                var lstOtolithDescriptions = manLookup.GetLookups(typeof(L_SDOtolithDescription), lv).OfType<L_SDOtolithDescription>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch Otolith Reading Remarks
                var lstEdgeStructures = manLookup.GetLookups(typeof(L_EdgeStructure), lv).OfType<L_EdgeStructure>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch DFUAreas
                var lstDFUAreas = manLookup.GetLookups(typeof(L_DFUArea), lv).OfType<L_DFUArea>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch Stat. Rect.
                var lstStatRectangles = manLookup.GetLookups(typeof(L_StatisticalRectangle), lv).OfType<L_StatisticalRectangle>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch species
                var lstSpecies = manLookup.GetLookups(typeof(L_Species), lv).OfType<L_Species>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch Stock
                var lstStocks = manLookup.GetLookups(typeof(L_Stock), lv).OfType<L_Stock>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch Preparation Methods
                var lstPreparationMethods = manLookup.GetLookups(typeof(L_SDPreparationMethod), lv).OfType<L_SDPreparationMethod>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch Light Types
                var lstLightTypes = manLookup.GetLookups(typeof(L_SDLightType), lv).OfType<L_SDLightType>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch sample types
                var lstSampleTypes = manLookup.GetLookups(typeof(L_SampleType), lv).OfType<L_SampleType>().OrderBy(x => x.UIDisplay).ToList();

                new Action(() =>
                {
                    _updatingCollections = true;
                    {
                        DFUAreas = lstDFUAreas;
                        if (!string.IsNullOrEmpty(_entity.DFUArea))
                        {
                            _entity.L_DFUArea = DFUAreas.Where(x => x.DFUArea == _entity.DFUArea).FirstOrDefault();
                            RaisePropertyChanged(() => SelectedDFUArea);
                        }

                        StatisticalRectangles = lstStatRectangles;
                        if (!string.IsNullOrEmpty(_entity.statisticalRectangle))
                        {
                            _entity.L_StatisticalRectangle = StatisticalRectangles.Where(x => x.statisticalRectangle == _entity.statisticalRectangle).FirstOrDefault();
                            RaisePropertyChanged(()=>SelectedStaticRectangle);
                        }

                        Sexes = lstSexCodes;
                        if (!string.IsNullOrEmpty(_entity.sexCode))
                        {
                            _entity.L_SexCode = Sexes.Where(x => x.sexCode == _entity.sexCode).FirstOrDefault();
                            RaisePropertyChanged(() => SelectedSexCode);
                        }

                        Maturities = lstMatuties;
                        if (_entity.Maturity != null)
                        {
                            _entity.Maturity = Maturities.Where(x => x.maturityId == _entity.maturityId).FirstOrDefault();
                            RaisePropertyChanged(() => SelectedMaturity);
                        }

                        OtolithReadingRemarks = lstOtolithReadingRemarks;
                        if(_entity.otolithReadingRemarkId!= null)
                        {
                            _entity.L_OtolithReadingRemark = OtolithReadingRemarks.Where(x => x.L_OtolithReadingRemarkID == _entity.otolithReadingRemarkId).FirstOrDefault();
                            RaisePropertyChanged(() => SelectedOtolithReadingRemark);
                        }

                        OtolithDescriptions = lstOtolithDescriptions;
                        if (_entity.L_SDOtolithDescription != null)
                        {
                            _entity.L_SDOtolithDescription = OtolithDescriptions.Where(x => x.L_sdOtolithDescriptionId == _entity.sdOtolithDescriptionId).FirstOrDefault();
                            RaisePropertyChanged(() => SelectedOtolithDescription);
                        }

                        EdgeStructures = lstEdgeStructures;
                        if (_entity.L_EdgeStructure != null)
                        {
                            _entity.L_EdgeStructure = EdgeStructures.Where(x => x.edgeStructure == _entity.edgeStructure).FirstOrDefault();
                            RaisePropertyChanged(() => SelectedEdgeStructure);
                        }

                        Species = lstSpecies;
                        if (_entity.L_Species != null)
                        {
                            _entity.L_Species = Species.Where(x => x.speciesCode == _entity.speciesCode).FirstOrDefault();
                            RaisePropertyChanged(() => SelectedSpecies);
                        }

                        Stocks = lstStocks;
                        if (_entity.L_Stock != null)
                        {
                            _entity.L_Stock = Stocks.Where(x => x.L_stockId == _entity.stockId).FirstOrDefault();
                            RaisePropertyChanged(() => SelectedStock);
                        }

                        PreparationMethods = lstPreparationMethods;
                        if (_entity.L_SDPreparationMethod != null)
                        {
                            _entity.L_SDPreparationMethod = PreparationMethods.Where(x => x.L_sdPreparationMethodId == _entity.sdPreparationMethodId).FirstOrDefault();
                            RaisePropertyChanged(() => SelectedPreparationMethod);
                        }

                        LightTypes = lstLightTypes;
                        if(_entity.L_SDLightType != null)
                        {
                            _entity.L_SDLightType = LightTypes.Where(x => x.L_sdLightTypeId == _entity.sdLightTypeId).FirstOrDefault();
                            RaisePropertyChanged(() => SelectedLightType);
                        }
                    }
                    _updatingCollections = false;

                    //Assign species to event species, if new.
                    try
                    {
                        if (IsNew && SelectedSpecies == null && Species != null && (_vmSDSample is SDSamplesViewModel) && (_vmSDSample as SDSamplesViewModel).Event != null)
                        {
                            SelectedSpecies = Species.Where(x => x.speciesCode == (_vmSDSample as SDSamplesViewModel).Event.speciesCode).FirstOrDefault();
                        }
                    }
                    catch { }

                }).Dispatch();
            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }

        }

       

        #region Delete Image Command


        public DelegateCommand<SDFile> DeleteImageCommand
        {
            get
            {
                if (_cmdRemoveImage == null)
                    _cmdRemoveImage = new DelegateCommand<SDFile>(param => RemoveImage(param));

                return _cmdRemoveImage;
            }
        }


        private void RemoveImage(SDFile file)
        {
            try
            {
                if(file.SDAnnotation.Count > 0)
                {
                    DispatchMessageBox(string.Format(Translate("AddEditSDSampleView", "DeletionErrorImageHasAnnotations"), file.fileName));
                    return;
                }


                //Check if image is added recently, if not add it to the removed SDFile list
                if (!_lstAddedFiles.Any(x => x.fileName == file.fileName))
                    _lstRemovedFiles.Add(file);

                //remove from the mail list of SDFile 
                _lstfiles.Remove(_lstfiles.Where(x => x.displayName == file.displayName).FirstOrDefault());
                RaisePropertyChanged(() => SampleImages);

                //remove from the recently added list of SDFiles 
                _lstAddedFiles.Remove(_lstAddedFiles.Where(x => x.fileName == file.fileName).FirstOrDefault());
            }
            catch (Exception)
            {

            }
           
        }

        #endregion


        #region Add ImageCommand


        public DelegateCommand AddImageCommand
        {
            get
            {
                if (_cmdAddImage == null)
                    _cmdAddImage = new DelegateCommand(() => SelectFile());
                return _cmdAddImage;
            }
        }


        public static bool IsImageAlreadyAddedToEvent(SDSamplesViewModel vmSamples, string imageName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageName) || vmSamples == null || !vmSamples.HasSamples)
                    return false;

                if (vmSamples.SDSampleList.Where(x => x.HasSDFiles && x.SDFile.Where(f => f.fileName != null && f.fileName.Equals(imageName, StringComparison.InvariantCultureIgnoreCase)).Any()).Any())
                    return true;
            }
            catch(Exception e) 
            {
                vmSamples.LogError(e);
            }

            return false;
        }

        private void SelectFile()
        {
            
            try
            {
                //Check 
                if(_vmSDSample is SDSamplesViewModel && (_vmSDSample as SDSamplesViewModel).Event != null && (_vmSDSample as SDSamplesViewModel).Event.IsYearlyReadingEventType &&
                   SampleImages != null && SampleImages.Count > 0)
                {
                    DispatchMessageBox(Translate("AddEditSDSampleView", "WarningOnlyOneImageForYearlyEvents"));
                    return;
                }

                string[] serverImageFolders = null;
                if (_vmSDSample != null && (_vmSDSample is SDSamplesViewModel) && (_vmSDSample as SDSamplesViewModel).Event != null)
                    serverImageFolders = (_vmSDSample as SDSamplesViewModel).Event.DefaultImageFoldersArray;

                var vm = new SelectFoldersOrFilesViewModel(false, true, true, serverImageFolders);
                vm.WindowWidth = 650;
                vm.WindowHeight = 500;
                vm.WindowTitle = Translate("AddEditSDEventView", "SelectFoldersHeader"); //"Select folders";

                AppRegionManager.LoadWindowViewFromViewModel(vm, true, "StandardWindowWithBorderStyle");

                //Assign selection, unless Cancel was clicked.
                if (!vm.IsCancelled)
                {
                    var pathsToAdd = vm.GetSelectedFiles();

                    if (pathsToAdd != null)
                    {
                        foreach (var p in pathsToAdd)
                        {
                            var selectedItem = new OtolithFileItem(null, p);

                            if (SampleImages != null && SampleImages.Count > 0 && SampleImages.Any(x => x.displayName == selectedItem.ImageName))
                            {
                                if (AppRegionManager.ShowMessageBox(string.Format(Translate("AddEditSDSampleView", "WarningImageAlreadyAddedWithInput"), selectedItem.ImageName), System.Windows.MessageBoxButton.OK) == System.Windows.MessageBoxResult.OK)
                                    continue;
                            }
                            else if (IsImageAlreadyAddedToEvent(_vmSDSample as SDSamplesViewModel, selectedItem.ImageName))
                            {
                                DispatchMessageBox(string.Format(Translate("AddEditSDSampleView", "WarningImageAlreadyAddedAnotherSample"), selectedItem.ImageName));
                                continue;
                            }
                            else if (_vmSDSample is SDSamplesViewModel && (_vmSDSample as SDSamplesViewModel).Event != null && (_vmSDSample as SDSamplesViewModel).Event.IsYearlyReadingEventType &&
                                     SampleImages != null && SampleImages.Count > 0)
                            {
                                DispatchMessageBox(Translate("AddEditSDSampleView", "WarningOnlyOneImageForYearlyEventsOngoing"));
                                return;
                            }
                            else
                            {
                                var img = new SDFile() { sdFileGuid = Guid.NewGuid(), fileName = selectedItem.ImageName, displayName = Path.GetFileName(selectedItem.ImageName), path = selectedItem.ImageDirectory };
                                //Add image to the main list
                                SampleImages.Add(img);
                                //Add to the recently added list
                                _lstAddedFiles.Add(img);
                            }
                        }
                    }

                }

                /*var vm = new SmartDots.SelectOtolithImagesViewModel(serverImageFolders);
                vm.InitializeAsync();
                AppRegionManager.LoadWindowViewFromViewModel(vm, true, "WindowWithBorderStyle");
                if (vm != null && !vm.IsCanceled && vm.SelectedOtolithItemList != null && vm.SelectedOtolithItemList.Count > 0)
                {
                    foreach (var selectedItem in vm.SelectedOtolithItemList)
                    {
                        if (SampleImages != null && SampleImages.Count > 0 && SampleImages.Any(x => x.displayName == selectedItem.ImageName))
                        {
                            if (AppRegionManager.ShowMessageBox(string.Format(Translate("AddEditSDSampleView", "WarningImageAlreadyAddedWithInput"), selectedItem.ImageName), System.Windows.MessageBoxButton.OK) == System.Windows.MessageBoxResult.OK)
                                continue;
                        }
                        else if (IsImageAlreadyAddedToEvent(_vmSDSample as SDSamplesViewModel, selectedItem.ImageName))
                        {
                            DispatchMessageBox(string.Format(Translate("AddEditSDSampleView", "WarningImageAlreadyAddedAnotherSample"), selectedItem.ImageName));
                            continue;
                        }
                        else if (_vmSDSample is SDSamplesViewModel && (_vmSDSample as SDSamplesViewModel).Event != null && (_vmSDSample as SDSamplesViewModel).Event.IsYearlyReadingEventType &&
                                 SampleImages != null && SampleImages.Count > 0)
                        {
                            DispatchMessageBox(Translate("AddEditSDSampleView", "WarningOnlyOneImageForYearlyEventsOngoing"));
                            return;
                        }
                        else
                        {
                            var img = new SDFile() { sdFileGuid = Guid.NewGuid(), fileName = selectedItem.ImageName, displayName = Path.GetFileName(selectedItem.ImageName), path = selectedItem.ImageDirectory };
                            //Add image to the main list
                            SampleImages.Add(img);
                            //Add to the recently added list
                            _lstAddedFiles.Add(img);
                        }
                    }
                }*/

            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
            finally
            {
                RaisePropertyChanged(() => SampleImages);
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
        /// Cancel any alterations or the new sample.
        /// </summary>
        private void Return()
        {
            Canceled = true;
            Close();
        }


        //Window closed button same logic as cancel button
        public override void FireClosing(object sender, CancelEventArgs e)
        {
            try
            {
                if (!Canceled)
                    return;

                //check for changes
                if (HasChanges() || (_lstAddedFiles != null && _lstAddedFiles.Count > 0) || (_lstRemovedFiles != null && _lstRemovedFiles.Count > 0))
                {
                    if (AppRegionManager.ShowMessageBox(Translate("AddEditSDSampleView", "HasChangesWarning"), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                    {
                        e.Cancel = true;
                        return;
                    }

                }
            }
            catch(Exception ex)
            {
                LogError(ex);
            }

        }

        #endregion


        #region Add Edit Lookups Command


        public DelegateCommand<string> AddEditLookupsCommand
        {
            get { return _cmdAddEditLookups ?? (_cmdAddEditLookups = new DelegateCommand<string>(p => AddEditLookups(p))); }
        }


        /// <summary>
        /// Add/Or edit a lookup list.
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
            catch (Exception e)
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

            try
            {
                if (!_blnValidate)
                    return strError;

                switch (strFieldName)
                {
                    case "AnimalId":
                        if (string.IsNullOrWhiteSpace(AnimalId))
                            strError = Translate("AddEditSDSampleView", "ValidationAnimalId");// "Please specify an Animal id"; 
                        else
                        {
                            var vm = _vmSDSample as SDSamplesViewModel;
                            bool isYearlyReading = _vmSDSample is SDSamplesViewModel && (_vmSDSample as SDSamplesViewModel).Event != null && (_vmSDSample as SDSamplesViewModel).Event.IsYearlyReadingEventType;
                            if (isYearlyReading && vm != null && vm.SDSampleList != null && vm.SDSampleList.Where(x => x.sdSampleId != _originalSample.sdSampleId && x.animalId != null && x.animalId.Equals(AnimalId, StringComparison.InvariantCultureIgnoreCase) && x.sdPreparationMethodId == _entity.sdPreparationMethodId).Any())
                                strError = string.Format(Translate("AddEditSDSampleView", "DupliacteAnimalIdForYearlyReadingEvent"), AnimalId);
                        }

                        break;
                    case "Cruise":
                        if (Cruise != null && Cruise.Length > 20)
                            strError = Translate("AddEditSDSampleView", "ValidationCruiseLength"); // "Cruise value cannt be longer than 20 character";
                        break;

                    case "Trip":
                        if (Trip != null && Trip.Length > 10)
                            strError = Translate("AddEditSDSampleView", "ValidationTripLength");
                        break;

                    case "Station":
                        if (Station != null && Station.Length > 10)
                            strError = Translate("AddEditSDSampleView", "ValidationStationLength");
                        break;

                    case "Latitude":
                        if(Latitude.HasValue && (Latitude.Value < -90 || Latitude.Value > 90))
                            strError = Translate("AddEditSDSampleView", "ValidationLatitudeRange");
                        break;

                    case "Longitude":
                        if (Longitude.HasValue && (Longitude.Value < -180 || Longitude.Value > 180))
                            strError = Translate("AddEditSDSampleView", "ValidationLongitudeRange");
                        break;

                    case "CatchDate":
                        if (CatchDate == null || (CatchDate != null && CatchDate == default(DateTime)))
                           strError = Translate("AddEditSDSampleView", "ValidationCatchDate");
                        break;

                    case "SelectedDFUArea":
                        //Use the entity directory for checking for null instead of SelectedDFUArea, because that will make validation work for the IsSDSampleValid method as well.
                        if (_entity.DFUArea == null)
                            strError = Translate("AddEditSDSampleView", "ValidationSelectedDFUArea");
                        break;

                    case "LengthMM":
                        if (LengthMM == null || LengthMM == 0)
                            strError = Translate("AddEditSDSampleView", "ValidationLength");
                        break;

                    case "SelectedPreparationMethod":
                        if (SelectedPreparationMethod == null)
                            strError = Translate("AddEditSDSampleView", "ValidationSelectedPreparationMethod");
                        break;

                    case "SelectedLightType":
                        if(SelectedLightType == null)
                            strError = Translate("AddEditSDSampleView", "ValidationLightTypeMandatory");
                        break;
                }
            }
            catch { }

            return strError;
        }


        /// <summary>
        /// Method for validating an SDSample using the same methods for validation as AddEditSDSampleViewModel has.
        /// </summary>
        /// <param name="errorMessage">If true is returned by the method, the error message will be assigned to this argument</param>
        /// <returns>Whether the SDSample is valid or not</returns>
        public static bool IsSDSampleValid(SDSamplesViewModel vmSamples, SDSample sd, ref string errorMessage)
        {
            var vm = new AddEditSDSampleViewModel();
            vm._entity = sd;
            vm._originalSample = sd;
            vm._vmSDSample = vmSamples;
            vm.IsNew = false;
            vm._blnValidate = true;
            vm.ValidateAllProperties(false, false);
            vm._blnValidate = false;

            errorMessage = vm.Error;
            return !vm.HasErrors;
        }


        protected override void ValidateAndSaveAsync()
        {
            _blnValidate = true;
            {
                //Validate all properties
                ValidateAllProperties();
            }
            _blnValidate = false;

            //If any errors, show them.
            if (HasErrors)
            {
                AppRegionManager.ShowMessageBox(Error, 5);
                return;
            }

            IsLoading = true;
            Task.Factory.StartNew(Save).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        /// <summary>
        /// Save any alterations or the new sample.
        /// </summary>
        private void Save()
        {
            try
            {
                if (_lstAddedFiles != null && _lstAddedFiles.Count > 0)
                {
                    foreach (var item in _lstAddedFiles)
                    {
                        _originalSample.SDFile.Add(item);
                        IsDirty = true;
                    }
                }

                if (_lstRemovedFiles != null && _lstRemovedFiles.Count > 0)
                {
                    foreach (var item in _lstRemovedFiles)
                    {
                        if (_originalSample.SDFile.Any(x => x.displayName == item.displayName))
                        {
                            _originalSample.SDFile.Remove(_originalSample.SDFile.Where(x => x.displayName == item.displayName).FirstOrDefault());
                            IsDirty = true;
                        }
                    }
                }

                bool hasChanges = IsDirty || HasChanges();

                //Copy all value type properties to original sample.
                _entity.CopyEntityValueTypesTo(_originalSample);

                if (!IsNew)
                {
                    if (hasChanges)
                    {
                        _originalSample.ChangeTracker.State = ObjectState.Modified;
                        _originalSample.ModifiedTimeLocal = DateTime.Now;
                    }
                    else
                    {
                        DispatchMessageBox(Translate("Common", "NoChanges"));
                        return;
                    }
                }

                //Copy navigation properties to original sample.
                _originalSample.L_SexCode = _entity.L_SexCode;
                _originalSample.L_DFUArea = _entity.L_DFUArea;
                _originalSample.L_StatisticalRectangle = _entity.L_StatisticalRectangle;
                _originalSample.Maturity = _entity.Maturity;
                _originalSample.L_OtolithReadingRemark = _entity.L_OtolithReadingRemark;
                _originalSample.L_SDOtolithDescription = _entity.L_SDOtolithDescription;
                _originalSample.L_EdgeStructure = _entity.L_EdgeStructure;
                _originalSample.L_Species = _entity.L_Species;
                _originalSample.L_Stock = _entity.L_Stock;
                _originalSample.L_SDPreparationMethod = _entity.L_SDPreparationMethod;
                _originalSample.L_SDLightType = _entity.L_SDLightType;

                Canceled = false;

                //Needs to dispatch this to the main UI thread, since the save method is executed in its own thread.
                new Action(() =>
                {
                    Close();
                }).Dispatch();
            }
            catch(Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }

            IsDirty = false;
        }





        private bool  HasChanges()
        {
            if (_entity != null && _originalSample != null && _entity.HasValueTypeChanges(_originalSample))
                return true;

            if(IsDirty)
                return true;


            return false;

        } 


        #endregion


    }
}
