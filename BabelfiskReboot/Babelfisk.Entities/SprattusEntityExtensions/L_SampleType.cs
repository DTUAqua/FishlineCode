using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_SampleType : ILookupEntity, IComparable
    {
        /// <summary>
        /// speciesCode code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.sampleType; }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1}", this.sampleType, this.description);
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
                string str = this.sampleType;
                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_SampleType other = obj as L_SampleType;

            if (other == null)
                return false;

            return this.L_sampleTypeId == other.L_sampleTypeId;
        }

        /*
        public override int GetHashCode()
        {
            return L_sampleTypeId.GetHashCode() ^
                  (sampleType == null ? 0 : sampleType.GetHashCode()) ^
                  (description == null ? 0 : description.GetHashCode());
        }
        */

        public int CompareTo(object obj)
        {
            L_SampleType other = obj as L_SampleType;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(sampleType))
                return "Angiv venligst kode.";

            if (sampleType.Length > 1)
                return "Koden må kun bestå af 1 tegn.";

            if (string.IsNullOrWhiteSpace(description))
                return "Angiv venligst beskrivelse.";

            if (description != null && description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (lst.OfType<L_SampleType>().Where(x => (x.L_sampleTypeId != L_sampleTypeId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.sampleType.Equals(sampleType, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En redskabsgruppe med kode '{0}' eksisterer allerede.", sampleType);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return String.Format("{0}", this.sampleType); }
        }

        public void NewLookupCreated()
        {
        }
    }
}
