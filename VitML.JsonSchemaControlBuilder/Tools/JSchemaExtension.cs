using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVisualJSONEditor.Tools
{
    public static class JSchemaExtension
    {

        public static bool HasFlag(this JSchemaType? nType, JSchemaType flag)
        {
            if (nType == null) return false;
            JSchemaType type = (JSchemaType)nType;
            return type.HasFlag(flag);
        }

        public static bool IsRequired(this JSchema sh, string key)
        {
            return sh.Required.FirstOrDefault(x => x.Equals(key)) != null;
        }

    }
}
