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
    }
}
