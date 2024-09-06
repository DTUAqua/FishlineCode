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

namespace FishLineMeasure.WPF.Views.Overview
{
    /// <summary>
    /// Interaction logic for OverviewView.xaml
    /// </summary>
    public partial class OverviewView : UserControl
    {
        public OverviewView()
        {
            InitializeComponent();

            SetVersion();
        }


        private void SetVersion()
        {
            Version v;
            bool blnInstalled = false;

            try
            {
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    v = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
                    blnInstalled = true;
                }
                else
                {
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    v = assembly.GetName().Version;
                }

                tbVersion.Text = v.ToString() + (blnInstalled ? "" : "");
            }
            catch
            {
                //Make sure version-display does not crash application.
            }
        }
    }
}
