using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_MaturityIndexMethod : ILookupEntity, IComparable
    {
        /// <summary>
        /// lengthMeasureUnit code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.maturityIndexMethod; }
        }

        public string FilterValue
        {
            get { return String.Format("{0} {1}", this.maturityIndexMethod, this.description); }
        }

        public string DefaultSortValue
        {
            get { return FilterValue; }
        }


        public string UIDisplay
        {
            get { return String.Format("{0} - {1}", this.maturityIndexMethod, this.description); }
        }


        public override bool Equals(object obj)
        {
            L_MaturityIndexMethod other = obj as L_MaturityIndexMethod;

            if (other == null)
                return false;

            return this.L_maturityIndexMethodId == other.L_maturityIndexMethodId;
        }

        /*
        public override int GetHashCode()
        {
            return L_maturityIndexMethodId.GetHashCode() ^
                (maturityIndexMethod == null ? 0 : maturityIndexMethod.GetHashCode()) ^
                (description == null ? 0 : description.GetHashCode());
        }
        */

        public int CompareTo(object obj)
        {
            L_MaturityIndexMethod other = obj as L_MaturityIndexMethod;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(maturityIndexMethod))
                return "Angiv venligst kode.";

            if (maturityIndexMethod.Length > 1)
                return "Koden må kun bestå af 1 tegn.";

            if (description.Length > 80)
                return "Bemærkninger må kun bestå af 80 tegn.";

            if (string.IsNullOrWhiteSpace(description))
                return "Alle nye/ændrede modenhedsmetoder skal have en beskrivelse."; 

            if (lst.OfType<L_MaturityIndexMethod>().Where(x => (x.L_maturityIndexMethodId != L_maturityIndexMethodId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.maturityIndexMethod.Equals(maturityIndexMethod, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("Modenhedsmetode med kode '{0}' eksisterer allerede.", maturityIndexMethod);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return String.Format("{0}", this.maturityIndexMethod); }
        }

        public void NewLookupCreated()
        {
        }
    }
}
