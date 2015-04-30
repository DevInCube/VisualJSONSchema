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
    class OverlayServerVM : AModuleVM
    {

        public OverlayServerVM()
        {         
            this.Schema = Resources.OverlayServer_schema;
            this.Data = Resources.OverlayServer;
        }

        public override void Init(JObjectVM vm)
        {
           // throw new NotImplementedException();
        }
    }
}
