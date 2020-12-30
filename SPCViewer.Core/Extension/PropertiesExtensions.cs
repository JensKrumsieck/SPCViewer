using ChemSharp.Spectroscopy;
using ChemSharp.Spectroscopy.DataProviders;

namespace SPCViewer.Core.Extension
{
    public static class PropertiesExtensions
    {
        /// <summary>
        /// Returns default Unit as String
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Unit(this Spectrum input) =>
            input.DataProvider switch
            {
                BrukerEPRProvider epr => epr["JUN"],
                BrukerNMRProvider => "ppm",
                VarianUVVisProvider => "nm",
                _ => ""
            };

        /// <summary>
        /// Returns default Quantity for X Axis as String
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Quantity(this Spectrum input) =>
            input.DataProvider switch
            {
                BrukerEPRProvider => "B",
                BrukerNMRProvider => "δ",
                VarianUVVisProvider => "λ",
                _ => ""
            };

        /// <summary>
        /// Returns default Quantity for Y Axis as String
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string YQuantity(this Spectrum input) =>
        input.DataProvider switch
        {
            BrukerEPRProvider => "a.u.",
            BrukerNMRProvider => "a.u.",
            VarianUVVisProvider => "rel. Abs.",
            _ => ""
        };
    }
}
