using MyToolkit.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVisualJSONEditor.ViewModels
{

    public abstract class JTokenVM : ObservableDictionary<string, object>
    {
        /// <summary>Gets or sets the schema of the token. </summary>
        public JSchema Schema { get; set; }

        /// <summary>Gets or sets the parent list if applicable (may be null). </summary>
        public ObservableCollection<JTokenVM> ParentList { get; set; }

        /// <summary>Converts the token to a JSON string. </summary>
        /// <returns>The JSON string. </returns>
        public string ToJson()
        {
            var token = ToJToken();
            return JsonConvert.SerializeObject(token, Formatting.Indented);
        }

        /// <summary>Converts the <see cref="JsonTokenModel"/> to a <see cref="JToken"/>. </summary>
        /// <returns>The <see cref="JToken"/>. </returns>
        public abstract JToken ToJToken();
    }
}
