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

namespace Babelfisk.WPF.Views.Reporting.AddEditViews
{
    /// <summary>
    /// Interaction logic for AddEditReportingTreeNodeView.xaml
    /// </summary>
    public partial class AddEditReportingTreeNodeView : UserControl
    {
        public AddEditReportingTreeNodeView()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(AddEditReportingTreeNodeView_Loaded);
        }

        void AddEditReportingTreeNodeView_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(tbName);
            tbName.SelectAll();
        }
    }
}
