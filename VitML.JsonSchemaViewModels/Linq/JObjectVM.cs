﻿using Newtonsoft.Json;
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
                        else if (value is JTokenVM)
                        {
                            (value as JTokenVM).PropertyChanged += (se1, ar1) =>
                            {
                                this.OnPropertyChanged(ar1.PropertyName);
                            };
                        }
                    }
                    
                }
            };
            this.PropertyChanged += JObjectVM_PropertyChanged;
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

        public static JTokenVM FromSchema(JSchema Schema)
        {
            return FromJson(Schema.GenerateData() as JToken, Schema);
        }

        public static JTokenVM FromJson(JToken token, JSchema schema)
        {
            if (token == null) return JValueVM.FromJson(null, schema);

            schema = CheckSchema(schema, token);

            switch (token.Type)
            {
                case (JTokenType.Object):
                    {
                        JObject obj = token as JObject;
                        JObjectVM objVM = new JObjectVM(); 
                  
                        foreach (var property in schema.Properties)
                        {
                            JSchema pSchema = property.Value;
                            JToken pData;
                            if (!obj.TryGetValue(property.Key, out pData))
                                pData = pSchema.GenerateData();

                            objVM[property.Key] = FromJson(pData, pSchema);
                        }

                        objVM.Schema = schema;
                        return objVM;
                    }
                case (JTokenType.Array):
                    {
                        JArray array = token as JArray;
                        JArrayVM arrayVM = new JArrayVM();
                        
                        for(int index = 0; index < array.Count; index++)
                        {
                            JSchema itemSchema = schema.GetItemSchemaByIndex(index);
                            JToken item = array[index];

                            if (item == null)
                                item = itemSchema.GenerateData(); 

                            JTokenVM itemVM = FromJson(item, itemSchema);
                            arrayVM.Items.Add(itemVM);
                        }

                        arrayVM.Schema = schema;
                        return arrayVM;
                    }
                default:
                    {                        
                        return JValueVM.FromJson(token as JValue, schema);
                    }
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
                        JArrayVM arrayVM = property.Value as JArrayVM;

                        int index = 0;
                        foreach (var obj in arrayVM.Items)
                        {
                            var propertySchema = propertyInfo.Value.GetItemSchemaByIndex(index++);                            
                            obj.Schema = CheckSchema(propertySchema, null);
                        }
                    }
                    _Properties.Add(property);
                }
            }
        }

        /// <summary>Gets the object's properties. </summary>
        public IEnumerable<JPropertyVM> Properties { get { return _Properties; } }

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
            var reader = new JTokenVMPathReader(this);
            return reader.GetValue(path);
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
