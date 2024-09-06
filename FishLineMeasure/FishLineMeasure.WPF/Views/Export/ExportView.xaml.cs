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
    /// Interaction logic for ExportView.xaml
    /// </summary>
    public partial class ExportView : UserControl
    {
        public ExportView()
        {
            InitializeComponent();
            this.DataContextChanged += ExportWindow_DataContextChanged;
           
        }

       
        private void ExportWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vmOld = e.OldValue as ExpoertViewModel;
            var vmNew = e.NewValue as ExpoertViewModel;

            if (vmOld != null)
                vmOld.OnUIMessage -= LengthViewModel_OnUIMessage;

            if (vmNew != null)
                vmNew.OnUIMessage += LengthViewModel_OnUIMessage;
        }

     
        private void LengthViewModel_OnUIMessage(ViewModels.AViewModel vm, string msg)
        {
            if (msg.Contains("PathChanged_"))
            {
                try
                {
                    PathTextBox.Focus();                
                    PathTextBox.Select(PathTextBox.Text.Length, 0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Random Error \r\n {ex.Message}");

                }
            }
        }
    }
}
