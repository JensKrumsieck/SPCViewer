using BluEPRint2.Spectrum;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot;
using System.Collections.Generic;
using System.Linq;
using BluEPRint2.Utility;

namespace BluEPRint2.Plot
{
    public class EPRPlotView : OxyPlot.Wpf.PlotView
    {
        public PlottableEPRSpectrum spc;

        public void rescale()
        {
            double maxY = 0;
            double minY = 0;
            foreach (LineSeries s in this.Model.Series)
            {
                //in some cases s.maxY/minY are not yet set... :/
                List<EPRDataPoint> dp = (List<EPRDataPoint>)s.ItemsSource;
                List<double> y = new List<double>();
                foreach(EPRDataPoint p in dp)
                {
                    y.Add(p.Y);
                }
                if (maxY < y.Max()) maxY = y.Max();
                if (minY > y.Min()) minY = y.Min();
            }
            maxY = maxY + maxY * 0.1;
            minY = minY + minY * 0.1;
            //rescale
            LinearAxis ya = (LinearAxis)this.Model.Axes[1];
            ya.AbsoluteMaximum = maxY;
            ya.AbsoluteMinimum = minY;
            //full zoom out on y scale
            ya.Zoom(ya.AbsoluteMinimum, ya.AbsoluteMaximum);
            this.InvalidatePlot();
        }
    }
}
