using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;

namespace Babelfisk.ViewModels.Input
{
    public class SpeciesListSEAItem : SpeciesListItem
    {
        #region Properties


        public string WeightEstimationMethod
        {
            get { return _speciesListEntity.weightEstimationMethod; }
            set
            {
                _speciesListEntity.weightEstimationMethod = value;
                IsDirty = true;
                RaisePropertyChanged(() => WeightEstimationMethod);
                RaisePropertyChanged(() => HasUnsavedData);
            }
        }


        #endregion



        public SpeciesListSEAItem(SpeciesListViewModel slvm, SpeciesList sl = null)
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

                    //Only default species list LandingCategory to "IND", if fishery type is "Industry" on SØS Trip.
                    if (strCode != null && strCode.Equals("ind", StringComparison.InvariantCultureIgnoreCase))
                    {
                        LandingCategory = strCode;
                        IsDirty = false;
                    }
                }
            }
            catch (Exception e)
            {
                DispatchMessageBox("Det var ikke muligt at sætte landingskategori automatisk ud fra turens fiskeritype (for nye rækker). " + e.Message);
            }
        }

    }
}
