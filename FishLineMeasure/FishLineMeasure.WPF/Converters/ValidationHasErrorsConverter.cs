using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace FishLineMeasure.WPF.Converters
{
    public class ValidationHasErrorsConverter: MarkupExtension, IMultiValueConverter
    {

        public ValidationHasErrorsConverter() { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new ValidationHasErrorsConverter();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2)
                return false;

            ReadOnlyObservableCollection<ValidationError> errors = values[0] as ReadOnlyObservableCollection<ValidationError>;

            if (errors == null)
                return false;

            var strError = string.Join("\n", (from e in errors select e.ErrorContent as string).ToArray());

            return !string.IsNullOrWhiteSpace(strError);
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
