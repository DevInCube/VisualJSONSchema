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
using VitML.JsonSchemaControlBuilder;

namespace MyVisualJSONEditor.Views.Controls
{
    public class JObjectTypeTemplateSelector : DataTemplateSelector
    {

        public static JTemplateSelector TemplateSelector { get; set; }

        private static Dictionary<string, DataTemplate> Templates = new Dictionary<string, DataTemplate>();
       // private static ResourceDictionary dict = new ResourceDictionary();

        static JObjectTypeTemplateSelector()
        {
            Templates.Add("Root", CreateTemplate(typeof(RootTemplate)));
            Templates.Add("Button", CreateTemplate(typeof(ButtonTemplate)));
            Templates.Add("Object", CreateTemplate(typeof(ObjectTemplate)));
            Templates.Add("ObjectRequired", CreateTemplate(typeof(ObjectRequiredTemplate)));
            Templates.Add("ObjectSimple", CreateTemplate(typeof(ObjectSimpleTemplate)));
            Templates.Add("String", CreateTemplate(typeof(StringTemplate)));
            Templates.Add("Label", CreateTemplate(typeof(LabelTemplate)));
            Templates.Add("Password", CreateTemplate(typeof(PasswordTemplate)));
            Templates.Add("Boolean", CreateTemplate(typeof(BooleanTemplate)));
            Templates.Add("Date", CreateTemplate(typeof(DateTemplate)));
            Templates.Add("Time", CreateTemplate(typeof(TimeTemplate)));
            Templates.Add("DateTime", CreateTemplate(typeof(DateTimeTemplate)));
            Templates.Add("Array", CreateTemplate(typeof(ArrayTemplate)));
            Templates.Add("List", CreateTemplate(typeof(ArrayCollapsedTemplate)));
            Templates.Add("ArrayStatic", CreateTemplate(typeof(ArrayStaticTemplate)));
            Templates.Add("SelectList", CreateTemplate(typeof(SelectListTemplate)));
            Templates.Add("Integer", CreateTemplate(typeof(IntegerTemplate)));
            Templates.Add("Number", CreateTemplate(typeof(NumberTemplate)));
            Templates.Add("Enum", CreateTemplate(typeof(EnumTemplate)));
            Templates.Add("TabRoot", CreateTemplate(typeof(TabRootTemplate)));

           // dict.Source = new Uri("Views/Templates/TemplatePack.xaml",
              //  UriKind.RelativeOrAbsolute);
         //   var root = dict["Root"];
        }

        private static DataTemplate CreateTemplate(Type templateType)
        {
            var template = new DataTemplate();
            template.VisualTree = new FrameworkElementFactory(templateType);
            return template;
        }

        public DataTemplate GetTemplate(string id)
        {
            return Templates[id];// (DataTemplate)dict[id];
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;

            var presenter = (FrameworkElement)container;

           // if (TemplateSelector != null)
                //return TemplateSelector.SelectTemplate(item, container);

            if (item is JObjectVM)
            {
                if ((item as JObjectVM).Schema.Format == "tab")
                    return GetTemplate("TabRoot");
                else
                    return GetTemplate("Root");
            }

            JSchema schema = null;
            bool required = false;
            bool expanded = true;
            if (item is JTokenVM)
            {
                schema = ((JTokenVM)item).Schema;
            }

            if (item is JPropertyVM)
            {
                schema = ((JPropertyVM)item).Schema;
                required = ((JPropertyVM)item).IsRequired;
                expanded = ((JPropertyVM)item).IsExpanded;
            }

            var type = schema.Type;

            if (type.HasFlag(JSchemaType.String))
            {
                if (schema.Enum.Count > 0)
                    return Templates["Enum"];
                else
                    switch (schema.Format)
                    {
                        case ("button"):
                            return Templates["Button"];
                        case ("label"):
                            return GetTemplate("Label");
                        case ("time"):
                            return Templates["Time"];
                        case ("date"):
                            return Templates["Date"];
                        case ("date-time"):
                            return Templates["DateTime"];
                        case ("password"):
                            return Templates["Password"];
                        default:
                            return GetTemplate("String");
                    }
            }
            if (type == JSchemaType.Integer)
            {
                if (schema.Enum.Count > 0)
                    return Templates["Enum"];
                else
                    return GetTemplate("Integer");
            }
            if (type == JSchemaType.Float)
                return GetTemplate("Number");
            if (type == JSchemaType.Boolean)
                return GetTemplate("Boolean");
            if (type.HasFlag(JSchemaType.Object))
            {
                switch (schema.Format)
                {
                    case ("simple"):
                        return Templates["ObjectSimple"];
                }
                if (required)
                    return GetTemplate("ObjectRequired");
                else
                    return GetTemplate("Object");
            }
            if (type == JSchemaType.Array)
            {
                switch (schema.Format)
                {
                    case ("list"):
                        return GetTemplate("List");
                    case ("select"):
                        return GetTemplate("SelectList");
                    case ("static"):
                        return GetTemplate("ArrayStatic");
                    default:
                        return GetTemplate("Array");
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
