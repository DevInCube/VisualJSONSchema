using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using VitML.JsonVM.Schema;

namespace VitML.JsonVM.Common
{
    public class PropertyStyleReader
    {

        private JObject data;

        public PropertyStyleReader(JObject styleData)
        {
            if (styleData == null) throw new ArgumentNullException("style");

            this.data = styleData;
        }

        public PropertyStyle Read()
        {
            PropertyStyle style = new PropertyStyle();
            foreach (var prop in data.Properties())
            {
                var value = prop.Value;
                switch (prop.Name)
                {
                    case (JSchemaExtendedKeywords.Style.Height):
                        style.Height = ReadDouble(value);
                        break;
                    case (JSchemaExtendedKeywords.Style.MinHeight):
                        style.MinHeight = ReadDouble(value);
                        break;
                    case (JSchemaExtendedKeywords.Style.MaxHeight):
                        style.MaxHeight = ReadDouble(value);
                        break;
                    case (JSchemaExtendedKeywords.Style.Width):
                        style.Width = ReadDouble(value);
                        break;
                    case (JSchemaExtendedKeywords.Style.MinWidth):
                        style.MinWidth = ReadDouble(value);
                        break;
                    case (JSchemaExtendedKeywords.Style.MaxWidth):
                        style.MaxWidth = ReadDouble(value);
                        break;

                    case (JSchemaExtendedKeywords.Style.ShowCount):
                        style.ShowCount = ReadBool(value);
                        break;
                    case (JSchemaExtendedKeywords.Style.DisplayMemberPath):
                        style.DisplayMemberPath = ReadString(value);
                        break;
                }
            }
            return style;
        }

        private string ReadString(JToken value)
        {
            if (!(value.Type == JTokenType.String))
                throw new Exception("String expected");

            return value.Value<string>();
        }

        private Double ReadDouble(JToken value)
        {
            if (!(value.Type == JTokenType.Float
                || value.Type == JTokenType.Integer))
                throw new Exception("Number expected");

            return value.Value<double>();
        }

        private bool? ReadBool(JToken value)
        {
            if (!(value.Type == JTokenType.Boolean))
                throw new Exception("Boolean expected");

            return value.Value<bool>();
        }

        private GridLength ReadLength(JToken value)
        {
            if (!(value.Type == JTokenType.Float
                || value.Type == JTokenType.Integer
                || value.Type == JTokenType.String))
                throw new Exception("Number or string expected");

            string length = value.ToString();
            return (GridLength)new GridLengthConverter().ConvertFromString(length);
        }
    }
}
