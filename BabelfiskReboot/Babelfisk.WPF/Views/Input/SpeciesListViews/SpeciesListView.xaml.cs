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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Anchor.Core;
using Anchor.Core.Controls;
using Babelfisk.ViewModels;
using Babelfisk.ViewModels.Input;
using Babelfisk.BusinessLogic;
using System.Windows.Controls.Primitives;

namespace Babelfisk.WPF.Views.Input
{
    /// <summary>
    /// Interaction logic for SpeciesListView.xaml
    /// </summary>
    public partial class SpeciesListView : UserControl, IDisposable
    {
        private Key? _keyLastPressedKey;

        public SpeciesListViewModel ViewModel
        {
            get { return this.DataContext as SpeciesListViewModel; }
        }

        public SpeciesListView()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;
           
            dataGrid.LoadingRow += dataGrid_LoadingRow;
            dataGrid.Sorting += dataGrid_Sorting;
            this.Loaded += SpeciesListView_Loaded;

            this.DataContextChanged += SpeciesListView_DataContextChanged;
            this.Unloaded += new RoutedEventHandler(SpeciesListView_Unloaded);
        }

        void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        /// <summary>
        /// Deregister OnScrollTo when species list is unloaded.
        /// </summary>
        protected void SpeciesListView_Unloaded(object sender, RoutedEventArgs e)
        {
            if(ViewModel != null)
                ViewModel.OnScrollTo -= ViewModel_OnScrollTo;
        }

        protected void SpeciesListView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is SpeciesListViewModel)
                (e.OldValue as SpeciesListViewModel).OnScrollTo -= ViewModel_OnScrollTo;

            if (e.NewValue is SpeciesListViewModel)
            {
                (e.NewValue as SpeciesListViewModel).OnScrollTo -= ViewModel_OnScrollTo;
                (e.NewValue as SpeciesListViewModel).OnScrollTo += ViewModel_OnScrollTo;
            }
        }


        protected void ViewModel_OnScrollTo(ViewModels.AViewModel obj, string strTo)
        {
            switch (strTo)
            {
                case "SelectedItem":
                    if (ViewModel != null && ViewModel.SelectedItem != null)
                    {
                        //make sure layout is updated.
                        dataGrid.UpdateLayout();

                        dataGrid.ScrollIntoView(ViewModel.SelectedItem);

                        dataGrid.UpdateLayout();

                        new Action(() =>
                        {
                            var xe = dataGrid.ItemContainerGenerator.ContainerFromItem(ViewModel.SelectedItem);

                            if (xe != null)
                            {
                                var row = xe as DataGridRow;

                                var cell = Anchor.Core.Controls.AncDataGrid.GetCell(dataGrid, row, 2);

                                row.Focus();

                                if (cell != null)
                                    cell.Focus();
                            }
                        }).Dispatch();
                    }

                    break;

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

                                    row.Focus();
                                    if (cell != null)
                                    {
                                        dataGrid.ScrollIntoView(cell);
                                        cell.Focus();
                                        try { Keyboard.Focus(cell); } catch  {}
                                    }

                                }
                            }).Dispatch();
                        }
                    }
                    break;
            }
        }


        



        protected void SpeciesListView_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeCollapsibleStoryBoard();
        }

        protected void Cell_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Below performs single click editing
            /*
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(100);
                new Action(() =>
                {
                    DataGridCell cell = null;
                    if (!dataGrid.IsEditing())
                    {
                        dataGrid.BeginEdit();
                    }
                    if (!dataGrid.IsCellEditing(ref cell) && cell != null)
                    {
                        cell.IsEditing = true;
                    }
                }).Dispatch();
            });*/
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


        private void dataGrid_Loaded(object sender, RoutedEventArgs e)
        {
         //   dataGrid.Focus();
         //   Keyboard.Focus(dataGrid);
        }


        bool blnLoaded = false;
        protected void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //Set focus on first cell in the grid when it has loaded.
            if (!blnLoaded && (ViewModel != null && ViewModel.IsNew))
            {
                blnLoaded = true;

                if (ViewModel != null && ViewModel.IsNew && ViewModel.TripType.IsSEA())
                {
                    new Action(() =>
                    {
                        tbStationNumber.Focus();
                        Keyboard.Focus(tbStationNumber);

                    }).Dispatch();
                }
                else if (dataGrid.Items.Count > 0)
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
                }
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
                        //ViewModel.IsDirty = true;
                    }
                }).Dispatch();
            });

            if(ViewModel != null)
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


        /// <summary>
        /// Method initializing the height of the row details grid (synching it with the storyboard/doubleanimation).
        /// </summary>
        private void InitializeCollapsibleStoryBoard()
        {
            if (BusinessLogic.Settings.Settings.Instance.IsSpeciesListRowDetailsCollapsed)
                grdFilter.Height = 0;
        }


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
                if (this.DataContext is SpeciesListViewModel)
                    (this.DataContext as SpeciesListViewModel).OnScrollTo -= ViewModel_OnScrollTo;

                this.DataContext = null;
            }
            catch { }
        }
    }
}
