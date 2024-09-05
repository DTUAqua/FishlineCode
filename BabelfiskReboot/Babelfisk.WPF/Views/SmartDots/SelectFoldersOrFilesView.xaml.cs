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

namespace Babelfisk.WPF.Views.SmartDots
{
    /// <summary>
    /// Interaction logic for SelectFoldersOrFilesView.xaml
    /// </summary>
    public partial class SelectFoldersOrFilesView : UserControl
    {
        public SelectFoldersOrFilesView()
        {
            InitializeComponent();
        }


        private void CheckBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void CheckBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //e.Handled = true;
            var vm = (sender as CheckBox).DataContext as FileTreeViewItem;
            vm.IsChecked = !vm.IsChecked;
            e.Handled = true;
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                foreach (var item in e.AddedItems.OfType<FileTreeViewItem>())
                {
                    if (!item.IsChecked)
                    {
                        // if bound data item still isn't selected, fix this
                        item.IsChecked = true;
                    }
                }
            }
            if (e.RemovedItems != null)
            {
                foreach (var item in e.RemovedItems.OfType<FileTreeViewItem>())
                {
                    if (item.IsChecked)
                    {
                        // if bound data item still is selected, fix this
                        item.IsChecked = false;
                    }
                }
            }

            e.Handled = true;
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

                OtolithImagesPreview.LoadPreviewWindow(b.DataContext, "Path", OtolithImagesPreview.SourceBindingType.FilePath);
            }
            catch { }
        }
    }
}
