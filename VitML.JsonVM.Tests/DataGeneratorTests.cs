﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using My.Json.Schema;
using VitML.JsonVM.Generation;
using Newtonsoft.Json.Linq;

namespace VitML.JsonVM.Tests
{
    [TestClass]
    public class DataGeneratorTests
    {
        [TestMethod]
        public void Generate_UnforcedObject_CreatesJNull()
        {
            JSchema sh = JSchema.Parse(@"{type:['null','object']}");

            DataGenerator gen = new DataGenerator(sh);
            DataGenerationSettings set = new DataGenerationSettings();
            set.Force = false;

            JToken res = gen.Generate(set);

            Assert.IsTrue(JToken.DeepEquals(JValue.CreateNull(), res));
        }

        [TestMethod]
        public void Generate_ForcedObject_CreatesJNull()
        {
            JSchema sh = JSchema.Parse(@"{type:['null','object']}");

            DataGenerator gen = new DataGenerator(sh);
            DataGenerationSettings set = new DataGenerationSettings();
            set.Force = true;

            JToken res = gen.Generate(set);

            Assert.IsTrue(JToken.DeepEquals(new JObject(), res));
        }

        [TestMethod]
        public void Generate_OnlyRequired_CreatesOnlyRequiredProps()
        {
            JSchema sh = JSchema.Parse(@"{
    type:'object',
    properties:{
        u:{type:'string'},
        r:{type:'string'}
    },
    required:['r'],
}");

            DataGenerator gen = new DataGenerator(sh);
            DataGenerationSettings set = new DataGenerationSettings();
            set.RequiredOnly = true;

            JToken res = gen.Generate(set);

            Assert.IsTrue(JToken.DeepEquals(JObject.Parse("{r:''}"), res));
        }
        [TestMethod]
        public void Generate_NotOnlyRequired_CreatesAllProps()
        {
            JSchema sh = JSchema.Parse(@"{
    type:'object',
    properties:{
        u:{type:'string'},
        r:{type:'string'}
    },
    required:['r'],
}");

            DataGenerator gen = new DataGenerator(sh);
            DataGenerationSettings set = new DataGenerationSettings();
            set.RequiredOnly = false;

            JToken res = gen.Generate(set);

            Assert.IsTrue(JToken.DeepEquals(JObject.Parse("{u:'',r:''}"), res));
        }

        [TestMethod]
        public void Generate_NotOnlyRequiredNested_CreatesAllPropsAndInNestedObject()
        {
            JSchema sh = JSchema.Parse(@"{
    type:'object',
    properties:{
        u:{type:'string'},
        r:{type:'string'},
        nested:{type:'object',
            properties:{
                u:{type:'string'},
            },
        }
    },
    required:['r'],
}");

            DataGenerator gen = new DataGenerator(sh);
            DataGenerationSettings set = new DataGenerationSettings();
            set.RequiredOnly = false;

            JToken res = gen.Generate(set);

            Assert.IsTrue(JToken.DeepEquals(JObject.Parse("{u:'',r:'', nested:{u:''}}"), res));
        }

        [TestMethod]
        public void Generate_CreateMinItems_CreatesMinItems()
        {
            JSchema sh = JSchema.Parse(@"{
    type:'array',
    items:{ type:'string' },
    minItems:2,    
}");

            DataGenerator gen = new DataGenerator(sh);
            DataGenerationSettings set = new DataGenerationSettings();
            set.CreateMinItems = true;

            JToken res = gen.Generate(set);

            Assert.IsTrue(JToken.DeepEquals(JToken.Parse("['','']"), res));
        }

        [TestMethod]
        public void Generate_DontCreateMinItems_CreatesEmptyArray()
        {
            JSchema sh = JSchema.Parse(@"{
    type:'array',
    items:{ type:'string' },
    minItems:2,    
}");

            DataGenerator gen = new DataGenerator(sh);
            DataGenerationSettings set = new DataGenerationSettings();
            set.CreateMinItems = false;

            JToken res = gen.Generate(set);

            Assert.IsTrue(JToken.DeepEquals(JToken.Parse("[]"), res));
        }
    }
}
