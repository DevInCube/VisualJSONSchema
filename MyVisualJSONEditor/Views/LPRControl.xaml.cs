using My.Json.Schema;
using MyVisualJSONEditor.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VitML.JsonVM.Linq;
using VitML.JsonVM.ViewModels;

namespace MyVisualJSONEditor.Views
{
    /// <summary>
    /// Interaction logic for LPRControl.xaml
    /// </summary>
    public partial class LPRControl : UserControl
    {
        public LPRControl()
        {
            InitializeComponent();
            this.DataContextChanged += LPRControl_DataContextChanged;
        }

        void LPRControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is JTokenVM)
            {
                this.DataContext = new LPRVM(e.NewValue as JCustomVM);
            }
        }
    }

    class LPRVM : ObservableObject, IJsonDataProvider
    {

        private JCustomVM jmodel;

        public string Test
        {
            get { return "Test OK"; }
        }

        public JObjectVM ModuleVM { get; set; }
        public JObjectVM PrincipalVM { get; set; }
        public JObjectVM ParametersVM { get; set; }

        public LPRVM(JCustomVM jdata)
        {
            this.jmodel = jdata;
            jdata.SetDataProvider(this);

            JToken inData = jdata.JsonData;

            var refResolver = new JSchemaPreloadedResolver();
            refResolver.Add(new Uri("http://vit.com.ua/edgeserver/core"), Resources.core );
            refResolver.Add( new Uri("http://vit.com.ua/edgeserver/definitions"),Resources.definitions);
            refResolver.Add(new Uri("http://vit.com.ua/edgeserver/drivers"), Resources.drivers);

            JSchema moduleSchema = JSchema.Parse(Resources.LPR_Recognizer_Module_schema, refResolver);
            ModuleVM = JObjectVM.FromJson(inData, moduleSchema) as JObjectVM;

            JSchema principalSchema = JSchema.Parse(Resources.LPR_Recognizer_Principal_schema, refResolver);
            PrincipalVM = JObjectVM.FromJson(inData, principalSchema) as JObjectVM;

            ModuleVM.PropertyChanged += ModuleVM_PropertyChanged;
            PrincipalVM.PropertyChanged += ModuleVM_PropertyChanged;
            this.PropertyChanged += LPRVM_PropertyChanged;
        }

        void ModuleVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged("");
        }

        void LPRVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            jmodel.Notify();
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

            return result; 
        }
    }
}
