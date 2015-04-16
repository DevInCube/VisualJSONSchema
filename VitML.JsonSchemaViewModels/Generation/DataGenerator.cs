using My.Json.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace VitML.JsonVM.Generation
{
    public class DataGenerator
    {

        private JSchema schema;

        public DataGenerator(JSchema schema)
        {
            if (schema == null) throw new ArgumentNullException("schema");

            this.schema = schema;
        }

        public JToken Generate(DataGenerationSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");

            JSchema sh = schema;

            var def = sh.Default;
            if (def != null)
            {
                if (def is JToken)
                    return def as JToken;
                else
                    return new JValue(def);
            }

            if (sh.Enum.Count > 0)
            {
                return sh.Enum.First();
            }

            JSchemaType type = sh.Type;

            if (sh.Type.HasFlag(JSchemaType.Null))
            {
                if (settings.Force == ForceLevel.None)
                    return JValue.CreateNull();
                else
                {
                    type &= ~JSchemaType.Null;
                    if (settings.Force == ForceLevel.ForceFirst)
                        settings.Force = ForceLevel.None;
                }
            }

            if (sh.OneOf.Count > 0)
            {
                JSchema first = sh.OneOf.First();
                var gen = new DataGenerator(first);
                JToken token = gen.Generate(settings);
                return token;
            }

            if (sh.AnyOf.Count > 0)
            {
                JSchema first = sh.AnyOf.First();
                var gen = new DataGenerator(first);
                JToken token = gen.Generate(settings);
                return token;
            }

            if (sh.AllOf.Count > 0)
            {
                JSchema composite = sh.MergeSchemaAllOf();
                var gen = new DataGenerator(composite);
                JToken token = gen.Generate(settings);
                return token;
            }

            switch (type)
            {
                case (JSchemaType.Object):
                    {
                        JObject obj = new JObject();
                        if (settings.RequiredOnly)
                        {
                            foreach (string req in sh.Required)
                            {
                                var gen = new DataGenerator(sh.Properties[req]);
                                JToken token = gen.Generate(settings);
                                obj.Add(new JProperty(req, token));
                            }
                        }
                        else
                        {
                            foreach (var prop in sh.Properties)
                            {
                                var gen = new DataGenerator(prop.Value);
                                JToken token = gen.Generate(settings);
                                obj.Add(new JProperty(prop.Key, token));                                
                            }
                        }
                        return obj;
                    }
                case (JSchemaType.Array):
                    {
                        JArray arr = new JArray();
                        if (settings.CreateMinItems)
                        {
                            if (sh.MinItems != null)
                            {
                                if (sh.ItemsSchema != null)
                                {
                                    for (int i = 0; i < sh.MinItems; i++)
                                    {
                                        var gen = new DataGenerator(sh.ItemsSchema);
                                        JToken token = gen.Generate(settings);
                                        arr.Add(token);
                                    }
                                }
                                else if (sh.ItemsArray.Count > 0)
                                {
                                    for (int i = 0; i < sh.MinItems; i++)
                                    {
                                        JSchema itemSchema;
                                        if (i < sh.ItemsArray.Count)
                                            itemSchema = sh.ItemsArray[i];
                                        else
                                        {
                                            if (!sh.AllowAdditionalItems)
                                                break;
                                            itemSchema = sh.AdditionalItems;
                                        }
                                        var gen = new DataGenerator(itemSchema);
                                        JToken token = gen.Generate(settings);
                                        arr.Add(token);
                                    }
                                }
                            }
                        }
                        return arr;
                    }
                case (JSchemaType.Boolean):
                    return new JValue(false);
                case (JSchemaType.Number):
                    return new JValue(0F);
                case (JSchemaType.Integer):
                    return new JValue(0);
                case (JSchemaType.String):
                    if (sh.Format == "date-time") return new JValue(new DateTime());
                    if (sh.Format == "date") return new JValue((new DateTime()).ToString("yyyy-MM-dd"));
                    if (sh.Format == "time") return new JValue(new TimeSpan());
                    if (sh.Format == "ipv4") return new JValue(IPAddress.None.ToString());
                    if (sh.Format == "ipv6") return new JValue("::");
                    if (sh.Format == "email") return new JValue("mail@mail");
                    if (sh.Format == "uri") return new JValue("uri:");
                    if (sh.Format == "hostname") return new JValue("host");
                    return JValue.CreateString(String.Empty);
                case (JSchemaType.Null):
                    return JValue.CreateNull();
                default:
                    return null;
            }
        }
    }
}
