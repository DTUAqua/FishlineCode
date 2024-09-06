using FishLineMeasure.ViewModels.Lenghts;
using System;
using System.Collections;
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

namespace FishLineMeasure.WPF.Views.Overview
{
    /// <summary>
    /// Interaction logic for LenghtWindow.xaml
    /// </summary>
    public partial class LenghtWindow : UserControl
    {
        public LenghtViewModel ViewModel
        {
            get { return this.DataContext as LenghtViewModel; }
        }



        public LenghtWindow()
        {
            InitializeComponent();
          
            this.DataContextChanged += LenghtWindow_DataContextChanged;
        }

        private void LenghtWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vmOld = e.OldValue as LenghtViewModel;
            var vmNew = e.NewValue as LenghtViewModel; 

            if(vmOld != null)
                vmOld.OnUIMessage -= LengthViewModel_OnUIMessage;

            if(vmNew != null)
                vmNew.OnUIMessage += LengthViewModel_OnUIMessage;
        }


        private void LengthViewModel_OnUIMessage(ViewModels.AViewModel vm, string msg)
        {
            if (msg.Contains("LengthListEnd_"))
            {
                try
                {
                    var vmn = ViewModel;
                    int amount = vmn.Measurements.Count() - 1;
                    var item = ListViewScrollViewer.Items[amount];
                    ListViewScrollViewer.SelectedItem = item;
                    ListViewScrollViewer.ScrollIntoView(ListViewScrollViewer.SelectedItem);
                }
                catch (Exception)
                {

                    
                }
            }
        }


        private void bdrNext_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var vm = ViewModel;
            if (vm != null)
                vm.MoveToNextOrder();
        }

        private void bdrPrev_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var vm = ViewModel;
            if (vm != null)
                vm.MoveToPreviousOrder();
        }
    }
}
