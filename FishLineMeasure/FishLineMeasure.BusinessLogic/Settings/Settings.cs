using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Microsoft.Practices.Prism.ViewModel;
using Anchor.Core;
using Babelfisk.Entities;
using Babelfisk.Entities.SprattusSecurity;

namespace FishLineMeasure.BusinessLogic.Settings
{
    /// <summary>
    /// Class implements methods for saving and retrieving settings data. Data can be stored in application wide
    /// Settings information is stored in XML files.
    /// </summary>
    [Serializable]
    public class Settings : NotificationObject
    {
        private static Users _user = null;


        public event Action<Settings, string> OnUserChanged;

        /// <summary>
        /// Singleton pattern.
        /// </summary>
        public static readonly Settings Instance = new Settings();


        /// <summary>
        /// String constant defining the root path of EaseTech
        /// </summary>
        private readonly string _strUserRootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Anchor.Core.Loggers.BaseLogger.AssemblyName + @"\");
        private readonly string _strUserDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        private readonly string _strSettingsFile;

        /// <summary>
        /// Reference to User domain settings
        /// </summary>
        private UserSettingsContainer m_scUser;

        /// <summary>
        /// Reference to Application domain settings.
        /// </summary>
        private SettingsContainer m_scApplication;


        private DataVersioning _lookupVersioning = null;



        #region Properties


        public static Users CurrentUser
        {
            get { return _user; }
            set
            {
                _user = value;
            }
        }


        public DataVersioning LookupVersions
        {
            get { return _lookupVersioning; }
        }


        /// <summary>
        /// User settings container
        /// </summary>
        private UserSettingsContainer User
        {
            get { return m_scUser; }
        }


        public bool IsUserSettingsLoaded
        {
            get { return m_scUser != null; }
        }


        /// <summary>
        /// A reference to the application global settings. 
        /// </summary>
        private SettingsContainer Application
        {
            get { return m_scApplication; }
        }


        #endregion


        #region Constructor

        private Settings()
        {
            _strSettingsFile = Path.Combine(SettingsPath, @"Settings.xml");

            m_scUser = null;

            //Create the application settings container
            m_scApplication = new SettingsContainer("Application");

            _lookupVersioning = DataVersioning.LoadFromFile(Path.Combine(this.OfflineFolderPath, "LookupVersions.xml"));

            if (!File.Exists(_strSettingsFile))
                return;

            //Load application settings container if file exist
            Load(m_scApplication, "Application");

            //Dont load the user settings container at this point, since a user name is required.
        }

        #endregion


        #region Methods

        private void Load(SettingsContainer sc, string strNodeName, string strName = null)
        {
            if (!File.Exists(_strSettingsFile))
            {
                sc = new SettingsContainer(strNodeName);
                return;
            }

            XDocument xdSettings = null;
            try
            {
                xdSettings = XDocument.Load(_strSettingsFile);
            }
            catch
            {
                sc = new SettingsContainer(strNodeName);
                return;
            }

            if (xdSettings.Element("Settings") != null)
            {
                if (strName != null)
                {
                    var v = GetUserSettingsNode(xdSettings, strName);
                    if(v != null)
                        sc.Load(v);
                }
                else
                    sc.Load(xdSettings.Element("Settings").Element(strNodeName));
            }
        }


        private XElement GetUserSettingsNode(XDocument xdSettings, string strName)
        {
            return (from e in xdSettings.Element("Settings").Elements("User")
                    where e.Attribute("name") != null && e.Attribute("name").Value == strName
                    select e).FirstOrDefault();
        }


        /// <summary>
        /// Load user settings from a user name
        /// </summary>
        public void LoadUserSettings(string strUserName)
        {
            m_scUser = new UserSettingsContainer(strUserName, this);

            Load(m_scUser, "User", strUserName);

            if (OnUserChanged != null)
                OnUserChanged(this, strUserName);
        }


        /// <summary>
        /// Save settings to disk.
        /// </summary>
        public void Save()
        {
            XElement xeApplication = Application.Save();
            XElement xeUser = User == null ? null : User.Save();

            XDocument xdSettings = null;

            if (File.Exists(_strSettingsFile))
            {
                try
                {
                    xdSettings = XDocument.Load(_strSettingsFile);
                }
                catch
                {
                    xdSettings = new XDocument(new XElement("Settings"));
                }
            }
            else
                xdSettings = new XDocument(new XElement("Settings"));

            if (xeApplication != null)
            {
                if (xdSettings.Element("Settings").Element(Application.Identifier) == null)
                    xdSettings.Element("Settings").Add(xeApplication);
                else
                    xdSettings.Element("Settings").Element(Application.Identifier).ReplaceWith(xeApplication);
            }

            if (xeUser != null)
            {
                var xeElm = GetUserSettingsNode(xdSettings, User.Identifier);
                if (xeElm == null)
                    xdSettings.Element("Settings").Add(xeUser);
                else
                    xeElm.ReplaceWith(xeUser);
            }

            xdSettings.Save(_strSettingsFile);

            //Save lookup versions
            if(_lookupVersioning != null)
                _lookupVersioning.Save();
        }


        #endregion


        #region Settings


        /// <summary>
        /// Return application root path for the logged in user.
        /// </summary>
        public string UserRootPath
        {
            get 
            {
                if (!Directory.Exists(_strUserRootPath))
                    Directory.CreateDirectory(_strUserRootPath);

                return _strUserRootPath; 
            }
        }


        /// <summary>
        /// Return settings path
        /// </summary>
        internal string SettingsPath
        {
            get 
            {
                string strSettings = Path.Combine(UserRootPath, "Settings");

                if (!Directory.Exists(strSettings))
                    Directory.CreateDirectory(strSettings);

                return strSettings; 
            }
        }


        /// <summary>
        /// Return data root path for logged in user.
        /// </summary>
        public string AppRootPath
        {
            get
            {
                var path = Path.Combine(_strUserDocumentsPath, @"FishLineMeasure");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }



        /// <summary>
        /// Return data root path for logged in user.
        /// </summary>
        public string DataRootPath
        {
            get
            {
                var path = Path.Combine(AppRootPath, @"Data");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }

        /// <summary>
        /// Return data root path for logged in user.
        /// </summary>
        public string BackupRootPath
        {
            get
            {
                var path = Path.Combine(AppRootPath, @"Backup");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }




        #region Application settings


        /// <summary>
        /// Get/Set main window startup width. This is an application setting which is stored when application closes.
        /// </summary>
        public double MainWindowStartupWidth
        {
            get { return Application["MainWindowStartupWidth"] == null ? 1024 : Application["MainWindowStartupWidth"].ToString().ToDouble(); }
            set { Application.SetStoredValue("MainWindowStartupWidth", value); }
        }


        /// <summary>
        /// Get/Set main window startup height. This is an application setting which is stored when application closes.
        /// </summary>
        public double MainWindowStartupHeight
        {
            get { return Application["MainWindowStartupHeight"] == null ? 768 : Application["MainWindowStartupHeight"].ToString().ToDouble(); }
            set { Application.SetStoredValue("MainWindowStartupHeight", value); }
        }


        /// <summary>
        /// Get/Set main window startup state. This is an application setting which is stored when application closes.
        /// </summary>
        public System.Windows.WindowState MainWindowStartupState
        {
            get { return Application["MainWindowStartupState"] == null ? System.Windows.WindowState.Normal : (System.Windows.WindowState)Enum.Parse(typeof(System.Windows.WindowState), Application["MainWindowStartupState"].ToString()); }
            set
            {
                System.Windows.WindowState ws = value;

                Application.SetStoredValue("MainWindowStartupState", ws.ToString());
            }
        }

        public double MainWindowMinWidth
        {
            get { return Application["MainWindowStartupWidth"] == null ? 900 : Application["MainWindowStartupWidth"].ToString().ToDouble(); }
            set { Application.SetStoredValue("MainWindowStartupWidth", value); }
        }


        /// <summary>
        /// Get/Set main window startup height. This is an application setting which is stored when application closes.
        /// </summary>
        public double MainWindowMinHeight
        {
            get { return Application["MainWindowStartupHeight"] == null ? 768 : Application["MainWindowStartupHeight"].ToString().ToDouble(); }
            set { Application.SetStoredValue("MainWindowStartupHeight", value); }
        }

        /// <summary>
        /// Get/Set default username at login
        /// </summary>
        public string UserName
        {
            get { return Application["UserName"] == null ? null : Application["UserName"].ToString(); }
            set { Application.SetStoredValue("UserName", value); }
        }

        public DateTime? LastLoginTime
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the path for Babelfisk offline data.
        /// </summary>
        public string OfflineFolderPath
        {
            get
            {
                string strPath = Path.Combine(UserRootPath, "OfflineData");

                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                return strPath;
            }
        }


        public string OfflineDataPath
        {
            get
            {
                string strPath = Path.Combine(OfflineFolderPath, "Data");

                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                return strPath;
            }
        }


        /// <summary>
        /// Returns the path for Babelfisk offline data.
        /// </summary>
        public string OfflineLookupDataPath
        {
            get
            {
                string strPath = Path.Combine(OfflineFolderPath, "LookupData");

                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);
                return strPath;
            }
        }


        /// <summary>
        /// Returns whether lookups have been synced to disk
        /// </summary>
        public bool HasLookups
        {
            get { return Directory.Exists(OfflineLookupDataPath) && Directory.GetFiles(OfflineLookupDataPath).Length > 0; }
        }


        /// <summary>
        /// Get/Set default username at login
        /// </summary>
        public string DefaultTrip
        {
            get { return Application["DefaultTrip"] == null ? null : Application["DefaultTrip"].ToString(); }
            set { Application.SetStoredValue("DefaultTrip", value); }
        }


        public string DefaultStation
        {
            get { return Application["DefaultStation"] == null ? null : Application["DefaultStation"].ToString(); }
            set { Application.SetStoredValue("DefaultStation", value); }
        }


        public int NumberOfBackupFiles
        {
            get { return Application["NumberOfBackupFiles"] == null ? 50 : Application["NumberOfBackupFiles"].ToString().ToInt32(); }
            set { Application.SetStoredValue("NumberOfBackupFiles", value); }
        }


        public string BluetoothAddress
        {
            get { return Application["BluetoothAddress"] == null ? null : Application["BluetoothAddress"].ToString(); }
            set { Application.SetStoredValue("BluetoothAddress", value); }
        }


      /*  public string LengthGroups
        {
            get { return Application["LengthGroups"] == null ? null : Application["LengthGroups"].ToString(); }
            set { Application.SetStoredValue("LengthGroups", value); }
        }*/

        public string LengthGroupsCollection
        {
            get { return Application["LengthGroupsCollection"] == null ? null : Application["LengthGroupsCollection"].ToString(); }
            set { Application.SetStoredValue("LengthGroupsCollection", value); }
        }

        public string SelectedLengthGroupName
        {
            get { return Application["SelectedLengthGroup"] == null ? null : Application["SelectedLengthGroup"].ToString(); }
            set { Application.SetStoredValue("SelectedLengthGroup", value); }
        }


        public int BluetoothSearchTimeoutSeconds
        {
            get { return Application["BluetoothSearchTimeoutSeconds"] == null ? 10 : Application["BluetoothSearchTimeoutSeconds"].ToString().ToInt32(); }
            set { Application.SetStoredValue("BluetoothSearchTimeoutSeconds", value); }
        }

        public bool BluetoothAutoConnect
        {
            get { return Application["BluetoothAutoConnect"] == null ? true : Application["BluetoothAutoConnect"].ToString().ToBool(); }
            set { Application.SetStoredValue("BluetoothAutoConnect", value); }
        }

        public string SelectedCaliberSetting
        {
            get { return Application["SelectedCaliberSetting"] == null ? "NA" : Application["SelectedCaliberSetting"].ToString(); }
            set { Application.SetStoredValue("SelectedCaliberSetting", value); }
        }

        public double ValueForDeletingLastEntry
        {
            get { return Application["ValueForDeletingLastEntry"] == null ? 0 : Application["ValueForDeletingLastEntry"].ToString().ToDouble(); }
            set { Application.SetStoredValue("ValueForDeletingLastEntry", value); }
        }

        public double ValueForGoingToNextStation
        {
            get { return Application["ValueForGoingToNextStation"] == null ? -1000 : Application["ValueForGoingToNextStation"].ToString().ToDouble(); }
            set { Application.SetStoredValue("ValueForGoingToNextStation", value); }
        }

        public double ValueForGoingToNextOrder
        {
            get { return Application["ValueForGoingToNextOrder"] == null ? 15.5 : Application["ValueForGoingToNextOrder"].ToString().ToDouble(); }
            set { Application.SetStoredValue("ValueForGoingToNextOrder", value); }
        }

        public int FrequencySettingRepeatForNewLenghtAdded
        {
            get { return Application["FrequencySettingRepeatForNewLenghtAdded"] == null ? 1 : Application["FrequencySettingRepeatForNewLenghtAdded"].ToString().ToInt32(); }
            set { Application.SetStoredValue("FrequencySettingRepeatForNewLenghtAdded", value); }
        }

        public int FrequencySettingRepeatForDeleteLenght
        {
            get { return Application["FrequencySettingRepeatForDeleteLenght"] == null ? 3 : Application["FrequencySettingRepeatForDeleteLenght"].ToString().ToInt32(); }
            set { Application.SetStoredValue("FrequencySettingRepeatForDeleteLenght", value); }
        }

        public int FrequencySettingRepeatForNextStation
        {
            get { return Application["FrequencySettingRepeatForNextStation"] == null ? 4 : Application["FrequencySettingRepeatForNextStation"].ToString().ToInt32(); }
            set { Application.SetStoredValue("FrequencySettingRepeatForNextStation", value); }
        }

        public int FrequencySettingRepeatForNextOrder
        {
            get { return Application["FrequencySettingRepeatForNextOrder"] == null ? 2 : Application["FrequencySettingRepeatForNextOrder"].ToString().ToInt32(); }
            set { Application.SetStoredValue("FrequencySettingRepeatForNextOrder", value); }
        }



        public int FrequencySettingForNewLenghtAdded
        {
            get { return Application["FrequencySettingForNewLenghtAdded"] == null ? 300 : Application["FrequencySettingForNewLenghtAdded"].ToString().ToInt32(); }
            set { Application.SetStoredValue("FrequencySettingForNewLenghtAdded", value); }
        }

        public int FrequencySettingForDeleteLenght
        {
            get { return Application["FrequencySettingForDeleteLenght"] == null ? 500 : Application["FrequencySettingForDeleteLenght"].ToString().ToInt32(); }
            set { Application.SetStoredValue("FrequencySettingForDeleteLenght", value); }
        }

        public int FrequencySettingForNextStation
        {
            get { return Application["FrequencySettingForNextStation"] == null ? 1200 : Application["FrequencySettingForNextStation"].ToString().ToInt32(); }
            set { Application.SetStoredValue("FrequencySettingForNextStation", value); }
        }

        public int FrequencySettingForNextOrder
        {
            get { return Application["FrequencySettingForNextOrder"] == null ? 1000 : Application["FrequencySettingForNextOrder"].ToString().ToInt32(); }
            set { Application.SetStoredValue("FrequencySettingForNextOrder", value); }
        }



        public bool FrequencySettingsStatusForNewLenght
        {
            get { return Application["FrequencySettingsStatusForNewLenght"] == null ? true : Application["FrequencySettingsStatusForNewLenght"].ToString().ToBool(); }
            set { Application.SetStoredValue("FrequencySettingsStatusForNewLenght", value); }
        }

        public bool FrequencySettingsStatusForDelete
        {
            get { return Application["FrequencySettingsStatusForDelete"] == null ? true : Application["FrequencySettingsStatusForDelete"].ToString().ToBool(); }
            set { Application.SetStoredValue("FrequencySettingsStatusForDelete", value); }
        }

        public bool FrequencySettingsStatusForNextStation
        {
            get { return Application["FrequencySettingsStatusForNextStation"] == null ? true : Application["FrequencySettingsStatusForNextStation"].ToString().ToBool(); }
            set { Application.SetStoredValue("FrequencySettingsStatusForNextStation", value); }
        }

        public bool FrequencySettingsStatusForNextOrder
        {
            get { return Application["FrequencySettingsStatusForNextOrder"] == null ? true : Application["FrequencySettingsStatusForNextOrder"].ToString().ToBool(); }
            set { Application.SetStoredValue("FrequencySettingsStatusForNextOrder", value); }
        }



        public bool UpdateLookupsAfterStartup
        {
            get { return Application["UpdateLookupsAfterStartup"] == null ? true : Application["UpdateLookupsAfterStartup"].ToString().ToBool(); }
            set { Application.SetStoredValue("UpdateLookupsAfterStartup", value); }
        }


        private int? _lengthCMDecimal = null;

        public int LengthCMDecimals
        {
            get 
            {
                if (_lengthCMDecimal != null)
                    return _lengthCMDecimal.Value;

                _lengthCMDecimal = Application["LengthCMDecimals"] == null ? 1 : Application["LengthCMDecimals"].ToString().ToInt32();
                return _lengthCMDecimal.Value;
            }
            set 
            {
                _lengthCMDecimal = value;
                Application.SetStoredValue("LengthCMDecimals", value);
            }
        }


        private int? _lengthMMDecimal = null;

        public int LengthMMDecimals
        {
            get 
            {
                if (_lengthMMDecimal != null)
                    return _lengthMMDecimal.Value;

                _lengthMMDecimal = Application["LengthMMDecimals"] == null ? 0 : Application["LengthMMDecimals"].ToString().ToInt32();
                return _lengthMMDecimal.Value;
            }
            set 
            {
                _lengthMMDecimal = value;
                Application.SetStoredValue("LengthMMDecimals", value);
            }
        }



        public bool ShowWarningOnScreenCloseCommand
        {
            get { return Application["ShowWarningOnScreenCloseCommand"] == null ? true : Application["ShowWarningOnScreenCloseCommand"].ToString().ToBool(); }
            set { Application.SetStoredValue("ShowWarningOnScreenCloseCommand", value); }
        }


        public bool ExportToCSVAsDefault
        {
            get { return Application["ExportToCSVAsDefault"] == null ? true : Application["ExportToCSVAsDefault"].ToString().ToBool(); }
            set { Application.SetStoredValue("ExportToCSVAsDefault", value); }
        }


        private Unit? _defaultDisplayUnit = null;

        public Unit DefaultDisplayUnit
        {
            get 
            {
                Unit unit;
                if (_defaultDisplayUnit == null)
                {
                    var strUnit = Application["DefaultDisplayUnit"] == null ? "" : Application["DefaultDisplayUnit"].ToString();

                    if (string.IsNullOrWhiteSpace(strUnit) || !Enum.TryParse<Unit>(strUnit, out unit))
                        unit = Unit.MM;
                }
                else
                    unit = _defaultDisplayUnit.Value;

                return unit;
            }
            set 
            {
                _defaultDisplayUnit = value;
                Application.SetStoredValue("DefaultDisplayUnit", value.ToString()); 
            }
        }



        #endregion

        #endregion
    }
}
