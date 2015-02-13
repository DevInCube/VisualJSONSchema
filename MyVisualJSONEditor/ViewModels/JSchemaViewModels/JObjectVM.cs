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
using MyVisualJSONEditor.Tools;

namespace MyVisualJSONEditor.ViewModels
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
            List<string> depPar = new List<string> { "Item[]", "Count", "Keys", "Values" };
            this.CollectionChanged += (se, ar) =>
            {
                if (ar.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add 
                    || ar.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
                {
                    if (ar.NewItems[0] is KeyValuePair<string, object>)
                    {
                        var pair = (KeyValuePair<string, object>)ar.NewItems[0];
                        var val = pair.Value;
                        if (val is JObjectVM)
                        {
                            var jobj = val as JObjectVM;
                            jobj.PropertyChanged += (se2, ar2) =>
                            {
                                if (!depPar.Contains(ar2.PropertyName))
                                    this.OnPropertyChanged(pair.Key+"."+ar2.PropertyName);
                            };
                        }
                        else if (val is JArrayVM)
                        {
                            var list = (val as JArrayVM).Items;
                            list.CollectionChanged += (se1, ar1) =>
                            {
                                this.OnPropertyChanged(pair.Key);
                                if (ar1.Action == NotifyCollectionChangedAction.Add)
                                {
                                    foreach (JTokenVM item in ar1.NewItems)
                                    {
                                        item.PropertyChanged += (se2, ar2) =>
                                        {
                                            if (!depPar.Contains(ar2.PropertyName))
                                                this.OnPropertyChanged(pair.Key + "." + ar2.PropertyName);
                                        };
                                    }
                                }
                            };
                        }
                        if (!depPar.Contains(pair.Key))
                            this.OnPropertyChanged(pair.Key);
                    }
                    
                }
            };
        }

        public static JObjectVM FromSchema(JSchema schema)
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
                    obj[property.Key] = GetDefaultValue(property);
            }
            obj.Schema = schema;
            return obj;
        }

        public static JObjectVM FromJson(JObject obj, JSchema schema)
        {
            if (schema.OneOf.Count > 0)
            {
                JSchema refSchema = null;
                if (schema.OneOf.Count == 1)
                    refSchema = schema.OneOf.First();
                else
                    throw new NotImplementedException("OneOf.Count > 1");
                return FromJson(obj, refSchema);
            }
            var result = new JObjectVM();
            foreach (var property in schema.Properties)
            {
                if (property.Value.Type.HasFlag(JSchemaType.Array))
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
            {
                if (sh.Default is JValue)
                    return ((JValue)sh.Default).Value;
                else
                    return sh.Default;
            }
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
                            obj.Schema = propertyInfo.Value.Items.First(); //here i use only first schema
                    }
                    _Properties.Add(property);
                }
            }
        }

        /// <summary>Gets the object's properties. </summary>
        public IEnumerable<JPropertyVM> Properties
        {
            get
            {
                return _Properties;
            }
        }


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
                    obj[pair.Key] = new JValue(pair.Value);
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
            return this.Properties.FirstOrDefault(x => x.Key == name);
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
