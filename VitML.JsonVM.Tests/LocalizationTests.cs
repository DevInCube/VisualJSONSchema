using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using My.Json.Schema;
using VitML.JsonVM;
using System.Globalization;
using VitML.JsonVM.Localization;

namespace VitML.JsonVM.Tests
{
    [TestClass]
    public class LocalizationTests
    {
        [TestMethod]
        public void Loc_SetUALocalization_OK()
        {
            JSchema schema = JSchema.Parse("{title:'{$loc:title} 0'}");

            SchemaLocalizationData lang = SchemaLocalizationData.Parse(@"
{
    'uk-UA' : {
        'title' : 'Заголовок',
    }
}
");

            schema.Localize(lang, new CultureInfo("uk-UA"));

            Assert.AreEqual("Заголовок 0", schema.Title);
        }

        [TestMethod]
        public void Loc_SetNonExistanceLang_LeavesKey()
        {
            JSchema schema = JSchema.Parse("{title:'{$loc:title} 0'}");

            SchemaLocalizationData lang = SchemaLocalizationData.Parse(@"
{
    'uk-UA' : {
        'title' : 'Заголовок',
    }
}
");

            schema.Localize(lang, new CultureInfo("ru-RU"));

            Assert.AreEqual("title 0", schema.Title);
        }
    }
}
