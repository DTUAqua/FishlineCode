using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Babelfisk.Entities
{
    [DataContract]
    public class QueryListItem : INotifyPropertyChanged
    {
        [DataMember]
        private object _strDisplayName;

        [DataMember]
        private object _strValue;


        #region Properties

        public object DisplayName
        {
            get { return _strDisplayName; }
            set
            {
                _strDisplayName = value;
                OnPropertyChanged("DisplayName");
            }
        }

        public object Value
        {
            get { return _strValue; }
            set
            {
                _strValue = value;
                OnPropertyChanged("Value");
            }
        }

        #endregion


        public QueryListItem()
        {
}


        public QueryListItem(string strDisplayName, string strValue)
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
