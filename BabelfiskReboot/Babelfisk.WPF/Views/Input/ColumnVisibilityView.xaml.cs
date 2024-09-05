using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Babelfisk.ViewModels.Input;
using Anchor.Core;

namespace Babelfisk.WPF.Views.Input
{
    /// <summary>
    /// Interaction logic for ColumnVisibilityView.xaml
    /// </summary>
    public partial class ColumnVisibilityView : UserControl
    {
        public ColumnVisibilityViewModel ViewModel
        {
            get { return this.DataContext as ColumnVisibilityViewModel; }
        }


        public ColumnVisibilityView()
        {
            InitializeComponent();
        }


        public CustomPopupPlacement[] placePopup(Size popupSize,
                                         Size targetSize,
                                         Point offset)
        {
            CustomPopupPlacement placement1 = new CustomPopupPlacement(new Point(0, targetSize.Height), PopupPrimaryAxis.Vertical);

            // CustomPopupPlacement placement2 =
            //     new CustomPopupPlacement(new Point(10, 20), PopupPrimaryAxis.Horizontal);

            CustomPopupPlacement[] ttplaces =
                    new CustomPopupPlacement[] { placement1/*, placement2*/ };
            return ttplaces;
        }

        private void popupPanelRight_Closed(object sender, EventArgs e)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(200);
                new Action(() =>
                {
                    //Make sure popup is closed
                    if (ViewModel != null && ViewModel.IsSettingsOpen)
                        ViewModel.IsSettingsOpen = false;
                }).Dispatch();
            });
        }
    }
}
