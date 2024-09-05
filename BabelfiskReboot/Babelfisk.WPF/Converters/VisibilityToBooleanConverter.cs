using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Babelfisk.WPF.Converters
{
    public class VisibilityToBooleanConverter : IValueConverter
    {
        public VisibilityToBooleanConverter() { }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            Visibility enmVisibilityValue, enmVisibilityParam;

            if (!Enum.TryParse(value.ToString(), out enmVisibilityValue))
            {
                return false;
            }

            if (!Enum.TryParse(parameter.ToString(), out enmVisibilityParam))
            {
                return false;
            }

            return enmVisibilityValue == enmVisibilityParam;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
