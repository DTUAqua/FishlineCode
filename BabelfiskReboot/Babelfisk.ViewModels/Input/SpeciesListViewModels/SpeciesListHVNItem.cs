using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.ViewModels.Input
{
    public class SpeciesListHVNItem : SpeciesListItem
    {
        
       
        #region Properties


       


        public decimal? SubSampleLandingWeightStep0
        {
            get { return GetSubsample(0) == null ? null : GetSubsample(0).landingWeight; }
            set
            {
                var s = GetSubsample(0);

                if (s != null)
                    s.landingWeight = value;
                else
                {
                    if (value != null)
                    {
                        s = new SubSample() { stepNum = 0, representative = "ja" };
                        s.landingWeight = value;
                        AddSubSample(s);
                    }
                }

                IsDirty = true;
                RaisePropertyChanged(() => SubSampleLandingWeightStep0);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        #endregion


        public override bool IsWeightsSpecified
        {
            get
            {
                return base.IsWeightsSpecified/* || SubSampleLandingWeightStep0 != null*/;
            }
        }


        public SpeciesListHVNItem(SpeciesListViewModel slvm, SpeciesList sl = null)
            : base(slvm, sl)
        {
           
        }


        protected override void AfterCreated(SpeciesListViewModel slvm)
        {
            try
            {
                if (this.IsNew && slvm != null && slvm.Trip != null && slvm.Trip.L_FisheryType != null && !string.IsNullOrEmpty(slvm.Trip.L_FisheryType.landingCategory))
                {
                    string strCode = slvm.Trip.L_FisheryType.landingCategory;

                    LandingCategory = strCode;
                    IsDirty = false;
                }
            }
            catch(Exception e)
            {
                DispatchMessageBox("Det var ikke muligt at sætte landingskategori automatisk ud fra turens fiskeritype (for nye rækker). " + e.Message);
            }
        }



        protected override string ValidateListItem(string strPropertyName)
        {
            string strError = null;

            switch (strPropertyName)
            {
                case "SizeSorting":
                    //Not mandatory
                    break;
            }

            return strError;
        }

    }
}
