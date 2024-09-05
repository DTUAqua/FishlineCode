using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Babelfisk.WPF.Converters
{
    public class DataGridColumnMapToVisibilityConverter : IValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var res = Visibility.Visible;

            //Default value is to show columns
            if (value == null || parameter == null)
                return res;

            string tripType = value as string;

            string strParam = parameter.ToString();

            var arr = strParam.Split('|');

            if (arr == null || arr.Length != 2)
                return res;

            string strGroup = arr[0];
            string strColumn = arr[1];

            if (string.IsNullOrWhiteSpace(tripType))
                return res;

            res = BusinessLogic.Settings.Settings.Instance.DataGridColumnSettings.GetVisibility(tripType, strGroup, strColumn) ? Visibility.Visible : Visibility.Collapsed;

            return res;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
