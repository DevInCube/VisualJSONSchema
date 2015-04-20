using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using My.Json.Schema;
using Newtonsoft.Json.Linq;
using VitML.JsonVM.Linq;
using System.Threading;

namespace VitML.JsonVM.Tests
{
    [TestClass]
    public class JObjectVMTests
    {

        [TestMethod]
        public void Data_SetAndGetString_Matches()
        {
            JSchema schema = JSchema.Parse(@"{
    type:'object',
    properties:{
        'prop':{type:'string'}
    }
}");
            JObjectVM vm = JObjectVM.FromSchema(schema) as JObjectVM;
            vm.Data["prop"] = JValue.CreateString("test");
            Assert.AreEqual("test", vm.Data["prop"].Value<string>());
        }

        [TestMethod]
        public void Data_SetAndGetNumber_Matches()
        {
            JSchema schema = JSchema.Parse(@"{
    type:'object',
    properties:{
        'num':{type:'number'}
    }
}");
            JObjectVM vm = JObjectVM.FromSchema(schema) as JObjectVM;
            vm.Data["num"] = new JValue(5.1D);
            Assert.AreEqual(5.1D, vm.Data["num"].Value<double>());
        }

        [TestMethod]
        public void Binding_InitialArrayItems_SendNotification()
        {
            bool notified = false;
            ManualResetEvent statsUpdatedEvent = new ManualResetEvent(false);

            JObject data = JObject.Parse("{'arr':[{'prop':''}]}");
            JSchema schema = JSchema.Parse(@"{
    type:'object',
    properties:{
        'arr':{
            type:'array',
            items:{
                type:'object',
                properties:{ 'prop':{type:'string'} }
            }
        }
    }
}");
            JObjectVM vm = JObjectVM.FromJson(data, schema) as JObjectVM;

            vm.PropertyChanged += (se, ar) => {
                if (ar.PropertyName.Equals("arr[0].prop.Value"))
                {
                    JObjectVM objVM = (se as JObjectVM);
                    string newValue = objVM.Data["arr[0].prop"].Value<string>();
                    if (newValue.Equals("change"))
                    {
                        notified = true;
                        statsUpdatedEvent.Set();
                    }
                }
            };

            vm.Data["arr[0].prop"] = JValue.CreateString("change");           

            statsUpdatedEvent.WaitOne(1000, false);
            Assert.IsTrue(notified);         
        }
    }
}
