using MyVisualJSONEditor.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVisualJSONEditor.ViewModels.Modules
{
    public class TestVM : AModuleVM
    {

        public TestVM()
        {
            this.Schema = Resources.TestSchema;
            this.Data = Resources.TestData;
        }
        
        public override void Init(VitML.JsonVM.Linq.JObjectVM vm)
        {
            //
        }
    }
}
