﻿using System;
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

namespace VitML.JsonSchemaControlBuilder.Views
{
    /// <summary>
    /// Interaction logic for JBuilder.xaml
    /// </summary>
    public partial class JBuilder : UserControl
    {

        public JBuilder()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data", typeof(object), typeof(JBuilder), new PropertyMetadata(default(object), OnChange));

        /// <summary>Gets or sets the <see cref="JsonObjectModel"/> to edit with the editor. </summary>
        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        private static void OnChange(object o, DependencyPropertyChangedEventArgs args)
        {
            (o as JBuilder).Update();
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
              
            JTokenVM obj = JObjectVM.FromSchema(schema);
            obj.ParentList = list;

            list.Add(obj);
        }

        private void OnRemoveArrayObject(object sender, RoutedEventArgs e)
        {
            JTokenVM obj = (JTokenVM)((Button)sender).Tag;

            obj.ParentList.Remove(obj);
        }

        private void OnCreateObject(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
            /*
            var property = (JPropertyVM)((CheckBox)sender).Tag;

            if (property != null)
            {
                if (property.Parent[property.Key] == null)
                {
                    property.Parent[property.Key] = JObjectVM.FromSchema(property.Schema);

                    property.RaisePropertyChanged("Value");
                    property.RaisePropertyChanged("HasValue");
                }
            }*/
        }

        private void OnRemoveObject(object sender, RoutedEventArgs e)
        {
            var property = (JPropertyVM)((CheckBox)sender).Tag;
            if (property != null)
            {
                if (property.Parent.ContainsKey(property.Key) && property.Parent[property.Key] != null)
                {
                    property.Parent[property.Key] = null;

                    property.RaisePropertyChanged("Value");
                    property.RaisePropertyChanged("HasValue");
                }
            }
        }
    }
}
