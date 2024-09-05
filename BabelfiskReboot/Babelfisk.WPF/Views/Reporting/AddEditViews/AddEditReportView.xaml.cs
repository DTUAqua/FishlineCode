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
using System.ComponentModel;
using Babelfisk.ViewModels.Reporting.AddEditModels;
using System.IO;
using System.Xml;
using System.Windows.Markup;
using Anchor.Core;

namespace Babelfisk.WPF.Views.Reporting.AddEditViews
{
    /// <summary>
    /// Interaction logic for AddEditReportView.xaml
    /// </summary>
    public partial class AddEditReportView : UserControl
    {

        public AddEditReportViewModel ViewModel
        {
            get { return this.DataContext as AddEditReportViewModel; }
        }


        public AddEditReportView()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            this.Loaded += new RoutedEventHandler(AddEditReportingTreeNodeView_Loaded);
        }

        protected void AddEditReportingTreeNodeView_Loaded(object sender, RoutedEventArgs e)
        {
          
            Keyboard.Focus(tbName);
            tbName.SelectAll();
        }



        private void btnRestriction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as Button;
                var tt = btn.ToolTip as ToolTip;
              
                if (btn == null || btn.ToolTip == null || tt == null)
                    return;

                var sp = tt.Content as StackPanel;

                if (sp == null)
                    return;

                sp = sp.CloneXaml();

                sp.Width = 600;
                sp.Children.OfType<TextBlock>().ToList().ForEach(x => x.FontSize = 15);
                ViewModel.AppRegionManager.ShowMessageBox(sp, MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }

       

    }
}
