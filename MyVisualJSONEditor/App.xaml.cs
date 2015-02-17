using MyVisualJSONEditor.ViewModels;
using MyVisualJSONEditor.Views;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MyVisualJSONEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
           /* TestWindow w = new TestWindow();
          
            var Schema = MyVisualJSONEditor.Properties.Resources.Schema;
            var JsonData = MyVisualJSONEditor.Properties.Resources.Data;
           
            JSchema sh = JSchema.Parse(Schema);
            JObject j = JObject.Parse(JsonData);
            w.tab1.Data = JObjectVM.FromJson(j, sh);
            w.tab2.Data = JObjectVM.FromJson(j, sh);
            w.tab3.Data = JObjectVM.FromJson(j, sh);*/

            MainWindow w = new MyVisualJSONEditor.MainWindow();
            w.Show();
        }
    }   

}
