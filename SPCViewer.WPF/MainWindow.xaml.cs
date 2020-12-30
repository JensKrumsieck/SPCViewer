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
            if (files == null) return;
            foreach (var file in files)
            {
                var page = new SpectrumViewModel(file);
                ViewModel.TabItems.Add(page);
                ViewModel.SelectedIndex = ViewModel.TabItems.IndexOf(page);
            }
        }
    }
}
