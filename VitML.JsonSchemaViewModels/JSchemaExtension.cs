using Newtonsoft.Json.Linq;
using My.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using VitML.JsonVM.Linq;

namespace VitML.JsonVM
{
    public static class JSchemaExtension
    {

        public static object GenerateData(this JSchema sh)
        {
            if (sh.Default != null)
            {
                if (sh.Default is JValue)
                    return ((JValue)sh.Default).Value;
                else
                    return sh.Default;
            }

            if (sh.Enum.Count > 0)
            {
                return sh.Enum.First();
            }

            switch (sh.Type)
            {
                case (JSchemaType.Object):
                    {
                        return null;
                        //@todo generate object
                        JObjectVM objVM = new JObjectVM();
                        JObject obj = new JObject();
                        foreach (string req in sh.Required)
                            obj.Add(new JProperty(req, sh.Properties[req].GenerateData()));

                        objVM["Value"] = obj;
                        objVM.Schema = sh;
                        return objVM;
                    }
                case (JSchemaType.Array):
                    return new JArray();
                case (JSchemaType.Boolean):
                    return new JValue(false);
                case (JSchemaType.Number):
                    return new JValue(0F);
                case (JSchemaType.Integer):
                    return new JValue(0);
                case (JSchemaType.String):
                    if (sh.Format == "date-time") return new DateTime();
                    if (sh.Format == "date") return (new DateTime()).ToString("yyyy-MM-dd");
                    if (sh.Format == "time") return new TimeSpan();
                    if (sh.Format == "ipv4") return new JValue(IPAddress.None.ToString());
                    if (sh.Format == "ipv6") return new JValue("::");
                    if (sh.Format == "email") return new JValue("mail@mail");
                    if (sh.Format == "uri") return new JValue("uri:");
                    if (sh.Format == "hostname") return new JValue("host");
                    return new JValue(String.Empty);
                case (JSchemaType.Null):
                    return null;
                default:
                    return null;
            }
        }

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

        public static JSchema GetItemSchemaByIndex(this JSchema pSchema, int index)
        {
            if (index < 0) throw new IndexOutOfRangeException();

            int itemsCount = pSchema.ItemsArray.Count;
            JSchema propertySchema;
            if (itemsCount == 0)
            {
                propertySchema = pSchema.ItemsSchema;
            }
            else
            {
                if (index < itemsCount)
                    propertySchema = pSchema.ItemsArray[index];
                else
                    propertySchema = pSchema.AdditionalItems;
            }
            return propertySchema;
        }
    }
}
