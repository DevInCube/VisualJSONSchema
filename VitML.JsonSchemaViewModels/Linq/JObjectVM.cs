using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using My.Json.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VitML.JsonVM.Common;
using VitML.JsonVM;
using VitML.JsonVM.Schema;

namespace VitML.JsonVM.Linq
{

    public interface IJsonData
    {
        JTokenVM this[string key] { get; set; }
    }

    /// <summary>Represents a JSON object. </summary>
    public class JObjectVM : JTokenVM
    {

        private IList<JPropertyVM> _Properties;

        public IJsonData Data { get; private set; }
        public IList<JPropertyVM> Properties { get { return _Properties; } }

        public JObjectVM(JSchema schema, JToken data)
            : base(schema, data)
        {
            Data = new JsonDataImpl(this);

            _Properties = new List<JPropertyVM>();
            if (Schema.Properties != null)
            {
                foreach (var propertyInfo in Schema.Properties)
                {
                    JSchema pSchema = propertyInfo.Value;

                    var property = new JPropertyVM(propertyInfo.Key, this);

                    JTokenVM pValue = JObjectVM.FromSchema(pSchema);
                    property.Value = pValue;
                    
                    _Properties.Add(property);
                }
            }

            this.PropertyChanged += JObjectVM_PropertyChanged;
        }

        protected override void OnDataChanged(string name, object value)
        {
            string key = name;
            if (value is JObjectVM)
            {
                var jobj = value as JObjectVM;
                jobj.PropertyChanged += (se2, ar2) =>
                {
                    this.OnPropertyChanged(name + "." + ar2.PropertyName);
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
            else
            {
                var val = value as JValueVM;
                val.PropertyChanged += (se1, ar1) => {
                    this.OnPropertyChanged(key);
                };
            }
            this.OnPropertyChanged(name);
        }

        private static JSchema CheckSchema(JSchema schema, JToken data)
        {
            if (schema.OneOf.Count > 0)
                return schema.OneOf.Choose(data);
            else if(schema.AllOf.Count > 0) 
                return schema.MergeSchemaAllOf();
            else if (schema.AnyOf.Count > 0)
                return schema.AnyOf.Choose(data);                
            else
                return schema;
        }

        public static JTokenVM FromSchema(JSchema Schema)
        {
            return FromJson(Schema.GenerateData() as JToken, Schema);
        }

        public static JTokenVM FromJson(JToken token, JSchema schema)
        {
            if (token == null) token = JValue.CreateNull();   
       
            //Check @todo

            switch (token.Type)
            {
                case (JTokenType.Object):
                    {
                        JObject obj = token as JObject;
                        JObjectVM objectVM = new JObjectVM(schema, token);
                        
                        foreach (var property in schema.Properties)
                        {
                            JSchema pSchema = property.Value;
                            JToken pData = obj[property.Key];
                            if (pData == null)
                                pData = pSchema.GenerateData();

                            objectVM[property.Key] = FromJson(pData, pSchema);                         
                        }                        
                        return objectVM;
                    }
                case (JTokenType.Array):
                    {
                        JArray array = token as JArray;
                        JArrayVM arrayVM = new JArrayVM(schema, token);

                        int index = 0;
                        for (int i = 0; i < array.Count; i++)
                        {
                            JToken item = array[i];
                            var propertySchema = schema.GetItemSchemaByIndex(index);
                            arrayVM.Items.Add(FromJson(item, propertySchema));
                        }                        
                        return arrayVM;
                    }
                default:
                    {
                        JValue value = token as JValue;
                        JValueVM vm = new JValueVM(schema, token);
                        vm.Value = value;                        
                        return vm;
                    }
            }
        }

        public override JToken ToJToken()
        {
            var obj = new JObject();
            foreach (var pair in this)
            {
                JTokenVM value = (JTokenVM)pair.Value;
                bool ignore = value.Schema.GetIgnore();
                if (ignore) continue;
                if (pair.Value is JTokenVM)
                    obj[pair.Key] = value.ToJToken();
                else
                {
                    if (pair.Value is JToken)
                        obj[pair.Key] = pair.Value as JToken;
                    else
                        obj[pair.Key] = new JValue(pair.Value);
                }
            }
            return obj;
        }

        public object GetValue(string path)
        {
            var pathReader = new JPathReader(this);
            return pathReader.GetValue(path);
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

        public void SetValue(string path, JTokenVM value)
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

        public string DisplayMemberPathPropertyName { 
            get { return this.Schema.GetDisplayMemberPath(); }
        }

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

        public JTokenVM this[string key]
        {
            get { return vm.GetValue<JTokenVM>(key); }
            set { vm.SetValue(key, value); }
        }
    }
}
