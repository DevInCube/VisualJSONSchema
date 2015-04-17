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
using VitML.JsonVM;

namespace VitML.JsonSchemaControlBuilder.Views
{
    /// <summary>
    /// Interaction logic for RequireBuilder.xaml
    /// </summary>
    public partial class ViewBuilder : UserControl
    {
        public ViewBuilder()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
           "Data", typeof(object), typeof(ViewBuilder), new PropertyMetadata(default(object), OnChange));

        /// <summary>Gets or sets the <see cref="JsonObjectModel"/> to edit with the editor. </summary>
        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        private static void OnChange(object o, DependencyPropertyChangedEventArgs args)
        {
            (o as ViewBuilder).Update();
        }

        private void Update()
        {
            this.Presenter.Content = Data;
        }

        private void OnCreateObject(object sender, RoutedEventArgs e)
        {
            JTokenVM token = (JTokenVM)((Button)sender).Tag;

            token.SetData(token.Schema.GenerateData());
            token.IsSpecified = true;
        }

        private void OnRemoveObject(object sender, RoutedEventArgs e)
        {
            JTokenVM token = (JTokenVM)((Button)sender).Tag;

            token.SetData(null);
            token.IsSpecified = false;
        }
    }
}
