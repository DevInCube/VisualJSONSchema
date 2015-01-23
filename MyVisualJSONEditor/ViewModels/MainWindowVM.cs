using MyVisualJSONEditor.JsonSchemaNS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyVisualJSONEditor.ViewModels
{
    public class MainWindowVM : ObservableObject
    {

        private string _Schema, _Data, _SchemaError, _DataError;
        private string _DbHost;
        private bool _IsValid;
        private ItemsControl _Control;

        public string DbHost
        {
            get { return _DbHost; }
            set
            {
                _DbHost = value;
                OnPropertyChanged("DbHost");
            }

        }


        public string Schema {
            get { return _Schema; }
            set {
                _Schema = value;
                OnPropertyChanged("Schema");
            }
        
        }
        public string SchemaError
        {
            get { return _SchemaError; }
            set
            {
                _SchemaError = value;
                OnPropertyChanged("SchemaError");
            }

        }
        public string Data
        {
            get { return _Data; }
            set
            {
                _Data = value;
                OnPropertyChanged("Data");
            }

        }
        public string DataError
        {
            get { return _DataError; }
            set
            {
                _DataError = value;
                OnPropertyChanged("DataError");
            }

        }

        public ItemsControl Control
        {
            get { return _Control; }
            set
            {
                _Control = value;
                OnPropertyChanged("Control");
            }
        }

        public Brush DataStatusColor
        {
            get { return _IsValid ? Brushes.Beige : Brushes.Bisque; }
        }

        public JObject JObject { get; set; }

        public object this[string key]
        {
            get
            {
                if (JObject != null)
                    return JObject.SelectToken(key);
                return null;
            }
            set
            {
                if (JObject != null)
                {
                    JToken token = JObject.SelectToken(key);
                    if (token != null)
                    {
                        JProperty property = token.Parent as JProperty;
                        if (property != null)
                        {
                            property.Value = new JValue(value);
                            OnPropertyChanged(key);
                        }
                    }
                }
            }
        }

        public MainWindowVM()
        {
            this.PropertyChanged += MainWindowVM_PropertyChanged;
            Control = new ItemsControl();
            Schema = MyVisualJSONEditor.Properties.Resources.Schema;
            Data = MyVisualJSONEditor.Properties.Resources.Data;
        }

        void MainWindowVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case("Schema"):
                case("Data"):
                    SchemaError = null;
                    Control.Items.Clear();
                    JSchema jschema = null;
                    try
                    {
                        jschema = JSchema.Parse(Schema);
                    }
                    catch (Exception ex)
                    {
                        SchemaError = ex.Message;
                        return;
                    }
                    DataError = null;
                    JObject jdata = null;
                    dynamic dData = null;
                    try
                    {
                        jdata = JObject.Parse(Data);
                        JObject = jdata;
                        dData = JsonConvert.DeserializeObject(Data);
                    }
                    catch (Exception ex)
                    {
                        DataError = ex.Message;
                        return;
                    }
                    JSBuilder.Build(Control, jschema, dData, this);
                    bool isValid = jdata.IsValid(jschema);
                    _IsValid = isValid;
                    OnPropertyChanged("DataStatusColor");
                    break;
            }
        }

    }
}
