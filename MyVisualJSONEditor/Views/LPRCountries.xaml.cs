using MyVisualJSONEditor.ViewModels.Injections;
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

namespace MyVisualJSONEditor.Views
{
    /// <summary>
    /// Interaction logic for LPRCountries.xaml
    /// </summary>
    public partial class LPRCountries : UserControl
    {
        public LPRCountries()
        {
            InitializeComponent();

            this.DataContextChanged += LPRControl_DataContextChanged;
        }

        void LPRControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is JTokenVM)
            {
                this.DataContext = new LPRCountriesVM(e.NewValue as JCustomVM);
            }
        }
    }
}
