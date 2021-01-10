using OxyPlot;
using SPCViewer.Core;
using SPCViewer.ViewModel.Extension;
using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using ChemSharp.Spectroscopy.Extension;
using PngExporter = OxyPlot.SkiaSharp.PngExporter;
using SvgExporter = SPCViewer.Core.Plots.SvgExporter;

namespace SPCViewer.ViewModel
{
    public static class SaveHandler
    {
        /// <summary>
        /// Handles Exporting files
        /// </summary>
        /// <param name="model"></param>
        /// <param name="filename"></param>
        public static void Handle(SpectrumViewModel model, string filename)
        {
            Enum.TryParse(typeof(SupportedExportExtensions), ExtensionHandler.GetExtension(filename).ToUpper(), out var extension);
            switch (extension)
            {
                case SupportedExportExtensions.SVG:
                case SupportedExportExtensions.PNG:
                    ExportImage(model, filename, (SupportedExportExtensions)extension);
                    break;
                case SupportedExportExtensions.DAT:
                case SupportedExportExtensions.CSV:
                    ExportASCII(model, filename, (SupportedExportExtensions)extension);
                    break;
            }
        }

        /// <summary>
        /// Handles exporting images
        /// </summary>
        /// <param name="model"></param>
        /// <param name="filename"></param>
        /// <param name="extension"></param>
        private static void ExportImage(SpectrumViewModel model, string filename, SupportedExportExtensions extension)
        {
            using var stream = File.Create(filename);
            IExporter exporter = extension switch
            {
                SupportedExportExtensions.SVG => new SvgExporter
                {
                    Height = Settings.Instance.ExportHeight,
                    Width = Settings.Instance.ExportWidth,
                    Dpi = Settings.Instance.ExportDPI
                },
                SupportedExportExtensions.PNG => new PngExporter
                {
                    Height = (int)Settings.Instance.ExportHeight,
                    Width = (int)Settings.Instance.ExportWidth,
                    Dpi = Settings.Instance.ExportDPI
                },
                _ => null
            };
            exporter?.Export(model.Model, stream);
        }

        /// <summary>
        /// Handles exporting ascii
        /// </summary>
        /// <param name="model"></param>
        /// <param name="filename"></param>
        /// <param name="extension"></param>
        private static void ExportASCII(SpectrumBaseViewModel model, string filename, SupportedExportExtensions extension)
        {
            var separator = ";";
            if (extension == SupportedExportExtensions.CSV) separator = ",";
            using var sw = new StreamWriter(filename);
            sw.WriteLine(model.Title);
            sw.WriteLine($"{model.Spectrum.Quantity()} {model.Spectrum.Unit()}{separator}{model.Spectrum.YQuantity()}{separator}");
            foreach (var dp in model.Spectrum.XYData)
                sw.WriteLine($"{dp.X.ToString(CultureInfo.InvariantCulture)}{separator}{dp.Y.ToString(CultureInfo.InvariantCulture)}{separator}");
        }
    }
}
