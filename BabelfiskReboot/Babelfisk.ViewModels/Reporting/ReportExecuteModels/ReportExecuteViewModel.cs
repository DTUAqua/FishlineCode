using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Anchor.Core;

namespace Babelfisk.ViewModels.Reporting.ReportExecuteModels
{
    public class ReportExecuteViewModel : AViewModel
    {
        private DelegateCommand _cmdBrowse;
        private DelegateCommand _cmdOpenExplorer;
        private DelegateCommand<string> _cmdLoadReport;
        private DelegateCommand _cmdResetPath;
        private DelegateCommand _cmdSavePathForAllReports;

        protected ReportViewModel _vmReport;

        private string _strLocalReportFolderPath;
       

        #region Properties


        public string LocalReportFolderPath
        {
            get { return _strLocalReportFolderPath; }
            set
            {
                _strLocalReportFolderPath = value;
                RaisePropertyChanged(() => LocalReportFolderPath);
                RaisePropertyChanged(() => HasLocalReportFolderPath);
            }
        }


        public bool HasLocalReportFolderPath
        {
            get { return !string.IsNullOrWhiteSpace(_strLocalReportFolderPath); }
        }


        public ReportViewModel Report
        {
            get { return _vmReport; }
        }


        #endregion



        public ReportExecuteViewModel(ReportViewModel vmReport)
        {
            _vmReport = vmReport;
            AssignLocalReportPath();
        }


        private void AssignLocalReportPath()
        {
            try
            {
                var path = BusinessLogic.Settings.Settings.Instance.GetReportFolderPath(_vmReport.ReportEntity.reportId);

                if (!string.IsNullOrWhiteSpace(path))
                {
                    //Do nothing, use existing path.
                    //path = path;
                }
                else if (!string.IsNullOrWhiteSpace(_vmReport.ReportEntity.outputPath))
                    path = _vmReport.ReportEntity.outputPath;
                else
                    path = BusinessLogic.Settings.Settings.Instance.LocalReportFolderPath;

                if (path != null && path.Contains(ReportViewModel.UserNameTag, StringComparison.InvariantCultureIgnoreCase))
                    path = path.Replace(ReportViewModel.UserNameTag, BusinessLogic.SecurityManager.CurrentUser.UserName);

                LocalReportFolderPath = path;
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }


        protected void SavePathAsDefaultForCurrentReport()
        {
            try
            {
                var outPath = _vmReport.ReportEntity.outputPath;
                //Add any user name to output path
                if (outPath != null && outPath.Contains(ReportViewModel.UserNameTag, StringComparison.InvariantCultureIgnoreCase))
                    outPath = outPath.Replace(ReportViewModel.UserNameTag, BusinessLogic.SecurityManager.CurrentUser.UserName);

                if (string.IsNullOrWhiteSpace(outPath))
                    outPath = BusinessLogic.Settings.Settings.Instance.LocalReportFolderPath;

                if(LocalReportFolderPath == null || !outPath.Equals(LocalReportFolderPath))
                    BusinessLogic.Settings.Settings.Instance.SetReportFolderPath(_vmReport.ReportEntity.reportId, LocalReportFolderPath, true);
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }


        protected void SavePathAsDefaultForAllReports()
        {
            try
            {
                BusinessLogic.Settings.Settings.Instance.LocalReportFolderPath = LocalReportFolderPath;
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }


        public bool HasLoadPermission(ref string strMessage)
        {
            try
            {
                if (User == null)
                {
                    strMessage = "Du skal være logged ind for at hente rapporter.";
                    return false;
                }

                if (!User.HasTask(Entities.SecurityTask.ViewReports))
                {
                    strMessage = "Du har ikke rettigheder til at hente rapporter.";
                    return false;
                }

                if (Report != null && Report.ReportEntity.Permissions.Count > 0)
                {
                    foreach (var p in Report.ReportEntity.Permissions)
                    {
                        if (!User.HasTask(p))
                        {
                            strMessage = "Du har ikke rettigheder til at hente rapporten.";

                            //Custom message for specific permissions
                            switch (p)
                            {
                                case Entities.SecurityTask.ViewDFADReports:
                                    strMessage = "Du har ikke rettigheder til at hente rapporter som indeholder DFAD data.";
                                    break;
                            }
                            return false;
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

            return true;
        }


        #region Browse Command


        public DelegateCommand BrowseCommand
        {
            get { return _cmdBrowse ?? (_cmdBrowse = new DelegateCommand(Browse)); }
        }


        private void Browse()
        {
            FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

            dlg.SelectedPath = LocalReportFolderPath;
            dlg.Description = "Vælg en destination til rapporten.";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LocalReportFolderPath = dlg.SelectedPath;
            }
        }


        #endregion


        #region Open Explorer Command


        public DelegateCommand OpenExplorerCommand
        {
            get { return _cmdOpenExplorer ?? (_cmdOpenExplorer = new DelegateCommand(OpenExplorer)); }
        }


        private void OpenExplorer()
        {
            if (!HasLocalReportFolderPath || !Directory.Exists(LocalReportFolderPath))
            {
                AppRegionManager.ShowMessageBox("Den valgte sti (mappe) eksisterer ikke, ret venligst stien og prøv igen.");
                return;
            }

            try
            {
                Process.Start(LocalReportFolderPath);
            }
            catch { }
        }



        #endregion


        #region Load Report Command


        public virtual DelegateCommand<string> LoadReportCommand
        {
            get { return _cmdLoadReport ?? (_cmdLoadReport = new DelegateCommand<string>(LoadReportInternal)); }
        }


        private void LoadReportInternal(string strParam)
        {
            string strErrorTxt = null;
            if (!HasLoadPermission(ref strErrorTxt))
            {
                if (strErrorTxt != null)
                    AppRegionManager.ShowMessageBox(strErrorTxt);
                return;
            }

            string strOutputPathRestriction = _vmReport.ReportEntity.outputPathRestriction;

            if (strOutputPathRestriction != null)
                strOutputPathRestriction = strOutputPathRestriction.Replace(ReportViewModel.UserNameTag, BusinessLogic.SecurityManager.CurrentUser.UserName);

            //If there is a restriction on output path, make sure that LocalReportFolderPath complies to the restriction.
            if (!string.IsNullOrWhiteSpace(_vmReport.ReportEntity.outputPath) && 
                !string.IsNullOrWhiteSpace(_vmReport.ReportEntity.outputPathRestriction) && 
                !string.IsNullOrWhiteSpace(LocalReportFolderPath) &&
                !LocalReportFolderPath.StartsWith(strOutputPathRestriction, StringComparison.InvariantCultureIgnoreCase))
            {
                AppRegionManager.ShowMessageBox(string.Format("Det er kun muligt at gemme rapporten på stier som starter med '{0}'. Angiv venligst en ny sti hvor rapporten skal gemmes og prøv igen.", strOutputPathRestriction), System.Windows.MessageBoxButton.OK);
                return;
            }

            LoadReport(strParam);
        }


        protected virtual void LoadReport(string strParam)
        {
        }


        #endregion


        #region Reset Path Command


        public virtual DelegateCommand ResetPathCommand
        {
            get { return _cmdResetPath ?? (_cmdResetPath = new DelegateCommand(ResetPath)); }
        }


        protected virtual void ResetPath()
        {
            if (AppRegionManager.ShowMessageBox("Er du sikker på du vil nulstille stien til hvor rapporten gemmes til standard-stien?", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;

            //Set any manual path to null, so default one is loaded.
            BusinessLogic.Settings.Settings.Instance.SetReportFolderPath(_vmReport.ReportEntity.reportId, null, true);

            AssignLocalReportPath();
        }


        #endregion


         #region Save Path For All Reports Command


        public virtual DelegateCommand SavePathForAllReportsCommand
        {
            get { return _cmdSavePathForAllReports ?? (_cmdSavePathForAllReports = new DelegateCommand(SavePathForAllReports)); }
        }


        protected virtual void SavePathForAllReports()
        {
            if (AppRegionManager.ShowMessageBox("Er du sikker på du vil sætte den angivne sti som standard for alle rapporter (der ikke har en specifik/individuel sti angivet i forvejen)?", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;

            this.SavePathAsDefaultForAllReports();
        }


        #endregion

        
        

    }
}
