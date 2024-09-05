using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_SexCode : ILookupEntity, IComparable
    {
        /// <summary>
        /// speciesCode code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.sexCode.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.num, this.sexCode, this.description);
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

                str += this.sexCode;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_SexCode other = obj as L_SexCode;

            if (other == null)
                return false;

            return this.L_sexCodeId == other.L_sexCodeId &&
                (this.sexCode == null ? other.sexCode == null : this.sexCode.Equals(other.sexCode)) &&
                (this.description == null ? other.description == null : this.description.Equals(other.description))
                ;
        }


        public int CompareTo(object obj)
        {
            L_SexCode other = obj as L_SexCode;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(sexCode))
                return "Angiv venligst kode.";

            if (sexCode.Length > 1)
                return "Koden må kun bestå af 1 tegn.";

            if (string.IsNullOrWhiteSpace(description))
                return "Angiv venligst en beskrivelse.";

            if (description != null && description.Length > 15)
                return "Beskrivelse må kun bestå af 15 tegn.";

            if (lst.OfType<L_SexCode>().Where(x => (x.L_sexCodeId != L_sexCodeId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.sexCode.Equals(sexCode, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En række med kode '{0}' eksisterer allerede.", sexCode);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
       /* public string CompareValue
        {
            get { return String.Format("{0} {1}", this.num, this.sexCode); }
        }*/

        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[2] { this.num.HasValue ? this.num.Value.ToString() : "", this.sexCode }; }
        }


        public void NewLookupCreated()
        {
        }
    }
}
