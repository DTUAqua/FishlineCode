using FishLineMeasure.ViewModels;
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
    /// Interaction logic for FishLineLoginView.xaml
    /// </summary>
    public partial class FishLineLoginView : UserControl
    {

        public FishLineLoginViewModel ViewModel
        {
            get { return this.DataContext as FishLineLoginViewModel; }
        }


        public FishLineLoginView()
        {
            InitializeComponent();

            this.DataContextChanged += LoginView_DataContextChanged;
        }


        void LoginView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
           // if (ViewModel == null)
           //    tbPassword.Clear();
       

            var vmOld = e.OldValue as FishLineLoginViewModel;
            var vmNew = e.NewValue as FishLineLoginViewModel;

            if (vmOld != null)
                vmOld.OnUIMessage -= ViewModel_OnUIMessage;

            if (vmNew != null)
                vmNew.OnUIMessage += ViewModel_OnUIMessage;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    if (ViewModel == null)
                        return;

                    if (!String.IsNullOrEmpty(ViewModel.UserName))
                        tbPassword.Focus();
                    else
                        tbUserName.Focus();
                }
                catch(Exception ex)
                {
                    AViewModel.LogError(ex);
                }
            }));
        }

        private void ViewModel_OnUIMessage(AViewModel vm, string msg)
        {
            if (msg == null)
                return;

            switch(msg)
            {
                case "UserNameTextBoxSelectText":
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        tbUserName.Focus();
                        tbUserName.SelectAll();
                    }), System.Windows.Threading.DispatcherPriority.ContextIdle);
                    break;

                case "PasswordTextBoxSelectText":
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        tbPassword.Focus();
                        tbPassword.SelectAll();
                    }), System.Windows.Threading.DispatcherPriority.ContextIdle);
                    break;
            }
        }

        private void tbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                var vm = ViewModel;

                if (vm != null)
                    vm.Password = tbPassword.Password;
            }
            catch (Exception ex)
            {
                AViewModel.LogError(ex);
            }
        }
    }
}
