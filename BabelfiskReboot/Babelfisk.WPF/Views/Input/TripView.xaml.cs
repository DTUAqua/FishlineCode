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
using Babelfisk.ViewModels.Input;

namespace Babelfisk.WPF.Views.Input
{
    /// <summary>
    /// Interaction logic for TripView.xaml
    /// </summary>
    public partial class TripView : UserControl, IDisposable
    {
        public TripViewModel ViewModel
        {
            get { return this.DataContext as TripViewModel; }
        }

        public TripView()
        {
            InitializeComponent();

            this.DataContextChanged += TripView_DataContextChanged;
        }

        protected void TripView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null && e.OldValue is TripViewModel)
                (e.OldValue as TripViewModel).OnScrollTo -= ViewModel_OnScrollTo;

            if(e.NewValue is TripViewModel)
                (e.NewValue as TripViewModel).OnScrollTo += ViewModel_OnScrollTo;  
        }


        protected void ViewModel_OnScrollTo(ViewModels.AViewModel obj, string strTo)
        {
            switch (strTo)
            {
                case "StationNumber":
                    scrollviewer.ScrollToHome();
                    break;

                case "RekSGTrId":
                case "REKTripNumber":
                    scrollviewer.ScrollToHome();
                    break;

                case "ContactPersonOrganization":
                    ScrollToControl(tbContactPersonOrganization);
                    break;

                case "ContactPersonAddress":
                    ScrollToControl(tbContactPersonAddress);
                    break;

                case "ContactPersonZipTown":
                    ScrollToControl(tbContactPersonZipTown);
                    break;

                case "ContactPersonTelephone":
                    ScrollToControl(tbContactPersonTelephone);
                    break;

                case "ContactPersonTelephonePrivate":
                    ScrollToControl(tbContactPersonTelephonePrivate);
                    break;

                case "ContactPersonTelephoneMobile":
                    ScrollToControl(tbContactPersonTelephoneMobile);
                    break;

                case "ContactPersonEmail":
                    ScrollToControl(tbContactPersonEmail);
                    break;

                case "ContactPersonFacebook":
                    ScrollToControl(tbContactPersonFacebook);
                    break;

                default:
                    scrollviewer.ScrollToHome();
                    break;
            }
        }


        private void ScrollToControl(UIElement ui)
        {
            Point relativePoint = ui.TransformToAncestor(scrollviewer).Transform(new Point(0, 0));
            double offset = scrollviewer.VerticalOffset;
            scrollviewer.ScrollToVerticalOffset(offset + relativePoint.Y - 5);
        }


        public void Dispose()
        {
            try
            {
                if (this.DataContext is TripViewModel)
                    (this.DataContext as TripViewModel).OnScrollTo -= ViewModel_OnScrollTo;

                this.DataContext = null;

                if (map != null)
                    map.Dispose();
            }
            catch { }
        }
    }
}
