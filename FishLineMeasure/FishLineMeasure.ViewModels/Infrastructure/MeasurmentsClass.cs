using System;
using System.Globalization;
using System.Xml.Linq;
using Anchor.Core;
using FishLineMeasure.BusinessLogic;

namespace FishLineMeasure.ViewModels.Infrastructure
{
    public class MeasurementsClass : AViewModel
    {
        private double _length;

        private Unit _unit;


        #region Properties


        public DateTime DateTimeUTC { get; set; }


        public DateTime DateTimeLocal
        {
            get { return DateTimeUTC.ToLocalTime(); }
        }


        public double Length
        {
            get { return _length; }
            set
            {
                _length = value;
                RaisePropertyChanged(nameof(Length));
            }
        }


        public Unit Unit
        {
            get { return _unit; }
            set
            {
                _unit = value;
                RaisePropertyChanged(() => Unit);
            }
        }


        public string LengthString
        {
            get 
            {
                var du = BusinessLogic.Settings.Settings.Instance.DefaultDisplayUnit;
                return string.Format("{0} {1}", _length.ConvertToUnit(Unit, du).TruncateToString(du == Unit.MM ? BusinessLogic.Settings.Settings.Instance.LengthMMDecimals : BusinessLogic.Settings.Settings.Instance.LengthCMDecimals), du.ToString().ToLower());
                //   return string.Format("{0} {1}", _length.ConvertToUnit(Unit, du).DoubleToString(du == Unit.MM ? BusinessLogic.Settings.Settings.Instance.LengthMMDecimals : BusinessLogic.Settings.Settings.Instance.LengthCMDecimals), du.ToString().ToLower()); 
            }
        }


        public OrderClass Lookups { get; set; }


        #endregion


        public static MeasurementsClass Create(XElement xElm)
        {
            var res = new MeasurementsClass();

            var xaTime = xElm.Attribute("dateTimeUTC");

            if (xaTime != null && !string.IsNullOrWhiteSpace(xaTime.Value))
                res.DateTimeUTC = DateTime.ParseExact(xaTime.Value, "dd-MM-yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal);
            var xeLength = xElm.Element("Length");

            double l = -1;
            if (xeLength != null && !string.IsNullOrWhiteSpace(xeLength.Value) && xeLength.Value.TryParseDouble(out l))
                res.Length = l;

            if(xeLength != null)
            {
                var xaUnit = xeLength.Attribute("unit");

                Unit u = Unit.CM; //Assume CM if nothing is defined, since that is what it started with.
                if (xaUnit != null)
                    Enum.TryParse<Unit>(xaUnit.Value, out u);

                res.Unit = u;
            }

            var xElmLookups = xElm.Element("Lookups");

            if(xElmLookups != null)
               res.Lookups = OrderClass.Create(xElmLookups);

            return res;
        }


        public static MeasurementsClass Create(DateTime timeUTC, double length, OrderClass lookups, Unit unit)
        {
            var res = new MeasurementsClass();
            res.DateTimeUTC = timeUTC;
            res.Length = length;
            res.Lookups = lookups.Clone();
            res.Unit = unit;

            return res;
        }


        public XElement ToXElement()
        {
            //Always store in MM, so convert to MM if Unit is of different type.
            var xElm = new XElement("Measurement", new XAttribute("dateTimeUTC", DateTimeUTC.ToString("dd-MM-yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture)),
                           new XElement("Length", Length.ConvertToUnit(Unit, Unit.MM).ToString(CultureInfo.InvariantCulture), new XAttribute("unit", Unit.MM.ToString())),
                           Lookups.ToXElement());

            return xElm;
        }
    }
}
