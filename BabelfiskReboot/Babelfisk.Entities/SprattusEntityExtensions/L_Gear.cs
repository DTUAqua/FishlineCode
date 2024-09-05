using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_Gear : ILookupEntity, IComparable
    {
        private static List<L_Platform> _lstPlatforms;

        private static List<L_GearType> _lstGearTypes;

        private static Dictionary<string, L_GearType> _dicGearTypes = null;
        private static Dictionary<string, L_Platform> _dicPlatforms = null;


        public static List<L_Platform> Platforms
        {
            get { return _lstPlatforms; }
            set
            { 
                _lstPlatforms = value;
                if (value != null)
                    _dicPlatforms = value.ToDictionary(x => x.platform);
            }
        }

        public static List<L_GearType> GearTypes
        {
            get { return _lstGearTypes; }
            set 
            { 
                _lstGearTypes = value;
                if (value != null)
                    _dicGearTypes = value.ToDictionary(x => x.gearType);
            }
        }


        public string L_GearTypeUIDisplay
        {
            get
            {
                if (gearType == null || _dicGearTypes == null || !_dicGearTypes.ContainsKey(gearType))
                    return "";

                return _dicGearTypes[gearType].UIDisplay;
            }
        }


        public string L_PlatformUIDisplay
        {
            get
            {
                if (platform == null || _dicPlatforms == null || !_dicPlatforms.ContainsKey(platform))
                    return "";

                return _dicPlatforms[platform].UIString;
            }
        }



        /// <summary>
        /// gearId code is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.gearId.ToString(); }
        }

        public string FilterValue
        {
            get
            {
                return String.Format("{0} {1} {2} {3} {4} {5}", this.L_PlatformUIDisplay, 
                                                            this.gear, this.gearText, this.version,
                                                            this.L_GearTypeUIDisplay,
                                                            this.description
                                                            );
            }
        }

        public string DefaultSortValue
        {
            get { return this.gear; }
        }


        public string UIDisplay
        {
            get
            {
                string str = this.gear;
                if (!string.IsNullOrWhiteSpace(platform))
                    str += " - " + platform;

                return str;
            }
        }

        
        public override bool Equals(object obj)
        {
            L_Gear other = obj as L_Gear;

            if (other == null)
                return false;

            return this.gearId == other.gearId &&
                   (gearText != null ? gearText.Equals(other.gearText) : other.gearText == null) &&
                   version.Equals(other.version) &&
                   (platform != null ? platform.Equals(other.platform) : other.platform == null) &&
                   (gearType != null ? gearType.Equals(other.gearType) : other.gearType == null) &&
                   (description != null ? description.Equals(other.description) : other.description == null)
                   ;
        }
        
        /*
        public override int GetHashCode()
        {
            return gearId.GetHashCode() ^
                  (platform == null ? 0 : platform.GetHashCode()) ^
                  (description == null ? 0 : description.GetHashCode()) ^
                  (gear == null ? 0 : gear.GetHashCode()) ^
                  (gearText == null ? 0 : gearText.GetHashCode())
                  ;
        }
        */

        public int CompareTo(object obj)
        {
            L_Gear other = obj as L_Gear;

            if (other == null)
                return -1;

            return this.UIDisplay.CompareTo(other.UIDisplay);
        }

        public void BeforeSave()
        {
        }


        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(gear))
                return "Angiv venligst kode.";

            if (gear.Length > 80)
                return "Koden må kun bestå af 80 tegn.";

            if (platform == null)
                return "Angiv venligst skib.";

            if (gearType == null)
                return "Angiv venligst redskabstype.";

            if (gearText == null && gearText.Length > 80)
                return "Tekst må kun bestå af 80 tegn.";

            if (version < 0)
                return "Angiv en version større end 0";

            if (description != null && description.Length > 160)
                return "Beskrivelse må kun bestå af 160 tegn.";

            if (lst.OfType<L_Gear>().Where(x => (x.gearId != gearId || (this.ChangeTracker.State == ObjectState.Added && x != this)) &&
                                                 x.gear.Equals(gear, StringComparison.InvariantCultureIgnoreCase) &&
                                                (x.platform != null && x.platform.Equals(platform)) &&
                                                x.version.Equals(version)
                                                ).Count() > 0)
                return string.Format("Et redskab med kode '{0}', skib '{1}' og version '{2}' eksisterer allerede.", gear, L_PlatformUIDisplay, version);

            return null;
        }


        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return this.gear; }
        }

        public void NewLookupCreated()
        {
        }
    }
}
