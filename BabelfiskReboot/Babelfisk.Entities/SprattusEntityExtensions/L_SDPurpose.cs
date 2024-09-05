using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_SDPurpose : ILookupEntity, IComparable
    {
       
        public string Id
        {
            get { return this.L_sdPurposeId.ToString(); }
        }


        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1}", this.purpose, this.description ?? "");
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

                str += this.purpose;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }

        public override bool Equals(object obj)
        {
            L_SDPurpose other = obj as L_SDPurpose;

            if (other == null)
                return false;

            return this.L_sdPurposeId == other.L_sdPurposeId &&
                (this.purpose != null ? this.purpose.Equals(other.purpose) : other.purpose == null) &&
                (description != null ? description.Equals(other.description) : other.description == null);
        }

        public int CompareTo(object obj)
        {
            L_SDPurpose other = obj as L_SDPurpose;

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
            if (string.IsNullOrWhiteSpace(purpose))
                return "Angiv venligst formål.";

            if (purpose.Length > 100)
                return "Koden må kun bestå af 100 tegn.";

            if (description != null && description.Length > 500)
                return "Beskrivelse må kun bestå af 500 tegn.";

            if (lst.OfType<L_SDPurpose>().Where(x => (x.L_sdPurposeId != L_sdPurposeId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.purpose.Equals(purpose, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("Formålet '{0}' eksisterer allerede.", purpose);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[2] { purpose, this.description != null ? this.description : "" }; }
        }
    }
}
