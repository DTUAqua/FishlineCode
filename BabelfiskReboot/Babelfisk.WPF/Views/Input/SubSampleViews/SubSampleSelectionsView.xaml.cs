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
using Babelfisk.ViewModels;

namespace Babelfisk.WPF.Views.Input
{
    /// <summary>
    /// Interaction logic for SubSampleSelectionsView.xaml
    /// </summary>
    public partial class SubSampleSelectionsView : UserControl
    {

        public AViewModel ViewModel
        {
            get { return this.DataContext as AViewModel; }
        }

        
        public UIElement FirstControl
        {
            get { return tbLengthMeasureType; }
        }


        public SubSampleSelectionsView()
        {
            InitializeComponent();
        }




    }
}
