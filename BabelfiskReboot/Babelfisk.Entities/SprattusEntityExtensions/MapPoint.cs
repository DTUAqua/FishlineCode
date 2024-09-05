using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Babelfisk.Entities.Sprattus
{
    [DataContract]
    public class MapPoint
    {
        private string _strStationName;
        private string _strTripName;
        private int _intId;
        private string _strLatStart;
        private string _strLonStart;
        private string _strLatStop;
        private string _strLonStop;

        [DataMember]
        public int Id
        {
            get { return _intId; }
            set { _intId = value; }
        }

        [DataMember]
        public string StationName
        {
            get { return _strStationName; }
            set { _strStationName = value; }
        }

        [DataMember]
        public string TripName
        {
            get { return _strTripName; }
            set { _strTripName = value; }
        }

        [DataMember]
        public string LatitudeStart
        {
            get { return _strLatStart; }
            set { _strLatStart = value; }
        }

        [DataMember]
        public string LongitudeStart
        {
            get { return _strLonStart; }
            set { _strLonStart = value; }
        }


        [DataMember]
        public string LatitudeStop
        {
            get { return _strLatStop; }
            set { _strLatStop = value; }
        }

        [DataMember]
        public string LongitudeStop
        {
            get { return _strLonStop; }
            set { _strLonStop = value; }
        }

        public string LatitudeStartDegreeMinutes
        {
            get { return ConvertStringToDegreeMinutes(LatitudeStart); }
        }

        public string LongitudeStartDegreeMinutes
        {
            get { return ConvertStringToDegreeMinutes(LongitudeStart); }
        }

        public string LatitudeStopDegreeMinutes
        {
            get { return ConvertStringToDegreeMinutes(LatitudeStop); }
        }

        public string LongitudeStopDegreeMinutes
        {
            get { return ConvertStringToDegreeMinutes(LongitudeStop); }
        }


        private string ConvertStringToDegreeMinutes(string pos)
        {
            if (pos == null)
                return null;

            try
            {
                StringBuilder sb = new StringBuilder(pos);

                var i = pos.IndexOf(".");

                if (i > 0)
                {
                    sb[i] = '°'; // index starts at 0!
                    sb.Insert(i+1, ' ');

                    if (sb.Length > 3)
                        sb[sb.Length - 3] = '\'';
                    return sb.ToString();
                }
            }
            catch { }

            return null;
        }

        public MapPoint()
        {
        }

        public MapPoint(string strStationName, string strTripName, int intId, string strLatStart, string strLonStart, string strLatStop, string strLonStop)
        {
            _strStationName = strStationName;
            _strTripName = strTripName;
            _intId = intId;
            _strLatStart = strLatStart;
            _strLonStart = strLonStart;
            _strLatStop = strLatStop;
            _strLonStop = strLonStop;
        }
    }
}
