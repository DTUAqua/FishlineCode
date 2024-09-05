using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_SDPreparationMethod : ILookupEntity, IComparable
    {
        public string Id
        {
            get { return this.L_sdPreparationMethodId.ToString(); }
        }


        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1}", this.preparationMethod, this.dkDescription ?? "");
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

                str += this.preparationMethod;

                if (!string.IsNullOrWhiteSpace(dkDescription))
                    str += " - " + dkDescription;

                return str;
            }
        }

        public override bool Equals(object obj)
        {
            L_SDPreparationMethod other = obj as L_SDPreparationMethod;

            if (other == null)
                return false;

            return this.L_sdPreparationMethodId == other.L_sdPreparationMethodId &&
                (this.preparationMethod != null ? this.preparationMethod.Equals(other.preparationMethod) : other.preparationMethod == null) &&
                (dkDescription != null ? dkDescription.Equals(other.dkDescription) : other.dkDescription == null) &&
                (ukDescription != null ? ukDescription.Equals(other.ukDescription) : other.ukDescription == null);
        }

        public int CompareTo(object obj)
        {
            L_SDPreparationMethod other = obj as L_SDPreparationMethod;

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
            if (string.IsNullOrWhiteSpace(preparationMethod))
                return "Angiv venligst forberedelsesmetode.";

            if (preparationMethod.Length > 10)
                return "Forberedelsesmetode må kun bestå af 10 tegn.";

            if (dkDescription != null && dkDescription.Length > 500)
                return "Beskrivelse (DK) må kun bestå af 500 tegn.";

            if (ukDescription != null && ukDescription.Length > 500)
                return "Beskrivelse (UK) må kun bestå af 500 tegn.";

            if (lst.OfType<L_SDPreparationMethod>().Where(x => (x.L_sdPreparationMethodId != L_sdPreparationMethodId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.preparationMethod.Equals(preparationMethod, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("Forberedelsesmetode '{0}' eksisterer allerede.", preparationMethod);

            return null;
        }

        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[1] { preparationMethod }; }
        }
    }
}
