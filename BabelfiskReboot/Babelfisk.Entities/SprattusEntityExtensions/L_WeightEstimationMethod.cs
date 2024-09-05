using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_WeightEstimationMethod : ILookupEntity, IComparable
    {
        /// <summary>
        /// weightEstimationMethod code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.weightEstimationMethod.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.num, this.weightEstimationMethod, this.description);
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
                string str = this.weightEstimationMethod;

                if (num.HasValue)
                    str = num + " - " + str;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_WeightEstimationMethod other = obj as L_WeightEstimationMethod;

            if (other == null)
                return false;

            return this.L_weightEstimationMethodId == other.L_weightEstimationMethodId &&
                (this.weightEstimationMethod != null ? this.weightEstimationMethod.Equals(other.weightEstimationMethod, StringComparison.InvariantCultureIgnoreCase) : other.weightEstimationMethod == null) &&
                (this.num.HasValue ? this.num.Value.Equals(other.num) : other.num.HasValue) &&
                (description != null ? description.Equals(other.description, StringComparison.InvariantCultureIgnoreCase) : other.description == null);
        }


        public int CompareTo(object obj)
        {
            L_WeightEstimationMethod other = obj as L_WeightEstimationMethod;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(weightEstimationMethod))
                return "Angiv venligst kode.";

            if (weightEstimationMethod.Length > 1)
                return "Koden må kun bestå af 1 tegn.";

            if (description != null && description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (lst.OfType<L_WeightEstimationMethod>().Where(x => (x.L_weightEstimationMethodId != L_weightEstimationMethodId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.weightEstimationMethod.Equals(weightEstimationMethod, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En række med kode '{0}' eksisterer allerede.", weightEstimationMethod);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
      /*  public string CompareValue
        {
            get { return String.Format("{0} {1}", this.num, this.weightEstimationMethod); }
        }*/

        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[2] { this.num.HasValue ? this.num.Value.ToString() : "", this.weightEstimationMethod }; }
        }


        public void NewLookupCreated()
        {
        }
    }
}
