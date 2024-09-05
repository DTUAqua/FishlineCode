using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Babelfisk.Entities.SprattusSecurity;

namespace Babelfisk.BusinessLogic.Settings
{
    [DataContract(IsReference = true)]
    public class OfflineUsers : SettingsBaseObject<OfflineUsers>
    {
        List<Users> _lstUsers;

        private bool _blnInitialized = false;

        [DataMember]
        public List<Users> Users
        {
            get { return _lstUsers; }
            set
            {
                _lstUsers = value;
                RaisePropertyChanged(() => Users);
            }
        }


        [DataMember]
        public bool IsInitialized
        {
            get { return _blnInitialized; }
            set
            {
                _blnInitialized = value;
                RaisePropertyChanged(() => IsInitialized);
            }
        }



        public OfflineUsers()
        {
            _lstUsers = new List<Users>();
        }


        public void AssignUsers(List<Users> lstUsers)
        {
            IsInitialized = true;
            _lstUsers = lstUsers.ToList();
        }

    }
}
