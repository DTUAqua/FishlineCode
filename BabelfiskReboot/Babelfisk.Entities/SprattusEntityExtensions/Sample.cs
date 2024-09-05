using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Babelfisk.Entities.Sprattus
{
     [Serializable]
    public partial class Sample : OfflineEntity
    {
        [DataMember]
        public int SpeciesListCount
        {
            get { return _intSpeciesListCount; }
            set
            {
                _intSpeciesListCount = value;
                OnNavigationPropertyChanged("SampleCount");
                OnNavigationPropertyChanged("HasSamples");
            }
        }
        private int _intSpeciesListCount;


        public bool HasSpeciesList
        {
            get { return SpeciesListCount > 0; }
        }
    }
}
