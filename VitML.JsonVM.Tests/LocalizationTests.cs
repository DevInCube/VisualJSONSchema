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
            SchemaLocalizationData lang = SchemaLocalizationData.Parse(@"
{
    'uk-UA' : {
        'title' : 'Заголовок',
    }
}
");
            string schemaStr = "{title:'{$loc:title} 0'}";
            schemaStr = lang.Localize(schemaStr, new CultureInfo("uk-UA"));
            JSchema schema = JSchema.Parse(schemaStr);

            Assert.AreEqual("Заголовок 0", schema.Title);
        }

        [TestMethod]
        public void Loc_SetInnerSchemaUALocalization_OK()
        {
            SchemaLocalizationData lang = SchemaLocalizationData.Parse(@"
{
    'uk-UA' : {
        'title' : 'Заголовок',
        'title2' : 'Тест',
    }
}
");
            string schemaStr = "{title:'{$loc:title} 0', properties:{'test':{title:'{$loc:title2}'}}}";
            schemaStr = lang.Localize(schemaStr, new CultureInfo("uk-UA"));
            JSchema schema = JSchema.Parse(schemaStr);

            Assert.AreEqual("Тест", schema.Properties["test"].Title);
        }

        [TestMethod]
        public void Loc_SetNonExistanceLang_LeavesKey()
        {
            SchemaLocalizationData lang = SchemaLocalizationData.Parse(@"
{
    'uk-UA' : {
        'title' : 'Заголовок',
    }
}
");
            string schemaStr = "{title:'{$loc:title} 0'}";
            schemaStr = lang.Localize(schemaStr, new CultureInfo("ru-RU"));
            JSchema schema = JSchema.Parse(schemaStr);

            Assert.AreEqual("title 0", schema.Title);
        }
    }
}
