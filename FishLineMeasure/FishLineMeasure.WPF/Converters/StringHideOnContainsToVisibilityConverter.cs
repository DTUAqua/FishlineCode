using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Anchor.Core;

namespace FishLineMeasure.WPF.Converters
{
    public class StringHideOnContainsToVisibilityConverter : IValueConverter
    {
        public StringHideOnContainsToVisibilityConverter() { }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strVal = value as string;
            string strParams = parameter as string;

            if (strVal == null || strParams == null)
                return Visibility.Visible;

            var v = strParams.Contains(strVal, StringComparison.InvariantCultureIgnoreCase) ? Visibility.Collapsed : Visibility.Visible;
            return v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
