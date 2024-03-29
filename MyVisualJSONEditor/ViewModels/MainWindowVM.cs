﻿using ICSharpCode.AvalonEdit.Document;
using MyToolkit.Command;
using MyVisualJSONEditor.Properties;
using MyVisualJSONEditor.ViewModels.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using My.Json.Schema;
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
using VitML.JsonVM.Linq;
using VitML.JsonVM.ViewModels;

namespace MyVisualJSONEditor.ViewModels
{
    public class MainWindowVM : ObservableObject
    {

        private AModuleVM _SelectedModule;
        private TextDocument _JsonSchemaDoc, _JsonDataDoc, _ResultDataDoc;
        private JObjectVM _Data;
        private JSchema _JSchema;
        private JSchemaPreloadedResolver refResolver;
        private string _SchemaError, _DataError, _ResultData;     
        private bool _IsValid, _IsResultValid;
        private ItemsControl _Control;

        public TextDocument JsonSchemaDoc
        {
            get { return _JsonSchemaDoc; }
            set => SetProperty(ref _JsonSchemaDoc, value);
        }

        public TextDocument JsonDataDoc
        {
            get { return _JsonDataDoc; }
            set => SetProperty(ref _JsonDataDoc, value);
        }

        public TextDocument ResultDataDoc
        {
            get { return _ResultDataDoc; }
            set => SetProperty(ref _ResultDataDoc, value);
        }

        public string JsonSchema
        {
            get { return JsonSchemaDoc.Text; }
            set
            {
                JsonSchemaDoc.Text = value;
                OnPropertyChanged(nameof(JsonSchemaDoc));
                OnPropertyChanged(nameof(JsonSchema));
            }

        }

        public string JsonData
        {
            get { return JsonDataDoc.Text; }
            set
            {
                JsonDataDoc.Text = value;
                OnPropertyChanged(nameof(JsonDataDoc));
                OnPropertyChanged(nameof(JsonData));
            }

        }

        public JObjectVM Data
        {
            get { return _Data;  }
            set => SetProperty(ref _Data, value);
        }

        public JSchema JSchema
        {
            get { return _JSchema; }
            set => SetProperty(ref _JSchema, value);
        }

        public string SchemaError
        {
            get { return _SchemaError; }
            set => SetProperty(ref _SchemaError, value);
        }
     
        public string DataError
        {
            get { return _DataError; }
            set => SetProperty(ref _DataError, value);
        }

        public ItemsControl Control
        {
            get { return _Control; }
            set => SetProperty(ref _Control, value);
        }

        public AModuleVM SelectedModule
        {
            get { return _SelectedModule; }
            set
            {
                if (SetProperty(ref _SelectedModule, value))
                {
                    JsonSchema = _SelectedModule.Schema;
                    JsonData = _SelectedModule.Data;
                    Refresh();
                }
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
        public ObservableCollection<AModuleVM> Modules { get; private set; }

        public ICommand JsonSchemaDocLostFocusCommand { get; set; }
        public ICommand JsonDataDocLostFocusCommand { get; set; }
        public ICommand RefreshCommand { get; set; }

        public MainWindowVM()
        {
            _JsonSchemaDoc = new TextDocument();
            _JsonDataDoc = new TextDocument();
            JsonSchemaDocLostFocusCommand = new RelayCommand(() => { OnPropertyChanged(nameof(JsonSchemaDoc)); });
            JsonDataDocLostFocusCommand = new RelayCommand(() => { OnPropertyChanged(nameof(JsonDataDoc)); });
            RefreshCommand = new RelayCommand(Refresh);

            refResolver = new JSchemaPreloadedResolver();
            refResolver.Add(new Uri("http://vit.com.ua/edgeserver/core"), Resources.core );
            refResolver.Add( new Uri("http://vit.com.ua/edgeserver/definitions"),Resources.definitions);
            refResolver.Add(new Uri("http://vit.com.ua/edgeserver/drivers"), Resources.drivers);
            refResolver.Add(new Uri("http://vit.com.ua/edgeserver/radar.drivers"), Resources.Radar_drivers_schema);
            refResolver.Add(new Uri("http://vit.com.ua/edgeserver/lpr.recognizer.module"), Resources.LPR_Recognizer_Module_schema);
            refResolver.Add(new Uri("http://vit.com.ua/edgeserver/lpr.recognizer.principal"), Resources.LPR_Recognizer_Principal_schema);
            refResolver.Add(new Uri("http://vit.com.ua/edgeserver/lpr.recognizer.parameters"), Resources.LPR_Recognizer_Parameters_schema);

            this.PropertyChanged += MainWindowVM_PropertyChanged;
            ValidationErrors = new ObservableCollection<string>();
            ResultValidationErrors = new ObservableCollection<string>();

            Control = new ItemsControl();

            Modules = new ObservableCollection<AModuleVM>();
            Modules.Add(new TestVM());
            Modules.Add(new CompositorVM());
            Modules.Add(new MediaGrabberVM());
            Modules.Add(new EventStoreVM());
            Modules.Add(new LPRVM());
            Modules.Add(new RadarVM());
            Modules.Add(new GrabControlVM());
            Modules.Add(new SpeedControlVM());
            Modules.Add(new OverlayServerVM());
            Modules.Add(new OverlayRenderVM());
            Modules.Add(new TrackerVM());

            SelectedModule = Modules.FirstOrDefault();
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
            IList<string> validErrors = new List<string>();
            ValidationErrors.Clear();
            bool isValid = jdata.IsValid(JSchema, out validErrors);
            if (isValid)
            {
                Data = JObjectVM.FromJson(jdata, JSchema) as JObjectVM;
                Data.PropertyChanged += (se, ar) =>
                {
                    ShowResult();
                };
                ShowResult();

                if (SelectedModule != null)
                    SelectedModule.Init(Data);
            }
            else
            {
                Data = null;
                foreach (var error in validErrors)
                    ValidationErrors.Add(error);
            }

            _IsValid = isValid;
            OnPropertyChanged(nameof(DataStatusColor));
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
            ResultDataDoc = new TextDocument();
            string result = Data.ToJson();
            ResultDataDoc.Text = result;        
            ResultValidationErrors.Clear();
            IList<string> validErrors = null;
            _IsResultValid = JObject.Parse(result).IsValid(JSchema, out validErrors);
            foreach (var error in validErrors)
                ResultValidationErrors.Add(error);
            OnPropertyChanged(nameof(ResultDataStatusColor));
        }

    }
}
