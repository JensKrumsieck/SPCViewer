using ChemSharp.Spectroscopy;
using ChemSharp.Spectroscopy.Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCViewer.ViewModel;

namespace SPCViewer.Test
{
    [TestClass]
    public class NMRTests
    {
        private Spectrum _spc;
        private SpectrumViewModel _svm;

        [TestInitialize]
        public void SetUp()
        {
            const string path = "files/nmr/fid"; 
            var mvm = new MainViewModel();
            var doc = new DocumentViewModel(mvm);
            _svm = new SpectrumViewModel(doc, path);
            _spc = _svm.Spectrum;
        }

        [TestMethod]
        public void TestCount() => Assert.AreEqual(32768, _spc.XYData.Count);

        [TestMethod]
        public void TestUnit() => Assert.AreEqual("ppm", _spc.Unit());

    }
}
