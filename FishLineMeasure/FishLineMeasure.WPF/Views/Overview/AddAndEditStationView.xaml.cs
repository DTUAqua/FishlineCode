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

namespace FishLineMeasure.WPF.Views.Overview
{
    /// <summary>
    /// Interaction logic for AddStationView.xaml
    /// </summary>
    public partial class AddAndEditStationView : UserControl
    {
        public AddAndEditStationView()
        {
            InitializeComponent();

            this.Loaded += addstation_Loaded;
        }

        private void addstation_Loaded(object sender, RoutedEventArgs e)
        {
            new Action(() =>
            {
                try
                {
                    tbStationNumber.Focus();
                    Keyboard.Focus(tbStationNumber);
                }
                catch { }
            }).Dispatch();
        }
    }
}
