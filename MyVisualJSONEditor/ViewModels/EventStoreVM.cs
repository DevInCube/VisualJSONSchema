using MyToolkit.Command;
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

    public class EventStoreVM : ObservableObject
    {

        public JObjectVM Data { get; set; }

        public EventStoreVM()
        {

            JObjectVM ps = Data.Data["Store"] as JObjectVM;
            ps.PropertyChanged += (AccessKeyEvent, o) =>
            {
                if (o.PropertyName == "DbPort")
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
    }
}
