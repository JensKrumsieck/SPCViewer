using System.ComponentModel;
using OxyPlot;
using OxyPlot.Annotations;

namespace SPCViewer.Core.Extension
{
    public class PeakAnnotation : ArrowAnnotation
    {
        /// <summary>
        /// The picked Peak
        /// </summary>
        public Peak Peak { get; set; }
        
        public PeakAnnotation(Peak peak)
        {
            Peak = peak;
            HeadLength = 0;
            TextRotation = -90;
            Tag = Peak;
            Text = Peak.X.ToString("N2");
            Peak.PropertyChanged += PeakOnPropertyChanged;
        }

        /// <summary>
        /// Refresh Annotation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PeakOnPropertyChanged(object sender, PropertyChangedEventArgs e) => PlotModel.InvalidatePlot(true);

        /// <inheritdoc/>
        public override void Render(IRenderContext rc)
        {
            var dataHeight = InverseTransform(new ScreenPoint(0, -PlotModel.PlotArea.Height));
            var height = dataHeight.Y;

            StartPoint = new DataPoint(Peak.X, Peak.Value + height * .025);
            EndPoint = new DataPoint(Peak.X, Peak.Value + height * .01);

            var top = PlotModel.PlotArea.Top;

            var tmp = Transform(StartPoint);
            var textScreenPoint = new ScreenPoint(tmp.X, top + rc.MeasureText(Text, ActualFont, ActualFontSize, ActualFontWeight, TextRotation).Height);
            TextPosition = InverseTransform(textScreenPoint);
            
            base.Render(rc);
        }
    }
}
