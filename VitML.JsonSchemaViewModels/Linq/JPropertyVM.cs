using Newtonsoft.Json.Linq;
using My.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using VitML.JsonVM.Common;
using VitML.JsonVM.ViewModels;

namespace VitML.JsonVM.Linq
{
    /// <summary>Describes a JSON property. </summary>
    public class JPropertyVM : ObservableObject, IStyleDecorator
    {

        private bool _IsEnabled = true;
        private PropertyStyle style;
        private ICommand _Command;
        private JSchema _SelectedSchema;

        /// <summary>Initializes a new instance of the <see cref="JsonPropertyModel"/> class. </summary>
        /// <param name="key">The key of the property. </param>
        /// <param name="parent">The parent object. </param>
        /// <param name="schema">The property type as schema object. </param>
        public JPropertyVM(string key, JObjectVM parent, JSchema schema)
        {
            Key = key;
            Parent = parent;
            OriginalSchema = schema;
            Schema = schema;            

            object ext = Schema.GetExtension("Style");
            if (ext != null && (ext is JToken))
                style = PropertyStyle.Parse(ext as JToken);

            Parent.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(Key))
                    OnPropertyChanged("Value");
            };
        }

        /// <summary>Gets the property key. </summary>
        public string Key { get; private set; }
        public string KeyTitle
        {
            get
            {
                return (Schema.Title ?? Key) ?? null;
            }
        }

        /// <summary>Gets the parent object. </summary>
        public JObjectVM Parent { get; private set; }

        /// <summary>Gets the property type as schema. </summary>
        public JSchema Schema { get; private set; }
        public JSchema OriginalSchema { get; private set; }
        public JSchema SelectedSchema {
            get { return _SelectedSchema; }
            set {
                _SelectedSchema = value;
                Schema = _SelectedSchema;
                JTokenVM data = JObjectVM.FromSchema(Schema);
                Value = data;
            }
        }

        /// <summary>Gets a value indicating whether the property is required. </summary>
        public bool IsRequired
        {
            get { return Parent.Schema.IsRequired(Key); }
        }

        /// <summary>Gets or sets the value of the property. </summary>
        public object Value
        {
            get
            {
                object val = Parent.ContainsKey(Key) ? Parent[Key] : null;
                return val;
            }
            set
            {
                Parent[Key] = value;

                if (OriginalSchema.OneOf.Count > 0)
                {
                    var val = value as JObjectVM;                                        
                    SelectedSchema = OriginalSchema.OneOf.MatchData(JToken.Parse(val.ToJson()));
                }

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

        public bool IsExpanded
        {
            get
            {
                object ext = Schema.GetExtension("IsExpanded");
                return (ext == null) ? true : bool.Parse(ext.ToString());
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

        public PropertyStyle Style { get { return style; } }

        #region Properties

        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set { _IsEnabled = value; OnPropertyChanged("IsEnabled"); }
        }

        public ICommand Command
        {
            get { return _Command; }
            set { _Command = value; OnPropertyChanged("Command"); }
        }

        #endregion Properties

    }
}
