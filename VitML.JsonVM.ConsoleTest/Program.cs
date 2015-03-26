using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VitML.JsonVM.Linq;

namespace VitML.JsonVM.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {

            JSchemaPreloadedResolver RefResolver = new JSchemaPreloadedResolver();
            RefResolver.Add(new Uri("http://vit.com.ua/edgeserver/core"), Resources.core);
            RefResolver.Add(new Uri("http://vit.com.ua/edgeserver/definitions"), Resources.definitions);
            RefResolver.Add(new Uri("http://vit.com.ua/edgeserver/drivers"), Resources.drivers);

            string jsonschema = Resources.compositor_schema;
            JSchema schema = JSchema.Parse(jsonschema, RefResolver);
            JTokenVM token = JObjectVM.FromSchema(schema);
            return;
        }
    }
}
