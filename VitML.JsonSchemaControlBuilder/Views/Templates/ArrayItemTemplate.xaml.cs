
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
using VitML.JsonVM.Linq;

namespace MyVisualJSONEditor.Views.Templates
{
    /// <summary>
    /// Interaction logic for ArrayItemTemplate.xaml
    /// </summary>
    public partial class ArrayItemTemplate : UserControl
    {
        public ArrayItemTemplate()
        {
            InitializeComponent();
        }

        private void OnRemoveArrayObject(object sender, RoutedEventArgs e)
        {
            var obj = (JTokenVM)((Button)sender).Tag;
            obj.ParentList.Remove(obj);
        }
    }
}
