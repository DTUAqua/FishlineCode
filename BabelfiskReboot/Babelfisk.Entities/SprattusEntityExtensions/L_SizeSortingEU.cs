using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_SizeSortingEU : ILookupEntity, IComparable
    {
        /// <summary>
        /// speciesCode code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.sizeSortingEU.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1}", this.sizeSortingEU, this.description);
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
                string str = this.sizeSortingEU.ToString();
                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_SizeSortingEU other = obj as L_SizeSortingEU;

            if (other == null)
                return false;

            return this.L_sizeSortingEUId == other.L_sizeSortingEUId &&
                   this.sizeSortingEU.Equals(other.sizeSortingEU) &&
                  (this.description == null ? other.description == null : this.description.Equals(other.description))
                ;
        }


        public int CompareTo(object obj)
        {
            L_SizeSortingEU other = obj as L_SizeSortingEU;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (description != null && description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (lst.OfType<L_SizeSortingEU>().Where(x => (x.L_sizeSortingEUId != L_sizeSortingEUId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.sizeSortingEU.Equals(sizeSortingEU)).Count() > 0)
                return string.Format("En størrelses-sortering med kode '{0}' eksisterer allerede.", sizeSortingEU);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return String.Format("{0}", this.sizeSortingEU); }
        }


        public void NewLookupCreated()
        {
        }
    }
}
