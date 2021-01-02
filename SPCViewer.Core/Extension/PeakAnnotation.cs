using OxyPlot;
using OxyPlot.Annotations;

namespace SPCViewer.Core.Extension
{
    public class PeakAnnotation : ArrowAnnotation
    {
        /// <summary>
        /// The picked Peak
        /// </summary>
        public DataPoint Peak { get; set; }

        public PeakAnnotation(DataPoint peak)
        {
            Peak = peak;
            HeadLength = 0;
            TextRotation = -90;
            Tag = peak;
            Text = peak.X.ToString("N2");
        }

        /// <inheritdoc/>
        public override void Render(IRenderContext rc)
        {
            var dataHeight = InverseTransform(new ScreenPoint(0, -PlotModel.PlotArea.Height));
            var height = dataHeight.Y;

            StartPoint = new DataPoint(Peak.X, Peak.Y + height * .05);
            EndPoint = new DataPoint(Peak.X, Peak.Y + height * .01);

            var top = PlotModel.PlotArea.Top;

            var tmp = Transform(StartPoint);
            var textScreenPoint = new ScreenPoint(tmp.X, top + rc.MeasureText(Text, ActualFont, ActualFontSize, ActualFontWeight, TextRotation).Height);
            TextPosition = InverseTransform(textScreenPoint);
            
            base.Render(rc);
        }
    }
}
