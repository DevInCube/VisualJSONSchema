using MyVisualJSONEditor.ViewModels;
using MyVisualJSONEditor.Tools;
using MyVisualJSONEditor.Views.Templates;
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
            if (item == null) return null;

            var presenter = (FrameworkElement)container;

            if (item is JObjectVM)
                return (DataTemplate)presenter.Resources["RootTemplate"];

            JSchema schema = null;
            if (item is JTokenVM)
                schema = ((JTokenVM)item).Schema;

            if (item is JPropertyVM)
                schema = ((JPropertyVM)item).Schema;

            var type = schema.Type;
            Type templateType = null;

            if (type == JSchemaType.String)
            {
                if (schema.Enum.Count > 0)
                    templateType = typeof(EnumTemplate);
                else
                    switch (schema.Format)
                    {
                        case ("time"):
                            templateType = typeof(TimeTemplate);
                            break;
                        case ("date"):
                            templateType = typeof(DateTemplate);
                            break;
                        case ("date-time"):
                            templateType = typeof(DateTimeTemplate);
                            break;
                        default:
                            templateType = typeof(StringTemplate);
                            break;
                    }
            }
            if (type == JSchemaType.Integer)
            {
                if (schema.Enum.Count > 0)
                    templateType = typeof(EnumTemplate);
                else
                    templateType = typeof(IntegerTemplate);
            }
            if (type == JSchemaType.Float)
                templateType = typeof(NumberTemplate);
            if (type == JSchemaType.Boolean)
                templateType = typeof(BooleanTemplate);
            if (type.HasFlag(JSchemaType.Object)) 
                templateType = typeof(ObjectTemplate);
            if (type == JSchemaType.Array)
            {
                if (schema.Format == "select")
                    templateType = typeof(SelectListTemplate);
                else
                    templateType = typeof(ArrayTemplate);
            }

            if (templateType != null)
            {
                var template = new DataTemplate();
                template.VisualTree = new FrameworkElementFactory(templateType);
                return template;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
