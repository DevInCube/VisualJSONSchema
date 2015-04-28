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
using VitML.JsonVM.Linq;
using VitML.JsonVM;
using VitML.JsonVM.Schema;
using My.Json.Schema;
using VitML.JsonVM.Generation;

namespace VitML.JsonSchemaControlBuilder.Views
{
    /// <summary>
    /// Interaction logic for JBuilder.xaml
    /// </summary>
    public partial class TokenBuilder : UserControl
    {

        public TokenBuilder()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data", typeof(object), typeof(TokenBuilder), new PropertyMetadata(default(object), OnChange));

        /// <summary>Gets or sets the <see cref="JsonObjectModel"/> to edit with the editor. </summary>
        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        private static void OnChange(object o, DependencyPropertyChangedEventArgs args)
        {
            (o as TokenBuilder).Update();
        }

        private void Update()
        {
            this.Presenter.Content = Data;
        }

        private void OnAddArrayObject(object sender, RoutedEventArgs e)
        {
            object tag = ((Button)sender).Tag;

            JArrayVM vm = tag as JArrayVM;

            var list = vm.Items;

            JSchema schemaEx = vm.Schema;

            if (schemaEx.MaxItems != null)
                if (list.Count >= schemaEx.MaxItems)
                    return;

            JSchema schema = schemaEx.GetItemSchemaByIndex(list.Count);

            var set = new DataGenerationSettings() { CreateMinItems = true };
            JTokenVM obj = JObjectVM.FromJson(schema.GenerateData(set), schema);
            obj.ParentList = vm;

            list.Add(obj);
        }

        private void OnRemoveArrayObject(object sender, RoutedEventArgs e)
        {
            JTokenVM obj = (JTokenVM)((Button)sender).Tag;

            obj.ParentList.Items.Remove(obj);
        }

        private void OnCreateObject(object sender, RoutedEventArgs e)
        {
            var objVM = (JObjectVM)((Button)sender).Tag;

            if (objVM != null)
            {
                objVM.SetData(objVM.Schema.GenerateData());
                objVM.IsSpecified = true;
            }
        }

        private void OnRemoveObject(object sender, RoutedEventArgs e)
        {
            var objVM = (JObjectVM)((Button)sender).Tag;
            if (objVM != null)
            {
                objVM.SetData(null);
                objVM.IsSpecified = false;
            }
        }

        private void OnCreateFromNull(object sender, RoutedEventArgs e)
        {
            object tag = ((Button)sender).Tag;
            JTokenVM token = tag as JTokenVM;
            DataGenerationSettings settings = new DataGenerationSettings();
            settings.Force = ForceLevel.ForceFirst;
            token.SetData(token.Schema.GenerateData(settings));
        }

        private void OnCreateNull(object sender, RoutedEventArgs e)
        {
            object tag = ((Button)sender).Tag;
            JTokenVM token = tag as JTokenVM;

            token.SetData(null);
        }
    }
}
