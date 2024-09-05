using Anchor.Core;
using Anchor.Core.Comparers;
using Babelfisk.ViewModels.SmartDots;
using Babelfisk.WPF.Infrastructure.DataGrid;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Babelfisk.WPF.Views.SmartDots
{
    /// <summary>
    /// Interaction logic for ImportFromCSVView.xaml
    /// </summary>
    public partial class ImportFromCSVView : UserControl, INotifyPropertyChanged, IDisposable
    {
        private Key? _keyLastPressedKey;

        public event PropertyChangedEventHandler PropertyChanged;

        private List<string> _lstSortBy = new List<string>();

        private UniversalComparer _uc;


        #region Properties


        public ImportFromCSVViewModel ViewModel
        {
            get { return this.DataContext as ImportFromCSVViewModel; }
        }


        public List<string> SortByColumnNames
        {
            get
            {
                return _lstSortBy;
            }
            private set
            {
                _lstSortBy = value;
                RaisePropertyChanged("SortByColumnNames");
                RaisePropertyChanged("SortByColumnNamesString");
            }
        }


        public string SortByColumnNamesString
        {
            get { return _lstSortBy == null || _lstSortBy.Count == 0 ? "" : string.Join(" -> ", _lstSortBy); }
        }


        #endregion



        public ImportFromCSVView()
        {
            try
            {
                InitializeComponent();
                this.DataContextChanged += ImportFromCSVView_DataContextChanged;
                dataGrid.PreviewKeyDown += DataGrid_PreviewKeyDown;

                dataGrid.Sorting += DataGrid_Sorting;
                dataGridMessages.Sorting += DataGridMessages_Sorting;

                var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGrid));
                if (dpd != null)
                    dpd.AddValueChanged(dataGrid, DataGridItemsSourceChanged);

                var dpdm = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGrid));
                if (dpdm != null)
                    dpdm.AddValueChanged(dataGridMessages, DataGridMessagesItemsSourceChanged);
            }
            catch { }
        }

      

        private void ImportFromCSVView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if ((e.OldValue as ImportFromCSVViewModel) != null)
                    (e.OldValue as ImportFromCSVViewModel).OnUIMessageParameter -= ImportFromCSVView_OnUIMessageParameter;

                ImportFromCSVViewModel mvm = null;
                if ((mvm = e.NewValue as ImportFromCSVViewModel) != null)
                    mvm.OnUIMessageParameter += ImportFromCSVView_OnUIMessageParameter;
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }


        private void ImportFromCSVView_OnUIMessageParameter(ViewModels.AViewModel arg1, string task, object param)
        {
            if (string.IsNullOrWhiteSpace(task))
                return;

            switch(task)
            {
                case "ScrollToErrorWarning":
                    ScrollToWarningOrError(param as SDMessageItem);
                    break;
            }
        }


        private void ScrollToWarningOrError(SDMessageItem sdmi)
        {
            try
            {
                if (sdmi == null || dataGridMessages == null || dataGridMessages.Items.Count == 0)
                    return;

                dataGridMessages.ScrollIntoView(dataGridMessages.Items[dataGridMessages.Items.Count - 1]); //scroll to last
                dataGridMessages.UpdateLayout();
                dataGridMessages.ScrollIntoView(sdmi);
            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }


        private void DataGridItemsSourceChanged(object sender, EventArgs e)
        {
            try
            {
                //Assign default sorting, whenever the datasource changes.
                var dgSender = sender as DataGrid;
                DataGridColumn defaultColumnSort = null;
                if (dgSender != null && dgSender.ItemsSource != null)
                {
                    defaultColumnSort = dgSender.Columns.Where(x => x.SortMemberPath != null && x.SortMemberPath.Equals("CSVRowNumber", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if(defaultColumnSort != null)
                        defaultColumnSort.SortDirection = ListSortDirection.Descending; //Here ascending is descending and vice versa, since HandleDataGridSorting, will flip it, is if the user clicked the column
                }

                HandleDataGridSorting(sender as DataGrid, new DataGridSortingEventArgs(defaultColumnSort));
            }
            catch { }
        }

        private void DataGridMessagesItemsSourceChanged(object sender, EventArgs e)
        {
            try
            {
                //Assign default sorting, whenever the datasource changes.
                var dgSender = sender as DataGrid;
                DataGridColumn defaultColumnSort = null;
                if (dgSender != null && dgSender.ItemsSource != null)
                {
                    defaultColumnSort = dgSender.Columns.Where(x => x.SortMemberPath != null && x.SortMemberPath.Equals("SampleItem.CSVRowNumber", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (defaultColumnSort != null)
                        defaultColumnSort.SortDirection = ListSortDirection.Descending; //Here ascending is descending and vice versa, since HandleDataGridSorting, will flip it, is if the user clicked the column
                }

                HandleDataGridSorting(sender as DataGrid, new DataGridSortingEventArgs(defaultColumnSort), false);
            }
            catch { }
        }

        private void DataGridMessages_Sorting(object sender, DataGridSortingEventArgs e)
        {
            HandleDataGridSorting(sender as DataGrid, e, false);

            e.Handled = true;
        }


        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            HandleDataGridSorting(sender as DataGrid, e);

            e.Handled = true;
        }


        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (e.AddedItems != null)
            {
                foreach (var item in e.AddedItems.OfType<SDSampleItem>())
                {
                    if (!item.IsSelected)
                    {
                        // if bound data item still isn't selected, fix this
                        item.IsSelected = true;
                    }
                }
            }
            if (e.RemovedItems != null)
            {
                foreach (var item in e.RemovedItems.OfType<SDSampleItem>())
                {
                    if (item.IsSelected)
                    {
                        // if bound data item still is selected, fix this
                        item.IsSelected = false;
                    }
                }
            }

            e.Handled = true;

        }


        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && (Keyboard.IsKeyDown(Key.A) || e.Key == Key.A))
                {
                   
                }
            }
            catch { }
        }


     


        /// <summary>
        /// Look at the collection source sort descriptors, to get find out what the grid is sorted by and to update the SortByColumnNames with the sorting.
        /// </summary>
        private void HandleDataGridSorting(DataGrid dgSender, DataGridSortingEventArgs e, bool isMainGrid = true)
        {
            try
            {
                if (dgSender == null)
                    return;

                var cView = CollectionViewSource.GetDefaultView(dgSender.ItemsSource) as ListCollectionView;

                List<string> sort = new List<string>();
                if (cView != null)
                {
                    var isShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

                    if (!isShiftDown && _uc != null)
                        _uc.Reset();

                    if (_uc == null)
                        _uc = new UniversalComparer(new Anchor.Core.Comparers.FastStringNumberComparer());

                    if (e != null && e.Column != null && !string.IsNullOrWhiteSpace(e.Column.SortMemberPath))
                    {
                        var dir = e.Column.SortDirection == null ? ListSortDirection.Ascending : (e.Column.SortDirection.Value == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending);
                        _uc.ReplaceOrAddProperty(e.Column.SortMemberPath, dir == ListSortDirection.Descending);
                        e.Column.SortDirection = dir;
                    }

                    foreach (var itm in _uc.Properties)
                    {
                        if (string.IsNullOrWhiteSpace(itm.Key))
                            continue;

                        var c = dgSender.Columns.Where(x => x.SortMemberPath != null && x.SortMemberPath.Equals(itm.Key, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                        if (c == null)
                            continue;

                        string header = null;
                        DoubleColumnHeader dch = c.Header as DoubleColumnHeader;
                        if (dch != null)
                            header = string.Format("{0} ({1})", string.IsNullOrWhiteSpace(dch.LowerHeaderToolTip) ? dch.LowerHeader : dch.LowerHeaderToolTip, c.SortDirection == ListSortDirection.Ascending ? "Asc" : "Dsc");

                        if (string.IsNullOrWhiteSpace(header))
                            continue;

                        sort.Add(header);
                    }

                    cView.CustomSort = _uc;
                }

                if(isMainGrid)
                    SortByColumnNames = sort;
            }
            catch { }
        }


        private void CheckBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }


        private void CheckBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //e.Handled = true;
            var vm = (sender as CheckBox).DataContext as SDSampleItem;
            vm.IsSelected = !vm.IsSelected;
            e.Handled = true;
        }


        private void selectAllCheckBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }


        private void selectAllCheckBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                bool? isAllSelected = ViewModel.IsAllSelected;


                ViewModel.IsAllSelected = true;

                isAllSelected = ViewModel.IsAllSelected;
                var dg = dataGrid;
                if (isAllSelected.HasValue)
                {
                    if (isAllSelected.Value)
                    {
                        dg.SelectedItems.Clear();
                        foreach (var itm in ViewModel.FilteredSDSampleItems)
                            dg.SelectedItems.Add(itm);
                    }
                    else
                        dg.SelectedItems.Clear();
                }
            }
            catch
            {

            }
            finally
            {
                e.Handled = true;
            }
        }

        protected void KeyDownHandler(object sender, KeyEventArgs e)
        {
            var isNumber = e.Key.IsNumber(true);
            var isLetter = e.Key.IsLetterOrSpace();

            _keyLastPressedKey = e.Key;
            if (e.IsDown && (e.Key == Key.Return || isNumber || isLetter))
            {
                DataGridCell cell = null;
                //Enter datagrid editing mode when hitting enter on a cell
                if (!dataGrid.IsEditing())
                {
                    dataGrid.BeginEdit();
                    e.Handled = true; //Don't move focus to next row
                }
                else if (!dataGrid.IsCellEditing(ref cell) && cell != null)
                {
                    cell.IsEditing = true;
                    if (e.Key != Key.Return)
                        e.Handled = true;
                }
            }
            else if (e.IsDown && e.Key == Key.Escape)
            {
                if (dataGrid.IsEditing())
                {
                    dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                }
            }
        }


     
        #region Column value controls initialized


        private void TextBox_Initialized(object sender, EventArgs e)
        {
            dataGrid.TextBox_Initialized(sender, e, _keyLastPressedKey);
        }


        private void FilteredComboBox_Initialized(object sender, EventArgs e)
        {
            dataGrid.FilteredComboBox_Initialized(sender, e, _keyLastPressedKey);

        }


        private void DropDownTextBox_Initialized(object sender, EventArgs e)
        {
            dataGrid.DropDownTextBox_Initialized(sender, e, _keyLastPressedKey);
        }



        private void PreviewKeyDown_Handler(object sender, KeyEventArgs e)
        {
            dataGrid.ArrowKeyNavigation(sender, e.Key);
        }
        private void PreviewKeyUp_Handler(object sender, KeyEventArgs e)
        {

        }

        #endregion



        public void RaisePropertyChanged(string propertyName)
        {
            try
            {
                var evt = PropertyChanged;
                if (evt != null)
                    evt(this, new PropertyChangedEventArgs(propertyName));
            }
            catch { }
        }

        public void Dispose()
        {
            try
            {
                if (ViewModel != null)
                    ViewModel.OnUIMessageParameter -= ImportFromCSVView_OnUIMessageParameter;
            }
            catch { }
        }


        private void dataGridMessages_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            /*if (e.Key == Key.A && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                dataGrid.SelectAll();
            }
            else */if (e.Key == Key.C && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                PasteSelectedRowsToClipboard(true);
            }
        }

        private void PasteSelectedRowsToClipboard(bool selectedItemsOnly = false)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(SDMessageItem.GetCSVHeader());

                IEnumerable<SDMessageItem> items = null;

                if (selectedItemsOnly)
                {
                    if (dataGridMessages == null || dataGridMessages.SelectedItems == null || dataGridMessages.SelectedItems.Count == 0)
                        return;
                    items = dataGridMessages.SelectedItems.OfType<SDMessageItem>();
                }
                else
                {
                    if (ViewModel == null || ViewModel.WarningAndErrorMessages == null)
                        return;

                    items = ViewModel.WarningAndErrorMessages;
                }

                items = items.OrderBy(x => x.SampleItem.CSVRowNumber);

                foreach (SDMessageItem dr in items)
                {
                    sb.AppendLine(dr.ToCSVValues());
                }

                Clipboard.SetText(sb.ToString());
            }
            catch(Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e); 
            }
        }

        private void btnCopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PasteSelectedRowsToClipboard();
            }
            catch { }
        }


        private void ImagesBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs ee)
        {
            if (ee.ClickCount != 2)
                return;

            try
            {
                var b = (sender as Border);

                if (b == null)
                    return;

                OtolithImagesPreview.LoadPreviewWindow(b.DataContext, "Sample.SDFile", OtolithImagesPreview.SourceBindingType.SDFiles);
            }
            catch { }
        }
    }
}



