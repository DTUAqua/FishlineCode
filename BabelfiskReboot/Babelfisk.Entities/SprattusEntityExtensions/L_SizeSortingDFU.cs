using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_SizeSortingDFU : ILookupEntity, IComparable
    {
        /// <summary>
        /// speciesCode code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.sizeSortingDFU.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.num, this.sizeSortingDFU, this.description);
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

                str += this.sizeSortingDFU;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_SizeSortingDFU other = obj as L_SizeSortingDFU;

            if (other == null)
                return false;

            return this.L_sizeSortingDFUId == other.L_sizeSortingDFUId &&
                (this.sizeSortingDFU == null ? other.sizeSortingDFU == null : this.sizeSortingDFU.Equals(other.sizeSortingDFU)) &&
                (this.description == null ? other.description == null : this.description.Equals(other.description))
                ;
        }


        public int CompareTo(object obj)
        {
            L_SizeSortingDFU other = obj as L_SizeSortingDFU;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(sizeSortingDFU))
                return "Angiv venligst kode.";

            if (sizeSortingDFU.Length > 3)
                return "Koden må kun bestå af 3 tegn.";

            if (description != null && description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (lst.OfType<L_SizeSortingDFU>().Where(x => (x.L_sizeSortingDFUId != L_sizeSortingDFUId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.sizeSortingDFU.Equals(sizeSortingDFU, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En størrelse med kode '{0}' eksisterer allerede.", sizeSortingDFU);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
       /* public string CompareValue
        {
            get { return String.Format("{0} {1}", this.num, this.sizeSortingDFU); }
        }*/


        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[2] { this.num.HasValue ? this.num.Value.ToString() : "", this.sizeSortingDFU }; }
        }


        public void NewLookupCreated()
        {
        }
    }
}
