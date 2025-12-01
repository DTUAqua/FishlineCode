using Anchor.Core;
using Babelfisk.Entities.Sprattus;
using Babelfisk.ViewModels.Map;
using BruTile.Predefined;
using BruTile.Web;
using ExCSS;
using GeometricLibrary.Core.Vector;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling.Layers;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brush = Mapsui.Styles.Brush;
using Color = Mapsui.Styles.Color;
using Exception = System.Exception;
using Font = Mapsui.Styles.Font;
using Pen = Mapsui.Styles.Pen;
using Point = NetTopologySuite.Geometries.Point;

namespace Babelfisk.WPF.Views.Map
{
    /// <summary>
    /// Interaction logic for MapsuiMapsView.xaml
    /// </summary>
    public partial class MapsuiMapView : UserControl, IDisposable
    {
        public MapViewModelMapsuiControl ViewModel
        {
            get { return this.DataContext as MapViewModelMapsuiControl; }
        }

        public MapsuiMapView()
        {
            try
            {
                InitializeComponent();
                var myMap = new Mapsui.Map();
                BuildMapsuiBaseMap(myMap);
                map.Map = myMap;
                this.DataContextChanged += MapView_DataContextChanged;
                map.Loaded += MapsuiMapsView_Loaded;

                if (DesignerProperties.GetIsInDesignMode(this))
                    this.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }

        }

        #region Listeners

        private readonly string[] _drawnLayers = { "LabelLayer", "LineLayer", "BorderLayer", "PointLayer", "PolygonLayer" };

        private bool _hoverInitialized = false;

        private IFeature _activeFeature = null;

        private DateTime _lastScrollTime = DateTime.MinValue;
        private const int ScrollCooldownMs = 200;


        public void InitializeHoverHandlers()
        {
            if (_hoverInitialized)
                return;

            _hoverInitialized = true;

            map.MouseMove += MapControl_MouseMove;
            map.MouseLeave += MapControl_MouseLeave;

            pointFeaturePopup.PreviewMouseWheel += Popup_PreviewMouseWheel;
            lineFeaturePopup.PreviewMouseWheel += Popup_PreviewMouseWheel;

        }

        public void RemoveHoverHandlers()
        {
            if (map != null)
            {
                map.MouseMove -= MapControl_MouseMove;
                map.MouseLeave -= MapControl_MouseLeave;

                pointFeaturePopup.PreviewMouseWheel -= Popup_PreviewMouseWheel;
                lineFeaturePopup.PreviewMouseWheel -= Popup_PreviewMouseWheel;

            }
        }

        private void ShowPopup(IFeature feature, System.Windows.Point mousePos)
        {
            bool isPoint = feature["IsPoint"] is bool b && b; ;

            var context = new
            {
                TripName = feature["TripName"],
                StationName = feature["StationName"],
                LatitudeStartDegreeMinutes = feature["LatitudeStartDegreeMinutes"],
                LongitudeStartDegreeMinutes = feature["LongitudeStartDegreeMinutes"],
                LatitudeStopDegreeMinutes = feature["LatitudeStopDegreeMinutes"],
                LongitudeStopDegreeMinutes = feature["LongitudeStopDegreeMinutes"]
            };

            double offsetX = mousePos.X + 12;
            double offsetY = mousePos.Y + 12;

            if (isPoint)
            {
                pointFeaturePopup.DataContext = context;
                pointFeaturePopup.HorizontalOffset = offsetX;
                pointFeaturePopup.VerticalOffset = offsetY;

                lineFeaturePopup.IsOpen = false;
                pointFeaturePopup.IsOpen = true;
            }
            else
            {
                lineFeaturePopup.DataContext = context;
                lineFeaturePopup.HorizontalOffset = offsetX;
                lineFeaturePopup.VerticalOffset = offsetY;

                pointFeaturePopup.IsOpen = false;
                lineFeaturePopup.IsOpen = true;
            }
        }

        private void Popup_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                _lastScrollTime = DateTime.Now;
                HidePopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }
        private void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (map == null)
                return;

            var pos = e.GetPosition(map);
            var mapPos = new MPoint(pos.X, pos.Y);
            try
            {
                var info = map.GetMapInfo(mapPos);
                var feature = info?.Feature;

                if (feature == _activeFeature)
                    return;

                if ((DateTime.Now - _lastScrollTime).TotalMilliseconds < ScrollCooldownMs)
                {
                    return;
                }

                if (!(map.Map.Layers.FirstOrDefault(l => l.Name == "LabelLayer") is MemoryLayer labelLayer) ||
                    !labelLayer.Features.Contains(feature))
                {
                    HidePopup();
                    return;
                }


                _activeFeature = feature;

                ShowPopup(feature, pos);
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }

        private void HidePopup()
        {
            _activeFeature = null;
            pointFeaturePopup.IsOpen = false;
            lineFeaturePopup.IsOpen = false;
        }

        private void MapControl_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                HidePopup();
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }


        protected void MapsuiMapsView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (map?.Map == null)
                    return;

                RebuildMap();
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }

        protected void MapView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if ((e.OldValue as MapViewModelMapsuiControl) != null)
                {
                    (e.OldValue as MapViewModelMapsuiControl).OnUIMessage -= MapsuiMapsView_OnUIMessage;
                }


                if (e.NewValue is MapViewModelMapsuiControl mvm)
                {
                    (e.NewValue as MapViewModelMapsuiControl).OnUIMessage += MapsuiMapsView_OnUIMessage;

                    if (mvm.IsWindow)
                    {
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

        protected void MapsuiMapsView_OnUIMessage(ViewModels.AViewModel vm, string msg)
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

        #endregion

        #region MapBuild

        private void RebuildMap()
        {
            try
            {

                ClearAllDrawnLayers();

                ClearTileCache();

                var vm = ViewModel;

                string geoFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "GeoJson");

                string fileName = "ices_areas.geojson";

                var features = ParseGeoJsonFile(Path.Combine(geoFolder, fileName));
                DrawGeoJson(features);

                if (vm.Points == null || !vm.IsEnabled)
                    return;

                Vec2d? vTmp = null;
                Vec2d vMin = new Vec2d(double.MaxValue);
                Vec2d vMax = new Vec2d(double.MinValue);

                MRect boundingBox = null;

                if (vm.IsPointsSelected)
                {
                    var groupsCount = vm.Points.GroupBy(x => x.TripName).Count();

                    bool blnShowTripName = groupsCount > 1;


                    foreach (var p in vm.Points)
                    {
                        if (p.LatitudeStart == null || p.LongitudeStop == null)
                            continue;

                        var latStop = MapViewModel.ConvertPositionFromDegreesToDouble(p.LatitudeStart ?? "00.00.000 N");
                        var lonStop = MapViewModel.ConvertPositionFromDegreesToDouble(p.LongitudeStart ?? "00.00.000 E");
                        var (x, y) = SphericalMercator.FromLonLat(lonStop, latStop);

                        DrawNumberAtLocation(x, y, p, true);

                        vTmp = new Vec2d(latStop, lonStop);
                        vMin = VMathd.Min(vTmp.Value, vMin);
                        vMax = VMathd.Max(vTmp.Value, vMax);

                        boundingBox = boundingBox == null ? new MRect(x - 10, y - 10, x + 10, y + 10) : boundingBox.Join(new MRect(x, y, x, y));
                    }
                }
                else
                {

                    foreach (var point in vm.Points)
                    {
                        if (!string.IsNullOrEmpty(point.LatitudeStart) && !string.IsNullOrEmpty(point.LongitudeStart) && !string.IsNullOrEmpty(point.LatitudeStop) && !string.IsNullOrEmpty(point.LongitudeStop))
                        {
                            var latStart = MapViewModel.ConvertPositionFromDegreesToDouble(point.LatitudeStart);
                            var lonStart = MapViewModel.ConvertPositionFromDegreesToDouble(point.LongitudeStart);
                            var latStop = MapViewModel.ConvertPositionFromDegreesToDouble(point.LatitudeStop);
                            var lonStop = MapViewModel.ConvertPositionFromDegreesToDouble(point.LongitudeStop);

                            var (x1, y1) = SphericalMercator.FromLonLat(lonStart, latStart);
                            var (x2, y2) = SphericalMercator.FromLonLat(lonStop, latStop);

                            if (Math.Abs(latStart - latStop) < 0.00001 && Math.Abs(lonStart - lonStop) < 0.00001)
                            {
                                latStop += 0.0001;
                                lonStop += 0.0001;
                            }

                            DrawLineWithLabel(x1, y1, x2, y2, point);

                            vTmp = new Vec2d(latStart, lonStart);
                            vMin = VMathd.Min(vTmp.Value, vMin);
                            vMax = VMathd.Max(vTmp.Value, vMax);
                            vTmp = new Vec2d(latStop, lonStop);
                            vMin = VMathd.Min(vTmp.Value, vMin);
                            vMax = VMathd.Max(vTmp.Value, vMax);
                            boundingBox = boundingBox == null ? new MRect(x1, y1, x2, y2) : boundingBox.Join(new MRect(x1, y1, x2, y2));
                        }
                    }
                }

                if (boundingBox != null)
                {
                    CenterMapToBounds(boundingBox);
                }

                var topLayer = map.Map.Layers.FirstOrDefault(l => l.Name == "LabelLayer");
                if (topLayer != null)
                {
                    map.Map.Layers.Remove(topLayer);
                    map.Map.Layers.Add(topLayer);
                }
                InitializeHoverHandlers();

            }
            catch (Exception e)
            {
                Anchor.Core.Loggers.Logger.LogError(e);
            }

        }

        private void CenterMapToBounds(MRect boundingBox)
        {
            try
            {
                if (map?.Map != null && boundingBox != null)
                {
                    double marginFactor = 0.2;
                    double paddingX = boundingBox.Width * marginFactor;
                    double paddingY = boundingBox.Height * marginFactor;
                    var expandedBox = new MRect(
                    boundingBox.MinX - paddingX,
                    boundingBox.MinY - paddingY,
                    boundingBox.MaxX + paddingX,
                    boundingBox.MaxY + paddingY
                    );
                    map.Map.Navigator.ZoomToBox(expandedBox, MBoxFit.Fit);

                    if (GetZoomLevel() > 15)
                    {
                        map.Map.Navigator.ZoomTo(15);
                    }
                }
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }


        #endregion

        #region IcesAreasFunctions

        private static JToken _cachedGeoJsonFeatures;
        private JToken ParseGeoJsonFile(string filePath)
        {
            try
            {
                if (_cachedGeoJsonFeatures != null)
                    return _cachedGeoJsonFeatures;

                if (!File.Exists(filePath))
                    return null;

                string json = File.ReadAllText(filePath);
                var obj = JObject.Parse(json);
                _cachedGeoJsonFeatures = obj["features"];

                return _cachedGeoJsonFeatures;

            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
                return null;
            }
        }
        private void DrawGeoJsonFeature(JToken feature)
        {
            var geometry = feature["geometry"];
            if (geometry == null) return;
            string type = geometry["type"].ToString();
            var coordinates = geometry["coordinates"];
            switch (type)
            {
                case "Point":
                    DrawPoint(coordinates);
                    break;
                case "LineString":
                    DrawLineString(coordinates);
                    break;
                case "Polygon":
                    DrawPolygon(coordinates);
                    break;
                case "MultiPolygon":
                    foreach (var polygon in coordinates)
                    {
                        DrawPolygon(polygon);
                    }
                    break;
                case "MultiLineString":
                    foreach (var line in coordinates)
                    {
                        DrawLineString(line);
                    }
                    break;
            }
        }

        private void DrawGeoJson(JToken features)
        {
            foreach (var feature in features)
            {
                DrawGeoJsonFeature(feature);
            }
        }

        private void DrawPoint(JToken coords)
        {
            double lon = coords[0].ToObject<double>();
            double lat = coords[1].ToObject<double>();
            DrawPoint(lon, lat);
        }
        private void DrawLineString(JToken coords)
        {
            var coordinates = new List<Coordinate>();
            foreach (var coord in coords)
            {
                double lon = coord[0].ToObject<double>();
                double lat = coord[1].ToObject<double>();
                coordinates.Add(new Coordinate(lon, lat));
            }
            DrawMultiLineWithoutLabel(coordinates);
        }

        private void DrawPolygon(JToken coords)
        {
            var coordinates = new List<Coordinate>();
            foreach (var coord in coords[0])
            {
                double lon = coord[0].ToObject<double>();
                double lat = coord[1].ToObject<double>();
                coordinates.Add(new Coordinate(lon, lat));
            }
            DrawPolygonWithoutLabel(coordinates);
        }
        #endregion

        #region UIImpl


        private void ZoomOut_Click(object sender, EventArgs e)
        {
            try
            {
                if (map != null)
                {
                    if (GetZoomLevel() > 3)
                    {
                        map.Map.Navigator.ZoomOut();
                    }
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
                if (GetZoomLevel() < 15)
                {
                    map.Map.Navigator.ZoomIn();
                }
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
                    Bitmap bmp = GetBrowserScreenshot();
                    var bmpSource = bmp.ToBitmapSource();

                    Clipboard.SetImage(bmpSource);

                    bmp?.Dispose();
                }
                catch (Exception ex)
                {
                    ViewModel?.AppRegionManager.ShowMessageBox("En uventet fejl opstod. " + ex.Message);
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

                    Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog
                    {
                        Filter = "JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|GIF (*.gif)|*.gif|Bitmap (*.bmp)|*.bmp|All Files|*.*"
                    };
                    bool? blnRes = sfd.ShowDialog(Application.Current.MainWindow);

                    if (blnRes.HasValue && blnRes.Value)
                        bmp.Save(sfd.FileName);

                    bmp?.Dispose();
                }
                catch (Exception ex)
                {
                    ViewModel?.AppRegionManager.ShowMessageBox("En uventet fejl opstod. " + ex.Message);
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
                        var img = new System.Windows.Controls.Image
                        {
                            Source = bmpSource
                        };

                        img.Measure(new System.Windows.Size(dlg.PrintableAreaWidth, dlg.PrintableAreaHeight));
                        img.Arrange(new Rect(new System.Windows.Point(0, 0), img.DesiredSize));

                        dlg.PrintVisual(img, "Map");
                    }

                    bmp?.Dispose();
                }
                catch (Exception ex)
                {
                    ViewModel?.AppRegionManager.ShowMessageBox("En uventet fejl opstod. " + ex.Message);
                }
            }).Dispatch();
        }
        private Bitmap GetBrowserScreenshot()
        {
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


        #endregion

        #region Utils

        public int GetZoomLevel()
        {
            double resolution = map.Map?.Navigator?.Viewport.Resolution ?? 1;

            const double initialResolution = 156543.03392804097;
            double zoomLevel = Math.Log(initialResolution / resolution, 2);
            int zoom = (int)Math.Round(zoomLevel);

            return zoom;
        }

        private void BuildMapsuiBaseMap(Mapsui.Map myMap)
        {
            try
            {
                var positronBase = new HttpTileSource(
                new GlobalSphericalMercator(),
                "https://basemaps.cartocdn.com/rastertiles/voyager_nolabels/{z}/{x}/{y}.png",
                name: "Carto Voyager No Labels",
                attribution: new BruTile.Attribution("© OpenStreetMap, © CARTO", "https://carto.com/")
            );

                var baseLayer = new TileLayer(positronBase)
                {
                    Name = "BaseLayer"
                };

                myMap.Layers.Add(baseLayer);

            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }
        public MemoryLayer GetOrCreateMemoryLayer(string name)
        {
            if (map?.Map == null) return null;

            if (!(map.Map?.Layers.FirstOrDefault(l => l.Name == name) is MemoryLayer layer))
            {
                layer = new MemoryLayer
                {
                    Name = name,
                    Enabled = true,
                    IsMapInfoLayer = true,
                    Features = new List<IFeature>()
                };
                map.Map.Layers.Add(layer);
            }
            layer.Style = null;
            return layer;
        }

        #endregion

        #region SharedStyles

        private static readonly SymbolStyle SharedPointStyle = new SymbolStyle
        {
            SymbolScale = 0.25,
            Fill = new Brush(Color.Red),
            Outline = new Pen(Color.Black, 0.2),
            SymbolType = SymbolType.Ellipse
        };


        private static readonly LabelStyle SharedLabelStyle = new LabelStyle
        {
            Font = new Font { FontFamily = "Arial", Size = 17, Bold = true },
            ForeColor = Color.Red,
            BackColor = new Brush(Color.FromArgb(255, 217, 234, 237)),
            Halo = new Pen(Color.White, 0.5),
            HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Center,
            VerticalAlignment = LabelStyle.VerticalAlignmentEnum.Bottom,
            LabelColumn = "StationName"
        };

        private static readonly VectorStyle SharedLineStylePurple3 = new VectorStyle
        {
            Line = new Pen
            {
                Color = Color.Purple,
                PenStrokeCap = PenStrokeCap.Round,
                StrokeJoin = StrokeJoin.Round,
                Width = 3
            }
        };

        private static readonly VectorStyle SharedLineStyleMagenta2 = new VectorStyle
        {
            Line = new Pen
            {
                Color = Color.Magenta,
                PenStrokeCap = PenStrokeCap.Round,
                StrokeJoin = StrokeJoin.Round,
                Width = 2
            }
        };

        private static readonly VectorStyle SharedLineStyleDarkCyan15 = new VectorStyle
        {
            Line = new Pen
            {
                Color = Color.DarkCyan,
                PenStrokeCap = PenStrokeCap.Round,
                StrokeJoin = StrokeJoin.Round,
                Width = 1.5
            }
        };

        private static readonly VectorStyle SharedPolygonLineStyle = new VectorStyle
        {
            Line = new Pen
            {
                Color = Color.Magenta,
                PenStrokeCap = PenStrokeCap.Round,
                StrokeJoin = StrokeJoin.Round,
                Width = 2
            },
            Opacity = 0.25f
        };

        #endregion

        #region MapsuiDrawFunctions
        public void DrawNumberAtLocation(double x, double y, MapPoint p, bool point)
        {
            var ntsPoint = new Point(x, y);

            var feature = new GeometryFeature
            {
                Geometry = ntsPoint,
                Styles = new List<IStyle> { SharedLabelStyle }
            };

            feature["StationName"] = p.StationName;
            feature["TripName"] = p.TripName;
            feature["LatitudeStartDegreeMinutes"] = p.LatitudeStartDegreeMinutes;
            feature["LongitudeStartDegreeMinutes"] = p.LongitudeStartDegreeMinutes;
            feature["LatitudeStopDegreeMinutes"] = p.LatitudeStopDegreeMinutes;
            feature["LongitudeStopDegreeMinutes"] = p.LongitudeStopDegreeMinutes;
            if (point) feature["IsPoint"] = true;
            else feature["IsPoint"] = false;

            var layer = GetOrCreateMemoryLayer("LabelLayer");
            ((List<IFeature>)layer.Features).Add(feature);
            layer.DataHasChanged();

        }

        public void DrawPoint(double x, double y)
        {
            var ntsPoint = new Point(x, y);

            var feature = new GeometryFeature
            {
                Geometry = ntsPoint,
                Styles = new List<IStyle> { SharedPointStyle }
            };

            var layer = GetOrCreateMemoryLayer("PointLayer");
            ((List<IFeature>)layer.Features).Add(feature);
            layer.DataHasChanged();
        }
        public void DrawLineWithLabel(double x1, double y1, double x2, double y2, MapPoint p)
        {

            List<Coordinate> coordinates = new List<Coordinate> { new Coordinate(x1, y1), new Coordinate(x2, y2) };

            LineString lineString = new LineString(coordinates.ToArray());

            var feature = new GeometryFeature
            {
                Geometry = lineString,
                Styles = new List<IStyle> { SharedLineStylePurple3 }
            };

            var lineLayer = GetOrCreateMemoryLayer("LineLayer");
            ((List<IFeature>)lineLayer.Features).Add(feature);
            lineLayer.DataHasChanged();


            double midX = (x1 + x2) / 2;
            double midY = (y1 + y2) / 2;

            DrawNumberAtLocation(midX, midY, p, false);

        }

        public void DrawLineWithoutLabel(double x1, double y1, double x2, double y2)
        {

            List<Coordinate> coordinates = new List<Coordinate> { new Coordinate(x1, y1), new Coordinate(x2, y2) };

            LineString lineString = new LineString(coordinates.ToArray());

            var feature = new GeometryFeature
            {
                Geometry = lineString,
                Styles = new List<IStyle> { SharedLineStyleMagenta2 }
            };

            var lineLayer = GetOrCreateMemoryLayer("BorderLayer");
            ((List<IFeature>)lineLayer.Features).Add(feature);
            lineLayer.DataHasChanged();

        }

        public void DrawMultiLineWithoutLabel(List<Coordinate> coords)
        {

            var transformedCoords = coords.Select(v => SphericalMercator.FromLonLat(v.X, v.Y).ToCoordinate()).ToArray();

            var lineString = new LineString(transformedCoords);

            var feature = new GeometryFeature
            {
                Geometry = lineString,
                Styles = new List<IStyle> { SharedLineStyleDarkCyan15 }
            };

            var lineLayer = GetOrCreateMemoryLayer("BorderLayer");
            ((List<IFeature>)lineLayer.Features).Add(feature);
            lineLayer.DataHasChanged();


        }

        public void DrawPolygonWithoutLabel(List<Coordinate> coordinates)
        {
            if (!coordinates[0].Equals2D(coordinates[coordinates.Count - 1]))
            {
                coordinates.Add(new Coordinate(coordinates[0].X, coordinates[0].Y));
            }

            LineString lineString = new LineString(coordinates.Select(v => SphericalMercator.FromLonLat(v.X, v.Y).ToCoordinate()).ToArray());

            var feature = new GeometryFeature
            {
                Geometry = lineString,
                Styles = new List<IStyle> { SharedPolygonLineStyle }
            };

            var polygonLayer = GetOrCreateMemoryLayer("PolygonLayer");
            ((List<IFeature>)polygonLayer.Features).Add(feature);
            polygonLayer.DataHasChanged();

        }
        #endregion

        #region MapDispose

        public void ClearLayer(string name)
        {
            if (map?.Map == null) return;

            if (map.Map?.Layers.FirstOrDefault(l => l.Name == name) is MemoryLayer layer)
            {
                ((List<IFeature>)layer.Features).Clear();
                layer.DataHasChanged();
            }
        }
        public void ClearAllDrawnLayers()
        {

            try
            {
                foreach (var layer in map.Map.Layers.OfType<MemoryLayer>().ToList())
                {
                    if (_drawnLayers.Contains(layer.Name))
                    {
                        ((List<IFeature>)layer.Features).Clear();
                        map.Map.Layers.Remove(layer);
                    }
                }
                map.RefreshGraphics();
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }

        public void ClearTileCache()
        {
            try
            {
                foreach (var tileLayer in map.Map.Layers.OfType<TileLayer>())
                {
                    tileLayer.ClearCache();
                }
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }

        public void Dispose()
        {
            try
            {
                RemoveHoverHandlers();

                this.DataContextChanged -= MapView_DataContextChanged;

                if (ViewModel != null)
                    ViewModel.OnUIMessage -= MapsuiMapsView_OnUIMessage;

                if (map != null)
                {
                    map.Loaded -= MapsuiMapsView_Loaded;
                    map.Map?.Layers.Clear();
                    map.Map?.Dispose();
                    map.Dispose();
                }

                mapGrid.Children.Clear();
                map = null;
                this.DataContext = null;
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }
        #endregion
    }
}
