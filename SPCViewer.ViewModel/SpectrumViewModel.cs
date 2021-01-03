﻿using ChemSharp.Extensions;
using ChemSharp.Spectroscopy;
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
using System.Windows.Input;
using ChemSharp.Spectroscopy.DataProviders;
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

        private ICommand _deleteCommand = null!;
        /// <summary>
        /// Delete Command for Integral and Peaks
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                return _deleteCommand ??= new RelayCommand(param =>
                {
                    if (param is Peak peak)
                        Peaks.Remove(peak);
                    if (param is Integral integral)
                        Integrals.Remove(integral);
                }, param => true);
            }
        }

        private ICommand _updateIntegralCommand = null!;
        /// <summary>
        /// Updates IntegralFactor Parameter
        /// </summary>
        public ICommand UpdateIntegralCommand
        {
            get
            {
                return _updateIntegralCommand ??= new RelayCommand(param =>
                {
                    var values = (object[])param;
                    var integral = (Integral)values[0];
                    var value = (string)values[1];
                    IntegralFactor = integral.RawValue / value.ToDouble();
                    integral.EditIndicator = false;
                }, param => true);
            }
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="path"></param>
        public SpectrumViewModel(string path)
        {
            PropertyChanged += OnPropertyChanged;
            //load file and set up spectrum
            Spectrum = new Spectrum { DataProvider = ExtensionHandler.Handle(path) };
            //init OxyPlot stuff
            Model = new DefaultPlotModel();
            Model.SetUp(Spectrum);
            Controller = PlotControls.DefaultController;
            MouseAction = UIAction.Zoom;
            InitSeries();
            //add annotation events
            Annotations.CollectionChanged += AnnotationsOnCollectionChanged;
            Peaks.CollectionChanged += PeaksOnCollectionChanged;
            Integrals.CollectionChanged += IntegralsOnCollectionChanged;
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
            //add series to model
            Model.Series.Add(ExperimentalSeries);
            Model.Series.Add(IntegralSeries);
            Model.Series.Add(DerivSeries);
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
            var epr = Spectrum.DataProvider is BrukerEPRProvider;
            var peaksIndices = points.Select(s => s.Y).ToList().FindPeakPositions(0, epr);
            foreach (var index in peaksIndices)
                if (Peaks.Count(s => Math.Abs(s.X - points[index].X) < 1e-9) < 1) Peaks.Add(new Peak(points[index]) { Factor = Model.NormalizationFactor });
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
            Model.YAxisZoom(Spectrum);
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
                        if (s is PeakAnnotation a) return Math.Abs(a.Peak.X - peak.X) < 1e-9;
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
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}