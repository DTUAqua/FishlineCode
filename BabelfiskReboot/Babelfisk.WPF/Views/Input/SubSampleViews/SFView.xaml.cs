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
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;

namespace Babelfisk.WPF.Views.Input
{
    /// <summary>
    /// Interaction logic for SFView.xaml
    /// </summary>
    public partial class SFView : UserControl, IDisposable
    {
        public DelegateCommand<object> _cmdCopyLinkToClipboard;
        public DelegateCommand<object> _cmdOpenLinkInExplorer;

        public event Action OnGridLoaded;

        private Key? _keyLastPressedKey;

        public SFViewModel ViewModel
        {
            get { return this.DataContext as SFViewModel; }
        }

        public SFView()
        {
            InitializeComponent();

            dataGrid.LoadingRow += dataGrid_LoadingRow;
            dataGrid.Sorting += dataGrid_Sorting;

            dataGrid.Loaded += new RoutedEventHandler(LAVView_Loaded);
            this.DataContextChanged += SpeciesListView_DataContextChanged;
            this.Unloaded += new RoutedEventHandler(SpeciesListView_Unloaded);
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
            if (e.OldValue is SFViewModel)
                (e.OldValue as SFViewModel).OnScrollTo -= ViewModel_OnScrollTo;

            if (e.NewValue is SFViewModel)
            {
                (e.NewValue as SFViewModel).OnScrollTo -= ViewModel_OnScrollTo;
                (e.NewValue as SFViewModel).OnScrollTo += ViewModel_OnScrollTo;
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

                                    row.Focus();

                                    if (cell != null)
                                    {
                                        cell.Focus();
                                    }
                                }
                            }).Dispatch();
                        }
                    }
                    break;

                case "FocusNext":
                    try
                    {
                        TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
                        UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

                        if (keyboardFocus != null)
                        {
                            keyboardFocus.MoveFocus(tRequest);
                        }

                       // Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(delegate { }));
                    }
                    catch { }
                    break;
            }
        }


        private void ScrollToEndGrid()
        {
            if (dataGrid.Items.Count > 0)
            {
                var border = VisualTreeHelper.GetChild(dataGrid, 0) as Decorator;
                if (border != null)
                {
                    var scroll = border.Child as ScrollViewer;
                    if (scroll != null) scroll.ScrollToEnd();
                }
            }
        }


        void LAVView_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && ViewModel.Items != null && ViewModel.Items.Count > 0 && ViewModel.SelectedItem == null)
                ViewModel.SelectedItem = ViewModel.Items[0];
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

        protected void Cell_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void dataGrid_Loaded(object sender, RoutedEventArgs e)
        {
           // dataGrid.Focus();
           // Keyboard.Focus(dataGrid);
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

               /* if (dataGrid.Items.Count > 0)
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


        private void PreviewKeyDown_Handler(object sender, KeyEventArgs e)
        {
            dataGrid.ArrowKeyNavigation(sender, e.Key);
        }

        private void PreviewKeyUp_Handler(object sender, KeyEventArgs e)
        {

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


        #region Otolith popup panel events


        public CustomPopupPlacement[] popupPanelOtolithImages_placePopup(Size popupSize, Size targetSize, System.Windows.Point offset)
        {
            CustomPopupPlacement placement1 = new CustomPopupPlacement(new System.Windows.Point(0, targetSize.Height + 2), PopupPrimaryAxis.Horizontal);

            CustomPopupPlacement[] ttplaces = new CustomPopupPlacement[] { placement1 };
            return ttplaces;
        }


        private void popupPanelOtolithImages_Closed(object sender, EventArgs e)
        {
            var ai = (sender as ChildPopup).Tag as AnimalItem;

            if (ai != null)
            {
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    System.Threading.Thread.Sleep(100);
                    new Action(() =>
                    {
                        //Make sure popup is closed
                         if (ai.IsAnimalFilesPopupOpen)
                             ai.IsAnimalFilesPopupOpen = false;
                    }).Dispatch();
                });
            }
        }


        #endregion


        #region Otolith image link events


        //Make sure popup does not close when clicking inside the box
        private void bdrOtolithBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }


        //Make sure popup does not close when clicking inside the box
        private void bdrOtolithBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }


        #endregion



        private void miAnimalIdCopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var mi = sender as MenuItem;

                if (mi != null && mi.Tag as AnimalItem != null)
                {
                    Clipboard.SetText((mi.Tag as AnimalItem).AnimalId);
                }
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }



        #region Open link in explorer command


        public DelegateCommand<object> OpenLinkInExplorerCommand
        {
            get { return _cmdOpenLinkInExplorer ?? (_cmdOpenLinkInExplorer = new DelegateCommand<object>(t => OpenLinkInExplorer(t as Babelfisk.Entities.Sprattus.AnimalFile))); }
        }


        public void OpenLinkInExplorer(Babelfisk.Entities.Sprattus.AnimalFile af)
        {
            if (af != null)
            {
                af.IsLoading = true;
                Task.Factory.StartNew(() =>
                {
                    AnimalItem.GoToPath(af.FullFilePath);
                }).ContinueWith(t => new Action(() => af.IsLoading = false).Dispatch());
            }
        }

        #endregion


        #region Copy Link To Clipboard Command


        public DelegateCommand<object> CopyLinkToClipboardCommand
        {
            get { return _cmdCopyLinkToClipboard ?? (_cmdCopyLinkToClipboard = new DelegateCommand<object>(t => CopyLinkToClipboard(t as Babelfisk.Entities.Sprattus.AnimalFile))); }
        }


        public void CopyLinkToClipboard(Babelfisk.Entities.Sprattus.AnimalFile af)
        {
            if(af != null)
                Clipboard.SetText(af.FullFilePath); 
        }



        #endregion

        public void Dispose()
        {
            try
            {
                if (this.DataContext is SFViewModel)
                    (this.DataContext as SFViewModel).OnScrollTo -= ViewModel_OnScrollTo;

                dataGrid.LoadingRow -= dataGrid_LoadingRow;
                dataGrid.Sorting -= dataGrid_Sorting;

                dataGrid.Loaded -= new RoutedEventHandler(LAVView_Loaded);
                this.DataContextChanged -= SpeciesListView_DataContextChanged;
                this.Unloaded -= new RoutedEventHandler(SpeciesListView_Unloaded);

                var lst = dataGrid.FindAllVisualChildren<ChildPopup>();

                if (lst != null && lst.Count > 0)
                {
                    lst.ForEach(x =>
                    {
                        if (x is IDisposable)
                            (x as IDisposable).Dispose();
                    });
                }

                this.DataContext = null;
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

                SubSampleViews.SFAnimalItemSDInfoView.LoadPreviewWindow(b.DataContext);
            }
            catch { }
        }
    }
}
