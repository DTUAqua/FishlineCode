using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;
using Anchor.Core;
using System.Threading.Tasks;
using Microsoft.Win32;
using Babelfisk.BusinessLogic.Offline;

namespace Babelfisk.ViewModels.Security
{
    public class RestoreBackupViewModel : AViewModel
    {
        private DelegateCommand _cmdOK;
        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdBrowse;

        private string _strBackupDirectory;

        private List<BackupFile> _lstBackupFiles;

        private bool _blnRestoreFromAutomaticBackupsList;

        private BackupFile _selectedBackupFile;

        private string _strSelectedBackupPath;


        #region Properties


        public List<BackupFile> BackupFiles
        {
            get { return _lstBackupFiles; }
            set
            {
                _lstBackupFiles = value;
                RaisePropertyChanged(() => BackupFiles);
                RaisePropertyChanged(() => HasBackupFiles);
            }
        }


        public BackupFile SelectedBackupFile
        {
            get { return _selectedBackupFile; }
            set
            {
                _selectedBackupFile = value;

               /* if (_selectedBackupFile != null)
                    SelectedBackupFilePath = _selectedBackupFile.FilePath;
                else
                    SelectedBackupFilePath = null;*/

                RaisePropertyChanged(() => SelectedBackupFile);
            }
        }


        public string SelectedBackupFilePath
        {
            get { return _strSelectedBackupPath; }
            set
            {
                _strSelectedBackupPath = value;
                RaisePropertyChanged(() => SelectedBackupFilePath);
            }
        }



        public bool HasBackupFiles
        {
            get { return _lstBackupFiles != null && _lstBackupFiles.Count > 0; }
        }


        public bool RestoreFromAutomaticBackupsList
        {
            get { return _blnRestoreFromAutomaticBackupsList; }
            set
            {
                _blnRestoreFromAutomaticBackupsList = value;
                RaisePropertyChanged(() => RestoreFromAutomaticBackupsList);
            }
        }


        #endregion



        public RestoreBackupViewModel(string strBackupDirectory)
        {
            _strBackupDirectory = strBackupDirectory;

            WindowWidth = 650;
            WindowHeight = 400;
            WindowTitle = "Gendan en tidligere backup";

            _blnRestoreFromAutomaticBackupsList = true;
            InitializeAsync();
        }


        public Task InitializeAsync()
        {
            IsLoading = true;

            return Task.Factory.StartNew(Initialize).ContinueWith(t => new Action(() => { IsLoading = false; }).Dispatch());
        }


        private void Initialize()
        {
            try
            {
                List<BackupFile> lst = BackupManager.Instance.GetBackupFiles();

                if (lst != null && lst.Count > 0)
                {
                    Dictionary<string, string> groupMap = new Dictionary<string, string>();

                    foreach (var g in lst.GroupBy(x => x.Group))
                    {
                        string strGroup = g.Key;

                        var fromDate = g.Min(x => x.BackupDateLocal).ToString("dd-MM-yyyy");
                        var toDate = g.Max(x => x.BackupDateLocal).ToString("dd-MM-yyyy"); ;

                        if (fromDate == toDate)
                            strGroup = "Backups taget d. " + fromDate;
                        else
                            strGroup = string.Format("Backups taget fra d. {0} til d. {1}", fromDate, toDate);

                        foreach (var itm in g)
                            itm.Tag = strGroup;
                    }

                    string strCurrent = string.Format("OfflineData_{0}", DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss"));
                    lst = lst.OrderByDescending(x => x.Group == "Current" ? strCurrent : x.Group).ThenByDescending(x => x.FilePath).ToList();
                }


                new Action(() =>
                {
                    BackupFiles = lst;
                }).Dispatch();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                DispatchMessageBox("En uventet fejl opstod. " + e.Message);
            }
        }


        #region OK Command


        public DelegateCommand OKCommand
        {
            get { return _cmdOK ?? (_cmdOK = new DelegateCommand(RestoreAndClose)); }
        }


        private void Validate()
        {
            Error = null;
            if (RestoreFromAutomaticBackupsList)
            {
                if (SelectedBackupFile == null)
                    Error = "Vælg venligst en backup fra listen, inden du gendanner.";
                else if (SelectedBackupFile != null && (string.IsNullOrWhiteSpace(SelectedBackupFile.FilePath) || !System.IO.File.Exists(SelectedBackupFile.FilePath)))
                    Error = "Filen for den valgte backup i listen eksisterer ikke.";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(_strSelectedBackupPath))
                    Error = "Angiv eller vælg venligst en backup-fil.";
                else if (!System.IO.File.Exists(_strSelectedBackupPath))
                    Error = "Den angivne fil eksisterer ikke, ret venligst stien eller klik 'Browse' for at vælge en ny fil.";
            }
        }

        private void RestoreAndClose()
        {
            Validate();

            if (HasErrors)
            {
                AppRegionManager.ShowMessageBox(Error);
                return;
            }

            if (AppRegionManager.ShowMessageBox("Er du sikker på du vil fortsætte med at gendanne den valgte backup? OBS! Alt nuværende offline data vil blive overskrevet med data fra den valgte backup.", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;

            string strFilePath = null;
            if (RestoreFromAutomaticBackupsList)
                strFilePath = SelectedBackupFile.FilePath;
            else
                strFilePath = SelectedBackupFilePath;

            //Perform restore and close window on success.
            Restore(strFilePath).ContinueWith(t => 
            {
                if (t.Result)
                {
                    new Action(() =>
                    {
                        Close();

                        OfflineDictionary.Instance.ReloadDictionaryFromDisk();
                        MainTree.InitializeTreeViewAsync();

                        DispatchMessageBox("Den valgte backup blev korrekt gendannet.");
                    }).Dispatch();
                }
                else
                {
                    DispatchMessageBox("En uventet fejl opstod - det var ikke muligt at gendanne den valgte backup.");
                }
            });
        }

        private Task<bool> Restore(string strFilePath)
        {
            return Task.Factory.StartNew(() =>
            {
                return BackupManager.Instance.RestoreBackup(strFilePath);
            });
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


        #region BrowseCommand


        public DelegateCommand BrowseCommand
        {
            get { return _cmdBrowse ?? (_cmdBrowse = new DelegateCommand(() => Browse())); }
        }


        public void Browse()
        {
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.Filter = "FishLine Backup Files (*.fbz)|*.fbz";
            sfd.DefaultExt = "fbz";
            sfd.Title = "Vælg en sikkerhedskopi.";

            if (sfd.ShowDialog() == true)
            {
                string strFileName = sfd.FileName;

                SelectedBackupFilePath = strFileName;

                ScrollTo("SelectedPathEnd");
            }
        }


        #endregion
    }
}
