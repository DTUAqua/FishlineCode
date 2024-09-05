using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anchor.Core;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.IO;
using Ionic.Zip;

namespace Babelfisk.ViewModels.Security
{
    public class BackupViewModel : AViewModel
    {
        private DelegateCommand _cmdOK;
        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdBrowseBackupDirectory;
        private DelegateCommand _cmdManualBackupData;
        private DelegateCommand _cmdRestoreBackup;

        private string _strBackupDirectory;

        private bool _blnIsAutomaticBackupEnabled;

        private int _intNoOfBackupsPerDay;

        private int _intNoOfBackupDays;

      

        #region Properties


        public bool IsAutomaticBackupEnabled
        {
            get { return _blnIsAutomaticBackupEnabled; }
            set
            {
                _blnIsAutomaticBackupEnabled = value;
                RaisePropertyChanged(() => IsAutomaticBackupEnabled);
            }
        }

        public int NoOfBackupsPerDay
        {
            get { return _intNoOfBackupsPerDay; }
            set
            {
                _intNoOfBackupsPerDay = value;
                RaisePropertyChanged(() => NoOfBackupsPerDay);
            }
        }


        public int NoOfBackupDays
        {
            get { return _intNoOfBackupDays; }
            set
            {
                _intNoOfBackupDays = value;
                RaisePropertyChanged(() => NoOfBackupDays);
            }
        }


        public string BackupDirectory
        {
            get { return _strBackupDirectory; }
            set
            {
                _strBackupDirectory = value;
                RaisePropertyChanged(() => BackupDirectory);
            }
        }


        #endregion



        public BackupViewModel()
        {
            _strBackupDirectory = BusinessLogic.Settings.Settings.Instance.BackupDirectoryPath;
            _blnIsAutomaticBackupEnabled = BusinessLogic.Settings.Settings.Instance.IsAutomaticBackupEnabled;
            _intNoOfBackupDays = BusinessLogic.Settings.Settings.Instance.NoOfBackupDays;
            _intNoOfBackupsPerDay = BusinessLogic.Settings.Settings.Instance.NoOfBackupsPerDay;

            WindowWidth = 700;
            WindowHeight = 450;
            WindowTitle = "Backupindstillinger";

            IsDirty = false;
        }




        #region OK Command


        public DelegateCommand OKCommand
        {
            get { return _cmdOK ?? (_cmdOK = new DelegateCommand(SaveAndClose)); }
        }


        private void SaveAndClose()
        {
            if (_strBackupDirectory != BusinessLogic.Settings.Settings.Instance.BackupDirectoryPath)
            {
                BusinessLogic.Settings.Settings.Instance.BackupDirectoryPath = _strBackupDirectory;
                IsDirty = true;
            }

            if (_intNoOfBackupDays != BusinessLogic.Settings.Settings.Instance.NoOfBackupDays)
            {
                BusinessLogic.Settings.Settings.Instance.NoOfBackupDays = _intNoOfBackupDays;
                IsDirty = true;
            }

            if (_intNoOfBackupsPerDay != BusinessLogic.Settings.Settings.Instance.NoOfBackupsPerDay)
            {
                BusinessLogic.Settings.Settings.Instance.NoOfBackupsPerDay = _intNoOfBackupsPerDay;
                IsDirty = true;
            }

            if (_blnIsAutomaticBackupEnabled != BusinessLogic.Settings.Settings.Instance.IsAutomaticBackupEnabled)
            {
                BusinessLogic.Settings.Settings.Instance.IsAutomaticBackupEnabled = _blnIsAutomaticBackupEnabled;
                IsDirty = true;
            }

            //Save settings, if anything has changed.
            if (IsDirty)
                BusinessLogic.Settings.Settings.Instance.Save();

            if (_blnIsAutomaticBackupEnabled)
            {
                //Backup data straight away.
                BackupManager.Instance.Backup();
            }
            else
            {
                //MAke sure backup manager is not running
                BackupManager.Instance.Stop();
            }

            Close();
        }


        #endregion


        #region Cancel Command


        public DelegateCommand CancelCommand
        {
            get { return _cmdCancel ?? (_cmdCancel = new DelegateCommand(Cancel)); }
        }


        private void Cancel()
        {
            Close();
        }


        #endregion


        #region Browse Backup Directory Command


        public DelegateCommand BrowseBackupDirectoryCommand
        {
            get { return _cmdBrowseBackupDirectory ?? (_cmdBrowseBackupDirectory = new DelegateCommand(BrowseBackupDirectory)); }
        }


        private void BrowseBackupDirectory()
        {
            var fd = new System.Windows.Forms.FolderBrowserDialog();
            fd.SelectedPath = _strBackupDirectory;

            var res = fd.ShowDialog();

            if (res == System.Windows.Forms.DialogResult.OK)
            {
                BackupDirectory = fd.SelectedPath;
            }
            
        }


        #endregion


        #region Manual Backup Command


        public DelegateCommand ManualBackupCommand
        {
            get { return _cmdManualBackupData ?? (_cmdManualBackupData = new DelegateCommand(() => ManualBackupDataAsync())); }
        }


        public void ManualBackupDataAsync()
        {
            if (!BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "FishLine Backup Files (*.fbz)|*.fbz";
            sfd.DefaultExt = "fbz";
            sfd.Title = "Vælg destination og filnavn for sikkerhedskopien.";

            if (sfd.ShowDialog() == true)
            {
                string strFileName = sfd.FileName;

                AppRegionManager.ShowLoadingWindow("Kopierer data, vent venligst...", true);
                Task.Factory.StartNew(() => ManualBackupData(strFileName)).ContinueWith(t => new Action(() => AppRegionManager.HideLoadingWindow()).Dispatch());
            }
        }


        private void ManualBackupData(string strFileName)
        {
            try
            {
                //If file already exists, delete it.
                if (File.Exists(strFileName))
                    File.Delete(strFileName);

                using (ZipFile zip = new ZipFile())
                {
                    zip.AddDirectory(BusinessLogic.Settings.Settings.Instance.OfflineFolderPath);
                    zip.Comment = "FishLine offline data backup. Created " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + ".";
                    zip.Save(strFileName);
                }
            }
            catch (Exception e)
            {
                DispatchMessageBox("En uventet fejl opstod. " + e.Message);
                return;
            }

            DispatchMessageBox("Sikkerhedskopien blev gemt korrekt.", 3);
        }


        #endregion


        #region Restore Backup Command


        public DelegateCommand RestoreBackupCommand
        {
            get { return _cmdRestoreBackup ?? (_cmdRestoreBackup = new DelegateCommand(() => RestoreBackup())); }
        }


        public void RestoreBackup()
        {
            if (!BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline)
                return;

            var vm = new RestoreBackupViewModel(_strBackupDirectory);

            AppRegionManager.LoadWindowViewFromViewModel(vm, false, "WindowWithBorderStyle");
        }

        #endregion

    }
}
