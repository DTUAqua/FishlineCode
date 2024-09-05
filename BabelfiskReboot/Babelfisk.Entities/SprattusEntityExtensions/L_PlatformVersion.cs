using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    public partial class L_PlatformVersion  : ILookupEntity
    {
        private static List<L_Platform> _lstPlatforms;

        private static List<L_NavigationSystem> _lstNavigationSystems;

        private static Dictionary<string, L_Platform> _dicPlatforms = null;
        private static Dictionary<string, L_NavigationSystem> _dicNavigationSystems = null;


        #region Properties

        /// <summary>
        /// platformVersionId is used as reference to other tables
        /// </summary>
        public string Id
        {
            get { return this.platformVersionId.ToString(); }
        }


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

        public static List<L_NavigationSystem> NavigationSystems
        {
            get { return _lstNavigationSystems; }
            set 
            { 
                _lstNavigationSystems = value;
                if (value != null)
                    _dicNavigationSystems = value.ToDictionary(x => x.navigationSystem);
            }
        }


        public string UIDisplay
        {
            get { return L_PlatformUIDisplay; }
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

        public string L_NavigationSystemUIDisplay
        {
            get
            {
                if (navigationSystem == null || _dicNavigationSystems == null || !_dicNavigationSystems.ContainsKey(navigationSystem))
                    return "";

                return _dicNavigationSystems[navigationSystem].CodeDescription;
            }
        }



        public string FilterValue
        {
            get { return String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9}", this.L_PlatformUIDisplay, 
                                                                                  this.version, 
                                                                                  this.revisionYear, 
                                                                                  this.L_NavigationSystemUIDisplay, 
                                                                                  this.registerTons,
                                                                                  this.length, 
                                                                                  this.power,
                                                                                  this.crew, 
                                                                                  this.radiosignal, 
                                                                                  this.description); }
        }


        public string DefaultSortValue
        {
            get
            {
                return platform;
            }
        }


        #endregion


        public void BeforeSave()
        {
           
        }

        public string GetErrors(List<ILookupEntity> lst)
        {
            if (string.IsNullOrWhiteSpace(platform))
                return "Angiv venligst et skib.";

            if (version <= 0)
                return "Version skal være større end 0.";

            if (radiosignal != null && radiosignal.Length > 50)
                return "Radiosignal må kun bestå af 50 tegn.";

            if (description != null && description.Length > 240)
                return "Beskrivelse må kun bestå af 240 tegn.";

            if (lst.OfType<L_PlatformVersion>().Where(x => (x.platformVersionId != platformVersionId || (this.ChangeTracker.State == ObjectState.Added && x != this)) && 
                                                            x.platform.Equals(platform, StringComparison.InvariantCultureIgnoreCase) && 
                                                            x.version == version
                                                            ).Count() > 0)
                return string.Format("Skibsversion med skibskode '{0}' og version '{1}' eksisterer allerede.", platform, version);

            return null;
        }


        public void NewLookupCreated()
        {
        }
    }
}
