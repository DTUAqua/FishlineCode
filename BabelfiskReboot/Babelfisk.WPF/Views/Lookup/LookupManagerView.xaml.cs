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
using Babelfisk.ViewModels.Lookup;
using Anchor.Core;
using System.Windows.Controls.Primitives;

namespace Babelfisk.WPF.Views.Lookup
{
    /// <summary>
    /// Interaction logic for LookupManagerView.xaml
    /// </summary>
    public partial class LookupManagerView : UserControl
    {
        public LookupManagerViewModel ViewModel
        {
            get { return this.DataContext as LookupManagerViewModel; }
        }

        public LookupManagerView()
        {
            InitializeComponent();

            this.Loaded += LookupManagerView_Loaded;

            this.DataContextChanged += dataContextChanged;
        }

        protected void dataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null && e.OldValue is AViewModel)
                (e.NewValue as AViewModel).OnScrollTo -= ViewModel_OnScrollTo;

            (e.NewValue as AViewModel).OnScrollTo += ViewModel_OnScrollTo;
        }

        protected void ViewModel_OnScrollTo(ViewModels.AViewModel obj, string strTo)
        {
            LookupType lt = null;
            foreach (var itm in itemsControl.Items)
            {
                lt = (itm as LookupType);
                if (lt != null && lt.Type.Name.Equals(strTo, StringComparison.InvariantCultureIgnoreCase))
                {
                    var uiitm = itemsControl.ItemContainerGenerator.ContainerFromItem(itm) as FrameworkElement;

                    if (uiitm != null)
                        uiitm.BringIntoView();
                    break;
                }
            }
        }


        public static T FindVisualChild<T>(DependencyObject instance) where T : DependencyObject
        {
            T control = default(T);

            if (instance != null)
            {

                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(instance); i++)
                {
                    if ((control = VisualTreeHelper.GetChild(instance, i) as T) != null)
                    {
                        break;
                    }

                    control = FindVisualChild<T>(VisualTreeHelper.GetChild(instance, i));
                }
            }

            return control;
        }
        

        void LookupManagerView_Loaded(object sender, RoutedEventArgs e)
        {
            searchTextbox.Focus();
        }
    }
}
