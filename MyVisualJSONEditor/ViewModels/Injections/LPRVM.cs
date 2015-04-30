using My.Json.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VitML.JsonVM.Linq;
using VitML.JsonVM;
using MyVisualJSONEditor.Properties;
using VitML.JsonVM.ViewModels;

namespace MyVisualJSONEditor.ViewModels.Injections
{
    class LPRVM : ObservableObject, IJsonDataProvider
    {

        private JCustomVM jmodel;

        public JObjectVM ModuleVM { get; set; }
        public JObjectVM PrincipalVM { get; set; }
        public JObjectVM ParametersVM { get; set; }

        public LPRVM(JCustomVM jdata)
        {
            this.jmodel = jdata;
            jdata.SetDataProvider(this);

            JToken inData = jdata.JsonData;

            var refResolver = new JSchemaPreloadedResolver();
            refResolver.Add(new Uri("http://vit.com.ua/edgeserver/core"), Resources.core);
            refResolver.Add(new Uri("http://vit.com.ua/edgeserver/definitions"), Resources.definitions);
            refResolver.Add(new Uri("http://vit.com.ua/edgeserver/drivers"), Resources.drivers);
            refResolver.Add(new Uri("http://vit.com.ua/edgeserver/radar.drivers"), Resources.Radar_drivers_schema);

            JSchema moduleSchema = JSchema.Parse(Resources.LPR_Recognizer_Module_schema, refResolver);
            ModuleVM = JObjectVM.FromJson(inData, moduleSchema) as JObjectVM;

            JSchema principalSchema = JSchema.Parse(Resources.LPR_Recognizer_Principal_schema, refResolver);
            PrincipalVM = JObjectVM.FromJson(inData, principalSchema) as JObjectVM;

            JSchema parametersSchema = JSchema.Parse(Resources.LPR_Recognizer_Parameters_schema, refResolver);
            ParametersVM = JObjectVM.FromJson(inData, parametersSchema) as JObjectVM;

            ModuleVM.PropertyChanged += ModuleVM_PropertyChanged;
            PrincipalVM.PropertyChanged += ModuleVM_PropertyChanged;
            ParametersVM.PropertyChanged += ModuleVM_PropertyChanged;
            this.PropertyChanged += LPRVM_PropertyChanged;
        }

        void ModuleVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e.PropertyName);
        }

        void LPRVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            jmodel.Notify(e.PropertyName);
        }

        public Newtonsoft.Json.Linq.JToken Data
        {
            get { return GenerateData(); }
        }

        private JToken GenerateData()
        {
            JObject result = new JObject();

            JObject module = ModuleVM.ToJToken() as JObject;
            if (module != null)
            {
                foreach (var prop in module.Properties())
                    result.Add(prop.Name, prop.Value);
            }
            JObject principal = PrincipalVM.ToJToken() as JObject;
            if (principal != null)
            {
                foreach (var prop in principal.Properties())
                    result.Add(prop.Name, prop.Value);
            }
            JObject parameters = ParametersVM.ToJToken() as JObject;
            if (parameters != null)
            {
                foreach (var prop in parameters.Properties())
                    result.Add(prop.Name, prop.Value);
            }

            return result;
        }
    }
}
