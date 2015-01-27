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

        private Dictionary<string, DataTemplate> Templates = new Dictionary<string, DataTemplate>();

        public JsonObjectTypeTemplateSelector()
        {
            Templates.Add("Object", CreateTemplate(typeof(ObjectTemplate)));
            Templates.Add("String", CreateTemplate(typeof(StringTemplate)));
            Templates.Add("Password", CreateTemplate(typeof(PasswordTemplate)));
            Templates.Add("Boolean", CreateTemplate(typeof(BooleanTemplate)));
            Templates.Add("Date", CreateTemplate(typeof(DateTemplate)));
            Templates.Add("Time", CreateTemplate(typeof(TimeTemplate)));
            Templates.Add("DateTime", CreateTemplate(typeof(DateTimeTemplate)));
            Templates.Add("Array", CreateTemplate(typeof(ArrayTemplate)));
            Templates.Add("SelectList", CreateTemplate(typeof(SelectListTemplate)));
            Templates.Add("Integer", CreateTemplate(typeof(IntegerTemplate)));
            Templates.Add("Number", CreateTemplate(typeof(NumberTemplate)));
            Templates.Add("Enum", CreateTemplate(typeof(EnumTemplate)));
        }

        protected DataTemplate CreateTemplate(Type templateType)
        {
            var template = new DataTemplate();
            template.VisualTree = new FrameworkElementFactory(templateType);
            return template;
        }

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

            if (type == JSchemaType.String)
            {
                if (schema.Enum.Count > 0)
                    return Templates["Enum"];
                else
                    switch (schema.Format)
                    {
                        case ("time"):
                            return Templates["Time"];
                        case ("date"):
                            return Templates["Date"];
                        case ("date-time"):
                            return Templates["DateTime"];
                        case ("password"):
                            return Templates["Password"];
                        default:
                            return Templates["String"];
                    }
            }
            if (type == JSchemaType.Integer)
            {
                if (schema.Enum.Count > 0)
                    return Templates["Enum"];
                else
                    return Templates["Integer"];
            }
            if (type == JSchemaType.Float)
                return Templates["Number"];
            if (type == JSchemaType.Boolean)
                return Templates["Boolean"];
            if (type.HasFlag(JSchemaType.Object))
                return Templates["Object"];
            if (type == JSchemaType.Array)
            {
                if (schema.Format == "select")
                    return Templates["SelectList"];
                else
                    return Templates["Array"];
            }

            return base.SelectTemplate(item, container);
        }
    }
}
