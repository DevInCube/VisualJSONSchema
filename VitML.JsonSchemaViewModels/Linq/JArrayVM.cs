using My.Json.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VitML.JsonVM.Linq
{

    public interface IJsonArray : IList<JToken>
    {
        //
    }

    public class JArrayVM : JTokenVM
    {

        public IJsonArray Data { get; private set; }

        public JArrayVM(JSchema schema, JToken data)
            : base(schema, data)
        {
            this.Data = new JsonArrayImpl(this);
            this.Items = new ObservableCollection<JTokenVM>();
            this.Items.CollectionChanged += Items_CollectionChanged;
            this.SelectedIndex = 0;
            this.DisplayMemberPath = "";
            this.CollectionChanged += JArrayVM_CollectionChanged;
        }

        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add
                || e.Action == NotifyCollectionChangedAction.Replace)
            {
                JTokenVM token = e.NewItems[0] as JTokenVM;
                token.ParentList = this.Items;
            }
        }

        void JArrayVM_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                var p = (KeyValuePair<string, object>)e.NewItems[0];
                OnPropertyChanged(p.Key);
            }
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
            private set
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

        public object SelectedItem
        {
            get { 
                int index = this.SelectedIndex;
                if (index > -1 && index < Items.Count)
                    return Data[index];
                return null;
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

    class JsonArrayImpl : IJsonArray
    {

        private JArrayVM vm;

        public JsonArrayImpl(JArrayVM vm)
        {
            this.vm = vm;
        }

        private JTokenVM CreateItem(JSchema schema, JToken item)
        {
            JTokenVM val = new JValueVM(schema, item);           
            val.ParentList = vm.Items;
            return val;
        }

        private JTokenVM GetItem(JToken item)
        {
            return vm.Items.FirstOrDefault(x => x["Value"] == item);
        }

        public int IndexOf(JToken item)
        {
            var it = GetItem(item);
            if (it == null) return -1;
            return vm.Items.IndexOf(it);
        }

        public void Insert(int index, JToken item)
        {
            var schema = vm.Schema.GetItemSchemaByIndex(index);
            vm.Items.Insert(index, CreateItem(schema, item));
        }

        public void RemoveAt(int index)
        {
            vm.Items.RemoveAt(index);
        }

        public JToken this[int index]
        {
            get
            {
                return vm.Items[index].Data;                
            }
            set
            {
                //vm.Items[index].Data = value;
            }
        }

        public void Add(JToken item)
        {
            int index = vm.Items.Count;
            var schema = vm.Schema.GetItemSchemaByIndex(index);

            vm.Items.Add(CreateItem(schema, item));
        }

        public void Clear()
        {
            vm.Items.Clear();
        }

        public bool Contains(JToken item)
        {
            return GetItem(item) != null;
        }

        public void CopyTo(JToken[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return vm.Items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(JToken item)
        {
            JTokenVM it = GetItem(item);
            return vm.Items.Remove(it);
        }

        public IEnumerator<JToken> GetEnumerator()
        {
            foreach (var item in vm.Items)
            {
                yield return item.Data;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
