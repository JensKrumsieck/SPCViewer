using BluEPRint2.Utility;
using BluEPRint2.Plot;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Linq;
using System.Windows;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

//combining EPRSpectrum with some Oxyplot Functionality

namespace BluEPRint2.Spectrum
{
    public class PlottableEPRSpectrum : EPRSpectrum 
    {
        public List<double> g = new List<double>();
        public List<Tuple<DataPoint, DataPoint>> A = new List<Tuple<DataPoint, DataPoint>>();
        public bool showInt = false;
        public bool showDeriv = false;

        public PlottableEPRSpectrum(string file)
        : base(file)
        {
            this.convertToMT();
        }

        protected PlottableEPRSpectrum() { }

        public PlottableEPRSpectrum plotEPR(EPRPlotView plotView)
        {
            PlotModel pm = new PlotModel();
            pm.IsLegendVisible = true;
            pm.LegendPosition = LegendPosition.RightTop;
            pm.DefaultFontSize = 14;
            pm.LegendFontSize = 14;
            LineSeries se = new LineSeries
            {
                ItemsSource = BPUtil.getDataPoints(getX(), getY()),
                Title = this.getTitle()
            };
            LinearAxis x = new LinearAxis { Position = AxisPosition.Bottom, Title = "B/"+unit };

            pm.PlotAreaBorderThickness = new OxyThickness(1.5);
            x.MajorGridlineThickness = 1.5;
            x.MinorGridlineThickness = 1.5;

            x.AbsoluteMaximum = getX().Last();
            x.AbsoluteMinimum = getX().First();

            LinearAxis y = new LinearAxis { Position = AxisPosition.Left };
            y.IsAxisVisible = false;

            LinkedAxis g = new LinkedAxis(x, calcG, calcB);
            g.Position = AxisPosition.Top;
            g.Title = "g";
            g.MajorGridlineThickness = 1.5;
            g.MinorGridlineThickness = 1.5;

            pm.Series.Add(se);
            pm.Axes.Add(x);
            pm.Axes.Add(y);
            pm.Axes.Add(g);

            //rescale
            y.AbsoluteMaximum = getY().Max() + getY().Max() * 0.1;
            y.AbsoluteMinimum = getY().Min() + getY().Min() * 0.1;
            //full zoom out on y scale
            y.Zoom(y.AbsoluteMinimum, y.AbsoluteMaximum);
            plotView.InvalidatePlot();

            //sync
            bool isc = false;
            x.AxisChanged += (s, e) =>
            {
                if (isc) return;
                isc = true;
                g.Zoom(calcG(x.ActualMinimum), calcG(x.ActualMaximum));
                pm.InvalidatePlot(false);
                isc = false;
            };
            g.AxisChanged += (s, e) =>
            {
                if (isc) return;
                isc = true;
                x.Zoom(calcB(g.ActualMinimum), calcB(g.ActualMaximum));
                pm.InvalidatePlot(false);
                isc = false;
            };

            pm.MouseDown += (s, e) => Application.Current.Windows.OfType<MainWindow>().First().pm_MouseDown(s, e);
            pm.MouseMove += (s, e) => Application.Current.Windows.OfType<MainWindow>().First().pm_MouseMove(s, e);
            pm.MouseUp += (s, e) => Application.Current.Windows.OfType<MainWindow>().First().pm_MouseUp(s, e);
            plotView.Model = pm;
            plotView.spc = this;
            return this;
        }

        public void loadBP2 ()
        {
            if(File.Exists(this.getFileName() + ".bp2"))
            {
                string json = File.ReadAllText(this.getFileName() + ".bp2");
                PlottableEPRSpectrum spc_tmp = JsonConvert.DeserializeObject<PlottableEPRSpectrum>(json, new JsonSerializerSettings
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                });
                
                this.g = spc_tmp.g;
                this.A = spc_tmp.A;
            }
        }

        public void plotInt(EPRPlotView pw)
        {
            LineSeries se = new LineSeries
            {
                ItemsSource = BPUtil.getDataPoints(this.getX(), this.getIntegral()),
                Title = "Integral"
            };

            pw.Model.Series.Add(se);
            pw.InvalidatePlot();
            pw.rescale();
            
        }

        public void plotDeriv(EPRPlotView pw)
        {
            LineSeries se = new LineSeries
            {
                ItemsSource = BPUtil.getDataPoints(this.getX(), this.getDerivative()),
                Title = "Derivative"
            };

            pw.Model.Series.Add(se);
            pw.InvalidatePlot();
            pw.rescale();

        }

    }
}
