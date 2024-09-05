using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Babelfisk.WPF.Converters
{
    public class BoolToErrorWarningImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var uri = (bool)value
           ? "pack://application:,,,/ImgSrcIfTrue.png"
           : "pack://application:,,,/ImgSrcIfFalse.png";
            return new BitmapImage(new Uri(uri));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
