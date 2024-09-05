using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using Anchor.Core;

namespace Babelfisk.BusinessLogic.Settings
{
    [DataContract(IsReference=true)]
    public class OfflineStatus : SettingsBaseObject<OfflineStatus>
    {
        private bool _blnIsOffline = false;

        [DataMember]
        public bool IsOffline
        {
            get { return _blnIsOffline; }
            set
            {
                _blnIsOffline = value;
                RaisePropertyChanged(() => IsOffline);
            }
        }


        public OfflineStatus()
        {
        }


        public void RaiseAllPropertiesChanged()
        {
            RaisePropertyChanged(() => IsOffline);
        }
    }
}
