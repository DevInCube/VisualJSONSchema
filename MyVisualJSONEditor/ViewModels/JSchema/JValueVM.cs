﻿using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVisualJSONEditor.ViewModels
{
    /// <summary>Represents a single JSON value. </summary>
    public class JValueVM : JTokenVM
    {
        /// <summary>Initializes a new instance of the <see cref="JsonValueModel"/> class. </summary>
        public JValueVM()
        {
            this["Value"] = null;
        }

        /// <summary>Creates a <see cref="JsonValueModel"/> from a <see cref="JValue"/> and a given schema. </summary>
        /// <param name="value">The value. </param>
        /// <param name="schema">The schema. </param>
        /// <returns>The <see cref="JsonValueModel"/>. </returns>
        public static JValueVM FromJson(JValue value, JSchema schema)
        {
            return new JValueVM
            {
                Schema = schema,
                Value = value.Value
            };
        }

        /// <summary>Gets or sets the value. </summary>
        public object Value
        {
            get { return ContainsKey("Value") ? this["Value"] : null; }
            set { this["Value"] = value; }
        }

        /// <summary>Converts the <see cref="JsonTokenModel"/> to a <see cref="JToken"/>. </summary>
        /// <returns>The <see cref="JToken"/>. </returns>
        public override JToken ToJToken()
        {
            return new JValue(Value);
        }
    }
}
