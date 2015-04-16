using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VitML.JsonVM.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MySchemaTest.Run();
            return;
            string shStr = @"{
    'id' : 'http://valid.com/#',
    'definitions' : {
        'test' : {
            '$schema' : 'http://json-schema.org/draft-04/schema#',
            'type' : 'string',
        }
    },
    'type':'object',
    'properties': {
        'foo': {
            'id' : 'id2',
            'type': 'string',
            'pattern':'',
        },
        'foo2': {
            'type': 'array',
            'minItems' : -1,
        }
    },
    'patternProperties' : {
        '' : {}
    },
    'additionalProperties': true
}";        
            var sh1 = JSchema.Parse(shStr);            
            JObject obj = JObject.Parse("{ 'foo':'ok'}");
            var isValid = obj.IsValid(sh1);
            var sh2 = JSchema.Parse(@"
{
    'type' : ['string','integer'],
}
");
            var valid = JValue.Parse("'dsdf'").IsValid(sh2);
            return;
            JSchemaPreloadedResolver res = new JSchemaPreloadedResolver();
            var r = JSchema.Parse(@"{
    'id': 'http://my.site/myschema#',
    'definitions': {
        'schema1': {
            'id': 'schema1',
'description' :'ok',
            'type': 'integer'
        },
    },
    'properties': {
        'schema2': {
            'type': 'object',
            'properties': { 'test' : {'type': 'object', 'description' :'dfggok', '$ref': '#/definitions/schema1'} }
        }
    }
}");
            
            var r2 = JSchema.Parse("{'type':'string'   , properties:{'test':{}}}");
            
            Newtonsoft.Json.Schema.JSchema sh = Newtonsoft.Json.Schema.JSchema.Parse(Resources.compositor_schema);
            Newtonsoft.Json.Schema.JSchemaReaderSettings dd;
            
            Newtonsoft.Json.Schema.JSchemaPreloadedResolver RefResolver = new Newtonsoft.Json.Schema.JSchemaPreloadedResolver();
            RefResolver.Add(new Uri("http://vit.com.ua/edgeserver/core"), Resources.core);
            RefResolver.Add(new Uri("http://vit.com.ua/edgeserver/definitions"), Resources.definitions);
            RefResolver.Add(new Uri("http://vit.com.ua/edgeserver/drivers"), Resources.drivers);
            
            string jsonschema = Resources.compositor_schema;
            Newtonsoft.Json.Schema.JSchema schema = Newtonsoft.Json.Schema.JSchema.Parse(jsonschema, RefResolver);
            //JTokenVM token = JObjectVM.FromSchema(schema);

            return;
        }
        /*
            using (ScriptEngine engine = new ScriptEngine("{16d51579-a30b-4c8b-a276-0ff4dc41e755}"))
            {                
                ParsedScript parsed = engine.Parse(Resources.jjv);                
                Console.WriteLine(parsed.CallMethod("MyFunc", 3));
            }*/
    }
}
