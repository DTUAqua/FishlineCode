using Babelfisk.Entities.Sprattus;
using Babelfisk.Entities.SprattusSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.SmartDots
{
    /// <summary>
    /// Items is used on the AddEditSDEventView to add age readers to an event. 
    /// </summary>
    public class AccessibleUserItem : AViewModel
    {
        private List<Users> _lstUsers;

        private Users _selectedUser;


        #region Properties


        /// <summary>
        /// List of users to choose from
        /// </summary>
        public List<Users> Users
        {
            get { return _lstUsers; }
        }


        /// <summary>
        /// Selected user/age reader.
        /// </summary>
        public Users SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                RaisePropertyChanged(() => SelectedUser);
                RaisePropertyChanged(() => HasSelectedUser);
            }
        }


        /// <summary>
        /// Whether or not a user/age reader has been selected yet.
        /// </summary>
        public bool HasSelectedUser
        {
            get { return _selectedUser != null;
            }
        }

        #endregion



        public AccessibleUserItem(List<Users> lstUsers, Users usr = null)
        {
            _lstUsers = lstUsers.ToList();

            _selectedUser = usr;
        }
    }
}
