using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FishLineMeasure.WPF.Converters
{
    public class IntToCodeFromDictionaryConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length < 2 || values[0] == null || values[1] == null)
                return "";

            int? index = values[0] as int?;
            Dictionary<int, string> dic = values[1] as Dictionary<int, string>;

            if (!index.HasValue)
                return "";

            if (dic == null)
                return values[0];

            if (dic.ContainsKey(index.Value))
                return dic[index.Value];
            else
                return index.Value;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
            // return new object[] { parameter };
        }
    }
}
