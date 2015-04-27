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
using VitML.JsonVM;

namespace MyVisualJSONEditor.Views.Controls
{
    public class ViewBuilderTemplateSelector : DataTemplateSelector
    {

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;

            FrameworkElement presenter = (FrameworkElement)container;       

            if (item is JTokenVM)
            {
                JTokenVM vm = item as JTokenVM;
                if(vm.IsRequired || vm.Schema.GetIgnore())
                    return (DataTemplate)presenter.Resources["Required"];
                else
                    return (DataTemplate)presenter.Resources["Unrequired"];                
            }
            else
            {
                return (DataTemplate)presenter.Resources["Error"];                
            }
        }
    }
}
