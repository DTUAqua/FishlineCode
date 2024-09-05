using Anchor.Core;
using Anchor.Core.Controls;
using Babelfisk.BusinessLogic;
using Babelfisk.Entities;
using Babelfisk.Entities.Sprattus;
using Babelfisk.ViewModels.Input;
using Babelfisk.ViewModels.Map;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Babelfisk.ViewModels.SmartDots
{
    public class ImportFromCSVViewModel: AViewModel
    {
        
        private DelegateCommand _cmdClose;
        private DelegateCommand _cmdSave;
        private DelegateCommand _cmdResetMessages;
        private DelegateCommand<SDSampleItem> _cmdSortWarningErrorMessages;

        private SDSamplesViewModel _sdSampleVM;

        private List<SDSampleItem> _lstSDSampleItems;
        private List<SDSampleItem> _lstFilteredSDSampleItems;
        private List<SDSampleItem> _lstSelectedSDSampleItems;

        private List<SDMessageItem> _lstWarningsAndErrors;

        private List<L_DFUArea> _lstDFUArea;
        private List<L_Species> _lstSpecies;
        private List<L_StatisticalRectangle> _lstStatisticalRectangle;
        private List<L_SexCode> _lstSexCode;
        private List<L_MaturityIndexMethod> _lstMaturityIndexMethod;
        private List<Maturity> _lstMaturity;
        private List<L_OtolithReadingRemark> _lstOtolithReadingRemarks;
        private List<L_SDOtolithDescription> _lstSDOtolithDescriptions;
        private List<L_EdgeStructure> _lstEdgeStructures;
        private List<L_SDPreparationMethod> _lstPreparationMethods;
        private List<L_SDLightType> _lstLightTypes;
        private List<L_Stock> _lstStocks;
        private List<DFUPerson> _lstDFUPersons;

        private string _searchString;
        private string _fileName;

        private string defautImagePath = "";

        private ColumnVisibilityViewModel _vmColumnVisibility;

        string[] _imagePaths = null;


        #region Properties


        public List<SDSampleItem> SDSampleItemList
        {
            get { return _lstSDSampleItems; }
            set
            {
                _lstSDSampleItems = value;
                RaisePropertyChanged(()=>SDSampleItemList);
                RaisePropertyChanged(() => TotalSamples);
                RaisePropertyChanged(() => HasSampleItems);
            }
        }


        public List<SDSampleItem> SelectedSDSampleItems
        {
            get { return _lstSelectedSDSampleItems; }
            set
            {
                _lstSelectedSDSampleItems = value;
                RaisePropertyChanged(() => SelectedSDSampleItems);
            }
        }


        public bool? IsAllSelected
        {
            get
            {

                if (FilteredSDSampleItems != null && FilteredSDSampleItems.Count > 0 && FilteredSDSampleItems.Count == FilteredSDSampleItems.Where(x => x.IsSelected).ToList().Count())
                    return true;
                else if (FilteredSDSampleItems != null && FilteredSDSampleItems.Any(x => x.IsSelected))
                    return null;
                else
                    return false;
            }
            set
            {
                SetAllCheckboxes();
                RaisePropertyChanged(() => IsAllSelected);

            }
        }


        public bool? IsAllSelectedBoxValue
        {
            get
            {

                if (FilteredSDSampleItems != null && FilteredSDSampleItems.Count > 0 && FilteredSDSampleItems.Count == FilteredSDSampleItems.Where(x => x.IsSelected).ToList().Count())
                    return true;
                else if (FilteredSDSampleItems != null && FilteredSDSampleItems.Any(x => x.IsSelected))
                    return null;
                else
                    return false;
            }
            set
            {
                SetAllCheckboxes();
                RaisePropertyChanged(() => IsAllSelected);
                RaisePropertyChanged(() => IsAllSelectedBoxValue);

            }
        }


        public List<SDSampleItem> FilteredSDSampleItems
        {
            get { return _lstFilteredSDSampleItems; }
            set
            {
                _lstFilteredSDSampleItems = value;
                RaisePropertyChanged(() => FilteredSDSampleItems);
                RaisePropertyChanged(()=> TotalVisibleSamples);
            }
        }


        public List<SDMessageItem> WarningAndErrorMessages
        {
            get { return _lstWarningsAndErrors; }
            set { 
                _lstWarningsAndErrors = value;
                RaisePropertyChanged(() => WarningAndErrorMessages);
                RaisePropertyChanged(() => HasWarningAndErrorMessages);
            }
        }


        /// <summary>
        /// Get/Set what to filter the animals by. The events are filtered on name, event type, species, and start/stop date.
        /// </summary>
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
                RaisePropertyChanged(() => SearchString);
                FilterSDSampleItems();
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
        /// Return list of DFU species for drop down. 
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
        /// Return list of Statistical Rectangles for drop down. 
        /// </summary>
        public List<L_StatisticalRectangle> StatisticalRectangles
        {
            get { return _lstStatisticalRectangle == null ? null : _lstStatisticalRectangle.ToList(); }
            private set
            {
                _lstStatisticalRectangle = value;
                RaisePropertyChanged(() => StatisticalRectangles);
            }
        }


        /// <summary>
        /// Return list of Sex Codes for drop down. 
        /// </summary>
        public List<L_SexCode> SexCodes
        {
            get { return _lstSexCode == null ? null : _lstSexCode.ToList(); }
            private set
            {
                _lstSexCode = value;
                RaisePropertyChanged(() => SexCodes);
            }
        }


        /// <summary>
        /// Return list of Maturity Index Methods for drop down. 
        /// </summary>
        public List<L_MaturityIndexMethod> MaturityIndexMethods
        {
            get { return _lstMaturityIndexMethod == null ? null : _lstMaturityIndexMethod.ToList(); }
            private set
            {
                _lstMaturityIndexMethod = value;
                RaisePropertyChanged(() => MaturityIndexMethods);
            }
        }


        /// <summary>
        /// Return list of Maturity Index Methods for drop down. 
        /// </summary>
        public List<Maturity> MaturityList
        {
            get { return _lstMaturity == null ? null : _lstMaturity.ToList(); }
            private set
            {
                _lstMaturity = value;
                RaisePropertyChanged(() => MaturityIndexMethods);
            }
        }


        /// <summary>
        /// Return list of Otolith Reading Remarks for drop down. 
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
        /// Return list of Otolith Descriptions for drop down. 
        /// </summary>
        public List<L_SDOtolithDescription> OtolithDescriptions
        {
            get { return _lstSDOtolithDescriptions == null ? null : _lstSDOtolithDescriptions.ToList(); }
            private set
            {
                _lstSDOtolithDescriptions = value;
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
        /// Return list of Light Types for drop down. 
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
        /// Return list of Stocks for drop down. 
        /// </summary>
        public List<L_Stock> Stocks

        {
            get { return _lstStocks == null ? null : _lstStocks.ToList(); }
            private set
            {
                _lstStocks = value;
                RaisePropertyChanged(() => Stocks);
            }
        }


        /// <summary>
        /// Return list of DFUPersons for drop down. 
        /// </summary>
        public List<DFUPerson> DFUPersons

        {
            get { return _lstDFUPersons == null ? null : _lstDFUPersons.ToList(); }
            private set
            {
                _lstDFUPersons = value;
                RaisePropertyChanged(() => DFUPersons);
            }
        }


        public int TotalVisibleSamples
        {
            get { return _lstFilteredSDSampleItems == null ? 0 : _lstFilteredSDSampleItems.Count; }
        }


        public int TotalSamples
        {
            get { return _lstSDSampleItems == null ? 0 : _lstSDSampleItems.Count; }
        }


        public int TotalSelectedSamples
        {
            get { return _lstFilteredSDSampleItems == null && _lstFilteredSDSampleItems.Any(x => x.IsSelected) ? 0 : _lstFilteredSDSampleItems.Where(x => x.IsSelected).ToList().Count(); }
        }


        public bool HasSampleItems
        {
            get { return _lstSDSampleItems != null && _lstSDSampleItems.Count > 0; }
        }


        public bool HasSelectedSDSampleItems
        {
            get { return _lstSDSampleItems != null && _lstSDSampleItems.Count > 0 ? _lstSDSampleItems.Any(x => x.IsSelected) : false; }
        }


        public bool HasWarningAndErrorMessages
        {
            get { return WarningAndErrorMessages != null && WarningAndErrorMessages.Count > 0; }
        }


        public String ColumnVisivilityAny
        {
            get { return "Any"; }
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


        public ImportFromCSVViewModel(SDSamplesViewModel vmSample, string fileName, string[] imagePaths)
        {
            WindowWidth = 1100;
            WindowHeight = 500;

            _fileName = fileName;
             _sdSampleVM = vmSample;

            _imagePaths = imagePaths;

            _lstFilteredSDSampleItems = new List<SDSampleItem>();
            _lstSDSampleItems = new List<SDSampleItem>();
            _lstSelectedSDSampleItems = new List<SDSampleItem>();

            IsDirty = false;

            try
            {
                ColumnVisibility = new ColumnVisibilityViewModel(ColumnVisivilityAny, "SDSampleItems", ColumnVisibilityChanged);
                ColumnVisibility.WindowTitle = string.Format("Viste kolonner");
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        public void InitializeAsync()
        {
            IsLoading = true;
            Task.Factory.StartNew(Initialize);
        }

        private void Initialize()
        {


            try
            {
                var manLookup = new LookupManager();
                var lv = new LookupDataVersioning();

                //Fetch DFUArea
                var lstDFUArea = manLookup.GetLookups(typeof(L_DFUArea), lv).OfType<L_DFUArea>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch species
                var lstSpecies = manLookup.GetLookups(typeof(L_Species), lv).OfType<L_Species>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch Statistical Rectangle
                var lstStatisticalRectangle = manLookup.GetLookups(typeof(L_StatisticalRectangle), lv).OfType<L_StatisticalRectangle>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch Sex Codes
                var lstSexCode = manLookup.GetLookups(typeof(L_SexCode), lv).OfType<L_SexCode>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch Maturity Index Method
                var lstMaturityIndexMethod = manLookup.GetLookups(typeof(L_MaturityIndexMethod), lv).OfType<L_MaturityIndexMethod>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch Otolith Reading Remarks
                var lstOtolithReadingRemarks = manLookup.GetLookups(typeof(L_OtolithReadingRemark), lv).OfType<L_OtolithReadingRemark>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch Otolith Reading Remarks
                var lstOtolithSDDescription = manLookup.GetLookups(typeof(L_SDOtolithDescription), lv).OfType<L_SDOtolithDescription>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch Edge Structures
                var lstEdgeStructures = manLookup.GetLookups(typeof(L_EdgeStructure), lv).OfType<L_EdgeStructure>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch Preparation Methods
                var lstPreparationMethod = manLookup.GetLookups(typeof(L_SDPreparationMethod), lv).OfType<L_SDPreparationMethod>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch Light Types
                var lstLightTypes = manLookup.GetLookups(typeof(L_SDLightType), lv).OfType<L_SDLightType>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch Light Types
                var lststocks = manLookup.GetLookups(typeof(L_Stock), lv).OfType<L_Stock>().OrderBy(x => x.UIDisplay).ToList();

                //Fetch Maturity (which fits with maturity id)
                var lstmaturity = manLookup.GetLookups(typeof(Maturity), lv).OfType<Maturity>().OrderBy(x => (x.maturityIndexMethod ?? "")).ThenBy(x => x.maturityIndex).ToList();

                var lstDFUPersons = manLookup.GetLookups(typeof(DFUPerson), lv).OfType<DFUPerson>().OrderBy(x => x.UIDisplay).ToList();

                new Action(() =>
                {
                    DFUAreas = lstDFUArea;
                    Species = lstSpecies;
                    StatisticalRectangles = lstStatisticalRectangle;
                    SexCodes = lstSexCode;
                    MaturityIndexMethods = lstMaturityIndexMethod;
                    MaturityList = lstmaturity;
                    OtolithReadingRemarks = lstOtolithReadingRemarks;
                    OtolithDescriptions = lstOtolithSDDescription;
                    EdgeStructures = lstEdgeStructures;
                    PreparationMethods = lstPreparationMethod;
                    LightTypes = lstLightTypes;
                    Stocks = lststocks;
                    DFUPersons = lstDFUPersons;

                    if (!string.IsNullOrEmpty(_fileName))
                    {
                        //GetDataFromCSV(_fileName);
                        GetDataFromCSVAsync(_fileName);
                    }
                }).Dispatch();

            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
            
        }

        public void GetDataFromCSVAsync(string fileName)
        {
            IsLoading = true;
            Task.Factory.StartNew(() => GetDataFromCSV(fileName));
        }

        private void FilterSDSampleItems()
        {
            try
            {
                if (_lstSDSampleItems == null)
                {
                    FilteredSDSampleItems = new List<SDSampleItem>();
                    return;
                }

                IEnumerable<SDSampleItem> lstSampleItems = SDSampleItemList;
                if (!string.IsNullOrWhiteSpace(SearchString))
                {
                    var search = SearchString ?? "";
                    lstSampleItems = lstSampleItems.Where(x => !string.IsNullOrEmpty(x.Sample.animalId) && x.Sample.animalId.ToString().Contains(SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                                     x.Sample.readOnly.ToString().Contains(SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                                     (!string.IsNullOrEmpty(x.Sample.cruise) && x.Sample.cruise.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.Sample.trip) && x.Sample.trip.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.Sample.station) && x.Sample.station.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.Sample.DFUArea) && x.Sample.DFUArea.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrWhiteSpace(x.Sample.speciesCode) && x.Sample.speciesCode.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.Sample.statisticalRectangle) && x.Sample.statisticalRectangle.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.Sample.L_Stock != null && x.Sample.L_Stock.stockCode.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.Sample.fishWeightG.HasValue && x.Sample.fishWeightG.Value.ToString().Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.Sample.fishLengthMM.HasValue && x.Sample.fishLengthMM.Value.ToString().Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.Sample.sexCode) && x.Sample.sexCode.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.Sample.Maturity != null && x.Sample.Maturity.UIDisplay.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.Sample.L_OtolithReadingRemark != null && x.Sample.L_OtolithReadingRemark.otolithReadingRemark.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.Sample.L_SDPreparationMethod != null && x.Sample.L_SDPreparationMethod.preparationMethod.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.Sample.L_SDLightType != null && x.Sample.L_SDLightType.lightType.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.Sample.L_SDOtolithDescription != null && x.Sample.L_SDOtolithDescription.otolithDescription.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.Sample.edgeStructure) && x.Sample.edgeStructure.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.Sample.createdByUserName) && x.Sample.createdByUserName.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.Sample.CreatedTimeLocal != null && x.Sample.CreatedTimeLocal.Value.ToString("dd-MM-yyyy").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.Sample.ModifiedTimeLocal != null && x.Sample.ModifiedTimeLocal.Value.ToString("dd-MM-yyyy").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(x.Sample.comments) && x.Sample.comments.Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)) ||
                                     (x.Sample.catchDate != null && x.Sample.catchDate.Value.ToString("dd-MM-yyyy").Contains(SearchString, StringComparison.InvariantCultureIgnoreCase)));
                }

                _lstWarningsAndErrors = new List<SDMessageItem>();

                FilteredSDSampleItems = lstSampleItems.OrderBy(x => x.CSVRowNumber).ToList();

                RaisePropertyChanged(() => IsAllSelected);
                RaisePropertyChanged(() => IsAllSelectedBoxValue);

                foreach (var item in FilteredSDSampleItems)
                {
                    if(item.WarningErrorMessages != null && item.WarningErrorMessages.Count > 0)
                        foreach (var msg in item.WarningErrorMessages)
                        {
                            _lstWarningsAndErrors.Add(msg);
                        }
                }

                WarningAndErrorMessages = WarningAndErrorMessages.OrderBy(x => x.SampleItem.CSVRowNumber)
                                                                 .ThenBy(x => x.Type).ToList();

            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }

        private void ColumnVisibilityChanged(List<Babelfisk.BusinessLogic.Settings.DataGridColumnSettings> cs)
        {
            //Refresh trip type since it is bound to all column, forcing their vibility to be refreshed
            RaisePropertyChanged(() => ColumnVisivilityAny);
        }

        public void RaiseIsAllSelected()
        {
            RaisePropertyChanged(() => HasSelectedSDSampleItems);
            RaisePropertyChanged(() => TotalSelectedSamples);
            RaisePropertyChanged(() => IsAllSelected);
            RaisePropertyChanged(() => IsAllSelectedBoxValue);
        }

        public void RefreshTotals()
        {
            RaisePropertyChanged(() => TotalVisibleSamples);
            RaisePropertyChanged(() => TotalSamples);
            RaisePropertyChanged(() => TotalSelectedSamples);
        }


        /// <summary>
        /// Method returns the value if it is there. This method never returns null.
        /// </summary>
        private string GetDataValue(CSVColumnHeader ch, Dictionary<CSVColumnHeader, int> columnHeadersAndIndexes, string[] data)
        {
            var index = columnHeadersAndIndexes[ch];

            if (index >= data.Length)
                return "";

            return data[index];
        }


        private CSVColumnHeader? MapColumnNameToColumnHeader(string columnName)
        {
            switch(columnName.ToLowerInvariant())
            {
                case "sampleid":
                    return CSVColumnHeader.SampleID;

                case "fromfl":
                    return CSVColumnHeader.FromFL;

                case "imagename":
                    return CSVColumnHeader.ImageName;

                case "species":
                    return CSVColumnHeader.Species;

                case "cruise":
                    return CSVColumnHeader.Cruise;

                case "trip":
                    return CSVColumnHeader.Trip;

                case "station":
                    return CSVColumnHeader.Station;

                case "catchdate":
                case "catchdate(utc)":
                    return CSVColumnHeader.CatchDate;

                case "areacode":
                case "area":
                    return CSVColumnHeader.AreaCode;

                case "statisticalrectangle":
                    return CSVColumnHeader.StatisticalRectangle;

                case "stock":
                    return CSVColumnHeader.Stock;

                case "lengthmm":
                case "length(mm)":
                    return CSVColumnHeader.LengthMM;

                case "weightg":
                case "weight(g)":
                    return CSVColumnHeader.WeightG;

                case "sexcode":
                    return CSVColumnHeader.SexCode;

                case "maturityindexmethod":
                    return CSVColumnHeader.MaturityIndexMethod;

                case "mturityindex":
                    return CSVColumnHeader.MturityIndex;

                case "aqscore":
                    return CSVColumnHeader.AQScore;

                case "preparationmethod":
                case "prepmethod":
                    return CSVColumnHeader.PreparationMethod;

                case "lighttype":
                    return CSVColumnHeader.LightType;

                case "otodescription":
                    return CSVColumnHeader.OtoDescription;

                case "edgetype":
                    return CSVColumnHeader.EdgeType;

                case "latposstarttext":
                case "latposstart":
                case "latitude":
                    return CSVColumnHeader.LatPosStartText;

                case "lonposstart":
                case "lonposstarttext":
                case "longitude":
                    return CSVColumnHeader.LonPosStartText;

                case "createdby":
                    return CSVColumnHeader.CreatedBy;

                case "comments":
                    return CSVColumnHeader.Comments;
            }

            return null;
        }


        private void GetDataFromCSV(string fileName)
        {
            try
            {
                var man = new BusinessLogic.SmartDots.SmartDotsManager();

                if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
                {
                    try
                    {
                        string[] lines = File.ReadAllLines(fileName);

                        var headers = lines[0].Split(';');
                        Dictionary<CSVColumnHeader, int> columnHeadersAndIndexes = new Dictionary<CSVColumnHeader, int>();
                        if (headers != null && headers.Length > 0)
                        {
                            int counter = 0;
                            foreach (var item in headers)
                            {
                                CSVColumnHeader? csvHeader = MapColumnNameToColumnHeader(item);
                                if(csvHeader.HasValue)
                                    columnHeadersAndIndexes.Add(csvHeader.Value, counter);
                                counter++;
                            }

                            int intVal;
                            decimal decimalVal;
                            double doubleVal;

                            DateTime datetimeVal;

                            List<SDSampleItem> lst = _lstSDSampleItems ?? new List<SDSampleItem>();
                            Dictionary<int, int> idsFromFishline = new Dictionary<int, int>();

                            for (int i = 1; i < lines.Length; i++)
                            {
                                SDSample thisSample = new SDSample();

                                var data = lines[i].Split(';');
                                List<string> warnings = new List<string>();
                                try
                                {
                                    //AnimalID (Mandatory)
                                    if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.SampleID))
                                    {
                                        thisSample.animalId = GetDataValue(CSVColumnHeader.SampleID, columnHeadersAndIndexes, data);
                                    }

                                    bool isFromFishLineValue = false;

                                    //IsFromFIshline
                                    if (!columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.FromFL))
                                    {
                                        var thisSampleItem = new SDSampleItem(this, thisSample, i + 1);
                                        thisSampleItem.WarningErrorMessages.Add(new SDMessageItem(thisSampleItem, MessageType.Error, Translate("ImportFromCSVView", "WarningIsFromFIshlineIssue")));
                                        lst.Insert(0, thisSampleItem);
                                        continue;
                                    }
                                    else if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.FromFL))
                                    {
                                        var fromFishlineValue = GetDataValue(CSVColumnHeader.FromFL, columnHeadersAndIndexes, data).Trim();

                                        if (string.IsNullOrWhiteSpace(fromFishlineValue) || !(fromFishlineValue.Equals("FALSK", StringComparison.InvariantCultureIgnoreCase)
                                                                                           || fromFishlineValue.Equals("SANDT", StringComparison.InvariantCultureIgnoreCase)
                                                                                           || fromFishlineValue.Equals("0", StringComparison.InvariantCultureIgnoreCase)
                                                                                           || fromFishlineValue.Equals("1", StringComparison.InvariantCultureIgnoreCase)
                                                                                           || fromFishlineValue.Equals("true", StringComparison.InvariantCultureIgnoreCase)
                                                                                           || fromFishlineValue.Equals("false", StringComparison.InvariantCultureIgnoreCase)))
                                        {
                                            var thisSampleItem = new SDSampleItem(this, thisSample, i + 1);
                                            thisSampleItem.WarningErrorMessages.Add(new SDMessageItem(thisSampleItem, MessageType.Error, Translate("ImportFromCSVView", "WarningIsFromFIshlineIssue")));
                                            lst.Insert(0, thisSampleItem);
                                            continue;
                                        }

                                        isFromFishLineValue = fromFishlineValue.Equals("SANDT", StringComparison.InvariantCultureIgnoreCase) ||
                                                              fromFishlineValue.Equals("1", StringComparison.InvariantCultureIgnoreCase) ||
                                                              fromFishlineValue.Equals("true", StringComparison.InvariantCultureIgnoreCase);

                                        if (isFromFishLineValue)
                                        {
                                            thisSample.ImportStatusEnum = SDSampleImportStatus.ImportedFromFishline;
                                            thisSample.readOnly = true;
                                        }
                                    }

                                    if (isFromFishLineValue)
                                    {
                                        int idVal;
                                        if (GetDataValue(CSVColumnHeader.SampleID, columnHeadersAndIndexes, data).TryParseInt32(out idVal))
                                        {
                                            idsFromFishline.Add(i + 1, idVal);
                                        }
                                        else //If animal id was not found, through an error.
                                        {
                                            var result = AppRegionManager.ShowMessageBox(string.Format(Translate("ImportFromCSVView", "ErrorAnimalId"), i + 1), System.Windows.MessageBoxButton.OK);
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        //cruise 
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.Cruise) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.Cruise, columnHeadersAndIndexes, data)))
                                        {
                                            thisSample.cruise = GetDataValue(CSVColumnHeader.Cruise, columnHeadersAndIndexes, data);
                                        }

                                        //trip
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.Trip) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.Trip, columnHeadersAndIndexes, data)))
                                            thisSample.trip = GetDataValue(CSVColumnHeader.Trip, columnHeadersAndIndexes, data);

                                        //station
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.Station) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.Station, columnHeadersAndIndexes, data)))
                                            thisSample.station = GetDataValue(CSVColumnHeader.Station, columnHeadersAndIndexes, data);


                                        //lengthMM (Mandatory)
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.LengthMM) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.LengthMM, columnHeadersAndIndexes, data))
                                            && GetDataValue(CSVColumnHeader.LengthMM, columnHeadersAndIndexes, data).TryParseInt32(out intVal))
                                            thisSample.fishLengthMM = intVal;

                                        //weightG
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.WeightG) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.WeightG, columnHeadersAndIndexes, data))
                                            && GetDataValue(CSVColumnHeader.WeightG, columnHeadersAndIndexes, data).TryParseDecimal(out decimalVal))
                                            thisSample.fishWeightG = decimalVal;

                                        //sexCode 
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.SexCode) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.SexCode, columnHeadersAndIndexes, data)))
                                        {
                                            thisSample.L_SexCode = SexCodes.Where(x => x.sexCode.Equals(GetDataValue(CSVColumnHeader.SexCode, columnHeadersAndIndexes, data), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                            if (thisSample.L_SexCode == null || string.IsNullOrEmpty(thisSample.L_SexCode.sexCode))
                                                warnings.Add(Translate("ImportFromCSVView", "WarningSexCodeIssue"));
                                        }

                                        //maturityIndexMethod
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.MaturityIndexMethod) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.MaturityIndexMethod, columnHeadersAndIndexes, data)))
                                            thisSample.L_MaturityIndexMethod = MaturityIndexMethods.Where(x => x.maturityIndexMethod == GetDataValue(CSVColumnHeader.MaturityIndexMethod, columnHeadersAndIndexes, data)).FirstOrDefault();

                                        int maturityIndex = 0;
                                        if (thisSample.L_MaturityIndexMethod != null && columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.MturityIndex) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.MturityIndex, columnHeadersAndIndexes, data)) && GetDataValue(CSVColumnHeader.MturityIndex, columnHeadersAndIndexes, data).TryParseInt32(out maturityIndex))
                                            thisSample.Maturity = MaturityList.Where(x => x.maturityIndexMethod == thisSample.L_MaturityIndexMethod.maturityIndexMethod && x.maturityIndex == maturityIndex).FirstOrDefault();

                                        //otolithReadingRemark
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.AQScore) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.AQScore, columnHeadersAndIndexes, data)))
                                            thisSample.L_OtolithReadingRemark = OtolithReadingRemarks.Where(x => x.otolithReadingRemark.Equals(GetDataValue(CSVColumnHeader.AQScore, columnHeadersAndIndexes, data), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();


                                        //edgeStructure
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.EdgeType) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.EdgeType, columnHeadersAndIndexes, data)))
                                        {
                                            thisSample.L_EdgeStructure = EdgeStructures.Where(x => x.edgeStructure.Equals(GetDataValue(CSVColumnHeader.EdgeType, columnHeadersAndIndexes, data), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                            if (thisSample.L_EdgeStructure == null || string.IsNullOrEmpty(thisSample.L_EdgeStructure.edgeStructure))
                                                warnings.Add(Translate("ImportFromCSVView", "WarningEdgeStructureIssue"));
                                        }

                                        //species
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.Species) && !string.IsNullOrWhiteSpace(GetDataValue(CSVColumnHeader.Species, columnHeadersAndIndexes, data)))
                                        {
                                            thisSample.L_Species = Species.Where(x => x.speciesCode.Equals(GetDataValue(CSVColumnHeader.Species, columnHeadersAndIndexes, data), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                        }
                                        else if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.Species) && string.IsNullOrWhiteSpace(GetDataValue(CSVColumnHeader.Species, columnHeadersAndIndexes, data)))
                                        {

                                            thisSample.L_Species = null;
                                        }
                                        else if (_sdSampleVM != null && _sdSampleVM.Event != null && _sdSampleVM.Event.speciesCode != null) //If no species was found from the file, use the event species.
                                            thisSample.L_Species = Species.Where(x => x.speciesCode == _sdSampleVM.Event.speciesCode).FirstOrDefault();

                                        //stock
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.Stock) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.Stock, columnHeadersAndIndexes, data)))
                                        {
                                            thisSample.L_Stock = Stocks.Where(x => x.stockCode.Equals(GetDataValue(CSVColumnHeader.Stock, columnHeadersAndIndexes, data), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                            if (thisSample.L_Stock == null)
                                                warnings.Add(Translate("ImportFromCSVView", "WarningStockIssue"));
                                        }

                                        //dfuArea (Mandatory)
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.AreaCode) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.AreaCode, columnHeadersAndIndexes, data)))
                                            thisSample.L_DFUArea = DFUAreas.Where(x => x.DFUArea.Equals(GetDataValue(CSVColumnHeader.AreaCode, columnHeadersAndIndexes, data), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();


                                        //latPosStartText
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.LatPosStartText) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.LatPosStartText, columnHeadersAndIndexes, data)))
                                        {
                                            if (GetDataValue(CSVColumnHeader.LatPosStartText, columnHeadersAndIndexes, data).TryParseDouble(out doubleVal))
                                                thisSample.latitude = doubleVal;
                                            else
                                            {
                                                warnings.Add(Translate("ImportFromCSVView", "WarningLatitudeIssue"));
                                            }
                                        }

                                        //lonPosStartText
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.LonPosStartText) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.LonPosStartText, columnHeadersAndIndexes, data)))
                                        {
                                            if (GetDataValue(CSVColumnHeader.LonPosStartText, columnHeadersAndIndexes, data).TryParseDouble(out doubleVal))
                                                thisSample.longitude = doubleVal;
                                            else
                                            {
                                                warnings.Add(Translate("ImportFromCSVView", "WarningLongitudeIssue"));
                                            }
                                        }

                                        //statisticalRectangle
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.StatisticalRectangle) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.StatisticalRectangle, columnHeadersAndIndexes, data)))
                                        {
                                            thisSample.L_StatisticalRectangle = StatisticalRectangles.Where(x => x.statisticalRectangle.Equals(GetDataValue(CSVColumnHeader.StatisticalRectangle, columnHeadersAndIndexes, data), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                            if (thisSample.L_StatisticalRectangle == null)
                                                warnings.Add(Translate("ImportFromCSVView", "WarningStatisticalRectangleIssue"));
                                        }

                                        //preparationMethod (Mandatory)
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.PreparationMethod) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.PreparationMethod, columnHeadersAndIndexes, data)))
                                            thisSample.L_SDPreparationMethod = PreparationMethods.Where(x => x.preparationMethod.Equals(GetDataValue(CSVColumnHeader.PreparationMethod, columnHeadersAndIndexes, data), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();


                                        //lightType
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.LightType) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.LightType, columnHeadersAndIndexes, data)))
                                        {
                                            thisSample.L_SDLightType = LightTypes.Where(x => x.lightType.Equals(GetDataValue(CSVColumnHeader.LightType, columnHeadersAndIndexes, data), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                            if (thisSample.L_SDLightType == null)
                                                warnings.Add(Translate("ImportFromCSVView", "WarningSDLightTypeIssue"));
                                        }

                                        //otolithDescription
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.OtoDescription) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.OtoDescription, columnHeadersAndIndexes, data)))
                                        {
                                            thisSample.L_SDOtolithDescription = OtolithDescriptions.Where(x => x.otolithDescription.Equals(GetDataValue(CSVColumnHeader.OtoDescription, columnHeadersAndIndexes, data), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                            if (thisSample.L_SDOtolithDescription == null)
                                                warnings.Add(Translate("ImportFromCSVView", "WarningOtolithDescriptionIssue"));
                                        }

                                        //createdBy
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.CreatedBy) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.CreatedBy, columnHeadersAndIndexes, data)))
                                        {
                                            thisSample.DFUPerson = DFUPersons.Where(x => x.initials != null && x.initials.Equals(GetDataValue(CSVColumnHeader.CreatedBy, columnHeadersAndIndexes, data), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                            thisSample.createdByUserName = thisSample.DFUPerson == null ? null : thisSample.DFUPerson.initials;
                                            if (thisSample.DFUPerson == null)
                                                warnings.Add(Translate("ImportFromCSVView", "WarningCreatedBy"));
                                        }

                                        if (thisSample.DFUPerson == null)
                                        {
                                            thisSample.createdById = DFUPersonLogin == null ? null : new Nullable<int>(DFUPersonLogin.dfuPersonId);
                                            thisSample.createdByUserName = DFUPersonLogin == null ? null : DFUPersonLogin.initials;
                                        }

                                        //catchDate (Mandatory)
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.CatchDate) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.CatchDate, columnHeadersAndIndexes, data)))
                                            if (DateTime.TryParseExact(GetDataValue(CSVColumnHeader.CatchDate, columnHeadersAndIndexes, data), "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out datetimeVal) ||
                                                DateTime.TryParseExact(GetDataValue(CSVColumnHeader.CatchDate, columnHeadersAndIndexes, data), "dd-MM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out datetimeVal) ||
                                                DateTime.TryParseExact(GetDataValue(CSVColumnHeader.CatchDate, columnHeadersAndIndexes, data), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out datetimeVal))
                                            {
                                                thisSample.catchDate = datetimeVal;
                                            }
                                            else
                                            {
                                                //Datetime format warning 
                                                warnings.Add(Translate("ImportFromCSVView", "WarningCatchDateFormatIssue"));
                                            }

                                        //comments
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.Comments) && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.Comments, columnHeadersAndIndexes, data)))
                                            thisSample.comments = GetDataValue(CSVColumnHeader.Comments, columnHeadersAndIndexes, data);


                                        //SDFile
                                        if (columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.ImageName)
                                            && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.ImageName, columnHeadersAndIndexes, data)))
                                        {
                                            var filesNames = GetDataValue(CSVColumnHeader.ImageName, columnHeadersAndIndexes, data).Split(',');
                                            int idInt;

                                            foreach (var item in filesNames)
                                            {
                                                thisSample.SDFile.Add(new SDFile()
                                                {
                                                    sdFileGuid = Guid.NewGuid(),
                                                    displayName = item

                                                });
                                            }
                                        }

                                        var thisSampleItem = new SDSampleItem(this, thisSample, i + 1);

                                        //Add error messages to this sample
                                        //Species 
                                        if (thisSample.L_Species == null)
                                        {
                                            thisSampleItem.WarningErrorMessages.Add(new SDMessageItem(thisSampleItem, MessageType.Error, Translate("ImportFromCSVView", "ErrorNoSpecies")));
                                        }
                                        else if (_sdSampleVM != null && _sdSampleVM.Event != null
                                            && !string.IsNullOrEmpty(_sdSampleVM.Event.speciesCode)
                                            && columnHeadersAndIndexes.ContainsKey(CSVColumnHeader.Species)
                                            && !string.IsNullOrEmpty(GetDataValue(CSVColumnHeader.Species, columnHeadersAndIndexes, data))
                                            && !_sdSampleVM.Event.speciesCode.Equals(GetDataValue(CSVColumnHeader.Species, columnHeadersAndIndexes, data), StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            //Error species do not match
                                            thisSampleItem.WarningErrorMessages.Insert(0, new SDMessageItem(thisSampleItem, MessageType.Error, string.Format(Translate("SDSelectAnimalsView", "WarningAnimalAnotherSpecies"), data[columnHeadersAndIndexes[CSVColumnHeader.Species]])));
                                        }

                                        //Missing animal Id error
                                        if (string.IsNullOrWhiteSpace(thisSample.animalId))
                                            thisSampleItem.WarningErrorMessages.Add(new SDMessageItem(thisSampleItem, MessageType.Error, Translate("ImportFromCSVView", "ErrorNoSampleId")));

                                        if (thisSample.SDFile.Count == 0)
                                            thisSampleItem.WarningErrorMessages.Add(new SDMessageItem(thisSampleItem, MessageType.Error, Translate("ImportFromCSVView", "ErrorNoImageName")));

                                        //Missing fish length error
                                        if (thisSample.fishLengthMM == null || thisSample.fishLengthMM == 0)
                                            thisSampleItem.WarningErrorMessages.Add(new SDMessageItem(thisSampleItem, MessageType.Error, Translate("ImportFromCSVView", "ErrorNofishLength")));


                                        //Missing preparation method error
                                        if (thisSample.L_SDPreparationMethod == null || !thisSample.sdPreparationMethodId.HasValue)
                                            thisSampleItem.WarningErrorMessages.Add(new SDMessageItem(thisSampleItem, MessageType.Error, Translate("ImportFromCSVView", "ErrorNoPreparationMethod")));

                                        if (thisSample.L_SDLightType == null || !thisSample.sdLightTypeId.HasValue)
                                            thisSampleItem.WarningErrorMessages.Add(new SDMessageItem(thisSampleItem, MessageType.Error, Translate("ImportFromCSVView", "ErrorNoLightType")));

                                        //Missing created by user error - not mandatory any more.
                                        //if (string.IsNullOrEmpty(thisSample.createdByUserName))
                                        //    thisSampleItem.WarningErrorMessages.Add(new SDMessageItem(thisSampleItem, MessageType.Error, Translate("ImportFromCSVView", "ErrorNoCreatedBy")));

                                        //Missing catch date error
                                        if (thisSample.catchDate == null || thisSample.catchDate == default(DateTime))
                                            thisSampleItem.WarningErrorMessages.Add(new SDMessageItem(thisSampleItem, MessageType.Error, Translate("ImportFromCSVView", "ErrorNoCatchDate")));

                                        //Check if sample already added to event (id and preparation method)
                                        if (_sdSampleVM != null && _sdSampleVM.HasSamples && _sdSampleVM.SDSampleList.Any(x => x.animalId == thisSample.animalId && x.sdPreparationMethodId == thisSample.sdPreparationMethodId))
                                        {
                                            thisSampleItem.IsAdded = true;
                                            thisSampleItem.WarningErrorMessages.Add(new SDMessageItem(thisSampleItem, MessageType.Duplicate, Translate("ImportFromCSVView", "Duplicate")));
                                        }

                                        thisSampleItem.IsSelected = !thisSampleItem.HasErrors && !thisSampleItem.HasDuplicates;

                                        //Add warning messages to this sample
                                        foreach (var wm in warnings)
                                        {
                                            thisSampleItem.WarningErrorMessages.Add(new SDMessageItem(thisSampleItem, MessageType.Warning, wm));
                                        }


                                        lst.Insert(0, thisSampleItem);

                                    }

                                }
                                catch (Exception exc)
                                {

                                    LogAndDispatchUnexpectedErrorMessage(exc);
                                }
                            }

                            //Get sample data from fishline
                            if (idsFromFishline != null && idsFromFishline.Count > 0)
                            {
                                var selectionAnimals = new List<SelectionAnimal>();

                                selectionAnimals = man.GetSelectionAnimals(idsFromFishline.Values.Distinct().ToArray(), false);

                                List<OtolithFileInformation> lstfileinformation = new List<OtolithFileInformation>();
                                try
                                {
                                    foreach (var chunk in idsFromFishline.InChunks(100))
                                    {
                                        List<string> animalIds = chunk.Select(x => x.Value.ToString()).ToList();

                                        var resfileinformation = man.GetFileInformationFromAnimalIds(animalIds, _imagePaths);
                                        if(resfileinformation != null && resfileinformation.Count> 0) 
                                        {
                                            lstfileinformation.AddRange(resfileinformation);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    LogAndDispatchUnexpectedErrorMessage(e);
                                }

                                foreach (var kv in idsFromFishline)
                                {
                                    try
                                    {
                                        var sa = selectionAnimals.Where(x => x.AnimalId == kv.Value).FirstOrDefault();

                                        if (sa == null)
                                        {
                                            var si = new SDSampleItem(this, new SDSample() { animalId = kv.Value.ToString(), readOnly = true }, kv.Key);
                                            si.WarningErrorMessages.Add(new SDMessageItem(si, MessageType.Error, string.Format(Translate("SDSelectAnimalsView", "AnimalIdNotFound"), kv.Value)));

                                            lst.Add(si);
                                        }
                                        else
                                        {
                                            var sdSample = GetSDSampleFromSelectionAnimal(sa, lstfileinformation);

                                            var si = new SDSampleItem(this, sdSample, kv.Key);


                                            if (_sdSampleVM != null && _sdSampleVM.Event != null && _sdSampleVM.Event.speciesCode != sa.SpeciesCode)
                                            {
                                                si.WarningErrorMessages.Add(new SDMessageItem(si, MessageType.Error, string.Format(Translate("SDSelectAnimalsView", "WarningAnimalAnotherSpecies"), sa.SpeciesCode)));
                                            }

                                            if (_sdSampleVM != null && _sdSampleVM.HasSamples && _sdSampleVM.SDSampleList.Any(x => x.animalId == si.Sample.animalId && x.sdPreparationMethodId == si.Sample.sdPreparationMethodId))
                                            {
                                                si.IsAdded = true;
                                                si.WarningErrorMessages.Add(new SDMessageItem(si, MessageType.Duplicate, Translate("ImportFromCSVView", "Duplicate")));
                                            }

                                            si.IsSelected = !si.HasErrors && !si.HasDuplicates;

                                            lst.Add(si);
                                        }

                                    }
                                    catch (Exception e)
                                    {
                                        LogAndDispatchUnexpectedErrorMessage(e);
                                    }
                                }
                            }

                            foreach (var item in lst)
                            {
                                //add created time
                                item.Sample.createdTime = DateTime.UtcNow;
                                item.Sample.modifiedTime = item.Sample.createdTime;

                                //Check DFU area. 
                                if (item.Sample.DFUArea == null)
                                    item.WarningErrorMessages.Insert(0, new SDMessageItem(item, MessageType.Error, Translate("ImportFromCSVView", "ErrorNoDFUArea")));
                                else if (item.Sample.DFUArea != null && _sdSampleVM != null && _sdSampleVM.Event != null && !_sdSampleVM.Event.L_DFUAreas.Any(x => x.DFUArea == item.Sample.DFUArea))
                                    item.WarningErrorMessages.Insert(0, new SDMessageItem(item, MessageType.Error, string.Format(Translate("SDSelectAnimalsView", "WarningAnimalAnotherArea"), item.Sample.DFUArea)));
                                //Missing area error
                            }

                            new Action(() =>
                            {
                                AssignImageFilePaths(lst);

                                foreach (var item in lst)
                                {
                                    item.RaiseHasErrors();
                                }

                                SDSampleItemList = lst;

                                FilterSDSampleItems();
                            }).Dispatch();
                        }

                    }
                    catch (Exception ex)
                    {

                        LogAndDispatchUnexpectedErrorMessage(ex);
                    }
                }

            }
            catch (Exception e)
            {
                LogAndDispatchUnexpectedErrorMessage(e);
            }
            finally
            {
                new Action(() =>
                {
                    IsLoading = false;
                }).Dispatch();
            }
        }


        private void AssignImageFilePaths(List<SDSampleItem> lst)
        {

            if (lst == null)
                return;

            var man = new BusinessLogic.SmartDots.SmartDotsManager();

            //check if elements in lst have pictures
            var allSdFileNames = lst.Where(y => y.Sample != null && y.Sample.SDFile != null).SelectMany(x => x.Sample.SDFile.Select(y => y.displayName)).Distinct().ToList();
            if (allSdFileNames != null)
            {
                try
                {
                    //get all file information objects for images by names 
                    var allPaths = man.GetFileInformationFromFileNames(allSdFileNames, _imagePaths);
                    if (allPaths == null)
                    {
                        //No file paths returned 
                        var result = AppRegionManager.ShowMessageBox(Translate("ImportFromCSVView", "NoFilePaths"), System.Windows.MessageBoxButton.OK);
                        return;
                    }
                    else 
                    {
                        HashSet<string> addImages = new HashSet<string>();
                        foreach (var sampleItem in lst.OrderBy(x => x.CSVRowNumber))
                        {
                            //get kv pair with otolith file info for each sample
                            if (sampleItem.Sample.SDFile != null && sampleItem.Sample.SDFile.Count > 0)
                            {
                                foreach (var sdFile in sampleItem.Sample.SDFile.ToList())
                                {
                                    List<OtolithFileInformation> otolithFileInfoList = null;
                                    var fileNameLower = sdFile.displayName.ToLowerInvariant();
                                    if (allPaths.ContainsKey(fileNameLower))
                                        otolithFileInfoList = allPaths[fileNameLower];

                                    //if no otolith file information return, remove sd file
                                    if (otolithFileInfoList == null || otolithFileInfoList.Count() < 1)
                                    {
                                        //add warning
                                        sampleItem.WarningErrorMessages.Add(new SDMessageItem(sampleItem, MessageType.Error, string.Format(Translate("ImportFromCSVView", "ErrorPathNotFoundSDFileRemoved"), sdFile.displayName)));
                                        //remove sdfile
                                        sampleItem.Sample.SDFile.Remove(sdFile);
                                        continue;
                                    }
                                    else if (otolithFileInfoList.Count == 1)
                                    {
                                        var otolithFileInfo = otolithFileInfoList.FirstOrDefault();
                                        sdFile.fileName = otolithFileInfo.FileName;
                                        sdFile.path = otolithFileInfo.RelativeDirectoryPath;
                                    }
                                    else if (otolithFileInfoList.Count > 1)
                                    {
                                        var relativeDirectoryPaths = otolithFileInfoList.Select(x => x.RelativeDirectoryPath).ToList();
                                        if (relativeDirectoryPaths != null && relativeDirectoryPaths.Count() > 0)
                                        {
                                            if (!string.IsNullOrEmpty(defautImagePath) && relativeDirectoryPaths.Contains(defautImagePath))
                                            {
                                                var otolithFile = otolithFileInfoList.Where(x => x.RelativeDirectoryPath == defautImagePath).FirstOrDefault();
                                                if (otolithFile != null)
                                                {
                                                    sdFile.fileName = otolithFile.FileName;
                                                    sdFile.path = otolithFile.RelativeDirectoryPath;
                                                }
                                                else
                                                {
                                                    //add warning
                                                    sampleItem.WarningErrorMessages.Add(new SDMessageItem(sampleItem, MessageType.Error, string.Format(Translate("ImportFromCSVView", "ErrorPathNotFoundSDFileRemoved"), sdFile.displayName)));
                                                    //remove sdfile
                                                    sampleItem.Sample.SDFile.Remove(sdFile);
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                string pathResult = null;
                                                new Action(() =>
                                                {
                                                    //User selects directory
                                                    pathResult = SelectPrimaryDirectory(relativeDirectoryPaths, sdFile.displayName, ref defautImagePath);
                                                }).DispatchInvoke();

                                                if (!string.IsNullOrEmpty(pathResult))
                                                {
                                                    var otolithFileInfo = otolithFileInfoList.Where(x => x.RelativeDirectoryPath == pathResult).FirstOrDefault();
                                                    if (otolithFileInfo != null)
                                                    {
                                                        sdFile.fileName = otolithFileInfo.FileName;
                                                        sdFile.path = otolithFileInfo.RelativeDirectoryPath;
                                                    }
                                                    else
                                                    {
                                                        //add warning
                                                        sampleItem.WarningErrorMessages.Add(new SDMessageItem(sampleItem, MessageType.Error, string.Format(Translate("ImportFromCSVView", "ErrorPathNotFoundSDFileRemoved"), sdFile.displayName)));
                                                        //remove sdfile
                                                        sampleItem.Sample.SDFile.Remove(sdFile);
                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    //if user cancels selection remove sd file
                                                    try
                                                    {
                                                        //add warning
                                                        sampleItem.WarningErrorMessages.Add(new SDMessageItem(sampleItem, MessageType.Error, string.Format(Translate("ImportFromCSVView", "ErrorPathNotFoundSDFileRemoved"), sdFile.displayName)));
                                                        //remove sdfile
                                                        sampleItem.Sample.SDFile.Remove(sdFile);
                                                        continue;
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        LogAndDispatchUnexpectedErrorMessage(e);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //add warning
                                        sampleItem.WarningErrorMessages.Add(new SDMessageItem(sampleItem, MessageType.Error, string.Format(Translate("ImportFromCSVView", "ErrorPathNotFoundSDFileRemoved"), sdFile.displayName)));
                                        //remove sdfile
                                        sampleItem.Sample.SDFile.Remove(sdFile);
                                        continue;
                                    }

                                    //Make sure image is not already added to another sample in the event
                                    if (AddEditSDSampleViewModel.IsImageAlreadyAddedToEvent(_sdSampleVM, fileNameLower))
                                    {
                                        //add warning
                                        sampleItem.WarningErrorMessages.Add(new SDMessageItem(sampleItem, MessageType.Error, string.Format(Translate("ImportFromCSVView", "ErrorImageAlreadyAddedAnotherEventSample"), sdFile.displayName)));
                                        //remove sdfile
                                        sampleItem.Sample.SDFile.Remove(sdFile);
                                        continue;
                                    }

                                    //Make sure the image is not already added to another sample to be imported.
                                    if(addImages.Contains(fileNameLower))
                                    {
                                        //add warning
                                        sampleItem.WarningErrorMessages.Add(new SDMessageItem(sampleItem, MessageType.Error, string.Format(Translate("ImportFromCSVView", "ErrorImageAlreadyAddedAnotherSample"), sdFile.displayName)));
                                        //remove sdfile
                                        sampleItem.Sample.SDFile.Remove(sdFile);
                                        continue;
                                    }

                                    //Image was added to a sample, so add it to a global list, so it can be checked for later.
                                    addImages.Add(fileNameLower);
                                }
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    LogAndDispatchUnexpectedErrorMessage(e);
                }
            }
                
        }


        public static string SelectPrimaryDirectory(List<string> paths, string fileName, ref string setDefaultImagePath)
        {
            if (paths == null)
                return null;

           string  selectedPath = "";

            try
            {
                var vm = new SmartDots.SelectDirectoryPathViewModel(paths, fileName);
                vm.InitializeView();
                _appRegionManager.LoadWindowViewFromViewModel(vm, true, "WindowWithBorderStyle");
                if (vm.Saved)
                {
                    if (vm.FilteredDirectoryPathItems.Any(x => x.IsSelected))
                        selectedPath = vm.FilteredDirectoryPathItems.Where(x => x.IsSelected).FirstOrDefault().PathString;

                    if (vm.IsDefaultPath)
                        setDefaultImagePath = selectedPath;
                }
            }
            catch (Exception e)
            {
                AViewModel.LogAndDispatchUnexpectedErrorMessageStatic(e);
            }

            return selectedPath;
        }


        private SDSample GetSDSampleFromSelectionAnimal(SelectionAnimal selectionAnimal, List<OtolithFileInformation> fileinformation)
        {
            var sdSample = new SDSample();

            try
            {
               
                sdSample.sdSampleGuid = Guid.NewGuid();
                sdSample.sdEventId = _sdSampleVM.Event.sdEventId;

                if (selectionAnimal != null)
                {
                    sdSample.readOnly = true;
                    sdSample.animalId = selectionAnimal.AnimalId.ToString();
                    sdSample.cruise = selectionAnimal.Cruise;
                    sdSample.trip = selectionAnimal.Trip;
                    sdSample.station = selectionAnimal.Station;
                    sdSample.DFUArea = selectionAnimal.AreaCode;
                    sdSample.statisticalRectangle = selectionAnimal.StatisticalRectangle;
                    if (!string.IsNullOrWhiteSpace(selectionAnimal.Latitude))
                        sdSample.latitude = MapViewModel.ConvertPositionFromDegreesToDouble(selectionAnimal.Latitude ?? "00.00.000 N");

                    if (!string.IsNullOrWhiteSpace(selectionAnimal.Longitude))
                        sdSample.longitude = MapViewModel.ConvertPositionFromDegreesToDouble(selectionAnimal.Longitude ?? "00.00.000 E");

                    sdSample.sexCode = selectionAnimal.SexCode;
                    sdSample.edgeStructure = selectionAnimal.EdgeStructureCode;

                    if (!string.IsNullOrWhiteSpace(selectionAnimal.OtolithReadingRemarkCode) && OtolithReadingRemarks != null && OtolithReadingRemarks != null)
                        sdSample.L_OtolithReadingRemark = OtolithReadingRemarks.Where(x => x.otolithReadingRemark.Equals(selectionAnimal.OtolithReadingRemarkCode)).FirstOrDefault();

                    sdSample.fishLengthMM = selectionAnimal.LengthMM;
                    sdSample.fishWeightG = selectionAnimal.WeightG; //Convert to grams
                    sdSample.maturityIndexMethod = selectionAnimal.MaturityIndexMethod;

                    if (!string.IsNullOrWhiteSpace(selectionAnimal.MaturityIndexMethod) && selectionAnimal.MaturityIndex.HasValue && MaturityList != null && MaturityList != null)
                        sdSample.Maturity = MaturityList.Where(x => x.maturityIndex == selectionAnimal.MaturityIndex.Value && x.maturityIndexMethod.Equals(selectionAnimal.MaturityIndexMethod, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                    sdSample.comments = ((selectionAnimal.SpeciesListRemark ?? "") + " " + (selectionAnimal.AnimalRemark ?? "")).Trim();
                    sdSample.createdById = DFUPersonLogin == null ? null : new Nullable<int>(DFUPersonLogin.dfuPersonId);
                    sdSample.createdByUserName = User.UserName;
                    
                    sdSample.createdTime = DateTime.UtcNow;
                    sdSample.modifiedTime = sdSample.createdTime;
                    sdSample.catchDate = selectionAnimal.GearStartDate;
                    sdSample.ImportStatusEnum = Entities.SDSampleImportStatus.ImportedFromFishline;
                    sdSample.speciesCode = selectionAnimal.SpeciesCode;

                    if (selectionAnimal.StockId.HasValue && Stocks != null)
                        sdSample.L_Stock = Stocks.Where(x => x.L_stockId == selectionAnimal.StockId.Value).FirstOrDefault();

                    //Add files to the sample
                    var files = fileinformation.Where(x => x.AnimalId == selectionAnimal.AnimalId.ToString()).ToList();

                    if (files != null && files.Count > 0)
                    {
                        string part = files[0].FileName.Substring(0, files[0].FileName.IndexOf('.'));
                        var mappingStrings = part.Split('_');
                        if (mappingStrings.Length > 1 && PreparationMethods != null)
                            sdSample.L_SDPreparationMethod = PreparationMethods.Where(x => x.preparationMethod == mappingStrings[1]).FirstOrDefault();
                        if (mappingStrings.Length > 2 && LightTypes != null)
                            sdSample.L_SDLightType = LightTypes.Where(x => x.lightType == mappingStrings[2]).FirstOrDefault();
                        if (mappingStrings.Length > 3 && LightTypes != null)
                            sdSample.L_SDOtolithDescription = OtolithDescriptions.Where(x => x.otolithDescription == mappingStrings[3]).FirstOrDefault();

                        if (sdSample.SDFile == null)
                            sdSample.SDFile = new TrackableCollection<SDFile>();

                        foreach (var item in files)
                        {
                            sdSample.SDFile.Add(new SDFile()
                            {
                                sdFileGuid = Guid.NewGuid(),
                                fileName = item.FileName,
                                displayName = Path.GetFileName(item.FileName),
                                path = item.RelativeDirectoryPath
                            });
                        }
                    }
                }

                return sdSample;
            }
            catch (Exception e)
            {

                LogAndDispatchUnexpectedErrorMessage(e);
            }

            return sdSample;
        }


        #region Reset Messages Command


        public DelegateCommand ResetMessageCommand
        {
            get
            {
                if (_cmdResetMessages == null)
                    _cmdResetMessages = new DelegateCommand(() => ResetWarningErrorMessages());

                return _cmdResetMessages;
            }
        }


        public void ResetWarningErrorMessages()
        {
            try
            {
                var lstWarningsAndErrors = new List<SDMessageItem>();

                foreach (var item in FilteredSDSampleItems)
                {
                    if (item.WarningErrorMessages != null && item.WarningErrorMessages.Count > 0)
                        foreach (var msg in item.WarningErrorMessages)
                        {
                            lstWarningsAndErrors.Add(msg);
                        }
                }

                WarningAndErrorMessages = lstWarningsAndErrors.OrderBy(x => x.SampleItem.CSVRowNumber)
                                                              .ThenBy(x => x.Type).ToList();
            }
            catch (Exception e)
            {

                LogAndDispatchUnexpectedErrorMessage(e);
            }
        }


        #endregion




        private void SetAllCheckboxes()
        {
            if (FilteredSDSampleItems != null && FilteredSDSampleItems.Count > 0)
            {
                if (FilteredSDSampleItems.Count == FilteredSDSampleItems.Where(x => x.IsSelected).ToList().Count())
                    foreach (var item in FilteredSDSampleItems)
                    {
                        item.SetIsSelected(false);
                    }
                else if (FilteredSDSampleItems.Any(x => x.IsSelected))
                    foreach (var item in FilteredSDSampleItems)
                    {
                        item.SetIsSelected(false);
                    }
                else
                    foreach (var item in FilteredSDSampleItems)
                    {
                        item.SetIsSelected(true);
                    }
            }

            RaisePropertyChanged(() => FilteredSDSampleItems);
            RaiseIsAllSelected();
        }

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


        #region Save Command


        public DelegateCommand SaveCommand
        {
            get
            {
                if (_cmdSave == null)
                    _cmdSave = new DelegateCommand(() => SaveSamples());

                return _cmdSave;
            }
        }

        private void SaveSamples()
        {
            if (!HasSelectedSDSampleItems)
            {
                var res = AppRegionManager.ShowMessageBox(Translate("ImportFromCSVView", "WarningNoSelectedSamples"), System.Windows.MessageBoxButton.OK);
                return;
            }

            List<SDSampleItem> lst = null;

            if (FilteredSDSampleItems != null && FilteredSDSampleItems.Count > 0 && HasSelectedSDSampleItems)
                lst = FilteredSDSampleItems.Where(x => x.IsSelected && !x.HasErrors).ToList();
 
            if(_sdSampleVM != null && _sdSampleVM.Event != null && lst != null && lst.Count > 0)
            {
                //If yearly reading event and any samples are duplicates, through an error. 
                if (_sdSampleVM.Event.IsYearlyReadingEventType)
                {
                    if (lst.Where(x => x.IsAdded).Any())
                    {
                        DispatchMessageBox(Translate("SDSelectAnimalsView", "YearlyReadingAlreadyAddedError")); // "It is only allowed to add an animal once for yearly reading events. One or more of the selected animals (the semi-transparent ones), have already been added to the event. Please uncheck the already added ones and try again.");
                        return;
                    }
                    else if (lst.GroupBy(x => x.Sample.animalId).Where(x => x.Count() > 1).Any())
                    {
                        DispatchMessageBox(Translate("SDSelectAnimalsView", "YearlyReadingDuplicatesSelectedError"));
                        return;
                    }

                }
            }

            SelectedSDSampleItems = lst;

            Close();
        }


        #endregion


        #region Close Command
        public DelegateCommand CloseCommand
        {
            get
            {
                if (_cmdClose == null)
                    _cmdClose = new DelegateCommand(() => CloseThis());

                return _cmdClose;
            }
        }

        private void CloseThis()
        {
            if (HasSelectedSDSampleItems)
            {
                if (AppRegionManager.ShowMessageBox(Translate("ImportFromCSVView", "WarningHasSelectedSamples"), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                    return;
            }

            Close();
        }

        #endregion


        #region Sort Error Warning Messages Command


        public DelegateCommand<SDSampleItem> SortErrorWarningMessagesCommand
        {
            get
            {
                if (_cmdSortWarningErrorMessages == null)
                    _cmdSortWarningErrorMessages = new DelegateCommand<SDSampleItem>(param => SortErrorWarningMessages(param));

                return _cmdSortWarningErrorMessages;
            }
        }


        private void SortErrorWarningMessages(SDSampleItem sdSampleItem)
        {

            if (sdSampleItem != null && WarningAndErrorMessages != null && WarningAndErrorMessages.Count > 0)
            {
                if (sdSampleItem.HasErrorsOrWarnings)
                {
                    ResetWarningErrorMessages();
                    PushUIMessageParameter("ScrollToErrorWarning", WarningAndErrorMessages.Where(x => x.SampleItem == sdSampleItem)
                                                                                          .OrderBy(x => x.SampleItem.CSVRowNumber)
                                                                                          .ThenByDescending(x => x.Type)
                                                                                          .FirstOrDefault());
                }
                //OnUIMessageParameter
                    /*WarningAndErrorMessages = WarningAndErrorMessages.OrderByDescending(x => x.SampleItem.CSVRowNumber == sdSampleItem.CSVRowNumber)
                        .ThenBy(x => x.SampleItem.CSVRowNumber == sdSampleItem.CSVRowNumber && x.Type == MessageType.Duplicate)
                        .ThenBy(x => x.SampleItem.CSVRowNumber == sdSampleItem.CSVRowNumber && x.Type == MessageType.Warning)
                        .ThenBy(x => x.SampleItem.CSVRowNumber == sdSampleItem.CSVRowNumber && x.Type == MessageType.Error)
                        .ThenBy(y => y.SampleItem.CSVRowNumber)
                        .ThenBy(x => x.Type == MessageType.Duplicate)
                        .ThenBy(x => x.Type == MessageType.Warning)
                        .ThenBy(x => x.Type == MessageType.Error).ToList();*/
            }

        }
        #endregion
    }
}
