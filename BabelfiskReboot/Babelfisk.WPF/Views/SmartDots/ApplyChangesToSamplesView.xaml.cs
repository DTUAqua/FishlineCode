using Anchor.Core;
using Anchor.Core.Comparers;
using Babelfisk.Entities.Sprattus;
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
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for ApplyChangesToSamplesView.xaml
    /// </summary>
    public partial class ApplyChangesToSamplesView : UserControl, INotifyPropertyChanged
    {
        private Key? _keyLastPressedKey;

        public event PropertyChangedEventHandler PropertyChanged;

        private List<string> _lstSortBy = new List<string>();

        private UniversalComparer _uc;


        #region Properties


        public ApplyChangesToSamplesViewModel ViewModel
        {
            get { return this.DataContext as ApplyChangesToSamplesViewModel; }
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



        public ApplyChangesToSamplesView()
        {
            try
            {
                InitializeComponent();

                dataGrid.Sorting += DataGrid_Sorting;

                var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGrid));
                if (dpd != null)
                {
                    dpd.AddValueChanged(dataGrid, DataGridItemsSourceChanged);
                }
            }
            catch { }
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
                    defaultColumnSort = dgSender.Columns.Where(x => x.SortMemberPath != null && x.SortMemberPath.Equals("Sample.animalId", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if(defaultColumnSort != null)
                        defaultColumnSort.SortDirection = ListSortDirection.Ascending; //Here ascending is descending and vice versa, since HandleDataGridSorting, will flip it, is if the user clicked the column
                }

                HandleDataGridSorting(sender as DataGrid, new DataGridSortingEventArgs(defaultColumnSort));
            }
            catch { }
        }


        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            HandleDataGridSorting(sender as DataGrid, e);

            e.Handled = true;
        }

        /// <summary>
        /// Look at the collection source sort descriptors, to get find out what the grid is sorted by and to update the SortByColumnNames with the sorting.
        /// </summary>
        private void HandleDataGridSorting(DataGrid dgSender, DataGridSortingEventArgs e)
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

                SortByColumnNames = sort;
            }
            catch { }
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


        private void CheckBox_Initialized(object sender, EventArgs e)
        {
            dataGrid.CheckBox_Initialized(sender, e, _keyLastPressedKey);
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

        private void CheckBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void CheckBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //e.Handled = true;
            var vm = (sender as CheckBox).DataContext as ModifiedSampleItem;
            vm.IsSelected = !vm.IsSelected;
            e.Handled = true;
        }




    }


    public class CompareItemCell : FrameworkElement, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        public string Value
        {
            get { return GetValue(ValueProperty) as string; }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(CompareItemCell), new UIPropertyMetadata(null, ValueChangedCallback));

        public static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as CompareItemCell;
            if (c != null)
                c.RaiseProperties();
        }

        public string NewValue
        {
            get { return GetValue(NewValueProperty) as string; }
            set { SetValue(NewValueProperty, value); }
        }

        public static readonly DependencyProperty NewValueProperty = DependencyProperty.Register("NewValue", typeof(string), typeof(CompareItemCell), new UIPropertyMetadata(null, NewValueChangedCallback));

       

        public static void NewValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as CompareItemCell;
            if (c != null)
                c.RaiseProperties();
        }

       
        public bool IsValuesEqual
        {
            get { return Value == NewValue; }
        }


        private void RaiseProperties()
        {
            RaisePropertyChanged("IsValuesEqual");
        }

        public CompareItemCell()
        {
        }


        public void RaisePropertyChanged(string str)
        {
            var evt = PropertyChanged;
            if (evt != null)
                evt(this, new PropertyChangedEventArgs(str));
        }
    }
}



