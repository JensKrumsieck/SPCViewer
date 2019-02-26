using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;

namespace BluEPRint2.Utility
{
    static class BPUtil
    {
        public static List<EPRDataPoint> getDataPoints(double[] x, double[] y)
        {
            List<EPRDataPoint> data = new List<EPRDataPoint>();
            for (int i = 0; i <= x.Length - 1; i++)
            {
                data.Add(new EPRDataPoint(x[i], y[i]));
            }
            return data;
        }

        public static double[] singleCol(double[,] inp)
        {
            double[] res = new double[inp.Length];
            for (int i = 0; i <= inp.Length - 1; i++)
            {
                res[i] = inp[0, i];
            }
            return res;
        }

        public static BitmapImage ToBitmapImage(this System.Drawing.Image bitmap)
        {
            BitmapImage bitmapImage = null;
            if (bitmap != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    bitmapImage = new BitmapImage();
                    bitmap.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                }
            }
            return bitmapImage;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
