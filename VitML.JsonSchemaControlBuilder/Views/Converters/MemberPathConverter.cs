using MyVisualJSONEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VitML.JsonSchemaControlBuilder.Views.Converters
{
    public class MemberPathConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //string path = (values[0] as JObjectVM).d;
            object title = (values[0] as JObjectVM).DisplayMemberPath;
            return (title == null) ? "<null>" : title.ToString();
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
