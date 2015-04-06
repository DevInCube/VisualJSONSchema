using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VitML.JsonVM.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {

            string shStr = @"{
    'definitions' : {
        'test' : {
            'type' : 'string',
        }
    },
    'type':'object',
    'properties': {
        'foo': {'type': 'string'}
    },
    'additionalProperties': false,    
    'oneOf' : [{'$ref':'#/definitions/test'}],
}";
            var sh1 = JSchema.Parse(shStr);
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
