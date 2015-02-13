﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVisualJSONEditor.ViewModels
{

    public interface IJsonArray : IList<object>
    {
        //
    }

    public class JArrayVM : JTokenVM
    {

        public IJsonArray Data { get; private set; }

        public JArrayVM()
        {
            this.Data = new JsonArrayImpl(this);
            this.Items = new ObservableCollection<JTokenVM>();
            this.SelectedIndex = 0;
            this.DisplayMemberPath = "";
            this.CollectionChanged += JArrayVM_CollectionChanged;
        }

        void JArrayVM_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
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

        private JTokenVM CreateItem(object item)
        {
            JTokenVM val = null;
            if (item is JTokenVM)
                val = item as JTokenVM;
            else
                val = new JValueVM() { Value = item };
            return val;
        }

        private JTokenVM GetItem(object item)
        {
            return vm.Items.FirstOrDefault(x => x["Value"] == item);
        }

        public int IndexOf(object item)
        {
            var it = GetItem(item);
            if (it == null) return -1;
            return vm.Items.IndexOf(it);
        }

        public void Insert(int index, object item)
        {

            vm.Items.Insert(index, CreateItem(item));
        }

        public void RemoveAt(int index)
        {
            vm.Items.RemoveAt(index);
        }

        public object this[int index]
        {
            get
            {
                var item = vm.Items[index];
                if (item is JValueVM)
                    return vm.Items[index]["Value"];
                else
                    return item;
            }
            set
            {
                vm.Items[index]["Value"] = value;
            }
        }

        public void Add(object item)
        {
            vm.Items.Add(CreateItem(item));
        }

        public void Clear()
        {
            vm.Items.Clear();
        }

        public bool Contains(object item)
        {
            return GetItem(item) != null;
        }

        public void CopyTo(object[] array, int arrayIndex)
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

        public bool Remove(object item)
        {
            JTokenVM it = GetItem(item);
            return vm.Items.Remove(it);
        }

        public IEnumerator<object> GetEnumerator()
        {
            foreach (var item in vm.Items)
            {
                yield return item["Value"];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
