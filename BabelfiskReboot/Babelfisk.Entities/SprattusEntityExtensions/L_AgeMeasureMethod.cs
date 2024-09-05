using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{ 
    public partial class L_AgeMeasureMethod : ILookupEntity, IComparable
    {
        /// <summary>
        /// samplingTypeId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.L_ageMeasureMethodId.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.ageMeasureMethod, this.description);
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
                string str = this.ageMeasureMethod;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            var other = obj as L_AgeMeasureMethod;

            if (other == null)
                return false;

            return this.L_ageMeasureMethodId == other.L_ageMeasureMethodId &&
                (this.ageMeasureMethod != null ? this.ageMeasureMethod.Equals(other.ageMeasureMethod, StringComparison.InvariantCultureIgnoreCase) : other.ageMeasureMethod == null) &&
                (description != null ? description.Equals(other.description, StringComparison.InvariantCultureIgnoreCase) : other.description == null);
        }


        public int CompareTo(object obj)
        {
            var other = obj as L_AgeMeasureMethod;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(ageMeasureMethod))
                return "Angiv venligst kode.";

            if (ageMeasureMethod.Length > 20)
                return "Koden må kun bestå af 20 tegn.";

            if (description != null && description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (lst.OfType<L_AgeMeasureMethod>().Where(x => (x.L_ageMeasureMethodId != L_ageMeasureMethodId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.ageMeasureMethod.Equals(ageMeasureMethod, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En række med kode '{0}' eksisterer allerede.", ageMeasureMethod);

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
            get { return new string[] { this.ageMeasureMethod }; }
        }


        public void NewLookupCreated()
        {
        }
    }
}
