﻿using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            get { return Schema.Required.FirstOrDefault(pp=>pp.Equals(Key))!=null; }
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
    }
}