using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_OtolithReadingRemark : ILookupEntity, IComparable
    {
        /// <summary>
        /// speciesCode code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.L_OtolithReadingRemarkID.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.num, this.otolithReadingRemark, this.description);
            }
        }


        public string DefaultSortValue
        {
            get { return FilterValue; }
        }


        public string UIDisplay
        {
            get
            {
                string str = "";

                if (num.HasValue)
                    str = num + " - ";

                str += this.otolithReadingRemark;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_OtolithReadingRemark other = obj as L_OtolithReadingRemark;

            if (other == null)
                return false;

            return this.L_OtolithReadingRemarkID == other.L_OtolithReadingRemarkID &&
                (this.otolithReadingRemark == null ? other.otolithReadingRemark == null : this.otolithReadingRemark.Equals(other.otolithReadingRemark)) &&
                (this.description == null ? other.description == null : this.description.Equals(other.description))
                ;
        }


        public int CompareTo(object obj)
        {
            L_OtolithReadingRemark other = obj as L_OtolithReadingRemark;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(otolithReadingRemark))
                return "Angiv venligst kode.";

            if (otolithReadingRemark.Length > 50)
                return "Koden må kun bestå af 50 tegn.";

            if (description != null && description.Length > 250)
                return "Beskrivelse må kun bestå af 250 tegn.";

            if (lst.OfType<L_OtolithReadingRemark>().Where(x => (x.L_OtolithReadingRemarkID != L_OtolithReadingRemarkID || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.otolithReadingRemark.Equals(otolithReadingRemark, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En række med kode '{0}' eksisterer allerede.", otolithReadingRemark);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
       /* public string CompareValue
        {
            get { return String.Format("{0} {1}", this.num, this.otolithReadingRemark); }
        }*/


        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[2] { this.num.HasValue ? this.num.Value.ToString() : "", this.otolithReadingRemark }; }
        }

        public void NewLookupCreated()
        {
        }
    }
}
