using Newtonsoft.Json.Linq;
using My.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VitML.JsonSchemaControlBuilder;
using VitML.JsonVM.Linq;
using VitML.JsonVM.Schema;

namespace MyVisualJSONEditor.Views.Controls
{
    public class JBuilderTemplateSelector : DataTemplateSelector
    {

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;

            FrameworkElement presenter = (FrameworkElement)container;

            JSchema schema = null;
            bool required = false;         

            if (item is JTokenVM)
            {
                JTokenVM vm = item as JTokenVM;
                if(vm.Data == null)
                    return (DataTemplate)presenter.Resources["Null"];
                schema = vm.Schema;
                if (schema == null)
                    throw new Exception("schema is missing");
                //required = schema.GetRequired();
            }
            else
            {
                return (DataTemplate)presenter.Resources["Error"];
                //throw new Exception("vm required");
            }

            var type = schema.Type;

            if (type.HasFlag(JSchemaType.String))
            {
                if (schema.Enum.Count > 0)
                    return (DataTemplate)presenter.Resources["Enum"];
                else
                    switch (schema.Format)
                    {
                        case ("button"):
                            return (DataTemplate)presenter.Resources["Button"];
                        case ("label"):
                            return (DataTemplate)presenter.Resources["Label"];
                        case ("time"):
                            return (DataTemplate)presenter.Resources["Time"];
                        case ("date"):
                            return (DataTemplate)presenter.Resources["Date"];
                        case ("date-time"):
                            return (DataTemplate)presenter.Resources["DateTime"];
                        case ("password"):
                            return (DataTemplate)presenter.Resources["Password"];
                        default:
                            return (DataTemplate)presenter.Resources["String"];
                    }
            }
            if (type == JSchemaType.Integer)
            {
                if (schema.Enum.Count > 0)
                    return (DataTemplate)presenter.Resources["Enum"];
                else
                    return (DataTemplate)presenter.Resources["Integer"];
            }
            if (type == JSchemaType.Number)
                return (DataTemplate)presenter.Resources["Number"];
            if (type == JSchemaType.Boolean)
                return (DataTemplate)presenter.Resources["Boolean"];
            if (type.HasFlag(JSchemaType.Object))
            {
                switch (schema.Format)
                {
                    case ("tab"):
                        return (DataTemplate)presenter.Resources["TabRoot"];
                    //return (DataTemplate)presenter.Resources["Root"]; //@todo
                    case ("simple"):
                        return (DataTemplate)presenter.Resources["ObjectSimple"];
                    case ("alt"):
                        return (DataTemplate)presenter.Resources["ObjectAlt"];
                }
                if (required)
                    return (DataTemplate)presenter.Resources["ObjectRequired"];
                else
                    return (DataTemplate)presenter.Resources["ObjectRequired"]; //@todo
            }
            if (type == JSchemaType.Array)
            {
                switch (schema.Format)
                {
                    case ("list"):
                        return (DataTemplate)presenter.Resources["List"];
                    case ("select"):
                        return (DataTemplate)presenter.Resources["SelectList"];
                    case ("static"):
                        return (DataTemplate)presenter.Resources["ArrayStatic"];
                    default:
                        return (DataTemplate)presenter.Resources["Array"];
                }
            }            

            return base.SelectTemplate(item, container);
        }
    }
}
