using MyVisualJSONEditor.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VitML.JsonSchemaControlBuilder.ViewModels
{
    public class PropertyStyle
    {

        public Double Height { get; set; }
        public Double MinHeight { get; set; }
        public Double MaxHeight { get; set; }

        public static PropertyStyle Parse(JToken data)
        {
            PropertyStyle st = new PropertyStyle();
            st.Height = GetDouble("Height", data);
            st.MinHeight = GetDouble("MinHeight", data);
            st.MaxHeight = GetDouble("MaxHeight", data);
            return st;
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
    }
}
