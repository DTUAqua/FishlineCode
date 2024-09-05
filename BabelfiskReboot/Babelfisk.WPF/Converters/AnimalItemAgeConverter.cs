using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Babelfisk.ViewModels.Input;
using System.Windows.Controls;

namespace Babelfisk.WPF.Converters
{
    public class AnimalItemAgeConverter : IValueConverter
    {
        private AnimalItem _aItem;

        public AnimalItemAgeConverter() { }


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            _aItem = value as AnimalItem;
            string sAge = parameter as string;
            TextBox tb = null;
            if (sAge == null && (tb = parameter as TextBox) != null)
                sAge = tb.Tag as string;

            if (_aItem == null || sAge == null)
                return null;

            int age = 0;
            int? intNumber = null;

            if (Int32.TryParse(sAge, out age))
                intNumber = _aItem.GetAgeNumber(age);

            return intNumber.HasValue ? intNumber.Value.ToString() : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string sValue = value as string;
            string sAge = parameter as string;

            if (sValue != null && sAge != null)
            {
                if (_aItem != null)
                {
                    int intAge = 0;
                    int intValue = 0;

                    if (Int32.TryParse(sAge, out intAge))
                    {
                        if (Int32.TryParse(sValue, out intValue))
                            _aItem.SetAgeNumber(intAge, intValue);
                        else
                            _aItem.SetAgeNumber(intAge, null);
                    }
                }
            }

            return _aItem;
        }
    }
}
