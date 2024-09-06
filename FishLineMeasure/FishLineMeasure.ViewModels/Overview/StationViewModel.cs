using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Anchor.Core;
using FishLineMeasure.ViewModels.Infrastructure;

namespace FishLineMeasure.ViewModels.Overview
{
    public class StationViewModel : AViewModel
    {
        private string _stationNumber;

        private string _filePath;

        private bool _isMeasurementsLoaded = false;

        private TripViewModel _parentTrip;

        private object _tag;


        #region Properties


        public object Tag
        {
            get { return _tag; }
            set
            { 
                _tag = value;
            }
        }


        public string StationNumber
        {
            get { return _stationNumber; }
            set
            {
                _stationNumber = value;
                RaisePropertyChanged(nameof(StationNumber));
            }
        }


        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                RaisePropertyChanged(nameof(FilePath));
            }
        }

        public bool IsMeasurementsLoaded
        {
            get { return _isMeasurementsLoaded; }
            set
            {
                _isMeasurementsLoaded = value;
                RaisePropertyChanged(nameof(IsMeasurementsLoaded));
            }
        }

        public TripViewModel ParentTrip
        {
            get { return _parentTrip; }
            set
            {
                _parentTrip = value;
                RaisePropertyChanged(nameof(ParentTrip));
            }
        }


        public int MeasurementsCount
        {
            get
            {
                try
                {
                    if (!File.Exists(FilePath))
                        return 0 ;

                    var xElmStation = XElement.Load(FilePath);
                    if (xElmStation == null)
                        return 0;

                    var xeMeasurements = xElmStation.Element("Measurements");

                    if (xeMeasurements == null)
                        return 0;

                    return xeMeasurements.Elements().Count();
                }
                catch(Exception e)
                {
                    LogError(e);
                }

                return 0;
            }
        }


        #endregion



        public StationViewModel()
        {
            IsMeasurementsLoaded = false;
        }

  

        public bool LoadData(string filePath)
        {
            FilePath = filePath;

            if (!File.Exists(filePath))
                throw new ApplicationException("Station file not found.");

            string lastFolderName = Path.GetFileNameWithoutExtension(filePath);
            string[] tempArray = lastFolderName.Split(new string[] { "]_[", "[", "]" }, StringSplitOptions.None);
            tempArray = tempArray.Where(instance => !string.IsNullOrEmpty(instance)).ToArray();
            if (tempArray.Length < 4)
            {
                return false;
            }
            StationNumber = tempArray[3];

            return true;
        }


        public XElement GetStationXml()
        {
            var xElmStation = XElement.Load(FilePath);
            return xElmStation;
        }


        public XElement GetMeasurementsXml(XElement xeStation = null)
        {
            var xElmStation = xeStation ?? GetStationXml();
            var xElmMeasurements = xElmStation.Element("Measurements");

            return xElmMeasurements;
        }

 
        public List<MeasurementsClass> GetMeasurementClasses(XElement xeMeasurements = null, bool dispatchErrorMessage = true)
        {
            List<MeasurementsClass> res = new List<MeasurementsClass>();

            try
            {
                var xElmMeasurements = xeMeasurements ?? GetMeasurementsXml();

                foreach (var xElmMeasurement in xElmMeasurements.Elements())
                {
                    var test = xElmMeasurement.Value;
                    var xaTime = xElmMeasurement.Attribute("dateTimeUTC");
                    var lic = MeasurementsClass.Create(xElmMeasurement);
                    res.Add(lic);
                }
            }
            catch (Exception e)
            {
                LogError(e);
                if(dispatchErrorMessage)
                    DispatchMessageBox(string.Format("En uventet fejl opstod under indlæsningen af målinger. {0}", e.Message ?? ""));
            }

            return res;
        }
            


    }
}
