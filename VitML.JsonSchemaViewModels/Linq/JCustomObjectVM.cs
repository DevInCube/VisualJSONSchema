using My.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VitML.JsonVM.Linq
{
    public class JCustomObjectVM : JObjectVM
    {

        private IJsonDataProvider provider;

        private JCustomObjectVM() { }

        public static JCustomObjectVM Create(JSchema schema)
        {
            var custom = new JCustomObjectVM();
            custom.SetSchema(schema);
            return custom;
        }

        public void Notify()
        {
            this.OnPropertyChanged("");
        }

        public void SetDataProvider(IJsonDataProvider provider)
        {
            this.provider = provider;
        }

        public override Newtonsoft.Json.Linq.JToken ToJToken()
        {
            return (provider != null) ? provider.Data : null;
        }
    }
}
