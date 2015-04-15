using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MyVisualJSONEditor.Views.Converters
{
    public class IntegerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as JValue).Value<long>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long integer;
            try
            {
                integer = long.Parse(value.ToString());
            }
            catch
            {
                integer = default(long);
            }
            return new JValue(integer);
        }
    }
}
