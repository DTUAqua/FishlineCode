using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_LandingCategory : ILookupEntity, IComparable
    {
        /// <summary>
        /// speciesCode code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.landingCategory.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.num, this.landingCategory, this.description);
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

                str += this.landingCategory;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_LandingCategory other = obj as L_LandingCategory;

            if (other == null)
                return false;

            return this.L_landingCategoryId == other.L_landingCategoryId &&
                (this.landingCategory == null ? other.landingCategory == null : this.landingCategory.Equals(other.landingCategory)) &&
                (this.description == null ? other.description == null : this.description.Equals(other.description))
                ;
        }


        public int CompareTo(object obj)
        {
            L_LandingCategory other = obj as L_LandingCategory;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(landingCategory))
                return "Angiv venligst kode.";

            if (landingCategory.Length > 3)
                return "Koden må kun bestå af 3 tegn.";

            if (description != null && description.Length > 70)
                return "Beskrivelse må kun bestå af 70 tegn.";

            if (lst.OfType<L_LandingCategory>().Where(x => (x.L_landingCategoryId != L_landingCategoryId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.landingCategory.Equals(landingCategory, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En landingskategori med kode '{0}' eksisterer allerede.", landingCategory);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        /*public string CompareValue
        {
            get { return String.Format("{0} {1}", this.num, this.landingCategory); }
        }*/

        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[2] { this.num.HasValue ? this.num.Value.ToString() : "", this.landingCategory }; }
        }


        public void NewLookupCreated()
        {
        }
    }
}
