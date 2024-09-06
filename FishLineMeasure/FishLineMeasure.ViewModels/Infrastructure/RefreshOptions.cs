using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels
{
    public class RefreshOptions
    {
        AViewModel[] _omit;
        Type[] _types;

        public AViewModel[] Omit
        {
            get { return _omit; }
        }

        public Type[] Types
        {
            get { return _types; }
        }


        public RefreshOptions(AViewModel[] omit = null, Type[] types = null)
        {
            _omit = omit ?? new AViewModel[] { };
            _types = types ?? new Type[] { };
        }
    }
}
