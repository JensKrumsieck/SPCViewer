using ChemSharp.DataProviders;
using ChemSharp.Extensions;
using ChemSharp.Spectroscopy.DataProviders;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using SPCViewer.Core;
using SPCViewer.Core.Extension;
using SPCViewer.Core.Plots;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TinyMVVM.Command;
using OxyDataPoint = OxyPlot.DataPoint;

namespace SPCViewer.ViewModel
{
    /// <summary>
    /// Inherits the SpectrumBaseViewModel and handles non JSON Serializable Stuff
    /// </summary>
    public class SpectrumViewModel : SpectrumBaseViewModel
    {
        /// <summary>
        /// The used PlotModel
        /// </summary>
        public DefaultPlotModel Model { get; }

        /// <summary>
        /// The Series containing experimental data
        /// </summary>
        public LineSeries ExperimentalSeries { get; set; }

        /// <summary>
        /// The Series containing integrated data
        /// </summary>
        public LineSeries IntegralSeries { get; set; }

        /// <summary>
        /// The Series containing derived data
        /// </summary>
        public LineSeries DerivSeries { get; set; }

        /// <summary>
        /// List of Annotations
        /// </summary>
        public ObservableCollection<Annotation> Annotations { get; set; } = new ObservableCollection<Annotation>();

        /// <summary>
        /// Currently Active UI Action
        /// </summary>
        private UIAction _mouseAction;

        public UIAction MouseAction
        {
            get => _mouseAction;
            set
            {
                Set(ref _mouseAction, value);
                MouseActionChanged();
            }
        }

        private ICommand _deleteIntegral;
        /// <summary>
        /// Delete Command for Integrals
        /// </summary>
        public ICommand DeleteIntegral => _deleteIntegral ??= new RelayCommand<Integral>(param => Integrals.Remove(param));

        private ICommand _deletePeak;
        /// <summary>
        /// Delete Command for Integrals
        /// </summary>
        public ICommand DeletePeak => _deletePeak ??= new RelayCommand<Peak>(param => Peaks.Remove(param));

        private ICommand _updateIntegralCommand;
        /// <summary>
        /// Updates IntegralFactor Parameter
        /// </summary>
        public ICommand UpdateIntegralCommand =>
            _updateIntegralCommand ??= new RelayCommand<object[]>(param =>
            {
                var integral = (Integral)param[0];
                var value = (string)param[1];
                IntegralFactor = integral.RawValue / value.ToDouble();
                integral.EditIndicator = false;
            });

        /// <summary>
        /// Gets the PlotController
        /// </summary>
        public PlotController Controller { get; }

        /// <summary>
        /// Pass-through to Special Parameters
        /// </summary>
        public Dictionary<string, string> SpecialParameters => Spectrum.GetSpecialParameters();

        /// <summary>
        /// ctor with path given
        /// </summary>
        /// <param name="path"></param>
        public SpectrumViewModel(string path) : this(ExtensionHandler.Handle(path)) { }

        /// <summary>
        /// ctor with provider given
        /// </summary>
        /// <param name="provider"></param>
        public SpectrumViewModel(IXYDataProvider provider) : base(provider)
        {
            //init OxyPlot stuff
            Model = new DefaultPlotModel();
            Model.SetUp(Spectrum);
            Controller = PlotControls.DefaultController;
            MouseAction = UIAction.Zoom;
            InitSeries();
            //add annotation events
            Subscribe(Annotations, Model.Annotations, () => Model.InvalidatePlot(true));
            Subscribe(Peaks, Annotations,
                AnnotationUtil.PeakAnnotation,
                peak => Annotations.FirstOrDefault(s => s.Tag as Peak == peak));
            Subscribe(Integrals, Annotations,
                AnnotationUtil.IntegralAnnotation,
                integral => Annotations.FirstOrDefault(s => s.Tag as Integral == integral));
        }

        /// <summary>
        /// Initializes Series Data Binding
        /// </summary>
        private void InitSeries()
        {
            ExperimentalSeries = new LineSeries
            {
                ItemsSource = Spectrum.XYData,
                Mapping = Model.Mapping,
                StrokeThickness = Settings.Instance.SeriesThickness,
                Color = OxyColor.Parse(Settings.Instance.ExperimentalColor)
            };
            IntegralSeries = new LineSeries
            {
                ItemsSource = Spectrum.Integral,
                Mapping = Model.Mapping,
                IsVisible = false,
                StrokeThickness = Settings.Instance.SeriesThickness,
                Color = OxyColor.Parse(Settings.Instance.IntegralColor)
            };
            DerivSeries = new LineSeries
            {
                ItemsSource = Spectrum.Derivative,
                Mapping = Model.Mapping,
                IsVisible = false,
                StrokeThickness = Settings.Instance.SeriesThickness,
                Color = OxyColor.Parse(Settings.Instance.DerivativeColor)
            };
            //add series to model
            Model.Series.Add(ExperimentalSeries);
            Model.Series.Add(IntegralSeries);
            Model.Series.Add(DerivSeries);
            Model.YAxisZoom();
        }

        /// <summary>
        /// Adds an Integral to List
        /// </summary>
        /// <param name="rect"></param>
        private void AddIntegral((OxyDataPoint, OxyDataPoint) rect)
        {
            var points = Spectrum.XYData.PointsFromRect(rect);
            if (!points.Any()) return;
            if (Integrals.Count == 0) IntegralFactor = points.Integrate().Last().Y;
            Integrals.Add(new Integral(points)
            {
                Factor = IntegralFactor
            });
        }

        /// <summary>
        /// Adds Peaks to List
        /// </summary>
        /// <param name="rect"></param>
        private void AddPeak((OxyDataPoint, OxyDataPoint) rect)
        {
            var points = Spectrum.XYData.PointsFromRect(rect);
            if (!points.Any()) return;
            var epr = Spectrum.DataProvider is BrukerEPRProvider;
            var peaksIndices = points.Select(s => s.Y).ToList().FindPeakPositions(null, epr);
            foreach (var index in peaksIndices)
                if (!Peaks.Any(s => Math.Abs(s.X - points[index].X) < 1e-9)) 
                    Peaks.Add(new Peak(points[index]) { Factor = Model.NormalizationFactor });
        }

        /// <summary>
        /// Adds Peaks to List
        /// </summary>
        /// <param name="rect"></param>
        private void Normalize((OxyDataPoint, OxyDataPoint) rect)
        {
            var points = Spectrum.XYData.PointsFromRect(rect);
            if (!points.Any()) return;
            var max = points.Max(s => s.Y);
            //send normalization factor to model
            Model.NormalizationFactor = max;
            //send factor to peaks
            foreach (var peak in Peaks) peak.Factor = max;
            Model.YAxisZoom();
            Model.InvalidatePlot(true);
        }

        /// <summary>
        /// Pick value into peak list
        /// not sure if it stays peak list
        /// </summary>
        /// <param name="point"></param>
        private void PickValue(ScreenPoint point)
        {
            var odp = ExperimentalSeries.GetNearestPoint(point, false);
            var realDataPoint = Spectrum.XYData.FromOxyDataPoint(odp.DataPoint);
            if (!Peaks.Any(s => Math.Abs(s.X - point.X) < 1e-9))
                Peaks.Add(new Peak(realDataPoint) { Factor = Model.NormalizationFactor });
        }

        /// <summary>
        /// Fires when MouseAction changes
        /// </summary>
        private void MouseActionChanged()
        {
            var action = MouseAction switch
            {
                UIAction.Integrate => UIActions.PrepareRectangleAction(AddIntegral),
                UIAction.PeakPicking => UIActions.PrepareRectangleAction(AddPeak),
                UIAction.Normalize => UIActions.PrepareRectangleAction(Normalize),
                UIAction.PickValue => UIActions.PreparePickAction(PickValue),
                UIAction.Tracker => UIActions.PreparePickAction(null),
                _ => UIActions.PrepareRectangleAction(null)
            };
            Controller.BindMouseDown(OxyMouseButton.Left, action);
        }
    }
}