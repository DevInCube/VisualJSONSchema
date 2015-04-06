using Newtonsoft.Json.Linq;
using My.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VitML.JsonVM.Extensions
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

        public static object GetExtension(this JSchema sh, string key)
        {
            var pair = sh.ExtensionData.FirstOrDefault(x => x.Key.Equals(key));
            if (!pair.Equals(default(KeyValuePair<string, JToken>)))
                return pair.Value;
            return null;
        }

        private static void Merge(JSchema parent, JSchema child)
        {
            foreach (var p in child.Properties)
                if (!parent.Properties.ContainsKey(p.Key))
                    parent.Properties.Add(p);
            foreach (var r in child.Required)
                if (!parent.Required.Contains(r))
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

        public static JSchema MatchData(this IList<JSchema> schemas, JToken data)
        {
            if (data == null)
                return null;
            foreach (var sh in schemas)
            {
                if (data.IsValid(sh))
                {
                    return sh;
                }
            }
            return null;
        }

        public static string GetDisplayMemberPath(this JSchema sh)
        {
            if (sh != null)
            {
                object style = sh.GetExtension("Style");
                if (style != null)
                {
                    object tok = (style as JToken).SelectToken("DisplayMemberPath");
                    if (tok != null)
                    {
                        return (tok as JToken).Value<string>();
                    }
                }
            }
            return "Value";
        }
    }
}
