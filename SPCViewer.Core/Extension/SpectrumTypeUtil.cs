using ChemSharp.Spectroscopy;
using ChemSharp.Spectroscopy.DataProviders;
using ChemSharp.Spectroscopy.Extension;

namespace SPCViewer.Core.Extension
{
    public static class SpectrumTypeUtil
    {
        /// <summary>
        /// Try to determine if an nmr spectrum is present by using provider or unit
        /// </summary>
        /// <param name="spc"></param>
        /// <returns></returns>
        public static bool IsNMRSpectrum(this Spectrum spc) => spc.DataProvider is BrukerNMRProvider || spc.Unit().Contains("ppm");

        /// <summary>
        /// Try to determine if an nmr spectrum is present by using provider or x axis quantity
        /// </summary>
        /// <param name="spc"></param>
        /// <returns></returns>
        public static bool IsEPRSpectrum(this Spectrum spc) => spc.DataProvider is BrukerEPRProvider || spc.Quantity() == "B";

        /// <summary>
        /// Try to determine if an uv spectrum is present by using provider or unit
        /// </summary>
        /// <param name="spc"></param>
        /// <returns></returns>
        public static bool IsUVSpectrum(this Spectrum spc) => spc.DataProvider is VarianUVVisProvider || spc.Unit().Contains("nm");
    }
}
