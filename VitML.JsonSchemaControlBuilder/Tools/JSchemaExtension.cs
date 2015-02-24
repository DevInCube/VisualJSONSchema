using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVisualJSONEditor.Tools
{
    public static class JSchemaExtension
    {

        public static bool HasFlag(this JSchemaType? nType, JSchemaType flag)
        {
            if (nType == null) return false;
            JSchemaType type = (JSchemaType)nType;
            return type.HasFlag(flag);
        }

        public static bool IsRequired(this JSchema sh, string key)
        {
            return sh.Required.FirstOrDefault(x => x.Equals(key)) != null;
        }

        public static JSchema MergeSchemas(IEnumerable<JSchema> schemas)
        {
            JSchema res = new JSchema();
            res.Type = schemas.First().Type; //@todo
            foreach (var sh in schemas)
            {
                foreach (var p in sh.Properties)
                    res.Properties.Add(p);
                foreach (var r in sh.Required)
                    res.Required.Add(r);
            }
            return res;
        }
    }
}
