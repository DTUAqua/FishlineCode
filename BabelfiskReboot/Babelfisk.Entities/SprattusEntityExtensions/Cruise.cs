using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Babelfisk.Entities.Sprattus
{
     [Serializable]
    public partial class Cruise : OfflineEntity
    {
        [DataMember]
        public int TripCount
        {
            get { return _intTripCount; }
            set
            {
                _intTripCount = value;
                OnNavigationPropertyChanged("TripCount");
                OnNavigationPropertyChanged("HasTrips");
            }
        }
        private int _intTripCount;


        public bool HasTrips
        {
            get { return TripCount > 0; }
        }
    }
}
