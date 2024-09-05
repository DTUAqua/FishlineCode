using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Globalization;
using Ionic.Zip;
using System.Threading.Tasks;

namespace Babelfisk.ViewModels.Security
{
    public class BackupManager
    {
        public static readonly BackupManager Instance = new BackupManager();

        private volatile bool _blnIsRunning = false;

        private readonly object _threadLock = new object();

        private AutoResetEvent _waitHandle = new AutoResetEvent(false);

        private Thread _t = null;


        private BackupManager()
        {
        }


        public void Backup(bool blnOnlyIfOffline = true)
        {
            try
            {
                if (!BusinessLogic.Settings.Settings.Instance.IsAutomaticBackupEnabled || !BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
                    return;

                bool blnRunning = false;
                lock (_threadLock)
                {
                    blnRunning = _blnIsRunning;
                }

                if (!blnRunning)
                    Start();
                else
                    _waitHandle.Set();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }


        private void Start()
        {
            try
            {
                lock (_threadLock)
                {
                    if (_blnIsRunning)
                        return;

                    _blnIsRunning = true;
                }

                _waitHandle.Reset();
                _t = new Thread(Run) { IsBackground = true };
                _t.Priority = ThreadPriority.Lowest;
                _t.Start();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }


        public void Stop()
        {
            try
            {
                lock (_threadLock)
                {
                    if (!_blnIsRunning)
                        return;

                    _blnIsRunning = false;
                }

                _waitHandle.Set();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }


        public bool RestoreBackup(string strBackupFilePath)
        {
            if (!File.Exists(strBackupFilePath))
                return false;

            string strTempFolder = string.Format("{0}_{1}", BusinessLogic.Settings.Settings.Instance.OfflineFolderPath, "old");

            //1) Move current offline data folder, to _old.
            try
            {
                Directory.Move(BusinessLogic.Settings.Settings.Instance.OfflineFolderPath, strTempFolder);
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                return false;
            }
            
            //2) Restore file.
            try
            {
                using (ZipFile zip = new ZipFile(strBackupFilePath))
                {
                    zip.ExtractAll(BusinessLogic.Settings.Settings.Instance.OfflineFolderPath, ExtractExistingFileAction.OverwriteSilently);
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);

                //Restore temp folder
                try
                {
                    Directory.Move(strTempFolder, BusinessLogic.Settings.Settings.Instance.OfflineFolderPath);
                }
                catch { }
                return false;
            }

            //3) Delete temp folder
            try
            {
                if (Directory.Exists(strTempFolder))
                    Directory.Delete(strTempFolder, true);
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return true;
        }

        private void Run()
        {
            while (_blnIsRunning)
            {
                try
                {
                    //Fetch generel backup directory from settings, in case user has changed it.
                    string strBackupDirectory = Path.Combine(BusinessLogic.Settings.Settings.Instance.BackupDirectoryPath, "Current");
                    string strFile = Path.Combine(BusinessLogic.Settings.Settings.Instance.BackupDirectoryPath, "currentBackup.fbz");

                    //Make sure general backup directory exists.
                    if (!Directory.Exists(BusinessLogic.Settings.Settings.Instance.BackupDirectoryPath))
                        Directory.CreateDirectory(BusinessLogic.Settings.Settings.Instance.BackupDirectoryPath);

                    //Make sure Current backup directory exists
                    if (!Directory.Exists(strBackupDirectory))
                        Directory.CreateDirectory(strBackupDirectory);

                    //Zip backup
                    using (ZipFile zip = new ZipFile())
                    {
                        zip.AddDirectory(BusinessLogic.Settings.Settings.Instance.OfflineFolderPath);
                        zip.Comment = string.Format("FishLine offline data backup. Created {0}.", System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                        zip.Save(strFile);
                    }

                    //Move backup to Current folder and truncate previous backups in Current folder.
                    BackupFile(strBackupDirectory, strFile);
                }
                catch (Exception e)
                {
                    Anchor.Core.Loggers.Logger.LogError(e);
                }

                try
                {
                    //Wait until next backup cycle.
                    _waitHandle.WaitOne();
                }
                catch { }
            }
        }



        public void EndCurrentBackupSession(DateTime dtUTCEndTime)
        {
            try
            {
                string strBackupDirectory = Path.Combine(BusinessLogic.Settings.Settings.Instance.BackupDirectoryPath, "Current");

                if (!Directory.Exists(strBackupDirectory))
                    return;

                var dir = Directory.GetDirectories(BusinessLogic.Settings.Settings.Instance.BackupDirectoryPath);
                var lst = dir.Where(x => !Path.GetFileNameWithoutExtension(x).Equals("Current", StringComparison.InvariantCultureIgnoreCase))
                             .Select(x => new { DiretoryPath = x, arr = Path.GetFileNameWithoutExtension(x).Split('_') }).Where(x => x.arr.Length > 1)
                             .ToList();

                List<KeyValuePair<string, DateTime>> lstExistingBackups = new List<KeyValuePair<string, DateTime>>();

                int intNumberOfBackupTrips = 3;
                if (lst.Count >= intNumberOfBackupTrips)
                {
                    foreach (var d in lst)
                    {
                        DateTime dt;

                        if (d.arr.Length != 2 || !DateTime.TryParseExact(d.arr[1], "yyyy-MM-dd-HH-mm-ss", null, DateTimeStyles.AssumeUniversal, out dt))
                            continue;

                        lstExistingBackups.Add(new KeyValuePair<string, DateTime>(d.DiretoryPath, dt));
                    }

                    lstExistingBackups = lstExistingBackups.OrderBy(x => x.Value).ToList();
                    int intOldBackupsCount = lstExistingBackups.Count;

                    foreach (var itm in lstExistingBackups)
                    {
                        if (intOldBackupsCount-- < intNumberOfBackupTrips)
                            break;

                        try
                        {
                            if (Directory.Exists(itm.Key))
                                Directory.Delete(itm.Key, true);
                        }
                        catch(Exception e) 
                        {
                            Anchor.Core.Loggers.Logger.LogError(e);
                        }
                    }
                }


                Directory.Move(strBackupDirectory, Path.Combine(BusinessLogic.Settings.Settings.Instance.BackupDirectoryPath, string.Format("OfflineData_{0}", dtUTCEndTime.ToString("yyyy-MM-dd-HH-mm-ss"))));
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }



        private void BackupFile(string strBackupDirectoryPath, string strFileToBackupPath)
        {
            if (!File.Exists(strFileToBackupPath))
                return;

            var files = Directory.GetFiles(strBackupDirectoryPath);

            string strFileNameWithExt = Path.GetFileName(strFileToBackupPath);
            string strFileName = Path.GetFileNameWithoutExtension(strFileToBackupPath);
            string strExt = Path.GetExtension(strFileToBackupPath);

            var lst = files.Select(x => new { FilePath = x, arr = Path.GetFileNameWithoutExtension(x).Split('_') }).Where(x => x.arr.Length > 1).ToList();

            int intNoOfBackupsPerDay = BusinessLogic.Settings.Settings.Instance.NoOfBackupsPerDay; // 10;
            int intNoOfBackupDays = BusinessLogic.Settings.Settings.Instance.NoOfBackupDays; // 30;
            string strToday = null;

            //Save x backup files.
            if (lst.Count >= intNoOfBackupsPerDay || lst.Capacity >= intNoOfBackupDays)
            {
                List<KeyValuePair<string, DateTime>> lstExistingBackups = new List<KeyValuePair<string, DateTime>>();

                //Parse dates of file backups.
                foreach (var f in lst)
                {
                    DateTime dt;
                    if (!DateTime.TryParseExact(f.arr[1], "yyyy-MM-dd-HH-mm-ss", null, DateTimeStyles.AssumeUniversal, out dt))
                        continue;

                    lstExistingBackups.Add(new KeyValuePair<string, DateTime>(f.FilePath, dt));
                }

                lstExistingBackups = lstExistingBackups.OrderBy(x => x.Value).ToList();

                strToday = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                var lstOldBackupsToday = lstExistingBackups.Where(x => x.Value.ToString("yyyy-MM-dd") == strToday).ToList();

                int intOldBackupTodayCount = lstOldBackupsToday.Count;
                //Cycle backups from today
                foreach (var itm in lstOldBackupsToday)
                {
                    if (intOldBackupTodayCount-- < intNoOfBackupsPerDay)
                        break;

                    if (File.Exists(itm.Key))
                        File.Delete(itm.Key);

                    lstExistingBackups.Remove(itm);
                }

                //Truncate backup from same day, so only one exist
                var lstOldBackupsGrouped = lstExistingBackups.Where(x => x.Value.ToString("yyyy-MM-dd") != strToday).OrderBy(x => x.Value).GroupBy(x => x.Value.ToString("yyyy-MM-dd")).ToList();

                foreach (var group in lstOldBackupsGrouped)
                {
                    int intItemsCount = group.Count();
                    if (intItemsCount < 2)
                        continue;

                    int i = 0;
                    foreach (var itm in group)
                    {
                        //Leave one backup per day.
                        if (i++ >= intItemsCount - 1)
                            break;

                        //Delete
                        if (File.Exists(itm.Key))
                            File.Delete(itm.Key);

                        lstExistingBackups.Remove(itm);
                    }
                }

                //Truncate backups from other days, so only conf.NoOfBackupdDays exist.
                var lstOldBackups = lstExistingBackups.Where(x => x.Value.ToString("yyyy-MM-dd") != strToday).OrderBy(x => x.Value).ToList();
                int intOldBackupsCount = lstOldBackups.Count;
                foreach (var itm in lstOldBackups)
                {
                    if (intOldBackupsCount-- <= intNoOfBackupDays)
                        break;

                    if (File.Exists(itm.Key))
                        File.Delete(itm.Key);

                    lstExistingBackups.Remove(itm);
                }
            }

            string strBackupFileName = string.Format("{0}_{1}{2}", "Backup", DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss"), strExt);
            string strBackupFilePath = Path.Combine(strBackupDirectoryPath, strBackupFileName);


            if (File.Exists(strBackupFilePath))
                File.Delete(strBackupFilePath);

            File.Move(strFileToBackupPath, strBackupFilePath);
        }


        /// <summary>
        /// Retrieve a list of all backed up files asynchronously.
        /// </summary>
        public Task<List<BackupFile>> GetBackupFilesAsync()
        {
            return Task.Factory.StartNew(() => GetBackupFiles());
        }
        

        /// <summary>
        /// Retrieve a list of all backuped up files.
        /// </summary>
        public List<BackupFile> GetBackupFiles()
        {
            List<BackupFile> _lstBackups = new List<BackupFile>();

            try
            {
                var files = Directory.GetFiles(BusinessLogic.Settings.Settings.Instance.BackupDirectoryPath, "*.fbz", SearchOption.AllDirectories);

                foreach (var f in files)
                {
                    string strFileName = System.IO.Path.GetFileNameWithoutExtension(f);

                    var arr = strFileName.Split('_');

                    if (arr.Length != 2)
                        continue;

                    DateTime dt;
                    if (!DateTime.TryParseExact(arr[1], "yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dt))
                        continue; //Ignore file if file name does not match normal pattern.

                    var directoryPath = Path.GetDirectoryName(f);
                    var directoryName = Path.GetFileNameWithoutExtension(directoryPath);

                    _lstBackups.Add(new BackupFile(f, dt, directoryName));
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return _lstBackups;
        }
    }
}
