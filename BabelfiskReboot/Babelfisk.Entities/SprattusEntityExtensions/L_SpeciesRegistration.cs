using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_SpeciesRegistration : ILookupEntity, IComparable
    {
        /// <summary>
        /// samplingTypeId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.speciesRegistrationId.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.num, this.speciesRegistration, this.description);
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

                str += this.speciesRegistration;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_SpeciesRegistration other = obj as L_SpeciesRegistration;

            if (other == null)
                return false;

            return this.speciesRegistrationId == other.speciesRegistrationId &&
                (this.speciesRegistration != null ? this.speciesRegistration.Equals(other.speciesRegistration, StringComparison.InvariantCultureIgnoreCase) : other.speciesRegistration == null) &&
                (description != null ? description.Equals(other.description, StringComparison.InvariantCultureIgnoreCase) : other.description == null);
        }


        public int CompareTo(object obj)
        {
            L_SpeciesRegistration other = obj as L_SpeciesRegistration;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(speciesRegistration))
                return "Angiv venligst kode.";

            if (speciesRegistration.Length > 3)
                return "Koden må kun bestå af 3 tegn.";

            if (description != null && description.Length > 50)
                return "Beskrivelse må kun bestå af 50 tegn.";

            if (lst.OfType<L_SpeciesRegistration>().Where(x => (x.speciesRegistrationId != speciesRegistrationId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.speciesRegistration.Equals(speciesRegistration, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En række med kode '{0}' eksisterer allerede.", speciesRegistration);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
       /*public string CompareValue
        {
            get { return String.Format("{0} {1}", this.num, this.speciesRegistration); }
        }*/
        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[2] { this.num.HasValue ? this.num.Value.ToString() : "", this.speciesRegistration }; }
        }


        public void NewLookupCreated()
        {
        }
    }
}
