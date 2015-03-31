using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VitML.Json.Schema
{
    public class JSchema : IJsonLineInfo
    {

        private JObject schema;
        private IDictionary<string, JSchema> Definitions;

        // Summary:
        //     Initializes a new instance of the Newtonsoft.Json.Schema.JSchema class.
        public JSchema()
        {
            schema = new JObject();
        }

        internal JSchema(JToken t)
        {
            if (!(t is JObject)) throw new NotImplementedException();
            var obj = t as JObject;
            this.schema = obj;
            Load( this.schema);
        }

        private void Load(JObject t)
        {
            var val = t.GetValue("id");
           // if (val != null)
               // this.Id = new Uri(val.Value<string>());
            val = t.GetValue("title");
            if (val != null)
                this.Title = val.Value<string>();
            val = t.GetValue("description");
            if (val != null)
                this.Description = val.Value<string>();
            val = t.GetValue("type");
            if (val != null)
            {
                if (val is JValue)
                    this.Type = ParseType((val as JValue).Value<string>());
                else if (val is JArray)
                {
                    foreach (var typeItem in (val as JArray).Children())
                    {
                        var type = ParseType((typeItem as JValue).Value<string>());
                        if (this.Type == null) this.Type = type;
                        else this.Type |= type;
                    }
                }
                else throw new NotImplementedException();
            }
            val = t.GetValue("format");
            if (val != null)
                this.Format = val.Value<string>();
            val = t.GetValue("pattern");
            if (val != null)
                this.Pattern = val.Value<string>();
            val = t.GetValue("definitions");
            if (val != null)
            {
                this.Definitions = new Dictionary<string, JSchema>();
                if (!(val is JObject)) throw new NotImplementedException();
                JObject definitions = val as JObject;
                foreach (var ch in definitions.Children())
                {
                    if (!(ch is JProperty)) throw new NotImplementedException();
                    var def = ch as JProperty;
                    this.Definitions.Add(def.Name, new JSchema(def.Value));
                }
            }
            val = t.GetValue("properties");
            if (val != null)
            {
                this.Properties = new Dictionary<string, JSchema>();
                if (!(val is JObject)) throw new NotImplementedException();
                JObject properties = val as JObject;
                foreach (var ch in properties.Children())
                {
                    if (!(ch is JProperty)) throw new NotImplementedException();
                    var prop = ch as JProperty;
                    this.Properties.Add(prop.Name, new JSchema(prop.Value));
                }
            }
            val = t.GetValue("default");
            if (val != null)
                this.Default = val.Value<JToken>();
            val = t.GetValue("required");
            if (val != null)
            {
                if (!(val is JArray)) throw new NotImplementedException();
                this.Required = new List<string>();
                var arr = val as JArray;
                foreach (var req in arr)
                {
                    this.Required.Add(req.Value<string>());
                }
            }
            val = t.GetValue("additionalProperties");
            if (val != null)
                this.AllowAdditionalProperties = val.Value<bool>();

            val = t.GetValue("oneOf");
            if (val != null)
            {
                this.OneOf = new List<JSchema>();
                if (!(val is JArray)) throw new NotImplementedException();
                JArray value = val as JArray;
                foreach (var ch in value.Children())
                {
                    if (!(ch is JObject)) throw new NotImplementedException();
                    var obj = ch as JObject;
                    JSchema schema = null;
                    JToken refUriToken = obj.GetValue("$ref");
                    if (refUriToken != null)
                    {
                        Uri refUri = new Uri(refUriToken.Value<string>());                        
                        //@todo
                    }
                    else
                        schema = new JSchema(obj);
                    this.OneOf.Add(schema);
                }
            }
            val = t.GetValue("allOf");
            if (val != null)
            {
                //@todo
            }
        }

        private JSchemaType? ParseType(string typestr)
        {
            switch (typestr)
            {
                case ("array"): return JSchemaType.Array;
                case ("boolean"): return JSchemaType.Boolean;
                case ("integer"): return JSchemaType.Integer;
                case ("none"): return JSchemaType.None;
                case ("null"): return JSchemaType.Null;
                case ("number"): return JSchemaType.Number;
                case ("object"): return JSchemaType.Object;
                case ("string"): return JSchemaType.String;
                default: return null;
            }            
        }

        // Summary:
        //     Gets the Newtonsoft.Json.Schema.JSchema associated with the Newtonsoft.Json.Linq.JToken.
        //
        // Parameters:
        //   t:
        //     The token.
        //
        // Returns:
        //     The Newtonsoft.Json.Schema.JSchema associated with the Newtonsoft.Json.Linq.JToken.
        public static explicit operator JSchema(JToken t)
        {
            return new JSchema(t);
        }
        //
        // Summary:
        //     Gets a Newtonsoft.Json.Linq.JToken associated with the Newtonsoft.Json.Schema.JSchema.
        //
        // Parameters:
        //   s:
        //     The schema.
        //
        // Returns:
        //     A Newtonsoft.Json.Linq.JToken associated with the Newtonsoft.Json.Schema.JSchema.
        public static implicit operator JToken(JSchema s)
        {
            return s.schema;
        }

        // Summary:
        //     Gets or sets the Newtonsoft.Json.Schema.JSchema for additional items.
        public JSchema AdditionalItems { get; set; }
        //
        // Summary:
        //     Gets or sets the Newtonsoft.Json.Schema.JSchema for additional properties.
        public JSchema AdditionalProperties { get; set; }
        //
        // Summary:
        //     Gets the AllOf schemas.
        public IList<JSchema> AllOf { get; private set; }
        //
        // Summary:
        //     Gets or sets a value indicating whether additional items are allowed.
        public bool AllowAdditionalItems { get; set; }
        //
        // Summary:
        //     Gets or sets a value indicating whether additional properties are allowed.
        public bool AllowAdditionalProperties { get; set; }
        //
        // Summary:
        //     Gets the AnyOf schemas.
        public IList<JSchema> AnyOf { get; private set; }
        //
        // Summary:
        //     Gets or sets the default value.
        public JToken Default { get; set; }
        //
        // Summary:
        //     Gets the object property dependencies.
        public IDictionary<string, object> Dependencies { get; private set; }
        //
        // Summary:
        //     Gets or sets the description of the schema.
        public string Description { get; set; }
        //
        // Summary:
        //     Gets the collection of valid enum values allowed.
        public IList<JToken> Enum { get; private set; }
        //
        // Summary:
        //     Gets or sets a flag indicating whether the value can not equal the number
        //     defined by the "maximum" attribute.
        public bool ExclusiveMaximum { get; set; }
        //
        // Summary:
        //     Gets or sets a flag indicating whether the value can not equal the number
        //     defined by the "minimum" attribute.
        public bool ExclusiveMinimum { get; set; }
        //
        // Summary:
        //     Gets the extension data for the Newtonsoft.Json.Schema.JSchema.
        public IDictionary<string, JToken> ExtensionData { get; private set; }
        //
        // Summary:
        //     Gets or sets the format.
        public string Format { get; set; }
        //
        // Summary:
        //     Gets or sets the schema ID.
        public Uri Id { get; set; }
        //
        // Summary:
        //     Gets the array item Newtonsoft.Json.Schema.JSchemas.
        public IList<JSchema> Items { get; private set; }
        //
        // Summary:
        //     Gets or sets a value indicating whether items in an array are validated using
        //     the Newtonsoft.Json.Schema.JSchema instance at their array position from
        //     Newtonsoft.Json.Schema.JSchema.Items.
        public bool ItemsPositionValidation { get; set; }
        //
        // Summary:
        //     Gets or sets the maximum value of a number.
        public double? Maximum { get; set; }
        //
        // Summary:
        //     Gets or sets the maximum number of array items.
        public int? MaximumItems { get; set; }
        //
        // Summary:
        //     Gets or sets the maximum length of a string.
        public int? MaximumLength { get; set; }
        //
        // Summary:
        //     Gets or sets the maximum number of object properties.
        public int? MaximumProperties { get; set; }
        //
        // Summary:
        //     Gets or sets the minimum value of a number.
        public double? Minimum { get; set; }
        //
        // Summary:
        //     Gets or sets the minimum number of array items.
        public int? MinimumItems { get; set; }
        //
        // Summary:
        //     Gets or sets the minimum length of a string.
        public int? MinimumLength { get; set; }
        //
        // Summary:
        //     Gets or sets the minimum number of object properties.
        public int? MinimumProperties { get; set; }
        //
        // Summary:
        //     Gets or sets the multiple of.
        public double? MultipleOf { get; set; }
        //
        // Summary:
        //     Gets the Not schema.
        public JSchema Not { get; set; }
        //
        // Summary:
        //     Gets the OneOf schemas.
        public IList<JSchema> OneOf { get; private set; }
        //
        // Summary:
        //     Gets or sets the pattern.
        public string Pattern { get; set; }
        //
        // Summary:
        //     Gets the object pattern properties.
        public IDictionary<string, JSchema> PatternProperties { get; private set; }
        //
        // Summary:
        //     Gets the object property Newtonsoft.Json.Schema.JSchemas.
        public IDictionary<string, JSchema> Properties { get; private set; }
        //
        // Summary:
        //     Gets the required object properties.
        public IList<string> Required { get; private set; }
        //
        // Summary:
        //     Gets or sets the title of the schema.
        public string Title { get; set; }
        //
        // Summary:
        //     Gets or sets the types of values allowed by the schema.
        public JSchemaType? Type { get; set; }
        //
        // Summary:
        //     Gets or sets a flag indicating whether the array items must be unique.
        public bool UniqueItems { get; set; }

        // Summary:
        //     Loads a Newtonsoft.Json.Schema.JSchema from the specified Newtonsoft.Json.JsonReader.
        //
        // Parameters:
        //   reader:
        //     The Newtonsoft.Json.JsonReader containing the JSON Schema to load.
        //
        // Returns:
        //     The Newtonsoft.Json.Schema.JSchema object representing the JSON Schema.
        public static JSchema Load(JsonReader reader)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Loads a Newtonsoft.Json.Schema.JSchema from a Newtonsoft.Json.JsonReader
        //     using the given Newtonsoft.Json.Schema.JSchemaResolver.
        //
        // Parameters:
        //   reader:
        //     The Newtonsoft.Json.JsonReader containing the JSON Schema to load.
        //
        //   settings:
        //     The Newtonsoft.Json.Schema.JSchemaReaderSettings used to load the schema.
        //
        // Returns:
        //     The Newtonsoft.Json.Schema.JSchema object representing the JSON Schema.
        //public static JSchema Load(JsonReader reader, JSchemaReaderSettings settings);
        //
        // Summary:
        //     Loads a Newtonsoft.Json.Schema.JSchema from a Newtonsoft.Json.JsonReader
        //     using the given Newtonsoft.Json.Schema.JSchemaResolver.
        //
        // Parameters:
        //   reader:
        //     The Newtonsoft.Json.JsonReader containing the JSON Schema to load.
        //
        //   resolver:
        //     The Newtonsoft.Json.Schema.JSchemaResolver to use when resolving schema references.
        //
        // Returns:
        //     The Newtonsoft.Json.Schema.JSchema object representing the JSON Schema.
        //public static JSchema Load(JsonReader reader, JSchemaResolver resolver);
        //
        // Summary:
        //     Load a Newtonsoft.Json.Schema.JSchema from a string that contains schema
        //     JSON.
        //
        // Parameters:
        //   json:
        //     A System.String that contains JSON.
        //
        // Returns:
        //     A Newtonsoft.Json.Schema.JSchema populated from the string that contains
        //     JSON.
        public static JSchema Parse(string json)
        {
            JObject t = JObject.Parse(json);
            return new JSchema(t);
        }
        //
        // Summary:
        //     Load a Newtonsoft.Json.Schema.JSchema from a string that contains schema
        //     JSON using the given Newtonsoft.Json.Schema.JSchemaReaderSettings.
        //
        // Parameters:
        //   json:
        //     The JSON.
        //
        //   settings:
        //     The Newtonsoft.Json.Schema.JSchemaReaderSettings used to load the schema.
        //
        // Returns:
        //     A Newtonsoft.Json.Schema.JSchema populated from the string that contains
        //     JSON.
        //public static JSchema Parse(string json, JSchemaReaderSettings settings);
        //
        // Summary:
        //     Load a Newtonsoft.Json.Schema.JSchema from a string that contains schema
        //     JSON using the given Newtonsoft.Json.Schema.JSchemaResolver.
        //
        // Parameters:
        //   json:
        //     The JSON.
        //
        //   resolver:
        //     The Newtonsoft.Json.Schema.JSchemaResolver to use when resolving schema references.
        //
        // Returns:
        //     A Newtonsoft.Json.Schema.JSchema populated from the string that contains
        //     JSON.
        //public static JSchema Parse(string json, JSchemaResolver resolver);
        //
        // Summary:
        //     Returns a System.String that represents the current System.Object.
        //
        // Returns:
        //     A System.String that represents the current System.Object.
        public override string ToString()
        {
            return schema.ToString();
        }
        //
        // Summary:
        //     Writes this schema to a Newtonsoft.Json.JsonWriter.
        //
        // Parameters:
        //   writer:
        //     A Newtonsoft.Json.JsonWriter into which this method will write.
        public void WriteTo(JsonWriter writer)
        {
            throw new NotImplementedException();
        }
        //
        // Summary:
        //     Writes this schema to a Newtonsoft.Json.JsonWriter using the specified Newtonsoft.Json.Schema.JSchemaResolver.
        //
        // Parameters:
        //   writer:
        //     A Newtonsoft.Json.JsonWriter into which this method will write.
        //
        //   settings:
        //     The settings used to write the schema.
        //public void WriteTo(JsonWriter writer, JSchemaWriterSettings settings);



        public bool HasLineInfo()
        {
            throw new NotImplementedException();
        }

        public int LineNumber
        {
            get { throw new NotImplementedException(); }
        }

        public int LinePosition
        {
            get { throw new NotImplementedException(); }
        }
    }
}
