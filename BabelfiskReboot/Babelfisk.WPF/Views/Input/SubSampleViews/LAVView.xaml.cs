using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Anchor.Core;
using Anchor.Core.Controls;
using Babelfisk.ViewModels;
using Babelfisk.ViewModels.Input;

namespace Babelfisk.WPF.Views.Input
{
    /// <summary>
    /// Interaction logic for LAVView.xaml
    /// </summary>
    public partial class LAVView : UserControl, IDisposable
    {
        public event Action OnGridLoaded;

        private Key? _keyLastPressedKey;

        public LAVViewModel ViewModel
        {
            get { return this.DataContext as LAVViewModel; }
        }

        public LAVView()
        {
            InitializeComponent();

            dataGrid.LoadingRow += dataGrid_LoadingRow;
            dataGrid.Sorting += dataGrid_Sorting;

            dataGrid.Loaded += new RoutedEventHandler(LAVView_Loaded);
            this.DataContextChanged += SpeciesListView_DataContextChanged;
            this.Unloaded += new RoutedEventHandler(SpeciesListView_Unloaded);
        }

        void LAVView_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && ViewModel.Items != null && ViewModel.Items.Count > 0 && ViewModel.SelectedItem == null)
                ViewModel.SelectedItem = ViewModel.Items[0];
        }

        /// <summary>
        /// Deregister OnScrollTo when species list is unloaded.
        /// </summary>
        protected void SpeciesListView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
                ViewModel.OnScrollTo -= ViewModel_OnScrollTo;
        }

        protected void SpeciesListView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is LAVViewModel)
                (e.OldValue as LAVViewModel).OnScrollTo -= ViewModel_OnScrollTo;

            if (e.NewValue is LAVViewModel)
            {
                (e.NewValue as LAVViewModel).OnScrollTo -= ViewModel_OnScrollTo;
                (e.NewValue as LAVViewModel).OnScrollTo += ViewModel_OnScrollTo;
            }
        }


        void KeyDownHandler(object sender, KeyEventArgs e)
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

        protected void ViewModel_OnScrollTo(ViewModels.AViewModel obj, string strTo)
        {
            switch (strTo)
            {
                case "NewItem":
                    if (ViewModel != null)
                    {
                        dataGrid.CommitEdit(DataGridEditingUnit.Row, true);

                        var emptyRow = ViewModel.Items.Where(x => x.IsNew && !x.IsDirty).FirstOrDefault();
                        if (emptyRow != null)
                        {
                             ViewModel.SelectedItem = emptyRow;

                             dataGrid.UpdateLayout();

                             dataGrid.ScrollIntoView(emptyRow);

                             dataGrid.UpdateLayout();

                             new Action(() =>
                             {
                                 var xe = dataGrid.ItemContainerGenerator.ContainerFromItem(emptyRow);
                                 if (xe != null)
                                 {
                                     var row = xe as DataGridRow;

                                     var cell = Anchor.Core.Controls.AncDataGrid.GetCell(dataGrid, row, 2);

                                     if (cell != null)
                                     {
                                         cell.Focus();
                                     }
                                 }
                             }).Dispatch();
                        }
                    }
                    break;
            }
        }

        protected void Cell_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void dataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //dataGrid.Focus();
            //Keyboard.Focus(dataGrid);
        }


        bool blnLoaded = false;
        protected void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //Set focus on first cell in the grid when it has loaded.
            if (!blnLoaded)
            {
                blnLoaded = true;

                if (OnGridLoaded != null)
                    OnGridLoaded();
                /*
                if (dataGrid.Items.Count > 0)
                {
                    DataGridRow dgrow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(dataGrid.Items[0]);

                    new Action(() =>
                    {
                        dgrow.Focus();
                        Keyboard.Focus(dgrow);
                    }).Dispatch();

                    new Action(() =>
                    {
                        dgrow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    }).Dispatch();
                }*/
            }
        }


        private void dataGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //dataGrid.Focus();
            //Keyboard.Focus(dataGrid);
        }


        private void dataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            //Make sure there are always to empty rows.
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(100);
                new Action(() =>
                {
                    if (ViewModel != null)
                    {
                        ViewModel.SyncNewRows();
                    }
                }).Dispatch();
            });

            if (ViewModel != null)
                ViewModel.RefreshSums();
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


        #endregion


        #region Perform custom sorting


        void dataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // switch off the datagrid's built-in sorting and handle via custom logic
            e.Handled = true;

            switch (e.Column.SortDirection)
            {
                case ListSortDirection.Ascending:
                    e.Column.SortDirection = ListSortDirection.Descending;
                    break;

                case ListSortDirection.Descending:
                default:
                    e.Column.SortDirection = ListSortDirection.Ascending;
                    break;
            }

            string column = e.Column.SortMemberPath;

            // delegate to the view model for actual sorting logic
            ViewModel.Sort(column, e.Column.SortDirection);
        }


        #endregion


        private void PreviewKeyDown_Handler(object sender, KeyEventArgs e)
        {
            dataGrid.ArrowKeyNavigation(sender, e.Key);
        }

        private void PreviewKeyUp_Handler(object sender, KeyEventArgs e)
        {

        }


        public void Dispose()
        {
            try
            {
                if (this.DataContext is LAVViewModel)
                    (this.DataContext as LAVViewModel).OnScrollTo -= ViewModel_OnScrollTo;

                this.DataContext = null;
            }
            catch { }
        }
    }
}
