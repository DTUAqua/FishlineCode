using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_EdgeStructure : ILookupEntity, IComparable
    {
        /// <summary>
        /// samplingTypeId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.edgeStructure.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.num, this.edgeStructure, this.description);
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
                string str = this.edgeStructure;

                if (num.HasValue)
                    str = num + " - " + str;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_EdgeStructure other = obj as L_EdgeStructure;

            if (other == null)
                return false;

            return this.L_edgeStructureId == other.L_edgeStructureId &&
                (this.edgeStructure != null ? this.edgeStructure.Equals(other.edgeStructure, StringComparison.InvariantCultureIgnoreCase) : other.edgeStructure == null) &&
                (this.num.HasValue ? this.num.Value.Equals(other.num) : other.num == null) &&
                (description != null ? description.Equals(other.description, StringComparison.InvariantCultureIgnoreCase) : other.description == null);
        }


        public int CompareTo(object obj)
        {
            L_EdgeStructure other = obj as L_EdgeStructure;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(edgeStructure))
                return "Angiv venligst kode.";

            if (edgeStructure.Length > 5)
                return "Koden må kun bestå af 5 tegn.";

            if (description != null && description.Length > 250)
                return "Beskrivelse må kun bestå af 250 tegn.";

            if (lst.OfType<L_EdgeStructure>().Where(x => (x.L_edgeStructureId != L_edgeStructureId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.edgeStructure.Equals(edgeStructure, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En række med kode '{0}' eksisterer allerede.", edgeStructure);

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
            get { return new string[2] { this.num.HasValue ? this.num.Value.ToString() : "", this.edgeStructure }; }
        }


        public void NewLookupCreated()
        {
        }
    }
}
