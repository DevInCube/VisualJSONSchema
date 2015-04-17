﻿using Newtonsoft.Json.Linq;
using My.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using VitML.JsonVM.Linq;
using VitML.JsonVM.Schema;
using VitML.JsonVM.Generation;
using System.Globalization;
using VitML.JsonVM.Localization;
using System.Text.RegularExpressions;

namespace VitML.JsonVM
{
    public static class JSchemaExtension
    {

        public static JToken GenerateData(this JSchema sh, DataGenerationSettings settings)
        {
            DataGenerator gen = new DataGenerator(sh);
            return gen.Generate(settings);
        }

        public static JToken GenerateData(this JSchema sh)
        {
            DataGenerator gen = new DataGenerator(sh);
            return gen.Generate(new DataGenerationSettings());
        }

        public static JSchema CheckSchema(this JSchema schema, JToken data)
        {
            if (data == null) return schema;

            if (schema.OneOf.Count > 0)
                return schema.OneOf.Choose(data);
            else if (schema.AllOf.Count > 0)
                return schema.MergeSchemaAllOf();
            else if (schema.AnyOf.Count > 0)
                return schema.AnyOf.Choose(data);
            else
                return schema;
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

        private static JSchema MergeSchemas(JSchema f, JSchema s)
        {
            JSchema n = new JSchema();

            n.Title = s.Title ?? f.Title;
            n.Description = s.Description ?? f.Description;

            n.Type = s.Type != JSchemaType.None ? s.Type : f.Type;

            foreach (var p in f.Properties)
                if (!n.Properties.ContainsKey(p.Key))
                    n.Properties.Add(p);
            foreach (var r in f.Required)
                if (!n.Required.Contains(r))
                    n.Required.Add(r);

            foreach (var p in s.Properties)
                if (!n.Properties.ContainsKey(p.Key))
                    n.Properties.Add(p);
            foreach (var r in s.Required)
                if (!n.Required.Contains(r))
                    n.Required.Add(r);

            n.Minimum = s.Minimum ?? f.Minimum;
            n.Maximum = s.Maximum ?? f.Maximum;

            n.MinLength = s.MinLength ?? f.MinLength;
            n.MaxLength = s.MaxLength ?? f.MaxLength;

            n.MinItems = s.MinItems ?? f.MinItems;
            n.MaxItems = s.MaxItems ?? f.MaxItems;

            n.MinProperties = s.MinProperties ?? f.MinProperties;
            n.MaxProperties = s.MaxProperties ?? f.MaxProperties;            

            n.Default = s.Default ?? f.Default;

            //@todo other properties
      
            return n;
        }

        public static JSchema MergeSchemaAllOf(this JSchema schema)
        {
            JSchema newSchema = new JSchema();

            newSchema.Title = schema.Title;
            newSchema.Description = schema.Description;
            newSchema.Type = schema.Type;

            foreach (var sh in schema.AllOf)
                newSchema = MergeSchemas(newSchema, sh);
            return newSchema;
        }

        public static JSchema Choose(this IList<JSchema> shList, JToken data)
        {
            if (data == null) throw new ArgumentNullException("data is null");
            foreach (var sh in shList)
                if (data.IsValid(sh))
                    return sh;
            throw new Exception("data is not valid against anyOf");
        }

        public static JSchema MatchData(this IList<JSchema> schemas, JToken data)
        {
            if (data == null)
                return null;
            foreach (var sh in schemas)
                if (data.IsValid(sh)) 
                    return sh;
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
            return "";
        }

        public static bool GetIgnore(this JSchema sh)
        {
            var ext = sh.GetExtension(JSchemaExtendedKeywords.Ignore);
            if (ext != null)
                return bool.Parse(ext.ToString());
            return false;
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

        public static void Localize(this JSchema schema, SchemaLocalizationData lang, CultureInfo info)
        {
            Regex regex = new Regex(@"\{\$loc\:.*\}");
            foreach (Match match in regex.Matches(schema.Title))
            {

            }            
        }
    }
}
