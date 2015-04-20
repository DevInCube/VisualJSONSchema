using MyToolkit.Command;
using MyVisualJSONEditor.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VitML.JsonVM.Linq;
using Xceed.Wpf.Toolkit;

namespace MyVisualJSONEditor.ViewModels
{
    public class CompositorVM : AModuleVM
    {

        private JObjectVM vm;

        public CompositorVM()
        {
            this.Schema = Resources.Compositor_schema;
            this.Data = Resources.Compositor;
        }

        public override void Init(JObjectVM vm)
        {
            this.vm = vm;

            JArrayVM sources = vm.GetToken("Source.QueueSet") as JArrayVM;

            var button = (vm.GetToken("Source.SetMaster") as JValueVM);
            button.Command = new RelayCommand(() =>
            {
                JObjectVM selected = (sources.SelectedItem as JObjectVM);
                JValue name = null;
                if (selected != null)
                {
                    name = selected.Data["Name"] as JValue;
                }
                vm.Data["Source.MasterName"] = name;
            });
        }

    }
}
