using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.WPF.Infrastructure
{
    internal interface IInvalidatable
    {
        void InvalidateView();
    }
}
