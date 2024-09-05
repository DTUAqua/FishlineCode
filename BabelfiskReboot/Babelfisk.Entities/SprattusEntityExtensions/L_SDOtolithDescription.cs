using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_SDOtolithDescription : ILookupEntity, IComparable
    {
        public string Id
        {
            get { return this.L_sdOtolithDescriptionId.ToString(); }
        }


        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1}", this.otolithDescription, this.dkDescription ?? "");
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

                str += this.otolithDescription;

                if (!string.IsNullOrWhiteSpace(dkDescription))
                    str += " - " + dkDescription;

                return str;
            }
        }

        public override bool Equals(object obj)
        {
            L_SDOtolithDescription other = obj as L_SDOtolithDescription;

            if (other == null)
                return false;

            return this.L_sdOtolithDescriptionId == other.L_sdOtolithDescriptionId &&
                (this.otolithDescription != null ? this.otolithDescription.Equals(other.otolithDescription) : other.otolithDescription == null) &&
                (dkDescription != null ? dkDescription.Equals(other.dkDescription) : other.dkDescription == null) &&
                (ukDescription != null ? ukDescription.Equals(other.ukDescription) : other.ukDescription == null);
        }

        public int CompareTo(object obj)
        {
            L_SDOtolithDescription other = obj as L_SDOtolithDescription;

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
            if (string.IsNullOrWhiteSpace(otolithDescription))
                return "Angiv venligst otolitbeskrivelse.";

            if (otolithDescription.Length > 15)
                return "Otolitbeskrivelse må kun bestå af 15 tegn.";

            if (dkDescription != null && dkDescription.Length > 500)
                return "Beskrivelse (DK) må kun bestå af 500 tegn.";

            if (ukDescription != null && ukDescription.Length > 500)
                return "Beskrivelse (UK) må kun bestå af 500 tegn.";

            if (lst.OfType<L_SDOtolithDescription>().Where(x => (x.L_sdOtolithDescriptionId != L_sdOtolithDescriptionId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.otolithDescription.Equals(otolithDescription, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("Otolitbeskrivelse '{0}' eksisterer allerede.", otolithDescription);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[1] { otolithDescription }; }
        }
    }
}
