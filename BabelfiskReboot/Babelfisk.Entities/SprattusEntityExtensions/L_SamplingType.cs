using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_SamplingType : ILookupEntity, IComparable
    {
        /// <summary>
        /// samplingTypeId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.samplingTypeId.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.num, this.samplingType, this.description);
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

                str += this.samplingType;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_SamplingType other = obj as L_SamplingType;

            if (other == null)
                return false;

            return this.samplingTypeId == other.samplingTypeId &&
                (this.samplingType != null ? this.samplingType.Equals(other.samplingType) : other.samplingType == null) &&
                (description != null ? description.Equals(other.description) : other.description == null);
        }

        /*
        public override int GetHashCode()
        {
            return samplingTypeId.GetHashCode() ^
                  (samplingType == null ? 0 : samplingType.GetHashCode()) ^
                  (description == null ? 0 : description.GetHashCode());
        }
        */

        public int CompareTo(object obj)
        {
            L_SamplingType other = obj as L_SamplingType;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(samplingType))
                return "Angiv venligst kode.";

            if (samplingType.Length > 1)
                return "Koden må kun bestå af 1 tegn.";

            if (description != null && description.Length > 50)
                return "Beskrivelse må kun bestå af 50 tegn.";

            if (lst.OfType<L_SamplingType>().Where(x => (x.samplingTypeId != samplingTypeId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.samplingType.Equals(samplingType, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En prøvetype med kode '{0}' eksisterer allerede.", samplingType);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
       /* public string CompareValue
        {
            get { return String.Format("{0} {1}", this.num, this.samplingType); }
        }*/

        public string[] CompareValues
        {
            get { return new string[2] { this.num.HasValue ? this.num.Value.ToString() : "", this.samplingType };/* String.Format("{0} {1}", this.num, this.samplingMethod);*/ }
        }


        public void NewLookupCreated()
        {
        }
    }
}
