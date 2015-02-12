﻿using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyVisualJSONEditor.Tools;

namespace MyVisualJSONEditor.ViewModels
{
    /// <summary>Describes a JSON property. </summary>
    public class JPropertyVM : ObservableObject
    {
        /// <summary>Initializes a new instance of the <see cref="JsonPropertyModel"/> class. </summary>
        /// <param name="key">The key of the property. </param>
        /// <param name="parent">The parent object. </param>
        /// <param name="schema">The property type as schema object. </param>
        public JPropertyVM(string key, JObjectVM parent, JSchema schema)
        {
            Key = key;
            Parent = parent;
            Schema = schema;

            Parent.PropertyChanged += (sender, args) => OnPropertyChanged("Value");
        }

        /// <summary>Gets the property key. </summary>
        public string Key { get; private set; }

        /// <summary>Gets the parent object. </summary>
        public JObjectVM Parent { get; private set; }

        /// <summary>Gets the property type as schema. </summary>
        public JSchema Schema { get; private set; }

        /// <summary>Gets a value indicating whether the property is required. </summary>
        public bool IsRequired
        {
            get { return Parent.Schema.IsRequired(Key); }
        }

        /// <summary>Gets or sets the value of the property. </summary>
        public object Value
        {
            get { return Parent.ContainsKey(Key) ? Parent[Key] : null; }
            set
            {
                Parent[Key] = value;

                OnPropertyChanged("Value");
                OnPropertyChanged("HasValue");
            }
        }

        /// <summary>Gets a value indicating whether the property has a value. </summary>
        public bool HasValue
        {
            get { return Value != null; }
        }

        public bool IsReadonly
        {
            get
            {
                var pair = Schema.ExtensionData.FirstOrDefault(x => x.Key == "readonly");
                if (!pair.Equals(default(KeyValuePair<string, JToken>)))
                    return bool.Parse(pair.Value.ToString());
                return false;
            }
        }

        public bool IsVisible
        {
            get
            {
                var pair = Schema.ExtensionData.FirstOrDefault(x => x.Key == "visible");
                if (!pair.Equals(default(KeyValuePair<string, JToken>)))
                    return bool.Parse(pair.Value.ToString());
                return true;
            }
        }

        public bool Ignore
        {
            get
            {
                var pair = Schema.ExtensionData.FirstOrDefault(x => x.Key == "ignore");
                if (!pair.Equals(default(KeyValuePair<string, JToken>)))
                    return bool.Parse(pair.Value.ToString());
                return false;
            }
        }

        public string DisplayMemberPath
        {
            get
            {
                var pair = Schema.ExtensionData.FirstOrDefault(x => x.Key == "DisplayMemberPath");
                string path = "Value";
                if (!pair.Equals(default(KeyValuePair<string, JToken>)))
                    path += "." + pair.Value.ToString();
                return path;
            }
        }
    }
}
