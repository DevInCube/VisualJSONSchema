using MyToolkit.Command;
using MyVisualJSONEditor.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVisualJSONEditor.ViewModels
{

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

    public class EventStoreVM : AModuleVM
    {

        public JObjectVM VM { get; private set; }

        public EventStoreVM()
        {
            this.Schema = Resources.EventStore_schema;
            this.Data = Resources.EventStore;
        }

        public IEnumerable<Post> LoadPosts()
        {
            var postList = new List<Post>()
            {
                new Post() { PostName = "1", PostId = "idddd" },
                new Post() { PostName = "2asdasds", PostId = "22222" },
                new Post() { PostName = "EdgeServer", PostId = "69e86fa7-e1ad-4e68-96b7-b910f40bdb49" }
            };
            return postList;
        }

        public override void Init(JObjectVM vm)
        {
            this.VM = vm;
            JObjectVM ps = VM.Data["Store"] as JObjectVM;
            ps.PropertyChanged += (AccessKeyEvent, o) =>
            {
                if (o.PropertyName == "DbPort")
                    return;
            };
            string ss = VM.Data["Store.ParamSet.PostName"] as string;
            JObjectVM paramSet = VM.GetValue<JObjectVM>("Store.ParamSet");

            JArrayVM posts = VM.Data["Store.ParamSet.Posts"] as JArrayVM;

            var objjj = VM.Data["FileApi.AddFactReact[0].ParamSet"];
            posts.Data.Add(new Post()
            {
                PostName = VM.Data["Store.ParamSet.PostName"] as string,
                PostId = VM.Data["Store.ParamSet.Post"] as string,
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
                        VM.Data["Store.ParamSet.PostName"] = post.PostName;
                        VM.Data["Store.ParamSet.Post"] = post.PostId;
                    }
                }
            };
            VM.GetProperty("Store.ParamSet.ConnectBtn").Command = new RelayCommand(() =>
            {
                var postList = LoadPosts().ToList();
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
        }
    }
}
