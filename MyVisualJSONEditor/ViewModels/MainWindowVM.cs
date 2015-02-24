using MyToolkit.Command;
using MyToolkit.Model;
using MyVisualJSONEditor.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyVisualJSONEditor.ViewModels
{
    public class MainWindowVM : ObservableObject
    {

        private JObjectVM _Data;
        private JSchema _JSchema;
        private JSchemaPreloadedResolver refResolver;
        private string _Schema, _SchemaError, _DataError, _ResultData, _JsonData;
        private string _DbHost;
        private bool _IsValid, _IsResultValid;
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

        public string JsonData
        {
            get { return _JsonData; }
            set
            {
                _JsonData = value;
                OnPropertyChanged("JsonData");
            }

        }

        public string ResultData
        {
            get { return _ResultData; }
            set
            {
                _ResultData = value;
                OnPropertyChanged("ResultData");
            }

        }

        public JObjectVM Data
        {
            get { return _Data;  }
            set 
            { 
                _Data = value; 
                OnPropertyChanged("Data"); 
            }
        }

        public JSchema JSchema
        {
            get { return _JSchema; }
            set { _JSchema = value; OnPropertyChanged("JSchema"); }
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
            get { return _IsValid ? Brushes.Azure : Brushes.Bisque; }
        }

        public Brush ResultDataStatusColor
        {
            get { return _IsResultValid ? Brushes.Azure : Brushes.Bisque; }
        }

        public ObservableCollection<string> ValidationErrors { get; private set; }

        public MainWindowVM()
        {
            refResolver = new JSchemaPreloadedResolver();
            JSchema shDefinitions = JSchema.Parse(Resources.definitions);
            refResolver.Add(shDefinitions, new Uri("http://vit.com.ua/edgeserver/definitions"));

            this.PropertyChanged += MainWindowVM_PropertyChanged;
            ValidationErrors = new ObservableCollection<string>();

            Control = new ItemsControl();
            Schema = Resources.MediaGrabber_schema;
            JsonData = Resources.MediaGrabber;
        }

        void MainWindowVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case("Schema"):
                case("JsonData"):
                    SchemaError = null;
                    Control.Items.Clear();
                    try
                    {
                        JSchema = JSchema.Parse(Schema, refResolver);
                    }
                    catch (Exception ex)
                    {
                        SchemaError = ex.Message;
                        return;
                    }
                    DataError = null;
                    JObject jdata = null;
                    try
                    {
                        jdata = JObject.Parse(JsonData);
                    }
                    catch (Exception ex)
                    {
                        DataError = ex.Message;
                        return;
                    }
                    IList<string> validErrors = null;
                    ValidationErrors.Clear();
                    bool isValid = jdata.IsValid(JSchema, out validErrors);
                    if (isValid)
                    {
                        Data = JObjectVM.FromJson(jdata, JSchema);
                        Data.PropertyChanged += (se, ar) =>
                        {
                            ShowResult();
                        };
                        ShowResult();
                    }
                    else
                    {
                        Data = null;
                        foreach (var error in validErrors)
                            ValidationErrors.Add(error);
                    }
                    
                    _IsValid = isValid;
                    OnPropertyChanged("DataStatusColor");
                    break;
            }
        }

        void ShowResult()
        {
            ResultData = Data.ToJson();
            _IsResultValid = JObject.Parse(ResultData).IsValid(JSchema);
            OnPropertyChanged("ResultDataStatusColor");
        }

    }
}
