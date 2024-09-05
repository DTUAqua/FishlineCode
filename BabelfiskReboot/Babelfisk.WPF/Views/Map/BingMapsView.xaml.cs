using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
using Babelfisk.ViewModels.Map;
using Anchor.Core;
using Microsoft.Maps.MapControl.WPF;
using GeometricLibrary.Core.Vector;
using System.IO;

namespace Babelfisk.WPF.Views.Map
{
    /// <summary>
    /// Interaction logic for BingMapsView.xaml
    /// </summary>
    public partial class BingMapsView : UserControl, IDisposable
    {
        public MapViewModelBingControl ViewModel
        {
            get { return this.DataContext as MapViewModelBingControl; }
        }

        public BingMapsView()
        {
            InitializeComponent();

            try
            {
                this.DataContextChanged += MapView_DataContextChanged;
                map.Loaded += BingMapsView_Loaded;

                if (DesignerProperties.GetIsInDesignMode(this))
                    this.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }


        private void ZoomOut_Click(object sender, EventArgs e)
        {
            try
            {
                if (map != null)
                {
                    //1
                    var zoom = Math.Max(1, map.ZoomLevel - 0.1);
                    map.ZoomLevel = zoom;
                    System.Diagnostics.Debug.WriteLine(map.ZoomLevel);
                }
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }

        private void ZoomIn_Click(object sender, EventArgs e)
        {
            try
            {
                if (map != null)
                {
                    //13
                    var zoom = Math.Min(13, map.ZoomLevel + 0.1);
                    map.ZoomLevel = zoom;
                    System.Diagnostics.Debug.WriteLine(map.ZoomLevel);
                }
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }


        protected void BingMapsView_Loaded(object sender, RoutedEventArgs e)
        {
            RebuildMap();
        }

        protected void MapView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if ((e.OldValue as MapViewModelBingControl) != null)
                {
                    (e.OldValue as MapViewModelBingControl).OnUIMessage -= BingMapsView_OnUIMessage;
                }

                MapViewModelBingControl mvm = null;
                if ((mvm = e.NewValue as MapViewModelBingControl) != null)
                {
                    (e.NewValue as MapViewModelBingControl).OnUIMessage += BingMapsView_OnUIMessage;

                    if (mvm.IsWindow)
                    {
                        //Clear width and height bindings so the map will fill out the whole window.
                        BindingOperations.ClearBinding(mapContent, TextBox.WidthProperty);
                        BindingOperations.ClearBinding(mapContent, TextBox.HeightProperty);
                    }
                }
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }

        protected void BingMapsView_OnUIMessage(ViewModels.AViewModel vm, string msg)
        {
            try
            {
                if (msg == null)
                    return;

                switch (msg)
                {
                    case "Rebuild":
                        RebuildMap();
                        break;
                }
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }


        private void ClearMap()
        {
            try
            {
                if (map.Children.Count > 0)
                {
                    foreach (var c in map.Children)
                    {
                        if (c is MapPolyline)
                        {
                            var p = c as MapPolyline;
                            p.MouseEnter -= poly_MouseEnter;
                            p.MouseLeave -= poly_MouseLeave;
                        }
                    }
                    map.Children.Clear();
                }
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }

        private void RebuildMap()
        {
            try
            {
                var vm = ViewModel;

                ClearMap();

                if (vm.Points == null || !vm.IsEnabled)
                    return;

                Vec2d? vTmp = null;
                Vec2d vMin = new Vec2d(double.MaxValue);
                Vec2d vMax = new Vec2d(double.MinValue);

                if (vm.IsPointsSelected)
                {
                    var tt = this.TryFindResource("ttPoint") as ToolTip;
                    var pinTemplate = this.TryFindResource("pinTemplate") as ControlTemplate;

                    var groupsCount = vm.Points.GroupBy(x => x.TripName).Count();

                    bool blnShowTripName = groupsCount > 1;

                    foreach (var p in vm.Points)
                    {
                        if (p.LatitudeStart == null || p.LongitudeStop == null)
                            continue;

                        Pushpin pin = new Pushpin();

                        var latStop = MapViewModel.ConvertPositionFromDegreesToDouble(p.LatitudeStop ?? "00.00.000 N");
                        var lonStop = MapViewModel.ConvertPositionFromDegreesToDouble(p.LongitudeStop ?? "00.00.000 E");

                        pin.Tag = blnShowTripName ? "yes" : "no";
                        pin.Location = new Location(latStop, lonStop);
                        pin.Width = Double.NaN;
                        pin.Height = Double.NaN;
                        pin.Template = pinTemplate;
                        pin.SetValue(ToolTipService.ShowDurationProperty, 60000);
                        pin.DataContext = p;
                        pin.ToolTip = tt;

                        map.Children.Add(pin);

                        vTmp = new Vec2d(latStop, lonStop);
                        vMin = VMathd.Min(vTmp.Value, vMin);
                        vMax = VMathd.Max(vTmp.Value, vMax);
                    }
                }
                else
                {
                    var tt = this.TryFindResource("ttPolyline") as ToolTip;

                    for (int i = 0; i < vm.Points.Count; i++)
                    {
                        var point = vm.Points[i];

                        if (!string.IsNullOrEmpty(point.LatitudeStart) && !string.IsNullOrEmpty(point.LongitudeStart) && !string.IsNullOrEmpty(point.LatitudeStop) && !string.IsNullOrEmpty(point.LongitudeStop))
                        {
                            var latStart = MapViewModel.ConvertPositionFromDegreesToDouble(point.LatitudeStart);
                            var lonStart = MapViewModel.ConvertPositionFromDegreesToDouble(point.LongitudeStart);
                            var latStop = MapViewModel.ConvertPositionFromDegreesToDouble(point.LatitudeStop);
                            var lonStop = MapViewModel.ConvertPositionFromDegreesToDouble(point.LongitudeStop);

                            bool blnPoint = false;
                            if (Math.Abs(latStart - latStop) < 0.00001 && Math.Abs(lonStart - lonStop) < 0.00001)
                            {
                                latStop += 0.0001;
                                lonStop += 0.0001;
                                blnPoint = true;
                            }

                            var p = new MapPolyline();
                            p.Locations = new LocationCollection() { new Location(latStart, lonStart), new Location(latStop, lonStop) };
                            p.Fill = System.Windows.Media.Brushes.Red;
                            p.Stroke = System.Windows.Media.Brushes.Red;
                            p.StrokeThickness = 2;
                            p.SetValue(ToolTipService.ShowDurationProperty, 20000);
                            p.DataContext = point;
                            p.ToolTip = tt;
                            p.MouseEnter += poly_MouseEnter;
                            p.MouseLeave += poly_MouseLeave;

                            map.Children.Add(p);

                            vTmp = new Vec2d(latStart, lonStart);
                            vMin = VMathd.Min(vTmp.Value, vMin);
                            vMax = VMathd.Max(vTmp.Value, vMax);
                            vTmp = new Vec2d(latStop, lonStop);
                            vMin = VMathd.Min(vTmp.Value, vMin);
                            vMax = VMathd.Max(vTmp.Value, vMax);
                        }
                    }
                }

                if (vTmp != null)
                {
                    new Action(() =>
                    {
                        CenteMap(vMin, vMax);
                        new Action(() =>
                        {
                            CenteMap(vMin, vMax);
                            new Action(() =>
                            {
                                CenteMap(vMin, vMax);
                            }).Dispatch(System.Windows.Threading.DispatcherPriority.ContextIdle);
                        }).Dispatch(System.Windows.Threading.DispatcherPriority.ContextIdle);

                    }).Dispatch(System.Windows.Threading.DispatcherPriority.Render);
                    /* System.Threading.Tasks.Task.Factory.StartNew(() =>
                     {
                         System.Threading.Thread.Sleep(50);
                         new Action(() =>
                         {
                             CenteMap(vMin, vMax);
                         }).Dispatch(System.Windows.Threading.DispatcherPriority.Render);
                     });*/
                }
            }
            catch(Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }
        }


        private void CenteMap(Vec2d vMin, Vec2d vMax)
        {
            try
            {
                vMin -= new Vec2d(0.2);
                vMax += new Vec2d(0.2);
                Vec2d vMid = (vMin + vMax) * 0.5;
                if (map.ActualHeight > 0)
                    map.SetView(new LocationRect(new Location(vMin.X, vMin.Y), new Location(vMax.X, vMax.Y)));
                var minv = Math.Min(map.ZoomLevel, 9);
                map.ZoomLevel = minv;

                map.Center = new Location(vMid.X, vMid.Y);
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }

        protected void poly_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                var p = sender as MapPolyline;
                p.Fill = System.Windows.Media.Brushes.Yellow;
                p.Stroke = System.Windows.Media.Brushes.Yellow;
                p.StrokeThickness = 3;
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }

        protected void poly_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                var p = sender as MapPolyline;
                p.Fill = System.Windows.Media.Brushes.Red;
                p.Stroke = System.Windows.Media.Brushes.Red;
                p.StrokeThickness = 2;
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }


        



        private void ClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            new Action(() =>
            {
                try
                {
                    // ToolTipService.
                    Bitmap bmp = GetBrowserScreenshot();
                    var bmpSource = bmp.ToBitmapSource();

                    Clipboard.SetImage(bmpSource);

                    if (bmp != null)
                        bmp.Dispose();
                }
                catch (Exception ex)
                {
                    if (ViewModel != null)
                        ViewModel.AppRegionManager.ShowMessageBox("En uventet fejl opstod. " + ex.Message);
                }
            }).Dispatch();
        }

        private void ScreenShotButton_Click(object sender, RoutedEventArgs e)
        {
            new Action(() =>
            {
                try
                {
                    Bitmap bmp = GetBrowserScreenshot();

                    Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                    sfd.Filter = "JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|GIF (*.gif)|*.gif|Bitmap (*.bmp)|*.bmp|All Files|*.*";
                    bool? blnRes = sfd.ShowDialog(Application.Current.MainWindow);

                    if (blnRes.HasValue && blnRes.Value)
                        bmp.Save(sfd.FileName);

                    if (bmp != null)
                        bmp.Dispose();
                }
                catch (Exception ex)
                {
                    if (ViewModel != null)
                        ViewModel.AppRegionManager.ShowMessageBox("En uventet fejl opstod. " + ex.Message);
                }
            }).Dispatch();
        }


        private void Print_Click(object sender, RoutedEventArgs e)
        {
            new Action(() =>
            {
                try
                {
                    Bitmap bmp = GetBrowserScreenshot();
                    var bmpSource = bmp.ToBitmapSource();

                    PrintDialog dlg = new PrintDialog();
                    bool? result = dlg.ShowDialog();

                    if (result.HasValue && result.Value)
                    {
                        var img = new System.Windows.Controls.Image();
                        img.Source = bmpSource;

                        img.Measure(new System.Windows.Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight));
                        img.Arrange(new Rect(new System.Windows.Point(0, 0), img.DesiredSize));

                        dlg.PrintVisual(img, "Map");
                    }

                    if (bmp != null)
                        bmp.Dispose();
                }
                catch (Exception ex)
                {
                    if (ViewModel != null)
                        ViewModel.AppRegionManager.ShowMessageBox("En uventet fejl opstod. " + ex.Message);
                }
            }).Dispatch();
        }


        private Bitmap GetBrowserScreenshot()
        {
            var size = map.RenderSize;
            var rtb = new RenderTargetBitmap( 
                (int)map.ActualWidth, //width 
                (int)map.ActualHeight, //height 
                96, //dpi x 
                96, //dpi y 
                PixelFormats.Pbgra32 // pixelformat 
                );
            rtb.Render(map);

            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            encoder.Save(stream);

            Bitmap bitmap = new Bitmap(stream);
            return bitmap;
        }

       /* private Bitmap GetBrowserScreenshot()
        {
            var topLeftCorner = map.PointToScreen(new System.Windows.Point(0, 0));
            var topLeftGdiPoint = new System.Drawing.Point((int)topLeftCorner.X, (int)topLeftCorner.Y);
            var size = new System.Drawing.Size((int)map.ActualWidth, (int)map.ActualHeight);

            Bitmap screenShot = new Bitmap((int)map.ActualWidth, (int)map.ActualHeight);

            using (var graphics = Graphics.FromImage(screenShot))
            {
                graphics.CopyFromScreen(topLeftGdiPoint, new System.Drawing.Point(),
                     size, CopyPixelOperation.SourceCopy);
            }

            return screenShot;
        }*/

        public void Dispose()
        {
            try
            {
                this.DataContext = null;

                if (ViewModel != null)
                    ViewModel.OnUIMessage -= BingMapsView_OnUIMessage;

                mapGrid.Children.Clear();

                ClearMap();

                bdrMap.Child = null;

                //Dispose and unrefence map completely
                map.Kill();
            }
            catch { }
        }
    }
}
