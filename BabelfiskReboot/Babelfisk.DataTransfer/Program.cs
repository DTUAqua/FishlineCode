using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anchor.Core;
using System.Data.Objects;
using System.Configuration;
using System.Data.EntityClient;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.DataTransfer
{
    class Program
    {
        static void Main(string[] args)
        {
            Anchor.Core.Loggers.Logger.Log(Anchor.Core.Loggers.LogType.Info, "Running FishLine Data Transfer: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));

            Console.WriteLine("------ Initializing FishLine data transfer -------");
            DataManager datMan = new DataManager();

            List<int> lst = datMan.GetCruiseIdsToTransfer();

            if (lst == null)
                lst = new List<int>();

            Console.WriteLine(String.Format("-  Found {0} cruises to transfer.", lst.Count));

            Console.WriteLine(String.Format("-  Retrieving lookup values."));

            Babelfisk.Warehouse.DataManager datManW = new Warehouse.DataManager();

            //Get treatment factors
            var lstTreatmentFactors = datMan.GetTreatmentFactors();
            if (lstTreatmentFactors == null || lstTreatmentFactors.Count == 0)
            {
                DataManager.LogError("Could not retrieve treatment factors from FishLine database.");
                Console.WriteLine(String.Format("-  Could not retrieve treatment factors from FishLine database."));
                return;
            }

            var lstStocks = datMan.GetStocks();

            var lstSpeciesAreaStocks = datMan.GetSpeciesAreaStocks();


            //Load lookups used for populating data to be exported
            var dicDFUPersons = datMan.GetLookups<DFUPerson>().ToDictionary(x => x.dfuPersonId);
            var dicPersons = datMan.GetLookups<Person>().ToDictionary(x => x.personId);
            var dicSamplingTypes = datMan.GetLookups <L_SamplingType>().ToDictionary(x => x.samplingTypeId);
            var dicSamplingMethods = datMan.GetLookups<L_SamplingMethod>().ToDictionary(x => x.samplingMethodId);
            var dicHarbours = datMan.GetLookups<L_Harbour>().ToDictionary(x => x.harbour);
            var dicPlatforms = datMan.GetLookups<L_Platform>().ToDictionary(x => x.platform);
            var dicCatchRegistrations = datMan.GetLookups<L_CatchRegistration>().ToDictionary(x => x.catchRegistrationId);
            var dicSpeciesRegistrations = datMan.GetLookups<L_SpeciesRegistration>().ToDictionary(x => x.speciesRegistrationId);
            var dicParasites = datMan.GetLookups<L_Parasite>().ToDictionary(x => x.L_parasiteId);
            var dicOtolithReadingRemarks = datMan.GetLookups<L_OtolithReadingRemark>().ToDictionary(x => x.L_OtolithReadingRemarkID);
            var dicReferences = datMan.GetLookups<L_Reference>().ToDictionary(x => x.L_referenceId);
            var dicSpecies = datMan.GetLookups<L_Species>().ToDictionary(x => x.speciesCode);
            var dicMaturity = datMan.GetLookups<Maturity>().ToDictionary(x => x.maturityId);
            var dicApplications = datMan.GetLookups<L_Application>().ToDictionary(x => x.L_applicationId);
            var dicHatchMonthReadabilities = datMan.GetLookups<L_HatchMonthReadability>().ToDictionary(x => x.L_HatchMonthReadabilityId);
            var dicVisualStocks = datMan.GetLookups<L_VisualStock>().ToDictionary(x => x.L_visualStockId);
            var dicGeneticStocks = datMan.GetLookups<L_GeneticStock>().ToDictionary(x => x.L_geneticStockId);
            var dicSelectionDeviceSource = datMan.GetLookups<L_SelectionDeviceSource>().ToDictionary(x => x.L_selectionDeviceSourceId);
            var dicLengthMeasureType = datMan.GetLookups<L_LengthMeasureType>().ToDictionary(x => x.L_lengthMeasureTypeId);

            if (dicSpecies == null || dicSpecies.Count == 0)
            {
                DataManager.LogError("Could not retrieve species from FishLine database.");
                Console.WriteLine(String.Format("-  Could not retrieve species from FishLine database."));
                return;
            }


            for (int i = 0; i < lst.Count; i++)
            {
                Anchor.Core.Loggers.Logger.Log(Anchor.Core.Loggers.LogType.Info, "Transferring cruise with id: " + lst[i]);

                //Create a new entity factory everytime, so messages are reset.
                Babelfisk.Warehouse.EntityFactory ef = new Warehouse.EntityFactory(lstTreatmentFactors, lstStocks, lstSpeciesAreaStocks);

                Console.WriteLine(String.Format("-  Processing {0}/{1} cruises.", i+1, lst.Count));

                Console.WriteLine(String.Format("   > Retrieving cruise with id '{0}'.", lst[i]));

                bool blnExceptionThrown = false;
                //1) Get cruise data
                var cruise = datMan.GetDataForRaising(lst[i], out blnExceptionThrown);

                if (cruise == null)
                {
                    //If an exception was thrown when getting cruise to transfer, do not delete the cruise from the dataware house, and leave the id in the transfer queue
                    if (blnExceptionThrown)
                    {
                        Anchor.Core.Loggers.Logger.Log(Anchor.Core.Loggers.LogType.Info, string.Format("An exception was thrown when getting cruise with id {0}. Continuing to next cruise leaving current cruise id in transfer queue.", lst[i]));
                        DataManager.LogWarning(String.Format("An exception was thrown when getting cruise with id {0}. Continuing to next cruise leaving current cruise id in transfer queue.", lst[i]), lst[i]);
                        continue;
                    }

                    //Double check that the cruise does indeed not exist in FishLine, before deleting it.
                    if (datMan.HasCruise(lst[i], out blnExceptionThrown) || blnExceptionThrown)
                    {
                        if (!blnExceptionThrown)
                            DataManager.LogWarning(String.Format("Could not get data for cruise with id '{0}' in FishLine. Cruise does however exist, so DataTransfer will try again at next transfer cycle.", lst[i]), lst[i]);
                        else
                            DataManager.LogError(string.Format("Could not get data for cruise with id '{0}' in FishLine and could not verify if the cruise exists in FishLine. Skipping cruise leaving cruise id in transfer queue.", lst[i]));
                       
                        continue;
                    }


                    DataManager.LogWarning(String.Format("Could not find data for cruise with id '{0}' in FishLine. Cruise will therefore be deleted from DW.", lst[i]), lst[i]);
                    Console.WriteLine(String.Format("     Could not find data for cruise with id '{0}' in FishLine. Cruise will be deleted from warehouse.", lst[i]));

                    try
                    {
                        datManW.DeleteCruiseAndAssociatedData(lst[i]);

                        //Truncate transfer queue.
                        datManW.TruncateTransferQueue(new List<int>() { lst[i] });
                    }
                    catch (Exception e)
                    {
                        DataManager.LogError(e, lst[i]);
                        Console.WriteLine(String.Format("     Could not delete cruise with id '{0}' form DW.", lst[i]));
                    }
                    continue;
                }

                //Assign lookups to cruise.
                BusinessLogic.Export.ExportManager.AssignLookupForCruise(cruise, dicDFUPersons,
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

                Console.WriteLine("   > Raising cruise.");

                Warehouse.Model.Cruise cDW = null;

                //2) Raise cruise data
                try
                {
                    cDW = ef.CreateDWCruise(cruise);
                }
                catch (Exception e)
                {
                    DataManager.LogError(e);
                }

                if (cDW == null)
                {
                    DataManager.LogError(String.Format("Could not raise cruise with id '{0}'. Cruise will therefore not be transferred to DW.", lst[i]));
                    Console.WriteLine(String.Format("     Could not raise cruise with id '{0}' and is therefore skipping it.", lst[i]));
                    continue;
                }

                Console.WriteLine("   > Inserting cruise.");

                //3) Insert cruise data
                try
                {
                    datManW.InsertCruise(cDW, ef.Messages, true);
                }
                catch (Exception e)
                {
                    DataManager.LogError(e);
                    Console.WriteLine(String.Format("     Could not insert cruise with id '{0}', it is therefore skipped.", lst[i]));
                }

                //Truncate transfer queue.
                datManW.TruncateTransferQueue(new List<int>() { lst[i] });

                cruise = null;
                cDW = null;

                //Make sure to free memory after each insert, since a whole cruise can fill quite a lot of space.
                GC.Collect();
            }

            //Synchronize species to dataware house
            SynchronizeSpeciesToWarehouse(datManW, dicSpecies);
        }



        private static void SynchronizeSpeciesToWarehouse(Babelfisk.Warehouse.DataManager datManW, Dictionary<string, L_Species> dicSpecies)
        {
            Console.WriteLine(String.Format("-  Synchronizing L_Species."));

            try
            {
                datManW.SynchronizeSpecies(dicSpecies);
            }
            catch (Exception e)
            {
                DataManager.LogError(e);
                Console.WriteLine(String.Format("     Synchronizing species failed. Error received: '{0}'.", e.Message));
            }

            Console.WriteLine(String.Format("-  Synchronizing L_Species Done."));
        }


        
    }
}
