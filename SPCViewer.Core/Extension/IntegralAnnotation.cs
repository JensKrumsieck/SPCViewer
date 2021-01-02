using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using System.Linq;

namespace SPCViewer.Core.Extension
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
            Text = integral.Value.ToString("N1");
        }


        /// <inheritdoc />
        public override void Render(IRenderContext rc)
        {
            var dataHeight = InverseTransform(new ScreenPoint(0, -PlotModel.PlotArea.Height));
            var height = dataHeight.Y;

            var series = PlotModel.Series[0] as LineSeries;
            var data =series?.ItemsSource;
            var lowest = data?.Cast<ChemSharp.DataPoint>().Min(s => s.Y) ?? 0d;

            StartPoint = new DataPoint(Integral.From,  lowest - height *0.01);
            EndPoint = new DataPoint(Integral.To,  lowest - height * 0.01);

            //text in screen space
            var textUpper = Transform(new DataPoint((Integral.To + Integral.From) / 2, lowest));
            var textHeight = rc.MeasureText(Text, ActualFont, ActualFontSize, ActualFontWeight, TextRotation).Height;

            TextPosition = InverseTransform(new ScreenPoint(textUpper.X, textUpper.Y + textHeight));

            base.Render(rc);
        }
    }
}
