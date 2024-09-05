using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class SubSample : OfflineEntity
    {

        #region Properties


        public bool IsRepresentative
        {
            get { return this.representative != null && this.representative.Equals("ja", StringComparison.InvariantCultureIgnoreCase); }
        }

        public bool? HasLAVRep
        {
            get 
            {
                return HasSubSampleType(SubSampleType.LAVRep, true);
            }
        }


        public bool? HasSFRep
        {
            get
            {
                return HasSubSampleType(SubSampleType.SFRep, true);
            }
        }


        public bool? HasSFNotRep
        {
            get
            {
                return HasSubSampleType(SubSampleType.SFNotRep, false);
            }
        }


        #endregion



        public bool? HasSubSampleType(SubSampleType sst, bool blnRep)
        {
             if (Animal == null)
                    return null;

             return IsRepresentative == blnRep && Animal.Where(x => x.SubSampleType == sst).Count() > 0;
        }
    }
}
