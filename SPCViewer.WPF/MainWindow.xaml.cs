using SPCViewer.Core;
using SPCViewer.ViewModel;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using static ChemSharp.Files.FileHandler;
using static SPCViewer.Core.ExtensionHandler;

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
            Settings.Instance.Load($"{AppDomain.CurrentDomain.BaseDirectory}/settings.json");
            //save here to update potential missing settings into file
            Settings.Instance.Save();
            ViewModel = new MainViewModel();
            DataContext = ViewModel;
            InitializeComponent();
            HandleArgs();
        }

        /// <summary>
        /// Handles stored command line args
        /// </summary>
        private void HandleArgs()
        {
            var app = Application.Current;
            if (app.Properties["args"] == null) return;
            var files = ((string[])app.Properties["args"])
                ?.Where(File.Exists)
                ?.Where(s => RecipeDictionary.ContainsKey(GetExtension(s)));
            ViewModel.OpenFiles(files.ToArray());
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
        {
            @"D:\Dokumente\Projects\ChemSharp\ChemSharp.Tests\files\epr.par",
            @"D:\Dokumente\Projects\ChemSharp\ChemSharp.Tests\files\uvvis.dsw",
            @"D:\Dokumente\Projects\ChemSharp\ChemSharp.Tests\files\nmr\fid"
        });
    }
}
