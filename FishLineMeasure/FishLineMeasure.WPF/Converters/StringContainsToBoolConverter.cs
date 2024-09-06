using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Anchor.Core;

namespace FishLineMeasure.WPF.Converters
{
    public class StringContainsToBoolConverter : IValueConverter
    {
        public StringContainsToBoolConverter() { }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strVal = value as string;
            string strParams = parameter as string;

            if (strVal == null || strParams == null)
                return false;

            return strParams.Contains(strVal, StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
