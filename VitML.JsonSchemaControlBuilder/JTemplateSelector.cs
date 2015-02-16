using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VitML.JsonSchemaControlBuilder
{
    public class JTemplateSelector
    {

        public virtual Control SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }
    }
}
