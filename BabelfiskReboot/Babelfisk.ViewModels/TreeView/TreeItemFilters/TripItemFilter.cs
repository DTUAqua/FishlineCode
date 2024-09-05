using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.TreeView.TreeItemFilters
{
    public class TripItemFilter : ITreeItemFilter
    {
        private string _strTripType;


        public string Name
        {
            get { return "Trip"; }
        }

        public string TripType
        {
            get { return _strTripType; }
            private set
            {
                _strTripType = value;
            }
        }


        public TripItemFilter(string strTripType)
        {
            TripType = strTripType;
        }
    }
}
