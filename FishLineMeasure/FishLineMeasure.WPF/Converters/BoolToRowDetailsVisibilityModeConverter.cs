using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace FishLineMeasure.WPF.Converters
{
    public class BoolToRowDetailsVisibilityModeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter == null || value == null)
                return DataGridRowDetailsVisibilityMode.Collapsed;

            bool blnValue = false;
            bool blnParam = true;

            if (value == null)
                return DataGridRowDetailsVisibilityMode.Collapsed;

            bool.TryParse(value.ToString(), out blnValue);

            if (parameter != null)
                bool.TryParse(parameter.ToString(), out blnParam);

            if (blnValue == blnParam)
                return DataGridRowDetailsVisibilityMode.VisibleWhenSelected;

            return DataGridRowDetailsVisibilityMode.Collapsed;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
