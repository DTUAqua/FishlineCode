using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Babelfisk.BusinessLogic.Settings
{
    [DataContract(IsReference=true)]
    public class DataGridColumnSettings : SettingsBaseObject<DataGridColumnSettings>
    {
        private string _strColumnName = null;

        private string _strColumnUIName = null;

        private bool? _blnIsVisible = null;

        private bool _blnDefaultIsVisible = false;

        private int _intSort = 0;

        [DataMember]
        private DataGridColumnSettingsContainer _parent;


        public Action<DataGridColumnSettings> IsVisibleChanged
        {
            get;
            set;
        }


        #region Properties


        /// <summary>
        /// Return column name. 
        /// </summary>
        [DataMember]
        public string ColumnName
        {
            get { return _strColumnName; }
            set
            {
                _strColumnName = value;
                RaisePropertyChanged(() => ColumnName);
            }
        }

        /// <summary>
        /// Return column name. 
        /// </summary>
        public string ColumnUIName
        {
            get { return _strColumnUIName; }
            set
            {
                _strColumnUIName = value;
                RaisePropertyChanged(() => ColumnUIName);
            }
        }


        /// <summary>
        /// Determines whether the column is visible or not. If it is null, the default visibilty value for the column should be used.
        /// </summary>
        [DataMember]
        public bool IsVisible
        {
            get { return _blnIsVisible ?? DefaultIsVisible; }
            set
            {
                _blnIsVisible = value;
                RaisePropertyChanged(() => IsVisible);

                if (IsVisibleChanged != null)
                    IsVisibleChanged(this);
            }
        }


        public int Sorting
        {
            get { return _intSort; }
            set
            {
                _intSort = value;
            }
        }


        public DataGridColumnSettingsContainer Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
            }
        }


        public bool DefaultIsVisible
        {
            get { return _blnDefaultIsVisible; }
            set
            {
                _blnDefaultIsVisible = value;
                RaisePropertyChanged(() => DefaultIsVisible);
            }
        }


        #endregion


        public DataGridColumnSettings()
        { }

        public DataGridColumnSettings(string strColumnName, DataGridColumnSettingsContainer parent)
        {
            _strColumnName = strColumnName;
            _parent = parent;
        }



        public void ResetToDefault()
        {
            _blnIsVisible = null;
            RaisePropertyChanged(() => IsVisible);
        }
    }
}
