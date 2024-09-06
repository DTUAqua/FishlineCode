using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FishLineMeasure.ViewModels.Lenghts;
using Anchor.Core;
using System.IO;
using System.Windows.Forms;
using Microsoft.Practices.Prism.Commands;

namespace FishLineMeasure.ViewModels.Overview
{
    public class XMLConvertCSV : AViewModel
    {
        private List<StationViewModel> _Stationss { get; set; } = new List<StationViewModel>();
        public List<Headers> Headers { get; set; } = new List<Headers>();
        public List<XMLInfoCLass> XMLDoucumentImportet { get; set; } = new List<XMLInfoCLass>();
        public string PathForCSV { get; set; }

        public XMLConvertCSV(List<StationViewModel> LStations, string path)
        {
            PathForCSV = path;
            XMLDoucumentImportet = new List<XMLInfoCLass>();
            _Stationss = LStations;
            SetAllHeaders();
            GetFileInfo();
            MakeDocument();
        }

        
        public void SetAllHeaders()
        {
            Headers = new List<Headers>();
            Headers.Add(new Overview.Headers() { Name = "Year" });
            Headers.Add(new Overview.Headers() { Name = "Cruise" });
            Headers.Add(new Overview.Headers() { Name = "Trip" });
            Headers.Add(new Overview.Headers() { Name = "Station" });
            Headers.Add(new Overview.Headers() { Name = "DateTime" });
            Headers.Add(new Overview.Headers() { Name = "Length" });
            Headers.Add(new Overview.Headers() { Name = "Unit" });
            foreach (var S in _Stationss)
            {
                var newXdocument = XElement.Load(S.FilePath);
                var xMesurments = newXdocument.Element("Measurements");
                foreach (XElement xe in newXdocument.Descendants("Measurement"))
                {
                    var xLookups = xe.Element("Lookups");
                    foreach (var Lookups in xLookups.Descendants("Lookup"))
                    {
                        var temp = Headers;
                        temp = temp.Where(X => X.Name == Lookups.FirstAttribute.Value.ToString()).ToList();
                        if (temp.Count == 0)
                            Headers.Add(new Overview.Headers() { Name = Lookups.FirstAttribute.Value.ToString() });
                    }
                }
            }
        }
        public void GetFileInfo()
        {
            var nfi = new System.Globalization.NumberFormatInfo();
            var strDelimiter = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var strDigitGrouping = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            nfi.NumberDecimalSeparator = strDelimiter;

            foreach (var S in _Stationss)
            {
                var newXdocument = XElement.Load(S.FilePath);
                var xHeader = newXdocument.Element("Header");
                var xMesurments = newXdocument.Element("Measurements");
                foreach (XElement xe in xMesurments.Descendants("Measurement"))
                {
                    var xLookups = xe.Element("Lookups");
                    Dictionary<string, string> _lookups = new Dictionary<string, string>();
                    foreach (var item in xLookups.Descendants("Lookup"))
                    {
                        var type = item.FirstAttribute.Value;
                        var value = item.FirstAttribute.NextAttribute.Value;
                        _lookups.Add(type, value);
                    }
                    var lenght = xe.Element("Length").Value.ToDouble().ToString(nfi);

                    XElement xeStationNumber = null;

                    xeStationNumber = xHeader.Element("StationNumber");
                    if (xeStationNumber == null)
                        xeStationNumber = newXdocument.Element("StationNumber");

                    var xLength = xe.Element("Length");
                    BusinessLogic.Unit unit = BusinessLogic.Unit.MM;

                    var xaUnit = xLength.Attribute("unit");
                    if (xaUnit != null && xaUnit.Value != null)
                        Enum.TryParse<BusinessLogic.Unit>(xaUnit.Value, out unit);

                    XMLDoucumentImportet.Add(new XMLInfoCLass()
                    {
                        year = xHeader.Element("Year").Value,
                        cruise = xHeader.Element("Cruise").Value,
                        trip = xHeader.Element("Trip").Value,
                        station = xeStationNumber == null ? "" : xeStationNumber.Value,
                        Length = xLength.Value.ToDouble().ConvertToUnit(unit, BusinessLogic.Unit.MM).Truncate(0).ToString(nfi),
                        Unit = BusinessLogic.Unit.MM.ToString(),
                        date = xe.Attribute("dateTimeUTC").Value,
                        Lookups = _lookups
                    });
                }
            }
        }
      
        private void MakeDocument()
        {
            var strSeperator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            string HeaderTags = "";
            try
            {
                foreach (var h in Headers)
                {
                    if (HeaderTags != "")
                    {
                        HeaderTags = $"{HeaderTags}{strSeperator}{h.Name}";
                    }
                    else
                    {
                        HeaderTags = $"{h.Name}";
                    }
                }
                using (FileStream fs = new FileStream(PathForCSV, FileMode.Create))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(HeaderTags);
                    foreach (var L in XMLDoucumentImportet)
                    {
                        string NewLine = $"{L.year}{strSeperator}{L.cruise}{strSeperator}{L.trip}{strSeperator}{L.station}{strSeperator}{L.date}{strSeperator}{L.Length}{strSeperator}{L.Unit}";
                        var AmountOfLookupsInThisLenght = L.Lookups.Count();
                        var _LookUps = GetAmountOfLookupsAddedToHeader();
                        foreach (var _L in _LookUps)
                        {
                            bool blnFound = false;
                            foreach (var __L in L.Lookups)
                            {
                                if (_L.Name == __L.Key)
                                {
                                    NewLine += $"{strSeperator}{__L.Value}";
                                    blnFound = true;
                                    break;
                                }
                            }

                            if(!blnFound)
                                NewLine += strSeperator;
                        }
                        sw.WriteLine(NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                DispatchMessageBox(ex.Message);
            }
        }
        private List<Headers> GetAmountOfLookupsAddedToHeader()
        {
            var temp = Headers;
            temp = Headers.Where(x => x.Name != "Year" && x.Name != "Cruise" && x.Name != "Trip" && x.Name != "Station" && x.Name != "Date" && x.Name != "Length" && x.Name != "Unit" && x.Name != "DateTime").ToList();
            return temp;
        }

    }
    public class Headers
    {
        public string Name { get; set; }
    }

    public class XMLInfoCLass
    {
        public string year;
        public string cruise;
        public string trip;
        public string station;
        public string date;
        public string Length;
        public string Unit;
        public Dictionary<string, string> Lookups;
    }
}
