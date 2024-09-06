using FishLineMeasure.ViewModels;
using FishLineMeasure.ViewModels.CustomControls;
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

namespace FishLineMeasure.WPF.Views.CustomControls
{
    /// <summary>
    /// Interaction logic for CheckBoxControlView.xaml
    /// </summary>
    public partial class CheckBoxControlView : UserControl
    {

        public AViewModel ViewModel
        {
            get { return this.DataContext as AViewModel; }
        }

        public CheckBoxControlView()
        {
            InitializeComponent();
        }

        private void TextBlock_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var vm = ViewModel;
                if (vm != null)
                    (vm as dynamic).IsChecked = true;
            }
            catch { }
        }
    }
}
