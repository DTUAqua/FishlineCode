using Babelfisk.Entities.Sprattus;
using FishLineMeasure.ViewModels.Export;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FishLineMeasure.WPF.Views.Export
{
    /// <summary>
    /// Interaction logic for SelectSpeciesListView.xaml
    /// </summary>
    public partial class SelectSpeciesListView : UserControl
    {
        public SelectSpeciesListViewModel ViewModel
        {
            get { return this.DataContext as SelectSpeciesListViewModel; }
        }


        public SelectSpeciesListView()
        {
            InitializeComponent();
        }

        private void Grid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var vm = ViewModel;
                SpeciesList sl = null;
                var fe = sender as FrameworkElement;
                if (fe != null)
                    sl = fe.Tag as SpeciesList;

                if (vm != null && sl != null)
                    vm.SelectedSpeciesList = sl == vm.SelectedSpeciesList ? null : sl;
            }
            catch(Exception ex)
            {
                ViewModels.AViewModel.LogError(ex);
            }
        }
    }
}
