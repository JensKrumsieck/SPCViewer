using SPCViewer.Core;
using SPCViewer.ViewModel;
using System.Windows;
using ChemSharp;
using MaterialDesignThemes.Wpf;

namespace SPCViewer.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel;

        public MainWindow()
        {
            Settings.Instance.Load("settings.json");
            //save here to update potential missing settings into file
            Settings.Instance.Save();
            ViewModel = new MainViewModel();
            DataContext = ViewModel;
            InitializeComponent();
        }

        /// <summary>
        /// Drag and Drop Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            var files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            ViewModel.OpenFiles(files);
        }

        /// <summary>
        /// Handles File Open by Dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_Click(object sender, RoutedEventArgs e) => ViewModel.OpenFiles(new[]
            {@"D:\Dokumente\Projects\ChemSharp\ChemSharp.Tests\files\uvvis.dsw"});

        private void Peak_OnClick(object sender, RoutedEventArgs e)
        {
            var chip = (Chip) e.Source;
            var dp = (DataPoint) chip.Tag;
            ViewModel.SelectedItem.Peaks.Remove(dp);
        }
    }
}
