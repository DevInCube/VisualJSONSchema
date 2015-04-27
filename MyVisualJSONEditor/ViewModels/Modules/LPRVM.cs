using MyVisualJSONEditor.Properties;
using MyVisualJSONEditor.Views;
using MyVisualJSONEditor.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VitML.JsonVM.Linq;

namespace MyVisualJSONEditor.ViewModels
{
    class LPRVM : AModuleVM
    {

        public LPRVM()
        {
            CustomTemplates templates = new CustomTemplates();
            DataTemplate testTemplate = (DataTemplate)templates.Resources["test"];
            TokenBuilderTemplateSelector.RegisterCustomTemplate("test", testTemplate);

            this.Schema = Resources.LPR_schema;
            this.Data = Resources.LPR;
        }

        public override void Init(JObjectVM vm)
        {
           // throw new NotImplementedException();
        }
    }
}
