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
    /// Interaction logic for StationView.xaml
    /// </summary>
    public partial class StationView : UserControl, IDisposable
    {
        public StationViewModel ViewModel
        {
            get { return this.DataContext as StationViewModel; }
        }


        public StationView()
        {
            InitializeComponent();

            this.DataContextChanged += StationView_DataContextChanged;
        }

        protected void StationView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null && e.OldValue is AInputViewModel)
            {
                (e.OldValue as AInputViewModel).OnScrollTo -= ViewModel_OnScrollTo;
                (e.OldValue as AInputViewModel).OnUIReturnMessage += StationView_OnUIReturnMessage;
            }

            if (e.NewValue is AInputViewModel)
            {
                (e.NewValue as AInputViewModel).OnScrollTo += ViewModel_OnScrollTo;
                (e.NewValue as AInputViewModel).OnUIReturnMessage += StationView_OnUIReturnMessage;
            }
        }

        protected object StationView_OnUIReturnMessage(string msg)
        {
            object res = null;

            if (msg == null)
                return res;

            switch (msg)
            {
                case "IsShovelDistVisible":
                    res = dpShovelDist.Visibility == System.Windows.Visibility.Visible;
                    break;

                case "IsDepthAveGearVisible":
                    res = dpDepthAveGear.Visibility == System.Windows.Visibility.Visible;
                    break;

                case "IsNetOpeningVisible":
                    res = dpNetOpening.Visibility == System.Windows.Visibility.Visible;
                    break;

                case "IsHaulDirectionVisible":
                    res = dpHaulDirection.Visibility == System.Windows.Visibility.Visible;
                    break;

                case "IsWaterSpeedVisible":
                    res = dpWaterSpeed.Visibility == System.Windows.Visibility.Visible;
                    break;

                case "IsHaulSpeedBotVisible":
                    res = dpHaulSpeedBot.Visibility == System.Windows.Visibility.Visible;
                    break;

                case "IsLengthBeamVisible":
                    res = dpLengthBeam.Visibility == System.Windows.Visibility.Visible;
                    break;

                case "IsNumberTrawlsNonScientificVisible":
                    res = dpNumberTrawls.Visibility == System.Windows.Visibility.Visible;
                    break;

                case "IsNumberTrawlsScientificVisible":
                    res = dpNumberTrawlsScientific.Visibility == System.Windows.Visibility.Visible;
                    break;
             
                case "IsLengthNetsVisible":
                    res = dpLengthNets.Visibility == System.Windows.Visibility.Visible;
                    break;

                case "IsHeightNetsVisible":
                    res = dpHeightNets.Visibility == System.Windows.Visibility.Visible;
                    break;

                case "IsLostNetsVisible":
                    res = dpLostNets.Visibility == System.Windows.Visibility.Visible;
                    break;

                case "IsNumNetsVisible":
                    res = dpNumNets.Visibility == System.Windows.Visibility.Visible;
                    break;

                case "IsCourseTrackVisible":
                    res = dpCourseTrack.Visibility == System.Windows.Visibility.Visible;
                    break;

                case "IsMeshSizeVisible":
                    res = dpMeshSize.Visibility == System.Windows.Visibility.Visible;
                    break;

                case "IsWireLengthVisible":
                    res = dpWireLength.Visibility == System.Windows.Visibility.Visible;
                    break;

                case "IsWingSpreadVisible":
                    res = dpWingSpread.Visibility == System.Windows.Visibility.Visible;
                    break;

                case "IsLengthRopeFlyerVisible":
                    res = dpLengthRopeFlyer.Visibility == System.Windows.Visibility.Visible;
                    break;

                case "IsWidthRopeFlyerVisible":
                    res = dpWidthRopeFlyer.Visibility == System.Windows.Visibility.Visible;
                    break;

                case "IsNumberHooksVisible":
                    res = dpNumberHooks.Visibility == System.Windows.Visibility.Visible;
                    break;
                    
   
            }

            
        
            return res;
        }


        protected void ViewModel_OnScrollTo(ViewModels.AViewModel obj, string strTo)
        {
            switch (strTo)
            {
                case "StationNumber":
                    scrollviewer.ScrollToHome();
                    break;

                case "SGId":
                    scrollviewer.ScrollToHome();
                    break;

                case "LabJournalNumber":
                    scrollviewer.ScrollToHome();
                    break;

                case "StationName":
                    scrollviewer.ScrollToHome();
                    break;

                case "HydroStation":
                    scrollviewer.ScrollToHome();
                    break;

                case "SelectedSampleType":
                    scrollviewer.ScrollToHome();
                    break;

                case "GearStartDateTime":
                    ScrollToControl(dpTimeAndPlace);
                    break;

                case "GearEndDateTime":
                    ScrollToControl(dpTimeAndPlace);
                    break;

                case "SpeciesRegistration":
                    ScrollToControl(cbxSpeciesRegistration);
                    break;

                case "CatchRegistration":
                    ScrollToControl(cbxCatchRegistration);
                    break;

                case "WindSpeed":
                    ScrollToControl(tbWindSpeed);
                    break;

                case "WindDirection":
                    ScrollToControl(tbWindDirection);
                    break;

                case "CurrentDirectionSrf":
                    ScrollToControl(tbDirectionSrf);
                    break;

                case "CurrentSpeedSrf":
                    ScrollToControl(tbCurrentSpeedSrf);
                    break;

                case "CurrentSpeedBot":
                    ScrollToControl(tbCurrentSpeedBot);
                    break;

                case "CurrentDirectionBot":
                    ScrollToControl(tbDirectionBot);
                    break;

                case "WaveDirection":
                    ScrollToControl(tbWaveDirection);
                    break;

                case "WaveHeight":
                    ScrollToControl(tbWaveHeight);
                    break;

                case "SalinityBot":
                    ScrollToControl(tbSalinityBot);
                    break;

                case "SalinitySrf":
                    ScrollToControl(tbSalinitySrf);
                    break;

                case "OxygenSrf":
                    ScrollToControl(tbOxygenSrf);
                    break;

                case "OxygenBot":
                    ScrollToControl(tbOxygenBot);
                    break;

                case "ThermoClineDepth":
                    ScrollToControl(tbThermoClineDepth);
                    break;

                case "TemperatureSrf":
                    ScrollToControl(tbTemperatureSrf);
                    break;

                case "TemperatureBot":
                    ScrollToControl(tbTemperatureBot);
                    break;

                case "DepthAvg":
                    ScrollToControl(tbDepthAvg);
                    break;

                case "MeshSize":
                    ScrollToControl(tbMeshSize);
                    break;

                case "HeightNets":
                    ScrollToControl(tbHeightNets);
                    break;

                case "LengthNets":
                    ScrollToControl(tbLengthNets);
                    break;

                case "LengthBeam":
                    ScrollToControl(tbLengthBeam);
                    break;

                case "HaulSpeedBot":
                    ScrollToControl(tbHaulSpeedBot);
                    break;

                case "HaulSpeedWat":
                    ScrollToControl(tbHaulSpeedWat);
                    break;

                case "HaulDirection":
                    ScrollToControl(tbHaulDirection);
                    break;

                case "DepthAveGear":
                    ScrollToControl(tbDepthAveGear);
                    break;

                case "NetOpening":
                    ScrollToControl(tbNetOpening);
                    break;

                case "ShovelDist":
                    ScrollToControl(tbShovelDist);
                    break;

                case "WingSpread":
                    ScrollToControl(tbWingSpread);
                    break;

                case "LengthRopeFlyer":
                    ScrollToControl(tbLengthRopeFlyer);
                    break;

                case "WidthRopeFlyer":
                    ScrollToControl(tbWidthRopeFlyer);
                    break;

                case "GearRemark":
                    ScrollToControl(tbGearRemark);
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
            
            if(itm != null)
                ViewModel.LoadStatisticalRectanglesAsync(itm.DFUArea);
        }

        private void GearType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel == null)
                return;

            var cbb = sender as Anchor.Core.Controls.FilteredComboBox;
            var itm = cbb.SelectedItem as L_GearType;

            if (itm != null)
                ViewModel.LoadSelectionDevicesAsync(itm.gearType);
        }


        private void SampleType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel == null)
                return;

            ViewModel.RefreshGearTypes();
            ViewModel.LoadSelectionDevicesAsync(null);
        }

        public void Dispose()
        {
            try
            {
                if (this.DataContext is AInputViewModel)
                {
                    (this.DataContext as AInputViewModel).OnScrollTo -= ViewModel_OnScrollTo;
                    (this.DataContext as AInputViewModel).OnUIReturnMessage -= StationView_OnUIReturnMessage;
                }

                this.DataContext = null;

                if(map != null)
                    map.Dispose();
            }
            catch{ }
        }
    }
}
