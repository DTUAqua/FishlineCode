using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using Anchor.Core;
using ICSharpCode.AvalonEdit.Document;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Babelfisk.Entities;

namespace Babelfisk.ViewModels.Reporting.AddEditModels
{
    public class AddEditRScriptViewModel : AViewModel
     {
        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdOK;
        private DelegateCommand<string> _cmdTestScriptCommand;


        private AddEditReportViewModel _vmReport;

        private TextDocument _tDoc;

        private ObservableCollection<Entities.ReportParameter> _lstOriginalParameters;

        private string _strScriptResult;

        private ReportExecuteModels.SelectParametersViewModel _vmTest;

        private bool _blnIsClosed = false;


        #region Properties


        public TextDocument Document
        {
            get
            {
                if (_tDoc == null)
                {
                    _tDoc = new TextDocument();
                    _tDoc.Text = _vmReport.Query ?? "";
                }

                return _tDoc;
            }
            set
            {
                _tDoc = value;
                RaisePropertyChanged(() => Document);
            }
        }


        public AddEditReportViewModel ReportViewModel
        {
            get { return _vmReport; }
        }


        public string ScriptResult
        {
            get { return _strScriptResult; }
            set
            {
                _strScriptResult = value;
                if (value != null)
                    ScrollTo("ResultEnd");

                RaisePropertyChanged(() => ScriptResult);
                RaisePropertyChanged(() => HasScriptResult);
            }
        }


        public bool HasScriptResult
        {
            get { return _strScriptResult != null; }
        }


        public bool OpenReportAutomatically
        {
            get { return BusinessLogic.Settings.Settings.Instance.OpenTestReportAutomatically; }
            set
            {
                BusinessLogic.Settings.Settings.Instance.OpenTestReportAutomatically = value;
                RaisePropertyChanged(() => OpenReportAutomatically);
            }
        }


        #endregion



        public AddEditRScriptViewModel(AddEditReportViewModel vmReport)
        {
            _vmReport = vmReport;
            _lstOriginalParameters = vmReport.Parameters == null ? new ObservableCollection<Entities.ReportParameter>() : vmReport.Parameters.Clone();
        }



     


        #region OK Command


        public DelegateCommand OKCommand
        {
            get { return _cmdOK ?? (_cmdOK = new DelegateCommand(OK)); }
        }


        public void OK()
        {
            //Turn on UI validation (this will also show fire an error on the UI controls causing errors).
            _blnValidate = true;

            //Validate all properties
            ValidateAllProperties();

            //Turn off UI validation
            _blnValidate = false;

            if (HasErrors)
                return;

            string strScript = Document.Text;
            _vmReport.Query = strScript;

            Close();

            _blnIsClosed = true;
        }




        #endregion



        #region Cancel Command


        public DelegateCommand CancelCommand
        {
            get { return _cmdCancel ?? (_cmdCancel = new DelegateCommand(Cancel)); }
        }


        public void Cancel()
        {
            //Reset any changes to parameters.
            if (_vmReport != null)
                _vmReport.Parameters = _lstOriginalParameters;

            Close();

            _blnIsClosed = true;
        }


        #endregion



        #region Test Script Command


        public DelegateCommand<string> TestScriptCommand
        {
            get { return _cmdTestScriptCommand ?? (_cmdTestScriptCommand = new DelegateCommand<string>(p => TestScriptAsync(p))); }
        }


        public void TestScriptAsync(string reportType)
        {
            string strScript = Document.Text;

            if(string.IsNullOrWhiteSpace(strScript))
            {
                AppRegionManager.ShowMessageBox("Angiv venligst et script først.", 3);
                return;
            }

            List<ReportFile> lstSupportFiles = new List<ReportFile>();
           
            if (_vmReport != null && _vmReport.Parameters != null && _vmReport.Parameters.Any())
            {
                var existingParams = _vmReport.Parameters.Where(p => strScript.Contains(p.ParameterName, StringComparison.InvariantCultureIgnoreCase)).ToList();

                if (existingParams.Any())
                {
                    var vm = new ReportExecuteModels.SelectParametersViewModel(existingParams, strScript, ReportExecuteModels.ParameterViewModel.ValueFormat.RScript, true);
                    vm.WindowTitle = "Test-parametre";
                    vm.WindowWidth = 650;

                    bool blnAssignedOldParameters = false;
                    if (_vmTest != null)
                    {
                        foreach (var tp in _vmTest.Parameters)
                        {
                            var q = vm.Parameters.Where(x => x.Parameter.ParameterName == tp.Parameter.ParameterName);
                            if (q.Any())
                            {
                                q.First().AssignSelection(tp);
                                blnAssignedOldParameters = true;
                            }
                        }
                    }

                    vm.UpdateStatement();

                    if (blnAssignedOldParameters && vm.Parameters.Count > 0) 
                        vm.FireParameterChanged(vm.Parameters.First());

                    AppRegionManager.LoadWindowViewFromViewModel(vm, true, "WindowWithBorderAutoHeightStyle");

                    if (vm.IsCancelled)
                        return;

                    _vmTest = vm;
                    strScript = vm.Statement;

                    lstSupportFiles = vm.GetSupportFiles();
                }
            }

            ScriptResult = "Tester script, vent venligst...";
            var repType = (reportType == "PDF" ? Entities.ReportResultType.PDF : Entities.ReportResultType.Word);
            IsLoading = true;
            Task.Factory.StartNew(() => TestScript(strScript, lstSupportFiles, repType)).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }

        public void TestScript(string strScript, List<ReportFile> lstSupportFiles, Entities.ReportResultType repType)
        {
            var man = new BusinessLogic.Reporting.ReportingManager();

            try
            {
                ReportFile[] additionalFiles;
                var res = man.ExecuteRScript(strScript, _vmReport.ReportEntity.reportId, lstSupportFiles, out additionalFiles, repType);

                if (_blnIsClosed)
                    return;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Output:");
                sb.AppendLine("-------------------------------------------------------------------------------------");

                sb.Append(res.Message ?? "");

                switch(res.Result)
                {
                    case Entities.DatabaseOperationStatus.Successful: //Success, save file
                        sb.Append(Environment.NewLine + "Scriptet blev eksekveret korrekt.");
                        if (additionalFiles != null && additionalFiles.Length > 0)
                            sb.Append(Environment.NewLine + string.Format("Følgende ekstra-filer blev genereret: {0}" + Environment.NewLine, string.Join(", ", additionalFiles.Select(x => x.FileName).ToArray())));
                        break;

                    case Entities.DatabaseOperationStatus.ValidationError: //Script errors
                        sb.Append(Environment.NewLine + "Scriptet fejlede, se venligst fejlbeskeden ovenfor.");
                        break;

                    case Entities.DatabaseOperationStatus.UnexpectedException: //Error unknown
                        sb.Append(Environment.NewLine + "En uventet fejl opstod, se venligst fejlbeskeden ovenfor.");
                        break;
                }

                bool blnSuccess = res.Result == Entities.DatabaseOperationStatus.Successful;

                var arr = res.Data as byte[];
                if (blnSuccess && arr != null && arr.Length > 0)
                {
                    arr = arr.Decompress();
                    string strDirectory = BusinessLogic.Settings.Settings.Instance.LocalReportFolderPath;
                    string strExt = repType == Entities.ReportResultType.PDF ? ".pdf" : ".docx";
                    string strPath = System.IO.Path.Combine(strDirectory, "test-rapport" + strExt);

                    if(File.Exists(strPath))
                    {
                        try
                        {
                            File.Delete(strPath);
                        }
                        catch
                        {
                            strPath = System.IO.Path.Combine(strDirectory, "test-rapport-" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") + strExt);
                        }
                    }

                    File.WriteAllBytes(strPath, arr);

                    sb.Append(string.Format(Environment.NewLine + "Rapport gemt: '{0}'", strPath));

                    if (OpenReportAutomatically)
                        Process.Start(strPath);
                }

                new Action(() =>
                {
                    ScriptResult = sb.ToString();
                }).Dispatch();
               
            }
            catch (Exception e)
            {
                new Action(() =>
                {
                    ScriptResult = string.Format("En uventet fejl opstod. {0}", e.Message);
                }).Dispatch();
            }
        }


        #endregion
    }
}
