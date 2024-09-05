using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Babelfisk.Entities;
using Anchor.Core;
using System.Text.RegularExpressions;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Forms;

namespace Babelfisk.ViewModels.Reporting.ReportExecuteModels
{
    public  class ParameterViewModel : AViewModel
    {
        public event Action<ParameterViewModel> OnValueChanged;

        private DelegateCommand _cmdRefresh;
        private DelegateCommand _cmdBrowse;


        public enum ValueFormat
        {
            SQL,
            RScript,
            None,
            FileNameDisplay
        }

        private SelectParametersViewModel _vmParameters;

        private ReportParameter _parameter;

        private string _strInternalValue;

        private ObservableCollection<ParameterListItem> _lst;
        private ObservableCollection<ParameterListItem> _lstAllItems;

        private bool _blnIsUpdatingList;

        private ObservableCollection<ParameterListItem> _lstPreselectItems;

        private bool _blnIsInitialized = false;

        public bool _blnHasInitialFocus;

        private bool _blnFilterByReferencedParameter;




        #region Properties


        public ReportParameter Parameter
        {
            get { return _parameter; }
            set
            {
                _parameter = value;
                RaisePropertyChanged(() => Parameter);
            }
        }


        public bool HasInitialFocus
        {
            get { return _blnHasInitialFocus; }
            set
            {
                _blnHasInitialFocus = value;
                RaisePropertyChanged(() => HasInitialFocus);
            }
        }


        public string InternalValue
        {
            get { return _strInternalValue; }
            set
            {
                if (_blnIsUpdatingList)
                    return;

                _strInternalValue = value;
                RaisePropertyChanged(() => InternalValue);
                RaisePropertyChanged(() => HasValue);
                FireValueChanged();
            }
        }


        public bool HasValue
        {
            get { return !string.IsNullOrWhiteSpace(_strInternalValue); }
        }


        public ObservableCollection<ParameterListItem> Values
        {
            get { return _lst; }
            set
            {
                if (_lst != null)
                {
                    foreach (var p in _lst)
                        p.PropertyChanged -= ListItemChanged;
                }

                _lst = value;

                if (_lst != null)
                {
                    foreach (var p in _lst)
                        p.PropertyChanged += ListItemChanged;
                }

                RaisePropertyChanged(() => Values);
            }
        }


        public double TextBoxHeight
        {
            get
            {
                int intDefaultHeight = 26;
                if (_parameter.NumberOfLines < 2)
                    return intDefaultHeight;

                return (intDefaultHeight*0.87) * _parameter.NumberOfLines;
            }
        }


        public bool IsTextBoxMultiline
        {
            get { return _parameter != null && _parameter.NumberOfLines > 1; }
        }


        public bool HasTextBoxMaxLength
        {
            get { return _parameter != null && _parameter.MaximumLength > 0; }
        }


        public int TextBoxMaxLength
        {
            get { return _parameter != null ? _parameter.MaximumLength : 0; }
        }


        public bool HasReferencedParameter
        {
            get { return ReferencedParameter != null && ReferencedParameter.Parameter.IsMultiSelect; }
        }


        public ParameterViewModel ReferencedParameter
        {
            get { return (_vmParameters == null || _parameter == null || !_parameter.ReferenceParameterIdFilterBy.HasValue || !_parameter.IsMultiSelect) ? null : _vmParameters.Parameters.Where(x => x._parameter.Id == _parameter.ReferenceParameterIdFilterBy.Value).FirstOrDefault(); }
        }


        public bool FilterByReferencedParameter
        {
            get { return _blnFilterByReferencedParameter; }
            set
            {
                _blnFilterByReferencedParameter = value;
                RaisePropertyChanged(() => FilterByReferencedParameter);

                AssignList(_lstAllItems.ToObservableCollection());
            }
        }


        #endregion


        public ParameterViewModel(SelectParametersViewModel pvm, ReportParameter repParameter)
        {
            _blnIsInitialized = false;
            _vmParameters = pvm;
            _parameter = repParameter;
            Values = repParameter.List;
        }


        public void ListItemChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "IsChecked")
            {
                FireValueChanged();
            }
        }


        public void FireValueChanged()
        {
            if (OnValueChanged != null)
                OnValueChanged(this);
        }




        public string GetValue(ValueFormat enmFormat = ValueFormat.None)
        {
            string strVal = null;

            bool blnIsString = _parameter.ReturnType == "String";
            bool blnIsCheckList = _parameter.SelectionMode == "CheckBoxList";
            bool blnIsValue = _parameter.ParameterType == "Value";
            bool blnIsSQL  = _parameter.ParameterType == "SQL";
            bool blnIsList = _parameter.ParameterType == "List";
            bool blnIsFile = _parameter.ParameterType == "File";

            switch(enmFormat)
            {
                case ValueFormat.FileNameDisplay:
                    if (blnIsValue || !blnIsCheckList)
                        strVal = InternalValue ?? "";
                    else
                        strVal = string.Join("-", _lst.Where(x => x.IsChecked).Select(x => x.DisplayName));
                    break;

                case ValueFormat.None:
                    if (blnIsValue || !blnIsCheckList)
                        strVal = InternalValue ?? "";
                    else
                        strVal = string.Join(", ", _lst.Where(x => x.IsChecked).Select(x => x.Value));
                    break;

                case ValueFormat.SQL:
                    if(blnIsValue || !blnIsCheckList) //DropDown, single value
                    {
                        //if (blnIsList && string.IsNullOrEmpty(InternalValue))
                        //    break;

                        strVal = string.Format("{0}{1}{2}", blnIsString ? "'" : "", InternalValue ?? "", blnIsString ? "'" : "");
                    } //SQL or list
                    else
                    {
                        strVal = string.Join(", ", _lst.Where(x => x.IsChecked).Select(x => string.Format("{0}{1}{2}", blnIsString ? "'" : "", x.Value ?? "", blnIsString ? "'" : "")));
                    }

                    break;

                case ValueFormat.RScript:
                    if (blnIsValue || !blnIsCheckList) //DropDown, single value
                    {
                       // if ((blnIsList || blnIsSQL) && InternalValue == null)
                       //     break;

                        strVal = string.Format("{0}{1}{2}", blnIsString ? "\"" : "", InternalValue ?? "", blnIsString ? "\"" : "");
                    } //SQL or list
                    else
                    {
                        strVal = string.Join(",", _lst.Where(x => x.IsChecked).Select(x => string.Format("{0}{1}{2}", blnIsString ? "\"" : "", x.Value ?? "", blnIsString ? "\"" : "")));
                    }
                    break;
            }

            return strVal; 
        }


        public bool IsUsing(ParameterViewModel pvm)
        {
            if (this.Parameter.ParameterType == "SQL" && this.Parameter.Statement != null && this.Parameter.Statement.Contains(pvm.Parameter.ParameterName, StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (this.HasReferencedParameter && pvm.Parameter.Id == this.Parameter.ReferenceParameterIdFilterBy && pvm.Parameter.IsMultiSelect && this.Parameter.IsMultiSelect)
                return true;

            return false;
        }

        public bool IsUsingAtleastOne(IEnumerable<ParameterViewModel> pvms)
        {
            foreach (var pvm in pvms)
            {
                if (this.Parameter.ParameterType == "SQL" && this.Parameter.Statement != null && this.Parameter.Statement.Contains(pvm.Parameter.ParameterName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            return false;
        }


        public void LoadAsync(List<ParameterViewModel> lstParameters)
        {
            //If this is a Value or static list parameter, nothing needs updating.
            if (this.Parameter.ParameterType != "SQL")
            {
                _blnIsInitialized = true;
                return;
            }

            IsLoading = true;
            Task.Factory.StartNew(() => Load(lstParameters)).ContinueWith(t => new Action(() => IsLoading = false).Dispatch());
        }


        public void Load(List<ParameterViewModel> lstParameters)
        {
            ObservableCollection<ParameterListItem> lst = new ObservableCollection<ParameterListItem>();
            try
            {
                string str = this.Parameter.Statement;

                foreach (var p in lstParameters)
                {
                    if (!str.Contains(p.Parameter.ParameterName, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    string strValue = p.GetValue(ParameterViewModel.ValueFormat.SQL);

                    if (string.IsNullOrEmpty(strValue))
                        return;

                    str = Regex.Replace(str, p.Parameter.ParameterName, strValue, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                }

                var man = new BusinessLogic.Reporting.ReportingManager();

                var res = man.ExecuteSQLStatement(str);

                lst = res.ToObservableCollection();
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);

            }


            new Action(() =>
            {
                _blnIsInitialized = true;
                _lstAllItems = lst;
                AssignList(lst.ToObservableCollection());
                _blnIsInitialized = true;
            }).Dispatch();
        }


        public void AssignSelection(ParameterViewModel pvm)
        {
            this._strInternalValue = pvm.InternalValue;

            if(_parameter.IsMultiSelect && pvm.Values != null && this.Values != null)
            {
                if (this.Values.Count > 0)
                {
                    foreach (var li in pvm.Values)
                    {
                        ParameterListItem pli = null;
                        if (li.IsChecked && (pli = this.Values.Where(x => x.DisplayName == li.DisplayName).FirstOrDefault()) != null)
                            pli.IsChecked = li.IsChecked;
                    }
                }
                else
                {
                    if (!_blnIsInitialized)
                        _lstPreselectItems = pvm.Values;
                }
            }

            if (_parameter.IsListType && !Values.Where(x => x.Value == InternalValue).Any())
                InternalValue = null;
        }



        public void AssignList(ObservableCollection<ParameterListItem> lst)
        {
            try
            {
                if (lst != null)
                {
                    if(this.Parameter.ParameterType != "SQL")
                        lst = lst.OrderBy(x => x.DisplayName).ToObservableCollection();

                    _blnIsUpdatingList = true;

                    //Filter items by referenced parameter.
                    if (_blnFilterByReferencedParameter && HasReferencedParameter && this.Parameter.IsMultiSelect)
                    {
                        var pRef = _vmParameters.Parameters.Where(x => x._parameter.Id == _parameter.ReferenceParameterIdFilterBy.Value).FirstOrDefault();

                        if (pRef != null && pRef._parameter.IsMultiSelect && pRef.Values != null)
                        {
                            foreach (var l in _lstAllItems)
                                if (!pRef.Values.Where(x => x.IsChecked && x.Value == l.Value).Any())
                                    lst.Remove(l);
                        }
                    }

                    ObservableCollection<ParameterListItem> lstOld = this.Values;

                    if (_lstPreselectItems != null)
                        lstOld = _lstPreselectItems;

                    if (lstOld != null && lstOld.Count > 0)
                    {
                        foreach (var itm in lstOld.Where(x => x.IsChecked))
                        {
                            ParameterListItem pli = null;
                            if ((pli = lst.Where(x => x.DisplayName == itm.DisplayName).FirstOrDefault()) != null)
                                pli.IsChecked = true;
                        }
                    }
                }
                Values = lst;
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
            finally
            {
                _blnIsUpdatingList = false;
                _lstPreselectItems = null;

                if (!_parameter.IsValueType && !Values.Where(x => x.Value == InternalValue).Any())
                    InternalValue = null;
                else
                    InternalValue = InternalValue;
            }
        }



        #region Refresh Command


        public DelegateCommand RefreshCommand
        {
            get {  return _cmdRefresh ?? (_cmdRefresh = new DelegateCommand(() => LoadAsync(_vmParameters.Parameters)));}
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
                var dlg = new System.Windows.Forms.OpenFileDialog();
                dlg.Multiselect = false;

                if (!string.IsNullOrWhiteSpace(InternalValue))
                    dlg.FileName = InternalValue;
                dlg.Title = "Vælg en fil.";

                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    InternalValue = dlg.FileName;
                }
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }


        #endregion
    }
}
