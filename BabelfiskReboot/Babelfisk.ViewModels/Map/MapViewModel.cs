using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Babelfisk.Entities.Sprattus;
using Anchor.Core;
using GeometricLibrary.Core.Vector;
using System.Threading.Tasks;
using Babelfisk.BusinessLogic.Settings;
using Microsoft.Practices.Prism.Commands;
using System.Threading;

namespace Babelfisk.ViewModels.Map
{
    public class MapViewModel : AViewModel, IMapViewModel
    {
        private string _mapContentAsString;

        private MapDisplayMode _enmDisplayMode;

        private object _objSource = null;

        private List<MapPoint> _lstPoints;

        private bool _blnIsEnabled;

        private bool _blnInitialized;

        private bool _blnIsWindow = false;

        private bool _blnShowWebBrowser = true;

        private DelegateCommand _cmdShowMapInWindow;
        private DelegateCommand _cmdRefeshMap;

        private bool _blnIsHidden = true;

        private bool _blnIsConnected = false;

        private static CultureInfo _ci = CultureInfo.InvariantCulture;


        #region Properties

        private string MapContentAsString
        {
            get
            {
                string strMapWithPrint = null;
                 // if (_BabelFisk.ShowMapInMainFrame) { strMapWithPrint = MapBodyWithPrint(); }
                 // else { strMapWithPrint = MapBodyWithOutPrint(); }
                strMapWithPrint = MapBodyWithOutPrint();

                _mapContentAsString =
                    "<html>\n" +
                        "<head>\n" +
                            "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/>\n" +
                            "<script type=\"text/javascript\" src=\"http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.3\"></script>\n" +
                            "<script type=\"text/javascript\">\n" +
                                "try" +
                                "{" +
                                    "var map, pushpin, linePoints, lineColor, lineWidth, lineId, poly;\n" +
                                    "var initialized = false;\n" +
                                    "var centered = false;\n" +
                                    "var cent, zo;\n" +
                                    "var pushpinOnMap = true;\n\n" +
                                    "function GetMap()\n" +
                                    "{\n" +
                                        "map = new VEMap('myMap');\n" +
                                        "map.onLoadMap = OnLoadMap;\n" +
                                        "map.LoadMap(new VELatLong(" + (55.4).ToString(CultureInfo.InvariantCulture) + ", " + (10.2).ToString(CultureInfo.InvariantCulture) + "), 6, VEMapStyle.Aerial, false);\n\n";

                                    //_mapContentAsString += GetMapPrimitives();
                                    _mapContentAsString +=

                                    "map.HideDashboard();\n" +
                                    "map.HideScalebar();\n" +
                                    "hideLogo()\n" +
                                    "map.AttachEvent('onmousewheel', onMouseWheel);\n" +
                                    "initialized = true; \n" +
                                    "}\n\n" +

                                    "function OnLoadMap(){ \n" + 
                                     GetMapPrimitives() +
                                     " }\n" +

                                "function PrintMap()\n" +
                                "{\n" +
                                    "var printOpt = new VEPrintOptions(true);\n" +
                                    "map.SetPrintOptions(printOpt);\n" +
                                    "window.print();\n" +
                                "}\n\n" +

                                "function GetPosition(e) { alert('Latitude = ' + e.view.LatLong.Latitude + ', Longitude = ' + e.view.LatLong.Longitude) }\n\n" +

                                "function onMouseWheel(e)\n" +
                                "{\n" +
                                    "var delta = 0;\n" +
                                    "if (!event) { event = window.event; }\n\n" +

                                    "if (event.wheelDelta)\n" +
                                    "{\n" +
                                        "delta = event.wheelDelta/120;\n" +
                                        "if (window.opera) { delta = -delta; }\n" +
                                    "}\n" +
                                    "else if (event.detail) { delta = -event.detail/3; }\n\n" +

                                    "if (event.preventDefault) { event.preventDefault(); }\n\n" +

                                    "event.returnValue = false;\n" +
                                "}\n" +

                                "window.onresize = mapResize;\n" + 
                                "function mapResize() {\n" +
                                    "map.Resize(); \n" +
                                    /*"if(initialized && !centered) { map.SetCenterAndZoom(cent, zo); centered = true;  }\n" + */
                                "}\n" +

                                "function hideLogo() {\n" +
                                  "try { \n" +
                                    "document.getElementById(\"MSVE_PoweredByLogo\").style.display = \"none\";" + 
                                    "var elms = document.getElementsByTagName(\"div\"); \n" +
                                    "for (i = 0; i < elms.length; i++) { \n" +
                                        "if (elms[i].firstChild != null && elms[i].firstChild.nodeName.toLowerCase().indexOf(\"span\") != -1 && \n" +
                                            "elms[i].innerHTML.indexOf(\"Microsoft Corporation\") != -1) \n" + 
                                            "elms[i].style.display = \"none\"; \n" +
                                    "} \n" +
                                     "}catch(e) {} \n " + 
                                "} \n" +
                            "}" +
                            "catch(e) { alert('Map not avalible.'); }" +
                        "</script>\n" +
                    "</head>\n" + strMapWithPrint +
                    "</html>";

                return _mapContentAsString;
            }
        }


        public object Content
        {
            get { return _objSource; }
            set
            {
                _objSource = value;
                RaisePropertyChanged(() => Content);
            }
        }


        public List<MapPoint> Points
        {
            get { return _lstPoints; }
            set
            {
                _lstPoints = value;
                RaisePropertyChanged(() => Points);
                RaisePropertyChanged(() => HasPoints);
                RaisePropertyChanged(() => ShowWebBrowser);
            }
        }


        public bool IsPointsSelected
        {
            get { return _enmDisplayMode == MapDisplayMode.Points; }
            set
            {
                if (_enmDisplayMode == MapDisplayMode.Lines && value == true)
                {
                    _enmDisplayMode = MapDisplayMode.Points;
                    Refresh();
                }
                else if (_enmDisplayMode == MapDisplayMode.Points && value == false)
                {
                    _enmDisplayMode = MapDisplayMode.Lines;
                    Refresh();
                }

                if(_enmDisplayMode.ToString() != Settings.Instance.MapDisplayMode)
                    Settings.Instance.MapDisplayMode = _enmDisplayMode.ToString();

                RaisePropertyChanged(() => IsPointsSelected);
            }
        }


        public bool IsEnabled
        {
            get { return _blnIsEnabled; }
            set
            {
                _blnIsEnabled = value;

                if (value && !_blnInitialized)
                    RefreshAsync();

                if (value != BusinessLogic.Settings.Settings.Instance.IsMapEnabled)
                    Settings.Instance.IsMapEnabled = value;

                RaisePropertyChanged(() => IsEnabled);
            }
        }


        public bool IsWindow
        {
            get { return _blnIsWindow; }
            set
            {
                _blnIsWindow = value;
                RaisePropertyChanged(() => IsWindow);
            }
        }


        public bool ShowWebBrowser
        {
            get
            {
                return _blnShowWebBrowser;
               // return (HasPoints && IsConnected) || !_blnInitialized;
            }
            set
            {
                _blnShowWebBrowser = value;
                RaisePropertyChanged(() => ShowWebBrowser);
            }
        }


        public bool HasPoints
        {
            get { return (Points != null && Points.Count > 0 && Points.Where(x => !string.IsNullOrEmpty(x.LatitudeStop) && !string.IsNullOrEmpty(x.LongitudeStop)).Count() > 0) || !_blnInitialized; }
        }


        public bool IsHidden
        {
            get { return _blnIsHidden; }
            set
            {
                _blnIsHidden = value;
                RaisePropertyChanged(() => IsHidden);
            }
        }


        public bool IsConnected
        {
            get { return _blnIsConnected; }
            set
            {
                _blnIsConnected = value;
                RaisePropertyChanged(() => IsConnected);
                RaisePropertyChanged(() => ShowWebBrowser);
                RaisePropertyChanged(() => ShowNoConnectionMessage);
            }
        }


        public bool ShowNoConnectionMessage
        {
            get { return !IsConnected && HasPoints; }
        }
        

        #endregion



        public MapViewModel()
        {
            Enum.TryParse(Settings.Instance.MapDisplayMode, out _enmDisplayMode);
            _blnIsEnabled = Settings.Instance.IsMapEnabled;
            WindowWidth = 700;
            WindowHeight = 500;
            _blnInitialized = false;
        }


        private string MapBodyWithOutPrint()
        {
            return "<body onload=\"GetMap();\" style=\"height: 100%; width: 100%; margin: 0;\" scroll=\"no\" topmargin=\"0\" leftmargin=\"0\" >\n" +
                    "<div id=\"myMap\" style=\"position: fixed; top: 0px; left: 0px; right:0px; bottom:0px; z-index: 100; width: 100%; height: 100%;\"></div>\n" + 
                  //  "<a href=\"javascript:PrintMap();\" style=\"z-index: 110; position: fixed; top: 0px; left: 4px; color: white;\" >Print</a>\n" +
                    "</body>\n";
        }


        public Task RefreshAsync()
        {
            return Task.Factory.StartNew(Refresh);
        }

        public void Refresh()
        {
            if (!IsEnabled)
                return;

            string strContent = MapContentAsString;

            _blnInitialized = true;

            if (Points == null || Points.Count == 0 || Points.Where(x => !string.IsNullOrEmpty(x.LatitudeStop) && !string.IsNullOrEmpty(x.LongitudeStop)).Count() == 0)
            {
                new Action(() =>
                {
                    ShowWebBrowser = false;
                    RaisePropertyChanged(() => HasPoints);
                    RaisePropertyChanged(() => ShowNoConnectionMessage);
                }).Dispatch();
                return;
            }

            if (!IsConnectedToNet())
            {
                new Action(() =>
                {
                    IsConnected = false;
                    ShowWebBrowser = false;
                }).Dispatch();
                return;
            }

            if (!IsConnected)
            {
                new Action(() =>
                {
                    IsConnected = true;
                }).Dispatch();
            }

            //Refresh this is a new thread, so the webbrowser has time to load before data is depicted.
            Task.Factory.StartNew(() =>
            {
                new Action(() =>
                {
                    try
                    {
                        if(!ShowWebBrowser)
                            ShowWebBrowser = true;
                        Content = strContent;
                    }
                    catch { };
                }).Dispatch();
            });
        }


        private string GetMapPrimitives()
        {
            string strPrimitives = "";

            if (Points != null && Points.Count > 0)
            {
                switch (_enmDisplayMode)
                {
                    case MapDisplayMode.Lines:
                        strPrimitives += GetLines();
                        break;

                    case MapDisplayMode.Points:
                        strPrimitives += GetPoints();
                        break;
                }
            }

            return strPrimitives;
        }



        /// <summary>
        /// Retrieve javascript for centering and zooming around position (lat, lon)
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        private string CenterAndZoom(double lat, double lon)
        {
            return "map.SetCenterAndZoom(new VELatLong(" + (lat).ToString(_ci) + "," + (lon).ToString(_ci) + "), 5);";
        }


        private string Center(double lat, double lon)
        {
            //Set global javascript cent variable to the center of the coordinates.
            string strCenter = "cent = new VELatLong(" + (lat).ToString(_ci) + "," + (lon).ToString(_ci) + ");\n";

            strCenter += "map.SetCenter(cent);\n"; 

            return strCenter;
        }


        private string SetView()
        {
            string str = "map.SetMapView(latlonArray);\n";
            str += "zo = map.GetZoomLevel();\n";
            str += "var minv = Math.min(zo, 6);\n";
            //Set the global javascript zo value to the zoom level.
            str += "zo = minv;\n";
            str += "map.SetZoomLevel(minv);\n";

            return str;
        }


        private string GetLines()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("lineColor = new VEColor(255,0,0,1);\n" + "lineWidth = 2;\n");
            sb.Append("map.ClearInfoBoxStyles();");

            Vec2d? vTmp = null;
            Vec2d vMin = new Vec2d(double.MaxValue);
            Vec2d vMax = new Vec2d(double.MinValue);
            sb.Append("var latlonArray = new Array();\n var increment = 0;\n");

            for (int i = 0; i < Points.Count; i++)
            {
                var point = Points[i];

                if (!string.IsNullOrEmpty(point.LatitudeStart) && !string.IsNullOrEmpty(point.LongitudeStart) && !string.IsNullOrEmpty(point.LatitudeStop) && !string.IsNullOrEmpty(point.LongitudeStop))
                {
                    var latStart = ConvertPositionFromDegreesToDouble(point.LatitudeStart);
                    var lonStart = ConvertPositionFromDegreesToDouble(point.LongitudeStart);
                    var latStop = ConvertPositionFromDegreesToDouble(point.LatitudeStop);
                    var lonStop = ConvertPositionFromDegreesToDouble(point.LongitudeStop);

                    

                   sb.Append("latlonArray[increment++] = new VELatLong(" + latStart.ToString(_ci) + ", " + lonStart.ToString(_ci) + ");\n");
                   sb.Append("latlonArray[increment++] = new VELatLong(" + latStop.ToString(_ci) + ", " + lonStop.ToString(_ci) + ");\n");


                    string strName = "Tur: " + point.TripName + ", Station: " + point.StationName;
                    string strPoint = "[new VELatLong(" + latStart.ToString(_ci) + "," + lonStart.ToString(_ci) + "), new VELatLong(" + latStop.ToString(_ci) + "," + lonStop.ToString(_ci) + ")]";
                    sb.Append("var shape" + i + " = new VEShape(VEShapeType.Polyline, " + strPoint + ");\n");
                    sb.Append("shape" + i + ".SetLineColor(lineColor);\n");
                    sb.Append("shape" + i + ".SetFillColor(lineColor);\n");
                    sb.Append("shape" + i + ".SetLineWidth(lineWidth);\n");
                    //sb.Append("shape" + i + ".HideIcon();");
                    sb.Append("shape" + i + ".SetCustomIcon('<div style=\\'width: 100%; height: 100%; \\'></div>');\n");
                    sb.Append("shape" + i + ".SetTitle('<span>" + point.TripName + " | " + point.StationName + "</span>');\n");
                    sb.Append("shape" + i + ".SetDescription('<span>" + strName + "</span>');\n");
                    sb.Append("map.AddShape(shape" + i + ");\n");

                    vTmp = new Vec2d(latStart, lonStart);
                    vMin = VMathd.Min(vTmp.Value, vMin);
                    vMax = VMathd.Max(vTmp.Value, vMax);
                    vTmp = new Vec2d(latStop, lonStop);
                    vMin = VMathd.Min(vTmp.Value, vMin);
                    vMax = VMathd.Max(vTmp.Value, vMax);
                }
            }

            if (vTmp != null)
            {
                Vec2d vMid = (vMin + vMax) * 0.5;
                sb.Append(SetView());
                sb.Append(Center(vMid.X, vMid.Y));
            }

            return sb.ToString();
        }


        private string GetPoints()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("map.ClearInfoBoxStyles();\n");

            Vec2d? vTmp = null;
            Vec2d vMin = new Vec2d(double.MaxValue);
            Vec2d vMax = new Vec2d(double.MinValue);
            sb.Append("var latlonArray = new Array();\n var increment = 0;\n");

            for (int i = 0; i < Points.Count; i++)
            {
                var point = Points[i];

                if (!string.IsNullOrEmpty(point.LatitudeStop) && !string.IsNullOrEmpty(point.LongitudeStop))
                {
                    var latStop = ConvertPositionFromDegreesToDouble(point.LatitudeStop);
                    var lonStop = ConvertPositionFromDegreesToDouble(point.LongitudeStop);

                    sb.Append("latlonArray[increment++] = new VELatLong(" + latStop.ToString(_ci) + ", " + lonStop.ToString(_ci) + ");\n");

                    string strName = "Tur: " + point.TripName + ", Station: " + point.StationName;
                    string strPoint = point.TripName + " | " + point.StationName;
                    sb.Append("var shape" + i + " = new VEShape(VEShapeType.Pushpin, new VELatLong(" + latStop.ToString(_ci) + ", " + lonStop.ToString(_ci) + "));\n");
                    sb.Append("shape" + i + ".SetTitle('<span>" + strPoint + "</span>');\n");
                    sb.Append("shape" + i + ".SetDescription('<span>" + strName + "</span>');\n");
                    sb.Append("shape" + i + ".SetCustomIcon('<table style=\\'font-size:12px; font-weight:bold; color:red; \\'><tr><td>" + point.StationName/*strPoint*/ + "</td></tr></table>');\n");
                    sb.Append("map.AddShape(shape"+i+");\n");

                    vTmp = new Vec2d(latStop, lonStop);
                    vMin = VMathd.Min(vTmp.Value, vMin);
                    vMax = VMathd.Max(vTmp.Value, vMax);
                }
            }

            if (vTmp != null)
            {
                Vec2d vMid = (vMin + vMax) * 0.5;
                sb.Append(SetView());
                sb.Append(Center(vMid.X, vMid.Y));
            }

            return sb.ToString();
        }


        public static bool IsConnectedToNet()
        {
            try
            {
                string myAddress = "www.google.dk";
                IPAddress[] addresslist = Dns.GetHostAddresses(myAddress);

                if (addresslist[0].ToString().Length > 6)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }


        public static double ConvertPositionFromDegreesToDouble(string strPosition)
        {
            double dblPositionFromDegreesToDecimal = -1;

            if (string.IsNullOrEmpty(strPosition))
                return dblPositionFromDegreesToDecimal;

            try
            {
                int intFirstDot = -1;
                int intSecondDot = -1;
                int intThirdDot = -1;
                int intDegrees = -1;
                double dblMinutes = -1;
                string strDirection = null;
                

                if (strPosition.Length > 0)
                {
                    // ------------------------------------------------------
                    strDirection = strPosition[strPosition.Length - 1].ToString();
                    // ------------------------------------------------------

                    char cDecimalSeparator = Convert.ToChar(System.Globalization.CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator);

                    intFirstDot = strPosition.IndexOf('.', 0);
                    intSecondDot = strPosition.IndexOf('.', intFirstDot + 1);
                    intThirdDot = strPosition.IndexOf(' ', intSecondDot + 1);
                    intDegrees = int.Parse(strPosition.Substring(0, intFirstDot).ToString());

                    string strTemp = strPosition.Substring(intFirstDot + 1, intThirdDot - intFirstDot);
                    //strTemp = strTemp.Replace('.', cDecimalSeparator);
                    //strTemp = strTemp.Replace(',', cDecimalSeparator);

                    dblMinutes = strTemp.ToDouble();// double.Parse(ConvertToLocale(strTemp));
                    double dblCal = (intDegrees + (dblMinutes / 60));
                    dblPositionFromDegreesToDecimal = dblCal;

                    if (strDirection == "S" || strDirection == "W")
                    {
                        dblPositionFromDegreesToDecimal = -dblPositionFromDegreesToDecimal;
                        /*string strDblPositionFromDegreesToDecimal = dblPositionFromDegreesToDecimal.ToString();
                        strDblPositionFromDegreesToDecimal = ConvertToLocale(strDblPositionFromDegreesToDecimal);

                        string strPositionFromDegreesToDecimal = "-" + strDblPositionFromDegreesToDecimal;
                        strPositionFromDegreesToDecimal = ConvertToLocale(strPositionFromDegreesToDecimal);
                        dblPositionFromDegreesToDecimal = double.Parse(strPositionFromDegreesToDecimal);*/
                    }
                }
            }
            catch { }

            return dblPositionFromDegreesToDecimal;
        }


        private string ConvertToLocale(string strValue)
        {
            strValue = strValue.Replace(Convert.ToChar("."), Convert.ToChar(System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator));

            return strValue;
        }



        #region Show map in window Command


        public DelegateCommand ShowMapInWindowCommand
        {
            get { return _cmdShowMapInWindow ?? (_cmdShowMapInWindow = new DelegateCommand(ShowMapInWindow)); }
        }

        private void ShowMapInWindow()
        {
            MapViewModel mvm = new MapViewModel();
            mvm.Points = _lstPoints;
            mvm.IsWindow = true;
            mvm.WindowTitle = this.WindowTitle;
            mvm.IsHidden = false;

            AppRegionManager.LoadWindowViewFromViewModel(mvm, false, "WindowToolBoxSimple");
            mvm.RefreshAsync();
        }

        #endregion


        #region Refresh Map Command


        public DelegateCommand RefreshMapCommand
        {
            get { return _cmdRefeshMap ?? (_cmdRefeshMap = new DelegateCommand(RefreshMap)); }
        }

        private void RefreshMap()
        {
            RefreshAsync();
        }

        #endregion

    }
}
