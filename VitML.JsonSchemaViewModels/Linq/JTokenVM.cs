using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using My.Json.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VitML.JsonVM.ViewModels;
using VitML.JsonVM.Schema;

namespace VitML.JsonVM.Linq
{

    public abstract class JTokenVM : ObservableObject
    {

        private JSchema originalSchema;

        /// <summary>Gets or sets the schema of the token. </summary>
        public JSchema Schema { get; private set; }

        /// <summary>Gets or sets the parent list if applicable (may be null). </summary>
        public JArrayVM ParentList { get; set; }   

        public JTokenVM()
        {                       
            //@todo resolve schema

            //ParentList = new ObservableCollection<JTokenVM>();
            /*
            this.CollectionChanged += (se, ar) =>
            {                
                if (ar.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add
                    || ar.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
                {
                    if (ar.NewItems[0] is KeyValuePair<string, object>)
                    {
                        var pair = (KeyValuePair<string, object>)ar.NewItems[0];
                        OnDataChanged(pair.Key, pair.Value);
                        var handler = DataChanged;
                        if (handler != null)
                            handler.Invoke(this, new JDataChangedEventArgs(pair.Key, pair.Value));
                    }
                }
            };*/
        }

        public virtual void SetSchema(JSchema schema)
        {
            if (schema == null) throw new ArgumentNullException("schema");

            this.originalSchema = schema;

            this.Schema = schema;
        }

        public virtual void SetData(JToken data)
        {
            if (originalSchema == null) throw new ArgumentNullException("originalSchema");

            //@change schema @todo
        }

        /// <summary>Converts the token to a JSON string. </summary>
        /// <returns>The JSON string. </returns>
        public string ToJson()
        {
            var token = ToJToken();
            return JsonConvert.SerializeObject(token, Formatting.Indented);
        }

        /// <summary>Converts the <see cref="JsonTokenModel"/> to a <see cref="JToken"/>. </summary>
        /// <returns>The <see cref="JToken"/>. </returns>
        public abstract JToken ToJToken();
    }
}
