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
        private DocumentViewModel _doc;

        [TestInitialize]
        public void SetUp()
        {
            const string path = "files/uvvis.dsw"; 
            var mvm = new MainViewModel();
            var doc = new DocumentViewModel(mvm);
            _svm = new SpectrumViewModel(_doc, path);
            _spc = _svm.Spectrum;
        }

        [TestMethod]
        public void TestModel()
        {
            Assert.IsNotNull(_doc.Model);
            Assert.AreEqual(3, _doc.Model.Series.Count);
            Assert.AreEqual(UIAction.Zoom, _doc.MouseAction);
        }

        [TestMethod]
        public void TestPeak()
        {
            _svm.Peaks.Add(new Peak(new DataPoint(0, 0)));
            Assert.AreEqual(1, _svm.Annotations.Count);
        }

        [TestMethod]
        public void TestIntegral()
        {
            _svm.Integrals.Add(new Integral(_spc.XYData));
            Assert.AreEqual(1, _svm.Annotations.Count);
        }
    }
}
