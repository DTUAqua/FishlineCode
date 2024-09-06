using Babelfisk.Entities;
using Babelfisk.Entities.Sprattus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.BusinessLogic.Lookups
{
    public partial class L_Ovigorous : OfflineEntity, ILookupEntity, IComparable, ILocalLookupEntity
    {
        private string _ovigorousCode;

        private string _description;



        public string OvigorousCode
        {
            get { return _ovigorousCode; }
            set
            {
                _ovigorousCode = value;
            }
        }


        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
            }
        }



        public string Id
        {
            get { return _ovigorousCode; }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1}", this._ovigorousCode, this._description);
            }
        }



        public string DefaultSortValue
        {
            get { return _ovigorousCode; }
        }


        public string UIDisplay
        {
            get
            {
                string str = this._ovigorousCode;
                if (!string.IsNullOrWhiteSpace(_description))
                    str += " - " + _description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_Ovigorous other = obj as L_Ovigorous;

            if (other == null)
                return false;

            return this._ovigorousCode == other._ovigorousCode;
        }

        public int CompareTo(object obj)
        {
            L_Ovigorous other = obj as L_Ovigorous;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(_ovigorousCode))
                return "Angiv venligst kode.";

            if (_description.Length > 3)
                return "Koden må kun bestå af 3 tegn.";

            if (_description != null && _description.Length > 80)
                return "Danske navn må kun bestå af 80 tegn.";

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return _ovigorousCode; }
        }

        ObjectChangeTracker _ct = new ObjectChangeTracker();

        public ObjectChangeTracker ChangeTracker
        {
            get { return _ct; }
            set
            {
                _ct = value;
            }
        }

        public void NewLookupCreated()
        {
        }
    }
}
