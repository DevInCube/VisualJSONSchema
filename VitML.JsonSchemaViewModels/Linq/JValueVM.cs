using Newtonsoft.Json.Linq;
using My.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VitML.JsonVM.Common;
using System.Windows.Input;

namespace VitML.JsonVM.Linq
{
    /// <summary>Represents a single JSON value. </summary>
    public class JValueVM : JTokenVM
    {

        private JToken _Value;
        private ICommand _Command;

        public override string DisplayMemberPath { get { return (Value != null) ? Value.ToString() : "<empty>"; } }

        public JToken Value
        {
            get { return _Value; }
            set => SetProperty(ref _Value, value);
        }

        public ICommand Command 
        { 
            get { return _Command; }
            set => SetProperty(ref _Command, value);
        }

        /// <summary>Initializes a new instance of the <see cref="JsonValueModel"/> class. </summary>
        private JValueVM()
        {
            PropertyChanged += JValueVM_PropertyChanged;
        }

        void JValueVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Value)))
                OnPropertyChanged(nameof(DisplayMemberPath));
        }

        /// <summary>Converts the <see cref="JsonTokenModel"/> to a <see cref="JToken"/>. </summary>
        /// <returns>The <see cref="JToken"/>. </returns>
        public override JToken ToJToken()
        {
            return Value;
        }

        public override void SetData(JToken data)
        {
            base.SetData(data);

            if (data == null) return;//@todo

            if (!(data is JValue)) throw new ArgumentException("data is not JValue");

            this.Value = data;
        }

        public static JValueVM Create(JSchema schema, JToken data)
        {
            JValueVM vm = new JValueVM();
            vm.SetSchema(schema);
            vm.SetData(data);
            return vm;
        }
    }
}
