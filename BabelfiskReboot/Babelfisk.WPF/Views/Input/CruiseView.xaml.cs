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
using Babelfisk.ViewModels.Input;
using Babelfisk.Entities.Sprattus;

namespace Babelfisk.WPF.Views.Input
{
    /// <summary>
    /// Interaction logic for CruiseView.xaml
    /// </summary>
    public partial class CruiseView : UserControl, IDisposable
    {

        public CruiseViewModel ViewModel
        {
            get { return this.DataContext as CruiseViewModel; }
        }

        public CruiseView()
        {
            InitializeComponent();
        }

        private void FilteredComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           // if(ViewModel != null && ViewModel.Cruise != null)
           //     ViewModel.Cruise.MarkAsModified();
        }

        private Brush _bOff = null;
        private void tbPercent_GotFocus_1(object sender, RoutedEventArgs e)
        {
            _bOff = (sender as TextBox).Foreground;
            (sender as TextBox).Foreground = Brushes.Black;
        }

        private void tbPercent_LostFocus_1(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Foreground = _bOff;
        }

        public void Dispose()
        {
            try
            {
                this.DataContext = null;

                if (map != null)
                    map.Dispose();
            }
            catch { }
        }
    }
}
