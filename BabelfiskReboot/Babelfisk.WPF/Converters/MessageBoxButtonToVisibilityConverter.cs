using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Babelfisk.WPF.Converters
{
    public class MessageBoxButtonToVisibilityConverter : IValueConverter
    {

        public MessageBoxButtonToVisibilityConverter() { }



        #region IValueConverter Members


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter == null || value == null)
                return Visibility.Collapsed;

            MessageBoxButton msgValue;
            MessageBoxButton msgParam;

            if (!Enum.TryParse(value.ToString(), out msgValue))
                return Visibility.Collapsed;

            if (!Enum.TryParse(parameter.ToString(), out msgParam))
                return Visibility.Collapsed;

            return msgValue == msgParam ? Visibility.Visible : Visibility.Collapsed;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
