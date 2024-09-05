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
using Babelfisk.ViewModels.Security;

namespace Babelfisk.WPF.Views.Security
{
    /// <summary>
    /// Interaction logic for UserView.xaml
    /// </summary>
    public partial class UserView : UserControl
    {
        public UserViewModel ViewModel
        {
            get
            {
                return this.DataContext as UserViewModel;
            }
        }


        public UserView()
        {
            InitializeComponent();

            this.DataContextChanged += UserView_DataContextChanged;
        }

        protected void UserView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ViewModel == null)
            {
                Password.Clear();
                PasswordRepeat.Clear();
            }
        }



        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
                ViewModel.Password = Password.Password;
        }

        private void PasswordRepeat_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
                ViewModel.PasswordRepeat = PasswordRepeat.Password;
        }
    }
}
