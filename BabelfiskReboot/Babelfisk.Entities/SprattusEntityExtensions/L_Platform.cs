using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
     [Serializable]
    public partial class L_Platform : OfflineEntity, ILookupEntity, IComparable
    {
        private static List<L_PlatformType> _lstPlatformTypes;

        private static List<L_Nationality> _lstNationalities;

        private static List<Person> _lstPersons;

        private static Dictionary<string, L_PlatformType> _dicPlatformTypes = null;
        private static Dictionary<string, L_Nationality> _dicNationalities = null;
        private static Dictionary<int, Person> _dicPersons = null;


        /// <summary>
        /// platform code is used as reference to other tables
        /// </summary>
         public string Id
         {
             get { return this.platform; }
         }


        public static List<L_PlatformType> PlatformTypes
        {
            get { return _lstPlatformTypes; }
            set 
            { 
                _lstPlatformTypes = value;
                if (value != null)
                    _dicPlatformTypes = value.ToDictionary(x => x.platformType);
            }
        }

        public static List<L_Nationality> Nationalities
        {
            get { return _lstNationalities; }
            set 
            { 
                _lstNationalities = value;
                if (value != null)
                    _dicNationalities = value.ToDictionary(x => x.nationality);
            }
        }

        public static List<Person> Persons
        {
            get { return _lstPersons; }
            set 
            { 
                _lstPersons = value;
                if (value != null)
                    _dicPersons = value.ToDictionary(x => x.personId);
            }
        }


        public string UIDisplay
        {
            get { return UIString; }
        }

        public string UIString
        {
            get 
            {
                string strPlatform = this.nationality;

                strPlatform += (string.IsNullOrWhiteSpace(strPlatform) ? "" : " - ") + this.platform;

                if (!string.IsNullOrEmpty(this.description))
                    strPlatform += " - " + this.description;

                return strPlatform;
            }
        }


        public string L_PlatformTypeUIDisplay
        {
            get
            {
                if (platformType == null || _dicPlatformTypes == null || !_dicPlatformTypes.ContainsKey(platformType))
                    return "";

                return _dicPlatformTypes[platformType].CodeDescription;
            }
        }

        public string L_NationalityUIDisplay
        {
            get
            {
                if (nationality == null || _dicNationalities == null || !_dicNationalities.ContainsKey(nationality))
                    return "";

                return _dicNationalities[nationality].CodeDescription;
            }
        }

        public string PersionUIDisplay
        {
            get
            {
                if (contactPersonId == null || _dicPersons == null || !_dicPersons.ContainsKey(contactPersonId.Value))
                    return "";

                return _dicPersons[contactPersonId.Value].name;
            }
        }




        /// <summary>
        /// IFilteredComboboxItem interface property
        /// </summary>
        public string CompareValue
        {
            get { return this.platform; }
        }


        public string FilterValue
        {
            get { return String.Format("{0} {1} {2} {3} {4} {5}", this.platform, 
                                                                  this.L_PlatformTypeUIDisplay, 
                                                                  this.name, 
                                                                  this.L_NationalityUIDisplay, 
                                                                  this.PersionUIDisplay, 
                                                                  this.description); }
        }

        public string DefaultSortValue
        {
            get { return platform; }
        }

        public bool GetErrors()
        {
            return !string.IsNullOrWhiteSpace(platform);
        }

        public void BeforeSave()
        {
            if(platform != null && platform != platform.ToUpper())
                platform = platform.ToUpper();

            if (boatIdentity != platform)
                boatIdentity = platform;
        }


        public override bool Equals(object obj)
        {
            L_Platform other = obj as L_Platform;

            if (other == null)
                return false;

            return this.L_platformId == other.L_platformId &&
                (platform != null ? platform.Equals(other.platform) : other.platform == null) &&
                (platformType != null ? platformType.Equals(other.platformType) : other.platformType == null) &&
                (name != null ? name.Equals(other.name) : other.name == null) &&
                (nationality != null ? nationality.Equals(other.nationality) : other.nationality == null) &&
                (contactPersonId.HasValue ? contactPersonId.Equals(other.contactPersonId) : other.contactPersonId == null) &&
                (description != null ? description.Equals(other.description) : other.description == null)
                ;
        }

         /*
        public override int GetHashCode()
        {
            return L_platformId.GetHashCode() ^
                   (platform == null ? 0 : platform.GetHashCode()) ^
                   (platformType == null ? 0 : platformType.GetHashCode()) ^
                   (name == null ? 0 : name.GetHashCode()) ^
                   (nationality == null ? 0 : nationality.GetHashCode()) ^
                   (contactPersonId.HasValue ? contactPersonId.Value.GetHashCode() : 0) ^
                   (description == null ? 0 : description.GetHashCode())
                   ;
        }
         */

        public int CompareTo(object obj)
        {
            L_Platform other = obj as L_Platform;

            if (other == null)
                return -1;

            return this.UIString.CompareTo(other.UIString);
        }

        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(platform))
                return "Angiv venligst skibs-kode.";

            if (platform.Length > 20)
                return "Skibskode må kun bestå af 20 tegn.";

            if(string.IsNullOrWhiteSpace(nationality))
                return "Alle nye/ændrede skibe skal have en nationalitet";

            if (name != null && name.Length > 30)
                return "Skibsnavn må kun bestå af 30 tegn.";

            if (description != null && description.Length > 80)
                return "Beskrivelse må kun bestå af 80 tegn.";

            if (lst.OfType<L_Platform>().Where(x => (x.L_platformId != L_platformId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && x.platform.Equals(platform, StringComparison.InvariantCultureIgnoreCase)).Count() > 0)
                return string.Format("Et skib med kode '{0}' eksisterer allerede.", platform);

            return null;
        }

        public void NewLookupCreated()
        {
        }
    }
}
