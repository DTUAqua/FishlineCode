using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_Treatment : ILookupEntity, IComparable
    {
        /// <summary>
        /// treatment is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.treatment.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.num, this.treatment, this.description);
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

                str += this.treatment;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_Treatment other = obj as L_Treatment;

            if (other == null)
                return false;

            return  this.L_treatmentId == other.L_treatmentId &&
                    (this.treatment != null ? treatment.Equals(other.treatment) : other.treatment == null) &&
                    (description != null ? description.Equals(other.description) : other.description == null)
                    ;
        }

        /*
        public override int GetHashCode()
        {
            return L_treatmentId.GetHashCode() ^
                  (treatment == null ? 0 : treatment.GetHashCode()) ^
                  (description == null ? 0 : description.GetHashCode());
        }
        */

        public int CompareTo(object obj)
        {
            L_Treatment other = obj as L_Treatment;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(treatment))
                return "Angiv venligst kode.";

            if (treatment.Length > 2)
                return "Koden må kun bestå af 2 tegn.";

            if (string.IsNullOrWhiteSpace(description))
                return "Alle nye/ændrede behandlinger skal have en beskrivelse.";

            if (description != null && description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (lst.OfType<L_Treatment>().Where(x => (x.L_treatmentId != L_treatmentId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.treatment.Equals(treatment, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En behandling med kode '{0}' eksisterer allerede.", treatment);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
       /* public string CompareValue
        {
            get { return String.Format("{0} {1}", this.num, this.treatment); }
        }*/

        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[2] { this.num.HasValue ? this.num.Value.ToString() : "", this.treatment }; }
        }


        public void NewLookupCreated()
        {
        }
    }
}
