using ChemSharp.Spectroscopy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCViewer.Core;
using SPCViewer.ViewModel;
using System.Linq;

namespace SPCViewer.Test
{
    [TestClass]
    public class PeakPickingTest
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
        public void TestPeakPickingAlgorithm()
        {
            var dataPoints = _spc.XYData.Where(s => s.X >= 300).ToArray();
            var data = dataPoints.Select(s => s.Y).ToList();
            var peakPos = data.FindPeakPositions();
            Assert.AreEqual(3, peakPos.Count);
            var peaks = peakPos.Select(index => new Peak(dataPoints[index])).ToList();
            var soret = peaks.Min(s => s.X);
            Assert.AreEqual(418, soret, 1);
        }
    }
}
