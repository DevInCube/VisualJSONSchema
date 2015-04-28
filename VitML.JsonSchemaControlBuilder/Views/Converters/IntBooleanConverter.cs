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
    public class IntBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int integer = (value as JValue).Value<int>();
            if (integer < 0 || integer > 1)
                integer = 0;
            return (integer == 1) ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolean = bool.Parse(value.ToString());
            int integer = boolean ? 1 : 0;
            return new JValue(integer);
        }
    }
}
