using MyToolkit.Command;
using MyVisualJSONEditor.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyVisualJSONEditor.ViewModels
{
    public class MainWindowVM : ObservableObject
    {

        private JObjectVM _Data;
        private string _Schema, _SchemaError, _DataError, _ResultData, _JsonData;
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
            set { _Data = value; OnPropertyChanged("Data"); }
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
            get { return _IsValid ? Brushes.Beige : Brushes.Bisque; }
        }

        

        public class Post
        {
            public string PostId { get; set; }
            public string PostName { get; set; }
        }
        private Post _SelectedPost;
        public ObservableCollection<Post> Posts { get; private set; }
        public Post SelectedPost
        {
            get { return _SelectedPost; }
            set
            {
                _SelectedPost = value;
                //@todo
                //if (_SelectedPost != null)
                    //this["Store.ParamSet.PostName"] = _SelectedPost.PostName;
                OnPropertyChanged("SelectedPost");
            }
        }

        public MainWindowVM()
        {
            Posts = new ObservableCollection<Post>();
            Posts.Add(new Post() { PostName = "Post1", PostId = "1"});
            Posts.Add(new Post() { PostName = "Post12", PostId = "12" });
            Posts.Add(new Post() { PostName = "Post123", PostId = "123" });
            this.PropertyChanged += MainWindowVM_PropertyChanged;
            Control = new ItemsControl();
            Schema = MyVisualJSONEditor.Properties.Resources.TestSchema;
            JsonData = MyVisualJSONEditor.Properties.Resources.TestData;
        }

        void MainWindowVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case("Schema"):
                case("JsonData"):
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
                    try
                    {
                        jdata = JObject.Parse(JsonData);
                    }
                    catch (Exception ex)
                    {
                        DataError = ex.Message;
                        return;
                    }
                    bool isValid = jdata.IsValid(jschema);
                    if (isValid)
                    {
                        Data = JObjectVM.FromJson(jdata, jschema);
                        Data.PropertyChanged += (se, ar) => { ResultData = Data.ToJson(); };
                    }
                    else
                    {
                        Data = null;
                    }
                    _IsValid = isValid;
                    OnPropertyChanged("DataStatusColor");
                    break;
            }
        }

    }
}
