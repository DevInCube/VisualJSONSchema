﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VitML.JsonSchemaControlBuilder.ViewModels.JSchemaViewModels
{
    public interface IStyleDecorator
    {
        bool IsReadonly { get; }
    }
}