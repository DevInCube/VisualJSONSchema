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
    class RadarVM : AModuleVM
    {

        public RadarVM()
        {         
            this.Schema = Resources.Radar_schema;
            this.Data = Resources.Radar;
        }

        public override void Init(JObjectVM vm)
        {
           // throw new NotImplementedException();
        }
    }
}
