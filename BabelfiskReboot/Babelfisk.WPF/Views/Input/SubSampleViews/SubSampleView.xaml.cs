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
using Anchor.Core.Controls;
using Anchor.Core;

namespace Babelfisk.WPF.Views.Input
{
    /// <summary>
    /// Interaction logic for SubSampleView.xaml
    /// </summary>
    public partial class SubSampleView : UserControl, IDisposable
    {
        public SubSampleView()
        {
            InitializeComponent();
        }

      

        private void SubSampleGridLoaded()
        {
            ctrlSelectionsView.FirstControl.Focus();
            Keyboard.Focus(ctrlSelectionsView.FirstControl);
            if (ctrlSelectionsView.FirstControl is FilteredComboBox)
            {
                (ctrlSelectionsView.FirstControl as FilteredComboBox).IsDropDownOpen = false;
            }
        }

        public void Dispose()
        {
            try
            {
               var lst = contentView.FindAllVisualChildren<UserControl>();

                if (lst != null && lst.Count > 0)
                {
                    lst.ForEach(x =>
                    {
                        if (x is IDisposable)
                            (x as IDisposable).Dispose();
                    });
                }

                contentView.Content = null;

                this.DataContext = null;
            }
            catch { }
        }
    }
}
