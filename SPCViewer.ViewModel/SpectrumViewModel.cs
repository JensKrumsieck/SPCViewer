using ChemSharp.Spectroscopy;
using ChemSharp.Spectroscopy.DataProviders;
using OxyPlot.Series;
using SPCViewer.Core;
using SPCViewer.Core.Extension;
using System.ComponentModel;
using System.IO;
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
        /// The used PlotModel
        /// </summary>
        public DefaultPlotModel Model { get; }

        /// <summary>
        /// The Series containing experimental data
        /// </summary>
        public LineSeries ExperimentalSeries { get; set; }


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
        }

        private void InitModel()
        {
            Model.Title = Path.GetFileName(Spectrum.Title);
            if (Spectrum.DataProvider is BrukerNMRProvider) Model.InvertX();
            if (Spectrum.DataProvider is BrukerEPRProvider || Spectrum.DataProvider is BrukerNMRProvider) Model.ToggleY();
            //setup x axis description
            Model.XAxis.Title = Spectrum.Quantity();
            Model.XAxis.Unit = Spectrum.Unit();
        }

        /// <summary>
        /// Initializes Series Data Binding
        /// </summary>
        private void InitSeries() =>
            ExperimentalSeries = new LineSeries
            {
                ItemsSource = Spectrum.XYData,
                Mapping = s => ((ChemSharp.DataPoint)s).Mapping()
            };


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
