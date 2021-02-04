using ChemSharp.Spectroscopy;
using SPCViewer.Core;
using System.Collections.ObjectModel;
using TinyMVVM;

namespace SPCViewer.ViewModel
{
    public abstract class SpectrumBaseViewModel : ListItemViewModel<DocumentViewModel, SpectrumViewModel>
    {
        private Spectrum _spectrum;
        /// <summary>
        /// Contains the shown spectrum
        /// </summary>
        public Spectrum Spectrum
        {
            get => _spectrum;
            set => Set(ref _spectrum, value);
        }

        /// <summary>
        /// Pass Through the Spectrum's title
        /// </summary>
        public override string Title => System.IO.Path.GetFileName(Spectrum?.Title);

        /// <summary>
        /// Gets or inits the File Path
        /// </summary>
        public string Path { get; init; }

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
            set => Set(ref _integralFactor, value, UpdateIntegrals);
        }

        /// <summary>
        /// List of Peaks
        /// </summary>
        public ObservableCollection<Peak> Peaks { get; set; } =
            new ObservableCollection<Peak>();

        protected SpectrumBaseViewModel(DocumentViewModel parent, Spectrum spc) : base(parent)
        {
            //load file and set up spectrum
            Spectrum = spc;
        }

        protected SpectrumBaseViewModel(DocumentViewModel parent, string path) : this(parent, SpectrumFactory.Create(path))
        {
            Path = path;
        }

        /// <summary>
        /// Fires when IntegralFactor changes
        /// </summary>
        private void UpdateIntegrals()
        {
            foreach (var integral in Integrals)
                integral.Factor = IntegralFactor;
        }
    }
}
