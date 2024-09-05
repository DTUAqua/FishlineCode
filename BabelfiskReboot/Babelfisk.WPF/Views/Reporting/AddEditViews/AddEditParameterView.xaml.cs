using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Babelfisk.ViewModels.Reporting.AddEditModels;
using Anchor.Core;

namespace Babelfisk.WPF.Views.Reporting.AddEditViews
{
    /// <summary>
    /// Interaction logic for AddEditParameterView.xaml
    /// </summary>
    public partial class AddEditParameterView : UserControl
    {
        private bool isManualEditCommit;

        public AddEditParameterViewModel ViewModel
        {
            get { return this.DataContext as AddEditParameterViewModel; }
        }



        public AddEditParameterView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Control_Loaded);
            this.DataContextChanged += AddEditParameterView_DataContextChanged;
            SetupAvalonEdit();
        }


        protected void AddEditParameterView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Make sure window centers on the screen after load.
            new Action(() =>
            {
                try
                {
                    Application curApp = Application.Current;
                    Window mainWindow = curApp.MainWindow;
                    var w = Window.GetWindow(this);
                    if (w != null)
                    {
                        w.Left = mainWindow.Left + (mainWindow.ActualWidth - this.ActualWidth) / 2;
                        w.Top = mainWindow.Top + (mainWindow.ActualHeight - Math.Min(550, this.ActualHeight)) / 2;
                    }
                }
                catch { }
            }).Dispatch();
        }


        private void SetupAvalonEdit()
        {

            using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Babelfisk.WPF.Resources.TextDefinitions.sql.xshd"))
            {
                using (var reader = new System.Xml.XmlTextReader(stream))
                {
                    avQuery.SyntaxHighlighting =  ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader,  ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
                }
            }
        }


        protected void Control_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(tbName);
            tbName.SelectAll();
        }


        private void DataGrid_RowEditEnding_1(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (!isManualEditCommit)
            {
                isManualEditCommit = true;
                DataGrid grid = (DataGrid)sender;
                grid.CommitEdit(DataGridEditingUnit.Row, true);
                isManualEditCommit = false;
                ViewModel.RowEditingEnding();
            }


        }

  

        private void DataGrid_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                DataGridRow rowContainer = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(dataGrid.CurrentItem);
                if (rowContainer != null)
                {
                    int columnIndex = dataGrid.Columns.IndexOf(dataGrid.CurrentColumn);
                    DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                    if (columnIndex == 0 || columnIndex == 1)
                    {
                        DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                        TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                        request.Wrapped = false;
                        //  cell.MoveFocus(request);
                        dataGrid.BeginEdit();
                    }
                    else if (columnIndex == 2 && presenter != null)
                    {
                        dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                    }
                }
            }
        }


        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }


        public void list_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                new Action(() =>
                {
                    var w = Window.GetWindow(this);
                    w.SizeToContent = SizeToContent.Manual;
                    w.Height = 550;
                }).Dispatch();
            }
        }


        public void value_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Reset height of window if Value-parameter is selected (assuming height is set to 550, because list-parameter type has been selected).
            if ((bool)e.NewValue == false)
            {
                new Action(() =>
                {
                    var w = Window.GetWindow(this);
                    w.Height = Double.NaN;
                    w.SizeToContent = SizeToContent.Height;
                }).Dispatch();
            }

        }


        private void txtClipboard_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Right)
                {
                    var tb = (sender as TextBlock);

                    if (tb != null && tb.Text != null)
                    {
                        string strText = tb.Text;
                        int intIndex = 0;
                        if ((intIndex = strText.IndexOf('<')) > 0)
                            strText = strText.Substring(0, intIndex);

                        strText.CopyToClipboard();
                    }
                }
            }
            catch { }
        }

        private void avQuery_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.F))
            {
                avQuery.TextArea.DefaultInputHandler.NestedInputHandlers.Add(new ICSharpCode.AvalonEdit.Search.SearchInputHandler(avQuery.TextArea));
            }
        }
    }
}
