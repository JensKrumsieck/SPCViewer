using System.Collections.Generic;
using System.Windows;
using BluEPRint2.Spectrum;
using BluEPRint2.Utility;

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

        private void Save()
        {
            if (this.CB_Spectra.SelectedIndex != 0)
            {
                FileHandler.HandleSave((PlottableEPRSpectrum)this.CB_Spectra.SelectedItem, (FileHandler.Format)this.CB_Format.SelectedIndex);
            }
            else
            {
                //save all
                foreach (PlottableEPRSpectrum spc in this.EPRSpectra)
                {
                    FileHandler.HandleSave(spc, (FileHandler.Format)this.CB_Format.SelectedIndex);
                }
            }
        }

        private void SvBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Save();
        }

        private void SvcBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Save();
            this.Close();
        }
    }
}
