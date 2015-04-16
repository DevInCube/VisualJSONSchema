using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VitML.JsonVM.Schema;

namespace VitML.JsonVM.Common
{
    public class PropertyStyle
    {
        #region properties

        public Double Height { get; set; }
        public Double MinHeight { get; set; }
        public Double MaxHeight { get; set; }

        public Double Width { get; set; }
        public Double MinWidth { get; set; }
        public Double MaxWidth { get; set; }

        public bool? ShowCount { get; set; }

        public string DisplayMemberPath { get; set; }

        #endregion properties

        public static PropertyStyle Parse(JToken data)
        {
            if (!(data is JObject)) throw new Exception("Style should be an object");

            JObject style = data as JObject;
            PropertyStyleReader reader = new PropertyStyleReader(style);
            return reader.Read();           
        }

        private static string GetValue(string key, JToken data)
        {
            JToken t = data.SelectToken(key);
            if (t == null) return null;
            return t.Value<string>();
        }

        private static GridLength GetLength(string key, JToken data)
        {
            string val = GetValue(key, data);
            if (val == null) return GridLength.Auto;
            return (GridLength)new GridLengthConverter().ConvertFromString(val);
        }

        private static Double GetDouble(string key, JToken data)
        {
            string val = GetValue(key, data);
            if (val == null) return Double.NaN;
            return Double.Parse((new LengthConverter()).ConvertFromString(val).ToString());
        }

        private static bool? GetBool(string key, JToken data)
        {
            string val = GetValue(key, data);
            if (val == null) return null;
            return bool.Parse(val);
        }
    }    
}
