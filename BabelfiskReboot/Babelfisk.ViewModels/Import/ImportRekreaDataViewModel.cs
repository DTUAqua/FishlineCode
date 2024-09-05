using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using Anchor.Core;
using System.Collections.ObjectModel;
using Babelfisk.Warehouse;
using Babelfisk.Entities.Sprattus;
using System.Globalization;
using Babelfisk.BusinessLogic;
using System.Xml.Linq;

namespace Babelfisk.ViewModels.Import
{
    public class ImportRekreaDataViewModel : AViewModel
    {
        private DelegateCommand _cmdImport;
        private DelegateCommand _cmdAnalyze;
        private DelegateCommand _cmdCancel;
        private DelegateCommand<string> _cmdBrowse;
        private DelegateCommand _cmdClearMessages;
        private DelegateCommand _cmdStopImporting;

        private string _strTripFile;
        private string _strCatchFile;
        private string _strRespFile;

        Dictionary<string, Cruise> _dicCruises = new Dictionary<string, Cruise>();

        private CultureInfo _dateParsingCulture = new CultureInfo("da-DK");

        private List<DFUPerson> _lstLookupsDFUPerson;
        private List<L_Species> _lstLookupsSpecies;
        private List<L_LandingCategory> _lstLandingCategories;
        private List<L_Treatment> _lstTreatment;
        private List<L_AgeMeasureMethod> _lstAgeMeasureMethod;
        private List<L_DFUArea> _lstDFUAreas;
        private List<L_LengthMeasureType> _lstLengthMeasureType;
        private List<L_LengthMeasureUnit> _lstLengthMeasureUnit;
        private List<L_Application> _lstApplication;
        private List<L_SampleType> _lstSampleType;

        private ImportRekreaMappingContainer _mappingContainer;

        private bool _blnIsAnalyzed = false;
        private volatile bool _blnIsImporting = false;

        private ObservableCollection<DWMessage> _lstMessages = new ObservableCollection<DWMessage>();




        #region Properties


        public bool IsImporting
        {
            get { return _blnIsImporting; }
            set
            {
                _blnIsImporting = value;
                RaisePropertyChanged(() => IsImporting);
            }
        }


        public ObservableCollection<DWMessage> Messages
        {
            get { return _lstMessages; }
            set
            {
                _lstMessages = value;
                RaisePropertyChanged(() => Messages);
            }
        }


        public bool IsAnalyzed
        {
            get { return _blnIsAnalyzed; }
            set
            {
                _blnIsAnalyzed = value;
                RaisePropertyChanged(() => IsAnalyzed);
            }
        }


        public string TripFilePath
        {
            get { return _strTripFile; }
            set
            {
                _strTripFile = value;
                RaisePropertyChanged(() => TripFilePath);
            }
        }


        public string CatchFilePath
        {
            get { return _strCatchFile; }
            set
            {
                _strCatchFile = value;
                RaisePropertyChanged(() => CatchFilePath);
            }
        }


        public string RespFilePath
        {
            get { return _strRespFile; }
            set
            {
                _strRespFile = value;
                RaisePropertyChanged(() => RespFilePath);
            }
        }


        #endregion


        public ImportRekreaDataViewModel()
        {
            WindowWidth = 780;
            WindowHeight = 600;
            WindowTitle = "Importer rekrea-data fra CSV";


        }


        /// <summary>
        /// Load all required lookup tables.
        /// </summary>
        private bool LoadLookups()
        {
            try
            {
                _lstLookupsDFUPerson = GetLookups<DFUPerson>();
                _lstLookupsSpecies = GetLookups<L_Species>();
                _lstLandingCategories = GetLookups<L_LandingCategory>();
                _lstTreatment = GetLookups<L_Treatment>();
                _lstAgeMeasureMethod = GetLookups<L_AgeMeasureMethod>();
                _lstDFUAreas = GetLookups<L_DFUArea>();
                _lstLengthMeasureType = GetLookups<L_LengthMeasureType>();
                _lstLengthMeasureUnit = GetLookups<L_LengthMeasureUnit>();
                _lstApplication = GetLookups<L_Application>();
                _lstSampleType = GetLookups<L_SampleType>();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                AddErrorMessage("Henter lookup tabeller.", "Lookups", string.Format("En eller flere lookup-tabeller kunne ikke hentes."));
                return false;
            }

            if (_lstLookupsDFUPerson == null || _lstLookupsDFUPerson.Count == 0 ||
                _lstLookupsSpecies == null || _lstLookupsSpecies.Count == 0 ||
                _lstLandingCategories == null || _lstLandingCategories.Count == 0 ||
                _lstTreatment == null || _lstTreatment.Count == 0 ||
                _lstAgeMeasureMethod == null || _lstAgeMeasureMethod.Count == 0 ||
                _lstDFUAreas == null || _lstDFUAreas.Count == 0 ||
                _lstLengthMeasureType == null || _lstLengthMeasureType.Count == 0 ||
                _lstLengthMeasureUnit == null || _lstLengthMeasureUnit.Count == 0 ||
                _lstApplication == null || _lstApplication.Count == 0 ||
                _lstSampleType == null || _lstSampleType.Count == 0)
            {
                AddErrorMessage("Henter lookup tabeller.", "Lookups", string.Format("En eller flere lookup-tabeller kunne ikke hentes."));
                return false;
            }

            return true;
        }



        private Cruise CreateAndGetCruise(int year, string cruiseName)
        {
            string key = year.ToString() + cruiseName;

            Cruise c = null;

            if (_dicCruises.ContainsKey(key))
                c = _dicCruises[key];
            else
            {
                var datMan = new BusinessLogic.DataInput.DataInputManager();
                c = datMan.CreateAndGetCruise(year, cruiseName);
                _dicCruises.Add(key, c);
            }

            return c;
        }


        public List<T> GetLookups<T>()
        {
            LookupManager lm = new LookupManager();

            var lstLookups = lm.GetLookups(typeof(T)).OfType<T>().ToList();

            if (lstLookups == null)
                lstLookups = new List<T>();

            return lstLookups;
        }


        private void AddErrorMessage(string origin, string recordType, string message)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("Error! Origin: {0}, RecordType: {1}, Message: {2}", origin, recordType, message));
            new Action(() =>
            {
                var dw = DWMessage.NewError(null, origin, recordType, null, message);
                dw.Index = _lstMessages.Count + 1;
                _lstMessages.Add(dw);

                ScrollTo("MessagesEnd");
            }).DispatchInvoke();
        }


        private void AddWarningMessage(string origin, string recordType, string message)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("Warning! Origin: {0}, RecordType: {1}, Message: {2}", origin, recordType, message));
            new Action(() =>
            {
                var dw = DWMessage.NewWarning(null, origin, recordType, null, message);
                dw.Index = _lstMessages.Count + 1;
                _lstMessages.Add(dw);

                ScrollTo("MessagesEnd");
            }).DispatchInvoke();
        }


        private Dictionary<int, string> GetHeaderAndDetermineSeperator(string header, out char seperator)
        {
            seperator = ',';
            Dictionary<int, string> headerIndexDictionary = new Dictionary<int, string>();

            var arrHeader = header.Split(seperator);

            if (arrHeader == null || arrHeader.Length < 10)
            {
                seperator = ';';
                arrHeader = header.Split(seperator);
            }

            for(int i = 0; i < arrHeader.Length; i++)
                headerIndexDictionary.Add(i, arrHeader[i] == null ? "" : arrHeader[i].Replace("\"", ""));

            return headerIndexDictionary;
        }



        #region Analyze command


        public DelegateCommand AnalyzeCommand
        {
            get { return _cmdAnalyze ?? (_cmdAnalyze = new DelegateCommand(AnalyzeAsync)); }
        }


        private void AnalyzeAsync()
        {
            if (string.IsNullOrWhiteSpace(_strTripFile) && string.IsNullOrWhiteSpace(_strCatchFile) && string.IsNullOrWhiteSpace(_strRespFile))
            {
                DispatchMessageBox("Vælg venligst en tur-, fangst- eller respondant-fil at importere.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(_strTripFile) && !File.Exists(_strTripFile))
            {
                DispatchMessageBox("Den angivne tur-fil kunne ikke lokaliseres. Rediger venligst stien prøv igen.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(_strCatchFile) && !File.Exists(_strCatchFile))
            {
                DispatchMessageBox("Den angivne fangst-fil kunne ikke lokaliseres. Rediger venligst stien prøv igen.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(_strRespFile) && !File.Exists(_strRespFile))
            {
                DispatchMessageBox("Den angivne respondant-fil kunne ikke lokaliseres. Rediger venligst stien prøv igen.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(_strTripFile) && string.IsNullOrWhiteSpace(_strCatchFile))
            {
                DispatchMessageBox("Vælg venligst den tilhørende fangst-fil til den valgte tur-fil.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_strTripFile) && !string.IsNullOrWhiteSpace(_strCatchFile))
            {
                DispatchMessageBox("Vælg venligst den tilhørende tur-fil til den valgte fangst-fil.");
                return;
            }

            IsAnalyzed = false;
            LoadingMessage = "Analyserer, vent venligst...";

            //Reset global variables.
            _dicCruises = new Dictionary<string, Cruise>();

            //Clear old messages.
            _lstMessages.Clear();

            _lstCatches = null;

            IsLoading = true;
            Task.Factory.StartNew(Analyze).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        private List<Trip> _lstCatches = null;

        private void Analyze()
        {
            //Step 1) Load needed lookups.
            if (!LoadLookups())
                return;

            //Step 2) Get mappings.
            //TODO: Get from service.
            _mappingContainer = new ImportRekreaMappingContainer(null);

            List<Trip> lstTrips = null;

            //Step 3) Parse trip data.
            if ((lstTrips = AnalyzeTripData()) == null)
                return;

            //Step 4) Parse sample data.
            if ((_lstCatches = AnalyzeCatchData(lstTrips)) == null)
                return;

            IsAnalyzed = true;
        }


        private string RemoveQuotes(string value, bool trim = true)
        {
            if (value == null)
                return value;

            var val = value.Replace("\"", "");

            if (trim)
                val = val.Trim();

            return val;
        }


        #region Trip data methods

        private List<Trip> AnalyzeTripData()
        {
            List<Trip> lst = new List<Trip>();

            if (string.IsNullOrWhiteSpace(_strTripFile) || !File.Exists(_strTripFile))
                return null;

            new Action(() =>
            {
                LoadingMessage = "Analyserer tur-fil, vent venligst...";
            }).DispatchInvoke();

            char seperator = ',';

            int lineNo = 1;
            using (FileStream fs = new FileStream(_strTripFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    var header = sr.ReadLine();

                    var dicIndexHeader = GetHeaderAndDetermineSeperator(header, out seperator);

                    if (dicIndexHeader == null || dicIndexHeader.Count == 0)
                    {
                        AddErrorMessage("Fortolkning af turdata.", "Tur", string.Format("Det var ikke muligt at fortolke første linje af turfilen \"{0}\". Der blev ikke fundet nogen kommaseparerede kolonner.", _strTripFile));
                        return null;
                    }

                    //Parse file
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lineNo++;
                        var arr = line.Split(seperator);

                        if (arr == null)
                        {
                            AddWarningMessage("Fortolkning af turdata.", "Tur", string.Format("Linje {0}. Der blev ikke fundet nogen kolonner, linjen ignoreres. Data: {1}", lineNo, line ?? ""));
                            continue;
                        }

                        if (arr.Length != dicIndexHeader.Count)
                        {
                            AddWarningMessage("Fortolkning af turdata.", "Tur", string.Format("Linje {0}. Antallet af kolonner ({1}) stemmer ikke overens med antallet af headere ({2}), linjen ignoreres.", lineNo, arr.Length, dicIndexHeader.Count));
                            continue;
                        }

                        var t = GetTripData(lineNo, arr, dicIndexHeader);

                        if (t != null)
                            lst.Add(t);
                    }
                }
            }

            return lst;
        }


        private Trip GetTripData(int lineNo, string[] arr, Dictionary<int, string> dicMap)
        {
            Trip t = new Trip();
            string cruiseName = null;
            int iVal = 0;
            string sVal = null;
            string qq = null;
            DateTime dt;

            Dictionary<string, string> dicSampleData = new Dictionary<string, string>();

            for (int i = 0; i < arr.Length; i++)
            {
                if (!dicMap.ContainsKey(i)) //Mapping was not found
                    continue;

                string header = dicMap[i].ToLower();

                switch (header)
                {
                    case "sgtrid":
                        sVal = RemoveQuotes(arr[i]);
                        t.sgTripId = sVal;
                        break;

                    case "casestudy":
                        cruiseName = _mappingContainer.Map(RemoveQuotes(arr[i]), ImportRekreaMappingContainer.RekreaMappingType.CruiseName);
                      
                        if(!string.IsNullOrWhiteSpace(cruiseName))
                            dicSampleData.Add("Cruise", cruiseName);
                        break;

                    case "tripid":
                        sVal = RemoveQuotes(arr[i]);
                        t.trip1 = sVal;
                        break;

                    case "tripnum":
                        sVal = RemoveQuotes(arr[i]);
                        t.tripNum = sVal;
                        break;

                    case "triptype":
                        qq = RemoveQuotes(arr[i]);
                        sVal = _mappingContainer.Map(qq, ImportRekreaMappingContainer.RekreaMappingType.L_TripTypeCode);

                        if (sVal != null)
                            t.tripType = sVal;

                        break;

                    case "datestart":
                        if (DateTime.TryParse(RemoveQuotes(arr[i]), _dateParsingCulture, DateTimeStyles.None, out dt))
                        {
                            t.timeZone = 0; //UTC
                            t.dateStart = dt;
                        }
                        break;

                    case "year":
                        sVal = RemoveQuotes(arr[i]);

                        if(!string.IsNullOrWhiteSpace(sVal))
                            dicSampleData.Add("Year", sVal);
                        break;

                    case "quarterstart":
                        sVal = RemoveQuotes(arr[i]);

                        if (!string.IsNullOrWhiteSpace(sVal))
                            dicSampleData.Add("QuarterStart", sVal);
                        break;

                    case "quartergearstart":
                        dicSampleData.Add("QuarterGearStart", RemoveQuotes(arr[i]));
                        break;

                    case "weekdayweekend":
                        //By kolonne på Sample
                        dicSampleData.Add("WeekdayWeekend", RemoveQuotes(arr[i]));
                        break;

                    case "dfuarea":
                        sVal = RemoveQuotes(arr[i]);
                        if (!string.IsNullOrWhiteSpace(sVal))
                        {
                            var sLookup = _lstDFUAreas.Where(x => x.DFUArea.Equals(sVal, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                            if (sLookup != null)
                                dicSampleData.Add("DFUArea", sLookup.DFUArea);
                            else
                            {
                                AddErrorMessage("Fortolkning af turdata.", "Tur", string.Format("Linje {0}{1}. Det var ikke muligt at mappe dfuArea \"{2}\" til en i Fiskeline, linjen ignoreres. Sørg for at området findes under 'Kodelister -> Aqua Områder', inden REKREA data importeres.", lineNo, string.IsNullOrWhiteSpace(t.trip1) ? "" : (", Turid " + t.trip1), sVal));
                                return null;
                            }
                        }
                        
                        break;

                    case "placename":
                        sVal = RemoveQuotes(arr[i]);
                        t.placeName = sVal;
                        break;

                    case "placecode":
                        sVal = RemoveQuotes(arr[i]);
                        t.placeCode = sVal;
                        break;

                    case "postalcode":
                        sVal = RemoveQuotes(arr[i]);
                        if (sVal != null && sVal.TryParseInt32(out iVal))
                            t.postalCode = iVal;
                        else if (!string.IsNullOrWhiteSpace(sVal))
                            AddWarningMessage("Fortolkning af turdata.", "Tur", string.Format("Linje {0}. Det var ikke muligt at konvertere postalCode \"{1}\" til heltal.", lineNo, sVal));
                        
                        break;

                    case "numberinplace":
                        sVal = RemoveQuotes(arr[i]);
                        if (sVal != null && sVal.TryParseInt32(out iVal))
                            t.numberInPlace = iVal;
                        else if (!string.IsNullOrWhiteSpace(sVal))
                            AddWarningMessage("Fortolkning af turdata.", "Tur", string.Format("Linje {0}. Det var ikke muligt at konvertere numberInPlace \"{1}\" til heltal.", lineNo, sVal));
                        break;

                    case "respyes":
                        sVal = RemoveQuotes(arr[i]);
                        if (sVal != null && sVal.TryParseInt32(out iVal))
                            t.respYes = iVal;
                        else if (!string.IsNullOrWhiteSpace(sVal))
                            AddWarningMessage("Fortolkning af turdata.", "Tur", string.Format("Linje {0}. Det var ikke muligt at konvertere respYes \"{1}\" til heltal.", lineNo, sVal));
                        break;

                    case "respno":
                        sVal = RemoveQuotes(arr[i]);
                        if (sVal != null && sVal.TryParseInt32(out iVal))
                            t.respNo = iVal;
                        else if (!string.IsNullOrWhiteSpace(sVal))
                            AddWarningMessage("Fortolkning af turdata.", "Tur", string.Format("Linje {0}. Det var ikke muligt at konvertere respNo \"{1}\" til heltal.", lineNo, sVal));
                        break;

                    case "resptot":
                        sVal = RemoveQuotes(arr[i]);
                        if (sVal != null && sVal.TryParseInt32(out iVal))
                            t.respTot = iVal;
                        else if(!string.IsNullOrWhiteSpace(sVal))
                            AddWarningMessage("Fortolkning af turdata.", "Tur", string.Format("Linje {0}. Det var ikke muligt at konvertere respTot \"{1}\" til heltal.", lineNo, sVal));
                        break;

                    case "interviewperson":
                        qq = RemoveQuotes(arr[i]);
                        sVal = _mappingContainer.Map(qq, ImportRekreaMappingContainer.RekreaMappingType.DFUPersonInitials);

                        if (sVal != null)
                            qq = sVal;

                        var dfuPerson = _lstLookupsDFUPerson.Where(x => x.initials != null && x.initials.Equals(qq, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                        if (dfuPerson == null)
                        {
                            AddErrorMessage("Fortolkning af turdata.", "Tur", string.Format("Linje {0}{1}. Det var ikke muligt at finde interview-personen med initialer \"{2}\" i Fiskeline, linjen ignoreres. Sørg for at personen findes under 'Kodelister -> Personer (aqua)', inden REKREA data importeres.", lineNo, string.IsNullOrWhiteSpace(t.trip1) ? "" : (", Turid " + t.trip1), qq));
                            return null;
                        }
                        else
                            dicSampleData.Add("InterviewPersonId", dfuPerson.dfuPersonId.ToString());

                        break;

                    default:
                        AddWarningMessage("Fortolkning af turdata.", "Tur", string.Format("Linje {0}{1}. Kolonnen \"{2}\" er ukendt og ignoreres.", lineNo, string.IsNullOrWhiteSpace(t.trip1) ? "" : (", Turid " + t.trip1), dicMap[i]));
                        break;
                }
            }


            if(string.IsNullOrWhiteSpace(t.trip1))
            {
                AddErrorMessage("Fortolkning af turdata.", "Tur", string.Format("Linje {0}{1}. tripId er ikke angivet, linjen ignoreres.", lineNo, string.IsNullOrWhiteSpace(t.trip1) ? "" : (", Turid " + t.trip1)));
                return null;
            }

            if (string.IsNullOrWhiteSpace(t.tripType))
            {
                AddErrorMessage("Fortolkning af turdata.", "Tur", string.Format("Linje {0}{1}. Tur-type er ikke angivet, linjen ignoreres.", lineNo, string.IsNullOrWhiteSpace(t.trip1) ? "" : (", Turid " + t.trip1)));
                return null;
            }

            if (!dicSampleData.ContainsKey("Cruise"))
            {
                AddErrorMessage("Fortolkning af turdata.", "Tur", string.Format("Linje {0}{1}. Casestudy kunne ikke mappes til et togtnavn, linjen ignoreres.", lineNo, string.IsNullOrWhiteSpace(t.trip1) ? "" : (", Turid " + t.trip1)));
                return null;
            }

            if (!dicSampleData.ContainsKey("Year"))
            {
                AddErrorMessage("Fortolkning af turdata.", "Tur", string.Format("Linje {0}{1}. Årstal mangler, linjen ignoreres.", lineNo, string.IsNullOrWhiteSpace(t.trip1) ? "" : (", Turid " + t.trip1)));
                return null;
            }

            t.Tag = dicSampleData;
            return t;
        }


        #endregion


        #region Catch data methods

        private List<Trip> AnalyzeCatchData(List<Trip> lstTripData)
        {
            List<Trip> lst = new List<Trip>();

            if (string.IsNullOrWhiteSpace(_strCatchFile) || !File.Exists(_strCatchFile))
                return null;

            new Action(() =>
            {
                LoadingMessage = "Analyserer fangst-fil, vent venligst...";
            }).DispatchInvoke();

            char seperator = ',';

            int lineNo = 1;
            using (FileStream fs = new FileStream(_strCatchFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    var header = sr.ReadLine();

                    var dicIndexHeader = GetHeaderAndDetermineSeperator(header, out seperator);

                    if (dicIndexHeader == null || dicIndexHeader.Count == 0)
                    {
                        AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Det var ikke muligt at fortolke første linje af fangstfilen \"{0}\". Der blev ikke fundet nogen kommaseparerede kolonner.", _strTripFile));
                        return null;
                    }

                    List<Sample> lstSamples = new List<Sample>();
                    Trip tCur = null;
                    Sample sCur = null;
                    SpeciesList slCur = null;
                    SubSample ssCur = null;
                    DateTime dt;
                    int i;

                    List<RekreaCatchData> lstCatchRows = new List<RekreaCatchData>();

                    //Parse file
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lineNo++;
                        var arr = line.Split(seperator);

                        if (arr == null)
                        {
                            AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Der blev ikke fundet nogen kolonner, linjen ignoreres. Data: {1}", lineNo, line ?? ""));
                            continue;
                        }

                        if (arr.Length != dicIndexHeader.Count)
                        {
                            AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Antallet af kolonner ({1}) stemmer ikke overens med antallet af headere ({2}), linjen ignoreres.", lineNo, arr.Length, dicIndexHeader.Count));
                            continue;
                        }

                        var rowData = GetCatchRowInformation(lineNo, arr, dicIndexHeader);

                        //If row data is NULL it means a warning has been added and the row needs to be skipped.
                        if (rowData == null)
                            continue;

                        if (string.IsNullOrWhiteSpace(rowData.TripId))
                        {
                            AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Linjen indeholder intet tripId, linjen ignoreres. Data: {1}", lineNo, line ?? ""));
                            continue;
                        }

                        var t = lstTripData.Where(x => x.trip1 != null && x.trip1.Equals(rowData.TripId, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                        if (t == null)
                        {
                            AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Det var ikke muligt at finde en række med matchende turid \"{1}\" i turfilen, linjen ignoreres. Dette kan enten skyldes at turen mangler i tur-filen, eller at linjen for turen i turfilen, indeholder fejl der gør at den ignoreres.", lineNo, rowData.TripId));
                            continue;
                        }

                        Dictionary<string, string> dicTripDetails = t.Tag as Dictionary<string, string>;
                        if (dicTripDetails == null)
                        {
                            AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Den fundne turrække for fangst-rækken indeholder ikke tilstrækkelig togtinformation, linjen ignoreres.", lineNo));
                            continue;
                        }

                        lstCatchRows.Add(rowData);
                    }

                    foreach (var t in lstTripData)
                    {
                        //tridId cannot be null, since this is checked during parsing of the trip earlier.
                        var tripId = t.trip1;

                        var gTrip = lstCatchRows.Where(x => x.TripId != null && x.TripId.Equals(tripId, StringComparison.InvariantCultureIgnoreCase)).ToList();

                        Dictionary<string, string> dicTripDetails = t.Tag as Dictionary<string, string>;
                        var cruiseName = dicTripDetails["Cruise"];
                        int year = 0;

                        if (!int.TryParse(dicTripDetails["Year"], out year))
                        {
                            AddErrorMessage("Fortolkning af turdata.", "Tur", string.Format("Linje {0}{1}. Turrækken indeholder ikke et korrekt togt år, linjen ignoreres.", lineNo, string.IsNullOrWhiteSpace(t.trip1) ? "" : (", Turid " + t.trip1)));
                            continue;
                        }

                        //Return cruise id from year and cruise name. If the cruise does not exist, this methods creates it. The methods also handles caching.
                        var cruise = CreateAndGetCruise(year, cruiseName);

                        //Update trip cruise id (if this is the first time).
                        if (t.cruiseId != cruise.cruiseId)
                        {
                            t.Cruise = cruise;
                        }

                        //Loop over samples
                        foreach (var gSample in gTrip.GroupBy(x => new { Station = x.RespNum, sgId = x.SgId }))
                        {
                            var sampleStation = gSample.Key.Station.ToString();
                            var sampleSgId = gSample.Key.sgId;

                            var s = GetSample(lstSamples, tripId, sampleStation, sampleSgId);

                            if (s == null)
                            {
                                var sd = gSample.First();

                                s = new Sample();
                                s.@virtual = "nej";

                               var st = _lstSampleType.Where(x => x.sampleType.Equals(sd.SampleTypeCode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                                if (st != null)
                                    s.sampleType = st.sampleType;

                                s.Trip = t;

                                s.sgId = sd.SgId;

                                //DateGearStart must exist, or the row would have been filtered away during parsing earlier.
                                s.dateGearStart = sd.DateGearStart.Value;
                                s.dateGearEnd = sd.DateGearStart.Value;
                                s.timeZone = 0; //UTC?
                                
                                if (sd.DateGearEnd.HasValue)
                                    s.dateGearEnd = sd.DateGearEnd.Value;

                                if(dicTripDetails.ContainsKey("DFUArea"))
                                    s.dfuArea = dicTripDetails["DFUArea"];

                                if (dicTripDetails.ContainsKey("InterviewPersonId") && int.TryParse(dicTripDetails["InterviewPersonId"], out i))
                                    s.samplePersonId = i;

                                if (dicTripDetails.ContainsKey("WeekdayWeekend"))
                                    s.weekdayWeekend = dicTripDetails["WeekdayWeekend"];

                                s.station = sampleStation;
                                lstSamples.Add(s);
                            }
                           
                            //Loop over SpeciesList and individual fish.
                            foreach (var cd in gSample)
                            {
                                //TODO: What should happen with rows with no species code (they should probably be ignored)
                                //If number = 0, there species list row should be ignored (not inserted)
                                if (string.IsNullOrWhiteSpace(cd.SpeciesCode) || !cd.Number.HasValue || (cd.Number.HasValue && cd.Number.Value == 0))
                                    continue;

                                var sl = s.SpeciesList.Where(x => x.speciesCode.Equals(cd.SpeciesCode, StringComparison.InvariantCultureIgnoreCase) && x.landingCategory == cd.LandingCategory && x.applicationId == cd.ApplicationId).FirstOrDefault();

                                bool blnNew = false;
                                //If specieslist entry is not there or the row has length undefined, then create a species list row for each missing length
                                if (sl == null /*|| cd.Length == null*/ /*&& !sl.SubSample.Where(x => x.stepNum == cd.StepNum.Value && x.representative == cd.Representative && !x.Animal.Any()).Any())*/)
                                {
                                    sl = new SpeciesList();
                                    sl.speciesCode = cd.SpeciesCode;
                                    sl.landingCategory = cd.LandingCategory;
                                    sl.treatment = cd.Treatment;
                                    sl.applicationId = cd.ApplicationId; //TODO: sl.Code == cd.Code
                                    s.SpeciesList.Add(sl);
                                    sl.OfflineState = ObjectState.Added;
                                    blnNew = true;

                                }

                                //Add to SpeciesList entity only, if length is not defined.
                                if (cd.Length == null)
                                {
                                    if (!blnNew)
                                    {
                                        if (cd.Number.HasValue && cd.Number.Value > 0)
                                        {
                                            int number = 0;

                                            if (sl.number.HasValue)
                                                number = sl.number.Value;

                                            number += cd.Number.Value;
                                            sl.number = number;
                                        }
                                        //TODO: This would overwrite an earlier row, so throw error?
                                        continue;
                                    }

                                    SubSample ss = new SubSample();
                                    ss.stepNum = cd.StepNum.Value;
                                    ss.representative = cd.Representative;
                                    ss.subSampleWeight = cd.Weight;
                                    sl.number = cd.Number;
                                    sl.SubSample.Add(ss);
                                }
                                else //Add to SpecieslList and Animal entities, if lengths are defined.
                                {
                                    var ss = sl.SubSample.Where(x => x.stepNum == cd.StepNum && x.representative == cd.Representative).FirstOrDefault();

                                    if (ss == null)
                                    {
                                        ss = new SubSample();
                                        ss.stepNum = cd.StepNum.Value;
                                        ss.representative = cd.Representative;
                                        sl.SubSample.Add(ss);
                                    }

                                    var maxIndividNum = 0;

                                    if (ss.Animal.Where(x => x.individNum.HasValue).Any())
                                        maxIndividNum = ss.Animal.Where(x => x.individNum.HasValue).Max(a => a.individNum.Value);

                                    //Create individiual animal items.
                                    for (int j = 0; j < cd.Number; j++)
                                    {
                                        decimal? weight = (cd.Weight.HasValue && cd.Number > 1) ? cd.Weight.Value / cd.Number : cd.Weight;

                                        Animal a = new Animal();
                                        a.length = cd.Length.HasValue ? new Nullable<int>(Input.AnimalItem.Convert(cd.Length.Value, cd.LengthMeasureUnit, "MM")) : null;
                                        a.weight = weight;
                                        a.individNum = ++maxIndividNum;
                                        a.lengthMeasureTypeId = cd.LengthMeasureTypeId;
                                        a.lengthMeasureUnit = cd.LengthMeasureUnit;
                                        a.catchNum = cd.CatchNum;
                                        a.otolithFinScale = cd.OtolithFinAndScale;
                                        a.number = 1;

                                        if (cd.Age.HasValue)
                                        {
                                            Age age = new Age();
                                            age.age1 = cd.Age.Value;
                                            age.ageMeasureMethodId = cd.AgeMeasureMethodId.Value;
                                            age.number = 1;
                                            a.Age.Add(age);
                                        }

                                        ss.Animal.Add(a);
                                    }
                                }

                            }

                            //Loop over each specieslist and calculate total number, if Animals exist on the species list.
                            foreach (var sl in s.SpeciesList)
                            {
                                int number = -1;
                               
                                foreach(var ss in sl.SubSample)
                                {
                                    decimal weight = -1;
                                    var animalsNumbers = ss.Animal.Where(x => x.number > 0);
                                    var animalsWeights = ss.Animal.Where(x => x.weight.HasValue);

                                    if (animalsNumbers.Any())
                                    {
                                        if (number == -1)
                                        {
                                            if (sl.number.HasValue)
                                                number = sl.number.Value;
                                            else
                                                number = 0;

                                            //Reset sl number, so sl.number is assigned further down, with updated value.
                                            if(number > 0)
                                                sl.number = null;
                                        }

                                        number += animalsNumbers.Select(x => x.number).Sum();
                                    }

                                    if (animalsWeights.Any())
                                    {
                                        if (weight == -1)
                                            weight = 0;

                                        //Only set subsampleweight if it has not already been set.
                                        if (!ss.subSampleWeight.HasValue)
                                            ss.subSampleWeight = animalsWeights.Select(x => x.weight.Value).Sum();
                                    }

                                    //Set the weight to 0, if the contained animal records do not have a weight
                                    if (!ss.subSampleWeight.HasValue && ss.Animal.Any())
                                        ss.subSampleWeight = 0;
                                }

                                //Only set number, if it has not already been set
                                if(!sl.number.HasValue)
                                    sl.number = number != -1 ? new Nullable<int>(number) : null;
                                
                            }
                        }

                        lst.Add(t);
                    }
                }
            }

            return lst;
        }


        private Sample GetSample(List<Sample> lstSamples, string tripId, string station, string sampleSgId)
        {
            var sample = (from s in lstSamples
                         // where s.Trip != null
                        //  let dicMap = s.Trip.Tag as Dictionary<string, string>
                          where s.Trip != null && s.Trip.trip1 != null && 
                                s.Trip.trip1.Equals(tripId, StringComparison.InvariantCultureIgnoreCase) &&
                                s.station != null && s.station.Equals(station, StringComparison.InvariantCultureIgnoreCase) &&
                                s.sgId != null && s.sgId.Equals(sampleSgId, StringComparison.InvariantCultureIgnoreCase)
                          select s).FirstOrDefault();

            return sample;
        }


        private class RekreaCatchData
        {
            public string SgId
            {
                get;
                set;
            }


            public string TripId
            {
                get;
                set;
            }


            /// <summary>
            /// Station
            /// </summary>
            public int? RespNum
            {
                get;
                set;
            }


            public int? CatchNum
            {
                get;
                set;
            }


            public string SpeciesCode
            {
                get;
                set;
            }


            public string catchCategory
            {
                get;
                set;
            }

            public string LandingCategory
            {
                get;
                set;
            }


            public int? Number
            {
                get;
                set;
            }


            /// <summary>
            /// CM
            /// </summary>
            public int? Length
            {
                get;
                set;
            }


            /// <summary>
            /// KG
            /// </summary>
            public decimal? Weight
            {
                get;
                set;
            }


            public bool? OtolithFinAndScale
            {
                get;
                set;
            }

            public int? Age
            {
                get;
                set;
            }

            public int? StepNum
            {
                get;
                set;
            }


            public string Representative
            {
                get;
                set;
            }


            public string Treatment
            {
                get;
                set;
            }


            public int? AgeMeasureMethodId
            {
                get;
                set;
            }


            public DateTime? DateGearStart
            {
                get;
                set;
            }


            public DateTime? DateGearEnd
            {
                get;
                set;
            }


            public string LengthMeasureUnit
            {
                get;
                set;
            }

            public int? LengthMeasureTypeId
            {
                get;
                set;
            }

            public int? ApplicationId
            {
                get;
                set;
            }

            
            public string SampleTypeCode
            {
                get;
                set;
            }
        }


        private RekreaCatchData GetCatchRowInformation(int lineNo, string[] arr, Dictionary<int, string> dicMap)
        {
            RekreaCatchData reData = new RekreaCatchData();

            int iVal = 0;
            double dVal = 0;
            decimal deVal = 0;
            string sVal = null;
            DateTime dt;
            reData.LengthMeasureUnit = "CM"; //Default to centimeters

            for (int i = 0; i < arr.Length; i++)
            {
                if (!dicMap.ContainsKey(i)) //Mapping was not found
                    continue;

                string header = dicMap[i].ToLower();

                switch (header)
                {
                    case "sgid":
                        reData.SgId = RemoveQuotes(arr[i]);
                        break;

                    case "tripid":
                        reData.TripId = (RemoveQuotes(arr[i]) ?? "").Trim();
                        break;

                    case "respnum":
                        if (int.TryParse(RemoveQuotes(arr[i]), out iVal))
                            reData.RespNum = iVal;
                        else
                        {
                            AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Værdien for kolonnen \"{1}\" kunne ikke konverteres til et heltal, linjen ignoreres.", lineNo, dicMap[i]));
                            return null;
                        }
                        break;

                    case "catchnum":
                        if (int.TryParse(RemoveQuotes(arr[i]), out iVal))
                            reData.CatchNum = iVal;
                        else
                        {
                            AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Værdien for kolonnen \"{1}\" kunne ikke konverteres til et heltal, linjen ignoreres.", lineNo, dicMap[i]));
                            return null;
                        }
                        break;

                    case "speciescode":
                        var sl = (RemoveQuotes(arr[i]) ?? "").Trim();
                        if(!string.IsNullOrWhiteSpace(sl))
                        {
                            var sLookup = _lstLookupsSpecies.Where(x => x.speciesCode.Equals(sl, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                            if (sLookup != null)
                                reData.SpeciesCode = sLookup.speciesCode;
                            else
                            {
                                AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Det var ikke muligt at mappe artskoden \"{1}\" til en i Fiskeline, linjen ignoreres.", lineNo, sl));
                                return null;
                            }
                        }
                        break;

                    case "catchcategory":
                        reData.catchCategory = RemoveQuotes(arr[i]);
                        break;

                    case "landingcategory":
                        var lc = RemoveQuotes(arr[i]);
                        if (!string.IsNullOrWhiteSpace(lc))
                        {
                            var sLookup = _lstLandingCategories.Where(x => x.landingCategory.Equals(lc, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                            if (sLookup != null)
                                reData.LandingCategory = sLookup.landingCategory;
                            else
                            {
                                AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Det var ikke muligt at mappe landingCategory \"{1}\" til en i Fiskeline, linjen ignoreres.", lineNo, lc));
                                return null;
                            }
                        }
                        break;

                    case "number":
                        if (int.TryParse(RemoveQuotes(arr[i]), out iVal))
                            reData.Number = iVal;
                        break;

                    case "length": 
                        if (int.TryParse(RemoveQuotes(arr[i]) ?? "", out iVal))
                            reData.Length = iVal;
                        break;

                    case "weight":
                        if ((RemoveQuotes(arr[i]) ?? "").TryParseDecimal(out deVal))
                            reData.Weight = deVal;
                        break;

                    case "otolith": //For Laks
                        sVal = RemoveQuotes(arr[i]);

                        if(!string.IsNullOrWhiteSpace(sVal))
                        {
                            if (sVal.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
                                reData.OtolithFinAndScale = true;
                            else if (sVal.Equals("N", StringComparison.InvariantCultureIgnoreCase))
                                reData.OtolithFinAndScale = false;
                        }
                        break;

                    case "finAndScale": //For cod
                        sVal = RemoveQuotes(arr[i]);

                        if (!string.IsNullOrWhiteSpace(sVal))
                        {
                            if (sVal.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
                                reData.OtolithFinAndScale = true;
                            else if (sVal.Equals("N", StringComparison.InvariantCultureIgnoreCase))
                                reData.OtolithFinAndScale = false;
                        }
                        break;

                    case "age":
                        if (int.TryParse(RemoveQuotes(arr[i]) ?? "", out iVal))
                            reData.Age = iVal;
                        //dic.Add("age", RemoveQuotes(arr[i]));
                        break;

                    case "stepnum":
                        if (int.TryParse(RemoveQuotes(arr[i]), out iVal))
                            reData.StepNum = iVal;
                        break;

                    case "representative":
                        sVal = (RemoveQuotes(arr[i]) ?? "").ToLower();

                        if(sVal != "nej" && sVal != "ja")
                        {
                            AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Kolonnen \"representative\" skal indeholde \"ja\" eller \"nej\". Kolonnen indholdte \"{1}\", linjen ignoreres.", lineNo, sVal));
                            return null;
                        }

                        reData.Representative = sVal;
                        break;

                    case "treatment":
                        sVal = RemoveQuotes(arr[i]);
                        if (!string.IsNullOrWhiteSpace(sVal))
                        {
                            var sLookup = _lstTreatment.Where(x => x.treatment.Equals(sVal, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                            if (sLookup != null)
                                reData.Treatment = sLookup.treatment;
                            else
                            {
                                AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Det var ikke muligt at mappe treatment \"{1}\" til en i Fiskeline, linjen ignoreres.", lineNo, sVal));
                                return null;
                            }
                        }
                        break;

                    case "agemeasuremethod":
                        sVal = RemoveQuotes(arr[i]);
                        if (!string.IsNullOrWhiteSpace(sVal))
                        {
                            var sLookup = _lstAgeMeasureMethod.Where(x => x.ageMeasureMethod.Equals(sVal, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                            if (sLookup != null)
                                reData.AgeMeasureMethodId = sLookup.L_ageMeasureMethodId;
                            else
                            {
                                AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Det var ikke muligt at mappe ageMeasureMethod \"{1}\" til en i Fiskeline, linjen ignoreres.", lineNo, sVal));
                                return null;
                            }
                        }
                        break;

                    case "dategearstart":
                        sVal = RemoveQuotes(arr[i]);
                        if (sVal != null && DateTime.TryParse(sVal, _dateParsingCulture, DateTimeStyles.None, out dt))
                            reData.DateGearStart = dt;
                        else
                        {
                            AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Det var ikke muligt at fortolke dateGearStart med værdien \"{1}\". linjen ignoreres.", lineNo, sVal ?? ""));
                            return null;
                        }
                        break;

                    case "dategearend":
                        sVal = RemoveQuotes(arr[i]);
                        if (sVal != null && DateTime.TryParse(sVal, _dateParsingCulture, DateTimeStyles.None, out dt))
                            reData.DateGearEnd = dt;
                        else if(!string.IsNullOrWhiteSpace(sVal))
                        {
                            AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Det var ikke muligt at fortolke dateGearEnd med værdien \"{1}\". linjen ignoreres.", lineNo, sVal ?? ""));
                            return null;
                        }
                        break;

                    case "lengthmeasureunit":
                        sVal = RemoveQuotes(arr[i]);
                        if (!string.IsNullOrWhiteSpace(sVal))
                        {
                            var sLookup = _lstLengthMeasureUnit.Where(x => x.lengthMeasureUnit.Equals(sVal, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                            if (sLookup != null)
                                reData.LengthMeasureUnit = sLookup.lengthMeasureUnit;
                            else
                            {
                                AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Det var ikke muligt at mappe lengthMeasureUnit \"{1}\" til en i Fiskeline, linjen ignoreres.", lineNo, sVal));
                                return null;
                            }
                        }
                        break;

                    case "lengthmeasuretype":
                        sVal = RemoveQuotes(arr[i]);
                        if (!string.IsNullOrWhiteSpace(sVal))
                        {
                            var sLookup = _lstLengthMeasureType.Where(x => x.lengthMeasureType.Equals(sVal, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                            if (sLookup != null)
                                reData.LengthMeasureTypeId = sLookup.L_lengthMeasureTypeId;
                            else
                            {
                                AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Det var ikke muligt at mappe lengthMeasureType \"{1}\" til en i Fiskeline, linjen ignoreres.", lineNo, sVal));
                                return null;
                            }
                        }
                        break;

                    case "code":
                    case "application":
                        sVal = RemoveQuotes(arr[i]);
                        if (!string.IsNullOrWhiteSpace(sVal))
                        {
                            var sLookup = _lstApplication.Where(x => x.code.Equals(sVal, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                            if (sLookup != null)
                                reData.ApplicationId = sLookup.L_applicationId;
                            else
                            {
                                AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Det var ikke muligt at mappe \"{1}\" til en L_Application kode i Fiskeline, linjen ignoreres.", lineNo, sVal));
                                return null;
                            }
                        }
                        break;

                    case "dfuarea": //ignore
                        break;

                    case "sampletype":
                        sVal = RemoveQuotes(arr[i]);
                        if (!string.IsNullOrWhiteSpace(sVal))
                        {
                            var sLookup = _lstSampleType.Where(x => x.sampleType.Equals(sVal, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                            if (sLookup != null)
                                reData.SampleTypeCode = sLookup.sampleType;
                            else
                            {
                                AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Det var ikke muligt at mappe \"{1}\" til en L_SampleType (redskabsgruppe) i Fiskeline, linjen ignoreres.", lineNo, sVal));
                                return null;
                            }
                        }
                        break;

                    default:
                        AddWarningMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Kolonnen \"{1}\" er ukendt og ignoreres.", lineNo, dicMap[i]));
                        break;
                }
            }

            //RespNum is mandatory
            if (!reData.RespNum.HasValue)
            {
                AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Respnum er obligatorisk, men ikke angivet. Linjen ignoreres.", lineNo, sVal));
                return null;
            }

            //DateGearStart is mandatory
            if (!reData.DateGearStart.HasValue)
            {
                AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. DateGearStart er obligatorisk, men ikke angivet. Linjen ignoreres.", lineNo, sVal));
                return null;
            }

            //CatchNum is mandatory
            if (!reData.CatchNum.HasValue)
            {
                AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. CatchNum er obligatorisk, men ikke angivet. Linjen ignoreres.", lineNo, sVal));
                return null;
            }

            //Stepnum is mandatory
            if (!reData.StepNum.HasValue)
            {
                AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. StepNum er obligatorisk, men ikke angivet. Linjen ignoreres.", lineNo, sVal));
                return null;
            }

            //Representative is mandatory
            if (string.IsNullOrWhiteSpace(reData.Representative))
            { 
                AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Representative er obligatorisk, men ikke angivet. Linjen ignoreres.", lineNo, sVal));
                return null;
            }

            //Treatment is mandatory
            if (string.IsNullOrWhiteSpace(reData.Treatment))
            {
                AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. Treatment er obligatorisk, men ikke angivet. Linjen ignoreres.", lineNo, sVal));
                return null;
            }

            //AgeMeasureMethodId is mandatory if the row has an age
            if(reData.Age.HasValue && !reData.AgeMeasureMethodId.HasValue)
            {
                AddErrorMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. AgeMeasureMethod er ikke angivet, men obligatorisk for rækker der har en alder angivet. Linjen ignoreres.", lineNo, sVal));
                return null;
            }

            if (string.IsNullOrWhiteSpace(reData.SampleTypeCode))
            {
                reData.SampleTypeCode = "X";
                AddWarningMessage("Fortolkning af fangstdata.", "Fangst", string.Format("Linje {0}. SampleType er ikke angivet, koden 'X' bruges i stedet for.", lineNo, sVal));
            }

            return reData;
        }


        #endregion


        #endregion


        #region Import command


        public DelegateCommand ImportCommand
        {
            get { return _cmdImport ?? (_cmdImport = new DelegateCommand(ImportAsync)); }
        }


        private void ImportAsync()
        {
            if (!IsAnalyzed)
            {
                DispatchMessageBox("Analyser venligst data først, inden der importeres.");
                return;
            }

            if(_lstCatches == null || _lstCatches.Count == 0)
            {
                DispatchMessageBox("Der blev ikke fundet nogen fangster at importere, se venligst \"Fejl og advarsler\"-listen.");
                return;
            }

            LoadingMessage = "Importerer data, vent venligst...";
            IsLoading = true;
            IsImporting = true;

            Task.Factory.StartNew(() => Import(_lstCatches.ToList())).ContinueWith(t => new Action(() =>
            {
                IsLoading = false;
                IsImporting = false;
            }).Dispatch());
        }


        private void Import(List<Trip> lstCatches)
        {
            BusinessLogic.Offline.OfflineManager oMan = new BusinessLogic.Offline.OfflineManager();

            int successfulImports = 0;
            for(int i = 0; i < lstCatches.Count; i++)
            {
                if (!IsImporting)
                    break;

                var t = lstCatches[i];

                try
                {
                    LoadingMessage = string.Format("Importerer {0}/{1} tur(e), vent venligst...", i+1, lstCatches.Count);
                    var res = oMan.SynchronizeTrip(ref t);

                    if (res.DatabaseOperationResult.DatabaseOperationStatus != Entities.DatabaseOperationStatus.Successful)
                    {
                        if(res.DatabaseOperationResult.DatabaseOperationStatus == Entities.DatabaseOperationStatus.DuplicateRecordException && res.DatabaseOperationResult.Message == "DuplicateKey")
                            AddWarningMessage("Import af tur", string.Format("Tur: {0}", t.trip1), string.Format("Turen ({0} -> {1}) eksisterer allerede og ignoreres.", t.Cruise.year, t.trip1 ));
                        else
                            AddErrorMessage("Import af tur", string.Format("Tur: {0}", t.trip1), string.Format("En uventet fejl opstod under importering af turen ({0} -> {1}). {2}.", t.Cruise.year, t.trip1, res.DatabaseOperationResult.Message + (!string.IsNullOrWhiteSpace(res.DatabaseOperationResult.UIMessage) ? ", " + res.DatabaseOperationResult.UIMessage : "")));
                    }
                    else
                    {
                        successfulImports++;
                    }
                }
                catch(Exception e)
                {
                    AddErrorMessage("Import af tur", string.Format("Tur: {0}", t.trip1), string.Format("En uventet fejl opstod under importering af turen. Fejl: {0}.", e.Message));
                }
            }

            DispatchMessageBox(string.Format("{0}/{1} tur(e) blev importeret korrekt.", successfulImports, lstCatches.Count));

            //Refresh tree on successful imports.
            if(successfulImports > 0)
                Windows.MainWindowViewModel.TreeView.RefreshTreeAsync();
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


        #region Browse command


        public DelegateCommand<string> BrowseCommand
        {
            get { return _cmdBrowse ?? (_cmdBrowse = new DelegateCommand<string>(s => Browse(s))); }
        }


        private void Browse(string browseType)
        {
            var dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Title = "Vælg venligst en CSV-fil.";
            dlg.Filter = "CSV files (*.csv)|*.csv";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;

            var strFile = dlg.FileName;

            switch(browseType)
            {
                case "Trip":
                    TripFilePath = strFile;
                    break;

                case "Catch":
                    CatchFilePath = strFile;
                    break;

                case "Respondent":
                    RespFilePath = strFile;
                    break;
            }
        }

        #endregion


        #region Clear Message command


        public DelegateCommand ClearMessagesCommand
        {
            get { return _cmdClearMessages ?? (_cmdClearMessages = new DelegateCommand(ClearMessages)); }
        }


        private void ClearMessages()
        {
            Messages.Clear();
        }


        #endregion


        #region Stop importing command


        public DelegateCommand StopImportingCommand
        {
            get { return _cmdStopImporting ?? (_cmdStopImporting = new DelegateCommand(StopImporting)); }
        }


        private void StopImporting()
        {
            IsImporting = false;
        }


        #endregion


        

    }

}
