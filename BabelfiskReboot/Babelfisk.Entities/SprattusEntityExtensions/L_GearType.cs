using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_GearType : OfflineEntity, ILookupEntity, IComparable
    {
        private static List<L_SampleType> _lstSampleTypes;

        public static List<L_SampleType> SampleTypes
        {
            get { return _lstSampleTypes.ToList(); }
            set { _lstSampleTypes = value; }
        }

        /// <summary>
        /// speciesCode code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.gearType; }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2} {3} {4} {5} {6}", this.gearType, this.description, this.fmCode, this.logbookCode, 
                                                            this.L_SampleType != null ? L_SampleType.UIDisplay : "", //CatchOperation
                                                             this.showInSeaHvnUI, this.showInVidUI );
            }
        }


        public string sampleType
        {
            get { return L_SampleType == null ? null : L_SampleType.sampleType; }
            set
            {
                L_SampleType = SampleTypes.Where(x => x.sampleType.Equals(value, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
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
                string str = this.gearType;
                if (!string.IsNullOrWhiteSpace(description))
                    str += " - " + description;

                return str;
            }
        }

        
        public override bool Equals(object obj)
        {
            L_GearType other = obj as L_GearType;

            if (other == null)
                return false;

            return this.L_gearTypeId == other.L_gearTypeId;
        }
        
        /*
        public override int GetHashCode()
        {
            return L_gearTypeId.GetHashCode() ^
                  (gearType == null ? 0 : gearType.GetHashCode()) ^
                  (description == null ? 0 : description.GetHashCode()) ^
                  (fmCode == null ? 0 : fmCode.GetHashCode()) ^
                  (logbookCode == null ? 0 : logbookCode.GetHashCode())
                  ;
        }
        */

        public int CompareTo(object obj)
        {
            L_GearType other = obj as L_GearType;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(gearType))
                return "Angiv venligst kode.";

            if (gearType.Length > 50)
                return "Koden må kun bestå af 50 tegn.";

            if (string.IsNullOrWhiteSpace(description))
                return "Angiv venligst bemærkninger.";

            if (description != null && description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (logbookCode != null && logbookCode.Length > 50)
                return "Logbogskode må kun bestå af 50 tegn.";

            if (string.IsNullOrWhiteSpace(sampleType))
                return "Alle nye/ændrede redskabstyper skal have en redskabsgruppe.";

            if (lst.OfType<L_GearType>().Where(x => (x.L_gearTypeId != L_gearTypeId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.gearType.Equals(gearType, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("En redskabstype med kode '{0}' eksisterer allerede.", gearType);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return gearType; }
        }

        public void NewLookupCreated()
        {
            showInVidUI = true;
            showInSeaHvnUI = true;
        }
    }
}
