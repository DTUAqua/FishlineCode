using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.ViewModels.Input
{
    public class SpeciesListVIDItem : SpeciesListItem
    {

        #region Properties


       


        #endregion



        public SpeciesListVIDItem(SpeciesListViewModel slvm, SpeciesList sl = null)
            : base(slvm, sl)
        {
            
        }


        public override void BeforeSave()
        {
            if (Treatment == null)
                this.Treatment = "UR";
        }


        protected override bool IsPropertyUsed(string strProperty)
        {
            switch (strProperty)
            {
                case "Treatment":
                    return false;
            }

            return true;
        }
    }
}
