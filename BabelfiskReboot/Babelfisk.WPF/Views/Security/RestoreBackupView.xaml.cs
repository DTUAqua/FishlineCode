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
    /// Interaction logic for RestoreBackupView.xaml
    /// </summary>
    public partial class RestoreBackupView : UserControl
    {
        public RestoreBackupViewModel ViewModel
        {
            get { return this.DataContext as RestoreBackupViewModel; }
        }

        public RestoreBackupView()
        {
            InitializeComponent();

            this.DataContextChanged += new DependencyPropertyChangedEventHandler(RestoreBackupView_DataContextChanged);
        }

        protected void RestoreBackupView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vmOld = e.OldValue as RestoreBackupViewModel;
            var vm = e.NewValue as RestoreBackupViewModel;

            if(vmOld != null)
                vm.OnScrollTo -= new Action<ViewModels.AViewModel, string>(vm_OnScrollTo);

            if (vm != null)
                vm.OnScrollTo += new Action<ViewModels.AViewModel, string>(vm_OnScrollTo);
        }


        protected void vm_OnScrollTo(ViewModels.AViewModel vm, string name)
        {
            try
            {
                if (name == "SelectedPathEnd")
                {
                    if (ViewModel != null && ViewModel.SelectedBackupFilePath != null)
                    {
                        tbSelectedPath.Focus();
                        tbSelectedPath.CaretIndex = ViewModel.SelectedBackupFilePath.Length;
                    }
                }
            }
            catch { }
        }
    }
}
