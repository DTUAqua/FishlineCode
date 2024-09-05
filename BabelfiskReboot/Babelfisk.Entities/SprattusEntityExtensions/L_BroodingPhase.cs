using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_BroodingPhase : ILookupEntity, IComparable
    {
        /// <summary>
        /// speciesCode code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.broodingPhase.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", this.num, this.broodingPhase, this.description);
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

                str += this.broodingPhase;

                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            L_BroodingPhase other = obj as L_BroodingPhase;

            if (other == null)
                return false;

            return this.L_broodingPhaseId == other.L_broodingPhaseId &&
                (this.broodingPhase == null ? other.broodingPhase == null : this.broodingPhase.Equals(other.broodingPhase)) &&
                (this.description == null ? other.description == null : this.description.Equals(other.description))
                ;
        }


        public int CompareTo(object obj)
        {
            L_BroodingPhase other = obj as L_BroodingPhase;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(broodingPhase))
                return "Angiv venligst kode.";

            if (broodingPhase.Length > 4)
                return "Koden må kun bestå af 4 tegn.";

            if (string.IsNullOrWhiteSpace(description))
                return "Alle nye/ændrede rugefaser skal have en beskrivelse."; 

            if (description != null && description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (lst.OfType<L_BroodingPhase>().Where(x => (x.L_broodingPhaseId != L_broodingPhaseId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.broodingPhase.Equals(broodingPhase, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En række med kode '{0}' eksisterer allerede.", broodingPhase);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        /*public string CompareValue
        {
            get { return String.Format("{0} {1}", this.num, this.broodingPhase); }
        }*/

        /// <summary>
        /// IFilteredComboboxMultiItem interface property
        /// </summary>
        public string[] CompareValues
        {
            get { return new string[2] { this.num.HasValue ? this.num.Value.ToString() : "", this.broodingPhase }; }
        }


        public void NewLookupCreated()
        {
        }
    }
}
