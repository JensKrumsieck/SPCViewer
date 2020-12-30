using SPCViewer.Core;
using SPCViewer.ViewModel;
using System.Windows;

namespace SPCViewer.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SpectrumViewModel ViewModel;

        public MainWindow()
        {
            Settings.Instance.Load();
            ViewModel = new SpectrumViewModel(@"D:\Dokumente\Projects\ChemSharp\ChemSharp.Tests\files\uvvis.dsw");
            DataContext = ViewModel;
            InitializeComponent();
        }
    }
}
