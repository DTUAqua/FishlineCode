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
using Anchor.Core;

namespace FishLineMeasure.WPF.Views.Lenghts
{
    /// <summary>
    /// Interaction logic for AddOrderClassGroupView.xaml
    /// </summary>
    public partial class AddOrderClassGroupView : UserControl
    {
        public AddOrderClassGroupView()
        {
            InitializeComponent();

            this.Loaded += AddOrderClassGroupView_Loaded;
        }

        private void AddOrderClassGroupView_Loaded(object sender, RoutedEventArgs e)
        {
            new Action(() =>
            {
                tbName.Focus();
                Keyboard.Focus(tbName);
            }).Dispatch();
        }
    }
}
