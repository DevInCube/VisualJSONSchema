using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVisualJSONEditor.ViewModels
{
    public abstract class AModuleVM : ObservableObject
    {

        public string Schema { get; set; }
        public string Data { get; set; }

        public abstract void Init(JObjectVM vm);

        public override string ToString()
        {
            return this.GetType().Name.ToString();
        }
    }
}
