using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VitML.JsonVM.Linq
{
    public interface IJsonDataProvider
    {

        JToken Data { get; }
    }
}
