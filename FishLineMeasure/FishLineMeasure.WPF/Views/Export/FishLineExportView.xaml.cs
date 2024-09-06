using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace FishLineMeasure.WPF.Views.Export
{
    /// <summary>
    /// Interaction logic for FishLineExportView.xaml
    /// </summary>
    public partial class FishLineExportView : UserControl
    {
        public FishLineExportView()
        {
            InitializeComponent();
        }


        private bool AutoScroll = true;
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            new Action(() =>
            {
                try
                {
                    if (e.ExtentHeightChange == 0)
                    {   // Content unchanged : user scroll event
                        if ((e.OriginalSource as ScrollViewer).VerticalOffset == (e.OriginalSource as ScrollViewer).ScrollableHeight)
                        {   // Scroll bar is in bottom
                            // Set autoscroll mode
                            AutoScroll = true;
                        }
                        else
                        {   // Scroll bar isn't in bottom
                            // Unset autoscroll mode
                            AutoScroll = false;
                        }
                    }

                    // Content scroll event : autoscroll eventually
                    if (AutoScroll && e.ExtentHeightChange != 0)
                    {   // Content changed and autoscroll mode set
                        // Autoscroll
                        (e.OriginalSource as ScrollViewer).ScrollToEnd();
                        //(e.OriginalSource as ScrollViewer).ScrollToVerticalOffset((e.OriginalSource as ScrollViewer).ExtentHeight);
                    }
                }
                catch (Exception ex)
                {
                    ViewModels.AViewModel.LogError(ex);
                }
            }).Dispatch(System.Windows.Threading.DispatcherPriority.ContextIdle);
        }
    }
}
