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
    public partial class MapsuiMapView : System.Windows.Controls.UserControl, IDisposable
    {
        public MapViewModelMapsuiControl ViewModel
        {
            get { return this.DataContext as MapViewModelMapsuiControl; }
        }

        public MapsuiMapView()
        {
            InitializeComponent();

            var myMap = new Mapsui.Map();

            Build_Map(myMap);

            map.Map = myMap;

            try
            {
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

        private Mapsui.IFeature _lastHoveredFeature;
        private List<IStyle> _lastOriginalStyles;
        private readonly string[] _hoverableLayers = { "LabelLayer", "LineLayer" };


        public void InitializeHoverHandlers()
        {
            map.MouseMove += MapControl_MouseMove;
            map.MouseLeave += MapControl_MouseLeave;
        }

        private void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var pos = e.GetPosition(map);
                var screenPosition = new Mapsui.MPoint(pos.X, pos.Y);
                var mapInfo = map.GetMapInfo(screenPosition);

                var feature = mapInfo.Feature;
                var layer = map.Map.Layers
                    .OfType<MemoryLayer>()
                    .FirstOrDefault(l => l.Features.Contains(feature));

                if (feature == null || layer == null || !_hoverableLayers.Contains(layer.Name))
                {
                    ResetHoverState();
                    return;
                }

                if (ReferenceEquals(feature, _lastHoveredFeature))
                    return;

                ResetHoverState();

                _lastOriginalStyles = feature.Styles?.ToList() ?? new List<IStyle>();
                _lastHoveredFeature = feature;
                feature.Styles.Clear();

                if (layer.Name == "LineLayer")
                {
                    feature.Styles.Add(GetRoundedLineStyle(7, Color.Black));
                    feature.Styles.Add(GetRoundedLineStyle(5, Color.Yellow));
                }
                else if (layer.Name == "LabelLayer")
                {
                    var label = _lastOriginalStyles.OfType<LabelStyle>().FirstOrDefault();
                    if (label != null)
                    {
                        var hoverLabel = new LabelStyle
                        {
                            Text = GetLabelText(label),
                            LabelColumn = label.LabelColumn,
                            Font = label.Font,
                            ForeColor = Color.Yellow,
                            BackColor = new Brush(Color.Black),
                            HorizontalAlignment = label.HorizontalAlignment,
                            VerticalAlignment = label.VerticalAlignment
                        };
                        feature.Styles.Add(hoverLabel);
                    }
                }

                layer.DataHasChanged();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MapControl_MouseMove error: {ex}");
            }
        }

        private void MapControl_MouseLeave(object sender, MouseEventArgs e)
        {
            ResetHoverState();
        }

        private void ResetHoverState()
        {
            try
            {
                if (_lastHoveredFeature == null)
                    return;

                _lastHoveredFeature.Styles.Clear();
                foreach (var style in _lastOriginalStyles)
                    _lastHoveredFeature.Styles.Add(style);

                var layer = map.Map.Layers
                    .OfType<MemoryLayer>()
                    .FirstOrDefault(l => l.Features.Contains(_lastHoveredFeature));
                layer?.DataHasChanged();

                _lastHoveredFeature = null;
                _lastOriginalStyles = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ResetHoverState error: {ex}");
            }
        }
        private string GetLabelText(LabelStyle labelStyle)
        {
            try
            {
                var prop = typeof(LabelStyle).GetProperty("Text",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance);
                return prop?.GetValue(labelStyle) as string ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
        private void ZoomOut_Click(object sender, EventArgs e)
        {
            try
            {
                if (map != null)
                {
                    //3
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
                //13
                if (GetZoomLevel() < 19)
                {
                    map.Map.Navigator.ZoomIn();
                }
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }


        protected void MapsuiMapsView_Loaded(object sender, RoutedEventArgs e)
        {
            if (map?.Map == null)
                return;

            RebuildMap();
        }

        protected void MapView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                if ((e.OldValue as MapViewModelMapsuiControl) != null)
                {
                    (e.OldValue as MapViewModelMapsuiControl).OnUIMessage -= MapsuiMapsView_OnUIMessage;
                }

                MapViewModelMapsuiControl mvm = null;
                if ((mvm = e.NewValue as MapViewModelMapsuiControl) != null)
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
        
        private void RebuildMap()
        {

            try
            {
                var vm = ViewModel;

                string geoFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "GeoJson");

                /*************Line file optimized for map integration************/
                string fileName = "ices_areas.geojson";                           

                /*************AREAS simplified***********************************/
                //string fileName = "icesAreas_simplified.geojson";            

                var features = ParseGeoJsonFile(System.IO.Path.Combine(geoFolder, fileName));
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

                        var latStop = MapViewModel.ConvertPositionFromDegreesToDouble(p.LatitudeStop ?? "00.00.000 N");
                        var lonStop = MapViewModel.ConvertPositionFromDegreesToDouble(p.LongitudeStop ?? "00.00.000 E");
                        var (x, y) = SphericalMercator.FromLonLat(lonStop, latStop);

                        DrawNumberAtLocation(x, y, p.StationName);

                        vTmp = new Vec2d(latStop, lonStop);
                        vMin = VMathd.Min(vTmp.Value, vMin);
                        vMax = VMathd.Max(vTmp.Value, vMax);

                        boundingBox = boundingBox == null ? new MRect(x, y, x, y) : boundingBox.Join(new MRect(x, y, x, y));
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

                            DrawLineWithLabel(x1, y1, x2, y2, point.StationName);

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

                if (vTmp != null)
                {
                    new Action(() =>
                    {
                        CenterMapToBounds(boundingBox);
                        new Action(() =>
                        {
                            CenterMapToBounds(boundingBox);
                            new Action(() =>
                            {
                                CenterMapToBounds(boundingBox);
                            }).Dispatch(System.Windows.Threading.DispatcherPriority.ContextIdle);
                        }).Dispatch(System.Windows.Threading.DispatcherPriority.ContextIdle);

                    }).Dispatch(System.Windows.Threading.DispatcherPriority.Render);
                }

                var topLayer = map.Map.Layers.FirstOrDefault(l => l.Name == "NumberLayer");
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

        private void CenterMapToBounds(Mapsui.MRect boundingBox)
        {
            try
            {
                if (map.Map != null)
                {
                    double marginFactor = 0.1;
                    double paddingX = boundingBox.Width * marginFactor;
                    double paddingY = boundingBox.Height * marginFactor;
                    var expandedBox = new Mapsui.MRect(
                    boundingBox.MinX - paddingX,
                    boundingBox.MinY - paddingY,
                    boundingBox.MaxX + paddingX,
                    boundingBox.MaxY + paddingY
                    );
                    map.Map.Navigator.ZoomToBox(expandedBox, Mapsui.MBoxFit.Fit);
                }
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }


        #region IcesAreasFunctions
        private JToken ParseGeoJsonFile(string filePath)
        {
            if (!File.Exists(filePath))
                return null;
            string json = File.ReadAllText(filePath);
            var obj = JObject.Parse(json);
            var features = obj["features"];
            if (features == null) return null;
            else return features;

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

        private void ClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            new Action(() =>
            {
                try
                {
                    Bitmap bmp = GetBrowserScreenshot();
                    var bmpSource = bmp.ToBitmapSource();

                    System.Windows.Clipboard.SetImage(bmpSource);

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
                    bool? blnRes = sfd.ShowDialog(System.Windows.Application.Current.MainWindow);

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
        public int GetZoomLevel()
        {
            double resolution = map.Map?.Navigator?.Viewport.Resolution ?? 0;

            const double initialResolution = 156543.03392804097; // zoom 0
            double zoomLevel = Math.Log(initialResolution / resolution, 2);
            int zoom = (int)Math.Round(zoomLevel);


            return zoom;
        }
        private void Build_Map(Mapsui.Map myMap)
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


            //MAP LABELS
            /*
            var labelSource = new HttpTileSource(
                new GlobalSphericalMercator(),
                "https://tiles.basemaps.cartocdn.com/light_only_labels/{z}/{x}/{y}.png",
                name: "Labels",
                attribution: new BruTile.Attribution("© OpenStreetMap contributors, © CARTO", "https://carto.com/attributions")
            );

            myMap.Layers.Add(new TileLayer(labelSource)
            {
                Name = "Labels"
            });
            */

        }
        
        #region MapsuiDrawFunctions
        public void DrawNumberAtLocation(double x, double y, string stationName)
        {
            var ntsPoint = new Point(x, y);

            var feature = new GeometryFeature
            {
                Geometry = ntsPoint,
                Styles = new List<IStyle>
                {
                    GetLabelStyle(stationName, Color.Red)
                }
            };

            var layer = GetOrCreateMemoryLayer("LabelLayer");
            ((List<Mapsui.IFeature>)layer.Features).Add(feature);
            layer.DataHasChanged();

        }
        public void DrawPoint(double x, double y)
        {
            var ntsPoint = new Point(x, y);

            var feature = new GeometryFeature
            {
                Geometry = ntsPoint,
                Styles = new List<IStyle>
                {
                    new SymbolStyle
                    {
                        SymbolScale = 0.25,
                        Fill = new Brush(Color.Red),
                        Outline = new Pen(Color.Black, 0.2),
                        SymbolType = SymbolType.Ellipse
                    }
                }
            };

            var layer = GetOrCreateMemoryLayer("PointLayer");
            ((List<Mapsui.IFeature>)layer.Features).Add(feature);
            layer.DataHasChanged();
        }
        public void DrawLineWithLabel(double x1, double y1, double x2, double y2, string labelText)
        {

            List<Coordinate> coordinates = new List<Coordinate> { new Coordinate(x1, y1), new Coordinate(x2, y2) };

            LineString lineString = new LineString(coordinates.ToArray());

            var feature = new GeometryFeature
            {
                Geometry = lineString,
                Styles = new List<IStyle> { GetRoundedLineStyle(3, Color.Purple) }
            };

            var lineLayer = GetOrCreateMemoryLayer("LineLayer");
            ((List<Mapsui.IFeature>)lineLayer.Features).Add(feature);
            lineLayer.DataHasChanged();


            double midX = (x1 + x2) / 2;
            double midY = (y1 + y2) / 2;

            DrawNumberAtLocation(midX, midY, labelText);

        }

        public void DrawLineWithoutLabel(double x1, double y1, double x2, double y2)
        {

            List<Coordinate> coordinates = new List<Coordinate> { new Coordinate(x1, y1), new Coordinate(x2, y2) };

            LineString lineString = new LineString(coordinates.ToArray());

            var feature = new GeometryFeature
            {
                Geometry = lineString,
                Styles = new List<IStyle> { GetRoundedLineStyle(2, Color.Magenta) }
            };

            var lineLayer = GetOrCreateMemoryLayer("BorderLayer");
            ((List<Mapsui.IFeature>)lineLayer.Features).Add(feature);
            lineLayer.DataHasChanged();
        }

        public void DrawMultiLineWithoutLabel(List<Coordinate> coords)
        {

            var transformedCoords = coords.Select(v => SphericalMercator.FromLonLat(v.X, v.Y).ToCoordinate()).ToArray();

            var lineString = new LineString(transformedCoords);

            var feature = new GeometryFeature
            {
                Geometry = lineString,
                Styles = new List<IStyle> { GetRoundedLineStyle(1.5, Color.DarkCyan) }
            };

            var lineLayer = GetOrCreateMemoryLayer("BorderLayer");
            ((List<Mapsui.IFeature>)lineLayer.Features).Add(feature);
            lineLayer.DataHasChanged();

        }

        public void DrawPolygonWithoutLabel(List<Coordinate> coordinates)
        {
            if (!coordinates[0].Equals2D(coordinates[coordinates.Count - 1]))
            {
                coordinates.Add(new Coordinate(coordinates[0].X, coordinates[0].Y));
            } //close the polygon
                
            LineString lineString = new LineString(coordinates.Select(v => SphericalMercator.FromLonLat(v.X, v.Y).ToCoordinate()).ToArray());

            var feature = new GeometryFeature
            {
                Geometry = lineString,
                Styles = new List<IStyle> { GetRoundedLineStyle(2, Color.Magenta, 0.25f) }
            };

            var polygonLayer = GetOrCreateMemoryLayer("PolygonLayer");
            ((List<Mapsui.IFeature>)polygonLayer.Features).Add(feature);
            polygonLayer.DataHasChanged();
        }
        #endregion

        #region StyleUtils
        public IStyle GetLabelStyle(string stationName, Color color)
        {
            return new LabelStyle
            {
                Text = stationName,
                Font = new Font { FontFamily = "Arial", Size = 17, Bold = true },
                ForeColor = Color.Red,
                BackColor = new Brush(Color.Transparent),
                Halo = new Pen(Color.White, 1),
                HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Center,
                VerticalAlignment = LabelStyle.VerticalAlignmentEnum.Bottom
            };
        }
        public MemoryLayer GetOrCreateMemoryLayer(string name)
        {
            var layer = map.Map.Layers.FirstOrDefault(l => l.Name == name) as MemoryLayer;
            if (layer == null)
            {
                layer = new MemoryLayer
                {
                    Name = name,
                    Enabled = true,
                    IsMapInfoLayer = true,
                    Features = new List<Mapsui.IFeature>()
                };
                map.Map.Layers.Add(layer);
            }
            layer.Style = null;
            return layer;
        }
        public IStyle GetRoundedLineStyle(double width, Color color, float opacity = 1, PenStyle penStyle = PenStyle.Solid, double minVisible = 0, double maxVisible = double.MaxValue)
        {
            return new VectorStyle
            {
                Line = new Pen
                {
                    Color = color,
                    PenStrokeCap = PenStrokeCap.Round,
                    StrokeJoin = StrokeJoin.Round,
                    PenStyle = penStyle,
                    Width = width
                },
                MinVisible = minVisible,
                MaxVisible = maxVisible,
                Opacity = opacity
            };
        }
        public void ClearLayer(string name)
        {
            var layer = map.Map.Layers.FirstOrDefault(l => l.Name == name) as MemoryLayer;
            if (layer != null)
            {
                ((List<Mapsui.IFeature>)layer.Features).Clear();
                layer.DataHasChanged();
            }
        }
        public void ClearAllLayers()
        {
            foreach (var layer in map.Map.Layers.OfType<MemoryLayer>())
            {
                ((List<Mapsui.IFeature>)layer.Features).Clear();
                layer.DataHasChanged();
            }
        }
        #endregion
        public void Dispose()
        {
            try
            {
                this.DataContext = null;
                map.Map?.Layers.Clear();
                if (ViewModel != null)
                    ViewModel.OnUIMessage -= MapsuiMapsView_OnUIMessage;
                mapGrid.Children.Clear();
                map.Children.Clear();
                bdrMap.Child = null;
                map.Map = null;
                map = null;
                map.Dispose();
            }
            catch (Exception ex)
            {
                Anchor.Core.Loggers.Logger.LogError(ex);
            }
        }
    }
}
