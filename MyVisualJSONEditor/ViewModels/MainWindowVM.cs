using ICSharpCode.AvalonEdit.Document;
using MyToolkit.Command;
using MyToolkit.Model;
using MyVisualJSONEditor.Properties;
using MyVisualJSONEditor.Tools;
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

        private AModuleVM VM;
        private TextDocument _JsonSchemaDoc, _JsonDataDoc;
        private JObjectVM _Data;
        private JSchema _JSchema;
        private JSchemaPreloadedResolver refResolver;
        private string _SchemaError, _DataError, _ResultData;     
        private bool _IsValid, _IsResultValid;
        private ItemsControl _Control;

        public TextDocument JsonSchemaDoc
        {
            get { return _JsonSchemaDoc; }
            set
            {
                _JsonSchemaDoc = value;
                OnPropertyChanged("JsonSchemaDoc");
            }
        }

        public TextDocument JsonDataDoc
        {
            get { return _JsonDataDoc; }
            set
            {
                _JsonDataDoc = value;
                OnPropertyChanged("JsonDataDoc");
            }
        }

        public string JsonSchema
        {
            get { return JsonSchemaDoc.Text; }
            set
            {
                JsonSchemaDoc.Text = value;
                OnPropertyChanged("JsonSchemaDoc");
                OnPropertyChanged("JsonSchema");
            }

        }

        public string JsonData
        {
            get { return JsonDataDoc.Text; }
            set
            {
                JsonDataDoc.Text = value;
                OnPropertyChanged("JsonDataDoc");
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
            set
            {
                _JSchema = value;
                OnPropertyChanged("JSchema");
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
        public ObservableCollection<string> ResultValidationErrors { get; private set; }

        public ICommand JsonSchemaDocLostFocusCommand { get; set; }
        public ICommand JsonDataDocLostFocusCommand { get; set; }
        public ICommand RefreshCommand { get; set; }

        public MainWindowVM()
        {
            _JsonSchemaDoc = new TextDocument();
            _JsonDataDoc = new TextDocument();
            JsonSchemaDocLostFocusCommand = new RelayCommand(() => { OnPropertyChanged("JsonSchemaDoc"); });
            JsonDataDocLostFocusCommand = new RelayCommand(() => { OnPropertyChanged("JsonDataDoc"); });
            RefreshCommand = new RelayCommand(Refresh);

            refResolver = new JSchemaPreloadedResolver();
            JSchema coreSchema = JSchema.Parse(Resources.core);
            JSchema shDefinitions = JSchema.Parse(Resources.definitions);
            JSchema driverDefinitions = JSchema.Parse(Resources.drivers);
            refResolver.Add(coreSchema, new Uri("http://vit.com.ua/edgeserver/core"));
            refResolver.Add(shDefinitions, new Uri("http://vit.com.ua/edgeserver/definitions"));
            refResolver.Add(driverDefinitions, new Uri("http://vit.com.ua/edgeserver/drivers"));

            this.PropertyChanged += MainWindowVM_PropertyChanged;
            ValidationErrors = new ObservableCollection<string>();
            ResultValidationErrors = new ObservableCollection<string>();

            Control = new ItemsControl();
            JsonSchema = Resources.Compositor_schema;            
            JsonData = Resources.Compositor;
            VM = new CompositorVM();
            Refresh();
        }

        void Refresh()
        {
            SchemaError = null;
            Control.Items.Clear();
            try
            {
                JSchema = JSchema.Parse(JsonSchema, refResolver);
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

                if (VM != null)
                    VM.Init(Data);
            }
            else
            {
                Data = null;
                foreach (var error in validErrors)
                    ValidationErrors.Add(error);
            }

            _IsValid = isValid;
            OnPropertyChanged("DataStatusColor");
        }

        void MainWindowVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                //
            }
        }

        void ShowResult()
        {
            ResultData = Data.ToJson();
            ResultValidationErrors.Clear();
            IList<string> validErrors = null;
            _IsResultValid = JObject.Parse(ResultData).IsValid(JSchema, out validErrors);
            foreach (var error in validErrors)
                ResultValidationErrors.Add(error);
            OnPropertyChanged("ResultDataStatusColor");
        }

    }
}
