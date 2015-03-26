using MyVisualJSONEditor.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VitML.JsonVM.Linq;

namespace MyVisualJSONEditor.ViewModels
{
    class MediaGrabberVM : AModuleVM
    {

        public MediaGrabberVM()
        {
            this.Schema = Resources.MediaGrabber_schema;
            this.Data = Resources.MediaGrabber;
        }

        public override void Init(JObjectVM vm)
        {
            //throw new NotImplementedException();
        }
    }
}
