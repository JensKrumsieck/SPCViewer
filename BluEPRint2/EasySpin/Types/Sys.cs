using System.Text;
using System.Diagnostics;
using System.Globalization;

namespace BluEPRint2.EasySpin.Types
{
    public class Sys
    {
        public double Spin;
        public double[] g;
        public double lw;
        public double[] A;
        public double[] D;
        public double[] gStrain;
        public string additional;
        public string[] Nucs;
        public int[] n;

        private Sys(){}

        //constructor for garlic use
        public Sys(double S, double[] g, double[] A,  string[] Nucs, int[] n, double lw, string additional)
        {
            this.Spin = S;
            this.g = g;
            this.lw = lw;
            this.A = A;
            this.Nucs = Nucs;
            this.n = n;
            this.additional = additional;
        }
        //constructur for pepper use (TODO)
        public Sys(double S, double[] g, double[] D, double[] A, double[] gStrain, string[] Nucs, string additional)
        {
            this.Spin = S;
            this.g = g;
            this.A = A;
            this.D = D;
            this.Nucs = Nucs;
            this.gStrain = gStrain;
            this.additional = additional;
        }

        //setup sys
        public void Setup(EasySpinHelper matlab)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            var sys = new StringBuilder();
            sys.Append("Sys = struct(");
            //no check for S
            sys.Append("'S', " + this.Spin.ToString(nfi) + ",");

            //g block start
            sys.Append("'g', [");
            for(int i = 0; i<= this.g.Length - 1; i++)
            {
                if(this.g[i] != 0)
                {
                    sys.Append(g[i].ToString(nfi));
                    if(i < this.g.Length - 1)
                    {
                        sys.Append(',');
                    }
                }
            }
            sys.Append("],");
            //g block end

            if(this.lw != 0) sys.Append("'lw'," + this.lw.ToString(nfi) + ",");
            
            //A block start
            if(this.A != null && this.A.Length > 0)
            {
                sys.Append("'A', [");
                for (int i = 0; i <= this.A.Length - 1; i++)
                {
                    sys.Append(this.A[i].ToString(nfi));
                    if (i < this.A.Length - 1)
                    {
                        sys.Append(',');
                    }
                }
                sys.Append("],");
            }

            //block nucs start
            if (this.Nucs != null && this.Nucs.Length > 0)
            {
                sys.Append("'Nucs', '");
                for (int i = 0; i <= this.Nucs.Length - 1; i++)
                {
                    sys.Append(this.Nucs[i]);
                    if (i < this.Nucs.Length - 1)
                    {
                        sys.Append(',');
                    }
                }
                sys.Append("',");
            }
            //blicks nucs end

            if (this.n != null && this.n.Length > 0)
            {
                sys.Append("'n', [");
                for (int i = 0; i <= this.n.Length - 1; i++)
                {
                    sys.Append(this.n[i]);
                    if (i < this.Nucs.Length - 1)
                    {
                        sys.Append(',');
                    }
                }
                sys.Append("],");
            }

            if(this.D != null && this.D.Length > 0)
            {
                sys.Append("'D', [");
                for (int i = 0; i <= this.D.Length - 1; i++)
                {
                    sys.Append(this.D[i].ToString(nfi));
                    if (i < this.D.Length - 1)
                    {
                        sys.Append(',');
                    }
                }
                sys.Append("],");
            }
            if (this.gStrain != null && this.gStrain.Length > 0)
            {
                sys.Append("'gStrain', [");
                for (int i = 0; i <= this.gStrain.Length - 1; i++)
                {
                    sys.Append(this.gStrain[i].ToString(nfi));
                    if (i < this.gStrain.Length - 1)
                    {
                        sys.Append(',');
                    }
                }
                sys.Append("],");
            }
            if(this.additional.Length != 0)
            {
                sys.Append(this.additional);
                sys.Append(",");
            }

            string _sys = "";
            //remove last comma
            var index = sys.ToString().LastIndexOf(',');
            if (index > 0)
            {
                _sys = sys.ToString().Substring(0, index);
            }

            sys = new StringBuilder();
            sys.Append(_sys);
            sys.Append(");");

            Debug.WriteLine(sys.ToString());
            //send to matlab
            matlab.Execute(sys.ToString());
        }
    }
}
