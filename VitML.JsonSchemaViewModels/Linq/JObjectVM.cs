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

namespace VitML.JsonVM.Linq
{

    public interface IJsonData
    {
        object this[string key] { get; set; }
    }

    /// <summary>Represents a JSON object. </summary>
    public class JObjectVM : JTokenVM
    {

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

                BindListener(key, value);
            }
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
                objVm.Properties.CollectionChanged += (se1, e1) =>
                {
                    //@todo
                };
                objVm.PropertyChanged += (se1, e1) =>
                {
                    this.OnPropertyChanged(String.Format("{0}.{1}", key, e1.PropertyName));
                };
            }
            else if (value is JArrayVM)
            {
                JArrayVM arrVm = (value as JArrayVM);
                arrVm.Items.CollectionChanged += (se1, e1) =>
                {
                    this.OnPropertyChanged(String.Format("{0}", key));
                    if (e1.Action == NotifyCollectionChangedAction.Add
                        || e1.Action == NotifyCollectionChangedAction.Replace)
                    {
                        foreach (JTokenVM item in e1.NewItems)
                        {
                            int index = arrVm.Items.IndexOf(item);
                            BindListener(String.Format("{0}[{1}]", key, index), item);                       
                        }
                    }
                };
                //@todo
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

                    Properties.Add(key, Create(pSchema, null));
                }
            }
        }

        public override void SetData(JToken data)
        {
            base.SetData(data);

            if (data == null) return;//@todo

            if (!(data is JObject)) throw new Exception("data is not JObject");

            JObject obj = data as JObject;

            foreach (var property in Schema.Properties)
            {
                JSchema pSchema = property.Value;
                JToken pData = obj[property.Key];
                if (pData == null)
                    pData = pSchema.GenerateData();

                Properties[property.Key] = FromJson(pData, pSchema);
            }     
        }

        /*
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
        }*/

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
                        JObjectVM objectVM = JObjectVM.Create(schema, obj);
                        /*
                        foreach (var property in schema.Properties)
                        {
                            JSchema pSchema = property.Value;
                            JToken pData = obj[property.Key];
                            if (pData == null)
                                pData = pSchema.GenerateData();

                            objectVM[property.Key] = FromJson(pData, pSchema);                         
                        }    */                    
                        return objectVM;
                    }
                case (JTokenType.Array):
                    {
                        JArray array = token as JArray;
                        JArrayVM arrayVM = JArrayVM.Create(schema, array);
                        /*
                        int index = 0;
                        for (int i = 0; i < array.Count; i++)
                        {
                            JToken item = array[i];
                            var propertySchema = schema.GetItemSchemaByIndex(index);
                            arrayVM.Items.Add(FromJson(item, propertySchema));
                        }    */                 
                        return arrayVM;
                    }
                default:
                    {
                        JValue value = token as JValue;
                        JValueVM vm = JValueVM.Create(schema, value);
                        //vm.Value = value;                        
                        return vm;
                    }
            }
        }

        public override JToken ToJToken()
        {
            var obj = new JObject();
            foreach (var pair in this.Properties)
            {
                JTokenVM value = (JTokenVM)pair.Value;
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
            if(value is JPropertyVM)
                value = null;//@todo (value as JPropertyVM).Value;
            if (value is JType)
                return (JType)value;
            else
                throw new Exception("type missmatch");
        }

        public void SetValue(string path, object value)
        {
            object obj = GetValue(path);
             if (obj == null) return;
            // if (obj is JPropertyVM)
              //   (obj as JPropertyVM).Value = null;//@todo value as JTokenVM;
             else if (obj is JTokenVM)
                 (obj as JTokenVM).SetData(value as JToken);
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
            get { return "null";}// Data[DisplayMemberPathPropertyName]; }
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
