using ChemSharp.DataProviders;
using ChemSharp.Extensions;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using SPCViewer.Core;
using SPCViewer.Core.Extension;
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
        public DocumentViewModel Parent { get; }

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
        public SpectrumViewModel(DocumentViewModel parent, string path) : this(parent,ExtensionHandler.Handle(path)) { }

        /// <summary>
        /// ctor with provider given
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="provider"></param>
        public SpectrumViewModel(DocumentViewModel parent, IXYDataProvider provider) : base(provider)
        {
            Parent = parent;
            Parent.SelectedIndexChanged += (s, e) => OnPropertyChanged(nameof(IsSelected));
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
            ExperimentalSeries = new LineSeries
            {
                ItemsSource = Spectrum.XYData,
                Mapping = Parent.Model.Mapping,
                StrokeThickness = Settings.Instance.SeriesThickness,
                Color = OxyColor.Parse(Settings.Instance.ExperimentalColor)
            };
            IntegralSeries = new LineSeries
            {
                ItemsSource = Spectrum.Integral,
                Mapping = Parent.Model.Mapping,
                IsVisible = false,
                StrokeThickness = Settings.Instance.SeriesThickness,
                Color = OxyColor.Parse(Settings.Instance.IntegralColor)
            };
            DerivSeries = new LineSeries
            {
                ItemsSource = Spectrum.Derivative,
                Mapping = Parent.Model.Mapping,
                IsVisible = false,
                StrokeThickness = Settings.Instance.SeriesThickness,
                Color = OxyColor.Parse(Settings.Instance.DerivativeColor)
            };
            //add series to model
            Parent.Model.Series.Add(ExperimentalSeries);
            Parent.Model.Series.Add(IntegralSeries);
            Parent.Model.Series.Add(DerivSeries);
            Parent.Model.YAxisRefresh();
        }

        /// <summary>
        /// Indicates if this spectrum is selected
        /// </summary>
        public bool IsSelected => Parent.SelectedItem == this;
    }
}