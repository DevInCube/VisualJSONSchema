﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using My.Json.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VitML.JsonVM.ViewModels;

namespace VitML.JsonVM.Linq
{

    public abstract class JTokenVM : ObservableDictionary
    {

        private JSchema _Schema;
        /// <summary>Gets or sets the schema of the token. </summary>
        public JSchema Schema
        {
            get { return _Schema; }
            set { _Schema = value; OnSetSchema(); }
        }

        /// <summary>Gets or sets the parent list if applicable (may be null). </summary>
        public ObservableCollection<JTokenVM> ParentList { get; set; }

        public event JDataChangedEventHandler DataChanged;

        public JTokenVM()
        {
            //ParentList = new ObservableCollection<JTokenVM>();

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
            };
        }

        protected virtual void OnDataChanged(string name, object value) { }

        protected virtual void OnSetSchema() { }


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
