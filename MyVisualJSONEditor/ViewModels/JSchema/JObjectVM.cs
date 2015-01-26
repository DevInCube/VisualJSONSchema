﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyVisualJSONEditor.ViewModels
{
    /// <summary>Represents a JSON object. </summary>
    public class JObjectVM : JTokenVM
    {
        public static JObjectVM FromSchema(JSchema schema)
        {
            var obj = new JObjectVM();
            foreach (var property in schema.Properties)
            {
                if (property.Value.Type == JSchemaType.Object)
                {
                    obj[property.Key] = FromSchema(property.Value);
                }
                else if (property.Value.Type == JSchemaType.Array)
                {
                    obj[property.Key] = new ObservableCollection<JTokenVM>();
                }
                else
                    obj[property.Key] = GetDefaultValue(property);
            }
            obj.Schema = schema;
            return obj;
        }

        private static object GetDefaultValue(KeyValuePair<string, JSchema> property)
        {
            JSchema sh = property.Value;
            if (sh.Default != null)
                return ((JValue)sh.Default).Value;
            switch (sh.Type)
            {
                case (JSchemaType.Boolean):
                    return false;
                case (JSchemaType.Float):
                    return 0F;
                case (JSchemaType.Integer):
                    return 0;
                case (JSchemaType.String):
                    if (sh.Format == "date-time") return new DateTime();
                    if (sh.Format == "date") return new DateTime();
                    if (sh.Format == "time") return new TimeSpan();
                    if (sh.Format == "ipv4") return new JValue(IPAddress.None); //@todo @test
                    return String.Empty;
                case (JSchemaType.Null):
                    return null;
                default:
                    return null;
            }
        }

        /// <summary>Gets the object's properties. </summary>
        public IEnumerable<JPropertyVM> Properties
        {
            get
            {
                var properties = new List<JPropertyVM>();
                if (Schema.Properties != null)
                {
                    foreach (var propertyInfo in Schema.Properties)
                    {
                        var property = new JPropertyVM(propertyInfo.Key, this, propertyInfo.Value);
                        if (property.Value is ObservableCollection<JTokenVM>)
                        {
                            foreach (var obj in (ObservableCollection<JTokenVM>)property.Value)
                                obj.Schema = propertyInfo.Value.Items.First(); //here i use only first schema
                        }
                        properties.Add(property);
                    }
                }
                return properties;
            }
        }


        /// <summary>Converts the <see cref="JsonTokenModel"/> to a <see cref="JToken"/>. </summary>
        /// <returns>The <see cref="JToken"/>. </returns>
        public override JToken ToJToken()
        {
            var obj = new JObject();
            foreach (var pair in this)
            {
                if (pair.Value is ObservableCollection<JTokenVM>)
                {
                    var array = (ObservableCollection<JTokenVM>)pair.Value;
                    var jArray = new JArray();
                    foreach (var item in array)
                        jArray.Add(item.ToJToken());
                    obj[pair.Key] = jArray;
                }
                else if (pair.Value is JTokenVM)
                    obj[pair.Key] = ((JTokenVM)pair.Value).ToJToken();
                else
                    obj[pair.Key] = new JValue(pair.Value);
            }
            return obj;
        }
    }
}
