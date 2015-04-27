using My.Json.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VitML.JsonVM.Linq
{
    public class JCustomVM : JTokenVM
    {

        private IJsonDataProvider provider;

        public JToken JsonData { get; private set; }

        private JCustomVM() { }

        public static JCustomVM Create(JSchema schema, JToken data)
        {
            var custom = new JCustomVM();
            custom.SetSchema(schema);
            custom.SetData(data);
            return custom;
        }

        public override void SetData(JToken data)
        {
            this.JsonData = data;
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
            if (provider != null)
                return provider.Data;
            else if (JsonData != null)
                return JsonData;
            else
                return Schema.GenerateData();            
        }

    }
}
