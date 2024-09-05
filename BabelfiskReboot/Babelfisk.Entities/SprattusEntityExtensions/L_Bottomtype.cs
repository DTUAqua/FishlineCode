using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_Bottomtype : ILookupEntity, IComparable
    {
        /// <summary>
        /// samplingTypeId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.bottomtype.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1}", this.bottomtype, this.description);
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
                string str = this.bottomtype.Trim();

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_Bottomtype other = obj as L_Bottomtype;

            if (other == null)
                return false;

            return this.L_bottomTypeId == other.L_bottomTypeId &&
                (this.bottomtype != null ? this.bottomtype.Equals(other.bottomtype, StringComparison.InvariantCultureIgnoreCase) : other.bottomtype == null) &&
                (description != null ? description.Equals(other.description, StringComparison.InvariantCultureIgnoreCase) : other.description == null);
        }


        public int CompareTo(object obj)
        {
            L_Bottomtype other = obj as L_Bottomtype;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(bottomtype))
                return "Angiv venligst kode.";

            if (bottomtype.Length > 3)
                return "Koden må kun bestå af 3 tegn.";

            if (description != null && description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (lst.OfType<L_Bottomtype>().Where(x => (x.L_bottomTypeId != L_bottomTypeId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.bottomtype.Equals(bottomtype, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En række med kode '{0}' eksisterer allerede.", bottomtype);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return String.Format("{0}", this.bottomtype); }
        }


        public void NewLookupCreated()
        {
        }
    }
}
