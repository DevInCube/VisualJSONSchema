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
                    JToken newvalue = (objVM.GetToken("arr[0].prop") as JValueVM).Value;
                    if(newvalue.Value<string>().Equals("change"))
                    {
                        notified = true;
                        statsUpdatedEvent.Set();
                    }
                }
            };

            JObjectVM obj = vm.GetValue<JObjectVM>("arr[0]");
            obj.SetValue("prop", JValue.CreateString("change"));

            statsUpdatedEvent.WaitOne(1000, false);
            Assert.IsTrue(notified);         
        }
    }
}
