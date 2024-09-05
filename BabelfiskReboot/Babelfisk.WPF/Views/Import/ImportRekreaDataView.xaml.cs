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
using Babelfisk.ViewModels;

namespace Babelfisk.WPF.Views.Import
{
    /// <summary>
    /// Interaction logic for ImportRekreaDataView.xaml
    /// </summary>
    public partial class ImportRekreaDataView : UserControl
    {
        public ImportRekreaDataView()
        {
            InitializeComponent();

            this.DataContextChanged += ImportRekreaDataView_DataContextChanged;
        }

        private void ImportRekreaDataView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null && e.OldValue is AViewModel)
            {
                (e.OldValue as AViewModel).OnScrollTo -= ViewModel_OnScrollTo;
            }

            if (e.NewValue is AViewModel)
            {
                (e.NewValue as AViewModel).OnScrollTo += ViewModel_OnScrollTo;
            }
        }

        protected void ViewModel_OnScrollTo(ViewModels.AViewModel obj, string strTo)
        {
            if(strTo != null && strTo.Equals("MessagesEnd", StringComparison.InvariantCultureIgnoreCase))
            {
                ctrlMessages.ScrollMessagesToEnd();
            }
        }
    }
}
