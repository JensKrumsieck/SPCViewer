using ChemSharp.Extensions;
using ChemSharp.Spectroscopy;
using ChemSharp.Spectroscopy.DataProviders;
using ChemSharp.Spectroscopy.Extension;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using SPCViewer.Core;
using SPCViewer.Core.Extension;
using SPCViewer.Core.Plots;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using DataPoint = ChemSharp.DataPoint;
using OxyDataPoint = OxyPlot.DataPoint;
using ZoomRectangleManipulator = SPCViewer.Core.Plots.ZoomRectangleManipulator;

namespace SPCViewer.ViewModel
{
    public class SpectrumViewModel : INotifyPropertyChanged
    {
        private Spectrum _spectrum;
        /// <summary>
        /// Contains the shown spectrum
        /// </summary>
        public Spectrum Spectrum
        {
            get => _spectrum;
            set
            {
                _spectrum = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Pass Through the Spectrum's title
        /// </summary>
        public string Title => Path.GetFileName(Spectrum?.Title);

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
        /// List of Integrals
        /// </summary>
        public ObservableCollection<Integral> Integrals { get; set; } = new ObservableCollection<Integral>();
        
        private double _integralFactor = 1;
        /// <summary>
        /// Integral factor used for Normalization
        /// </summary>
        public double IntegralFactor
        {
            get => _integralFactor;
            set
            {
                _integralFactor = value; 
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// List of Peaks
        /// </summary>
        public ObservableCollection<Peak> Peaks { get; set; } =
            new ObservableCollection<Peak>();

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
                _mouseAction = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="path"></param>
        public SpectrumViewModel(string path)
        {
            PropertyChanged += OnPropertyChanged;

            var provider = ExtensionHandler.Handle(path);
            Spectrum = new Spectrum { DataProvider = provider };
            Model = new DefaultPlotModel();
            Controller = PlotControls.DefaultController;
            MouseAction = UIAction.Zoom;
            //add annotation events
            Annotations.CollectionChanged += AnnotationsOnCollectionChanged;
            Peaks.CollectionChanged += PeaksOnCollectionChanged;
            Integrals.CollectionChanged += IntegralsOnCollectionChanged;
            //init oxyplot stuff
            InitSeries();
            InitModel();
        }

        /// <summary>
        /// Initializes PlotModel
        /// </summary>
        private void InitModel()
        {
            Model.Title = Path.GetFileName(Spectrum.Title);
            //setup x axis 
            Model.XAxis.Title = Spectrum.Quantity();
            Model.XAxis.Unit = Spectrum.Unit();
            Model.XAxis.AbsoluteMinimum = Spectrum.XYData.Min(s => s.X);
            Model.XAxis.AbsoluteMaximum = Spectrum.XYData.Max(s => s.X);
            //setup y axis
            Model.YAxis.Title = Spectrum.YQuantity();
            var min = Spectrum.XYData.Min(s => s.Y);
            var max = Spectrum.XYData.Max(s => s.Y);
            Model.YAxis.AbsoluteMinimum = min - max * 0.5;
            Model.YAxis.AbsoluteMaximum = max * 1.5;
            Model.YAxis.Zoom(min - max * .1, max * 1.1);

            if (Spectrum.DataProvider is BrukerNMRProvider) Model.InvertX();
            if (Spectrum.DataProvider is BrukerEPRProvider || Spectrum.DataProvider is BrukerNMRProvider) Model.DisableY();

            Model.Series.Add(ExperimentalSeries);
            Model.Series.Add(IntegralSeries);
            Model.Series.Add(DerivSeries);
        }

        /// <summary>
        /// Initializes Series Data Binding
        /// </summary>
        private void InitSeries()
        {
            ExperimentalSeries = new LineSeries
            {
                ItemsSource = Spectrum.XYData,
                Mapping = Model.Mapping
            };
            IntegralSeries = new LineSeries
            {
                ItemsSource = Spectrum.Integral,
                Mapping = Model.Mapping,
                IsVisible = false
            };
            DerivSeries = new LineSeries
            {
                ItemsSource = Spectrum.Derivative,
                Mapping = Model.Mapping,
                IsVisible = false
            };
        }

        /// <summary>
        /// Gets the PlotController
        /// </summary>
        public PlotController Controller { get; }

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
            var peaksIndices = points.Select(s => s.Y).ToList().FindPeakPositions();
            foreach (var index in peaksIndices)
                if (Peaks.Count(s => s.X - points[index].X < 1e-6 ) < 1) Peaks.Add(new Peak(points[index]));
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
            Model.InvalidatePlot(true);
        }


        #region EventStuff
        /// <summary>
        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged" />
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MouseAction):
                {
                    //bind action to plot controller
                    Action<(OxyDataPoint, OxyDataPoint)> rectAction = MouseAction switch
                    {
                        UIAction.Integrate => AddIntegral,
                        UIAction.PeakPicking => AddPeak,
                        UIAction.Normalize => Normalize,
                        _ => null
                    };
                    var action = new DelegatePlotCommand<OxyMouseDownEventArgs>((view, controller, args) =>
                        controller.AddMouseManipulator(view, new ZoomRectangleManipulator(view, rectAction), args));
                    Controller.BindMouseDown(OxyMouseButton.Left, action);
                    break;
                }
                case nameof(IntegralFactor):
                {
                    foreach (var integral in Integrals) integral.Factor = IntegralFactor;
                    break;
                }
            }
        }

        /// <summary>
        /// Handles changing in Annotations Collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnnotationsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Handles Sync to Model Annotations as you can't bind to it...
            if (e.NewItems != null)
                foreach (Annotation annotation in e.NewItems)
                    Model.Annotations.Add(annotation);
            if (e.OldItems != null)
                foreach (Annotation annotation in e.OldItems)
                    Model.Annotations.Remove(annotation);
            //Redraw
            Model.InvalidatePlot(true);
        }

        /// <summary>
        /// Do a collection changed here to sync annotations to peaks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PeaksOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var maxY = Spectrum.XYData.Max(s => s.Y);
            if (e.NewItems != null)
                foreach (Peak peak in e.NewItems)
                    Annotations.Add(AnnotationUtil.PeakAnnotation(peak));
            if (e.OldItems != null)
                foreach (Peak peak in e.OldItems)
                {
                    var an = Annotations.FirstOrDefault(s =>
                    {
                        if (s is PeakAnnotation a) return a.Peak.X - peak.X <= 1e-6; 
                        return false;
                    });
                    Annotations.Remove(an);
                }
            Model.InvalidatePlot(true);
        }

        /// <summary>
        /// Do a collection changed here to sync annotations to integrals
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IntegralsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (Integral integral in e.NewItems)
                    Annotations.Add(AnnotationUtil.IntegralAnnotation(integral));
            if (e.OldItems != null)
                foreach (Integral integral in e.OldItems)
                {
                    var an = Annotations.FirstOrDefault(s => s.Tag as Integral == integral);
                    Annotations.Remove(an);
                }
            Model.InvalidatePlot(true);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged" />
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}