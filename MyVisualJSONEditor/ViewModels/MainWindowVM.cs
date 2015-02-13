using MyToolkit.Command;
using MyVisualJSONEditor.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

        public class Post
        {
            public string PostId { get; set; }
            public string PostName { get; set; }

            public override bool Equals(object obj)
            {
                if (this == obj) return true;
                Post o = obj as Post;
                if (o == null) return false;
                return PostId.Equals(o.PostId) && PostName.Equals(o.PostName);
            }
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
            Schema = MyVisualJSONEditor.Properties.Resources.Schema;
            JsonData = MyVisualJSONEditor.Properties.Resources.Data;
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
                        JSchema = JSchema.Parse(Schema);
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
                    bool isValid = jdata.IsValid(JSchema);
                    if (isValid)
                    {
                        Data = JObjectVM.FromJson(jdata, JSchema);
                        Data2 = Data;
                        JObjectVM ps = Data.Data["Store"] as JObjectVM;
                        ps.PropertyChanged += (AccessKeyEvent, o) =>
                        {
                            if(o.PropertyName=="DbPort")
                                return;
                        };
                        string ss = Data.Data["Store.ParamSet.PostName"] as string;
                        JObjectVM paramSet = Data.GetValue<JObjectVM>("Store.ParamSet");
                        
                        JArrayVM posts = Data.Data["Store.ParamSet.Posts"] as JArrayVM;
                       
                        var objjj = Data.Data["FileApi.AddFactReact[0].ParamSet"];
                        posts.Data.Add(new Post()
                        {
                            PostName = Data.Data["Store.ParamSet.PostName"] as string,
                            PostId = Data.Data["Store.ParamSet.Post"] as string,
                        });
                        posts.SelectedIndex = 0;
                        posts.PropertyChanged += (sss, ee) =>
                        {
                            if (ee.PropertyName == "SelectedIndex")
                            {
                                int index = posts.SelectedIndex;
                                if (index > -1 && index < posts.Data.Count)
                                {
                                    Post post = posts.Data[index] as Post;
                                    Data.Data["Store.ParamSet.PostName"] = post.PostName;
                                    Data.Data["Store.ParamSet.Post"] = post.PostId;
                                }
                            }
                        };
                        paramSet.GetProperty("ConnectBtn").Command = new RelayCommand(() =>
                        {
                            var postList = new List<Post>()
                            {
                                new Post() { PostName = "1", PostId = "idddd" },
                                new Post() { PostName = "2asdasds", PostId = "22222" },
                                new Post() { PostName = "EdgeServer", PostId = "69e86fa7-e1ad-4e68-96b7-b910f40bdb49" }
                            };
                            var selItem = posts.SelectedItem;
                            var selIndex = 0;
                            if (selItem != null)
                            {
                                var item = postList.FirstOrDefault(x => x.Equals(selItem));
                                if (item != null)
                                    selIndex = postList.IndexOf(item);
                            }
                            posts.Data.Clear();
                            foreach (var post in postList)
                                posts.Data.Add(post);
                            posts.SelectedIndex = selIndex;
                        });
                        Data.PropertyChanged += (se, ar) => {
                            ShowResult();
                        };
                        ShowResult();
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

        public static JObjectVM Data2;

        void ShowResult()
        {
            ResultData = Data.ToJson();
            _IsResultValid = JObject.Parse(ResultData).IsValid(JSchema);
            OnPropertyChanged("ResultDataStatusColor");
        }

    }
}
