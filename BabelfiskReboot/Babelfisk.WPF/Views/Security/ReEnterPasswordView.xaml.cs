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
using Anchor.Core;
using Babelfisk.ViewModels.Security;

namespace Babelfisk.WPF.Views.Security
{
    /// <summary>
    /// Interaction logic for ReEnterPasswordView.xaml
    /// </summary>
    public partial class ReEnterPasswordView : UserControl
    {
        public ReEnterPasswordViewModel ViewModel
        {
            get { return this.DataContext as ReEnterPasswordViewModel; }
        }


        public ReEnterPasswordView()
        {
            InitializeComponent();

            this.Loaded += ReEnterPasswordView_Loaded;
        }

        void ReEnterPasswordView_Loaded(object sender, RoutedEventArgs e)
        {
            new Action(() =>
            {
                tbPassword.Focus();
                Keyboard.Focus(tbPassword);
            }).Dispatch();
        }


        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
                ViewModel.Password = tbPassword.Password;
        }
    }
}
