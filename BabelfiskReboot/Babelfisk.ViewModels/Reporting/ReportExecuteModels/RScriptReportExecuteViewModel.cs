using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;
using System.IO;
using Babelfisk.BusinessLogic.Reporting;
using Babelfisk.Entities;
using System.Xml.Linq;
using System.Threading.Tasks;
using Anchor.Core;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Globalization;

namespace Babelfisk.ViewModels.Reporting.ReportExecuteModels
{
    public class RScriptReportExecuteViewModel : ReportExecuteViewModel
    {
        

        public RScriptReportExecuteViewModel(ReportViewModel vmReport)
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


        private void Load(int intReportId, string reportType)
        {
            try
            {
                var repMan = new ReportingManager();

                string strDetails = repMan.GetReportDetails(intReportId);

                if (_vmReport.IsDisposed)
                    return;

                if (!string.IsNullOrWhiteSpace(strDetails))
                {
                    var xeDetails = XElement.Parse(strDetails);

                    List<ReportParameter> lstParameters;
                    string strQuery;
                    string strReportFileName;

                    AddEditModels.AddEditReportViewModel.Load(xeDetails, out lstParameters, out strQuery, out strReportFileName);
                    List<ReportFile> lstSupportFiles = new List<ReportFile>();

                    if (_vmReport != null && lstParameters != null && lstParameters.Any())
                    {
                        var existingParams = lstParameters.Where(p => strQuery.Contains(p.ParameterName, StringComparison.InvariantCultureIgnoreCase)).ToList();
                        

                        if (existingParams.Any())
                        {
                            new Action(() => IsLoading = false).DispatchInvoke();

                            var vm = new ReportExecuteModels.SelectParametersViewModel(existingParams, strQuery, ReportExecuteModels.ParameterViewModel.ValueFormat.RScript);
                            vm.WindowTitle = "Vælg parametre";
                            vm.WindowWidth = 650;

                            new Action(() =>
                            {
                                AppRegionManager.LoadWindowViewFromViewModel(vm, true, "WindowWithBorderAutoHeightStyle");
                            }).DispatchInvoke();

                            if (vm.IsCancelled)
                                return;

                            strQuery = vm.Statement;

                            lstSupportFiles = vm.GetSupportFiles();

                            try
                            {
                                //Handle custom report file name
                                if (strReportFileName != null)
                                {
                                    strReportFileName = vm.ReplaceStringWithParameterFileNameDisplayValues(strReportFileName);

                                    //Make sure filename is not too long.
                                    if (strReportFileName.Length > 160)
                                        strReportFileName = strReportFileName.Substring(0, 160);
                                }
                            }
                            catch (Exception e)
                            {
                                Anchor.Core.Loggers.Logger.LogError(e);
                            }

                            new Action(() => IsLoading = true).DispatchInvoke();
                        }
                    }

                    var repType = (reportType == "PDF" ? Entities.ReportResultType.PDF : Entities.ReportResultType.Word);
                    IsLoading = true;
                    RunScript(strQuery, intReportId, strReportFileName, lstSupportFiles, repType);
                }

                SavePathAsDefaultForCurrentReport();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                DispatchMessageBox("En uventet fejl opstod. " + e.Message);
            }
        }



        public void RunScript(string strScript, int intReport, string strReportCustomFileName, List<ReportFile> lstSupportFiles, Entities.ReportResultType repType)
        {
            var man = new BusinessLogic.Reporting.ReportingManager();

            try
            {
                ReportFile[] additionalFiles;
                var res = man.ExecuteRScript(strScript, intReport, lstSupportFiles, out additionalFiles, repType);

                if (_vmReport.IsDisposed)
                    return;

                StringBuilder sb = new StringBuilder();
               

                switch (res.Result)
                {
                    case Entities.DatabaseOperationStatus.ValidationError: //Script errors
                        sb.Append("En uventet fejl opstod under generering af rapporten. Prøv venligst igen eller/og kontakt en administrator hvis problemet fortsætter.");
                        //sb.Append(res.Message ?? "");
                        break;

                    case Entities.DatabaseOperationStatus.UnexpectedException: //Error unknown
                        sb.Append("En uventet fejl opstod. ");
                        sb.Append(res.Message ?? "");
                        break;
                }

                bool blnSuccess = res.Result == Entities.DatabaseOperationStatus.Successful;

                var arr = res.Data as byte[];
                if (blnSuccess && arr != null && arr.Length > 0)
                {
                    arr = arr.Decompress();
                    string strDirectory = LocalReportFolderPath;
                    string strExt = repType == Entities.ReportResultType.PDF ? ".pdf" : ".docx";
                    string strReportName = !string.IsNullOrWhiteSpace(strReportCustomFileName) ? strReportCustomFileName : (_vmReport.ReportEntity.name ?? "report");
                    strReportName = strReportName.Remove(Path.GetInvalidFileNameChars());
                    string strPath = System.IO.Path.Combine(strDirectory, strReportName + strExt);

                    if (File.Exists(strPath))
                    {
                        bool blnAbort = false;
                      
                        new Action(() =>
                        {
                            if (AppRegionManager.ShowMessageBox(string.Format("Rapporten '{0}' eksisterer allerede, ønsker du at fortsætte og overskrive rapporten?", strPath), System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.Cancel)
                                blnAbort = true;
                        }).DispatchInvoke();

                        if (blnAbort)
                            return;

                        try
                        {
                            File.Delete(strPath);
                        }
                        catch
                        {
                            strPath = System.IO.Path.Combine(strDirectory, strReportName + "-" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") + strExt);

                            DispatchMessageBox(string.Format("Det var ikke muligt at overskrive den eksisterende rapport, rapporten gemmes i stedet som: '{0}'.", strPath));
                        }
                    }

                    File.WriteAllBytes(strPath, arr);

                    List<string> lstAdditionalFilesPath = new List<string>();
                    if (additionalFiles != null && additionalFiles.Length > 0)
                    {
                        lstAdditionalFilesPath = SaveAdditionalFiles(additionalFiles, strDirectory);
                    }

                    if (BusinessLogic.Settings.Settings.Instance.OpenReportAutomatically)
                        Process.Start(strPath);

                    if (lstAdditionalFilesPath.Count > 0)
                    {
                        StringBuilder sbl = new StringBuilder();
                        sbl.AppendLine("Udover rapporten, blev følgende filer gemt:");
                        sbl.AppendLine("");
                        for (int i = 0; i < lstAdditionalFilesPath.Count; i++)
                            sbl.AppendLine(lstAdditionalFilesPath[i]);

                        DispatchMessageBox(sbl.ToString());
                    }
                }
                else
                {
                    DispatchMessageBox(sb.ToString());
                }
            }
            catch (Exception e)
            {
                DispatchMessageBox(string.Format("En uventet fejl opstod. {0}", e.Message));
            }
        }


        private List<string> SaveAdditionalFiles(ReportFile[] files, string strDirectory)
        {
            List<string> lst = new List<string>();

            foreach (var file in files)
            {
                string strPath = Path.Combine(strDirectory, file.FileName);
                if (File.Exists(strPath))
                {
                    bool blnAbort = false;

                    new Action(() =>
                    {
                        if (AppRegionManager.ShowMessageBox(string.Format("Filen '{0}' eksisterer allerede i mappen, ønsker du at overskrive filen?", file.FileName), System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.Cancel)
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

                        DispatchMessageBox(string.Format("Det var ikke muligt at overskrive filen '{0}', filen gemmes i stedet som: '{1}'.", file.FileName, Path.GetFileName(strPath)));
                    }
                }

                try
                {
                    if (file.DecompressedData == null)
                        file.DecompressData();

                    if (file.DecompressedData != null && file.DecompressedData.Length > 0)
                    {
                        file.DecompressedData = ConvertCSVFormatToLocalFormat(file.FileName, file.DecompressedData);

                        File.WriteAllBytes(strPath, file.DecompressedData);
                        lst.Add(strPath);
                    }
                }
                catch (Exception e)
                {
                    DispatchMessageBox(string.Format("Det var ikke muligt at gemme filen '{0}'. Fejlbesked: {1}", Path.GetFileName(strPath), e.Message));
                }
            }

            return lst;
        }


        public static byte[] ConvertCSVFormatToLocalFormat(string FileName, byte[] arr)
        {
            var DecompressedData = arr;

            if (DecompressedData != null && DecompressedData.Length > 0)
            {
                //Make sure delimiter and seperator for csv files conform to users settings.
                string strExt = "";
                if (FileName != null && (strExt = Path.GetExtension(FileName)) != null && strExt.Equals(".csv", StringComparison.InvariantCultureIgnoreCase))
                {
                    var strSeperator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                    var strDelimiter = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

                    int intMaxLinesToAnalyze = 0;
                    List<string> lstLines = new List<string>();

                    using (MemoryStream ms = new MemoryStream(DecompressedData))
                    {
                        //Read first 10 lines to determine seperator
                        using (StreamReader sr = new StreamReader(ms, Encoding.Default))
                        {
                            string strLine = null;
                            while ((strLine = sr.ReadLine()) != null && intMaxLinesToAnalyze++ < 10)
                                lstLines.Add(strLine);
                        }
                    }

                    //Determine seperator
                    string strOldSeperator = null;
                    if (lstLines.Count > 0)
                        strOldSeperator = DetermineSeperator(lstLines, new char[] { '\t', ',', ':', ';' });

                    //Convert to new seperator
                    if (!string.IsNullOrEmpty(strOldSeperator) && strOldSeperator != strSeperator)
                    {
                        using (MemoryStream msDest = new MemoryStream())
                        {
                            using (StreamWriter sw = new StreamWriter(msDest, Encoding.Default))
                            {
                                using (MemoryStream ms = new MemoryStream(DecompressedData))
                                {
                                    using (Microsoft.VisualBasic.FileIO.TextFieldParser tfp = new Microsoft.VisualBasic.FileIO.TextFieldParser(ms, Encoding.Default))
                                    {
                                        tfp.SetDelimiters(new string[] { strOldSeperator });
                                        tfp.HasFieldsEnclosedInQuotes = true;
                                        tfp.TrimWhiteSpace = false;

                                        string strDelimiterToReplace = strDelimiter == "." ? "," : ".";
                                        string[] arrFields = null;
                                        while (!tfp.EndOfData && (arrFields = tfp.ReadFields()) != null)
                                        {
                                            if (arrFields != null && arrFields.Length > 0)
                                            {
                                                for (int i = 0; i < arrFields.Length; i++)
                                                    arrFields[i] = arrFields[i].Replace(strDelimiterToReplace, strDelimiter);

                                                sw.WriteLine(string.Join(strSeperator, arrFields));
                                            }
                                        }
                                    }
                                }
                            }

                            DecompressedData = msDest.ToArray();
                        }
                    }
                }
            }

            return DecompressedData;
        }


        

        /// <summary>
        /// Analyze the given lines of text and try to determine the correct delimiter used. If multiple
        /// candidate delimiters are found, the highest frequency delimiter will be returned.
        /// </summary>
        /// <example>
        /// string discoveredDelimiter = DetectDelimiter(dataLines, new char[] { '\t', '|', ',', ':', ';' });
        /// </example>
        /// <param name="lines">Lines to inspect</param>
        /// <param name="delimiters">Delimiters to search for</param>
        /// <returns>The most probable delimiter by usage, or null if none found.</returns>
        public static string DetermineSeperator(IEnumerable<string> lines, IEnumerable<char> delimiters)
        {
            Dictionary<char, int> delimFrequency = new Dictionary<char, int>();

            // Setup our frequency tracker for given delimiters
            delimiters.ToList().ForEach(curDelim =>
              delimFrequency.Add(curDelim, 0)
            );

            // Get a total sum of all occurrences of each delimiter in the given lines
            delimFrequency.ToList().ForEach(curDelim =>
              delimFrequency[curDelim.Key] = lines.Sum(line => line.Count(p => p == curDelim.Key))
            );

            // Find delimiters that have a frequency evenly divisible by the number of lines
            // (correct & consistent usage) and order them by largest frequency
            var possibleDelimiters = delimFrequency
                              .Where(f => f.Value > 0 && f.Value % lines.Count() == 0)
                              .OrderByDescending(f => f.Value)
                              .ToList();

            // If more than one possible delimiter found, return the most used one
            if (possibleDelimiters.Count() > 0)
            {
                return possibleDelimiters.First().Key.ToString();
            }
            else
            {
                return null;
            }

        }

    }
}
