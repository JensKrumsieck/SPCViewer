using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using System.ComponentModel;
using System.Linq;

namespace SPCViewer.Core.Plots
{
    public class IntegralAnnotation : ArrowAnnotation
    {
        /// <summary>
        /// The picked Integral
        /// </summary>
        public Integral Integral { get; set; }

        public IntegralAnnotation(Integral integral)
        {
            Integral = integral;
            HeadLength = 0;
            Tag = integral;
            Text = integral.Value.ToString("N2");
            Integral.PropertyChanged += IntegralOnPropertyChanged;
        }

        /// <summary>
        /// Refresh Text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IntegralOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Integral.Value)) return;
            Text = Integral.Value.ToString("N2");
            PlotModel.InvalidatePlot(true);
        }

        /// <inheritdoc />
        public override void Render(IRenderContext rc)
        {
            var dataHeight = InverseTransform(new ScreenPoint(0, -PlotModel.PlotArea.Height));
            var height = dataHeight.Y;

            var series = PlotModel.Series[0] as LineSeries;
            var data = series?.ItemsSource;
            var lowest = data?.Cast<ChemSharp.DataPoint>().Min(s => s.Y) ?? 0d;
            lowest /= ((DefaultPlotModel) PlotModel).NormalizationFactor;

            StartPoint = new DataPoint(Integral.From, lowest - height * 0.005);
            EndPoint = new DataPoint(Integral.To, lowest - height * 0.005);

            //text in screen space
            var textUpper = Transform(new DataPoint((Integral.To + Integral.From) / 2, lowest));
            var textHeight = rc.MeasureText(Text, ActualFont, ActualFontSize, ActualFontWeight, TextRotation).Height;

            TextPosition = InverseTransform(new ScreenPoint(textUpper.X, textUpper.Y + textHeight));

            base.Render(rc);
        }
    }
}
