using OxyPlot;
using SPCViewer.Core;
using SPCViewer.ViewModel.Extension;
using System;
using System.IO;
using PngExporter = OxyPlot.SkiaSharp.PngExporter;
using SvgExporter = SPCViewer.Core.Plots.SvgExporter;

namespace SPCViewer.ViewModel
{
    public static class SaveHandler
    {
        public static void Handle(SpectrumViewModel model, string filename)
        {
            Enum.TryParse(typeof(SupportedExportExtensions), ExtensionHandler.GetExtension(filename).ToUpper(), out var extension);
            using var stream = File.Create(filename);
            IExporter exporter = extension switch
            {
                SupportedExportExtensions.SVG => new SvgExporter
                {
                    Height = Settings.Instance.ExportHeight,
                    Width = Settings.Instance.ExportWidth
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
    }
}
