using ChemSharp.Spectroscopy;
using ChemSharp.Spectroscopy.Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCViewer.ViewModel;

namespace SPCViewer.Test
{
    [TestClass]
    public class EPRTests
    {
        private Spectrum _spc;
        private SpectrumViewModel _svm;

        [TestInitialize]
        public void SetUp()
        {
            const string path = "files/epr.par";
            var mvm = new MainViewModel();
            var doc = new DocumentViewModel(mvm);
            _svm = new SpectrumViewModel(doc, path);
            _spc = _svm.Spectrum;
        }

        [TestMethod]
        public void TestCount() => Assert.AreEqual(2048, _spc.XYData.Count);

        [TestMethod]
        public void TestUnit() => Assert.AreEqual("G", _spc.Unit());
    }
}
