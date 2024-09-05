using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_SamplingMethod : ILookupEntity, IComparable
    {
        /// <summary>
        /// speciesCode code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.samplingMethodId.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.num, this.samplingMethod, this.description);
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

                str += this.samplingMethod;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_SamplingMethod other = obj as L_SamplingMethod;

            if (other == null)
                return false;

            return this.samplingMethodId == other.samplingMethodId &&
                (this.samplingMethod == null ? other.samplingMethod == null : this.samplingMethod.Equals(other.samplingMethod)) &&
                (this.description == null ? other.description == null : this.description.Equals(other.description))
                ;
        }

        /*
        public override int GetHashCode()
        {
            return samplingMethodId.GetHashCode() ^
                  (samplingMethod == null ? 0 : samplingMethod.GetHashCode()) ^
                  (description == null ? 0 : description.GetHashCode());
        }
        */

        public int CompareTo(object obj)
        {
            L_SamplingMethod other = obj as L_SamplingMethod;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(samplingMethod))
                return "Angiv venligst kode.";

            if (samplingMethod.Length > 12)
                return "Koden må kun bestå af 12 tegn.";

            if (description != null && description.Length > 50)
                return "Beskrivelse må kun bestå af 50 tegn.";

            if (lst.OfType<L_SamplingMethod>().Where(x => (x.samplingMethodId != samplingMethodId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.samplingMethod.Equals(samplingMethod, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En indsamlingsmetode med kode '{0}' eksisterer allerede.", samplingMethod);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[2] { this.num.HasValue ? this.num.Value.ToString() : "", this.samplingMethod };/* String.Format("{0} {1}", this.num, this.samplingMethod);*/ }
        }


        public void NewLookupCreated()
        {
        }
    }
}
