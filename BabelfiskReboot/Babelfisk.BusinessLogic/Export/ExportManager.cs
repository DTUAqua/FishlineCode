using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.BusinessLogic.BabelfiskService;
using Babelfisk.Entities;
using Babelfisk.Entities.Sprattus;
using System.IO;
using Anchor.Core;

namespace Babelfisk.BusinessLogic.Export
{
    public partial class ExportManager
    {
        private Dictionary<int, L_LengthMeasureType> LengthMeasureType = new Dictionary<int, L_LengthMeasureType>();
        /// <summary>
        /// Export data to destination defined by strDetinationPath
        /// </summary>
        public List<Babelfisk.Warehouse.DWMessage> RaiseData(Action<DataExportStatus> statusCallback, List<int> lstCruiseIds, List<int> lstTripIds, List<int> lstSampleIds, out List<Babelfisk.Warehouse.Model.Cruise> lstResults)
        {
            //Return status (retrieving data)
            statusCallback(DataExportStatus.RetrievingData);

            //1) Retreive data to export (in memory)
            DataRetrievalManager datMan = new DataRetrievalManager();
            List<Cruise> lst = datMan.GetDataForRaising(lstCruiseIds, lstTripIds, lstSampleIds);

            var ldv = new LookupDataVersioning();

            //2) Assign lookups to data to be exported
            AssignLookupsForDataExport(ref lst, ldv);

            //3) Create DW tables and raise data
            statusCallback(DataExportStatus.RaiseData);

            List<Babelfisk.Warehouse.DWMessage> lstMessage = new List<Warehouse.DWMessage>();
            lstResults = CreateAndRaiseData(lst, ldv, ref lstMessage);

            lst = null;

            //Make sure cruises are collected since they can be quite big.
            GC.Collect();

            return lstMessage;
        }



        /// <summary>
        /// Loops through list of cruises and assigns any lookups needed for data export.
        /// </summary>
        private void AssignLookupsForDataExport(ref List<Cruise> lst, LookupDataVersioning ldv)
        {
            var lookupMan = new LookupManager();

            //Load lookups used for populating data to be exported
            var dicDFUPersons = lookupMan.GetLookups(typeof(DFUPerson), ldv).OfType<DFUPerson>().ToDictionary(x => x.dfuPersonId);
            var dicPersons = lookupMan.GetLookups(typeof(Person), ldv).OfType<Person>().ToDictionary(x => x.personId); 
            var dicSamplingTypes = lookupMan.GetLookups(typeof(L_SamplingType), ldv).OfType<L_SamplingType>().ToDictionary(x => x.samplingTypeId);
            var dicSamplingMethods = lookupMan.GetLookups(typeof(L_SamplingMethod), ldv).OfType<L_SamplingMethod>().ToDictionary(x => x.samplingMethodId);
            var dicHarbours = lookupMan.GetLookups(typeof(L_Harbour), ldv).OfType<L_Harbour>().ToDictionary(x => x.harbour);
            var dicPlatforms = lookupMan.GetLookups(typeof(L_Platform), ldv).OfType<L_Platform>().ToDictionary(x => x.platform);
            var dicCatchRegistrations = lookupMan.GetLookups(typeof(L_CatchRegistration), ldv).OfType<L_CatchRegistration>().ToDictionary(x => x.catchRegistrationId);
            var dicSpeciesRegistrations = lookupMan.GetLookups(typeof(L_SpeciesRegistration), ldv).OfType<L_SpeciesRegistration>().ToDictionary(x => x.speciesRegistrationId);
            var dicParasites = lookupMan.GetLookups(typeof(L_Parasite), ldv).OfType<L_Parasite>().ToDictionary(x => x.L_parasiteId);
            var dicOtolithReadingRemarks = lookupMan.GetLookups(typeof(L_OtolithReadingRemark), ldv).OfType<L_OtolithReadingRemark>().ToDictionary(x => x.L_OtolithReadingRemarkID);
            var dicReferences = lookupMan.GetLookups(typeof(L_Reference), ldv).OfType<L_Reference>().ToDictionary(x => x.L_referenceId);
            var dicSpecies = lookupMan.GetLookups(typeof(L_Species), ldv).OfType<L_Species>().ToDictionary(x => x.speciesCode);
            var dicMaturity = lookupMan.GetLookups(typeof(Maturity), ldv).OfType<Maturity>().ToDictionary(x => x.maturityId);
            var dicApplications = lookupMan.GetLookups(typeof(L_Application), ldv).OfType<L_Application>().ToDictionary(x => x.L_applicationId);
            var dicHatchMonthReadabilities = lookupMan.GetLookups(typeof(L_HatchMonthReadability), ldv).OfType<L_HatchMonthReadability>().ToDictionary(x => x.L_HatchMonthReadabilityId);
            var dicVisualStocks = lookupMan.GetLookups(typeof(L_VisualStock), ldv).OfType<L_VisualStock>().ToDictionary(x => x.L_visualStockId);
            var dicGeneticStocks = lookupMan.GetLookups(typeof(L_GeneticStock), ldv).OfType<L_GeneticStock>().ToDictionary(x => x.L_geneticStockId);
            var dicSelectionDeviceSource = lookupMan.GetLookups(typeof(L_SelectionDeviceSource), ldv).OfType<L_SelectionDeviceSource>().ToDictionary(x => x.L_selectionDeviceSourceId);
            var dicLengthMeasureType = lookupMan.GetLookups(typeof(L_LengthMeasureType), ldv).OfType<L_LengthMeasureType>().ToDictionary(x => x.L_lengthMeasureTypeId);
            LengthMeasureType = dicLengthMeasureType;

            foreach (var c in lst)
            {
                AssignLookupForCruise(c, dicDFUPersons,
                                         dicPersons,
                                         dicSamplingTypes,
                                         dicSamplingMethods,
                                         dicHarbours,
                                         dicPlatforms,
                                         dicCatchRegistrations,
                                         dicSpeciesRegistrations,
                                         dicParasites,
                                         dicOtolithReadingRemarks,
                                         dicReferences,
                                         dicSpecies,
                                         dicMaturity,
                                         dicApplications,
                                         dicHatchMonthReadabilities,
                                         dicVisualStocks,
                                         dicGeneticStocks,
                                         dicSelectionDeviceSource,
                                         dicLengthMeasureType);
            }
        }


        public static void AssignLookupForCruise(Cruise c, Dictionary<int, DFUPerson> dicDFUPersons,
                                                      Dictionary<int, Person> dicPersons,
                                                      Dictionary<int, L_SamplingType> dicSamplingTypes,
                                                      Dictionary<int, L_SamplingMethod> dicSamplingMethods,
                                                      Dictionary<string, L_Harbour> dicHarbours,
                                                      Dictionary<string, L_Platform> dicPlatforms,
                                                      Dictionary<int, L_CatchRegistration> dicCatchRegistrations,
                                                      Dictionary<int, L_SpeciesRegistration> dicSpeciesRegistrations,
                                                      Dictionary<int, L_Parasite> dicParasites,
                                                      Dictionary<int, L_OtolithReadingRemark> dicOtolithReadingRemarks,
                                                      Dictionary<int, L_Reference> dicReferences,
                                                      Dictionary<string, L_Species> dicSpecies,
                                                      Dictionary<int, Maturity> dicMaturity, 
                                                      Dictionary<int, L_Application> dicApplications, 
                                                      Dictionary<int, L_HatchMonthReadability> dicHatchMonthReadabilities,
                                                      Dictionary<int, L_VisualStock> dicVisualStocks,
                                                      Dictionary<int, L_GeneticStock> dicGeneticStocks,
                                                      Dictionary<int, L_SelectionDeviceSource> dicSelectionDeviceSource,
                                                      Dictionary<int, L_LengthMeasureType> dicLengthMeasureType)
        {
            if (c.responsibleId.HasValue && dicDFUPersons.ContainsKey(c.responsibleId.Value))
                c.AssignNavigationPropertyWithoutChanges("DFUPerson1", dicDFUPersons[c.responsibleId.Value]);

            if (c.dataHandlerId.HasValue && dicDFUPersons.ContainsKey(c.dataHandlerId.Value))
                c.AssignNavigationPropertyWithoutChanges("DFUPerson", dicDFUPersons[c.dataHandlerId.Value]);

            foreach (var t in c.Trip)
            {
                if (t.samplingTypeId.HasValue && dicSamplingTypes.ContainsKey(t.samplingTypeId.Value))
                    t.AssignNavigationPropertyWithoutChanges("L_SamplingType", dicSamplingTypes[t.samplingTypeId.Value]);

                if (t.samplingMethodId.HasValue && dicSamplingMethods.ContainsKey(t.samplingMethodId.Value))
                    t.AssignNavigationPropertyWithoutChanges("L_SamplingMethod", dicSamplingMethods[t.samplingMethodId.Value]);

                if (!string.IsNullOrEmpty(t.harbourLanding) && dicHarbours.ContainsKey(t.harbourLanding))
                    t.AssignNavigationPropertyWithoutChanges("L_Harbour", dicHarbours[t.harbourLanding]);

                if (!string.IsNullOrEmpty(t.harbourSample) && dicHarbours.ContainsKey(t.harbourSample))
                    t.AssignNavigationPropertyWithoutChanges("L_Harbour1", dicHarbours[t.harbourSample]);

                if (t.tripLeaderId.HasValue && dicDFUPersons.ContainsKey(t.tripLeaderId.Value))
                    t.AssignNavigationPropertyWithoutChanges("DFUPerson", dicDFUPersons[t.tripLeaderId.Value]);

                if (t.dataHandlerId.HasValue && dicDFUPersons.ContainsKey(t.dataHandlerId.Value))
                    t.AssignNavigationPropertyWithoutChanges("DFUPerson1", dicDFUPersons[t.dataHandlerId.Value]);

                if (t.contactPersonId.HasValue && dicPersons.ContainsKey(t.contactPersonId.Value))
                    t.AssignNavigationPropertyWithoutChanges("Person", dicPersons[t.contactPersonId.Value]);

                if (!string.IsNullOrEmpty(t.platform1) && dicPlatforms.ContainsKey(t.platform1))
                    t.AssignNavigationPropertyWithoutChanges("L_Platform", dicPlatforms[t.platform1]);

                if (!string.IsNullOrEmpty(t.platform2) && dicPlatforms.ContainsKey(t.platform2))
                    t.AssignNavigationPropertyWithoutChanges("L_Platform1", dicPlatforms[t.platform2]);

                foreach (var s in t.Sample)
                {
                    if (s.catchRegistrationId.HasValue && dicCatchRegistrations.ContainsKey(s.catchRegistrationId.Value))
                        s.AssignNavigationPropertyWithoutChanges("L_CatchRegistration", dicCatchRegistrations[s.catchRegistrationId.Value]);

                    if (s.speciesRegistrationId.HasValue && dicSpeciesRegistrations.ContainsKey(s.speciesRegistrationId.Value))
                        s.AssignNavigationPropertyWithoutChanges("L_SpeciesRegistration", dicSpeciesRegistrations[s.speciesRegistrationId.Value]);

                    if (s.samplePersonId.HasValue && dicDFUPersons.ContainsKey(s.samplePersonId.Value))
                        s.AssignNavigationPropertyWithoutChanges("DFUPerson", dicDFUPersons[s.samplePersonId.Value]);

                    if (s.analysisPersonId.HasValue && dicDFUPersons.ContainsKey(s.analysisPersonId.Value))
                        s.AssignNavigationPropertyWithoutChanges("DFUPerson1", dicDFUPersons[s.analysisPersonId.Value]);
                    
                    if(s.selectionDeviceSourceId.HasValue && dicSelectionDeviceSource.ContainsKey(s.selectionDeviceSourceId.Value))
                        s.AssignNavigationPropertyWithoutChanges("L_SelectionDeviceSource", dicSelectionDeviceSource[s.selectionDeviceSourceId.Value]);

                    foreach (var sl in s.SpeciesList)
                    {
                        if (sl.maturityReaderId.HasValue && dicDFUPersons.ContainsKey(sl.maturityReaderId.Value))
                            sl.AssignNavigationPropertyWithoutChanges("MaturityReader", dicDFUPersons[sl.maturityReaderId.Value]);

                        if (sl.ageReadId.HasValue && dicDFUPersons.ContainsKey(sl.ageReadId.Value))
                            sl.AssignNavigationPropertyWithoutChanges("DFUPerson", dicDFUPersons[sl.ageReadId.Value]);

                        if (sl.hatchMonthReaderId.HasValue && dicDFUPersons.ContainsKey(sl.hatchMonthReaderId.Value))
                            sl.AssignNavigationPropertyWithoutChanges("HatchMontReader", dicDFUPersons[sl.hatchMonthReaderId.Value]);

                        if (!string.IsNullOrEmpty(sl.speciesCode) && dicSpecies.ContainsKey(sl.speciesCode))
                            sl.AssignNavigationPropertyWithoutChanges("L_Species", dicSpecies[sl.speciesCode]);

                        if (sl.applicationId.HasValue && dicApplications.ContainsKey(sl.applicationId.Value))
                            sl.AssignNavigationPropertyWithoutChanges("L_Application", dicApplications[sl.applicationId.Value]);

                        foreach (var ss in sl.SubSample)
                        {
                            foreach (var a in ss.Animal)
                            {
                                if(a.lengthMeasureTypeId.HasValue && dicLengthMeasureType.ContainsKey(a.lengthMeasureTypeId.Value))
                                {
                                    a.AssignNavigationPropertyWithoutChanges("L_LengthMeasureType", dicLengthMeasureType[a.lengthMeasureTypeId.Value]);
                                }

                                foreach (var ai in a.AnimalInfo)
                                {
                                    if (ai.parasiteId.HasValue && dicParasites.ContainsKey(ai.parasiteId.Value))
                                        ai.AssignNavigationPropertyWithoutChanges("L_Parasite", dicParasites[ai.parasiteId.Value]);

                                    foreach (var r in ai.R_AnimalInfoReference)
                                    {
                                        if (dicReferences.ContainsKey(r.L_referenceId))
                                            r.AssignNavigationPropertyWithoutChanges("L_Reference", dicReferences[r.L_referenceId]);
                                    }

                                    if (ai.maturityId.HasValue && dicMaturity.ContainsKey(ai.maturityId.Value) && (ai.Maturity == null || ai.Maturity.maturityId != ai.maturityId.Value))
                                        ai.AssignNavigationPropertyWithoutChanges("Maturity", dicMaturity[ai.maturityId.Value]);
                                }

                                foreach (var age in a.Age)
                                {
                                    if (age.otolithReadingRemarkId.HasValue && dicOtolithReadingRemarks.ContainsKey(age.otolithReadingRemarkId.Value))
                                        age.AssignNavigationPropertyWithoutChanges("L_OtolithReadingRemark", dicOtolithReadingRemarks[age.otolithReadingRemarkId.Value]);

                                    if (age.hatchMonthReadabilityId.HasValue && dicHatchMonthReadabilities.ContainsKey(age.hatchMonthReadabilityId.Value))
                                        age.AssignNavigationPropertyWithoutChanges("L_HatchMonthReadability", dicHatchMonthReadabilities[age.hatchMonthReadabilityId.Value]);

                                    if (age.visualStockId.HasValue && dicVisualStocks.ContainsKey(age.visualStockId.Value))
                                        age.AssignNavigationPropertyWithoutChanges("L_VisualStock", dicVisualStocks[age.visualStockId.Value]);

                                    if (age.geneticStockId.HasValue && dicGeneticStocks.ContainsKey(age.geneticStockId.Value))
                                        age.AssignNavigationPropertyWithoutChanges("L_GeneticStock", dicGeneticStocks[age.geneticStockId.Value]);

                                    if (age.sdAgeReadId.HasValue && dicDFUPersons.ContainsKey(age.sdAgeReadId.Value))
                                        age.AssignNavigationPropertyWithoutChanges("DFUPerson", dicDFUPersons[age.sdAgeReadId.Value]);
                                }
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Convert at list of cruises to dataware house cruises (returning a list of datawarehouse cruise entity).
        /// The conversion also includes raising and position to area and statistical rectangle calculations.
        /// </summary>
        private List<Babelfisk.Warehouse.Model.Cruise> CreateAndRaiseData(List<Cruise> lstCruises, LookupDataVersioning ldv, ref List<Babelfisk.Warehouse.DWMessage> lstMessages)
        {
            var lookupMan = new LookupManager();
            var lstTreatmentFactors = lookupMan.GetLookups(typeof(TreatmentFactor), ldv).OfType<TreatmentFactor>().ToList();
            var lstStocks = lookupMan.GetLookups(typeof(L_Stock), ldv).OfType<L_Stock>().ToList();
            var lstSpeciesAreaStock = lookupMan.GetLookups(typeof(R_StockSpeciesArea), ldv).OfType<R_StockSpeciesArea>().ToList();

            var lst = new List<Babelfisk.Warehouse.Model.Cruise>();
            var ef = new Babelfisk.Warehouse.EntityFactory(lstTreatmentFactors, lstStocks, lstSpeciesAreaStock);

            for (int i = 0; i < lstCruises.Count; i++)
            {
                var c = lstCruises[i];
                var cDW = ef.CreateDWCruise(c);

                lst.Add(cDW);
            }

            lstMessages = ef.Messages;

            return lst;
        }

        public DatabaseOperationResult SaveCruiseToDataWarehouse(Babelfisk.Warehouse.Model.Cruise c, List<Babelfisk.Warehouse.DWMessage> lstMessages, ref List<Babelfisk.Warehouse.DWMessage> lstNewMessages, bool blnDeleteCruiseBeforeInsert)
        {
            var sv = DataClientFactory.CreateDataInputClient();

            try
            {
                var arr = c.ToByteArrayDataContract();
                arr = arr.Compress();

                if (lstMessages == null)
                    lstMessages = new List<Warehouse.DWMessage>();

                var arrM = lstMessages.ToByteArrayDataContract();
                arrM = arrM.Compress();

                sv.SupplyCredentials();

                Babelfisk.Warehouse.DWMessage[] arrNewMessages = new Warehouse.DWMessage[] {};
                var res = (sv as IDataInput).SaveCruiseToDataWarehouse(arr, arrM, blnDeleteCruiseBeforeInsert, ref arrNewMessages);
                lstNewMessages.AddRange(arrNewMessages);

                arr = null;
                GC.Collect();
                sv.Close();

                return res;
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                sv.Abort();
                throw e;
            }
        }
    }
}
