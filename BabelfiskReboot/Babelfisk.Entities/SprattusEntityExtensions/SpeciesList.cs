using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class SpeciesList : OfflineEntity
    {
        [DataMember]
        public bool IsLAVPresent
        {
            get { return _blnIsLAVPresent; }
            set
            {
                _blnIsLAVPresent = value;

                //Use below one instead of OnPropertyChanged, to make sure the object isn't rendered "dirty"
                OnNavigationPropertyChanged("IsLAVPresent");
                //OnPropertyChanged("IsLAVPresent");
            }
        }
        private bool _blnIsLAVPresent;


        [DataMember]
        public bool IsSFPresent
        {
            get { return _blnIsSFPresent; }
            set
            {
                _blnIsSFPresent = value;
                //Use below one instead of OnPropertyChanged, to make sure the object isn't rendered "dirty"
                OnNavigationPropertyChanged("IsSFPresent");
                //OnPropertyChanged("IsSFPresent");
            }
        }
        private bool _blnIsSFPresent;


        public bool IsDTO
        {
            get { return string.IsNullOrEmpty(speciesCode) ? false : speciesCode.Equals("DTO", StringComparison.InvariantCultureIgnoreCase); }
        }

        public bool HasSexCode
        {
            get { return !string.IsNullOrEmpty(sexCode); }
        }

        public bool HasOvigorous
        {
            get { return !string.IsNullOrEmpty(ovigorous); }
        }

        public bool HasCuticulaHardnesse
        {
            get { return !string.IsNullOrEmpty(cuticulaHardness); }
        }

        public bool HasSizeSortingDFU
        {
            get { return !string.IsNullOrEmpty(sizeSortingDFU); }
        }

        public bool HasSizeSortingEU
        {
            get { return sizeSortingEU.HasValue; }
        }

    }
}
