
using Newtonsoft.Json.Linq;
using My.Json.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

     
    }
}
