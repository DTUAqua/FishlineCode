using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anchor.Core;
using System.Reflection;
using System.IO;
using System.Globalization;

namespace Babelfisk.Warehouse
{
    /// <summary>
    /// 
    /// </summary>
    public partial class EntityFactory
    {
        private List<DWMessage> _lstMessages;

        private List<Entities.Sprattus.TreatmentFactor> _lstTreatmentFactors;

        private List<Babelfisk.Entities.Sprattus.L_Stock> _lstStocks;

        private List<Babelfisk.Entities.Sprattus.R_StockSpeciesArea> _lstSpeciesAreaStock;


        private static KeyValuePair<string, string>[] _arrCharactersToRemove = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>(",", ""), new KeyValuePair<string, string>(";", ""), new KeyValuePair<string, string>("\n", " "), new KeyValuePair<string, string>("\r", "") };
     

        public List<DWMessage> Messages
        {
            get { return _lstMessages; }
        }

        public EntityFactory(List<Babelfisk.Entities.Sprattus.TreatmentFactor> lstTreatmentFactors, List<Babelfisk.Entities.Sprattus.L_Stock> lstStocks, List<Babelfisk.Entities.Sprattus.R_StockSpeciesArea> lstSpeciesAreaStock)
        {
            _lstTreatmentFactors = lstTreatmentFactors;
            _lstStocks = lstStocks ?? new List<Entities.Sprattus.L_Stock>();
            _lstSpeciesAreaStock = lstSpeciesAreaStock ?? new List<Entities.Sprattus.R_StockSpeciesArea>();
            if (_lstStocks != null && _lstSpeciesAreaStock != null)
                _lstSpeciesAreaStock.ForEach(x => x.L_Stock = _lstStocks.Where(y => y.L_stockId == x.L_stockId).FirstOrDefault());
            _lstMessages = new List<DWMessage>();
        }


        /// <summary>
        /// TODO - Get it straight what it should match with if several rows are found for same species code and area code.
        /// </summary>
        public string GetStockCodeOld(string speciesCode, string dfuAreaCode, string statRectCode, int? quarter)
        {
            string stockCode = null;
            try
            {
                var items = _lstSpeciesAreaStock.Where(x => x.speciesCode.Equals(speciesCode, StringComparison.InvariantCultureIgnoreCase) &&
                                                            x.DFUArea.Equals(dfuAreaCode, StringComparison.InvariantCultureIgnoreCase) &&
                                                            (statRectCode ?? "").Equals(x.statisticalRectangle ?? "", StringComparison.InvariantCultureIgnoreCase) &&
                                                            (quarter ?? -1) == (x.quarter ?? -1)).ToList();

                Babelfisk.Entities.Sprattus.L_Stock stock = null;

                if(items.Count == 1)
                    stock = items[0].L_Stock;

                /*if (items.Count == 1)
                    stock = items[0].L_Stock;
                else if(items.Count > 1)
                {
                    //Get where statistical rectangle and quarter fits
                    var itm = items.Where(x => (statRectCode ?? "").Equals(x.statisticalRectangle ?? "", StringComparison.InvariantCultureIgnoreCase) &&
                                               (quarter ?? -1) == (x.quarter ?? -1)).FirstOrDefault();

                    if (itm != null)
                        stock = itm.L_Stock;
                    else
                    {
                        //If none where found, get the one with fitting statistical rectangle only.
                        var itmStat = items.Where(x => (statRectCode ?? "").Equals(x.statisticalRectangle ?? "", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        if (itmStat != null)
                            stock = itmStat.L_Stock;
                        else
                        {
                            //If non where found, get the one with fitting quarter.
                            var itmQuarter = items.Where(x => (quarter ?? -1) == (x.quarter ?? -1)).FirstOrDefault();
                            if (itmQuarter != null)
                                stock = itmQuarter.L_Stock;
                        }
                    }
                }*/

                if (stock != null)
                    stockCode = stock.stockCode;
            }
            catch { }

            return stockCode;
        }

        /// <summary>
        /// Get stock code from species, area, ret, and quarter.
        /// </summary>
        public string GetStockCode(string speciesCode, string dfuAreaCode, string statRectCode, int? quarter)
        {
            var ls = GetStock(_lstSpeciesAreaStock, speciesCode, dfuAreaCode, statRectCode, quarter);

            if (ls != null)
                return ls.stockCode;

            return null;
        }


        /// <summary>
        /// Get stock from species, area, ret, and quarter.
        /// </summary>
        public static Babelfisk.Entities.Sprattus.L_Stock GetStock(List<Babelfisk.Entities.Sprattus.R_StockSpeciesArea> lstSpeciesAreaStock, string speciesCode, string dfuAreaCode, string statRectCode, int? quarter)
        {
            try
            {
                //Step 1) Get all relations matching on species code and area.
                var items = lstSpeciesAreaStock.Where(x => x.speciesCode.Equals(speciesCode, StringComparison.InvariantCultureIgnoreCase) &&
                                                            x.DFUArea.Equals(dfuAreaCode, StringComparison.InvariantCultureIgnoreCase)).ToList();

                if (items == null || items.Count == 0)
                    return null;

                IEnumerable<Entities.Sprattus.R_StockSpeciesArea> tmpLst = null;

                //Step 1.5) If an item matches exactly the rectangle and the quarter, use that. This step should not be used, since it could result in getting wrong ones.
                //tmpLst = items.Where(x => (statRectCode ?? "").Equals(x.statisticalRectangle ?? "", StringComparison.InvariantCultureIgnoreCase) &&
                //                          (quarter ?? -1) == (x.quarter ?? -1));

                //if (tmpLst.Count() == 1)
                //    return tmpLst.First().L_Stock;

                //Step 2) If only one item exists and rectangle and quarter is null, use it (we are done).
                if (items.Count == 1 && string.IsNullOrWhiteSpace(items[0].statisticalRectangle) && items[0].quarter == null)
                    return items[0].L_Stock;
               
                //Step 3) If any relations exist where rectangle is not null and quarter is null, then remove rows where rectangle does not fit.
                tmpLst = items.Where(x => !string.IsNullOrWhiteSpace(x.statisticalRectangle) && x.quarter == null);
                if(tmpLst.Any())
                {
                    foreach(var i in tmpLst.ToList())
                    {
                        if (!(statRectCode ?? "").Equals(i.statisticalRectangle ?? "", StringComparison.InvariantCultureIgnoreCase))
                            items.Remove(i);
                    }
                }

                //Step 4) If any relations exist where rectangle is null and quarter is not null, then remove rows where quarter does not fit.
                tmpLst = items.Where(x => string.IsNullOrWhiteSpace(x.statisticalRectangle) && x.quarter != null);
                if (tmpLst.Any())
                {
                    foreach (var i in tmpLst.ToList())
                    {
                        if ((quarter ?? -1) != (i.quarter ?? -1))
                            items.Remove(i);
                    }
                }

                //Step 5) If any relations exist where rectangle is not null and quarter is not null, then remove rows where rectangle and quarter does not fit.
                tmpLst = items.Where(x => !string.IsNullOrWhiteSpace(x.statisticalRectangle) && x.quarter != null);
                if (tmpLst.Any())
                {
                    foreach (var i in tmpLst.ToList())
                    {
                        if (!(statRectCode ?? "").Equals(i.statisticalRectangle ?? "", StringComparison.InvariantCultureIgnoreCase) || (quarter ?? -1) != (i.quarter ?? -1))
                            items.Remove(i);
                    }
                }

                //Step 6) if only one row is left, use that.
                if (items.Count == 1)
                    return items[0].L_Stock;
            }
            catch { }

            return null;
        }



        public Babelfisk.Warehouse.Model.Cruise CreateDWCruise(Babelfisk.Entities.Sprattus.Cruise c)
        {
            InitializeShapefiles();

            Babelfisk.Warehouse.Model.Cruise cDW = new Model.Cruise();
            cDW.ChangeTracker.ChangeTrackingEnabled = false;

            cDW.cruiseId = c.cruiseId;
            cDW.year = c.year;
            cDW.cruise1 = c.cruise1;
            cDW.responsibleId = c.responsibleId;

            if(c.DFUPerson1 != null)
                cDW.responsibleName = c.DFUPerson1.name;

            cDW.participants = c.participants;
            cDW.summary = c.summary; 
            cDW.remark = c.remark;

            //Create trips
            if (c.Trip != null && c.Trip.Count > 0)
            {
                foreach (var t in c.Trip)
                    cDW.Trips.Add(CreateDWTrip(t));
            }

            RemoveCharactersFromStringProperties(cDW, _arrCharactersToRemove);
            
            return cDW;
        }


        public Model.Trip CreateDWTrip(Babelfisk.Entities.Sprattus.Trip t)
        {
            var tDW = new Model.Trip();
            tDW.ChangeTracker.ChangeTrackingEnabled = false;

            tDW.tripId = t.tripId;
            tDW.cruiseId = t.cruiseId;
            tDW.year = t.Cruise.year;
            tDW.cruise = t.Cruise.cruise1;
            tDW.trip1 = t.trip1;
            tDW.tripType = t.tripType;
            tDW.logBldNr = t.logBldNr;

            tDW.timeZone = t.timeZone;
            tDW.dateStart = t.dateStart;
            tDW.dateEnd = t.dateEnd;

            if (t.L_SamplingType != null)
                tDW.samplingType = t.L_SamplingType.samplingType;

            if (t.L_SamplingMethod != null)
                tDW.samplingMethod = t.L_SamplingMethod.samplingMethod;

            tDW.fisheryType = t.fisheryType;

            tDW.platform1 = t.platform1;
            if (t.L_Platform != null)
                tDW.nationalityPlatform1 = t.L_Platform.nationality;

            tDW.fDFVesselPlatform1 = t.fDFVessel;

            tDW.platform2 = t.platform2;
            if (t.L_Platform1 != null)
                tDW.nationalityPlatform2 = t.L_Platform1.nationality;

            tDW.harbourLanding = t.harbourLanding;

            if (t.nationality != null)
                tDW.nationalityHarbourLanding = t.nationality;
            else if (t.L_Harbour != null)
                tDW.nationalityHarbourLanding = t.L_Harbour.nationality;

            tDW.contactPersonId = t.contactPersonId;
            if (t.Person != null)
                tDW.contactPersonName = t.Person.name;

            tDW.tripLeaderId = t.tripLeaderId;
            if (t.DFUPerson != null)
                tDW.tripLeaderName = t.DFUPerson.name;

            tDW.dataHandlerId = t.dataHandlerId;
            if (t.DFUPerson1 != null)
                tDW.dataHandlerName = t.DFUPerson1.name;

            tDW.remark = t.remark;

            tDW.dateSample = t.dateSample;

            tDW.harbourSample = t.harbourSample;

            if (t.L_Harbour1 != null)
                tDW.nationalityHarbourSample = t.L_Harbour1.nationality;

            //Assign REK variables
            tDW.tripNum = t.tripNum;
            tDW.placeName = t.placeName;
            tDW.placeCode = t.placeCode;
            tDW.postalCode = t.postalCode;
            tDW.numberInPlace = t.numberInPlace;
            tDW.respYes = t.respYes;
            tDW.respNo = t.respNo;
            tDW.respTot = t.respTot;
            tDW.sgTripId = t.sgTripId;

            //Create trips
            if (t.Sample != null && t.Sample.Count > 0)
            {
                foreach (var s in t.Sample)
                    tDW.Samples.Add(CreateDWSample(s));
            }

            RemoveCharactersFromStringProperties(tDW, _arrCharactersToRemove);

            return tDW;
        }


        public Model.Sample CreateDWSample(Babelfisk.Entities.Sprattus.Sample s)
        {
            var sDW = new Model.Sample();
            sDW.ChangeTracker.ChangeTrackingEnabled = false;

            sDW.sampleId = s.sampleId;
            sDW.tripId = s.tripId;
            sDW.year = s.Trip.Cruise.year;
            sDW.cruise = s.Trip.Cruise.cruise1;
            sDW.trip = s.Trip.trip1;
            sDW.tripType = s.Trip.tripType;
            sDW.station = s.station;
            sDW.stationName = s.stationName;
            sDW.labJournalNum = s.labJournalNum;
            sDW.gearQuality = s.gearQuality;
            sDW.hydroStnRef = s.hydroStnRef;
            sDW.haulType = s.haulType;
            sDW.dateGearStart = s.dateGearStart;
            sDW.dateGearEnd = s.dateGearEnd;
            sDW.timeZone = s.timeZone;
            sDW.quarterGearStart = s.dateGearStart.GetQuarter();

            if (s.Trip.IsHVN)
                sDW.fishingtime = null;
            else
            {
                //Calculate fishing time
                var tsFishingTime = Convert.ToDecimal((s.dateGearEnd - s.dateGearStart).TotalMinutes);

                if (tsFishingTime < 0)
                {
                    //Error i date time
                    AddMessage(DWMessage.NewError(GetCruiseId(s), GetOrigin(s), "Sample", s.sampleId.ToString(), String.Format("Sample.fishingTime blev udregnet til {0} hvilket er mindre end 0. Den blev udregnet ved: (Sample.gearDateEnd - Sample.gearDateStart).TotalMinutes() hvor gearDateEnd = {1} og gearDateStart = {2}", tsFishingTime.ToString(CultureInfo.InvariantCulture), s.dateGearEnd.ToString("dd/MM/yyyy HH:mm:ss"), s.dateGearStart.ToString("dd/MM/yyyy HH:mm:ss"))));
                }
                else if (tsFishingTime >= (decimal)Math.Pow(10, 7))
                {
                    //Error to large
                    AddMessage(DWMessage.NewError(GetCruiseId(s), GetOrigin(s), "Sample", s.sampleId.ToString(), String.Format("Sample.fishingTime blev udregnet til {0}, men må ikke være større end {1}. Den blev udregnet ved: (Sample.gearDateEnd - Sample.gearDateStart).TotalMinutes() hvor gearDateEnd = {2} og gearDateStart = {3}", tsFishingTime.ToString(CultureInfo.InvariantCulture), Math.Pow(10, 7), s.dateGearEnd.ToString("dd/MM/yyyy HH:mm:ss"), s.dateGearStart.ToString("dd/MM/yyyy HH:mm:ss"))));
                }

                sDW.fishingtime = tsFishingTime;  
            }

            sDW.latPosStartText = s.latPosStartText;
            sDW.lonPosStartText = s.lonPosStartText;
            sDW.latPosEndText = s.latPosEndText;
            sDW.lonPosEndText = s.lonPosEndText;

            decimal? latPosStartdec = null;
            if (!string.IsNullOrWhiteSpace(sDW.latPosStartText) && (latPosStartdec = PositionTextToDecimal(sDW.latPosStartText)) != null)
                sDW.latPosStartDec = latPosStartdec;
            
            decimal? lonPosStartdec = null;
            if(!string.IsNullOrWhiteSpace(sDW.lonPosStartText) && (lonPosStartdec = PositionTextToDecimal(sDW.lonPosStartText)) != null)
                sDW.lonPosStartDec = lonPosStartdec;

            decimal? latPosEnddec = null;
            if (!string.IsNullOrWhiteSpace(sDW.latPosEndText) && (latPosEnddec = PositionTextToDecimal(sDW.latPosEndText)) != null)
                sDW.latPosEndDec = latPosEnddec;

            decimal? lonPosEnddec = null;
            if (!string.IsNullOrWhiteSpace(sDW.lonPosEndText) && (lonPosEnddec = PositionTextToDecimal(sDW.lonPosEndText)) != null)
                sDW.lonPosEndDec = lonPosEnddec;

            if (!string.IsNullOrEmpty(s.statisticalRectangle))
                sDW.statisticalRectangle = s.statisticalRectangle;

            if (!string.IsNullOrEmpty(s.dfuArea))
                sDW.dfuArea = s.dfuArea;

            //Calculate area and rectangle for start positions
            if (latPosStartdec.HasValue && lonPosStartdec.HasValue)
            {
                //If dfArea is not assigned in UI, find it
                if (sDW.dfuArea == null)
                    sDW.dfuArea = GetArea(Convert.ToDouble(latPosStartdec.Value), Convert.ToDouble(lonPosStartdec.Value));

                if (sDW.dfuArea == null)
                    AddMessage(DWMessage.NewWarning(GetCruiseId(s), GetOrigin(s), "Sample", s.sampleId.ToString(), String.Format("Område blev ikke fundet til start-position ({0}, {1})", sDW.latPosStartText, sDW.lonPosStartText)));

                //If rectangle is not specified in UI, assign it. Only assign rectangle if area was found. Because if area is not found, it means the position is on land or outside bounds of area shapefile shapes.
                if (sDW.statisticalRectangle == null && sDW.dfuArea != null)
                {
                    sDW.statisticalRectangle = GetRectangle(Convert.ToDouble(latPosStartdec.Value), Convert.ToDouble(lonPosStartdec.Value));

                    if (sDW.statisticalRectangle == null)
                        AddMessage(DWMessage.NewWarning(GetCruiseId(s), GetOrigin(s), "Sample", s.sampleId.ToString(), String.Format("ICES Square blev ikke fundet til start-position ({0}, {1})", sDW.latPosStartText, sDW.lonPosStartText)));
                }
            }


            //Calculate area and rectangle for end positions
            if (latPosEnddec.HasValue && lonPosEnddec.HasValue)
            {
                sDW.dfuAreaEnd = GetArea(Convert.ToDouble(latPosEnddec.Value), Convert.ToDouble(lonPosEnddec.Value));

                if (sDW.dfuAreaEnd == null)
                    AddMessage(DWMessage.NewWarning(GetCruiseId(s), GetOrigin(s), "Sample", s.sampleId.ToString(), String.Format("Område blev ikke fundet til slut-position ({0}, {1})", sDW.latPosEndText, sDW.lonPosEndText)));

                //Only assign rectangle if area was found. Because if area is not found, it means the position is on land or outside bounds of area shapefile shapes.
                if (sDW.dfuAreaEnd != null)
                {
                    sDW.statisticalRectangleEnd = GetRectangle(Convert.ToDouble(latPosEnddec.Value), Convert.ToDouble(lonPosEnddec.Value));

                    if (sDW.statisticalRectangleEnd == null)
                        AddMessage(DWMessage.NewWarning(GetCruiseId(s), GetOrigin(s), "Sample", s.sampleId.ToString(), String.Format("ICES Square blev ikke fundet til slut-position ({0}, {1})", sDW.latPosEndText, sDW.lonPosEndText)));
                }
            }


            //Calculate distance between start and end position in nautical miles
            if (latPosStartdec.HasValue && latPosEnddec.HasValue && lonPosStartdec.HasValue && lonPosEnddec.HasValue)
            {
                double? meters = CalculateDistanceInMeters(latPosStartdec.Value, lonPosStartdec.Value, latPosEnddec.Value, lonPosEnddec.Value);

                if (meters.HasValue)
                {
                    var nMiles = Convert.ToDecimal(ConvertMetersToNauticalMiles(meters.Value));

                    if(nMiles < 1000)
                        sDW.distancePositions = nMiles;
                    else
                        AddMessage(DWMessage.NewError(GetCruiseId(s), GetOrigin(s), "Sample", s.sampleId.ToString(), String.Format("Sample.distancePositions blev udregnet til {0} sømil, men den må ikke være større end 999 sømil (den sættes derfor til NULL). Udregningen foregik ved hjælp af positionerne og en formel der tager hensyn til jordens krumning.", nMiles.ToString(CultureInfo.InvariantCulture))));
                    
                }
            }
            
            //Convert fishingtime from sec to hours and calculate nortical miles
            var ft = (s.haulSpeedBot.HasValue && sDW.fishingtime.HasValue) ? new Nullable<decimal>(s.haulSpeedBot.Value * (sDW.fishingtime.Value / 60.0M)) : null;

            //Only assign bottom distance if value is smaller than 1000 which is the reserved space in the database (Numeric(6.3)
            if (ft.HasValue)
            {
                if(ft.Value < 1000)
                    sDW.distanceBottom = ft.Value;
                else
                    AddMessage(DWMessage.NewError(GetCruiseId(s), GetOrigin(s), "Sample", s.sampleId.ToString(), String.Format("Sample.distanceBottom blev udregnet til {0}, men må ikke være større end 999. Den blev udregnet ved: Sample.haulSpeedBot * (Sample.fishingTime / 3600.0) hvor haulSpeedBot = {1} og fishingTime = {2}", ft.Value.ToString(CultureInfo.InvariantCulture), s.haulSpeedBot.Value.ToString(CultureInfo.InvariantCulture), sDW.fishingtime.Value.ToString(CultureInfo.InvariantCulture))));
            }

            sDW.courseTrack = s.courseTrack;

            //Handle target species
            if (s.R_TargetSpecies != null && s.R_TargetSpecies.Count > 0)
            {
                var lstSpecies = s.R_TargetSpecies.Select(x => x.speciesCode).Distinct().ToList();

                sDW.targetSpecies1 = lstSpecies[0];

                if (lstSpecies.Count > 1)
                    sDW.targetSpecies2 = lstSpecies[1];

                if (lstSpecies.Count > 2)
                    sDW.targetSpecies3 = lstSpecies[2];
            }

            //sDW.totalWeight = s.totalWeight;
            //sDW.weightEstimationMethod = s.weightEstimationMethod;

            if(s.L_CatchRegistration != null)
                sDW.catchRegistration = s.L_CatchRegistration.catchRegistration;

            if(s.L_SpeciesRegistration != null)
                sDW.speciesRegistration = s.L_SpeciesRegistration.speciesRegistration;

            if (s.L_SelectionDeviceSource != null)
                sDW.selectionDeviceSource = s.L_SelectionDeviceSource.selectionDeviceSource;

            sDW.gearType = s.gearType;
            sDW.selectionDevice = s.selectionDevice;
            sDW.meshSize = s.meshSize;
            sDW.netOpening = s.netOpening;
            sDW.otterBoardDist = s.shovelDist;
            sDW.numberTrawls = s.numberTrawls;
            sDW.wireLength = s.wireLength;
            sDW.wingSpread = s.wingSpread;
            sDW.numNets = s.numNets;
            sDW.lostNets = s.lostNets;
            sDW.heightNets = s.heightNets;
            sDW.lengthNets = s.lengthNets;
            sDW.lengthRopeFlyer = s.lengthRopeFlyer;
            sDW.widthRopeFlyer = s.widthRopeFlyer;
            sDW.numberHooks = s.numberHooks;
            sDW.lengthBeam = s.lengthBeam;
            sDW.depthAveGear = s.depthAveGear;
            sDW.haulDirection = s.haulDirection;
            sDW.haulSpeedBot = s.haulSpeedBot;
            sDW.haulSpeedWat = s.haulSpeedWat;
            sDW.gearRemark = s.gearRemark;
            sDW.depthAvg = s.depthAvg;
            sDW.bottomType = s.bottomType;
            sDW.windDirection = s.windDirection;
            sDW.windSpeed = s.windSpeed;
            sDW.currentDirectionSrf = s.currentDirectionSrf;
            sDW.currentSpeedSrf = s.currentSpeedSrf;
            sDW.currentDirectionBot = s.currentDirectionBot;
            sDW.currentSpeedBot = s.currentSpeedBot;
            sDW.waveDirection = s.waveDirection;
            sDW.waveHeigth = s.waveHeigth;
            sDW.thermoCline = s.thermoCline;
            sDW.thermoClineDepth = s.thermoClineDepth;
            sDW.temperatureSrf = s.temperatureSrf;
            sDW.temperatureBot = s.temperatureBot;
            sDW.oxygenSrf = s.oxygenSrf;
            sDW.oxygenBot = s.oxygenBot;
            sDW.salinitySrf = s.salinitySrf;
            sDW.salinityBot = s.salinityBot;

            sDW.samplePersonId = s.samplePersonId;
            if (s.DFUPerson != null)
                sDW.samplePersonName = s.DFUPerson.name;

            sDW.analysisPersonId = s.analysisPersonId;
            if (s.DFUPerson1 != null)
                sDW.analysisPersonName = s.DFUPerson1.name;

            sDW.remark = s.remark;

            //Assign REK variables
            sDW.sgId = s.sgId;
            sDW.weekdayWeekend = s.weekdayWeekend;

            foreach (var sl in s.SpeciesList)
            {
                sDW.SpeciesLists.Add(CreateDWSpeciesList(sl, sDW));
            }

            //Calculate raising factors for all species lists.
            CalculateRaisingFactors(sDW, s);

            //Create species list raised records
            CreateDWSpeciesListRaised(sDW);

            RemoveCharactersFromStringProperties(sDW, _arrCharactersToRemove);

            return sDW;
        }


       


        public Model.SpeciesListRaised CreateDWSpeciesListRaised(Model.Sample sDW)
        {
            Model.SpeciesListRaised slrDW = new Model.SpeciesListRaised();
            slrDW.ChangeTracker.ChangeTrackingEnabled = false;

            var q = from s in sDW.SpeciesLists
                    where s.raisingFactor.HasValue //Filter away SLs with non-representative animals and SLs where raisingfactor could not be calculated
                    group s by new
                    {
                        s.sampleId,
                        s.year,
                        s.cruise,
                        s.trip,
                        s.tripType,
                        s.station,
                        s.dateGearStart,
                        s.quarterGearStart,
                        s.dfuArea,
                        s.statisticalRectangle,
                        s.gearQuality,
                        s.gearType,
                        s.meshSize,
                        s.speciesCode,
                        s.landingCategory,
                        s.dfuBase_Category,
                        s.sizeSortingEU,
                        s.sizeSortingDFU,
                        s.sexCode,
                        s.cuticulaHardness,
                        s.ovigorous
                    } into g
                    select new { SLR = new Model.SpeciesListRaised()
                                        {
                                            //Set specieslist raised to species list id (there should be a 1 to 1 relation between them)
                                            speciesListRaisedId = g.First().speciesListId,
                                            year = g.Key.year,
                                            cruise = g.Key.cruise,
                                            trip = g.Key.trip,
                                            tripType = g.Key.tripType,
                                            station = g.Key.station,
                                            dateGearStart = g.Key.dateGearStart,
                                            quarterGearStart = g.Key.quarterGearStart,
                                            dfuArea = g.Key.dfuArea,
                                            statisticalRectangle = g.Key.statisticalRectangle,
                                            gearQuality = g.Key.gearQuality,
                                            gearType = g.Key.gearType,
                                            meshSize = g.Key.meshSize,
                                            speciesCode = g.Key.speciesCode,
                                            stock = GetStockCode(g.Key.speciesCode, g.Key.dfuArea, g.Key.statisticalRectangle, g.Key.quarterGearStart),
                                            landingCategory = g.Key.landingCategory,
                                            dfuBase_Category = g.Key.dfuBase_Category,
                                            sizeSortingEU = g.Key.sizeSortingEU,
                                            sizeSortingDFU = g.Key.sizeSortingDFU,
                                            sexCode = g.Key.sexCode,
                                            cuticulaHardness = g.Key.cuticulaHardness,
                                            ovigorous = g.Key.ovigorous,

                                            //Calculations
                                            weightSubSample = g.Sum(x => GetDWSpeciesListWeight(x, false)),
                                            weightTotal = g.Sum(x => GetDWSpeciesListWeight(x)),
                                            numberSubSample = g.Sum(x => GetDWSpeciesListNumber(x, false)),
                                            numberTotal = g.Sum(x => GetDWSpeciesListNumber(x))

                                        }, SpeciesLists = g.ToList() };

            //Create and assign animal raised
            foreach (var kv in q)
            {
                var slr = kv.SLR;

                var ar = CreateAnimalRaised(slr, kv.SpeciesLists);
                if(ar != null && ar.Count > 0)
                    slr.AnimalRaiseds.AddRange(ar);

                sDW.SpeciesListRaiseds.Add(slr);
            }

            RemoveCharactersFromStringProperties(slrDW, _arrCharactersToRemove);

            return slrDW;
        }




        public List<Model.AnimalRaised> CreateAnimalRaised(Model.SpeciesListRaised slrDW, List<Model.SpeciesList> slDWs)
        {
            Model.SpeciesList slDW = slDWs.FirstOrDefault();

            if (slDWs.Count == 0)
            {
                AddMessage(DWMessage.NewError(GetCruiseId(slrDW), GetOrigin(slrDW), "SpeciesListRaised", "Ny", "Der er blevet oprettet en SpecieslistRaised række, men den tilhørende SpeciesList-række blev ikke fundet."));
                return null;
            }
            else if(slDWs.Count > 1)
            {
                AddMessage(DWMessage.NewWarning(GetCruiseId(slrDW), GetOrigin(slrDW), "SpeciesListRaised", "Ny", "Flere SpeciesList-rækker blev fundet til en enkelt SpecieslistRaised-række, dette burde ikke kunne være muligt."));
            }

            
            Model.AnimalRaised arDW = new Model.AnimalRaised();
            slrDW.ChangeTracker.ChangeTrackingEnabled = false;

            var q = from a in slDW.Animals
                    where a.representative.Equals("ja", StringComparison.InvariantCultureIgnoreCase)
                    group a by new
                    {
                        a.SpeciesList.year,
                        a.SpeciesList.cruise,
                        a.SpeciesList.trip,
                        a.SpeciesList.tripType,
                        a.SpeciesList.station,
                        a.SpeciesList.dateGearStart,
                        a.SpeciesList.quarterGearStart,
                        a.SpeciesList.dfuArea,
                        a.SpeciesList.statisticalRectangle,
                        a.SpeciesList.gearQuality,
                        a.SpeciesList.gearType,
                        a.SpeciesList.meshSize,
                        a.SpeciesList.speciesCode,
                        a.SpeciesList.landingCategory,
                        a.SpeciesList.dfuBase_Category,
                        a.SpeciesList.sizeSortingEU,
                        a.SpeciesList.sizeSortingDFU,
                        SL_sexCode = a.SpeciesList.sexCode,
                        a.sexCode,
                        a.SpeciesList.cuticulaHardness,
                        a.SpeciesList.ovigorous,
                        a.broodingPhase,
                        a.length,
                        a.lengthMeasureType,
                        a.lengthMeasureUnit
                    } into g
                    select new Model.AnimalRaised()
                    {
                        speciesListRaisedId = slrDW.speciesListRaisedId,
                        year = g.Key.year,
                        cruise = g.Key.cruise,
                        trip = g.Key.trip,
                        tripType = g.Key.tripType,
                        station = g.Key.station,
                        dateGearStart = g.Key.dateGearStart,
                        quarterGearStart = g.Key.quarterGearStart,
                        dfuArea = g.Key.dfuArea,
                        statisticalRectangle = g.Key.statisticalRectangle,
                        gearQuality = g.Key.gearQuality,
                        gearType = g.Key.gearType,
                        meshSize = g.Key.meshSize,
                        speciesCode = g.Key.speciesCode,
                        stock = GetStockCode(g.Key.speciesCode, g.Key.dfuArea, g.Key.statisticalRectangle, g.Key.quarterGearStart),
                        landingCategory = g.Key.landingCategory,
                        dfuBase_Category = g.Key.dfuBase_Category,
                        sizeSortingEU = g.Key.sizeSortingEU,
                        sizeSortingDFU = g.Key.sizeSortingDFU,
                        speciesList_sexCode = g.Key.SL_sexCode,
                        sexCode = g.Key.sexCode,
                        cuticulaHardness = g.Key.cuticulaHardness,
                        ovigorous = g.Key.ovigorous,
                        broodingPhase = g.Key.broodingPhase,
                        length = g.Key.length,
                        lengthMeasureType = g.Key.lengthMeasureType,
                        lengthMeasureUnit = g.Key.lengthMeasureUnit,
                        numberSumSamplePerLength = g.Sum(x => GetDWAnimalNumberPerLength(x, false)),
                        numberTotalPerLength = g.Sum(x => GetDWAnimalNumberPerLength(x)),
                        weightMean = GetDWAnimalMeanWeight(g.Sum(x => GetDWAnimalWeight(x)), g.Where(x => x.representative.Equals("ja", StringComparison.InvariantCultureIgnoreCase)).Sum(x => x.number)),
                    };


            var lst = q.ToList();

            //Remove unwanted characters
            foreach(var itm in lst)
                RemoveCharactersFromStringProperties(itm, _arrCharactersToRemove);

            return lst;
        }


        public decimal? GetDWAnimalMeanWeight(decimal weight, decimal number)
        {
            if (weight == 0 || number == 0)
                return null;

            return weight / number;
        }

        public decimal GetDWAnimalWeight(Model.Animal a)
        {
            if ( /*a.gearQuality == null || !a.gearQuality.Equals("V", StringComparison.InvariantCultureIgnoreCase) || */  //GearQuality is not filtered, since HVN doesn't have a gearQuality.
                a.representative == null || !a.representative.Equals("ja", StringComparison.InvariantCultureIgnoreCase) ||
                a.treatmentFactor == null || !a.treatmentFactor.HasValue ||
                !a.weight.HasValue)
                return 0;

            return a.weight.Value * a.treatmentFactor.Value;
        }

        public decimal GetDWAnimalNumberPerLength(Model.Animal a, bool blnIncludeRaisingFactor = true)
        {
            if ( /*a.gearQuality == null || !a.gearQuality.Equals("V", StringComparison.InvariantCultureIgnoreCase) || */
                 a.representative == null || !a.representative.Equals("ja", StringComparison.InvariantCultureIgnoreCase))
                return 0;

            return a.number * (blnIncludeRaisingFactor ? (a.SpeciesList.raisingFactor ?? 0) : 1);
        }

        public decimal GetDWSpeciesListWeight(Model.SpeciesList sl, bool blnIncludeRaisingFactor = true)
        {
           /*if(sl.gearQuality == null || !sl.gearQuality.Equals("V", StringComparison.InvariantCultureIgnoreCase))
                return 0;*/

            //Got lowest weight of all defined weights
            var min = Math.Min(Math.Min(Math.Min(sl.weightStep0 ?? decimal.MaxValue, sl.weightStep1 ?? decimal.MaxValue), sl.weightStep2 ?? decimal.MaxValue), sl.weightStep3 ?? decimal.MaxValue);

            //If none was found, return 0
            if (min == decimal.MaxValue)
                return 0;

            return min * (blnIncludeRaisingFactor ? (sl.raisingFactor ?? 0) : 1) * (sl.treatmentFactor ?? 1.0M);
        }

        public decimal GetDWSpeciesListNumber(Model.SpeciesList sl, bool blnIncludeRaisingFactor = true)
        {
            /* if(sl.gearQuality == null || !sl.gearQuality.Equals("V", StringComparison.InvariantCultureIgnoreCase))
                 return 0;*/

            decimal number = 0;

            var q = sl.Animals.Where(x => x.representative.Equals("ja", StringComparison.InvariantCultureIgnoreCase));
        
            //Get sum of number from animal records
            if (q.Any())
                number = q.Sum(x => x.number);
            else  //If no representative animals are there, use SL.number if there.
                number = sl.number.HasValue ? sl.number.Value : 0;

            //Multiply it with the raisingfactor
            return number * (blnIncludeRaisingFactor ? (sl.raisingFactor ?? 0) : 1);
        }


        public Model.SpeciesList CreateDWSpeciesList(Babelfisk.Entities.Sprattus.SpeciesList sl, Model.Sample sParent)
        {
            Model.SpeciesList slDW = new Model.SpeciesList();
            slDW.ChangeTracker.ChangeTrackingEnabled = false;

            slDW.speciesListId = sl.speciesListId;
            slDW.sampleId = sl.sampleId;
            slDW.year = sl.Sample.Trip.Cruise.year;
            slDW.cruise = sl.Sample.Trip.Cruise.cruise1;
            slDW.trip = sl.Sample.Trip.trip1;
            slDW.tripType = sl.Sample.Trip.tripType;
            slDW.station = sl.Sample.station;
            slDW.dateGearStart = sl.Sample.dateGearStart;

            slDW.quarterGearStart = sParent.quarterGearStart;
            slDW.dfuArea = sParent.dfuArea;
            slDW.statisticalRectangle = sParent.statisticalRectangle;

            slDW.gearQuality = sParent.gearQuality;
            slDW.gearType = sl.Sample.gearType;
            slDW.meshSize = sl.Sample.meshSize;
            slDW.speciesCode = sl.speciesCode;
            slDW.landingCategory = sl.landingCategory;
            slDW.dfuBase_Category = sl.dfuBase_Category;
            slDW.sizeSortingEU = sl.sizeSortingEU;
            slDW.sizeSortingDFU = sl.sizeSortingDFU;
            slDW.sexCode = sl.sexCode;
            slDW.ovigorous = sl.ovigorous;
            slDW.cuticulaHardness = sl.cuticulaHardness;
            slDW.treatment = sl.treatment;

            Babelfisk.Entities.Sprattus.SubSample ss = null;

            if ((ss = sl.SubSample.Where(x => x.IsRepresentative && x.stepNum == 0).FirstOrDefault()) != null)
            {
                if (ss.sumAnimalWeights.HasValue && ss.sumAnimalWeights.Value)
                {
                    var ani = ss.Animal.Where(x => x.weight.HasValue);
                    slDW.weightStep0 = ani.Count() > 0 ? new Nullable<decimal>(ani.Sum(x => x.weight.Value)) : null;
                    ss.subSampleWeight = slDW.weightStep0; //Also assign subSampleWeight on SubSample, so it can be used in raisingfactor calculation
                }
                else
                    slDW.weightStep0 = ss.subSampleWeight;

                slDW.landingWeightStep0 = ss.landingWeight;
            }

            if ((ss = sl.SubSample.Where(x => x.IsRepresentative && x.stepNum == 1).FirstOrDefault()) != null)
            {
                if (ss.sumAnimalWeights.HasValue && ss.sumAnimalWeights.Value)
                {
                    var ani = ss.Animal.Where(x => x.weight.HasValue);
                    slDW.weightStep1 = ani.Count() > 0 ? new Nullable<decimal>(ani.Sum(x => x.weight.Value)) : null;
                    ss.subSampleWeight = slDW.weightStep1; //Also assign subSampleWeight on SubSample, so it can be used in raisingfactor calculation
                }
                else
                    slDW.weightStep1 = ss.subSampleWeight;

                slDW.landingWeightStep1 = ss.landingWeight;
            }

            if ((ss = sl.SubSample.Where(x => x.IsRepresentative && x.stepNum == 2).FirstOrDefault()) != null)
            {
                if (ss.sumAnimalWeights.HasValue && ss.sumAnimalWeights.Value)
                {
                    var ani = ss.Animal.Where(x => x.weight.HasValue);
                    slDW.weightStep2 = ani.Count() > 0 ? new Nullable<decimal>(ani.Sum(x => x.weight.Value)) : null;
                    ss.subSampleWeight = slDW.weightStep2; //Also assign subSampleWeight on SubSample, so it can be used in raisingfactor calculation
                }
                else
                    slDW.weightStep2 = ss.subSampleWeight;
            }

            if ((ss = sl.SubSample.Where(x => x.IsRepresentative && x.stepNum == 3).FirstOrDefault()) != null)
            {
                if (ss.sumAnimalWeights.HasValue && ss.sumAnimalWeights.Value)
                {
                    var ani = ss.Animal.Where(x => x.weight.HasValue);
                    slDW.weightStep3 = ani.Count() > 0 ? new Nullable<decimal>(ani.Sum(x => x.weight.Value)) : null;
                    ss.subSampleWeight = slDW.weightStep3; //Also assign subSampleWeight on SubSample, so it can be used in raisingfactor calculation
                }
                else
                    slDW.weightStep3 = ss.subSampleWeight;
            }

            if ((ss = sl.SubSample.Where(x => !x.IsRepresentative && x.stepNum == 0).FirstOrDefault()) != null)
                slDW.weightNonRep = ss.subSampleWeight;

            slDW.number = sl.number;
            if (sl.L_Species == null)
                throw new ApplicationException(string.Format("En art mangler for en række i artslisten for station: {0} -> {1} -> {2} - {3} -> {4}.", sParent.year.ToString(), sParent.cruise ?? "N/A", sParent.tripType ?? "N/A", sParent.trip ?? "N/A", sParent.station ?? "N/A"));

            slDW.treatmentFactor = GetTreatmentFactor(slDW.dateGearStart, sl.L_Species.treatmentFactorGroup, sl.treatment, _lstTreatmentFactors);

            slDW.wemSpeciesList = sl.weightEstimationMethod;
            slDW.totalWeight = sl.Sample.totalWeight;
            slDW.wemTotalWeight = sl.Sample.weightEstimationMethod;
            slDW.remark = sl.remark;

            slDW.bmsNonRep = sl.bmsNonRep;

            //Assign REK variables
            if(sl.L_Application != null)
                slDW.applicationCode = sl.L_Application.code;

            foreach (var a in sl.SubSample.SelectMany(x => x.Animal))
            {
                slDW.Animals.Add(CreateDWAnimal(a, sl, slDW));
            }

            slDW.stock = GetStockCode(slDW.speciesCode, slDW.dfuArea, slDW.statisticalRectangle, slDW.quarterGearStart);

            RemoveCharactersFromStringProperties(slDW, _arrCharactersToRemove);

            return slDW;
        }


        private Model.Animal CreateDWAnimal(Babelfisk.Entities.Sprattus.Animal a, Babelfisk.Entities.Sprattus.SpeciesList sl, Model.SpeciesList slParent)
        {
            Model.Animal aDW = new Model.Animal();
            aDW.ChangeTracker.ChangeTrackingEnabled = false;

            var animalInfo = a.AnimalInfo.FirstOrDefault();

            aDW.animalId = a.animalId;

            if(animalInfo != null)
                aDW.animalInfoId = animalInfo.animalInfoId;

            aDW.speciesListId = slParent.speciesListId;
            aDW.year = slParent.year;
            aDW.cruise = slParent.cruise;
            aDW.trip = slParent.trip;
            aDW.tripType = slParent.tripType;
            aDW.station = slParent.station;
            aDW.quarterGearStart = slParent.quarterGearStart;
            aDW.dateGearStart = slParent.dateGearStart;
            aDW.dfuArea = slParent.dfuArea;
            aDW.statisticalRectangle = slParent.statisticalRectangle;
            aDW.gearQuality = slParent.gearQuality;
            aDW.gearType = slParent.gearType;
            aDW.meshSize = slParent.meshSize;
            aDW.speciesCode = slParent.speciesCode;
            aDW.landingCategory = slParent.landingCategory;
            aDW.dfuBase_Category = slParent.dfuBase_Category;
            aDW.sizeSortingEU = slParent.sizeSortingEU;
            aDW.sizeSortingDFU = slParent.sizeSortingDFU;
            aDW.ovigorous = slParent.ovigorous;
            aDW.cuticulaHardness = slParent.cuticulaHardness;
            aDW.treatment = slParent.treatment;
            aDW.speciesList_sexCode = slParent.sexCode;
            aDW.sexCode = a.sexCode;
            aDW.representative = a.SubSample.representative;
            aDW.individNum = a.individNum;
            aDW.number = a.number;
            aDW.speciesList_number = slParent.number;
            aDW.length = a.length;

            if (a.L_LengthMeasureType != null)
                aDW.lengthMeasureType = a.L_LengthMeasureType.lengthMeasureType;

            aDW.lengthMeasureUnit = a.lengthMeasureUnit;
            aDW.weight = a.weight;  //31

            aDW.treatmentFactor = slParent.treatmentFactor;

            if (animalInfo != null && animalInfo.Maturity != null)
            {
                aDW.maturityIndex = animalInfo.Maturity.maturityIndex;
                aDW.maturityIndexMethod = animalInfo.Maturity.maturityIndexMethod;
            }

            aDW.broodingPhase = a.broodingPhase;

            if(animalInfo != null)
            {
                aDW.weightGutted = animalInfo.weightGutted;
                aDW.weightLiver = animalInfo.weightLiver;
                aDW.weightGonads = animalInfo.weightGonads;

                if (animalInfo.L_Parasite != null)
                    aDW.parasiteCode = animalInfo.L_Parasite.num;

                if (animalInfo.Fat != null)
                {
                    aDW.fatIndex = animalInfo.Fat.fatIndex;
                    aDW.fatIndexMethod = animalInfo.Fat.fatIndexMethod;
                }

                aDW.numVertebra = animalInfo.numVertebra;
            }

            aDW.maturityReaderId = sl.maturityReaderId;

            if(sl.MaturityReader != null)
                aDW.maturityReader = sl.MaturityReader.name;

            aDW.remark = a.remark;

            //Assign REK variables
            aDW.catchNum = a.catchNum;
            aDW.otolithFinScale = a.otolithFinScale;

            if (animalInfo != null)
                aDW.animalInfo_remark = animalInfo.remark;

            foreach (var age in a.Age)
            {
                aDW.Ages.Add(CreateDWAge(age, aDW, sl));
            }


            if (animalInfo != null)
            {
                foreach (var r in animalInfo.R_AnimalInfoReference)
                    aDW.R_AnimalReference.Add(CreateDWR_AnimalReference(r, aDW));
            }

            //Add otolith images
            if (a.AnimalFiles != null && a.AnimalFiles.Count > 0)
            {
                foreach (var af in a.AnimalFiles)
                {
                    var afRefDW = CreateDWR_AnimalPictureReference(af, aDW);
                    if (afRefDW != null)
                        aDW.R_AnimalPictureReference.Add(afRefDW);
                }
            }

            aDW.stock = GetStockCode(aDW.speciesCode, aDW.dfuArea, aDW.statisticalRectangle, aDW.quarterGearStart);

            RemoveCharactersFromStringProperties(aDW, _arrCharactersToRemove);

            return aDW;
        }


        private Model.R_AnimalReference CreateDWR_AnimalReference(Babelfisk.Entities.Sprattus.R_AnimalInfoReference r_a, Model.Animal aParent)
        {
            Model.R_AnimalReference rDW = new Model.R_AnimalReference();
            rDW.ChangeTracker.ChangeTrackingEnabled = false;

            rDW.animalId = aParent.animalId;
            rDW.referenceId = r_a.L_referenceId;

            if (r_a.L_Reference != null)
                rDW.referenceName = r_a.L_Reference.reference;

            RemoveCharactersFromStringProperties(rDW, _arrCharactersToRemove);

            return rDW;
        }

        private Model.R_AnimalPictureReference CreateDWR_AnimalPictureReference(Babelfisk.Entities.Sprattus.AnimalFile af, Model.Animal aParent)
        {
            if (af.FileTypeEnum != Entities.AnimalFileType.OtolithImage)
                return null;

            Model.R_AnimalPictureReference rDW = new Model.R_AnimalPictureReference();
            rDW.ChangeTracker.ChangeTrackingEnabled = false;

            rDW.animalId = aParent.animalId;
            rDW.pictureReference = af.FullFilePath;

            return rDW;
        }


        private Model.Age CreateDWAge(Babelfisk.Entities.Sprattus.Age a, Model.Animal aParent, Babelfisk.Entities.Sprattus.SpeciesList sl)
        {
            Model.Age aDW = new Model.Age();
            aDW.ChangeTracker.ChangeTrackingEnabled = false;

            aDW.ageId = a.ageId;
            aDW.animalId = a.animalId;
            aDW.year = aParent.year;
            aDW.cruise = aParent.cruise;
            aDW.trip = aParent.trip;
            aDW.tripType = aParent.tripType;
            aDW.station = aParent.station;
            aDW.dateGearStart = aParent.dateGearStart;
            aDW.quarterGearStart = aParent.quarterGearStart;
            aDW.dfuArea = aParent.dfuArea;
            aDW.statisticalRectangle = aParent.statisticalRectangle;
            aDW.gearQuality = aParent.gearQuality;
            aDW.gearType = aParent.gearType;
            aDW.meshSize = aParent.meshSize;
            aDW.speciesCode = aParent.speciesCode;
            aDW.landingCategory = aParent.landingCategory;
            aDW.dfuBase_Category = aParent.dfuBase_Category;
            aDW.sizeSortingEU = aParent.sizeSortingEU;
            aDW.sizeSortingDFU = aParent.sizeSortingDFU;
            aDW.ovigorous = aParent.ovigorous;
            aDW.cuticulaHardness = aParent.cuticulaHardness;
            aDW.treatment = aParent.treatment;
            aDW.speciesList_sexCode = aParent.speciesList_sexCode;
            aDW.sexCode = aParent.sexCode;
            aDW.representative = aParent.representative;
            aDW.individNum = aParent.individNum;
            aDW.number = a.number;
            aDW.length = aParent.length;
            aDW.lengthMeasureType = aParent.lengthMeasureType;
            aDW.age1 = a.age1;
            aDW.agePlusGroup = sl.agePlusGroup;
            aDW.otolithWeight = a.otolithWeight;
            aDW.edgeStructure = a.edgeStructure;
            aDW.genetics = a.genetics;

            if (a.L_OtolithReadingRemark != null)
                aDW.otolithReadingRemark = a.L_OtolithReadingRemark.otolithReadingRemark;

            if (a.L_HatchMonthReadability != null)
                aDW.hatchMonthRemark = a.L_HatchMonthReadability.hatchMonthRemark;

            aDW.hatchMonth = a.hatchMonth;
            aDW.ageReadId = sl.ageReadId;

            if (sl.DFUPerson != null)
                aDW.ageReadName = sl.DFUPerson.name;

            aDW.hatchMonthReaderId = sl.hatchMonthReaderId;

            if (sl.HatchMontReader != null)
                aDW.hatchMonthReaderName = sl.HatchMontReader.name;

            aDW.remark = a.remark;

            if(a.L_VisualStock != null)
                aDW.visualStock = a.L_VisualStock.visualStock;

            if(a.L_GeneticStock != null)
                aDW.geneticStock = a.L_GeneticStock.geneticStock;

            aDW.stock = GetStockCode(aDW.speciesCode, aDW.dfuArea, aDW.statisticalRectangle, aDW.quarterGearStart);

            aDW.sdAgeInfoUpdated = a.sdAgeInfoUpdated;
            aDW.sdAgeReadId = a.sdAgeReadId;
            if (a.DFUPerson != null)
                aDW.sdAgeReadName = a.DFUPerson.name;
            aDW.sdAnnotationId = a.sdAnnotationId;

            RemoveCharactersFromStringProperties(aDW, _arrCharactersToRemove);

            return aDW;
        }


        public static decimal? GetTreatmentFactor(DateTime dt, string strTreatmentFactorGroup, string strTreatment, List<Entities.Sprattus.TreatmentFactor> lstTreatmentFactors)
        {
            if (lstTreatmentFactors == null || string.IsNullOrWhiteSpace(strTreatmentFactorGroup) || string.IsNullOrWhiteSpace(strTreatment))
                return null;

            var lstFactors = lstTreatmentFactors.Where(x => x.treatmentFactorGroup.Equals(strTreatmentFactorGroup, StringComparison.InvariantCultureIgnoreCase) && x.treatment.Equals(strTreatment, StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (lstFactors.Count == 0)
                return null;

            lstFactors = lstFactors.OrderBy(x => x.versioningDate).ToList();

            Entities.Sprattus.TreatmentFactor treatmentFactor = null;
            for (int i = 0; i < lstFactors.Count; i++)
            {
                if (dt >= lstFactors[i].versioningDate)
                    treatmentFactor = lstFactors[i];
            }

            return treatmentFactor == null ? null : new Nullable<decimal>(treatmentFactor.factor);
        }


        /// <summary>
        /// Add message to log - this can be used for user display, after raising.
        /// </summary>
        private void AddMessage(DWMessage msg)
        {
            msg.Index = _lstMessages.Count+1;
            _lstMessages.Add(msg);
        }


        private string GetOrigin(Entities.Sprattus.SpeciesList sl)
        {
            return GetOrigin(sl.Sample);
        }

        private string GetOrigin(Entities.Sprattus.Sample s)
        {
            if (s.Trip == null)
                return "";

            if (s.Trip.tripType.Equals("HVN", StringComparison.InvariantCultureIgnoreCase))
                return GetOrigin(s.Trip);
            else
                return string.Format("{0}->{1}", GetOrigin(s.Trip), s.station);
        }

        private string GetOrigin(Entities.Sprattus.Trip t)
        {
            return string.Format("{0}->{1}", GetOrigin(t.Cruise), t.trip1);
        }

        private string GetOrigin(Entities.Sprattus.Cruise c)
        {
            return string.Format("{0}->{1}", c.year, c.cruise1);
        }

        private string GetOrigin(Model.SpeciesListRaised slrDW)
        {
            string str = string.Format("{0}->{1}->{2}", slrDW.year, slrDW.cruise, slrDW.trip);

            if (slrDW.tripType.Equals("hvn", StringComparison.InvariantCultureIgnoreCase))
                str += "->" + slrDW.station;

            return str;
        }

        private int? GetCruiseId(Model.SpeciesListRaised slrDW)
        {
            return slrDW.Sample.Trip1.cruiseId;
        }

        public int? GetCruiseId(Entities.Sprattus.SpeciesList e)
        {
            return GetCruiseId(e.Sample);
        }

        public int? GetCruiseId(Entities.Sprattus.Sample e)
        {
            return GetCruiseId(e.Trip);
        }

        public int? GetCruiseId(Entities.Sprattus.Trip e)
        {
            if (e == null)
                return -1;

            return GetCruiseId(e.Cruise);
        }

        public int? GetCruiseId(Entities.Sprattus.Cruise c)
        {
            return c.cruiseId;
        }

        internal decimal? PositionTextToDecimal(string strPosition)
        {
            decimal? num = null;
            try
            {
                string str = strPosition.Right(1);
                int num1 = strPosition.IndexOf(".", 0);// Strings.InStr(1, strPosition, ".", 0);
                int num2 = strPosition.IndexOf(".", num1 + 1);// Strings.InStr(checked(num1 + 1), strPosition, ".", 0);
                int num3 = strPosition.IndexOf(" ", num2 + 1); // Strings.InStr(checked(num2 + 1), strPosition, " ", 0);
                int num4 = strPosition.Substring(0, num1).ToInt32(); //IntegerType.FromString(Strings.Mid(strPosition, 1, num1)); 57

                //var g = Strings.Mid(strPosition, checked(num1 + 1), checked(num3 - num1))
                //DoubleType.FromString(Strings.Replace(g, ".", ",", 1, -1, 0));
                var s = strPosition.Substring(num1 + 1, num3 - num1);
                double num5 = s.ToDouble();
                decimal num6 = new decimal((double)num4 + num5 / 60);
                if (str.Equals("S", StringComparison.InvariantCultureIgnoreCase) || str.Equals("W", StringComparison.InvariantCultureIgnoreCase))
                    num6 = decimal.Negate(num6);

                num = num6;
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return num;
        }


        private double ConvertMetersToNauticalMiles(double dblMeters)
        {
            return dblMeters / 1852;
        }


        internal double? CalculateDistanceInMeters(decimal latStart, decimal lonStart, decimal latEnd, decimal lonEnd)
        {
            double? num = null;
            double num1 = 6371000;
            try
            {
                double num2 = 0.0174532925199433 * Convert.ToDouble(latStart);
                double num3 = 0.0174532925199433 * Convert.ToDouble(lonStart);
                double num4 = 0.0174532925199433 * Convert.ToDouble(latEnd);
                double num5 = 0.0174532925199433 * Convert.ToDouble(lonEnd);
                double num6 = num4 - num2;
                double num7 = num5 - num3;
                double num8 = Math.Pow(Math.Sin(num6 / 2), 2) + Math.Cos(num4) * Math.Cos(num2) * Math.Pow(Math.Sin(num7 / 2), 2);
                double num9 = 2 * Math.Atan2(Math.Sqrt(num8), Math.Sqrt(1 - num8));
                num = num1 * num9;
                return num;
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return num;
        }


        public static void RemoveCharactersFromStringProperties<T>(T obj, KeyValuePair<string, string>[] arrCharactersToRemove,  params string[] omittedProperties)
        {
            try
            {
                if (arrCharactersToRemove == null || arrCharactersToRemove.Length == 0)
                    return;

                List<string> lst = new List<string>();

                Type t = obj.GetType();

                PropertyInfo[] pi = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                var typeString = typeof(string);
                var lstProperties = pi.Where(x => x.PropertyType == typeString);

                var omitted = omittedProperties == null ? new HashSet<string>() : omittedProperties.Distinct().ToHashSet();

                foreach (var prop in lstProperties)
                {
                    if (omitted.Contains(prop.Name))
                        continue;

                    var valOrig = prop.GetValue(obj, new object[] { }) as string;

                    if (string.IsNullOrEmpty(valOrig))
                        continue;
                            
                    var val = valOrig;

                    for (int i = 0; i < arrCharactersToRemove.Length; i++)
                        val = val.Replace(arrCharactersToRemove[i].Key, arrCharactersToRemove[i].Value);

                    if (val != valOrig)
                        prop.SetValue(obj, val, new object[] { });
                }
            }
            catch(Exception e) 
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }

    }
}
