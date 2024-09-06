using FishLineMeasure.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishLineMeasure.ViewModels
{
    public static class ExtensionMethods
    {
        public static double ConvertToUnit(this double dblValue, Unit fromUnit, Unit toUnit)
        {
            switch (fromUnit)
            {
                case Unit.MM:
                    if (toUnit == Unit.MM)
                        return dblValue;
                    else if (toUnit == Unit.CM)
                        return dblValue / 10;
                    else if (toUnit == Unit.SC)
                        return dblValue / 5;
                    break;

                case Unit.CM:
                    if (toUnit == Unit.MM)
                        return dblValue * 10;
                    else if (toUnit == Unit.CM)
                        return dblValue;
                    else if (toUnit == Unit.SC)
                        return dblValue * 2;
                    break;

                case Unit.SC:
                    if (toUnit == Unit.MM)
                        return dblValue * 5;
                    else if (toUnit == Unit.CM)
                        return dblValue / 2;
                    else if (toUnit == Unit.SC)
                        return dblValue;
                    break;
            }

            throw new ApplicationException("Length measuring unit is not supported.");
        }

    }
}
