using Anchor.Core;
using Anchor.Core.Comparers;
using Babelfisk.ViewModels.SmartDots;
using Babelfisk.WPF.Infrastructure.DataGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
    /// Interaction logic for SDSelectAnimalsView.xaml
    /// </summary>
    public partial class SDSelectAnimalsView : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<string> _lstSortBy = new List<string>();

        private UniversalComparer _uc;


        public SDSelectAnimalsViewModel ViewModel
        {
            get { return this.DataContext as SDSelectAnimalsViewModel; }
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

        #region input constrain to numbers only
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void PreviewTextInputNumbers(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = !IsTextAllowed(e.Text);
        }

        #endregion

        public string SortByColumnNamesString
        {
            get { return _lstSortBy == null || _lstSortBy.Count == 0 ? "" : string.Join(" -> ", _lstSortBy); }
        }

        public SDSelectAnimalsView()
        {
            InitializeComponent();
            dataGrid.PreviewKeyDown += DataGrid_PreviewKeyDown;

            try
            {
                dataGrid.Sorting += DataGrid_Sorting;

                var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGrid));
                if (dpd != null)
                {
                    dpd.AddValueChanged(dataGrid, DataGridItemsSourceChanged);
                }
            }
            catch (Exception)
            {

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
                    defaultColumnSort = dgSender.Columns.Where(x => x.SortMemberPath != null && x.SortMemberPath.Equals("SelectionAnimal.AnimalId", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (defaultColumnSort != null)
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

        /*   private void DataGridItemsSourceChanged(object sender, EventArgs e)
           {
               new Action(() =>
               {
                   var dgSender = sender as DataGrid;
                   var cView = CollectionViewSource.GetDefaultView(dgSender.Items);
                   if (cView.SortDescriptions.Count == 0)
                   {
                       var c = dgSender.Columns.Where(x => x.SortMemberPath != null && x.SortMemberPath.Equals("SelectionAnimal.AnimalId", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                       if (c != null)
                       {
                           cView.SortDescriptions.Add(new SortDescription(c.SortMemberPath, ListSortDirection.Descending));
                           c.SortDirection = ListSortDirection.Descending;
                       }
                   }
                   HandleSortByDescription(sender as DataGrid);
               }).Dispatch();
           }


           private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
           {
               //Put it action, so the method is called after the sorting has completed.
               new Action(() =>
               {
                   HandleSortByDescription(sender as DataGrid);
               }).Dispatch();
           }


           /// <summary>
           /// Look at the collection source sort descriptors, to get find out what the grid is sorted by and to update the SortByColumnNames with the sorting.
           /// </summary>
           private void HandleSortByDescription(DataGrid dgSender)
           {
               try
               {
                   if (dgSender == null)
                       return;
                   var cView = CollectionViewSource.GetDefaultView(dgSender.Items);

                   List<string> sort = new List<string>();
                   if (cView != null)
                   {
                       foreach (var itm in cView.SortDescriptions)
                       {
                           if (string.IsNullOrWhiteSpace(itm.PropertyName))
                               continue;

                           var c = dgSender.Columns.Where(x => x.SortMemberPath != null && x.SortMemberPath.Equals(itm.PropertyName, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

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
                   }

                   SortByColumnNames = sort;
               }
               catch { }
           }*/

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && (Keyboard.IsKeyDown(Key.A) || e.Key == Key.A))
                {
                    if (ViewModel.FilteredAnimals.Count > 1000)
                    {
                        e.Handled = true;
                        ViewModel.AppRegionManager.ShowMessageBox("You can only select all animals when there is less than 1000 animals in the list.");
                        return;
                    }
                }
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
            var vm = (sender as CheckBox).DataContext as SelectionAnimalItem;
            vm.IsSelected = !vm.IsSelected;
            e.Handled = true;
        }


        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                foreach (var item in e.AddedItems.OfType<SelectionAnimalItem>())
                {
                    //System.Diagnostics.Debug.WriteLine("** Item {0} is added to selection.", item.SelectionAnimal.AnimalId);

                    if (!item.IsSelected)
                    {
                        // if bound data item still isn't selected, fix this
                        item.IsSelected = true;
                    }
                }
            }
            if (e.RemovedItems != null)
            {
                foreach (var item in e.RemovedItems.OfType<SelectionAnimalItem>())
                { 
                    //System.Diagnostics.Debug.WriteLine("** Item {0} is removed from selection.", item.SelectionAnimal.AnimalId);

                    if (item.IsSelected)
                    {
                        // if bound data item still is selected, fix this
                        item.IsSelected = false;
                    }
                }
            }

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

                if ((isAllSelected != null && !isAllSelected.Value) && ViewModel.FilteredAnimals.Count > 1000)
                {
                    ViewModel.AppRegionManager.ShowMessageBox(ViewModels.AViewModel.Translate("SDSampleView", "SelectAllSamplesError"));
                    return;
                }

                ViewModel.IsAllSelected = true;
                isAllSelected = ViewModel.IsAllSelected;
                var dg = dataGrid;
                if (isAllSelected.HasValue)
                {
                    if (isAllSelected.Value)
                    {
                        dg.SelectedItems.Clear();
                        foreach (var itm in ViewModel.FilteredAnimals)
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
    }
}
