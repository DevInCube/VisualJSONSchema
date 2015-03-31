using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VitML.Json.Schema
{
    // Summary:
    //     The value types allowed by the Newtonsoft.Json.Schema.JSchema.
    [Flags]
    public enum JSchemaType
    {
        // Summary:
        //     No type specified.
        None = 0,
        //
        // Summary:
        //     String type.
        String = 1,
        //
        // Summary:
        //     Number type.
        Number = 2,
        //
        // Summary:
        //     Integer type.
        Integer = 4,
        //
        // Summary:
        //     Boolean type.
        Boolean = 8,
        //
        // Summary:
        //     Object type.
        Object = 16,
        //
        // Summary:
        //     Array type.
        Array = 32,
        //
        // Summary:
        //     Null type.
        Null = 64,
    }
}
