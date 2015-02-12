using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVisualJSONEditor.ViewModels
{
    public class JArrayVM : JTokenVM
    {

        public JArrayVM()
        {
            this.Items = new ObservableCollection<JTokenVM>();
            this.SelectedIndex = 0;
            this.DisplayMemberPath = "";
        }

        public override JToken ToJToken()
        {
            var jArray = new JArray();
            foreach (var item in Items)
                jArray.Add(item.ToJToken());
            return jArray;
        }

        public ObservableCollection<JTokenVM> Items
        {
            get { return ContainsKey("Value") ? this["Value"] as ObservableCollection<JTokenVM> : null; }
            set
            {
                this["Value"] = value;
                OnPropertyChanged("Items");
            }
        }

        public int SelectedIndex
        {
            get { return ContainsKey("SelectedIndex") ? int.Parse(this["SelectedIndex"].ToString()) : 0; }
            set
            {
                this["SelectedIndex"] = value;
                OnPropertyChanged("SelectedIndex");
            }
        }

        public string DisplayMemberPath
        {
            get { return "Value" + (ContainsKey("DisplayMemberPath") ? "." + this["DisplayMemberPath"] : ""); }
            set
            {
                this["DisplayMemberPath"] = value;
                OnPropertyChanged("DisplayMemberPath");
            }
        }

    }
}
