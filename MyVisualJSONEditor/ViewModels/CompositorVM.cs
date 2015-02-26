using MyToolkit.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;

namespace MyVisualJSONEditor.ViewModels
{
    public class CompositorVM : ObservableObject
    {

        private JObjectVM vm;

        public CompositorVM()
        {
            //
        }

        public void Init(JObjectVM vm)
        {
            this.vm = vm;
            JArrayVM sources = vm.GetValue<JArrayVM>("Source.QueueSet");
            var prop = vm.GetValue("Source.SetMaster") as JPropertyVM;
            if (prop != null)
            {
                prop.Command = new RelayCommand(() => {
                    JObjectVM selected = (sources.SelectedItem as JObjectVM);
                    string name = null;
                    if (selected != null)
                    {
                        name = selected["Name"].ToString();
                    }
                    vm.Data["Source.MasterName"] = name;
                });
            }
            sources.Items.CollectionChanged += Items_CollectionChanged;
            sources.PropertyChanged += sources_PropertyChanged;
        }

        void sources_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("SelectedIndex"))
            {
                vm.GetProperty("Source.SetMaster").IsEnabled = (sender as JArrayVM).SelectedIndex >= 0;
            }
        }

        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                JObjectVM removedItem = (e.OldItems[0] as JObjectVM);
                if (removedItem["Name"].ToString().Equals(vm.Data["Source.MasterName"]))
                {
                    vm.Data["Source.MasterName"] = null;
                }
            }
        }
    }
}
