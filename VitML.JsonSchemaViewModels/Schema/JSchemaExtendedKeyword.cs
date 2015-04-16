using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VitML.JsonVM.Schema
{
    public static class JSchemaExtendedKeywords
    {
        public const string Visible = "visible";
        public const string Readonly = "readonly";
        public const string Ignore = "ignore";
        public const string Expanded = "expanded";        

        public static class Style
        {
            public const string Key = "Style";

            public const string Width = "Width";
            public const string MinWidth = "MinWidth";
            public const string MaxWidth = "MaxWidth";
            public const string Height = "Height";
            public const string MinHeight = "MinHeight";
            public const string MaxHeight = "MaxHeight";
            public const string DisplayMemberPath = "DisplayMemberPath";
            public const string ShowCount = "ShowCount";         
        }
    }
}
