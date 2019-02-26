using System.Text;
using System.Globalization;
using System.Diagnostics;

namespace BluEPRint2.EasySpin.Types
{
    public class Exp
    {
        public double frequency;
        public double resolution;
        public double Xmin;
        public double Xmax;
        public double Temperature;
        public double ModAmp;
        public int Harmonic;

        public Exp(double freq, double res, double min, double max, double Temp = 298, double modamp = 0, int Harmonic = 1)
        {
            this.frequency = freq;
            this.resolution = res;
            this.Xmin = min;
            this.Xmax = max;
            this.ModAmp = modamp;
            this.Temperature = Temp;
            this.Harmonic = Harmonic;
        }

        public void Setup(EasySpinHelper matlab)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            var exp = new StringBuilder();
            exp.Append("Exp  = struct(");
            exp.Append("'mwFreq'," + this.frequency.ToString(nfi) + ",");
            exp.Append("'nPoints'," + this.resolution + ",");
            exp.Append("'Range', [" + this.Xmin.ToString(nfi) + " " + this.Xmax.ToString(nfi) + "],");
            exp.Append("'Temperature'," + this.Temperature.ToString(nfi) + ",");
            exp.Append("'Harmonic'," + this.Harmonic + ");");

            Debug.WriteLine(exp.ToString());
            matlab.Execute(exp.ToString());
        }
    }
}
