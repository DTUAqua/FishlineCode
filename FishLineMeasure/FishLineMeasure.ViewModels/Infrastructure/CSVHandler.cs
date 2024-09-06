using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FishLineMeasure.ViewModels.Infrastructure
{
    public class CSVHandler : AViewModel
    {
        #region Properties
        private string _PathForCVS;
        private string _XMLPath;
        public string PathForCVS
        {
            get
            {
                return _PathForCVS;
            }
            set
            {
                _PathForCVS = value;
                RaisePropertyChanged(nameof(PathForCVS));
            }
        }
        public string XMLPath
        {
            get
            {
                return _XMLPath;
            }
            set
            {
                _XMLPath = value;
                RaisePropertyChanged(nameof(XMLPath));
            }
        }
  
        public List<XMLInfoClass> XMLInfoHolder { get; set; } = new List<XMLInfoClass>();

        #endregion

        public CSVHandler()
        {
            XMLInfoHolder = new List<XMLInfoClass>();
        }

        public void CreateCSVFile()
        {
            if (PathForCVS == null || XMLPath == null || !File.Exists(XMLPath))
            {
                return;
            }
            var xDoc = XElement.Load(XMLPath);
            var xHeader = xDoc.Element("Header");
            var xMesurments = xDoc.Element("Measurements");
            foreach (XElement xe in xMesurments.Descendants("Measurement"))
            {
                var xLookups = xe.Element("Lookups");
                Dictionary<string, string> __lookups = new Dictionary<string, string>();
                foreach (var item in xLookups.Descendants("Lookup"))
                {
                    var type = item.FirstAttribute.Value;
                    var value = item.FirstAttribute.NextAttribute.Value;
                    __lookups.Add(type, value);
                }

                XMLInfoHolder.Add(new XMLInfoClass()
                {
                    year = xHeader.Element("Year").Value,
                    cruise = xHeader.Element("Cruise").Value,
                    trip = xHeader.Element("Trip").Value,
                    Lenght = xe.Element("Length").Value,
                    date = xe.Attribute("dateTimeUTC").Value,
                    Lookups = __lookups
                });
            }
            List<string> CSVFile = new List<string>();
            foreach (var index in XMLInfoHolder)
            {
                string lookUpsString = "";
                foreach (var aLookup in index.Lookups)
                {
                    lookUpsString = lookUpsString + $"{aLookup.Key}:{aLookup.Value}   ";
                }
                CSVFile.Add($"{index.year},{index.cruise},{index.trip},{index.date},{index.Lenght},{lookUpsString}");
            }
            
            using (StreamWriter sw = new StreamWriter(PathForCVS, append: true))
            {
                foreach (var aLine in CSVFile)
                {
                    sw.WriteLine(aLine);
                }
            }
        }
    }

    public class XMLInfoClass
    {
        public string year;
        public string cruise;
        public string trip;
        public string date;
        public string Lenght;
        public Dictionary<string, string> Lookups;
    }
}
