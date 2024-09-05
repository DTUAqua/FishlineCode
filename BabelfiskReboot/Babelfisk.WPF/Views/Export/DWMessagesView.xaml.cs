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
using Babelfisk.Warehouse;

namespace Babelfisk.WPF.Views.Export
{
    /// <summary>
    /// Interaction logic for DWMessagesView.xaml
    /// </summary>
    public partial class DWMessagesView : UserControl
    {
        public static readonly DependencyProperty IsSourceIdColumnVisibleProperty = DependencyProperty.Register("IsSourceIdColumnVisible", typeof(Boolean), typeof(DWMessagesView), new PropertyMetadata(true));

        public bool IsSourceIdColumnVisible
        {
            get { return (bool)GetValue(IsSourceIdColumnVisibleProperty); }
            set { SetValue(IsSourceIdColumnVisibleProperty, value); }
        }


        public DWMessagesView()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(DWMessagesView_Loaded);
        }

        void DWMessagesView_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid.Focus();
            Keyboard.Focus(dataGrid);
        }


        private void dataGrid_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.A && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                dataGrid.SelectAll();
            }
            else if (e.Key == Key.C && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                PasteSelectedRowsToClipboard();
            }
        }


        private void PasteSelectedRowsToClipboard()
        {
            if (dataGrid == null || dataGrid.SelectedItems == null || dataGrid.SelectedItems.Count == 0)
                return;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(DWMessage.GetSemicolonSeperatedHeader);
            
            foreach (DWMessage dr in dataGrid.SelectedItems.OfType<DWMessage>().OrderBy(x => x.Index))
            {
                sb.AppendLine(dr.GetSemicolonSeperatedData);
            }

            Clipboard.SetText(sb.ToString());
        }


        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            PasteSelectedRowsToClipboard();
        }


        public void ScrollMessagesToEnd()
        {
            try
            {
                if (dataGrid.Items.Count > 0)
                    dataGrid.ScrollIntoView(dataGrid.Items[dataGrid.Items.Count - 1]);
            }
            catch(Exception)
            {

            }
        }

    }
}
