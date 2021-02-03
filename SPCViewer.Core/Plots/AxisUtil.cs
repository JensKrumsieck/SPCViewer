using ChemSharp.Extensions;
using ChemSharp.Spectroscopy;
using ChemSharp.Spectroscopy.DataProviders;
using ChemSharp.Spectroscopy.Extension;
using ChemSharp.UnitConversion;

namespace SPCViewer.Core.Plots
{
    public static class AxisUtil
    {
        public static void SecondarySetUp(this LinearAxisEx secondaryAxis, Spectrum spc)
        {
            switch (spc.DataProvider)
            {
                case BrukerEPRProvider _:
                    SetUpGAxis(secondaryAxis, spc);
                    break;
                case VarianUVVisProvider _:
                    SetUpWaveAxis(secondaryAxis, spc);
                    break;
            }
        }

        private static void SetUpGAxis(this LinearAxisEx secondaryAxis, Spectrum spc)
        {
            var freq = spc.GetSpecialParameters()["Frequency"].ToSingle();
            double Converter(double s) => SpecialConverters.BFromG((float)s, freq, spc.Unit());
            double InverseConverter(double s) => SpecialConverters.GFromB((float)s, freq, spc.Unit());
            secondaryAxis.Converter = Converter;
            secondaryAxis.InverseConverter = InverseConverter;
            secondaryAxis.Title = "g";
            secondaryAxis.Unit = "";
        }

        private static void SetUpWaveAxis(this LinearAxisEx secondaryAxis, Spectrum spc)
        {
            var converter = new EnergyUnitConverter("nm", "cm^-1");
            secondaryAxis.Converter = s=>converter.Convert(s) /1000;
            secondaryAxis.InverseConverter = s=>converter.ConvertInverted(s) *1000;
            secondaryAxis.Title = "ν";
            secondaryAxis.Unit = "10^{3} cm^{-1}";
        }
    }
}
