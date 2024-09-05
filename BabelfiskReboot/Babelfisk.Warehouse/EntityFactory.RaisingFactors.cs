using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Warehouse
{
    public partial class EntityFactory
    {

        /// <summary>
        /// DTO: Stands for del-total.
        /// RaisingFactor general: rf = (Step0Weight / Step1Weight) * (Step1Weight / Step2Weight) * (Step2Weight / Step3Weight);
        /// </summary>
        public void CalculateRaisingFactors(Model.Sample sDW, Entities.Sprattus.Sample s)
        {
            //Get DW specieslist and save them in a dictionary for quick access using their specieslistid.
            var dicDWSpeciesLists = sDW.SpeciesLists.ToDictionary(x => x.speciesListId);

            //Select DTOs (there can be several because of different landing categories and different dfuBase_category).
            //Make sure to find the right DTO by comparing SL.landingCategory = DTO.landingCategory & SL.dfuBase_category = DTO.dfuBase_category
            var lstDTOs = s.SpeciesList.Where(x => x.IsDTO).ToList();

            //Add Species DTOs (this is potential DTO's that does not have speciescode = "DTO", but nevertheless acts as DTO).
            var lstPotentialSpeciesDTOs = s.SpeciesList.Where(x => !x.IsDTO &&
                                                             (x.SubSample.Where(y => y.IsRepresentative && (y.stepNum == 0 || y.stepNum == 1) && y.subSampleWeight.HasValue).Any())).ToList();

            //List for adding species DTOs as they are found.
            List<Entities.Sprattus.SpeciesList> lstUsedSpeciesDTOs = new List<Entities.Sprattus.SpeciesList>();

            var lstAllSubSamples = s.SpeciesList.SelectMany(x => x.SubSample).Where(x => x.IsRepresentative).ToList();

            //Loop through each species list and calculate raising factors.
            foreach (var sl in s.SpeciesList)
            {
                //Skip DTOs
                if (sl.IsDTO)
                    continue;

                //Get representative subsamples
                var lstSubSamplesRepDesc = sl.SubSample.Where(x => x.IsRepresentative).OrderByDescending(x => x.stepNum).ToList();

                if (lstSubSamplesRepDesc == null || lstSubSamplesRepDesc.Count == 0)
                    continue;

                decimal? raisingFactor = CalculateRaisingFactor(lstSubSamplesRepDesc, sl, lstDTOs, lstPotentialSpeciesDTOs, lstUsedSpeciesDTOs, lstAllSubSamples);

                if (raisingFactor.HasValue)
                    raisingFactor = Math.Round(raisingFactor.Value, 3);

                //Assign raising factor.
                if (raisingFactor.HasValue && dicDWSpeciesLists.ContainsKey(sl.speciesListId))
                    dicDWSpeciesLists[sl.speciesListId].raisingFactor = raisingFactor;
            }

            //Loop through used species DTOs and set their raising factor to NULL (as done with normal raising factors)
            foreach (var sl in lstUsedSpeciesDTOs)
                if (dicDWSpeciesLists.ContainsKey(sl.speciesListId))
                    dicDWSpeciesLists[sl.speciesListId].raisingFactor = null;
        }


        private decimal? CalculateRaisingFactor(List<Entities.Sprattus.SubSample> lstSubSamplesRepDesc,      //Representative subsamples from SL
                                                Entities.Sprattus.SpeciesList sl,                            //Current species list
                                                List<Entities.Sprattus.SpeciesList> lstDTOs,                 //SLs with species = DTO
                                                List<Entities.Sprattus.SpeciesList> lstPotentialSpeciesDTOs, //SLs that are potential species DTOs
                                                List<Entities.Sprattus.SpeciesList> lstUsedSpeciesDTOs,      //SLs that have been used as species DTO
                                                List<Entities.Sprattus.SubSample> lstAllSubSamples)          //all subsamples in sample
        {
            decimal? raisingFactor = 1;

            //1st check: If step0 or step1 has 'x' in subSampleWeight and no landingweight, throw warning that raisingfactor is not calculated for 'x'es.
            //2nd check: if step > 1 has 'x' in subSampleWeight throw warning that raisingfactor is not calculated for 'x'es.
            /*if (lstSubSamplesRepDesc.Where(x => (x.stepNum == 0 || x.stepNum == 1) && x.sumAnimalWeights.HasValue && x.sumAnimalWeights.Value && !x.landingWeight.HasValue).Any() ||
                lstSubSamplesRepDesc.Where(x => x.stepNum == 1 && x.sumAnimalWeights.HasValue && x.sumAnimalWeights.Value && !x.landingWeight.HasValue).Any() ||
                lstSubSamplesRepDesc.Where(x => x.stepNum > 1 && x.sumAnimalWeights.HasValue && x.sumAnimalWeights.Value).Any())
            {
                AddMessage(DWMessage.NewWarning(GetOrigin(sl), "SpeciesList " + GetSpeciesListKeyDetails(sl), sl.speciesListId.ToString(), String.Format("Raisingfaktoren udregnes ikke for artslister hvor 'x' er angivet som vægt (medmindre købsvægt er specificeret).")));
                raisingFactor = null;
                return raisingFactor;
            }*/


            //If there is only 1 subsample and subSampleWeight step0 is 'x', return a raisingfactor of 1
            if(lstSubSamplesRepDesc.Count == 1 &&
               lstSubSamplesRepDesc.Where(x => x.stepNum == 0 && x.sumAnimalWeights.HasValue && x.sumAnimalWeights.Value && !x.landingWeight.HasValue &&
                                          x.subSampleWeight.HasValue //Make sure subSampleWeight has a value. when sumAnimalWeights are used, subSampleWeight = Sum(Animal.weight), which is calculated earlier (when creating the dataware house species lists)
                                          ).Any())
            {
                return raisingFactor;
            }

            //If 'x' i set in weight, but no Animals with weight exists, return null as raising factor.
            if (lstSubSamplesRepDesc.Where(x => x.sumAnimalWeights.HasValue && x.sumAnimalWeights.Value && !x.subSampleWeight.HasValue && !x.landingWeight.HasValue).Count() == lstSubSamplesRepDesc.Count)
                return null;

            int intMaxStepNum = lstSubSamplesRepDesc.Max(x => x.stepNum);

            //Loop through step intMaxStepNum down to 0.
            for (int intStepNum = intMaxStepNum; intStepNum > 0; intStepNum--)
            {
                Entities.Sprattus.SubSample curSubSample = lstSubSamplesRepDesc.Where(x => x.stepNum == intStepNum).FirstOrDefault();
                Entities.Sprattus.SubSample parentSubSample = lstSubSamplesRepDesc.Where(x => x.stepNum == intStepNum - 1).FirstOrDefault();

                decimal? curWeight = null;
                decimal? parentWeight = null;

                //Assign weight or landingweight
                if (curSubSample != null)
                {
                    curWeight = GetWeightOrLandingWeight(curSubSample);

                    //Throw an error if weight 0 is ever met.
                    if (curWeight.HasValue && curWeight.Value == 0)
                    {
                        AddMessage(DWMessage.NewError(GetCruiseId(sl), GetOrigin(sl), "SpeciesList " + GetSpeciesListKeyDetails(sl), sl.speciesListId.ToString(), String.Format("Raisingfaktoren kunne ikke udregnes. Vægten er 0 for SubSample med Id = {0} med vægt = {1}.", curSubSample.subSampleId, GetWeightOrLandingWeight(curSubSample))));
                        raisingFactor = null;
                        break;
                    }
                }



                //Type 1 + 2: If parent subsample exists with weights, calculate raising factor after normal equation (parent.Weight / cur.Weight).
                //This also calculates landing weights, if subSampleWeight is null
                if (curWeight.HasValue &&
                   (parentSubSample != null) &&
                   (HasWeightOrLandingWeight(parentSubSample)))
                {
                    parentWeight = GetWeightOrLandingWeight(parentSubSample);

                    //If curSubSample has landingweight (and subsampleweight is from SUM(animal.weight)), and parent.subSampleWeight is null ('x' is entered) and parent.landingweight != null, using landingweights in calculation.
                    if ((curSubSample.landingWeight.HasValue && curSubSample.subSampleWeight.HasValue && curSubSample.sumAnimalWeights.HasValue && curSubSample.sumAnimalWeights.Value) &&
                        (parentSubSample.landingWeight.HasValue && !parentSubSample.subSampleWeight.HasValue && parentSubSample.sumAnimalWeights.HasValue && parentSubSample.sumAnimalWeights.Value)
                        )
                    {
                        curWeight = curSubSample.landingWeight;
                    }
                    else
                    //1st check: If landingweights are used in this calculation for stepnum 0 and 1, but there are subSampleWeights on stepnum 2 and/or 3 as well, throw error.
                    //2nd check: If subSampleWeights are used for stepnum 0 and landingweights for stepnum 1 (or vice versa), throw error
                    if ((UsingLandingWeight(curSubSample) && lstSubSamplesRepDesc.Where(x => x.stepNum > 1 && x.subSampleWeight.HasValue).Any()) || //1st
                        (UsingLandingWeight(curSubSample) && !UsingLandingWeight(parentSubSample)) || (!UsingLandingWeight(curSubSample) && UsingLandingWeight(parentSubSample)) //2nd
                        )
                    {

                        AddMessage(DWMessage.NewError(GetCruiseId(sl), GetOrigin(sl), "SpeciesList " + GetSpeciesListKeyDetails(sl), sl.speciesListId.ToString(), String.Format("Raisingfaktoren kunne ikke udregnes. Der er rod i købsvægte og normale vægte.")));
                        raisingFactor = null;
                        break;
                    }
                }

                //Type 3: If weights are missing on parent steps of a SubSample, use the DTO in the raising factor calculation.
                else if ((parentSubSample == null || !HasWeight(parentSubSample)) && (lstDTOs.Count > 0 || lstPotentialSpeciesDTOs.Count > 0)) //Type 3: weight exists on current subsample, but not parent. Use DTO instead
                {
                    Entities.Sprattus.SpeciesList DTO = null;
                    bool blnUsingSpeciesDTO = false;

                    //If one of the properties below are specified and parent SL with weight specified on intStepNum-1 and same species exist, look for a species DTO instead.
                    if ((sl.HasSexCode || sl.HasOvigorous || sl.HasCuticulaHardnesse ||
                        sl.HasSizeSortingDFU || sl.HasSizeSortingEU)
                        && (lstDTOs.Count == 0 || lstPotentialSpeciesDTOs.Where(x => x.speciesCode.Equals(sl.speciesCode, StringComparison.InvariantCultureIgnoreCase) && //Has other same species
                                                                                     x.SubSample.Where(y => y.IsRepresentative && y.stepNum == intStepNum - 1).Any()).Any()) //With a parent weight
                        )
                    {
                        blnUsingSpeciesDTO = true;
                        if (lstPotentialSpeciesDTOs.Count > 0)
                            DTO = GetSpeciesDTO(lstPotentialSpeciesDTOs, sl, intStepNum);
                    }

                    //Try searching for real DTO if no species DTO was found.
                    if (DTO == null)
                    {
                        blnUsingSpeciesDTO = false;
                        DTO = GetDTO(lstDTOs, sl);
                    }

                    //If a matching DTO was not found, log error.
                    if (DTO == null)
                    {
                        AddMessage(DWMessage.NewError(GetCruiseId(sl), GetOrigin(sl), "SpeciesList " + GetSpeciesListKeyDetails(sl), sl.speciesListId.ToString(), String.Format("Raisingfaktoren kunne ikke udregnes. Det var ikke muligt at finde en matchende DTO{0}", curSubSample != null ? string.Format(" for SubSample med Id = {0} og vægt = {1}", curSubSample.subSampleId, GetWeightOrLandingWeight(curSubSample)) : "")));
                        raisingFactor = null;
                        break;
                    }

                    var dtoSubSampleParent = DTO.SubSample.Where(x => x.stepNum == intStepNum - 1 && HasWeight(x)).FirstOrDefault();
                    var dtoSubSampleCurrent = DTO.SubSample.Where(x => x.stepNum == intStepNum && HasWeight(x)).FirstOrDefault();

                    //If current SL only has landingweight, but a DTO was found, throw error. raisingfactors cannot be calculated using DTOs
                    if (!blnUsingSpeciesDTO && sl.SubSample.Where(x => x.stepNum == 1 && UsingLandingWeight(x)).Any())
                    {
                        AddMessage(DWMessage.NewError(GetCruiseId(sl), GetOrigin(sl), "SpeciesList " + GetSpeciesListKeyDetails(sl), sl.speciesListId.ToString(), String.Format("Raisingfaktoren kunne ikke udregnes. Der blev fundet en DTO, men raisingfaktorer kan ikke udregnes på landingsvægte kombineret med DTO'er{0}.", curSubSample != null ? string.Format(" (SubSample Id = {0} og vægt = {1})", curSubSample.subSampleId, GetWeightOrLandingWeight(curSubSample)) : "")));
                        raisingFactor = null;
                        break;
                    }

                    //If a matching DTO subsample was not found log error.
                    if (dtoSubSampleParent == null)
                    {
                        AddMessage(DWMessage.NewError(GetCruiseId(sl), GetOrigin(sl), "SpeciesList " + GetSpeciesListKeyDetails(sl), sl.speciesListId.ToString(), String.Format("Raisingfaktoren kunne ikke udregnes{0} - En DTO (med id {1}) blev fundet, men den havde ikke en SubSample med stepNum = {2}, som skulle være brugt til udregningen.", curSubSample != null ? string.Format(" for SubSample med Id = {0} og vægt = {1}", curSubSample.subSampleId, GetWeightOrLandingWeight(curSubSample)) : "", DTO.speciesListId, intStepNum - 1)));
                        raisingFactor = null;
                        break;
                    }

                    List<Entities.Sprattus.SubSample> ssRefs = null;

                    if (blnUsingSpeciesDTO)
                    {
                        ssRefs = lstAllSubSamples.Where(x => HasWeight(x) && //Has weight
                                                            sl.speciesCode.Equals(x.SpeciesList.speciesCode) && //Species code is the same
                                                            x.speciesListId != sl.speciesListId && //Is different SpeciesList
                                                            !x.SpeciesList.SubSample.Where(y => y.stepNum == intStepNum - 1).Any() && //Has no parent step
                                                            StringEquals(x.SpeciesList.landingCategory, sl.landingCategory) && //LandingCategory the same
                                                            StringEquals(x.SpeciesList.dfuBase_Category, sl.dfuBase_Category) && //sfuBase_Category the same
                                                            x.stepNum == intStepNum && //stepNum the same
                                                            // StringEquals(DTO.treatment, x.SpeciesList.treatment) &&   //MDU 22.06.2023: Added treatment join.
                                                             (DTO.HasSexCode ? DTO.sexCode.Equals(x.SpeciesList.sexCode, StringComparison.InvariantCultureIgnoreCase) : true) && //Sexcode is the same
                                                             (DTO.HasOvigorous ? DTO.ovigorous.Equals(x.SpeciesList.ovigorous, StringComparison.InvariantCultureIgnoreCase) : true) && //ovigorous is the same
                                                             (DTO.HasCuticulaHardnesse ? DTO.cuticulaHardness.Equals(x.SpeciesList.cuticulaHardness, StringComparison.InvariantCultureIgnoreCase) : true) && //cuticulaHardness is the same
                                                             (DTO.HasSizeSortingDFU ? DTO.sizeSortingDFU.Equals(x.SpeciesList.sizeSortingDFU, StringComparison.InvariantCultureIgnoreCase) : true) && //sizeSortingDFU is the same
                                                             (DTO.HasSizeSortingEU ? DTO.sizeSortingEU.Equals(x.SpeciesList.sizeSortingEU) : true) //sizeSortingEU is the same
                                                            ).ToList();

                        lstUsedSpeciesDTOs.Add(DTO);
                    }
                    else
                    {
                        //Select other SubSamples with matching LandingCategory and dfuBase_Category on the same stepNum, that does not have a parent step SubSample
                        ssRefs = lstAllSubSamples.Where(x => HasWeight(x) && //Has weight
                                                             x.speciesListId != sl.speciesListId && //Is different SpeciesList
                                                             !x.SpeciesList.SubSample.Where(y => y.IsRepresentative && y.stepNum == intStepNum - 1).Any() && //Has no parent step
                                                             StringEquals(x.SpeciesList.landingCategory, sl.landingCategory) && //LandingCategory the same
                                                             StringEquals(x.SpeciesList.dfuBase_Category, sl.dfuBase_Category) && //sfuBase_Category the same
                                                             x.stepNum == intStepNum && //stepNum the same
                                                             GetSpeciesDTO(lstPotentialSpeciesDTOs, x.SpeciesList, intStepNum) == null
                                                             ).ToList();
                    }

                    //Assign nominator and denominator
                    curWeight = (curWeight ?? 0) + ssRefs.Sum(x => GetWeight(x)) + (dtoSubSampleCurrent != null ? GetWeight(dtoSubSampleCurrent).Value : 0);
                    parentWeight = GetWeight(dtoSubSampleParent).Value;
                }
                //No DTO's found to a SubSample
                else if ((parentSubSample == null || !HasWeight(parentSubSample)) && lstDTOs.Count == 0)
                {
                    AddMessage(DWMessage.NewError(GetCruiseId(sl), GetOrigin(sl), "SpeciesList " + GetSpeciesListKeyDetails(sl), sl.speciesListId.ToString(), String.Format("Raisingfaktoren kunne ikke udregnes. Det var ikke muligt at finde en matchende DTO{0}.", curSubSample != null ? string.Format(" for SubSample med Id = {0} og vægt = {1}", curSubSample.subSampleId, GetWeightOrLandingWeight(curSubSample)) : "")));
                    raisingFactor = null;
                    break;
                }


                //Calculate raising factor item.
                decimal? factor = null;
                if (parentWeight != null && (curWeight != null && curWeight > 0))
                    factor = parentWeight.Value / curWeight;

                if (factor == null)
                    continue; //ERROR ?

                if (raisingFactor == null)
                    raisingFactor = factor;
                else
                    raisingFactor *= factor;
            }

            return raisingFactor;
        }


        private string GetSpeciesListKeyDetails(Entities.Sprattus.SpeciesList sl)
        {
            string str = "[" + sl.speciesCode;

            if (!string.IsNullOrEmpty(sl.landingCategory))
                str += ", " + sl.landingCategory;

            if (!string.IsNullOrEmpty(sl.dfuBase_Category))
                str += ", " + sl.dfuBase_Category;

            if (sl.sizeSortingEU.HasValue)
                str += ", " + sl.sizeSortingEU.Value;

            if (!string.IsNullOrEmpty(sl.sizeSortingDFU))
                str += ", " + sl.sizeSortingDFU;

            if (!string.IsNullOrEmpty(sl.sexCode))
                str += ", " + sl.sexCode;

            if (!string.IsNullOrEmpty(sl.ovigorous))
                str += ", " + sl.ovigorous;

            if (!string.IsNullOrEmpty(sl.cuticulaHardness))
                str += ", " + sl.cuticulaHardness;

            return str + "]";
        }


        private bool UsingLandingWeight(Entities.Sprattus.SubSample s)
        {
            return !s.subSampleWeight.HasValue && s.landingWeight.HasValue;
        }


        private decimal? GetWeight(Entities.Sprattus.SubSample s)
        {
            return s.subSampleWeight;
        }

        private decimal? GetWeightOrLandingWeight(Entities.Sprattus.SubSample s)
        {
            return s.subSampleWeight ?? s.landingWeight;
        }

        private bool HasWeight(Entities.Sprattus.SubSample s)
        {
            return s.subSampleWeight.HasValue;
        }

        private bool HasWeightOrLandingWeight(Entities.Sprattus.SubSample s)
        {
            return s.subSampleWeight.HasValue || s.landingWeight.HasValue;
        }

        private Entities.Sprattus.SpeciesList GetDTO(List<Entities.Sprattus.SpeciesList> lstDTOs, Entities.Sprattus.SpeciesList sl)
        {
            Entities.Sprattus.SpeciesList DTO = null;

            //If several DTOs but one of them is NULL in categories, return a null DTO, since this is an error and should be reported.
            if (lstDTOs.Count > 1 && lstDTOs.Where(x => string.IsNullOrEmpty(x.landingCategory) && string.IsNullOrEmpty(x.dfuBase_Category)).Any())
                return null;

            //Find a DTO matching on landingcategory and dfuBase_category
            if ((DTO = lstDTOs.Where(x => StringEquals(x.landingCategory, sl.landingCategory) && StringEquals(x.dfuBase_Category, sl.dfuBase_Category)).FirstOrDefault()) != null)
            { }
            //If there is only 1 DTO with no landingcategory and dfuBase_category specified, then use this, even though sl.landingcategory and sl.dfuBase_category does not match
            else if (lstDTOs.Count == 1 && string.IsNullOrEmpty(lstDTOs.First().landingCategory) && string.IsNullOrEmpty(lstDTOs.First().dfuBase_Category))
                DTO = lstDTOs.First();

            return DTO;
        }



        private Entities.Sprattus.SpeciesList GetSpeciesDTO(List<Entities.Sprattus.SpeciesList> lstPotentialSpeciesDTOs, Entities.Sprattus.SpeciesList sl, int intStepNum)
        {
            Entities.Sprattus.SpeciesList DTO = null;

            var DTOs = lstPotentialSpeciesDTOs.Where(x => sl.speciesCode.Equals(x.speciesCode) &&
                                                      (x.SubSample.Where(y => y.IsRepresentative && y.stepNum == intStepNum - 1 && y.subSampleWeight.HasValue).Any()) &&
                                                      (StringEquals(x.landingCategory, sl.landingCategory) && StringEquals(x.dfuBase_Category, sl.dfuBase_Category)) &&
                                                      //  StringEquals(x.treatment, sl.treatment) &&   //MDU 22.06.2023: Added treatment join.
                                                      (x.sexCode == null || x.speciesCode.Equals(sl.sexCode)) &&
                                                      (x.ovigorous == null || x.ovigorous.Equals(sl.ovigorous)) &&
                                                      (x.cuticulaHardness == null || x.cuticulaHardness.Equals(sl.cuticulaHardness)) &&
                                                      (x.sizeSortingDFU == null || x.sizeSortingDFU.Equals(sl.sizeSortingDFU)) &&
                                                      (x.sizeSortingEU == null || x.sizeSortingEU.Equals(sl.sizeSortingEU))
                                                       );

            //Give DTO's a score based on how many of the below columns are != null and match the SL precisely. The more matches, the higher score for that DTO.
            //The DTO with the highest score is then selected. If two DTO have the same sore, the one with the lowest species id is chosen (so the SL added to the specieslist first).
            DTOs = DTOs.OrderByDescending(x => (x.sexCode != null ? 1 : 0) +
                                     (x.ovigorous != null ? 1 : 0) +
                                     (x.cuticulaHardness != null ? 1 : 0) +
                                     (x.sizeSortingDFU != null ? 1 : 0) +
                                     (x.sizeSortingEU != null ? 1 : 0))
                       .ThenBy(x => x.speciesListId);

            DTO = DTOs.FirstOrDefault();

            return DTO;
        }


        private bool StringEquals(string s1, string s2)
        {
            return (s1 == null && s2 == null) || (s1 != null && s1.Equals(s2, StringComparison.InvariantCultureIgnoreCase));
        }

        private bool IntEquals(int? s1, int? s2)
        {
            return (s1 == null && s2 == null) || (s1 != null && s1.Equals(s2));
        }
    }
}
