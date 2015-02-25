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

        public CompositorVM()
        {
            //
        }

        public void Init(JObjectVM vm)
        {
            var prop = vm.GetValue("Source.SetMaster") as JPropertyVM;
            if (prop != null)
            {
                prop.Command = new RelayCommand(() => {
                    //(vm.GetValue<JArrayVM>("Source.QueueSet").SelectedItem as JObjectVM)["Name"]
                    MessageBox.Show("Set MASTER!");
                });
            }
        }
    }
}
