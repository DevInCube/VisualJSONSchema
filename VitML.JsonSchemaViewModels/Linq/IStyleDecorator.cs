using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VitML.JsonVM.Common;

namespace VitML.JsonVM.Linq
{
    public interface IStyleDecorator
    {

        bool IsReadonly { get; }
        bool IsVisible { get; }
        bool Ignore { get; }
        PropertyStyle Style { get; }
    }
}
