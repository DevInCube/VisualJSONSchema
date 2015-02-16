using MyVisualJSONEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyVisualJSONEditor.Views.Templates
{
    /// <summary>
    /// Interaction logic for ObjectTemplate.xaml
    /// </summary>
    public partial class TabRootTemplate : UserControl
    {
        public TabRootTemplate()
        {
            InitializeComponent();
        }

        private void OnCreateObject(object sender, RoutedEventArgs e)
        {
            var property = (JPropertyVM)((CheckBox)sender).Tag;
            if (property != null)
            {
                if (property.Parent[property.Key] == null)
                {
                    property.Parent[property.Key] = JObjectVM.FromSchema(property.Schema);
                    //property.RaisePropertyChanged<JPropertyVM>(i => i.HasValue); //@todo
                }
            }
        }

        private void OnRemoveObject(object sender, RoutedEventArgs e)
        {
            var property = (JPropertyVM)((CheckBox)sender).Tag;
            if (property != null)
            {
                if (property.Parent.ContainsKey(property.Key) && property.Parent[property.Key] != null)
                {
                    property.Parent[property.Key] = null;
                    //property.RaisePropertyChanged<JPropertyVM>(i => i.HasValue); //@todo
                }
            }
        }
    }
}
