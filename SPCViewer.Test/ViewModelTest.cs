using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPCViewer.ViewModel;

namespace SPCViewer.Test
{
    [TestClass]
    public class ViewModelTest
    {
        [TestMethod]
        public void TestSpectrumViewModel()
        {
            const string path = "files/epr.par";
            var vm = new SpectrumViewModel(path);
            Assert.AreEqual(2048, vm.Spectrum.XYData.Count);
        }
    }
}
