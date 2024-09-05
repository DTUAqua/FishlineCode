using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
     [Serializable]
    public partial class L_Nationality : ILookupEntity, IComparable
    {
        /// <summary>
        /// nationality code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.nationality; }
        }

        public string FilterValue
        {
            get { return String.Format("{0} {1}", this.nationality, this.description); }
        }


        public string DefaultSortValue
        {
            get { return FilterValue; }
        }

        public string CodeDescription
        {
            get { return String.Format("{0} - {1}", this.nationality, this.description); }
        }

        public string UIDisplay
        {
            get { return CodeDescription; }
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return String.Format("{0}", this.nationality); }
        }

        public override bool Equals(object obj)
        {
            L_Nationality other = obj as L_Nationality;

            if (other == null)
                return false;

            return this.L_nationalityId == other.L_nationalityId;
        }

         /*
        public override int GetHashCode()
        {
            return L_nationalityId.GetHashCode() ^
                   (nationality == null ? 0 : nationality.GetHashCode()) ^
                   (description == null ? 0 : description.GetHashCode());
        }
         */

        public int CompareTo(object obj)
        {
            L_Nationality other = obj as L_Nationality;

            if (other == null)
                return -1;

            return this.CodeDescription.CompareTo(other.CodeDescription);
        }

        public void BeforeSave()
        {
        }

        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(nationality))
                return "Angiv venligst nationalitets-kode.";

            if (nationality != null && nationality.Length > 3)
                return "Nationalitets-kode må kun bestå af 3 tegn.";

            if (description != null && description.Length > 15)
                return "Beskrivelse må kun bestå af 15 tegn.";

            if (string.IsNullOrWhiteSpace(description))
                return "Alle nye/ændrede nationaliteter skal have en beskrivelse."; 

            if (lst.OfType<L_Nationality>().Where(x => (x.L_nationalityId != L_nationalityId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.nationality.Equals(nationality, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En nationalitet med kode '{0}' eksisterer allerede.", nationality);

            return null;
        }

        public void NewLookupCreated()
        {
        }
    }
}
