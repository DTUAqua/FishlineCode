using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_Harbour : ILookupEntity, IComparable
    {
        private static List<L_Nationality> _lstNationalities;

        public static List<L_Nationality> Nationalities
        {
            get { return _lstNationalities; }
            set { _lstNationalities = value; }
        }

        /// <summary>
        /// harbour code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.harbour; }
        }


        /// <summary>
        /// Used when filtering the records
        /// </summary>
        public string FilterValue
        {
            get { return String.Format("{0} {1} {2} {3} {4}", this.harbour, this.L_Nationality != null ? L_Nationality.CodeDescription : "", this.description, this.harbourNES, this.harbourEU); }
        }


        public string DefaultSortValue
        {
            get { return FilterValue; }
        }

        public string nationalityUI
        {
            get { return this.L_Nationality == null ? null : L_Nationality.nationality; }
            set
            {
                L_Nationality = Nationalities.Where(x => x.nationality.Equals(value, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            }
        }


        /// <summary>
        /// Value displayed in comboboxes
        /// </summary>
        public string UIDisplay
        {
            get 
            {
                string str = this.harbour;

                if (!string.IsNullOrWhiteSpace(this.description))
                    str += " - " + this.description;

                if (!string.IsNullOrWhiteSpace(this.nationality))
                    str = nationality + " - " + str;
                
                return str;
            }
        }

        
        public override bool Equals(object obj)
        {
            L_Harbour other = obj as L_Harbour;

            if (other == null)
                return false;

            var blnEquals = this.L_harbourId.Equals(other.L_harbourId) &&
                            (harbour == null ? other.harbour == null : harbour.Equals(other.harbour)) &&
                            (L_Nationality == null ? other.L_Nationality == null : L_Nationality.Equals(other.L_Nationality)) &&
                            (description == null ? other.description == null : description.Equals(other.description)) &&
                            (harbourNES == null ? other.harbourNES == null : harbourNES.Equals(other.harbourNES)) &&
                            (harbourEU == null ? other.harbourEU == null : harbourEU.Equals(other.harbourEU))
                            ;

            return blnEquals;
        }
        
        /*
        public override int GetHashCode()
        {
            return L_harbourId.GetHashCode() ^
                   (harbour == null ? 0 : harbour.GetHashCode()) ^
                   (description == null ? 0 : description.GetHashCode()) ^
                   (harbourNES == null ? 0 : harbourNES.GetHashCode()) ^
                   (harbourEU == null ? 0 : harbourEU.GetHashCode()) 
                   ;
        }
        */

        public int CompareTo(object obj)
        {
            L_Harbour other = obj as L_Harbour;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }


        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(harbour))
                return "Angiv venligst kode.";

            if (harbour != null && harbour.Length > 4)
                return "Havnekode må kun bestå af 4 tegn.";

            if (description != null && description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (harbourNES != null && harbourNES.Length > 8)
                return "NES kode må kun bestå af 8 tegn.";

            if (harbourEU != null && harbourEU.Length > 8)
                return "EU kode må kun bestå af 8 tegn.";

            if (string.IsNullOrWhiteSpace(nationality))
                return "Alle nye/ændrede havne skal have en nationalitet."; 

            //Make sure duplicate lookups can not be added.
            if (lst.OfType<L_Harbour>().Where(x => (x.L_harbourId != L_harbourId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.harbour.Equals(harbour, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En havn med kode '{0}' eksisterer allerede.", harbour);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return this.harbour; }
        }


        public void NewLookupCreated()
        {
        }
    }
}
