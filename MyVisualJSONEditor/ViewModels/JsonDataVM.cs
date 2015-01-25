using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVisualJSONEditor.ViewModels
{
    public abstract class JsonDataVM : ObservableObject, IDictionary<string, object>
    {

        public JObject JsonData { get; set; }
        
        private Dictionary<string, string> _MyDict;

        public IDictionary<string, object> _
        {
            get { return this; }            
        }

        public object this[string key]
        {
            get
            {
                if (JsonData != null)
                {
                    JToken token = JsonData.SelectToken(key);
                    if (token != null)
                    {
                        object val = token.ToObject<object>();
                        return val;
                    }                    
                }
                return null;
            }
            set
            {
                if (JsonData != null)
                {
                    JToken token = JsonData.SelectToken(key);
                    if (token != null)
                    {
                        JProperty property = token.Parent as JProperty;
                        if (property != null)
                        {
                            (property.Value as JValue).Value = value;
                            string propName = String.Format("_[{0}]", key);
                            OnPropertyChanged(propName);
                            OnPropertyChanged("_");
                        }
                    }
                }
            }
        }


        public void Add(string key, object value)
        {
            //throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            return false;
        }

        public ICollection<string> Keys
        {
            get { return null; }
        }

        public bool Remove(string key)
        {
            return true;
        }

        public bool TryGetValue(string key, out object value)
        {
            value = null;
            return false;
        }

        public ICollection<object> Values
        {
            get { return null;  }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            //throw new NotImplementedException();
        }

        public void Clear()
        {
            //throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return false;
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            //
        }

        public int Count
        {
            get { return 0; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return true;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return null;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return null;
        }
    }
}
