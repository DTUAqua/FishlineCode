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
using Babelfisk.ViewModels.Offline;

namespace Babelfisk.WPF.Views.Offline
{
    /// <summary>
    /// Interaction logic for CompareEntitiesView.xaml
    /// </summary>
    public partial class CompareEntitiesView : UserControl
    {
        public CompareEntitiesViewModel ViewModel
        {
            get { return this.DataContext as CompareEntitiesViewModel; }
        }


        public CompareEntitiesView()
        {
            InitializeComponent();

            dataGrid.SelectionChanged += (obj, e) => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => dataGrid.UnselectAll()));
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel.RepeatForFutureConflicts = !ViewModel.RepeatForFutureConflicts;
        }
    }
}
