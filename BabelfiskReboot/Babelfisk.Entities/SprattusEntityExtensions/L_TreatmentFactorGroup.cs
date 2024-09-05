using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Anchor.Core;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_TreatmentFactorGroup : ILookupEntity, IComparable
    {
        /// <summary>
        /// treatmentFactorGroup code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.treatmentFactorGroup; }
        }

        public string FilterValue
        {
            get { return String.Format("{0} {1}", this.treatmentFactorGroup, this.description); }
        }


        public string DefaultSortValue
        {
            get { return FilterValue; }
        }


        public string UIDisplay
        {
            get { return String.Format("{0} - {1}", this.treatmentFactorGroup, this.description); }
        }


        public override bool Equals(object obj)
        {
            L_TreatmentFactorGroup other = obj as L_TreatmentFactorGroup;

            if (other == null)
                return false;

            return this.L_treatmentFactorGroupId.Equals(other.L_treatmentFactorGroupId) &&
                    (this.treatmentFactorGroup != null ? this.treatmentFactorGroup.Equals(other.treatmentFactorGroup) : other.treatmentFactorGroup == null) &&
                    (this.description != null ? this.description.Equals(other.description) : other.description == null)
                    ;
        }

        /* Dont override get hash code, since the lookup values will change in a grid, and therefore their hashvalue also (resulting in disappearing keys/lookups)
        public override int GetHashCode()
        {
            return this.L_treatmentFactorGroupId.GetHashCode() ^
                    (this.treatmentFactorGroup == null ? 0 : this.treatmentFactorGroup.GetHashCode()) ^
                    (this.description == null ? 0 : this.description.GetHashCode());
        }
        */

        public int CompareTo(object obj)
        {
            L_TreatmentFactorGroup other = obj as L_TreatmentFactorGroup;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(treatmentFactorGroup))
                return "En eller flere behandlingsfaktorgrupper mangler en kode.";

            if (treatmentFactorGroup.Length > 3)
                return "Koden for Behandlingsfaktorgrupper må kun bestå af 3 tegn.";

            if (string.IsNullOrWhiteSpace(description))
                return "Angiv venligst en beskrivelse for alle behandlingsfaktorgrupper.";

            if (description.Length > 80)
                return "Beskrivelse for behandlingsfaktorgrupper må kun bestå af 80 tegn.";

            if (lst.OfType<L_TreatmentFactorGroup>().Where(x => (x.L_treatmentFactorGroupId != L_treatmentFactorGroupId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.treatmentFactorGroup.Equals(treatmentFactorGroup, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En behandlingsfaktor-gruppe med kode '{0}' eksisterer allerede.", treatmentFactorGroup);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return String.Format("{0}", this.treatmentFactorGroup); }
        }


        public void NewLookupCreated()
        {
        }
    }
}
