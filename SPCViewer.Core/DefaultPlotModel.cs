using OxyPlot;
using OxyPlot.Axes;

namespace SPCViewer.Core
{
    public class DefaultPlotModel : PlotModel
    {
        public LinearAxis XAxis { get; }
        public LinearAxis YAxis { get; }

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
                AxislineThickness = Settings.Instance.AxisThickness
            };
            if (PlotAreaBorderThickness.Equals(new OxyThickness(0)))
            {
                YAxis.AxislineStyle = LineStyle.Solid;
                XAxis.AxislineStyle = LineStyle.Solid;
            }
            Axes.Add(YAxis);
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
        public void ToggleY() => YAxis.IsAxisVisible = !YAxis.IsAxisVisible;
    }
}
