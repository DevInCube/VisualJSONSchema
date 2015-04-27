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
                this.DataContext = new LPRVM(e.NewValue as JCustomObjectVM);
            }
        }
    }

    class LPRVM : ObservableObject, IJsonDataProvider
    {

        private JCustomObjectVM jmodel;

        public string Test
        {
            get { return "Test OK"; }
        }

        public JObjectVM ModuleVM { get; set; }
        public JObjectVM PrincipalVM { get; set; }
        public JObjectVM ParametersVM { get; set; }

        public LPRVM(JCustomObjectVM jdata)
        {
            this.jmodel = jdata;
            jdata.SetDataProvider(this);

            JSchema moduleSchema = JSchema.Parse(Resources.LPR_Recognizer_Module_schema);
            ModuleVM = JObjectVM.FromSchema(moduleSchema) as JObjectVM;

            ModuleVM.PropertyChanged += ModuleVM_PropertyChanged;
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
            return ModuleVM.ToJToken();
        }
    }
}
