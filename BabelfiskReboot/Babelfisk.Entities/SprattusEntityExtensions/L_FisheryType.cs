using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_FisheryType : ILookupEntity, IComparable
    {
        private static List<L_LandingCategory> _lstLandingCategories;

        private static Dictionary<string, L_LandingCategory> _dicLandingCategories = null;

        /// <summary>
        /// samplingTypeId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.fisheryType.ToString(); }
        }


        public static List<L_LandingCategory> LandingCategories
        {
            get { return _lstLandingCategories; }
            set
            {
                _lstLandingCategories = value;
                if (value != null)
                    _dicLandingCategories = value.ToDictionary(x => x.landingCategory);
            }
        }


        public string L_LandingCategoryUIDisplay
        {
            get
            {
                if (landingCategory == null || _dicLandingCategories == null || !_dicLandingCategories.ContainsKey(landingCategory))
                    return "";

                return _dicLandingCategories[landingCategory].UIDisplay;
            }
        }


        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.fisheryType, this.landingCategory, this.description);
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
                string str = this.fisheryType;
                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_FisheryType other = obj as L_FisheryType;

            if (other == null)
                return false;

            return this.L_fisheryTypeId == other.L_fisheryTypeId &&
                (this.fisheryType != null ? this.fisheryType.Equals(other.fisheryType, StringComparison.InvariantCultureIgnoreCase) : other.fisheryType == null) &&
                (landingCategory != null ? landingCategory.Equals(other.landingCategory, StringComparison.InvariantCultureIgnoreCase) : other.landingCategory == null) &&
                (description != null ? description.Equals(other.description, StringComparison.InvariantCultureIgnoreCase) : other.description == null);
        }


        public int CompareTo(object obj)
        {
            L_FisheryType other = obj as L_FisheryType;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(fisheryType))
                return "Angiv venligst kode.";

            if (fisheryType.Length > 6)
                return "Koden må kun bestå af 6 tegn.";

            if (description != null && description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (lst.OfType<L_FisheryType>().Where(x => (x.L_fisheryTypeId != L_fisheryTypeId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.fisheryType.Equals(fisheryType, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En række med kode '{0}' eksisterer allerede.", fisheryType);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return UIDisplay; }
        }


        public void NewLookupCreated()
        {
        }

    }
}
