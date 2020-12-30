using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCViewer.ViewModel;

namespace SPCViewer.Test
{
    [TestClass]
    public class ViewModelTest
    {
        private SpectrumViewModel ViewModel;

        [TestInitialize]
        public void Init()
        {
            const string path = "files/epr.par";
            ViewModel = new SpectrumViewModel(path);
        }

        [TestMethod]
        public void TestSpectrumViewModel()
        {
            Assert.AreEqual(2048, ViewModel.Spectrum.XYData.Count);
            Assert.AreEqual(1, ViewModel.Model.Series.Count);
            Assert.AreEqual(2, ViewModel.Model.Axes.Count);
        }
    }
}
