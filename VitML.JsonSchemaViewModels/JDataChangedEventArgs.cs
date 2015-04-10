using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VitML.JsonVM
{
    public class JDataChangedEventArgs : EventArgs
    {

        public string Name { get; private set; }
        public object Value { get; private set; }

        public JDataChangedEventArgs(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
