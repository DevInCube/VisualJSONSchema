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

    public interface IJsonArray : IList<JTokenVM>
    {
        //
    }

    public class JArrayVM : JTokenVM
    {

        private int _SelectedIndex;
        private string _DisplayMemberPathPropertyName;
        private ObservableCollection<JTokenVM> _Items;


        public IJsonArray Data { get; private set; }

        private JArrayVM()
        {
            this.Data = new JsonArrayImpl(this);

            if (Items == null) CreateItems();
            
            this.SelectedIndex = 0;
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

        public ObservableCollection<JTokenVM> Items
        {
            get { return _Items; }
            private set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }              

        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                _SelectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }
        
        public JTokenVM SelectedItem
        {
            get { 
                int index = this.SelectedIndex;
                if (index > -1 && index < Items.Count)
                    return Data[index];
                return null;
            }
        }

        public string DisplayMemberPathPropertyName
        {
            get { return _DisplayMemberPathPropertyName; }
        }

        public override void SetData(JToken data)
        {
            base.SetData(data);

            if (data == null || data.Type == JTokenType.Null)
            {
                OnPropertyChanged("Items");
                return;//@todo
            }

            if (!(data is JArray)) throw new Exception("data is not JArray");

            JArray array = data as JArray;
            
            this.Items.Clear();

            int index = 0;
            for (int i = 0; i < array.Count; i++)
            {
                JToken item = array[i];
                var propertySchema = this.Schema.GetItemSchemaByIndex(index).CheckSchema(item);
                this.Items.Add(JObjectVM.FromJson(item, propertySchema));
            }
            OnPropertyChanged("Items");
        }

        public override void SetSchema(JSchema schema)
        {
            base.SetSchema(schema);

            if (Style != null)
                _DisplayMemberPathPropertyName = Style.DisplayMemberPath;
            else _DisplayMemberPathPropertyName = "";
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

        private JTokenVM GetItem(JTokenVM item)
        {
            return vm.Items.FirstOrDefault(x => x == item);
        }

        public int IndexOf(JTokenVM item)
        {
            var it = GetItem(item);
            if (it == null) return -1;
            return vm.Items.IndexOf(it);
        }

        public void Insert(int index, JTokenVM item)
        {
            item.ParentList = vm;
            vm.Items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            vm.Items.RemoveAt(index);
        }

        public JTokenVM this[int index]
        {
            get
            {
                return vm.Items[index];
            }
            set
            {
                vm.Items[index] = value;
            }
        }

        public void Add(JTokenVM item)
        {
            item.ParentList = vm;
            vm.Items.Add(item);
        }

        public void Clear()
        {
            vm.Items.Clear();
        }

        public bool Contains(JTokenVM item)
        {
            return GetItem(item) != null;
        }

        public void CopyTo(JTokenVM[] array, int arrayIndex)
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

        public bool Remove(JTokenVM item)
        {
            JTokenVM it = GetItem(item);
            return vm.Items.Remove(it);
        }

        public IEnumerator<JTokenVM> GetEnumerator()
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
