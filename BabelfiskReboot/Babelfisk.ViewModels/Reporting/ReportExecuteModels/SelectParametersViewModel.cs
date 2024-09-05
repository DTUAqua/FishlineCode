using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Anchor.Core.Common;
using Babelfisk.Entities;
using Microsoft.Practices.Prism.Commands;
using Anchor.Core;

namespace Babelfisk.ViewModels.Reporting.ReportExecuteModels
{
    public class SelectParametersViewModel : AViewModel
    {
        private DelegateCommand _cmdCancel;
        private DelegateCommand _cmdOK;

        private List<ParameterViewModel> _lstParameters;

        private bool _blnIsCancelled = true;

        private string _strStatement;

        private string _strOrigStatement;

        private bool _blnIsTestMode;

        private ParameterViewModel.ValueFormat _valueFormat;

        #region Properties


        public List<ParameterViewModel> Parameters
        {
            get { return _lstParameters; }
            set
            {
                _lstParameters = value;
                RaisePropertyChanged(() => Parameters);
            }
        }


        public bool IsCancelled
        {
            get { return _blnIsCancelled; }
        }


        public string Statement
        {
            get { return _strStatement; }
        }


        public bool IsInTestMode
        {
            get { return _blnIsTestMode; }
        }


        #endregion


        public SelectParametersViewModel(List<ReportParameter> lst, string strStatement, ParameterViewModel.ValueFormat valueFormat, bool blnIsTestMode = false)
        {
            _valueFormat = valueFormat;
            _blnIsTestMode = blnIsTestMode;

            //Assign original statement which is used to build _strStatement from selected parameters. Also set _strStatement to original statement to start with.
            _strOrigStatement = _strStatement = strStatement;

            _blnIsCancelled = true;
            _lstParameters = lst.Select(x => new ParameterViewModel(this, x)).ToList();

            foreach (var p in _lstParameters)
                p.OnValueChanged += p_OnValueChanged;

            //Load all lists that are not dependent on other parameters. 
            if (_lstParameters.Count > 0)
            {
                for (int i = 0; i < _lstParameters.Count; i++)
                {
                    var p = _lstParameters[i];

                    if (p.Parameter.IsSQLType && (_lstParameters.Count == 1 || !p.IsUsingAtleastOne(_lstParameters.Take(i))))
                        _lstParameters.First().LoadAsync(_lstParameters);

                    //Reset all to not focused
                    if (p.HasInitialFocus)
                        p.HasInitialFocus = false;
                }

                _lstParameters.First().HasInitialFocus = true;
            }
        }

        protected void p_OnValueChanged(ParameterViewModel obj)
        {
            UpdateStatement();
        }


        public void FireParameterChanged(ParameterViewModel pChanged)
        {
            if (pChanged == null)
                return;

            foreach (var p in _lstParameters)
            {
                if (p == pChanged)
                    continue;

                if (p.IsUsing(pChanged))
                    p.LoadAsync(_lstParameters);
            }
        }

       

        public void UpdateStatement()
        {
            _strStatement = _strOrigStatement;
            if (Parameters != null && Parameters.Count > 0)
            {
                //Order by parameter name length, to ensure parameters with almost equal names, are replaced correctly (i.e @paramYear and @paramYear2. If @paramYear was replaced first, @paramYear2 could not be found)
                foreach (var p in Parameters.Where(x => x.Parameter != null && x.Parameter.ParameterName != null && !x.Parameter.IsFileType).OrderByDescending(x => x.Parameter.ParameterName.Length))
                {
                    if (!_strStatement.Contains(p.Parameter.ParameterName, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    string strValue = p.GetValue(_valueFormat);

                    if (strValue != null)
                        _strStatement = Regex.Replace(_strStatement, p.Parameter.ParameterName, strValue, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                }
            }
            
            RaisePropertyChanged(() => Statement);
        }


        public string ReplaceStringWithParameterFileNameDisplayValues(string str)
        {
            var strOrig = str;

            if (Parameters != null && Parameters.Count > 0)
            {
                //Order by parameter name length, to ensure parameters with almost equal names, are replaced correctly (i.e @paramYear and @paramYear2. If @paramYear was replaced first, @paramYear2 would not be found)
                foreach (var p in Parameters.Where(x => x.Parameter != null && x.Parameter.ParameterName != null).OrderByDescending(x => x.Parameter.ParameterName.Length))
                {
                    if (!strOrig.Contains(p.Parameter.ParameterName, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    string strValue = p.GetValue(ParameterViewModel.ValueFormat.FileNameDisplay);

                    if (strValue == null)
                        strValue = "";

                    str = Regex.Replace(str, p.Parameter.ParameterName, strValue, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                }
            }

            return str;
        }


        public List<ReportFile> GetSupportFiles()
        {
            List<ReportFile> lst = new List<ReportFile>();

            if (Parameters != null && Parameters.Count > 0)
            {
                foreach (var p in Parameters.Where(x => x.Parameter.IsFileType && x.HasValue && System.IO.File.Exists(x.InternalValue)))
                {
                    ReportFile r = new ReportFile();
                    r.FileName = p.Parameter.ParameterName;
                    r.DecompressedData = p.InternalValue.LoadFileBytesFromPath(System.IO.FileShare.ReadWrite);
                    r.CompressData(true);
                    lst.Add(r);
                }
            }

            return lst;
        }


        protected override string ValidateField(string strFieldName)
        {
            //Only perform validation when user clicks "Save".
            if (_blnValidate)
            {
                switch (strFieldName)
                {
                    case "Parameters":
                        if (Parameters != null)
                        {
                            foreach (var p in Parameters.Where(x => !x.Parameter.IsOptional))
                            {
                                if (!p.Parameter.IsMultiSelect && !p.HasValue)
                                    return string.Format("'{0}' er obligatorisk. {1} inden du fortsætter.", p.Parameter.DisplayName, p.Parameter.IsValueType ? "Indtast venligst en værdi" : (p.Parameter.IsFileType ? "Vælg venligst en fil" : "Vælg venligst en værdi i listen"));
                                else if (p.Parameter.IsMultiSelect && !p.Values.Where(x => x.IsChecked).Any())
                                    return string.Format("'{0}' er obligatorisk. Vælg venligst (mindst) en værdi i listen inden du fortsætter.", p.Parameter.DisplayName);
                                else if (!p.Parameter.IsMultiSelect && p.Parameter.IsFileType && !System.IO.File.Exists(p.InternalValue))
                                    return string.Format("Filen du har valgt til '{0}' kunne ikke lokaliseres. Rediger venligst stien og prøv igen.", p.Parameter.DisplayName);
                            }

                            foreach(var p in Parameters.Where(x => x.Parameter.IsOptional ))
                            {
                                if(p.Parameter.IsFileType && p.HasValue && !System.IO.File.Exists(p.InternalValue))
                                    return string.Format("Filen du har valgt til '{0}' kunne ikke lokaliseres. Rediger venligst stien (eller fjern den) og prøv igen.", p.Parameter.DisplayName);
                            }
                        }
                        break;
                }
            }

            return null;
        }


        private void DeRegisterEvents()
        {
            foreach (var p in _lstParameters)
                p.OnValueChanged -= p_OnValueChanged;
        }


        #region Cancel Command


        public DelegateCommand CancelCommand
        {
            get { return _cmdCancel ?? (_cmdCancel = new DelegateCommand(Cancel)); }
        }

        public void Cancel()
        {
            _blnIsCancelled = true;

            DeRegisterEvents();

            Close();
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

            DeRegisterEvents();

            UpdateStatement();

            _blnIsCancelled = false;
            Close();
        }


        #endregion
    }
}
