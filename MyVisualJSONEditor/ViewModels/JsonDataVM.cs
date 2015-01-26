using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVisualJSONEditor.ViewModels
{
    public abstract class JsonDataVM : ObservableObject
    {

        public JObject JsonData { get; set; }

        public JsonDataVM _
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

    }
}
