using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_SDSampleType : ILookupEntity, IComparable
    {
        public string Id
        {
            get { return this.L_sdSampleTypeId.ToString(); }
        }


        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1}", this.sampleType, this.description ?? "");
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

                str += this.sampleType;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }

        public override bool Equals(object obj)
        {
            L_SDSampleType other = obj as L_SDSampleType;

            if (other == null)
                return false;

            return this.L_sdSampleTypeId == other.L_sdSampleTypeId &&
                (this.sampleType != null ? this.sampleType.Equals(other.sampleType) : other.sampleType == null) &&
                (description != null ? description.Equals(other.description) : other.description == null);
        }

        public int CompareTo(object obj)
        {
            L_SDSampleType other = obj as L_SDSampleType;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }
        public void NewLookupCreated()
        {
        }

        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(sampleType))
                return "Angiv venligst prøvetagningstype.";

            if (sampleType.Length > 100)
                return "Prøvetagningstype må kun bestå af 100 tegn.";

            if (description != null && description.Length > 500)
                return "Beskrivelse må kun bestå af 500 tegn.";

            if (lst.OfType<L_SDSampleType>().Where(x => (x.L_sdSampleTypeId != L_sdSampleTypeId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.sampleType.Equals(sampleType, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("Prøvetagningstypen '{0}' eksisterer allerede.", sampleType);

            return null;
        }

        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[2] { sampleType, this.description != null ? this.description : "" }; }
        }

    }
}
