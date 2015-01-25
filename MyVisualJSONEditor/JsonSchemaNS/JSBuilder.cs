using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace MyVisualJSONEditor.JsonSchemaNS
{
    public static class JSBuilder
    {

        public static void Build(ItemsControl control, string key, JSchema sh, object DataContext)
        {
            ItemsControl innerControl = null;
            switch (sh.Type)
            {
                case (JSchemaType.Object):
                    innerControl = new ItemsControl();
                    innerControl.Margin = new Thickness(2);
                    if (sh.Title != null)
                    {
                        GroupBox box = new GroupBox();
                        box.Header = sh.Title;
                        Expander exp = new Expander();
                        exp.IsExpanded = true;
                        box.Content = exp;
                        exp.Content = innerControl;
                        control.Items.Add(box);
                    }
                    else
                    {
                        control.Items.Add(innerControl);
                    }
                    break;  
                case(JSchemaType.Array):

                    JToken widgetExt = sh.ExtensionData.FirstOrDefault(x => x.Key == "widget").Value;
                    if (widgetExt != null)
                    {
                        switch (widgetExt.ToString())
                        {
                            case("combobox"):
                                ComboBox combo = new ComboBox();
                                string path = key;
                                string itemsSourcePath = String.Format("{0}.ItemsSource", path);
                                string selectedItemPath = String.Format("{0}.SelectedItem", path);
                                string displayMemberPath = "PostName";
                                 JToken bindingExt = sh.ExtensionData.FirstOrDefault(x => x.Key == "bindings").Value;
                                 if (bindingExt != null)
                                 {
                                     JToken selItemToken = bindingExt.SelectToken("SelectedItem");
                                     if (selItemToken != null)
                                     {
                                         selectedItemPath = selItemToken.SelectToken("path").Value<string>();
                                     }
                                     JToken itemsSourceToken = bindingExt.SelectToken("ItemsSource");
                                     if (itemsSourceToken != null)
                                     {
                                         itemsSourcePath = itemsSourceToken.SelectToken("path").Value<string>();
                                     } 
                                 }
                                CreateBinding(combo, ComboBox.ItemsSourceProperty, itemsSourcePath, DataContext);
                                CreateBinding(combo, ComboBox.SelectedItemProperty, selectedItemPath, DataContext);
                                combo.DisplayMemberPath = displayMemberPath;
                                control.Items.Add( WrapWithLabel(combo, sh.Title));
                                return;
                        }
                    }

                    DockPanel dock = new DockPanel() { LastChildFill = true };
                    ItemsControl itemsControl = new ItemsControl();
                    Grid grid = new Grid();
                    Button addButton = new Button() { 
                        Content = "+ Add",
                        MinWidth = 75,
                        Height = 23
                        //@todo Command
                    };
                    addButton.Click += (sen, args) => {

                        JToken newItem = sh.Default;
                       // data.ToList().Add(newItem);
                        //@todo create new instance
                        //@todo get default
                    };
                    grid.Children.Add(addButton);
                    DockPanel.SetDock(grid, Dock.Bottom);
                    dock.Children.Add(grid);
                    dock.Children.Add(itemsControl);
                    control.Items.Add(dock);
                    JSchema oneSchema =  sh.Items.First();
                    /*if (data != null)
                    {
                        foreach (JToken item in data.ToList())
                        {
                            ItemsControl inner = new ItemsControl();
                            var wrap = WrapListItem(inner);
                            itemsControl.Items.Add(wrap);
                            Build(inner, oneSchema, item, DataContext); //@todo  here i use of one First() Schema
                        }
                    }*/
                    return;
                case(JSchemaType.Integer):
                    IntegerUpDown intUpDown = new IntegerUpDown();
                   // if (data != null)
                     //   intUpDown.Value = int.Parse(data.ToString()); 
                    //else
                    {
                        if (sh.Default != null)
                            intUpDown.Value = int.Parse(sh.Default.ToString());
                    }
                    intUpDown.Maximum = (int?)sh.Maximum ?? int.MaxValue;
                    intUpDown.Minimum = (int?)sh.Minimum ?? int.MinValue;
                    intUpDown.IsReadOnly = sh.IsReadonly();
                    //intUpDown.SetBinding(IntegerUpDown.ValueProperty, CreateBinding(data, DataContext));
                    control.Items.Add(WrapWithLabel(intUpDown, sh.Title));
                    break;
                case(JSchemaType.Float):
                    DoubleUpDown dbUpDown = new DoubleUpDown();
                  //  if (data != null)
                   //     dbUpDown.Value = double.Parse(data.ToString()); 
                   // else
                    {
                        if (sh.Default != null)
                            dbUpDown.Value = double.Parse(sh.Default.ToString());
                    }
                    dbUpDown.Maximum = (double?)sh.Maximum ?? double.MaxValue;
                    dbUpDown.Minimum = (double?)sh.Minimum ?? double.MinValue;
                    dbUpDown.IsReadOnly = sh.IsReadonly();
                  //  dbUpDown.SetBinding(DoubleUpDown.ValueProperty, CreateBinding(data, DataContext));
                    control.Items.Add(WrapWithLabel(dbUpDown, sh.Title));
                    break;
                case (JSchemaType.String):
                    TextBox tb = new TextBox();
                    tb.MaxLength = (int?)sh.MaximumLength ?? int.MaxValue;
                    tb.IsReadOnly = sh.IsReadonly();
                    string bPath = String.Format("_[{0}]", key);
                    CreateBinding(tb, TextBox.TextProperty, bPath, DataContext);
                    if (sh.Default != null)
                        tb.Text = sh.Default.ToString();                
                    control.Items.Add(WrapWithLabel(tb, sh.Title));
                    break;
            }

            if (innerControl != null)
            {
                if (sh.Properties.Count > 0)
                {
                    foreach (var property in sh.Properties)
                    {
                        string pk = property.Key;
                        JToken innerData = null;
                    //    if (data != null)
                     //       innerData = data.SelectToken(key);
                        string innerKey = key + (string.IsNullOrEmpty(key) ? "" : ".") + pk;
                        Build(innerControl, innerKey, property.Value, DataContext);
                    }
                }
                else
                {
                    if (sh.OneOf.Count > 0)
                    {
                        if (sh.OneOf.Count == 1)
                        {
                            JSchema sh2 = sh.OneOf.First();
                            Build(innerControl, key, sh2, DataContext); //@todo
                        }
                        else
                        {
                            // @todo define which one is here
                        }
                    }
                }
            }
        }

        public class MultiConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (values.Count() > 1 && values[1] != null)
                    return values[1];
                return values[0];
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
            {
                int sourcesNum = targetTypes.Count();
                object[] values = new object[sourcesNum];
                for (int i = 0; i < sourcesNum; i++)
                    values[i] = value;
                return values;
            }
        }

        private static Binding CreateBinding(JToken data, object DataContext)
        {
            Binding binding = new Binding();
            binding.Source = DataContext;
            binding.Path = new PropertyPath(string.Format("[{0}]", data.Path));
            binding.Mode = BindingMode.TwoWay;
            binding.NotifyOnTargetUpdated = true;
            binding.NotifyOnSourceUpdated = true;
            return binding;
        }

        private static StackPanel WrapWithLabel(FrameworkElement el, string text)
        {
            StackPanel sp = new StackPanel();
            Label lbl = new Label();
            lbl.Content = text;
            sp.Children.Add(lbl);
            sp.Children.Add(el);
            return sp;
        }

        private static FrameworkElement WrapListItem(FrameworkElement item)
        {
            Grid g = new Grid();
            Button remBtn = new Button(){
                Content = "- Remove",
                MinWidth = 75,
                Height = 23,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0,0,5,0)
            };
            g.Children.Add(item);
            g.Children.Add(remBtn);
            return g;
        }

        public static bool IsReadonly(this JSchema sh)
        {
            JToken readOnlyExt = sh.ExtensionData.FirstOrDefault(x => x.Key == "readonly").Value;
            return (readOnlyExt != null && bool.Parse(readOnlyExt.ToString()));
        }

        private static void Bind(FrameworkElement el, DependencyProperty dp, JSchema sh, object DataContext)
        {
            var binding = sh.GetBinding(DataContext);
            if (binding != null)
                el.SetBinding(dp, binding);
        }

        private static Binding GetBindingExtension(this JSchema sh)
        {
            Binding binding = null;
            JToken bindingExt = sh.ExtensionData.FirstOrDefault(x => x.Key == "binding").Value;
            if (bindingExt != null)
            {
                string path = bindingExt.SelectToken("path").ToString();
                binding = new Binding(path);
                JToken mode = bindingExt.SelectToken("mode");
                if (mode != null)
                {
                    binding.Mode = (BindingMode)Enum.Parse(typeof(BindingMode), mode.ToString(), true);
                }
            }
            return binding;
        }

        private static Binding GetBinding(this JSchema sh, object DataContext)
        {
            Binding binding = null;
            JToken bindingExt = sh.ExtensionData.FirstOrDefault(x => x.Key == "binding").Value;
            if (bindingExt != null)
            {
                string path = bindingExt.SelectToken("path").ToString();
                binding = new Binding(path);
                JToken mode = bindingExt.SelectToken("mode");
                if (mode != null)
                {
                    binding.Mode = (BindingMode)Enum.Parse(typeof(BindingMode), mode.ToString(), true);
                }
                binding.Source = DataContext;
            }
            return binding;
        }

        private static void CreateBinding(FrameworkElement el, DependencyProperty dp, string path, object DataContext)
        {
            Binding binding = new Binding();
            binding.Path = new PropertyPath(path);
            binding.Source = DataContext;
            binding.BindsDirectlyToSource = true;
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            el.SetBinding(dp, binding);
        }

    }
}
