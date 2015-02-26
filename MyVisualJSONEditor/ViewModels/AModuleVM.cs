using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVisualJSONEditor.ViewModels
{
    public abstract class AModuleVM : ObservableObject
    {

        public abstract void Init(JObjectVM vm);
    }
}
