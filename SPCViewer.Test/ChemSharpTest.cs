using ChemSharp.Spectroscopy;
using ChemSharp.Spectroscopy.DataProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SPCViewer.Test
{
    [TestClass]
    public class ChemSharpTest
    {
        [TestMethod]
        public void TestEPR()
        {
            const string path = "files/epr.par";
            var spc = new Spectrum()
            {
                DataProvider = new BrukerEPRProvider(path)
            };
            Assert.AreEqual(2048, spc.XYData.Count);
        }
    }
}
