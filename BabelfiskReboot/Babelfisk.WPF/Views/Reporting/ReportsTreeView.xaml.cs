using System;
using System.Collections.Generic;
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
using Babelfisk.ViewModels.Reporting;

namespace Babelfisk.WPF.Views.Reporting
{
    /// <summary>
    /// Interaction logic for ReportsTreeView.xaml
    /// </summary>
    public partial class ReportsTreeView : UserControl
    {
        private ReportDropHandler _dropHandler;

        public ReportsTreeViewModel ViewModel
        {
            get { return this.DataContext as ReportsTreeViewModel; }
        }


        public ReportDropHandler DropHandler
        {
            get { return _dropHandler ?? (_dropHandler = new ReportDropHandler() { TreeViewModel = ViewModel }); }
        }


        public ReportsTreeView()
        {
            InitializeComponent();

            this.DataContextChanged += ReportsTreeView_DataContextChanged;
        }

        protected void ReportsTreeView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue != null && _dropHandler != null)
            {
                _dropHandler.TreeViewModel = ViewModel;
            }
        }


        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (ViewModel != null)
            {
                if (treeView.SelectedItem is Entities.Sprattus.Report)
                {
                    var rep = treeView.SelectedItem as Entities.Sprattus.Report;

                    if (rep != null)
                        ViewModel.SelectedItem = new ReportViewModel(rep);
                }
                else if(ViewModel.SelectedItem != null)
                    ViewModel.SelectedItem = null;
            }
        }


        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        private void treeView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem tvi = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (e.RightButton == MouseButtonState.Pressed && tvi != null && tvi.Header is Babelfisk.Entities.INodeItem)
            {
                (tvi.Header as Babelfisk.Entities.INodeItem).IsSelected = true;
            }
            else if (tvi == null)
            {
                if (treeView.SelectedItem != null && treeView.SelectedItem is Babelfisk.Entities.INodeItem)
                    (treeView.SelectedItem as Babelfisk.Entities.INodeItem).IsSelected = false;
            }
        }


        /// <summary>
        /// Method which brings the selected treeviewitem into view.
        /// </summary>
        private void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            if (!Object.ReferenceEquals(sender, e.OriginalSource))
                return;

            TreeViewItem item = e.OriginalSource as TreeViewItem;
            if (item != null)
            {
                EventHandler eventHandler = null;
                eventHandler = new EventHandler(delegate
                {
                    treeView.LayoutUpdated -= eventHandler;
                    try
                    {
                        item.BringIntoView();
                    }
                    catch { }
                });
                treeView.LayoutUpdated += eventHandler;
            }
        }

      
    }
}
