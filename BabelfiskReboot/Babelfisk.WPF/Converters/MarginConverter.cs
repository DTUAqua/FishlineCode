using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Anchor.Core;

namespace Babelfisk.WPF.Converters
{
    public class MarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var direction = "left";
                int multipler = 1;
                int padding = 0;

                if (parameter != null)
                {
                    var str = parameter.ToString();
                    var arr = str.Split(';');
                    if (arr.Length > 0)
                        direction = arr[0].ToLower();

                    if (arr.Length > 1)
                        int.TryParse(arr[1], out multipler);

                    if (arr.Length > 2)
                        int.TryParse(arr[2], out padding);
                }

                var val = value == null ? "" : value.ToString();

                double dblVal = 0;
                if (val != null)
                    val.TryParseDouble(out dblVal);


                switch (direction)
                {
                    default:
                    case "left":
                        return new Thickness((dblVal * multipler) + padding, 0, 0, 0);

                    case "top":
                        return new Thickness(0, (dblVal * multipler) + padding, 0, 0);

                    case "right":
                        return new Thickness(0, 0, (dblVal * multipler) + padding, 0);

                    case "bottom":
                        return new Thickness(0, 0, 0, (dblVal * multipler) + padding);
                }
            }
            catch { }

            return new Thickness(0, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
