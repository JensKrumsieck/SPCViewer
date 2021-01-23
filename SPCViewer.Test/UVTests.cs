using ChemSharp.Spectroscopy;
using ChemSharp.Spectroscopy.Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCViewer.ViewModel;

namespace SPCViewer.Test
{
    [TestClass]
    public class UVTests
    {
        private Spectrum _spc;
        private SpectrumViewModel _svm;

        [TestInitialize]
        public void SetUp()
        {
            const string path = "files/uvvis.dsw";
            var doc = new DocumentViewModel();
            _svm = new SpectrumViewModel(doc, path);
            _spc = _svm.Spectrum;
        }

        [TestMethod]
        public void TestCount() => Assert.AreEqual(901, _spc.XYData.Count);

        [TestMethod]
        public void TestUnit() => Assert.AreEqual("nm", _spc.Unit());

    }
}