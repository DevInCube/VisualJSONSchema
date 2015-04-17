﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            JObject obj = JObject.Parse(json);

            var data = new SchemaLocalizationData();
            foreach (var prop in obj)
            {
                var langDict = new Dictionary<string, string>();
                data.langData[prop.Key] = langDict;
                JObject lang = prop.Value as JObject;
                foreach (var pair in lang)
                {
                    langDict[pair.Key] = (pair.Value as JValue).Value<string>();
                }            
            }
            return data;
        }

        public string GetString(string key, CultureInfo info)
        {
            if (langData.ContainsKey(info.Name))
            {
                if (langData[info.Name].ContainsKey(key))
                {
                    return langData[info.Name][key];
                }
            }
            return null;
        }
    }
}
