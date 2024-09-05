using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class Animal : OfflineEntity
    {
        public bool HasIndividNum
        {
            get { return this.individNum.HasValue && this.individNum.Value > 0; }
        }


        public SubSampleType SubSampleType
        {
            get
            {
                if (this.SubSample == null)
                    return Entities.SubSampleType.Unknown;

                if (this.SubSample.IsRepresentative)
                    return (HasIndividNum ? Entities.SubSampleType.SFRep : Entities.SubSampleType.LAVRep);
                else
                    return (HasIndividNum  ? Entities.SubSampleType.SFNotRep : Entities.SubSampleType.Other);
            }
        }
    }
}
