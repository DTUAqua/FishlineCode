using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Babelfisk.BusinessLogic.Settings
{
    [DataContract(IsReference = true)]
    public class DataGridColumnSettingsContainer : SettingsBaseObject<DataGridColumnSettingsContainer>
    {
        /// <summary>
        /// Dictionary containing:
        /// Key: Group name of columns. For example "SpeciesList", "LavRep", "SF"
        /// </summary>
        [DataMember]
        Dictionary<string, Dictionary<string, DataGridColumnSettings>> _dic;


        public DataGridColumnSettingsContainer()
        {
            _dic = new Dictionary<string, Dictionary<string, DataGridColumnSettings>>();
        }


        public void Initialize()
        {
            if (_dic == null)
                _dic = new Dictionary<string, Dictionary<string, DataGridColumnSettings>>();

            InitializeDefaults();

            //Make sure parent is set.
            foreach (var itm in _dic.Values)
            {
                foreach (var c in itm)
                {
                    c.Value.Parent = this;
                }
            }

        }


        private void InitializeDefaults()
        {
            //Add SpeciesList for HVN
            if (!_dic.ContainsKey("HVN:SpeciesList"))
                _dic.Add("HVN:SpeciesList", new Dictionary<string, DataGridColumnSettings>());

            //Add SpeciesList for SØS
            if (!_dic.ContainsKey("SØS:SpeciesList"))
                _dic.Add("SØS:SpeciesList", new Dictionary<string, DataGridColumnSettings>());

            //Add SpeciesList for VID
            if (!_dic.ContainsKey("VID:SpeciesList"))
                _dic.Add("VID:SpeciesList", new Dictionary<string, DataGridColumnSettings>());

            //Add SpeciesList for VID
            if (!_dic.ContainsKey("REKTBD:SpeciesList"))
                _dic.Add("REKTBD:SpeciesList", new Dictionary<string, DataGridColumnSettings>());

            //Add SpeciesList for VID
            if (!_dic.ContainsKey("REKOMR:SpeciesList"))
                _dic.Add("REKOMR:SpeciesList", new Dictionary<string, DataGridColumnSettings>());

            //Add SpeciesList for VID
            if (!_dic.ContainsKey("REKHVN:SpeciesList"))
                _dic.Add("REKHVN:SpeciesList", new Dictionary<string, DataGridColumnSettings>());

            //Add SpeciesList for LAVRep
            if (!_dic.ContainsKey("LAVRep:SingleFish"))
                _dic.Add("LAVRep:SingleFish", new Dictionary<string, DataGridColumnSettings>());

            //Add SpeciesList for SFRep
            if (!_dic.ContainsKey("SFRep:SingleFish"))
                _dic.Add("SFRep:SingleFish", new Dictionary<string, DataGridColumnSettings>());

            //Add SpeciesList for SFNotRep
            if (!_dic.ContainsKey("SFNotRep:SingleFish"))
                _dic.Add("SFNotRep:SingleFish", new Dictionary<string, DataGridColumnSettings>());
          
            if (!_dic.ContainsKey("SFRep:REKSingleFish"))
                _dic.Add("SFRep:REKSingleFish", new Dictionary<string, DataGridColumnSettings>());

            if (!_dic.ContainsKey("SFNotRep:REKSingleFish"))
                _dic.Add("SFNotRep:REKSingleFish", new Dictionary<string, DataGridColumnSettings>());

            if (!_dic.ContainsKey("Any:SDSelectAnimals"))
                _dic.Add("Any:SDSelectAnimals", new Dictionary<string, DataGridColumnSettings>());

            if (!_dic.ContainsKey("Any:SDSamples"))
                _dic.Add("Any:SDSamples", new Dictionary<string, DataGridColumnSettings>());

            InitializeSpeciesListHVNDefaults();
            InitializeSpeciesListVIDDefaults();
            InitializeSpeciesListSØSDefaults();
            InitializeSpeciesListREKDDefaults(_dic["REKTBD:SpeciesList"]);
            InitializeSpeciesListREKDDefaults(_dic["REKOMR:SpeciesList"]);
            InitializeSpeciesListREKDDefaults(_dic["REKHVN:SpeciesList"]);
            InitializeLAVRepDefaults();
            InitializeSFRepDefaults();
            InitializeSFNotRepDefaults();
            InitializeREKSFNotRepDefaults();
            InitializeREKSFRepDefaults();
            InitializeSDSelectAnimalsDefaults();
            InitializeSDSamplesDefaults();
        }

        private void InitializeSDSamplesDefaults()
        {
            var speciesList = _dic["Any:SDSamples"];

            int intSorting = 0;


            /*
            string name = "AnimalId";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("# ({0})", Settings.Translate("SDSelectAnimalsView", "AnimalId"));
            speciesList[name].Sorting = intSorting++;
            */

            string name = "Images";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("Common", "Images"));
            speciesList[name].Sorting = intSorting++;

            name = "Cruise";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSelectAnimalsView", "Cruise"));
            speciesList[name].Sorting = intSorting++;

            name = "Trip";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSelectAnimalsView", "Trip"));
            speciesList[name].Sorting = intSorting++;

            name = "Station";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSelectAnimalsView", "Station"));
            speciesList[name].Sorting = intSorting++;

            name = "CatchDate";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));
            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "CatchDate"));
            speciesList[name].Sorting = intSorting++;

            name = "DFUArea";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "DFUAre"));
            speciesList[name].Sorting = intSorting++;

            name = "StatisticalRectangle";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "StatisticalRectangle"));
            speciesList[name].Sorting = intSorting++;

            name = "Species";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("AddEditSDEventView", "Species"));
            speciesList[name].Sorting = intSorting++;

            name = "StockId";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "Stock"));
            speciesList[name].Sorting = intSorting++;

            name = "Length";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "ColumnLength"));
            speciesList[name].Sorting = intSorting++;

            name = "Weight";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "ColumnWeight"));
            speciesList[name].Sorting = intSorting++;

            name = "SexCode";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "SexCode"));
            speciesList[name].Sorting = intSorting++;

            name = "Maturity";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "MaturityID"));
            speciesList[name].Sorting = intSorting++;

            name = "OtolithReadingRemark";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSelectAnimalsView", "OtolithReadingRemark"));
            speciesList[name].Sorting = intSorting++;

            name = "PreparationMethod";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "PreparationMethod"));
            speciesList[name].Sorting = intSorting++;

            name = "LightType";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "LightType"));
            speciesList[name].Sorting = intSorting++;

            name = "OtolithDescription";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "OtolithDescription"));
            speciesList[name].Sorting = intSorting++;

            name = "EdgeStructure";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSelectAnimalsView", "EdgeStructure"));
            speciesList[name].Sorting = intSorting++;

           
            name = "Latitude";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("Common", "Latitude"));
            speciesList[name].Sorting = intSorting++;

            name = "Longitude";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("Common", "Longitude"));
            speciesList[name].Sorting = intSorting++;            

            name = "CreatedBy";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "CreatedBy"));
            speciesList[name].Sorting = intSorting++;

            name = "Created";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("Common", "Created"));
            speciesList[name].Sorting = intSorting++;

            name = "Modified";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("Common", "Modified"));
            speciesList[name].Sorting = intSorting++;

            name = "Comments";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("Common", "Comments"));
            speciesList[name].Sorting = intSorting++;

            CleanUpEmptyEntries(speciesList);
        }

        private void InitializeSDSelectAnimalsDefaults()
        {
            var speciesList = _dic["Any:SDSelectAnimals"];

            int intSorting = 0;

            /*string name = "AnimalId";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSelectAnimalsView", "AnimalId")); ;
            speciesList[name].Sorting = intSorting++;
            */

            string name = "HasImages";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSelectAnimalsView", "HasImages"));
            speciesList[name].Sorting = intSorting++;

            name = "Species";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("AddEditSDEventView", "Species"));
            speciesList[name].Sorting = intSorting++;

            name = "Cruise";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSelectAnimalsView", "Cruise"));
            speciesList[name].Sorting = intSorting++;

            name = "Trip";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSelectAnimalsView", "Trip"));
            speciesList[name].Sorting = intSorting++;

            name = "TripType";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSelectAnimalsView", "TripType"));
            speciesList[name].Sorting = intSorting++;

            name = "Station";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSelectAnimalsView", "Station"));
            speciesList[name].Sorting = intSorting++;

            name = "CruiseYear";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSelectAnimalsView", "CruiseYear"));
            speciesList[name].Sorting = intSorting++;

            name = "Quarter";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSelectAnimalsView", "Quarter"));
            speciesList[name].Sorting = intSorting++;

            name = "GearStartDate";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "CatchDate"));
            speciesList[name].Sorting = intSorting++;

            name = "AreaCode";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSelectAnimalsView", "AreaCode"));
            speciesList[name].Sorting = intSorting++;

            name = "StatisticalRectangle";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "StatisticalRectangle"));
            speciesList[name].Sorting = intSorting++;

            name = "Stock";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "Stock"));
            speciesList[name].Sorting = intSorting++;

            name = "Length";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSelectAnimalsView", "ColumnLength"));
            speciesList[name].Sorting = intSorting++;

            name = "Weight";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSelectAnimalsView", "ColumnWeight"));
            speciesList[name].Sorting = intSorting++;
            
            name = "SexCode";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "SexCode"));
            speciesList[name].Sorting = intSorting++;

            name = "Maturity";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "MaturityID"));
            speciesList[name].Sorting = intSorting++;

            name = "Age";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("Common", "Age"));
            speciesList[name].Sorting = intSorting++;

            name = "OtolithReadingRemark";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSelectAnimalsView", "OtolithReadingRemark"));
            speciesList[name].Sorting = intSorting++;

            name = "PreparationMethod";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "PreparationMethod"));
            speciesList[name].Sorting = intSorting++;

            name = "LightType";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "LightType"));
            speciesList[name].Sorting = intSorting++;

            name = "OtolithDescription";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSampleView", "OtolithDescription"));
            speciesList[name].Sorting = intSorting++;
           
            name = "EdgeStructure";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("SDSelectAnimalsView", "EdgeStructure"));
            speciesList[name].Sorting = intSorting++;

            name = "Latitude";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("Common", "Latitude"));
            speciesList[name].Sorting = intSorting++;

            name = "Longitude";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("Common", "Longitude"));
            speciesList[name].Sorting = intSorting++;
 
            name = "Comment";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = string.Format("{0}", Settings.Translate("Common", "Comments"));
            speciesList[name].Sorting = intSorting++;


            CleanUpEmptyEntries(speciesList);
        }

        private void InitializeSpeciesListREKDDefaults(Dictionary<string, DataGridColumnSettings> speciesList)
        {
            int intSorting = 0;

            string name = "Index";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "# (Indeks)";
            speciesList[name].Sorting = intSorting++;

            name = "Species";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Art";
            speciesList[name].Sorting = intSorting++;

            name = "LandingCategory";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Landingskategori";
            speciesList[name].Sorting = intSorting++;

            name = "SizeSorting";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Sortering";
            speciesList[name].Sorting = intSorting++;

            name = "SizeSortingDFU";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Opdeling";
            speciesList[name].Sorting = intSorting++;

            name = "Treatment";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Behandling";
            speciesList[name].Sorting = intSorting++;

            name = "Sex";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Køn";
            speciesList[name].Sorting = intSorting++;

            name = "Ovigorous";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Rogn";
            speciesList[name].Sorting = intSorting++;

            name = "Application";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Anvendelse";
            speciesList[name].Sorting = intSorting++;

            name = "Number";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Antal";
            speciesList[name].Sorting = intSorting++;

            name = "SubSampleWeightStep1";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Trin 1 - Vægt (kg)";
            speciesList[name].Sorting = intSorting++;

            name = "SubSampleWeightStep2";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Trin 2 - Vægt (kg)";
            speciesList[name].Sorting = intSorting++;

            name = "SubSampleWeightNotRep";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Ej Rep - Vægt (kg)";
            speciesList[name].Sorting = intSorting++;

            name = "SubSampleWeightBMSNotRep";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "BMS Ej Rep - Vægt (kg)";
            speciesList[name].Sorting = intSorting++;

            name = "LE";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "LE";
            speciesList[name].Sorting = intSorting++;

            name = "WeightEstimationMethod";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Vægtestimeringsmetode";
            speciesList[name].Sorting = intSorting++;

            if (speciesList.ContainsKey("Remark"))
                speciesList.Remove("Remark");
        }


        private void InitializeSpeciesListVIDDefaults()
        {
            var speciesList = _dic["VID:SpeciesList"];

            int intSorting = 0;

            string name = "Index";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "# (Indeks)";
            speciesList[name].Sorting = intSorting++;

            name = "Species";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Art";
            speciesList[name].Sorting = intSorting++;

            name = "LandingCategory";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Landingskategori";
            speciesList[name].Sorting = intSorting++;

            name = "SizeSorting";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Sortering";
            speciesList[name].Sorting = intSorting++;

            name = "SizeSortingDFU";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Opdeling";
            speciesList[name].Sorting = intSorting++;

            name = "Treatment";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Behandling";
            speciesList[name].Sorting = intSorting++;

            name = "Sex";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Køn";
            speciesList[name].Sorting = intSorting++;

            name = "Ovigorous";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Rogn";
            speciesList[name].Sorting = intSorting++;

            name = "Application";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Anvendelse";
            speciesList[name].Sorting = intSorting++;

            name = "Number";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Antal";
            speciesList[name].Sorting = intSorting++;

            name = "SubSampleWeightStep1";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Trin 1 - Vægt (kg)";
            speciesList[name].Sorting = intSorting++;

            name = "SubSampleWeightStep2";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Trin 2 - Vægt (kg)";
            speciesList[name].Sorting = intSorting++;

            name = "SubSampleWeightNotRep";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Ej Rep - Vægt (kg)";
            speciesList[name].Sorting = intSorting++;

            name = "SubSampleWeightBMSNotRep";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "BMS Ej Rep - Vægt (kg)";
            speciesList[name].Sorting = intSorting++;

            name = "LE";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "LE";
            speciesList[name].Sorting = intSorting++;

            name = "WeightEstimationMethod";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Vægtestimeringsmetode";
            speciesList[name].Sorting = intSorting++;

            if (speciesList.ContainsKey("Remark"))
                speciesList.Remove("Remark");
        }

        private void InitializeSpeciesListSØSDefaults()
        {
            var speciesList = _dic["SØS:SpeciesList"];

            int intSorting = 0;

            string name = "Index";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "# (Indeks)";
            speciesList[name].Sorting = intSorting++;

            name = "Species";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Art";
            speciesList[name].Sorting = intSorting++;

            name = "LandingCategory";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Landingskategori";
            speciesList[name].Sorting = intSorting++;

            name = "SizeSorting";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Sortering";
            speciesList[name].Sorting = intSorting++;

            name = "SizeSortingDFU";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Opdeling";
            speciesList[name].Sorting = intSorting++;

            name = "Treatment";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Behandling";
            speciesList[name].Sorting = intSorting++;

            name = "Sex";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Køn";
            speciesList[name].Sorting = intSorting++;

            name = "Ovigorous";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Rogn";
            speciesList[name].Sorting = intSorting++;

            name = "Application";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Anvendelse";
            speciesList[name].Sorting = intSorting++;

            name = "Number";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Antal";
            speciesList[name].Sorting = intSorting++;

            name = "SubSampleWeightStep1";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Trin 1 - Vægt (kg)";
            speciesList[name].Sorting = intSorting++;

            name = "SubSampleWeightStep2";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Trin 2 - Vægt (kg)";
            speciesList[name].Sorting = intSorting++;

            name = "SubSampleWeightNotRep";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Ej Rep - Vægt (kg)";
            speciesList[name].Sorting = intSorting++;

            name = "SubSampleWeightBMSNotRep";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "BMS Ej Rep - Vægt (kg)";
            speciesList[name].Sorting = intSorting++;

            name = "LE";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "LE";
            speciesList[name].Sorting = intSorting++;

            name = "WeightEstimationMethod";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Vægtestimeringsmetode";
            speciesList[name].Sorting = intSorting++;

            if (speciesList.ContainsKey("Remark"))
                speciesList.Remove("Remark");
        }

        private void InitializeSpeciesListHVNDefaults()
        {
            var speciesList = _dic["HVN:SpeciesList"];

            int intSorting = 0;

            string name = "Index";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "# (Indeks)";
            speciesList[name].Sorting = intSorting++;

            name = "Species";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Art";
            speciesList[name].Sorting = intSorting++;

            name = "LandingCategory";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Landingskategori";
            speciesList[name].Sorting = intSorting++;

            name = "SizeSorting";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Sortering";
            speciesList[name].Sorting = intSorting++;

            name = "SizeSortingDFU";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Opdeling";
            speciesList[name].Sorting = intSorting++;

            name = "Treatment";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Behandling";
            speciesList[name].Sorting = intSorting++;

            name = "Application";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Anvendelse";
            speciesList[name].Sorting = intSorting++;

            name = "Sex";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Køn";
            speciesList[name].Sorting = intSorting++;

            name = "Ovigorous";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Rogn";
            speciesList[name].Sorting = intSorting++;

            name = "Number";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Antal";
            speciesList[name].Sorting = intSorting++;

            name = "SubSampleWeightStep1";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Trin 1 - Vægt (kg)";
            speciesList[name].Sorting = intSorting++;

            name = "SubSampleWeightStep2";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Trin 2 - Vægt (kg)";
            speciesList[name].Sorting = intSorting++;

            name = "SubSampleWeightNotRep";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Ej Rep - Vægt (kg)";
            speciesList[name].Sorting = intSorting++;

            name = "SubSampleWeightBMSNotRep";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "BMS Ej Rep - Vægt (kg)";
            speciesList[name].Sorting = intSorting++;

            name = "LE";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "LE";
            speciesList[name].Sorting = intSorting++;

            name = "WeightEstimationMethod";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Vægtestimeringsmetode";
            speciesList[name].Sorting = intSorting++;

            if (speciesList.ContainsKey("Remark"))
                speciesList.Remove("Remark");
        }

        private void InitializeLAVRepDefaults()
        {
            var speciesList = _dic["LAVRep:SingleFish"];

            int intSorting = 0;

            string name = "Index";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "# (Indeks)";
            speciesList[name].Sorting = intSorting++;

            name = "Length";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Længde";
            speciesList[name].Sorting = intSorting++;

            name = "Number";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Antal";
            speciesList[name].Sorting = intSorting++;

            name = "WeightInGrams";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Vægt (g)";
            speciesList[name].Sorting = intSorting++;

            name = "Sex";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Køn";
            speciesList[name].Sorting = intSorting++;

            name = "BroodingPhase";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Rugefase";
            speciesList[name].Sorting = intSorting++;

            if (speciesList.ContainsKey("Remark"))
                speciesList.Remove("Remark");
        }

        private void InitializeSFRepDefaults()
        {
            var speciesList = _dic["SFRep:SingleFish"];
            int intSorting = 0;

            string name = "AnimalId";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Id";
            speciesList[name].Sorting = intSorting++;

            name = "IndividNum";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Individnr.";
            speciesList[name].Sorting = intSorting++;

            name = "Length";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Længde";
            speciesList[name].Sorting = intSorting++;

            name = "WeightInGrams";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Vægt (g)";
            speciesList[name].Sorting = intSorting++;

            name = "Sex";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Køn";
            speciesList[name].Sorting = intSorting++;

            name = "Maturity";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Modenhed";
            speciesList[name].Sorting = intSorting++;

            name = "Age";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Alder";
            speciesList[name].Sorting = intSorting++;

            name = "OtolithReadingRemark";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Otolith læsbarhed";
            speciesList[name].Sorting = intSorting++;

            name = "EdgeStructure";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Kantstruktur";
            speciesList[name].Sorting = intSorting++;

            name = "HatchMonth";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Klækningsmåned";
            speciesList[name].Sorting = intSorting++;

            name = "HatchMonthRemark";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Klækningsmånedslæsbarhed";
            speciesList[name].Sorting = intSorting++;

            name = "Parasite";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Parasit";
            speciesList[name].Sorting = intSorting++;

            name = "Genetics";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Genetik";
            speciesList[name].Sorting = intSorting++;

            name = "WeightGutted";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "R-vægt (g)";
            speciesList[name].Sorting = intSorting++;

            name = "WeightGonads";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "G-vægt (g)";
            speciesList[name].Sorting = intSorting++;

            name = "WeightLiver";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "L-vægt (g)";
            speciesList[name].Sorting = intSorting++;

            name = "References";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Referencer";
            speciesList[name].Sorting = intSorting++;

            name = "VisualStock";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Vis. bes.";
            speciesList[name].Sorting = intSorting++;

            name = "GenericStock";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Gen. bes.";
            speciesList[name].Sorting = intSorting++;

            name = "AquaDotsStatus";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Opdateret fra SmartDots";
            speciesList[name].Sorting = intSorting++;

            name = "AnimalFiles";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Otolith-billeder";
            speciesList[name].Sorting = intSorting++;
        }

        private void InitializeSFNotRepDefaults()
        {
            var speciesList = _dic["SFNotRep:SingleFish"];

            int intSorting = 0;

            string name = "AnimalId";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Id";
            speciesList[name].Sorting = intSorting++;

            name = "IndividNum";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Individnr.";
            speciesList[name].Sorting = intSorting++;

            name = "Length";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Længde";
            speciesList[name].Sorting = intSorting++;

            name = "WeightInGrams";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Vægt (g)";
            speciesList[name].Sorting = intSorting++;

            name = "Sex";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Køn";
            speciesList[name].Sorting = intSorting++;

            name = "Maturity";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Modenhed";
            speciesList[name].Sorting = intSorting++;

            name = "Age";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Alder";
            speciesList[name].Sorting = intSorting++;

            name = "OtolithReadingRemark";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Otolith læsbarhed";
            speciesList[name].Sorting = intSorting++;

            name = "EdgeStructure";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Kantstruktur";
            speciesList[name].Sorting = intSorting++;

            name = "HatchMonth";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Klækningsmåned";
            speciesList[name].Sorting = intSorting++;

            name = "HatchMonthRemark";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Klækningsmånedslæsbarhed";
            speciesList[name].Sorting = intSorting++;

            name = "Parasite";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Parasit";
            speciesList[name].Sorting = intSorting++;

            name = "Genetics";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Genetik";
            speciesList[name].Sorting = intSorting++;

            name = "WeightGutted";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "R-vægt (g)";
            speciesList[name].Sorting = intSorting++;

            name = "WeightGonads";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "G-vægt (g)";
            speciesList[name].Sorting = intSorting++;

            name = "WeightLiver";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "L-vægt (g)";
            speciesList[name].Sorting = intSorting++;

            name = "References";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Referencer";
            speciesList[name].Sorting = intSorting++;

            name = "VisualStock";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Vis. bes.";
            speciesList[name].Sorting = intSorting++;

            name = "GenericStock";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Gen. bes.";
            speciesList[name].Sorting = intSorting++;

            name = "AquaDotsStatus";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Opdateret fra SmartDots";
            speciesList[name].Sorting = intSorting++;

            name = "AnimalFiles";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Otolith-billeder";
            speciesList[name].Sorting = intSorting++;
        }


        private void InitializeREKSFRepDefaults()
        {
            var speciesList = _dic["SFRep:REKSingleFish"];

            int intSorting = 0;

            string name = "AnimalId";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Id";
            speciesList[name].Sorting = intSorting++;

            name = "IndividNum";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Individnr.";
            speciesList[name].Sorting = intSorting++;

            name = "Length";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Længde";
            speciesList[name].Sorting = intSorting++;

            name = "WeightInGrams";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Vægt (g)";
            speciesList[name].Sorting = intSorting++;

            name = "Sex";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Køn";
            speciesList[name].Sorting = intSorting++;

            name = "Maturity";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Modenhed";
            speciesList[name].Sorting = intSorting++;

            name = "Age";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Alder";
            speciesList[name].Sorting = intSorting++;

            name = "OtolithFinScale";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Otolith/FinScale";
            speciesList[name].Sorting = intSorting++;

            name = "OtolithReadingRemark";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Otolith læsbarhed";
            speciesList[name].Sorting = intSorting++;

            name = "EdgeStructure";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Kantstruktur";
            speciesList[name].Sorting = intSorting++;

            name = "HatchMonth";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Klækningsmåned";
            speciesList[name].Sorting = intSorting++;

            name = "HatchMonthRemark";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Klækningsmånedslæsbarhed";
            speciesList[name].Sorting = intSorting++;

            name = "Parasite";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Parasit";
            speciesList[name].Sorting = intSorting++;

            name = "Genetics";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Genetik";
            speciesList[name].Sorting = intSorting++;

            name = "WeightGutted";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "R-vægt (g)";
            speciesList[name].Sorting = intSorting++;

            name = "WeightGonads";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "G-vægt (g)";
            speciesList[name].Sorting = intSorting++;

            name = "WeightLiver";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "L-vægt (g)";
            speciesList[name].Sorting = intSorting++;

            name = "References";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Referencer";
            speciesList[name].Sorting = intSorting++;

            name = "AquaDotsStatus";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Opdateret fra SmartDots";
            speciesList[name].Sorting = intSorting++;

            name = "AnimalFiles";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Otolith-billeder";
            speciesList[name].Sorting = intSorting++;
        }

        private void InitializeREKSFNotRepDefaults()
        {
            var speciesList = _dic["SFNotRep:REKSingleFish"];

            int intSorting = 0;

            string name = "AnimalId";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Id";
            speciesList[name].Sorting = intSorting++;

            name = "IndividNum";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Individnr.";
            speciesList[name].Sorting = intSorting++;

            name = "Length";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Længde";
            speciesList[name].Sorting = intSorting++;

            name = "WeightInGrams";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Vægt (g)";
            speciesList[name].Sorting = intSorting++;

            name = "Sex";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Køn";
            speciesList[name].Sorting = intSorting++;

            name = "Maturity";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Modenhed";
            speciesList[name].Sorting = intSorting++;

            name = "Age";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Alder";
            speciesList[name].Sorting = intSorting++;

            name = "OtolithFinScale";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Otolith/FinScale";
            speciesList[name].Sorting = intSorting++;

            name = "OtolithReadingRemark";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Otolith læsbarhed";
            speciesList[name].Sorting = intSorting++;

            name = "EdgeStructure";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Kantstruktur";
            speciesList[name].Sorting = intSorting++;

            name = "HatchMonth";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Klækningsmåned";
            speciesList[name].Sorting = intSorting++;

            name = "HatchMonthRemark";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Klækningsmånedslæsbarhed";
            speciesList[name].Sorting = intSorting++;

            name = "Parasite";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "Parasit";
            speciesList[name].Sorting = intSorting++;

            name = "Genetics";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Genetik";
            speciesList[name].Sorting = intSorting++;

            name = "WeightGutted";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "R-vægt (g)";
            speciesList[name].Sorting = intSorting++;

            name = "WeightGonads";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "G-vægt (g)";
            speciesList[name].Sorting = intSorting++;

            name = "WeightLiver";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = false;
            speciesList[name].ColumnUIName = "L-vægt (g)";
            speciesList[name].Sorting = intSorting++;

            name = "References";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Referencer";
            speciesList[name].Sorting = intSorting++;

            name = "AquaDotsStatus";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Opdateret fra SmartDots";
            speciesList[name].Sorting = intSorting++;

            name = "AnimalFiles";
            if (!speciesList.ContainsKey(name))
                speciesList.Add(name, new DataGridColumnSettings(name, this));

            speciesList[name].DefaultIsVisible = true;
            speciesList[name].ColumnUIName = "Otolith-billeder";
            speciesList[name].Sorting = intSorting++;
        }

        public Dictionary<string, DataGridColumnSettings> GetColumns(string strVariableType, string strGroup)
        {
            if (_dic == null || strVariableType == null || strGroup == null)
                return null;

            string strKey = string.Format("{0}:{1}", strVariableType, strGroup);

            if (!_dic.ContainsKey(strKey))
                return null;

            var group = _dic[strKey];

            return group;
        }


        public bool GetVisibility(string strVariableType, string strGroup, string strColumnName)
        {
            try
            {
                if (strColumnName == null)
                    return true;

                var group = GetColumns(strVariableType, strGroup);
  
                if (group == null || !group.ContainsKey(strColumnName))
                    return true;

                return group[strColumnName].IsVisible;
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return true;
        }


        public void ResetAllColumnVisibilitiesToDefault(string strVariableType, string strGroup)
        {
            var group = GetColumns(strVariableType, strGroup);

            if (group == null || group.Count == 0)
                return;

            List<DataGridColumnSettings> lstResetColumns = new List<DataGridColumnSettings>();
            foreach (var c in group)
            {
                c.Value.ResetToDefault();
                lstResetColumns.Add(c.Value);
            }
        }

        private void CleanUpEmptyEntries(Dictionary<string,DataGridColumnSettings> speciesList)
        {
            if (speciesList != null)
            {
                foreach (var itm in speciesList.Values.ToList())
                {
                    if (string.IsNullOrWhiteSpace(itm.ColumnUIName))
                        speciesList.Remove(itm.ColumnName);
                }
            }
        }

    }
}
