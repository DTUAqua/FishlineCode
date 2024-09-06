using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using FishLineMeasure.ViewModels;

namespace FishLineMeasure.WPF.Converters
{
    //[ValueConversion(typeof(ReadOnlyObservableCollection<ValidationError>), typeof(bool))]
    public class ValidationErrorsIsWarningConverter : MarkupExtension, IMultiValueConverter
    {
        
        public ValidationErrorsIsWarningConverter() { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new ValidationErrorsIsWarningConverter();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2)
                return false;

            ReadOnlyObservableCollection<ValidationError> errors = values[0] as ReadOnlyObservableCollection<ValidationError>;

            if (errors == null)
                return false;

            var strError = string.Join("\n", (from e in errors select e.ErrorContent as string).ToArray());

            return !string.IsNullOrWhiteSpace(strError) && strError.StartsWith(AViewModel.WarningPrefix, StringComparison.InvariantCultureIgnoreCase);
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
