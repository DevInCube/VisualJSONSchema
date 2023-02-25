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
using VitML.JsonVM.Common;

namespace VitML.JsonVM.Linq
{

    public abstract class JTokenVM : ObservableObject
    {

        private JSchema originalSchema;
        private bool _HasValue;
        private bool _IsSpecified;
        private JSchema _Schema;
        private PropertyStyle _Style;
        private string _KeyTitle;
        private bool _IsNullable;        

        public string KeyTitle
        {
            get { return _KeyTitle; }
        }

        public PropertyStyle Style 
        { 
            get { return _Style; }
            private set => SetProperty(ref _Style, value);
        }

        /// <summary>Gets or sets the schema of the token. </summary>
        public JSchema Schema
        {
            get { return _Schema;  }
            set 
            {
                if (SetProperty(ref _Schema, value))
                {
                    if (_Schema.ExtensionData.ContainsKey(JSchemaExtendedKeywords.Style.Key))
                    {
                        var ext = _Schema.ExtensionData[JSchemaExtendedKeywords.Style.Key];
                        Style = PropertyStyle.Parse(ext);
                    }
                }
            }
        }

        /// <summary>Gets or sets the parent list if applicable (may be null). </summary>
        public JArrayVM ParentList { get; set; }

        public bool IsRequired { get; set; }

        public bool HasValue
        {
            get { return _HasValue; }
            set => SetProperty(ref _HasValue, value);
        }

        public bool IsSpecified
        {
            get { return _IsSpecified; }
            set => SetProperty(ref _IsSpecified, value);
        }

        public bool IsNullable
        {
            get { return _IsNullable; }
            set => SetProperty(ref _IsNullable, value);
        }

        public List<JSchema> AlternativeSchemas { get; private set; }

        public virtual string DisplayMemberPath { get { return "<>"; } }

        public JTokenVM()
        {
            IsRequired = true;
            IsSpecified = true;
        }

        public virtual void SetSchema(JSchema schema)
        {
            if (schema == null) throw new ArgumentNullException("schema");            

            this.originalSchema = schema;

            this.AlternativeSchemas = new List<JSchema>();
            foreach (var alt in originalSchema.OneOf)
                this.AlternativeSchemas.Add(alt);
            OnPropertyChanged(nameof(AlternativeSchemas));

            this.Schema = schema;

            _KeyTitle = Schema.Title;
            IsNullable = Schema.Type.HasFlag(JSchemaType.Null);            
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
