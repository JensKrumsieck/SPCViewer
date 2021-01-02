using OxyPlot;
using OxyPlot.Annotations;

namespace SPCViewer.Core.Extension
{
    public static class AnnotationUtil
    {
        /// <summary>
        /// Returns default PeakAnnotation Settings
        /// </summary>
        /// <param name="peak"></param>
        /// <returns></returns>
        public static ArrowAnnotation PeakAnnotation(Peak peak) =>
            new PeakAnnotation(peak)
            {
                StrokeThickness = Settings.Instance.AxisThickness,
            };

        /// <summary>
        /// Returns default IntegralAnnotation Settings
        /// </summary>
        /// <param name="integral"></param>
        /// <returns></returns>
        public static ArrowAnnotation IntegralAnnotation(Integral integral) =>
            new IntegralAnnotation(integral)
            {
                StrokeThickness = Settings.Instance.AxisThickness
            };

    }
}
