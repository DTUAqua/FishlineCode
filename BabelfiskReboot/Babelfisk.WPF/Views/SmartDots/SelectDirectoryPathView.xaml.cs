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
    /// Interaction logic for SelectDirectoryPathView.xaml
    /// </summary>
    public partial class SelectDirectoryPathView : UserControl
    {
        private Key? _keyLastPressedKey;

        public SelectDirectoryPathViewModel ViewModel
        {
            get { return this.DataContext as SelectDirectoryPathViewModel; }
        }

        public SelectDirectoryPathView()
        {
            InitializeComponent();
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


        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    e.Handled = true;
                    var vm = ViewModel;
                    if (vm != null)
                        vm.SaveCommand.Execute();
                    return;
                }
            }
            catch { }
        }
    }
}
