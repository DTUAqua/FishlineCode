using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Babelfisk.Warehouse.Model;
using System.Globalization;
using Anchor.Core;
using System.Reflection;

namespace Babelfisk.BusinessLogic.Export
{
    public partial class ExportManager
    {
        private string _strSeperator; //seperator is used for seperating the columns in the csv-file.
        private string _strDelimiter; //delimiter is used as numeric delimiter in numbers (for example 1.34 - here '.' is the delimiter)
        private string _strDigitGrouping; //Digit grouping is used as numeric grouping in numbers (for example 1.345,244 - here ',' is the digit grouping character)
        NumberFormatInfo _nfi = new NumberFormatInfo();

        private bool _blnInitialized = false;


        private void InitializeExport(string strDestinationPath)
        {
            if (!Directory.Exists(strDestinationPath))
                throw new ApplicationException(String.Format("Mappen {0} eksisterer ikke.", strDestinationPath));

            if (_blnInitialized)
                return;

            _strSeperator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            _strDelimiter = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            _strDigitGrouping = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;

            //Set number format used for transforming decimal values to string
            _nfi.NumberDecimalSeparator = _strDelimiter;
            _nfi.NumberGroupSeparator = _strDigitGrouping;

            _blnInitialized = true;
        }


        public void SaveLookupTable<T>(string strDestinationPath, string strFilePrefix, params string[] omittedProperties)
        {
            InitializeExport(strDestinationPath);

            LookupManager lm = new LookupManager();

            var lstSpecies = lm.GetLookups(typeof(T)).OfType<T>().ToList();

            if (lstSpecies == null)
                lstSpecies = new List<T>();
            Dictionary<string, string> headerMap = null;
            Dictionary<string, string> valueMap = null;

            if (typeof(T).Name == typeof(Babelfisk.Entities.Sprattus.L_Species).Name)
            {
                foreach (var s in lstSpecies.OfType<Babelfisk.Entities.Sprattus.L_Species>())
                {
                    if (LengthMeasureType != null && s.standardLengthMeasureTypeId.HasValue && LengthMeasureType.ContainsKey(s.standardLengthMeasureTypeId.Value))
                        s.L_StandardLengthMeasureType = LengthMeasureType[s.standardLengthMeasureTypeId.Value];
                }

                headerMap = new Dictionary<string, string>() { { "standardLengthMeasureTypeId", "standardLengthMeasureType" } };
                valueMap = new Dictionary<string, string>() { { "standardLengthMeasureTypeId", "StandardLengthMeasureType" } };
            }

            WriteEntitiesToFile(strDestinationPath, strFilePrefix, true, lstSpecies, headerMap, null, null, valueMap, omittedProperties);
        }

        /// <summary>
        /// Save cruise data to csv file
        /// </summary>
        public void SaveDataToCSVCombined(string strDestinationPath, string strFilePrefix, List<Cruise> lstCruises, bool blnFirstFile)
        {
            InitializeExport(strDestinationPath);

            //Store data in csv files.
            SaveCombinedData(strDestinationPath, strFilePrefix, blnFirstFile, lstCruises);

            SaveIndividData(strDestinationPath, strFilePrefix, blnFirstFile, lstCruises);
        }


        public void SaveDataToCSVTables(string strDestinationPath, string strFilePrefix, List<Cruise> lstCruises, bool blnFirstFile)
        {
            InitializeExport(strDestinationPath);

            //Write raw data tables
            WriteEntitiesToFile<Cruise>(strDestinationPath, strFilePrefix, blnFirstFile, lstCruises, new Dictionary<string, string>() { {"cruise1","cruise"} }, null, null, null, "Trips", "ChangeTracker");
            var lstTrips = lstCruises.SelectMany(x => x.Trips).ToList();
            WriteEntitiesToFile<Trip>(strDestinationPath, strFilePrefix, blnFirstFile, lstTrips, new Dictionary<string, string>() { { "trip1", "trip" } }, null, null, null, "Cruise1", "Samples", "ChangeTracker");
            var lstSamples = lstTrips.SelectMany(x => x.Samples).ToList();
            WriteEntitiesToFile<Sample>(strDestinationPath, strFilePrefix, blnFirstFile, lstSamples, null, null, null, null, "Trip1", "SpeciesListRaiseds", "SpeciesLists", "ChangeTracker");
            var lstSpeciesLists = lstSamples.SelectMany(x => x.SpeciesLists).ToList();
            WriteEntitiesToFile<SpeciesList>(strDestinationPath, strFilePrefix, blnFirstFile, lstSpeciesLists, null, null, null, null, "Sample", "Animals", "ChangeTracker");
            var lstAnimals = lstSpeciesLists.SelectMany(x => x.Animals).ToList();
            WriteEntitiesToFile<Animal>(strDestinationPath, strFilePrefix, blnFirstFile, lstAnimals, null, GetExtraAnimalHeaders, GetExtraAnimalData, null, "SpeciesList", "R_AnimalReference", "R_AnimalPictureReference", "Ages", "ChangeTracker");
            var lstAges = lstAnimals.SelectMany(x => x.Ages).ToList();
            WriteEntitiesToFile<Age>(strDestinationPath, strFilePrefix, blnFirstFile, lstAges, null, null, null, null, "Animal", "ChangeTracker");

            //Write raised tables
            var lstSLRaised = lstSamples.SelectMany(x => x.SpeciesListRaiseds).ToList();
            WriteEntitiesToFile<SpeciesListRaised>(strDestinationPath, strFilePrefix, blnFirstFile, lstSLRaised, null, null, null, null, "Sample", "AnimalRaiseds", "ChangeTracker");
            var lstAnimalRaised = lstSLRaised.SelectMany(x => x.AnimalRaiseds).ToList();
            WriteEntitiesToFile<AnimalRaised>(strDestinationPath, strFilePrefix, blnFirstFile, lstAnimalRaised, null, null, null, null,"animalRaisedId", "SpeciesListRaised", "ChangeTracker");

        }


        private List<string> GetExtraAnimalHeaders()
        {
            return new List<string> { "referenceIds", "referenceNames", "pictureReferences" };
        }

        private List<string> GetExtraAnimalData(Animal a)
        {
            return new List<string>() { (a.R_AnimalReference == null || a.R_AnimalReference.Count == 0) ? null : string.Join(", ", a.R_AnimalReference.Select(x => x.referenceId)), (a.R_AnimalReference == null || a.R_AnimalReference.Count == 0) ? null : string.Join(", ", a.R_AnimalReference.Select(x => x.referenceName)), (a.R_AnimalPictureReference == null || a.R_AnimalPictureReference.Count == 0) ? null : string.Join(", ", a.R_AnimalPictureReference.Select(x => x.pictureReference)) };
        }


        /// <summary>
        /// Write entities to disk.
        /// </summary>
        /// <param name="strDestinationPath">Directory path to file destination</param>
        /// <param name="strFilePrefix">File prefix (creation date for example)</param>
        /// <param name="lstData">List of entities to wite to disk</param>
        /// <param name="headerMap">Map a column header name to another header name</param>
        /// <param name="extraHeaders">Add extra column headers to the end of the row</param>
        /// <param name="extraData">Add extra column values to the end of the row</param>
        /// <param name="omitProperties">Dont write properties with the entity property-names given in omitProperties to disk</param>
        private void WriteEntitiesToFile<T>(string strDestinationPath, string strFilePrefix, bool blnFirstFile, List<T> lstData, Dictionary<string, string> headerMap,
                                            Func<List<string>> extraHeaders, Func<T, List<string>> extraData, Dictionary<string, string> valueMap,
                                            params string[] omitProperties)
        {
            string strFilePath = Path.Combine(strDestinationPath, string.Format("{0}{1}.csv", typeof(T).Name, strFilePrefix));

            //Retrieve all properties of the entity
            var allProperties = typeof(T).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            //Filter away unwanted properties
            var dic = omitProperties.ToHashSet();
            var filteredProperties = allProperties.Where(x => !dic.Contains(x.Name)).ToList();

            //Make sure file is deleted, if it already exists
            if (blnFirstFile && File.Exists(strFilePath))
                File.Delete(strFilePath);

            using (FileStream fs = new FileStream(strFilePath, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    int intExtraHeaders = extraHeaders != null ? extraHeaders().Count : 0;

                    //Write header
                    if (fs.Length == 0)
                    {
                        //Retrieve file header, and use header map to rename som columns.
                        List<string> lstHeader = filteredProperties.Select(x => (headerMap != null && headerMap.ContainsKey(x.Name)) ? headerMap[x.Name] : x.Name).ToList();

                        //Append extra headers
                        if (extraHeaders != null && intExtraHeaders > 0)
                            lstHeader.AddRange(extraHeaders());

                        var arrHeader = lstHeader.ToArray();

                        WriteData(sw, arrHeader.Length, arrHeader);
                    }

                    for (int i = 0; i < lstData.Count; i++)
                    {
                        string[] arrDataRow = new string[filteredProperties.Count + intExtraHeaders];
                        int j = 0;
                        for (j = 0; j < filteredProperties.Count; j++)
                        {
                            var p = filteredProperties[j];
                            PropertyInfo pTmp = null;
                            if (valueMap != null && valueMap.ContainsKey(p.Name) && (pTmp = allProperties.Where(x => x.Name.Equals(valueMap[p.Name])).FirstOrDefault()) != null)
                                p = pTmp;

                                object val = p.GetValue(lstData[i], new object[] { });
                                arrDataRow[j] = GetValue(val);
                        }

                        //Append extra data
                        if (extraData != null)
                        {
                            var lstExtraData = extraData(lstData[i]);
                            for (int x = 0; j < arrDataRow.Length; x++, j++)
                                arrDataRow[j] = lstExtraData[x];
                        }

                        WriteData(sw, arrDataRow.Length, arrDataRow);
                    }
                }
            }
        }



        private void SaveCombinedData(string strDestinationPath, string strFilePrefix, bool blnFirstFile, List<Cruise> lstCruises)
        {
            string strRawDataFileName = Path.Combine(strDestinationPath, "rådata" + strFilePrefix + ".csv");
            string strRaisedDataFileName = Path.Combine(strDestinationPath, "opgangetdata" + strFilePrefix + ".csv");

            //Make sure file is deleted, if it already exists
            if (blnFirstFile && File.Exists(strRawDataFileName))
                File.Delete(strRawDataFileName);

            //Make sure file is deleted, if it already exists
            if (blnFirstFile && File.Exists(strRaisedDataFileName))
                File.Delete(strRaisedDataFileName);

            using (FileStream fsRaw = new FileStream(strRawDataFileName, FileMode.Append, FileAccess.Write))
            {
                using (FileStream fsRaised = new FileStream(strRaisedDataFileName, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter swRaw = new StreamWriter(fsRaw, Encoding.UTF8))
                    {
                        using (StreamWriter swRaised = new StreamWriter(fsRaised, Encoding.UTF8))
                        {
                            int intRawDataColumnsCount = WriteRawDataHeader(fsRaw.Length, swRaw);
                            int intRaisedDataColumnsCount = WriteRaisedDataHeader(fsRaised.Length, swRaised);

                            foreach (var cruise in lstCruises)
                            {
                                List<string> lstRawData = new List<string>();

                                WriteCruiseRawData(cruise, intRawDataColumnsCount, swRaw, intRaisedDataColumnsCount, swRaised);
                            }

                            /*
                              if (fsRaw.Length == 0)
                                  sw.WriteLine(String.Join(";", "SampleId", "Year", "Cruise", "Trip", "Station", "SpeciesCode", "LandingCategory", "dfuBase_Category", "SexCode", "SizeSortingDFU", "SizeSortingEU", "Ovigorous", "CuticualHardness", "Treament", "DfuArea", "StatRect", "WeightStep0", "WeightStep1", "WeightStep2", "WeightStep3", "LandingWeightStep0", "LandingWeightStep1", "RaisingFactor"));
                              foreach (var sl in lstCruises.SelectMany(x => x.Trips).SelectMany(x => x.Sample).SelectMany(x => x.SpeciesLists))
                              {
                                  sw.WriteLine(String.Join(";", GetValue(sl.sampleId), GetValue(sl.year), GetValue(sl.cruise), GetValue(sl.trip), GetValue(sl.station), GetValue(sl.speciesCode), GetValue(sl.landingCategory), GetValue(sl.dfuBase_Category), GetValue(sl.sexCode), GetValue(sl.sizeSortingDFU), GetValue(sl.sizeSortingEU), GetValue(sl.ovigorous), GetValue(sl.cuticulaHardness), GetValue(sl.treatment), GetValue(sl.dfuArea), GetValue(sl.statisticalRectangle), GetValue(sl.weightStep0), GetValue(sl.weightStep1), GetValue(sl.weightStep2), GetValue(sl.weightStep3), GetValue(sl.landingWeightStep0), GetValue(sl.landingWeightStep1), GetValue(sl.raisingFactor)));
                              }
                            */
                        }
                    }
                }
            }
        }


        private void SaveIndividData(string strDestinationPath, string strFilePrefix, bool blnFirstFile, List<Cruise> lstCruises)
        {
            string strIndividDataFileName = Path.Combine(strDestinationPath, "individdata" + strFilePrefix + ".csv");

            //Make sure file is deleted, if it already exists
            if (blnFirstFile && File.Exists(strIndividDataFileName))
                File.Delete(strIndividDataFileName);

            using (FileStream fsIndivid = new FileStream(strIndividDataFileName, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter swIndivid = new StreamWriter(fsIndivid, Encoding.UTF8))
                {
                   // var ages = lstCruises.SelectMany(x => x.Trips).SelectMany(x => x.Samples).SelectMany(x => x.SpeciesLists).SelectMany(x => x.Animals).SelectMany(x => x.Ages);

                    var lstAnimals = lstCruises.SelectMany(x => x.Trips).SelectMany(x => x.Samples).SelectMany(x => x.SpeciesLists)
                                               .SelectMany(x => x.Animals)
                                               .Where(x => (x.individNum.HasValue && x.individNum.Value > 0) || (x.Ages != null && x.Ages.Where(y => y.age1.HasValue).Any())).ToList();

                   // var lstAnimals = ages.Where(a => (a.individNum.HasValue && a.individNum.Value != 0) ||
                   //                                     (a.age1.HasValue && a.individNum.HasValue && a.individNum.Value == 0) ||
                   //                                     (a.age1.HasValue && !a.individNum.HasValue)).ToList();
                    WriteIndividData(lstAnimals, swIndivid, fsIndivid.Length);
                }
            }
        }


        private void WriteIndividData(List<Animal> lstAnimals, StreamWriter swIndivid, long lngFileLength)
        {
            var arrHeader = new string[] {    "Animal.animalId", 
                                              "Animal.animalInfoId",
                                              "Animal.speciesListId",
                                              "Animal.year",
                                              "Animal.cruise",
                                              "Animal.trip",
                                              "Animal.tripType",
                                              "Animal.station", 
                                              "Animal.dateGearStart",
                                              "Animal.quarterGearStart",
                                              "Animal.dfuArea",
                                              "Animal.statisticalRectangle",
                                              "Animal.gearQuality",
                                              "Animal.gearType",
                                              "Animal.meshSize",
                                              "Animal.speciesCode", 
                                              "Animal.stock",
                                              "Animal.landingCategory",
                                              "Animal.dfuBase_Category",
                                              "Animal.sizeSortingEU",
                                              "Animal.sizeSortingDFU",
                                              "Animal.ovigorous",
                                              "Animal.cuticulaHardness",
                                              "Animal.treatment", 
                                              "Animal.speciesList_sexCode",
                                              "Animal.sexCode",
                                              "Animal.representative",
                                              "Animal.individNum",
                                              "Animal.number",
                                              "Animal.speciesList_number",
                                              "Animal.length",
                                              "Animal.lengthMeasureUnit",
                                              "Animal.lengthMeasureType",
                                              "Animal.weight",
                                              "Animal.treatmentFactor",
                                              "Animal.maturityIndex",
                                              "Animal.maturityIndexMethod",
                                              "Animal.broodingPhase", 
                                              "Animal.weightGutted",
                                              "Animal.weightLiver",
                                              "Animal.weightGonads",
                                              "Animal.parasiteCode",
                                              "Animal.fatIndex",
                                              "Animal.fatIndexMethod",
                                              "Animal.numVertebra", 
                                              "Animal.maturityReaderId",
                                              "Animal.maturityReader",
                                              "Animal.remark",
                                              "Animal.animalInfo_remark",
                                              "Animal.referenceIds",
                                              "Animal.referenceNames",
                                              "Animal.pictureReferences",
                                              "Animal.catchNum",
                                              "Animal.otolithFinScale",

                                              "Age.ageId",
                                              "Age.age",
                                              "Age.number",
                                              "Age.agePlusGroup",
                                              "Age.otolithWeight",
                                              "Age.edgeStructure",
                                              "Age.otolithReadingRemark",
                                              "Age.hatchMonth",
                                              "Age.hatchMonthRemark",
                                              "Age.ageReadId",
                                              "Age.ageReadName",
                                              "Age.hatchMonthReaderId",
                                              "Age.hatchMonthReaderName",
                                              "Age.remark",
                                              "Age.genetics",
                                              "Age.visualStock",
                                              "Age.geneticStock",
                                              "Age.sdAgeInfoUpdated",
                                              "Age.sdAgeReadId",
                                              "Age.sdAgeReadName",
                                              "Age.sdAnnotationId"
                                              };

            //Only write header, if it has not already been written.
            if(lngFileLength == 0)
                WriteData(swIndivid, arrHeader.Length, arrHeader);

            foreach (var ani in lstAnimals)
            {
                var arr = new string[arrHeader.Length];
                int intIndex = 0;

                arr[intIndex++] = GetValue(ani.animalId);
                arr[intIndex++] = GetValue(ani.animalInfoId);
                arr[intIndex++] = GetValue(ani.speciesListId);
                arr[intIndex++] = GetValue(ani.year);
                arr[intIndex++] = GetValue(ani.cruise);
                arr[intIndex++] = GetValue(ani.trip);
                arr[intIndex++] = GetValue(ani.tripType);
                arr[intIndex++] = GetValue(ani.station);
                arr[intIndex++] = GetValue(ani.dateGearStart);
                arr[intIndex++] = GetValue(ani.quarterGearStart);
                arr[intIndex++] = GetValue(ani.dfuArea);
                arr[intIndex++] = GetValue(ani.statisticalRectangle);
                arr[intIndex++] = GetValue(ani.gearQuality);
                arr[intIndex++] = GetValue(ani.gearType);
                arr[intIndex++] = GetValue(ani.meshSize);
                arr[intIndex++] = GetValue(ani.speciesCode);
                arr[intIndex++] = GetValue(ani.stock);
                arr[intIndex++] = GetValue(ani.landingCategory);
                arr[intIndex++] = GetValue(ani.dfuBase_Category);
                arr[intIndex++] = GetValue(ani.sizeSortingEU);
                arr[intIndex++] = GetValue(ani.sizeSortingDFU);
                arr[intIndex++] = GetValue(ani.ovigorous);
                arr[intIndex++] = GetValue(ani.cuticulaHardness);
                arr[intIndex++] = GetValue(ani.treatment);
                arr[intIndex++] = GetValue(ani.speciesList_sexCode);
                arr[intIndex++] = GetValue(ani.sexCode);
                arr[intIndex++] = GetValue(ani.representative);
                arr[intIndex++] = GetValue(ani.individNum);
                arr[intIndex++] = GetValue(ani.number);
                arr[intIndex++] = GetValue(ani.speciesList_number);
                arr[intIndex++] = GetValue(ani.length);
                arr[intIndex++] = GetValue(ani.lengthMeasureUnit);
                arr[intIndex++] = GetValue(ani.lengthMeasureType);
                arr[intIndex++] = GetValue(ani.weight);
                arr[intIndex++] = GetValue(ani.treatmentFactor);
                arr[intIndex++] = GetValue(ani.maturityIndex);
                arr[intIndex++] = GetValue(ani.maturityIndexMethod);
                arr[intIndex++] = GetValue(ani.broodingPhase);
                arr[intIndex++] = GetValue(ani.weightGutted);
                arr[intIndex++] = GetValue(ani.weightLiver);
                arr[intIndex++] = GetValue(ani.weightGonads);
                arr[intIndex++] = GetValue(ani.parasiteCode);
                arr[intIndex++] = GetValue(ani.fatIndex);
                arr[intIndex++] = GetValue(ani.fatIndexMethod);
                arr[intIndex++] = GetValue(ani.numVertebra);
                arr[intIndex++] = GetValue(ani.maturityReaderId);
                arr[intIndex++] = GetValue(ani.maturityReader);
                arr[intIndex++] = GetValue(ani.remark);
                arr[intIndex++] = GetValue(ani.animalInfo_remark);
                arr[intIndex++] = (ani.R_AnimalReference == null || ani.R_AnimalReference.Count == 0) ? null : string.Join(", ", ani.R_AnimalReference.Select(x => x.referenceId));
                arr[intIndex++] = (ani.R_AnimalReference == null || ani.R_AnimalReference.Count == 0) ? null : string.Join(", ", ani.R_AnimalReference.Select(x => x.referenceName));
                arr[intIndex++] = (ani.R_AnimalPictureReference == null || ani.R_AnimalPictureReference.Count == 0) ? null : string.Join(", ", ani.R_AnimalPictureReference.Select(x => x.pictureReference));
                arr[intIndex++] = GetValue(ani.catchNum);
                arr[intIndex++] = GetValue(ani.otolithFinScale);

                if (ani.Ages != null && ani.Ages.Where(x => x.age1.HasValue).Any())
                {
                    int intIndexTmp = intIndex;

                    foreach (var age in ani.Ages.Where(x => x.age1.HasValue))
                    {
                        intIndex = intIndexTmp;
                        arr[intIndex++] = GetValue(age.ageId);
                        arr[intIndex++] = GetValue(age.age1);
                        arr[intIndex++] = GetValue(age.number);
                        arr[intIndex++] = GetValue(age.agePlusGroup);
                        arr[intIndex++] = GetValue(age.otolithWeight);
                        arr[intIndex++] = GetValue(age.edgeStructure);
                        arr[intIndex++] = GetValue(age.otolithReadingRemark);
                        arr[intIndex++] = GetValue(age.hatchMonth);
                        arr[intIndex++] = GetValue(age.hatchMonthRemark);
                        arr[intIndex++] = GetValue(age.ageReadId);
                        arr[intIndex++] = GetValue(age.ageReadName);
                        arr[intIndex++] = GetValue(age.hatchMonthReaderId);
                        arr[intIndex++] = GetValue(age.hatchMonthReaderName);
                        arr[intIndex++] = GetValue(age.remark);
                        arr[intIndex++] = GetValue(age.genetics);
                        arr[intIndex++] = GetValue(age.visualStock);
                        arr[intIndex++] = GetValue(age.geneticStock);
                        arr[intIndex++] = GetValue(age.sdAgeInfoUpdated);
                        arr[intIndex++] = GetValue(age.sdAgeReadId);
                        arr[intIndex++] = GetValue(age.sdAgeReadName);
                        arr[intIndex++] = GetValue(age.sdAnnotationId);
                        WriteData(swIndivid, intIndex, arr);
                    }
                }
                else
                    WriteData(swIndivid, intIndex, arr);
            }
        }


        private void WriteCruiseRawData(Cruise c, int intRawDataColumnsCount, StreamWriter swRaw, int intRaisedDataColumnsCount, StreamWriter swRaised)
        {
            string[] arr = new string[intRawDataColumnsCount];
            string[] arrRaised = new string[intRaisedDataColumnsCount];

            int intIndex = 0;
            arrRaised[intIndex] = arr[intIndex++] = GetValue(c.cruiseId);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(c.year);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(c.cruise1);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(c.responsibleId);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(c.responsibleName);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(c.participants);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(c.summary);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(c.remark);

            //If no trip data exists, add the cruise only
            if (c.Trips == null || c.Trips.Count == 0)
            {
                WriteData(swRaw, intIndex, arr);
                WriteData(swRaised, intIndex, arrRaised);
            }
            else
            {
                foreach (var t in c.Trips)
                    WriteTripRawData(t, intIndex, arr, swRaw, arrRaised, swRaised);
            }
        }


        private void WriteTripRawData(Trip t, int intIndex, string[] arr, StreamWriter swRaw, string[] arrRaised, StreamWriter swRaised)
        {
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.tripId);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.trip1);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.tripType);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.logBldNr);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.timeZone);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.dateStart);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.dateEnd);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.samplingType);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.samplingMethod);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.fisheryType);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.platform1);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.nationalityPlatform1);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.fDFVesselPlatform1);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.nationalityPlatform2);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.platform2);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.dateSample);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.harbourSample);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.nationalityHarbourSample);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.harbourLanding);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.nationalityHarbourLanding);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.contactPersonId);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.contactPersonName);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.tripLeaderId);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.tripLeaderName);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.dataHandlerId);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.dataHandlerName);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.remark);
            //REKREA
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.sgTripId);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.tripNum);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.placeName);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.placeCode);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.postalCode);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.numberInPlace);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.respYes);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.respNo);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(t.respTot);

            //If no trip data exists, add the cruise only
            if (t.Samples == null || t.Samples.Count == 0)
            {
                WriteData(swRaw, intIndex, arr);
                WriteData(swRaised, intIndex, arrRaised);
            }
            else
            {
                foreach (var s in t.Samples)
                    WriteSampleRawData(s, intIndex, arr, swRaw, arrRaised, swRaised);
            }
        }


        private void WriteSampleRawData(Sample s, int intIndex, string[] arr, StreamWriter swRaw, string[] arrRaised, StreamWriter swRaised)
        {
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.sampleId);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.station);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.stationName);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.labJournalNum);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.gearQuality);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.hydroStnRef);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.haulType);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.dateGearStart);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.dateGearEnd);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.timeZone);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.quarterGearStart);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.fishingtime);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.latPosStartText);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.lonPosStartText);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.latPosEndText);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.lonPosEndText);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.latPosStartDec);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.lonPosStartDec);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.latPosEndDec);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.lonPosEndDec);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.dfuArea);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.dfuAreaEnd);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.statisticalRectangle);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.statisticalRectangleEnd);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.distancePositions);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.distanceBottom);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.courseTrack);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.targetSpecies1);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.targetSpecies2);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.targetSpecies3);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.catchRegistration);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.speciesRegistration);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.gearType);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.selectionDevice);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.selectionDeviceSource);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.meshSize);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.netOpening);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.otterBoardDist);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.numberTrawls);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.wireLength);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.wingSpread);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.numNets);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.lostNets);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.heightNets);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.lengthNets);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.lengthRopeFlyer);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.widthRopeFlyer);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.numberHooks);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.lengthBeam);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.depthAveGear);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.haulDirection);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.haulSpeedBot);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.haulSpeedWat);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.gearRemark);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.depthAvg);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.bottomType);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.windDirection);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.windSpeed);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.currentDirectionSrf);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.currentSpeedSrf);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.currentDirectionBot);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.currentSpeedBot);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.waveDirection);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.waveHeigth);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.thermoCline);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.thermoClineDepth);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.temperatureSrf);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.temperatureBot);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.oxygenSrf);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.oxygenBot);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.salinitySrf);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.salinityBot);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.samplePersonId);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.samplePersonName);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.analysisPersonId);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.analysisPersonName);
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.remark);   //77  
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.sgId);   //77  
            arrRaised[intIndex] = arr[intIndex++] = GetValue(s.weekdayWeekend);   //77  

            //If no trip data exists, add the cruise only
            if (s.SpeciesLists == null || s.SpeciesLists.Count == 0)
                WriteData(swRaw, intIndex, arr);
            else
                foreach (var sl in s.SpeciesLists)
                    WriteSpeciesListRawData(sl, intIndex, arr, swRaw);

            if (s.SpeciesListRaiseds == null || s.SpeciesListRaiseds.Count == 0)
                WriteData(swRaised, intIndex, arrRaised);
            else
                foreach (var sl in s.SpeciesListRaiseds)
                    WriteSpeciesListRaisedData(sl, intIndex, arrRaised, swRaised);     
        }


        private void WriteSpeciesListRaisedData(SpeciesListRaised sl, int intIndex, string[] arr, StreamWriter sw)
        {
            arr[intIndex++] = GetValue(sl.speciesListRaisedId);
            arr[intIndex++] = GetValue(sl.speciesCode);
            arr[intIndex++] = GetValue(sl.stock);
            arr[intIndex++] = GetValue(sl.landingCategory);
            arr[intIndex++] = GetValue(sl.dfuBase_Category);
            arr[intIndex++] = GetValue(sl.sizeSortingEU);
            arr[intIndex++] = GetValue(sl.sizeSortingDFU);
            arr[intIndex++] = GetValue(sl.sexCode);
            arr[intIndex++] = GetValue(sl.cuticulaHardness);
            arr[intIndex++] = GetValue(sl.ovigorous);
            arr[intIndex++] = GetValue(sl.weightSubSample);
            arr[intIndex++] = GetValue(sl.weightTotal);
            arr[intIndex++] = GetValue(sl.numberSubSample);
            arr[intIndex++] = GetValue(sl.numberTotal);

            //If no trip data exists, add the cruise only
            if (sl.AnimalRaiseds == null || sl.AnimalRaiseds.Count == 0)
                WriteData(sw, intIndex, arr);
            else
            {
                foreach (var a in sl.AnimalRaiseds)
                    WriteAnimalRaised(a, intIndex, arr, sw);
            }     
        }

        private void WriteAnimalRaised(AnimalRaised a, int intIndex, string[] arr, StreamWriter sw)
        {
            //animalRaisedId should not be included, since it is always 0 (the database normally creates this, when inserting AnimalRaised records in DW)
            //arr[intIndex++] = GetValue(a.animalRaisedId);
            arr[intIndex++] = GetValue(a.sexCode);
            arr[intIndex++] = GetValue(a.broodingPhase);
            arr[intIndex++] = GetValue(a.length);
            arr[intIndex++] = GetValue(a.lengthMeasureUnit);
            arr[intIndex++] = GetValue(a.lengthMeasureType);
            arr[intIndex++] = GetValue(a.numberSumSamplePerLength);
            arr[intIndex++] = GetValue(a.numberTotalPerLength);
            arr[intIndex++] = GetValue(a.weightMean);

            WriteData(sw, intIndex, arr);
        }

        private void WriteSpeciesListRawData(SpeciesList sl, int intIndex, string[] arr, StreamWriter swRaw)
        {
            arr[intIndex++] = GetValue(sl.speciesListId);
            arr[intIndex++] = GetValue(sl.speciesCode);
            arr[intIndex++] = GetValue(sl.stock);
            arr[intIndex++] = GetValue(sl.landingCategory);
            arr[intIndex++] = GetValue(sl.dfuBase_Category);
            arr[intIndex++] = GetValue(sl.sizeSortingEU);
            arr[intIndex++] = GetValue(sl.sizeSortingDFU);
            arr[intIndex++] = GetValue(sl.sexCode);
            arr[intIndex++] = GetValue(sl.ovigorous);
            arr[intIndex++] = GetValue(sl.cuticulaHardness);
            arr[intIndex++] = GetValue(sl.treatment);
            arr[intIndex++] = GetValue(sl.weightStep0);
            arr[intIndex++] = GetValue(sl.weightStep1);
            arr[intIndex++] = GetValue(sl.weightStep2);
            arr[intIndex++] = GetValue(sl.weightStep3);
            arr[intIndex++] = GetValue(sl.landingWeightStep0);
            arr[intIndex++] = GetValue(sl.landingWeightStep1);
            arr[intIndex++] = GetValue(sl.treatmentFactor);
            arr[intIndex++] = GetValue(sl.raisingFactor);
            arr[intIndex++] = GetValue(sl.wemSpeciesList);
            arr[intIndex++] = GetValue(sl.weightNonRep);
            arr[intIndex++] = GetValue(sl.bmsNonRep);
            arr[intIndex++] = GetValue(sl.number);
            arr[intIndex++] = GetValue(sl.totalWeight);
            arr[intIndex++] = GetValue(sl.wemTotalWeight);
            arr[intIndex++] = GetValue(sl.remark);
            arr[intIndex++] = GetValue(sl.applicationCode);

            //If no trip data exists, add the cruise only
            if (sl.Animals == null || sl.Animals.Count == 0)
                WriteData(swRaw, intIndex, arr);
            else
            {
                foreach (var a in sl.Animals)
                    WriteAnimalRawData(a, intIndex, arr, swRaw);
            }      
        }

        private void WriteAnimalRawData(Animal a, int intIndex, string[] arr, StreamWriter swRaw)
        {
            arr[intIndex++] = GetValue(a.animalId);
            arr[intIndex++] = GetValue(a.animalInfoId);
            arr[intIndex++] = GetValue(a.sexCode);
            arr[intIndex++] = GetValue(a.representative);
            arr[intIndex++] = GetValue(a.individNum);
            arr[intIndex++] = GetValue(a.number);
            arr[intIndex++] = GetValue(a.length);
            arr[intIndex++] = GetValue(a.lengthMeasureUnit);
            arr[intIndex++] = GetValue(a.lengthMeasureType);
            arr[intIndex++] = GetValue(a.weight);
            arr[intIndex++] = GetValue(a.maturityIndex);
            arr[intIndex++] = GetValue(a.maturityIndexMethod);
            arr[intIndex++] = GetValue(a.broodingPhase);
            arr[intIndex++] = GetValue(a.weightGutted);
            arr[intIndex++] = GetValue(a.weightLiver);
            arr[intIndex++] = GetValue(a.weightGonads);
            arr[intIndex++] = GetValue(a.parasiteCode);
            arr[intIndex++] = GetValue(a.fatIndex);
            arr[intIndex++] = GetValue(a.fatIndexMethod);
            arr[intIndex++] = GetValue(a.numVertebra);
            arr[intIndex++] = GetValue(a.maturityReaderId);
            arr[intIndex++] = GetValue(a.maturityReader);
            arr[intIndex++] = GetValue(a.remark);
            arr[intIndex++] = GetValue(a.animalInfo_remark);
            arr[intIndex++] = (a.R_AnimalReference == null || a.R_AnimalReference.Count == 0) ? null : string.Join(", ", a.R_AnimalReference.Select(x => x.referenceId));
            arr[intIndex++] = (a.R_AnimalReference == null || a.R_AnimalReference.Count == 0) ? null : string.Join(", ", a.R_AnimalReference.Select(x => x.referenceName));
            arr[intIndex++] = (a.R_AnimalPictureReference == null || a.R_AnimalPictureReference.Count == 0) ? null : string.Join(", ", a.R_AnimalPictureReference.Select(x => x.pictureReference));
            arr[intIndex++] = GetValue(a.catchNum);
            arr[intIndex++] = GetValue(a.otolithFinScale);

            //If no trip data exists, add the cruise only
            if (a.Ages == null || a.Ages.Count == 0)
                WriteData(swRaw, intIndex, arr);
            else
            {
                foreach (var age in a.Ages)
                    WriteAgeRawData(age, intIndex, arr, swRaw);
            }      
        }

        private void WriteAgeRawData(Age a, int intIndex, string[] arr, StreamWriter swRaw)
        {
            arr[intIndex++] = GetValue(a.ageId);
            arr[intIndex++] = GetValue(a.age1);
            arr[intIndex++] = GetValue(a.number);
            arr[intIndex++] = GetValue(a.agePlusGroup);
            arr[intIndex++] = GetValue(a.otolithWeight);
            arr[intIndex++] = GetValue(a.edgeStructure);
            arr[intIndex++] = GetValue(a.otolithReadingRemark);
            arr[intIndex++] = GetValue(a.hatchMonth);
            arr[intIndex++] = GetValue(a.hatchMonthRemark);
            arr[intIndex++] = GetValue(a.ageReadId);
            arr[intIndex++] = GetValue(a.ageReadName);
            arr[intIndex++] = GetValue(a.hatchMonthReaderId);
            arr[intIndex++] = GetValue(a.hatchMonthReaderName);
            arr[intIndex++] = GetValue(a.remark);
            arr[intIndex++] = GetValue(a.genetics);
            arr[intIndex++] = GetValue(a.visualStock);
            arr[intIndex++] = GetValue(a.geneticStock);
            arr[intIndex++] = GetValue(a.sdAgeInfoUpdated);
            arr[intIndex++] = GetValue(a.sdAgeReadId);
            arr[intIndex++] = GetValue(a.sdAgeReadName);
            arr[intIndex++] = GetValue(a.sdAnnotationId);

            WriteData(swRaw, intIndex, arr);
        }

        private int WriteRawDataHeader(long intFileStreamLength, StreamWriter sw)
        {
           var arrHeader = new string[] { "Cruise.cruiseId", //167
                                          "Cruise.year",
                                          "Cruise.cruise",
                                          "Cruise.responsibleId",
                                          "Cruise.responsibleName",
                                          "Cruise.participants",
                                          "Cruise.summary",
                                          "Cruise.remark",

                                          "Trip.tripId", //159
                                          "Trip.trip",
                                          "Trip.tripType",
                                          "Trip.logBldNr",
                                          "Trip.timeZone",
                                          "Trip.dateStart",
                                          "Trip.dateEnd",
                                          "Trip.samplingType",
                                          "Trip.samplingMethod",
                                          "Trip.fisheryType",
                                          "Trip.platform1",
                                          "Trip.nationalityPlatform1",
                                          "Trip.fDFVesselPlatform1",
                                          "Trip.nationalityPlatform2",
                                          "Trip.platform2",
                                          "Trip.dateSample",
                                          "Trip.harbourSample",
                                          "Trip.nationalityHarbourSample",
                                          "Trip.harbourLanding",
                                          "Trip.nationalityHarbourLanding",
                                          "Trip.contactPersonId",
                                          "Trip.contactPersonName",
                                          "Trip.tripLeaderId",
                                          "Trip.tripLeaderName",
                                          "Trip.dataHandlerId",
                                          "Trip.dataHandlerName",
                                          "Trip.remark",
                                          "Trip.sgTripId",
                                          "Trip.tripNum",
                                          "Trip.placeName",
                                          "Trip.placeCode",
                                          "Trip.postalCode",
                                          "Trip.numberInPlace",
                                          "Trip.respYes",
                                          "Trip.respNo",
                                          "Trip.respTot",

                                          "Sample.sampleId", //135
                                          "Sample.station",
                                          "Sample.stationName",
                                          "Sample.labJournalNum",
                                          "Sample.gearQuality",
                                          "Sample.hydroStnRef",
                                          "Sample.haulType",
                                          "Sample.dateGearStart",
                                          "Sample.dateGearEnd",
                                          "Sample.timeZone",
                                          "Sample.quarterGearStart",
                                          "Sample.fishingtime",
                                          "Sample.latPosStartText",
                                          "Sample.lonPosStartText",
                                          "Sample.latPosEndText",
                                          "Sample.lonPosEndText",
                                          "Sample.latPosStartDec",
                                          "Sample.lonPosStartDec",
                                          "Sample.latPosEndDec",
                                          "Sample.lonPosEndDec",
                                          "Sample.dfuArea",
                                          "Sample.dfuAreaEnd",
                                          "Sample.statisticalRectangle",
                                          "Sample.statisticalRectangleEnd",
                                          "Sample.distancePositions",
                                          "Sample.distanceBottom",
                                          "Sample.courseTrack",
                                          "Sample.targetSpecies1",
                                          "Sample.targetSpecies2",
                                          "Sample.targetSpecies3",
                                          "Sample.catchRegistration",
                                          "Sample.speciesRegistration",
                                          "Sample.gearType",
                                          "Sample.selectionDevice", //100
                                          "Sample.selectionDeviceSource",
                                          "Sample.meshSize",
                                          "Sample.netOpening",
                                          "Sample.otterBoardDist",
                                          "Sample.numberTrawls",
                                          "Sample.wireLength",
                                          "Sample.wingSpread",
                                          "Sample.numNets",
                                          "Sample.lostNets",
                                          "Sample.heightNets",
                                          "Sample.lengthNets",
                                          "Sample.lengthRopeFlyer",
                                          "Sample.widthRopeFlyer",
                                          "Sample.numberHooks",
                                          "Sample.lengthBeam",
                                          "Sample.depthAveGear",
                                          "Sample.haulDirection",
                                          "Sample.haulSpeedBot",
                                          "Sample.haulSpeedWat",
                                          "Sample.gearRemark",
                                          "Sample.depthAvg",
                                          "Sample.bottomType",
                                          "Sample.windDirection",
                                          "Sample.windSpeed",
                                          "Sample.currentDirectionSrf",
                                          "Sample.currentSpeedSrf",
                                          "Sample.currentDirectionBot",
                                          "Sample.currentSpeedBot",
                                          "Sample.waveDirection",
                                          "Sample.waveHeigth",
                                          "Sample.thermoCline",
                                          "Sample.thermoClineDepth",
                                          "Sample.temperatureSrf",
                                          "Sample.temperatureBot",
                                          "Sample.oxygenSrf",
                                          "Sample.oxygenBot",
                                          "Sample.salinitySrf",
                                          "Sample.salinityBot",
                                          "Sample.samplePersonId",
                                          "Sample.samplePersonName",
                                          "Sample.analysisPersonId",
                                          "Sample.analysisPersonName",
                                          "Sample.remark",
                                          "Sample.sgId",
                                          "Sample.weekdayWeekend",

                                          "SpeciesList.speciesListId", //58
                                          "SpeciesList.speciesCode",
                                          "SpeciesList.stock",
                                          "SpeciesList.landingCategory",
                                          "SpeciesList.dfuBase_Category",
                                          "SpeciesList.sizeSortingEU",
                                          "SpeciesList.sizeSortingDFU",
                                          "SpeciesList.sexCode",
                                          "SpeciesList.ovigorous",
                                          "SpeciesList.cuticulaHardness",
                                          "SpeciesList.treatment",
                                          "SpeciesList.weightStep0",
                                          "SpeciesList.weightStep1",
                                          "SpeciesList.weightStep2",
                                          "SpeciesList.weightStep3",
                                          "SpeciesList.landingWeightStep0",
                                          "SpeciesList.landingWeightStep1",
                                          "SpeciesList.treatmentFactor",
                                          "SpeciesList.raisingFactor",
                                          "SpeciesList.wemSpeciesList",
                                          "SpeciesList.weightNonRep",
                                          "SpeciesList.bmsNonRep",
                                          "SpeciesList.number",
                                          "SpeciesList.totalWeight",
                                          "SpeciesList.wemTotalWeight",
                                          "SpeciesList.remark",
                                          "SpeciesList.applicationCode",

                                          "Animal.animalId", //34
                                          "Animal.animalInfoId",
                                          "Animal.sexCode",
                                          "Animal.representative",
                                          "Animal.individNum",
                                          "Animal.number",
                                          "Animal.length",
                                          "Animal.lengthMeasureUnit",
                                          "Animal.lengthMeasureType",
                                          "Animal.weight",
                                          "Animal.maturityIndex",
                                          "Animal.maturityIndexMethod",
                                          "Animal.broodingPhase",
                                          "Animal.weightGutted",
                                          "Animal.weightLiver",
                                          "Animal.weightGonads",
                                          "Animal.parasiteCode",
                                          "Animal.fatIndex",
                                          "Animal.fatIndexMethod",
                                          "Animal.numVertebra",
                                          "Animal.maturityReaderId",
                                          "Animal.maturityReader",
                                          "Animal.remark",
                                          "Animal.animalInfo_remark",
                                          "Animal.referenceIds",
                                          "Animal.referenceNames",
                                          "Animal.pictureReferences",
                                          "Animal.catchNum",
                                          "Animal.otolithFinScale",

                                          "Age.ageId",
                                          "Age.age",
                                          "Age.number",
                                          "Age.agePlusGroup",
                                          "Age.otolithWeight",
                                          "Age.edgeStructure",
                                          "Age.otolithReadingRemark",
                                          "Age.hatchMonth",
                                          "Age.hatchMonthRemark",
                                          "Age.ageReadId",
                                          "Age.ageReadName",
                                          "Age.hatchMonthReaderId",
                                          "Age.hatchMonthReaderName",
                                          "Age.remark", 
                                          "Age.genetics",
                                          "Age.visualStock",
                                          "Age.geneticStock",
                                          "Age.sdAgeInfoUpdated",
                                          "Age.sdAgeReadId",
                                          "Age.sdAgeReadName",
                                          "Age.sdAnnotationId"
                                        };

            int intColumnsCount = arrHeader.Length;

            if (intFileStreamLength > 0)
                return intColumnsCount;

            WriteData(sw, arrHeader.Length, arrHeader);

            return intColumnsCount;
        }

        private int WriteRaisedDataHeader(long intFileStreamLength, StreamWriter sw)
        {
            var arrHeader = new string[] { "Cruise.cruiseId", //167
                                          "Cruise.year",
                                          "Cruise.cruise",
                                          "Cruise.responsibleId",
                                          "Cruise.responsibleName",
                                          "Cruise.participants",
                                          "Cruise.summary",
                                          "Cruise.remark",

                                          "Trip.tripId", //159
                                          "Trip.trip",
                                          "Trip.tripType",
                                          "Trip.logBldNr",
                                          "Trip.timeZone",
                                          "Trip.dateStart",
                                          "Trip.dateEnd",
                                          "Trip.samplingType",
                                          "Trip.samplingMethod",
                                          "Trip.fisheryType",
                                          "Trip.platform1",
                                          "Trip.nationalityPlatform1",
                                          "Trip.fDFVesselPlatform1",
                                          "Trip.nationalityPlatform2",
                                          "Trip.platform2",
                                          "Trip.dateSample",
                                          "Trip.harbourSample",
                                          "Trip.nationalityHarbourSample",
                                          "Trip.harbourLanding",
                                          "Trip.nationalityHarbourLanding",
                                          "Trip.contactPersonId",
                                          "Trip.contactPersonName",
                                          "Trip.tripLeaderId",
                                          "Trip.tripLeaderName",
                                          "Trip.dataHandlerId",
                                          "Trip.dataHandlerName",
                                          "Trip.remark",
                                          "Trip.sgTripId",
                                          "Trip.tripNum",
                                          "Trip.placeName",
                                          "Trip.placeCode",
                                          "Trip.postalCode",
                                          "Trip.numberInPlace",
                                          "Trip.respYes",
                                          "Trip.respNo",
                                          "Trip.respTot",

                                          "Sample.sampleId", //135
                                          "Sample.station",
                                          "Sample.stationName",
                                          "Sample.labJournalNum",
                                          "Sample.gearQuality",
                                          "Sample.hydroStnRef",
                                          "Sample.haulType",
                                          "Sample.dateGearStart",
                                          "Sample.dateGearEnd",
                                          "Sample.timeZone",
                                          "Sample.quarterGearStart",
                                          "Sample.fishingtime",
                                          "Sample.latPosStartText",
                                          "Sample.lonPosStartText",
                                          "Sample.latPosEndText",
                                          "Sample.lonPosEndText",
                                          "Sample.latPosStartDec",
                                          "Sample.lonPosStartDec",
                                          "Sample.latPosEndDec",
                                          "Sample.lonPosEndDec",
                                          "Sample.dfuArea",
                                          "Sample.dfuAreaEnd",
                                          "Sample.statisticalRectangle",
                                          "Sample.statisticalRectangleEnd",
                                          "Sample.distancePositions",
                                          "Sample.distanceBottom",
                                          "Sample.courseTrack",
                                          "Sample.targetSpecies1",
                                          "Sample.targetSpecies2",
                                          "Sample.targetSpecies3",
                                          "Sample.catchRegistration",
                                          "Sample.speciesRegistration",
                                          "Sample.gearType",
                                          "Sample.selectionDevice", //100
                                          "Sample.selectionDeviceSource",
                                          "Sample.meshSize",
                                          "Sample.netOpening",
                                          "Sample.otterBoardDist",
                                          "Sample.numberTrawls",
                                          "Sample.wireLength",
                                          "Sample.wingSpread",
                                          "Sample.numNets",
                                          "Sample.lostNets",
                                          "Sample.heightNets",
                                          "Sample.lengthNets",
                                          "Sample.lengthRopeFlyer",
                                          "Sample.widthRopeFlyer",
                                          "Sample.numberHooks",
                                          "Sample.lengthBeam",
                                          "Sample.depthAveGear",
                                          "Sample.haulDirection",
                                          "Sample.haulSpeedBot",
                                          "Sample.haulSpeedWat",
                                          "Sample.gearRemark",
                                          "Sample.depthAvg",
                                          "Sample.bottomType",
                                          "Sample.windDirection",
                                          "Sample.windSpeed",
                                          "Sample.currentDirectionSrf",
                                          "Sample.currentSpeedSrf",
                                          "Sample.currentDirectionBot",
                                          "Sample.currentSpeedBot",
                                          "Sample.waveDirection",
                                          "Sample.waveHeigth",
                                          "Sample.thermoCline",
                                          "Sample.thermoClineDepth",
                                          "Sample.temperatureSrf",
                                          "Sample.temperatureBot",
                                          "Sample.oxygenSrf",
                                          "Sample.oxygenBot",
                                          "Sample.salinitySrf",
                                          "Sample.salinityBot",
                                          "Sample.samplePersonId",
                                          "Sample.samplePersonName",
                                          "Sample.analysisPersonId",
                                          "Sample.analysisPersonName",
                                          "Sample.remark",
                                          "Sample.sgId",
                                          "Sample.weekdayWeekend",

                                          "SpeciesListRaised.speciesListRaisedId",
                                          "SpeciesListRaised.speciesCode",
                                          "SpeciesListRaised.stock",
                                          "SpeciesListRaised.landingCategory",
                                          "SpeciesListRaised.dfuBase_Category",
                                          "SpeciesListRaised.sizeSortingEU",
                                          "SpeciesListRaised.sizeSortingDFU",
                                          "SpeciesListRaised.sexCode",
                                          "SpeciesListRaised.cuticulaHardness",
                                          "SpeciesListRaised.ovigorous",
                                          "SpeciesListRaised.weightSubSample",
                                          "SpeciesListRaised.weightTotal",
                                          "SpeciesListRaised.numberSubSample",
                                          "SpeciesListRaised.numberTotal",

                                          //animalRaisedId should not be included, since it is always 0 (the database normally creates this, when inserting AnimalRaised records in DW)
                                         /* "AnimalRaised.animalRaisedId",*/
                                          "AnimalRaised.sexCode",
                                          "AnimalRaised.broodingPhase",
                                          "AnimalRaised.length",
                                          "AnimalRaised.lengthMeasureUnit",
                                          "AnimalRaised.lengthMeasureType",
                                          "AnimalRaised.numberSumSamplePerLength",
                                          "AnimalRaised.numberTotalPerLength",
                                          "AnimalRaised.weightMean",
                                        };

            int intColumnsCount = arrHeader.Length;

            if (intFileStreamLength > 0)
                return intColumnsCount;

            WriteData(sw, arrHeader.Length, arrHeader);

            return intColumnsCount;
        }


        private string GetValue(int? i)
        {
            return string.Format("{0}", i.HasValue ? i.Value.ToString() : "");
        }

        private string GetValue(bool? bln)
        {
            return string.Format("{0}", bln.HasValue ? (bln.Value ? "1" : "0" ) : "");
        }

        private string GetValue(DateTime? dt)
        {
            string strDate = dt.HasValue ? dt.Value.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture) : "";
            return string.Format("{0}", strDate);
        }

        private string GetValue(decimal? i)
        {
            //Make sure delimiter is formatted with the one specified in _strDelimiter.
            return string.Format("{0}", i.HasValue ? i.Value.ToString(_nfi) : "");
        }


        private string GetValue(string s)
        {
            //Remove new lines and also the seperator character, so unexpected columns do not occur.
            return string.Format("{0}", s ?? "").Replace("\n", "").Replace("\r", "").Replace(_strSeperator, "");
        }


        private string GetValue(object o)
        {
            string res = "";

            if (o == null)
                return res;

            if (o is Nullable<int>)
                res = GetValue(o as Nullable<int>);
            else if (o is Nullable<bool>)
                res = GetValue(o as Nullable<bool>);
            else if (o is Nullable<DateTime>)
                res = GetValue(o as Nullable<DateTime>);
            else if (o is Nullable<decimal>)
                res = GetValue(o as Nullable<decimal>);
            else if (o is string)
                res = GetValue(o as string);
            else
                res = GetValue(o.ToString() as string);

            return res;
        }


        private void WriteData(StreamWriter sw, int intClearIndex, params string[] arrData)
        {
            for (int i = intClearIndex; i < arrData.Length; i++)
                arrData[i] = null;

            sw.WriteLine(String.Join(_strSeperator, arrData));
        }


    }
}
