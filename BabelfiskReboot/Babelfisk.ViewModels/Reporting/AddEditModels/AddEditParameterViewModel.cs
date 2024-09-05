using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babelfisk.Entities;
using Anchor.Core;
using Microsoft.Practices.Prism.Commands;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Document;

namespace Babelfisk.ViewModels.Reporting.AddEditModels
{
    public class AddEditParameterViewModel : AViewModel
    {
        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdOK;
        private DelegateCommand<object> _cmdRemoveParameterItem;
        private DelegateCommand _cmdTestSQLQuery;
        private DelegateCommand _cmdResetReferencedParameter;

        private ReportParameter _parameter;

        private List<ReportParameter> _lstAvailableParameters;

        private AddEditReportViewModel _vmReport;

        private bool _blnIsEdit;

        private List<ParameterListItem> _lstQueryResults;

        private ReportExecuteModels.SelectParametersViewModel _vmTest;

        private string _strMaximumLength;

        private List<ReportParameter> _lstReferenceableParameters;

       

        #region Properties


        public bool IsEdit
        {
            get { return _blnIsEdit; }
            set
            { 
                _blnIsEdit = value;
                RaisePropertyChanged(() => IsEdit);
            }
        }


        public ReportParameter Parameter
        {
            get { return _parameter; }
        }


        public List<KeyValuePair<string, string>> Types
        {
            get
            {
                return new List<KeyValuePair<string, string>>() 
                {
                    new KeyValuePair<string, string>("Value", "Værdi"),
                    new KeyValuePair<string, string>("SQL", "Liste (SQL-query)"),
                    new KeyValuePair<string, string>("List", "Liste (statisk)"),
                    new KeyValuePair<string, string>("File", "Fil"),
                };
            }
        }


        public List<KeyValuePair<string, string>> ReturnTypes
        {
            get
            {
                return new List<KeyValuePair<string, string>>() 
                {
                    new KeyValuePair<string, string>("String", "Streng"),
                    new KeyValuePair<string, string>("Integer", "Integer (heltal)"),
                    new KeyValuePair<string, string>("Decimal", "Decimal"),
                    new KeyValuePair<string, string>("Boolean", "Boolean"),
                };
            }
        }


        public List<KeyValuePair<string, string>> SelectionModes
        {
            get
            {
                return new List<KeyValuePair<string, string>>() 
                {
                    new KeyValuePair<string, string>("DropDownList", "Nedrulningslist (enkelt valg)"),
                    new KeyValuePair<string, string>("CheckBoxList", "Afkrydsningsliste (flere valg)"),
                };
            }
        }


        public List<KeyValuePair<string, string>> LineCounts
        {
            get
            {
                return new List<KeyValuePair<string, string>>() 
                {
                    new KeyValuePair<string, string>("1", "1 linje"),
                    new KeyValuePair<string, string>("2", "2 linjer"),
                    new KeyValuePair<string, string>("3", "3 linjer"),
                    new KeyValuePair<string, string>("4", "4 linjer"),
                    new KeyValuePair<string, string>("5", "5 linjer"),
                    new KeyValuePair<string, string>("6", "6 linjer"),
                    new KeyValuePair<string, string>("7", "7 linjer"),
                    new KeyValuePair<string, string>("8", "8 linjer"),
                    new KeyValuePair<string, string>("9", "9 linjer"),
                    new KeyValuePair<string, string>("10", "10 linjer"),
                };
            }
        }


        /// <summary>
        /// Available parameters are used for displaying to the user which parameters can be used in the SQL query.
        /// </summary>
        public List<ReportParameter> AvailableParameters
        {
            get { return _lstAvailableParameters; }
            set
            {
                _lstAvailableParameters = value;
                RaisePropertyChanged(() => AvailableParameters);
                RaisePropertyChanged(() => HasAvailableParameters);
            }
        }


        public bool HasAvailableParameters
        {
            get { return _lstAvailableParameters != null && _lstAvailableParameters.Count > 0; }
        }


        public List<ParameterListItem> QueryResults
        {
            get { return _lstQueryResults;  }
            set
            {
                _lstQueryResults = value;
                RaisePropertyChanged(() => QueryResults);
                RaisePropertyChanged(() => HasQueryResults);
            }
        }


        public bool HasQueryResults
        {
            get { return QueryResults != null; }
        }


        private TextDocument _tDoc;
        public TextDocument Document
        {
            get 
            {
                if (_tDoc == null)
                {
                    _tDoc = new TextDocument();
                    _tDoc.Text = _parameter.Statement ?? "";
                }

                return _tDoc;
            }
            set
            {
                _tDoc = value;
                RaisePropertyChanged(() => Document);
            }
        }


        public string Statement
        {
            get { return Document.Text; }
            set
            {
                Document.Text = value ?? "";
                _parameter.Statement = value;
                RaisePropertyChanged(() => Statement);
            }
        }


        public string MaximumLength
        {
            get { return _strMaximumLength; }
            set
            {
                _strMaximumLength = value;
                RaisePropertyChanged(() => MaximumLength);
            }
        }


        public int? ReferenceParameterId
        {
            get { return _parameter.ReferenceParameterIdFilterBy; }
            set 
            { 
                _parameter.ReferenceParameterIdFilterBy = value;
                RaisePropertyChanged(() => ReferenceParameterId);
                RaisePropertyChanged(() => HasReferencedParameter);
            }
        }


        public bool HasReferencedParameter
        {
            get { return _parameter != null && _parameter.ReferenceParameterIdFilterBy.HasValue; }
        }


        public List<ReportParameter> ReferenceableParameters
        {
            get { return _lstReferenceableParameters; }
            set
            {
                _lstReferenceableParameters = value;
                RaisePropertyChanged(() => ReferenceableParameters);
            }
        }


        #endregion




        public AddEditParameterViewModel(AddEditReportViewModel vm, ReportParameter p = null)
        {
            _vmReport = vm;
            if (p == null)
            {
                IsEdit = false;
                _parameter = new ReportParameter();
                _parameter.Id = vm.Parameters.Count == 0 ? 0 : (vm.Parameters.Max(x => x.Id) + 1);
                _parameter.ParameterName = "@param"; // +_parameter.Id;
                _parameter.ReturnType = "String";
                _parameter.ParameterType = "Value";
                _parameter.SelectionMode = "DropDownList";
                _strMaximumLength = "0";
                _parameter.NumberOfLines = 1;
            }
            else
            {
                IsEdit = true;
                _parameter = p.Clone();
                _strMaximumLength = _parameter.MaximumLength.ToString();
            }

            _lstQueryResults = null;
            UpdateAvailableParameters();

            RowEditingEnding();
        }


        private void UpdateAvailableParameters()
        {
            if (_vmReport.Parameters == null || _vmReport.Parameters.Count == 0)
                return;

            AvailableParameters = _vmReport.Parameters.TakeUntil(x => x.Id, _parameter.Id).ToList();

            //ReferenceableParameters are parameters that are multiselect and not the current parameter.
            ReferenceableParameters = _vmReport.Parameters.TakeUntil(x => x.Id, _parameter.Id).Where(x => x.IsMultiSelect).ToList();
        }


        public void RowEditingEnding()
        {
            AddEmptyRows();
        }

        private void AddEmptyRows()
        {
            try
            {
                if (_parameter.List == null)
                    return;

                int intEmptyRows = 2;

                //Determine how many empty row to add.
                int intCount = intEmptyRows;
                if ((intCount = _parameter.List.Where(x => string.IsNullOrEmpty(x.DisplayName) && string.IsNullOrEmpty(x.Value)).Count()) < intEmptyRows)
                    intEmptyRows = intEmptyRows - intCount;
                else
                    return;

                //Add empty rows
                for (int i = 0; i < intEmptyRows; i++)
                    _parameter.List.Add(new ParameterListItem(null, null));
            }
            finally
            {
                
            }
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
                    case "Parameter":
                        if (string.IsNullOrWhiteSpace(_parameter.DisplayName))
                            return "Angiv venligst det viste navn for parameteren.";
                        else if (string.IsNullOrWhiteSpace(_parameter.ParameterName))
                            return _parameter.IsFileType ? "Angiv venligst filnavn." : "Angiv venligst parameternavn.";
                        else if(_vmReport.Parameters.Where(x => x.Id != _parameter.Id && x.ParameterName.Equals(_parameter.ParameterName, StringComparison.InvariantCultureIgnoreCase)).Any())
                            return "En parameter med samme parameternavn er allerede defineret. Angiv venligst et andet parameternavn.";
                        else if(string.IsNullOrWhiteSpace(_parameter.ParameterType))
                            return "Angiv venligst parametertype.";
                        else if (_parameter.ParameterType != "Value" && string.IsNullOrWhiteSpace(_parameter.SelectionMode))
                            return "Angiv venligst listetype.";
                        else if (string.IsNullOrWhiteSpace(_parameter.ReturnType))
                            return "Angiv venligst data-returneringstype.";
                        else if (_parameter.ParameterType == "List")
                        {
                            if(!_parameter.List.Where(i => !string.IsNullOrEmpty(i.DisplayName) && !string.IsNullOrEmpty(i.Value)).Any())
                                return "Angiv venligst mindst én listeværdi.";
                            else if (_parameter.List.Where(i => (!string.IsNullOrEmpty(i.DisplayName) && string.IsNullOrEmpty(i.Value)) || (string.IsNullOrEmpty(i.DisplayName) && !string.IsNullOrEmpty(i.Value))).Any())
                                return "Angiv venligst begge felter (viste værdi og værdi) for alle listeværdier.";
                            else
                            {
                                foreach(var p in _parameter.List.Where(i => !string.IsNullOrEmpty(i.DisplayName) && !string.IsNullOrEmpty(i.Value)))
                                {
                                    switch(_parameter.ReturnType)
                                    {
                                        case "Integer":
                                            int i;
                                            if (!int.TryParse(p.Value, out i))
                                                return "En eller flere listeværdier kunne ikke konverteres til Integer.";
                                            break;
                                        case "Decimal":
                                            decimal d;
                                            if (!decimal.TryParse(p.Value, out d))
                                                return "En eller flere listeværdier kunne ikke konverteres til Decimal.";
                                            break;
                                        case "Boolean":
                                             Boolean b;
                                            if (!bool.TryParse(p.Value, out b))
                                                return "En eller flere listeværdier kunne ikke konverteres til Boolean.";
                                            break;
                                    }
                                }
                            }
                        }
                        else if(_parameter.ParameterType == "SQL" && string.IsNullOrEmpty(Statement))
                            return "Angiv venligst en SQL-query.";
                        else if (_parameter.IsFileType && _parameter.ParameterName.HasInvalidFileNameCharacters())
                            return string.Format("Filnavn indeholder karakterer som et filnavn ikke må indeholde. Følgende karakterer er ikke lovlige: {0}.", string.Join(", ", System.IO.Path.GetInvalidFileNameChars()));
                        else if (_parameter.IsFileType && _vmReport.Parameters != null && _vmReport.Parameters.Where(x => x.IsFileType && x.Id != _parameter.Id && x.ParameterName != null && x.ParameterType.Equals(_parameter.ParameterName, StringComparison.InvariantCultureIgnoreCase)).Any())
                            return string.Format("En parameter med filnavn ('{0}') eksisterer allerede. Indtast venligst et andet filnavn.", _parameter.ParameterName);
                        else if (_parameter.IsFileType && _vmReport.ReportFiles != null && _vmReport.ReportFiles.Where(x => x.FileName != null && System.IO.Path.GetFileName(x.FileName).Equals(_parameter.ParameterName, StringComparison.InvariantCultureIgnoreCase)).Any())
                            return string.Format("En hjælpefil med filnavn ('{0}') eksisterer allerede. Indtast venligst et andet filnavn.", _parameter.ParameterName);
                       /* else if (_parameter.IsFileType && _parameter.ParameterName != null)
                        {
                            var ext = System.IO.Path.GetExtension(_parameter.ParameterName);
                            if (string.IsNullOrWhiteSpace(ext))
                                return string.Format("Filnavnet ('{0}') skal indeholde filtype (f.eks. '.csv' eller '.jpg').", _parameter.ParameterName);
                        }*/
                        break;

                    case "MaximumLength":
                        int intMaxLength = -1;
                        if (!MaximumLength.TryParseInt32(out intMaxLength) || intMaxLength < 0)
                            return "Angiv venligst et heltal >= 0 for 'Max streng-længde'.";
                        break;
                }
            }

            return null;
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

            _parameter.Statement = Statement;

            IsLoading = true;
            Task.Factory.StartNew(Save).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());

        }


        /// <summary>
        /// Save changes to database. This method does not validate any fields (this should be done prior to calling the method).
        /// </summary>
        private void Save()
        {
            try
            {
                //Remove empty parameters
                var p = _parameter.Clone();
                var lst = p.List.ToList();

                foreach (var i in lst)
                    if (string.IsNullOrEmpty(i.DisplayName) && string.IsNullOrEmpty(i.Value))
                        p.List.Remove(i);

                if ((p.IsSQLType || p.IsValueType) && p.List.Count > 0)
                    p.List.Clear();

                int intMaxLength;
                if (MaximumLength.TryParseInt32(out intMaxLength))
                    p.MaximumLength = intMaxLength;

                //Make multiselect flag is not set, if type is value type.
                if (p.IsMultiSelect && p.IsValueType)
                    p.SelectionMode = "DropDownList";

                //Make sure the referenced parameter exists and if it doesn't, reset the referenced id.
                if (p.ReferenceParameterIdFilterBy.HasValue && (ReferenceableParameters == null || !ReferenceableParameters.Where(x => x.Id == p.ReferenceParameterIdFilterBy.Value).Any()))
                    p.ReferenceParameterIdFilterBy = null;

                new Action(() =>
                {
                    if (IsEdit)
                    {
                        for (int i = 0; i < _vmReport.Parameters.Count; i++)
                        {
                            if (_vmReport.Parameters[i].Id == p.Id)
                            {
                                _vmReport.Parameters[i] = p;
                                break;
                            }
                        }
                    }
                    else
                    {
                        _vmReport.Parameters.Add(p);
                    }

                     _vmReport.Parameters = _vmReport.Parameters;
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


        #region Reset Referenced Parameter Command


        public DelegateCommand ResetReferencedParameterCommand
        {
            get { return _cmdResetReferencedParameter ?? (_cmdResetReferencedParameter = new DelegateCommand(ResetReferencedParameter)); }
        }


        public void ResetReferencedParameter()
        {
            ReferenceParameterId = null;
        }


        #endregion

        

        #region Remove Parameter Item Command

        public DelegateCommand<object> RemoveParameterItemCommand
        {
            get { return _cmdRemoveParameterItem ?? (_cmdRemoveParameterItem = new DelegateCommand<object>(p => RemoveParameterItem(p as ParameterListItem))); }
        }


        public void RemoveParameterItem(ParameterListItem itm)
        {
            if (itm == null)
                return;


            if (AppRegionManager.ShowMessageBox(string.Format("Er du sikker på du vil slette parameteren '{0}'?", itm.DisplayName), System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                return;


            _parameter.List.Remove(itm);
            AddEmptyRows();
        }


        #endregion


        #region Test SQL Query Command


        public DelegateCommand TestSQLQueryCommand
        {
            get { return _cmdTestSQLQuery ?? (_cmdTestSQLQuery = new DelegateCommand(TestSQLQueryAsync)); }
        }


       

        public void TestSQLQueryAsync()
        {
            if(string.IsNullOrWhiteSpace(Statement))
            {
                AppRegionManager.ShowMessageBox("Angiv venligst en query først.");
                return;
            }

            string strStatement = Statement;

            if (_vmReport != null && _vmReport.Parameters != null && _vmReport.Parameters.Where(p => p.Id < _parameter.Id).Any())
            {
                var existingParams = _vmReport.Parameters.Where(p => p.Id < _parameter.Id && strStatement.Contains(p.ParameterName, StringComparison.InvariantCultureIgnoreCase)).ToList();

                if (existingParams.Any())
                {
                    var vm = new ReportExecuteModels.SelectParametersViewModel(existingParams, strStatement, ReportExecuteModels.ParameterViewModel.ValueFormat.SQL, true);
                    //var vm = new TestParametersViewModel(existingParams, strStatement);
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

                    if (blnAssignedOldParameters && vm.Parameters.Count > 0)
                        vm.FireParameterChanged(vm.Parameters.First());

                    AppRegionManager.LoadWindowViewFromViewModel(vm, true, "WindowWithBorderAutoHeightStyle");

                    if (vm.IsCancelled)
                        return;

                    _vmTest = vm;
                    strStatement = vm.Statement;
                }
            }

            QueryResults = null;
            IsLoading = true;

            Task.Factory.StartNew(() => TestSQLQuery(strStatement)).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        private void TestSQLQuery(string strStatement)
        {
            var man = new BusinessLogic.Reporting.ReportingManager();

            try
            {
                var res = man.ExecuteSQLStatement(strStatement);

                new Action(() =>
                {
                    QueryResults = res;
                }).Dispatch();
            }
            catch (Exception e)
            {
                DispatchMessageBox(e.Message);
            }
        }


        #endregion

    }
}
