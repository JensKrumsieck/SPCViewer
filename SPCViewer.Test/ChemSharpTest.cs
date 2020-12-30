using ChemSharp.Spectroscopy;
using ChemSharp.Spectroscopy.DataProviders;
using ChemSharp.Spectroscopy.Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SPCViewer.Test
{
    [TestClass]
    public class ChemSharpTest
    {
        private Spectrum _spc;

        [TestInitialize]
        public void Init()
        {
            const string path = "files/epr.par";
            _spc = new Spectrum()
            {
                DataProvider = new BrukerEPRProvider(path)
            };
        }

        [TestMethod]
        public void TestEPR()
        {
            Assert.AreEqual(2048, _spc.XYData.Count);
            Assert.AreEqual("B", _spc.Quantity());
            Assert.AreEqual(_spc["JUN"], _spc.Unit());
        }
    }
}
