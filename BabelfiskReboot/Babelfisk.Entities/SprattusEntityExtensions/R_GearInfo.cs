using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class R_GearInfo : ILookupEntity, IComparable
    {
        private static List<L_Gear> _lstGears;

        private static List<L_GearInfoType> _lstGearInfoTypes;

        private static Dictionary<int, L_Gear> _dicGears = null;
        private static Dictionary<string, L_GearInfoType> _dicGearInfoTypes = null;

        public static List<L_Gear> Gears
        {
            get { return _lstGears; }
            set 
            { 
                _lstGears = value;
                if (value != null)
                    _dicGears = value.ToDictionary(x => x.gearId);
            }
        }

        public static List<L_GearInfoType> GearInfoTypes
        {
            get { return _lstGearInfoTypes; }
            set 
            { 
                _lstGearInfoTypes = value;
                if (value != null)
                    _dicGearInfoTypes = value.ToDictionary(x => x.gearInfoType);
            }
        }


        public string L_GearUIDisplay
        {
            get
            {
                if (_dicGears == null || !_dicGears.ContainsKey(this.gearId))
                    return "";

                return _dicGears[this.gearId].UIDisplay;
            }
        }


        public string L_GearInfoTypeUIDisplay
        {
            get
            {
                if (gearInfoType == null || _dicGears == null || !_dicGearInfoTypes.ContainsKey(this.gearInfoType))
                    return "";

                return _dicGearInfoTypes[this.gearInfoType].UIDisplay;
            }
        }


        /// <summary>
        /// gearId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.R_gearInfoID.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2}", L_GearUIDisplay, L_GearInfoTypeUIDisplay, this.gearValue);
            }
        }


        public string DefaultSortValue
        {
            get { return FilterValue; }
        }


        /// <summary>
        /// Used for display in drop down list for example
        /// </summary>
        public string UIDisplay
        {
            get
            {
                string str = FilterValue;
                return str;
            }
        }


        public override bool Equals(object obj)
        {
            R_GearInfo other = obj as R_GearInfo;

            if (other == null)
                return false;

            return this.R_gearInfoID == other.R_gearInfoID && 
                        (gearId.Equals(other.gearId)) &&
                        (gearInfoType == null ? other.gearInfoType == null : gearInfoType.Equals(other.gearInfoType)) &&
                        (gearValue == null ? other.gearValue == null : gearValue.Equals(other.gearValue))
                ;
        }

        /*
        public override int GetHashCode()
        {
            return this.R_gearInfoID.GetHashCode() ^
                   gearId.GetHashCode() ^
                   (gearInfoType == null ? 0 : gearInfoType.GetHashCode()) ^
                   (gearValue == null ? 0 : gearValue.GetHashCode())
                  ;
        }
        */

        public int CompareTo(object obj)
        {
            R_GearInfo other = obj as R_GearInfo;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
          
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (gearId == 0)
                return "Angiv venligst redskab.";

            if (string.IsNullOrEmpty(gearInfoType))
                return "Angiv venligst redskabstype.";

            if (string.IsNullOrWhiteSpace(gearValue))
                return "Angiv venligst redskabsværdi.";

            if (gearValue.Length > 80)
                return "Redskabsværdi må kun bestå af 80 tegn.";

            if (lst.OfType<R_GearInfo>().Where(x => (x.R_gearInfoID != R_gearInfoID || (this.ChangeTracker.State == ObjectState.Added && x != this)) &&
                                                    (x.gearId.Equals(gearId)) &&
                                                    (x.gearInfoType != null && x.gearInfoType.Equals(gearInfoType)))
                                                    .Count() > 0)
                return string.Format("En redskabsværdi eksisterer allerede for redskab '{0}', redskabstype '{1}'.", L_GearUIDisplay, L_GearInfoTypeUIDisplay);

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
        }
    }
}
