using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.ViewModels.Lookup
{
    public class ChildLookupType : LookupType
    {
        private string _strParentKeyPropertyName;

        private string _strForeignKeyPropertyName;


        #region Properties


        public string ParentKeyPropertyName
        {
            get { return _strParentKeyPropertyName; }
            set
            {
                _strParentKeyPropertyName = value;
                RaisePropertyChanged(() => ParentKeyPropertyName);
            }
        }


        public string ForeignKeyPropertyName
        {
            get { return _strForeignKeyPropertyName; }
            set
            {
                _strForeignKeyPropertyName = value;
                RaisePropertyChanged(() => ForeignKeyPropertyName);
            }
        }

        #endregion


        public ChildLookupType(string strLookupName, Type lookupType, string[] arrTypeIncludes, Action<Babelfisk.BusinessLogic.LookupManager> loadLists, string strParentKeyPropertyName, string strForeignKeyPropertyName, params LookupColumn[] columns) 
            : base(strLookupName, lookupType, arrTypeIncludes, loadLists, columns)
        {
            _strParentKeyPropertyName = strParentKeyPropertyName;
            _strForeignKeyPropertyName = strForeignKeyPropertyName;
        }
    }
}
