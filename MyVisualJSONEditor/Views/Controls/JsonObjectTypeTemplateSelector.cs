using MyVisualJSONEditor.ViewModels;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyVisualJSONEditor.Views.Controls
{
    public class JsonObjectTypeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var presenter = (FrameworkElement)container;

            JSchema schema = null;
            if (item is JTokenVM)
                schema = ((JTokenVM)item).Schema;

            if (item is JPropertyVM)
                schema = ((JPropertyVM)item).Schema;

            var type = schema.Type;

            if (type == JSchemaType.String && schema.Format == "date-time")
                return (DataTemplate)presenter.Resources["DateTimeTemplate"];
            if (type == JSchemaType.String && schema.Format == "date")
                return (DataTemplate)presenter.Resources["DateTemplate"];
            if (type == JSchemaType.String && schema.Format == "time")
                return (DataTemplate)presenter.Resources["TimeTemplate"];
            if (type == JSchemaType.String && schema.Enum.Count > 0)
                return (DataTemplate)presenter.Resources["EnumTemplate"];
            if (type == JSchemaType.String)
                return (DataTemplate)presenter.Resources["StringTemplate"];

            if (type == JSchemaType.Integer && schema.Enum.Count > 0)
                return (DataTemplate)presenter.Resources["EnumTemplate"];
            if (type == JSchemaType.Integer)
                return (DataTemplate)presenter.Resources["IntegerTemplate"];

            if (type == JSchemaType.Float)
                return (DataTemplate)presenter.Resources["NumberTemplate"];
            if (type == JSchemaType.Boolean)
                return (DataTemplate)presenter.Resources["BooleanTemplate"];
            if (type == JSchemaType.Object)
                return (DataTemplate)presenter.Resources["ObjectTemplate"];
            if (type == JSchemaType.Array)
            {
                if (schema.Format == "select")
                    return (DataTemplate)presenter.Resources["SelectTemplate"];
                return (DataTemplate)presenter.Resources["ArrayTemplate"];
            }

            return base.SelectTemplate(item, container);
        }
    }
}
