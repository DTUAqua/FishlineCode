using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Babelfisk.Entities.Sprattus
{
    [DataContract(IsReference = true)]
    [Serializable]
    public partial class L_Year : INotifyPropertyChanged
    {
        [DataMember]
        public int Year
        {
            get { return _intYear; }
            set
            {
                _intYear = value;
                OnPropertyChanged("Year");
            }
        }
        private int _intYear;

        [DataMember]
        public int CruiseCount
        {
            get { return _intCruiseCount; }
            set
            {
                _intCruiseCount = value;
                OnPropertyChanged("CruiseCount");
                OnPropertyChanged("HasCruises");
            }
        }
        private int _intCruiseCount;


        public bool HasCruises
        {
            get { return CruiseCount > 0; }
        }


        public string CompareValue
        {
            get { return Year.ToString(); }
        }


        public override bool Equals(object obj)
        {
            L_Year other = obj as L_Year;

            if (other == null)
                return false;

            return this.Year == other.Year;
        }

         /*
        public override int GetHashCode()
        {
            return Year.GetHashCode();
        }
         */

        public override string ToString()
        {
            return CompareValue;
        }


        protected virtual void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
