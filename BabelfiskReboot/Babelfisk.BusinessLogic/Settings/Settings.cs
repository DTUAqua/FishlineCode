using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Microsoft.Practices.Prism.ViewModel;
using Anchor.Core;
using Babelfisk.Entities;
using System.Reflection;
using System.Globalization;

namespace Babelfisk.BusinessLogic.Settings
{
    /// <summary>
    /// Class implements methods for saving and retrieving settings data. Data can be stored in application wide
    /// Settings information is stored in XML files.
    /// </summary>
    [Serializable]
    public class Settings : NotificationObject
    {
        public event Action<Settings, string> OnUserChanged;

        /// <summary>
        /// Singleton pattern.
        /// </summary>
        public static readonly Settings Instance = new Settings();


        /// <summary>
        /// String constant defining the root path of EaseTech
        /// </summary>
        private readonly string _strUserRootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Anchor.Core.Loggers.BaseLogger.AssemblyName + @"\" /*@"Babelfisk\"*/);
        private readonly string _strSharedRootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Anchor.Core.Loggers.BaseLogger.AssemblyName + @"\" /*@"Babelfisk\"*/);
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

       public static Anchor.Core.Language.Translater Translater;


        #region Properties

        public static string Translate(string strSection, string strKey)
        {
            if (Translater == null)
                return "";

            return Translater.Translate(strSection, strKey);
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

            //Make sure to reset memory stored variables, in case of when a user logs out and in as another user.
            _dataGridColumnSettingsContainer = null;

            Load(m_scUser, "User", strUserName);

            if (OnUserChanged != null)
                OnUserChanged(this, strUserName);
        }


        /// <summary>
        /// Save settings to disk.
        /// </summary>
        public void Save()
        {
            //Save offline status if it exists
            if(_offlineStatus != null)
                OfflineStatus = _offlineStatus;

            //Persist ServerNames
            if(_serverNames != null)
                BusinessLogic.Settings.Settings.Instance.ServerNames = _serverNames;

            //Persist OfflineUsers
            if(_offlineUsers != null)
                BusinessLogic.Settings.Settings.Instance.OfflineUsers = _offlineUsers;

            if (_dataGridColumnSettingsContainer != null)
                BusinessLogic.Settings.Settings.Instance.DataGridColumnSettings = _dataGridColumnSettingsContainer;

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
        /// Return application root path shared by all users.
        /// </summary>
        public string SharedRootPath
        {
            get
            {
                if (!Directory.Exists(_strSharedRootPath))
                    Directory.CreateDirectory(_strSharedRootPath);

                return _strSharedRootPath;
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


        /// <summary>
        /// Get/Set default username at login
        /// </summary>
        public string UserName
        {
            get { return Application["UserName"] == null ? null : Application["UserName"].ToString(); }
            set { Application.SetStoredValue("UserName", value); }
        }


        /// <summary>
        /// Returns the path for Babelfisk offline data.
        /// </summary>
        public string BackupDirectoryPath
        {
            get { return Application["BackupDirectoryPath"] == null ? Path.Combine(UserRootPath, "Backups") : Application["BackupDirectoryPath"].ToString(); }
            set { Application.SetStoredValue("BackupDirectoryPath", value); }
        }


        public bool IsAutomaticBackupEnabled
        {
            get { return Application["IsAutomaticBackupEnabled"] == null ? true : Application["IsAutomaticBackupEnabled"].ToString().ToBool(); }
            set { Application.SetStoredValue("IsAutomaticBackupEnabled", value); }
        }

        public int NoOfBackupsPerDay
        {
            get { return Application["NoOfBackupsPerDay"] == null ? 50 : Application["NoOfBackupsPerDay"].ToString().ToInt32(); }
            set { Application.SetStoredValue("NoOfBackupsPerDay", value); }
        }

        public int NoOfBackupDays
        {
            get { return Application["NoOfBackupDays"] == null ? 30 : Application["NoOfBackupDays"].ToString().ToInt32(); }
            set { Application.SetStoredValue("NoOfBackupDays", value); }
        }


        private OfflineStatus _offlineStatus;
        /// <summary>
        /// Get/Set OfflineStatus
        /// </summary>
        public OfflineStatus OfflineStatus
        {
            get 
            {
                try
                {
                    if (_offlineStatus == null)
                        _offlineStatus = Application["OfflineStatus"] == null ? new OfflineStatus() : OfflineStatus.Deserialize(Application["OfflineStatus"].ToString());
                }
                catch (Exception e)
                {
                    Anchor.Core.Loggers.Logger.LogError(e);
                }

                if (_offlineStatus == null)
                    _offlineStatus = new OfflineStatus();

                return _offlineStatus;
            }
            set { Application.SetStoredValue("OfflineStatus", value.Serialize()); }
        }


        private ServerNames _serverNames;

        public ServerNames ServerNames
        {
            get
            {
                if (_serverNames == null)
                    _serverNames = Application["ServerNames"] == null ? new ServerNames() : ServerNames.Deserialize(Application["ServerNames"].ToString());

                return _serverNames;
            }
            set { Application.SetStoredValue("ServerNames", value.Serialize()); }
        }


        private OfflineUsers _offlineUsers;

        public OfflineUsers OfflineUsers
        {
            get
            {
                if (_offlineUsers == null)
                {
                    try
                    {
                        _offlineUsers = Application["OfflineUsers"] == null ? new OfflineUsers() : OfflineUsers.Deserialize(Application["OfflineUsers"].ToString());
                    }
                    catch (Exception e)
                    {
                        Anchor.Core.Loggers.Logger.LogError(e);
                        _offlineUsers = new OfflineUsers();
                    }

                }

                return _offlineUsers;
            }
            set { Application.SetStoredValue("OfflineUsers", value.Serialize()); }
        }



        #region Users list settings

        public string UsersSortingMethod
        {
            get { return (User == null || User["UsersSortingMethod"] == null) ? "UserName" : User["UsersSortingMethod"].ToString(); }
            set { User.SetStoredValue("UsersSortingMethod", value); }
        }

        #endregion


        #region Aquadots settings

        public string SDEventsSortingMethod
        {
            get { return (User == null || User["SDEventsSortingMethod"] == null) ? "EventName" : User["SDEventsSortingMethod"].ToString(); }
            set { User.SetStoredValue("SDEventsSortingMethod", value); }
        }

        public bool SDEventsSortDecending
        {
            get { return User["SDEventsSortDecending"] == null ? true : User["SDEventsSortDecending"].ToString().ToBool(); }
            set { User.SetStoredValue("SDEventsSortDecending", value); }
        }

        #endregion


        #endregion


        #region User Settings


        public bool IsMapEnabled
        {
            get { return User["IsMapEnabled"] == null ? false : User["IsMapEnabled"].ToString().ToBool(); }
            set { User.SetStoredValue("IsMapEnabled", value); }
        }


        public string MapDisplayMode
        {
            get { return User["MapDisplayMode"] == null ? "Points" : User["MapDisplayMode"].ToString(); }
            set { User.SetStoredValue("MapDisplayMode", value); }
        }


        public double MapWidth
        {
            get { return User["MapWidth"] == null ? 130.0 : User["MapWidth"].ToString().ToDouble(); }
            set { if(!double.IsNaN(value)) User.SetStoredValue("MapWidth", value); }
        }

        public double MapHeight
        {
            get { return User["MapHeight"] == null ? 130.0 : User["MapHeight"].ToString().ToDouble(); }
            set { if (!double.IsNaN(value)) User.SetStoredValue("MapHeight", value); }
        }


        public bool IsSpeciesListRowDetailsCollapsed
        {
            get { return User["IsSpeciesListRowDetailsCollapsed"] == null ? false : User["IsSpeciesListRowDetailsCollapsed"].ToString().ToBool(); }
            set { User.SetStoredValue("IsSpeciesListRowDetailsCollapsed", value); Save(); }
        }



        public bool LavAutoDecrement
        {
            get { return User["LavAutoDecrement"] == null ? false : User["LavAutoDecrement"].ToString().ToBool(); }
            set { User.SetStoredValue("LavAutoDecrement", value); Save(); }
        }


        public double ZoomLevelCruise
        {
            get { return User["ZoomLevelCruise"] == null ? 100 : User["ZoomLevelCruise"].ToString().ToDouble(); }
            set 
            {
                User.SetStoredValue("ZoomLevelCruise", ClampZoomValue(value)); 
                RaisePropertyChanged(() => ZoomLevelCruise); 
            }
        }


        public double ZoomLevelTrip
        {
            get { return User["ZoomLevelTrip"] == null ? 100 : User["ZoomLevelTrip"].ToString().ToDouble(); }
            set
            {
                User.SetStoredValue("ZoomLevelTrip", ClampZoomValue(value));
                RaisePropertyChanged(() => ZoomLevelTrip);
            }
        }


        public double ZoomLevelTripHVN
        {
            get { return User["ZoomLevelTripHVN"] == null ? 100 : User["ZoomLevelTripHVN"].ToString().ToDouble(); }
            set
            {
                User.SetStoredValue("ZoomLevelTripHVN", ClampZoomValue(value));
                RaisePropertyChanged(() => ZoomLevelTripHVN);
            }
        }


        public double ZoomLevelStation
        {
            get { return User["ZoomLevelStation"] == null ? 100 : User["ZoomLevelStation"].ToString().ToDouble(); }
            set
            {
                User.SetStoredValue("ZoomLevelStation", ClampZoomValue(value));
                RaisePropertyChanged(() => ZoomLevelStation);
            }
        }


        public double ZoomLevelSpeciesList
        {
            get { return User["ZoomLevelSpeciesList"] == null ? 100 : User["ZoomLevelSpeciesList"].ToString().ToDouble(); }
            set
            {
                User.SetStoredValue("ZoomLevelSpeciesList", ClampZoomValue(value));
                RaisePropertyChanged(() => ZoomLevelSpeciesList);
            }
        }


        public double ZoomLevelLAV
        {
            get { return User["ZoomLevelLAV"] == null ? 100 : User["ZoomLevelLAV"].ToString().ToDouble(); }
            set
            {
                User.SetStoredValue("ZoomLevelLAV", ClampZoomValue(value));
                RaisePropertyChanged(() => ZoomLevelLAV);
            }
        }

        public double ZoomLevelSF
        {
            get { return User["ZoomLevelSF"] == null ? 100 : User["ZoomLevelSF"].ToString().ToDouble(); }
            set
            {
                User.SetStoredValue("ZoomLevelSF", ClampZoomValue(value));
                RaisePropertyChanged(() => ZoomLevelSF);
            }
        }


        private double ClampZoomValue(double dblValue)
        {
            return Math.Min(Math.Max(40.0, dblValue), 160.0);
        }


        /// <summary>
        /// Path to default folder for exported data
        /// </summary>
        public string ExportFolderPath
        {
            get { return User["ExportFolderPath"] == null ? GetDefaultExportPath() : User["ExportFolderPath"].ToString(); }
            set { User.SetStoredValue("ExportFolderPath", value); }
        }


        /// <summary>
        /// Path to default folder for exported data
        /// </summary>
        public string ExportFilePrefix
        {
            get { return User["ExportFilePrefix"] == null ? null : User["ExportFilePrefix"].ToString(); }
            set { User.SetStoredValue("ExportFilePrefix", value, true); }
        }


       


        public bool OpenReportAutomatically
        {
            get { return User["OpenReportAutomatically"] == null ? true : User["OpenReportAutomatically"].ToString().ToBool(); }
            set { User.SetStoredValue("OpenReportAutomatically", value); Save(); }
        }

        public bool OpenTestReportAutomatically
        {
            get { return User["OpenTestReportAutomatically"] == null ? true : User["OpenTestReportAutomatically"].ToString().ToBool(); }
            set { User.SetStoredValue("OpenTestReportAutomatically", value); Save(); }
        }



        private string GetDefaultExportPath()
        {
            string strPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FishLine");

            try
            {
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);
            }
            catch(Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return strPath;
        }


        /// <summary>
        /// Whether raised data should be saved in 8 files or 3 files. If this is true, data is saved in 8 csv files, one 
        /// for each data warehouse table. If it is false, data is combined into 3 csv files.
        /// </summary>
        public bool SaveRawDWTables
        {
            get { return User["SaveRawDWTables"] == null ? false : User["SaveRawDWTables"].ToString().ToBool(); }
            set { User.SetStoredValue("SaveRawDWTables", value); }
        }


        private DataGridColumnSettingsContainer _dataGridColumnSettingsContainer;

        public DataGridColumnSettingsContainer DataGridColumnSettings
        {
            get
            {
                if (_dataGridColumnSettingsContainer == null)
                {
                    try
                    {
                        _dataGridColumnSettingsContainer = User["DataGridColumnSettings"] == null ? new DataGridColumnSettingsContainer() : DataGridColumnSettingsContainer.Deserialize(User["DataGridColumnSettings"].ToString());
                        _dataGridColumnSettingsContainer.Initialize();
                    }
                    catch (Exception e)
                    {
                        Anchor.Core.Loggers.Logger.LogError(e);
                        _dataGridColumnSettingsContainer = new DataGridColumnSettingsContainer();
                    }

                }

                return _dataGridColumnSettingsContainer;
            }
            set { User.SetStoredValue("DataGridColumnSettings", value.Serialize()); }
        }


        /// <summary>
        /// Path to default folder for reports
        /// </summary>
        public string LocalReportFolderPath
        {
            get { return User["LocalReportFolderPath"] == null ? ExportFolderPath : User["LocalReportFolderPath"].ToString(); }
            set { User.SetStoredValue("LocalReportFolderPath", value); }
        }


        private Dictionary<int, string> _dicLocalReportFolderPaths = null;

        /// <summary>
        /// Dictionary of report ids and corresponding report output folders.
        /// </summary>
        public Dictionary<int, string> LocalReportFolderPaths
        {
            get
            {
                if (_dicLocalReportFolderPaths == null)
                {
                    try
                    {
                        _dicLocalReportFolderPaths = User["LocalReportFolderPaths"] == null ? new Dictionary<int, string>() : SettingsBaseObject<Dictionary<int, string>>.Deserialize(User["LocalReportFolderPaths"].ToString());

                        if (_dicLocalReportFolderPaths == null)
                            _dicLocalReportFolderPaths = new Dictionary<int, string>();
                    }
                    catch (Exception e)
                    {
                        Anchor.Core.Loggers.Logger.LogError(e);
                        _dicLocalReportFolderPaths = new Dictionary<int, string>();
                    }

                }

                return _dicLocalReportFolderPaths;
            }
            set
            {
                _dicLocalReportFolderPaths = value;
                User.SetStoredValue("DataGridColumnSettings", SettingsBaseObject<Dictionary<int, string>>.Serialize(value, new Type[] {typeof(Dictionary<int, string>)}));
            }
        }

        public string GetReportFolderPath(int intReportId)
        {
            string res = null;

            try
            {
                var dic = LocalReportFolderPaths;

                if (dic == null || !dic.ContainsKey(intReportId))
                    return null;

                res = dic[intReportId];
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return res;
        }


        public void SetReportFolderPath(int intReportId, string path, bool persistToDisk = true)
        {
            try
            {
                var dic = LocalReportFolderPaths;

                if (dic == null)
                    return;

                if (dic.ContainsKey(intReportId))
                {
                    if (path == null)
                        dic.Remove(intReportId);
                    else
                        dic[intReportId] = path;
                }
                else
                {
                    if(path != null)
                        dic.Add(intReportId, path);
                }

                //Persist report settings to disk
                if (persistToDisk)
                    LocalReportFolderPaths = dic;
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }



        #endregion


        #endregion


        public string Language
        {
            get
            {
                string strLan = "";
                if (Application["Language"] == null)
                {
                    strLan = CultureInfo.CurrentCulture.Name;

                    try
                    {
                        var t = Anchor.Core.Language.TranslaterFactory.Instance.GetTranslater(strLan);
                    }
                    catch
                    {
                        strLan = "da-DK";
                    }
                }
                else
                    strLan = Application["Language"].ToString();

                return strLan;
            }
            set 
            { 
                Application.SetStoredValue("Language", value); 
            }
        }


        public string LanguageFolder
        {
            get { return Path.Combine(_strUserRootPath, "Xml\\Languages"); }
        }


        public void CopyResourceToAppDataIfDifferent(string strManifestURI, string strRelativeDestinationPath)
        {
            try
            {
               var names =  Assembly.GetEntryAssembly().GetManifestResourceNames();
                //"VDEC.WPF.Xml.Trip.xslt"
                var resStream = Assembly.GetEntryAssembly().GetManifestResourceStream(strManifestURI);

                if (resStream == null)
                    return;

                string strDirPath = System.IO.Path.Combine(_strUserRootPath, System.IO.Path.GetDirectoryName(strRelativeDestinationPath));

                if (!System.IO.Directory.Exists(strDirPath))
                    System.IO.Directory.CreateDirectory(strDirPath);

                string strFilePath = System.IO.Path.Combine(_strUserRootPath, strRelativeDestinationPath);

                //If file exists and has the same size, do not copy it over again.
                if (System.IO.File.Exists(strFilePath))
                {
                    var fi = new System.IO.FileInfo(strFilePath);

                    if (fi.Length == resStream.Length)
                        return;
                }

                byte[] b;

                using (System.IO.BinaryReader br = new System.IO.BinaryReader(resStream))
                {
                    b = br.ReadBytes((int)resStream.Length);
                }

                System.IO.File.WriteAllBytes(strFilePath, b);
                resStream.Dispose();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

        }
    }
}
