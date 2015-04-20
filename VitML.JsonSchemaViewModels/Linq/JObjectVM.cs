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
using VitML.JsonVM.ViewModels;
using System.Collections;

namespace VitML.JsonVM.Linq
{

    public interface IJsonData
    {
        object this[string key] { get; set; }
    }

    /// <summary>Represents a JSON object. </summary>
    public class JObjectVM : JTokenVM
    {

        private IList<string> _RequiredPropertyNames;        

        public PropertyDictionary Properties { get; private set; }

        private JObjectVM()
        {
            //Data = new JsonDataImpl(this);

            Properties = new PropertyDictionary();

            Properties.CollectionChanged += Properties_CollectionChanged;

            this.PropertyChanged += JObjectVM_PropertyChanged;
        }      

        void Properties_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add
                || e.Action == NotifyCollectionChangedAction.Replace)
            {

                var pair = (KeyValuePair<string, JTokenVM>)(e.NewItems[0]);
                string key = pair.Key;
                JTokenVM value = pair.Value;

                value.IsRequired = IsPropertyRequired(key);

                BindListener(key, value);
            }
        }

        public bool IsPropertyRequired(string propertyName)
        {
            if (_RequiredPropertyNames == null) 
                return false;
            return _RequiredPropertyNames.FirstOrDefault(x => x.Equals(propertyName)) != null;
        }

        private void BindListener(string key, JTokenVM value)
        {
            if (value is JValueVM)
            {
                value.PropertyChanged += (se1, e1) =>
                {
                    this.OnPropertyChanged(String.Format("{0}", key));
                };
            }
            else if (value is JObjectVM)
            {
                JObjectVM objVm = (value as JObjectVM);

                if (objVm.Properties.Count > 0)
                {
                    IList props = objVm.Properties.Values.ToList();
                    BindProperties(props, key);
                }

                objVm.Properties.CollectionChanged += (se1, e1) =>
                {
                    this.OnPropertyChanged(String.Format("{0}", key));
                    if (e1.Action == NotifyCollectionChangedAction.Add
                        || e1.Action == NotifyCollectionChangedAction.Replace)
                    {
                        BindProperties(e1.NewItems, key);
                    }
                };

                objVm.PropertyChanged += (se1, e1) =>
                {
                    this.OnPropertyChanged(String.Format("{0}.{1}", key, e1.PropertyName));
                };
            }
            else if (value is JArrayVM)
            {
                JArrayVM arrVm = (value as JArrayVM);

                if (arrVm.Items.Count > 0)
                {
                    BindListItems(arrVm.Items, key);
                }

                arrVm.Items.CollectionChanged += (se1, e1) =>
                {
                    this.OnPropertyChanged(String.Format("{0}", key));
                    if (e1.Action == NotifyCollectionChangedAction.Add
                        || e1.Action == NotifyCollectionChangedAction.Replace)
                    {                        
                        BindListItems(e1.NewItems, key);
                    }
                };

                arrVm.PropertyChanged += (se1, e1) =>
                {
                    this.OnPropertyChanged(String.Format("{0}.{1}", key, e1.PropertyName));
                };                
            } 
        }

        private void BindProperties(IList list, string key)
        {
            //@todo
        }

        private void BindListItems(IList list, string key)
        {
            foreach (JTokenVM item in list)
            {
                int index = list.IndexOf(item);
                BindListener(String.Format("{0}[{1}]", key, index), item);
            }
        }

        public static JObjectVM Create(JSchema schema, JToken data)
        {
            JObjectVM objectVM = new JObjectVM();
            objectVM.SetSchema(schema);
            objectVM.SetData(data);            
            return objectVM;
        }

        public override void SetSchema(JSchema schema)
        {
            base.SetSchema(schema);

            if (Schema.Properties != null)
            {
                foreach (var propertyInfo in Schema.Properties)
                {
                    string key = propertyInfo.Key;
                    JSchema pSchema = propertyInfo.Value;

                    Properties.Add(key, FromJson(null, pSchema));
                }
            }

            _RequiredPropertyNames = Schema.Required;
            
        }

        private string ResolveDisplayMemberPath(string defaultPath)
        {
            if (String.IsNullOrWhiteSpace(defaultPath))
            {
                if (Properties.Count > 0)
                    return Properties.First().Key;
                else
                    return (Schema != null) ? (Schema.Title ?? "") : "";
            }
            else
            {
                return defaultPath;
            }
        }

        public override void SetData(JToken data)
        {
            base.SetData(data);

            if (data == null || data.Type == JTokenType.Null) return;//@todo

            if (!(data is JObject)) throw new Exception("data is not JObject");

            JObject obj = data as JObject;

            foreach (var property in Schema.Properties)
            {
                JSchema pSchema = property.Value;
                JToken pData = obj[property.Key];

                bool spec = pData != null;
                if (pData == null)
                    pData = pSchema.GenerateData();

                JTokenVM tokenVM = FromJson(pData, pSchema);
                if (!spec) tokenVM.IsSpecified = false;
                
                Properties[property.Key] = tokenVM;
            }     
            OnPropertyChanged("Properties");
        }        

        public static JTokenVM FromSchema(JSchema Schema)
        {
            return FromJson(Schema.GenerateData() as JToken, Schema);
        }

        public static JTokenVM FromJson(JToken token, JSchema schema)
        {
            if (token == null || JValue.DeepEquals(JValue.CreateNull(), token as JValue))
            {
                if (schema.Type.HasFlag(JSchemaType.Object))
                   return JObjectVM.Create(schema, null);
                if (schema.Type.HasFlag(JSchemaType.Array))
                    return JArrayVM.Create(schema, null);
                return JValueVM.Create(schema, null);
            }

            switch (token.Type)
            {
                case (JTokenType.Object):
                    {
                        JObject obj = token as JObject;
                        JObjectVM objectVM = JObjectVM.Create(schema, obj);        
                        return objectVM;
                    }
                case (JTokenType.Array):
                    {
                        JArray array = token as JArray;
                        JArrayVM arrayVM = JArrayVM.Create(schema, array);        
                        return arrayVM;
                    }
                default:
                    {
                        JValue value = token as JValue;
                        JValueVM vm = JValueVM.Create(schema, value);                       
                        return vm;
                    }
            }
        }

        public override JToken ToJToken()
        {
            if (!HasValue) return null;
            var obj = new JObject();
            foreach (var pair in this.Properties)
            {
                JTokenVM value = (JTokenVM)pair.Value;
                if (!value.IsSpecified)
                    continue;
                bool ignore = value.Schema.GetIgnore();
                if (ignore) continue;
                if (pair.Value is JTokenVM)
                    obj[pair.Key] = value.ToJToken();
                else
                {
                    obj[pair.Key] = pair.Value.ToJToken();
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
            if (value is KeyValuePair<string, JTokenVM>)
                value = ((KeyValuePair<string, JTokenVM>)value).Value;
            if (value is JType)
                return (JType)value;
            else
                throw new Exception("type missmatch");
        }

        public void SetValue(string path, object value)
        {
            object obj = GetValue(path);
            if (obj == null) return;
            if (obj is JTokenVM)
                (obj as JTokenVM).SetData(value as JToken);
            else if (obj is KeyValuePair<string, JTokenVM>)
                throw new NotImplementedException("cant set to " + obj.GetType());
            else
                throw new NotImplementedException("cant set to " + obj.GetType());
        }      

        public override string DisplayMemberPath
        {
            get {
                var pathReader = new JPathReader(this);
                var displayPath = ResolveDisplayMemberPath(ParentList.DisplayMemberPathPropertyName);
                JTokenVM token = pathReader.GetToken(displayPath);
                string result = null;
                if (token != null)
                {
                    if (token is JValueVM)
                        result = (token as JValueVM).Value.ToString();
                    else
                        result = token.ToJson();
                }
                if (string.IsNullOrWhiteSpace(result))
                    return String.Format("{0}:<empty>", displayPath);
                else
                    return result;
            }
        }

        void JObjectVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ParentList != null)
                if (e.PropertyName.Equals(ResolveDisplayMemberPath(ParentList.DisplayMemberPathPropertyName)))
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
