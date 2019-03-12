using BluEPRint2.Plot;
using BluEPRint2.Spectrum;
using BluEPRint2.Utility;
using Microsoft.Win32;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace BluEPRint2
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool g_isPicking = false;
        private bool coupling_isEnabled = false;
        private bool coupling_isMeasure = false;
        private DataPoint coupling_start = new DataPoint(0, 0);
        private DataPoint coupling_end = new DataPoint (0,0);
        private DataPoint mousePosition = new DataPoint(0, 0);

        public MainWindow()
        {
            InitializeComponent();
            this.Title = "BluEPRint 2";
            //clear tab menu
            mdiTabMenu.Items.Clear();
            mdiTabMenu.AllowDrop = true;

            //load last session if enabled in options
            if(File.Exists(AppDomain.CurrentDomain.BaseDirectory + "last.session") && Properties.Settings.Default.auto_reload)
            {
                List<string> Lines = File.ReadLines(AppDomain.CurrentDomain.BaseDirectory + "last.session").ToList<string>();
                foreach(string line in Lines)
                {
                    this.HandleFileLoad(line);
                }
            }
        }


 
        private void createMDIChild(PlottableEPRSpectrum spc)
        {
            //create inner stackpanel
            StackPanel side = new StackPanel();
            side.Orientation = Orientation.Vertical;
            
            //create textbox
            TextBlock tb = new TextBlock();
            tb.Padding = new Thickness(5);
            tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            tb.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            tb.MinHeight = 200;
            tb.Inlines.Add(new Bold(new Run(spc.getTitle())));
            tb.Inlines.Add("\nFrequency: " + spc.freq.ToString() + " GHz");
            tb.Inlines.Add("\nResolution: " + spc.getX().Length);
            tb.Inlines.Add("\nField: " + spc.getX().First() + " - " + spc.getX().Last() + " " + spc.unit);
            tb.Inlines.Add("\nMicrowave Power: " + spc.power + " mW");
            tb.Inlines.Add("\nModulation Amplitude: " + (spc.modAmp / 10) + " mT");
            tb.Inlines.Add("\nReceiver Gain: " + spc.gain.ToString("E"));
            tb.Inlines.Add("\nConversion Time: " + spc.convTime + " msec");
            tb.Inlines.Add("\nTimeConversion: " + spc.timeConv + " msec");
            tb.Inlines.Add("\nNumber of Scans: " + spc.scans);
            tb.Inlines.Add("\n\nOperator: " + spc.operator_);
            tb.Inlines.Add("\nDate: " + spc.date);
            tb.Inlines.Add("\nComment: " + spc.comment);
            tb.Inlines.Add("\n\nDouble Integral: " + spc.doubleIntegral().ToString("E"));
            tb.TextWrapping = TextWrapping.Wrap;
            side.Children.Add(tb);

            //create plotview
            EPRPlotView plotView = new EPRPlotView();
            plotView.Padding = new Thickness(5);
            plotView.Width = this.ActualWidth * 0.80;
            plotView.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            spc.plotEPR(plotView);
            if(File.Exists(spc.getFileName() + ".bp2"))
            {
                spc.loadBP2();
            }

            //stack items
            StackPanel sp = new StackPanel();
            sp.Background = Brushes.White;
            sp.Orientation = Orientation.Horizontal;
            sp.Children.Add(plotView);
            sp.Children.Add(side);

            //add all together
            TabItem eprItem = new TabItem();
            eprItem.Header = spc.getTitle();
            eprItem.Content = sp;
            mdiTabMenu.Items.Add(eprItem);
            mdiTabMenu.SelectedItem = eprItem;

            this.RefreshData();
        }

        private void createMDIChild(string filename)
        {
            PlottableEPRSpectrum spc = new PlottableEPRSpectrum(filename);
            this.createMDIChild(spc);
        }

        private void MdiTabMenu_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                foreach (string fileName in files)
                {
                    this.HandleFileLoad(fileName);
                }
            }
        }

        public void HandleFileLoad(string fileName)
        {
            try
            {
                //check if par & spc exists!
                if (Path.GetExtension(fileName) == ".spc" || Path.GetExtension(fileName) == ".par")
                {
                    if (File.Exists(fileName) && File.Exists(Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + ".spc") && (File.Exists(Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + ".par")))
                    {
                        this.createMDIChild(fileName);
                    }
                }
                else if (Path.GetExtension(fileName) == ".bp2")
                {
                    //check if bp2 exists
                    if (File.Exists(Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + ".bp2"))
                    {
                        string json = File.ReadAllText(Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(fileName) + ".bp2");
                        PlottableEPRSpectrum spc_tmp = JsonConvert.DeserializeObject<PlottableEPRSpectrum>(json, new JsonSerializerSettings
                        {
                            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                        });

                        PlottableEPRSpectrum spc = new PlottableEPRSpectrum(spc_tmp.fileName);
                        spc.g = spc_tmp.g;
                        spc.A = spc_tmp.A;
                        createMDIChild(spc);
                        this.RefreshData();
                        if (spc_tmp.showInt)
                        {
                            spc.showInt = true;
                            this.IntButton.Foreground = Brushes.Magenta;
                            spc.plotInt(this.getActivePlotView());
                        }
                        if (spc_tmp.showDeriv)
                        {
                            spc.showDeriv = true;
                            this.DerivButton.Foreground = Brushes.SaddleBrown;
                            spc.plotDeriv(this.getActivePlotView());
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void pm_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            if (e.ChangedButton == OxyMouseButton.Left && this.coupling_isEnabled)
            {
                this.coupling_start = new DataPoint(this.getActivePlotView().Model.Axes[0].InverseTransform(e.Position.X), this.getActivePlotView().Model.Axes[1].InverseTransform(e.Position.Y));
                this.coupling_isMeasure = true;
            }
        }

        public void pm_MouseMove(object sender, OxyMouseEventArgs e)
        {
            if (this.coupling_isEnabled && this.coupling_isMeasure)
            {
                EPRPlotView pw = this.getActivePlotView();
                pw.HideTracker();

                ArrowAnnotation Line = new ArrowAnnotation()
                {
                    StrokeThickness = 1,
                    Color = OxyColors.Black,
                    HeadWidth = 1,
                    StartPoint = coupling_start,
                    EndPoint = new DataPoint(pw.Model.Axes[0].InverseTransform(e.Position.X), coupling_start.Y)
                };
                pw.Model.Annotations.Add(Line);
                pw.Model.InvalidatePlot(false);
                mousePosition = new DataPoint(pw.Model.Axes[0].InverseTransform(e.Position.X), pw.Model.Axes[1].InverseTransform(e.Position.Y));
            }
        }

        public void pm_MouseUp(object sender, OxyMouseEventArgs e)
        {
            if (this.coupling_isEnabled && this.coupling_isMeasure)
            {
                coupling_end = new DataPoint(this.getActivePlotView().Model.Axes[0].InverseTransform(e.Position.X), this.getActivePlotView().Model.Axes[1].InverseTransform(e.Position.Y));
                coupling_isEnabled = false;
                coupling_isMeasure = false;
                this.mdiTabMenu.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 63, 81, 181));
                EPRPlotView pw = this.getActivePlotView();
                this.CouplingButton.Foreground = Brushes.Black;
                pw.Model.Annotations.Clear();
                pw.spc.A.Add(new Tuple<DataPoint, DataPoint>(coupling_start, coupling_end));
                this.RefreshData();
            }
            if (this.g_isPicking)
            {
                this.getActiveSpectrum().g.Add(this.getActiveSpectrum().calcG(this.getActivePlotView().Model.Axes[0].InverseTransform(e.Position.X)));
                this.g_isPicking = false;
                this.mdiTabMenu.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 63, 81, 181));
                this.gButton.Foreground = Brushes.Black;

                //refresh couplings && g
                this.RefreshData();
            }
        }
        public StackPanel getActiveSidebar()
        {
            TabItem ActivePage = (TabItem)mdiTabMenu.SelectedItem;
            StackPanel sp = (StackPanel)ActivePage.Content;
            return (StackPanel)sp.Children[1];
        }

        public void clearSidebar()
        {
            //delete all chips
            StackPanel sidebar = this.getActiveSidebar();
            sidebar.Children.OfType<MaterialDesignThemes.Wpf.Chip>().ToList().ForEach(b => sidebar.Children.Remove(b));
            sidebar.Children.OfType<Label>().ToList().ForEach(b => sidebar.Children.Remove(b));
        }

        private void RefreshData()
        {
            this.clearSidebar();
            StackPanel sidebar = this.getActiveSidebar();
           
            //g
            Label lblg = new Label();
            lblg.FontWeight = System.Windows.FontWeights.Bold;
            lblg.Content = "g-Values";
            sidebar.Children.Add(lblg);

            foreach (double g in this.getActivePlotView().spc.g)
            {
                PlottableEPRSpectrum spc = this.getActiveSpectrum();
                ArrowAnnotation Line = new ArrowAnnotation()
                {
                    StrokeThickness = 1,
                    Color = OxyColors.Black,
                    Text = g.ToString("g6"),
                    StartPoint = new DataPoint(spc.calcB(g), spc.getY().Max() - spc.getY().Max() * 0.18),
                    EndPoint = new DataPoint(spc.calcB(g), spc.getY().Max() - spc.getY().Max() * 0.8),
                };

                MaterialDesignThemes.Wpf.Chip chip = new MaterialDesignThemes.Wpf.Chip();
                chip.IsDeletable = true;
                chip.Content = g.ToString("g6");
                chip.Tag = g;
                chip.DeleteClick += (s, e) => GChip_DeleteClick(s, e);
                sidebar.Children.Add(chip);

                this.getActivePlotView().Model.Annotations.Add(Line);
            }
            
            //couplings
            Label lblA = new Label();
            lblA.FontWeight = System.Windows.FontWeights.Bold;
            lblA.Content = "Hyperfinecouplings";
            sidebar.Children.Add(lblA);

            foreach (Tuple<DataPoint, DataPoint> tp in this.getActivePlotView().spc.A)
            {
                double distance = Math.Abs(tp.Item1.X - tp.Item2.X);
                ArrowAnnotation Line = new ArrowAnnotation()
                {
                    StrokeThickness = 1,
                    Color = OxyColors.Black,
                    Text = distance.ToString("g4") + " mT",
                    StartPoint = tp.Item1,
                    EndPoint = new DataPoint(tp.Item2.X, tp.Item1.Y)
                };
                this.getActivePlotView().Model.Annotations.Add(Line);

                MaterialDesignThemes.Wpf.Chip chip = new MaterialDesignThemes.Wpf.Chip();
                chip.IsDeletable = true;
                chip.Content = distance.ToString("g4") + " mT";
                chip.Tag = tp;
                chip.DeleteClick += (s, e) => AChip_DeleteClick(s, e);
                sidebar.Children.Add(chip);
            }

            

            this.getActivePlotView().Model.InvalidatePlot(false);
        }

        private void AChip_DeleteClick(object sender, RoutedEventArgs e)
        {
            MaterialDesignThemes.Wpf.Chip chip = (MaterialDesignThemes.Wpf.Chip)sender;
            this.getActiveSpectrum().A.Remove((Tuple<DataPoint, DataPoint>)chip.Tag);
            this.getActivePlotView().Model.Annotations.Clear();
            this.RefreshData();
        }

        private void GChip_DeleteClick(object sender, RoutedEventArgs e)
        {
            MaterialDesignThemes.Wpf.Chip chip = (MaterialDesignThemes.Wpf.Chip)sender;
            this.getActiveSpectrum().g.Remove((double)chip.Tag);
            this.getActivePlotView().Model.Annotations.Clear();
            this.RefreshData();
        }

        private PlottableEPRSpectrum getActiveSpectrum()
        {
            EPRPlotView plotView = this.getActivePlotView();
            return plotView.spc;
        }
        private EPRPlotView getActivePlotView()
        {
            if (mdiTabMenu.SelectedItem == null) mdiTabMenu.SelectedItem = mdiTabMenu.Items[0];
            TabItem ActivePage = (TabItem)mdiTabMenu.SelectedItem;
            StackPanel sp = (StackPanel)ActivePage.Content;
            return (EPRPlotView)sp.Children[0];
        }

        private void SimButton_Click(object sender, RoutedEventArgs e)
        {
            SimWindow sw = new SimWindow(this.getActivePlotView());
            sw.Show();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            string initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (Properties.Settings.Default.load_path != "") initialDir = Properties.Settings.Default.load_path;
            OpenFileDialog ofd = new OpenFileDialog
            {
                InitialDirectory = initialDir,
                Filter = "Bruker EMX Files (*.par)|*.par|BluEPRint Files (*.bp2)|*.bp2",
                RestoreDirectory = true,
                Multiselect = true
            };
            var DialogResult = ofd.ShowDialog();

            if (DialogResult.HasValue && DialogResult.Value)
            {
                foreach (string fileName in ofd.FileNames)
                {
                    this.HandleFileLoad(fileName);
                }
            }
        }

        private void MdiTabMenu_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effects = DragDropEffects.Copy;
        }

        private void MdiTabMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(mdiTabMenu.Items.Count != 0)
            {
                if (Properties.Settings.Default.es_plug != "" && Properties.Settings.Default.es_path != "") this.SimButton.IsEnabled = true;
                this.CouplingButton.IsEnabled = true;
                this.gButton.IsEnabled = true;
                this.sessionSaveButton.IsEnabled = true;
                this.IntButton.IsEnabled = true;
                this.DerivButton.IsEnabled = true;
                this.SaveButton.IsEnabled = true;
                this.ExAsciiBtn.IsEnabled = true;
                this.ExPdfBtn.IsEnabled = true;
                this.ExPngBtn.IsEnabled = true;

                //show state
                if (this.getActiveSpectrum().showInt)
                {
                    this.IntButton.Foreground = Brushes.Magenta;
                }
                else
                {
                    this.IntButton.Foreground = Brushes.Black;
                }
                //show state
                if (this.getActiveSpectrum().showDeriv)
                {
                    this.DerivButton.Foreground = Brushes.SaddleBrown;
                }
                else
                {
                    this.DerivButton.Foreground = Brushes.Black;
                }
            }
            else
            {
                this.SimButton.IsEnabled = false;
                this.CouplingButton.IsEnabled = false;
                this.gButton.IsEnabled = false;
                this.sessionSaveButton.IsEnabled = false;
                this.IntButton.IsEnabled = false;
                this.DerivButton.IsEnabled = false;
                this.SaveButton.IsEnabled = false;
                this.ExAsciiBtn.IsEnabled = false;
                this.ExPdfBtn.IsEnabled = false;
                this.ExPngBtn.IsEnabled = false;
            }
        }

        private void CouplingButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.coupling_isEnabled)
            {
                //exit g mode
                if (g_isPicking)
                {
                    g_isPicking = false;
                    this.gButton.Foreground = Brushes.Black;
                }

                this.coupling_isEnabled = true;
                this.mdiTabMenu.BorderBrush = Brushes.OrangeRed;
                this.CouplingButton.Foreground = Brushes.OrangeRed;
                this.getActivePlotView().Model.Annotations.Clear();
                this.getActivePlotView().Model.InvalidatePlot(false);
            }
            else
            {
                this.coupling_isEnabled = false;
                this.coupling_isMeasure = false;
                this.mdiTabMenu.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 63, 81, 181));
                EPRPlotView pw = this.getActivePlotView();
                this.CouplingButton.Foreground = Brushes.Black;
                pw.Model.Annotations.Clear();

                //refresh couplings && g
                this.RefreshData();
            }
        }

        private void GButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.g_isPicking)
            {
                //exit coupling mode
                if (coupling_isEnabled)
                {
                    coupling_isEnabled = false;
                    coupling_isMeasure = false;
                    this.CouplingButton.Foreground = Brushes.Black;
                }

                this.g_isPicking = true;
                this.gButton.Foreground = Brushes.LimeGreen;
                this.mdiTabMenu.BorderBrush = Brushes.LimeGreen;
            }
            else
            {
                this.g_isPicking = false;
                this.mdiTabMenu.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 63, 81, 181));
                this.gButton.Foreground = Brushes.Black;
                
                //refresh couplings && g
                this.RefreshData();
            }
        }

        private void SessionSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (mdiTabMenu.Items.Count != 0)
            {
                //save session-File
                List<PlottableEPRSpectrum> EPRSpectra = this.getAllSpectra();
                string filename = AppDomain.CurrentDomain.BaseDirectory + "last.session";
                List<string> sessionFiles = new List<string>();
                foreach (PlottableEPRSpectrum spc in EPRSpectra)
                {
                    //save bp2
                    FileHandler.HandleSave(spc, FileHandler.Format.bp2);
                    sessionFiles.Add(spc.fileName + ".bp2");
                }
                using (StreamWriter sw = File.CreateText(filename))
                {
                    foreach (string s in sessionFiles)
                    {
                        sw.WriteLine(s);
                    }
                }

            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //resize all
            foreach (EPRPlotView plotView in BPUtil.FindVisualChildren<EPRPlotView>((DependencyObject)this))
            {
                plotView.Width = 0.80 * this.ActualWidth;
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow();
            sw.Show();
        }

        private void IntButton_Click(object sender, RoutedEventArgs e)
        {
            EPRPlotView pw = this.getActivePlotView();
            if (!this.getActiveSpectrum().showInt)
            {
                this.getActiveSpectrum().showInt = true;
                this.IntButton.Foreground = Brushes.Magenta;
                this.getActiveSpectrum().plotInt(pw);
            }
            else
            {
                var test = pw.Model.Series.Where(s => s.Title.Contains("Integral"));
                if (test.Count() != 0)
                {
                    pw.Model.Series.Remove(test.First());
                }
                this.getActiveSpectrum().showInt = false;
                pw.InvalidatePlot();
                pw.rescale();
                this.IntButton.Foreground = Brushes.Black;
            }
        }

        private void DerivButton_Click(object sender, RoutedEventArgs e)
        {
            EPRPlotView pw = this.getActivePlotView();
            if (!this.getActiveSpectrum().showDeriv)
            {
                this.getActiveSpectrum().showDeriv = true;
                this.DerivButton.Foreground = Brushes.SaddleBrown;
                this.getActiveSpectrum().plotDeriv(pw);
            }
            else
            {
                var test = pw.Model.Series.Where(s => s.Title.Contains("Derivative"));
                if (test.Count() != 0)
                {
                    pw.Model.Series.Remove(test.First());
                }
                this.getActiveSpectrum().showDeriv = false;
                pw.InvalidatePlot();
                pw.rescale();
                this.DerivButton.Foreground = Brushes.Black;
            }
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            InfoWindow iw = new InfoWindow();
            iw.Show();
        }

        public List<PlottableEPRSpectrum> getAllSpectra()
        {
            List<PlottableEPRSpectrum> EPRSpectra = new List<PlottableEPRSpectrum>();

            if (mdiTabMenu.Items.Count != 0)
            {
                foreach (TabItem tab in mdiTabMenu.Items)
                {
                    StackPanel sp = (StackPanel)tab.Content;
                    EPRPlotView plotView = (EPRPlotView)sp.Children[0];
                    PlottableEPRSpectrum spc = plotView.spc;
                    EPRSpectra.Add(spc);
                }
            }
            return EPRSpectra;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            List<PlottableEPRSpectrum> EPRSpectra = this.getAllSpectra();
            SaveWindow sw = new SaveWindow(EPRSpectra);
            sw.Show();
        }

        private void ExAsciiBtn_Click(object sender, RoutedEventArgs e)
        {
            FileHandler.HandleSave(this.getActiveSpectrum(), FileHandler.Format.dat);
        }

        private void ExPngBtn_Click(object sender, RoutedEventArgs e)
        {
            FileHandler.HandleSave(this.getActiveSpectrum(), FileHandler.Format.png);
        }

        private void ExPdfBtn_Click(object sender, RoutedEventArgs e)
        {
            FileHandler.HandleSave(this.getActiveSpectrum(), FileHandler.Format.pdf);
        }
    }
}
