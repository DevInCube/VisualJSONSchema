using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using My.Json.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VitML.JsonVM.ViewModels;
using VitML.JsonVM.Schema;

namespace VitML.JsonVM.Linq
{

    public abstract class JTokenVM : ObservableObject
    {

        private JSchema originalSchema;
        private bool _HasValue;
        private JSchema _Schema;

        /// <summary>Gets or sets the schema of the token. </summary>
        public JSchema Schema
        {
            get { return _Schema;  }
            set 
            { 
                _Schema = value; 
                OnPropertyChanged("Schema");
            }
        }

        /// <summary>Gets or sets the parent list if applicable (may be null). </summary>
        public JArrayVM ParentList { get; set; }

        public bool IsRequired { get; set; }

        public bool HasValue
        {
            get { return _HasValue; }
            set { _HasValue = value; OnPropertyChanged("HasValue"); }
        }

        public List<JSchema> AlternativeSchemas { get; private set; }

        public JTokenVM()
        {
            IsRequired = true;            
        }

        public virtual void SetSchema(JSchema schema)
        {
            if (schema == null) throw new ArgumentNullException("schema");            

            this.originalSchema = schema;

            this.AlternativeSchemas = new List<JSchema>();
            foreach (var alt in originalSchema.OneOf)
                this.AlternativeSchemas.Add(alt);
            OnPropertyChanged("AlternativeSchemas");

            this.Schema = schema;
        }

        public virtual void SetData(JToken data)
        {
            if (originalSchema == null) throw new ArgumentNullException("originalSchema");

            Schema = originalSchema.CheckSchema(data);

            HasValue = !(data == null || data.Type == JTokenType.Null);
        }

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
