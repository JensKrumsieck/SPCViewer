using OxyPlot;
using OxyPlot.Axes;
using SPCViewer.Core.Extension;
using System;

namespace SPCViewer.Core.Plots
{
    public class DefaultPlotModel : PlotModel
    {
        public LinearAxis XAxis { get; }
        public LinearAxis YAxis { get; }

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

            XAxis = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Key = "X",
                TitleFormatString = Settings.Instance.AxisFormat,
                AxislineThickness = Settings.Instance.AxisThickness
            };
            Axes.Add(XAxis);
            YAxis = new LinearAxis()
            {
                Position = AxisPosition.Left,
                Key = "Y",
                TitleFormatString = Settings.Instance.AxisFormat,
                AxislineThickness = Settings.Instance.AxisThickness,
                IsZoomEnabled = true
            };
            if (PlotAreaBorderThickness.Equals(new OxyThickness(0)))
            {
                YAxis.AxislineStyle = LineStyle.Solid;
                XAxis.AxislineStyle = LineStyle.Solid;
            }
            Axes.Add(YAxis);
            Mapping = s => ((ChemSharp.DataPoint)s).Mapping(NormalizationFactor);
        }

        /// <summary>
        /// Inverts XAxis
        /// </summary>
        public void InvertX()
        {
            var start = XAxis.StartPosition;
            XAxis.StartPosition = XAxis.EndPosition;
            XAxis.EndPosition = start;
        }

        /// <summary>
        /// toggles Y Axis visibility
        /// </summary>
        public void DisableY()
        {
            YAxis.TickStyle = TickStyle.None;
            YAxis.LabelFormatter = d => null;
            YAxis.Title = null;
            YAxis.AxislineStyle = LineStyle.None;
        }
    }
}
