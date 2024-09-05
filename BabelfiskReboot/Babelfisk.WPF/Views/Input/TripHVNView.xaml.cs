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
using Babelfisk.Entities.Sprattus;
using Babelfisk.ViewModels.Input;

namespace Babelfisk.WPF.Views.Input
{
    /// <summary>
    /// Interaction logic for TripHVNView.xaml
    /// </summary>
    public partial class TripHVNView : UserControl, IDisposable
    {
        public TripHVNViewModel ViewModel
        {
            get { return this.DataContext as TripHVNViewModel; }
        }

        public TripHVNView()
        {
            InitializeComponent();

            this.DataContextChanged += TripView_DataContextChanged;
        }

        protected void TripView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null && e.OldValue is TripHVNViewModel)
                (e.OldValue as TripHVNViewModel).OnScrollTo -= ViewModel_OnScrollTo;

            if(e.NewValue is TripHVNViewModel)
                (e.NewValue as TripHVNViewModel).OnScrollTo += ViewModel_OnScrollTo;
        }



        protected void ViewModel_OnScrollTo(ViewModels.AViewModel obj, string strTo)
        {
            switch (strTo)
            {
                case "TripNumber":
                    scrollviewer.ScrollToHome();
                    break;

                case "SpeciesRegistration":
                    ScrollToControl(cbxSpeciesRegistration);
                    break;

                case "CatchRegistration":
                    ScrollToControl(cbxCatchRegistration);
                    break;

                case "LogbookNumber":
                    ScrollToControl(tbLogbookNumber);
                    break;

                case "MeshSize":
                    ScrollToControl(tbMeshSize);
                    break;

                case "Remark":
                    ScrollToControl(tbRemark);
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


        private void Area_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel == null)
                return;

            var cbb = sender as Anchor.Core.Controls.FilteredComboBox;
            var itm = cbb.SelectedItem as L_DFUArea;

            if (itm != null)
                ViewModel.LoadStatisticalRectanglesAsync(itm.DFUArea);
        }

        public void Dispose()
        {
            try
            {
                if (this.DataContext is TripHVNViewModel)
                    (this.DataContext as TripHVNViewModel).OnScrollTo -= ViewModel_OnScrollTo;

                this.DataContext = null;
            }
            catch { }
        }
    }
}
