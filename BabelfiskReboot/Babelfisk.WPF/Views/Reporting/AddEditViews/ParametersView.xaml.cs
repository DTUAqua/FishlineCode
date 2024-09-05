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

namespace Babelfisk.WPF.Views.Reporting.AddEditViews
{
    /// <summary>
    /// Interaction logic for ParametersView.xaml
    /// </summary>
    public partial class ParametersView : UserControl
    {
        public static readonly DependencyProperty IsAddButtonVisibleProperty = DependencyProperty.Register("IsAddButtonVisible", typeof(bool), typeof(ParametersView), new UIPropertyMetadata(true));

        public bool IsAddButtonVisible
        {
            get { return (bool)GetValue(IsAddButtonVisibleProperty); }
            set { SetValue(IsAddButtonVisibleProperty, value); }
        }


        public static readonly DependencyProperty IsDeleteButtonVisibleProperty = DependencyProperty.Register("IsDeleteButtonVisible", typeof(bool), typeof(ParametersView), new UIPropertyMetadata(true));

        public bool IsDeleteButtonVisible
        {
            get { return (bool)GetValue(IsDeleteButtonVisibleProperty); }
            set { SetValue(IsDeleteButtonVisibleProperty, value); }
        }

        public static readonly DependencyProperty ItemBackgroundBrushProperty = DependencyProperty.Register("ItemBackgroundBrush", typeof(Brush), typeof(ParametersView), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));

        public Brush ItemBackgroundBrush
        {
            get { return (Brush)GetValue(ItemBackgroundBrushProperty); }
            set { SetValue(ItemBackgroundBrushProperty, value); }
        }



        public ParametersView()
        {
            InitializeComponent();
        }


        private void txtParameterName_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                var tb = (sender as TextBlock);

                if (tb != null && tb.Text != null)
                    tb.Text.CopyToClipboard();
            }
        }
    }
}
