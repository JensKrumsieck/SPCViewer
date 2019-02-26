using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace BluEPRint2.Spectrum
{
    public class EPRSpectrum : AbstractSpectrum
    {
        private double[] gAxis;
        public double freq = 9.5; //default X-Band freq
        public double modAmp = 0; //modulation amplitude
        public double power = 0; //microwave power
        public double convTime = 0; //conversion time
        public double timeConv = 0; //time conversion
        public double gain = 0; //receiver gain
        public double scans = 1; //no of scans
        public string unit = "G"; //unit of x axis (Gauss)
        public string timeStamp = "";
        public string operator_ = "";
        public string comment = "";
        public string date = "";

        //constructor with filename
        public EPRSpectrum(string file)
            : base() {
            this.fileName = Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file);
            this.Load();
        }
        protected EPRSpectrum() { }

        //load from file
        private void Load()
        {
            //get y Data
            double[] yData;
            using (BinaryReader br = new BinaryReader(File.OpenRead(this.fileName + ".spc")))
            {
                yData = new double[br.BaseStream.Length / 4];
                br.BaseStream.Position = 0;
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    float cur;
                    if ((cur = br.ReadSingle()) != 0)
                    {
                        yData[(br.BaseStream.Position / 4) - 1] = cur;
                    }
                }
            }

            //getting params and save as dictionary first
            Dictionary<string, string> Params = new Dictionary<string, string>();
            using (StreamReader sr = new StreamReader(fileName + ".par"))
            {
                string buf;
                while ((buf = sr.ReadLine()) != null)
                {
                    string[] lines = buf.Split('\n');

                    foreach (string line in lines)
                    {
                        try
                        {
                            Params.Add(line.Substring(0, 3).Trim(), line.Substring(3).Trim());
                        }
                        catch (Exception e) { }
                    }
                }
            }

            //setting up params
            this.freq = Convert.ToDouble(Params.ContainsKey("MF") ? Params["MF"] : "9.5", CultureInfo.InvariantCulture);
            this.modAmp = Convert.ToDouble(Params.ContainsKey("RMA") ? Params["RMA"] : "0", CultureInfo.InvariantCulture);
            this.convTime = Convert.ToDouble(Params.ContainsKey("RCT") ? Params["RCT"] : "0", CultureInfo.InvariantCulture);
            this.timeConv = Convert.ToDouble(Params.ContainsKey("RTC") ? Params["RTC"] : "0", CultureInfo.InvariantCulture);
            this.power = Convert.ToDouble(Params.ContainsKey("MP") ? Params["MP"] : "0", CultureInfo.InvariantCulture);
            this.gain = Convert.ToDouble(Params.ContainsKey("RRG") ? Params["RRG"] : "0", CultureInfo.InvariantCulture);
            this.scans = Convert.ToDouble(Params.ContainsKey("JNS") ? Params["JNS"] : "1", CultureInfo.InvariantCulture);
            this.timeStamp = (Params.ContainsKey("JDA") ? Params["JDA"] : "-") + " " + (Params.ContainsKey("JTM") ? Params["JTM"] : "-");
            this.operator_ = Params.ContainsKey("JON") ? Params["JON"] : "-";
            this.comment = Params.ContainsKey("JCO") ? Params["JCO"] : "-";
            this.unit = Params.ContainsKey("JUN") ? Params["JUN"] : "G";
            this.date = Params.ContainsKey("JDA") ? Params["JDA"] : "-";

            //build x-axis
            //center field
            double hcf = Convert.ToDouble(Params.ContainsKey("HCF") ? Params["HCF"] : "0", CultureInfo.InvariantCulture);
            //sweep width
            double hsw = Convert.ToDouble(Params.ContainsKey("HSW") ? Params["HSW"] : "0", CultureInfo.InvariantCulture);
            //resolution
            int res = Convert.ToInt32(Params.ContainsKey("RES") ? Params["RES"] : "0", CultureInfo.InvariantCulture);
            double min = hcf - (hsw / 2);
            double d = hsw / (res - 1);
            double[]xData = new double[res];
            for (int i = 0; i <= res - 1; i++)
            {
                xData[i] = min + (d * i);
            }
            loadData(xData, yData);
        }

        //calculate g scale and return
        public double[] makeGAxis(double[] data)
        {
            double[] gData = new double[data.Length];
            for (int i = 0; i <= data.Length - 1; i++)
            {
                gData[i] = calculateG(data[i], this.freq);
            }
            return gData;
        }
        public static double calculateG(double value, double freq, string unit = "G")
        {
            switch (unit)
            {
                case "G":
                    value = value / 10000; //G to T
                    break;
                case "mT":
                    value = value / 1000; //mT to T
                    break;
                case "T":
                    //do nothing
                    break; ;
                default:
                    value = value / 10000; //G to T
                    break;
            }
            return Constants.planckh * freq * 1e9 / (Constants.bohrm * value);
        }
        public double calcG(double value)
        {
            return calculateG(value, this.freq, this.unit);
        }

        public double calculateB(double value, double freq, string unit)
        {
            value = Constants.planckh * freq * 1e9 / (Constants.bohrm * value);
            switch (unit)
            {
                case "G":
                    value = value * 10000; //G to T
                    break;
                case "mT":
                    value = value * 1000; //mT to T
                    break;
                case "T":
                    //do nothing
                    break; ;
                default:
                    value = value * 10000; //G to T
                    break;
            }
            return value;
        }

        public double calcB(double value)
        {
            return calculateB(value, this.freq, this.unit);
        }

        public double[] getG()
        {
            if (this.gAxis == null || this.gAxis.Length != this.getX().Length || this.gAxis.Length == 0)
            {
                this.gAxis = makeGAxis(this.getX());
            }
            return this.gAxis;
        }

        public double doubleIntegral()
        {
            double sum = 0;
            for (int i = 0; i <= this.getX().Length - 1; i++)
            {
                sum += getIntegral()[i];
            }
            return sum;
        }

        public string getTitle()
        {
            return Path.GetFileNameWithoutExtension(this.fileName);
        }

        //formatter for oxyplot axis
        public string _formatter(double d)
        {
            return calculateG(d, this.freq).ToString("G6");
        }

        public void convertToMT() //convert magnetic field to mT
        {
            double value = 1;
            switch (unit)
            {
                case "G":
                    value = value / 10; //G to mT
                    break;
                case "mT":
                    value = value / 1; //mT to mT
                    break;
                case "T":
                    value = value * 1000; //T to T
                    break; ;
                default:
                    value = value / 10000; //G to T
                    break;
            }
            for (int i = 0; i <= xAxis.Length -1; i++)
            {
                xAxis[i] = xAxis[i] * value;
            }
            this.unit = "mT";
        }
    }
}
