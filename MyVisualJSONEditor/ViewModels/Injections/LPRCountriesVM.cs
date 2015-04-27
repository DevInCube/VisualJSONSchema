using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VitML.JsonVM.Linq;
using VitML.JsonVM.ViewModels;

namespace MyVisualJSONEditor.ViewModels.Injections
{
    public class LPRCountriesVM : ObservableObject, IJsonDataProvider
    {

        private JArray countries;

        public LPRCountriesVM(JCustomVM data)
        {

            JArray countries = data.JsonData as JArray;
            this.countries = countries;
        }


        public Newtonsoft.Json.Linq.JToken Data
        {
            get { return countries; }
        }
    }
}
