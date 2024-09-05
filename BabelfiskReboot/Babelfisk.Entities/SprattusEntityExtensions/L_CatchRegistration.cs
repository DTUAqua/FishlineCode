using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_CatchRegistration : ILookupEntity, IComparable
    {
        /// <summary>
        /// samplingTypeId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.catchRegistrationId.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.num, this.catchRegistration, this.description);
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

                str += this.catchRegistration;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_CatchRegistration other = obj as L_CatchRegistration;

            if (other == null)
                return false;

            return this.catchRegistrationId == other.catchRegistrationId &&
                (this.catchRegistration != null ? this.catchRegistration.Equals(other.catchRegistration, StringComparison.InvariantCultureIgnoreCase) : other.catchRegistration == null) &&
                (description != null ? description.Equals(other.description, StringComparison.InvariantCultureIgnoreCase) : other.description == null);
        }


        public int CompareTo(object obj)
        {
            L_CatchRegistration other = obj as L_CatchRegistration;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(catchRegistration))
                return "Angiv venligst kode.";

            if (catchRegistration.Length > 3)
                return "Koden må kun bestå af 3 tegn.";

            if (description != null && description.Length > 50)
                return "Beskrivelse må kun bestå af 50 tegn.";

            if (lst.OfType<L_CatchRegistration>().Where(x => (x.catchRegistrationId != catchRegistrationId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.catchRegistration.Equals(catchRegistration, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En række med kode '{0}' eksisterer allerede.", catchRegistration);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
       /* public string CompareValue
        {
            get { return String.Format("{0} {1}", this.num, this.catchRegistration); }
        }*/
        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[2] { this.num.HasValue ? this.num.Value.ToString() : "", this.catchRegistration }; }
        }


        public void NewLookupCreated()
        {
        }
    }
}
