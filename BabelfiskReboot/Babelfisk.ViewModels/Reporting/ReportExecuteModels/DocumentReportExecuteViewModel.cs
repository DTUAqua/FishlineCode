using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anchor.Core;
using Babelfisk.BusinessLogic.Reporting;

namespace Babelfisk.ViewModels.Reporting.ReportExecuteModels
{
    public class DocumentReportExecuteViewModel : ReportExecuteViewModel
    {
        public DocumentReportExecuteViewModel(ReportViewModel vmReport)
            : base(vmReport)
        {
        }




        protected override void LoadReport(string strParam)
        {
            if (!HasLocalReportFolderPath || !Directory.Exists(LocalReportFolderPath))
            {
                AppRegionManager.ShowMessageBox("Mappen hvor rapporten skulle gemmes kunne ikke lokaliseres, ret venligst stien og prøv igen.");
                return;
            }

            if (strParam == null)
                return;

            IsLoading = true;
            Task.Factory.StartNew(() => Load(_vmReport.ReportEntity.reportId, strParam)).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }



        private void Load(int intReportId, string strParam)
        {
            try
            {
                var repMan = new ReportingManager();

                var files = repMan.GetReportFiles(intReportId);

                if (_vmReport.IsDisposed)
                    return;

                if (files != null && files.Count > 0)
                {
                    string strDirectory = LocalReportFolderPath;

                    foreach (var file in files)
                    {
                        string strPath = Path.Combine(strDirectory, file.FileName);
                        if (File.Exists(strPath))
                        {
                            bool blnAbort = false;

                            new Action(() =>
                            {
                                if (AppRegionManager.ShowMessageBox(string.Format("Dokumentet '{0}' eksisterer allerede i mappen, ønsker du at overskrive dokumentet?", file.FileName), System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.Cancel)
                                    blnAbort = true;
                            }).DispatchInvoke();

                            if (blnAbort)
                                continue;

                            try
                            {
                                File.Delete(strPath);
                            }
                            catch
                            {
                                string strReportName = Path.GetFileNameWithoutExtension(strPath);
                                string strExt = Path.GetExtension(strPath);
                                strPath = System.IO.Path.Combine(strDirectory, strReportName + "-" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") + strExt);

                                DispatchMessageBox(string.Format("Det var ikke muligt at overskrive dokumentet '{0}', dokumentet gemmes i stedet som: '{1}'.", file.FileName, Path.GetFileName(strPath)));
                            }
                        }

                        if (file.DecompressedData == null)
                            file.DecompressData();

                        if (file.DecompressedData != null && file.DecompressedData.Length > 0)
                        {
                            //Convert an eventual csv file to local format what seperator and decimal concerns.
                            file.DecompressedData = RScriptReportExecuteViewModel.ConvertCSVFormatToLocalFormat(file.FileName, file.DecompressedData);

                            File.WriteAllBytes(strPath, file.DecompressedData);
                        }
                    }

                    if (BusinessLogic.Settings.Settings.Instance.OpenReportAutomatically)
                        Process.Start(Path.Combine(strDirectory, files.First().FileName));
                }

                SavePathAsDefaultForCurrentReport();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                DispatchMessageBox("En uventet fejl opstod. " + e.Message);
            }
        }

    }
}
