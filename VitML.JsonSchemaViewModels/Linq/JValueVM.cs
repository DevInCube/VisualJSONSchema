using Newtonsoft.Json.Linq;
using My.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VitML.JsonVM.Common;

namespace VitML.JsonVM.Linq
{
    /// <summary>Represents a single JSON value. </summary>
    public class JValueVM : JTokenVM
    {

        public override string DisplayMemberPath
        {
            get
            {
                return (Value != null) ? Value.ToString() : "<empty>";
            }
        }

        /// <summary>Initializes a new instance of the <see cref="JsonValueModel"/> class. </summary>
        private JValueVM()
        {
            //this["Value"] = null;
            PropertyChanged += JValueVM_PropertyChanged;
        }

        void JValueVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Value"))
                OnPropertyChanged("DisplayMemberPath");
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

        private JToken _Value;
        public JToken Value { get { return _Value; } set { _Value = value; OnPropertyChanged("Value"); } }

        public static JValueVM Create(JSchema schema, JToken data)
        {
            JValueVM vm = new JValueVM();
            vm.SetSchema(schema);
            vm.SetData(data);
            return vm;
        }
    }
}
