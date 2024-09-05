using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_Application : ILookupEntity, IComparable
    {
        /// <summary>
        /// samplingTypeId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.L_applicationId.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.num, this.code, this.description);
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
                string str = this.code;

                if (num.HasValue)
                    str = num + " - " + str;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            var other = obj as L_Application;

            if (other == null)
                return false;

            return this.L_applicationId == other.L_applicationId &&
                (this.code != null ? this.code.Equals(other.code, StringComparison.InvariantCultureIgnoreCase) : other.code == null) &&
                (description != null ? description.Equals(other.description, StringComparison.InvariantCultureIgnoreCase) : other.description == null);
        }


        public int CompareTo(object obj)
        {
            L_Application other = obj as L_Application;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(code))
                return "Angiv venligst kode.";

            if (code.Length > 200)
                return "Koden må kun bestå af 200 tegn.";

            if (description != null && description.Length > 300)
                return "Beskrivelse må kun bestå af 300 tegn.";

            if (lst.OfType<L_Application>().Where(x => (x.L_applicationId != L_applicationId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.code.Equals(code, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En række med kode '{0}' eksisterer allerede.", code);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
      /*  public string CompareValue
        {
            get { return String.Format("{0} {1}", this.num, this.edgeStructure); }
        }*/

        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[2] { this.num.HasValue ? this.num.Value.ToString() : "", this.code }; }
        }


        public void NewLookupCreated()
        {
        }
    }
}
