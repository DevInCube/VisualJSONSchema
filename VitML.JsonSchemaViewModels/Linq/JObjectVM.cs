using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VitML.JsonVM.Common;
using VitML.JsonVM.Extensions;

namespace VitML.JsonVM.Linq
{

    public interface IJsonData
    {
        object this[string key] { get; set; }
    }

    /// <summary>Represents a JSON object. </summary>
    public class JObjectVM : JTokenVM
    {

        private List<JPropertyVM> _Properties;

        public IJsonData Data { get; private set; }

        public JObjectVM()
        {
            Data = new JsonDataImpl(this);
            this.CollectionChanged += (se, ar) =>
            {
                if (ar.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add 
                    || ar.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
                {
                    if (ar.NewItems[0] is KeyValuePair<string, object>)
                    {
                        var pair = (KeyValuePair<string, object>)ar.NewItems[0];
                        string key = pair.Key;
                        var value = pair.Value;
                        if (value is JObjectVM)
                        {
                            var jobj = value as JObjectVM;
                            jobj.PropertyChanged += (se2, ar2) =>
                            {
                                this.OnPropertyChanged(pair.Key + "." + ar2.PropertyName);
                            };
                        }
                        else if (value is JArrayVM)
                        {
                            var list = (value as JArrayVM).Items;
                            list.CollectionChanged += (se1, ar1) =>
                            {
                                this.OnPropertyChanged(key);
                                if (ar1.Action == NotifyCollectionChangedAction.Add
                                    || ar1.Action == NotifyCollectionChangedAction.Replace)
                                {
                                    foreach (JTokenVM item in ar1.NewItems)
                                    {
                                        item.PropertyChanged += (senderItem, e2) =>
                                        {
                                            int index = list.IndexOf(senderItem as JTokenVM);
                                            string msg = String.Format("{0}[{1}].{2}", key, index, e2.PropertyName);
                                            this.OnPropertyChanged(msg);
                                        };
                                    }
                                }
                            };
                        }
                    }
                    
                }
            };
            this.PropertyChanged += JObjectVM_PropertyChanged;
        }

        public static JTokenVM FromSchema(JSchemaEx jex)
        {
            return FromSchema(jex.Schema);
        }

        internal static JTokenVM FromSchema(JSchema schema)
        {
            if (schema.OneOf.Count > 0)
            {
                JSchema refSchema = null;
                if (schema.OneOf.Count == 1)
                    refSchema = schema.OneOf.First();
                else
                    throw new NotImplementedException("OneOf.Count > 1");
                return FromSchema(refSchema);
            }
            if (schema.Type.HasFlag(JSchemaType.Object))
            {
                var obj = new JObjectVM();
                if (schema.Default != null)
                    return FromJson(schema.Default as JObject, schema);
                foreach (var property in schema.Properties)
                {
                    if (property.Value.Type.HasFlag(JSchemaType.Object))
                    {
                        obj[property.Key] = FromSchema(property.Value);
                    }
                    else if (property.Value.Type.HasFlag(JSchemaType.Array))
                    {
                        obj[property.Key] = new JArrayVM();
                    }
                    else
                        obj[property.Key] = GetDefaultValue(property.Value);
                }
                obj.Schema = schema;
                return obj;
            }
            else
            {
                return new JValueVM() { Value = GetDefaultValue(schema), Schema = schema };
            }
        }

        private static JSchema CheckSchema(JSchema schema, JToken data)
        {
            if (schema.OneOf.Count > 0)
            {
                schema.ChooseOneOf(data);
                return schema;
            }
            else if(schema.AllOf.Count > 0) 
            {
                schema.MergeAllOf();
                return schema;
            }
            else if (schema.AnyOf.Count > 0)
            {
                throw new NotImplementedException("AnyOf");
            }
            else
            {
                return schema;
            }
        }        

        public static JObjectVM FromJson(JObject obj, JSchema schema)
        {                        
            schema = CheckSchema(schema, obj);
            var result = new JObjectVM();
            foreach (var property in schema.Properties)
            {
                if (property.Value.Type.HasFlag(JSchemaType.Array))
                {
                    var propertySchema = property.Value.Items.First();//@todo
                    var value = obj[property.Key];
                    List<JTokenVM> list = null;
                    if (value != null)
                    {
                        var objects = value.Select(o => o is JObject ?
                            (JTokenVM)JObjectVM.FromJson((JObject)o, propertySchema) 
                            : JValueVM.FromJson((JValue)o, CheckSchema(propertySchema, o))
                        );

                        list = new List<JTokenVM>(objects);
                    }
                    else
                    {
                        list = new List<JTokenVM>();
                    }
                    JArrayVM array = new JArrayVM();
                    result[property.Key] = array;
                    foreach (var item in list)
                        array.Items.Add(item);
                }
                else if (property.Value.Type.HasFlag(JSchemaType.Object))
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
                        result[property.Key] = (JValue)value;
                    else
                        result[property.Key] = GetDefaultValue(property.Value);
                }
            }
            result.Schema = schema;
            return result;
        }

        private static object GetDefaultValue(JSchema sh)
        {
            if (sh.Default != null)
            {
                if (sh.Default is JValue)
                    return ((JValue)sh.Default).Value;
                else
                    return sh.Default;
            }
            if (sh.Enum.Count > 0)
                return sh.Enum.First();
            switch (sh.Type)
            {
                case (JSchemaType.Boolean):
                    return new JValue(false);
                case (JSchemaType.Number):
                    return new JValue(0F);
                case (JSchemaType.Integer):
                    return new JValue(0);
                case (JSchemaType.String):
                    if (sh.Format == "date-time") return new DateTime();
                    if (sh.Format == "date") return (new DateTime()).ToString("yyyy-MM-dd");
                    if (sh.Format == "time") return new TimeSpan();
                    if (sh.Format == "ipv4") return new JValue(IPAddress.None.ToString()); //@todo @test
                    return new JValue(String.Empty);
                case (JSchemaType.Null):
                    return null;
                default:
                    return null;
            }
        }

        protected override void OnSetSchema()
        {
            _Properties = new List<JPropertyVM>();
            if (Schema.Properties != null)
            {
                foreach (var propertyInfo in Schema.Properties)
                {
                    var property = new JPropertyVM(propertyInfo.Key, this, propertyInfo.Value);
                    if (property.Value is JArrayVM)
                    {
                        foreach (var obj in ((JArrayVM)property.Value).Items)
                        {
                            obj.Schema = CheckSchema(propertyInfo.Value.Items.First(), null); //here i use only first schema
                        }
                    }
                    _Properties.Add(property);
                }
            }
        }

        /// <summary>Gets the object's properties. </summary>
        public IEnumerable<JPropertyVM> Properties { get { return _Properties; } }

        /// <summary>Converts the <see cref="JsonTokenModel"/> to a <see cref="JToken"/>. </summary>
        /// <returns>The <see cref="JToken"/>. </returns>
        public override JToken ToJToken()
        {
            var obj = new JObject();
            foreach (var pair in this)
            {
                bool ignore = Properties.FirstOrDefault(x => x.Key == pair.Key).Ignore;
                if (ignore) continue;
                if (pair.Value is JTokenVM)
                    obj[pair.Key] = ((JTokenVM)pair.Value).ToJToken();
                else
                {
                    if (pair.Value is JValue)
                        obj[pair.Key] = pair.Value as JValue;
                    else
                        obj[pair.Key] = new JValue(pair.Value);
                }
            }
            return obj;
        }

        public object GetValue(string path)
        {
            StringBuilder nameBuffer = new StringBuilder();
            string propName = null;
            int index = 0;
            bool hasIndexer = false;
            object result = null;
            JTokenVM obj = this;
            for (int pos = 0; pos <= path.Length; pos++)
            {
                bool isEOF = pos == path.Length;
                char ch =(char)0;
                if(!isEOF)
                    ch = path[pos];
                if (isEOF || ch == '.')
                {
                    propName = nameBuffer.ToString();
                    nameBuffer.Clear();
                    if (obj is JArrayVM)
                    {
                        throw new Exception("illegal array call");
                    }
                    else if (obj is JObjectVM)
                    {
                        JPropertyVM prop = (obj as JObjectVM).Properties.FirstOrDefault(x => x.Key == propName);
                        if (!hasIndexer)
                        {
                            if (isEOF)
                                result = prop;
                            else
                                obj = prop.Value as JTokenVM;
                        }
                        else
                        {
                            hasIndexer = false;
                            obj = prop.Value as JTokenVM;
                            if (obj is JArrayVM)
                            {
                                object value = (obj as JArrayVM).Data[index];
                                if (isEOF)
                                    result = value;
                                else
                                {
                                    if (value is JTokenVM)
                                        obj = value as JTokenVM;
                                    else
                                        throw new Exception("is not a JTokenVM item but a " + value.GetType());
                                }
                            }
                            else
                            {
                                throw new Exception("cant call indexer on JObjectVM");
                            }
                        }
                    }
                    continue;
                }
                else if (ch == '[')
                {
                    index = ReadIntIndexer(path, pos + 1, ref pos);
                    hasIndexer = true;
                    continue;
                }
                nameBuffer.Append(ch);
            }
            return result;
        }

        private int ReadIntIndexer(string path, int startPos, ref int endPos)
        {
            StringBuilder buffer = new StringBuilder();
            for (int pos = startPos; pos < path.Length; pos++)
            {
                char ch = path[pos];
                bool isLastChar = pos == path.Length - 1;
                if (ch == ']' || isLastChar)
                {
                    string index = buffer.ToString();
                    buffer.Clear();
                    int indexInt = 0;
                    if (int.TryParse(index, out indexInt))
                    {
                        endPos = pos;
                        return indexInt;
                    }
                    else
                    {
                        throw new NotImplementedException("Path dict not supperted");
                    }
                }
                buffer.Append(ch);
            }
            throw new Exception("unclosed indexer");
        }

        public JType GetValue<JType>(string path)
        {
            object value = GetValue(path);
            if (value == null) return default(JType);
            if(value is JPropertyVM)
                value = (value as JPropertyVM).Value;
            if (value is JType)
                return (JType)value;
            else
                throw new Exception("type missmatch");
        }

        public void SetValue(string path, object value)
        {
            object obj = GetValue(path);
             if (obj == null) return;
             if (obj is JPropertyVM)
                 (obj as JPropertyVM).Value = value;
             else if (obj is JTokenVM)
                 (obj as JTokenVM)["Value"] = value;
             else
                 throw new NotImplementedException("cant set to " + obj.GetType());
        }

        public JPropertyVM GetProperty(string name)
        {
            return GetValue(name) as JPropertyVM;
        }

        public string DisplayMemberPathPropertyName { get { return this.Schema.GetDisplayMemberPath(); } }

        public object DisplayMemberPath
        {
            get { return Data[DisplayMemberPathPropertyName]; }
        }

        void JObjectVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(DisplayMemberPathPropertyName))
                OnPropertyChanged("DisplayMemberPath");
        }
    }

    class JsonDataImpl : IJsonData
    {
        private JObjectVM vm;

        public JsonDataImpl(JObjectVM vm)
        {
            this.vm = vm;
        }

        public object this[string key]
        {
            get { return vm.GetValue<object>(key); }
            set { vm.SetValue(key, value); }
        }
    }
}
