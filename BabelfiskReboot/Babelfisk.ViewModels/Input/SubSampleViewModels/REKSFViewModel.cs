using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities;

namespace Babelfisk.ViewModels.Input
{
    public class REKSFViewModel : SFViewModel
    {
        public REKSFViewModel(SubSampleViewModel parent, SubSampleType enmSubSampleType)
            : base(parent, enmSubSampleType)
        {
        }
    }
}
