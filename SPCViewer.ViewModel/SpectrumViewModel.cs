using ChemSharp.Spectroscopy;
using ChemSharp.Spectroscopy.DataProviders;
using ChemSharp.Spectroscopy.Extension;
using OxyPlot;
using OxyPlot.Series;
using SPCViewer.Core;
using SPCViewer.Core.Extension;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

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
        /// Pass Through the Spectrums title
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
        /// ctor
        /// </summary>
        /// <param name="path"></param>
        public SpectrumViewModel(string path)
        {
            var provider = ExtensionHandler.Handle(path);
            Spectrum = new Spectrum() { DataProvider = provider };
            Model = new DefaultPlotModel();
            InitSeries();
            InitModel();
            Model.Series.Add(ExperimentalSeries);
            Model.Series.Add(IntegralSeries);
            Model.Series.Add(DerivSeries);
        }

        private void InitModel()
        {
            Model.Title = Path.GetFileName(Spectrum.Title);
            if (Spectrum.DataProvider is BrukerNMRProvider) Model.InvertX();
            if (Spectrum.DataProvider is BrukerEPRProvider || Spectrum.DataProvider is BrukerNMRProvider) Model.ToggleY();
            //setup x axis 
            Model.XAxis.Title = Spectrum.Quantity();
            Model.XAxis.Unit = Spectrum.Unit();
            Model.XAxis.AbsoluteMinimum = Spectrum.XYData.Min(s => s.X);
            Model.XAxis.AbsoluteMaximum = Spectrum.XYData.Max(s => s.X);
            //setup y axis
            Model.YAxis.Title = Spectrum.YQuantity();
        }

        /// <summary>
        /// Initializes Series Data Binding
        /// </summary>
        private void InitSeries()
        {
            ExperimentalSeries = new LineSeries
            {
                ItemsSource = Spectrum.XYData,
                Mapping = s => ((ChemSharp.DataPoint)s).Mapping()
            };
            IntegralSeries = new LineSeries
            {
                ItemsSource = Spectrum.Integral,
                Mapping = s => ((ChemSharp.DataPoint)s).Mapping(),
                IsVisible = false
            };
            DerivSeries = new LineSeries()
            {
                ItemsSource = Spectrum.Derivative,
                Mapping = s => ((ChemSharp.DataPoint)s).Mapping(),
                IsVisible = false
            };
        }

        /// <summary>
        /// Gets the PlotController
        /// </summary>
        public PlotController Controller => PlotControls.DefaultController;

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
    }
}
