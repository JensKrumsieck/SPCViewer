using BluEPRint2.Spectrum;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using OxyPlot;
using BluEPRint2.Plot;
using OxyPlot.Wpf;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;

namespace BluEPRint2.Utility
{
    public static class FileHandler
    {
        //supported file formats
        public enum Format
        {
            bp2,
            dat,
            png,
            pdf
        };

        public static void HandleSave(PlottableEPRSpectrum spc, Format format)
        {
            switch (format)
            {
                case Format.bp2:
                    HandleSaveBP2(spc);
                    break;
                case Format.dat:
                    HandleSaveDAT(spc);
                    break;
                case Format.png:
                    HandleSavePNG(spc);
                    break;
                case Format.pdf:
                    HandleSavePDF(spc);
                    break;
            }
        }

        //Handle File Save as bp2 (BluEPRint 2 File)
        private static void HandleSaveBP2(PlottableEPRSpectrum spc)
        {
            string spc_filename = spc.fileName + ".bp2";
            using (StreamWriter sw = File.CreateText(spc_filename))
            {
                JsonSerializer json = new JsonSerializer();
                json.Serialize(sw, spc);
            }
        }
        
        //Handle File Save as DAT
        private static void HandleSaveDAT(PlottableEPRSpectrum spc)
        {
            using (StreamWriter sw = new StreamWriter(spc.fileName + ".dat"))
            {
                sw.WriteLine("B[" + spc.unit + "];g;Spectrum;Absorption(Integral);Derivative of Signal");
                //X,G,Spectrum,Absorption,Deriv
                for (int i = 0; i <= spc.getY().Length - 1; i++)
                {
                    sw.WriteLine(spc.getX()[i] + ";" + spc.getG()[i] + ";" + spc.getY()[i] + ";" + spc.getIntegral()[i] + ";" + spc.getDerivative()[i]);
                }
            }
        }

        //save as PNG
        private static void HandleSavePNG(PlottableEPRSpectrum spc)
        {
            // get EPR Plotview control
            EPRPlotView epr = new EPRPlotView();

            ItemCollection Tabs = Application.Current.Windows.OfType<MainWindow>().First().mdiTabMenu.Items;
            foreach(TabItem tab in Tabs)
            {
                StackPanel sp = (StackPanel)tab.Content;
                EPRPlotView temp_epr = (EPRPlotView)sp.Children[0];
                if (temp_epr.spc == spc)
                {
                    epr = temp_epr;
                }
            }
            //export full hd
            PngExporter.Export(epr.Model, epr.spc.fileName + ".png", 1920, 1080, OxyColors.Transparent, 300);
        }

        //save as PDF
        private static void HandleSavePDF(PlottableEPRSpectrum spc)
        {
            // get EPR Plotview control
            EPRPlotView epr = new EPRPlotView();

            ItemCollection Tabs = Application.Current.Windows.OfType<MainWindow>().First().mdiTabMenu.Items;
            foreach (TabItem tab in Tabs)
            {
                StackPanel sp = (StackPanel)tab.Content;
                EPRPlotView temp_epr = (EPRPlotView)sp.Children[0];
                if (temp_epr.spc == spc)
                {
                    epr = temp_epr;
                }
            }

            //export pdf
            using (Stream stream = File.Create(spc.fileName + ".pdf"))
            {
                PdfExporter.Export(epr.Model, stream, 600, 400);
            }
        }
    }
}
