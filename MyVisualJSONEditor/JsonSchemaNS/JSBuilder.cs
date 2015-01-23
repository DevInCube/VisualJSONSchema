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

        public static void Build(ItemsControl control, JSchema sh, JToken data, object DataContext)
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
                                //@todo
                                CreateBinding(combo, ComboBox.ItemsSourceProperty, "Posts", DataContext);
                                CreateBinding(combo, ComboBox.SelectedItemProperty, "SelectedPost", DataContext);
                                combo.DisplayMemberPath = "PostName";
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
                        data.ToList().Add(newItem);
                        //@todo create new instance
                        //@todo get default
                    };
                    grid.Children.Add(addButton);
                    DockPanel.SetDock(grid, Dock.Bottom);
                    dock.Children.Add(grid);
                    dock.Children.Add(itemsControl);
                    control.Items.Add(dock);
                    JSchema oneSchema =  sh.Items.First();
                    if (data != null)
                    {
                        foreach (JToken item in data.ToList())
                        {
                            ItemsControl inner = new ItemsControl();
                            var wrap = WrapListItem(inner);
                            itemsControl.Items.Add(wrap);
                            Build(inner, oneSchema, item, DataContext); //@todo  here i use of one First() Schema
                        }
                    }
                    return;
                case(JSchemaType.Integer):
                    IntegerUpDown intUpDown = new IntegerUpDown();
                    if (data != null)
                        intUpDown.Value = int.Parse(data.ToString()); 
                    else
                    {
                        if (sh.Default != null)
                            intUpDown.Value = int.Parse(sh.Default.ToString());
                    }
                    intUpDown.Maximum = (int?)sh.Maximum ?? int.MaxValue;
                    intUpDown.Minimum = (int?)sh.Minimum ?? int.MinValue;
                    intUpDown.IsReadOnly = sh.IsReadonly();
                    intUpDown.SetBinding(IntegerUpDown.ValueProperty, CreateBinding(data, DataContext));
                    control.Items.Add(WrapWithLabel(intUpDown, sh.Title));
                    break;
                case(JSchemaType.Float):
                    DoubleUpDown dbUpDown = new DoubleUpDown();
                    if (data != null)
                        dbUpDown.Value = double.Parse(data.ToString()); 
                    else
                    {
                        if (sh.Default != null)
                            dbUpDown.Value = double.Parse(sh.Default.ToString());
                    }
                    dbUpDown.Maximum = (double?)sh.Maximum ?? double.MaxValue;
                    dbUpDown.Minimum = (double?)sh.Minimum ?? double.MinValue;
                    dbUpDown.IsReadOnly = sh.IsReadonly();
                    dbUpDown.SetBinding(DoubleUpDown.ValueProperty, CreateBinding(data, DataContext));
                    control.Items.Add(WrapWithLabel(dbUpDown, sh.Title));
                    break;
                case (JSchemaType.String):
                    TextBox tb = new TextBox();
                    tb.MaxLength = (int?)sh.MaximumLength ?? int.MaxValue;
                    tb.IsReadOnly = sh.IsReadonly();
                    MultiBinding mBinding = new MultiBinding();
                    mBinding.Converter = new MultiConverter();
                    Binding oBinding = CreateBinding(data, DataContext);
                    mBinding.Bindings.Add(oBinding);
                    Binding tBinding = GetBinding(sh, DataContext);
                    if (tBinding != null)
                    {
                        mBinding.Bindings.Add(tBinding);
                    }
                    mBinding.Mode = BindingMode.TwoWay;
                    mBinding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
                    mBinding.NotifyOnTargetUpdated = true;
                    mBinding.NotifyOnSourceUpdated = true;
                    tb.SetBinding(TextBox.TextProperty, mBinding);
                    tb.TargetUpdated += (se, ar) => {
                        MultiBindingExpression b = BindingOperations.GetMultiBindingExpression(se as TextBox, TextBox.TextProperty);
                        if (b != null)
                        {
                            b.BindingExpressions.First().UpdateSource();
                            b.UpdateSource();
                        }
                    };
                    tb.SourceUpdated += (se2, ar2) => {
                        return;
                    };
                    //
                    tb.KeyUp += (s, args) => {
                        TextBox textBox = s as TextBox;
                        JToken newData = JToken.FromObject(textBox.Text);
                        bool isValid = newData.IsValid(sh);
                        if (!isValid) textBox.Background = Brushes.Bisque;
                        else textBox.Background = Brushes.White;
                    };
                    if (data != null)
                        tb.Text = data.ToString();
                    else
                    {
                        if (sh.Default != null)
                            tb.Text = sh.Default.ToString();
                    }                    
                    control.Items.Add(WrapWithLabel(tb, sh.Title));
                    break;
            }

            if (innerControl != null)
            {
                if (sh.Properties.Count > 0)
                {
                    foreach (var property in sh.Properties)
                    {
                        string key = property.Key;
                        JToken innerData = null;
                        if (data != null)
                            innerData = data.SelectToken(key);
                        Build(innerControl, property.Value, innerData, DataContext); 
                    }
                }
                else
                {
                    if (sh.OneOf.Count > 0)
                    {
                        if (sh.OneOf.Count == 1)
                        {
                            JSchema sh2 = sh.OneOf.First();
                            Build(innerControl, sh2, data, DataContext); //@todo
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
            Binding binding = new Binding(path);
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = DataContext;
            el.SetBinding(dp, binding);
        }

    }
}
