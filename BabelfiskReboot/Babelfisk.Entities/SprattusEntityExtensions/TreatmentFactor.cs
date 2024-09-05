using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class TreatmentFactor : ILookupEntity, IComparable
    {
        private static List<L_TreatmentFactorGroup> _lstTreatmentFactorGroups;
        private static List<L_Treatment> _lstTreatments;

        private static Dictionary<string, L_TreatmentFactorGroup> _dicTreatmentFactorGroups = null;
        private static Dictionary<string, L_Treatment> _dicTreatments = null;


        public static List<L_TreatmentFactorGroup> TreatmentFactorGroups
        {
            get { return _lstTreatmentFactorGroups; }
            set
            {
                _lstTreatmentFactorGroups = value;
                if (value != null)
                    _dicTreatmentFactorGroups = value.ToDictionary(x => x.treatmentFactorGroup);
            }
        }


        public static List<L_Treatment> Treatments
        {
            get { return _lstTreatments; }
            set
            {
                _lstTreatments = value;
                if (value != null)
                    _dicTreatments = value.ToDictionary(x => x.treatment);
            }
        }

        public string L_TreatmentFactorGroupUIDisplay
        {
            get
            {
                if (treatmentFactorGroup == null || _dicTreatmentFactorGroups == null || !_dicTreatmentFactorGroups.ContainsKey(treatmentFactorGroup))
                    return "";

                return _dicTreatmentFactorGroups[treatmentFactorGroup].UIDisplay;
            }
        }


        public string L_TreatmentUIDisplay
        {
            get
            {
                if (treatment == null || _dicTreatments == null || !_dicTreatments.ContainsKey(treatment))
                    return "";

                return _dicTreatments[treatment].UIDisplay;
            }
        }


        /// <summary>
        /// treatmentFactorId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.treatmentFactorId.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2} {3} {4}", L_TreatmentFactorGroupUIDisplay, L_TreatmentUIDisplay, factor, description, versioningDate.ToString("dd-MM-yyyy HH:mm:ss"));
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
                string str = this.factor.ToString();

                if (!string.IsNullOrWhiteSpace(treatment))
                    str += " - " + treatment;

                if(!string.IsNullOrEmpty(treatmentFactorGroup))
                    str += " - " + treatmentFactorGroup;

                return str;
            }
        }


        public override bool Equals(object obj)
        {
            TreatmentFactor other = obj as TreatmentFactor;

            if (other == null)
                return false;

            return  (treatmentFactorId.Equals(other.treatmentFactorId)) &&
                    (treatmentFactorGroup != null ? treatmentFactorGroup.Equals(other.treatmentFactorGroup) : other.treatmentFactorGroup == null) &&
                    (treatment != null ? treatment.Equals(other.treatment) : other.treatment == null) &&
                    (factor.Equals(other.factor)) &&
                    (description != null ? description.Equals(other.description) : other.description == null) &&
                    versioningDate.Equals(other.versioningDate)
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
            TreatmentFactor other = obj as TreatmentFactor;

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
                return "Angiv venligst behandlingsgruppe for alle behandlingsfaktorer.";

            if (treatmentFactorGroup.Length > 3)
                return "Gruppe må kun bestå af 3 tegn for behandlingsfaktorer.";

            if (string.IsNullOrWhiteSpace(treatment))
                return "Angiv venligst behandling for alle behandlingsfaktorer.";

            if (treatment.Length > 2)
                return "Behandling for alle behandlingsfaktorer må kun bestå af 2 tegn.";

            if (description != null && description.Length > 80)
                return "Beskrivelse for alle behandlingsfaktorer må kun bestå af 80 tegn.";

            if (lst.OfType<TreatmentFactor>().Where(x => (x.treatmentFactorId != treatmentFactorId || (this.ChangeTracker.State == ObjectState.Added && x != this)) &&
                                                          x.treatmentFactorGroup.Equals(treatmentFactorGroup, StringComparison.InvariantCultureIgnoreCase) &&
                                                          x.treatment.Equals(treatment, StringComparison.InvariantCultureIgnoreCase) &&
                                                          x.versioningDate.Equals(versioningDate)
                                                          ).Count() > 0)
                return string.Format("En behandlingsfaktor med behandlingsgruppe '{0}', behandling '{1}' og versionerings-dato '{2}' eksisterer allerede.", treatmentFactorGroup, treatment, versioningDate.ToString("dd-MM-yyyy HH:mm:ss"));

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return UIDisplay; }
        }


        public void NewLookupCreated()
        {
            factor = 1.000m;
            versioningDate = DateTime.UtcNow;
        }
    }
}
