using MyVisualJSONEditor.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVisualJSONEditor.ViewModels
{
    class LPRVM : AModuleVM
    {

        public LPRVM()
        {
            this.Schema = Resources.LPR_schema;
            this.Data = Resources.LPR;
        }

        public override void Init(JObjectVM vm)
        {
           // throw new NotImplementedException();
        }
    }
}
