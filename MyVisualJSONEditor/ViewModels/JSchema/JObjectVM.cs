using Newtonsoft.Json;
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

        public JObjectVM()
        {
            this.CollectionChanged += (se, ar) => {
                if (ar.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
                {
                    if (ar.NewItems[0] is KeyValuePair<string, object>)
                    {
                        var pair = (KeyValuePair<string, object>)ar.NewItems[0];
                        var val = pair.Value;
                        if (val is JObjectVM)
                        {
                            var jobj = val as JObjectVM;
                            jobj.PropertyChanged += (se2, ar2) => {
                                this.OnPropertyChanged(pair.Key);
                            };
                        }
                    }
                }
            };
        }

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

        public static JObjectVM FromJson(JObject obj, JSchema schema)
        {
            var result = new JObjectVM();
            foreach (var property in schema.Properties)
            {
                if (property.Value.Type == JSchemaType.Array)
                {
                    var propertySchema = property.Value.Items.First();
                    var value = obj[property.Key];
                    ObservableCollection<JTokenVM> list = null;
                    if (value != null)
                    {
                        var objects = value.Select(o => o is JObject ?
                            (JTokenVM)FromJson((JObject)o, propertySchema) : JValueVM.FromJson((JValue)o, propertySchema));

                        list = new ObservableCollection<JTokenVM>(objects);
                        foreach (var item in list)
                            item.ParentList = list;
                    }
                    else
                    {
                        list = new ObservableCollection<JTokenVM>();
                    }

                    result[property.Key] = list;
                }
                else if (property.Value.Type == JSchemaType.Object)
                {
                    var token = obj[property.Key];
                    if (token is JObject)
                        result[property.Key] = FromJson((JObject)token, property.Value);
                    else
                        result[property.Key] = null;
                }
                else
                {
                    JToken value;
                    if (obj.TryGetValue(property.Key, out value))
                        result[property.Key] = ((JValue)value).Value;
                    else
                        result[property.Key] = GetDefaultValue(property);
                }
            }
            result.Schema = schema;
            return result;
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
