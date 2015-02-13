using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVisualJSONEditor.ViewModels
{
    public class ObservableDictionary : IDictionary<string, object>, INotifyCollectionChanged, INotifyPropertyChanged
    {

        private Dictionary<string, object> dict = new Dictionary<string, object>();

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Add(string key, object value)
        {
            dict.Add(key, value);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<string, object>(key, value));
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
            if(!this.ContainsKey(key)) return false;
            object val = dict[key];
            bool removed = dict.Remove(key);
            if (removed)
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<string, object>(key, val));
            return removed;
        }

        public bool TryGetValue(string key, out object value)
        {
            return dict.TryGetValue(key, out value);
        }

        public ICollection<object> Values
        {
            get { return dict.Values; }
        }

        public object this[string key]
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
                    OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<string, object>(key, value));
                }
                else
                {
                    KeyValuePair<string, object> oldItem = new KeyValuePair<string, object>(key, dict[key]);
                    dict[key] = value;
                    OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<string, object>(key, value), oldItem);
                    OnPropertyChanged(key);
                }
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            dict.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            dict.Clear();
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return dict.ContainsKey(item.Key) && dict[item.Key].Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
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

        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected void OnCollectionChanged(
           NotifyCollectionChangedAction action,
           object item
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
            object newItem,
            object oldItem
            )
        {
            if (this.CollectionChanged != null)
            {
                var args = new NotifyCollectionChangedEventArgs(action, newItem, oldItem);
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
