using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using Anchor.Core;
using System.Windows.Data;
using Babelfisk.Entities.Sprattus;
using System.Windows.Controls;
using Babelfisk.Entities;

namespace Babelfisk.ViewModels.Lookup
{
    public class LookupManagerViewModel : AViewModel
    {
        /// <summary>
        /// Command for selecting a lookup type.
        /// </summary>
        private DelegateCommand<LookupType> _cmdLookupTypeSelected;


        /// <summary>
        /// Selected lookup view model.
        /// </summary>
        private AViewModel _vmLookup;


        /// <summary>
        /// Selected lookup type.
        /// </summary>
        private LookupType _selectedLookupType;


        /// <summary>
        /// List with all lookup types (used for populating _colLookupTypes).
        /// </summary>
        private List<LookupType> _lstAllLookupTypes;


        /// <summary>
        /// Collection bound to UI.
        /// </summary>
        private ICollectionView _colLookupTypes;


        /// <summary>
        /// Lookup type search field value.
        /// </summary>
        private string _strSearchString;


        private bool _blnChangesSaved;


        private Type _defaultSelectedType;


        #region Properties

        /// <summary>
        /// Property set to true when a LookupViewModel was successfully saved.
        /// </summary>
        public bool ChangesSaved
        {
            get { return _blnChangesSaved; }
            set
            {
                _blnChangesSaved = value;
                RaisePropertyChanged(() => ChangesSaved);
            }
        }


        public AViewModel SelectedLookupViewModel
        {
            get { return _vmLookup; }
            set
            {
                _vmLookup = value;
                RaisePropertyChanged(() => SelectedLookupViewModel);
            }
        }


        public LookupType SelectedLookupType
        {
            get { return _selectedLookupType; }
            set
            {
                _selectedLookupType = value;
                RaisePropertyChanged(() => SelectedLookupType);
                RaisePropertyChanged(() => IsLookupSelected);
            }
        }


        public bool IsLookupSelected
        {
            get { return _selectedLookupType != null; }
        }


        public override bool HasUnsavedData
        {
            get
            {
                return IsSelectedLookupDirty();
            }
        }


        public string SearchString
        {
            get { return _strSearchString; }
            set
            {
                _strSearchString = value;
                RaisePropertyChanged(() => SearchString);

                if (LookupTypesCollection != null)
                    LookupTypesCollection.Refresh();
            }
        }


        public ICollectionView LookupTypesCollection
        {
            get { return _colLookupTypes; }
            protected set
            {
                _colLookupTypes = value;
                RaisePropertyChanged(() => LookupTypesCollection);
            }
        }



        #endregion


        public LookupManagerViewModel(Type t = null)
        {
            _defaultSelectedType = t;
            WindowTitle = "Kodelister";
            WindowWidth = 950;
            WindowHeight = 650;

            InitializeAsync();
        }


        private void InitializeAsync()
        {
            IsLoading = true;
            Task.Factory.StartNew(LoadLookupTypes);
        }


        private void LoadLookupTypes()
        {
            _lstAllLookupTypes = new List<LookupType>();

            #region Lookup type creation

            

            _lstAllLookupTypes.Add(new LookupType("Personer (aqua)", typeof(DFUPerson), new string[] { "L_DFUDepartment" }, LoadDFUPersonLists,
                                  LookupColumn.Create("Initialer", "initials", false, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Fulde navn", "name", false, new DataGridLength(100, System.Windows.Controls.DataGridLengthUnitType.Star))
                                /*ComboboxLookupColumn.Create("Sektion", "L_DFUDepartment", false, "DFUDepartments", "CodeDescription", null, null, new DataGridLength(100, System.Windows.Controls.DataGridLengthUnitType.Star))*/
                                  ) { CanEditOffline = true });


            _lstAllLookupTypes.Add(new LookupType("Skibe", typeof(L_Platform), /*new string[] { "L_PlatformType", "L_Nationality", "Person" }*/ null, LoadPlatformLists, 
                                  LookupColumn.Create("Kode", "platform", true, new System.Windows.Controls.DataGridLength(80)),
                                /*ComboboxLookupColumn.Create("Type", "platformType", false, "PlatformTypes", "CodeDescription", "platformType", "L_PlatformTypeUIDisplay", new System.Windows.Controls.DataGridLength(130)),
                                  LookupColumn.Create("Navn", "name", false, new System.Windows.Controls.DataGridLength(200)),*/
                                  ComboboxLookupColumn.Create("Nationalitet", "nationality", false, "Nationalities", "CodeDescription", "nationality", "L_NationalityUIDisplay", new System.Windows.Controls.DataGridLength(130)),
                                /*ComboboxLookupColumn.Create("Kontaktperson", "contactPersonId", false, "Persons", "name", "personId", "PersionUIDisplay", new System.Windows.Controls.DataGridLength(130)),*/
                                  LookupColumn.Create("Beskrivelse", "description", false, new System.Windows.Controls.DataGridLength(100, DataGridLengthUnitType.Star))
                                  ) { SecurityTasks = new List<SecurityTask>() { SecurityTask.EditLookups, SecurityTask.EditSomeLookups }, CanEditOffline = true });

            
            _lstAllLookupTypes.Add(new LookupType("Kontaktpersoner", typeof(Person), null, null,
                                  LookupColumn.Create("Navn", "name", false, new System.Windows.Controls.DataGridLength(180)),
                                  LookupColumn.Create("Organisation", "organization", false, new DataGridLength(140)),
                                  LookupColumn.Create("Adresse", "address", false, new DataGridLength(200)),
                                  LookupColumn.Create("Post-nr/By", "zipTown", false, new DataGridLength(120)),
                                  LookupColumn.Create("Tlf. nr. (skib)", "telephone", false, new DataGridLength(100)),
                                  LookupColumn.Create("Tlf. nr. (privat)", "telephonePrivate", false, new DataGridLength(100)),
                                  LookupColumn.Create("Tlf. nr. (mobil)", "telephoneMobile", false, new DataGridLength(100)),
                                  LookupColumn.Create("E-mail", "email", false, new DataGridLength(100)),
                                  LookupColumn.Create("Facebook", "facebook", false, new DataGridLength(100)),
                                  LookupColumn.Create("SE nr.", "SEno", false, new DataGridLength(90)),
                                  LookupColumn.Create("Bankkonto", "bankAccountNo", false, new DataGridLength(120))
                                  ) { SecurityTasks = new List<SecurityTask>() { SecurityTask.EditLookups, SecurityTask.EditSomeLookups }, CanEditOffline = true });


            _lstAllLookupTypes.Add(new LookupType("Havne", typeof(L_Harbour), new string[] { "L_Nationality" }, LoadHarbourLists,
                                  LookupColumn.Create("Kode", "harbour", true, new System.Windows.Controls.DataGridLength(90)),
                                  LookupColumn.Create("NES kode", "harbourNES", false, new System.Windows.Controls.DataGridLength(90)),
                                  LookupColumn.Create("EU kode", "harbourEU", false, new System.Windows.Controls.DataGridLength(90)),
                                  ComboboxLookupColumn.Create("Nationalitet", "nationalityUI", false, "Nationalities", "CodeDescription", "nationality", "L_Nationality.nationality", new System.Windows.Controls.DataGridLength(130)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Nationaliteter", typeof(L_Nationality), null, null,
                                  LookupColumn.Create("Kode", "nationality", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Aqua Områder", typeof(L_DFUArea), new string[] { "L_DFUArea2" }, LoadAreaLists,
                                  LookupColumn.Create("Kode", "DFUArea", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("NES kode", "areaNES", false, new System.Windows.Controls.DataGridLength(100)),
                                   LookupColumn.Create("ICES kode", "areaICES", false, new System.Windows.Controls.DataGridLength(100)),
                                  ComboboxLookupColumn.Create("Hører til område", "dfuArea2", false, "Areas", "UIDisplay", "DFUArea", "dfuArea2", new System.Windows.Controls.DataGridLength(150)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star)))
                                  {
                                        ChildLookupType = new ChildLookupType("Squares tilknyttet valgte område", typeof(ICES_DFU_Relation_FF), null, LoadICES_DFU_Relation_FFLists, "DFUAreaUpper", "Area_20_21Upper",
                                                          ComboboxLookupColumn.Create("Område", "area_20_21", true, "Areas", "UIDisplay", "DFUArea", "Area20_21UIDisplay", new System.Windows.Controls.DataGridLength(110), null, true),
                                                          ComboboxLookupColumn.Create("Square", "statisticalRectangle", false, "Rectangles", "UIDisplay", "statisticalRectangle", "RectangleUIDisplay", new System.Windows.Controls.DataGridLength(90, DataGridLengthUnitType.Star))
                                                          )
                                  }
                                  );


            _lstAllLookupTypes.Add(new LookupType("Behandlingsfaktor-grupper", typeof(L_TreatmentFactorGroup), null, null,
                                  LookupColumn.Create("Kode", "treatmentFactorGroup", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star)))
                                  {
                                     ChildLookupType = new ChildLookupType("Behandlingsfaktorer", typeof(TreatmentFactor), null, LoadTreatmentFactorLists, "treatmentFactorGroup", "treatmentFactorGroup",
                                                         ComboboxLookupColumn.Create("Gruppe", "treatmentFactorGroup", true, "TreatmentFactorGroups", "UIDisplay", "treatmentFactorGroup", "treatmentFactorGroup", new System.Windows.Controls.DataGridLength(65, DataGridLengthUnitType.Auto), null, true),
                                                         ComboboxLookupColumn.Create("Behandling", "treatment", false, "Treatments", "UIDisplay", "treatment", "L_TreatmentUIDisplay", new System.Windows.Controls.DataGridLength(200)),
                                                         LookupColumn.Create("Faktor", "factor", false, new DataGridLength(100)),
                                                         LookupColumn.Create("Versionsdato", "versioningDate", false, new DataGridLength(100, DataGridLengthUnitType.Auto)),
                                                         LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                                         )                                      
                                  });


            _lstAllLookupTypes.Add(new LookupType("Modenhedsmetoder", typeof(L_MaturityIndexMethod), null, null,
                                  LookupColumn.Create("Kode", "maturityIndexMethod", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star)))
                                  {
                                     ChildLookupType = new ChildLookupType("Modenhedsindeks", typeof(Maturity), null, LoadMaturityLists, "maturityIndexMethod", "maturityIndexMethod",
                                                         ComboboxLookupColumn.Create("Modenhedsmetode", "maturityIndexMethod", true, "MaturityIndexMethods", "UIDisplay", "maturityIndexMethod", "maturityIndexMethod", new System.Windows.Controls.DataGridLength(120, DataGridLengthUnitType.Auto), null, true),
                                                         LookupColumn.Create("Modenhedsindeks", "maturityIndex", false, new System.Windows.Controls.DataGridLength(120, DataGridLengthUnitType.Auto)),
                                                         LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                                         )      
                                  }
                                  );


            _lstAllLookupTypes.Add(new LookupType("Behandlinger", typeof(L_Treatment), null, null,
                                  LookupColumn.Create("Kode", "treatment", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Arter", typeof(L_Species), new string[] { "L_TreatmentFactorGroup" }, LoadSpeciesLists,
                                  LookupColumn.Create("Kode", "speciesCode", true, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Danske navn", "dkName", false, new System.Windows.Controls.DataGridLength(100, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Engelske navn", "ukName", false, new System.Windows.Controls.DataGridLength(100, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Latinske navn", "latin", false, new System.Windows.Controls.DataGridLength(100, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("NODC kode", "nodc", false, new System.Windows.Controls.DataGridLength(100, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("ICES kode", "icesCode", false, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("NES kode", "speciesNES", false, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("FAO kode", "speciesFAO", false, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto)),
                                  ComboboxLookupColumn.Create("Behandlingsfaktor-gruppe", "treatmentFactorGroupUI", false, "TreatmentFactors", "UIDisplay", "treatmentFactorGroup", "L_TreatmentFactorGroup.UIDisplay", new System.Windows.Controls.DataGridLength(110, DataGridLengthUnitType.Auto)),
                                //LookupColumn.Create("Aqua-base kode", "dfuFisk_Code", false, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Tsn", "tsn", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Aphia id", "aphiaID", false, new System.Windows.Controls.DataGridLength(70, DataGridLengthUnitType.Auto), null, LookupColumn.LookupColumnType.TextBox, ""),
                                  ComboboxLookupColumn.Create("Standard længdemålingstype", "standardLengthMeasureTypeId", false, "LengthMeasureTypes", "UIDisplay", "L_lengthMeasureTypeId", "L_StandardLengthMeasureDisplay", new System.Windows.Controls.DataGridLength(130)),
                                  LookupColumn.Create("Min. længde (mm)", "lengthMin", false, new System.Windows.Controls.DataGridLength(100, DataGridLengthUnitType.Auto), null, LookupColumn.LookupColumnType.TextBox, ""),
                                  LookupColumn.Create("Max. længde (mm)", "lengthMax", false, new System.Windows.Controls.DataGridLength(100, DataGridLengthUnitType.Auto), null, LookupColumn.LookupColumnType.TextBox, ""),
                                  LookupColumn.Create("Min. alder", "ageMin", false, new System.Windows.Controls.DataGridLength(100, DataGridLengthUnitType.Auto), null, LookupColumn.LookupColumnType.TextBox, ""),
                                  LookupColumn.Create("Max. alder", "ageMax", false, new System.Windows.Controls.DataGridLength(100, DataGridLengthUnitType.Auto), null, LookupColumn.LookupColumnType.TextBox, ""),
                                  LookupColumn.Create("Min. vægt (UR) (g)", "weightMin", false, new System.Windows.Controls.DataGridLength(100, DataGridLengthUnitType.Auto), null, LookupColumn.LookupColumnType.TextBox, ""),
                                  LookupColumn.Create("Max. vægt (UR) (g)", "weightMax", false, new System.Windows.Controls.DataGridLength(100, DataGridLengthUnitType.Auto), null, LookupColumn.LookupColumnType.TextBox, "")
                                  ) { CanEditOffline = true });


            _lstAllLookupTypes.Add(new LookupType("Redskabskvaliteter", typeof(L_GearQuality), null, null,
                                  LookupColumn.Create("Kode", "gearQuality", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Square", typeof(L_StatisticalRectangle), null, null,
                                  LookupColumn.Create("Kode", "statisticalRectangle", true, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto), 70),
                                  LookupColumn.Create("Min bred.", "latitudeDecMin", false, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto),90),
                                  LookupColumn.Create("Max bred.", "latitudeDecMax", false, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto), 90),
                                  LookupColumn.Create("Min læng.", "longitudeDecMin", false, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto), 90),
                                  LookupColumn.Create("Max læng.", "longitudeDecMax", false, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto), 90),
                                  LookupColumn.Create("Mid bred.", "latitudeDecMid", false, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto), 90),
                                  LookupColumn.Create("Mid læng.", "longitudeDecMid", false, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto), 90),

                                  LookupColumn.Create("Min bred. streng", "latitudeTextMin", false, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Max bred. streng", "latitudeTextMax", false, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Min læng. streng", "longitudeTextMin", false, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Max læng. streng", "longitudeTextMax", false, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Mid bred. streng", "latitudeTextMid", false, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Mid læng. streng", "longitudeTextMid", false, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Længdemålingstype", typeof(L_LengthMeasureType), null, null,
                                 LookupColumn.Create("Kode", "lengthMeasureType", false, new System.Windows.Controls.DataGridLength(100)),
                                 LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star)),
                                 LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                 LookupColumn.Create("RDBES", "RDBES", false, new System.Windows.Controls.DataGridLength(120))
                                 ));


            _lstAllLookupTypes.Add(new LookupType("Længdeenheder", typeof(L_LengthMeasureUnit), null, null,
                                  LookupColumn.Create("Kode", "lengthMeasureUnit", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));

           
            _lstAllLookupTypes.Add(new LookupType("Fisk fra flere skibe", typeof(L_SamplingType), null, null,
                                  LookupColumn.Create("Kode", "samplingType", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Indsamlingsmetoder", typeof(L_SamplingMethod), null, null,
                                  LookupColumn.Create("Kode", "samplingMethod", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Redskabstyper", typeof(L_GearType), new string[] { "L_SampleType" }, LoadGearTypeLists,
                                  LookupColumn.Create("Kode", "gearType", true, new System.Windows.Controls.DataGridLength(80)),
                                  LookupColumn.Create("NES kode", "fmCode", false, new DataGridLength(100, DataGridLengthUnitType.SizeToHeader)),
                                /*LookupColumn.Create("Logbogskode", "logbookCode", false, new DataGridLength(100, DataGridLengthUnitType.Auto)),*/
                                  ComboboxLookupColumn.Create("Redskabsgruppe", "sampleType", false, "SampleTypes", "UIDisplay", "sampleType", "L_SampleType.UIDisplay", new System.Windows.Controls.DataGridLength(110, DataGridLengthUnitType.SizeToHeader)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star)),
                                  LookupColumn.Create("HVN/SØS", "showInSeaHvnUI", false, new DataGridLength(74), null, LookupColumn.LookupColumnType.CheckBox),
                                  LookupColumn.Create("VID", "showInVidUI", false, new DataGridLength(34), null, LookupColumn.LookupColumnType.CheckBox)
                                  ) { CanEditOffline = true });

            

            _lstAllLookupTypes.Add(new LookupType("Square & farvand", typeof(ICES_DFU_Relation_FF), null, LoadICES_DFU_Relation_FFLists,
                                  ComboboxLookupColumn.Create("Square", "statisticalRectangle", false, "Rectangles", "UIDisplay", "statisticalRectangle", "RectangleUIDisplay", new System.Windows.Controls.DataGridLength(90)),
                                  ComboboxLookupColumn.Create("Område", "area_20_21", false, "Areas", "UIDisplay", "DFUArea", "Area20_21UIDisplay", new System.Windows.Controls.DataGridLength(200, DataGridLengthUnitType.Star))
                                /*ComboboxLookupColumn.Create("Område", "Area", false, "Areas", "UIDisplay", "DFUArea", "AreaUIDisplay", new System.Windows.Controls.DataGridLength(200, DataGridLengthUnitType.Star))*/
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Oparbejdning af arter", typeof(L_SpeciesRegistration), null, null,
                                  LookupColumn.Create("Kode", "speciesRegistration", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Prøvetagningsniveau", typeof(L_CatchRegistration), null, null,
                                  LookupColumn.Create("Kode", "catchRegistration", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Selektionsudstyr", typeof(L_SelectionDevice), null, null,
                                  LookupColumn.Create("Kode", "selectionDevice", true, new System.Windows.Controls.DataGridLength(150)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star)))
            {
                ChildLookupType = new ChildLookupType("Redskabstyper (relation)", typeof(R_GearTypeSelectionDevice), null, LoadR_GearTypeSelectionDeviceLists, "selectionDevice", "selectionDevice",
                                  LookupColumn.Create("Selektionsudstyr", "selectionDevice", true, new System.Windows.Controls.DataGridLength(130, DataGridLengthUnitType.Auto)),
                                /*ComboboxLookupColumn.Create("Selektionsudstyr", "selectionDevice", true, "SelectionDevices", "UIDisplay", "selectionDevice", "L_SelectionDeviceUIDisplay", new System.Windows.Controls.DataGridLength(130, DataGridLengthUnitType.Auto)),*/
                                  ComboboxLookupColumn.Create("Redskabstype", "gearType", false, "GearTypes", "UIDisplay", "gearType", "L_GearTypeUIDisplay", new System.Windows.Controls.DataGridLength(180, DataGridLengthUnitType.Star))
                                  )
            });

            _lstAllLookupTypes.Add(new LookupType("Selektionsudstyrskilde", typeof(L_SelectionDeviceSource), null, null,
                                  LookupColumn.Create("Kode", "selectionDeviceSource", false, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Fiskerityper", typeof(L_FisheryType), null, LoadFisheryTypeLists,
                                  LookupColumn.Create("Kode", "fisheryType", true, new System.Windows.Controls.DataGridLength(100)),
                                  ComboboxLookupColumn.Create("Landingskategori", "landingCategory", false, "LandingCategories", "UIDisplay", "landingCategory", "L_LandingCategoryUIDisplay", new System.Windows.Controls.DataGridLength(200)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Landingskategorier", typeof(L_LandingCategory), null, null,
                                  LookupColumn.Create("Kode", "landingCategory", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Træktyper", typeof(L_HaulType), null, null,
                                  LookupColumn.Create("Kode", "haulType", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Springlag", typeof(L_ThermoCline), null, null,
                                  LookupColumn.Create("Kode", "thermoCline", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));

            _lstAllLookupTypes.Add(new LookupType("Tidszoner", typeof(L_TimeZone), null, null,
                                  LookupColumn.Create("Zone", "timeZone", true, new System.Windows.Controls.DataGridLength(80)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Sorteringer", typeof(L_SizeSortingEU), null, null,
                                  LookupColumn.Create("Kode", "sizeSortingEU", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Opdelinger", typeof(L_SizeSortingDFU), null, null,
                                 LookupColumn.Create("Kode", "sizeSortingDFU", true, new System.Windows.Controls.DataGridLength(100)),
                                 LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                 LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                 ));


            _lstAllLookupTypes.Add(new LookupType("Køn", typeof(L_SexCode), null, null,
                                 LookupColumn.Create("Kode", "sexCode", true, new System.Windows.Controls.DataGridLength(100)),
                                 LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                 LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                 ));


            _lstAllLookupTypes.Add(new LookupType("Vægtestimeringsmetoder", typeof(L_WeightEstimationMethod), null, null,
                                  LookupColumn.Create("Kode", "weightEstimationMethod", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Rugefaser", typeof(L_BroodingPhase), null, null,
                                  LookupColumn.Create("Kode", "broodingPhase", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Otolith læsbarheder", typeof(L_OtolithReadingRemark), null, null,
                                  LookupColumn.Create("Kode", "otolithReadingRemark", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star)),
                                  LookupColumn.Create("Overfør alder fra AquaDots til Fiskeline", "transAgeFromAquaDotsToFishLine", false, new DataGridLength(100), null, LookupColumn.LookupColumnType.CheckBox)
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Kantstrukturer", typeof(L_EdgeStructure), null, null,
                                  LookupColumn.Create("Kode", "edgeStructure", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Parasitter", typeof(L_Parasite), null, null,
                                  LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Referencer", typeof(L_Reference), null, null,
                                  LookupColumn.Create("Kode", "reference", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));


            _lstAllLookupTypes.Add(new LookupType("Bundtyper", typeof(L_Bottomtype), null, null,
                                  LookupColumn.Create("Kode", "bottomtype", true, new System.Windows.Controls.DataGridLength(70)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));

            _lstAllLookupTypes.Add(new LookupType("Anvendeligheder", typeof(L_Application), null, null,
                                 LookupColumn.Create("Kode", "code", false, new System.Windows.Controls.DataGridLength(100)),
                                 LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                 LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                 ));

            _lstAllLookupTypes.Add(new LookupType("Klækningsmånedslæsbarheder", typeof(L_HatchMonthReadability), null, null,
                                 LookupColumn.Create("Kode", "hatchMonthRemark", false, new System.Windows.Controls.DataGridLength(100)),
                                 LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                 LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                 ));

            /*
            _lstAllLookupTypes.Add(new LookupType("Visuel bestande", typeof(L_Species), new string[] { "L_TreatmentFactorGroup" }, LoadSpeciesLists,
                                  LookupColumn.Create("Kode", "speciesCode", true, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Danske navn", "dkName", true, new System.Windows.Controls.DataGridLength(100, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Engelske navn", "ukName", true, new System.Windows.Controls.DataGridLength(100, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Latinske navn", "latin", true, new System.Windows.Controls.DataGridLength(100, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("NODC kode", "nodc", true, new System.Windows.Controls.DataGridLength(100, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("ICES kode", "icesCode", true, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("NES kode", "speciesNES", true, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("FAO kode", "speciesFAO", true, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto)),
                                  ComboboxLookupColumn.Create("Behandlingsfaktor-gruppe", "treatmentFactorGroupUI", true, "TreatmentFactors", "UIDisplay", "treatmentFactorGroup", "L_TreatmentFactorGroup.UIDisplay", new System.Windows.Controls.DataGridLength(110, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Tsn", "tsn", true, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Aphia id", "aphiaID", true, new System.Windows.Controls.DataGridLength(70, DataGridLengthUnitType.Auto), null, LookupColumn.LookupColumnType.TextBox, "")
            {
                ChildLookupType = new ChildLookupType("Modenhedsindeks", typeof(Maturity), null, LoadMaturityLists, "maturityIndexMethod", "maturityIndexMethod",
                                    ComboboxLookupColumn.Create("Modenhedsmetode", "maturityIndexMethod", true, "MaturityIndexMethods", "UIDisplay", "maturityIndexMethod", "maturityIndexMethod", new System.Windows.Controls.DataGridLength(120, DataGridLengthUnitType.Auto), null, true),
                                    LookupColumn.Create("Modenhedsindeks", "maturityIndex", false, new System.Windows.Controls.DataGridLength(120, DataGridLengthUnitType.Auto)),
                                    LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                    )
            }*/

            _lstAllLookupTypes.Add(new LookupType("Visuel bestande", typeof(L_VisualStock), null, LoadVisualStockLists,
                                  LookupColumn.Create("Kode", "visualStock", false, new System.Windows.Controls.DataGridLength(90)),
                                  ComboboxLookupColumn.Create("Art", "speciesCode", false, "Species", "UIDisplay", "speciesCode", "speciesCode", new System.Windows.Controls.DataGridLength(130)),
                                  LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                  LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                  ));

            _lstAllLookupTypes.Add(new LookupType("Genetisk bestande", typeof(L_GeneticStock), null, LoadGeneticStockLists,
                                 LookupColumn.Create("Kode", "geneticStock", false, new System.Windows.Controls.DataGridLength(90)),
                                 ComboboxLookupColumn.Create("Art", "speciesCode", false, "Species", "UIDisplay", "speciesCode", "speciesCode", new System.Windows.Controls.DataGridLength(130)),
                                 LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                 LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                 ));

            _lstAllLookupTypes.Add(new LookupType("Bestand/art/område-relation", typeof(R_StockSpeciesArea), null, LoadStockLists,
                                 ComboboxLookupColumn.Create("Bestand", "L_stockId", false, "Stocks", "UIDisplay", "L_stockId", "L_StockCodeDisplay", new System.Windows.Controls.DataGridLength(100, DataGridLengthUnitType.Star)),
                                 ComboboxLookupColumn.Create("Art", "speciesCode", false, "Species", "UIDisplay", "speciesCode", "speciesCode", new System.Windows.Controls.DataGridLength(100)),
                                 ComboboxLookupColumn.Create("Område", "DFUArea", false, "Areas", "UIDisplay", "DFUArea", "DFUArea", new System.Windows.Controls.DataGridLength(100)),
                                 ComboboxLookupColumn.Create("Rektangel", "statisticalRectangle", false, "StatisticalRectangles", "UIDisplay", "statisticalRectangle", "statisticalRectangle", new System.Windows.Controls.DataGridLength(100)),
                                 ComboboxLookupColumn.Create("Kvartal", "quarter", false, "Quarters", null, null, "quarter", new System.Windows.Controls.DataGridLength(60))
                                 ){ Message = "OBS! Husk at overføre tidligere data til varehuset hvis relationer ændres eller nye tilføjes før ændringerne træder i kraft på det tidligere data i varehuset." });

            _lstAllLookupTypes.Add(new LookupType("Aquadots formål", typeof(L_SDPurpose), null, null,
                                LookupColumn.Create("Formål", "purpose", false, new System.Windows.Controls.DataGridLength(150)),
                                LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                ));

            _lstAllLookupTypes.Add(new LookupType("Aquadots hændelsestyper", typeof(L_SDEventType), null, LoadSDEventTypeLists,
                                LookupColumn.Create("Hændelsestype", "eventType", false, new System.Windows.Controls.DataGridLength(150)),
                                LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star)),
                                 LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
                                ComboboxLookupColumn.Create("Opdater aldre", "ageUpdatingMethod", false, "AgeUpdatingMethods", "Value", "Key", "AgeUpdatingMethodUIDisplay", new System.Windows.Controls.DataGridLength(150))
                                ));

            _lstAllLookupTypes.Add(new LookupType("Aquadots prøvetagningstyper", typeof(L_SDSampleType), null, null,
                                LookupColumn.Create("Prøvetagningstype", "sampleType", false, new System.Windows.Controls.DataGridLength(160)),
                                LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                ));

            _lstAllLookupTypes.Add(new LookupType("Aquadots lystyper", typeof(L_SDLightType), null, null,
                               LookupColumn.Create("Lystype", "lightType", false, new System.Windows.Controls.DataGridLength(150)),
                               LookupColumn.Create("Beskrivelse (DK)", "dkDescription", false, new DataGridLength(150, DataGridLengthUnitType.Star)),
                               LookupColumn.Create("Beskrivelse (UK)", "ukDescription", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                               ));

            _lstAllLookupTypes.Add(new LookupType("Aquadots otolitbeskrivelser", typeof(L_SDOtolithDescription), null, null,
                               LookupColumn.Create("Otolitbeskrivelse", "otolithDescription", false, new System.Windows.Controls.DataGridLength(160)),
                               LookupColumn.Create("Beskrivelse (DK)", "dkDescription", false, new DataGridLength(150, DataGridLengthUnitType.Star)),
                               LookupColumn.Create("Beskrivelse (UK)", "ukDescription", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                               ));

            _lstAllLookupTypes.Add(new LookupType("Aquadots forberedelsesmetoder", typeof(L_SDPreparationMethod), null, null,
                              LookupColumn.Create("Forberedelsesmetode", "preparationMethod", false, new System.Windows.Controls.DataGridLength(160)),
                              LookupColumn.Create("Beskrivelse (DK)", "dkDescription", false, new DataGridLength(150, DataGridLengthUnitType.Star)),
                              LookupColumn.Create("Beskrivelse (UK)", "ukDescription", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                              ));

            _lstAllLookupTypes.Add(new LookupType("Bestande (Stock)", typeof(L_Stock), null, null,
                               LookupColumn.Create("Kode", "stockCode", false, new System.Windows.Controls.DataGridLength(160)),
                               LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                               ));

            _lstAllLookupTypes.Add(new LookupType("Aquadots aflæser-erfaring", typeof(L_SDReaderExperience), null, null,
                                LookupColumn.Create("Erfaring", "readerExperience", false, new System.Windows.Controls.DataGridLength(150)),
                                LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                                ));

            _lstAllLookupTypes.Add(new LookupType("Aquadots aflæsere", typeof(DFUPerson), null, null,
                                  LookupColumn.Create("Initialer", "initials", true, new System.Windows.Controls.DataGridLength(100)),
                                  LookupColumn.Create("Fulde navn", "name", true, new DataGridLength(100, System.Windows.Controls.DataGridLengthUnitType.Star)))
            {
                ChildLookupType = new ChildLookupType("Aquadots aflæserekspertise", typeof(R_SDReader), null, LoadR_SDReaderLists, "dfuPersonId", "dfuPersonId",
                                    ComboboxLookupColumn.Create("Aflæser", "dfuPersonId", true, "DFUPersons", "UIDisplay", "dfuPersonId", "DFUPersonDisplay", new System.Windows.Controls.DataGridLength(80), null, true),
                                    LookupColumn.Create("1. års aflæsning (generelt)", "firstYearAgeReadingGeneral", false, new DataGridLength(190)),
                                    ComboboxLookupColumn.Create("Art", "SpeciesCodeForLookupList", false, "Species", "UIDisplay", "speciesCode", "speciesCode", new System.Windows.Controls.DataGridLength(60)),
                                    ComboboxLookupColumn.Create("Bestand", "stockId", false, "StocksForSelectedSpecies", "UIDisplay", "L_stockId", "StockDisplay", new System.Windows.Controls.DataGridLength(110)),
                                    LookupColumn.Create("1. års aflæsning (art/bestand)", "firstYearAgeReadingCurrent", false, new DataGridLength(210)),
                                    ComboboxLookupColumn.Create("Erfaring", "sdReaderExperienceId", false, "ReaderExperiences", "UIDisplay", "L_SDReaderExperienceId", "ReaderExperienceDisplay", new System.Windows.Controls.DataGridLength(90)),
                                    ComboboxLookupColumn.Create("Forb. metode", "sdPreparationMethodId", false, "PreparationMethods", "UIDisplay", "L_sdPreparationMethodId", "PreperationMethodDisplay", new System.Windows.Controls.DataGridLength(120)),
                                    LookupColumn.Create("Kommentar", "comment", false, new DataGridLength(150))
                                    )
            });

            _lstAllLookupTypes.Add(new LookupType("Maveindhold", typeof(L_StomachStatus), null, null,
                                 LookupColumn.Create("Kode", "stomachStatus", false, new System.Windows.Controls.DataGridLength(150)),
                                 LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star)),
                                 LookupColumn.Create("Nummer", "num", false, new System.Windows.Controls.DataGridLength(80)),
                                 LookupColumn.Create("Enkeltfisk", "showInAnimal", false, new DataGridLength(100), null, LookupColumn.LookupColumnType.CheckBox),
                                 LookupColumn.Create("Mave", "showInStomach", false, new DataGridLength(100), null, LookupColumn.LookupColumnType.CheckBox)
                                 ));

            #endregion

            //Sort the lookup types
            _lstAllLookupTypes = _lstAllLookupTypes.OrderBy(x => x.Name).ToList();

            SetCollectionView();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                IsLoading = false;
            }));
        }


        #region Lookup list initialization methods

        private void LoadR_SDReaderLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            var lSpecies = lm.GetLookups(typeof(L_Species));
            var lStock = lm.GetLookups(typeof(L_Stock));
            var lReaderExperience = lm.GetLookups(typeof(L_SDReaderExperience));
            var lPrepartionMethod = lm.GetLookups(typeof(L_SDPreparationMethod));
            var lDFUPersons = lm.GetLookups(typeof(DFUPerson));
            var lStockSpeciesArea = lm.GetLookups(typeof(R_StockSpeciesArea));

            R_SDReader.Species = lSpecies.OfType<L_Species>().OrderBy(x => x.speciesCode).ToList();
            R_SDReader.Stocks = lStock.OfType<L_Stock>().OrderBy(x => x.stockCode).ToList();
            R_SDReader.ReaderExperiences = lReaderExperience.OfType<L_SDReaderExperience>().OrderBy(x => x.readerExperience).ToList();
            R_SDReader.PreparationMethods = lPrepartionMethod.OfType<L_SDPreparationMethod>().OrderBy(x => x.preparationMethod).ToList();
            R_SDReader.DFUPersons = lDFUPersons.OfType<DFUPerson>().OrderBy(x => x.UIDisplay).ToList();
            R_SDReader.StockSpeciesArea = lStockSpeciesArea.OfType<R_StockSpeciesArea>().ToList();
        }

        private void LoadSDEventTypeLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            L_SDEventType.AgeUpdatingMethods = new List<KeyValuePair<string, string>>
            {
                 new KeyValuePair<string, string>(SDEventTypeAgeUpdatingMethod.NeverUpdateAges.ToString(), "Never update ages"),
                 new KeyValuePair<string, string>(SDEventTypeAgeUpdatingMethod.UpdateAges.ToString(), "Always update ages"),
            };
        }


        private void LoadDFUPersonLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            var lDepartments = lm.GetLookups(typeof(L_DFUDepartment));
            DFUPerson.DFUDepartments = lDepartments.OfType<L_DFUDepartment>().OrderBy(x => x.CodeDescription).ToList();
        }

        private void LoadPlatformLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            var lPlatforms = lm.GetLookups(typeof(L_PlatformType));
            var lNationalities = lm.GetLookups(typeof(L_Nationality));
            var lPersons = lm.GetLookups(typeof(Person));

            L_Platform.PlatformTypes = lPlatforms.OfType<L_PlatformType>().OrderBy(x => x.CodeDescription).ToList();
            L_Platform.Nationalities = lNationalities.OfType<L_Nationality>().OrderBy(x => x.CodeDescription).ToList();
            L_Platform.Persons = lPersons.OfType<Person>().OrderBy(x => x.name).ToList();
        }


        private void LoadPlatformVersionLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            var lPlatforms = lm.GetLookups(typeof(L_Platform));
            var lNavigationSystem = lm.GetLookups(typeof(L_NavigationSystem));
   
            L_PlatformVersion.Platforms = lPlatforms.OfType<L_Platform>().OrderBy(x => x.platform).ToList();
            L_PlatformVersion.NavigationSystems = lNavigationSystem.OfType<L_NavigationSystem>().OrderBy(x => x.CodeDescription).ToList();
        }


        private void LoadHarbourLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            var lNationalities = lm.GetLookups(typeof(L_Nationality));
            L_Harbour.Nationalities = lNationalities.OfType<L_Nationality>().OrderBy(x => x.CodeDescription).ToList();
        }

        private void LoadAreaLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            var lAreas = lm.GetLookups(typeof(L_DFUArea));
            L_DFUArea.Areas = lAreas.OfType<L_DFUArea>().OrderBy(x => x.UIDisplay).ToList();
        }

        private void LoadSpeciesLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            var lTreat = lm.GetLookups(typeof(L_TreatmentFactorGroup));
            var lLMTypes = lm.GetLookups(typeof(L_LengthMeasureType));

            L_Species.TreatmentFactors = lTreat.OfType<L_TreatmentFactorGroup>().OrderBy(x => x.UIDisplay).ToList();

            L_Species.LengthMeasureTypes = lLMTypes.OfType<L_LengthMeasureType>().OrderBy(x => x.num).ThenBy(x => x.UIDisplay).ToList();
        }

        private void LoadVisualStockLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            var lSpecies = lm.GetLookups(typeof(L_Species));
            L_VisualStock.Species = lSpecies.OfType<L_Species>().OrderBy(x => x.UIDisplay).ToList();
        }

        private void LoadGeneticStockLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            var lSpecies = lm.GetLookups(typeof(L_Species));
            L_GeneticStock.Species = lSpecies.OfType<L_Species>().OrderBy(x => x.UIDisplay).ToList();
        }

        private void LoadStockLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            var lSpecies = lm.GetLookups(typeof(L_Species));
            var lStock = lm.GetLookups(typeof(L_Stock));
            var lRectangles = lm.GetLookups(typeof(L_StatisticalRectangle));
            var lAreas = lm.GetLookups(typeof(L_DFUArea));

            R_StockSpeciesArea.Stocks = lStock.OfType<L_Stock>().OrderBy(x => x.UIDisplay).ToList();
            R_StockSpeciesArea.Species = lSpecies.OfType<L_Species>().OrderBy(x => x.UIDisplay).ToList();
            R_StockSpeciesArea.StatisticalRectangles = lRectangles.OfType<L_StatisticalRectangle>().OrderBy(x => x.UIDisplay).ToList();
            R_StockSpeciesArea.Areas = lAreas.OfType<L_DFUArea>().OrderBy(x => x.UIDisplay).ToList();
            //Quarters is a static list already populated and therefore not needed assigned.
        }


        private void LoadGearTypeLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            var l = lm.GetLookups(typeof(L_SampleType));
            L_GearType.SampleTypes = l.OfType<L_SampleType>().OrderBy(x => x.UIDisplay).ToList();
        }

        private void LoadGearLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            var lGear = lm.GetLookups(typeof(L_GearType));
            var lPlatforms = lm.GetLookups(typeof(L_Platform));

            L_Gear.GearTypes = lGear.OfType<L_GearType>().OrderBy(x => x.UIDisplay).ToList();
            L_Gear.Platforms = lPlatforms.OfType<L_Platform>().OrderBy(x => x.UIString).ToList();
        }


        private void LoadR_GearInfoLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            var lGear = lm.GetLookups(typeof(L_Gear));
            var lInfoTypes = lm.GetLookups(typeof(L_GearInfoType));

            R_GearInfo.Gears = lGear.OfType<L_Gear>().OrderBy(x => x.UIDisplay).ToList();
            R_GearInfo.GearInfoTypes = lInfoTypes.OfType<L_GearInfoType>().OrderBy(x => x.UIDisplay).ToList();
        }


        private void LoadTreatmentFactorLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            var ltg = lm.GetLookups(typeof(L_TreatmentFactorGroup));
            var lt = lm.GetLookups(typeof(L_Treatment));

            TreatmentFactor.TreatmentFactorGroups = ltg.OfType<L_TreatmentFactorGroup>().OrderBy(x => x.UIDisplay).ToList();
            TreatmentFactor.Treatments = lt.OfType<L_Treatment>().OrderBy(x => x.UIDisplay).ToList();
        }


        private void LoadMaturityLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            var ltg = lm.GetLookups(typeof(L_MaturityIndexMethod));

            Maturity.MaturityIndexMethods = ltg.OfType<L_MaturityIndexMethod>().OrderBy(x => x.UIDisplay).ToList();
        }


        private void LoadICES_DFU_Relation_FFLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            var areas = lm.GetLookups(typeof(L_DFUArea));
            var rects = lm.GetLookups(typeof(L_StatisticalRectangle));

            ICES_DFU_Relation_FF.Areas = areas.OfType<L_DFUArea>().OrderBy(x => x.UIDisplay).ToList();
            ICES_DFU_Relation_FF.Rectangles = rects.OfType<L_StatisticalRectangle>().OrderBy(x => x.UIDisplay).ToList();
        }


        private void LoadR_GearTypeSelectionDeviceLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            var ltg = lm.GetLookups(typeof(L_SelectionDevice));
            var lt = lm.GetLookups(typeof(L_GearType));

            R_GearTypeSelectionDevice.SelectionDevices = ltg.OfType<L_SelectionDevice>().OrderBy(x => x.UIDisplay).ToList();
            R_GearTypeSelectionDevice.GearTypes = lt.OfType<L_GearType>().OrderBy(x => x.UIDisplay).ToList();
        }


        private void LoadFisheryTypeLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            var ltg = lm.GetLookups(typeof(L_LandingCategory));

            L_FisheryType.LandingCategories = ltg.OfType<L_LandingCategory>().OrderBy(x => x.UIDisplay).ToList();
        }


        #endregion


        private void SetCollectionView()
        {
            ICollectionView icol = CollectionViewSource.GetDefaultView(_lstAllLookupTypes);

            //Apply filtering method
            icol.Filter = CollectionViewFilter;

            //Remove any existing sort descriptions
            icol.SortDescriptions.Clear();

            Dispatcher.BeginInvoke(new Action(delegate()
            {
                LookupTypesCollection = icol;

                if (_defaultSelectedType != null)
                {
                    var lt = _lstAllLookupTypes.Where(x => x.Type == _defaultSelectedType).FirstOrDefault();

                    if (lt != null)
                        ChangeLookupType(lt);

                    ScrollTo(_defaultSelectedType.Name);

                    _defaultSelectedType = null;
                }
            }));
        }


        /// <summary>
        /// Method used for filtering trips.
        /// </summary>
        private bool CollectionViewFilter(object itm)
        {
            if (SearchString == "" || SearchString == null)
                return true;

            LookupType t = itm as LookupType;

            return t.Name.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase);
        }



        public bool IsSelectedLookupDirty()
        {
            if (SelectedLookupViewModel == null || !SelectedLookupType.HasEditingRights)
                return false;

            var method = SelectedLookupViewModel.GetType().GetMethods().Where(x => x.Name.Equals("IsDirtyDeepCheck")).FirstOrDefault();
            bool blnIsDirty = (bool)method.Invoke(SelectedLookupViewModel, null);

            return blnIsDirty;
        }



        #region Lookup Type Selected Command


        public DelegateCommand<LookupType> LookupTypeSelectedCommand
        {
            get
            {
                if (_cmdLookupTypeSelected == null)
                    _cmdLookupTypeSelected = new DelegateCommand<LookupType>(type => ChangeLookupType(type));

                return _cmdLookupTypeSelected;
            }
        }


        private bool ShowDiscardLookupList()
        {
            //If the current lookup view model is dirty, warn the user that changes will be lost if continuing.
            if (IsSelectedLookupDirty())
            {
                var res = AppRegionManager.ShowMessageBox("Ændringer til nøgleværdi-listen er ikke gemt, er du sikker på du vil fortsætte uden at gemme?", System.Windows.MessageBoxButton.YesNo);

                if (res == System.Windows.MessageBoxResult.No)
                {
                    RaisePropertyChanged(() => SelectedLookupViewModel);
                    RaisePropertyChanged(() => SelectedLookupType);
                    return false;
                }
            }

            return true;
        }

        private void ChangeLookupType(LookupType lookupType)
        {
            if (!ShowDiscardLookupList())
                return;

            SelectedLookupType = lookupType;
            AViewModel avm = new LookupViewModel(this, lookupType);

            if (SelectedLookupViewModel != null)
                SelectedLookupViewModel.Dispose();

            SelectedLookupViewModel = avm;
        }

        #endregion



        /// <summary>
        /// Display warning message when lookup window closes.
        /// </summary>
        public override void FireClosing(object sender, CancelEventArgs e)
        {
            if (!ShowDiscardLookupList())
                e.Cancel = true;
               
            base.FireClosing(sender, e);
        }


        public override void WindowCloseButtonClicked()
        {
            base.WindowCloseButtonClicked();

            Dispose();
        }


        /// <summary>
        /// Cleanup on disposal.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            if (SelectedLookupViewModel != null)
                SelectedLookupViewModel.Dispose();
        }


        //Below are som Lookups that have been added early in development, but decided removed later. They are still here incase
        //they need to be included later.
        /*
            _lstAllLookupTypes.Add(new LookupType("Navigationssystemmer", typeof(L_NavigationSystem), null, null, 
                                    LookupColumn.Create("Kode", "navigationSystem", true, new System.Windows.Controls.DataGridLength(100)),
                                    LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(100, System.Windows.Controls.DataGridLengthUnitType.Star))));
            */
        /*
        _lstAllLookupTypes.Add(new LookupType("Sektioner", typeof(L_DFUDepartment), null, null, 
                                LookupColumn.Create("Kode", "dfuDepartment", true, new DataGridLength(100)),
                                LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(100, System.Windows.Controls.DataGridLengthUnitType.Star))));
        */
        /*
        _lstAllLookupTypes.Add(new LookupType("Skibstyper", typeof(L_PlatformType), null, null, 
                                LookupColumn.Create("Kode", "platformType", true, new DataGridLength(100)),
                                LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(100, System.Windows.Controls.DataGridLengthUnitType.Star))));
        */

        /*
        _lstAllLookupTypes.Add(new LookupType("Redskabsgrupper", typeof(L_SampleType), null, null,
                 LookupColumn.Create("Kode", "sampleType", true, new System.Windows.Controls.DataGridLength(100)),
                 LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Star))
                 ));
        */

        /*
        _lstAllLookupTypes.Add(new LookupType("Skibsversioner", typeof(L_PlatformVersion),*/
        /*new string[] { "L_Platform", "L_NavigationSystem" }*/
        /*null, LoadPlatformVersionLists,
            ComboboxLookupColumn.Create("Skib", "platform", false, "Platforms", "UIString", "platform", "L_PlatformUIDisplay", new System.Windows.Controls.DataGridLength(130, DataGridLengthUnitType.Auto)),
            LookupColumn.Create("Version", "version", false, new System.Windows.Controls.DataGridLength(50, DataGridLengthUnitType.Auto)),
            LookupColumn.Create("Revisionsår", "revisionYear", false, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto)),
            ComboboxLookupColumn.Create("Navigationssystem", "navigationSystem", false, "NavigationSystems", "CodeDescription", "navigationSystem", "L_NavigationSystemUIDisplay", new System.Windows.Controls.DataGridLength(90, DataGridLengthUnitType.Auto)),
            LookupColumn.Create("Ton", "registerTons", false, new System.Windows.Controls.DataGridLength(60, DataGridLengthUnitType.Auto)),
            LookupColumn.Create("Længde (m)", "length", false, new System.Windows.Controls.DataGridLength(70, DataGridLengthUnitType.Auto)),
            LookupColumn.Create("Kræfter (HP)", "power", false, new System.Windows.Controls.DataGridLength(70, DataGridLengthUnitType.Auto)),
            LookupColumn.Create("Besætning (antal)", "crew", false, new System.Windows.Controls.DataGridLength(80, DataGridLengthUnitType.Auto)),
            LookupColumn.Create("Radiosignal", "radiosignal", false, new System.Windows.Controls.DataGridLength(90, DataGridLengthUnitType.Auto)),
            LookupColumn.Create("Beskrivelse", "description", false, null, 300)
            ));
        */

        /*
        _lstAllLookupTypes.Add(new LookupType("Redskabsinfotyper", typeof(L_GearInfoType), null, null,
                                    LookupColumn.Create("Kode", "gearInfoType", true, new System.Windows.Controls.DataGridLength(100, DataGridLengthUnitType.Auto)),
                                    LookupColumn.Create("DFU base ref.", "dfuBase_dfuref", false, new System.Windows.Controls.DataGridLength(120, DataGridLengthUnitType.Auto)),
                                    LookupColumn.Create("Enhed", "unit", false, new System.Windows.Controls.DataGridLength(100, DataGridLengthUnitType.Auto)),
                                    LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Auto))
                            ));
        */

        //Don't load includes here for speed optimizations.
        /* _lstAllLookupTypes.Add(new LookupType("Redskaber", typeof(L_Gear),*/
        /*new string[] { "L_Platform", "L_GearType" }*/
        /*null, LoadGearLists,
        LookupColumn.Create("Kode", "gear", false, new System.Windows.Controls.DataGridLength(120)),
        LookupColumn.Create("Tekst", "gearText", false, new DataGridLength(120)),
        ComboboxLookupColumn.Create("Skib", "platform", false, "Platforms", "UIString", "platform", "L_PlatformUIDisplay", new System.Windows.Controls.DataGridLength(110, DataGridLengthUnitType.Auto)),
        LookupColumn.Create("Version", "version", false, new DataGridLength(100, DataGridLengthUnitType.Auto)),
        ComboboxLookupColumn.Create("Redskabstype", "gearType", false, "GearTypes", "UIDisplay", "gearType", "L_GearTypeUIDisplay", new System.Windows.Controls.DataGridLength(110, DataGridLengthUnitType.Auto)),
        LookupColumn.Create("Beskrivelse", "description", false, new DataGridLength(150, DataGridLengthUnitType.Auto))
        ));
        */

        //Don't load includes here for speed optimizations.
        /*_lstAllLookupTypes.Add(new LookupType("Redskabsværdier", typeof(R_GearInfo), null, LoadR_GearInfoLists,
                                    ComboboxLookupColumn.Create("Redskab", "gearId", false, "Gears", "UIDisplay", "gearId", "L_GearUIDisplay", new System.Windows.Controls.DataGridLength(140)),
                                    ComboboxLookupColumn.Create("Redskabsinfotype", "gearInfoType", false, "GearInfoTypes", "UIDisplay", "gearInfoType", "L_GearInfoTypeUIDisplay", new System.Windows.Controls.DataGridLength(200)),
                                    LookupColumn.Create("Redskabsværdi", "gearValue", false, new System.Windows.Controls.DataGridLength(120, DataGridLengthUnitType.Star))
                            ));
        */
    }

}
