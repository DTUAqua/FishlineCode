using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class Maturity : ILookupEntity, IComparable
    {
        private static List<L_MaturityIndexMethod> _lstMaturityIndexMethods;

        private static Dictionary<string, L_MaturityIndexMethod> _dicMaturityIndexMethods = null;


        public static List<L_MaturityIndexMethod> MaturityIndexMethods
        {
            get { return _lstMaturityIndexMethods; }
            set
            {
                _lstMaturityIndexMethods = value;
                if (value != null)
                    _dicMaturityIndexMethods = value.ToDictionary(x => x.maturityIndexMethod);
            }
        }


        public string L_MaturityIndexMethodUIDisplay
        {
            get
            {
                if (maturityIndexMethod == null || _dicMaturityIndexMethods == null || !_dicMaturityIndexMethods.ContainsKey(maturityIndexMethod))
                    return "";

                return _dicMaturityIndexMethods[maturityIndexMethod].UIDisplay;
            }
        }


        /// <summary>
        /// treatmentFactorId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.maturityId.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", L_MaturityIndexMethodUIDisplay, maturityIndex, description);
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
                string str = this.maturityIndex.ToString();

                if (!string.IsNullOrWhiteSpace(maturityIndexMethod))
                    str += " - " + maturityIndexMethod;

                return str;
            }
        }

        public string FullUIDisplay
        {
            get
            {
                string str = this.maturityIndex.ToString();

                if (!string.IsNullOrWhiteSpace(maturityIndexMethod))
                    str += " - " + maturityIndexMethod;

                if(!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            Maturity other = obj as Maturity;

            if (other == null)
                return false;

            return (maturityId.Equals(other.maturityId)) &&
                    (maturityIndexMethod != null ? maturityIndexMethod.Equals(other.maturityIndexMethod) : other.maturityIndexMethod == null) &&
                    (maturityIndex.Equals(other.maturityIndex)) &&
                    (description != null ? description.Equals(other.description) : other.description == null)
                    ;
        }

        /*
        public override int GetHashCode()
        {
            return treatmentFactorId.GetHashCode() ^
                  (treatmentFactorGroup == null ? 0 : treatmentFactorGroup.GetHashCode()) ^
                  (treatment == null ? 0 : treatment.GetHashCode()) ^
                  factor.GetHashCode() ^
                  (description == null ? 0 : description.GetHashCode())
                  ;
        }
        */

        public int CompareTo(object obj)
        {
            Maturity other = obj as Maturity;

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
                return "Angiv venligst modenhedsmetode for alle modenhedsrækker.";

            if (maturityIndexMethod.Length > 1)
                return "Modenhedsmetode må kun bestå af 1 tegn for modenhedsrækker.";

            if (description != null && description.Length > 80)
                return "Beskrivelse for alle modenhedsrækker må kun bestå af 80 tegn.";

            if (lst.OfType<Maturity>().Where(x => (x.maturityId != maturityId || (this.ChangeTracker.State == ObjectState.Added && x != this)) &&
                                                          x.maturityIndex.Equals(maturityIndex) &&
                                                          x.maturityIndexMethod.Equals(maturityIndexMethod, StringComparison.InvariantCultureIgnoreCase)
                                                          ).Count() > 0)
                return string.Format("En modenhed med modenhedsmetode '{0}' og modenhedsindeks '{1}' eksisterer allerede.", maturityIndexMethod, maturityIndex);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        /*public string CompareValue
        {
            get { return String.Format("{0} {1}", L_MaturityIndexMethodUIDisplay, maturityIndex); }
        }*/

        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[2] { maturityIndex.ToString(), L_MaturityIndexMethodUIDisplay  }; }
        }


        public void NewLookupCreated()
        {
            maturityIndex = 1;
        }
    }
}
