using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Babelfisk.Entities;
using Anchor.Core;
using Microsoft.Practices.Prism.Commands;
using Babelfisk.Entities.Sprattus;
using Babelfisk.BusinessLogic;

namespace Babelfisk.ViewModels.Offline
{
    public class CompareEntitiesViewModel : AViewModel 
    {
        private DelegateCommand _cmdServerWins;
        private DelegateCommand _cmdClientWins;
        private DelegateCommand _cmdStopSync;

        private OfflineEntity _oeServer;

        private OfflineEntity _oeClient;

        private string _strHeader;

        private string _strDescription;

        private Type _compareType;

        private List<CompareItem> _lstCompareItems;

        private bool _blnRepeatForFutureConflicts;

        private bool _blnChoiceMade = false;

        private bool _blnStopSync = false;

        private bool _blnTripTypeDifferent = false;

        private bool _blnIsWinsEnabled = true;


        private OverwritingMethod _overwritingMethod;


        #region Properties


        public OverwritingMethod SelectedOverwritingMethod
        {
            get { return _overwritingMethod; }
            set
            {
                _overwritingMethod = value;
                RaisePropertyChanged(() => SelectedOverwritingMethod);
            }
        }


        public List<CompareItem> CompareItems
        {
            get { return _lstCompareItems; }
            set
            {
                _lstCompareItems = value;
                RaisePropertyChanged(() => CompareItems);
            }
        }



        public bool RepeatForFutureConflicts
        {
            get { return _blnRepeatForFutureConflicts; }
            set
            {
                _blnRepeatForFutureConflicts = value;
                RaisePropertyChanged(() => RepeatForFutureConflicts);
            }
        }


        public bool IsSyncCancelled
        {
            get { return _blnStopSync; }
        }
        


        public string Header
        {
            get { return _strHeader; }
        }


        public string Description
        {
            get { return _strDescription; }
        }


        public string GetEntityTranslatedName
        {
            get
            {
                string strTypeName = CompareType.Name;

                switch (strTypeName)
                {
                    case "Cruise":
                        return "togt";

                    case "Trip":
                        return "tur";

                    case "Sample":
                        return "station";

                    default:
                        return strTypeName;
                }
            }
        }


        public string GetEntityTranslatedNamePlural
        {
            get
            {
                string strTypeName = CompareType.Name;

                switch (strTypeName)
                {
                    case "Cruise":
                        return "Togtet";

                    case "Trip":
                        return "Turen";

                    case "Sample":
                        return "Stationen";

                    default:
                        return strTypeName;
                }
            }
        }


        public string GetEntityTranslatedNamePlural2
        {
            get
            {
                string strTypeName = CompareType.Name;

                switch (strTypeName)
                {
                    case "Cruise":
                        return "Togter";

                    case "Trip":
                        return "Ture";

                    case "Sample":
                        return "Stationer";

                    default:
                        return strTypeName;
                }
            }
        }




        public string EntityImageType
        {
            get
            {
                string str = "Cruise";

                if (CompareType.Name == "Trip")
                {
                    if ((_oeClient as Trip).IsHVN)
                        str = "HVN";
                    else if ((_oeClient as Trip).IsSEA)
                        str = "SØS";
                    else
                        str = "VID";
                }
                else if (CompareType.Name == "Sample")
                    str = "Sample";

                return str;
            }
        }


        public bool IsTripTypeDifferent
        {
            get { return _blnTripTypeDifferent; }
            set
            {
                _blnTripTypeDifferent = value;
                RaisePropertyChanged(() => IsTripTypeDifferent);
            }
        }


        public Type CompareType
        {
            get { return _compareType; }
        }


        public bool IsWinsEnabled
        {
            get { return _blnIsWinsEnabled; }
            set
            {
                _blnIsWinsEnabled = value;
                RaisePropertyChanged(() => IsWinsEnabled);
            }
        }


        #endregion


        public CompareEntitiesViewModel(string strHeader, OfflineEntity oeServer, OfflineEntity oeClient)
        {
            _compareType = oeClient.GetType();
            _blnRepeatForFutureConflicts = false;

            WindowTitle = String.Format("{0} {1} konflikter med {2} {1} i databasen på land.", (CompareType.Name == "Cruise") ? "Et" : "En", GetEntityTranslatedName, (CompareType.Name == "Cruise") ? "et" : "en");


            WindowWidth = 800;
            WindowHeight = 450;

            if (_compareType.Name == "Sample")
            {
                _strDescription = String.Format("Stationen eksisterer allerede i databasen på land. Stop venligst synkroniseringen og angiv et anden stationsnummer til den nye station. Se forskellene mellem de to stationer nedenfor.", GetEntityTranslatedNamePlural, GetEntityTranslatedNamePlural.ToLower(), CompareType.Name == "Cruise" ? "det" : "den", GetEntityTranslatedNamePlural2.ToLower());
                //Comment out below statement to enable Client/Server wins buttons in UI (this is to be done on unexpected errors when data backups are debugged)
                IsWinsEnabled = false;
            }
            else
                _strDescription = String.Format("{0} eksisterer allerede i databasen på land. Vælg om du vil overskrive {1} i databasen, eller beholde {2} som {2} er. Se forskellene mellem de to {3} nedenfor.", GetEntityTranslatedNamePlural, GetEntityTranslatedNamePlural.ToLower(), CompareType.Name == "Cruise" ? "det" : "den", GetEntityTranslatedNamePlural2.ToLower());

            _strHeader = strHeader;
            _overwritingMethod = OverwritingMethod.ServerWins;
            _oeServer = oeServer;
            _oeClient = oeClient;

            InitializeAsync();
        }

        private void InitializeAsync()
        {
            IsLoading = true;
            Task.Factory.StartNew(Initialize).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        private void Initialize()
        {
            var lst = new List<CompareItem>();

            string strTypeName = CompareType.Name;

            switch (strTypeName)
            {
                case "Cruise":
                    CompareCruise(ref lst);
                    break;

                case "Trip":
                    if ((_oeClient as Trip).tripType != (_oeServer as Trip).tripType)
                    {
                        lst.Add(new CompareItem("Tur type", (_oeServer as Trip).tripType, (_oeClient as Trip).tripType));
                        lst.Add(new CompareItem("Tur nr.", (_oeServer as Trip).trip1, (_oeClient as Trip).trip1));
                        _blnTripTypeDifferent = true;
                        break;
                    }

                    if ((_oeClient as Trip).IsHVN)
                        CompareTripHVN(ref lst);
                    else
                        CompareTrip(ref lst);
                    break;

                case "Sample":
                    CompareSample(ref lst);
                    break;

                default:
                    
                    break;
            }

            /* Test data
            for (int i = 0; i < 10; i++)
                lst.Add(new CompareItem("testing " + i.ToString(), "value " + i.ToString(), "value " + i.ToString()));

            lst.Add(new CompareItem("testing ", "value dif", "value differ"));

            for (int i = 9; i < 20; i++)
                lst.Add(new CompareItem("testing " + i.ToString(), "value " + i.ToString(), "value " + i.ToString()));
            */

            new Action(() =>
            {
                CompareItems = lst;
                RaisePropertyChanged(() => IsTripTypeDifferent);
            }).Dispatch();
        }


        private void CompareCruise(ref List<CompareItem> lst)
        {
            Cruise server = _oeServer as Cruise;
            Cruise client = _oeClient as Cruise;

            var lm = new LookupManager();
            var lstDFUPersons = lm.GetLookups(typeof(DFUPerson)).OfType<DFUPerson>().ToList();

            lst.Add(new CompareItem("Togt år", server.year.ToString(), client.year.ToString()));
            lst.Add(new CompareItem("Togt titel", server.cruise1, client.cruise1));
            lst.Add(new CompareItem("Togt beskrivelse", (server.cruiseTitle ?? "") + (string.IsNullOrWhiteSpace(server.cruiseTitle) ? "" : " ") + server.summary, (client.cruiseTitle ?? "") + (string.IsNullOrWhiteSpace(client.cruiseTitle) ? "" : " ") + client.summary));
            lst.Add(new CompareItem("Projektleder", server.responsibleId.HasValue ? GetLookupValue(lstDFUPersons, p => p.dfuPersonId == server.responsibleId.Value ) : null, client.responsibleId.HasValue ? GetLookupValue(lstDFUPersons, p => p.dfuPersonId == client.responsibleId ) : null));
            lst.Add(new CompareItem("Deltager(e)", server.participants, client.participants));
            lst.Add(new CompareItem("Bemærkninger", server.remark, client.remark));
        }


        private void CompareTrip(ref List<CompareItem> lst)
        {
            Trip server = _oeServer as Trip;
            Trip client = _oeClient as Trip;

            var lm = new LookupManager();
            var lstDFUPersons = lm.GetLookups(typeof(DFUPerson)).OfType<DFUPerson>().ToList();
            var lstSamplingMethods = lm.GetLookups(typeof(L_SamplingMethod)).OfType<L_SamplingMethod>().ToList();
            var lstPersons = lm.GetLookups(typeof(Person)).OfType<Person>().ToList();

            lst.Add(new CompareItem("Tur nr.", server.trip1, client.trip1));
            if (client.IsSEA)
            {
                lst.Add(new CompareItem("Logbogsbladnummer", server.logBldNr, client.logBldNr));
                lst.Add(new CompareItem("Indsamlingsmetode", server.samplingMethodId.HasValue ? GetLookupValue(lstSamplingMethods, s => s.samplingMethodId == server.samplingMethodId.Value) : null, client.samplingMethodId.HasValue ? GetLookupValue(lstSamplingMethods, s => s.samplingMethodId == client.samplingMethodId.Value) : null));
            }
            lst.Add(new CompareItem("Tidszone ved afgangstid", GetString(server.timeZone), GetString(client.timeZone)));
            lst.Add(new CompareItem("Afgangstid", GetString(server.dateStart, "dd-MM-yyyy HH:mm", server.timeZone), GetString(client.dateStart, "dd-MM-yyyy HH:mm", client.timeZone)));
            lst.Add(new CompareItem("Ankomsttid", GetString(server.dateEnd, "dd-MM-yyyy HH:mm", server.timeZone), GetString(client.dateEnd, "dd-MM-yyyy HH:mm", client.timeZone)));
            if (client.IsSEA)
                lst.Add(new CompareItem("Landingshavn", server.harbourLanding, client.harbourLanding));
            lst.Add(new CompareItem("Primært kutter nr.", server.platform1, client.platform1));
            if (client.IsSEA)
            {
                lst.Add(new CompareItem("Kamera på tur", GetString(server.fDFVessel), GetString(client.fDFVessel)));
                lst.Add(new CompareItem("Makker kutter nr.", server.platform2, client.platform2));
                lst.Add(new CompareItem("KontaktPerson", server.contactPersonId.HasValue ? GetLookupValue(lstPersons, p => p.personId == server.contactPersonId.Value) : null, client.contactPersonId.HasValue ? GetLookupValue(lstPersons, p => p.personId == client.contactPersonId.Value) : null));
            }
            lst.Add(new CompareItem("Turleder", server.tripLeaderId.HasValue ? GetLookupValue(lstDFUPersons, p => p.dfuPersonId == server.tripLeaderId.Value) : null, client.tripLeaderId.HasValue ? GetLookupValue(lstDFUPersons, p => p.dfuPersonId == client.tripLeaderId.Value) : null));
            lst.Add(new CompareItem("Indtaster", server.dataHandlerId.HasValue ? GetLookupValue(lstDFUPersons, p => p.dfuPersonId == server.dataHandlerId.Value) : null, client.dataHandlerId.HasValue ? GetLookupValue(lstDFUPersons, p => p.dfuPersonId == client.dataHandlerId.Value) : null));
            lst.Add(new CompareItem("Bemærkninger.", server.remark, client.remark));
        }


        private void CompareTripHVN(ref List<CompareItem> lst)
        {
            Trip server = _oeServer as Trip;
            Trip client = _oeClient as Trip;
            Sample serverSample = server.Sample.FirstOrDefault() ?? new Sample();
            Sample clientSample = client.Sample.FirstOrDefault() ?? new Sample();

            var lm = new LookupManager();
            var lstDFUPersons = lm.GetLookups(typeof(DFUPerson)).OfType<DFUPerson>().ToList();
            var lstSamplingMethods = lm.GetLookups(typeof(L_SamplingMethod)).OfType<L_SamplingMethod>().ToList();
            var lstSamplingTypes = lm.GetLookups(typeof(L_SamplingType)).OfType<L_SamplingType>().ToList();
            var lstCatchRegistrations = lm.GetLookups(typeof(L_CatchRegistration)).OfType<L_CatchRegistration>().ToList();
            var lstSpeciesRegistrations = lm.GetLookups(typeof(L_SpeciesRegistration)).OfType<L_SpeciesRegistration>().ToList();
            
            lst.Add(new CompareItem("Journal nr.", server.trip1, client.trip1));
            lst.Add(new CompareItem("Logbogsbladnummer", server.logBldNr, client.logBldNr));
            lst.Add(new CompareItem("Indsamlingsdato", GetString(server.dateSample, "dd-MM-yyyy"), GetString(client.dateSample, "dd-MM-yyyy")));
            lst.Add(new CompareItem("Indsamlingshavn", server.harbourSample, client.harbourSample));
            lst.Add(new CompareItem("Landingsdato", GetString(server.dateEnd, "dd-MM-yyyy"), GetString(client.dateEnd, "dd-MM-yyyy")));
            lst.Add(new CompareItem("Landingshavn", server.harbourLanding, client.harbourLanding));
            lst.Add(new CompareItem("Farvand", serverSample.dfuArea, clientSample.dfuArea));
            lst.Add(new CompareItem("Square", serverSample.statisticalRectangle, clientSample.statisticalRectangle));
            lst.Add(new CompareItem("Primært fartøj", server.platform1, client.platform1));
            lst.Add(new CompareItem("Makkerfartøj", server.platform2, client.platform2));
            lst.Add(new CompareItem("Fiskeritype", server.fisheryType, client.fisheryType));
            lst.Add(new CompareItem("Redskabstype", serverSample.gearType, clientSample.gearType));
            lst.Add(new CompareItem("Maskevidde", GetString(serverSample.meshSize), GetString(clientSample.meshSize)));
            lst.Add(new CompareItem("Fisk fra flere skibe", server.samplingTypeId.HasValue ? GetLookupValue(lstSamplingTypes, s => s.samplingTypeId == server.samplingTypeId.Value) : null, client.samplingTypeId.HasValue ? GetLookupValue(lstSamplingTypes, s => s.samplingTypeId == client.samplingTypeId.Value) : null));
            lst.Add(new CompareItem("Indsamlingsmetode", server.samplingMethodId.HasValue ? GetLookupValue(lstSamplingMethods, s => s.samplingMethodId == server.samplingMethodId.Value) : null, client.samplingMethodId.HasValue ? GetLookupValue(lstSamplingMethods, s => s.samplingMethodId == client.samplingMethodId.Value) : null));
            lst.Add(new CompareItem("Prøvetagningsniveau", serverSample.catchRegistrationId.HasValue ? GetLookupValue(lstCatchRegistrations, s => s.catchRegistrationId == serverSample.catchRegistrationId.Value) : null, clientSample.catchRegistrationId.HasValue ? GetLookupValue(lstCatchRegistrations, s => s.catchRegistrationId == clientSample.catchRegistrationId.Value) : null));
            lst.Add(new CompareItem("Oparbejdning af arter", serverSample.speciesRegistrationId.HasValue ? GetLookupValue(lstSpeciesRegistrations, s => s.speciesRegistrationId == serverSample.speciesRegistrationId.Value) : null, clientSample.speciesRegistrationId.HasValue ? GetLookupValue(lstSpeciesRegistrations, s => s.speciesRegistrationId == clientSample.speciesRegistrationId.Value) : null));
            lst.Add(new CompareItem("Indsamler", serverSample.samplePersonId.HasValue ? GetLookupValue(lstDFUPersons, p => p.dfuPersonId == serverSample.samplePersonId.Value) : null, clientSample.samplePersonId.HasValue ? GetLookupValue(lstDFUPersons, p => p.dfuPersonId == clientSample.samplePersonId.Value) : null));
            lst.Add(new CompareItem("Oparbejder af", serverSample.analysisPersonId.HasValue ? GetLookupValue(lstDFUPersons, p => p.dfuPersonId == serverSample.analysisPersonId.Value) : null, clientSample.analysisPersonId.HasValue ? GetLookupValue(lstDFUPersons, p => p.dfuPersonId == clientSample.analysisPersonId.Value) : null));
            lst.Add(new CompareItem("Indtaster", server.dataHandlerId.HasValue ? GetLookupValue(lstDFUPersons, p => p.dfuPersonId == server.dataHandlerId.Value) : null, client.dataHandlerId.HasValue ? GetLookupValue(lstDFUPersons, p => p.dfuPersonId == client.dataHandlerId.Value) : null));
            lst.Add(new CompareItem("Bemærkninger.", server.remark, client.remark));
        }


        private void CompareSample(ref List<CompareItem> lst)
        {
            Sample server = _oeServer as Sample;
            Sample client = _oeClient as Sample;

            var lm = new LookupManager();
            var lstCatchRegistrations = lm.GetLookups(typeof(L_CatchRegistration)).OfType<L_CatchRegistration>().ToList();
            var lstSpeciesRegistrations = lm.GetLookups(typeof(L_SpeciesRegistration)).OfType<L_SpeciesRegistration>().ToList();
            var lstSelectionDeviceSource = lm.GetLookups(typeof(L_SelectionDeviceSource)).OfType<L_SelectionDeviceSource>().ToList();

            lst.Add(new CompareItem("Stations nr.", server.station, client.station));
            lst.Add(new CompareItem("Lab. journal nr.", server.labJournalNum, client.labJournalNum));
            lst.Add(new CompareItem("Fast station", server.stationName, client.stationName));
            lst.Add(new CompareItem("Redskabsgruppe", server.sampleType, client.sampleType));
            lst.Add(new CompareItem("Redskabskvalitet", server.gearQuality, client.gearQuality));
            lst.Add(new CompareItem("Primær målart", server.R_TargetSpecies.Count == 0 ? null : server.R_TargetSpecies.First().speciesCode, client.R_TargetSpecies.Count == 0 ? null : client.R_TargetSpecies.First().speciesCode));
            lst.Add(new CompareItem("Træktype", server.haulType, client.haulType));
            lst.Add(new CompareItem("CTD Station", server.hydroStnRef, client.hydroStnRef));
            lst.Add(new CompareItem("Tidszone ved redskab sat", GetString(server.timeZone), GetString(client.timeZone)));
            lst.Add(new CompareItem("Redskab sat", GetString(server.dateGearStart, "dd-MM-yyyy HH:mm:ss", server.timeZone), GetString(client.dateGearStart, "dd-MM-yyyy HH:mm:ss", client.timeZone)));
            lst.Add(new CompareItem("Redskab bjærget", GetString(server.dateGearEnd, "dd-MM-yyyy HH:mm:ss", server.timeZone), GetString(client.dateGearEnd, "dd-MM-yyyy HH:mm:ss", client.timeZone)));
            lst.Add(new CompareItem("Startbreddegrad", server.latPosStartText, client.latPosStartText));
            lst.Add(new CompareItem("Startlængdegrad", server.lonPosStartText, client.lonPosStartText));
            lst.Add(new CompareItem("Slutbreddegrad", server.latPosEndText, client.latPosEndText));
            lst.Add(new CompareItem("Slutlængdegrad", server.lonPosEndText, client.lonPosEndText)); 
            lst.Add(new CompareItem("Farvand", server.dfuArea, client.dfuArea));
            lst.Add(new CompareItem("Square", server.statisticalRectangle, client.statisticalRectangle));
            lst.Add(new CompareItem("Prøvetagningsniveau", server.catchRegistrationId.HasValue ? GetLookupValue(lstCatchRegistrations, s => s.catchRegistrationId == server.catchRegistrationId.Value) : null, client.catchRegistrationId.HasValue ? GetLookupValue(lstCatchRegistrations, s => s.catchRegistrationId == client.catchRegistrationId.Value) : null));
            lst.Add(new CompareItem("Oparbejdning af arter", server.speciesRegistrationId.HasValue ? GetLookupValue(lstSpeciesRegistrations, s => s.speciesRegistrationId == server.speciesRegistrationId.Value) : null, client.speciesRegistrationId.HasValue ? GetLookupValue(lstSpeciesRegistrations, s => s.speciesRegistrationId == client.speciesRegistrationId.Value) : null));
            lst.Add(new CompareItem("Vindretning", GetString(server.windDirection), GetString(client.windDirection)));
            lst.Add(new CompareItem("Vindhastighed", GetString(server.windSpeed), GetString(client.windSpeed)));
            lst.Add(new CompareItem("Bølgeretning", GetString(server.waveDirection), GetString(client.waveDirection)));
            lst.Add(new CompareItem("Bølgehøjdeequiv.", GetString(server.waveHeigth), GetString(client.waveHeigth)));
            lst.Add(new CompareItem("Strømretning, overfl.", GetString(server.currentDirectionSrf), GetString(client.currentDirectionSrf)));
            lst.Add(new CompareItem("Strømretning, bund", GetString(server.currentDirectionBot), GetString(client.currentDirectionBot)));
            lst.Add(new CompareItem("Strømhast. overfl.", GetString(server.currentSpeedSrf), GetString(client.currentSpeedSrf)));
            lst.Add(new CompareItem("Strømhast, bund", GetString(server.currentSpeedBot), GetString(client.currentSpeedBot)));
            lst.Add(new CompareItem("Bundtype", server.bottomType, client.bottomType));
            lst.Add(new CompareItem("Dybde, bund", GetString(server.depthAvg), GetString(client.depthAvg)));
            lst.Add(new CompareItem("Temperatur, overflade", GetString(server.temperatureSrf), GetString(client.temperatureSrf)));
            lst.Add(new CompareItem("Temperatur, bund", GetString(server.temperatureBot), GetString(client.temperatureBot)));
            lst.Add(new CompareItem("Ilt, overflade", GetString(server.oxygenSrf), GetString(client.oxygenSrf)));
            lst.Add(new CompareItem("Ilt, bund", GetString(server.oxygenBot), GetString(client.oxygenBot)));
            lst.Add(new CompareItem("Springlag", server.thermoCline, client.thermoCline));
            lst.Add(new CompareItem("Springlag, dybde", GetString(server.thermoClineDepth), GetString(client.thermoClineDepth)));
            lst.Add(new CompareItem("Salinitet, overflade", GetString(server.salinitySrf), GetString(client.salinitySrf)));
            lst.Add(new CompareItem("Salinitet, bund", GetString(server.salinityBot), GetString(client.salinityBot)));
            lst.Add(new CompareItem("Redskabstype", server.gearType, client.gearType));
            lst.Add(new CompareItem("Selektionsudstyr", server.selectionDevice, client.selectionDevice));
            lst.Add(new CompareItem("Selektionsudstryskilde", server.selectionDeviceSourceId.HasValue ? GetLookupValue(lstSelectionDeviceSource, s => s.L_selectionDeviceSourceId == server.selectionDeviceSourceId.Value) : null, client.selectionDeviceSourceId.HasValue ? GetLookupValue(lstSelectionDeviceSource, s => s.L_selectionDeviceSourceId == client.selectionDeviceSourceId.Value) : null));
            lst.Add(new CompareItem("Maskevidde", GetString(server.meshSize), GetString(client.meshSize)));
            lst.Add(new CompareItem("Antal garn", GetString(server.numNets), GetString(client.numNets)));
            lst.Add(new CompareItem("Mistede garn", GetString(server.lostNets), GetString(client.lostNets)));
            lst.Add(new CompareItem("Højde i garn", GetString(server.heightNets), GetString(client.heightNets)));
            lst.Add(new CompareItem("Længde af garn", GetString(server.lengthNets), GetString(client.lengthNets)));
            lst.Add(new CompareItem("Længde af bom", GetString(server.lengthBeam), GetString(client.lengthBeam)));
            lst.Add(new CompareItem("Trækhastighed, bund", GetString(server.haulSpeedBot), GetString(client.haulSpeedBot)));
            lst.Add(new CompareItem("Fiskedybde", GetString(server.depthAveGear), GetString(client.depthAveGear)));
            lst.Add(new CompareItem("Meter tov", GetString(server.lengthRopeFlyer), GetString(client.lengthRopeFlyer)));
            lst.Add(new CompareItem("Tov tykkelse", GetString(server.widthRopeFlyer), GetString(client.widthRopeFlyer)));
            lst.Add(new CompareItem("Antal kroge", GetString(server.numberHooks), GetString(client.numberHooks)));
            lst.Add(new CompareItem("Redskabsbemærkninger", server.gearRemark, client.gearRemark));
            lst.Add(new CompareItem("Bemærkninger.", server.remark, client.remark));
            lst.Add(new CompareItem("Totalvægt (kg)", GetString(server.totalWeight), GetString(client.totalWeight)));
            lst.Add(new CompareItem("Vægtestimeringsmetode", server.weightEstimationMethod, client.weightEstimationMethod));
        }


        private string GetString(short? d)
        {
            return d.HasValue ? d.Value.ToString() : null;
        }

        private string GetString(decimal? d)
        {
            return d.HasValue ? d.Value.ToString() : null;
        }

        private string GetString(bool? bln)
        {
            return bln.HasValue ? (bln.Value ? "Ja" : "Nej") : null;
        }

        private string GetString(DateTime? dt, string strFormat, int? intTimeZone = null)
        {
            if (dt.HasValue)
            {
                DateTime time = dt.Value;

                if (intTimeZone.HasValue)
                    time = time.AddHours(intTimeZone.Value);

                return time.ToString(strFormat);
            }

            return null;
        }

        private string GetString(int? intValue)
        {
            return intValue.HasValue ? intValue.Value.ToString() : null;
        }

        private string GetLookupValue<T>(List<T> lst, Func<T, bool> whereClause) where T : class, ILookupEntity
        {
            T res = lst.Where(whereClause).FirstOrDefault();

            if (res != null)
                return res.UIDisplay;

            return "";
        }



        #region Server Wins Command


        public DelegateCommand ServerWinsCommand
        {
            get { return _cmdServerWins ?? (_cmdServerWins = new DelegateCommand(ServerWins)); }
        }


        public void ServerWins()
        {
            SelectedOverwritingMethod = OverwritingMethod.ServerWins;
            _blnChoiceMade = true;
            Close();
        }


        #endregion


        #region Client Wins Command


        public DelegateCommand ClientWinsCommand
        {
            get { return _cmdClientWins ?? (_cmdClientWins = new DelegateCommand(ClientWins)); }
        }


        public void ClientWins()
        {
            SelectedOverwritingMethod = OverwritingMethod.ClientWins;
            _blnChoiceMade = true;
            Close();
        }


        #endregion

        #region Stop Sync Command


        public DelegateCommand StopSyncCommand
        {
            get { return _cmdStopSync ?? (_cmdStopSync = new DelegateCommand(StopSync)); }
        }


        public void StopSync()
        {
            if (AppRegionManager.ShowMessageBox("Er du sikker på du vil stoppe synkroniseringen?", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;

            _blnStopSync = true;
            _blnChoiceMade = true;
            Close();
        }


        #endregion

        


        public override void FireClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            base.FireClosing(sender, e);

            if (!_blnChoiceMade)
            {
                e.Cancel = true;
                DispatchMessageBox("Vælg venligst om server-værdierne skal overskrives eller ej (eller stop synkroniseringen).");
            }
        }
    }
}
