using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_HaulType : ILookupEntity, IComparable
    {
        /// <summary>
        /// samplingTypeId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.haulType.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.num, this.haulType, this.description);
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
                string str = this.haulType;

                if (num.HasValue)
                    str = num + " - " + str;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_HaulType other = obj as L_HaulType;

            if (other == null)
                return false;

            return this.L_haulTypeId == other.L_haulTypeId &&
                (this.haulType != null ? this.haulType.Equals(other.haulType, StringComparison.InvariantCultureIgnoreCase) : other.haulType == null) &&
                (this.num.HasValue ? this.num.Value.Equals(other.num) : other.num.HasValue) &&
                (description != null ? description.Equals(other.description, StringComparison.InvariantCultureIgnoreCase) : other.description == null);
        }


        public int CompareTo(object obj)
        {
            L_HaulType other = obj as L_HaulType;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(haulType))
                return "Angiv venligst kode.";

            if (haulType.Length > 1)
                return "Koden må kun bestå af 1 tegn.";

            if (description != null && description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (lst.OfType<L_HaulType>().Where(x => (x.L_haulTypeId != L_haulTypeId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.haulType.Equals(haulType, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En række med kode '{0}' eksisterer allerede.", haulType);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return String.Format("{0} {1}", this.num, this.haulType); }
        }


        public void NewLookupCreated()
        {
        }
    }
}
