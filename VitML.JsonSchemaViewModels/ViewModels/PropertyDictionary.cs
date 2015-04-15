using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VitML.JsonVM.Linq;

namespace VitML.JsonVM.ViewModels
{
    public class PropertyDictionary : IDictionary<string, JTokenVM>, INotifyCollectionChanged, INotifyPropertyChanged
    {

        private Dictionary<string, JTokenVM> dict = new Dictionary<string, JTokenVM>();

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Add(string key, JTokenVM value)
        {
            dict.Add(key, value);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<string, JTokenVM>(key, value));
        }

        public bool ContainsKey(string key)
        {
            return dict.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return dict.Keys; }
        }

        public bool Remove(string key)
        {
            if (!this.ContainsKey(key)) return false;
            JTokenVM val = dict[key];
            bool removed = dict.Remove(key);
            if (removed)
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<string, JTokenVM>(key, val));
            return removed;
        }

        public bool TryGetValue(string key, out JTokenVM value)
        {
            return dict.TryGetValue(key, out value);
        }

        public ICollection<JTokenVM> Values
        {
            get { return dict.Values; }
        }

        public JTokenVM this[string key]
        {
            get
            {
                return dict[key];
            }
            set
            {
                if (!dict.ContainsKey(key))
                {
                    dict[key] = value;
                    OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<string, JTokenVM>(key, value));
                }
                else
                {
                    KeyValuePair<string, JTokenVM> oldItem = new KeyValuePair<string, JTokenVM>(key, dict[key]);
                    dict[key] = value;
                    OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<string, JTokenVM>(key, value), oldItem);
                    OnPropertyChanged(key);
                    OnPropertyChanged("[" + key + "]");
                }
            }
        }

        public void Add(KeyValuePair<string, JTokenVM> item)
        {
            dict.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            dict.Clear();
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public bool Contains(KeyValuePair<string, JTokenVM> item)
        {
            return dict.ContainsKey(item.Key) && dict[item.Key].Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<string, JTokenVM>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return dict.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, JTokenVM> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, JTokenVM>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected void OnCollectionChanged(
           NotifyCollectionChangedAction action,
           KeyValuePair<string, JTokenVM> item
           )
        {
            if (this.CollectionChanged != null)
            {
                var args = new NotifyCollectionChangedEventArgs(action, item);
                CollectionChanged.Invoke(this, args);
            }
        }

        protected void OnCollectionChanged(
           NotifyCollectionChangedAction action
           )
        {
            if (this.CollectionChanged != null)
            {
                CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(action));
            }
        }

        protected void OnCollectionChanged(
            NotifyCollectionChangedAction action,
            IList newItems,
            IList oldItems
            )
        {
            if (this.CollectionChanged != null)
            {
                var args = new NotifyCollectionChangedEventArgs(action, newItems, oldItems);
                CollectionChanged.Invoke(this, args);
            }
        }

        protected void OnCollectionChanged(
            NotifyCollectionChangedAction action,
            KeyValuePair<string, JTokenVM> newItem,
            KeyValuePair<string, JTokenVM> oldItem
            )
        {
            if (this.CollectionChanged != null)
            {
                int index = Keys.ToList().IndexOf(((KeyValuePair<string, JTokenVM>)newItem).Key);
                var args = new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index);
                CollectionChanged.Invoke(this, args);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
