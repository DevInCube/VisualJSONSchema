using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VitML.JsonVM.Generation
{
    public class DataGenerationSettings
    {

        public bool Force { get; set; }
        public bool RequiredOnly { get; set; }
        public bool CreateMinItems { get; set; }

        public DataGenerationSettings()
        {
            Force = false;
            RequiredOnly = true;
            CreateMinItems = true;
        }
    }
}
