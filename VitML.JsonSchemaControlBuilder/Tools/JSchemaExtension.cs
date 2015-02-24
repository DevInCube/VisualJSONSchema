using Newtonsoft.Json.Linq;
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

        private static void Merge(JSchema parent, JSchema child)
        {
            foreach (var p in child.Properties)
                parent.Properties.Add(p);
            foreach (var r in child.Required)
                parent.Required.Add(r);
        }

        public static void MergeAllOf(this JSchema schema)
        {
            foreach (var sh in schema.AllOf)
                Merge(schema, sh);
            schema.AllOf.Clear();
        }

        public static void ChooseOneOf(this JSchema schema, JToken data)
        {
            if (schema.OneOf.Count == 1)
                Merge(schema, schema.OneOf.First());
            else
            {
                if (data == null) 
                    return;
                foreach (var sh in schema.OneOf)
                {
                    if(data.IsValid(sh))
                    {
                        Merge(schema, sh);
                        return;
                    }
                }
            }
        }
    }
}
