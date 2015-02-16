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

namespace MyVisualJSONEditor.Views.Controls
{
    /// <summary>
    /// Interaction logic for BindablePasswordBox.xaml
    /// </summary>
    public partial class BindablePasswordBox : UserControl
    {

        private bool _updating = false;

        public BindablePasswordBox()
        {
            InitializeComponent();
            this.box.PasswordChanged += box_PasswordChanged;
        }

        void box_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _updating = true;
            this.Password = this.box.Password;
            _updating = false;
        }

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
          "Password", typeof(string), typeof(BindablePasswordBox), new PropertyMetadata(String.Empty, PasswordChanged));

        private static void PasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BindablePasswordBox box = d as BindablePasswordBox;

            if (box != null)
                box.box.PasswordChanged -= box.box_PasswordChanged;

            if (e.NewValue != null)
            {
                if (!box._updating)
                {
                    box.box.Password = e.NewValue.ToString();
                }
            }
            else
                box.Password = string.Empty;

            box.box.PasswordChanged += box.box_PasswordChanged;
        }

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }
    }
}
