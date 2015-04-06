using Newtonsoft.Json;
using My.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VitML.JsonVM.Common
{
    public class JSchemaEx
    {

        private JSchema schema;

        internal JSchema Schema { get { return schema; } }

        internal JSchemaEx(JSchema schema)
        {
            this.schema = schema;
        }

        public static implicit operator JSchemaEx(JSchema ex)
        {
            return new JSchemaEx(ex);
        }

        public static JSchemaEx Parse(string jschema)
        {
            return new JSchemaEx(JSchema.Parse(jschema));
        }

        public static JSchemaEx Parse(string jschema, JSchemaLinker linker)
        {
            return new JSchemaEx(JSchema.Parse(jschema, linker.Resolver));
        }
    }
}
