using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Anchor.Core;
using Babelfisk.BusinessLogic.Reporting;
using Babelfisk.Entities;
using Babelfisk.Entities.Sprattus;
using Microsoft.Practices.Prism.Commands;

namespace Babelfisk.ViewModels.Reporting.AddEditModels
{
    [DataContract]
    public class AddEditReportViewModel : AViewModel
    {
        private DelegateCommand _cmdAddParameter;
        private DelegateCommand<object> _cmdEditParameter;
        private DelegateCommand<object> _cmdRemoveParameter;
        private DelegateCommand _cmdEditQueryCommand;
        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdOK;
        private DelegateCommand _cmdAddReportFile;
        private DelegateCommand<object> _cmdRemoveReportFile;
        private DelegateCommand _cmdReportFileNameHelp;
        private DelegateCommand _cmdBrowse;

        private bool _blnIsEdit = false;
        private bool _blnAddingFiles;


        private Report _entReport;

        private ObservableCollection<ReportParameter> _lstParameters;

        private string _strQuery;

        private ReportingTreeNode _entParent;

        private bool _blnIsSaved;

        private Babelfisk.ViewModels.OutputFormat _enmOutputFormat;

        private ObservableCollection<ReportFile> _lstReportFiles;

        private bool _blnHasReportFilesBeenEdited = false;

        private HashSet<Entities.SecurityTask> _lstPermissions;

        private string _strReportFileName;




        #region Properties


        public bool IsSaved
        {
            get { return _blnIsSaved; }
            set
            {
                _blnIsSaved = value;
                RaisePropertyChanged(() => IsSaved);
            }
        }


        public bool IsEdit
        {
            get { return _blnIsEdit; }
        }


        public Report ReportEntity
        {
            get { return _entReport; }
        }


        public string Name
        {
            get { return _entReport.name; }
            set
            {
                _entReport.name = value;
                IsDirty = true;
                RaisePropertyChanged(() => Name);
            }
        }


        public string Description
        {
            get { return _entReport.description; }
            set
            {
                _entReport.description = value;
                IsDirty = true;
                RaisePropertyChanged(() => Description);
            }
        }


        public string OutputPath
        {
            get { return _entReport.outputPath; }
            set
            {
                _entReport.outputPath = value;
                IsDirty = true;
                RaisePropertyChanged(() => OutputPath);
                RaisePropertyChanged(() => HasOutputPath);
            }
        }

        public bool HasOutputPath
        {
            get { return !string.IsNullOrWhiteSpace(_entReport.outputPath); }
        }


        public string OutputPathRestriction
        {
            get { return _entReport.outputPathRestriction; }
            set
            {
                _entReport.outputPathRestriction = value;
                IsDirty = true;
                RaisePropertyChanged(() => OutputPathRestriction);
                RaisePropertyChanged(() => HasOutputPathRestriction);
            }
        }

        public bool HasOutputPathRestriction
        {
            get { return !string.IsNullOrWhiteSpace(_entReport.outputPathRestriction); }
        }


        public string ReportFileName
        {
            get { return _strReportFileName; }
            set
            {
                _strReportFileName = value;
                IsDirty = true;
                RaisePropertyChanged(() => ReportFileName);
            }
        }


        public string ReportType
        {
            get { return _entReport.type; }
            set
            {
                _entReport.type = value;
                IsDirty = true;
                RaisePropertyChanged(() => ReportType);
                RaisePropertyChanged(() => IsRScriptSelected);
                RaisePropertyChanged(() => IsDocumentTypeSelected);
            }
        }


        public List<KeyValuePair<string, string>> ReportTypes
        {
            get
            {
                return new List<KeyValuePair<string, string>>() 
                {
                    new KeyValuePair<string, string>(Babelfisk.ViewModels.ReportType.RScript.ToString(), "R-script (R Markdown)"),
                    new KeyValuePair<string, string>(Babelfisk.ViewModels.ReportType.Document.ToString(), "Dokument")
                };
            }
        }


        public Babelfisk.ViewModels.OutputFormat OutputFormat
        {
            get { return _enmOutputFormat; }
            set
            {
                _enmOutputFormat = value;
                IsDirty = true;
                RaisePropertyChanged(() => OutputFormat);
            }
        }


        public List<KeyValuePair<Babelfisk.ViewModels.OutputFormat, string>> OutputFormats
        {
            get
            {
                return new List<KeyValuePair<Babelfisk.ViewModels.OutputFormat, string>>() 
                {
                    new KeyValuePair<Babelfisk.ViewModels.OutputFormat, string>(Babelfisk.ViewModels.OutputFormat.All, "PDF og Word"),
                    new KeyValuePair<Babelfisk.ViewModels.OutputFormat, string>(Babelfisk.ViewModels.OutputFormat.PDF, "Kun PDF"),
                     new KeyValuePair<Babelfisk.ViewModels.OutputFormat, string>(Babelfisk.ViewModels.OutputFormat.Word, "Kun Word")
                };
            }
        }


        public ObservableCollection<ReportParameter> Parameters
        {
            get { return _lstParameters; }
            set
            {
                _lstParameters = value;
                RaisePropertyChanged(() => Parameters);
                RaisePropertyChanged(() => HasParameters);
            }
        }


        public bool HasParameters
        {
            get { return _lstParameters != null && _lstParameters.Count > 0; }
        }


        public string Query
        {
            get { return _strQuery; }
            set
            {
                _strQuery = value;
                RaisePropertyChanged(() => Query);
                RaisePropertyChanged(() => HasQuery);
                RaisePropertyChanged(() => QueryLineCount);
                RaisePropertyChanged(() => QueryCharacterCount);
            }
        }


        public bool HasQuery
        {
            get { return !string.IsNullOrWhiteSpace(_strQuery); }
        }


        public int QueryLineCount
        {
            get 
            {
                int intCount = 0;

                try
                {
                    intCount = string.IsNullOrEmpty(_strQuery) ? 0 : _strQuery.Split('\n').Length;
                }
                catch { }

                return intCount;
            }
        }


        public int QueryCharacterCount
        {
            get
            {
                int intCount = 0;

                try
                {
                    intCount = string.IsNullOrEmpty(_strQuery) ? 0 : _strQuery.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", "").Length;
                }
                catch { }

                return intCount;
            }
        }


        public string TreeLocation
        {
            get
            {
                return MoveTreeNodeViewModel.GetTargetString(_entParent);
            }
        }


        public bool IsRScriptSelected
        {
            get { return ReportType == Babelfisk.ViewModels.ReportType.RScript.ToString(); }
        }

        public bool IsDocumentTypeSelected
        {
            get { return ReportType == Babelfisk.ViewModels.ReportType.Document.ToString(); }
        }


        public ObservableCollection<ReportFile> ReportFiles
        {
            get { return _lstReportFiles; }
            set
            {
                _lstReportFiles = value;
                RaisePropertyChanged(() => ReportFiles);
                RaisePropertyChanged(() => HasReportFiles);
            }
        }


        public bool HasReportFiles
        {
            get { return _lstReportFiles != null && _lstReportFiles.Count > 0; }
        }


        public bool IsAddingFiles
        {
            get { return _blnAddingFiles; }
            set
            {
                _blnAddingFiles = value;
                RaisePropertyChanged(() => IsAddingFiles);
            }
        }


        public string ReportFileNameHelpText
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("Angiv et valgfrit filnavn for rapporten. Angives der ikke noget filnavn, vil filnavnet få samme navn som selve rapporten.");
                sb.AppendLine("");
                sb.AppendLine("Det er muligt at bruge rapportens input-parametre som dynamiske parametre i filnavnet. Hvis rapporten har følgende parametre defineret:");
                sb.AppendLine("");
                sb.AppendLine("- @paramYear");
                sb.AppendLine("- @paramTrip");
                sb.AppendLine("");
                sb.AppendLine("Kan man f.eks. specificere filnavnet som:");
                sb.AppendLine("");
                sb.AppendLine("'fangstrapport_@paramYear_@paramTrip'");
                sb.AppendLine("");
                sb.AppendLine("Når rapporten herefter hentes af brugeren, vil @paramYear og @paramTrip blive erstattet med brugerens valg.");

                return sb.ToString();
            }
        }


        public bool IsDFADRestricted
        {
            get { return _lstPermissions != null && _lstPermissions.Contains(SecurityTask.ViewDFADReports); }
            set
            {
                if (_lstPermissions == null)
                    _lstPermissions = new HashSet<SecurityTask>();

                if (value)
                {
                    if (!_lstPermissions.Contains(SecurityTask.ViewDFADReports))
                        _lstPermissions.Add(SecurityTask.ViewDFADReports);
                }
                else
                {
                    if (_lstPermissions.Contains(SecurityTask.ViewDFADReports))
                        _lstPermissions.Remove(SecurityTask.ViewDFADReports);
                }

                RaisePropertyChanged(() => IsDFADRestricted);
            }
        }
    

        #endregion


        public AddEditReportViewModel(Report report, ReportingTreeNode repParent)
        {
            _entParent = repParent;
            Parameters = new ObservableCollection<ReportParameter>();

            if (report == null)
            {
                _blnIsEdit = false;
                _entReport = new Report();
                _enmOutputFormat = ViewModels.OutputFormat.All;
                
                if (repParent != null)
                    _entReport.ReportingTreeNodes.Add(repParent);
            }
            else
            {
                _blnIsEdit = true;
                _entReport = report.Clone();
            }

            _lstPermissions = _entReport.Permissions;

            InitializeAsync();
        }


        public void InitializeAsync()
        {
            if (!IsEdit)
                return;

            IsLoading = true;
            _blnHasReportFilesBeenEdited = false;
            Task.Factory.StartNew(Initialize).ContinueWith(t => new Action(() => { IsLoading = false; IsDirty = false; }).Dispatch());
        }


        private void Initialize()
        {
            try
            {
                var repMan = new ReportingManager();

                string strDetails = repMan.GetReportDetails(_entReport.reportId);
                List<ReportFile> lstFiles = repMan.GetReportFiles(_entReport.reportId);
             
                List<ReportParameter> lst = null;
                string strQuery = null;
                string strReportFileName = null;
                var enmOutputFormat = OutputFormat.All;

                if(!string.IsNullOrWhiteSpace(strDetails))
                {
                    var xeDetails = XElement.Parse(strDetails);

                    if (_entReport.outputFormat != null)
                        Enum.TryParse<OutputFormat>(_entReport.outputFormat, out enmOutputFormat);

                    Load(xeDetails, out lst, out strQuery, out strReportFileName);
                }

                if (lstFiles != null)
                    lstFiles.ForEach(x => x.DecompressData(true));

                new Action(() =>
                {
                    if (lst != null)
                        Parameters = lst.ToObservableCollection();

                    Query = strQuery;

                    OutputFormat = enmOutputFormat;

                    ReportFileName = strReportFileName;

                    if (lstFiles != null)
                    {
                        ReportFiles = lstFiles.ToObservableCollection();
                        lstFiles.ForEach(x => { x.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ReportFile_PropertyChanged); });
                    }
                }).Dispatch();
            }
            catch(Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }

        private void ReportFile_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FileName")
                _blnHasReportFilesBeenEdited = true;
        }


        private XElement ToXml()
        {
            XElement xeParameters = new XElement("Parameters");

            if (_lstParameters != null)
            {
                foreach (var p in _lstParameters)
                    xeParameters.Add(p.ToXml());
            }

            XElement xeReportFileName = new XElement("ReportFileName", _strReportFileName ?? "");

            XElement xeQuery = new XElement("Query");

            if (_strQuery != null)
                xeQuery.Value = _strQuery;

            XElement xeReport = new XElement("Report", xeParameters, xeReportFileName, xeQuery);

            return xeReport;
        }


        public static void Load(XElement xeReport, out List<ReportParameter> lst, out string strQuery, out string strReportFileName)
        {
            lst = new List<ReportParameter>();
            strQuery = null;
            strReportFileName = null;

            var xeParameters = xeReport.Element("Parameters");

            if (xeParameters != null)
            {
                foreach (var p in xeParameters.Elements())
                {
                    var rp = ReportParameter.Load(p);
                    if (rp != null)
                        lst.Add(rp);
                }
            }

            var xeQuery = xeReport.Element("Query");

            if (xeQuery != null)
                strQuery = xeQuery.Value;

            XElement xeReportFileName = xeReport.Element("ReportFileName");

            if (xeReportFileName != null)
                strReportFileName = xeReportFileName.Value;
                
        }

        /// <summary>
        /// Validates property with name strFieldName. This method is overriding a base method which is called
        /// whenever a property is changed.
        /// </summary>
        protected override string ValidateField(string strFieldName)
        {
            //Only perform validation when user clicks "Save".
            if (_blnValidate)
            {
                switch (strFieldName)
                {
                    case "Name":
                        if (string.IsNullOrWhiteSpace(Name))
                            return "Angiv venligst navn.";
                        break;

                    case "ReportType":
                        if (string.IsNullOrWhiteSpace(ReportType))
                            return "Angiv venligst en rapporttype.";
                        break;

                    case "ReportFiles":
                        if (IsDocumentTypeSelected)
                        {
                            //Make sure report contains at least one file
                            if (ReportFiles == null || ReportFiles.Count == 0)
                                return "Upload venligst mindst én fil til rapporten.";
                        }

                        if(ReportFiles != null)
                        {
                            int intCurrentReportIndex = 0;
                            foreach (var rep in ReportFiles)
                            {
                                //Make sure filenames are unique
                                var t = ReportFiles.Select((x, index) => new { Item = x, Index = index }).ToList();
                                if (ReportFiles.Select((x, index) => new { Item = x, Index = index }).Where(x => x.Index < intCurrentReportIndex &&  x.Item != rep && x.Item.FileName != null && x.Item.FileName.Equals(rep.FileName, StringComparison.InvariantCultureIgnoreCase)).Any())
                                {
                                    if(IsDocumentTypeSelected)
                                        rep.SetError("To eller flere dokumenter med samme navn eksisterer. Sørg venligst for at alle dokument-navne er unikke.");
                                    else
                                        rep.SetError("To eller flere hjælpefiler med samme navn eksisterer. Sørg venligst for at alle hjælpefil-navne er unikke.");
                                    return rep.Error;
                                }

                                if (IsRScriptSelected && Parameters != null && rep.FileName != null && Parameters.Where(x => x.IsFileType && x.ParameterName != null && x.ParameterName.Equals(Path.GetFileName(rep.FileName), StringComparison.InvariantCultureIgnoreCase)).Any())
                                {
                                    rep.SetError(string.Format("Hjælpefil-navnet '{0}' er allerede brugt som parameter. Omdøb venligst hjælpefil-navnet til noget andet.", Path.GetFileName(rep.FileName)));
                                    return rep.Error;
                                }

                                //Make sure each ReportFile does not have any errors.
                                rep.ValidateAllProperties();
                                if (rep.HasErrors)
                                    return rep.Error;

                                intCurrentReportIndex++;
                            }
                        }

                        break;

                    case "OutputPathRestriction":
                        if (!string.IsNullOrWhiteSpace(OutputPath) &&
                           !string.IsNullOrWhiteSpace(OutputPathRestriction) &&
                           !OutputPath.StartsWith(OutputPathRestriction, StringComparison.InvariantCultureIgnoreCase))
                            return "'Restriktion af output-mappe' kan kun indeholde noget af eller hele stien angivet i 'Output-mappe'.";
                        break;
                }
            }

            return null;
        }


        #region Add Parameter Command


        public DelegateCommand AddParameterCommand
        {
            get { return _cmdAddParameter ?? (_cmdAddParameter = new DelegateCommand(AddParameter)); }
        }


        private void AddParameter()
        {
            var vm = new AddEditModels.AddEditParameterViewModel(this);
            vm.WindowWidth = 700;
            vm.WindowTitle = string.Format("Ny parameter");
            _appRegionManager.LoadWindowViewFromViewModel(vm, true, "WindowWithBorderAutoHeightStyle");
        }

        #endregion


        #region Edit Parameter Command


        public DelegateCommand<object> EditParameterCommand
        {
            get { return _cmdEditParameter ?? (_cmdEditParameter = new DelegateCommand<object>(p => EditParameter(p))); }
        }


        private void EditParameter(object obj)
        {
            ReportParameter p = obj as ReportParameter;

            if (p == null)
                return;

            var vm = new AddEditModels.AddEditParameterViewModel(this, p);
            vm.WindowWidth = 700;
            vm.WindowTitle = string.Format(string.Format("Rediger parameter '{0}'", p.DisplayName));
            _appRegionManager.LoadWindowViewFromViewModel(vm, true, "WindowWithBorderAutoHeightStyle");
        }

        #endregion


        #region Remove Parameter Command


        public DelegateCommand<object> RemoveParameterCommand
        {
            get { return _cmdRemoveParameter ?? (_cmdRemoveParameter = new DelegateCommand<object>(p => DeleteParameter(p))); }
        }


        private void DeleteParameter(object obj)
        {
            ReportParameter p = obj as ReportParameter;

            if (p == null)
                return;

            if (AppRegionManager.ShowMessageBox(string.Format("Er du sikker på du vil slette parameteren '{0}'?", p.ParameterName ?? ""), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;

            Parameters.Remove(p);
        }

        #endregion


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

            IsLoading = true;
            Task.Factory.StartNew(() => Save()).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());

        }

        /// <summary>
        /// Save changes to database. This method does not validate any fields (this should be done prior to calling the method).
        /// </summary>
        private void Save(bool blnCloseOnSuccess = true)
        {
            try
            {
                if (!IsEdit && _entParent != null)
                    _entReport.ReportingTreeNodes.Add(_entParent);

                XElement xeDetails = ToXml();

                string strDetails = xeDetails.ToString(SaveOptions.None);

                _entReport.Details = strDetails;

                _entReport.outputFormat = _enmOutputFormat.ToString();

                _entReport.Files = (ReportFiles == null || ReportFiles.Count == 0) ? new List<ReportFile>() : ReportFiles.ToList();

                if (_entReport.Files != null && _entReport.Files.Count > 0)
                    _entReport.Files.ForEach(x => x.CompressData(true));

                _entReport.Permissions = _lstPermissions;

                var man = new BusinessLogic.Reporting.ReportingManager();
                DatabaseOperationResult res = man.SaveReport(ref _entReport);

                //If saving cruise failed, show error message.
                if (res.DatabaseOperationStatus != DatabaseOperationStatus.Successful)
                {
                    DispatchMessageBox("En uventet fejl opstod. " + res.Message);
                    return;
                }
                
                _blnIsSaved = true;

                new Action(() =>
                {
                    //Reset report changetracker.
                    _entReport.AcceptChanges();

                    if(blnCloseOnSuccess)
                        Close();
                }).Dispatch();
            }
            catch (Exception e)
            {
                DispatchMessageBox(String.Format("En uventet fejl opstod. {0}", e.Message));
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }


      


        #endregion


        #region Cancel Command


        public DelegateCommand CancelCommand
        {
            get { return _cmdCancel ?? (_cmdCancel = new DelegateCommand(Cancel)); }
        }


        public void Cancel()
        {
            Close();
        }


        #endregion


        #region Edit Query Command


        public DelegateCommand EditQueryCommand
        {
            get { return _cmdEditQueryCommand ?? (_cmdEditQueryCommand = new DelegateCommand(EditQuery)); }
        }


        public void EditQuery()
        {
            if (_blnHasReportFilesBeenEdited)
            {
                if (AppRegionManager.ShowMessageBox("Du har redigeret navnet på, tilføjet eller fjernet en eller flere hjælpefiler. Hvis du vil teste dit R-script med disse ændringer, skal rapporten først gemmes. Tryk 'Fortryd' for at annullere redigeringen af R-scriptet så rapporten kan gemmes først, eller 'OK' for at fortsættes til R-scriptet.", System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.Cancel)
                    return;
            }

            var vm = new AddEditModels.AddEditRScriptViewModel(this);
            vm.WindowWidth = 800;
            vm.WindowHeight = 600;
            vm.WindowTitle = string.Format("R Markdown script");
            _appRegionManager.LoadWindowViewFromViewModel(vm, true, "WindowWithBorderStyle");
        }


        #endregion


        #region Add Report File Command


        public DelegateCommand AddReportFileCommand
        {
            get { return _cmdAddReportFile ?? (_cmdAddReportFile = new DelegateCommand(AddReportFileAsync)); }
        }

        public void AddReportFileAsync()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Title = "Vælg en eller flere dokumenter";
            dlg.Multiselect = true;

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                IsAddingFiles = true;
                Task.Factory.StartNew(() => AddReportFile(dlg.FileNames))
                            .ContinueWith(t => new Action(() => IsAddingFiles = false).Dispatch());
            }
        }

        public void AddReportFile(string[] arrFileNames)
        {
            List<ReportFile> lst = new List<ReportFile>();
            List<ReportFile> lstAlreadyAdded = new List<ReportFile>();
            try
            {
                //Iterate files, adding them to file collection.
                foreach (var file in arrFileNames)
                {
                    byte[] arr = null;
                    using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        arr = new byte[(int)fs.Length];
                        fs.Read(arr, 0, arr.Length);
                    }

                    if (arr != null)
                    {
                        ReportFile rf = new ReportFile();
                        rf.FileName = Path.GetFileName(file);
                        rf.DecompressedData = arr;

                        if (ReportFiles != null && ReportFiles.Where(x => x.FileName != null && x.FileName.Equals(rf.FileName, StringComparison.InvariantCultureIgnoreCase)).Any())
                        {
                            lstAlreadyAdded.Add(rf);
                            continue;
                        }

                        lst.Add(rf);
                        _blnHasReportFilesBeenEdited = true;
                        rf.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ReportFile_PropertyChanged);
                    }
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
                AppRegionManager.ShowMessageBox("En uventet fejl opstod. " + e.Message);
            }

            if (lst.Count > 0)
            {
                new Action(() =>
                {
                    if (ReportFiles == null || ReportFiles.Count == 0)
                        ReportFiles = lst.ToObservableCollection();
                    else
                        ReportFiles.AddRange(lst);
                    RaisePropertyChanged(() => ReportFiles);
                    RaisePropertyChanged(() => HasReportFiles);
                }).Dispatch();
            }

            if (lstAlreadyAdded.Count > 0)
            {
                new Action(() =>
                {
                    if (lstAlreadyAdded.Count > 0)
                        AppRegionManager.ShowMessageBox(string.Format("{0} \"{1}\" er allerede i listen og er dermed ikke blevet tilføjet.", lstAlreadyAdded.Count == 1 ? "Filen" : "Filerne", string.Join(", ", lstAlreadyAdded.Select(x => x.FileName))));
                }).Dispatch();
            }
        }
    


        #endregion


        #region Remove Report File Command


        public DelegateCommand<object> RemoveReportFileCommand
        {
            get { return _cmdRemoveReportFile ?? (_cmdRemoveReportFile = new DelegateCommand<object>(p => RemoveReportFile(p))); }
        }


        private void RemoveReportFile(object obj)
        {
            ReportFile rf = obj as ReportFile;

            if (rf == null)
                return;

            if (AppRegionManager.ShowMessageBox(string.Format("Er du sikker på du vil fjerne filen '{0}' fra rapporten?", rf.FileName ?? ""), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;

            if(ReportFiles != null)
                ReportFiles.Remove(rf);

            rf.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(ReportFile_PropertyChanged);
            _blnHasReportFilesBeenEdited = true;
        }

        #endregion



        #region Browse Command


        public DelegateCommand BrowseCommand
        {
            get { return _cmdBrowse ?? (_cmdBrowse = new DelegateCommand(Browse)); }
        }


        private void Browse()
        {
            try
            {
                FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

                if (!string.IsNullOrWhiteSpace(OutputPath))
                    dlg.SelectedPath = OutputPath;
                dlg.Description = "Vælg en destination til rapporten.";

                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    OutputPath = dlg.SelectedPath;
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }


        #endregion


        #region Report File Name Help Command


        public DelegateCommand ReportFileNameHelpCommand
        {
            get { return _cmdReportFileNameHelp ?? (_cmdReportFileNameHelp = new DelegateCommand(() => ReportFileNameHelp())); }
        }


        private void ReportFileNameHelp()
        {
            AppRegionManager.ShowMessageBox(ReportFileNameHelpText);
        }

        #endregion
    }
}
