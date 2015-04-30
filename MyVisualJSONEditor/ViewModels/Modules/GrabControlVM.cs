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
    public class GrabControlVM : AModuleVM
    {

        public GrabControlVM()
        {
            this.Schema = Resources.GrabControl_schema;
            this.Data = Resources.GrabControl;
        }

        public override void Init(JObjectVM vm)
        {
            //
        }

    }
}
