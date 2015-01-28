using MyVisualJSONEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ArrayTemplate.xaml
    /// </summary>
    public partial class ArrayTemplate : UserControl
    {
        public ArrayTemplate()
        {
            InitializeComponent();
        }

        private void OnAddArrayObject(object sender, RoutedEventArgs e)
        {
            var property = (JPropertyVM)((Button)sender).Tag;

            if (property.Value == null)
                property.Value = new ObservableCollection<JTokenVM>();

            var list = (property.Value as JArrayVM).Items;
            var schema = property.Schema.Items;

            var obj = JObjectVM.FromSchema(schema.First());
            obj.ParentList = list;

            list.Add(obj);
        }
    }
}
