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

namespace MyVisualJSONEditor.Views.Controls
{
    public class JBuilderTemplateSelector : DataTemplateSelector
    {

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;

            var presenter = (FrameworkElement)container;

           // if (TemplateSelector != null)
                //return TemplateSelector.SelectTemplate(item, container);

            if (item is JObjectVM)
            {
                if ((item as JObjectVM).Schema.Format == "tab")
                    return (DataTemplate)presenter.Resources["TabRoot"];
                else
                    return (DataTemplate)presenter.Resources["Root"];
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

            if ((item is JValue && (item as JValue).Value == null) || schema == null)
                return (DataTemplate)presenter.Resources["Null"];

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
                    case ("simple"):
                        return (DataTemplate)presenter.Resources["ObjectSimple"];
                    case ("alt"):
                        return (DataTemplate)presenter.Resources["ObjectAlt"];
                }
                if (required)
                    return (DataTemplate)presenter.Resources["ObjectRequired"];
                else
                    return (DataTemplate)presenter.Resources["Object"];
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
