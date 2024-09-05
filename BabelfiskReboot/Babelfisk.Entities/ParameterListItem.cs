using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Babelfisk.Entities
{
    [DataContract]
    public class ParameterListItem : INotifyPropertyChanged
    {
        [DataMember]
        private string _strDisplayName;

        [DataMember]
        private string _strValue;

        private bool _blnIsChecked;


        #region Properties

        public string DisplayName
        {
            get { return _strDisplayName; }
            set
            {
                _strDisplayName = value;
                OnPropertyChanged("DisplayName");
            }
        }

        public string Value
        {
            get { return _strValue; }
            set
            {
                _strValue = value;
                OnPropertyChanged("Value");
            }
        }


        public bool IsChecked
        {
            get { return _blnIsChecked; }
            set
            {
                _blnIsChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        #endregion


        public ParameterListItem()
        {
}


        public ParameterListItem(string strDisplayName, string strValue)
        {
            _strDisplayName = strDisplayName;
            _strValue = strValue;
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        #endregion
    }
}
