using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_Species : OfflineEntity, ILookupEntity, IComparable
    {
        private static List<L_TreatmentFactorGroup> _lstTreatmentFactors;
        private static List<L_LengthMeasureType> _lstLengthMeasureTypes;

        private static Dictionary<int, L_LengthMeasureType> _dicLengthMeasureTypes = null;

        public static List<L_TreatmentFactorGroup> TreatmentFactors
        {
            get { return _lstTreatmentFactors; }
            set { _lstTreatmentFactors = value; }
        }

        public static List<L_LengthMeasureType> LengthMeasureTypes
        {
            get { return _lstLengthMeasureTypes; }
            set 
            { 
                _lstLengthMeasureTypes = value;
                if (value != null)
                    _dicLengthMeasureTypes = value.ToDictionary(x => x.L_lengthMeasureTypeId);

            }
        }

        /// <summary>
        /// speciesCode code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.speciesCode; }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}",   this.speciesCode, this.dkName, this.ukName, this.nodc, this.latin, 
                                                                                  this.L_TreatmentFactorGroup != null ? L_TreatmentFactorGroup.UIDisplay : "", 
                                                                                  this.dfuFisk_Code, this.tsn, this.aphiaID); }
        }

        public string treatmentFactorGroupUI
        {
            get { return L_TreatmentFactorGroup == null ? null : L_TreatmentFactorGroup.treatmentFactorGroup; }
            set
            {
                L_TreatmentFactorGroup = TreatmentFactors.Where(x => x.treatmentFactorGroup.Equals(value, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            }
        }

       
        public string L_StandardLengthMeasureDisplay
        {
            get
            {
                if (_dicLengthMeasureTypes == null || !standardLengthMeasureTypeId.HasValue || !_dicLengthMeasureTypes.ContainsKey(standardLengthMeasureTypeId.Value))
                    return "";

                return _dicLengthMeasureTypes[standardLengthMeasureTypeId.Value].UIDisplay;
            }
        }


        public string StandardLengthMeasureType
        {
            get { return L_StandardLengthMeasureType == null ? null : L_StandardLengthMeasureType.lengthMeasureType; }
        }


        public string DefaultSortValue
        {
            get { return speciesCode; }
        }


        public string UIDisplay
        {
            get 
            {
                string str = this.speciesCode;
                if (!string.IsNullOrWhiteSpace(dkName))
                    str += " - " + dkName;
                
                return str; 
            }
        }


        public override bool Equals(object obj)
        {
            L_Species other = obj as L_Species;

            if (other == null)
                return false;

            return this.L_speciesId == other.L_speciesId;
        }

        /*
        public override int GetHashCode()
        {
            return L_speciesId.GetHashCode() ^
                  (speciesCode == null ? 0 : speciesCode.GetHashCode()) ^
                  (dkName == null ? 0 : dkName.GetHashCode()) ^
                  (ukName == null ? 0 : ukName.GetHashCode()) ^
                  (nodc == null ? 0 : nodc.GetHashCode()) ^
                  (latin == null ? 0 : latin.GetHashCode()) ^
                  (icesCode == null ? 0 : icesCode.GetHashCode()) ^
                  (treatmentFactorGroup == null ? 0 : treatmentFactorGroup.GetHashCode()) ^
                  (dfuFisk_Code == null ? 0 : dfuFisk_Code.GetHashCode()) ^
                  (tsn == null ? 0 : tsn.GetHashCode()) ^
                  (aphiaID == null ? 0 : aphiaID.GetHashCode())
                  ;
        }
        */

        public int CompareTo(object obj)
        {
            L_Species other = obj as L_Species;

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
                return "Angiv venligst kode.";

            if (speciesCode.Length > 3)
                return "Koden må kun bestå af 3 tegn.";

            if (dkName != null && dkName.Length > 80)
                return "Danske navn må kun bestå af 80 tegn.";

            if (ukName != null && ukName.Length > 80)
                return "Engelske navn må kun bestå af 80 tegn.";

            if (nodc != null && nodc.Length > 80)
                return "NODC kode må kun bestå af 80 tegn.";

            if (latin != null && latin.Length > 80)
                return "Latinske navn må kun bestå af 80 tegn.";

            //if (icesCode != null && icesCode.Length > 3)
            //    return "ICES kode må kun bestå af 3 tegn.";

            if (dfuFisk_Code != null && dfuFisk_Code.Length > 3)
                return "Aqua-base kode må kun bestå af 3 tegn.";

            //if(speciesNES != null && speciesNES.Length > 3)
            //    return "NES kode må kun bestå af 3 tegn.";

            if (speciesFAO != null && speciesFAO.Length > 3)
                return "FAO kode må kun bestå af 3 tegn.";

            if (string.IsNullOrEmpty(treatmentFactorGroup))
                return "Alle nye/ændrede arter skal have en behandlingsfaktorgruppe.";

            if (tsn != null && tsn.Length > 6)
                return "Tsn må kun bestå af 6 tegn.";

            if (lengthMin.HasValue && lengthMin.Value < 0)
                return "'Min. længde (mm)' skal være større end eller lig med 0.";

            if (lengthMax.HasValue && lengthMax.Value < 0)
                return "'Max. længde (mm)' skal være større end eller lig med 0.";

            if (ageMin.HasValue && ageMin.Value < 0)
                return "'Min. alder' skal være større end eller lig med 0.";

            if (ageMax.HasValue && ageMax.Value < 0)
                return "'Max. alder' skal være større end eller lig med 0.";

            if (weightMin.HasValue && weightMin.Value < 0)
                return "'Min. vægt (UR) (g)' skal være større end eller lig med 0.";

            if (weightMax.HasValue && weightMax.Value < 0)
                return "'Max. væg (UR) (g)' skal være større end eller lig med 0.";

            if (lst.OfType<L_Species>().Where(x => (x.L_speciesId != L_speciesId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.speciesCode.Equals(speciesCode, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En art med kode '{0}' eksisterer allerede.", speciesCode);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return speciesCode; }
        }


        public void NewLookupCreated()
        {
        }
    }
}
