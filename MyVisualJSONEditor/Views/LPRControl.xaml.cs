using My.Json.Schema;
using MyVisualJSONEditor.Properties;
using MyVisualJSONEditor.ViewModels.Injections;
using Newtonsoft.Json.Linq;
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
using VitML.JsonVM.ViewModels;

namespace MyVisualJSONEditor.Views
{
    /// <summary>
    /// Interaction logic for LPRControl.xaml
    /// </summary>
    public partial class LPRControl : UserControl
    {
        public LPRControl()
        {
            InitializeComponent();
            this.DataContextChanged += LPRControl_DataContextChanged;
        }

        void LPRControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is JTokenVM)
            {
                this.DataContext = new LPRVM(e.NewValue as JCustomVM);
            }
        }
    }

    
}
