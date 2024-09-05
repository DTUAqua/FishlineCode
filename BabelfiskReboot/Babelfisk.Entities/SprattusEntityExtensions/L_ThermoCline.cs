using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_ThermoCline : ILookupEntity, IComparable
    {
        /// <summary>
        /// samplingTypeId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.thermoCline.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.num, this.thermoCline, this.description);
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

                str += this.thermoCline;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_ThermoCline other = obj as L_ThermoCline;

            if (other == null)
                return false;

            return this.L_thermoClineId == other.L_thermoClineId &&
                (this.thermoCline != null ? this.thermoCline.Equals(other.thermoCline, StringComparison.InvariantCultureIgnoreCase) : other.thermoCline == null) &&
                (this.num.HasValue ? this.num.Value.Equals(other.num) : other.num == null) &&
                (description != null ? description.Equals(other.description, StringComparison.InvariantCultureIgnoreCase) : other.description == null);
        }


        public int CompareTo(object obj)
        {
            L_ThermoCline other = obj as L_ThermoCline;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(thermoCline))
                return "Angiv venligst kode.";

            if (thermoCline.Length > 10)
                return "Koden må kun bestå af 10 tegn.";

            if (description != null && description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (lst.OfType<L_ThermoCline>().Where(x => (x.L_thermoClineId != L_thermoClineId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.thermoCline.Equals(thermoCline, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En række med kode '{0}' eksisterer allerede.", thermoCline);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
       /* public string CompareValue
        {
            get { return String.Format("{0} {1}", this.num, this.thermoCline); }
        }*/

        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[2] { this.num.HasValue ? this.num.Value.ToString() : "", this.thermoCline }; }
        }


        public void NewLookupCreated()
        {
        }
    }
}
