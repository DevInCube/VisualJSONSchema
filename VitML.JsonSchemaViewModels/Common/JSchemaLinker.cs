using My.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VitML.JsonVM.Common
{
    public class JSchemaLinker
    {

        private JSchemaPreloadedResolver resolver;

        internal JSchemaPreloadedResolver Resolver { get { return resolver; } }

        public JSchemaLinker() 
        {
            resolver = new JSchemaPreloadedResolver();
        }

        public void Add(string jRef, string jschema)
        {
            resolver.Add(new Uri(jRef), jschema);
        }
    }
}
