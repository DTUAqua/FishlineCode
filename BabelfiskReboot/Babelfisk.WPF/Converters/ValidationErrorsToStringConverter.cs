using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Anchor.Core;
using Babelfisk.ViewModels;

namespace Babelfisk.WPF.Converters
{
    [ValueConversion(typeof(ReadOnlyObservableCollection<ValidationError>), typeof(string))]
    public class ValidationErrorsToStringConverter : MarkupExtension, IValueConverter
    {
        public ValidationErrorsToStringConverter() { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new ValidationErrorsToStringConverter();
        }

        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            ReadOnlyObservableCollection<ValidationError> errors =
                value as ReadOnlyObservableCollection<ValidationError>;

            if (errors == null)
                return string.Empty;

            var strError = string.Join("\n", (from e in errors select e.ErrorContent as string).ToArray());

            if (strError != null)
                strError = strError.Replace(AViewModel.WarningPrefix, "");

            return strError;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
