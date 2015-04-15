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

        /// <summary>Initializes a new instance of the <see cref="JsonValueModel"/> class. </summary>
        public JValueVM(JSchema schema, JToken data)
            : base(schema, data)
        {
            this["Value"] = null;
        }

        public string Key { get { return null; } }

        /// <summary>Gets or sets the value. </summary>
        public object Value
        {
            get { return ContainsKey("Value") ? this["Value"] : null; }
            set { this["Value"] = value; OnPropertyChanged("Value"); }
        }

        /// <summary>Converts the <see cref="JsonTokenModel"/> to a <see cref="JToken"/>. </summary>
        /// <returns>The <see cref="JToken"/>. </returns>
        public override JToken ToJToken()
        {
            return (Value is JValue) ? Value as JValue : new JValue(Value);
        }    
       
    }
}
