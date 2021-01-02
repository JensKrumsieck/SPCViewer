using OxyPlot;
using OxyPlot.Annotations;

namespace SPCViewer.Core.Extension
{
    public static class AnnotationUtil
    {
        /// <summary>
        /// Returns default PeakAnnotation Settings
        /// </summary>
        /// <param name="point"></param>
        /// <param name="maxY"></param>
        /// <returns></returns>
        public static ArrowAnnotation PeakAnnotation(ChemSharp.DataPoint point, double maxY) =>
            new ArrowAnnotation()
            {
                StartPoint = new DataPoint(point.X, point.Y + .1),
                EndPoint = new DataPoint(point.X, point.Y + 1),
                Text = point.X.ToString("N3"),
                StrokeThickness = Settings.Instance.AxisThickness,
                HeadLength = 0,
                TextRotation = -90,
                TextPosition = new DataPoint(point.X, maxY),
                Tag = point
            };

        /// <summary>
        /// Returns default IntegralAnnotation Settings
        /// </summary>
        /// <param name="integral"></param>
        /// <returns></returns>
        public static ArrowAnnotation IntegralAnnotation(Integral integral) =>
            new ArrowAnnotation()
            {
                StartPoint = new DataPoint(integral.From, -.1),
                EndPoint = new DataPoint(integral.To, -.1),
                TextPosition = new DataPoint((integral.From + integral.To) / 2, -.6),
                Text = integral.Value.ToString("N3"),
                Tag = integral,
                StrokeThickness = Settings.Instance.AxisThickness,
                HeadLength = 0
            };

    }
}
