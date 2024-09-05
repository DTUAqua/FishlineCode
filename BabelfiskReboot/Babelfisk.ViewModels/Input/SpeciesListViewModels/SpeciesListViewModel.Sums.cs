using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.Input
{
    public partial class SpeciesListViewModel
    {

        #region Properties


        public decimal? Step0Sum
        {
            get
            {
                return GetSum<SpeciesListItem>(x => x.SubSampleWeightStep0Dec);
            }
        }


        public decimal? Step0SumUR
        {
            get
            {

                return GetSumFactor<SpeciesListItem>(x => x.SubSampleWeightStep0Dec, GetTreatmentFactor);
            }
        }


        public Tuple<decimal?, decimal?> Step0SumTuple
        {
            get { return new Tuple<decimal?, decimal?>(Step0Sum, Step0SumUR); }
        }


        public decimal? Step0LandingSum
        {
            get
            {
                return GetSum<SpeciesListHVNItem>(x => x.SubSampleLandingWeightStep0);
            }
        }


        public decimal? Step1Sum
        {
            get
            {
                return GetSum<SpeciesListItem>(x => x.SubSampleWeightStep1Dec);
            }
        }


        public decimal? Step1SumUR
        {
            get
            {

                return GetSumFactor<SpeciesListItem>(x => x.SubSampleWeightStep1Dec, GetTreatmentFactor);
            }
        }

        public Tuple<decimal?, decimal?> Step1SumTuple
        {
            get { return new Tuple<decimal?, decimal?>(Step1Sum, Step1SumUR); }
        }


        public decimal? Step2Sum
        {
            get
            {
                return GetSum<SpeciesListItem>(x => x.SubSampleWeightStep2Dec);
            }
        }


        public decimal? Step2SumUR
        {
            get
            {

                return GetSumFactor<SpeciesListItem>(x => x.SubSampleWeightStep2Dec, GetTreatmentFactor);
            }
        }


        public Tuple<decimal?, decimal?> Step2SumTuple
        {
            get { return new Tuple<decimal?, decimal?>(Step2Sum, Step2SumUR); }
        }


        public decimal? StepNotRepSum
        {
            get
            {
                return GetSum<SpeciesListItem>(x => x.SubSampleWeightNotRepDec);
            }
        }


        public string StepNotRepSumString
        {
            get
            {
                var res = StepNotRepSum;
                return res == null ? null : res.Value.ToString("0.####");
            }
        }


        public decimal? StepBMSNotRepSum
        {
            get
            {
                return GetSum<SpeciesListItem>(x => x.SubSampleWeightBMSNotRepDec);
            }
        }

        public string StepBMSNotRepSumString
        {
            get
            {
                var res = StepBMSNotRepSum;
                return res == null ? null : res.Value.ToString("0.####");
            }
        }


        #endregion



        private decimal? GetSum<T>(Func<T, decimal?> getWeight) 
        {
            if (Items == null || Items.Count == 0)
                return null;

            var q = Items.OfType<T>().Where(x => getWeight(x).HasValue);

            var dec = (q.Count() == 0) ? new Nullable<decimal>() : Math.Round(q.Sum(x => getWeight(x).Value), 4);
            return dec;
        }

        private decimal? GetSumFactor<T>(Func<T, decimal?> getWeight, Func<T, decimal> getFactor)
        {
            if (Items == null || Items.Count == 0)
                return null;

            var q = Items.OfType<T>().Where(x => getWeight(x).HasValue);

            var dec = (q.Count() == 0) ? new Nullable<decimal>() : q.Sum(x => getWeight(x).Value * getFactor(x));
            return dec;
        }


        private bool HasTreatmentFactor(SpeciesListItem sli)
        {
            if (sli == null || sli.Treatment == null || _lstTreatmentFactors == null || _lstSpecies == null)
                return false;

            var species = _lstSpecies.Where(x => x.speciesCode == sli.Species).FirstOrDefault();

            if (species == null)
                return false;

            var factor = Babelfisk.Warehouse.EntityFactory.GetTreatmentFactor(_sample.dateGearStart, species.treatmentFactorGroup, sli.Treatment, _lstTreatmentFactors);
            return factor != null;
        }

        private decimal GetTreatmentFactor(SpeciesListItem sli)
        {
            if (sli == null || sli.Treatment == null || _lstTreatmentFactors == null || _lstSpecies == null)
                return 1;

            var species = _lstSpecies.Where(x => x.speciesCode == sli.Species).FirstOrDefault();

            if (species == null)
                return 1;

            var factor = Babelfisk.Warehouse.EntityFactory.GetTreatmentFactor(_sample.dateGearStart, species.treatmentFactorGroup, sli.Treatment, _lstTreatmentFactors);
   
             if (factor == null)
                 return 1;

            return factor.Value;
        }


        public void RefreshSums()
        {
            RaisePropertyChanged(() => Step0Sum);
            RaisePropertyChanged(() => Step0SumUR);
            RaisePropertyChanged(() => Step0SumTuple);
            RaisePropertyChanged(() => Step0LandingSum);
            RaisePropertyChanged(() => Step1Sum);
            RaisePropertyChanged(() => Step1SumUR);
            RaisePropertyChanged(() => Step1SumTuple);
            RaisePropertyChanged(() => Step2Sum);
            RaisePropertyChanged(() => Step2SumUR);
            RaisePropertyChanged(() => Step2SumTuple);
            RaisePropertyChanged(() => StepNotRepSum);
            RaisePropertyChanged(() => StepNotRepSumString);
            RaisePropertyChanged(() => StepBMSNotRepSum);
            RaisePropertyChanged(() => StepBMSNotRepSumString);
        }
    }
}
