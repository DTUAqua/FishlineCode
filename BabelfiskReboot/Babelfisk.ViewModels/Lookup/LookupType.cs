using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using Babelfisk.Entities;

namespace Babelfisk.ViewModels.Lookup
{
    public class LookupType : AViewModel 
    {
        private string _strLookupName;

        private Type _tLookup;

        private LookupColumn[] _columns;

        private string[] _arrTypeIncludes;

        private Action<Babelfisk.BusinessLogic.LookupManager> _aLoadLists;

        private ChildLookupType _childLookupType;

        private List<SecurityTask> _lstSecurityTasks;

        private bool _blnCanEditOffline;

        private string _message;

        #region Properties


        public string Name
        {
            get { return _strLookupName; }
        }

        public Type Type
        {
            get { return _tLookup; }
        }


        public LookupColumn[] Columns
        {
            get { return _columns; }
        }


        public string[] TypeIncludes
        {
            get { return _arrTypeIncludes; }
        }


        public ChildLookupType ChildLookupType
        {
            get { return _childLookupType; }
            set
            {
                _childLookupType = value;
                RaisePropertyChanged(() => ChildLookupType);
            }
        }


      /*  public List<string> Roles
        {
            get { return _lstRoles; }
            set
            {
                _lstRoles = value;
                RaisePropertyChanged(() => Roles);
            }
        }*/

        public List<SecurityTask> SecurityTasks
        {
            get { return _lstSecurityTasks; }
            set
            {
                _lstSecurityTasks = value;
                RaisePropertyChanged(() => SecurityTasks);
            }
        }


        public bool HasEditingRights
        {
            get
            {
                if (SecurityTasks == null)
                    return true;

                return User.HasOneOrMoreTasks(SecurityTasks.ToArray());
            }
        }


        public bool CanEditOffline
        {
            get { return _blnCanEditOffline; }
            set { _blnCanEditOffline = true; }
        }


        public bool IsOffline
        {
            get { return BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline; }
        }


        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged(() => Message);
                RaisePropertyChanged(() => HasMessage);
            }
        }


        public bool HasMessage
        {
            get { return !string.IsNullOrWhiteSpace(_message); }
        }


        #endregion


        public LookupType(string strLookupName, Type lookupType, string[] arrTypeIncludes, Action<Babelfisk.BusinessLogic.LookupManager> loadLists, params LookupColumn[] columns)
        {
            _blnCanEditOffline = false;
            _strLookupName = strLookupName;
            _tLookup = lookupType;
            _arrTypeIncludes = arrTypeIncludes;
            _columns = columns;
            _aLoadLists = loadLists;
            _lstSecurityTasks = new List<SecurityTask>() { SecurityTask.EditLookups }; //Admins can add lookups per default.
        }


        public void LoadLists(Babelfisk.BusinessLogic.LookupManager lm)
        {
            if (_aLoadLists != null)
                _aLoadLists(lm);
        }
    }
}
