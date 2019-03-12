using System.Collections.Generic;
using System.Windows;
using BluEPRint2.Spectrum;

namespace BluEPRint2
{
    /// <summary>
    /// Interaktionslogik für SaveWindow.xaml
    /// </summary>
    public partial class SaveWindow : Window
    {
        List<PlottableEPRSpectrum> EPRSpectra;
        public SaveWindow(List<PlottableEPRSpectrum> Spectra)
        {
            InitializeComponent();
            this.EPRSpectra = Spectra;
            foreach(PlottableEPRSpectrum spc in this.EPRSpectra)
            {
                this.CB_Spectra.Items.Add(spc);
            }
        }
    }
}
