using ChemSharp.DataProviders;
using ChemSharp.Extensions;
using OxyPlot;
using OxyPlot.Annotations;
using SPCViewer.Core;
using SPCViewer.Core.Extension;
using SPCViewer.Core.Plots;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TinyMVVM.Command;

namespace SPCViewer.ViewModel
{
    /// <summary>
    /// Inherits the SpectrumBaseViewModel and handles non JSON Serializable Stuff
    /// </summary>
    public class SpectrumViewModel : SpectrumBaseViewModel
    {
        /// <summary>
        /// The Series containing experimental data
        /// </summary>
        public LineSeriesEx ExperimentalSeries { get; set; }

        /// <summary>
        /// The Series containing integrated data
        /// </summary>
        public LineSeriesEx IntegralSeries { get; set; }

        /// <summary>
        /// The Series containing derived data
        /// </summary>
        public LineSeriesEx DerivSeries { get; set; }

        private double _strokeThickness = Settings.Instance.SeriesThickness;
        /// <summary>
        /// all Series StrokeThickness
        /// </summary>
        public double StrokeThickness
        {
            get => _strokeThickness;
            set => Set(ref _strokeThickness, value, () =>
            {
                ExperimentalSeries.StrokeThickness = DerivSeries.StrokeThickness = IntegralSeries.StrokeThickness = value;
                Parent.Model.InvalidatePlot(true);
            });
        }

        private OxyColor _color = OxyColor.Parse(Settings.Instance.ExperimentalColor);
        /// <summary>
        /// Color of Experimental value
        /// </summary>
        public OxyColor Color
        {
            get => _color;
            set => Set(ref _color, value, () =>
            {
                ExperimentalSeries.Color = Color;
                Parent.Model.InvalidatePlot(true);
            });
        }

        /// <summary>
        /// List of Annotations
        /// </summary>
        public ObservableCollection<Annotation> Annotations { get; set; } = new ObservableCollection<Annotation>();

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
        /// Pass-through to Special Parameters
        /// </summary>
        public Dictionary<string, string> SpecialParameters => Spectrum.GetSpecialParameters();

        /// <summary>
        /// ctor with path given
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="path"></param>
        public SpectrumViewModel(DocumentViewModel parent, string path) : this(parent, ExtensionHandler.Handle(path)) { }

        /// <summary>
        /// ctor with provider given
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="provider"></param>
        public SpectrumViewModel(DocumentViewModel parent, IXYDataProvider provider) : base(parent, provider)
        {
            //init OxyPlot stuff
            Parent.Model.SetUp(Spectrum);
            InitSeries();
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
            ExperimentalSeries = new LineSeriesEx
            {
                ItemsSource = Spectrum.XYData,
                Mapping = Parent.Model.Mapping,
                StrokeThickness = StrokeThickness,
                Color = OxyColor.Parse(Settings.Instance.ExperimentalColor)
            };
            IntegralSeries = new LineSeriesEx
            {
                ItemsSource = Spectrum.Integral,
                Mapping = Parent.Model.Mapping,
                IsVisible = false,
                StrokeThickness = StrokeThickness,
                Color = OxyColor.Parse(Settings.Instance.IntegralColor)
            };
            DerivSeries = new LineSeriesEx
            {
                ItemsSource = Spectrum.Derivative,
                Mapping = Parent.Model.Mapping,
                IsVisible = false,
                StrokeThickness = StrokeThickness,
                Color = OxyColor.Parse(Settings.Instance.DerivativeColor)
            };
            //add series to model
            Parent.Model.Series.Add(ExperimentalSeries);
            Parent.Model.Series.Add(IntegralSeries);
            Parent.Model.Series.Add(DerivSeries);
            Parent.Model.YAxisRefresh();
        }
    }
}