using FishLineMeasure.ViewModels.Overview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anchor.Core;
using Microsoft.Practices.Prism.Commands;
using System.Collections.ObjectModel;
using Babelfisk.Entities.Sprattus;
using Babelfisk.Entities;
using FishLineMeasure.BusinessLogic.Data;
using FishLineMeasure.BusinessLogic;
using System.ComponentModel;

namespace FishLineMeasure.ViewModels.Export
{
    public class FishLineExportViewModel : AViewModel
    {
        private DelegateCommand _cmdClose;
        private DelegateCommand _cmdExportData;

        private List<StationViewModel> _lstStations;

        private bool _dataAnalyzed = false;

        private List<ExportTuple> _lstExportData;


        private ObservableCollection<ExportLogEntryItem> _lstLogEntries;

        private List<L_Application> _lstApplications = null;


        #region Properties


        public ObservableCollection<ExportLogEntryItem> LogEntries
        {
            get { return _lstLogEntries; }
            set
            {
                _lstLogEntries = value;
                RaisePropertyChanged(nameof(LogEntries));
            }
        }


        public bool IsDataAnalyzed
        {
            get { return _dataAnalyzed; }
            set
            {
                _dataAnalyzed = value;
                RaisePropertyChanged(() => IsDataAnalyzed);
            }
        }



        #endregion



        public FishLineExportViewModel(List<StationViewModel> lstStations)
        {
            WindowTitle = "Eksporter stationer til FiskeLine";
            WindowWidth = 800;
            WindowHeight = 500;

            //Reset row number.
            ExportLogEntryItem.StaticIndex = 0;

            //Assign stations to export.
            _lstStations = lstStations;
            LogEntries = new ObservableCollection<ExportLogEntryItem>();
        }


        public Task AnalyzeDataAsync()
        {
            LoadingMessage = "Analyserer data, vent venligst...";
            IsLoading = true;
            IsDataAnalyzed = false;

            return Task.Factory.StartNew(AnalyzeData).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        class ExportTuple
        {
            public List<Infrastructure.MeasurementsClass> Measurements;
            public ExportLogEntryItem MainExportLog;
            public StationViewModel Station;
            public Sample ServerStation;
        }


        public List<T> GetLookups<T>()
        {
            LookupManager lm = new LookupManager();

            var lstLookups = lm.GetLookups(typeof(T)).OfType<T>().ToList();

            if (lstLookups == null)
                lstLookups = new List<T>();

            return lstLookups;
        }


        private static string cruiseHeader = "TOGT";
        private static string tripHeader = "TUR";
        private static string stationHeader = "STATION";
        private static string speicesListHeader = "LÆNGDEFORD.";
        private static string exportHeader = "EKSPORTERING";

        private void AnalyzeData()
        {
            try
            {
                _lstApplications = GetLookups<L_Application>();

                var datMan = new BusinessLogic.Data.DataManager();

                var s = _lstStations.First();

                int year = s.ParentTrip.Year;
                string cruise = s.ParentTrip.Cruise;
                string trip = s.ParentTrip.Trip;

                var cruiseLog = new ExportLogEntryItem(string.Format("År: {0}, navn: {1}.", year, cruise), cruiseHeader, ExportLogState.Pending) { ShowTopBorder = false };
                DispatchAddLogEntry(cruiseLog);

                var c = datMan.GetCruise(year, cruise);

                if(c == null)
                {
                    //TODO Cruise is not found in fish line.
                    new Action(() => { cruiseLog.State = ExportLogState.Failed; }).DispatchInvoke();
                    DispatchAddLogEntry(new ExportLogEntryItem(string.Format("Togtet eksisterer ikke i Fiskeline. Opret venligst togtet i Fiskeline først inden stationerne eksporteres.")) { ShowTopBorder = false });
                    return;
                }

                new Action(() => { cruiseLog.State = ExportLogState.Passed; }).DispatchInvoke();

                var lstServerStations = datMan.GetStationsForDataImport(c.cruiseId, trip, _lstStations.Select(x => x.StationNumber).ToList());

                var tripLog = new ExportLogEntryItem(string.Format("Tur nr: {0}.", trip), tripHeader, ExportLogState.Pending);
                DispatchAddLogEntry(tripLog);

                if (lstServerStations == null)
                {
                    new Action(() => { tripLog.State = ExportLogState.Failed; }).DispatchInvoke();
                    DispatchAddLogEntry(new ExportLogEntryItem(string.Format("Turen eksisterer ikke i Fiskeline. Opret venligst turen først inden stationerne eksporteres.")) { ShowTopBorder = false });
                    return;
                }

                new Action(() => { tripLog.State = ExportLogState.Passed; }).DispatchInvoke();


                List<ExportTuple> lstExportData = new List<ExportTuple>();
                for (int i = 0; i < _lstStations.Count; i++)
                {
                    ExportTuple et = CheckStation(_lstStations[i], lstServerStations);

                    if (et != null)
                        lstExportData.Add(et);
                }

                _lstExportData = lstExportData;

                new Action(() => 
                { 
                    IsDataAnalyzed = _lstExportData != null && _lstExportData.Count > 0;
                    LoadingMessage = "";
                }).DispatchInvoke();
            }
            catch(Exception e)
            {
                LogError(e);
                DispatchMessageBox("En uventet fejl opstod under analyseringen af Fiskeline data. Fejl: " + (e.Message ?? ""));
            }
        }


        private ExportTuple CheckStation(StationViewModel station, List<Sample> lstServerStations)
        {
            ExportTuple et = new ExportTuple();

            et.Station = station;
            et.ServerStation = lstServerStations.Where(x => x.station != null && x.station.Equals(et.Station.StationNumber, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            et.MainExportLog = new ExportLogEntryItem(string.Format("Stations nr: {0}.", et.Station.StationNumber), stationHeader, ExportLogState.Pending);
            DispatchAddLogEntry(et.MainExportLog);

            if (et.ServerStation == null)
            {
                new Action(() => { et.MainExportLog.State = ExportLogState.Failed; }).DispatchInvoke();
                DispatchAddLogEntry(new ExportLogEntryItem(string.Format("Stationen eksisterer ikke i Fiskeline. Opret venligst stationen først inden målingerne kan indsættes.")) { ShowTopBorder = false });
                return null;
            }

            et.Measurements = et.Station.GetMeasurementClasses(null, false);

            var lstMeasurementGroups = et.Measurements.Where(x => x.Lookups != null).GroupBy(x => x.Lookups.GroupString).ToList();

            ExportLogState checkState = ExportLogState.Passed;

            foreach (var g in lstMeasurementGroups)
            {
                var itm = g.FirstOrDefault();

                var speciesCode = itm.Lookups.GetLookupCode(LookupType.Species);
                var landingCatCode = itm.Lookups.GetLookupCode(LookupType.LandingCategory);
                var sizeSortEUCode = itm.Lookups.GetLookupCode(LookupType.SizeSortingEU);
                var sexCode = itm.Lookups.GetLookupCode(LookupType.Sex);
                var ovigorous = itm.Lookups.GetLookupCode(LookupType.Ovigorous);

                if(string.IsNullOrWhiteSpace(speciesCode))
                {
                    checkState = ExportLogState.Warning;
                    DispatchAddLogEntry(new ExportLogEntryItem(string.Format("En længdefordeling for stationen mangler artskode og vil derfor blive sprunget over under eksporteringen."), speicesListHeader, ExportLogState.Failed) { ShowTopBorder = false });
                    continue;
                }

                //Get species list, if any, for lookup combination.
                var slList = et.ServerStation.SpeciesList.Where(x => x.speciesCode != null && x.speciesCode.Equals(speciesCode, StringComparison.InvariantCultureIgnoreCase) &&
                                                                ((string.IsNullOrWhiteSpace(x.landingCategory) && string.IsNullOrWhiteSpace(landingCatCode)) || (!string.IsNullOrWhiteSpace(x.landingCategory) && x.landingCategory.Equals(landingCatCode, StringComparison.InvariantCultureIgnoreCase))) &&
                                                                ((!x.sizeSortingEU.HasValue && string.IsNullOrWhiteSpace(sizeSortEUCode)) || (x.sizeSortingEU.HasValue && x.sizeSortingEU.Value.ToString().Equals(sizeSortEUCode, StringComparison.InvariantCultureIgnoreCase))) &&
                                                                ((string.IsNullOrWhiteSpace(x.sexCode) && string.IsNullOrWhiteSpace(sexCode)) || (!string.IsNullOrWhiteSpace(x.sexCode) && x.sexCode.Equals(sexCode, StringComparison.InvariantCultureIgnoreCase))) &&
                                                                ((string.IsNullOrWhiteSpace(x.ovigorous) && string.IsNullOrWhiteSpace(ovigorous)) || (!string.IsNullOrWhiteSpace(x.ovigorous) && x.ovigorous.Equals(ovigorous, StringComparison.InvariantCultureIgnoreCase)))
                                                           ).ToList();

                SpeciesList sl = null;
                ExportLogEntryItem slLog = null;

                //Species list was not found, create new one
                if (slList.Count == 0)
                {
                    DispatchAddLogEntry((slLog = new ExportLogEntryItem(string.Format("Artslisterække mangler: \"{0}\". En ny artslisterække vil blive tilføjet.", itm.Lookups.GroupStringWithHeaders), speicesListHeader, ExportLogState.Info) { ShowTopBorder = false }));

                    sl = new SpeciesList();
                    sl.speciesCode = speciesCode;
                    sl.landingCategory = landingCatCode;

                    int sizeSortingEU = 0;
                    if (!string.IsNullOrWhiteSpace(sizeSortEUCode) && int.TryParse(sizeSortEUCode, out sizeSortingEU))
                        sl.sizeSortingEU = sizeSortingEU;

                    sl.sexCode = sexCode;
                    sl.ovigorous = ovigorous == null ? null : ovigorous.ToLower();

                    sl.treatment = "UR";
                    et.ServerStation.SpeciesList.Add(sl);
                    sl.OfflineState = ObjectState.Added;
                }
                //More than one species list was found, select one from list
                else if (slList.Count > 1)
                {
                    var vmSelect = new SelectSpeciesListViewModel(station.ParentTrip.Cruise, station.ParentTrip.Trip, station.StationNumber, speciesCode, landingCatCode, sexCode, sizeSortEUCode, _lstApplications, slList.ToArray());

                    new Action(() =>
                    {
                        AppRegionManager.LoadWindowViewFromViewModel(vmSelect, true, "WindowToolBox");
                    }).DispatchInvoke();

                    if (vmSelect.SelectedSpeciesList == null)
                    {
                        DispatchAddLogEntry(new ExportLogEntryItem(string.Format("Artslisterækken: \"{0}\" blev sprunget over og eksporteres ikke.", itm.Lookups.GroupStringWithHeaders), speicesListHeader, ExportLogState.Warning) { ShowTopBorder = false });
                        continue;
                    }

                    sl = vmSelect.SelectedSpeciesList;
                    DispatchAddLogEntry((slLog = new ExportLogEntryItem(string.Format("Artslisterækken: \"{0}\" blev valgt manuelt til: \"{1}\".", GetLookupsForDisplay(sl), itm.Lookups.GroupStringWithHeaders), speicesListHeader, ExportLogState.Info) { ShowTopBorder = false }));
                    //  checkState = ExportLogState.Warning;
                }
                else
                {
                    sl = slList.First();

                    DispatchAddLogEntry((slLog = new ExportLogEntryItem(string.Format("Artslisterække fundet: \"{0}\".", itm.Lookups.GroupStringWithHeaders), speicesListHeader, ExportLogState.Passed) { ShowTopBorder = false }));
                }

                //Only implement "LAV, rep" at the moment, single fish will come later.
                if ("LAV, rep" == "LAV, rep")
                {
                    var ss = GetSubSample(sl, SubSampleType.LAVRep);

                    //Create new SubSample if it does not exist already.
                    if (ss == null)
                    {
                        ss = new SubSample();
                        ss.representative = "ja";
                        ss.stepNum = 0;
                        ss.subSampleWeight = 0;
                        sl.SubSample.Add(ss);
                        sl.OfflineState = ObjectState.Modified;
                    }

                    if(HasAlreadyAddedLengths(ss) && slLog != null)
                    {
                        new Action(() =>
                        {
                            slLog.Message = slLog.Message + " OBS! Artslisterækken har allerede længder tilknyttet. Længderne i fordelingen vil derfor blive flettet med de eksisterende i Fiskeline under eksporteringen.";
                            slLog.State = ExportLogState.Warning;
                        }).DispatchInvoke();
                    }

                    var lengthGroups = g.GroupBy(x => x.LengthString);
                    foreach (var lg in lengthGroups)
                    {
                        var first = lg.First();
                        var length = Convert.ToInt32(first.Length.ConvertToUnit(first.Unit, BusinessLogic.Unit.MM).Truncate(0));

                        var a = ss.Animal.Where(x => x.length.HasValue && x.length.Value == length).FirstOrDefault();
                        if (a == null)
                        {
                            a = new Animal();
                            a.number = 0;
                            a.individNum = null;
                            //Also add sex code to animals.
                            if (!string.IsNullOrWhiteSpace(sexCode))
                                a.sexCode = sexCode;
                            a.lengthMeasureUnit = "MM"; //Hardcode to CM for FishLine display.
                            a.length = new Nullable<int>(length);
                        }
                            
                        a.number += lg.Count();

                        ss.Animal.Add(a);
                    }
                }
                else
                {
                    var ss = GetSubSample(sl, SubSampleType.SFRep);

                    //Create new SubSample if it does not exist already.
                    if (ss == null)
                    {
                        ss = new SubSample();
                        ss.representative = "ja";
                        ss.stepNum = 0;
                        ss.subSampleWeight = 0;
                        sl.SubSample.Add(ss);
                    }

                    if (HasAlreadyAddedLengths(ss) && slLog != null)
                    {
                        new Action(() =>
                        {
                            slLog.Message = slLog.Message + " OBS! Artslisterækken har allerede længder tilknyttet.";
                            slLog.State = ExportLogState.Warning;
                        }).DispatchInvoke();
                    }

                    var maxIndividNum = 0;

                    if (ss.Animal.Where(x => x.individNum.HasValue).Any())
                        maxIndividNum = ss.Animal.Where(x => x.individNum.HasValue).Max(a => a.individNum.Value);

                    foreach (var length in g)
                    {

                        Animal a = new Animal();
                        a.number = 1;
                        a.individNum = ++maxIndividNum;
                        a.lengthMeasureUnit = "MM"; //Hardcode to CM for FishLine display.
                        a.length = new Nullable<int>(Convert.ToInt32(length.Length.ConvertToUnit(length.Unit, BusinessLogic.Unit.MM).Truncate(0)));

                        ss.Animal.Add(a);
                    }
                }
            }

            new Action(() => { et.MainExportLog.State = checkState; }).DispatchInvoke();

            return et;
        }


        private bool HasAlreadyAddedLengths(SubSample ss)
        {
            if (ss.Animal.Count > 0)
                return true;

            return false;
        }


        private string GetLookupsForDisplay(SpeciesList sl)
        {
            List<string> lstRes = new List<string>();

            var speciesHeader = Lookups.LookupsViewModel.GetLookupDisplayNameShort(typeof(L_Species));
            var landingCatHeader = Lookups.LookupsViewModel.GetLookupDisplayNameShort(typeof(L_LandingCategory));
            var sizeSortingEUHeader = Lookups.LookupsViewModel.GetLookupDisplayNameShort(typeof(L_SizeSortingEU));
            var sizeSortingDFUHeader = Lookups.LookupsViewModel.GetLookupDisplayNameShort(typeof(L_SizeSortingDFU));
            var sexHeader = Lookups.LookupsViewModel.GetLookupDisplayNameShort(typeof(L_SexCode));
            var ovirogousHeader = "Rogn";
            var applicationHeader = Lookups.LookupsViewModel.GetLookupDisplayNameShort(typeof(L_Application));

            if (!string.IsNullOrWhiteSpace(sl.speciesCode))
                lstRes.Add(string.Format("{0}: {1}", speciesHeader, sl.speciesCode));

            if (!string.IsNullOrWhiteSpace(sl.landingCategory))
                lstRes.Add(string.Format("{0}: {1}", landingCatHeader, sl.landingCategory));

            if (sl.sizeSortingEU.HasValue)
                lstRes.Add(string.Format("{0}: {1}", sizeSortingEUHeader, sl.sizeSortingEU.Value));

            if (!string.IsNullOrWhiteSpace(sl.sizeSortingDFU))
                lstRes.Add(string.Format("{0}: {1}", sizeSortingDFUHeader, sl.sizeSortingDFU));

            if (!string.IsNullOrWhiteSpace(sl.sexCode))
                lstRes.Add(string.Format("{0}: {1}", sexHeader, sl.sexCode));

            if (!string.IsNullOrWhiteSpace(sl.ovigorous))
                lstRes.Add(string.Format("{0}: {1}", ovirogousHeader, sl.ovigorous));

            if (sl.applicationId.HasValue)
            {
                var lApp = _lstApplications.Where(x => x.L_applicationId == sl.applicationId.Value).FirstOrDefault();

                if(lApp != null)
                    lstRes.Add(string.Format("{0}: {1} - {2}", sizeSortingEUHeader, lApp.code, lApp.description));
            }

            return string.Join(", ", lstRes);
        }
     

        /// <summary>
        /// GEt sumsample to hang lengths on. It can either be:
        /// Rep: 
        /// SubSample step 0, 1 or 2. 
        /// Not rep:
        /// SubSample step 0
        /// </summary>
        /// <param name="sl"></param>
        /// <param name="ssType"></param>
        /// <returns></returns>
        private SubSample GetSubSample(SpeciesList sl, SubSampleType ssType)
        {
            if (sl.SubSample == null || sl.SubSample.Count == 0)
                return null;

            SubSample sub = null;
            switch (ssType)
            {
                case SubSampleType.LAVRep:
                case SubSampleType.SFRep:
                    sub = sl.SubSample.OrderByDescending(x => x.stepNum).Where(x => x.IsRepresentative && (x.subSampleWeight.HasValue || x.landingWeight.HasValue || (x.sumAnimalWeights.HasValue && x.sumAnimalWeights.Value))).FirstOrDefault();
                    break;

                case SubSampleType.SFNotRep:
                    var q = sl.SubSample.Where(x => !x.IsRepresentative && (x.subSampleWeight.HasValue || (x.sumAnimalWeights.HasValue && x.sumAnimalWeights.Value)));

                    if (q.Count() == 1)
                        sub = q.FirstOrDefault();
                    break;
            }

            return sub;
        }


        private void DispatchAddLogEntry(ExportLogEntryItem log)
        {
            new Action(() =>
            {
                LogEntries.Add(log);
            }).DispatchInvoke();
        }



        #region Export command


        public DelegateCommand ExportCommand
        {
            get
            {
                return _cmdExportData ?? (_cmdExportData = new DelegateCommand(() => ExportDataAsync()));
            }
        }


        private void ExportDataAsync()
        {
            if(_lstExportData == null || _lstExportData.Count == 0)
            {
                AppRegionManager.ShowMessageBox("Ingen af de valgte stationer kan eksporteres lige nu.");
                return;
            }

            if (AppRegionManager.ShowMessageBox(string.Format("Er du sikker på du vil eksportere {0} / {1} station(er) til Fiskeline (LAV, rep)?", _lstExportData.Count, _lstStations.Count), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;

            LoadingMessage = "Exporterer data, vent venligst";
            IsLoading = true;

            Task.Factory.StartNew(ExportData).ContinueWith(t => new Action(() => { IsLoading = false; }).Dispatch());
        }

        
        private void ExportData()
        {
            try
            {
                if (_lstExportData == null || _lstExportData.Count == 0)
                {
                    DispatchMessageBox("Som resultat af analysen, er der ikke nogen stationer som kan eksporteres til Fiskeline.");
                    return;
                }

                var trips = _lstExportData.Select(x => x.ServerStation.Trip).DistinctBy(x => x.tripId).ToList();

                var dMan = new DataManager();
                foreach(var t in trips)
                {
                    var trip = t;
                    var res = dMan.SynchronizeTrip(ref trip);

                    if (res.DatabaseOperationResult.DatabaseOperationStatus == DatabaseOperationStatus.Successful)
                    {
                        DispatchAddLogEntry(new ExportLogEntryItem(string.Format("Turen '{0}' blev eksporteret korrekt.", trip.trip1), exportHeader, ExportLogState.Passed));

                        var stations = _lstExportData.Where(x => x.ServerStation.Trip == t).ToList();
                        var stationsString = string.Join(", ", stations.Select(x => x.Station.StationNumber));

                        DispatchAddLogEntry(new ExportLogEntryItem(string.Format("Følgende stationer blev eksporteret: {0}.", stationsString), exportHeader, ExportLogState.None) { ShowTopBorder = false });
                    }
                    else
                        DispatchAddLogEntry(new ExportLogEntryItem(string.Format("Eksportering af turen '{0}' fejlede. Fejlbesked: {1}", trip.trip1, res.DatabaseOperationResult.Message ?? "N/A"), exportHeader, ExportLogState.Failed));
                }

                IsDataAnalyzed = false;
            }
            catch (Exception e)
            {
                LogError(e);
                DispatchMessageBox("En uventet fejl opstod under exportingen til Fiskeline. Fejl: " + (e.Message ?? ""));
            }
        }

        #endregion



        #region Close command


        public DelegateCommand CloseCommand
        {
            get
            {
                if (_cmdClose == null)
                    _cmdClose = new DelegateCommand(() => CloseView());

                return _cmdClose;
            }
        }


        private void CloseView()
        {
            Close();
        }

        #endregion


        public override void FireClosing(object sender, CancelEventArgs e)
        {
            base.FireClosing(sender, e);

            if (IsLoading)
            {
                DispatchMessageBox("Vent venligst med at lukke formen, indtil den er færdig med at arbejde.");
                e.Cancel = true;
            }
        }
    }
}
