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
            Assert.AreEqual(14, Settings.Instance.FontSize);
            Assert.AreEqual(200, Settings.Instance.FontWeight);
        }
    }
}
