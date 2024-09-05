using Babelfisk.ViewModels.SmartDots.TreeViews;
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
using Anchor.Core;

namespace Babelfisk.WPF.Views.SmartDots.TreeViews
{
    /// <summary>
    /// Interaction logic for FolderTreeView.xaml
    /// </summary>
    public partial class FolderTreeView : UserControl
    {
        public FolderTreeView()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Currently it is handled by setting Focusable = false on the TreeViewItem to ignore the dummy nodes.
        /// This however has the sideeffect of selecting the node below the current one if using the right-arrow
        /// when the dummy-loading-node i shown. When time permits, more time could be used with the method below
        /// to select the dummy-node parent whenever i gets selected/focus.
        /// </summary>
        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var vParent = e.OldValue as FolderTreeViewItem;
            var vChild = e.NewValue as FolderTreeViewItem;

            if (vParent == null || vChild == null)
                return;

            if (vChild.IsDummy)
            {
                new Action(() =>
                {
                    vParent.IsSelected = true;
                }).Dispatch();
            }

            System.Diagnostics.Debug.WriteLine("Tree view item selected " + vChild.Header);
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

        private void Item_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement && (sender as FrameworkElement).TemplatedParent is ContentPresenter)
            {
                var c = ((sender as FrameworkElement).TemplatedParent as ContentPresenter).DataContext as FolderTreeViewItem;
                c.IsChecked = !c.IsChecked.HasValue ? false : !c.IsChecked;
                c.IsSelected = true;
                e.Handled = true;
            }
        }

        private void TreeView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var tvi = sender as TreeViewItem;

            if (tvi != null && (e.Key == Key.Return || e.Key == Key.Space) && tvi.Header is FolderTreeViewItem && (tvi.Header as FolderTreeViewItem).IsSelected)
            {
               /* if (tvi.Header is YearTreeItemViewModel)
                {
                    (tvi.Header as YearTreeItemViewModel).IsChecked = !(tvi.Header as YearTreeItemViewModel).IsChecked.HasValue ? false : !(tvi.Header as YearTreeItemViewModel).IsChecked;
                    (tvi.Header as YearTreeItemViewModel).IsSelected = true;
                }
                else if (tvi.Header is CruiseTreeItemViewModel)
                {
                    (tvi.Header as CruiseTreeItemViewModel).IsChecked = !(tvi.Header as CruiseTreeItemViewModel).IsChecked.HasValue ? false : !(tvi.Header as CruiseTreeItemViewModel).IsChecked;
                    (tvi.Header as CruiseTreeItemViewModel).IsSelected = true;
                }
                else if (tvi.Header is TripTreeItemViewModel)
                {
                    (tvi.Header as TripTreeItemViewModel).IsChecked = !(tvi.Header as TripTreeItemViewModel).IsChecked.HasValue ? false : !(tvi.Header as TripTreeItemViewModel).IsChecked;
                    (tvi.Header as TripTreeItemViewModel).IsSelected = true;
                }*/
            }
        }

    }
}
