using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Babelfisk.Entities.Sprattus
{
     [Serializable]
    public partial class Trip : OfflineEntity
    {
        [DataMember]
        public int SampleCount
        {
            get { return _intSampleCount; }
            set
            {
                _intSampleCount = value;
                OnNavigationPropertyChanged("SampleCount");
                OnNavigationPropertyChanged("HasSamples");
            }
        }
        private int _intSampleCount;

        public bool HasSamples
        {
            get { return SampleCount > 0; }
        }


        [DataMember]
        public int SpeciesListCount
        {
            get { return _intSpeciesListCount; }
            set
            {
                _intSpeciesListCount = value;
                OnNavigationPropertyChanged("SpeciesListCount");
                OnNavigationPropertyChanged("HasSpeciesLists");
            }
        }
        private int _intSpeciesListCount;

        public bool HasSpeciesLists
        {
            get { return _intSpeciesListCount > 0; }
        }


        public bool IsHVN
        {
            get
            {
                if (tripType == null)
                    return false;

                return tripType.ToLower().Equals("hvn");
            }
        }

        public bool IsVID
        {
            get
            {
                if (tripType == null)
                    return false;

                return tripType.ToLower().Equals("vid");
            }
        }

        public bool IsSEA
        {
            get
            {
                if (tripType == null)
                    return false;

                return tripType.ToLower().Equals("søs");
            }
        }

        public object Tag
        {
            get;
            set;
        }

    }
}
