using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_SDLightType : ILookupEntity, IComparable
    {
        public string Id
        {
            get { return this.L_sdLightTypeId.ToString(); }
        }


        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1}", this.lightType, this.dkDescription ?? "");
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

                str += this.lightType;

                if (!string.IsNullOrWhiteSpace(dkDescription))
                    str += " - " + dkDescription;

                return str;
            }
        }

        public override bool Equals(object obj)
        {
            L_SDLightType other = obj as L_SDLightType;

            if (other == null)
                return false;

            return this.L_sdLightTypeId == other.L_sdLightTypeId &&
                (this.lightType != null ? this.lightType.Equals(other.lightType) : other.lightType == null) &&
                (dkDescription != null ? dkDescription.Equals(other.dkDescription) : other.dkDescription == null) &&
                (ukDescription != null ? ukDescription.Equals(other.ukDescription) : other.ukDescription == null);
        }

        public int CompareTo(object obj)
        {
            L_SDLightType other = obj as L_SDLightType;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }
        public void NewLookupCreated()
        {
        }

        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(lightType))
                return "Angiv venligst lystype.";

            if (lightType.Length > 15)
                return "Lystypen må kun bestå af 15 tegn.";

            if (dkDescription != null && dkDescription.Length > 500)
                return "Beskrivelse (DK) må kun bestå af 500 tegn.";

            if(ukDescription != null && ukDescription.Length > 500)
                return "Beskrivelse (UK) må kun bestå af 500 tegn.";

            if (lst.OfType<L_SDLightType>().Where(x => (x.L_sdLightTypeId != L_sdLightTypeId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.lightType.Equals(lightType, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("Lystypen '{0}' eksisterer allerede.", lightType);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[1] { lightType }; }
        }
    }
}
