using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCViewer.Core;

namespace SPCViewer.Test
{
    [TestClass]
    public class SettingsTest
    {
        [TestMethod]
        public void TestSettings()
        {
            //saves default settings
            Settings.Instance.Load();
            Settings.Instance.Save();

            //reloads
            Settings.Instance.Load("settings.json");
            Assert.AreEqual("Arial", Settings.Instance.Font);
            Assert.AreEqual(14, Settings.Instance.FontSize);
            Assert.AreEqual(200, Settings.Instance.FontWeight);
            Assert.AreEqual("{0} / {1}", Settings.Instance.AxisFormat);
            Assert.AreEqual("#0092ca", Settings.Instance.AnnotationColor);
        }
    }
}
