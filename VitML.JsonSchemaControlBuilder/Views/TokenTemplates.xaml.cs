using My.Json.Schema;
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
using VitML.JsonVM.Generation;
using VitML.JsonVM.Linq;
using VitML.JsonVM;

namespace VitML.JsonSchemaControlBuilder.Views
{
    /// <summary>
    /// Interaction logic for TokenTemplates.xaml
    /// </summary>
    public partial class TokenTemplates : UserControl
    {
        public TokenTemplates()
        {
            InitializeComponent();
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

            JTokenVM obj = JObjectVM.FromSchema(schema);
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
