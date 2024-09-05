using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_LengthMeasureUnit : ILookupEntity, IComparable
    {
        /// <summary>
        /// lengthMeasureUnit code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.lengthMeasureUnit; }
        }

        public string FilterValue
        {
            get { return String.Format("{0} {1}", this.lengthMeasureUnit, this.description); }
        }


        public string DefaultSortValue
        {
            get { return FilterValue; }
        }


        public string UIDisplay
        {
            get { return String.Format("{0} - {1}", this.lengthMeasureUnit, this.description); }
        }


        public override bool Equals(object obj)
        {
            L_LengthMeasureUnit other = obj as L_LengthMeasureUnit;

            if (other == null)
                return false;

            return this.L_lengthMeasureUnitId == other.L_lengthMeasureUnitId;
        }

        /*
        public override int GetHashCode()
        {
            return L_lengthMeasureUnitId.GetHashCode() ^
                (lengthMeasureUnit == null ? 0 : lengthMeasureUnit.GetHashCode()) ^
                (description == null ? 0 : description.GetHashCode());
        }
        */

        public int CompareTo(object obj)
        {
            L_LengthMeasureUnit other = obj as L_LengthMeasureUnit;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(lengthMeasureUnit))
                return "Angiv venligst kode.";

            if (lengthMeasureUnit.Length > 2)
                return "Koden må kun bestå af 2 tegn.";

            if (description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (string.IsNullOrWhiteSpace(description))
                return "Alle nye/ændrede længdeenheder skal have en beskrivelse."; 

            if (lst.OfType<L_LengthMeasureUnit>().Where(x => (x.L_lengthMeasureUnitId != L_lengthMeasureUnitId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.lengthMeasureUnit.Equals(lengthMeasureUnit, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En længdeenhed med kode '{0}' eksisterer allerede.", lengthMeasureUnit);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return String.Format("{0}", this.lengthMeasureUnit); }
        }

        public void NewLookupCreated()
        {
        }
    }
}
