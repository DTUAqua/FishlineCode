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
using Babelfisk.ViewModels.TreeView;

using Anchor.Core;

namespace Babelfisk.WPF.Views.TreeView
{
    /// <summary>
    /// Interaction logic for MainTreeView.xaml
    /// </summary>
    public partial class MainTreeView : UserControl
    {

        public MainTreeViewModel ViewModel
        {
            get { return this.DataContext as MainTreeViewModel; }
        }

        public MainTreeView()
        {
            InitializeComponent();

            treeView.Loaded += new RoutedEventHandler(treeView_Loaded);
        }


        /// <summary>
        /// Set focus to treeview on start up.
        /// </summary>
        private void treeView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                treeView.Focus();
            }
            catch { }
        }



        /// <summary>
        /// Currently it is handled by setting Focusable = false on the TreeViewItem to ignore the dummy nodes.
        /// This however has the sideeffect of selecting the node below the current one if using the right-arrow
        /// when the dummy-loading-node i shown. When time permits, more time could be used with the method below
        /// to select the dummy-node parent whenever i gets selected/focus.
        /// </summary>
        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var vParent = e.OldValue as TreeItemViewModel;
                var vChild = e.NewValue as TreeItemViewModel;

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
            catch { }
        }


        /// <summary>
        /// Method which brings the selected treeviewitem into view.
        /// </summary>
        private void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            try
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
            catch
            {
            }
        } 



        private void Cruise_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement && (sender as FrameworkElement).TemplatedParent is ContentPresenter)
                {
                    var c = ((sender as FrameworkElement).TemplatedParent as ContentPresenter).DataContext as CruiseTreeItemViewModel;
                    if (c.LoadCruiseView())
                        c.IsSelected = true;
                }
            }
            catch { }
        }


        private void Trip_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement && (sender as FrameworkElement).TemplatedParent is ContentPresenter)
                {
                    var t = ((sender as FrameworkElement).TemplatedParent as ContentPresenter).DataContext as TripTreeItemViewModel;
                    if (t.LoadTripView())
                        t.IsSelected = true;
                }
            }
            catch { }
        }


        private void Sample_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement && (sender as FrameworkElement).TemplatedParent is ContentPresenter)
                {
                    var t = ((sender as FrameworkElement).TemplatedParent as ContentPresenter).DataContext as SampleTreeItemViewModel;
                    if (t.LoadSampleView())
                        t.IsSelected = true;
                }
            }
            catch { }
        }


        private void SpeciesList_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement && (sender as FrameworkElement).TemplatedParent is ContentPresenter)
                {
                    var t = ((sender as FrameworkElement).TemplatedParent as ContentPresenter).DataContext as SpeciesListTreeItemViewModel;
                    if (t.LoadSpeciesListView())
                        t.IsSelected = true;
                }
            }
            catch { }
        }


        private void TreeView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                var tvi = sender as TreeViewItem;

                if (tvi != null && e.Key == Key.Return && tvi.Header is TreeItemViewModel && (tvi.Header as TreeItemViewModel).IsSelected)
                {
                    if (tvi.Header is CruiseTreeItemViewModel)
                        (tvi.Header as CruiseTreeItemViewModel).LoadCruiseView();
                    else if (tvi.Header is TripTreeItemViewModel)
                        (tvi.Header as TripTreeItemViewModel).LoadTripView();
                    else if (tvi.Header is SampleTreeItemViewModel)
                        (tvi.Header as SampleTreeItemViewModel).LoadSampleView();
                    else if (tvi.Header is SpeciesListTreeItemViewModel)
                        (tvi.Header as SpeciesListTreeItemViewModel).LoadSpeciesListView();
                }
            }
            catch { }
        }

    }
}
