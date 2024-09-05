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
using System.Windows.Threading;
using Babelfisk.ViewModels.Security;
using Anchor.Core;

namespace Babelfisk.WPF.Views.Security
{
    /// <summary>
    /// Interaction logic for LogOnView.xaml
    /// </summary>
    public partial class LogOnView : UserControl
    {

        public LogOnViewModel ViewModel
        {
            get { return this.DataContext as LogOnViewModel; }
        }

        public LogOnView()
        {
            InitializeComponent();
            this.DataContextChanged += LogOnView_DataContextChanged;
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

                tbVersion.Text = v.ToString() + (blnInstalled ? "" : " - Development");
            }
            catch
            {
                //Make sure version-display does not crash application.
            }
        }

        protected void LogOnView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ViewModel == null && Password != null)
                Password.Clear();

            new Action(() =>
            {
                try
                {
                    if (ViewModel == null)
                        return;

                    if (!String.IsNullOrEmpty(ViewModel.UserName))
                    {
                        Password.Focus();
                        Keyboard.Focus(Password);
                    }
                    else
                    {
                        tbUserName.Focus();
                        Keyboard.Focus(tbUserName);
                    }
                }
                catch { }
            }).Dispatch(DispatcherPriority.ContextIdle);

        }


        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel != null)
                    ViewModel.Password = Password.Password;
            }
            catch { }
        }


        private void cbxLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ViewModel == null)
                    return;

                ViewModel.SetLanguage();

                //Refresh UI, so language will change
                var frame = new DispatcherFrame();
                var dispatcherOperationCallback = new DispatcherOperationCallback(delegate
                {
                    frame.Continue = false;
                    return null;
                });
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, dispatcherOperationCallback, null);
                Dispatcher.PushFrame(frame);
            }
            catch { }
        }
    }
}
