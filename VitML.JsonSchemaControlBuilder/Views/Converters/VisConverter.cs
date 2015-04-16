using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MyVisualJSONEditor.Views.Converters
{
    public class VisConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool res = true;
            if (value == null)
                res = false;
            else
            {
                if (value is bool)
                    res = bool.Parse(value.ToString());
                else if (value is IList)
                    res = (value as IList).Count > 0;
            }
            bool invert = false;
            if (parameter != null)
                invert = bool.Parse(parameter.ToString());
            if (invert)
                res = !res;
            return (res) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
