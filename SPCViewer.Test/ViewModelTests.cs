using ChemSharp;
using ChemSharp.Spectroscopy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCViewer.Core;
using SPCViewer.ViewModel;

namespace SPCViewer.Test
{
    [TestClass]
    public class ViewModelTests
    {
        private Spectrum _spc;
        private SpectrumViewModel _svm;

        [TestInitialize]
        public void SetUp()
        {
            const string path = "files/nmr/fid";
            _svm = new SpectrumViewModel(path);
            _spc = _svm.Spectrum;
        }

        [TestMethod]
        public void TestModel()
        {
            Assert.IsNotNull(_svm.Model);
            Assert.AreEqual(3, _svm.Model.Series.Count);
            Assert.AreEqual(UIAction.Zoom, _svm.MouseAction);
        }

        [TestMethod]
        public void TestPeak()
        {
            _svm.Peaks.Add(new Peak(new DataPoint(0, 0)));
            Assert.AreEqual(1, _svm.Model.Annotations.Count);
        }

        [TestMethod]
        public void TestIntegral()
        {
            _svm.Integrals.Add(new Integral(_spc.XYData));
            Assert.AreEqual(1, _svm.Model.Annotations.Count);
        }
    }
}
