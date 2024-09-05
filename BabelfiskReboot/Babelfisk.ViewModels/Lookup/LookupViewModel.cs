using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Anchor.Core.Controls;
using Babelfisk.Entities;
using Babelfisk.Entities.Sprattus;
using Microsoft.Practices.Prism.Commands;
using Anchor.Core;

namespace Babelfisk.ViewModels.Lookup
{
    public class LookupViewModel : AViewModel
    {
        private DelegateCommand<object> _cmdRemove;
        private DelegateCommand _cmdAdd;
        private DelegateCommand _cmdSave;


        /// <summary>
        /// List with all lookups (used for populating _colLookupsUI).
        /// </summary>
        private List<ILookupEntity> _colLookupsAll;


        /// <summary>
        /// List containing lookups displayed on UI
        /// </summary>
        private List<ILookupEntity> _colLookupsUI;


        private ILookupEntity _selectedLookup;


        /// <summary>
        /// Collection containing lookup grid columns.
        /// </summary>
        private ObservableCollection<DataGridColumn> _columnCollection;


        /// <summary>
        /// Reference to the lookup type
        /// </summary>
        private LookupType _lookupType;


        /// <summary>
        /// Lookup search field value.
        /// </summary>
        private string _strSearchValue;

        private object _objExactSearchValue;

        private string _strExactSearchProperty;


        private bool _blnIsLocked;

        private bool _blnIsChild;


        private LookupViewModel _childLookup;


        private LookupViewModel _parentLookup;

        private LookupManagerViewModel _lookupManager;

        #region Properties

        public List<ILookupEntity> LookupSet
        {
            get { return _colLookupsUI; }
            set
            {
                _colLookupsUI = value;
                RaisePropertyChanged(() => LookupSet);
                RaisePropertyChanged(() => AllLookupsCount);
            }
        }


        public int AllLookupsCount
        {
            get { return _colLookupsAll != null ? _colLookupsAll.Count : 0; }
        }


        public string Header
        {
            get { return _lookupType.Name; }
        }

        public string HeaderLowerCase
        {
            get { return _lookupType.Name.ToLower(); }
        }

        public string Message
        {
            get { return _lookupType == null ? "" : _lookupType.Message; }
            set
            {
                if(_lookupType != null)
                    _lookupType.Message = value;
                RaisePropertyChanged(() => Message);
                RaisePropertyChanged(() => HasMessage);
            }
        }


        public bool HasMessage
        {
            get { return !string.IsNullOrWhiteSpace(Message); }
        }


        public string SearchValue
        {
            get { return _strSearchValue; }
            set
            {
                _strSearchValue = value;
                Filter();
                RaisePropertyChanged(() => SearchValue);
            }
        }


        public string LookupTypeString
        {
            get { return typeof(ILookupEntity).Name; }
        }


        public bool IsLocked
        {
            get { return _blnIsLocked; }
            private set
            {
                _blnIsLocked = value;
                RaisePropertyChanged(() => IsLocked);
            }
        }


        /// <summary>
        /// Get/Set collection of grid columns
        /// </summary>
        public ObservableCollection<DataGridColumn> ColumnCollection
        {
            get
            {
                return _columnCollection;
            }
            protected set
            {
                _columnCollection = value;
                RaisePropertyChanged(() => ColumnCollection);
            }
        }


        public LookupViewModel ChildLookup
        {
            get { return _childLookup; }
            set
            {
                _childLookup = value;
                RaisePropertyChanged(() => ChildLookup);
                RaisePropertyChanged(() => HasChildLookup);
                RaisePropertyChanged(() => ShowChildLookupGrid);
            }
        }


        public bool HasChildLookup
        {
            get { return _childLookup != null; }
        }


        public bool ShowChildLookupGrid
        {
            get { return HasChildLookup && SelectedLookup != null; }
        }


        public ILookupEntity SelectedLookup
        {
            get { return _selectedLookup; }
            set
            {
                if (ChildLookup != null && !ChildLookup.IsLookupsValid())
                    return;

                _selectedLookup = value;

                FilterChild();
                RaisePropertyChanged(() => SelectedLookup);
                RaisePropertyChanged(() => ShowChildLookupGrid);
                RaisePropertyChanged(() => HasSelectedLookup);
            }
        }


        public bool HasSelectedLookup
        {
            get { return _selectedLookup != null; }
        }


        public LookupType LookupType
        {
            get { return _lookupType; }
        }


        public bool CanEditOffline
        {
            get
            {
                return _lookupType.CanEditOffline;
            }
        }


        public bool IsOffline
        {
            get { return BusinessLogic.Settings.Settings.Instance.OfflineStatus.IsOffline; }
        }

        #endregion


        /// <summary>
        /// Use parameterized constructor.
        /// </summary>
        private LookupViewModel() { }

        public LookupViewModel(LookupManagerViewModel lookupManager, LookupType lt, LookupViewModel parent = null)
        {
            _lookupManager = lookupManager;
            _lookupType = lt;
            _blnIsChild = lt is ChildLookupType;
            _parentLookup = parent;

            InitializeAsync();

            //If this is not a child/nested lookup, register to key down event.
            if(parent == null)
                RegisterToKeyDown();
        }


        /// <summary>
        /// Initializes lookup view model asynchronously
        /// </summary>
        private void InitializeAsync()
        {
            IsLoading = true;

            if (_lookupType.ChildLookupType != null)
                ChildLookup = new LookupViewModel(_lookupManager, _lookupType.ChildLookupType, this);

            Task.Factory.StartNew(Initialize);
        }


        /// <summary>
        /// Initialize view model loading lookup values and lookup columns.
        /// </summary>
        private void Initialize()
        {
            try
            {
                BusinessLogic.LookupManager lookupMan = new BusinessLogic.LookupManager();
                var lst = lookupMan.GetLookups(_lookupType.Type, null, _lookupType.TypeIncludes);

                //Load lookup type lists
                _lookupType.LoadLists(lookupMan);

                var comp = new Anchor.Core.Comparers.NaturalComparer(true);
                lst = lst.OrderBy(x => x.DefaultSortValue, comp).ToList();

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    var columns = CreateColumns();
                    _colLookupsAll = lst;
                    ColumnCollection = columns;
                    Filter();
                }));
            }
            catch (Exception e)
            {
                DispatchMessageBox(String.Format("{0}. {1}", Translater.Translate("Errors", "1"), e.Message));
            }
            finally
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    IsLoading = false;
                }));
            }
        }


        #region Column creation methods

        /// <summary>
        /// Create data grid columns
        /// </summary>
        private ObservableCollection<DataGridColumn> CreateColumns()
        {
            ObservableCollection<DataGridColumn> col = new ObservableCollection<DataGridColumn>();
            Style cellStyle = Application.Current.FindResource("GridCellStyle") as Style;
            Style readOnlyCellStyle = Application.Current.FindResource("GridCellReadOnlyStyle") as Style;

            int intColumnAutoWidthCount = Math.Max(_lookupType.Columns.Where(x => !x.Width.HasValue).Count(), 1);
            
            for (int i = 0; i < _lookupType.Columns.Length; i++)
            {
                LookupColumn c = _lookupType.Columns[i];
                DataGridColumn co = null;

                if (c.IsReadOnly)
                    IsLocked = true;

                switch (c.ColumnType)
                {
                    case LookupColumn.LookupColumnType.TextBox:
                        co = new DataGridTextColumn();
                        SetBinding(co as DataGridTextColumn, c);
                        break;

                    case LookupColumn.LookupColumnType.Dropdown:
                        var cc = c as ComboboxLookupColumn;
                        co = CreateComboboxColumn(cc);

                        /*var itmSource = GetPropertyValue(_lookupType.Type, cc.ItemsSource) as System.Collections.IEnumerable;
                        co = new DataGridComboBoxColumn()
                        {
                            SelectedItemBinding = new Binding(c.BindingProperty),
                            ItemsSource = itmSource,
                            DisplayMemberPath = cc.DisplayValue
                        };*/
                        break;

                    case LookupColumn.LookupColumnType.CheckBox:
                        co = CreateCheckboxColumn(c); //new DataGridCheckBoxColumn();                   
                       // SetBinding(co as DataGridColumn, c);
                        break;
                }

                if (c.MinWidth != null)
                    co.MinWidth = c.MinWidth.Value;

                co.Header = c;
                if(c.Width.HasValue)
                    co.Width = c.Width.HasValue ? c.Width.Value : new DataGridLength(100 / intColumnAutoWidthCount, DataGridLengthUnitType.Star);

                if (c.IsReadOnly)
                    co.CellStyle = readOnlyCellStyle;
               
                col.Add(co);
            }

            return col;
        }


        private DataGridColumn CreateCheckboxColumn(LookupColumn cc)
        {
            DataGridTemplateColumn dgtc = new DataGridTemplateColumn();

            Style cbStyle = Application.Current.FindResource("sCheckBox2") as Style;

            //Create cell template
            FrameworkElementFactory facCell = new FrameworkElementFactory(typeof(CheckBox));
            Binding bCell = new Binding(cc.BindingProperty);
            facCell.SetValue(CheckBox.IsCheckedProperty, bCell);
            facCell.SetValue(CheckBox.StyleProperty, cbStyle);
            facCell.SetValue(CheckBox.IsEnabledProperty, false);
            facCell.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
            facCell.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            facCell.SetValue(CheckBox.MarginProperty, new Thickness(5,0,0,0));

            dgtc.CanUserSort = true;
            dgtc.SortMemberPath = bCell.Path.Path;

            //Edit template
            FrameworkElementFactory facEdit = new FrameworkElementFactory(typeof(CheckBox));
            Binding bSelectedItem = new Binding(cc.BindingProperty);
            bSelectedItem.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            facEdit.SetValue(CheckBox.IsCheckedProperty, bSelectedItem);
            facEdit.SetValue(CheckBox.StyleProperty, cbStyle);
            facEdit.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);
            facEdit.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            facEdit.SetValue(CheckBox.MarginProperty, new Thickness(5, 0, 0, 0));

            Binding bFocus = new Binding();
            bFocus.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
            facEdit.SetValue(System.Windows.Input.FocusManager.FocusedElementProperty, bFocus);


            DataTemplate dtCell = new DataTemplate();
            dtCell.VisualTree = facCell;
            DataTemplate dtEdit = new DataTemplate();
            dtEdit.VisualTree = facEdit;

            dgtc.CellTemplate = dtCell;
            dgtc.CellEditingTemplate = dtEdit;

            return dgtc;
        }


        /// <summary>
        /// Create combobox column dynamically so FilteredComboBox can be used
        /// </summary>
        private DataGridColumn CreateComboboxColumn(ComboboxLookupColumn cc)
        {
            var itmSource = cc.GetComboboxItems(_lookupType.Type);
            DataGridTemplateColumn dgtc = new DataGridTemplateColumn();

            //Create cell template
            FrameworkElementFactory facCell = new FrameworkElementFactory(typeof(TextBlock));
            
            Binding bCell= null;
            if(cc.NonEditDisplayPath != null)
                bCell = new Binding(cc.NonEditDisplayPath);
            else
                bCell = new Binding(cc.BindingProperty + (!string.IsNullOrWhiteSpace(cc.DisplayValuePath) ? "." : "") + (cc.DisplayValuePath ?? ""));
            facCell.SetValue(TextBlock.TextProperty, bCell);
            facCell.SetValue(TextBlock.PaddingProperty, new Thickness(3, 0, 3, 0));
            DataTemplate dtCell = new DataTemplate();
            dtCell.VisualTree = facCell;

            dgtc.CanUserSort = true;
            dgtc.SortMemberPath = bCell.Path.Path;

            //Create edit template
            FrameworkElementFactory facEdit = new FrameworkElementFactory(typeof(FilteredComboBox));
            FrameworkElementFactory fecEditVirt = new FrameworkElementFactory(typeof(VirtualizingStackPanel));
            Binding bSelectedItem = new Binding(cc.BindingProperty);
            bSelectedItem.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            ItemsPanelTemplate virtTemp = new ItemsPanelTemplate();
            virtTemp.VisualTree = fecEditVirt;
           

            Binding bFocus = new Binding();
            bFocus.RelativeSource = new RelativeSource(RelativeSourceMode.Self);

            if (cc.SelectedValuePath != null)
            {
                facEdit.SetValue(FilteredComboBox.SelectedValueProperty, bSelectedItem);
                if(!string.IsNullOrWhiteSpace(cc.SelectedValuePath))
                    facEdit.SetValue(FilteredComboBox.SelectedValuePathProperty, cc.SelectedValuePath);
            }
            else
                facEdit.SetValue(FilteredComboBox.SelectedItemProperty, bSelectedItem);


            //  facEdit.SetValue(FilteredComboBox.IsEnabledProperty, false);


            //facEdit.SetValue(FilteredComboBox.IsSynchronizedWithCurrentItemProperty, true);
            if (itmSource != null)
                facEdit.SetValue(FilteredComboBox.ItemsSourceProperty, itmSource);
            else
            {
                Binding bSource = new Binding(cc.ItemsSource);
                facEdit.SetValue(FilteredComboBox.ItemsSourceProperty, bSource);
            }


            if (!string.IsNullOrWhiteSpace(cc.DisplayValuePath))
                facEdit.SetValue(FilteredComboBox.DisplayMemberPathProperty, cc.DisplayValuePath);
            facEdit.SetValue(FilteredComboBox.ItemsPanelProperty, virtTemp);
            facEdit.SetValue(FilteredComboBox.MinimumSearchLengthProperty, 1);
            facEdit.SetValue(FilteredComboBox.IsEditableProperty, true);
            facEdit.SetValue(FilteredComboBox.IsTextSearchCaseSensitiveProperty, false);
            //Focus combobox on edit
            facEdit.SetValue(System.Windows.Input.FocusManager.FocusedElementProperty, bFocus);

            DataTemplate dtEdit = new DataTemplate();
            dtEdit.VisualTree = facEdit;

            dgtc.CellTemplate = dtCell;

            if (!cc.IsEditReadOnly)
            dgtc.CellEditingTemplate = dtEdit;

            return dgtc;
        }


        /// <summary>
        /// Set DataGridTextColumn binding and add any wanted custom converters (for handling decimals and doubles i.e)
        /// </summary>
        private static Type _typeNullableDouble = typeof(Nullable<Double>);
        private static Type _typeNullableDecimal = typeof(Nullable<Decimal>);
        private void SetBinding(DataGridTextColumn dgtc, LookupColumn c)
        {
            var prop = this._lookupType.Type.GetProperty(c.BindingProperty, BindingFlags.Public | BindingFlags.Instance);
            
            var b = new Binding(c.BindingProperty);

            if (prop != null)
            {
                Type t = prop.PropertyType;
           
                //Handle decimal seperator in decimal and doubles
                if (typeof(decimal).FullName.Equals(t.FullName) || _typeNullableDecimal.FullName.Equals(t.FullName))
                     b.Converter = new Anchor.Core.Converters.InvCultureStringToDecimalConverter();
                else if(typeof(double).FullName.Equals(t.FullName) || _typeNullableDouble.FullName.Equals(t.FullName))
                    b.Converter = new Anchor.Core.Converters.InvCultureStringToDoubleConverter();
            }

            if(c.TargetNullValue != null)
                b.TargetNullValue = c.TargetNullValue;

            dgtc.Binding = b;
        }


        private void SetBinding(DataGridCheckBoxColumn dgcc, LookupColumn c)
        {
            var prop = this._lookupType.Type.GetProperty(c.BindingProperty, BindingFlags.Public | BindingFlags.Instance);

            var b = new Binding(c.BindingProperty);
            dgcc.Binding = b;
        }


        /// <summary>
        /// Get property value from lookup type 't' with name 'strName' (using reflection)
        /// </summary>
        public static object GetPropertyValue(Type t, string strName)
        {
            PropertyInfo f = t.GetProperty(strName, BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            if(f != null)
                return f.GetValue(null, null);

            return null;
        }


        #endregion


        #region Filter methods


        /// <summary>
        /// Filter displayed lookups according to SearchValue (and filter deleted lookups away as well).
        /// </summary>
        private void Filter()
        {
            if (_colLookupsAll == null)
                return;

            IEnumerable<ILookupEntity> vLst = _colLookupsAll.Where(x => x.ChangeTracker.State != ObjectState.Deleted);

            if (!string.IsNullOrEmpty(SearchValue))
                vLst = vLst.Where(x => x.FilterValue != null && x.FilterValue.ToLower().Contains(SearchValue.ToLower()));

            if (!string.IsNullOrEmpty(_strExactSearchProperty))
                vLst = vLst.Where(x => (_objExactSearchValue == null ? "" : _objExactSearchValue).Equals(GetPropertyValue(x, _strExactSearchProperty)));

            LookupSet = vLst.ToList();
        }


        public void FilterChild()
        {
            if (!HasChildLookup || _lookupType.ChildLookupType == null)
                return;

            if (SelectedLookup == null)
                ChildLookup.ExatchFilter(null, null); //Reset search
            else
                ChildLookup.ExatchFilter(GetPropertyValue(SelectedLookup, _lookupType.ChildLookupType.ParentKeyPropertyName), _lookupType.ChildLookupType.ForeignKeyPropertyName);

        }


        private object GetPropertyValue(ILookupEntity lookup, string strPropertyName)
        {
            var prop = lookup.GetType().GetProperty(strPropertyName, BindingFlags.Public | BindingFlags.Instance);
            object val = prop.GetValue(lookup, new object[] { });

            return val;
        }


        private void SetPropertyValue(ILookupEntity lookup, string strPropertyName, object value)
        {
            var prop = lookup.GetType().GetProperty(strPropertyName, BindingFlags.Public | BindingFlags.Instance);
            prop.SetValue(lookup, value, new object[] { });
        }


        public void ExatchFilter(object filterValue, string strFilterProperty)
        {
            _objExactSearchValue = filterValue;
            _strExactSearchProperty = strFilterProperty;

            Filter();
        }


        #endregion



        #region Save Command


        public DelegateCommand SaveCommand
        {
            get
            {
                if (_cmdSave == null)
                    _cmdSave = new DelegateCommand(() => SaveChangesAsync());

                return _cmdSave;
            }
        }


        public void SaveChangesAsync()
        {
            IsLoading = true;

            //Save changes for selected entities
            Task.Factory.StartNew(() => SaveChanges());
        }


        public bool IsDirtyDeepCheck()
        {
            if (!IsDirty)
                return false;

            if (HasChildLookup && ChildLookup.IsDirtyDeepCheck())
                return true;

            return _colLookupsAll.Where(x => x.ChangeTracker.State != ObjectState.Unchanged).Count() > 0;
        }


        private bool IsLookupsValid()
        {
            if (_colLookupsAll == null)
                return true;

            var entityToSaveAndIndexes = _colLookupsAll.Select((x, i) => new Tuple<ILookupEntity, int>(x, i)).Where(x => x.Item1.ChangeTracker.State != ObjectState.Unchanged).ToList();
            List<ILookupEntity> lstEntitiesToSave = entityToSaveAndIndexes.Select(x => x.Item1).ToList();

            return IsLookupsValid(lstEntitiesToSave);
        }

        private bool IsLookupsValid(List<ILookupEntity> lstEntitiesToSave)
        {
            bool blnValid = true;

            foreach (ILookupEntity lookup in lstEntitiesToSave)
            {
                //Ignore deleted entities
                if (lookup.ChangeTracker.State == ObjectState.Deleted)
                    continue;

                string strError = lookup.GetErrors(_colLookupsAll);
                blnValid = strError == null;
                if (strError != null)
                {
                    DispatchMessageBox(strError);
                    break;
                }
            }

            return blnValid;
        }


        protected void SaveChanges(bool blnSaveDeletedOnly = false)
        {
            try
            {
                List<Tuple<ILookupEntity, int>> entityToSaveAndIndexes;
                if (blnSaveDeletedOnly)
                    entityToSaveAndIndexes = _colLookupsAll.Select((x, i) => new Tuple<ILookupEntity, int>(x, i)).Where(x => x.Item1.ChangeTracker.State == ObjectState.Deleted).ToList();
                else
                    entityToSaveAndIndexes = _colLookupsAll.Select((x, i) => new Tuple<ILookupEntity, int>(x, i)).Where(x => x.Item1.ChangeTracker.State != ObjectState.Unchanged).ToList();

                List<ILookupEntity> lstEntitiesToSave = entityToSaveAndIndexes.Select(x => x.Item1).ToList();

                if (!IsLookupsValid(lstEntitiesToSave))
                    return;

                if (HasChildLookup && !ChildLookup.IsLookupsValid())
                    return;

                DatabaseOperationResult dRes = DatabaseOperationResult.CreateSuccessResult();

                //Save deleted entities
                if (HasChildLookup)
                    ChildLookup.SaveChanges(true);

                if (lstEntitiesToSave.Count > 0)
                {
                    BusinessLogic.LookupManager lMan = new BusinessLogic.LookupManager();
                    dRes = lMan.SaveLookups(ref lstEntitiesToSave);
                }

                //Save the rest of the entities
                if (HasChildLookup)
                    ChildLookup.SaveChanges();

                if (dRes.DatabaseOperationStatus == DatabaseOperationStatus.Successful)
                {

                    if (_lookupManager != null)
                        new Action(() => { _lookupManager.ChangesSaved = true; }).Dispatch();

                    Dispatcher.BeginInvoke(new Action(() => { IsDirty = false; }));
                    if(!blnSaveDeletedOnly)
                        Initialize();
                    /*List<int> indexes = entityToSaveAndIndexes.Select(x => x.Item2).ToList();
                    ResetEntities(lstEntitiesToSave, indexes);*/

                    if(!_blnIsChild)
                        DispatchMessageBox(Translater.Translate("LookupView", "SaveMessage"), 2);
                }
                else
                    DispatchMessageBox(Translater.Translate("LookupView", "SaveMessageError") + " " + dRes.Message);
            }
            catch (Exception e)
            {
                DispatchMessageBox(String.Format("{0}. {1}", Translater.Translate("Errors", "1"), e.Message));
            }
            finally
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    IsLoading = false;
                }));
            }
        }


        private void ResetEntities(List<ILookupEntity> savedEntities, List<int> indexes)
        {
            if (savedEntities.Count != indexes.Count)
                throw new ApplicationException("Index mismatch");

            for (int i = 0; i < indexes.Count; i++)
            {
                ILookupEntity l = savedEntities[i];
                int intIndex = indexes[i];

                //Skip deleted lookups in first go
                if (l.ChangeTracker.State == ObjectState.Deleted)
                    continue;
                else
                {
                    if (l is R_GearInfo)
                    {
                        R_GearInfo ll = l as R_GearInfo;

                        if(ll.L_GearInfoType != null)
                            ll.L_GearInfoType.AcceptChanges();
                        if(ll.L_Gear != null)
                            ll.L_Gear.AcceptChanges();
                    }

                    l.AcceptChanges();

                    _colLookupsAll[intIndex] = l;
                }
            }

            //Remove deleted entities
            _colLookupsAll.RemoveAll(x => x.ChangeTracker.State == ObjectState.Deleted);


            Filter();
        }


        #endregion


        #region Remove Command


        public DelegateCommand<object> RemoveCommand
        {
            get
            {
                if (_cmdRemove == null)
                    _cmdRemove = new DelegateCommand<object>(param => RemoveItem(param as ILookupEntity));

                return _cmdRemove;
            }
        }


        private void RemoveItem(ILookupEntity item)
        {
            if (item == null)
                return;

            //Show confirmation message if lookup to remove is an existing db value.
            if (item.ChangeTracker.State != ObjectState.Added)
            {
                var res =AppRegionManager.ShowMessageBox(Translater.Translate("LookupView", "RemoveConfirmation"), System.Windows.MessageBoxButton.YesNo);

                if (res == System.Windows.MessageBoxResult.No)
                    return;

                //Check database if value indeed can be deleted.
                IsLoading = true;

                Task.Factory.StartNew(CanDeleteLookupAsync, item);
            }
            else
            {
                _colLookupsAll.Remove(item);

                //Remove child items from parent as well (if the grid has children).
                if (_childLookup != null)
                {
                    var childItems = GetChildEntities(item);

                    foreach (var it in childItems)
                        _childLookup._colLookupsAll.Remove(it);
                    _childLookup.Filter();
                }

                IsDirty = true;

                if (_parentLookup != null)
                    _parentLookup.IsDirty = true;

                Filter();
            }
        }


        private List<ILookupEntity> GetChildEntities(ILookupEntity ilParent)
        {
            if (_childLookup._colLookupsAll == null)
                return new List<ILookupEntity>();

            IEnumerable<ILookupEntity> vLst = _childLookup._colLookupsAll.Where(x => x.ChangeTracker.State != ObjectState.Deleted);

            var value = GetPropertyValue(ilParent, (_childLookup._lookupType as ChildLookupType).ParentKeyPropertyName);

            if (!string.IsNullOrEmpty(_childLookup._strExactSearchProperty))
                vLst = vLst.Where(x => (value == null ? "" : value).Equals(GetPropertyValue(x, _childLookup._strExactSearchProperty)));

            return vLst.ToList();
        }


        private bool HasChildDependencies(List<Record> lst)
        {
            if (lst.Count > 1 || _childLookup == null || _childLookup.LookupType == null)
                return true;

            IEnumerable<ILookupEntity> vLst = _childLookup._colLookupsAll.Where(x => x.ChangeTracker.State != ObjectState.Deleted);

            if (!string.IsNullOrEmpty(_childLookup._strExactSearchProperty))
                vLst = vLst.Where(x => (_childLookup._objExactSearchValue == null ? "" : _childLookup._objExactSearchValue).Equals(GetPropertyValue(x, _childLookup._strExactSearchProperty)));

            return !(lst[0].Result.Equals(_childLookup.LookupType.Type.Name, StringComparison.InvariantCultureIgnoreCase) && (vLst.Count() == 0));
           
        }

        private void CanDeleteLookupAsync(object itm)
        {
            ILookupEntity item = itm as ILookupEntity;
            try
            {
                BusinessLogic.LookupManager lMan = new BusinessLogic.LookupManager();
                var lstDependencies = lMan.GetDependencies(item);
                bool canDelete = lstDependencies.Count == 0 || HasChildDependencies(lstDependencies) == false;

                if (!canDelete)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(Translater.Translate("LookupView", "DeleteError"));
                    sb.Append(" Følgende tabeller refererer nøgleværdien:");
                    for(int i = 0; i < lstDependencies.Count; i++)
                        sb.Append((i == 0 ? " " : ", ") + lstDependencies[i].Result);
                   
                    DispatchMessageBox(sb.ToString());
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        //Delete object
                        item.MarkAsDeleted();

                        IsDirty = true;

                        if (_parentLookup != null)
                            _parentLookup.IsDirty = true;

                        Filter();
                    }));
                }
            }
            catch (Exception e)
            {
                DispatchMessageBox(String.Format("{0}. {1}", Translater.Translate("Errors", "1"), e.Message));
            }
            finally
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    IsLoading = false;
                }));
            }
        }

        #endregion


        #region Add Command


        public DelegateCommand AddCommand
        {
            get
            {
                if (_cmdAdd == null)
                    _cmdAdd = new DelegateCommand(() => AddLookup());

                return _cmdAdd;
            }
        }


        private void AddLookup()
        {
            try
            {
                //Child lookups are not valid, do not create new parents
                if (ChildLookup != null && !ChildLookup.IsLookupsValid())
                    return;

                if (_parentLookup != null && !_parentLookup.IsLookupsValid())
                    return;

                ConstructorInfo ci = _lookupType.Type.GetConstructor(new Type[] {});
                object o = ci.Invoke(new object[] {});
                ILookupEntity lookup = o as ILookupEntity;

                //Set any default values
                lookup.NewLookupCreated();

                _colLookupsAll.Insert(0, lookup);

                //Make sure _objExactSearchValue is updated.
                if (_parentLookup != null)
                    _parentLookup.FilterChild();

                //Set foreign key as parent key by default.
                if (_blnIsChild && lookup != null && !string.IsNullOrEmpty(_strExactSearchProperty) && _objExactSearchValue != null)
                {
                    SetPropertyValue(lookup, _strExactSearchProperty, _objExactSearchValue);
                }

                IsDirty = true;

                if (_parentLookup != null)
                    _parentLookup.IsDirty = true;

                SearchValue = null;
                
                Filter();
            }
            catch (Exception e)
            {
                AppRegionManager.ShowMessageBox("En uventet fejl opstod. " + e.Message);
            }
        }


        #endregion


        public override void Dispose()
        {
            base.Dispose();

            if (_childLookup != null)
                _childLookup.Dispose();
        }


        /// <summary>
        /// Handle ctrl+n and ctrl+s
        /// </summary>
        /// <param name="e"></param>
        protected override void GlobelPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.N && (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl) || System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightCtrl)))
                AddLookup();
            else if (e.Key == System.Windows.Input.Key.S && (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl) || System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightCtrl)))
                SaveChanges();
        }
    }
}
