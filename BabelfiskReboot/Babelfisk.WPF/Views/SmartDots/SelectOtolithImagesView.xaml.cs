using Anchor.Core;
using Anchor.Core.Comparers;
using Babelfisk.ViewModels.SmartDots;
using Babelfisk.WPF.Infrastructure.DataGrid;
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

namespace Babelfisk.WPF.Views.SmartDots
{
    /// <summary>
    /// Interaction logic for SelectOtolithImagesView.xaml
    /// </summary>
    public partial class SelectOtolithImagesView : UserControl
    {
        private Key? _keyLastPressedKey;

        public SelectOtolithImagesViewModel ViewModel
        {
            get { return this.DataContext as SelectOtolithImagesViewModel; }
        }

        public SelectOtolithImagesView()
        {
            InitializeComponent();
        }

        private void selectAllCheckBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }



        private void CheckBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void CheckBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //e.Handled = true;
            var vm = (sender as CheckBox).DataContext as OtolithFileItem;
            vm.IsSelected = !vm.IsSelected;
            e.Handled = true;
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                foreach (var item in e.AddedItems.OfType<OtolithFileItem>())
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
                foreach (var item in e.RemovedItems.OfType<OtolithFileItem>())
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


        private void ImagesBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs ee)
        {
            if (ee.ClickCount != 2)
                return;

            try
            {
                var b = (sender as Border);

                if (b == null)
                    return;

                OtolithImagesPreview.LoadPreviewWindow(b.DataContext, "ImagePath", OtolithImagesPreview.SourceBindingType.FilePath);
            }
            catch { }
        }

    }
}
