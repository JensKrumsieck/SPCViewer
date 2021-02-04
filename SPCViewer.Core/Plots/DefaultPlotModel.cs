using ChemSharp.Spectroscopy;
using ChemSharp.Spectroscopy.Extension;
using OxyPlot;
using OxyPlot.Axes;
using SPCViewer.Core.Extension;
using System;
using System.Linq;

namespace SPCViewer.Core.Plots
{
    public class DefaultPlotModel : PlotModel
    {
        public LinearAxisEx XAxis { get; }
        public LinearAxisEx YAxis { get; }
        public LinearAxisEx SecondaryAxis { get; }

        /// <summary>
        /// Mapping Factor
        /// </summary>
        public double NormalizationFactor { get; set; } = 1;

        /// <summary>
        /// Global Mapping Function
        /// </summary>
        public Func<object, DataPoint> Mapping { get; set; }

        /// <summary>
        /// Constructor with default values
        /// </summary>
        public DefaultPlotModel()
        {
            Padding = new OxyThickness(Settings.Instance.Padding);
            DefaultFontSize = Settings.Instance.FontSize;
            DefaultFont = Settings.Instance.Font;
            PlotAreaBorderThickness = new OxyThickness(Settings.Instance.BorderThickness);
            TitleFontWeight = Settings.Instance.FontWeight;
            Background = OxyColors.Transparent;

            XAxis = new LinearAxisEx
            {
                Position = AxisPosition.Bottom,
                Key = "X",
                TitleFormatString = Settings.Instance.AxisFormat,
                AxislineThickness = Settings.Instance.AxisThickness
            };
            Axes.Add(XAxis);
            YAxis = new LinearAxisEx
            {
                Position = AxisPosition.Left,
                Key = "Y",
                TitleFormatString = Settings.Instance.AxisFormat,
                AxislineThickness = Settings.Instance.AxisThickness,
                IsZoomEnabled = true
            };
            SecondaryAxis = new LinearAxisEx
            {
                Position = AxisPosition.Top,
                Key = "TopX",
                TitleFormatString = Settings.Instance.AxisFormat,
                AxislineThickness = Settings.Instance.AxisThickness,
                IsZoomEnabled = true
            };
            Axes.Add(SecondaryAxis);
            SecondaryAxis.LinkedTo = XAxis;
            SecondaryAxis.IsInverted = true;

            if (PlotAreaBorderThickness.Equals(new OxyThickness(0)))
            {
                YAxis.AxislineStyle = LineStyle.Solid;
                XAxis.AxislineStyle = LineStyle.Solid;
            }
            Axes.Add(YAxis);
            Mapping = s => ((ChemSharp.DataPoint)s).Mapping(NormalizationFactor);
        }

        /// <summary>
        /// Sets up the Model with Spectrum Data
        /// </summary>
        public void SetUp(Spectrum spectrum)
        {
            //setup x axis 
            XAxis.Title = SecondaryAxis.Title = spectrum.Quantity();
            XAxis.Unit = SecondaryAxis.Unit = spectrum.Unit();
            XAxis.AbsoluteMinimum = spectrum.XYData.Min(s => s.X);
            XAxis.AbsoluteMaximum = spectrum.XYData.Max(s => s.X);
            //setup y axis
            YAxis.Title = spectrum.YQuantity();

            if (spectrum.IsNMRSpectrum()) XAxis.IsInverted = true;
            if (spectrum.IsEPRSpectrum() || spectrum.IsNMRSpectrum()) YAxis.IsVisible = false;
            if (spectrum.IsEPRSpectrum() || spectrum.IsUVSpectrum()) SecondaryAxis.SecondarySetUp(spectrum);
        }

        /// <summary>
        /// Zooms YAxis based on Spectrum
        /// </summary>
        public virtual void YAxisRefresh(bool zoom = true)
        {
            if (zoom)
            {
                var min = Minimum();
                var max = Maximum();
                min /= NormalizationFactor;
                max /= NormalizationFactor;
                YAxis.AbsoluteMinimum = min - max;
                YAxis.AbsoluteMaximum = max * 2;
                YAxis.Zoom(min - max * .1, max * 1.25);
            }
            InvalidatePlot(true);
        }

        /// <summary>
        /// Return Series Maximum
        /// </summary>
        /// <returns></returns>
        public double Maximum() => (from LineSeriesEx series in Series.Where(s => s.IsVisible) select series.ItemsSource.Cast<ChemSharp.DataPoint>().Max(s => s.Y)).Prepend(double.MinValue).Max();

        /// <summary>
        /// returns Series Minimum
        /// </summary>
        /// <returns></returns>
        public double Minimum() => (from LineSeriesEx series in Series.Where(s => s.IsVisible) select series.ItemsSource.Cast<ChemSharp.DataPoint>().Min(s => s.Y)).Prepend(double.MaxValue).Min();
    }
}
