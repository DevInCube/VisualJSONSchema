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

        private JArrayVM()
        {
            this.Data = new JsonArrayImpl(this);

            if (Items == null) CreateItems();
            
           // this.SelectedIndex = 0;
           // this.DisplayMemberPath = "";
           // this.CollectionChanged += JArrayVM_CollectionChanged;
        }

        private void CreateItems()
        {            
            this.Items = new ObservableCollection<JTokenVM>();
            this.Items.CollectionChanged += Items_CollectionChanged;
        }

        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add
                || e.Action == NotifyCollectionChangedAction.Replace)
            {
                JTokenVM token = e.NewItems[0] as JTokenVM;
                token.ParentList = this;
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

        private ObservableCollection<JTokenVM> _Items;

        public ObservableCollection<JTokenVM> Items
        {
            get { return _Items; }
            private set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }
        /*
        public int SelectedIndex
        {
            get { return ContainsKey("SelectedIndex") ? int.Parse(this["SelectedIndex"].ToString()) : 0; }
            set
            {
                this["SelectedIndex"] = value;
                OnPropertyChanged("SelectedIndex");
            }
        }*/
        /*
        public object SelectedItem
        {
            get { 
                int index = this.SelectedIndex;
                if (index > -1 && index < Items.Count)
                    return Data[index];
                return null;
            }
        }*/

        /*
        public string DisplayMemberPath
        {
            get { return "Value" + (ContainsKey("DisplayMemberPath") ? "." + this["DisplayMemberPath"] : ""); }
            set
            {
                this["DisplayMemberPath"] = value;
                OnPropertyChanged("DisplayMemberPath");
            }
        }*/


        public override void SetData(JToken data)
        {
            base.SetData(data);

            if (!(data is JArray)) throw new Exception("data is not JArray");

            JArray array = data as JArray;

            if (Items == null) CreateItems();
            this.Items.Clear();

            int index = 0;
            for (int i = 0; i < array.Count; i++)
            {
                JToken item = array[i];
                var propertySchema = this.Schema.GetItemSchemaByIndex(index);
                this.Items.Add(JObjectVM.FromJson(item, propertySchema));
            }       
        }

        public static JArrayVM Create(JSchema schema, JArray array)
        {
            JArrayVM arr = new JArrayVM();
            arr.SetSchema(schema);
            arr.SetData(array);
            return arr;
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
            JTokenVM val = JValueVM.Create(schema, item);           
            val.ParentList = vm;
            return val;
        }

        private JTokenVM GetItem(JToken item)
        {
            return null;//@todo vm.Items.FirstOrDefault(x => x == item);
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
                return null;//vm.Items[index]["Value"];  @todo
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
                yield return null;//@todo item.Data;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
