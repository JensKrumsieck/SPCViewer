using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using winforms = System.Windows.Forms;

namespace BluEPRint2
{
    /// <summary>
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            //try search easyspin if not setup
            if (Properties.Settings.Default.es_path == "")
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (Directory.Exists(path + @"\MATLAB"))
                {
                    path = path + @"\MATLAB\";
                }
                string[] files = Directory.GetFiles(path, "easyspin.m", SearchOption.AllDirectories);
                if (files.Length != 0)
                {
                    Properties.Settings.Default.es_path = System.IO.Path.GetDirectoryName(files[0]);
                }
            }

            if (Properties.Settings.Default.load_path == "")
            {
                Properties.Settings.Default.load_path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            if (Properties.Settings.Default.es_plug == "")
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"ESPlug.dll"))
                {
                    Properties.Settings.Default.es_plug = AppDomain.CurrentDomain.BaseDirectory + @"ESPlug.dll";
                }
            }
            //save new settings
            Properties.Settings.Default.Save();
            espathTB.Text = Properties.Settings.Default.es_path;
            esplugTB.Text = Properties.Settings.Default.es_plug;
            startPathTB.Text = Properties.Settings.Default.load_path;
            ToggleAutoLoad.IsChecked = Properties.Settings.Default.auto_reload;
        }

        private void SearchESBtn_Click(object sender, RoutedEventArgs e)
        {
            string initialDir = AppDomain.CurrentDomain.BaseDirectory;
            if (Properties.Settings.Default.es_plug != "") initialDir = Properties.Settings.Default.es_plug;
            OpenFileDialog ofd = new OpenFileDialog
            {
                InitialDirectory = initialDir,
                Filter = "ESPlug.dll|ESPlug.dll",
                RestoreDirectory = true
            };
            var DialogResult = ofd.ShowDialog();
            if (DialogResult.HasValue && DialogResult.Value)
            {
                esplugTB.Text = ofd.FileName;
            }

        }
        private void SearchESPathBtn_Click(object sender, RoutedEventArgs e)
        {
            string initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (Directory.Exists(initialDir + @"\MATLAB"))
            {
                initialDir = initialDir + @"\MATLAB\";
            }

            if (Properties.Settings.Default.es_path != "")
            {
                initialDir = Properties.Settings.Default.es_path;
            }

            OpenFileDialog ofd = new OpenFileDialog
            {
                InitialDirectory = initialDir,
                Filter = "easyspin.m|easyspin.m",
                RestoreDirectory = true
            };
            var DialogResult = ofd.ShowDialog();
            if (DialogResult.HasValue && DialogResult.Value)
            {
                espathTB.Text = System.IO.Path.GetDirectoryName(ofd.FileName);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.es_path = espathTB.Text;
            Properties.Settings.Default.es_plug = esplugTB.Text;
            Properties.Settings.Default.load_path = startPathTB.Text;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.auto_reload = (bool)ToggleAutoLoad.IsChecked;
            this.Close();
        }

        private void SearchStartPathBtn_Click(object sender, RoutedEventArgs e)
        {
            string initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (Properties.Settings.Default.load_path != "") initialDir = Properties.Settings.Default.load_path;
            winforms.FolderBrowserDialog fbd = new winforms.FolderBrowserDialog();
            fbd.SelectedPath = initialDir;
            if(fbd.ShowDialog() == winforms.DialogResult.OK)
            {
                startPathTB.Text = fbd.SelectedPath;
            }
        }

    }
}
