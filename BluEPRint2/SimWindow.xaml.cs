using BluEPRint2.EasySpin;
using BluEPRint2.EasySpin.Types;
using BluEPRint2.Plot;
using BluEPRint2.Spectrum;
using BluEPRint2.Utility;
using Newtonsoft.Json;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BluEPRint2
{
    /// <summary>
    /// Interaktionslogik für SimWindow.xaml
    /// </summary>
    public partial class SimWindow : Window
    {
        public EPRPlotView plotView;
        public Exp exp;
        public EPRSpectrum spc;

        public SimWindow(EPRPlotView plotView)
        {
            InitializeComponent();
            //garlic mode is default
            spiceImg.Source = BluEPRint2.Properties.Resources.garlic.ToBitmapImage();

            this.plotView = plotView;
            this.spc = plotView.spc;
            //setup exp params
            this.exp = new Exp(spc.freq, spc.getX().Length, spc.getX().First(), spc.getX().Last());

            Load();
            this.ExpTBlock.Text = "Experimental Data: \nFrequency: " + exp.frequency + " GHz \nRange: " + spc.getX().First() + " - "
                + spc.getX().Last() + " mT\nResolution: " + spc.getX().Length + " Points";
        }

        private void Load()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            if (File.Exists(spc.fileName + ".sim"))
            {
                string json = File.ReadAllText(this.spc.fileName + ".sim");
                Sys sys = JsonConvert.DeserializeObject<Sys>(json, new JsonSerializerSettings
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                });

                if (sys.Spin == 0.5) RB_S0_5.IsChecked = true;
                if (sys.Spin == 1) RB_S1.IsChecked = true;
                if (sys.Spin == 1.5) RB_S1_5.IsChecked = true;
                if (sys.Spin == 2) RB_S2.IsChecked = true;
                if (sys.Spin == 2.5) RB_S2_5.IsChecked = true;
                if (sys.Spin > 2.5) { RB_SOther.IsChecked = true; TB_SOther.Text = sys.Spin.ToString(nfi); }

                if (sys.g.Length != 0) TB_gVal.Text = String.Join(",", sys.g.Select(x => x.ToString(nfi)).ToArray());
                if (sys.A != null && sys.A.Length != 0) TB_HFCoup.Text = String.Join(",", sys.A.Select(x => x.ToString(nfi)).ToArray());
                else TB_HFCoup.Text = "";
                if (sys.D != null && sys.D.Length != 0) TB_ZFS.Text = String.Join(",", sys.D.Select(x => x.ToString(nfi)).ToArray());
                else TB_ZFS.Text = "";
                if (sys.additional != null&& sys.additional.Length != 0)
                {
                    TB_Additional.Text = sys.additional;
                    ToggleAdditional.IsChecked = true;
                }
                if (sys.gStrain != null && sys.gStrain.Length != 0) TB_lw.Text = String.Join(",", sys.gStrain.Select(x => x.ToString(nfi)).ToArray());
                if (sys.lw != 0) TB_lw.Text = sys.lw.ToString(nfi);
                if (sys.Nucs != null && sys.Nucs.Length != 0) TB_Nucs.Text = String.Join(",", sys.Nucs);
                else TB_Nucs.Text = "";
                if (sys.n != null && sys.n.Length != 0) TB_NucsN.Text = String.Join(",", sys.n);
                else TB_NucsN.Text = "";
            }
        }

        private double getSpin()
        {
            double Spin = 0.5;
            if (RB_S0_5.IsChecked == true) Spin = 0.5;
            if (RB_S1.IsChecked == true) Spin = 1;
            if (RB_S1_5.IsChecked == true) Spin = 1.5;
            if (RB_S2.IsChecked == true) Spin = 2;
            if (RB_S2_5.IsChecked == true) Spin = 2.5;
            if (RB_SOther.IsChecked == true) Spin = Convert.ToDouble(TB_SOther.Text);
            return Spin;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;            
            btn.IsEnabled = false;

            //remove existing sim
            var test = plotView.Model.Series.Where(s => s.Title.Contains("Simulation"));
            if (test.Count() != 0)
            {
                plotView.Model.Series.Remove(test.First());
            }

            this.ExpTBlock.Text = "Experimental Data: \nFrequency: " + exp.frequency + " GHz \nRange: " + this.spc.getX().First() + " - "
                + this.spc.getX().Last() + " mT\nResolution: " + this.spc.getX().Length 
                + " Points\nTemperature: " + this.exp.Temperature + " K";
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            double Spin = getSpin();
            double[] g = Array.ConvertAll(TB_gVal.Text.Split(','), i => Convert.ToDouble(i, CultureInfo.InvariantCulture));
            double[] A = new double[] { };
            if (TB_HFCoup.Text.Length != 0)
            {
                A = Array.ConvertAll(TB_HFCoup.Text.Split(','), i => Convert.ToDouble(i, CultureInfo.InvariantCulture));
            }
            double[] D = new double[] { };
            if (TB_ZFS.Text.Length != 0)
            {
                D = Array.ConvertAll(TB_ZFS.Text.Split(','), i => Convert.ToDouble(i, CultureInfo.InvariantCulture));
                
            }
            string[] Nucs = new string[] { };
            if (TB_Nucs.Text.Length != 0)
            {
                Nucs = TB_Nucs.Text.Split(',');
            }
            int[] N = new int[] { };
            if (TB_NucsN.Text.Length != 0)
            {
                N = Array.ConvertAll(TB_NucsN.Text.Split(','), i => Convert.ToInt32(i, CultureInfo.InvariantCulture));
            }

            this.ExpTBlock.Text += "\n\nSimulation Parameters:\nSpin: " + Spin + "\ng-Values: " + String.Join(",", g.Select(p => p.ToString(nfi)).ToArray());
            this.ExpTBlock.Text += "\nNucs: " + String.Join(",", Nucs.Select(p => p.ToString(nfi)).ToArray());
            this.ExpTBlock.Text += "\nA: " + String.Join(",", A.Select(p => p.ToString(nfi)).ToArray());
            double lw = 0;
            double[] gStrain = new double[] { };
            if (Spin <= 0.5)
            {
                lw = Convert.ToDouble(TB_lw.Text, nfi);
                this.ExpTBlock.Text += "\nn: " + TB_NucsN.Text + "\nlw: " + lw;
            }
            else
            {
                if (TB_lw.Text.Length != 0)
                {
                    this.ExpTBlock.Text += "\nD: " + String.Join(",", D.Select(p => p.ToString(nfi)).ToArray());
                    gStrain = Array.ConvertAll(TB_lw.Text.Split(','), i => Convert.ToDouble(i, CultureInfo.InvariantCulture));
                    this.ExpTBlock.Text += "\ngStrain: " + String.Join(",", gStrain.Select(p => p.ToString(nfi)).ToArray());
                }
            }
            string additional = "";
            if(ToggleAdditional.IsChecked == true)
            {
                additional = TB_Additional.Text;
                this.ExpTBlock.Text += "\nAddtional Parameters: \n" + additional;
            }
            Sys sys;
            //garlic mode
            if (Spin <= 0.5)
            {
                sys = new Sys(Spin, g, A, Nucs, N, lw, additional);
                Task.Factory.StartNew(() =>
                {
                    double[] y = AddSim(sys, this.exp, spc);
                    Dispatcher.Invoke(() =>
                        {
                           
                            LineSeries se = new LineSeries
                            {
                                ItemsSource = BPUtil.getDataPoints(spc.getX(), y),
                                Title = "Simulation"
                            };


                            plotView.Model.Series.Add(se);
                            plotView.InvalidatePlot();
                            plotView.rescale();

                            btn.IsEnabled = true;
                        });
                });
                this.ExpTBlock.Text += "\nMode is garlic";
            }

            //pepper mode
            else
            {
                sys = new Sys(Spin, g, D, A, gStrain, Nucs, additional);
                Task.Factory.StartNew(() =>
                {
                    double[] y = AddSim(sys, this.exp, spc);
                    Dispatcher.Invoke(() =>
                    {

                        LineSeries se = new LineSeries
                        {
                            ItemsSource = BPUtil.getDataPoints(spc.getX(), y),
                            Title = "Simulation"
                        };


                        plotView.Model.Series.Add(se);
                        plotView.InvalidatePlot();
                        plotView.rescale();

                        btn.IsEnabled = true;
                    });
                });
                this.ExpTBlock.Text += "\nMode is pepper";
            }

            //save sim-File
            string filename = spc.fileName + ".sim";
            using (StreamWriter sw = File.CreateText(filename))
            {
                JsonSerializer json = new JsonSerializer();
                json.Serialize(sw, sys);
                this.ExpTBlock.Text += "\n\nParameters saved to " + filename;
            }

        }

        public double[] AddSim(Sys sys, Exp exp, EPRSpectrum spc)
        {
            EasySpinHelper matlab = new EasySpinHelper(Properties.Settings.Default.es_plug);
            matlab.SetPath(Properties.Settings.Default.es_path);
            matlab.Execute("clc, clear");

            matlab.Put("y", spc.getY());
            if (sys.Spin > 0.5)
            {
                matlab.pepper(sys, exp);
            }
            else
            {
                matlab.garlic(sys, exp);
            }
            matlab.Execute("spc = rescale(spc,y,'lsq0');");
            matlab.Execute("spc = spc - max(y)/2");
            object data = matlab.Get("spc");
            double[,] datax = (double[,])data;
            double[] y = BPUtil.singleCol(datax);
            return y;
        }

        private void RB_S_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsInitialized)
            {
                if (this.RB_SOther.IsChecked == true) this.TB_SOther.IsEnabled = true;
                else this.TB_SOther.IsEnabled = false;

                if (getSpin() > 0.5)
                {
                    this.ToggleLW.IsChecked = true;
                    this.TB_ZFS.IsEnabled = true;
                    this.TB_NucsN.IsEnabled = false;
                    spiceImg.Source = BluEPRint2.Properties.Resources.chili.ToBitmapImage();
                }
                else
                {
                    this.ToggleLW.IsChecked = false;
                    this.TB_ZFS.IsEnabled = false;
                    this.TB_NucsN.IsEnabled = true;
                    spiceImg.Source = BluEPRint2.Properties.Resources.garlic.ToBitmapImage();
                }
            }
        }

        private void TB_Temp_TextChanged(object sender, TextChangedEventArgs e)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            if (this.IsInitialized)
            {
                if (TB_Temp.Text != "") { try { this.exp.Temperature = Convert.ToDouble(TB_Temp.Text, nfi); } catch (Exception ex) { } }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
