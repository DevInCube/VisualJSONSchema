using MyVisualJSONEditor.ViewModels;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
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

namespace MyVisualJSONEditor.Views
{
    /// <summary>
    /// Interaction logic for JsonSchemaBuilder.xaml
    /// </summary>
    public partial class JsonSchemaBuilder : UserControl
    {

        public JsonSchemaBuilder()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
           "Data", typeof(object), typeof(JsonSchemaBuilder), new PropertyMetadata(default(object), OnChange));

        /// <summary>Gets or sets the <see cref="JsonObjectModel"/> to edit with the editor. </summary>
        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        private static void OnChange(object o, DependencyPropertyChangedEventArgs args)
        {
            (o as JsonSchemaBuilder).Update();
        }

        private void Update()
        {
            Presenter.Content = Data;
        }

        private void OnAddArrayObject(object sender, RoutedEventArgs e)
        {
            var property = (JPropertyVM)((Button)sender).Tag;

            if (property.Value == null)
                property.Value = new ObservableCollection<JTokenVM>();

            var list = (ObservableCollection<JTokenVM>)property.Value;
            var schema = property.Schema.Items;

            var obj =  JObjectVM.FromSchema(schema.First());
            obj.ParentList = list;

            list.Add(obj);
        }

        private void OnRemoveArrayObject(object sender, RoutedEventArgs e)
        {
            var obj = (JTokenVM)((Button)sender).Tag;
            obj.ParentList.Remove(obj);
        }
    }
}
