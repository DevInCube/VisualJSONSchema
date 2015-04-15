using Newtonsoft.Json.Linq;
using My.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using VitML.JsonVM.Common;
using VitML.JsonVM.ViewModels;
using VitML.JsonVM.Schema;

namespace VitML.JsonVM.Linq
{
    /// <summary>Describes a JSON property. </summary>
    public class JPropertyVM : ObservableObject
    {

        private bool _IsEnabled = true;     
        private ICommand _Command;        

        /// <summary>Initializes a new instance of the <see cref="JsonPropertyModel"/> class. </summary>
        /// <param name="key">The key of the property. </param>
        /// <param name="parent">The parent object. </param>
        /// <param name="schema">The property type as schema object. </param>
        public JPropertyVM(string key, JObjectVM parent)
        {
            Key = key;
            Parent = parent;                    

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
                return null;// (Value.Schema.Title ?? Key) ?? null;
            }
        }

        /// <summary>Gets the parent object. </summary>
        public JObjectVM Parent { get; private set; }        

        /// <summary>Gets a value indicating whether the property is required. </summary>
        public bool IsRequired
        {
            get { return Parent.Schema.IsRequired(Key); }
        }
        /*
        /// <summary>Gets or sets the value of the property. </summary>
        public JTokenVM Value
        {
            get
            {
                object val = Parent.ContainsKey(Key) ? Parent[Key] : null;
                if (!(val is JTokenVM)) throw new Exception("baaaaad");
                return val as JTokenVM;
            }
            set
            {
                if (value == null) throw new ArgumentNullException();

                Parent[Key] = value;

                //Schema.SetData(value); //Change schema @todo

                OnPropertyChanged("Value");
                OnPropertyChanged("HasValue");
            }
        }*/

        /// <summary>Gets a value indicating whether the property has a value. </summary>
        public bool HasValue
        {
            get { return false;}// !JToken.DeepEquals(Value.Data, JValue.CreateNull()); }
        }

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


        public void RaisePropertyChanged(string p)
        {
            OnPropertyChanged(p);
        }
    }
}
