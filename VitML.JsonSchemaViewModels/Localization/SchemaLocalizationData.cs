using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VitML.JsonVM.Localization
{
    public class SchemaLocalizationData
    {

        private Dictionary<string, Dictionary<string, string>> langData;

        private SchemaLocalizationData()
        {
            langData = new Dictionary<string, Dictionary<string, string>>();
        }

        public static SchemaLocalizationData Parse(string json)
        {
            JArray array = JArray.Parse(json);

            return new SchemaLocalizationData();
        }
    }
}
