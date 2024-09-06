using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FishLineMeasure.WPF.Infrastructure
{
    internal interface IInvalidatable
    {
        void InvalidateView();
    }
}
