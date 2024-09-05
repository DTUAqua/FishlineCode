using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_VisualStock : ILookupEntity, IComparable
    {
        private static List<L_Species> _lstSpecies;

        private static Dictionary<string, L_Species> _dicSpecies = null;


        public static List<L_Species> Species
        {
            get { return _lstSpecies; }
            set
            {
                _lstSpecies = value;
                if (value != null)
                    _dicSpecies = value.ToDictionary(x => x.speciesCode);
            }
        }


        public string L_SpeciesUIDisplay
        {
            get
            {
                if (speciesCode == null || _dicSpecies == null || !_dicSpecies.ContainsKey(speciesCode))
                    return "";

                return _dicSpecies[speciesCode].UIDisplay;
            }
        }


        /// <summary>
        /// treatmentFactorId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.L_visualStockId.ToString(); }
        }


        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2} {3}", num.HasValue ? num.Value.ToString() : "", L_SpeciesUIDisplay, visualStock ?? "", description);
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

                str += this.visualStock ?? "";

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            var other = obj as L_VisualStock;

            if (other == null)
                return false;

            return (L_visualStockId.Equals(other.L_visualStockId)) &&
                    (speciesCode != null ? speciesCode.Equals(other.speciesCode) : other.speciesCode == null) &&
                    (visualStock != null ? visualStock.Equals(other.visualStock) : other.visualStock == null) &&
                    (this.num.HasValue ? this.num.Value.Equals(other.num) : other.num == null) &&
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
            L_VisualStock other = obj as L_VisualStock;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }


        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(speciesCode))
                return "Angiv venligst en art for alle rækker.";

            if (visualStock.Length > 50)
                return "Visuel bestandskode må kun bestå af 50 tegn.";

            if (description != null && description.Length > 100)
                return "Beskrivelse for alle rækker må kun bestå af 100 tegn.";

            if (lst.OfType<L_VisualStock>().Where(x => (x.L_visualStockId != L_visualStockId || (this.ChangeTracker.State == ObjectState.Added && x != this)) &&
                                                          x.visualStock.Equals(visualStock) &&
                                                          x.speciesCode.Equals(speciesCode, StringComparison.InvariantCultureIgnoreCase)
                                                          ).Count() > 0)
                return string.Format("En visuel bestand med art '{0}' og visuel bestandskode '{1}' eksisterer allerede.", speciesCode, visualStock);

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
            get { return new string[] { this.num.HasValue ? this.num.Value.ToString() : "", visualStock.ToString(), L_SpeciesUIDisplay }; }
        }


        public void NewLookupCreated()
        {
           // maturityIndex = 1;
        }
    }
}
