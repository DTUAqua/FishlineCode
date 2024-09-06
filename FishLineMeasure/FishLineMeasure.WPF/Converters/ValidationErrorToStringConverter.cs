using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using FishLineMeasure.ViewModels;

namespace FishLineMeasure.WPF.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class ValidationErrorToStringConverter: MarkupExtension, IValueConverter
    {
        public ValidationErrorToStringConverter() { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new ValidationErrorToStringConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var error = value as string;

            if (error == null)
                return string.Empty;

            var strError = error.Replace(AViewModel.WarningPrefix, "");

            return strError;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
