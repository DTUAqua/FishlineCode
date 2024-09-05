using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Anchor.Core;

namespace Babelfisk.WPF.Converters
{
    public class StringShowOnContainsToVisibilityConverter : IValueConverter
    {
        public StringShowOnContainsToVisibilityConverter() { }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strVal = value as string;
            string strParams = parameter as string;

            if (strVal == null || strParams == null)
                return Visibility.Collapsed;

            var v = strParams.Contains(strVal, StringComparison.InvariantCultureIgnoreCase) ? Visibility.Visible : Visibility.Collapsed;
            return v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
