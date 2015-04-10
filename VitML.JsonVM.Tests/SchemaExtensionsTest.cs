using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using My.Json.Schema;
using Newtonsoft.Json.Linq;

namespace VitML.JsonVM.Tests
{
    [TestClass]
    public class SchemaExtensionsTest
    {

        [TestMethod]
        public void JSchema_GenerateFromEmpty_CreatesNull()
        {
            JSchema sh = JSchema.Parse("{}");
            var data = sh.GenerateData();

            Assert.AreEqual(null, data);
        }

        [TestMethod]
        public void JSchema_GenerateString_OK()
        {
            JSchema sh = JSchema.Parse("{'type':'string'}");
            var data = sh.GenerateData();

            Assert.AreEqual(new JValue(""), data);
        }

        [TestMethod]
        public void JSchema_GenerateFloat_OK()
        {
            JSchema sh = JSchema.Parse("{'type':'number'}");
            var data = sh.GenerateData();

            Assert.AreEqual(new JValue(0F), data);
        }

        [TestMethod]
        public void JSchema_GenerateSimpleObject_OK()
        {
            JSchema sh = JSchema.Parse("{'type':'object'}");
            var data = sh.GenerateData();

            Assert.IsTrue(JToken.DeepEquals(new JObject(), data));
        }

        [TestMethod]
        public void JSchema_GenerateSimpleObjectOrNull_CreateNull()
        {
            JSchema sh = JSchema.Parse("{'type':['object','null']}");
            var data = sh.GenerateData();

            Assert.IsTrue(JToken.DeepEquals(JValue.CreateNull(), data));
        }

        [TestMethod]
        public void JSchema_GenerateObjectWithProperty_OK()
        {
            JSchema sh = JSchema.Parse(@"{
    type:'object',
    properties:{
        'foo':{type:'string'},
        'bar':{type:'integer'},
    },
    required:['foo'],
}");
            var data = sh.GenerateData();

            Assert.IsTrue(JToken.DeepEquals(JObject.Parse("{foo:''}"), data));
        }

        [TestMethod]
        public void JSchema_GenerateEmptyArray_OK()
        {
            JSchema sh = JSchema.Parse(@"{type:'array'}");
            var data = sh.GenerateData();

            Assert.IsTrue(JToken.DeepEquals(new JArray(), data));
        }

        [TestMethod]
        public void JSchema_GenerateArrayWithItemsSchemaAndMinItems_OK()
        {
            JSchema sh = JSchema.Parse(@"{
    type:'array',
    items:{
        type:'string',
        'default':'foo',
    },
    minItems:2,    
}");
            var data = sh.GenerateData();

            Assert.IsTrue(JToken.DeepEquals(JToken.Parse("['foo','foo']"), data));
        }

        [TestMethod]
        public void JSchema_GenerateFromOneOf_GeneratesFirstSchema()
        {
            JSchema sh = JSchema.Parse(@"{
    oneOf:[{type:'string'},{'type':'integer'}]  
}");
            var data = sh.GenerateData();

            Assert.IsTrue(JToken.DeepEquals(JToken.Parse("''"), data));
        }

        [TestMethod]
        public void JSchema_GenerateAnyOf_GeneratesFirstSchema()
        {
            JSchema sh = JSchema.Parse(@"{
    anyOf:[{type:'string'},{'type':'integer'}]  
}");
            var data = sh.GenerateData();

            Assert.IsTrue(JToken.DeepEquals(JToken.Parse("''"), data));
        }

        [TestMethod]
        public void JSchema_GenerateAllOf_GeneratesFromCompositeSchema()
        {
            JSchema sh = JSchema.Parse(@"{
    allOf:[{type:'string'},{default:'foobar'}]  
}");
            var data = sh.GenerateData();

            Assert.IsTrue(JToken.DeepEquals(JToken.Parse("'foobar'"), data));
        }
    }
}
